# TannerydSample
The purpose of this repository is to insert sample items to test database and measure how long it took [Tannyryd](https://github.com/mtanneryd/ef6-bulk-operations) library to do so

## Setup instructions
1. Clone this repository (duuuh)
2. Execute following script on your database server of choice
```SQL
CREATE DATABASE [SampleDatabase]
ALTER DATABASE [SampleDatabase] SET READ_COMMITTED_SNAPSHOT ON 
GO

USE [SampleDatabase]
GO

CREATE TABLE dbo.Test
(	
	Id INT IDENTITY PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL,
	DateOfBirth DATE NOT NULL DEFAULT '19991231'
)

CREATE TYPE [dbo].[TestInsertType] AS TABLE(
	Name NVARCHAR(100) NOT NULL,
	DateOfBirth DATE NOT NULL
)

CREATE PROCEDURE [dbo].[InsertTest]
    @myTableType TestInsertType readonly
AS
BEGIN
    insert into [dbo].Test select * from @myTableType 
END
```
3. Modify `app.config` file section
```XML
<add name="Context"
    connectionString="Data Source=<yourDbServerName>;Initial Catalog=SampleDatabase;Integrated Security=SSPI;MultipleActiveResultSets=True"
    providerName="System.Data.SqlClient" />
```
4. Open solution and rebuild it
5. Navigate to TannerydSample\ConsoleApp1\bin\Debug and run `.\ConsoleApp1.exe 1 proc`. This will measure thread execution with sql procedure insert. To use Tanneryd bulk operations, replace `proc` with `bulk`

## Remarks
Solution is created in Visual Studio 2019 community edition.

# TLDR
The best seems to be SQL bulk copy, SQL procedure is the fastest but has extreme deviations under high load, Tanneryd buk operations, although good themselves, can't come close to the performance of SQL bulk copy and SQL procedure insert