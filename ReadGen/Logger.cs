using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReadGen
{
    class Logger
    {
        public static void logIt(ConfigInfo cd, String s)
        {
            if(cd.ac.logfile == null)
            {
                Console.WriteLine(s);
            }
            DateTime localTime = DateTime.Now;
            if (!File.Exists(cd.ac.logfile))
            {
                using (StreamWriter sw = File.CreateText(cd.ac.logfile))
                {

                }
            }
            using (StreamWriter sw = File.AppendText(cd.ac.logfile))
            {
                sw.WriteLine(localTime.ToString() + "  " + s);
            }
        }
    }
}
