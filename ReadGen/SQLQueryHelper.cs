using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace ReadGen
{
    class SQLQueryHelper
    {
        
        protected ConfigInfo cd;
        protected SqlConnection conn;
        protected SqlDataReader reader;
        public SQLQueryHelper(ConfigInfo cd)
        {
            this.cd = cd;
        }
        public bool openSqlConnection()
        {
            conn = new SqlConnection("user id =...;password=...;Integrated Security=SSPI;Data Source=" + cd.ec.datasource +
               ";Initial Catalog=" + cd.ec.catalog);
            try
            {
                conn.Open();
                Console.WriteLine("Connection to: " + cd.ec.datasource + " successfull.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }
        public void closeSqlConnection()
        {
            try
            {
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public ListDetail getListEntries(string plate, string state, string readDate)
        {
            ListDetail ld = new ListDetail();
            bool bFound = false;
            if (!openSqlConnection())
            {
                Console.WriteLine("SQLQueryHelper::getReaderFromCameraName: " +
                    "Could not open connection to: " + cd.ec.datasource);
                return null;
            }

            String sql = "select list_detail_id,list_id,list_type_id from list_detail where plate='" + plate + "' and begin_date <'" + readDate + "' and ( begin_date <= '" + readDate +
                "' or end_date is null) and list_type_id = (select list_type_id from list_type_lookup where description = 'list_type_lookup_HotList')";
            Console.WriteLine("sql:" + sql);
            SqlCommand command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                ld.list_detail_id = reader.GetGuid(0).ToString();
                ld.list_id = reader.GetGuid(1).ToString();
                ld.list_type_id = reader.GetInt32(2);
                bFound = true;
            }
            closeSqlConnection();
            if (!bFound)
            {
                return null;
            }
            return ld;
        }
        public List<ListDetail> getListEntries(ReadStruct rs,string timeStamp)
        {
            List<ListDetail> ldList = new List<ListDetail>();

            bool bFound = false;
            if (!openSqlConnection())
            {
                Console.WriteLine("SQLQueryHelper::getReaderFromCameraName: " +
                    "Could not open connection to: " + cd.ec.datasource);
                return null;
            }
            string plate = rs.plate;
            string state = rs.state;
            string readDate = timeStamp;
            string sql = deriveListDetailStr(plate, state, readDate);
            Console.WriteLine("sql:" + sql);
            SqlCommand command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                ListDetail ld = new ListDetail();
                ld.list_detail_id = reader.GetGuid(0).ToString();
                ld.list_id = reader.GetGuid(1).ToString();
                ld.list_type_id = reader.GetInt32(2);
                ld.alarm_class_id = reader.GetInt32(3);
                bFound = true;
                ldList.Add(ld);
            }

            closeSqlConnection();
            if (!bFound)
            {
                return null;
            }
            return ldList;
        }
        private string deriveListDetailStr(string plate, string state, string readDate)
        {
            string s = "select list_detail_id,list_id,list_type_id,alarm_class_id from list_detail where plate='" + plate + "' and begin_date <'" + readDate + 
                "' and ( end_date >= '" + readDate +
                "' or end_date is null) and list_type_id = (select list_type_id from list_type_lookup where description = 'list_type_lookup_HotList')";

            if (state == null)
            {
                return s;
            }
            if(state.Length < 2)
            {
                return s;
            }
            string end = "\nand ( locale_id = '" + state + "' or locale_id is null )";
            string complete = s + end;
            return complete;
        }
        public CGInfo getReaderFromCameraName(string cameraName)
        {
            

            if (!openSqlConnection())
            {
                Console.WriteLine("SQLQueryHelper::getReaderFromCameraName: " +
                    "Could not open connection to: " + cd.ec.datasource);
                return null;
            }
            //Get the parent ID from the CameraName
            CGInfo cgi = getParentID(cameraName);
            if(cgi.readerId == null)
            {
                Console.WriteLine("SQLQueryHelper::getReaderFromCameraName: ERROR " + cameraName + " Doesnt have a readerID.");
                return null;
            }

            //Get the reader from the parent ID of the Camera Name.
            CGInfo cgiReader = getReaderFromParentId(cgi.readerId);
            cgi.reader = cgiReader.reader;
            cgi.lat = cgiReader.lat;
            cgi.lon = cgiReader.lon;
            closeSqlConnection();
            return cgi;
        }

        private CGInfo getReaderFromParentId(string parentId)
        {
            CGInfo cgi = new CGInfo();
            //select description from sites where site_id = '7EF7AE03-B003-4919-8B50-F9B006538059';
            String sql = "select description,lat,lon from sites where site_id = '" + parentId + "'";
            string readerName = null;
            SqlCommand command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while(reader.Read())
            {
                readerName = reader.GetString(0);
                cgi.reader = readerName;
                if (!reader.IsDBNull(1))
                {
                    cgi.lat = reader.GetDouble(1);
                }
                if (!reader.IsDBNull(2))
                {
                    cgi.lon = reader.GetDouble(2);
                }
            }
            reader.Close();
            return cgi;
        }
        private CGInfo getParentID(string cameraName)
        {
            CGInfo cgi = new CGInfo();

            cgi.cameraName = cameraName;
            String sql = "select parent_id,site_id,domain_id,lat,lon from sites where description='" + cameraName + "'";
            Console.WriteLine(sql);
            SqlCommand command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while(reader.Read())
            {                
                cgi.readerId = reader.GetGuid(0).ToString();
                
                cgi.cameraId = reader.GetGuid(1).ToString();
                int domain = reader.GetInt32(2);
                cgi.domainIdStr = domain.ToString();
                if(!reader.IsDBNull(3))
                {
                    cgi.lat = reader.GetDouble(3);
                }
                if(!reader.IsDBNull(4))
                {
                    cgi.lon = reader.GetDouble(4);
                }
                
            }
            reader.Close();
            return cgi;
        }
    }
}
