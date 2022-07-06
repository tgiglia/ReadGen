using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    public class DeriveImages
    {
        public ImagesData getImages(ConfigInfo ci, ReadStruct rs)
        {
            ImagesData id = new ImagesData();

            if(rs.overviewimage != null && rs.plateimage != null)//OK use whats in the record.
            {
                id.plateBytes = ReadInImageFile.readIn(rs.plateimage);
                id.overviewBytes = ReadInImageFile.readIn(rs.overviewimage);
            }
            else
            {
                if(ci.ac.plate_image_path != null && ci.ac.overview_image_path != null)
                {
                    String plateImageFilePath = ci.ac.plate_image_path +"\\" + rs.plate + "_plate.jpg";
                    String overviewImageFilePath = ci.ac.overview_image_path + "\\" + rs.plate + "_overview.jpg";
                    id.plateBytes = ReadInImageFile.readIn(plateImageFilePath);
                    id.overviewBytes = ReadInImageFile.readIn(overviewImageFilePath);
                }

            }

            return id;
        }
    }
}
