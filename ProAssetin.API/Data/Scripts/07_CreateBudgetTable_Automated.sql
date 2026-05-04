-- =============================================
-- Automated Script: ProAssetinBudgets in all tenant DBs (ProAsset_%)
-- =============================================
-- Uses CHAR(39) for the Status default literal to avoid nested-quote errors
-- when building dynamic SQL.

DECLARE @sql NVARCHAR(MAX);
DECLARE @dbName NVARCHAR(128);
DECLARE @tableName NVARCHAR(128) = N'ProAssetinBudgets';

DECLARE db_cursor CURSOR FOR
SELECT name 
FROM sys.databases 
WHERE name LIKE N'ProAsset_%'
   AND name != N'ProAssetinDev'
   AND state_desc = N'ONLINE';

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @dbName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT N'Processing database: ' + @dbName;

    SET @sql = N'
    USE [' + @dbName + N'];

    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = ''' + @tableName + N''')
    BEGIN
        CREATE TABLE [dbo].[' + @tableName + N'] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [Name] NVARCHAR(200) NOT NULL,
            [Description] NVARCHAR(500) NULL,
            [FiscalYear] INT NOT NULL,
            [Category] NVARCHAR(100) NULL,
            [AllocatedAmount] DECIMAL(18,2) NOT NULL,
            [SpentAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
            [Status] NVARCHAR(50) NOT NULL DEFAULT ' + CHAR(39) + N'Active' + CHAR(39) + N',
            [TenantId] NVARCHAR(100) NOT NULL,
            [CreatedByUserId] NVARCHAR(450) NULL,
            [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            [UpdatedAt] DATETIME2 NULL,
            CONSTRAINT [PK_' + @tableName + N'] PRIMARY KEY ([Id])
        );

        CREATE INDEX [IX_' + @tableName + N'_TenantId] ON [dbo].[' + @tableName + N']([TenantId]);
        CREATE INDEX [IX_' + @tableName + N'_FiscalYear] ON [dbo].[' + @tableName + N']([FiscalYear]);
        CREATE INDEX [IX_' + @tableName + N'_Status] ON [dbo].[' + @tableName + N']([Status]);
        CREATE INDEX [IX_' + @tableName + N'_CreatedByUserId] ON [dbo].[' + @tableName + N']([CreatedByUserId]);
    END;
    ';

    EXEC sp_executesql @sql;

    FETCH NEXT FROM db_cursor INTO @dbName;
END;

CLOSE db_cursor;
DEALLOCATE db_cursor;

PRINT N'';
PRINT N'===============================================';
PRINT N'Budget table creation complete for all tenants.';
PRINT N'===============================================';
GO
