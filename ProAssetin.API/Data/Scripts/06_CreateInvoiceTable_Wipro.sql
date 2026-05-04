-- =============================================
-- Script to Create Invoice Table for Wipro
-- =============================================
-- This script creates the ProAssetinInvoices table in ProAsset_Wipro database
-- =============================================

USE [ProAsset_Wipro]
GO

-- =============================================
-- Table: ProAssetinInvoices
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
    
    -- Create indexes
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

PRINT '';
PRINT '===============================================';
PRINT 'Invoice Table Creation Complete!';
PRINT 'Database: ' + DB_NAME();
PRINT '===============================================';
GO

