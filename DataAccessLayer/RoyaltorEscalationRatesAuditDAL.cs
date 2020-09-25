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
    public class RoyaltorEscalationRatesAuditDAL : IRoyaltorEscalationRatesAuditDAL
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


        public DataSet GetEscalationRatesAuditData(string royaltorId, string fromDate, string toDate, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter inFromDate = new OracleParameter();
                OracleParameter inToDate = new OracleParameter();
                OracleParameter curRoyAuditData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_royaltor_esc_rates_audit.p_get_royaltor_esc_rates_audit", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                inFromDate.OracleDbType = OracleDbType.Varchar2;
                inFromDate.Direction = ParameterDirection.Input;
                inFromDate.Value = fromDate;
                orlCmd.Parameters.Add(inFromDate);

                inToDate.OracleDbType = OracleDbType.Varchar2;
                inToDate.Direction = ParameterDirection.Input;
                inToDate.Value = toDate;
                orlCmd.Parameters.Add(inToDate);

                curRoyAuditData.OracleDbType = OracleDbType.RefCursor;
                curRoyAuditData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curRoyAuditData);

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
