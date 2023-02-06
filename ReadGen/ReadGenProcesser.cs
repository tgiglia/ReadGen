using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public abstract class ReadGenProcesser
    {
        Random rndGlobal;
        
        public abstract ProcessingReturn executeProcess(ConfigInfo ci);

        public ReadGenProcesser()
        {
            rndGlobal = new Random();
        }
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
        public string genTimestamp(string s)
        {
            
            DateTime localTime = DateTime.Now;


            if(s.Equals("TODAY NOW"))
            {
                DateTimeOffset localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
                return noMilliseconds(localTimeAndOffset);
            }
            if(s.Contains("TODAY"))
            {
                return deriveTodayWTimeStamp(s);
            }
            return deriveFromAbsoluteTimeStamp(s);
            
        }
        private string deriveFromAbsoluteTimeStamp(string s)
        {
            var parsedDate = DateTime.Parse(s);
            DateTimeOffset localTimeAndOffset = new DateTimeOffset(parsedDate, TimeZoneInfo.Local.GetUtcOffset(parsedDate));
            return noMilliseconds(localTimeAndOffset);
        }
        private string deriveTodayWTimeStamp(string s)
        {
            if (s.Length > 6)
            {
                //Get everything after TODAY and space
                String theEnd = s.Substring(6);
                //Console.WriteLine("theEnd:" + theEnd + "|");
                DateTime today = DateTime.Now;
                String theTime = deriveTimeFromStamp(theEnd);
                //Console.WriteLine("deriveTimeStamp: returning: " + theTime);
                return theTime;
            }
            return null;
        }
        protected String deriveTimeFromStamp(String stamp)
        {
            String[] fields = stamp.Split(':');
            if (fields == null)
            {
                return null;
            }

            DateTime dt = DateTime.Now;
            //Console.WriteLine("We have " + fields.Length + " fields in: " + stamp);
            DateTime formatedTime;
            TimeSpan ts;
            switch (fields.Length)
            {
                case 1:
                    {
                        int hour = Int32.Parse(fields[0]);
                        ts = new TimeSpan(hour, 0, 0);
                        break;
                    }
                case 2:
                    {
                        int hour = Int32.Parse(fields[0]);
                        int minutes = Int32.Parse(fields[1]);
                        ts = new TimeSpan(hour, minutes, 0);
                        break;

                    }
                case 3:
                    {
                        int hour = Int32.Parse(fields[0]);
                        int minutes = Int32.Parse(fields[1]);
                        int seconds = Int32.Parse(fields[2]);
                        ts = new TimeSpan(hour, minutes, seconds);
                        break;
                    }


                default: return null;
            }

            formatedTime = dt.Date + ts;
            DateTimeOffset localTimeAndOffset = new DateTimeOffset(formatedTime, TimeZoneInfo.Local.GetUtcOffset(formatedTime));
            return noMilliseconds(localTimeAndOffset);
        }

        public string noMilliseconds(DateTimeOffset dto)
        {
            return dto.ToString("yyy-MM-ddTHH:mm:sszzz");
        }
        protected string getCameraFromCamfile(ConfigInfo ci)
        {
            int idx = rndGlobal.Next(0, ci.cameras.Count);

            return ci.cameras[idx];
        }
        protected int getMSDelay(DateTime start, DateTime end)
        {
            if (start > end)
            {
                //Console.WriteLine("getMSDelay: START is greater than END!");
                DateTime tmp = end;
                end = start;
                start = tmp;
            }
            int iDelay = 0;
            int hourDelay = (end.Hour - start.Hour) * 3600000;
            int minuteDelay = (end.Minute - start.Minute) * 60000;
            int secondDelay = (end.Second - start.Second) * 1000;
            int millisecondDelay = end.Millisecond - start.Millisecond;
            iDelay = hourDelay + minuteDelay + secondDelay + millisecondDelay;
            return iDelay;
        }

    }
}
