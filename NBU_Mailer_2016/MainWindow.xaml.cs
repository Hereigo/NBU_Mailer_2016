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
            InitializeParameters();

            // TODO: Inject TIMER !
            // TODO: Inject TIMER !
            // TODO: Inject TIMER !

            RunEveryTenMinutes();
        }


        // - USEFUL CODE-SNIPPET :)  !!!!!!!!!!!!!!!
        // - USEFUL CODE-SNIPPET :)  !!!!!!!!!!!!!!!
        //
        // string methodName = MethodInfo.GetCurrentMethod().Name;
        // try{ }
        // catch (Exception exc) { 
        //     nLogger.Error(methodName + "() - " + exc.Message);
        // }


        // TODO: REFACTORE IT USING FOLDER-OPEN-DIALOG !!!
        private void InitializeParameters()
        {
            string methodName = MethodInfo.GetCurrentMethod().Name;

            string settsFile = "NBU_Mailer_2016.txt";
            try
            {
                if (!File.Exists(settsFile))
                {
                    File.Create(settsFile);
                    MessageBox.Show("Write Correct Full Path For NbuMail Files In First Line And Save It!");
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
        private FileInfo[] GetTodayEnvelopesList()
        {
            // SET TODAY ENVELOPES PATH EVERY RUN BECAUSE DEPENDS ON DATE !!!

            envelTodayShortPath = "A" + DateTime.Now.ToString("ddMMyy") + "\\";

            FileInfo[] todayEnvelopes =
                new DirectoryInfo(NbuRootDir + envelTodayDirName + envelTodayShortPath).GetFiles();

            return todayEnvelopes;
        }


        // RUN QUERY OF ALL SHEDULLED TASKS :
        private void RunEveryTenMinutes()
        {
            // TEMPORARY DEBUGGING !!!!!!!
            // TEMPORARY DEBUGGING !!!!!!!
            // TEMPORARY DEBUGGING !!!!!!!

            FileInfo[] todayEnvelopes = GetTodayEnvelopesList();

            if (todayEnvelopes.Length > 0)
            {
                for (int i = 0; i < todayEnvelopes.Length; i++)
                {
                    //  FILL ENVELOPE PROPS FOR EVERY ENVEL-FILE:

                    Envelope env = new Envelope(todayEnvelopes[i]);

                    textBox_4_Tests_Only.Text += Environment.NewLine + "===================================";
                    foreach (PropertyInfo propInfo in env.GetType().GetProperties())
                    {
                        textBox_4_Tests_Only.Text += Environment.NewLine + propInfo.GetValue(env, null);
                    }
                }
            }
            else
            {
                MessageBox.Show("No New Envelopes.");
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
}
