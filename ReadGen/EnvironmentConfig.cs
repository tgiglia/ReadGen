using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    class EnvironmentConfig
    {
        
        public String readAgg { get; set; }
        public String username { get; set; }
        public String password { get; set; }
        public String catalog { get; set; }
        public String timezone { get; set; }
        public String genalarms { get; set; }
        public override string ToString()
        {
            return "readAgg: " + readAgg +
                "\nusername: " + username +
                "\npassword: " + password +
                "\ncatalog: " + catalog +
                "\ntimezone: " + timezone +
                "\ngenalarms: " + genalarms;
        }
        
    }
}
