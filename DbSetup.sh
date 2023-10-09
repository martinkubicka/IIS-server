#!/bin/bash

# Database connection details
DB_HOST="antioznuk-martinkubicka22-d781.aivencloud.com"
DB_PORT="15939"
DB_USER="avnadmin"
DB_PASSWORD="AVNS_4iCQ_2BI9PsIL6BZ2nu"
DB_NAME="defaultdb"
SQL_SCRIPT="DbSetup.sql"

mysql -h "$DB_HOST" -P "$DB_PORT"  -u "$DB_USER" -p"$DB_PASSWORD" "$DB_NAME" < "$SQL_SCRIPT"

# Check the exit status of the mysql command
if [ $? -eq 0 ]; then
  echo "SQL script '$SQL_SCRIPT' executed successfully."
else
  echo "Error executing SQL script '$SQL_SCRIPT'."
  exit 1
fi

