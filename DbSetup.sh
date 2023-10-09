#!/bin/bash

# Database connection details
DB_HOST="db4free.net"
DB_USER="antioznuk"
DB_PASSWORD="koblizek"
DB_NAME="antioznuk"
SQL_SCRIPT="DbSetup.sql"

mysql -h "$DB_HOST" -u "$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" < "$SQL_SCRIPT"

# Check the exit status of the mysql command
if [ $? -eq 0 ]; then
  echo "SQL script '$SQL_SCRIPT' executed successfully."
else
  echo "Error executing SQL script '$SQL_SCRIPT'."
  exit 1
fi

