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
            string plateLookupStr = null;
            if(rs.overviewimage != null && rs.plateimage != null)//OK use whats in the record.
            {
                id.plateBytes = ReadInImageFile.readIn(rs.plateimage);
                id.overviewBytes = ReadInImageFile.readIn(rs.overviewimage);
                plateLookupStr = rs.plate;
            }
            else
            {

                if(ci.ac.plate_image_path != null && ci.ac.overview_image_path != null)
                {
                    try
                    {
                        //Try to find the plate in the plate_image_path
                        String plateImageFilePath = ci.ac.plate_image_path + "\\" + rs.plate + "_plate.jpg";
                        String overviewImageFilePath = ci.ac.overview_image_path + "\\" + rs.plate + "_overview.jpg";
                        id.plateBytes = ReadInImageFile.readIn(plateImageFilePath);
                        id.overviewBytes = ReadInImageFile.readIn(overviewImageFilePath);
                        plateLookupStr = rs.plate;
                    }
                    catch(Exception e)//The plate does not exist in the plate_image_path, choose a random file.
                    {
                        Console.WriteLine(e.Message + "\nWill use a random file from: " + ci.ac.plate_image_path);
                        //id.plateBytes = readInRandomFile(ci.ac.plate_image_path);
                        RandomFileData rfd = readInRandomFileData(ci.ac.plate_image_path);
                        id.plateBytes = rfd.imageBytes;
                        plateLookupStr = rfd.plate;
                        if (rfd.imageBytes == null)
                        {
                            Console.WriteLine("There were no files at: " + ci.ac.plate_image_path);
                            return null;
                        }
                        String overviewImageFilePath = ci.ac.overview_image_path + "\\" + rfd.plate + "_overview.jpg";
                        id.overviewBytes = ReadInImageFile.readIn(overviewImageFilePath);
                        if (id.overviewBytes == null)
                        {
                            Console.WriteLine("There were no files at: " + ci.ac.overview_image_path);
                            return null;
                        }

                    }                    
                }
                else
                {
                    Console.WriteLine("DeriveImages::getImages: ERROR: No plate or overview image was specified. In addition plate_image_path and " +
                        "overview_image_path are null.");
                    return null;
                }

            }
            //Lookup the coordinates in the plate lookup file.
            PlateLookup pl = ci.lookupPlate(plateLookupStr);
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
            //take the full path based on the IDX
            //Get the after path
            //get the plate
            //initialze the return object.
            return ReadInImageFile.readIn(fileEntries[idx]);            
        }

        private RandomFileData readInRandomFileData(String thePath)
        {
            if (!Directory.Exists(thePath))
            {
                Console.WriteLine("DeriveImages::readInRandomFileData " + thePath + " does not exist!");
                return null;
            }
            String[] fileEntries = Directory.GetFiles(thePath);
            int idx = rndGlobal.Next(0, fileEntries.Length);
            String afterPath = fileEntries[idx].Substring(thePath.Length + 1);
            String justPlate = afterPath.Split('_')[0];
            RandomFileData rfd = new RandomFileData();
            rfd.plate = justPlate;
            rfd.imageBytes = ReadInImageFile.readIn(fileEntries[idx]);

            return rfd;
        }

        /*THIS COULD BE USED TO PARSE OUT THE PLATE NAME.
        private Boolean parseFilePaths(String directoryPath)
        {
            foreach (string fileName in fileEntries)
            {
                //Console.WriteLine(fileName);
                plateImages.Add(fileName);
                String afterPath = fileName.Substring(directoryPath.Length + 1);
                //Console.WriteLine(afterPath);
                //now get the plate
                String justPlate = afterPath.Split('_')[0];
                //Console.WriteLine(justPlate);
                plateList.Add(justPlate);
            }

            return true;
        } */
    }
}
