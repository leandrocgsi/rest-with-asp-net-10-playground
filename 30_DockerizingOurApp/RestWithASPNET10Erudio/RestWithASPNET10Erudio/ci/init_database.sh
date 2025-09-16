#!/bin/bash
set -e

echo "Waiting for SQL Server to start..."
/opt/mssql/bin/sqlservr &

# Wait until SQL Server is ready to accept connections
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "SELECT 1" &> /dev/null
do
  echo "SQL Server is not ready yet... waiting 5 seconds"
  sleep 5
done

echo "SQL Server is ready!"

# Create the database if it doesn't exist
/opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "IF DB_ID('$DATABASE_NAME') IS NULL CREATE DATABASE [$DATABASE_NAME]" -d master

# Execute all SQL scripts (DDL and DML) in order
for i in $(find /home/database/ -name "*.sql" | sort --version-sort); do
    echo "Executing script $i..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -d $DATABASE_NAME -i "$i"
done

wait
