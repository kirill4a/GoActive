#!/bin/bash

if [ -z "$1" ]; then
  read -p "Please provide a migration name: " migration_name
else
  migration_name=$1
fi

if [ -z "$migration_name" ]; then
  echo "Migration name cannot be empty."
  exit 1
fi

cd ../
dotnet ef migrations add $migration_name --context GeoContext --output-dir Migrations
