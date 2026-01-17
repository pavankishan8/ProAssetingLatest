# Complete Database Setup Guide

## Quick Setup Steps

### Step 1: Create Master Database Schema

**Option A: Using Entity Framework (Recommended)**
```powershell
cd ProAssetin.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Option B: Using SQL Scripts**
1. Open SQL Server Management Studio
2. Connect to `localhost\SQLEXPRESS`
3. Execute `Data/Scripts/CompleteDatabaseSchema.sql`

### Step 2: Run the API to Auto-Create Test Users

```powershell
dotnet run
```

The API will automatically:
- Create roles (Admin, User)
- Create test users with password `Admin123`
- Create tenant databases on first login

### Step 3: Access Swagger UI

Navigate to: `https://localhost:5001/swagger`

## Test Login Credentials

| Email | Password | Role | Tenant | Database |
|-------|----------|------|--------|----------|
| `admin@infosys.com` | `Admin123` | Admin | infosys | ProAsset_Infosys |
| `user@infosys.com` | `Admin123` | User | infosys | ProAsset_Infosys |
| `admin@wipro.com` | `Admin123` | Admin | wipro | ProAsset_Wipro |

## Database Tables Overview

### Master Database (ProAssetinDev)

#### Identity Tables (ProAssetin naming)
- `ProAssetinUsers` - All users
- `ProAssetinRoles` - Roles (Admin, User)
- `ProAssetinUserRoles` - User-Role mappings
- `ProAssetinUserClaims` - User claims
- `ProAssetinUserLogins` - External logins
- `ProAssetinRoleClaims` - Role claims
- `ProAssetinUserTokens` - User tokens

#### Application Tables
- `ProAssetinCompanies` - Company information
- `ProAssetinEmployeeConfigurations` - Employee settings

### Tenant Databases (ProAsset_Infosys, ProAsset_Wipro, etc.)

#### Asset Management
- `ProAssetinAssets` - Asset records
- `ProAssetinInventoryLogs` - Inventory activity logs

#### Financial
- `ProAssetinInvoices` - Invoice records
- `ProAssetinPurchaseOrders` - Purchase orders
- `ProAssetinVendors` - Vendor information

#### Support
- `ProAssetinTickets` - Support tickets

## Table Relationships

```
ProAssetinCompanies (1) ←→ (M) ProAssetinUsers
ProAssetinUsers (1) ←→ (M) ProAssetinEmployeeConfigurations
ProAssetinUsers (M) ←→ (M) ProAssetinRoles (via ProAssetinUserRoles)

[Tenant Databases]
ProAssetinAssets (1) ←→ (M) ProAssetinInventoryLogs
ProAssetinUsers [from master] ←→ (M) ProAssetinAssets (AssignedTo)
ProAssetinUsers [from master] ←→ (M) ProAssetinTickets (AssignedTo)
ProAssetinUsers [from master] ←→ (M) ProAssetinInvoices (CreatedBy)
```

## Important Notes

1. **Table Naming**: All tables use `ProAssetin` prefix (not `AspNet`)
2. **Cross-Database References**: Tenant databases reference `ProAssetinUsers` in master database
3. **Multi-Tenancy**: Each tenant has its own database with same schema
4. **Auto-Creation**: Tenant databases are created automatically on first user login

## Verify Database Setup

1. Check master database:
   ```sql
   USE ProAssetinDev;
   SELECT * FROM ProAssetinUsers;
   SELECT * FROM ProAssetinRoles;
   ```

2. Check tenant database (after login):
   ```sql
   USE ProAsset_Infosys;
   SELECT * FROM ProAssetinAssets;
   ```

3. Verify relationships:
   ```sql
   -- Users with their roles
   SELECT u.Email, r.Name as Role
   FROM ProAssetinUsers u
   JOIN ProAssetinUserRoles ur ON u.Id = ur.UserId
   JOIN ProAssetinRoles r ON ur.RoleId = r.Id;
   ```

## Troubleshooting

If tables are not created:
1. Check connection string in `appsettings.json`
2. Ensure SQL Server is running
3. Verify database exists: `SELECT name FROM sys.databases WHERE name = 'ProAssetinDev'`
4. Run migrations: `dotnet ef database update`

