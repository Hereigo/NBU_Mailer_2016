namespace Plakhtiy_NB_Mailer_Test
{
    using System;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;

    class Sprusnbu2sql
    {
        public void SprusnbuUpdateInSql(string db, string usr, string psw, string table, string fileDbfFolder, string fileDbf)
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

                // CLEAR SPRUSNBU TABLE !!!

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
