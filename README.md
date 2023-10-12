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
   dotnet user-secrets set "jwt-issuer" "secretkeyissuer"  
   ```

   ```
   dotnet user-secrets set "jwt-audience" "http://localhost:5203/"  
   ```

   ```
   dotnet user-secrets set "jwt-secret" "iis-itu-super-secret-extra-long-key"
   ```
   
   You need to setup your gmail account so you can use it for sending mails from this app.
   ```
   dotnet user-secrets set "mail" "your gmail" 
   ```
   ```
   dotnet user-secrets set "mail-password" "your password"
   ```
