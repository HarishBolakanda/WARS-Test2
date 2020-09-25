using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;
using Oracle.DataAccess.Types;

namespace WARS.DataAccessLayer
{
    public class BOReportsDAL : IBOReportsDAL
    {

        #region Global Declarations        
        ConnectionDAL connDAL;
        OracleConnection orlConn;        
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        OracleParameter ErrorId;        
        string sErrorMsg;
        #endregion Global Declarations

        public void GetBOReportFolderCuid(string rptFolderName, out string rptFolderCUID, out Int32 iErrorId)
        {            
            rptFolderCUID = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();                
                ErrorId = new OracleParameter();
                OracleParameter pRptFolderName = new OracleParameter();
                OracleParameter pRptFolderCUID = new OracleParameter();

                orlCmd = new OracleCommand("pkg_bo_reports_screen.p_get_bo_rpt_folder_cuid", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRptFolderName.OracleDbType = OracleDbType.Varchar2;
                pRptFolderName.Direction = ParameterDirection.Input;
                pRptFolderName.Value = rptFolderName;
                orlCmd.Parameters.Add(pRptFolderName);

                pRptFolderCUID.OracleDbType = OracleDbType.Varchar2;
                pRptFolderCUID.Size = 200;
                pRptFolderCUID.Direction = System.Data.ParameterDirection.Output;
                pRptFolderCUID.ParameterName = "rptFolderCUID";
                orlCmd.Parameters.Add(pRptFolderCUID);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                rptFolderCUID = orlCmd.Parameters["rptFolderCUID"].Value.ToString();
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                                
                
            }
            catch (Exception ex)
            {
                iErrorId = 2;                
            }
            finally
            {
                CloseConnection();
            }
            

        }

        
       
        #region Private Methods
        private void OpenConnection(out Int32 iErrorId, out string sErrorMsg)
        {
            connDAL = new ConnectionDAL();
            orlConn = connDAL.Open(out iErrorId, out sErrorMsg);
        }

        public void CloseConnection()
        {
            if (connDAL != null)
            {
                connDAL.Close();
            }
        }
        #endregion

    }
}
