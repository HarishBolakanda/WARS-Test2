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
    public class ParticipantAuditDAL : IParticipantAuditDAL
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
                
        public DataSet GetParticipantData(string catNo,string fromDate,string toDate, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCatno = new OracleParameter();
                OracleParameter inFromDate = new OracleParameter();
                OracleParameter inToDate = new OracleParameter();
                OracleParameter curParticipantData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_participant_audit.p_get_participant_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCatno.OracleDbType = OracleDbType.Varchar2;
                inCatno.Direction = ParameterDirection.Input;
                inCatno.Value = catNo;
                orlCmd.Parameters.Add(inCatno);

                inFromDate.OracleDbType = OracleDbType.Varchar2;
                inFromDate.Direction = ParameterDirection.Input;
                inFromDate.Value = fromDate;
                orlCmd.Parameters.Add(inFromDate);

                inToDate.OracleDbType = OracleDbType.Varchar2;
                inToDate.Direction = ParameterDirection.Input;
                inToDate.Value = toDate;
                orlCmd.Parameters.Add(inToDate);

                curParticipantData.OracleDbType = OracleDbType.RefCursor;
                curParticipantData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curParticipantData);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "out_n_status_code";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

               // iErrorId = Convert.ToInt32(orlCmd.Parameters["out_n_status_code"].Value.ToString());

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
