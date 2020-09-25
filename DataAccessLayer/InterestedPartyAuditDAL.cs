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
    public class InterestedPartyAuditDAL : IInterestedPartyAuditDAL
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
                
        public DataSet GetInterestedPartyAuditData(Int32 intPartyId, out string intParty, out Int32 iErrorId)
        {
            ds = new DataSet();
            intParty = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter pIntPartyId = new OracleParameter();
                OracleParameter pIntPartyAuditDetails = new OracleParameter();
                OracleParameter pIntPartyData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_interested_party_audit.p_get_interested_party_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pIntPartyId.OracleDbType = OracleDbType.Int32;
                pIntPartyId.Direction = ParameterDirection.Input;
                pIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(pIntPartyId);

                pIntPartyAuditDetails.OracleDbType = OracleDbType.RefCursor;
                pIntPartyAuditDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pIntPartyAuditDetails);


                pIntPartyData.OracleDbType = OracleDbType.Varchar2;
                pIntPartyData.Size = 250;
                pIntPartyData.Direction = ParameterDirection.Output;
                pIntPartyData.ParameterName = "out_v_interested_party";
                orlCmd.Parameters.Add(pIntPartyData);


                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                intParty = orlCmd.Parameters["out_v_interested_party"].Value.ToString();

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
