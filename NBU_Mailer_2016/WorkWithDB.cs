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
using System.IO;

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

        #endregion


        private bool DbIsExists()
        {
            bool dbExists = false;

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

                        dbExists = (databaseID > 0);
                    }
                }
            }
            catch (Exception e)
            {
                nLogger.Error("{0}() - {1}", methodName, e.Message);
                dbExists = false;
            }

            return dbExists;
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


        // CREATING EMPTY TABLE (NOT USED TEMPORARY !!!) :
        private string CreateTableInDB(string tableName)
        {
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
            }
            catch (Exception e)
            {
                rezulMsg = "WARNING! - " + e.Message;
                nLogger.Error("{0}() - {1}", methodName, e.Message);
            }

            return rezulMsg;
        }


        // UPLOAD DBF INTO DB AND FIX CHARACTERS :
        public string UpdateSprusnbuFromDbf(string tableForUpdate, string fileDbfFolder, string fileDbf)
        {
            string methodName = MethodInfo.GetCurrentMethod().Name;

            string rez = "Starting Check DB & Tables...";

            if (!TableIsExists(tableForUpdate))
            {
                rez = "DB or TABLE not Exists OR Login Failed !";
            }
            else
            {
                rez = "Start Rebuilding...";

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

                rez = rez + " & " + FixDosCharsInSprusnbu(tableForUpdate);
            }

            return rez;
        }


        // FIXING UKR. DOS-CHARACTERS :
        private string FixDosCharsInSprusnbu(string tableName)
        {
            string rez = "Start Chars Updating...";

            //      if (ch == '°') text2 += 'Ї';
            // else if (ch == 'Ў') text2 += 'І';
            // else if (ch == 'ў') text2 += 'і';
            string charsFixCmd =
                "UPDATE " + tableName + " SET FNHOST = REPLACE(FNHOST, '°', 'Ї')" +
                "UPDATE " + tableName + " SET FNHOST = REPLACE(FNHOST, 'Ў', 'І')" +
                "UPDATE " + tableName + " SET KFASE = REPLACE(KFASE, '°', 'Ї')" +
                "UPDATE " + tableName + " SET KFASE = REPLACE(KFASE, 'Ў', 'І')" +
                "INSERT INTO " + tableName + " (IDHOST)VALUES('U000')";
            //   TEMPORARY: SPECIAL FOR MY FIELDS TO DIVIDE BETWEEN DATES
            //   TEMPORARY: SPECIAL FOR MY FIELDS TO DIVIDE BETWEEN DATES
            //   TEMPORARY: SPECIAL FOR MY FIELDS TO DIVIDE BETWEEN DATES

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


        public int EnvelopeUpload(string envelopeTable, Envelope env)
        {
            int execRezult = 0;

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                string fileBodyParam1;
                string fileBodyParam2;
                byte[] fileBody;

                // IF FILE NOT FOUND :
                if (!File.Exists(env.fileLocation))
                {
                    fileBody = new byte[1];
                    fileBodyParam1 = "";
                    fileBodyParam2 = "";
                }
                else
                { 
                    FileStream fs = new FileStream(env.fileLocation, FileMode.Open, FileAccess.Read);
                    int fileSize = (int)fs.Length;
                    fileBody = new byte[fileSize];
                    fs.Read(fileBody, 0, fileSize);

                    fs.Close();

                    fileBodyParam1 = ", FILE_BODY";
                    fileBodyParam2 = ", @FILE_BODY";
                }

                //	[FROM] nvarchar(15),
                //	[TO] nvarchar(15),
                //	[FILE_NAME] nvarchar(15),
                //	[FILE_SIZE] int,
                //	[FILE_BODY] IMAGE,
                //	[FILE_DATE] datetime,
                //	[DATE_SENT] datetime,
                //	[DATE_DELIV] datetime,
                //	[ENV_NAME] nvarchar(15),
                //	[ENV_PATH] nvarchar(255),

                string insertString = "INSERT INTO " + envelopeTable +
                " ([FROM], [TO], [FILE_NAME], [FILE_SIZE], [FILE_DATE], [DATE_SENT], [DATE_DELIV], [ENV_NAME], [ENV_PATH] " +
                fileBodyParam1 + ") VALUES" +
                " (@FROM, @TO, @FILE_NAME, @FILE_SIZE, @FILE_DATE, @DATE_SENT, @DATE_DELIV, @ENV_NAME, @ENV_PATH" +
                fileBodyParam2 + ")";

                using (SqlConnection sqlConn = new SqlConnection(_CONNSTR))
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = new SqlCommand(insertString, sqlConn))
                    {
                        cmd.Parameters.AddWithValue("@FILE_BODY", fileBody);
                        cmd.Parameters.AddWithValue("@FROM", env.sendFromAddress);
                        cmd.Parameters.AddWithValue("@TO", env.recieveAddress);
                        cmd.Parameters.AddWithValue("@FILE_NAME", env.fileName);
                        cmd.Parameters.AddWithValue("@FILE_SIZE", Convert.ToInt32(env.fileSize));
                        cmd.Parameters.AddWithValue("@FILE_DATE", env.fileModified);
                        cmd.Parameters.AddWithValue("@DATE_SENT", env.fileSent);
                        cmd.Parameters.AddWithValue("@DATE_DELIV", env.fileDelivered);
                        cmd.Parameters.AddWithValue("@ENV_NAME", env.envelopeName);
                        cmd.Parameters.AddWithValue("@ENV_PATH", env.envelopePath);

                        execRezult = cmd.ExecuteNonQuery();

                        sqlConn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                execRezult = 0;
                nLogger.Error("{0}() - {1}", methodName, e.Message);
            }

            return execRezult;

        }
    }
}
