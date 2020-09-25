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
    public class BreakdownGroupMaintenanceDAL : IBreakdownGroupMaintenanceDAL
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

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter curBreakdownGroupData = new OracleParameter();
                OracleParameter curTerritoryList = new OracleParameter();
                OracleParameter curConfiguartionList = new OracleParameter();
                OracleParameter curSalesTypeList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_breakdown_group.p_get_intial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curBreakdownGroupData.OracleDbType = OracleDbType.RefCursor;
                curBreakdownGroupData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curBreakdownGroupData);

                curTerritoryList.OracleDbType = OracleDbType.RefCursor;
                curTerritoryList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTerritoryList);

                curConfiguartionList.OracleDbType = OracleDbType.RefCursor;
                curConfiguartionList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curConfiguartionList);

                curSalesTypeList.OracleDbType = OracleDbType.RefCursor;
                curSalesTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSalesTypeList);

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

        public DataSet UpdateBreakdownGroupData(string breakdownGrpCode, string breakdownGrpDesc, string territoryGrpCode,
            string configGrpCode, string salesTypeGrpCode, string GFSPLAccount, string GFSBLAccount, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inBreakdownGrpCode = new OracleParameter();
                OracleParameter inBreakdownGrpDesc = new OracleParameter();
                OracleParameter inTerritoryGrpCode = new OracleParameter();
                OracleParameter inConfigGrpCode = new OracleParameter();
                OracleParameter inSalesTypeGrpCode = new OracleParameter();
                OracleParameter inGFSPLAccount = new OracleParameter();
                OracleParameter inGFSBLAccount = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curBreakdownGroupData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_breakdown_group.p_update_breakdown_group_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inBreakdownGrpCode.OracleDbType = OracleDbType.Varchar2;
                inBreakdownGrpCode.Direction = ParameterDirection.Input;
                inBreakdownGrpCode.Value = breakdownGrpCode;
                orlCmd.Parameters.Add(inBreakdownGrpCode);

                inBreakdownGrpDesc.OracleDbType = OracleDbType.Varchar2;
                inBreakdownGrpDesc.Direction = ParameterDirection.Input;
                inBreakdownGrpDesc.Value = breakdownGrpDesc;
                orlCmd.Parameters.Add(inBreakdownGrpDesc);

                inTerritoryGrpCode.OracleDbType = OracleDbType.Varchar2;
                inTerritoryGrpCode.Direction = ParameterDirection.Input;
                inTerritoryGrpCode.Value = territoryGrpCode;
                orlCmd.Parameters.Add(inTerritoryGrpCode);

                inConfigGrpCode.OracleDbType = OracleDbType.Varchar2;
                inConfigGrpCode.Direction = ParameterDirection.Input;
                inConfigGrpCode.Value = configGrpCode;
                orlCmd.Parameters.Add(inConfigGrpCode);

                inSalesTypeGrpCode.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeGrpCode.Direction = ParameterDirection.Input;
                inSalesTypeGrpCode.Value = salesTypeGrpCode;
                orlCmd.Parameters.Add(inSalesTypeGrpCode);

                inGFSPLAccount.OracleDbType = OracleDbType.Varchar2;
                inGFSPLAccount.Direction = ParameterDirection.Input;
                inGFSPLAccount.Value = GFSPLAccount;
                orlCmd.Parameters.Add(inGFSPLAccount);

                inGFSBLAccount.OracleDbType = OracleDbType.Varchar2;
                inGFSBLAccount.Direction = ParameterDirection.Input;
                inGFSBLAccount.Value = GFSBLAccount;
                orlCmd.Parameters.Add(inGFSBLAccount);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curBreakdownGroupData.OracleDbType = OracleDbType.RefCursor;
                curBreakdownGroupData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curBreakdownGroupData);

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

        public DataSet InsertBreakdownGroupData(string breakdownGrpCode, string breakdownGrpDesc, string territoryGrpCode,
            string configGrpCode, string salesTypeGrpCode, string GFSPLAccount, string GFSBLAccount, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inBreakdownGrpCode = new OracleParameter();
                OracleParameter inBreakdownGrpDesc = new OracleParameter();
                OracleParameter inTerritoryGrpCode = new OracleParameter();
                OracleParameter inConfigGrpCode = new OracleParameter();
                OracleParameter inSalesTypeGrpCode = new OracleParameter();
                OracleParameter inGFSPLAccount = new OracleParameter();
                OracleParameter inGFSBLAccount = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curBreakdownGroupData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_breakdown_group.p_insert_breakdown_group_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inBreakdownGrpCode.OracleDbType = OracleDbType.Varchar2;
                inBreakdownGrpCode.Direction = ParameterDirection.Input;
                inBreakdownGrpCode.Value = breakdownGrpCode;
                orlCmd.Parameters.Add(inBreakdownGrpCode);

                inBreakdownGrpDesc.OracleDbType = OracleDbType.Varchar2;
                inBreakdownGrpDesc.Direction = ParameterDirection.Input;
                inBreakdownGrpDesc.Value = breakdownGrpDesc;
                orlCmd.Parameters.Add(inBreakdownGrpDesc);

                inTerritoryGrpCode.OracleDbType = OracleDbType.Varchar2;
                inTerritoryGrpCode.Direction = ParameterDirection.Input;
                inTerritoryGrpCode.Value = territoryGrpCode;
                orlCmd.Parameters.Add(inTerritoryGrpCode);

                inConfigGrpCode.OracleDbType = OracleDbType.Varchar2;
                inConfigGrpCode.Direction = ParameterDirection.Input;
                inConfigGrpCode.Value = configGrpCode;
                orlCmd.Parameters.Add(inConfigGrpCode);

                inSalesTypeGrpCode.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeGrpCode.Direction = ParameterDirection.Input;
                inSalesTypeGrpCode.Value = salesTypeGrpCode;
                orlCmd.Parameters.Add(inSalesTypeGrpCode);

                inGFSPLAccount.OracleDbType = OracleDbType.Varchar2;
                inGFSPLAccount.Direction = ParameterDirection.Input;
                inGFSPLAccount.Value = GFSPLAccount;
                orlCmd.Parameters.Add(inGFSPLAccount);

                inGFSBLAccount.OracleDbType = OracleDbType.Varchar2;
                inGFSBLAccount.Direction = ParameterDirection.Input;
                inGFSBLAccount.Value = GFSBLAccount;
                orlCmd.Parameters.Add(inGFSBLAccount);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curBreakdownGroupData.OracleDbType = OracleDbType.RefCursor;
                curBreakdownGroupData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curBreakdownGroupData);

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

        public DataSet DeleteBreakdownGroupData(string breakdownGrpCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inBreakdownGrpCode = new OracleParameter();
                OracleParameter curBreakdownGroupData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_breakdown_group.p_delete_breakdown_group_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inBreakdownGrpCode.OracleDbType = OracleDbType.Varchar2;
                inBreakdownGrpCode.Direction = ParameterDirection.Input;
                inBreakdownGrpCode.Value = breakdownGrpCode;
                orlCmd.Parameters.Add(inBreakdownGrpCode);

                curBreakdownGroupData.OracleDbType = OracleDbType.RefCursor;
                curBreakdownGroupData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curBreakdownGroupData);

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
