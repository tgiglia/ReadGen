using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ReadGen
{
    class ReadFileJSONParser
    {
        public void testIt(ConfigInfo ci)
        {
            try
            {
                Console.WriteLine("Reading in: " + ci.ac.readfile);
                var myJsonString = File.ReadAllText(ci.ac.readfile);
                Console.WriteLine("Trying to deserialize.......");
                ReadContainer myJsonObject = JsonConvert.DeserializeObject<ReadContainer>(myJsonString);
                Console.WriteLine("We got: " + myJsonObject.Reads.Count + " objects");
               
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public ReadContainer parseReadFile(ConfigInfo ci)
        {
            try
            {
                Console.WriteLine("Reading in: " + ci.ac.readfile);
                var myJsonString = File.ReadAllText(ci.ac.readfile);
                Console.WriteLine("Trying to deserialize.......");
                ReadContainer myJsonObject = JsonConvert.DeserializeObject<ReadContainer>(myJsonString);
                return myJsonObject;
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
