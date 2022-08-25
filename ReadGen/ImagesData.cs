using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public class ImagesData
    {
        public string origx { get; set; }
        public string origy { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public byte[] plateBytes { get; set; }
        public byte[] overviewBytes { get; set; }
    }
}
