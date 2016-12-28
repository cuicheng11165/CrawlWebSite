USE [test]
GO

/****** Object:  StoredProcedure [dbo].[InsertOrUpdateUrl]    Script Date: 2016/12/28 17:49:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertOrUpdateUrl]
	@Host nvarchar(500),   
	@Url nvarchar(500),   
    @Title nvarchar(500),  
	@Keywords nvarchar(500),   
	@Status int   
AS
BEGIN
exec CreateTable @Host
declare @sqlscript nvarchar(500)

--set @sqlscript = 'if exists (select * from  '''+ @Host +'''  where Url= '''+ @Url +''' ) 
--update  '+ @Host +'  set Url='''+@Url+', Title= '''+ @Title +'''  , Keywords= '''+ @Keywords +'''  , Status= '''+ '1' +'  where Url= '''+@Url  +''' 
--else 
--begin
set @sqlscript =  'insert into  '+  @Host+' values( '+ @Url +' , '+ @Title +' , '+ @Keywords +', '+@Status+' ) '
--end '

exec sp_executesql @sqlscript 

END 



GO


