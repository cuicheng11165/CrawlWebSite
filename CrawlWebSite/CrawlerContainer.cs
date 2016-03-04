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

        ConcurrentDictionary<string, string> UsedDictionary { set; get; } = new ConcurrentDictionary<string, string>();

        private HashSet<string> failedUrls = new HashSet<string>();

        ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
        public bool HasFailed(string url)
        {
            bool succeed;
            rwl.EnterReadLock();

            try
            {
                succeed = failedUrls.Contains(url);
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ExitReadLock();
            }

            if (!succeed)
            {
                if (SqlHelper.TrySelectFromFailedWeb(url))
                {
                    rwl.EnterWriteLock();
                    try
                    {
                        failedUrls.Add(url);
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        rwl.ExitWriteLock();
                    }

                    succeed = true;
                }
            }
            return succeed;
        }

        internal bool HasSucceed(string host)
        {
            var succeed = UsedDictionary.ContainsKey(host);
            if (!succeed)
            {
                string result;
                if (SqlHelper.TrySelectFromSuccessfulWeb(host, out result))
                {
                    UsedDictionary[host] = result;
                    succeed = true;
                }
            }
            return succeed;
        }

        public void AddFailedUrl(string url)
        {
            rwl.EnterWriteLock();
            try
            {
                failedUrls.Add(url);
                if (failedUrls.Count > itemthresold)
                {
                    Console.WriteLine("Insert {0} items into failed items", itemthresold);
                    SqlHelper.InsertToFailedWeb(failedUrls);
                    failedUrls.Clear();
                }
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ExitWriteLock();
            }
        }

        public void AddSuccessfulUrl(string url, string keyword)
        {
            UsedDictionary[url] = keyword;
            if (UsedDictionary.Count > itemthresold)
            {
                var task = new Task(delegate
                {
                    Console.WriteLine("Insert {0} items into successful items", itemthresold);
                    SqlHelper.InserToSuccessfulWeb(UsedDictionary);
                    UsedDictionary.Clear();
                });
                task.Start();
            }
        }

        public void Enqueue(string url)
        {
            lock (queuesync)
            {
                if (!UrlQueue.Contains(url))
                {
                    UrlQueue.Add(url);
                }

                if (UrlQueue.Count > 10000)
                {
                    //var t = Task.Factory.StartNew(delegate
                    //{
                    Console.WriteLine("Insert {0} items into cache items", itemthresold);
                    SqlHelper.InsertToCacheWeb(UrlQueue);
                    UrlQueue.RemoveRange(0, UrlQueue.Count);
                    //});
                    //t.Wait();

                }
            }


        }

        public string Dequeue()
        {
            while (UrlQueue.Count == 0)
            {
                Thread.Sleep(5000);
            }
            lock (queuesync)
            {

                var result = UrlQueue[UrlQueue.Count - 1];
                UrlQueue.RemoveAt(UrlQueue.Count - 1);
                return result;
            }
        }
    }
}
