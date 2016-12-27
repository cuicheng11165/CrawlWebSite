using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    class Program
    {
        static void Main(string[] args)
        {

            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            string starturl = "http://news.baidu.com/";

            MongoConn conn = new MongoConn();

            DataDispatcher dispatcher = new DataDispatcher();
            //dispatcher.Run(starturl);


            while (true)
            {
                var tables = conn.MoveNextTable();
                foreach (var tableName in tables)
                {
                    var record = conn.MoveNextRecord(tableName);
                    if (record != null)
                    {
                        dispatcher.Run(record);
                    }
                }
            }

        }
    }
}

