using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeEngine.Objects
{
    public class CrawlSequenceObject
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public Int32 Sequence { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsActive { get; set; }
        public Int32 CrawlDepth { get; set; }
        public long InitialNodeCommandId { get; set; }
        public string ConfigName { get; set; }

        public List<CrawlSequenceObject> Children { get; set; }

        public CrawlSequenceObject()
        {
            Children = new List<CrawlSequenceObject>();
        }

        public CrawlSequenceObject(DataRow row)
        {
            Children = new List<CrawlSequenceObject>();

            if (row["id"] != DBNull.Value)
                Id = (long)row["id"];
            if (row["parent_id"] != DBNull.Value)
                ParentId = (long)row["parent_id"];
            if (row["site_name"] != DBNull.Value)
                SiteName = (string)row["site_name"];
            if (row["site_url"] != DBNull.Value)
                SiteUrl = (string)row["site_url"];
            if (row["sequence"] != DBNull.Value)
                Sequence = (Int32)row["sequence"];
            if (row["is_current"] != DBNull.Value)
                IsCurrent = (bool)row["is_current"];
            if (row["is_active"] != DBNull.Value)
                IsActive = (bool)row["is_active"];
            if (row["crawl_depth"] != DBNull.Value)
                CrawlDepth = (Int32)row["crawl_depth"];
            if (row["initial_node_command_id"] != DBNull.Value)
                InitialNodeCommandId = (long)row["initial_node_command_id"];
            if (row["config_name"] != DBNull.Value)
                row["config_name"] = (string)row["config_name"];
        }
    }
}
