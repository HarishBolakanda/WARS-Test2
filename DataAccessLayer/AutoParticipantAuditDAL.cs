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
    public class AutoParticipantAuditDAL : IAutoParticipantAuditDAL
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

        public DataSet GetAutoParticipantAuditData(string autoPartId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter inAutoPartId = new OracleParameter();
                OracleParameter curAuditData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_auto_participant_audit.p_auto_participant_audit_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inAutoPartId.OracleDbType = OracleDbType.Varchar2;
                inAutoPartId.Direction = ParameterDirection.Input;
                inAutoPartId.Value = autoPartId;
                orlCmd.Parameters.Add(inAutoPartId);

                curAuditData.OracleDbType = OracleDbType.RefCursor;
                curAuditData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curAuditData);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "out_n_status_code";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["out_n_status_code"].Value.ToString());

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
        #endregion Private Methods

    }
}
