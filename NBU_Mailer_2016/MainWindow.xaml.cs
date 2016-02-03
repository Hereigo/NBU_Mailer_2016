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

        static string envelopesTodayPath;


        public MainWindow()
        {
            InitializeComponent();
            InitializeParameters();

            // TEMPORARY DEBUGGING !!!!!!!
            // TEMPORARY DEBUGGING !!!!!!!
            // TEMPORARY DEBUGGING !!!!!!!
            foreach (var item in GetTodayEnvelopesList())
                textBox_4_Tests_Only.Text += Environment.NewLine + item;
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


        private List<string> GetTodayEnvelopesList()
        {
            // SET TODAY ENVELOPES PATH EVERY RUN BECAUSE DEPENDS ON DATE !!!

            envelopesTodayPath = NbuRootDir + "ARH\\A" + DateTime.Now.ToString("ddMMyy") + "\\";

            List<string> todayNewEnvelopes = new List<string>();

            foreach (FileInfo envelope in new DirectoryInfo(envelopesTodayPath).GetFiles())
            {
                todayNewEnvelopes.Add(envelope.Name);
            }

            return todayNewEnvelopes;
        }


        private void RunEveryTenMinutes()
        {
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
