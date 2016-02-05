using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Reflection;

namespace NBU_Mailer_2016
{
    class WorkWithDB
    {
        public static Logger nLogger = LogManager.GetCurrentClassLogger();


        // TODO: REFACTOR THIS !!!!!!!!
        // TODO: REFACTOR THIS !!!!!!!!
        // TODO: REFACTOR THIS !!!!!!!!
        // TODO: REFACTOR THIS !!!!!!!!

        public bool IfDbExists(string databaseName, string userName, string passw)
        {
            string connStr = "Data Source=MAIN;uid=" + userName + ";password=" + passw + ";database=" + databaseName;

            bool bdExists = false;
            string sqlDBQuery;

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                sqlDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", databaseName);

                SqlConnection dbConnect = new SqlConnection(connStr);

                using (dbConnect)
                {
                    using (SqlCommand cmd = new SqlCommand(sqlDBQuery, dbConnect))
                    {
                        dbConnect.Open();

                        object resultObj = cmd.ExecuteScalar();

                        int databaseID = 0;

                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out databaseID);
                        }

                        dbConnect.Close();

                        bdExists = (databaseID > 0);
                    }
                }
            }
            catch (Exception exc)
            {
                nLogger.Error(methodName + "() - " + exc.Message);
                bdExists = false;
            }
            return bdExists;
        }
    }
}
