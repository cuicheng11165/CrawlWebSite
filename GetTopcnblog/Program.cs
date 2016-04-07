using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GetTopcnblog
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamWriter writer = new StreamWriter("toppost.txt"))
            {
                for (int i = 2; i < 75; i++)
                {
                    HttpWebRequest client = WebRequest.CreateHttp("http://www.cnblogs.com/mvc/AggSite/PostList.aspx");
                    client.Method = "Post";
                    client.Accept = "text/plain, */*; q=0.01";
                    client.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3");
                    client.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
                    client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    client.ContentType = "application/json; charset=utf-8";
                    client.Referer = "http://www.cnblogs.com/pick/";
                    client.Host = "www.cnblogs.com";

                    var stream = client.GetRequestStream();
                    var content = "{\"CategoryType\":\"Picked\",\"ParentCategoryId\":0,\"CategoryId\":-2,\"PageIndex\":" + i + ",\"TotalPostCount\":1504,\"ItemListActionName\":\"PostList\"}";
                    var writebytes = Encoding.UTF8.GetBytes(content);
                    stream.Write(writebytes, 0, writebytes.Length);
                    stream.Close();
                    var response = client.GetResponse();

                    //var text = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.Load(response.GetResponseStream(), true);

                    var nodes = doc.DocumentNode.SelectNodes("//div[@class='post_item']");

                    foreach (var item in nodes)
                    {
                        var dignumberNode = item.SelectSingleNode("div/div/span[@class='diggnum']");
                        if (dignumberNode == null)
                        {
                            continue;
                        }
                        var dig = Convert.ToInt32(dignumberNode.InnerText);
                        if (dig < 15)
                        {
                            continue;
                        }

                        var titlelnkNode = item.SelectSingleNode("div/h3/a[@class='titlelnk']");
                        if (titlelnkNode == null)
                        {
                            continue;
                        }
                        var link = titlelnkNode.GetAttributeValue("href", "");
                        var title = titlelnkNode.InnerText;
                        Console.WriteLine(title);
                        writer.WriteLine("{0}\t{1}\t{2}", dig, title, link);
                    }

                }
            }


        }
    }
}
