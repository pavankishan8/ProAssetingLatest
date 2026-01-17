# Fixing ASP.NET Core HTTPS Development Certificate Issues

## Common Solutions

### Option 1: Trust the Certificate (Recommended)
Run PowerShell as **Administrator** and execute:

```powershell
dotnet dev-certs https --trust
```

If you get an error, try with the `--clean` flag first:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### Option 2: Check Certificate Status
Check if the certificate is already trusted:

```powershell
dotnet dev-certs https --check --verbose
```

### Option 3: Manual Certificate Trust (If auto-trust fails)
1. Export the certificate:
   ```powershell
   dotnet dev-certs https --export-path "C:\temp\aspnetcore-https.pfx" --password "your-password"
   ```

2. Import it to the Trusted Root Certification Authorities:
   - Open `certlm.msc` (Certificate Manager)
   - Navigate to: Trusted Root Certification Authorities → Certificates
   - Right-click → All Tasks → Import
   - Browse to `C:\temp\aspnetcore-https.pfx`
   - Enter the password
   - Click Next and Finish

### Option 4: Run Without HTTPS (Quick Fix for Development)
If you just want to run the API without HTTPS for now, you can modify `Properties/launchSettings.json` to only use HTTP:

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Then run with:
```powershell
dotnet run --launch-profile http
```

### Option 5: Update Frontend API URL
If you disable HTTPS, update the Angular frontend's `environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // Changed from https://localhost:5001
};
```

## Notes

- The certificate error doesn't prevent the API from running - you'll just see a browser warning
- For production, you'll use proper SSL certificates from a certificate authority
- The development certificate is only needed for local HTTPS testing


