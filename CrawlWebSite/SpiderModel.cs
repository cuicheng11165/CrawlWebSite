using System.ComponentModel.DataAnnotations.Schema;

namespace CrawlWebSite
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class SpiderModel : DbContext
    {
        public SpiderModel()
            : base("name=SpiderModel")
        {
        }

        public string TableName { set; get; }

        public DbSet<Pass> Passed { get; set; }

        public DbSet<Fail> Failed { get; set; }

        public DbSet<WebPool> WebPool { get; set; }

        public DbSet<ContextUrl> Context { get; set; }
    }

    public class ContextUrl
    {
        [Index]
        public string Domain { set; get; }

        public string Url { set; get; }
    }

    public class WebPool
    {
        [Index]
        public string Url { set; get; }

        public int InUrl { set; get; }

        public int OutUrl { set; get; }
    }

    public class Pass
    {
        [Index]
        public string Domain { set; get; }
        [Index]
        public string Url { set; get; }

        public DateTime AccessTime { set; get; }
    }

    public class Fail
    {

        [Index]
        public string Domain { set; get; }
        [Index]
        public string Url { set; get; }

        public string ErrorMessage { set; get; }

        public DateTime AccessTime { set; get; }
    }
}
