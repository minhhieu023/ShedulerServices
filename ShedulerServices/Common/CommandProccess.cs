using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using MailKit.Net.Smtp;

using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ShedulerServices.Common
{
    public static class CommandProccess
    {
        public static void Command(string testFilePath,
                            string reportPath, 
                            string logs)
        {
           // var tcs = new TaskCompletionSource<int>();

            string javaFileStandAlone = Path.Combine(@".\FileScheduler\"+
                                      "test1.jar");
            string javaCommand =
                String.Format(@"-jar ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}""",
                javaFileStandAlone, testFilePath, reportPath, logs, "0");
            var startTime = DateTime.UtcNow;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "java.exe";
                        startInfo.Arguments = javaCommand;
                        process.StartInfo = startInfo;
            //process.Exited += (sender, args) =>
            //{
            //    tcs.SetResult(process.ExitCode);
            //    process.Dispose();
            //};
            process.Start();
            process.WaitForExit();
            Console.WriteLine("ExitCode: " + process.ExitCode);
            process.Close();

        }
        public static void SendEmail(string destEmail,
                              string fileName,
                              string pjName,
                              string funcName,
                              DateTime startTime,
                              DateTime endTime,
                              string reportFilePath)
        {
            var mailContents = "All of your test case of file '{0}' had been run throught. Please check the details result below. <br /> Project: {1} <br /> Function: {2} <br /> Testcase File: {0} <br /> Start time: {3} <br /> Complete time: {4} <br /> Total run time: {5} <br />";
            TimeSpan duration = endTime - startTime;
            var durationStr = string.Format("{0:00}h {1:00}m {2:00}s", duration.Hours, duration.Minutes, duration.Seconds);

            var multipart = new Multipart("mixed");
            var readableFilename = fileName.Split('-')[0];
            var textPart = new TextPart(TextFormat.Html)
            {
                Text = string.Format(mailContents, readableFilename + ".csv", pjName, funcName, startTime.ToString(), endTime.ToString(), durationStr),
                ContentTransferEncoding = ContentEncoding.Base64,
            };
            multipart.Add(textPart);

            MemoryStream fileStream = new MemoryStream(File.ReadAllBytes(reportFilePath));
            fileStream.Position = 0;

            var attachmentPart = new MimePart();
            var readableAttachFilename = Path.GetFileName(reportFilePath).Split('-')[0] + ".html";
            attachmentPart.Content = new MimeContent(fileStream);
            attachmentPart.ContentId = readableAttachFilename;
            attachmentPart.ContentTransferEncoding = ContentEncoding.Base64;
            attachmentPart.FileName = readableAttachFilename;
            multipart.Add(attachmentPart);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("testappsend99@gmail.com"));
            email.To.Add(MailboxAddress.Parse(destEmail));
            email.Subject = "Your test had been done";
            email.Body = multipart;

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("testappsend99@gmail.com", @"123QWEasd?!");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
