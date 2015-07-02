using ScrapeEngine.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeEngine.Objects
{
    public class NodeCommandObject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Xpath { get; set; }
        public string DomainObjectName { get; set; }
        public string DomainObjectProperty { get; set; }

        public List<CommandFunctionObject> Commands { get; set; }

        public NodeCommandObject()
        {
            Commands = new List<CommandFunctionObject>();
        }

        public NodeCommandObject(System.Data.DataRow row)
        {
            this.Commands = new List<CommandFunctionObject>();

            if (row["id"] != DBNull.Value)
                Id = (long)row["id"];
            if (row["name"] != DBNull.Value)
                Name = (string)row["name"];
            if (row["description"] != DBNull.Value)
                Description = (string)row["description"];
            if (row["xpath"] != DBNull.Value)
                Xpath = (string)row["xpath"];
            if (row["first_cmd_id"] != DBNull.Value)
                this.Commands.Add(new CommandFunctionObject(Convert.ToInt32(row["first_cmd_id"])));
            if (row["second_cmd_id"] != DBNull.Value)
                this.Commands.Add(new CommandFunctionObject(Convert.ToInt32(row["second_cmd_id"])));
            if (row["domain_object_name"] != DBNull.Value)
                DomainObjectName = (string)row["domain_object_name"];
            if (row["domain_object_property"] != DBNull.Value)
                DomainObjectProperty = (string)row["domain_object_property"];
        }

        public NodeCommandObject(long id)
        {
            SQLAccess.Clear();
            SQLAccess.Procedure = "NodeCommandById";
            SQLAccess.Parameters.Add(@"id", id);
            var table = SQLAccess.ExecuteProcedure();
            if (table.Rows.Count > 0)
            {
                var nodeCommand = new NodeCommandObject(table.Rows[0]);
                this.Id = nodeCommand.Id;
                this.Name = nodeCommand.Name;
                this.Description = nodeCommand.Description;
                this.Xpath = nodeCommand.Xpath;
                this.Commands = nodeCommand.Commands;
            }
        }
    }
}
