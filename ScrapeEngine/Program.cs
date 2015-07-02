using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            HubskiScraper_v2 hubski = new HubskiScraper_v2();
            hubski.RetrieveNextCrawlSequence();
            hubski.BeginScraping();
            Console.ReadLine();
        }
    }
}
