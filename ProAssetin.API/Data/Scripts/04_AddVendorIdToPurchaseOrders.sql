-- =============================================
-- Script to Add VendorId Column to PurchaseOrders Table
-- =============================================
-- This script adds the VendorId column to the ProAssetinPurchaseOrders table
-- for all tenant databases (databases with "ProAsset_" prefix)

DECLARE @sql NVARCHAR(MAX);
DECLARE @dbName NVARCHAR(128);
DECLARE @tableName NVARCHAR(128) = 'ProAssetinPurchaseOrders';
DECLARE @columnName NVARCHAR(128) = 'VendorId';

-- Cursor to iterate through all tenant databases
-- Exclude master database (ProAssetinDev) as it doesn't have PurchaseOrders table
DECLARE db_cursor CURSOR FOR
SELECT name 
FROM sys.databases 
WHERE name LIKE 'ProAsset_%'
   AND name != 'ProAssetinDev'
   AND state_desc = 'ONLINE';

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @dbName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Processing database: ' + @dbName;
    
    -- Build dynamic SQL to add column in current database
    -- First check if table exists before trying to add column
    SET @sql = '
    USE [' + @dbName + '];
    
    -- Check if table exists
    IF EXISTS (
        SELECT * 
        FROM sys.tables 
        WHERE name = ''' + @tableName + '''
    )
    BEGIN
        -- Check if column already exists
        IF NOT EXISTS (
            SELECT * 
            FROM sys.columns 
            WHERE object_id = OBJECT_ID(''' + @tableName + ''') 
            AND name = ''' + @columnName + '''
        )
        BEGIN
            -- Add VendorId column
            ALTER TABLE [dbo].[' + @tableName + ']
            ADD [' + @columnName + '] INT NULL;
            
            -- Create index on VendorId
            CREATE INDEX [IX_' + @tableName + '_' + @columnName + '] 
            ON [dbo].[' + @tableName + ']([' + @columnName + ']);
            
            PRINT ''Column ' + @columnName + ' added to ' + @tableName + ''';
        END
        ELSE
        BEGIN
            PRINT ''Column ' + @columnName + ' already exists in ' + @tableName + ''';
        END;
    END
    ELSE
    BEGIN
        PRINT ''Table ' + @tableName + ' does not exist in ' + @dbName + ' - skipping'';
    END;
    ';
    
    EXEC sp_executesql @sql;
    
    FETCH NEXT FROM db_cursor INTO @dbName;
END;

CLOSE db_cursor;
DEALLOCATE db_cursor;

PRINT ' ';
PRINT '===============================================';
PRINT 'VendorId Column Addition to PurchaseOrders Complete!';
PRINT '===============================================';
GO
