-- =============================================
-- Automated: ProAssetinSecurityIncidents on all ProAsset_% tenant DBs
-- =============================================

DECLARE @sql NVARCHAR(MAX);
DECLARE @dbName NVARCHAR(128);
DECLARE @tableName NVARCHAR(128) = N'ProAssetinSecurityIncidents';

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
            [Id] INT IDENTITY(1,1) NOT NULL,
            [IncidentReference] NVARCHAR(100) NOT NULL,
            [Title] NVARCHAR(200) NOT NULL,
            [Description] NVARCHAR(2000) NULL,
            [Category] NVARCHAR(100) NULL,
            [Severity] NVARCHAR(50) NOT NULL DEFAULT ' + CHAR(39) + N'Medium' + CHAR(39) + N',
            [Status] NVARCHAR(50) NOT NULL DEFAULT ' + CHAR(39) + N'Open' + CHAR(39) + N',
            [ReportedDate] DATETIME2 NOT NULL,
            [ResolvedDate] DATETIME2 NULL,
            [AffectedSystem] NVARCHAR(300) NULL,
            [AssignedToName] NVARCHAR(200) NULL,
            [Notes] NVARCHAR(1000) NULL,
            [TenantId] NVARCHAR(100) NOT NULL,
            [CreatedByUserId] NVARCHAR(450) NULL,
            [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            [UpdatedAt] DATETIME2 NULL,
            CONSTRAINT [PK_' + @tableName + N'] PRIMARY KEY ([Id])
        );

        CREATE UNIQUE INDEX [IX_' + @tableName + N'_RefTenant] ON [dbo].[' + @tableName + N']([IncidentReference], [TenantId]);
        CREATE INDEX [IX_' + @tableName + N'_TenantId] ON [dbo].[' + @tableName + N']([TenantId]);
        CREATE INDEX [IX_' + @tableName + N'_Status] ON [dbo].[' + @tableName + N']([Status]);
        CREATE INDEX [IX_' + @tableName + N'_Severity] ON [dbo].[' + @tableName + N']([Severity]);
        CREATE INDEX [IX_' + @tableName + N'_ReportedDate] ON [dbo].[' + @tableName + N']([ReportedDate]);
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
PRINT N'Security incidents table creation complete.';
PRINT N'===============================================';
GO
