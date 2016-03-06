using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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

            ThreadPool.SetMaxThreads(100, 100);

            string starturl = "http://www.sina.com.cn/";
            WebCrawler crawl = new WebCrawler();
            crawl.Start(starturl);

        }
    }
}

