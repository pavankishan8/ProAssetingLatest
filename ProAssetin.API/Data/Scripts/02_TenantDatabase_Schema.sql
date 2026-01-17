-- =============================================
-- ProAssetin Tenant Database Schema
-- Databases: ProAsset_Infosys, ProAsset_Wipro
-- Purpose: Customer-specific business data
-- =============================================
-- Usage: Run this script for EACH tenant database
-- Example: USE [ProAsset_Infosys] GO, then run script
-- Example: USE [ProAsset_Wipro] GO, then run script
-- =============================================

PRINT '===============================================';
PRINT 'Creating Tenant Database Schema';
PRINT 'Database: ' + DB_NAME();
PRINT '===============================================';
GO

-- =============================================
-- Table 1: ProAssetinAssets
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinAssets]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinAssets] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [AssetId] NVARCHAR(100) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL DEFAULT '',
        [Location] NVARCHAR(50) NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Available',
        [Make] NVARCHAR(100) NULL,
        [Model] NVARCHAR(100) NULL,
        [SerialNumber] NVARCHAR(100) NULL,
        [PurchasePrice] DECIMAL(18,2) NULL,
        [PurchaseDate] DATETIME2 NULL,
        [WarrantyExpiryDate] DATETIME2 NULL,
        [Description] NVARCHAR(200) NULL,
        [AssignedToUserId] NVARCHAR(450) NULL,
        [TenantId] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ProAssetinAssets] PRIMARY KEY ([Id])
    );
    
    CREATE UNIQUE INDEX [IX_ProAssetinAssets_AssetId_TenantId] ON [dbo].[ProAssetinAssets]([AssetId], [TenantId]);
    CREATE INDEX [IX_ProAssetinAssets_TenantId] ON [dbo].[ProAssetinAssets]([TenantId]);
    CREATE INDEX [IX_ProAssetinAssets_Status] ON [dbo].[ProAssetinAssets]([Status]);
    CREATE INDEX [IX_ProAssetinAssets_Category] ON [dbo].[ProAssetinAssets]([Category]);
    CREATE INDEX [IX_ProAssetinAssets_AssignedToUserId] ON [dbo].[ProAssetinAssets]([AssignedToUserId]);
    PRINT '✓ Table created: ProAssetinAssets';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinAssets';
END
GO

-- =============================================
-- Table 2: ProAssetinInventoryLogs
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinInventoryLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinInventoryLogs] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [AssetId] INT NOT NULL,
        [Action] NVARCHAR(50) NOT NULL,
        [PerformedByUserId] NVARCHAR(450) NULL,
        [Quantity] DECIMAL(18,2) NULL,
        [Notes] NVARCHAR(500) NULL,
        [TenantId] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_ProAssetinInventoryLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProAssetinInventoryLogs_ProAssetinAssets] FOREIGN KEY ([AssetId]) 
            REFERENCES [dbo].[ProAssetinAssets]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ProAssetinInventoryLogs_AssetId] ON [dbo].[ProAssetinInventoryLogs]([AssetId]);
    CREATE INDEX [IX_ProAssetinInventoryLogs_TenantId] ON [dbo].[ProAssetinInventoryLogs]([TenantId]);
    CREATE INDEX [IX_ProAssetinInventoryLogs_CreatedAt] ON [dbo].[ProAssetinInventoryLogs]([CreatedAt]);
    CREATE INDEX [IX_ProAssetinInventoryLogs_PerformedByUserId] ON [dbo].[ProAssetinInventoryLogs]([PerformedByUserId]);
    PRINT '✓ Table created: ProAssetinInventoryLogs';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinInventoryLogs';
END
GO

-- =============================================
-- Table 3: ProAssetinInvoices
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinInvoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinInvoices] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [InvoiceNumber] NVARCHAR(100) NOT NULL,
        [VendorName] NVARCHAR(200) NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [InvoiceDate] DATETIME2 NOT NULL,
        [DueDate] DATETIME2 NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [Description] NVARCHAR(500) NULL,
        [PurchaseOrderNumber] NVARCHAR(100) NULL,
        [TenantId] NVARCHAR(100) NOT NULL,
        [CreatedByUserId] NVARCHAR(450) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ProAssetinInvoices] PRIMARY KEY ([Id])
    );
    
    CREATE UNIQUE INDEX [IX_ProAssetinInvoices_InvoiceNumber] ON [dbo].[ProAssetinInvoices]([InvoiceNumber]);
    CREATE INDEX [IX_ProAssetinInvoices_TenantId] ON [dbo].[ProAssetinInvoices]([TenantId]);
    CREATE INDEX [IX_ProAssetinInvoices_Status] ON [dbo].[ProAssetinInvoices]([Status]);
    CREATE INDEX [IX_ProAssetinInvoices_InvoiceDate] ON [dbo].[ProAssetinInvoices]([InvoiceDate]);
    CREATE INDEX [IX_ProAssetinInvoices_CreatedByUserId] ON [dbo].[ProAssetinInvoices]([CreatedByUserId]);
    PRINT '✓ Table created: ProAssetinInvoices';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinInvoices';
END
GO

-- =============================================
-- Table 4: ProAssetinPurchaseOrders
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinPurchaseOrders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinPurchaseOrders] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [PONumber] NVARCHAR(100) NOT NULL,
        [VendorId] INT NULL,
        [VendorName] NVARCHAR(200) NULL,
        [TotalAmount] DECIMAL(18,2) NOT NULL,
        [PODate] DATETIME2 NOT NULL,
        [ExpectedDeliveryDate] DATETIME2 NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Draft',
        [Description] NVARCHAR(500) NULL,
        [TenantId] NVARCHAR(100) NOT NULL,
        [CreatedByUserId] NVARCHAR(450) NULL,
        [ApprovedByUserId] NVARCHAR(450) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ProAssetinPurchaseOrders] PRIMARY KEY ([Id])
    );
    
    CREATE UNIQUE INDEX [IX_ProAssetinPurchaseOrders_PONumber] ON [dbo].[ProAssetinPurchaseOrders]([PONumber]);
    CREATE INDEX [IX_ProAssetinPurchaseOrders_TenantId] ON [dbo].[ProAssetinPurchaseOrders]([TenantId]);
    CREATE INDEX [IX_ProAssetinPurchaseOrders_Status] ON [dbo].[ProAssetinPurchaseOrders]([Status]);
    CREATE INDEX [IX_ProAssetinPurchaseOrders_CreatedByUserId] ON [dbo].[ProAssetinPurchaseOrders]([CreatedByUserId]);
    CREATE INDEX [IX_ProAssetinPurchaseOrders_PODate] ON [dbo].[ProAssetinPurchaseOrders]([PODate]);
    CREATE INDEX [IX_ProAssetinPurchaseOrders_VendorId] ON [dbo].[ProAssetinPurchaseOrders]([VendorId]);
    PRINT '✓ Table created: ProAssetinPurchaseOrders';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinPurchaseOrders';
END
GO

-- =============================================
-- Table 5: ProAssetinTickets
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinTickets]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinTickets] (
        [TaskID] INT IDENTITY(1,1) NOT NULL,
        [TaskTitle] NVARCHAR(500) NOT NULL,
        [TaskAssignedToID] NVARCHAR(450) NULL,
        [TaskAssignedToName] NVARCHAR(200) NULL,
        [TaskState] NVARCHAR(50) NOT NULL DEFAULT 'Open',
        [Priority] NVARCHAR(50) NULL,
        [Description] NVARCHAR(2000) NULL,
        [Resolution] NVARCHAR(2000) NULL,
        [TenantId] NVARCHAR(100) NOT NULL,
        [CreatedByUserId] NVARCHAR(450) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [ResolvedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ProAssetinTickets] PRIMARY KEY ([TaskID])
    );
    
    CREATE INDEX [IX_ProAssetinTickets_TenantId] ON [dbo].[ProAssetinTickets]([TenantId]);
    CREATE INDEX [IX_ProAssetinTickets_TaskState] ON [dbo].[ProAssetinTickets]([TaskState]);
    CREATE INDEX [IX_ProAssetinTickets_TaskAssignedToID] ON [dbo].[ProAssetinTickets]([TaskAssignedToID]);
    CREATE INDEX [IX_ProAssetinTickets_Priority] ON [dbo].[ProAssetinTickets]([Priority]);
    CREATE INDEX [IX_ProAssetinTickets_CreatedAt] ON [dbo].[ProAssetinTickets]([CreatedAt]);
    PRINT '✓ Table created: ProAssetinTickets';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinTickets';
END
GO

-- =============================================
-- Table 6: ProAssetinVendors
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinVendors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinVendors] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [VendorName] NVARCHAR(200) NOT NULL,
        [ContactPerson] NVARCHAR(200) NULL,
        [Email] NVARCHAR(100) NULL,
        [PhoneNumber] NVARCHAR(50) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(100) NULL,
        [State] NVARCHAR(50) NULL,
        [Country] NVARCHAR(50) NULL,
        [GSTNumber] NVARCHAR(50) NULL,
        [TaxID] NVARCHAR(50) NULL,
        [TenantId] NVARCHAR(100) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ProAssetinVendors] PRIMARY KEY ([Id])
    );
    
    CREATE INDEX [IX_ProAssetinVendors_TenantId] ON [dbo].[ProAssetinVendors]([TenantId]);
    CREATE INDEX [IX_ProAssetinVendors_VendorName] ON [dbo].[ProAssetinVendors]([VendorName]);
    CREATE INDEX [IX_ProAssetinVendors_IsActive] ON [dbo].[ProAssetinVendors]([IsActive]);
    PRINT '✓ Table created: ProAssetinVendors';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinVendors';
END
GO

-- =============================================
-- Table 7: ProAssetinCompanySettings
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinCompanySettings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinCompanySettings] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [TenantId] NVARCHAR(100) NOT NULL,
        [CompanyLogo] NVARCHAR(MAX) NULL,
        [CompanyLogoMimeType] NVARCHAR(50) NULL,
        [CompanyName] NVARCHAR(200) NOT NULL,
        [Address] NVARCHAR(500) NULL,
        [PhoneNumber] NVARCHAR(50) NULL,
        [Email] NVARCHAR(100) NULL,
        [Industry] NVARCHAR(100) NULL,
        [SPOCInformation] NVARCHAR(200) NULL,
        [GSTNumber] NVARCHAR(50) NULL,
        [Website] NVARCHAR(200) NULL,
        [Currency] NVARCHAR(50) NULL DEFAULT 'USD',
        [TimeZone] NVARCHAR(50) NULL DEFAULT 'UTC',
        [DateFormat] NVARCHAR(10) NULL DEFAULT 'MM/dd/yyyy',
        [TimeFormat] NVARCHAR(10) NULL DEFAULT '12h',
        [DefaultPageSize] INT NOT NULL DEFAULT 10,
        [EnableEmailNotifications] BIT NOT NULL DEFAULT 1,
        [EnableSMSNotifications] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_ProAssetinCompanySettings] PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_ProAssetinCompanySettings_TenantId] UNIQUE ([TenantId])
    );
    
    CREATE INDEX [IX_ProAssetinCompanySettings_TenantId] ON [dbo].[ProAssetinCompanySettings]([TenantId]);
    PRINT '✓ Table created: ProAssetinCompanySettings';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinCompanySettings';
END
GO

-- =============================================
-- Table 8: ProAssetinSoftware
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinSoftware]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinSoftware] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [SoftwareName] NVARCHAR(200) NOT NULL,
        [Version] NVARCHAR(50) NULL,
        [LicenseType] NVARCHAR(100) NULL,
        [LicenseKey] NVARCHAR(100) NULL,
        [VendorId] INT NULL,
        [VendorName] NVARCHAR(200) NULL,
        [PurchasePrice] DECIMAL(18,2) NULL,
        [PurchaseDate] DATETIME2 NULL,
        [LicenseExpiryDate] DATETIME2 NULL,
        [TotalLicenses] INT NULL,
        [UsedLicenses] INT NULL,
        [AvailableLicenses] INT NULL,
        [Description] NVARCHAR(500) NULL,
        [InstallationPath] NVARCHAR(200) NULL,
        [Category] NVARCHAR(100) NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
        [TenantId] NVARCHAR(100) NOT NULL,
        [PurchasedByUserId] NVARCHAR(450) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ProAssetinSoftware] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProAssetinSoftware_ProAssetinVendors_VendorId] FOREIGN KEY ([VendorId]) REFERENCES [dbo].[ProAssetinVendors] ([Id]) ON DELETE SET NULL
    );
    
    CREATE INDEX [IX_ProAssetinSoftware_TenantId] ON [dbo].[ProAssetinSoftware]([TenantId]);
    CREATE INDEX [IX_ProAssetinSoftware_SoftwareName] ON [dbo].[ProAssetinSoftware]([SoftwareName]);
    CREATE INDEX [IX_ProAssetinSoftware_Status] ON [dbo].[ProAssetinSoftware]([Status]);
    CREATE INDEX [IX_ProAssetinSoftware_Category] ON [dbo].[ProAssetinSoftware]([Category]);
    CREATE INDEX [IX_ProAssetinSoftware_VendorId] ON [dbo].[ProAssetinSoftware]([VendorId]);
    PRINT '✓ Table created: ProAssetinSoftware';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinSoftware';
END
GO

PRINT '';
PRINT '===============================================';
PRINT 'Tenant Database Schema Creation Complete!';
PRINT 'Total Tables Created: 8';
PRINT 'Database: ' + DB_NAME();
PRINT '===============================================';
GO

