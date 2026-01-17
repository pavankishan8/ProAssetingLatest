# Troubleshooting Guide

## API Not Starting or Swagger Not Loading

### 1. Check if API is Running
Make sure the API is actually running. Check the console output for:
- "Now listening on: https://localhost:5001"
- Any error messages

### 2. Swagger URL
The Swagger UI should be accessible at:
- **HTTPS**: `https://localhost:5001/swagger`
- **HTTP**: `http://localhost:5000/swagger`

If RoutePrefix is set to empty string in Program.cs, Swagger will be at root `/` instead.

### 3. Common Startup Issues

#### Database Connection Error
```
Cannot open database "ProAssetinDev" requested by the login
```

**Solution**: 
1. Check SQL Server is running
2. Update connection string in `appsettings.json`
3. Create the database manually or run migrations:
   ```powershell
   dotnet ef database update
   ```

#### Port Already in Use
```
Failed to bind to address https://localhost:5001
```

**Solution**:
1. Change port in `launchSettings.json`
2. Or kill the process using the port:
   ```powershell
   netstat -ano | findstr :5001
   taskkill /PID <PID> /F
   ```

#### Migration Issues
If you get migration errors, try:
```powershell
# Remove all migrations and start fresh
Remove-Item -Recurse -Force Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Verify API is Running

Check these URLs:
- Root: `http://localhost:5000` or `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger` or `https://localhost:5001/swagger`
- Health check (if added): `http://localhost:5000/health`

### 5. Browser Issues
- Clear browser cache
- Try incognito/private mode
- Try a different browser
- Accept the self-signed certificate warning if using HTTPS

### 6. Check Logs
Check console output when running `dotnet run` for error messages.


