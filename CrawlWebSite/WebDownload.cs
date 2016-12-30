using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using MongoDB.Driver;

namespace CrawlWebSite
{
    class WebDownload
    {
        MongoConn conn = new MongoConn();

        public void Fetch(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            url = url.TrimEnd('/');
            var uriInstance = new Uri(url);

            if (!MongoConn.TableNames.Contains(uriInstance.Host))
            {
                conn.CreateTable(uriInstance.Host);
            }

            var hasDescription = conn.HasDescription(uriInstance.Host);
            var isRoot = url.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) < 8;
            if (!hasDescription && !isRoot)
            {
                Fetch(string.Format("{0}://{1}", uriInstance.Scheme, uriInstance.Host));
            }
            //if (hasDescription && isRoot)
            //{
            //    return;
            //}


            HtmlDocument doc = new HtmlDocument();
            try
            {

                var webclient = HttpWebRequest.CreateHttp(uriInstance);
                webclient.Accept = "text/html";
                webclient.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0";
                webclient.Host = uriInstance.Host;
                //webclient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0");
                //webclient.Headers.Add("Accept", "text/html, application/xhtml+xml, */*");
                //webclient.Headers.Add("Accept-Encoding", "gzip, deflate");
                //webclient.Headers.Add("Host", uriInstance.Host);
                webclient.Headers.Add("Accept-Language", "zh-CN");

                var response = webclient.GetResponse();

                var contentType = response.ContentType;

                if (contentType.IndexOf("text/html") < 0)
                {
                    conn.UpsertUrlToHost(uriInstance.Host, url, "", "", 2);
                    return;
                }

                var reader = response.GetResponseStream();


                var meo = new MemoryStream();

                byte[] buff = new byte[1024 * 1024];
                int offset = 0;
                int read;
                do
                {
                    read = reader.Read(buff, 0, buff.Length);
                    offset += read;
                    meo.Write(buff, 0, read);
                } while (read >= buff.Length);


                meo.Position = 0;
                doc.Load(meo, Encoding.UTF8, true);



                //var meta = doc.DocumentNode.SelectSingleNode("/html/head/meta");
                //var charset = meta.GetAttributeValue("charset", "");
                //var contentValue = meta.GetAttributeValue("content", "");

                //var charsetPos = contentValue.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
                //if (!string.IsNullOrEmpty(contentValue) && charsetPos > 0)
                //{
                //    var chasetValue = contentValue.Substring(charsetPos + "charset=".Length);
                //    if (!string.IsNullOrEmpty(chasetValue) && !string.Equals(chasetValue, "utf-8", StringComparison.OrdinalIgnoreCase))
                //    {
                //        meo.Position = 0;
                //        doc.Load(meo, Encoding.GetEncoding(chasetValue));
                //    }
                //}



                if (doc.Encoding.BodyName != Encoding.UTF8.BodyName && doc.OptionDefaultStreamEncoding.BodyName != Encoding.UTF8.BodyName)
                {
                    meo.Position = 0;
                    doc.Load(meo, doc.Encoding);
                }


                var metaNodes = doc.DocumentNode.SelectNodes("/html/head/meta");
                var title = doc.DocumentNode.SelectSingleNode("/html/head/title");

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
                var titleText = title == null ? "" : title.InnerText;
                conn.UpsertUrlToHost(uriInstance.Host, url, titleText, sb.ToString(), 1);

                if (url.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) < 8)
                {
                    conn.UpsertUrlDescription(uriInstance.Host, titleText, sb.ToString());
                }
            }
            catch (Exception e)
            {
                conn.UpsertUrlToHost(uriInstance.Host, url, "", "", 2);
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
                    try
                    {
                        var hrefUri = new Uri(href);
                        if (!MongoConn.TableNames.Contains(hrefUri.Host))
                        {
                            conn.CreateTable(hrefUri.Host);
                        }
                        conn.UpsertUrlToHost(hrefUri.Host, href.TrimEnd('/'), "", "", 0);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

        }




    }
}