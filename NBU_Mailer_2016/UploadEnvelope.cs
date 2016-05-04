using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace NBU_Mailer_2016
{
    class UploadEnvelope
    {
        public int EnvelopeUpload(string envelopeTable, Envelope env, string _CONNSTR)
        {
            int execRezult = 0;
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

                // CREATE TABLE[NBU_ENVELOPES] (
                // [ID] INT NOT NULL IDENTITY(1000,1) PRIMARY KEY,
                // [FROM] nvarchar(15),
                // [TO] nvarchar(15),
                // [FILE_NAME] nvarchar(15),
                // [FILE_SIZE] INT,
                // [FILE_BODY] IMAGE,
                // [FILE_DATE] datetime,
                // [DATE_SENT] datetime,
                // [DATE_DELIV] datetime,
                // [ENV_NAME] nvarchar(15),
                // [ENV_PATH] nvarchar(255),
                // [SPRUSNBU_BANK_ID] INT,
                // 
                // INSERT INTO NBU_ENVELOPES([FROM], [TO], [FILE_NAME], FILE_SIZE, FILE_BODY, FILE_DATE, DATE_SENT, DATE_DELIV, ENV_NAME, ENV_PATH, SPRUSNBU_BANK_ID)
                // VALUES('Sender1', 'Reciever1', 'FileName1', 333, '1/1/1', '1/1/1', '1/1/1', 'E_1_UYTX.ERT', 'Some-path', 1)


                // SELECT [id] FROM SPRUSNBU_BANKS WHERE [idhost] = 'ubla'

                string insertString = "INSERT INTO " + envelopeTable +
                " ([FROM], [TO], [FILE_NAME], [FILE_SIZE], [FILE_DATE], [DATE_SENT], [DATE_DELIV], [ENV_NAME], [ENV_PATH],"+
                " [SPRUSNBU_BANK_ID]" + fileBodyParam1 + ") VALUES" +
                " (@FROM, @TO, @FILE_NAME, @FILE_SIZE, @FILE_DATE, @DATE_SENT, @DATE_DELIV, @ENV_NAME, @ENV_PATH," +
                " @SPRUSNBU_BANK_ID" + fileBodyParam2 + ")";

                string currentIdHost = env.sendFromAddress.Substring(env.sendFromAddress.Length - 4);

                // TODO : FULL REBUILD !!!!!!!!
                // TODO : FULL REBUILD !!!!!!!!
                // TODO : FULL REBUILD !!!!!!!!
                // TODO : FULL REBUILD !!!!!!!!

                //string getIdForHost = "SELECT [ID] FROM SPRUSNBU_BANKS WHERE [IDHOST]='" + currentIdHost + "'";

                // RETURN  -1  !!!!!!
                // RETURN  -1  !!!!!!
                // RETURN  -1  !!!!!!
                // RETURN  -1  !!!!!!
                // RETURN  -1  !!!!!!

                string getIdForHost = "SELECT[ID] FROM SPRUSNBU_BANKS WHERE[IDHOST] = 'UiXQ'";

                // TODO : SETUP DEFAULT + UPLOAD INTO DB RECORDS WITHOUT OKPO or MFO !!!!!!
                // TODO : SETUP DEFAULT + UPLOAD INTO DB RECORDS WITHOUT OKPO or MFO !!!!!!
                // TODO : SETUP DEFAULT + UPLOAD INTO DB RECORDS WITHOUT OKPO or MFO !!!!!!
                // TODO : SETUP DEFAULT + UPLOAD INTO DB RECORDS WITHOUT OKPO or MFO !!!!!!

                MessageBox.Show("QUERY : " + getIdForHost);
                
                using (SqlConnection sqlConn = new SqlConnection(_CONNSTR))
                {
                    sqlConn.Open();

                    int IdForHost = 1;

                    using (SqlCommand cmd = new SqlCommand(getIdForHost, sqlConn))
                    {
                        IdForHost = cmd.ExecuteNonQuery();

                        MessageBox.Show("REZULT ID = " + IdForHost);
                    }

                    // ?????
                    // ?????
                    // ?????
                    sqlConn.Close();
                    sqlConn.Open();

                    using (SqlCommand cmd = new SqlCommand(insertString, sqlConn))
                    {
                        cmd.Parameters.AddWithValue("@SPRUSNBU_BANK_ID", 13);
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
                MessageBox.Show(e.Message);
            }

            return execRezult;
        }
    }
}
