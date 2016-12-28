USE [test]
GO

/****** Object:  StoredProcedure [dbo].[InsertOrUpdateWebDescription]    Script Date: 2016/12/28 17:49:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertOrUpdateWebDescription]
	@Host nvarchar(500),  
	@Title nvarchar(1000),  	 
	@Keywords nvarchar(500),   
	@Status int   
AS
BEGIN


if exists (select * from WebSite where Host=@Host) 
update WebSite set Keywords=@Keywords , Status=@Status where Host=@Host
else
insert into WebSite values(@Host,@Title,@Keywords,@Status)


END




GO


