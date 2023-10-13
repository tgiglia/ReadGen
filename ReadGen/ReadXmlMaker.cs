using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReadGen
{
    public class ReadXmlMaker
    {
        StringBuilder sb;
        public ReadXmlMaker()
        {
            sb = new StringBuilder();
        }
        
        public String deriveXml(CGInfo cgi, PlateLookup pl, String timeStamp, Guid overviewImageId, ConfigInfo cd)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = true
            };

            XmlWriter xw = XmlWriter.Create(sb, xmlWriterSettings);

            xw.WriteStartDocument();
            xw.WriteStartElement("read", "elsag:lprcore");
            xw.WriteAttributeString("id", cgi.id);

            xw.WriteAttributeString("domain", cgi.domainIdStr);
            xw.WriteAttributeString("site", cgi.readerId);
            xw.WriteAttributeString("camera", cgi.cameraName);
            xw.WriteAttributeString("camera_site_id", cgi.cameraId);
            xw.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
            xw.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");

            xw.WriteStartElement("timestamp");
            xw.WriteString(timeStamp);
            xw.WriteEndElement();
            
            xw.WriteStartElement("class");
            xw.WriteString("Invariant");
            xw.WriteEndElement();

            xw.WriteStartElement("plate");
            xw.WriteString(pl.plate);
            xw.WriteEndElement();

            xw.WriteStartElement("confidence");
            xw.WriteString("75");
            xw.WriteEndElement();

            xw.WriteStartElement("overviews");
            xw.WriteStartElement("snapshot");
            xw.WriteAttributeString("id", overviewImageId.ToString());
            xw.WriteAttributeString("rev", "1");
            xw.WriteAttributeString("camera", cgi.cameraName);
            //xw.WriteAttributeString("rev", "1");

            xw.WriteStartElement("timestamp");
            xw.WriteString(timeStamp);
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.WriteEndElement();

            xw.WriteStartElement("facing");
            xw.WriteString("Front");
            xw.WriteEndElement();

            xw.WriteStartElement("platelocation");
            xw.WriteAttributeString("height", pl.height);
            xw.WriteAttributeString("width", pl.width);
            xw.WriteAttributeString("y", pl.origy);
            xw.WriteAttributeString("x", pl.origx);
            xw.WriteEndElement();

            xw.WriteStartElement("charlocations");
            xw.WriteEndElement();

            xw.WriteEndElement();//end read element
            xw.WriteEndDocument();
            xw.Close();

            return sb.ToString();
        }
        public String deriveXmlUS(CGInfo cgi, String plate, String timeStamp, byte[] imageBytes, byte[] oimageBytes, EOCGuid eocGuid,
                                     ConfigInfo configData,ReadStruct rs, ImagesData id)
        {

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true,
                OmitXmlDeclaration = true
            };
            XmlWriter xw = XmlWriter.Create(sb, xmlWriterSettings);

            xw.WriteStartDocument();
            xw.WriteStartElement("read", "elsag:lprcore");
            xw.WriteAttributeString("id", cgi.id);

            xw.WriteAttributeString("domain", cgi.domainIdStr);
            xw.WriteAttributeString("site", cgi.readerId);
            xw.WriteAttributeString("camera", cgi.cameraName);
            xw.WriteAttributeString("camera_site_id", cgi.cameraId);
            xw.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
            xw.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");

            xw.WriteStartElement("timestamp");
            xw.WriteString(timeStamp);
            xw.WriteEndElement();

            if(rs.speed != 0)
            {
                xw.WriteStartElement("speed");
                xw.WriteString(rs.speed.ToString());
                xw.WriteEndElement();
            }
            

            //Add the image data
            if(rs.read_method == null)
            {
                xw.WriteStartElement("imagedata");
                string s = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                xw.WriteString(s);
                xw.WriteEndElement();
            }
            else if(rs.read_method.Equals("LPR"))
            {
                xw.WriteStartElement("imagedata");
                string s = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                xw.WriteString(s);
                xw.WriteEndElement();
            }
            

            xw.WriteStartElement("class");
            xw.WriteString("Invariant");
            xw.WriteEndElement();


            if(rs.state != null)
            {
                xw.WriteStartElement("state");
                xw.WriteString(rs.state);
                xw.WriteEndElement();
            }
            xw.WriteStartElement("plate");
            xw.WriteString(plate);
            xw.WriteEndElement();

            string sConfidence = null;
            try
            {
                sConfidence = rs.confidence.ToString();
            }catch(Exception e)
            {
                sConfidence = "75";
            }
            xw.WriteStartElement("confidence");
            xw.WriteString(sConfidence);
            xw.WriteEndElement();

            if((rs.model != null) && (rs.make != null))
            {
                xw.WriteStartElement("vehicleinformation");
                xw.WriteStartElement("Make");
                xw.WriteString(rs.make);
                xw.WriteEndElement();//end make
                xw.WriteStartElement("ModelId");
                xw.WriteString(rs.model);
                xw.WriteEndElement();//end ModelId
                xw.WriteEndElement();//end vehicleinformation
            }

            xw.WriteStartElement("overviews");
            //Create snapshot element
            xw.WriteStartElement("snapshot");
            //write id attribute string
            //write camera attribute string
            xw.WriteAttributeString("id", eocGuid.overviewImageID.ToString());
            xw.WriteAttributeString("camera", cgi.cameraName);
            //create timestamp element
            xw.WriteStartElement("timestamp");
            //write timestamp string
            xw.WriteString(timeStamp);
            //End timestamp element
            xw.WriteEndElement();
            if (rs.read_method == null)
            {
                //create imagedata element
                xw.WriteStartElement("imagedata");
                //write imagedata element
                String s = Convert.ToBase64String(oimageBytes, 0, oimageBytes.Length);
                xw.WriteString(s);
                //end imagedata element
                xw.WriteEndElement();
            }
            else if (rs.read_method.Equals("LPR"))
            {
                //create imagedata element
                xw.WriteStartElement("imagedata");
                //write imagedata element
                String s = Convert.ToBase64String(oimageBytes, 0, oimageBytes.Length);
                xw.WriteString(s);
                //end imagedata element
                xw.WriteEndElement();
            }

            //end snapshot element
            xw.WriteEndElement();
            xw.WriteEndElement();

            //Start Position
            xw.WriteStartElement("gps");
            xw.WriteAttributeString("rev", "0");
            xw.WriteStartElement("timestamp");
            //write timestamp string
            xw.WriteString(timeStamp);
            xw.WriteEndElement();//End timestamp element
            xw.WriteStartElement("position");
            xw.WriteAttributeString("lat", cgi.lat.ToString());
            xw.WriteAttributeString("long", cgi.lon.ToString());
            xw.WriteAttributeString("errorradius", "15.9357481");
            xw.WriteEndElement();
            //End position
            xw.WriteStartElement("velocity");
            xw.WriteAttributeString("east", "0.0251652766");
            xw.WriteAttributeString("north", "-0.03739889");
            xw.WriteEndElement();//end velocity
            xw.WriteEndElement();//End GPS

            xw.WriteStartElement("facing");
            xw.WriteString("Front");
            xw.WriteEndElement();
            if (rs.read_method != null)
            {
                if (rs.read_method.Length > 2)
                {
                    xw.WriteStartElement("method");
                    xw.WriteString(rs.read_method);
                    xw.WriteEndElement();
                }


            }



            //Plate location
            xw.WriteStartElement("platelocation");
            xw.WriteAttributeString("height", id.height);
            xw.WriteAttributeString("width", id.width);
            xw.WriteAttributeString("y", id.origy);
            xw.WriteAttributeString("x", id.origx);
            xw.WriteEndElement();
            

            xw.WriteEndElement();//end read element
            xw.WriteEndDocument();
            xw.Close();
            return sb.ToString();

        }

        public String deriveXmlUS(CGInfo cgi, String plate, String timeStamp,  EOCGuid eocGuid,
                                    ConfigInfo configData, ReadStruct rs, ImagesData id)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true,
                OmitXmlDeclaration = true
            };
            XmlWriter xw = XmlWriter.Create(sb, xmlWriterSettings);

            xw.WriteStartDocument();
            xw.WriteStartElement("read", "elsag:lprcore");
            xw.WriteAttributeString("id", cgi.id);

            xw.WriteAttributeString("domain", cgi.domainIdStr);
            xw.WriteAttributeString("site", cgi.readerId);
            xw.WriteAttributeString("camera", cgi.cameraName);
            xw.WriteAttributeString("camera_site_id", cgi.cameraId);
            xw.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
            xw.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");

            xw.WriteStartElement("timestamp");
            xw.WriteString(timeStamp);
            xw.WriteEndElement();

            if (rs.speed != 0)
            {
                xw.WriteStartElement("speed");
                xw.WriteString(rs.speed.ToString());
                xw.WriteEndElement();
            }
          

            xw.WriteStartElement("class");
            xw.WriteString("Invariant");
            xw.WriteEndElement();


            if (rs.state != null)
            {
                xw.WriteStartElement("state");
                xw.WriteString(rs.state);
                xw.WriteEndElement();
            }
            xw.WriteStartElement("plate");
            xw.WriteString(plate);
            xw.WriteEndElement();

            string sConfidence = null;
            try
            {
                sConfidence = rs.confidence.ToString();
            }
            catch (Exception e)
            {
                sConfidence = "75";
            }
            xw.WriteStartElement("confidence");
            xw.WriteString(sConfidence);
            xw.WriteEndElement();

            if ((rs.model != null) && (rs.make != null))
            {
                xw.WriteStartElement("vehicleinformation");
                xw.WriteStartElement("Make");
                xw.WriteString(rs.make);
                xw.WriteEndElement();//end make
                xw.WriteStartElement("ModelId");
                xw.WriteString(rs.model);
                xw.WriteEndElement();//end ModelId
                xw.WriteEndElement();//end vehicleinformation
            }


            //Start Position
            xw.WriteStartElement("gps");
            xw.WriteAttributeString("rev", "0");
            xw.WriteStartElement("timestamp");
            //write timestamp string
            xw.WriteString(timeStamp);
            xw.WriteEndElement();//End timestamp element
            xw.WriteStartElement("position");
            xw.WriteAttributeString("lat", cgi.lat.ToString());
            xw.WriteAttributeString("long", cgi.lon.ToString());
            xw.WriteAttributeString("errorradius", "15.9357481");
            xw.WriteEndElement();
            //End position
            xw.WriteStartElement("velocity");
            xw.WriteAttributeString("east", "0.0251652766");
            xw.WriteAttributeString("north", "-0.03739889");
            xw.WriteEndElement();//end velocity
            xw.WriteEndElement();//End GPS

            xw.WriteStartElement("facing");
            xw.WriteString("Front");
            xw.WriteEndElement();
            //Plate location
            /*xw.WriteStartElement("platelocation");
            xw.WriteAttributeString("height", id.height);
            xw.WriteAttributeString("width", id.width);
            xw.WriteAttributeString("y", id.origy);
            xw.WriteAttributeString("x", id.origx);
            xw.WriteEndElement();*/


            xw.WriteEndElement();//end read element
            xw.WriteEndDocument();
            xw.Close();
            return sb.ToString();

            
        } 
        private string deriveResultText(ConfigInfo ci, AlarmMgmtStruct ams)
        {
            SQLQueryHelper sqh = new SQLQueryHelper(ci);
            DomainEmail dm = sqh.getDomainAndEmailFromUser(ci.ec.username);
            DateTime dt = DateTime.Now;
            StringBuilder sb = new StringBuilder();

            if(dm == null)
            {
                return null;
            }
            if(dm.email == null)
            {
                sb.Append("***\n");
                sb.Append(ci.ec.username + " " + dm.domain + "\n" + dt.ToString() + "\n\n");
                sb.Append(ams.note + "\n");
                sb.Append("Alarm managed by ReadGen");

            }else
            {
                sb.Append("***\n");
                sb.Append("[eoc-url=mailto:" + dm.email + "]" + 
                    ci.ec.username + "[//eoc-url] "+ dm.domain + "\n");
                sb.Append(dt.ToString() + "\n\n");
                sb.Append(ams.note + "\n");
                sb.Append("Alarm managed by ReadGen");
            }
            return sb.ToString();
        }
        public String buildAlarmXMLUS(String readXML, CGInfo cgi, String alarmId, String timeStamp,
            String plate, 
            ListDetail ld,ConfigInfo ci)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true,
                OmitXmlDeclaration = true
            };
            AlarmMgmtStruct ams  = ci.getRandomAlarmMgmt();
            XmlWriter xw = XmlWriter.Create(sb, xmlWriterSettings);
            xw.WriteStartDocument();
            xw.WriteStartElement("alarm", "elsag:lprcore");
            xw.WriteAttributeString("id", alarmId);
            xw.WriteAttributeString("rev", "1");
            xw.WriteAttributeString("status", ams.status.ToString());
            xw.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
            xw.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");

            xw.WriteStartElement("alarmtimestamp");
            xw.WriteString(timeStamp);
            xw.WriteEndElement();//alarmtimestamp

            if(ams.status != 4)
            {
                xw.WriteStartElement("RejectedReason2");
                xw.WriteString(ams.reason.ToString());
                xw.WriteEndElement();//RejectedReason2
                string s = deriveResultText(ci, ams);
                if(s != null)
                {
                    xw.WriteStartElement("ResultText");
                    xw.WriteString(ams.note);
                    xw.WriteEndElement();//ResultText
                }
                
            }

            xw.WriteStartElement("domain");
            xw.WriteString(ld.listDomainId.ToString());
            xw.WriteEndElement();//domain
            //Write the READ XML
            xw.WriteRaw(readXML);
            xw.WriteStartElement("hotlistentry");
            xw.WriteAttributeString("id", ld.list_detail_id);
            xw.WriteAttributeString("rev", "1");
            xw.WriteAttributeString("ListId", ld.list_id);
            xw.WriteAttributeString("ListTypeId", ld.list_type_id.ToString());
            /*
            xw.WriteStartElement("DomainId");
            xw.WriteString("1");
            xw.WriteEndElement();//END DomainId
            */
            if(ld.locale_id != null)
            {
                xw.WriteStartElement("LocaleCode");
                xw.WriteString(ld.locale_id);
                xw.WriteEndElement();
            }
            xw.WriteStartElement("Plate");
            xw.WriteString(plate);
            xw.WriteEndElement();//end Plate

            xw.WriteStartElement("OfficerNotes");
            xw.WriteEndElement();//END OfficerNotes

            xw.WriteStartElement("AlarmClassId2");
            
            xw.WriteString(ld.alarm_class_id.ToString());
            xw.WriteEndElement();//AlarmClassId2

            xw.WriteStartElement("CreateDate");
            xw.WriteString(ld.created_date.ToString()) ;
            xw.WriteEndElement();//END Create Date

            if(ld.begin_date != null)
            {
                xw.WriteStartElement("BeginDate");
                xw.WriteString(ld.begin_date.ToString());
                xw.WriteEndElement();//END BeginDate
            }
            if(ld.end_date != null)
            {
                xw.WriteStartElement("EndDate");
                xw.WriteString(ld.end_date.ToString());
                xw.WriteEndElement();//END EndDate
            }
            if(ld.notes != null)
            {
                xw.WriteStartElement("Notes");
                xw.WriteString(ams.note);
                xw.WriteEndElement();//END Notes
            }

            xw.WriteEndElement();//END hotlistentry

            xw.WriteEndElement();//END alarm
            xw.WriteEndDocument();
            xw.Close();
            return sb.ToString();           
        }

        public String buildAlarmXMLUK(String readXML, CGInfo cgi, String alarmId, String timeStamp, String plate, String hotListId,
    String yesterdayStr)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true,
                OmitXmlDeclaration = true
            };
            XmlWriter xw = XmlWriter.Create(sb, xmlWriterSettings);
            xw.WriteStartDocument();
            xw.WriteStartElement("alarm", "elsag:lprcore");
            xw.WriteAttributeString("id", alarmId);
            xw.WriteAttributeString("rev", "1");
            xw.WriteAttributeString("status", "4");
            xw.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
            xw.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");

            xw.WriteStartElement("alarmtimestamp");
            xw.WriteString(timeStamp);
            xw.WriteEndElement();

            xw.WriteStartElement("domain");
            xw.WriteString("1");
            xw.WriteEndElement();
            xw.WriteRaw(readXML);

            xw.WriteStartElement("hotlistentryid");
            xw.WriteAttributeString("id", hotListId);
            xw.WriteAttributeString("rev", "1");
            xw.WriteAttributeString("ListId", "cd8ed894-23c3-4c2e-ad6b-f91ba7b14b66");
            xw.WriteAttributeString("ListTypeId", "7");


            xw.WriteStartElement("LocalCode");
            xw.WriteString("GB");
            xw.WriteEndElement();

            xw.WriteStartElement("Plate");
            xw.WriteString(plate);
            xw.WriteEndElement();

            xw.WriteStartElement("AlarmClassId2");
            xw.WriteString("22");
            xw.WriteEndElement();

            xw.WriteStartElement("AlarmClassId");
            xw.WriteString("22");
            xw.WriteEndElement();

            xw.WriteStartElement("BeginDate");
            xw.WriteString(yesterdayStr);
            xw.WriteEndElement();
            /*
            xw.WriteStartElement("Notes");
            xw.WriteString("C-Burglary");
            xw.WriteEndElement();
            */
            xw.WriteStartElement("CreateDate");
            xw.WriteString(timeStamp);
            xw.WriteEndElement();
            xw.WriteEndElement();

            xw.WriteEndDocument();
            xw.Close();
            return sb.ToString();
        }
    }
}


