# GEO: deal with EF core migrations

## Prerequisites

* Set database connection string `ConnectionStrings:Geo` in user `secrets.json` (_managing user secrets depends on your IDE_)
* Switch to Configuration `Debug`. In `Release` user `secrets.json` will be [skipped](ContextFactory.cs).

## Actions

>### use this script to add new migration: 
>
>[add_migration.sh](./scripts/add_migration.sh)

>### use this script to generate SQL script from migrations: 
>
>[generate_script.sh](./scripts/generate_script.sh)

>### use this script to apply migrations: 
>
>[apply_migrations.sh](./scripts/apply_migrations.sh)

>### use this script to remove the last migration:
>
>[remove_migration.sh](./scripts/remove_migration.sh)


To run scripts in VSCode, you can use the integrated terminal. Open the terminal in VSCode and execute the scripts.
