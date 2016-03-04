﻿using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    class WebCrawler
    {
        CrawlerContainer container = new CrawlerContainer();

        public void Fetch(string url)
        {
            var uriInstance = new Uri(url);

            var newUrl = string.Format("{0}://{1}", uriInstance.Scheme, uriInstance.Host);

            if (container.HasFailed(newUrl))
            {
                return;
            }
            if (container.HasSucceed(newUrl))
            {
                return;
            }

            try
            {
                WebClient webclient = new WebClient();
                webclient.Encoding = Encoding.UTF8;
                webclient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0");
                webclient.Headers.Add("Accept", "text/html, application/xhtml+xml, */*");
                //webclient.Headers.Add("Accept-Encoding", "gzip, deflate");
                webclient.Headers.Add("Host", uriInstance.Host);
                webclient.Headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3");
                webclient.Headers.Add("Accept-Charset", "utf-8");
                webclient.Headers.Add("Content-Type", "text/html; charset=gb2312");

                var reader = webclient.OpenRead(url);

                HtmlDocument doc = new HtmlDocument();
                doc.Load(reader, Encoding.UTF8);

                var metaNodes = doc.DocumentNode.SelectNodes("/html/head/meta");

                StringBuilder sb = new StringBuilder();
                foreach (var node in metaNodes)
                {
                    var name = node.GetAttributeValue("name", "NotFound");
                    var content = node.GetAttributeValue("content", "NotFound");

                    if (string.Equals(name, "NotFound") || string.Equals(content, "NotFound"))
                    {
                        continue;
                    }

                    sb.Append(name);
                    sb.Append(":");
                    sb.Append(content);
                    sb.Append("  ");
                }

                this.container.AddSuccessfulUrl(newUrl, sb.ToString());


                var anchors = doc.DocumentNode.SelectNodes("//a");

                const string defaultValue = "NotFound";

                foreach (var ele in anchors)
                {
                    var href = ele.GetAttributeValue("href", defaultValue);
                    if (string.Equals(href, defaultValue))
                    {
                        continue;
                    }
                    if (!href.StartsWith("http"))
                    {
                        continue;
                    }

                    var uri = new Uri(href);

                    var hostdns = uri.Host.Split('.');


                    this.container.Enqueue(string.Format("{0}://{1}", uri.Scheme, uri.Host));
                }
            }
            catch (Exception e)
            {
                container.AddFailedUrl(url);
            }

        }

        internal void Start(string startUrl)
        {
            Fetch(startUrl);
            Go();
        }

        public void Go()
        {
            while (true)
            {
                var resultUrl = container.Dequeue();
                var task = Task.Factory.StartNew(delegate
                {
                    this.Fetch(resultUrl);
                });
                //task.Wait();
            }

        }
    }
}