using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public class EOCGuid
    {
        public Guid readID { get; set; }
        public Guid overviewImageID { get; set; }
        public string justTuple { get; set; }

        public void createGuidUS(string timeStamp, string plate, string camera)
        {
            
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                justTuple = JustTuple(camera);
                //Console.WriteLine("createGuids: camera: " + camera + " justTuple: " + justTuple + " timestamp: " + timeStamp);
                string derive = timeStamp + "\0" + plate + "\0";

                byte[] deriveByte = Encoding.UTF8.GetBytes(derive);

                //Console.WriteLine("The length of deriveByte is: " + deriveByte.Length);

                byte[] deriveHash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(derive));

                readID = new Guid(string.Join("",
                deriveHash.Select(singlebyte => singlebyte.ToString("x2"))));
                bool carry = true;
                for (int i = 15; i >= 0 && carry; i--)
                {
                    byte oldValue = deriveHash[i]++;
                    carry = oldValue > deriveHash[i];
                }
                overviewImageID = new Guid(string.Join("",
                deriveHash.Select(singlebyte => singlebyte.ToString("x2"))));
                Console.WriteLine("Plate GUID: " + readID.ToString());
                Console.WriteLine("Overview GUID: " + overviewImageID.ToString());
            }
        }
        public void createGuids(string timeStamp, string plate, string camera)
        {
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                justTuple = JustTuple(camera);
                //Console.WriteLine("createGuids: camera: " + camera + " justTuple: " + justTuple + " timestamp: " + timeStamp);
                string derive = timeStamp + "\0" + plate + "\0" + justTuple;

                byte[] deriveByte = Encoding.UTF8.GetBytes(derive);

                //Console.WriteLine("The length of deriveByte is: " + deriveByte.Length);

                byte[] deriveHash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(derive));

                readID = new Guid(string.Join("",
                deriveHash.Select(singlebyte => singlebyte.ToString("x2"))));
                bool carry = true;
                for (int i = 15; i >= 0 && carry; i--)
                {
                    byte oldValue = deriveHash[i]++;
                    carry = oldValue > deriveHash[i];
                }
                overviewImageID = new Guid(string.Join("",
                deriveHash.Select(singlebyte => singlebyte.ToString("x2"))));
                Console.WriteLine("Plate GUID: " + readID.ToString());
                Console.WriteLine("Overview GUID: " + overviewImageID.ToString());
            }
        }

        private string JustTuple(String camera)
        {
            char[] b = camera.ToArray();
            StringBuilder sb = new StringBuilder();

            int len = b.Length;
            int cnt = 0;
            int numOfUnderScore = 0;
            do
            {
                if (!Char.IsLetterOrDigit(b[cnt]))
                {
                    numOfUnderScore++;
                }
                if (numOfUnderScore < 3)
                {
                    sb.Append(b[cnt]);
                }
                cnt++;
            } while (cnt < len && numOfUnderScore < 3);

            return sb.ToString();
        }
    }
}
