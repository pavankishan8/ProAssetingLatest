# Connection Strings Configuration Guide

## Overview

The application uses **three connection strings**:

1. **MasterConnection** - For authentication (stores all users in `ProAssetinDev` database)
2. **Infosys** - Direct connection to Infosys customer database (`ProAsset_Infosys`)
3. **Wipro** - Direct connection to Wipro customer database (`ProAsset_Wipro`)

## How It Works

### Login Flow

1. **User logs in with email** (e.g., `pavan@infosys.com`)
2. **Authentication uses MasterConnection** → Connects to `ProAssetinDev` database to verify credentials
3. **Email domain is extracted** → `infosys.com` → tenant ID: `infosys`
4. **Tenant database is determined** → `ProAsset_Infosys`
5. **All data operations use tenant connection** → `ProAsset_Infosys` database

### Connection String Configuration

In `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Infosys": "Server=localhost\\SQLEXPRESS;Database=ProAsset_Infosys;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "Wipro": "Server=localhost\\SQLEXPRESS;Database=ProAsset_Wipro;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "MasterConnection": "Server=localhost\\SQLEXPRESS;Database=ProAssetinDev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### Master Connection (ProAssetinDev)

**Purpose**: Stores all users across all tenants for authentication

**Tables**:
- `ProAssetinUsers` - All user accounts
- `ProAssetinRoles` - Roles (Admin, User)
- `ProAssetinUserRoles` - User-Role mappings
- `ProAssetinCompanies` - Company information
- All other Identity tables

**Usage**: Used by `ApplicationDbContext` for authentication

### Customer Connection Strings

**Infosys Connection**:
- **Database**: `ProAsset_Infosys`
- **Usage**: Users with `@infosys.com` email domain
- **Example**: `pavan@infosys.com` → Uses `Infosys` connection

**Wipro Connection**:
- **Database**: `ProAsset_Wipro`
- **Usage**: Users with `@wipro.com` email domain
- **Example**: `admin@wipro.com` → Uses `Wipro` connection

**Tables**:
- `ProAssetinAssets` - Asset records
- `ProAssetinInventoryLogs` - Inventory logs
- `ProAssetinInvoices` - Invoice records
- `ProAssetinPurchaseOrders` - Purchase orders
- `ProAssetinTickets` - Support tickets
- `ProAssetinVendors` - Vendor information

**Usage**: Used by `TenantDbContext` for all business data operations

## Example Scenarios

### Scenario 1: User Login

```
User Email: pavan@infosys.com
Password: Admin123

Flow:
1. MasterConnection → ProAssetinDev → Verify user credentials
2. Extract domain: infosys.com → tenantId: infosys
3. Build tenant connection: ProAsset_Infosys
4. JWT token includes TenantId claim
5. All subsequent API calls use ProAsset_Infosys database
```

### Scenario 2: Get Assets

```
Request: GET /api/assets
Headers: Authorization: Bearer {token}

Flow:
1. Extract TenantId from JWT token claim
2. Build tenant connection: ProAsset_{TenantId}
3. Query ProAssetinAssets from tenant database
4. Return assets for that tenant only
```

## Configuration Details

### MasterConnection

```json
"MasterConnection": "Server=localhost\\SQLEXPRESS;Database=ProAssetinDev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

- **Server**: SQL Server instance
- **Database**: `ProAssetinDev` (fixed)
- **Used by**: `ApplicationDbContext` (authentication)

### Infosys Connection

```json
"Infosys": "Server=localhost\\SQLEXPRESS;Database=ProAsset_Infosys;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

- **Database**: `ProAsset_Infosys` (fixed)
- **Used by**: `TenantDbContext` when tenant is `infosys`

### Wipro Connection

```json
"Wipro": "Server=localhost\\SQLEXPRESS;Database=ProAsset_Wipro;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

- **Database**: `ProAsset_Wipro` (fixed)
- **Used by**: `TenantDbContext` when tenant is `wipro`

## Database Creation

### Master Database (ProAssetinDev)

Created automatically on first API run, or manually execute:
```sql
USE master;
CREATE DATABASE ProAssetinDev;
```

Then run migrations:
```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Tenant Databases (ProAsset_{Domain})

Created automatically when:
1. User registers with a new email domain
2. First user login from a new tenant

Or manually execute:
```sql
USE master;
CREATE DATABASE ProAsset_Infosys;
CREATE DATABASE ProAsset_Wipro;
-- etc.
```

## Important Notes

1. **Email-based tenant resolution**: The tenant is determined by the email domain, not by configuration
2. **Automatic database creation**: Tenant databases are created automatically on first use
3. **Isolation**: Each tenant has its own database, providing complete data isolation
4. **Cross-database references**: Tenant databases reference `ProAssetinUsers` in the master database for assigned users

## Troubleshooting

### Error: "Cannot open database"

**Solution**: Ensure the tenant database exists or let the system create it automatically on first login.

### Error: "Invalid connection string"

**Solution**: Verify `MasterConnection`, `Infosys`, and `Wipro` connection strings are correctly formatted in `appsettings.json`.

### Users can't login

**Solution**: 
1. Check `MasterConnection` points to `ProAssetinDev`
2. Ensure master database has been created
3. Verify users exist in `ProAssetinUsers` table

