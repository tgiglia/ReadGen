﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            CGInfo cgi = sqh.getReaderFromCameraName(camera);
            if(cgi == null)
            {
                Console.WriteLine("SequentialProcessor::processRead: ERROR. " +
                    "Could not find reader for: " + camera);
                return false;
            }
            if(rs.longitude != 0)
            {
                cgi.lon = rs.longitude;
            }
            if(rs.latitude != 0)
            {
                cgi.lat = rs.latitude;
            }
            Console.WriteLine("SequentialProcessor::processRead: reader: " + cgi.reader);
            //Perform the Image lookup -- should be lib function
            DeriveImages di = new DeriveImages();
            ImagesData id = di.getImages(ci, rs);
            if(id.plateBytes == null || id.overviewBytes == null)
            {
                Console.WriteLine("SequentialProcessor::processRead: Image Read FAILED!");
                return false;
            }
            //Do we have to generate alarms? Check genalarms in Environment file
            //if genalarms 
            if(ci.ec.genalarms.Equals("true"))
            {
                Console.WriteLine("SequentialProcessor::processRead: genalarms is set to true. Checking for list entries....");
                //Check EOC_TRAN for list entries
                // generate alarms if there are list entries
            }


            //Create the XML
            EOCGuid eocGuid = new EOCGuid();
            string timeStamp = genTimestamp(rs.read_date);
            eocGuid.createGuidUS(timeStamp, rs.plate, rs.camera_name);
            Guid readId = eocGuid.readID;
            cgi.id = readId.ToString();
            ReadXmlMaker rxm = new ReadXmlMaker();
            String requestXml = rxm.deriveXmlUS(cgi, rs.plate, timeStamp, id.plateBytes, id.overviewBytes, eocGuid, ci);
            Console.WriteLine("XML:\n" + requestXml);
            //Send the REST request
            PutReadRequest prr = new PutReadRequest(ci.ec.username,ci.ec.password,ci.ec.readAgg);
            HttpStatusCode status = prr.PutResourceReadRequest(cgi.id, requestXml);
            Console.WriteLine("SequentialProcessor::processRead: status = " + status.ToString());

            return true;
        }                                          
    }
}
