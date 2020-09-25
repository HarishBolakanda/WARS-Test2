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
    public class RoyaltorPayeeSuppAuditDAL : IRoyaltorPayeeSuppAuditDAL
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

        public DataSet GetRoyPayeeSuppAuditData(string royaltorId, string intPartyId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                OracleParameter curRoyPayeeSuppAuditData = new OracleParameter();
                OracleParameter curPayeeList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_royaltor_payee_supp_audit.p_get_roy_payee_supp_audit", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                inIntPartyId.OracleDbType = OracleDbType.Varchar2;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                curRoyPayeeSuppAuditData.OracleDbType = OracleDbType.RefCursor;
                curRoyPayeeSuppAuditData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curRoyPayeeSuppAuditData);

                curPayeeList.OracleDbType = OracleDbType.RefCursor;
                curPayeeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeList);

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
