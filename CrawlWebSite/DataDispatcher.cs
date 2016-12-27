using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    class DataDispatcher
    {
        WebDownload download = new WebDownload();
        public void Run(string siteUrl)
        {

            download.Fetch(siteUrl);
        }
    }
}


