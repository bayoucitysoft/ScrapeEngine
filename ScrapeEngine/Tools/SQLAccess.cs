using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeEngine.Tools
{
    public class SQLAccess
    {
        private static string connString = "Data Source=localhost;Initial Catalog=HubskiScraper;User ID=caleb;Password=caleb;Integrated Security=False;";
        public static string Procedure { get; set; }
        public static Dictionary<string, object> Parameters { get; set; }
        public static bool Success { get; set; }

        public static class ErrorLog
        {
            public static string PublicMessage { get; set; }
            internal static string ErrorHeader { get; set; }
            internal static string PrivateMessage { get; set; }
            internal static DateTime TimeStamp { get; set; }
            internal static Guid Guid { get; set; }
        }

        internal static void ExecuteNonQuery()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(Procedure, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (var param in Parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);

                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Success = true;
                    }
                }
            }
            catch
            {
                Success = false;
                SetError(Guid.NewGuid(), Procedure);
            }
        }

        private static void SetError(Guid guid, string proc)
        {
            ErrorLog.Guid = guid;
            ErrorLog.TimeStamp = DateTime.Now;
            ErrorLog.ErrorHeader = @"Error Inserting " + proc;
        }

        internal static DataTable ExecuteProcedure()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(Procedure, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (var param in Parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
            }
            catch
            {
                //do some erro handling sometime, eh?
            }
            SQLAccess.Clear();
            return dt;
        }

        internal static void Clear()
        {
            Procedure = string.Empty;
            if (Parameters == null)
                Parameters = new Dictionary<string, object>();
            else
                Parameters.Clear();
        }
    }
}
