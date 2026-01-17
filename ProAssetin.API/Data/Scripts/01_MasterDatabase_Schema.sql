-- =============================================
-- ProAssetin Master Database Schema
-- Database: ProAssetinDev
-- Purpose: Authentication and User Management
-- =============================================

USE [ProAssetinDev]
GO

PRINT '===============================================';
PRINT 'Creating Master Database Schema';
PRINT 'Database: ProAssetinDev';
PRINT '===============================================';
GO

-- =============================================
-- Table 1: ProAssetinCompanies
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinCompanies]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinCompanies] (
        [CompanyID] NVARCHAR(50) NOT NULL,
        [CompanyName] NVARCHAR(200) NOT NULL,
        [Address] NVARCHAR(500) NULL,
        [PhoneNumber] NVARCHAR(50) NULL,
        [Industry] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_ProAssetinCompanies] PRIMARY KEY ([CompanyID])
    );
    
    CREATE INDEX [IX_ProAssetinCompanies_CompanyName] ON [dbo].[ProAssetinCompanies]([CompanyName]);
    PRINT '✓ Table created: ProAssetinCompanies';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinCompanies';
END
GO

-- =============================================
-- Table 2: ProAssetinUsers
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinUsers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinUsers] (
        [Id] NVARCHAR(450) NOT NULL,
        [UserName] NVARCHAR(256) NULL,
        [NormalizedUserName] NVARCHAR(256) NULL,
        [Email] NVARCHAR(256) NULL,
        [NormalizedEmail] NVARCHAR(256) NULL,
        [EmailConfirmed] BIT NOT NULL DEFAULT 0,
        [PasswordHash] NVARCHAR(MAX) NULL,
        [SecurityStamp] NVARCHAR(MAX) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        [PhoneNumber] NVARCHAR(MAX) NULL,
        [PhoneNumberConfirmed] BIT NOT NULL DEFAULT 0,
        [TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
        [LockoutEnd] DATETIMEOFFSET NULL,
        [LockoutEnabled] BIT NOT NULL DEFAULT 1,
        [AccessFailedCount] INT NOT NULL DEFAULT 0,
        -- Custom fields
        [FirstName] NVARCHAR(100) NOT NULL DEFAULT '',
        [LastName] NVARCHAR(100) NOT NULL DEFAULT '',
        [TenantId] NVARCHAR(100) NULL,
        [CompanyID] NVARCHAR(50) NULL,
        [DomainAccount] NVARCHAR(100) NULL,
        [EmployeeType] NVARCHAR(50) NULL,
        [Location] NVARCHAR(100) NULL,
        [ProjectName] NVARCHAR(100) NULL,
        [Team] NVARCHAR(100) NULL,
        [CustomerName] NVARCHAR(100) NULL,
        [WorkType] NVARCHAR(50) NULL,
        [ReportingManager] NVARCHAR(100) NULL,
        [RegisterDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastLoginAt] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_ProAssetinUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProAssetinUsers_ProAssetinCompanies] FOREIGN KEY ([CompanyID]) 
            REFERENCES [dbo].[ProAssetinCompanies]([CompanyID]) ON DELETE SET NULL
    );
    
    CREATE INDEX [EmailIndex] ON [dbo].[ProAssetinUsers]([NormalizedEmail]);
    CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[ProAssetinUsers]([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
    CREATE INDEX [IX_ProAssetinUsers_TenantId] ON [dbo].[ProAssetinUsers]([TenantId]);
    CREATE INDEX [IX_ProAssetinUsers_CompanyID] ON [dbo].[ProAssetinUsers]([CompanyID]);
    CREATE INDEX [IX_ProAssetinUsers_Email] ON [dbo].[ProAssetinUsers]([Email]);
    PRINT '✓ Table created: ProAssetinUsers';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinUsers';
END
GO

-- =============================================
-- Table 3: ProAssetinRoles
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinRoles] (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NULL,
        [NormalizedName] NVARCHAR(256) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_ProAssetinRoles] PRIMARY KEY ([Id])
    );
    
    CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[ProAssetinRoles]([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
    PRINT '✓ Table created: ProAssetinRoles';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinRoles';
END
GO

-- =============================================
-- Table 4: ProAssetinUserRoles
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinUserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinUserRoles] (
        [UserId] NVARCHAR(450) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        CONSTRAINT [PK_ProAssetinUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_ProAssetinUserRoles_ProAssetinUsers] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[ProAssetinUsers]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProAssetinUserRoles_ProAssetinRoles] FOREIGN KEY ([RoleId]) 
            REFERENCES [dbo].[ProAssetinRoles]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ProAssetinUserRoles_RoleId] ON [dbo].[ProAssetinUserRoles]([RoleId]);
    PRINT '✓ Table created: ProAssetinUserRoles';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinUserRoles';
END
GO

-- =============================================
-- Table 5: ProAssetinUserClaims
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinUserClaims]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinUserClaims] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [ClaimType] NVARCHAR(MAX) NULL,
        [ClaimValue] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_ProAssetinUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProAssetinUserClaims_ProAssetinUsers] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[ProAssetinUsers]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ProAssetinUserClaims_UserId] ON [dbo].[ProAssetinUserClaims]([UserId]);
    PRINT '✓ Table created: ProAssetinUserClaims';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinUserClaims';
END
GO

-- =============================================
-- Table 6: ProAssetinUserLogins
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinUserLogins]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinUserLogins] (
        [LoginProvider] NVARCHAR(128) NOT NULL,
        [ProviderKey] NVARCHAR(128) NOT NULL,
        [ProviderDisplayName] NVARCHAR(MAX) NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        CONSTRAINT [PK_ProAssetinUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_ProAssetinUserLogins_ProAssetinUsers] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[ProAssetinUsers]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ProAssetinUserLogins_UserId] ON [dbo].[ProAssetinUserLogins]([UserId]);
    PRINT '✓ Table created: ProAssetinUserLogins';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinUserLogins';
END
GO

-- =============================================
-- Table 7: ProAssetinRoleClaims
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinRoleClaims]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinRoleClaims] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        [ClaimType] NVARCHAR(MAX) NULL,
        [ClaimValue] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_ProAssetinRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProAssetinRoleClaims_ProAssetinRoles] FOREIGN KEY ([RoleId]) 
            REFERENCES [dbo].[ProAssetinRoles]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ProAssetinRoleClaims_RoleId] ON [dbo].[ProAssetinRoleClaims]([RoleId]);
    PRINT '✓ Table created: ProAssetinRoleClaims';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinRoleClaims';
END
GO

-- =============================================
-- Table 8: ProAssetinUserTokens
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinUserTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinUserTokens] (
        [UserId] NVARCHAR(450) NOT NULL,
        [LoginProvider] NVARCHAR(128) NOT NULL,
        [Name] NVARCHAR(128) NOT NULL,
        [Value] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_ProAssetinUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_ProAssetinUserTokens_ProAssetinUsers] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[ProAssetinUsers]([Id]) ON DELETE CASCADE
    );
    PRINT '✓ Table created: ProAssetinUserTokens';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinUserTokens';
END
GO

-- =============================================
-- Table 9: ProAssetinEmployeeConfigurations
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProAssetinEmployeeConfigurations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProAssetinEmployeeConfigurations] (
        [ConfigurationID] INT IDENTITY(1,1) NOT NULL,
        [EmployeeID] NVARCHAR(450) NOT NULL,
        [PreDefinedAssetID] NVARCHAR(50) NULL,
        [GSTNumber] NVARCHAR(50) NULL,
        [Image] VARBINARY(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_ProAssetinEmployeeConfigurations] PRIMARY KEY ([ConfigurationID]),
        CONSTRAINT [FK_ProAssetinEmployeeConfigurations_ProAssetinUsers] FOREIGN KEY ([EmployeeID]) 
            REFERENCES [dbo].[ProAssetinUsers]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ProAssetinEmployeeConfigurations_EmployeeID] ON [dbo].[ProAssetinEmployeeConfigurations]([EmployeeID]);
    PRINT '✓ Table created: ProAssetinEmployeeConfigurations';
END
ELSE
BEGIN
    PRINT '⚠ Table already exists: ProAssetinEmployeeConfigurations';
END
GO

PRINT '';
PRINT '===============================================';
PRINT 'Master Database Schema Creation Complete!';
PRINT 'Total Tables Created: 9';
PRINT '===============================================';
GO

