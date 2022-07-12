using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public abstract class ReadGenProcesser
    {
        
        public abstract ProcessingReturn executeProcess(ConfigInfo ci);
        public string getRandomCamera(ConfigInfo ci)
        {
            Random rnd = new Random();
            int idx = rnd.Next(0, ci.cameras.Count);
            return ci.cameras[idx];
        }
        public string genTimestamp()
        {
            String s = null;
            DateTime localTime = DateTime.Now;

            DateTimeOffset localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));

            s = noMilliseconds(localTimeAndOffset);
            return s;
        }
        public string noMilliseconds(DateTimeOffset dto)
        {
            return dto.ToString("yyy-MM-ddTHH:mm:sszzz");
        }
    }
}
