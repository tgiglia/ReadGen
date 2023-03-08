using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("Usage: ReadGen <app config> <environment config>");
                Console.WriteLine("args length: " + args.Length);
                return;
            }


            //Console.WriteLine("Congradulations you entered enough command line arguments!");
            ConfigInfo ci = new ConfigInfo(args[0], args[1]);
            if(!ci.Load())
            {
                Console.WriteLine("Error Config Load Failed: " + ci.errorDesc);
                return;
            }

            Console.WriteLine("Application Configuration\n" + ci.ac.ToString() + "\n");
            Console.WriteLine("Environment Configuration\n" + ci.ec.ToString());
            Console.WriteLine("Read File: we have: " + ci.rc.Reads.Count + " reads in the file.");
            Console.WriteLine(ci.cameras.Count + " Entries in the camera file.");
            Console.WriteLine(ci.alarmUsers.Count + " Entries in the alarm users file.");
            Console.WriteLine(ci.plateLookups.Count + " Entries in the plate lookup file.");            
            ReadGenProcesser rgp = AbstractFactory(ci);
            //Execute the processing
            ProcessingReturn pr = rgp.executeProcess(ci);
            Console.WriteLine("Execution Status: " + pr.status);
            Console.WriteLine("Execution Description: " + pr.description);
            if(pr.status != 0)
            {
                sendFailureNotifications(ci, pr);
            }
            /*Console.WriteLine("Press any key to end.");
            Console.ReadKey(); */


        }

        static void sendFailureNotifications(ConfigInfo ci,ProcessingReturn pr)
        {
            if(ci.ac.notifyfile == null)
            {
                Console.WriteLine("sendFailureNotifications: There is no notifyfile field. Cannot send email notifications.");
                return;
            }
            if(ci.ac.emailRecipients == null)
            {
                Console.WriteLine("sendFailureNotifications: No email recipients have been configured. Cannot send email notifications.");
                return;
            }
            if(ci.ac.emailRecipients.Count <= 0)
            {
                Console.WriteLine("sendFailureNotifications: No email recipients have been configured. Cannot send email notifications.");
                return;
            }
            StringBuilder sb = new StringBuilder();
            DateTime dt = DateTime.Now;
            sb.Append("ReadGen encountered errors in execution at: " + dt.ToString() + "\n\n");
            sb.Append("Environment Config: " + ci.environmentConfigPath + "\n");
            sb.Append("Application Config: " + ci.appConfigPath + "\n");
            sb.Append("Reads File: " + ci.ac.readfile + "\n");
            sb.Append("There were " + pr.status + " reads that failed during execution.");

            string subject = "ReadGen Error Report";

            EmailNotification en = new EmailNotification();

            if(!en.sendEmailWithCCList(ci,sb.ToString(),subject,ci.ac.emailRecipients))
            {
                Console.WriteLine("ERROR COULD NOT SEND ERROR NOTIFICATIONS.");
                Console.WriteLine(sb.ToString());
            }
            else
            {
                Console.WriteLine("Successfully emailed error notifications.");
            }


        }
        static ReadGenProcesser AbstractFactory(ConfigInfo ci)
        {
            if(ci.ac.proctype.Equals("sequential"))
            {
                return new SequentialProcessor();
            }
            if (ci.ac.proctype.Equals("random"))
            {
                return new RandomProcessor();
            }
            if (ci.ac.proctype.Equals("lookup"))
            {
                return new LookupProcessor();
            }
            return new DoNothingProcessor();
        }
    }
}
