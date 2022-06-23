using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ReadGen
{
    public class AlarmMgmtJSONParser
    {
        public AlarmMgmtContainer parseFile(string path)
        {
            try
            {
                Console.WriteLine("AlarmMgmtContainer::parseFile: Reading in: " + path);
                var jsonString = File.ReadAllText(path);
                Console.WriteLine("AlarmMgmtContainer::parseFile:Trying to deserialize.......");
                AlarmMgmtContainer amc = JsonConvert.DeserializeObject<AlarmMgmtContainer>(jsonString);
                return amc;

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            
            return null;
        }
    }
}
