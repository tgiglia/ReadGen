using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReadGen
{
    public class DeriveImages
    {
        Random rndGlobal;
        public DeriveImages()
        {
            rndGlobal = new Random();
        }
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
                    try
                    {
                        String plateImageFilePath = ci.ac.plate_image_path + "\\" + rs.plate + "_plate.jpg";
                        String overviewImageFilePath = ci.ac.overview_image_path + "\\" + rs.plate + "_overview.jpg";
                        id.plateBytes = ReadInImageFile.readIn(plateImageFilePath);
                        id.overviewBytes = ReadInImageFile.readIn(overviewImageFilePath);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message + "\nWill use a random file from: " + ci.ac.plate_image_path);
                        id.plateBytes = readInRandomFile(ci.ac.plate_image_path);
                        if(id.plateBytes == null)
                        {
                            Console.WriteLine("There were no files at: " + ci.ac.plate_image_path);
                            return null;
                        }
                        id.overviewBytes = readInRandomFile(ci.ac.overview_image_path);
                        if(id.overviewBytes == null)
                        {
                            Console.WriteLine("There were no files at: " + ci.ac.overview_image_path);
                            return null;
                        }

                    }
                    
                }

            }
            //Lookup the coordinates in the plate lookup file.
            PlateLookup pl = ci.lookupPlate(rs.plate);
            if(pl != null)
            {
                id.height = pl.height;
                id.origx = pl.origx;
                id.origy = pl.origy;
                id.width = pl.width;
            }
            else
            {
                id.height = "0";
                id.origx = "0";
                id.origy = "0";
                id.width = "0";
            }

            return id;
        }
        private byte[] readInRandomFile(String thePath)
        {

            if (!Directory.Exists(thePath))
            {
                Console.WriteLine("DeriveImages::readInRandomFile " + thePath + " does not exist!");
                return null;
            }
            String[] fileEntries = Directory.GetFiles(thePath);
            //Now choose a random file
            int idx = rndGlobal.Next(0, fileEntries.Length);
            return ReadInImageFile.readIn(fileEntries[idx]);            
        }
    }
}
