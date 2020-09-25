using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Web;

namespace WARS.DataAccessLayer
{
    public class ConnectionDAL
    {
        string sConnStr;
        OracleConnection connObj;

        public ConnectionDAL()
        {
            System.Web.HttpContext.Current.Session["WARSDBConnectionString"] = "Data Source=WARSD14.WORLD;User Id=WARSE;Password=warse";//********* FOR TESTING ONLY
            sConnStr = Convert.ToString(System.Web.HttpContext.Current.Session["WARSDBConnectionString"]);             
            connObj = new OracleConnection(sConnStr);
        }

        public OracleConnection Open(out Int32 iErrorId, out string sErrorMsg)
        {
            try
            {
                iErrorId = 0;
                sErrorMsg = string.Empty;
                if (connObj.State == System.Data.ConnectionState.Closed)
                {
                    connObj.Open();
                }
            }
            catch (Exception ex)
            {
                iErrorId = 2;
                sErrorMsg = "ERROR: Unable to open database connection. <br><br>" + ex.Message;
                throw ex;
            }
            return connObj;
        }
        
        public void Close()
        {
            if (connObj.State == System.Data.ConnectionState.Open)
            {
                connObj.Close();
            }
        }
                

    }
}
