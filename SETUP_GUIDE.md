# ProAssetin - Complete Setup Guide

This guide will help you set up and run the complete ProAssetin application (Backend API + Angular Frontend).

## Prerequisites

1. **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Node.js 18+ and npm** - [Download here](https://nodejs.org/)
3. **SQL Server Express** (or full version) - [Download here](https://www.microsoft.com/sql-server/sql-server-downloads)
4. **Angular CLI** (will be installed globally if not present)

## Backend Setup (.NET Core 8 API)

### Step 1: Navigate to API Directory
```bash
cd ProAssetin.API
```

### Step 2: Restore Packages
```bash
dotnet restore
```

### Step 3: Update Connection String
Edit `appsettings.json` and update the connection string to match your SQL Server:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ProAssetinDev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### Step 4: Create Database and Run Migrations
```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Add initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### Step 5: Run the API
```bash
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger` (or http://localhost:5000/swagger)

### Step 6: Test the API
1. Open Swagger UI in your browser
2. Try registering a user via `/api/auth/register`
3. Login via `/api/auth/login` to get a JWT token
4. Use the "Authorize" button in Swagger to set the token and test protected endpoints

## Frontend Setup (Angular)

The Angular frontend will be created in the next steps. Follow the Angular setup instructions once the frontend project is ready.

## Multi-Tenancy Configuration

The system automatically creates tenant databases based on email domain:

- User with email `admin@infosys.com` → Database: `ProAsset_Infosys`
- User with email `user@wipro.com` → Database: `ProAsset_Wipro`

Tenant databases are created automatically when:
1. A user registers with a new domain
2. A user logs in and the tenant database doesn't exist

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server is running
- Check connection string matches your SQL Server instance name
- Verify Windows Authentication or update to SQL Server Authentication if needed

### Port Already in Use
- Change ports in `Properties/launchSettings.json`
- Or kill the process using the port

### Migration Errors
- Ensure SQL Server is accessible
- Check connection string is correct
- Delete existing migrations and recreate if needed

## Next Steps

Once both backend and frontend are running:
1. Register a new user account
2. Login to get JWT token
3. Access the dashboard and asset management features
4. Test multi-tenancy by registering users with different email domains

