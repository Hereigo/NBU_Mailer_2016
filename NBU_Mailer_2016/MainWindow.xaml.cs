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

namespace NBU_Mailer_2016
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // SET ALL PATHES HERE :

        // TODO: STATIC CLASS FOR STATIC FIELDS FOR SETTINGS

        public static Logger nLogger = LogManager.GetCurrentClassLogger();

        static string NbuRootDir;

        static string envelTodayDirName = "ARH\\";

        static string envelTodayShortPath;


        public MainWindow()
        {
            InitializeComponent();

            InitializeStartParams();

            // TODO: Inject TIMER !
            // TODO: Inject TIMER !
            // TODO: Inject TIMER !

            RunEveryTenMinutes();
        }


        // - USEFUL CODE-SNIPPET :)  !!!!!!!!!!!!!!!
        // - USEFUL CODE-SNIPPET :)  !!!!!!!!!!!!!!!
        //
        // using NLog;
        // using System.Reflection;
        // public static Logger nLogger = LogManager.GetCurrentClassLogger();
        //
        // string methodName = MethodInfo.GetCurrentMethod().Name;
        // try{ }
        // catch (Exception exc) { 
        //     nLogger.Error(methodName + "() - " + exc.Message);
        // }


        // SET: START DIR, SQL DB, etc...
        private void InitializeStartParams()
        {
            string settsFile = "NBU_Mailer_2016.txt";

            // TODO: CREATE BD - IF NOT EXISTS !!!
            // TODO: CREATE BD - IF NOT EXISTS !!!
            // TODO: CREATE BD - IF NOT EXISTS !!!

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                if (!File.Exists(settsFile))
                {
                    File.Create(settsFile);
                    MessageBox.Show("Write Correct Full Path For NbuMail Files In First Line And Save It!");
                    // TODO: REFACTORE IT USING FOLDER-OPEN-DIALOG !!!
                    // TODO: REFACTORE IT USING FOLDER-OPEN-DIALOG !!!
                    Process.Start(settsFile);
                    Application.Current.Shutdown();
                }
                else
                {
                    string readedString = File.ReadAllText(settsFile).Trim();

                    if (Directory.Exists(readedString))
                    {
                        NbuRootDir = readedString;
                        MessageBox.Show("Start Folder  ==  " + NbuRootDir);
                    }
                    else
                    {
                        MessageBox.Show("Write Correct Full Path For NbuMail Files In First Line And Save It!");
                        // TODO: REFACTORE IT USING FOLDER-OPEN-DIALOG !!!
                        // TODO: REFACTORE IT USING FOLDER-OPEN-DIALOG !!!
                        // TODO: REFACTORE IT USING FOLDER-OPEN-DIALOG !!!
                        Process.Start(settsFile);
                        Application.Current.Shutdown();
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

            envelTodayShortPath = "A" + dt.ToString("ddMMyy") + "\\";

            FileInfo[] todayEnvelopes =
                new DirectoryInfo(NbuRootDir + envelTodayDirName + envelTodayShortPath).GetFiles();

            return todayEnvelopes;
        }


        // RUN QUERY OF ALL SHEDULLED TASKS :
        private void RunEveryTenMinutes()
        {

        }


        private void btnShowSelectedDateEnv_Click(object sender, RoutedEventArgs e)
        {
            textBox_4_Tests_Only.Text = "TODAY ENVELOPES:";

            FileInfo[] todayEnvelopes = GetEnvelopesListForDate(dataPicker.SelectedDate.Value);

            if (todayEnvelopes.Length > 0)
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
                    textBox_4_Tests_Only.Text += Environment.NewLine + "===================================";
                    foreach (PropertyInfo propInfo in env.GetType().GetProperties())
                        textBox_4_Tests_Only.Text += Environment.NewLine + propInfo.GetValue(env, null);
                }
            }
            else
            {
                textBox_4_Tests_Only.Text += Environment.NewLine + "===================================" +
                    "No New Envelopes.";
            }
        }

        private void btnCheckDB_Click(object sender, RoutedEventArgs e)
        {
            WorkWithDB workWithBD = new WorkWithDB();

            if (workWithBD.IfDbExists("test", "test", "test"))
            {
                MessageBox.Show("DB EXISTS !");
            }
            else
            {
                MessageBox.Show("DB NOT EXISTS !");
            }
        }


        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!
        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!
        // TODO: CHECK LOG IF ENVEL NOT LOADED YET !!!

        // 1. IF NOT SUNDAY

        // 2. IF LATER THAN 23:00 - Send/Upload Log + Create ALL BackUps IF NOT YET!

        // 3. IF EARLIER THAN 23:00

        //      - 

        //      - 

        //      - 

        //      - 





    }
}
