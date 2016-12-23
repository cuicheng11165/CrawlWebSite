using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    class DataDispatcher
    {

        public void Run(string siteUrl)
        {

            IEnumerable<string> result;

            WebDownload download = new WebDownload();
            result = download.Fetch(siteUrl);




            var uri = new Uri(siteUrl);


            if (!Directory.Exists(uri.Host))
            {
                Directory.CreateDirectory(uri.Host);
            }


            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, uri.Host, Guid.NewGuid().ToString() + ".txt");

            foreach (var url in result)
            {
                var cUri = new Uri(url);
                if (cUri.Host == uri.Host)
                {
                    WebSchedule.Add(new Task(delegate
                    {
                        new DataDispatcher().Run(url);
                    }));
                }
                else
                {
                    var writer = File.AppendText(file);
                    writer.WriteLine(url);
                    writer.Close();
                }
            }

            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file);

                foreach (var line in lines)
                {
                    new DataDispatcher().Run(line);
                }

                File.Delete(file);
            }
        }
    }
}


