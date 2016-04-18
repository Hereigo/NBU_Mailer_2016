﻿using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace NBU_Mailer_2016
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region SET ALL PATHES HERE :

        // TODO: WARNING !!!
        // TODO: WARNING !!!
        // TODO: WARNING !!!

        const string _DATABASE = "Andrew2"; // ANDREW

        const string _SPRUSNBU_TBL = "SPRUSNBU"; // SPRUSNBU$

        const string _ENVELOPE_TBL = "ENVELOPES";

        readonly string textBoxClearHeader = Environment.NewLine + "SET LOGIN AND PASSWORD BEFORE USE !!!" +
                Environment.NewLine + Environment.NewLine + "Database = " + _DATABASE + " !!!" +
                Environment.NewLine + Environment.NewLine + "Table is = " + _SPRUSNBU_TBL + " !!!" +
                Environment.NewLine + Environment.NewLine + "TODAY ENVELOPES:";

        // TODO: WARNING !!!
        // TODO: WARNING !!!
        // TODO: WARNING !!!

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
        #endregion


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

            byte timerMinutes = 15;  // 15 minutes should be by default.

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
                        WorkWithDB workWithDB = new WorkWithDB(_DATABASE, dbLogin, dbPassw);

                        labelForTimer.Content = "Next Autorun at - " + DateTime.Now.AddMinutes(15).ToShortTimeString();

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

                                    // IT MEANS - UPLOADED SUCCESSFULLY !
                                    if (workWithDB.EnvelopeUpload(_ENVELOPE_TBL, env) != 0)
                                    {
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


        // MANUAL !!! UPLOAD "SPRUSNBU" INTO SQL FROM DBF:
        private void btnSprusnbuUpd_Click(object sender, RoutedEventArgs e)
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
                    WorkWithDB workWithDB = new WorkWithDB(_DATABASE, dbLogin, dbPassw);

                    MessageBox.Show(workWithDB.UpdateSprusnbuFromDbf(_SPRUSNBU_TBL,
                        dbfFile.Directory.ToString(), dbfFile.Name));
                }
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
