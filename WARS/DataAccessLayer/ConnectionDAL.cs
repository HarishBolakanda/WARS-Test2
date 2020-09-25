using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Oracle.DataAccess.Client;

namespace WARS.DataAccessLayer
{
    public class ConnectionDAL
    {
        string sConnStr;
        OracleConnection connObj;

        public ConnectionDAL()
        {            
            sConnStr = ConfigurationManager.ConnectionStrings["WARSConnectionString"].ConnectionString;
            connObj = new OracleConnection(sConnStr);
        }

        public void LoginConnectionDAL()
        {
            sConnStr = ConfigurationManager.ConnectionStrings["WARSConnectionString"].ConnectionString;
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
