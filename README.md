# Project setup

1. Enable secret storage and set a secret

   - To use user secrets, run the following command in the project directory (use cd IIS-server/IIS_SERVER/IIS_SERVER command in terminal):


   ```
   dotnet user-secrets init
   ```

   ```
   dotnet user-secrets set "DB:ConnectionString" "your mysql connection string"
   ```

   ```
   dotnet user-secrets set "jwt-issuer" "any string"
   ```

   ```
   dotnet user-secrets set "jwt-audience" "any string"
   ```

   ```
   dotnet user-secrets set "jwt-secret" "any string"
   ```
