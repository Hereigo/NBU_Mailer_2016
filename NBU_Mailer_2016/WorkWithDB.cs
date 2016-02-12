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

        private readonly string _DATABASE;
        private readonly string _USER;
        private readonly string _PASS;
        private readonly string _CONNSTR;

        public WorkWithDB(string database, string user, string passw)
        {
            _DATABASE = database;
            _USER = user;
            _PASS = passw;
            _CONNSTR = "Data Source=MAIN;uid=" + _USER + ";password=" + _PASS + ";database=" + _DATABASE;
        }


        #region SQL TABLES SCRIPTS :

        private const string sprunbuDescr =
            "(" +
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

        private const string partnersDescr =
            "(" +
            " [UID] uniqueidentifier NOT NULL DEFAULT (newid())" +
            " ) ON [PRIMARY];";

        #endregion


        private bool DbIsExists()
        {
            bool bdExists = false;

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                string sqlDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", _DATABASE);

                SqlConnection dbConnect = new SqlConnection(_CONNSTR);

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


        private bool TableIsExists(string tableName)
        {
            bool tableIsExists = false;

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                if (DbIsExists())
                {
                    SqlConnection sqlConnect = new SqlConnection(_CONNSTR);

                    string tableQuery = @"select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='" + tableName + "'";

                    try
                    {
                        using (sqlConnect)
                        {
                            sqlConnect.Open();

                            using (SqlCommand sqlCmd = new SqlCommand(tableQuery, sqlConnect))
                            {
                                var rez = sqlCmd.ExecuteScalar();
                                if (rez != null)
                                {
                                    tableIsExists = true;
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        nLogger.Error(methodName + "() - " + exc.Message);
                    }

                    tableIsExists = true;
                }
            }
            catch (Exception exc)
            {
                tableIsExists = false;
                nLogger.Error(methodName + "() - " + exc.Message);
            }

            return tableIsExists;
        }

        // CREATING EMPTY TABLE :
        // public string CreateTableInDB(string tableName, string createCommand)
        public string CreateTableInDB()
        {
            string tableName = "SPRUSNBU";
            string createCommand = sprunbuDescr;

            string rezulMsg = "Attempt To Create Table " + tableName + " Starting...";

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                //TODO: FINDOUT W.T.F.???!!!
                //TODO: FINDOUT W.T.F.???!!!
                //TODO: FINDOUT W.T.F.???!!!

                //if (TableIsExists(tableName))
                //{
                //    rezulMsg = tableName + " is already exists!";
                //}
                //else
                //{

                using (SqlConnection sqlConnect = new SqlConnection(_CONNSTR))
                {
                    string creatingTablCmd = "USE " + _DATABASE +
                        " CREATE TABLE [" + tableName + "] " + createCommand;

                    sqlConnect.Open();

                    using (SqlCommand sqlcmd = new SqlCommand(creatingTablCmd, sqlConnect))
                    {
                        sqlcmd.ExecuteNonQuery();
                    }
                }
                rezulMsg = tableName + " - Created Successfully.";

                //}

                // TODO: SPECIAL FOR SPRUSNBU ONLY !!!
                // TODO: SPECIAL FOR SPRUSNBU ONLY !!!
                // TODO: SPECIAL FOR SPRUSNBU ONLY !!!
                // TODO: SPECIAL FOR SPRUSNBU ONLY !!!

                //FillSprusnbuFromDbf("SPRUSNBU", "D:\\_SPRUSNBU", "SPRUSNBU.DBF");

                //UpdateCharsInSprusnbu("SPRUSNBU");
            }
            catch (Exception exc)
            {
                rezulMsg = "WARNING! - " + exc.Message;
                nLogger.Error(methodName + "() - " + exc.Message);
            }

            try
            {

            }
            catch (Exception exc)
            {
                rezulMsg = "DBF ERROR! - " + exc.Message;
                nLogger.Error(methodName + "() - " + exc.Message);
            }

            return rezulMsg;
        }


        public string FillSprusnbuFromDbf(string tableForUpdate, string fileDbfFolder, string fileDbf)
        {
            string rez = "Start Rebuilding...";

            string methodName = MethodInfo.GetCurrentMethod().Name;

            string oleConnectionStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                fileDbfFolder + ";Extended Properties=dBASE IV";

            OleDbConnection oleConn = new OleDbConnection(oleConnectionStr);
            SqlConnection sqlConn = new SqlConnection(_CONNSTR);
            DataTable dTabl = new DataTable();

            try
            {
                oleConn.Open();
                sqlConn.Open();

                // CLEAR SPRUSNBU TABLE BEFORE FILLING IT !!!

                SqlCommand sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandText = "DELETE FROM " + tableForUpdate;
                sqlCmd.ExecuteNonQuery();

                SqlBulkCopy bulk = new SqlBulkCopy(sqlConn);
                bulk.DestinationTableName = tableForUpdate;
                bulk.BatchSize = 100000;

                OleDbCommand oleCmd = oleConn.CreateCommand();
                oleCmd.CommandText = "SELECT * FROM " + fileDbfFolder + "\\" + fileDbf + "\"";
                dTabl.Load(oleCmd.ExecuteReader());

                bulk.WriteToServer(dTabl);

                // TODO: SPECIAL FOR MY FIELDS TO DIVIDE BETWEEN DATES
                // SQL : INSERT INTO SPRUSNBU$ (IDHOST) VALUES ('U000')

                rez = "DBF Uploaded.";
            }
            catch (Exception exc)
            {
                rez = "ERR! - " + exc.Message;
                nLogger.Error(methodName + "() - " + exc.Message);
            }
            finally
            {
                oleConn.Close();
                sqlConn.Close();
            }
            return rez;
        }



        // FIXING UKR. DOS-CHARACTERS :
        public string UpdateCharsInSprusnbu(string tableForUpdate)
        {
            string rez = "Start Chars Updating...";

            //      if (ch == '°') text2 += 'Ї';
            // else if (ch == 'Ў') text2 += 'І';
            // else if (ch == 'ў') text2 += 'і';
            string charsFixCmd = "UPDATE " + tableForUpdate + " SET FNHOST = REPLACE(FNHOST, '°', 'Ї')" +
                "UPDATE " + tableForUpdate + " SET FNHOST = REPLACE(FNHOST, 'Ў', 'І')" +
                "UPDATE " + tableForUpdate + " SET KFASE = REPLACE(KFASE, '°', 'Ї')" +
                "UPDATE " + tableForUpdate + " SET KFASE = REPLACE(KFASE, 'Ў', 'І')";

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                using (SqlConnection sqlConnect = new SqlConnection(_CONNSTR))
                {
                    sqlConnect.Open();

                    using (SqlCommand sqlcmd = new SqlCommand(charsFixCmd, sqlConnect))
                    {
                        sqlcmd.ExecuteNonQuery();
                    }
                }
                rez = "Characters Replaced.";
            }
            catch (Exception exc)
            {
                rez = "ERR! - " + exc.Message;
                nLogger.Error(methodName + "() - " + exc.Message);
            }
            return rez;
        }

    }
}
