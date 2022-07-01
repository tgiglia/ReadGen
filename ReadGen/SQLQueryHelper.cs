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
        protected int numDomains;
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
        public string getReaderFromCameraName(string cameraName)
        {
            string readerName = null;
            if(!openSqlConnection())
            {
                Console.WriteLine("SQLQueryHelper::getReaderFromCameraName: " +
                    "Could not open connection to: " + cd.ec.datasource);
                return null;
            }
            //Get the parent ID from the CameraName
            string parentId = getParentID(cameraName);
            if(parentId == null)
            {
                return null;
            }

            //Get the reader from the parent ID of the Camera Name.
            readerName = getReaderFromParentId(parentId);
            closeSqlConnection();
            return readerName;
        }
        private string getReaderFromParentId(string parentId)
        {
            //select description from sites where site_id = '7EF7AE03-B003-4919-8B50-F9B006538059';
            String sql = "select description from sites where site_id = '" + parentId + "'";
            string readerName = null;
            SqlCommand command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while(reader.Read())
            {
                readerName = reader.GetString(0);
            }
            reader.Close();
            return readerName;
        }
        private string getParentID(string cameraName)
        {
            String parentId = null;
            String sql = "select parent_id from sites where description='" + cameraName + "'";
            Console.WriteLine(sql);
            SqlCommand command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while(reader.Read())
            {
                parentId = reader.GetGuid(0).ToString();
            }
            reader.Close();
            return parentId;
        }
    }
}
