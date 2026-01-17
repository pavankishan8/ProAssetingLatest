# Database Setup Guide

## Quick Setup

### Option 1: Using Entity Framework Migrations (Recommended)

1. **Install EF Core tools** (if not already installed):
   ```powershell
   dotnet tool install --global dotnet-ef
   ```

2. **Create initial migration**:
   ```powershell
   cd ProAssetin.API
   dotnet ef migrations add InitialCreate
   ```

3. **Create database and apply migrations**:
   ```powershell
   dotnet ef database update
   ```

4. **Run the API** - Test users will be automatically created:
   ```powershell
   dotnet run
   ```

### Option 2: Using SQL Scripts (Manual Setup)

1. **Create master database schema**:
   - Open SQL Server Management Studio
   - Connect to `localhost\SQLEXPRESS`
   - Open `Data/Scripts/CreateDatabase.sql`
   - Execute the script

2. **Seed test users**:
   - Run `Data/Scripts/SeedTestUser.sql`
   - **Note**: The password hash in SQL script may not work correctly
   - Better to use Option 1 (EF Migrations) or register users via API

3. **Create tenant databases** (will be created automatically on first user login):
   - Or manually create: `ProAsset_Infosys`, `ProAsset_Wipro`
   - Run `Data/Scripts/CreateTenantTables.sql` for each tenant database

## Test Users (Auto-created when running the API)

The API automatically seeds these test users on startup (Development mode only):

| Email | Password | Role | Tenant | Database |
|-------|----------|------|--------|----------|
| `admin@infosys.com` | `Admin123` | Admin | infosys | ProAsset_Infosys |
| `user@infosys.com` | `Admin123` | User | infosys | ProAsset_Infosys |
| `admin@wipro.com` | `Admin123` | Admin | wipro | ProAsset_Wipro |

## Manual User Creation

### Via API Registration Endpoint
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "Admin",
  "lastName": "User",
  "email": "admin@infosys.com",
  "password": "Admin123",
  "confirmPassword": "Admin123"
}
```

### Via Swagger UI
1. Start the API: `dotnet run`
2. Navigate to: `https://localhost:5001/swagger`
3. Use the `POST /api/auth/register` endpoint

## Login Credentials

After setup, you can login with:

**Email**: `admin@infosys.com`  
**Password**: `Admin123`

This will:
- Create tenant database `ProAsset_Infosys` automatically (if it doesn't exist)
- Create all necessary tables in the tenant database
- Authenticate and return a JWT token

## Troubleshooting

### "Database does not exist" error
- Ensure SQL Server is running
- Check connection string in `appsettings.json`
- Create the database manually: `CREATE DATABASE ProAssetinDev;`

### "Cannot open database" error
- Verify SQL Server authentication (Windows Auth or SQL Auth)
- Check connection string credentials
- Ensure database exists

### Tables not created
- Run EF migrations: `dotnet ef database update`
- Or execute the SQL scripts manually
- Check application logs for errors

### Test users not created
- Ensure running in Development mode (`ASPNETCORE_ENVIRONMENT=Development`)
- Check application logs during startup
- Users are created automatically on first API run

## Database Schema

### Master Database (ProAssetinDev)
- `AspNetUsers` - All users across all tenants
- `AspNetRoles` - User roles
- `AspNetUserRoles` - User-Role mappings
- Identity-related tables

### Tenant Databases (ProAsset_Infosys, ProAsset_Wipro, etc.)
- `Assets` - Asset records
- `InventoryLogs` - Inventory activity logs

Each tenant database is isolated and created automatically on first use.


