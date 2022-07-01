using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    class SequentialProcessor : ReadGenProcesser
    {
        public override ProcessingReturn executeProcess(ConfigInfo ci)
        {
            ProcessingReturn pr = new ProcessingReturn();
            pr.status = 0;
            pr.description = "Success";

            Console.WriteLine("SequentialProcesser: executing...");
            //How many Reads do we have?
            int iNumReads = ci.rc.Reads.Count;
            foreach(ReadStruct rs in ci.rc.Reads)
            {
               if(!processRead(ci,rs))
                {
                    pr.status++;
                }
            }
            if(pr.status > 0)
            {
                pr.description = "A number of reads failed.";
            }
            return pr;
        }

        


        private bool processRead(ConfigInfo ci, ReadStruct rs)
        {
            Console.WriteLine("Working with: " + rs.camera_name);
            string camera = null;
            //If we don't have a camera 
            if(rs.camera_name == null)
            {
                
                if(ci.cameras == null)
                {
                    Console.WriteLine("SequentialProcessor::processRead: ERROR. No cameras.");
                    return false;
                }
                if(ci.cameras.Count == 0)
                {
                    Console.WriteLine("SequentialProcessor::processRead: ERROR. No cameras.");
                    return false;
                }
                //Select a camera from the Camera file -- should be lib function
                camera = getRandomCamera(ci);
            }
            else
            {
                camera = rs.camera_name;
            }
            //Find its Reader in the database. -- should be lib function
            SQLQueryHelper sqh = new SQLQueryHelper(ci);
            string reader = sqh.getReaderFromCameraName(camera);
            if(reader == null)
            {
                Console.WriteLine("SequentialProcessor::processRead: ERROR. " +
                    "Could not find reader for: " + camera);
                return false;
            }
            //Perform the Image lookup -- should be lib function
            //Do we have to generate alarms? Check genalarms in Environment file
            //if genalarms 
            //Check EOC_TRAN for list entries
            // generate alarms if there are list entries
            //Create the XML
            //Send the REST request


            return true;
        }                                          
    }
}
