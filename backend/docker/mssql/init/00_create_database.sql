IF DB_ID(N'HRIS') IS NULL
BEGIN
    CREATE DATABASE [HRIS];
END
GO

-- Pin to SQL Server 2012 (110) behavior since prod is locked at that version.
ALTER DATABASE [HRIS] SET COMPATIBILITY_LEVEL = 110;
GO
