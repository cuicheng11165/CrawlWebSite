using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    class DataDispatcher
    {
        static readonly LimitedConcurrencyLevelTaskScheduler scheduler = new LimitedConcurrencyLevelTaskScheduler(5);

        WebDownload download = new WebDownload();
        public void Run(string siteUrl)
        {
            var newTask = new Task(() =>
              {
                  download.Fetch(siteUrl);
              });
            newTask.RunSynchronously(scheduler);
        }
    }
}


