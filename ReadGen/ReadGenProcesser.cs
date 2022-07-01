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
    }
}
