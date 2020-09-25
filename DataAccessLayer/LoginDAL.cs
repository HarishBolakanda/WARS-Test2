using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;

namespace WARS.DataAccessLayer
{
    public class LoginDAL : ILoginDAL
    {

        #region Global Declarations        
        ConnectionDAL connDAL;
        OracleConnection orlConn;
        //OracleTransaction txn;
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        OracleParameter ErrorId;        
        #endregion Global Declarations


        public DataTable UserAuthentication(string loggedUser, out Int32 iErrorId, out string sErrorMsg)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                OracleParameter logged_user = new OracleParameter();
                OracleParameter cur_user_info = new OracleParameter();

                orlCmd = new OracleCommand("pkg_common_screens.p_user_authentication", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

                cur_user_info.OracleDbType = OracleDbType.RefCursor;
                cur_user_info.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_user_info);
                
                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);                               
                
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                
            }
            catch (Exception ex)
            {
                iErrorId = 2;
                sErrorMsg = "ERROR: Unable to get user data. <br><br>" + ex.Message;
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds.Tables[0];

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
