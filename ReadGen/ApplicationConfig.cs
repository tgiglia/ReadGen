using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    class ApplicationConfig
    {
        public enum procclass { sequential,random,lookup,unknown}
        public String proctype { get; set; }
        public int numtosend { get; set; }
        public int msdelay { get; set; }
        public String readfile { get; set; }
        public String camfile { get; set; }
        public String alarmmgtfile { get; set; }
        public String alarmuserfile { get; set; }
        public String overview_image_path { get; set; }
        public String plate_image_path { get; set; }
        public String plate_lookup { get; set; }
        public procclass ProcClassification { get; set; }
        public override string ToString()
        {
            return "proctype: " + proctype +
                "\nnumtosend: " + numtosend +
                "\nmsdelay: " + msdelay +
                "\nreadfile: " + readfile +
                "\ncamfile: " + camfile +
                "\nalarmmgtfile: " + alarmmgtfile +
                "\nalarmuserfile: " + alarmuserfile +
                "\noverview_image_path: " + overview_image_path +
                "\nplate_image_path: " + plate_image_path +
                "\nplate_lookup: " + plate_lookup;

        }
        
    }
}
