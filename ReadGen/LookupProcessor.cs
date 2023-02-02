using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ReadGen
{
    class LookupProcessor : ReadGenProcesser
    {
        
        public LookupProcessor()
        {
            
        }
        public override ProcessingReturn executeProcess(ConfigInfo ci)
        {
            ProcessingReturn pr = new ProcessingReturn();
            pr.status = 0;
            pr.description = "Success";
            Console.WriteLine("LookupProcessor: executing...");
            if(ci.ac.camfile == null)
            {
                Console.WriteLine("LookupProcessor::executeProcess: camfile is NULL, terminating request...");
                pr.status = -1;
                return pr;
            }
            if(ci.ac.plate_image_path == null)
            {
                Console.WriteLine("LookupProcessor::executeProcess: plate_image_path is NULL, terminating request...");
                pr.status = -1;
                return pr;
            }
            if (!Directory.Exists(ci.ac.plate_image_path))
            {
                Console.WriteLine("LookupProcessor::executeProcess: " + ci.ac.plate_image_path + " does not exist!");
                pr.status = -1;
                return pr;
            }
            if(ci.ac.overview_image_path == null)
            {
                Console.WriteLine("LookupProcessor::executeProcess: overview_image_path is NULL, terminating request...");
                pr.status = -1;
                return pr;
            }
            if(!Directory.Exists(ci.ac.overview_image_path))
            {
                Console.WriteLine("LookupProcessor::executeProcess: " + ci.ac.overview_image_path + " does not exist!");
                pr.status = -1;
                return pr;
            }
            String[] fileEntries = Directory.GetFiles(ci.ac.plate_image_path);
            String[] overviewFileEntries = Directory.GetFiles(ci.ac.overview_image_path);
            int iIdx = 0;
            foreach(String plateFile in fileEntries)
            {
                String afterPath = plateFile.Substring(ci.ac.plate_image_path.Length + 1);
                String justPlate = afterPath.Split('_')[0];
                string cameraName = getCameraFromCamfile(ci);
                SQLQueryHelper sqh = new SQLQueryHelper(ci);
                CGInfo cgi = sqh.getReaderFromCameraName(cameraName);
                if (cgi == null)
                {
                    Console.WriteLine("SequentialProcessor::processRead: ERROR. " +
                        "Could not find reader for: " + cameraName);
                    pr.status = -1;
                    return pr;
                }
                ReadStruct rs = new ReadStruct();
                rs.camera_name = cameraName;
                rs.plateimage = plateFile;
                rs.overviewimage = overviewFileEntries[iIdx];
                rs.read_date = genTimestamp("TODAY NOW");

                if (rs.longitude != 0)
                {
                    cgi.lon = rs.longitude;
                }
                if (rs.latitude != 0)
                {
                    cgi.lat = rs.latitude;
                }
                DeriveImages di = new DeriveImages();
                ImagesData id = di.getImages(ci, rs);
                if (id.plateBytes == null || id.overviewBytes == null)
                {
                    Console.WriteLine("LookupProcessor::processRead: Image Read FAILED!");
                    pr.status = -1;
                    return pr;
                }
                EOCGuid eocGuid = new EOCGuid();
                eocGuid.createGuidUS(rs.read_date, rs.plate, rs.camera_name);
                Guid readId = eocGuid.readID;
                cgi.id = readId.ToString();
                ReadXmlMaker rxm = new ReadXmlMaker();
                String requestXml = rxm.deriveXmlUS(cgi, rs.plate, rs.read_date, id.plateBytes, id.overviewBytes, eocGuid, ci, rs,id);
                Console.WriteLine("XML:\n" + requestXml);
                PutReadRequest prr = new PutReadRequest(ci.ec.username, ci.ec.password, ci.ec.readAgg);
                try
                {
                    
                    HttpStatusCode status = prr.PutResourceReadRequest(cgi.id, requestXml);
                    Console.WriteLine("LookupProcessor::processRead: status = " + status.ToString());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    pr.status++;
                    continue;
                }
                

                if (ci.ec.genalarms.Equals("true"))
                {
                    Console.WriteLine("SequentialProcessor::processRead: genalarms is set to true. Checking for list entries....");
                    //Check EOC_TRAN for list entries
                    // generate alarms if there are list entries
                    //ListDetail ld = sqh.getListEntries(rs.plate, rs.state, rs.read_date);
                    List<ListDetail> ldList = sqh.getListEntries(rs, rs.read_date);
                    if (ldList != null)
                    {
                        foreach (ListDetail ld in ldList)
                        {
                            Console.WriteLine("**** WE Can generate alarms: " + ld.list_detail_id);
                            Guid alarmG = Guid.NewGuid();
                            String sAlarmXML = rxm.buildAlarmXMLUS(requestXml, cgi, alarmG.ToString(), rs.read_date, rs.plate, ld,ci);
                            Console.WriteLine(sAlarmXML);
                            prr.PutResourceAlarmRequest(alarmG.ToString(), sAlarmXML);
                        }

                    }
                }


                iIdx++;
            }
            
            return pr;
        }
        
    }
}
