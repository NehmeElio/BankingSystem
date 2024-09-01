# Bank Management System

## Table of Contents
1. [Docker Setup](#docker-setup)
2. [Overview](#overview)
3. [General Considerations](#general-considerations)
4. [AccountMicroService](#accountmicroservice)
5. [TransactionMicroservice](#transactionmicroservice)
6. [Technical Details](#technical-details)
7. [Database Structure](#database-structure)
8. [Security Considerations](#security-considerations)
9. [Performance Optimizations](#performance-optimizations)
10. [Future Enhancements](#future-enhancements)

## Docker Setup

To set up the required Docker containers for this project, follow these steps:

### 1. Keycloak Container

A pre-configured Keycloak container is available as a .tar file.

1. Download `keycloak_with_data.tar` from [this Google Drive link](https://drive.google.com/drive/folders/14sRsK8KgQDFwShbP3KEMG2H78Zphp_db?usp=sharing).
2. Run the following commands:

   ```bash
   cd <your-file-path>
   docker load -i keycloak_with_data.tar
   docker run -d -p 8080:8080 keycloak_with_data
   ```

3. Access the Keycloak console at `http://localhost:8080` with credentials:
   - Username: admin
   - Password: admin
4. Use the "BankRealm" for this project.

### 2. PostgreSQL Container

1. Download and extract the `bank-db` folder from the provided Google Drive link.
2. Run the following command, replacing `<your-file-path>` with the path to the extracted `bank-db` folder:

   ```bash
   docker run -d --name postgres_container -e POSTGRES_PASSWORD=123 -v <your-file-path>:/var/lib/postgresql/data -p 5434:5432 postgres
   ```

### 3. RabbitMQ Container

1. Download and extract the `rabbitmq-data` folder from the provided Google Drive link.
2. Run the following command, replacing `<your-file-path>` with the path to the extracted `rabbitmq-data` folder:

   ```bash
   docker run -d --name rabbitmq-container -p 5672:5672 -p 15672:15672 -v <your-file-path>:/var/lib/rabbitmq rabbitmq:3-management
   ```

After running these commands, you should have three containers running with the pre-configured data:
1. Keycloak on port 8080
2. PostgreSQL on port 5434
3. RabbitMQ on ports 5672 and 15672 (management interface)

## Overview
This project implements a comprehensive Bank Management System consisting of two microservices:
1. AccountMicroService: Manages account creation, branch management, and employee onboarding.
2. TransactionMicroservice: Handles all transaction-related operations, including deposits, withdrawals, and recurrent transactions.

Each microservice has its own dedicated database:
- AccountMicroService: bankdb_Account
- TransactionMicroservice: bankdb

## General Considerations
- **Simplified Authentication**: All passwords are set to '123' for development purposes. This should be changed in a production environment.
- **Exception Handling**: A GlobalExceptionMiddleware with custom exceptions (defined in SharedLibraries) manages error handling across the system.
- **Multitenancy**: Implemented using an Action Filter, which utilizes Keycloak user roles and branch information to determine the appropriate database schema and access levels.
- **External Libraries**: The project leverages several libraries from nuget.org, including:
  - Elio.Logging: For comprehensive logging capabilities
  - Elio.HealthCheck: To monitor the health of the microservices
  - Elio.Caching: For efficient data caching
- **Local Storage**: Implemented in the application layer to store Roles name and ID mappings, as well as IntervalType and TransactionType for the Transaction services.

## AccountMicroService

### Authentication
1. **Login Process**:
   - Use admin credentials (username: admin, password: 123) for initial access.
   - Upon successful login, a token is generated for authorization of subsequent requests.
2. **Authorization**:
   - Managed by policies defined in AuthenticationConfiguration (located in the Infra Layer).
   - Different roles (admin, employee, customer) have varying levels of access to endpoints.

### Branch Creation
1. **Endpoint Input**:
   ```json
   {
     "branchName": "jouniehbranch",
     "address": "jounieh"
   }
   ```
   Note: Branch name must end with "branch" (validated by FluentValidator).

2. **Process**:
   a. BranchSchemaService copies tables from the default 'beirutbranch'.
   b. Two roles are created with specific permissions:
      - branchname_employee: Full access to own branch, read-only for others.
      - branchname_customer: Can make transactions, cannot delete or add accounts/recurrent transactions.
   c. The new branch is added to the public Branch table.
   d. A RabbitMQ message is published with the required data for the TransactionMicroservice.
   e. Keycloak service adds the required realm roles for the branch.

3. **Cross-Service Communication**:
   - RabbitMqSenderService in AccountMicroService sends the message.
   - RabbitMqReceiverService in TransactionMicroservice handles the creation on its end.

### Employee Creation
1. **Endpoint Input**:
   ```json
   {
     "username": "jouniehbranch_employee",
     "firstName": "Jason",
     "lastName": "Abdallah",
     "email": "jason@gmail.com",
     "branch": "jouniehbranch",
     "password": "123"
   }
   ```

2. **Process**:
   a. Input is validated using FluentValidator.
   b. Keycloak service (in the infra layer) adds the account to Keycloak and assigns the required realm role.
      Note: Two separate requests are used due to a Keycloak bug.
   c. An entry is added to the user table with the appropriate branch/role mapping (using LocalStorage cache).
   d. A gRPC message is sent to create this user in the TransactionMicroservice database.

### Account Creation
1. **Authentication**: Login with employee credentials (e.g., jouniehbranch_employee:123).

2. **Endpoint Input**:
   ```json
   {
     "firstName": "Joe",
     "lastName": "Joe",
     "username": "joe",
     "password": "123",
     "email": "joe1@gmail.com"
   }
   ```

3. **Process**:
   a. An action filter checks the logged-in employee's claims and sets the appropriate role and schema in the TenantService.
   b. The system checks if the customer already has an account using first and last name.
   c. If new, a customer entry is created in the public branch.
   d. The number of accounts is checked (max 5 per customer) and incremented.
   e. A new account entry is created and added to the database.
   f. A gRPC request is sent to the TransactionMicroservice (acting as a gRPC server on port 5001).
   g. Keycloak service adds the account with appropriate roles (customer and branch).
   h. A user entry is added to the public branch, and a corresponding gRPC request is sent.

## TransactionMicroservice

### Customer Endpoints
- **Authentication**: Login using customer credentials (e.g., joe:123).
- **Available Operations**:
  1. **Deposit**: Adds to the account balance.
  2. **Withdraw**: Removes from the account balance (cannot go negative).
  3. **GetAccountInformation**: Provides a ViewModel with account details.
  4. **Transactions**: OData endpoint for querying transactions with various filters.

- **Process**:
  - User info and branch information are extracted from token claims using the action filter (application layer).
  - Appropriate handlers manage the business logic for each operation.
  - Automapper in the application layer handles ViewModel transformations.

### Employee Endpoints
- **Authentication**: Login with employee credentials (e.g., jouniehbranch_employee:123).
- **Key Feature**: Add daily, weekly, or monthly recurrent transactions.
- **Recurrent Transaction Handling**:
  - Managed by a background service that runs once per day at a specified time.
  - RecurrentTransactionService processes these transactions:
    1. Retrieves all branches from the public schema.
    2. Uses a factory to create a dbcontext instance for each schema.
    3. Checks the recurrenttransaction table in each schema.
    4. Executes due transactions, updating account balances and transaction dates.

### Admin Endpoints
1. **Rollback Specific Account**
2. **Rollback Entire Database**

- **Process** (handled by RollbackHandler):
  1. Determines the appropriate branch and creates a dbcontext using a factory.
  2. For transactions: Only considers those created after the rollback date.
  3. For recurrent transactions: Uses a version column to track payments and adjust accordingly.
  4. Removes entries created after the rollback date.
  5. Adjusts account balances to reflect the rollback.

## Technical Details
- gRPC server (TransactionMicroservice) runs on port 5001 with HTTP/2 enabled to avoid HTTPS requirement.
- TransactionMicroservice operates on port 5000.
- OData Configuration for transaction queries is defined in the application layer.
- Background service for recurrent transactions is configurable for specific run times.

## Database Structure
- Public schema contains Branches table and User table with branch/role mappings.
- Each branch has its own schema with tables for accounts, transactions, and recurrent transactions.
- Version column in recurrent transactions table tracks the number of executed payments.

## Security Considerations
- Keycloak is used for identity and access management.
- Roles and permissions are granularly defined for different user types.
- Token-based authentication with claims for branch and role information.

## Performance Optimizations
- Use of LocalStorage for caching static data like role mappings.
- Efficient database querying using OData for transaction history.
- Background processing for recurrent transactions to distribute load.

## Future Enhancements
- Implement more robust password policies.
- Add more comprehensive logging and monitoring.
- Enhance the rollback feature with more granular options.
- Implement additional security measures like rate limiting and IP whitelisting.

Note: This README provides an in-depth overview of the system. For specific implementation details, please refer to the respective microservice codebases and additional documentation.
