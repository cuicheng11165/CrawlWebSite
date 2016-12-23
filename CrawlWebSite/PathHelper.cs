using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    static class PathHelper
    {
        public static string GetPathFromHost(string host)
        {
            return Path.Combine("../../../", "OutPut", host);
        }
    }
}
