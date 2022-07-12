using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public class CGInfo
    {
        public String id { get; set; } //translates to derived ID
        public String reader { get; set; }
        public String readerId { get; set; } //translates to site in Request Body
        public String domainIdStr { get; set; }
        public String cameraName { get; set; }
        public String cameraId { get; set; }  //translates to camera_site_id in Request Body
        public double lat { get; set; }
        public double lon { get; set; }

        public override string ToString()
        {
            return reader + "," + readerId + "," + domainIdStr + "," + cameraId;
        }
    }
}
