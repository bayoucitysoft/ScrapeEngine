using HtmlAgilityPack;
using ScrapeEngine.NodeCommand;
using ScrapeEngine.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeEngine.Objects
{
    public class ScrapeEngine
    {
        public string CurrentUrl { get; set; }

        public HtmlWeb Client { get; set; }
        public HtmlDocument Doc { get; set; }
        public HtmlNodeCollection RootNodes { get; set; }
        public CrawlSequenceObject CrawlSequence { get; set; }
        public NodeCommandObject CurrentCommand { get; set; }
        public List<NodeCommandObject> CommandCollection { get; set; }
        public List<Dictionary<string, string>> DomainObjectCollection { get; set; }

        public void RetrieveNextCrawlSequence()
        {
            SQLAccess.Clear();
            SQLAccess.Procedure = "NextCrawlSequence";
            var table = SQLAccess.ExecuteProcedure();
            this.CrawlSequence = new CrawlSequenceObject(table.Rows[0]);
            if (!string.IsNullOrEmpty(this.CrawlSequence.SiteUrl))
                CurrentUrl = this.CrawlSequence.SiteUrl;
            if (this.CrawlSequence.InitialNodeCommandId != 0)
                CurrentCommand = new NodeCommandObject(this.CrawlSequence.InitialNodeCommandId);
        }

        public void Initailize()
        {
            Client = new HtmlWeb();
            CurrentCommand = new NodeCommandObject();
            CommandCollection = new List<NodeCommandObject>();
            DomainObjectCollection = new List<Dictionary<string, string>>();
        }

        internal void BeginScraping()
        {
            if (!string.IsNullOrEmpty(CurrentUrl))
            {
                if (CommandCollection.Count == 0)
                    CommandCollection = NodeCommandByCrawlSequenceId(CrawlSequence.Id);

                Doc = Client.Load(CurrentUrl);
                RootNodes = Doc.DocumentNode.SelectNodes(CurrentCommand.Xpath);

                if (CommandCollection.Count > 0)
                    foreach (HtmlNode node in RootNodes)
                        DomainObjectCollection.Add(NodeCommandRouter.RouteNode(node, CommandCollection));



            }
        }

        private List<NodeCommandObject> NodeCommandByCrawlSequenceId(long id)
        {
            List<NodeCommandObject> response = new List<NodeCommandObject>();
            SQLAccess.Clear();
            SQLAccess.Procedure = "NodeCommandByCrawlSequenceId";
            SQLAccess.Parameters.Add(@"@crawl_sequence_id", id);
            DataTable dt = SQLAccess.ExecuteProcedure();
            if (dt.Rows.Count > 0)
                foreach (DataRow row in dt.Rows)
                    response.Add(new NodeCommandObject(row));
            return response;
        }


        public void GenerateRootNodes()
        {

        }
    }
}
