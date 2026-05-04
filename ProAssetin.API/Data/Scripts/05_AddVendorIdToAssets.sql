-- =============================================
-- Add VendorId column to ProAssetinAssets table
-- =============================================
-- Purpose: Add VendorId foreign key to Assets table
-- Usage: Run this script for EACH tenant database
-- Example: USE [ProAsset_Infosys] GO, then run script
-- Example: USE [ProAsset_Wipro] GO, then run script
-- =============================================

PRINT '===============================================';
PRINT 'Adding VendorId column to ProAssetinAssets';
PRINT 'Database: ' + DB_NAME();
PRINT '===============================================';
GO

-- Check if VendorId column already exists
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinAssets]') 
               AND name = 'VendorId')
BEGIN
    -- Add VendorId column
    ALTER TABLE [dbo].[ProAssetinAssets]
    ADD [VendorId] INT NULL;
    
    -- Add foreign key constraint
    ALTER TABLE [dbo].[ProAssetinAssets]
    ADD CONSTRAINT [FK_ProAssetinAssets_ProAssetinVendors_VendorId] 
    FOREIGN KEY ([VendorId]) 
    REFERENCES [dbo].[ProAssetinVendors]([Id]) 
    ON DELETE SET NULL;
    
    -- Add index for better query performance
    CREATE INDEX [IX_ProAssetinAssets_VendorId] 
    ON [dbo].[ProAssetinAssets]([VendorId]);
    
    PRINT '✓ VendorId column added to ProAssetinAssets';
END
ELSE
BEGIN
    PRINT '⚠ VendorId column already exists in ProAssetinAssets';
END
GO

PRINT '===============================================';
PRINT 'Script completed';
PRINT '===============================================';

