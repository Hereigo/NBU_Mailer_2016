using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NBU_Mailer_2016
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // SET ALL PATHES HERE :

        // TODO: WARNING !!!
        // TODO: WARNING !!!
        // TODO: WARNING !!!

        const string _DATABASE = "Andrew";

        const string _SPRUSNBU_TBL = "SPRUSNBU$";

        // TODO: WARNING !!!
        // TODO: WARNING !!!
        // TODO: WARNING !!!

        public static Logger nLogger = LogManager.GetCurrentClassLogger();


        // - USEFUL CODE-SNIPPET :)  !!!!!!!!!!!!!!!
        // - USEFUL CODE-SNIPPET :)  !!!!!!!!!!!!!!!
        //
        // using NLog;
        // using System.Reflection;
        // public static Logger nLogger = LogManager.GetCurrentClassLogger();
        //
        // string methodName = MethodInfo.GetCurrentMethod().Name;
        // try{ }
        // catch (Exception e) { 
        //     nLogger.Error("{0}() - {1}", methodName, e.Message);
        // }


        const string settsFile = "NBU_Mailer_2016.ini";

        static string NbuRootDir;

        static string envelTodayDirName = "\\ARH\\";

        static string envelTodayShortPath;


        // RUN ALL INITIALIZERS & START TIMER :
        public MainWindow()
        {
            InitializeComponent();

            InitializeStartParams();

            textBox_4_Tests_Only.Text = Environment.NewLine + "Database = " + _DATABASE + " !!!" + 
                Environment.NewLine + Environment.NewLine + "Table is = " + _SPRUSNBU_TBL + " !!!";

            DispatcherTimer dispTimer = new DispatcherTimer();
            dispTimer.Interval = new TimeSpan(0, 15, 0); // 15 minutes
            dispTimer.Tick += RunEveryFifteenMin;
            dispTimer.Start();

            labelForTimer.Content = "Next Autorun at " + DateTime.Now.AddMinutes(15).ToShortTimeString();
        }


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
                nLogger.Error(methodName + "() - " + exc.Message);
            }

        }


        // RUN TASKS USING SHCHEDULER :
        private void RunEveryFifteenMin(object sender, EventArgs e)
        {
            ProcessEnvelopes();
        }


        // UNPACK ENVELOPES AND SHOW ALL PARAMS :
        private void ProcessEnvelopes()
        {
            labelForTimer.Content = "Next Autorun at - " + DateTime.Now.AddMinutes(15).ToShortTimeString();

            textBox_4_Tests_Only.Text = "TODAY ENVELOPES:";

            FileInfo[] todayEnvelopes = GetEnvelopesListForDate(dataPicker.SelectedDate.Value);

            if (todayEnvelopes != null && todayEnvelopes.Length > 0)
            {
                for (int i = 0; i < todayEnvelopes.Length; i++)
                {
                    //  CREATE ENVELOPES FROM EVERY ENVELOPE-FILE :
                    Envelope env = new Envelope(todayEnvelopes[i]);

                    // UPLOAD INTO DB


                    // WRITE IN LOF IF OK.


                    // DEBUGGING OUTPUT TO TEXTBOX !!!
                    // DEBUGGING OUTPUT TO TEXTBOX !!!
                    // DEBUGGING OUTPUT TO TEXTBOX !!!
                    textBox_4_Tests_Only.Text += Environment.NewLine + "====================================";
                    //foreach (PropertyInfo propInfo in env.GetType().GetProperties())
                    //    textBox_4_Tests_Only.Text += Environment.NewLine + propInfo.GetValue(env, null);
                    textBox_4_Tests_Only.Text += Environment.NewLine + "en.envelopeName - " + env.envelopeName;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "en.envelopePath - " + env.envelopePath;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "e.fileDelivered - " + env.fileDelivered;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "en.fileLocation - " + env.fileLocation;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "e..fileModified - " + env.fileModified;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "env....fileName - " + env.fileName;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "env....fileSent - " + env.fileSent;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "env....fileSize - " + env.fileSize;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "recieve_Address - " + env.recieveAddress;
                    textBox_4_Tests_Only.Text += Environment.NewLine + "sendFromAddress - " + env.sendFromAddress;
                }
            }
            else
            {
                textBox_4_Tests_Only.Text += Environment.NewLine + "====================================" +
                    Environment.NewLine + Environment.NewLine + "No New Envelopes For Selected Date.";
            }
        }


        // SETTS-FILE WITH STRT DIR :
        private void InitializeStartParams()
        {
            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                // TODO: CHECK BD - CREATE IF NOT EXISTS !!!
                // TODO: CHECK BD - CREATE IF NOT EXISTS !!!
                // TODO: CHECK BD - CREATE IF NOT EXISTS !!!

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
                        MessageBox.Show("Start Folder Set As - " + NbuRootDir);
                    }
                    else
                    {
                        MessageBox.Show("Start Folder Is Not Set! Set It Before Use!");
                    }
                }
            }
            catch (Exception exc)
            {
                nLogger.Error(methodName + "() - " + exc.Message);
            }
        }


        // GET LIST OF ALL TODAY ENVELOPES:
        private FileInfo[] GetEnvelopesListForDate(DateTime dt)
        {
            // SET TODAY ENVELOPES PATH EVERY RUN BECAUSE DEPENDS ON DATE !!!

            FileInfo[] todayEnvelopes;

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                envelTodayShortPath = "A" + dt.ToString("ddMMyy") + "\\";

                todayEnvelopes = new DirectoryInfo(NbuRootDir +
                    envelTodayDirName + envelTodayShortPath).GetFiles();
            }
            catch (Exception exc)
            {
                todayEnvelopes = null;
                nLogger.Error(methodName + "() - " + exc.Message);
            }

            return todayEnvelopes;
        }


        // UNPACK ENVELOPES AND SHOW ALL PARAMS :
        private void btnShowSelectedDateEnv_Click(object sender, RoutedEventArgs e)
        {
            ProcessEnvelopes();
        }


        // "SPRUSNBU" FROM DBF:
        private void btnSprusnbuUpd_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".dbf";
            dlg.Filter = "DBF Files Only (*.dbf)|*.dbf";

            bool? dbfFileSelected = dlg.ShowDialog();

            if (dbfFileSelected == true)
            {
                FileInfo dbfFile = new FileInfo(dlg.FileName);

                string login = textBoxForSqlLogin.Text.Trim();
                string passw = textBoxForSqlPassword.Text.Trim();

                if (login.Length < 1 || passw.Length < 1)
                {
                    MessageBox.Show("Set Login & Password Before!");
                }
                else
                {
                    WorkWithDB workWithBD = new WorkWithDB(_DATABASE, login, passw);

                    MessageBox.Show(workWithBD.UpdateSprusnbuFromDbf(_SPRUSNBU_TBL,
                        dbfFile.Directory.ToString(), dbfFile.Name));
                }
            }
        }



        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!
        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!
        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!

        // 1. IF NOT SUNDAY

        // 2. IF LATER THAN 23:00 - Send/Upload Log + Create ALL BackUps IF NOT YET!

        // 3. IF EARLIER THAN 23:00

        //      - 

    }
}
