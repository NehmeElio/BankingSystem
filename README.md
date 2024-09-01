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

[The rest of the README content remains the same as in the previous version...]

