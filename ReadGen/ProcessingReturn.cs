using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public class ProcessingReturn
    {
        public int status { get; set; }
        public string description { get; set; }
        public override string ToString()
        {
            return "status: " + status + "\nDescription: " + description;
        }
    }
}
