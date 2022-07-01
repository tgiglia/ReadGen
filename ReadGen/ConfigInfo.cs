using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace ReadGen
{
    public class ConfigInfo
    {
        String appConfigPath;
        String envrionmentConfigPath;
        public List<String> cameras;
        public List<String> alarmUsers;
        public String errorDesc { get; set; }
        public ApplicationConfig ac { get; set; }
        public EnvironmentConfig ec { get; set; }
        public ReadContainer rc { get; set; }
        public AlarmMgmtContainer amc { get; set; }
        public ConfigInfo(String acp, String ecp)
        {
            appConfigPath = acp;
            envrionmentConfigPath = ecp;
            ac = new ApplicationConfig();
            ac.ProcClassification = ApplicationConfig.procclass.unknown;
            ec = new EnvironmentConfig();
            errorDesc = "No Failure";
            cameras = new List<String>();
            alarmUsers = new List<String>();
        }

        public bool Load()
        {
            if(!parseApplicationConfig())
            {
                return false;
            }
            if(!parseEnvironmentConfig())
            {
                return false;
            }
            if(ac.readfile != null)
            {
                ReadFileJSONParser rfjp = new ReadFileJSONParser();
                rc = rfjp.parseReadFile(this);
            }
            if(ac.camfile != null)
            {
               if(!loadCameraFile(ac.camfile))
                {
                    Console.WriteLine("Load: ERROR! could not load camfile: " + ac.camfile);
                }
            }
            if(ac.alarmuserfile != null)
            {
                if(!loadAlarmUsersFile(ac.alarmuserfile))
                {
                    Console.WriteLine("Load: ERROR! could not load camfile: " + ac.alarmuserfile);
                }
            }
            if(ac.alarmmgtfile != null)
            {
                AlarmMgmtJSONParser amjp = new AlarmMgmtJSONParser();
                amc = amjp.parseFile(ac.alarmmgtfile);
            }
            return true;
        }
        private bool loadCameraFile(String filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line.Length > 0)
                        {
                            cameras.Add(line);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            
            return true;
        }
        private bool loadAlarmUsersFile(String filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line.Length > 0)
                        {
                            alarmUsers.Add(line);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        private bool parseEnvironmentConfig()
        {
            try
            {
                XmlDocument appXml = new XmlDocument();
                appXml.Load(envrionmentConfigPath);
                XmlNode configs = appXml.SelectSingleNode("configuration");
                if (configs == null)
                {
                    errorDesc = "ConfigData::parseEnvironmentConfig: configs is null.";
                    return false;
                }
                XmlNode xnReadAgg = findField(configs, "readAgg");
                if (xnReadAgg == null)
                {
                    return false;
                }
                ec.readAgg = xnReadAgg.Value;

                XmlNode xnUsername = findField(configs, "username");
                if(xnUsername == null)
                {
                    return false;
                }
                ec.username = xnUsername.Value;

                XmlNode xnPassword = findField(configs, "password");
                if(xnPassword == null)
                {
                    return false;
                }
                ec.password = xnPassword.Value;

                XmlNode xnCatalog = findField(configs, "catalog");
                if(xnCatalog == null)
                {
                    return false;
                }
                ec.catalog = xnCatalog.Value;

                XmlNode xnTimezone = findField(configs, "timezone");
                if(xnTimezone == null)
                {
                    return false;
                }
                ec.timezone = xnTimezone.Value;

                XmlNode xnDataSource = findField(configs, "datasource");
                if(xnDataSource == null)
                {
                    return false;
                }
                ec.datasource = xnDataSource.Value;

                XmlNode xnGenalarms = findField(configs, "genalarms");
                if(xnGenalarms == null)
                {
                    return false;
                }
                ec.genalarms = xnGenalarms.Value;

            }
            catch(Exception e)
            {
                errorDesc = e.Message;
                return false;
            }
            return true;
        }
        private void setProcClass()
        {
            if(ac.proctype.Equals("sequential"))
            {
                ac.ProcClassification = ApplicationConfig.procclass.sequential;
                return;
            }
            if(ac.proctype.Equals("random"))
            {
                ac.ProcClassification = ApplicationConfig.procclass.random;
                return;
            }
            if(ac.proctype.Equals("lookup"))
            {
                ac.ProcClassification = ApplicationConfig.procclass.lookup;
                return;
            }
            ac.ProcClassification = ApplicationConfig.procclass.unknown;
        }
        private bool parseApplicationConfig()
        {
            try
            {
                
                XmlDocument appXml = new XmlDocument();
                appXml.Load(appConfigPath);
                XmlNode configs = appXml.SelectSingleNode("configuration");
                if (configs == null)
                {
                    errorDesc = "ConfigData::parseApplicationConfig: configs is null.";
                    return false;
                }
                XmlNode proc = configs.SelectSingleNode("proctype");
                if(proc == null)
                {
                    errorDesc = "ConfigData::parseApplicationConfig: could not find proctype.";
                    return false;
                }
                XmlAttributeCollection values = proc.Attributes;
                XmlNode xnProc = values.GetNamedItem("value");
                ac.proctype = xnProc.Value;
                setProcClass();


                XmlNode xnNumToSend = findField(configs, "numtosend");    
                if(xnNumToSend == null)
                {
                    ac.numtosend = 0;
                }
                else
                {
                    String tmp = xnNumToSend.Value;
                    ac.numtosend = Int32.Parse(tmp);
                }
                

                XmlNode xnMsDelay = findField(configs, "msdelay");
                if(xnMsDelay == null)
                {
                    ac.msdelay = 0;
                }
                else
                {
                    String tmp = xnMsDelay.Value;
                    ac.msdelay = Int32.Parse(tmp);
                }
                

                XmlNode xnReadFile = findField(configs,"readfile");
                if(xnReadFile == null)
                {
                    return false;
                }
                ac.readfile = xnReadFile.Value;

                XmlNode xnCamFile = findField(configs, "camfile");
                if(xnCamFile == null)
                {
                    ac.camfile = null;
                }
                else
                {
                    ac.camfile = xnCamFile.Value;
                }
                

                XmlNode xnAlarmmgtfile = findField(configs, "alarmmgtfile");
                if(xnAlarmmgtfile == null)
                {
                    ac.alarmmgtfile = null;
                }
                else
                {
                    ac.alarmmgtfile = xnAlarmmgtfile.Value;
                }
                

                XmlNode xnAlarmuserfile = findField(configs, "alarmuserfile");
                if(xnAlarmuserfile == null)
                {
                    ac.alarmuserfile = null;
                }
                else
                {
                    ac.alarmuserfile = xnAlarmuserfile.Value;
                }
                

                XmlNode xnOverview_image_path = findField(configs, "overview_image_path");
                if(xnOverview_image_path == null)
                {
                    ac.overview_image_path = null;
                }
                else
                {
                    ac.overview_image_path = xnOverview_image_path.Value;
                }
                

                XmlNode xnPlate_image_path = findField(configs, "plate_image_path");
                if(xnPlate_image_path == null)
                {
                    ac.plate_image_path = null;
                }
                else
                {
                    ac.plate_image_path = xnPlate_image_path.Value;
                }

                

                XmlNode xnPlate_lookup = findField(configs, "plate_lookup");
                if(xnPlate_lookup == null)
                {
                    ac.plate_lookup = null;
                }
                else
                {
                    ac.plate_lookup = xnPlate_lookup.Value;
                }
                

            }
            catch (Exception e)
            {
                errorDesc = e.Message;
                return false;
            }
            return true;
        }
        private XmlNode findField(XmlNode configs, String field)
        {
            XmlNode xn;
            XmlNode proc = configs.SelectSingleNode(field);
            if (proc == null)
            {
                errorDesc = "ConfigData::parseApplicationConfig: could not find " + field;
                return null;
            }
            XmlAttributeCollection values = proc.Attributes;
            xn = values.GetNamedItem("value");

            return xn;

        }
    }
}
