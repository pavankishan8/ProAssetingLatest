-- =============================================
-- Simple Script to Add VendorId Column to PurchaseOrders Table
-- =============================================
-- Run this script on EACH tenant database (ProAsset_Infosys, ProAsset_Wipro, etc.)
-- This is a simpler alternative to the automated script

-- STEP 1: Run on ProAsset_Infosys database
-- =============================================
USE [ProAsset_Infosys];
GO

-- Check if column already exists
IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('ProAssetinPurchaseOrders') 
    AND name = 'VendorId'
)
BEGIN
    -- Add VendorId column
    ALTER TABLE [dbo].[ProAssetinPurchaseOrders]
    ADD [VendorId] INT NULL;
    
    -- Create index on VendorId
    CREATE INDEX [IX_ProAssetinPurchaseOrders_VendorId] 
    ON [dbo].[ProAssetinPurchaseOrders]([VendorId]);
    
    PRINT 'VendorId column added to ProAssetinPurchaseOrders in ProAsset_Infosys';
END
ELSE
BEGIN
    PRINT 'VendorId column already exists in ProAssetinPurchaseOrders (ProAsset_Infosys)';
END
GO

-- STEP 2: Run on ProAsset_Wipro database
-- =============================================
USE [ProAsset_Wipro];
GO

-- Check if column already exists
IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('ProAssetinPurchaseOrders') 
    AND name = 'VendorId'
)
BEGIN
    -- Add VendorId column
    ALTER TABLE [dbo].[ProAssetinPurchaseOrders]
    ADD [VendorId] INT NULL;
    
    -- Create index on VendorId
    CREATE INDEX [IX_ProAssetinPurchaseOrders_VendorId] 
    ON [dbo].[ProAssetinPurchaseOrders]([VendorId]);
    
    PRINT 'VendorId column added to ProAssetinPurchaseOrders in ProAsset_Wipro';
END
ELSE
BEGIN
    PRINT 'VendorId column already exists in ProAssetinPurchaseOrders (ProAsset_Wipro)';
END
GO

-- =============================================
-- NOTES:
-- =============================================
-- If you have additional tenant databases, repeat the above pattern:
-- 1. USE [DatabaseName];
-- 2. Check if column exists
-- 3. ALTER TABLE to add column
-- 4. CREATE INDEX on the new column
-- =============================================

