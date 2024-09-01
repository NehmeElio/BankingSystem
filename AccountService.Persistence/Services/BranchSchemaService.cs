using AccountService.Persistence.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AccountService.Persistence.Services;

public class BranchSchemaService : IBranchSchemaService
{
    private readonly IConfiguration _configuration;

    public BranchSchemaService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void CreateBranchSchema(string newBranchName, string sourceSchemaName = "beirutbranch")
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var accountTable = "\"Account\"";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Start a transaction
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Create the new schema
                    var createSchemaCmd = $"CREATE SCHEMA IF NOT EXISTS \"{newBranchName}\";";
                    using (var command = new NpgsqlCommand(createSchemaCmd, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Copy tables from the source schema to the new schema
                    var copyTablesCmd = $@"
                            DO $$
                            DECLARE
                                r RECORD;
                            BEGIN
                                FOR r IN
                                    SELECT tablename
                                    FROM pg_tables
                                    WHERE schemaname = '{sourceSchemaName}'
                                LOOP
                                    EXECUTE 'CREATE TABLE {newBranchName}.' || quote_ident(r.tablename) || ' (LIKE {sourceSchemaName}.' || quote_ident(r.tablename) || ' INCLUDING ALL);';
                                END LOOP;
                            END
                            $$;";
                    using (var command = new NpgsqlCommand(copyTablesCmd, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Create roles
                    var createRolesCmd = $@"
                            DO $$
                            BEGIN
                                IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = '{newBranchName}_employee') THEN
                                    EXECUTE 'CREATE ROLE {newBranchName}_employee WITH LOGIN PASSWORD ''123'';';
                                END IF;
                                IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = '{newBranchName}_customer') THEN
                                    EXECUTE 'CREATE ROLE {newBranchName}_customer WITH LOGIN PASSWORD ''123'';';
                                END IF;
                            END
                            $$;";
                    using (var command = new NpgsqlCommand(createRolesCmd, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Grant permissions to roles
                    var grantPermissionsCmd = $@"
                            DO $$
                            BEGIN
                                -- Grant permissions to employee role
                                EXECUTE 'GRANT USAGE ON SCHEMA {newBranchName} TO {newBranchName}_employee;';
                                EXECUTE 'GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA {newBranchName} TO {newBranchName}_employee;';
                                EXECUTE 'GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA {newBranchName} TO {newBranchName}_employee;';

                                -- Grant read-only permissions to employee role on other schemas
                                EXECUTE 'GRANT USAGE ON SCHEMA {sourceSchemaName} TO {newBranchName}_employee;';
                                EXECUTE 'GRANT SELECT ON ALL TABLES IN SCHEMA {sourceSchemaName} TO {newBranchName}_employee;';
                                EXECUTE 'GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA {sourceSchemaName} TO {newBranchName}_employee;';
                                
                                 -- Grant read-write access for employee role on the public schema
                                EXECUTE 'GRANT USAGE ON SCHEMA public TO {newBranchName}_employee;';
                                EXECUTE 'GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO {newBranchName}_employee;';
                                EXECUTE 'GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO {newBranchName}_employee;';
                                
                                -- Revoke all permissions from customer role
                                EXECUTE 'REVOKE ALL ON SCHEMA {newBranchName} FROM {newBranchName}_customer;';
                                EXECUTE 'REVOKE ALL ON ALL TABLES IN SCHEMA {newBranchName} FROM {newBranchName}_customer;';
                                EXECUTE 'REVOKE ALL ON ALL SEQUENCES IN SCHEMA {newBranchName} FROM {newBranchName}_customer;';


                                -- perimissions for customers
                               EXECUTE 'GRANT USAGE ON SCHEMA {newBranchName} TO {newBranchName}_customer;';
                               EXECUTE 'GRANT SELECT,UPDATE ON {newBranchName}.{accountTable} TO {newBranchName}_customer;';
                               END
                               $$;";
                    using (var command = new NpgsqlCommand(grantPermissionsCmd, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error while creating branch schema and setting up roles", ex);
                }
            }
        }
    }
}