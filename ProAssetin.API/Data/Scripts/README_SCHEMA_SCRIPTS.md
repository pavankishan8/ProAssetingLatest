# Database Schema Creation Scripts

## 📋 Script Execution Order

Execute the scripts in the following order:

### Step 1: Create Databases
**File**: `00_CreateDatabases.sql`
- Creates master database: `ProAssetinDev`
- Creates Infosys database: `ProAsset_Infosys`
- Creates Wipro database: `ProAsset_Wipro`

```sql
-- Run in SQL Server Management Studio
-- Right-click on master database → New Query → Execute
```

### Step 2: Create Master Database Tables
**File**: `01_MasterDatabase_Schema.sql`
- Run on `ProAssetinDev` database
- Creates 9 tables for authentication and user management

```sql
USE [ProAssetinDev]
GO
-- Then run the script
```

**Tables Created**:
1. `ProAssetinCompanies` - Company information
2. `ProAssetinUsers` - All user accounts
3. `ProAssetinRoles` - User roles (Admin, User)
4. `ProAssetinUserRoles` - User-Role mappings
5. `ProAssetinUserClaims` - User claims
6. `ProAssetinUserLogins` - External login providers
7. `ProAssetinRoleClaims` - Role claims
8. `ProAssetinUserTokens` - User tokens
9. `ProAssetinEmployeeConfigurations` - Employee settings

### Step 3: Create Tenant Database Tables
**File**: `02_TenantDatabase_Schema.sql`
- Run on **EACH** tenant database separately
- Creates 7 tables for business data (includes CompanySettings)

**For Infosys Database**:
```sql
USE [ProAsset_Infosys]
GO
-- Then run the script
```

**For Wipro Database**:
```sql
USE [ProAsset_Wipro]
GO
-- Then run the script
```

**Tables Created** (per tenant database):
1. `ProAssetinAssets` - Asset records
2. `ProAssetinInventoryLogs` - Inventory activity logs
3. `ProAssetinInvoices` - Invoice records
4. `ProAssetinPurchaseOrders` - Purchase order records
5. `ProAssetinTickets` - Support ticket records
6. `ProAssetinVendors` - Vendor information
7. `ProAssetinSoftware` - Software and license records
8. `ProAssetinCompanySettings` - Company settings and configuration

### Step 4: Create CompanySettings Table (Optional - For Existing Databases Only)
**File**: `03_CreateCompanySettingsTable.sql`
- **Auto-creates** the `ProAssetinCompanySettings` table in **ALL** tenant databases at once
- Finds all databases with `ProAsset_` prefix automatically
- Safe to run multiple times (idempotent)
- **Only needed if** you have existing tenant databases without CompanySettings table
- CompanyLogo is stored as **NVARCHAR(MAX)** for Base64 encoding
- **Note**: If you're creating new databases, Step 3 already includes this table

```sql
-- Run in SQL Server Management Studio
-- Right-click on master database → New Query → Execute
-- This script will process all tenant databases automatically
```

### Step 5: Add VendorId to PurchaseOrders (Optional - For Existing Databases Only)

**Option A - Automated Script (Recommended)**:
**File**: `04_AddVendorIdToPurchaseOrders.sql`
- **Auto-adds** the `VendorId` column to `ProAssetinPurchaseOrders` table in **ALL** tenant databases at once
- Finds all databases with `ProAsset_` prefix automatically
- Safe to run multiple times (idempotent)
- **Only needed if** you have existing PurchaseOrders tables without VendorId column
- Creates an index on VendorId for better query performance
- **Note**: If you're creating new databases, Step 3 already includes this column

```sql
-- Run in SQL Server Management Studio
-- Right-click on master database → New Query → Execute
-- This script will process all tenant databases automatically
```

**Option B - Manual Script (Simple ALTER TABLE)**:
**File**: `04_AddVendorIdToPurchaseOrders_Simple.sql`
- Simple ALTER TABLE syntax to manually add the column
- Run on each tenant database separately
- Use this if you prefer manual control or the automated script doesn't work

**For Infosys Database**:
```sql
USE [ProAsset_Infosys];
GO
ALTER TABLE [dbo].[ProAssetinPurchaseOrders]
ADD [VendorId] INT NULL;

CREATE INDEX [IX_ProAssetinPurchaseOrders_VendorId] 
ON [dbo].[ProAssetinPurchaseOrders]([VendorId]);
GO
```

**For Wipro Database**:
```sql
USE [ProAsset_Wipro];
GO
ALTER TABLE [dbo].[ProAssetinPurchaseOrders]
ADD [VendorId] INT NULL;

CREATE INDEX [IX_ProAssetinPurchaseOrders_VendorId] 
ON [dbo].[ProAssetinPurchaseOrders]([VendorId]);
GO
```


## 🚀 Quick Setup Guide

### Option 1: Run Scripts Manually

1. **Create Databases**:
   ```sql
   -- Open SQL Server Management Studio
   -- Connect to: localhost\SQLEXPRESS
   -- Open 00_CreateDatabases.sql
   -- Execute
   ```

2. **Create Master Tables**:
   ```sql
   USE [ProAssetinDev]
   GO
   -- Open and execute 01_MasterDatabase_Schema.sql
   ```

3. **Create Infosys Tables**:
   ```sql
   USE [ProAsset_Infosys]
   GO
   -- Open and execute 02_TenantDatabase_Schema.sql
   ```

4. **Create Wipro Tables**:
   ```sql
   USE [ProAsset_Wipro]
   GO
   -- Open and execute 02_TenantDatabase_Schema.sql
   ```

### Option 2: Execute All Scripts at Once

You can combine all scripts in one file:

```sql
-- 1. Create Databases
:r 00_CreateDatabases.sql

-- 2. Master Database Schema
USE [ProAssetinDev]
GO
:r 01_MasterDatabase_Schema.sql

-- 3. Infosys Database Schema
USE [ProAsset_Infosys]
GO
:r 02_TenantDatabase_Schema.sql

-- 4. Wipro Database Schema
USE [ProAsset_Wipro]
GO
:r 02_TenantDatabase_Schema.sql
```

## 📊 Database Structure Overview

```
ProAssetinDev (Master Database)
├── ProAssetinCompanies
├── ProAssetinUsers
├── ProAssetinRoles
├── ProAssetinUserRoles
├── ProAssetinUserClaims
├── ProAssetinUserLogins
├── ProAssetinRoleClaims
├── ProAssetinUserTokens
└── ProAssetinEmployeeConfigurations

ProAsset_Infosys (Infosys Tenant)
├── ProAssetinAssets
├── ProAssetinInventoryLogs
├── ProAssetinInvoices
├── ProAssetinPurchaseOrders
├── ProAssetinTickets
├── ProAssetinVendors
├── ProAssetinSoftware
└── ProAssetinCompanySettings

ProAsset_Wipro (Wipro Tenant)
├── ProAssetinAssets
├── ProAssetinInventoryLogs
├── ProAssetinInvoices
├── ProAssetinPurchaseOrders
├── ProAssetinTickets
├── ProAssetinVendors
├── ProAssetinSoftware
└── ProAssetinCompanySettings
```

## ✅ Verification Queries

After running scripts, verify tables were created:

### Master Database:
```sql
USE [ProAssetinDev]
SELECT name FROM sys.tables WHERE name LIKE 'ProAssetin%' ORDER BY name;
-- Should return 9 tables
```

### Tenant Databases:
```sql
USE [ProAsset_Infosys]
SELECT name FROM sys.tables WHERE name LIKE 'ProAssetin%' ORDER BY name;
-- Should return 8 tables

USE [ProAsset_Wipro]
SELECT name FROM sys.tables WHERE name LIKE 'ProAssetin%' ORDER BY name;
-- Should return 8 tables
```

## 📝 Notes

- All scripts are **idempotent** - safe to run multiple times
- Scripts check for table existence before creating
- Indexes are created automatically
- Foreign keys reference master database users (cross-database)
- All tables use **ProAssetin** prefix (not AspNet)

## 🔧 Troubleshooting

### Error: "Cannot create database"

**Solution**: Check SQL Server permissions and file paths in script.

### Error: "Table already exists"

**Solution**: This is expected - script will skip existing tables.

### Error: "Foreign key constraint"

**Solution**: Ensure master database tables are created before tenant tables.

