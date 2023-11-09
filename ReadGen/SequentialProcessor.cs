using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
                DateTime starttime = DateTime.Now;
                if(!processRead(ci,rs))
                {
                    pr.status++;
                }
                DateTime endtime = DateTime.Now;
                int runTime = getMSDelay(starttime, endtime);
                if(runTime < ci.ac.msdelay)
                {
                    int msToWait = ci.ac.msdelay - runTime;
                    Console.WriteLine("Sleeping " + msToWait + " milliseconds...");
                    Thread.Sleep(msToWait);
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
            Logger.logIt(ci, "");
            Logger.logIt(ci,"************************************");
            Logger.logIt(ci,"Working with Plate:" + rs.plate);
            Logger.logIt(ci,"Working with Camera: " + rs.camera_name);
            if(rs.testing_notes != null)
            {
                Logger.logIt(ci,"Testing Notes: " + rs.testing_notes);
            }
            
            
            string camera = null;
            //If we don't have a camera 
            if(rs.camera_name == null)
            {
                
                if(ci.cameras == null)
                {
                    Logger.logIt(ci,"SequentialProcessor::processRead: ERROR. No cameras.");
                    Logger.logIt(ci, "*******************************************");
                    return false;
                }
                if(ci.cameras.Count == 0)
                {
                    Logger.logIt(ci,"SequentialProcessor::processRead: ERROR. No cameras.");
                    Logger.logIt(ci, "*******************************************");
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
            if(cgi == null)
            {
                Logger.logIt(ci,"SequentialProcessor::processRead: ERROR. " +
                    "Could not find reader for: " + camera);
                Logger.logIt(ci, "*******************************************");
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
            Logger.logIt(ci,"SequentialProcessor::processRead: reader: " + cgi.reader);
            //Perform the Image lookup -- should be lib function
            DeriveImages di = new DeriveImages();
            ImagesData id = di.getImages(ci, rs);
            if(id == null)
            {
                Logger.logIt(ci,"SequentialProcessor::processRead: Image Read FAILED!");
                return false;
            }
            if(id.plateBytes == null || id.overviewBytes == null)
            {
                Logger.logIt(ci,"SequentialProcessor::processRead: Image Read FAILED!");
                return false;
            }
           


            //Create the XML
            EOCGuid eocGuid = new EOCGuid();
            string timeStamp = null;
            if(rs.read_date != null)
            {
                timeStamp = genTimestamp(rs.read_date);
            }
            else
            {
                timeStamp = genTimestamp();
            }
           
            eocGuid.createGuidUS(timeStamp, rs.plate, camera);
            Guid readId = eocGuid.readID;
            cgi.id = readId.ToString();
            ReadXmlMaker rxm = new ReadXmlMaker();
           
            String requestXml = rxm.deriveXmlUS(cgi, rs.plate, timeStamp, id.plateBytes, id.overviewBytes, eocGuid, ci,rs,id);
            //String requestXml = rxm.deriveXmlUS(cgi, rs.plate, timeStamp, eocGuid, ci, rs, id);

            Logger.logIt(ci, requestXml);
            //Send the REST request
            PutReadRequest prr = new PutReadRequest(ci.ec.username,ci.ec.password,ci.ec.readAgg);
            try
            {
                HttpStatusCode status = prr.PutResourceReadRequest(cgi.id, requestXml);
                Console.WriteLine("SequentialProcessor::processRead: status = " + status.ToString());
            }
            catch(Exception e)
            {
                Logger.logIt(ci,e.Message);
                Logger.logIt(ci, "****************************************");
                return false;
            }
            //Do we have to generate alarms? Check genalarms in Environment file
            //if genalarms            
            if (ci.ec.genalarms.Equals("true"))
            {
                Logger.logIt(ci,"SequentialProcessor::processRead: genalarms is set to true. Checking for list entries....");
                //Check EOC_TRAN for list entries
                // generate alarms if there are list entries
                //ListDetail ld = sqh.getListEntries(rs.plate, rs.state, timeStamp);
                List<ListDetail> ldList = sqh.getListEntries(rs,timeStamp);
                if (ldList != null)
                {
                    foreach(ListDetail ld in ldList)
                    {
                        Logger.logIt(ci,"**** WE Can generate alarms: " + ld.list_detail_id);
                        Guid alarmG = Guid.NewGuid();
                        String sAlarmXML = rxm.buildAlarmXMLUS(requestXml, cgi, alarmG.ToString(), timeStamp, rs.plate, ld,ci);
                        Logger.logIt(ci, sAlarmXML);                       
                        prr.PutResourceAlarmRequest(alarmG.ToString(), sAlarmXML);
                    }
                    
                }
            }
           

            return true;
        }                                          
    }
}
