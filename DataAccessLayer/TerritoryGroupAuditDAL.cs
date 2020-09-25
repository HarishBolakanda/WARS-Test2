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
    public class TerritoryGroupAuditDAL : ITerritoryGroupAuditDAL
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
        
        public DataSet GetTerritoryGroupAuditData(string territoryGroupCode, string fromDate, string toDate, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inTerritoryGroupCode = new OracleParameter();
                OracleParameter inFromDate = new OracleParameter();
                OracleParameter inToDate = new OracleParameter();
                OracleParameter curterritoryGroupAuditData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_territory_group_audit.p_get_territory_groups_audit", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inTerritoryGroupCode.OracleDbType = OracleDbType.Varchar2;
                inTerritoryGroupCode.Direction = ParameterDirection.Input;
                inTerritoryGroupCode.Value = territoryGroupCode;
                orlCmd.Parameters.Add(inTerritoryGroupCode);

                inFromDate.OracleDbType = OracleDbType.Varchar2;
                inFromDate.Direction = ParameterDirection.Input;
                inFromDate.Value = fromDate;
                orlCmd.Parameters.Add(inFromDate);

                inToDate.OracleDbType = OracleDbType.Varchar2;
                inToDate.Direction = ParameterDirection.Input;
                inToDate.Value = toDate;
                orlCmd.Parameters.Add(inToDate);

                curterritoryGroupAuditData.OracleDbType = OracleDbType.RefCursor;
                curterritoryGroupAuditData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curterritoryGroupAuditData);

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
