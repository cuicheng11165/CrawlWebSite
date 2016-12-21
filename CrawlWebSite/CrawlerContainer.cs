using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    class CrawlerContainer
    {
        SpiderModel context = new SpiderModel();

        List<string> UrlQueue = new List<string>();
        private object queuesync = new object();

        const int itemthresold = 100;

        public bool HasFailed(string url)
        {
            var find = context.Failed.FirstOrDefault(w => string.Equals(url, w.Url));
            return find != null;
        }



        internal bool HasSucceed(string url)
        {
            var find = context.Passed.FirstOrDefault(w => string.Equals(url, w.Url));
            return find != null;
        }
        public void AddFailedUrl(string url, string errorMessage)
        {
            Console.WriteLine("Add Failed: {0}, {1}", url, errorMessage);
            context.Failed.Add(new Fail() { Url = url, ErrorMessage = errorMessage, AccessTime = DateTime.Now });
        }

        public void AddSuccessfulUrl(string url, string keyword)
        {
            Console.WriteLine("Add Successful: {0}", url);
            context.Passed.Add(new Pass() { Url = url, AccessTime = DateTime.Now });
        }

        private Dictionary<string, WebPool> hosts = new Dictionary<string, WebPool>();
        public void Enqueue(string url)
        {
            var host = UrlUtil.GetHost(url);

            var entity = context.WebPool.FirstOrDefault(w => w.Url == host);
            if (entity != null)
            {

            }

        }

        public string Dequeue()
        {
            return null;
        }
    }


    public class UrlUtil
    {
        public static string GetHost(string url)
        {
            return new Uri(url).Host;
        }
    }
}
