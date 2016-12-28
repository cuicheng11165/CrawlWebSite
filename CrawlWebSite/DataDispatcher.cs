using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    class DataDispatcher
    {
        static readonly List<Task> lists = new List<Task>();

        WebDownload download = new WebDownload();
        public void Run(string siteUrl)
        {
            var newTask = new Task(() =>
            {
                download.Fetch(siteUrl);
            });
            if (lists.Count < 10)
            {
                lists.Add(newTask);
                newTask.RunSynchronously();
            }
            else
            {
                var index = Task.WaitAny(lists.ToArray());
                lists.RemoveAt(index);
                lists.Add(newTask);
                newTask.RunSynchronously();
            }
        }
    }
}


