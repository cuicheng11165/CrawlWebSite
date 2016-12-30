using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlWebSite
{

    class SQLQuery
    {
        public const string connectionString = "Data Source=.;Initial Catalog=test;Integrated Security=True";
        public const string CreateTable = @"declare @createTableSql nvarchar(500) 
set @createTableSql =  'CREATE TABLE [dbo].['+ @TableName+'] (
	[Url] [nvarchar](1000) NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[Keywords] [nvarchar](1000) NULL,
	[Status] int
) ON [PRIMARY]' 

if not exists (select * from sys.tables where name= @TableName)
exec sp_executesql @createTableSql";

        public const string InsertUrl = @"declare @curStatus int
select  @curStatus=Status from [dbo].[{0}] where Url=@Url
if @curStatus is null
insert into [dbo].[{0}] values (@Url,@Title,@Keywords,@Status)
else if @curStatus=0
begin
update [dbo].[{0}] set Title=@Title, Keywords=@Keywords , Status=@Status where Url=@Url 
end
";

        public const string UpsertDescription = @"if exists (select * from WebSite where Host=@Url) 
update WebSite set Title=@Title, Keywords=@Keywords where Host=@Url
else
insert into WebSite values (@Url,@Title,@Keywords,@Status)
";


        public const string HasDescription = "select * from WebSite where Host=@Url";

        public static string SelectTables = "select name from sys.tables";

        public static string SelectUrl = @"declare @url nvarchar(500)
select top 1 @url=Url from [dbo].[{0}] where Status=0
update [dbo].[{0}] set Status= 1 where url=@url
select @url
";

        public static string SelectDescription = @"declare @url nvarchar(500)
select top 1 @url=Host from [dbo].[WebSite] where Status=0
update [dbo].[WebSite] set Status= 1 where Host=@url
select @url
";

    }
}
