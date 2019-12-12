# TannerydSample
The purpose of this repository is to insert sample items to test database and measure how long it took Tanneryd library to do so

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
```
3. Modify `app.config` file section
```
<add name="Context"
    connectionString="Data Source=<yourDbServerName>;Initial Catalog=SampleDatabase;Integrated Security=SSPI;MultipleActiveResultSets=True"
    providerName="System.Data.SqlClient" />

```
4. Open sln file and run it

## Remarks
Solution is created in Visual Studio 2019 community edition.
