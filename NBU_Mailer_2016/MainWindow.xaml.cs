using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Threading;

namespace NBU_Mailer_2016
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    [Serializable]
    class AppSettings : ISerializable
    {
        public string srvName;
        public string dbName;
        public string tabSprus;
        public string tabInbox;
        public string tabOutbox;

        public AppSettings()
        {
            srvName = "";
            dbName = "";
            tabSprus = "";
            tabInbox = "";
            tabOutbox = "";
        }

        public AppSettings(SerializationInfo info, StreamingContext context)
        {
            srvName = info.GetString("srvName");
            dbName = info.GetString("dbName");
            tabSprus = info.GetString("tabSprus");
            tabInbox = info.GetString("tabInbox");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("srvName", srvName);
            info.AddValue("dbName", dbName);
            info.AddValue("tabSprus", tabSprus);
            info.AddValue("tabInbox", tabInbox);
        }
    }

    public partial class MainWindow : Window
    {
        static string _SERVER = "";
        static string _DATABASE = "";
        static string _SPRUSNBU_2014 = "SPRUSNBU$";
        static string _SPRUSNBU_2016 = "";
        static string _ENVELOPE_TBL = "";

        string appConfigFile = "C:\\NBUMAIL\\NBU_Mailer_2016.cfg";
        Stream stream;
        AppSettings setts = new AppSettings();
        BinaryFormatter binFormatter = new BinaryFormatter();

        private void btn_LoadSqlConfig_Click(object sender, RoutedEventArgs e)
        {
            LoadSqlConnectSettings();
        }

        private void LoadSqlConnectSettings()
        {
            try
            {
                if (!File.Exists(appConfigFile))
                {
                    MessageBox.Show("В директории NBUMAIL файл NBU_Mailer_2016.cfg не найден!");
                }
                else
                {
                    if (MessageBox.Show("Загрузить параметры из конфиг-файла?", "ВНИМАНИЕ!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        stream = File.Open(appConfigFile, FileMode.Open);
                        setts = (AppSettings)binFormatter.Deserialize(stream);
                        stream.Close();

                        _SERVER = setts.srvName;
                        txt_Server.Text = setts.srvName;

                        _DATABASE = setts.dbName;
                        txt_Database.Text = setts.dbName;

                        _SPRUSNBU_2016 = setts.tabSprus;
                        txt_Sprusnbu.Text = setts.tabSprus;

                        _ENVELOPE_TBL = setts.tabInbox;
                        txt_TabInbox.Text = setts.tabInbox;

                        MessageBox.Show("Настройки загружены.");
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_SaveSqlConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Перезаписать конфиг-файл текущими параметрами?",
                    "ВНИМАНИЕ!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    setts.srvName = txt_Server.Text;
                    setts.dbName = txt_Database.Text;
                    setts.tabSprus = txt_Sprusnbu.Text;
                    setts.tabInbox = txt_TabInbox.Text;

                    stream = File.Open(appConfigFile, FileMode.Create);
                    binFormatter.Serialize(stream, setts);
                    stream.Close();

                    MessageBox.Show("Настройки сохранены.");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static byte timerMinutes = 15;  // 15 minutes should be by default.

        string textBoxClearHeader = Environment.NewLine + "SET LOGIN AND PASSWORD BEFORE USE !!!" +
                Environment.NewLine + Environment.NewLine + "TODAY ENVELOPES:" + Environment.NewLine +
            "============================================";

        public static Logger nLogger = LogManager.GetCurrentClassLogger();

        const string settsFile = "NBU_Mailer_2016.ini";

        static string NbuRootDir;

        static string envelTodayDirName = "\\ARH\\";

        static string envelTodayShortPath;

        static string[] possibleOutputDirs = {
            "\\USERD\\",
            "\\USERD\\Admin\\",
            "\\USERD\\Admin\\APPL\\",
            "\\USERD\\Admin\\unknown\\",
            "\\USERD\\NBU\\APPL\\",
            "\\USERD\\NBU\\unknown\\",
            "\\USERD\\Comp\\",
            "\\USERD\\Comp\\APPL\\",
            "\\USERD\\Vsem\\",
            "\\USERD\\Vsem\\APPL\\",
            "\\USERD\\Vsem\\unknown\\",
            "\\USERD\\unknown\\",
            "\\USERD\\TEST\\"
                //  DEBUGGING !!!!!!!!!!!!!!
                //  DEBUGGING !!!!!!!!!!!!!!
                //  DEBUGGING !!!!!!!!!!!!!!
                //  DEBUGGING !!!!!!!!!!!!!!
                //  DEBUGGING !!!!!!!!!!!!!!
        };

        // SELECT START FOLDER PATH :
        private void btnSelectStartDir_Click(object sender, RoutedEventArgs e)
        {
            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                var foldBrowsDlg = new System.Windows.Forms.FolderBrowserDialog();

                System.Windows.Forms.DialogResult FBDResult = foldBrowsDlg.ShowDialog(this.GetIWin32Window());

                if (FBDResult == System.Windows.Forms.DialogResult.OK)
                {
                    string path = foldBrowsDlg.SelectedPath;
                    File.WriteAllText(settsFile, path);

                    NbuRootDir = path;

                    // SprusDbfFor2016 = NbuRootDir + "\\SPRUSNBU.DBF";

                    textBoxForStartDir.Text = path;
                }
            }
            catch (Exception exc)
            {
                nLogger.Error("{0}() - {1}", methodName, exc.Message);
            }
        }


        // SETTS-FILE WITH START DIR :
        private void InitializeStartParams()
        {
            string methodName = MethodInfo.GetCurrentMethod().Name;

            LoadSqlConnectSettings();

            try
            {
                if (!File.Exists(settsFile))
                {
                    MessageBox.Show("Start Folder Is Not Set! Set It Before Use!");
                }
                else
                {
                    string readedString = File.ReadAllText(settsFile).Trim();

                    if (Directory.Exists(readedString))
                    {
                        NbuRootDir = readedString;
                        textBoxForStartDir.Text = NbuRootDir;
                    }
                    else
                    {
                        MessageBox.Show("Start Folder Is Not Set! Set It Before Use!");
                    }
                }
            }
            catch (Exception e)
            {
                nLogger.Error("{0}() - {1}", methodName, e.Message);
            }
        }


        // RUN ALL INITIALIZERS & START TIMER :
        public MainWindow()
        {
            InitializeComponent();

            InitializeStartParams();

            textBox_4_Tests_Only.Text = textBoxClearHeader;

            DispatcherTimer dispTimer = new DispatcherTimer();
            dispTimer.Interval = new TimeSpan(0, timerMinutes, 0);
            dispTimer.Tick += RunEveryFifteenMin;
            dispTimer.Start();

            labelForTimer.Content = "Run Every " + timerMinutes + " min. Next Autorun at " +
                DateTime.Now.AddMinutes(timerMinutes).ToString("HH:mm:ss");
        }


        // RUN TASK IMMEDIATELY & RESET DATE TO CURREND DATE :
        private void btnShowSelectedDateEnv_Click(object sender, RoutedEventArgs e)
        {
            ProcessEnvelopes();
            dataPicker.SelectedDate = DateTime.Now;
        }


        // RUN TASKS USING SHCHEDULER :
        private void RunEveryFifteenMin(object sender, EventArgs e)
        {
            // DON'T RUN UNTIL 7 O'CLOCK FOR WAITHING ALL BACKUP TASKS HAS FINISHED :

            if (DateTime.Now.Hour > 7)
            {
                // TODO:  TEMPORARY AUTO SPRUSNBU UPLOADER !!!!!
                // TODO:  TEMPORARY AUTO SPRUSNBU UPLOADER !!!!!
                // TODO:  TEMPORARY AUTO SPRUSNBU UPLOADER !!!!!

                string dir4sprusnbu = "C:\\NBUMAIL\\SPRUSNBD4SQL\\";

                string log4sprusnbu = "NBU_Mailer_2016_Sprusnbu_Upload.log";

                string currentSprusFile = "SPRUSNBU.DBF";

                string todaySprusFilePath = dir4sprusnbu + DateTime.Now.ToString("yy-MM-dd") + ".SPRUSNBU.DBF";

                if ((DateTime.Now.Hour == 11 || DateTime.Now.Hour == 15) && DateTime.Now.Minute > 40)
                {
                    try
                    {
                        if (!Directory.Exists(dir4sprusnbu)) Directory.CreateDirectory(dir4sprusnbu);

                        bool currSprusAlreadyLoad = false;

                        FileInfo[] oldSprusFiles = new DirectoryInfo(dir4sprusnbu).GetFiles("*.dbf");

                        DateTime currSprusDate = new FileInfo("C:\\NBUMAIL\\" + currentSprusFile).LastWriteTime;

                        // MessageBox.Show(oldSprusFiles.Length + " files in " + dir4sprusnbu);

                        foreach (FileInfo file in oldSprusFiles)
                        {
                            if (file.LastWriteTime == currSprusDate)
                            {
                                currSprusAlreadyLoad = true;
                                break;
                            }
                        }

                        if (!currSprusAlreadyLoad)
                        {
                            // CREATE TEMPORARY FILE FOR ODBC UPLOADING :
                            File.Copy("C:\\NBUMAIL\\" + currentSprusFile, dir4sprusnbu + currentSprusFile);

                            string dbLogin = passwordBoxLogin.Password.Trim();
                            string dbPassw = passwordBoxPassw.Password.Trim();

                            // UPLOAD IN SQL - UPDATE 2016 !!!
                            // UPLOAD IN SQL - UPDATE 2016 !!!

                            string table = "SPRUSNBU_BANKS";

                            string connString = "Server=" + _SERVER + "; Database=" + _DATABASE + "; Uid=" + dbLogin + "; Pwd=" + dbPassw + "";

                            UploadDbfIntoSql uploadDbf = new UploadDbfIntoSql();


                            // UPLOAD 2 =SPRUSNBU= 4 NEW 2016 :

                            string uploadRez = uploadDbf.ReadDbfAndInsert(dir4sprusnbu + currentSprusFile, _DATABASE, table, connString);

                            File.AppendAllText(log4sprusnbu, "\r\n" + DateTime.Now + " - " + uploadRez);

                            // UPLOAD 2 =SPRUSNBU_BANKS= 4 OLD 2015

                            // UPLOAD IN SQL - OVERWRITE 2014 !!!
                            // UPLOAD IN SQL - OVERWRITE 2014 !!!

                            WorkWithDB_2015 workWithDB = new WorkWithDB_2015(_DATABASE, dbLogin, dbPassw);

                            string uploadRez2014 = workWithDB.UpdateSprusnbuFromDbf(_SPRUSNBU_2014, dir4sprusnbu, currentSprusFile);

                            File.AppendAllText(log4sprusnbu, "\r\n" + DateTime.Now + " - " + uploadRez2014);

                            System.Threading.Thread.Sleep(4000);

                            // STORE TEMPORARY FILE FOR NEXT TIME CHECKING :
                            File.Move(dir4sprusnbu + currentSprusFile, todaySprusFilePath);
                        }
                    }
                    catch (Exception exc)
                    {
                        File.AppendAllText(log4sprusnbu, "\r\n" + DateTime.Now + " - " + exc.Message);
                    }
                }

                ///////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////

                ProcessEnvelopes();
            }
        }


        // UNPACK ENVELOPES AND UPLOAD INTO DB :
        private void ProcessEnvelopes()
        {
            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                string todayUploadedLog = DateTime.Now.ToString("yyyyMMdd") + "_Uploaded.log";

                // POSSIBLE A NEW DAY STARTED & NEW LOG FILE NEEDED :

                if (!File.Exists(todayUploadedLog))
                {
                    File.Create(todayUploadedLog);
                    textBox_4_Tests_Only.Text = textBoxClearHeader;
                    dataPicker.SelectedDate = DateTime.Now;
                }
                else
                {
                    string dbLogin = passwordBoxLogin.Password.Trim();
                    string dbPassw = passwordBoxPassw.Password.Trim();

                    if (dbLogin.Length < 1 || dbPassw.Length < 1)
                    {
                        MessageBox.Show("Set Login & Password Before!");
                    }
                    else
                    {
                        WorkWithDB_2015 workWithDB = new WorkWithDB_2015(_DATABASE, dbLogin, dbPassw);

                        labelForTimer.Content = "Run Every " + timerMinutes + " min. Next Autorun at " +
                                        DateTime.Now.AddMinutes(timerMinutes).ToString("HH:mm:ss");
                        // "Next Autorun at - " + DateTime.Now.AddMinutes(15).ToShortTimeString();

                        // GET TODAY ENVELOPES FROM TODAY-FOLDER (OR ANOTHER SELECTED DATE) :

                        FileInfo[] todayEnvelopes = GetEnvelopesListForDate(dataPicker.SelectedDate.Value);

                        if (todayEnvelopes != null && todayEnvelopes.Length > 0)
                        {
                            for (int i = 0; i < todayEnvelopes.Length; i++)
                            {
                                // CREATE ENVELOPE USING EVERY ENVELOPE-FILE PARAMS :

                                Envelope env = new Envelope(todayEnvelopes[i]);
                                // TODO:  SOMETHING WRONG...
                                // TODO:  SOMETHING WRONG...
                                // TODO:  SOMETHING WRONG...
                                env.fileLocation = "FILE NOT FOUND !!!";

                                // CHECK IN LOG FILE IF NOT UPLOADED YET :

                                if (!File.ReadAllText(todayUploadedLog).Contains(env.envelopeName))
                                {
                                    // GET UNPACKED FILES (LOOKING IN ALL POSSIBLE FOLDERS) :

                                    foreach (string possiblePath in possibleOutputDirs)
                                    {
                                        string outputFilePath = NbuRootDir + possiblePath + env.fileName;

                                        if (File.Exists(outputFilePath))
                                        {
                                            env.fileLocation = outputFilePath;
                                            break;
                                        }
                                    }

                                    // START UPLOAD INTO DB HERE !!! :
                                    UploadEnvelope upload = new UploadEnvelope();

                                    string connString = "Server=" + _SERVER + "; Database=" + _DATABASE + "; Uid=" + dbLogin + "; Pwd=" + dbPassw + "";

                                    if (upload.EnvelopeUpload(_ENVELOPE_TBL, env, connString) != 0)
                                    // if (workWithDB.EnvelopeUpload(_ENVELOPE_TBL, env) != 0)  // OLD 2015 VERSION;
                                    {
                                        // IT MEANS - UPLOADED SUCCESSFULLY !

                                        File.AppendAllText(todayUploadedLog, env.envelopeName + " - " + env.fileName + Environment.NewLine);

                                        nLogger.Trace("{0}() - {1}", env.envelopeName, env.fileName);

                                        textBox_4_Tests_Only.Text += Environment.NewLine + DateTime.Now + " - " + env.envelopeName + " - " + env.fileName;
                                    }
                                }
                            }
                        }
                        nLogger.Trace("Job finished successfully.");

                        textBox_4_Tests_Only.Text += Environment.NewLine + DateTime.Now + " - Ok.";
                    }
                }
            }
            catch (Exception e)
            {
                nLogger.Error("{0}() - {1}", methodName, e.Message);
            }
        }


        // GET LIST OF ALL TODAY ENVELOPES:
        private FileInfo[] GetEnvelopesListForDate(DateTime dt)
        {
            // SET TODAY ENVELOPES PATH EVERY RUN BECAUSE IT DEPENDS ON DATE !!!

            FileInfo[] todayEnvelopes;

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                envelTodayShortPath = "A" + dt.ToString("ddMMyy") + "\\";

                todayEnvelopes = new DirectoryInfo(NbuRootDir +
                    envelTodayDirName + envelTodayShortPath).GetFiles();
            }
            catch (Exception e)
            {
                todayEnvelopes = new FileInfo[0];
                nLogger.Error("{0}() - {1}", methodName, e.Message);
            }

            return todayEnvelopes;
        }


        // MANUAL !!! UPLOAD "SPRUSNBU" INTO !!! OLD PROM !!! SQL FROM DBF:
        private void btnSprusnbuUpd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".dbf";
                dlg.Filter = "DBF Files Only (*.dbf)|*.dbf";

                bool? dbfFileSelected = dlg.ShowDialog();

                if (dbfFileSelected == true)
                {
                    FileInfo dbfFile = new FileInfo(dlg.FileName);

                    string dbLogin = passwordBoxLogin.Password.Trim();
                    string dbPassw = passwordBoxPassw.Password.Trim();

                    if (dbLogin.Length < 1 || dbPassw.Length < 1)
                    {
                        MessageBox.Show("Set Login & Password Before!");
                    }
                    else
                    {
                        WorkWithDB_2015 workWithDB = new WorkWithDB_2015(_DATABASE, dbLogin, dbPassw);

                        MessageBox.Show(workWithDB.UpdateSprusnbuFromDbf(_SPRUSNBU_2014,
                            dbfFile.Directory.ToString(), dbfFile.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }


        // MANUAL !!! UPLOAD "SPRUSNBU" INTO NEW 2016 SQL FROM DBF:
        private void NewSprusnbuIntoSql_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".dbf";
                dlg.Filter = "DBF Files Only (*.dbf)|*.dbf";

                bool? dbfFileSelected = dlg.ShowDialog();

                if (dbfFileSelected == true)
                {
                    //FileInfo dbfFile = new FileInfo(dlg.FileName);

                    string dbLogin = passwordBoxLogin.Password.Trim();
                    string dbPassw = passwordBoxPassw.Password.Trim();

                    if (dbLogin.Length < 1 || dbPassw.Length < 1)
                    {
                        MessageBox.Show("Set Login & Password Before!");
                    }
                    else
                    {
                        // W A R N I N G !!!!!!!!!!!!!!!!!!!!
                        // W A R N I N G !!!!!!!!!!!!!!!!!!!!
                        // W A R N I N G !!!!!!!!!!!!!!!!!!!!
                        // W A R N I N G !!!!!!!!!!!!!!!!!!!!
                        // W A R N I N G !!!!!!!!!!!!!!!!!!!!

                        

                        string connString = "Server=" + _SERVER + "; Database=" + _DATABASE + "; Uid=" + dbLogin + "; Pwd=" + dbPassw + "";

                        UploadDbfIntoSql uploadDbf = new UploadDbfIntoSql();

                        string uploadRez = uploadDbf.ReadDbfAndInsert(dlg.FileName, _DATABASE, _SPRUSNBU_2016, connString);

                        MessageBox.Show(uploadRez);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }


        // JUST OPEN LOG FILE FOR VIEW :
        private void btnViewUploaded_Click(object sender, RoutedEventArgs e)
        {
            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                string todayUploadedLog = DateTime.Now.ToString("yyyyMMdd") + "_Uploaded.log";

                if (File.Exists(todayUploadedLog))
                {
                    Process.Start(todayUploadedLog);
                }
                else
                {
                    MessageBox.Show(todayUploadedLog + " Not Found!");
                }
            }
            catch (Exception ex)
            {
                nLogger.Error("{0}() - {1}", methodName, ex.Message);
            }
        }



        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!
        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!
        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!

        // 1. IF NOT SUNDAY

        // 2. IF LATER THAN 23:00 - Send/Upload Log + Create ALL BackUps IF NOT YET!

        // 3. IF EARLIER THAN 23:00

        //      - 

        // using NLog;
        // using System.Reflection;
        // public static Logger nLogger = LogManager.GetCurrentClassLogger();
        //
        //
        // string methodName = MethodInfo.GetCurrentMethod().Name;
        // try{ }
        // catch (Exception e) { 
        //     nLogger.Error("{0}() - {1}", methodName, e.Message);
        // }
    }
}
