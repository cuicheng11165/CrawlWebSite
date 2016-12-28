using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlWebSite
{

    class SQLConn
    {
        const string CreateTable = @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '@tableName')
  CREATE TABLE ['@tableName'] (
    [Host] [nvarchar](1000) NOT NULL,
    [Title] [nvarchar](1000) NULL,
    [Keywords] [nvarchar](1000) NULL
  ) ON [PRIMARY]";



    }
}
