-- =============================================
-- Script to Create CompanySettings Table for All Tenant Databases
-- =============================================
-- This script creates the ProAssetinCompanySettings table in all tenant databases
-- (databases with "ProAsset_" prefix)

DECLARE @sql NVARCHAR(MAX);
DECLARE @dbName NVARCHAR(128);
DECLARE @tableName NVARCHAR(128) = 'ProAssetinCompanySettings';

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
    PRINT 'Processing database: ' + @dbName;
    
    -- Build dynamic SQL to create table in current database
    SET @sql = '
    USE [' + @dbName + '];
    
    -- Check if table already exists
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = ''' + @tableName + ''')
    BEGIN
        PRINT ''Creating table in database: ' + @dbName + ''';
        
        CREATE TABLE [dbo].[' + @tableName + '] (
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
            [Currency] NVARCHAR(50) NULL DEFAULT ''USD'',
            [TimeZone] NVARCHAR(50) NULL DEFAULT ''UTC'',
            [DateFormat] NVARCHAR(10) NULL DEFAULT ''MM/dd/yyyy'',
            [TimeFormat] NVARCHAR(10) NULL DEFAULT ''12h'',
            [DefaultPageSize] INT NOT NULL DEFAULT 10,
            [EnableEmailNotifications] BIT NOT NULL DEFAULT 1,
            [EnableSMSNotifications] BIT NOT NULL DEFAULT 0,
            [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT [PK_' + @tableName + '] PRIMARY KEY ([Id]),
            CONSTRAINT [UQ_' + @tableName + '_TenantId] UNIQUE ([TenantId])
        );
        
        CREATE INDEX [IX_' + @tableName + '_TenantId] 
        ON [dbo].[' + @tableName + ']([TenantId]);
        
        PRINT ''Table created successfully in database: ' + @dbName + ''';
    END
    ELSE
    BEGIN
        PRINT ''Table already exists in database: ' + @dbName + ''';
    END
    ';
    
    -- Execute the dynamic SQL
    EXEC sp_executesql @sql;
    
    FETCH NEXT FROM db_cursor INTO @dbName;
END;

CLOSE db_cursor;
DEALLOCATE db_cursor;

PRINT 'Script execution completed.';
PRINT 'All tenant databases have been processed.';

