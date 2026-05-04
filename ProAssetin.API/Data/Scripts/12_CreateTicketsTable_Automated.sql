-- =============================================
-- Automated: ProAssetinTickets on all ProAsset_% tenant DBs (if missing)
-- =============================================

DECLARE @sql NVARCHAR(MAX);
DECLARE @dbName NVARCHAR(128);
DECLARE @tableName NVARCHAR(128) = N'ProAssetinTickets';

DECLARE db_cursor CURSOR FOR
SELECT name FROM sys.databases
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
            [TaskID] INT IDENTITY(1,1) NOT NULL,
            [TaskTitle] NVARCHAR(500) NOT NULL,
            [TaskAssignedToID] NVARCHAR(450) NULL,
            [TaskAssignedToName] NVARCHAR(200) NULL,
            [TaskState] NVARCHAR(50) NOT NULL DEFAULT ' + CHAR(39) + N'Open' + CHAR(39) + N',
            [Priority] NVARCHAR(50) NULL,
            [Description] NVARCHAR(2000) NULL,
            [Resolution] NVARCHAR(2000) NULL,
            [TenantId] NVARCHAR(100) NOT NULL,
            [CreatedByUserId] NVARCHAR(450) NULL,
            [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            [UpdatedAt] DATETIME2 NULL,
            [ResolvedAt] DATETIME2 NULL,
            CONSTRAINT [PK_' + @tableName + N'] PRIMARY KEY ([TaskID])
        );

        CREATE INDEX [IX_' + @tableName + N'_TenantId] ON [dbo].[' + @tableName + N']([TenantId]);
        CREATE INDEX [IX_' + @tableName + N'_TaskState] ON [dbo].[' + @tableName + N']([TaskState]);
        CREATE INDEX [IX_' + @tableName + N'_TaskAssignedToID] ON [dbo].[' + @tableName + N']([TaskAssignedToID]);
        CREATE INDEX [IX_' + @tableName + N'_Priority] ON [dbo].[' + @tableName + N']([Priority]);
        CREATE INDEX [IX_' + @tableName + N'_CreatedAt] ON [dbo].[' + @tableName + N']([CreatedAt]);
    END;
    ';

    EXEC sp_executesql @sql;

    FETCH NEXT FROM db_cursor INTO @dbName;
END;

CLOSE db_cursor;
DEALLOCATE db_cursor;

PRINT N'';
PRINT N'===============================================';
PRINT N'Tickets table check complete for all tenants.';
PRINT N'===============================================';
GO
