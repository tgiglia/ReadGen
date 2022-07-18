using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public class ReadStruct
    {
        public string read_method { get; set; }
        public string plate { get; set; }
        public string state { get; set; }
        public string read_date { get; set; }
        public string camera_name { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int error_radius { get; set; }
        public int speed { get; set; }
        public int confidence { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string overviewimage { get; set; }
        public string plateimage { get; set; }
    }
}
