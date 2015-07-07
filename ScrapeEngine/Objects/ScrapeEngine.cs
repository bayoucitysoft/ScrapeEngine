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

                if (DomainObjectCollection.Count > 0)
                {
                    ValidateSchema();
                    UploadDomainObject();
                }
            }
        }

        private void ValidateSchema()
        {
            foreach (var domainObject in DomainObjectCollection)
                foreach (var key in domainObject.Keys)
                {
                    SQLAccess.Clear();
                    SQLAccess.Procedure = "ValidateDomainObject";
                    SQLAccess.Parameters.Add(@"domain_object_name", this.CrawlSequence.ConfigName);
                    SQLAccess.Parameters.Add(@"domain_object_property", key);
                    SQLAccess.ExecuteNonQuery();
                }
                
        }

        private void UploadDomainObject()
        {
            foreach (var domainObject in DomainObjectCollection)
            {
                Guid guid = Guid.NewGuid();
                foreach (var pair in domainObject)
                {
                    SQLAccess.Clear();
                    SQLAccess.Procedure = "FlattenDomainObject";
                    SQLAccess.Parameters.Add(@"object_guid", guid);
                    SQLAccess.Parameters.Add(@"column_name", pair.Key);
                    SQLAccess.Parameters.Add(@"column_value", pair.Value);
                    SQLAccess.Parameters.Add(@"table_name", CrawlSequence.ConfigName);
                    SQLAccess.ExecuteProcedure();
                }
                SQLAccess.Clear();
                SQLAccess.Procedure = "UploadDomainObjectFromCache";
                SQLAccess.Parameters.Add(@"guid", guid);
                SQLAccess.ExecuteProcedure();
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
