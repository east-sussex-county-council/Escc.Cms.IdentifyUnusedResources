using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace Escc.Cms.IdentifyUnusedResources
{
    class Program
    {
        // REMEMBER: if copying logging code, set assembly attribute for Log4Net in AssemblyInfo.cs
        private static ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Help();
                    return;
                }

                // Read the original report generated from CMS
                var sourceFile = args[0];
                var report = String.Empty;
                using (var reader = new StreamReader(sourceFile))
                {
                    report = reader.ReadToEnd();
                }

                // Extract the URLs of the unused resources
                var unusedResources = Regex.Matches(report, "Path: /Resources(?<URL>[^\n]+)\nPage Count: 0", RegexOptions.Singleline);
                var listOfResources = new StringBuilder();

                foreach (Match m in unusedResources)
                {
                    listOfResources.Append(m.Groups["URL"].Value);
                    Console.WriteLine(m.Groups["URL"].Value);
                }

                // Send it as an email
                using (var message = new MailMessage())
                {
                    message.To.Add(args[1]);
                    message.From = new MailAddress(Environment.MachineName + "@eastsussex.gov.uk");
                    message.Subject = "CMS unused resources";
                    message.Body = listOfResources.ToString();

                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(message);
                }

                Console.WriteLine(unusedResources.Count + " unused resources found. Email sent to " + args[1]);
                log.Info(unusedResources.Count + " unused resources found. Email sent to " + args[1]);
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                log.Error(ex.Message);
            }
        }

        private static void Help()
        {
            Console.WriteLine("Usage: Escc.Cms.IdentifyUnusedResources.exe sourceFile emailTo");
        }
    }
}
