using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NLog;

namespace NBU_Mailer_2016
{
    struct Envelope
    {
        public static Logger nLogger = LogManager.GetCurrentClassLogger();

        public string envelopePath { get; }
        public string envelopeName { get; }
        public string sendFromAddress { get; }
        public string recieveAddress { get; }
        public string fileName { get; }
        public ulong fileSize { get; }
        public DateTime fileModified { get; }
        public DateTime fileSent { get; }
        public DateTime fileDelivered { get; }

        public string fileLocation { get; set; }

        // private string fileCategory;

        public Envelope(FileInfo envelope)
        {
            envelopePath = envelope.DirectoryName;
            envelopeName = envelope.Name;
            fileSize = (ulong)envelope.Length;
            sendFromAddress = "";
            recieveAddress = "";
            fileName = "";
            fileLocation = "";
            fileModified = DateTime.Parse("01.01.01 01:01:01");
            fileSent = DateTime.Parse("01.01.01 01:01:01");
            fileDelivered = DateTime.Parse("01.01.01 01:01:01");

            string methodName = MethodInfo.GetCurrentMethod().Name;
            try
            {
                FileStream fs = new FileStream(envelope.FullName, FileMode.Open, FileAccess.Read);

                StreamReader sr = new StreamReader(fs);
                string readOneLine;

                while ((readOneLine = sr.ReadLine()) != "" && readOneLine != null)
                {
                    if (readOneLine.Contains("FROM:"))
                    {
                        Match match = Regex.Match(readOneLine, @"[\S]{2,8}[@][U][^,]{3}", RegexOptions.IgnoreCase);
                        if (match.Success) sendFromAddress = match.ToString();
                    }
                    else if (readOneLine.Contains("TO:"))
                    {
                        Match matchTo = Regex.Match(readOneLine, @"[\S]{2,8}[@][U][^,]{3}", RegexOptions.IgnoreCase);
                        Match matchFN = Regex.Match(readOneLine, @"F:([^,]){2,16}", RegexOptions.IgnoreCase);
                        Match matchFS = Regex.Match(readOneLine, @"C:\d{8}", RegexOptions.IgnoreCase);
                        Match matchFC = Regex.Match(readOneLine, @"D:\d{12}", RegexOptions.IgnoreCase);

                        if (matchTo.Success) recieveAddress = matchTo.ToString();
                        if (matchFN.Success) fileName = matchFN.ToString().Substring(2);
                        if (matchFS.Success) fileSize = ulong.Parse(matchFS.ToString().Substring(2));
                        if (matchFC.Success)
                        {
                            string tempStr = matchFC.ToString().Substring(2);
                            string recreated = tempStr.Substring(0, 2) + "-" + tempStr.Substring(2, 2) + "-" + tempStr.Substring(4, 2) +
                                " " + tempStr.Substring(6, 2) + ":" + tempStr.Substring(8, 2) + ":" + tempStr.Substring(10, 2);
                            fileModified = DateTime.Parse(recreated);
                        }
                    }
                    else if (readOneLine.Contains("DATE:"))
                    {
                        fileSent = DateTime.Parse(readOneLine.Substring(6));
                    }
                    else if (readOneLine.Contains("DATE-DELIVERED:"))
                    {
                        fileDelivered = DateTime.Parse(readOneLine.Substring(16));
                    }
                    else
                    {
                        // DO NOTHING  :)
                    }
                }
            }
            catch (Exception e)
            {
                nLogger.Error("{0}() - {1}", methodName, e.Message);
            }


            //if (fileDelivered == DateTime.Parse("01.01.01 01:01:01"))
            //    fileDelivered = DateTime.Parse(DateTime.Now.ToString("dd.MM.yy HH:mm:ss"));

            //GetFileLocation(dirForUnpacked);

            //   RE-CHECK EXTENSION !!!!!!!!!!!
            //   RE-CHECK EXTENSION !!!!!!!!!!!
            //   RE-CHECK EXTENSION !!!!!!!!!!!

        }
    }
}
