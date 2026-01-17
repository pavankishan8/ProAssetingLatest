-- =============================================
-- Create All Databases
-- Run this script FIRST before creating tables
-- =============================================

USE [master]
GO

PRINT '===============================================';
PRINT 'Creating ProAssetin Databases';
PRINT '===============================================';
GO

-- =============================================
-- Database 1: ProAssetinDev (Master)
-- =============================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ProAssetinDev')
BEGIN
    CREATE DATABASE [ProAssetinDev]
    ON 
    ( NAME = 'ProAssetinDev',
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ProAssetinDev.mdf',
      SIZE = 100MB,
      MAXSIZE = 500MB,
      FILEGROWTH = 10MB )
    LOG ON 
    ( NAME = 'ProAssetinDev_Log',
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ProAssetinDev_Log.ldf',
      SIZE = 10MB,
      MAXSIZE = 100MB,
      FILEGROWTH = 10MB );
    PRINT '✓ Database created: ProAssetinDev';
END
ELSE
BEGIN
    PRINT '⚠ Database already exists: ProAssetinDev';
END
GO

-- =============================================
-- Database 2: ProAsset_Infosys (Infosys Customer)
-- =============================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ProAsset_Infosys')
BEGIN
    CREATE DATABASE [ProAsset_Infosys]
    ON 
    ( NAME = 'ProAsset_Infosys',
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ProAsset_Infosys.mdf',
      SIZE = 100MB,
      MAXSIZE = 500MB,
      FILEGROWTH = 10MB )
    LOG ON 
    ( NAME = 'ProAsset_Infosys_Log',
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ProAsset_Infosys_Log.ldf',
      SIZE = 10MB,
      MAXSIZE = 100MB,
      FILEGROWTH = 10MB );
    PRINT '✓ Database created: ProAsset_Infosys';
END
ELSE
BEGIN
    PRINT '⚠ Database already exists: ProAsset_Infosys';
END
GO

-- =============================================
-- Database 3: ProAsset_Wipro (Wipro Customer)
-- =============================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ProAsset_Wipro')
BEGIN
    CREATE DATABASE [ProAsset_Wipro]
    ON 
    ( NAME = 'ProAsset_Wipro',
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ProAsset_Wipro.mdf',
      SIZE = 100MB,
      MAXSIZE = 500MB,
      FILEGROWTH = 10MB )
    LOG ON 
    ( NAME = 'ProAsset_Wipro_Log',
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\ProAsset_Wipro_Log.ldf',
      SIZE = 10MB,
      MAXSIZE = 100MB,
      FILEGROWTH = 10MB );
    PRINT '✓ Database created: ProAsset_Wipro';
END
ELSE
BEGIN
    PRINT '⚠ Database already exists: ProAsset_Wipro';
END
GO

PRINT '';
PRINT '===============================================';
PRINT 'Database Creation Complete!';
PRINT 'Created 3 databases:';
PRINT '  1. ProAssetinDev (Master)';
PRINT '  2. ProAsset_Infosys (Infosys Customer)';
PRINT '  3. ProAsset_Wipro (Wipro Customer)';
PRINT '';
PRINT 'Next Steps:';
PRINT '  1. Run 01_MasterDatabase_Schema.sql on ProAssetinDev';
PRINT '  2. Run 02_TenantDatabase_Schema.sql on ProAsset_Infosys';
PRINT '  3. Run 02_TenantDatabase_Schema.sql on ProAsset_Wipro';
PRINT '===============================================';
GO

