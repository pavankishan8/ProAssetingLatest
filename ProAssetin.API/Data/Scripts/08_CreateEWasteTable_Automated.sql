-- =============================================
-- Automated: ProAssetinEWasteDisposals on all ProAsset_% tenant DBs
-- =============================================

DECLARE @sql NVARCHAR(MAX);
DECLARE @dbName NVARCHAR(128);
DECLARE @tableName NVARCHAR(128) = N'ProAssetinEWasteDisposals';

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
            [DisposalReference] NVARCHAR(100) NOT NULL,
            [AssetId] INT NULL,
            [ItemDescription] NVARCHAR(500) NOT NULL,
            [Category] NVARCHAR(100) NULL,
            [Quantity] INT NOT NULL DEFAULT 1,
            [EstimatedWeightKg] DECIMAL(18,2) NULL,
            [RecyclerName] NVARCHAR(200) NULL,
            [PickupDate] DATETIME2 NULL,
            [DisposalDate] DATETIME2 NULL,
            [CertificateReference] NVARCHAR(200) NULL,
            [Status] NVARCHAR(50) NOT NULL DEFAULT ' + CHAR(39) + N'Scheduled' + CHAR(39) + N',
            [Notes] NVARCHAR(1000) NULL,
            [TenantId] NVARCHAR(100) NOT NULL,
            [CreatedByUserId] NVARCHAR(450) NULL,
            [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            [UpdatedAt] DATETIME2 NULL,
            CONSTRAINT [PK_' + @tableName + N'] PRIMARY KEY ([Id]),
            CONSTRAINT [FK_' + @tableName + N'_Assets] FOREIGN KEY ([AssetId]) REFERENCES [dbo].[ProAssetinAssets]([Id]) ON DELETE SET NULL
        );

        CREATE UNIQUE INDEX [IX_' + @tableName + N'_RefTenant] ON [dbo].[' + @tableName + N']([DisposalReference], [TenantId]);
        CREATE INDEX [IX_' + @tableName + N'_TenantId] ON [dbo].[' + @tableName + N']([TenantId]);
        CREATE INDEX [IX_' + @tableName + N'_Status] ON [dbo].[' + @tableName + N']([Status]);
        CREATE INDEX [IX_' + @tableName + N'_DisposalDate] ON [dbo].[' + @tableName + N']([DisposalDate]);
        CREATE INDEX [IX_' + @tableName + N'_AssetId] ON [dbo].[' + @tableName + N']([AssetId]);
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
PRINT N'E-waste table creation complete for all tenants.';
PRINT N'===============================================';
GO
