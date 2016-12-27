using System.Threading.Tasks;

namespace CrawlWebSite
{
    public class WebSchedule
    { 
        static LimitedConcurrencyLevelTaskScheduler scheduler = new LimitedConcurrencyLevelTaskScheduler(30);

        public static void Add(Task task)
        {
            task.Start(scheduler);
        }

    }
}