using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Reflection;
using System.Data.OleDb;
using System.Data;

namespace NBU_Mailer_2016
{
    class WorkWithDB
    {
        public static Logger nLogger = LogManager.GetCurrentClassLogger();

        // TODO: REFACTOR THIS !!!!!!!!
        // TODO: REFACTOR THIS !!!!!!!!
        // TODO: REFACTOR THIS !!!!!!!!
        private bool DbIsExists(string databaseName, string userName, string passw)
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


        public string CreateSprusnbuTable(string databaseName, string tableName, string userName, string passw, string fileDbfFolder, string fileDbf)
        {
            bool tabelIsExists = false;

            string connectString = "Data Source=MAIN;uid=" + userName + ";password=" + passw + ";database=" + databaseName;

            string rezultMsg = "DataBase is Unreachable or Login Failed!";

            if (DbIsExists(databaseName, userName, passw))
            {
                SqlConnection sqlConnect = new SqlConnection(connectString);

                string tableQuery = @"select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='" + tableName + "'";

                string methodName = MethodInfo.GetCurrentMethod().Name;
                try
                {
                    // TODO: FIND OUT WHAT IS IT FOR ???
                    // TODO: FIND OUT WHAT IS IT FOR ???
                    // TODO: FIND OUT WHAT IS IT FOR ???
                    //string cmd = string.Format(tableQuery, "DISPLAY_STAT");

                    using (sqlConnect)
                    {
                        sqlConnect.Open();

                        using (SqlCommand sqlCmd = new SqlCommand(tableQuery, sqlConnect))
                        {
                            var rez = sqlCmd.ExecuteScalar();
                            if (rez != null)
                            {
                                tabelIsExists = true;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    nLogger.Error(methodName + "() - " + exc.Message);
                    rezultMsg = "ERR! - " + exc.Message;
                }

                //  IF TABLE NOT EXISTS - TRYING TO CREATE :
                if (tabelIsExists)
                {
                    rezultMsg = "OK - Table " + tableName + " Is Already Exists.";
                }
                else
                {
                    try
                    {
                        // TODO:  W A R N I N G  SPECIAL FOR SPRUSNBU ONLY !!!!!!
                        // TODO:  W A R N I N G  SPECIAL FOR SPRUSNBU ONLY !!!!!!
                        // TODO:  W A R N I N G  SPECIAL FOR SPRUSNBU ONLY !!!!!!

                        string creatingTablCmd = "USE Andrew2 CREATE TABLE [" + tableName + "] (" +
                            " [IDHOST] nvarchar(255)," +
                            " [IDHFIRST] nvarchar(255)," +
                            " [FNHOST] nvarchar(255)," +
                            " [WHATUS] nvarchar(255)," +
                            " [TELE] nvarchar(255)," +
                            " [MFOM] nvarchar(255)," +
                            " [OKPO] nvarchar(255)," +
                            " [GROUP] nvarchar(255)," +
                            " [KTELE] nvarchar(255)," +
                            " [KFASE] nvarchar(255)," +
                            " [TYPESEND] nvarchar(255)," +
                            " [Y_N_SEND] nvarchar(255)," +
                            " [P_M_B] nvarchar(255)," +
                            " [N_PATH] nvarchar(255)," +
                            " [N_ADD_PATH] nvarchar(255)," +
                            " [UID] uniqueidentifier NOT NULL DEFAULT (newid())" +
                            " ) ON [PRIMARY];";

                        // CREATING EMPTY TABLE :
                        using (sqlConnect = new SqlConnection(connectString))
                        {
                            sqlConnect.Open();
                            using (SqlCommand sqlcmd = new SqlCommand(creatingTablCmd, sqlConnect))
                            {
                                sqlcmd.ExecuteNonQuery();
                            }
                        }

                        // UPLOADING DATA FROM SPRUSNBU.DBF FILE :
                        FillSprusnbuFromDbf(databaseName, userName, passw, tableName, fileDbfFolder, fileDbf);

                        // FIXING UKR. DOS-LIKE CHARACTERS :
                        //      if (ch == '°') text2 += 'Ї';
                        // else if (ch == 'Ў') text2 += 'І';
                        // else if (ch == 'ў') text2 += 'і';
                        string charsFixCmd = "UPDATE " + tableName + " SET FNHOST = REPLACE(FNHOST, '°', 'Ї')" +
                            "UPDATE " + tableName + " SET FNHOST = REPLACE(FNHOST, 'Ў', 'І')" +
                            "UPDATE " + tableName + " SET KFASE = REPLACE(KFASE, '°', 'Ї')" +
                            "UPDATE " + tableName + " SET KFASE = REPLACE(KFASE, 'Ў', 'І')";

                        using (sqlConnect = new SqlConnection(connectString))
                        {
                            sqlConnect.Open();
                            using (SqlCommand sqlcmd = new SqlCommand(charsFixCmd, sqlConnect))
                            {
                                sqlcmd.ExecuteNonQuery();
                            }

                        }

                        rezultMsg = "Table - " + tableName + " is Created Successfully.";

                    }
                    catch (Exception exc)
                    {
                        nLogger.Error(methodName + "() - " + exc.Message);
                        rezultMsg = "ERR! - " + exc.Message;
                    }
                    //finally
                    //{
                    //    if (sqlConnect.State == SqlConnection.State.Open)
                    //    {
                    //        sqlConnect.Close();
                    //    }
                    //}
                }
            }
            return rezultMsg;
        }


        private void FillSprusnbuFromDbf(string db, string usr, string psw, string table, string fileDbfFolder, string fileDbf)
        {
            OleDbConnection oleConn = new OleDbConnection();
            SqlConnection sqlConn = new SqlConnection();
            DataTable dTabl = new DataTable();

            oleConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileDbfFolder + ";Extended Properties=dBASE IV";

            sqlConn.ConnectionString = @"Data Source=MAIN;uid=" + usr + ";password=" + psw + ";database=" + db;

            try
            {
                oleConn.Open();
                sqlConn.Open();

                // CLEAR SPRUSNBU TABLE BEFORE FILLING IT !!!

                SqlCommand sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandText = "DELETE FROM " + table;
                sqlCmd.ExecuteNonQuery();

                SqlBulkCopy bulk = new SqlBulkCopy(sqlConn);
                bulk.DestinationTableName = table;
                bulk.BatchSize = 100000;

                OleDbCommand oleCmd = oleConn.CreateCommand();
                oleCmd.CommandText = "SELECT * FROM " + fileDbfFolder + "\\" + fileDbf + "\"";
                dTabl.Load(oleCmd.ExecuteReader());

                bulk.WriteToServer(dTabl);

                // SPECIAL FOR MY FIELDS TO DIVIDE BETWEEN DATES

                // SQL : INSERT INTO SPRUSNBU$ (IDHOST) VALUES ('U000')

            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
            finally
            {
                oleConn.Close();
                sqlConn.Close();
            }
        }
    }
}
