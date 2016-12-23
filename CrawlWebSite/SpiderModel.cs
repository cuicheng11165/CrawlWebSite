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
    }

    public class Pass
    {
        public Guid Id { set; get; }

        public string Url { set; get; }

        public DateTime AccessTime { set; get; }
        public string Keywords { get; set; }
    }


}
