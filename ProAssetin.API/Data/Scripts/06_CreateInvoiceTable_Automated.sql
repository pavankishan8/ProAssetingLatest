-- =============================================
-- Automated Script to Create Invoice Table for All Tenant Databases
-- =============================================
-- This script creates the ProAssetinInvoices table in all tenant databases
-- (databases with "ProAsset_" prefix)

DECLARE @sql NVARCHAR(MAX);
DECLARE @dbName NVARCHAR(128);
DECLARE @tableName NVARCHAR(128) = 'ProAssetinInvoices';

-- Cursor to iterate through all tenant databases
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
    
    -- Build dynamic SQL to create table in current database
    SET @sql = '
    USE [' + @dbName + '];
    
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = ''' + @tableName + ''')
    BEGIN
        PRINT ''Creating ' + @tableName + ' table in database: ' + @dbName + '';
        
        CREATE TABLE [dbo].[' + @tableName + '] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [InvoiceNumber] NVARCHAR(100) NOT NULL,
            [VendorName] NVARCHAR(200) NULL,
            [Amount] DECIMAL(18,2) NOT NULL,
            [InvoiceDate] DATETIME2 NOT NULL,
            [DueDate] DATETIME2 NULL,
            [Status] NVARCHAR(50) NOT NULL DEFAULT ''Pending'',
            [Description] NVARCHAR(500) NULL,
            [PurchaseOrderNumber] NVARCHAR(100) NULL,
            [TenantId] NVARCHAR(100) NOT NULL,
            [CreatedByUserId] NVARCHAR(450) NULL,
            [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            [UpdatedAt] DATETIME2 NULL,
            CONSTRAINT [PK_' + @tableName + '] PRIMARY KEY ([Id])
        );
        
        CREATE UNIQUE INDEX [IX_' + @tableName + '_InvoiceNumber] ON [dbo].[' + @tableName + ']([InvoiceNumber]);
        CREATE INDEX [IX_' + @tableName + '_TenantId] ON [dbo].[' + @tableName + ']([TenantId]);
        CREATE INDEX [IX_' + @tableName + '_Status] ON [dbo].[' + @tableName + ']([Status]);
        CREATE INDEX [IX_' + @tableName + '_InvoiceDate] ON [dbo].[' + @tableName + ']([InvoiceDate]);
        CREATE INDEX [IX_' + @tableName + '_CreatedByUserId] ON [dbo].[' + @tableName + ']([CreatedByUserId]);
        
        PRINT ''Table ' + @tableName + ' created in ' + @dbName + '';
    END
    ELSE
    BEGIN
        PRINT ''Table ' + @tableName + ' already exists in ' + @dbName + '';
    END;
    ';
    
    EXEC sp_executesql @sql;
    
    FETCH NEXT FROM db_cursor INTO @dbName;
END;

CLOSE db_cursor;
DEALLOCATE db_cursor;

PRINT '';
PRINT '===============================================';
PRINT 'Invoice Table Creation Complete for All Tenants!';
PRINT '===============================================';
GO

