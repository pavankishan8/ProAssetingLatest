# ProAssetin Database Schema Documentation

## Database Naming Convention
All tables use the **ProAssetin** prefix instead of AspNet:
- `ProAssetinUsers` (instead of AspNetUsers)
- `ProAssetinRoles` (instead of AspNetRoles)
- `ProAssetinCompanies`
- `ProAssetinAssets`
- etc.

## Master Database: ProAssetinDev

### Core Identity Tables
1. **ProAssetinUsers** - All users across all tenants
2. **ProAssetinRoles** - User roles (Admin, User)
3. **ProAssetinUserRoles** - User-Role mappings
4. **ProAssetinUserClaims** - User claims
5. **ProAssetinUserLogins** - External login providers
6. **ProAssetinRoleClaims** - Role claims
7. **ProAssetinUserTokens** - User tokens

### Application Tables
8. **ProAssetinCompanies** - Company/organization information
9. **ProAssetinEmployeeConfigurations** - Employee-specific configurations

## Tenant Databases (ProAsset_Infosys, ProAsset_Wipro, etc.)

### Asset Management Tables
1. **ProAssetinAssets** - Asset records
2. **ProAssetinInventoryLogs** - Inventory activity logs

### Financial Tables
3. **ProAssetinInvoices** - Invoice records
4. **ProAssetinPurchaseOrders** - Purchase order records
5. **ProAssetinVendors** - Vendor information

### Support Tables
6. **ProAssetinTickets** - Support ticket records

## Table Relationships

### Master Database Relationships
```
ProAssetinCompanies (1) ──→ (M) ProAssetinUsers
ProAssetinUsers (1) ──→ (M) ProAssetinEmployeeConfigurations
ProAssetinUsers (M) ──→ (M) ProAssetinRoles (via ProAssetinUserRoles)
```

### Tenant Database Relationships
```
ProAssetinAssets (1) ──→ (M) ProAssetinInventoryLogs
ProAssetinUsers [from master DB] ──→ (M) ProAssetinAssets (AssignedToUserId)
ProAssetinUsers [from master DB] ──→ (M) ProAssetinTickets (TaskAssignedToID)
ProAssetinUsers [from master DB] ──→ (M) ProAssetinInvoices (CreatedByUserId)
ProAssetinUsers [from master DB] ──→ (M) ProAssetinPurchaseOrders (CreatedByUserId)
```

## Key Fields

### ProAssetinUsers
- `Id` (PK) - GUID/string
- `Email` - User email (also determines tenant)
- `TenantId` - Tenant identifier (derived from email domain)
- `CompanyID` - Foreign key to ProAssetinCompanies
- `FirstName`, `LastName`
- Custom fields: `DomainAccount`, `EmployeeType`, `Location`, `ProjectName`, etc.

### ProAssetinAssets
- `Id` (PK) - Auto-increment
- `AssetId` - Unique asset identifier per tenant
- `Name`, `Category`, `Status`
- `AssignedToUserId` - Foreign key to ProAssetinUsers (cross-database reference)

### ProAssetinInvoices
- `Id` (PK)
- `InvoiceNumber` - Unique per tenant
- `Amount`, `Status`
- `CreatedByUserId` - Foreign key to ProAssetinUsers

### ProAssetinTickets
- `TaskID` (PK)
- `TaskTitle`, `TaskState`, `Priority`
- `TaskAssignedToID` - Foreign key to ProAssetinUsers

## Setup Instructions

1. **Create Master Database**:
   - Execute `Data/Scripts/CompleteDatabaseSchema.sql`

2. **Seed Test Data**:
   - Run the API (`dotnet run`) - users will be auto-created
   - Or execute `Data/Scripts/SeedTestUser.sql` (note: password hashing may not work)

3. **Tenant Databases**:
   - Created automatically on first user login
   - Or manually: Execute `Data/Scripts/CreateTenantDatabaseSchema.sql` for each tenant

## Test Login Credentials

**Email**: `admin@infosys.com`  
**Password**: `Admin123`

**Email**: `admin@wipro.com`  
**Password**: `Admin123`

