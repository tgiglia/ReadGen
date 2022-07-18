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
                                     ConfigInfo configData,ReadStruct rs)
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
            xw.WriteStartElement("imagedata");
            string s = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
            xw.WriteString(s);
            xw.WriteEndElement();

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
            //create imagedata element
            xw.WriteStartElement("imagedata");
            //write imagedata element
            s = Convert.ToBase64String(oimageBytes, 0, oimageBytes.Length);
            xw.WriteString(s);
            //end imagedata element
            xw.WriteEndElement();

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

            xw.WriteEndElement();//end read element
            xw.WriteEndDocument();
            xw.Close();
            return sb.ToString();

        }

    }
}
