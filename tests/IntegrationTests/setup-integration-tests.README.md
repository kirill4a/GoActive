# HOWTO: setup local dev environment for integrattion tests

### Prerequisites
Integration tests are based on [Testcontainers .NET](https://dotnet.testcontainers.org/) library which requires docker. 
Make sure you have docker or docker desktop installed. \
See also: [Getting started with Testcontainers for .NET](https://testcontainers.com/guides/getting-started-with-testcontainers-for-dotnet/)

1. Add next settings to your user `secrets.json` for each integration tests project. (_managing user secrets depends on your IDE_)\
You can use the same secrets file across multiple tests projects.

    ```json
    {
        "TestContainers:PgOptions:Database": "TestDb",
        "TestContainers:PgOptions:Username": "TestUser",
        "TestContainers:PgOptions:Password": "TestPassword"
    }
    ```
   
If you have any docker-related troubles starting testcontainers tests, refer the following resources: 
+ [Stackoverflow](https://stackoverflow.com/questions/76655770/testcontainers-how-to-fix-bind-source-path-does-not-exist)
+ [Custom Configuration](https://dotnet.testcontainers.org/custom_configuration/)
+ [How to run tests with TestContainers in WSL2 without Docker Desktop](https://gist.github.com/sz763/3b0a5909a03bf2c9c5a057d032bd98b7)
+ [Trying Rootless Docker with Testcontainers](https://bsideup.github.io/posts/rootless_docker/)
