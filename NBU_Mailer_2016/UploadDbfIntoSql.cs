namespace NBU_Mailer_2016
{
    using System;
    using System.Data;
    using System.Data.Odbc;
    using System.Data.SqlClient;

    class UploadDbfIntoSql
    {
        // READ DBF-FILE & INSERT NEW OR UPDATE EXISTING RECORDS IN SQL DB RELYING ON [IDHOST] FIELD :

        public string ReadDbfAndInsert(string dbfFilePath, string db, string table, string connString)
        {
            //string createTablCmd = "USE " + db + " CREATE TABLE [" + table + "] " + "(" +
            //            " [IDHOST] nvarchar(6) NOT NULL," +
            //            " [FNHOST] nvarchar(50) NOT NULL," +
            //            " [MFOM] integer," +
            //            " [OKPO] integer," +
            //            " [KTELE] nvarchar(30)," +
            //            " [KFASE] nvarchar(50)," +
            //            " [PARTNER] bit," +
            //            " [UID] uniqueidentifier NOT NULL DEFAULT (newid())" +
            //            " ) ON [PRIMARY];";
            string retStr = "Trying to start...";
            try
            {
                if (!TableIsExists(table, connString))
                {
                    retStr = "Not Found DB Table : " + table;
                }
                else
                {
                    OdbcConnection connect = new OdbcConnection("Driver={Microsoft dBase Driver (*.dbf)};SourceType=DBF;SourceDB=" +
                            dbfFilePath + ";Exclusive=No; Collate=Machine;NULL=NO;DELETED=NO;BACKGROUNDFETCH=NO;");

                    connect.Open();

                    OdbcCommand cmd = new OdbcCommand("SELECT * FROM " + dbfFilePath, connect);

                    OdbcDataReader dataRdr = cmd.ExecuteReader();

                    // INCOMING_DBF_FILE - MAKET :
                    //   0        1        2       3       4    5     6      7      8      9      10
                    // IDHOST, IDHFIRST, FNHOST, WHATUS, TELE, MFOM, OKPO, GROUP, KTELE, KFASE,  . . .

                    if (dataRdr.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(dataRdr);

                        using (SqlConnection sqlConnect = new SqlConnection(connString))
                        {
                            sqlConnect.Open();

                            string insertUpdateCmd = "";

                            // row[dt.Columns[0]] - IDHOST - nvarchar(6)
                            // row[dt.Columns[2]] - FNHOST - nvarchar(50)
                            // row[dt.Columns[5]] - MFOM  - integer
                            // row[dt.Columns[6]] - OKPO  - integer
                            // row[dt.Columns[8]] - KTELE - nvarchar(30)
                            // row[dt.Columns[9]] - KFASE - nvarchar(50)

                            foreach (DataRow row in dt.Rows)
                            {
                                int mfom = String.IsNullOrEmpty(row[dt.Columns[5]].ToString()) ? 0 : Convert.ToInt32(row[dt.Columns[5]]);
                                int okpo = String.IsNullOrEmpty(row[dt.Columns[6]].ToString()) ? 0 : Convert.ToInt32(row[dt.Columns[6]]);

                                // IF CURRENT RECORD HAS MFO OR OKPO :
                                // OR may be it is NEEDLESS ?
                                // if (mfom > 0 || okpo > 0)
                                // {

                                string idHost = row[dt.Columns[0]].ToString(); // ID !!!

                                string fnHost = row[dt.Columns[2]].ToString().ToString().Replace('\'', '=').Replace('"', '=');
                                string kTele = row[dt.Columns[8]].ToString().ToString().Replace('\'', '=').Replace('"', '=');
                                string kFase = row[dt.Columns[9]].ToString().ToString().Replace('\'', '=').Replace('"', '=');

                                // UPDATE Table1 SET(…) WHERE Column1 =’SomeValue’
                                // IF @@ROWCOUNT = 0
                                // INSERT INTO Table1 VALUES (…)

                                insertUpdateCmd = "UPDATE " + table + " SET " +
                                                "MFOM = '" + mfom + "', OKPO = '" + okpo + "', FNHOST = '" + fnHost +
                                                "', KTELE = '" + kTele + "', KFASE = '" + kFase +
                                                "' WHERE IDHOST = '" + idHost +
                                                "' IF @@ROWCOUNT = 0 INSERT INTO " + table +
                                                " (IDHOST, MFOM, OKPO, KTELE, FNHOST, KFASE, PARTNER) VALUES (" +
                                                "'" + idHost + "', " + "'" + mfom + "', " + "'" + okpo + "', " +
                                                "'" + kTele + "', " + "'" + fnHost + "', " + "'" + kFase + "', " + 0 + ")";
                                // TODO: USE STRINGBUILDER !!!
                                // TODO: USE STRINGBUILDER !!!
                                // TODO: USE STRINGBUILDER !!!

                                using (SqlCommand sqlcmd = new SqlCommand(insertUpdateCmd, sqlConnect))
                                {
                                    sqlcmd.ExecuteNonQuery();
                                }
                                // TODO: WRITE LOG OF CHANGES !!!
                                // TODO: WRITE LOG OF CHANGES !!!
                                // TODO: WRITE LOG OF CHANGES !!!
                                //}
                            }


                            insertUpdateCmd = "UPDATE " + table + " SET " +
                                "KTELE = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' WHERE IDHOST = 'U000'";
                            
                            using (SqlCommand sqlcmd = new SqlCommand(insertUpdateCmd, sqlConnect))
                            {
                                sqlcmd.ExecuteNonQuery();
                            }



                            // ALL ROWS INSERTED. NOW REPLACE SPECIFIC DOS SYMBOLS IN THEM :

                            string charsFixCmd = "UPDATE " + table + " SET FNHOST = REPLACE(FNHOST, '=', '\"')" +
                                                 "UPDATE " + table + " SET FNHOST = REPLACE(FNHOST, '°', 'Ї')" +
                                                 "UPDATE " + table + " SET FNHOST = REPLACE(FNHOST, 'Ў', 'І')" +
                                                 "UPDATE " + table + " SET KFASE = REPLACE(KFASE, '=', '\"')" +
                                                 "UPDATE " + table + " SET KFASE = REPLACE(KFASE, '°', 'Ї')" +
                                                 "UPDATE " + table + " SET KFASE = REPLACE(KFASE, 'Ў', 'І')";

                            using (SqlCommand sqlcmd = new SqlCommand(charsFixCmd, sqlConnect))
                            {
                                sqlcmd.ExecuteNonQuery();
                            }
                        }
                    }
                    connect.Close();

                    retStr = table + " updated succesfully.";
                }
            }
            catch (Exception exc)
            {
                //Console.WriteLine(exc.Message);
                retStr = exc.Message;
            }

            return retStr;
        }


        private bool TableIsExists(string tableName, string connString)
        {
            bool tableIsExists = false;
            try
            {
                SqlConnection sqlConnect = new SqlConnection(connString);

                string tableQuery = @"select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='" + tableName + "'";

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
                // TODO: INFORM ABOUT EXCEPTION !!!
                // TODO: INFORM ABOUT EXCEPTION !!!
                // TODO: INFORM ABOUT EXCEPTION !!!

                tableIsExists = false;
            }
            return tableIsExists;
        }
    }
}
