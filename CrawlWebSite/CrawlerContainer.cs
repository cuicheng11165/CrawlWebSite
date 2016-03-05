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

        //ConcurrentDictionary<string, string> UsedDictionary { set; get; } = new ConcurrentDictionary<string, string>();

        //private Dictionary<string, string> failedUrls = new Dictionary<string, string>();

        ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
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
            SqlHelper.InsertToFailedWeb(url, errorMessage);
        }

        object dic = new object();
        public void AddSuccessfulUrl(string url, string keyword)
        {
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

                if (UrlQueue.Count > 1000)
                {
                    //var t = Task.Factory.StartNew(delegate
                    //{
                    Console.WriteLine("Insert {0} items into cache items", itemthresold);
                    var insertCount = UrlQueue.Count / 2;
                    SqlHelper.InsertToCacheWeb(UrlQueue.Take(insertCount));
                    UrlQueue.RemoveRange(0, insertCount);
                    //});
                    //t.Wait();

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

                    if (SqlHelper.TryPopFromCacheWeb(out result))
                    {

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
