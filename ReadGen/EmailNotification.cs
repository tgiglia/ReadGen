using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace ReadGen
{
    class EmailNotification
    {
        public void testEmail(ConfigInfo cd)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient server = new SmtpClient(cd.emailAddress);
                mail.From = new MailAddress(cd.emailFrom);
                mail.To.Add(cd.emailTo);

                mail.Subject = "Test Email";
                mail.Body = "This is a for testing SMTP mail from Brewster AWS.";

                server.Port = 25;
                server.EnableSsl = false;
                server.Host = cd.emailAddress;
                server.Credentials = new System.Net.NetworkCredential("tom.giglia", "RIA.45acp");
                server.Send(mail);
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        public bool sendEmail(ConfigInfo cd, String body, String subject)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient server = new SmtpClient(cd.emailAddress);
                mail.From = new MailAddress(cd.emailFrom);
                mail.To.Add(cd.emailTo);

                mail.Subject = subject;
                mail.Body = body;

                server.Port = 25;
                server.EnableSsl = false;
                server.Host = cd.emailAddress;
                server.Credentials = new System.Net.NetworkCredential("tom.giglia", "RIA.45acp");
                server.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }
            return true;
        }
        public bool sendEmailWithCCList(ConfigInfo cd, String body, String subject, List<String> addresses)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient server = new SmtpClient(cd.emailAddress);
                mail.From = new MailAddress(cd.emailFrom);
                mail.To.Add(cd.emailTo);
                if (addresses.Count > 0)
                {
                    foreach (String s in addresses)
                    {
                        MailAddress copy = new MailAddress(s);
                        mail.CC.Add(copy);
                    }
                }
                mail.Subject = subject;
                mail.Body = body;

                server.Port = 25;
                server.EnableSsl = false;
                server.Host = cd.emailAddress;
                server.Credentials = new System.Net.NetworkCredential("tom.giglia", "RIA.45acp");
                server.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return true;
        }

    }
}
