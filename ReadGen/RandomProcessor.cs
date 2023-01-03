﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ReadGen
{
    class RandomProcessor : ReadGenProcesser
    {
        Random rndGlobal;
        public RandomProcessor()
        {
            rndGlobal = new Random();
        }
        public override ProcessingReturn executeProcess(ConfigInfo ci)
        {
            ProcessingReturn pr = new ProcessingReturn();
            pr.status = 0;
            pr.description = "Success";


            Console.WriteLine("RandomProcessor: executing...");
            for (int i = 0; i < ci.ac.numtosend; i++)
            {
                //Get a number to processes
                int idx = rndGlobal.Next(0, ci.rc.Reads.Count);
                //process the read
                //Check for the Camera here - if its null then get one from the camfile ci.cameras

                if (!processRead(ci, ci.rc.Reads[idx]))
                {
                    pr.status++;
                }

            }
            return pr;
        }

        private bool processRead(ConfigInfo ci, ReadStruct rs)
        {
            Console.WriteLine("Working with: " + rs.camera_name);
            string camera = null;
            //If we don't have a camera 
            if (rs.camera_name == null)
            {

                if (ci.cameras == null)
                {
                    Console.WriteLine("RandomProcessor::processRead: ERROR. No cameras.");
                    return false;
                }
                if (ci.cameras.Count == 0)
                {
                    Console.WriteLine("RandomProcessor::processRead: ERROR. No cameras.");
                    return false;
                }
                //Select a camera from the Camera file -- should be lib function
                camera = getCameraFromCamfile(ci);
            }
            else
            {
                camera = rs.camera_name;
            }
            //Find its Reader in the database. -- should be lib function
            SQLQueryHelper sqh = new SQLQueryHelper(ci);
            CGInfo cgi = sqh.getReaderFromCameraName(camera);
            if (cgi == null)
            {
                Console.WriteLine("RandomProcessor::processRead: ERROR. " +
                    "Could not find reader for: " + camera);
                return false;
            }
            if (rs.longitude != 0)
            {
                cgi.lon = rs.longitude;
            }
            if (rs.latitude != 0)
            {
                cgi.lat = rs.latitude;
            }
            Console.WriteLine("RandomProcessor::processRead: reader: " + cgi.reader);
            //Perform the Image lookup -- should be lib function
            DeriveImages di = new DeriveImages();
            ImagesData id = di.getImages(ci, rs);
            if (id.plateBytes == null || id.overviewBytes == null)
            {
                Console.WriteLine("RandomProcessor::processRead: Image Read FAILED!");
                return false;
            }



            //Create the XML
            EOCGuid eocGuid = new EOCGuid();
            string timeStamp = null;
            if (rs.read_date != null)
            {
                timeStamp = genTimestamp(rs.read_date);
            }
            else
            {
                timeStamp = genTimestamp();
            }

            eocGuid.createGuidUS(timeStamp, rs.plate,camera);
            Guid readId = eocGuid.readID;
            cgi.id = readId.ToString();
            ReadXmlMaker rxm = new ReadXmlMaker();

            String requestXml = rxm.deriveXmlUS(cgi, rs.plate, timeStamp, id.plateBytes, id.overviewBytes, eocGuid, ci, rs,id);
            Console.WriteLine("XML:\n" + requestXml);
            //Send the REST request
            PutReadRequest prr = new PutReadRequest(ci.ec.username, ci.ec.password, ci.ec.readAgg);
            HttpStatusCode status = prr.PutResourceReadRequest(cgi.id, requestXml);
            Console.WriteLine("RandomProcessor::processRead: status = " + status.ToString());
            //Do we have to generate alarms? Check genalarms in Environment file
            //if genalarms 
            if (ci.ec.genalarms.Equals("true"))
            {
                Console.WriteLine("RandomProcessor::processRead: genalarms is set to true. Checking for list entries....");
                //Check EOC_TRAN for list entries
                // generate alarms if there are list entries
                List<ListDetail> ldList = sqh.getListEntries(rs, timeStamp);
                if (ldList != null)
                {
                    foreach (ListDetail ld in ldList)
                    {
                        Console.WriteLine("**** WE Can generate alarms: " + ld.list_detail_id);
                        Guid alarmG = Guid.NewGuid();
                        String sAlarmXML = rxm.buildAlarmXMLUS(requestXml, cgi, alarmG.ToString(), timeStamp, rs.plate, ld);
                        Console.WriteLine(sAlarmXML);
                        prr.PutResourceAlarmRequest(alarmG.ToString(), sAlarmXML);
                    }

                }
            }


            return true;
        }

    }
}
