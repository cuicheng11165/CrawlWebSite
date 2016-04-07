using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("d:\\prxy.txt");

            var writer = File.AppendText("d:\\passedProxy.txt");

            ThreadPool.SetMaxThreads(100, 100);
            foreach (var line in lines)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        var tmp = line;
                        HttpWebRequest.DefaultWebProxy = new WebProxy(tmp.Trim());
                        WebClient client = new WebClient();
                        client.DownloadString("http://www.cnblogs.com/");


                        writer.WriteLine(line);

                        Console.WriteLine("{0} success", line);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("{0} failed", line);
                    }
                });
            }

            Console.ReadLine();
        }
    }
}
