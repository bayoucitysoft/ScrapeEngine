using ScrapeEngine.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeEngine.Objects
{
    public class CommandFunctionObject
    {
        public long? Id { get; set; }
        public string Operation { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }

        public CommandFunctionObject(long? id)
        {
            SQLAccess.Clear();
            SQLAccess.Procedure = "CommandFunctionById";
            SQLAccess.Parameters.Add(@"@id", id);
            DataTable dt = SQLAccess.ExecuteProcedure();
            if (dt.Rows.Count > 0)
            {
                var commandFunction = new CommandFunctionObject(dt.Rows[0]);
                this.Id = commandFunction.Id;
                this.Operation = commandFunction.Operation;
                this.Property = commandFunction.Property;
                this.Value = commandFunction.Value;
            }
        }

        public CommandFunctionObject(DataRow row)
        {
            if (row["id"] != DBNull.Value)
                Id = (long)row["id"];
            if (row["operation"] != DBNull.Value)
                Operation = (string)row["operation"];
            if (row["property"] != DBNull.Value)
                Property = (string)row["property"];
            if (row["value"] != DBNull.Value)
                Value = (string)row["value"];
        }
    }
}
