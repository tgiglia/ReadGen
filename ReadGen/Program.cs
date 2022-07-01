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
            Console.WriteLine("Congradulations you entered enough command line arguments!");
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

            Console.WriteLine("Lets try to parse the readfile JSON....");
            ReadFileJSONParser rfjp = new ReadFileJSONParser();
            rfjp.testIt(ci);
            ReadGenProcesser rgp = AbstractFactory(ci);
            //Execute the processing
            ProcessingReturn pr = rgp.executeProcess(ci);
            Console.WriteLine("Execution Status: " + pr.status);
            Console.WriteLine("Execution Description: " + pr.description);

            Console.WriteLine("Press any key to end.");
            Console.ReadKey();
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
