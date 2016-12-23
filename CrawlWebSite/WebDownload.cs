using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace CrawlWebSite
{
    class WebDownload
    {
        public IEnumerable<string> Fetch(string url)
        {
            SpiderModel context = new SpiderModel();
            var find = context.Passed.FirstOrDefault(f => f.Url == url);
            if (find != null)
            {
                yield break;
            }

            var uriInstance = new Uri(url);


            HtmlDocument doc = new HtmlDocument();
            try
            {

                WebClient webclient = new WebClient();
                webclient.Encoding = Encoding.UTF8;
                webclient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0");
                webclient.Headers.Add("Accept", "text/html, application/xhtml+xml, */*");
                //webclient.Headers.Add("Accept-Encoding", "gzip, deflate");
                webclient.Headers.Add("Host", uriInstance.Host);
                webclient.Headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3");

                var reader = webclient.OpenRead(url);


                doc.Load(reader);


                var metaNodes = doc.DocumentNode.SelectNodes("/html/head/meta");

                StringBuilder sb = new StringBuilder();
                if (metaNodes != null)
                {
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
                }



                if (url.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) < 8)
                {
                    var path = PathHelper.GetPathFromHost(url);
                    File.AppendAllText(Path.Combine(path, "Description.txt"), sb.ToString());
                }

            }
            catch (Exception e)
            {
                yield break;
            }

            var anchors = doc.DocumentNode.SelectNodes("//a");
            if (anchors != null)
            {
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

                    yield return href;

                }
            }

        }




    }
}