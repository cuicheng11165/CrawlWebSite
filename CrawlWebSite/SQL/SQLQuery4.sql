USE [test]
GO

/****** Object:  StoredProcedure [dbo].[CreateTable]    Script Date: 2016/12/28 17:49:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[CreateTable]
    @TableName nvarchar(500)       
AS   

declare @createTableSql nvarchar(500) 
set @createTableSql =  'CREATE TABLE '+ @TableName+' (
	[Url] [nvarchar](1000) NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[Keywords] [nvarchar](1000) NULL,
	[Status] int
) ON [PRIMARY]' 

if not exists (select * from sys.tables where name= @TableName)
exec sp_executesql @createTableSql




GO


