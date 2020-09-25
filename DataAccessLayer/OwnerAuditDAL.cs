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
    public class OwnerAuditDAL : IOwnerAuditDAL
    {
        #region Global Declarations
        ConnectionDAL connDAL;
        OracleConnection orlConn;
        OracleTransaction txn;
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        OracleParameter ErrorId;
        string sErrorMsg;
        #endregion Global Declarations

        public DataSet GetOwnerAuditData(string ownerCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inOwnerCode = new OracleParameter();
                OracleParameter curOwnerData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_owner_audit.p_get_owner_audit_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inOwnerCode.OracleDbType = OracleDbType.Varchar2;
                inOwnerCode.Direction = ParameterDirection.Input;
                inOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(inOwnerCode);

                curOwnerData.OracleDbType = OracleDbType.RefCursor;
                curOwnerData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curOwnerData);

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
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

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
