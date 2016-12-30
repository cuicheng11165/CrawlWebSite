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

            string starturl = "http://www.ithome.com";

            MongoConn conn = new MongoConn();


            MongoConn.TableNames = new HashSet<string>(conn.MoveNextTable());

            DataDispatcher dispatcher = new DataDispatcher();
            dispatcher.Run(starturl);

            while (true)
            {

                foreach (var tableName in MongoConn.TableNames)
                {
                    if (tableName == "WebSite")
                    {
                        continue;
                    }
                    string record = conn.MoveNextRecord(tableName);
                    //string record = conn.MoveNextDescription();
                    if (!string.IsNullOrEmpty(record))
                    {
                        dispatcher.Run(record);
                        record = conn.MoveNextRecord(tableName);
                    }
                }
            }

        }
    }
}

