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
        List<string> UrlQueue = new List<string>();
        private object queuesync = new object();

        const int itemthresold = 100;

        public bool HasFailed(string url)
        {
            return SqlHelper.TrySelectFromFailedWeb(url);
        }

        internal bool EnsurePrimaryDomain(string newUrl)
        {
            return SqlHelper.HasSuccessfulDomain(newUrl);
        }

        internal bool HasSucceed(string url)
        {
            return SqlHelper.TrySelectFromSuccessfulWeb(url);
        }
        public void AddFailedUrl(string url, string errorMessage)
        {
            Console.WriteLine("Add Failed: {0}", url);
            SqlHelper.InsertToFailedWeb(url, errorMessage);
        }

        public void AddSuccessfulUrl(string url, string keyword)
        {
            Console.WriteLine("Add Successful: {0}", url);
            SqlHelper.InserToSuccessfulWeb(url, keyword);
        }

        public void Enqueue(string url)
        {
            lock (queuesync)
            {
                if (!UrlQueue.Contains(url))
                {
                    UrlQueue.Add(url);
                }

                if (UrlQueue.Count > 200)
                {
                    Console.WriteLine("Insert {0} items into cache items", itemthresold);
                    var insertCount = UrlQueue.Count / 2;
                    SqlHelper.InsertToCacheWeb(UrlQueue.Take(insertCount));
                    UrlQueue.RemoveRange(0, insertCount);
                }
            }
        }

        public string Dequeue()
        {
            lock (queuesync)
            {
                string result;
                if (UrlQueue.Count == 0)
                {
                    if (!SqlHelper.TryPopFromCacheWeb(out result))
                    {
                        Thread.Sleep(10000);
                    }
                }
                else
                {
                    result = UrlQueue[UrlQueue.Count - 1];
                    UrlQueue.RemoveAt(UrlQueue.Count - 1);
                }
                return result;
            }
        }
    }
}
