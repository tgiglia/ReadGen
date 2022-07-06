using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReadGen
{
    class ReadInImageFile
    {
        public static byte[] readIn(String filePath)
        {
            byte[] imageBytes = null;
            try
            {
                imageBytes = File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("ReadInImageFile::readin: Failed to open: " + filePath);
                Console.WriteLine(e.Message);
                Console.WriteLine("Exiting...");
                throw e;
            }
            return imageBytes;
        }
    }
}
