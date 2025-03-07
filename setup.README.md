# HOWTO: setup local dev environment

1. Run next command in the Terminal

    ```shell
    docker compose -f "./docker-compose/infra.dev.yml" -p geo-infra-dev up -d
    ```
   
1. Add next settings to your user `secrets.json` for `GoActive.WebApi` project. (_managing user secrets depends on your IDE_)

    ```json
   {
    "ConnectionStrings": {
        "Geo": "Host=localhost;Port=5432;Database=geo;Username=your_username;Password=your_password"
    }
   }
    ```
1. Add next settings to your user `secrets.json` for `GoActive.Infrastructure.Storage.Geo.Migrations` project. (_managing user secrets depends on your IDE_)

    ```json
   {
    "ConnectionStrings": {
        "Geo": "Host=localhost;Port=5432;Database=geo;Username=your_username;Password=your_password"
    }
   }
    ```

**NOTE:** Secrets file folder name (secrets key) should be the same as specified in correspondent project settings:
   ```xml
   <UserSecretsId>ee28b0fb-2ee5-4b48-a0ef-d84288b63bed</UserSecretsId>
   ```

# HOWTO: access pgAdmin and connect postgreSQL database

After previous actions being completed you can navigare [localhost:5056](http://localhost:5056) in your browser and authorize in pgAdmin web version.\
* Use `PGADMIN_DEFAULT_EMAIL` and `PGADMIN_DEFAULT_PASSWORD` values for pgAdmin service\
(as specified in [docker compose configuration](./docker-compose/infra.dev.yml)) 
* Use `postgres-geo` hostname to connect pg server via pgAdmin\
(as specified in [docker compose configuration](./docker-compose/infra.dev.yml))