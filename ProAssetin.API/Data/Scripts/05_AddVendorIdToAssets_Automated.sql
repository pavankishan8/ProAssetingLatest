-- =============================================
-- Add VendorId column to ProAssetinAssets table (AUTOMATED)
-- =============================================
-- Purpose: Add VendorId foreign key to Assets table in ALL tenant databases
-- Usage: Run this script from master database - it will process all tenant databases automatically
-- Example: Open SQL Server Management Studio, connect, and execute this script
-- =============================================
-- This script finds all databases with "ProAsset_" prefix and adds VendorId column to each

DECLARE @sql NVARCHAR(MAX) = '';
DECLARE @dbName NVARCHAR(255);

-- Cursor to iterate through all tenant databases
DECLARE db_cursor CURSOR FOR
SELECT name 
FROM sys.databases 
WHERE name LIKE 'ProAsset_%'
AND state_desc = 'ONLINE';

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @dbName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '===============================================';
    PRINT 'Processing database: ' + @dbName;
    PRINT '===============================================';
    
    -- Build dynamic SQL for each database
    SET @sql = N'
    USE [' + @dbName + N'];
    GO
    
    -- Check if VendorId column already exists
    IF NOT EXISTS (SELECT * FROM sys.columns 
                   WHERE object_id = OBJECT_ID(N''[dbo].[ProAssetinAssets]'') 
                   AND name = ''VendorId'')
    BEGIN
        -- Add VendorId column
        ALTER TABLE [dbo].[ProAssetinAssets]
        ADD [VendorId] INT NULL;
        
        -- Add foreign key constraint (only if Vendors table exists)
        IF EXISTS (SELECT * FROM sys.tables WHERE name = ''ProAssetinVendors'')
        BEGIN
            ALTER TABLE [dbo].[ProAssetinAssets]
            ADD CONSTRAINT [FK_ProAssetinAssets_ProAssetinVendors_VendorId_' + @dbName + N'] 
            FOREIGN KEY ([VendorId]) 
            REFERENCES [dbo].[ProAssetinVendors]([Id]) 
            ON DELETE SET NULL;
        END
        
        -- Add index for better query performance
        CREATE INDEX [IX_ProAssetinAssets_VendorId] 
        ON [dbo].[ProAssetinAssets]([VendorId]);
        
        PRINT ''✓ VendorId column added to ProAssetinAssets in database: ' + @dbName + N''';
    END
    ELSE
    BEGIN
        PRINT ''⚠ VendorId column already exists in ProAssetinAssets in database: ' + @dbName + N''';
    END
    GO
    ';
    
    -- Execute the dynamic SQL
    EXEC sp_executesql @sql;
    
    FETCH NEXT FROM db_cursor INTO @dbName;
END;

CLOSE db_cursor;
DEALLOCATE db_cursor;

PRINT '===============================================';
PRINT 'Script completed - processed all tenant databases';
PRINT '===============================================';

