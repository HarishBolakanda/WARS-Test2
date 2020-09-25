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
    public class AccountTypeMaintenanceDAL : IAccountTypeMaintenanceDAL
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
                OracleParameter curAccoutTypeData = new OracleParameter();
                OracleParameter curAccountTypeList = new OracleParameter();
                OracleParameter curSourceTypeList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_account_type.p_get_intial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curAccoutTypeData.OracleDbType = OracleDbType.RefCursor;
                curAccoutTypeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curAccoutTypeData);

                curAccountTypeList.OracleDbType = OracleDbType.RefCursor;
                curAccountTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curAccountTypeList);

                curSourceTypeList.OracleDbType = OracleDbType.RefCursor;
                curSourceTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSourceTypeList);

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

        public DataSet UpdateAccountTypeMapData(string accoutCodeType, string isInclude, string sourceType,
            string consolidLevel, string isIncludeNew, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inAccoutCodeType = new OracleParameter();
                OracleParameter inIsInclude = new OracleParameter();
                OracleParameter inSourceType = new OracleParameter();
                OracleParameter inConsolidLevel = new OracleParameter();
                OracleParameter inIsIncludeNew = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curAccountTypeData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_account_type.p_update_account_type_map_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inAccoutCodeType.OracleDbType = OracleDbType.Varchar2;
                inAccoutCodeType.Direction = ParameterDirection.Input;
                inAccoutCodeType.Value = accoutCodeType;
                orlCmd.Parameters.Add(inAccoutCodeType);

                inIsInclude.OracleDbType = OracleDbType.Varchar2;
                inIsInclude.Direction = ParameterDirection.Input;
                inIsInclude.Value = isInclude;
                orlCmd.Parameters.Add(inIsInclude);

                inSourceType.OracleDbType = OracleDbType.Varchar2;
                inSourceType.Direction = ParameterDirection.Input;
                inSourceType.Value = sourceType;
                orlCmd.Parameters.Add(inSourceType);

                inConsolidLevel.OracleDbType = OracleDbType.Varchar2;
                inConsolidLevel.Direction = ParameterDirection.Input;
                inConsolidLevel.Value = consolidLevel;
                orlCmd.Parameters.Add(inConsolidLevel);

                inIsIncludeNew.OracleDbType = OracleDbType.Varchar2;
                inIsIncludeNew.Direction = ParameterDirection.Input;
                inIsIncludeNew.Value = isIncludeNew;
                orlCmd.Parameters.Add(inIsIncludeNew);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curAccountTypeData.OracleDbType = OracleDbType.RefCursor;
                curAccountTypeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curAccountTypeData);

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

        public DataSet InsertAccountTypeMapData(string accoutCodeType, string accountTypeId, string sourceType,
            string consolidLevel, string isInclude, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inAccoutCodeType = new OracleParameter();
                OracleParameter inAccountTypeId = new OracleParameter();
                OracleParameter inSourceType = new OracleParameter();
                OracleParameter inConsolidLevel = new OracleParameter();
                OracleParameter inIsInclude = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curAccountTypeData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_account_type.p_insert_account_type_map_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inAccoutCodeType.OracleDbType = OracleDbType.Varchar2;
                inAccoutCodeType.Direction = ParameterDirection.Input;
                inAccoutCodeType.Value = accoutCodeType;
                orlCmd.Parameters.Add(inAccoutCodeType);

                inAccountTypeId.OracleDbType = OracleDbType.Varchar2;
                inAccountTypeId.Direction = ParameterDirection.Input;
                inAccountTypeId.Value = accountTypeId;
                orlCmd.Parameters.Add(inAccountTypeId);

                inSourceType.OracleDbType = OracleDbType.Varchar2;
                inSourceType.Direction = ParameterDirection.Input;
                inSourceType.Value = sourceType;
                orlCmd.Parameters.Add(inSourceType);

                inConsolidLevel.OracleDbType = OracleDbType.Varchar2;
                inConsolidLevel.Direction = ParameterDirection.Input;
                inConsolidLevel.Value = consolidLevel;
                orlCmd.Parameters.Add(inConsolidLevel);

                inIsInclude.OracleDbType = OracleDbType.Varchar2;
                inIsInclude.Direction = ParameterDirection.Input;
                inIsInclude.Value = isInclude;
                orlCmd.Parameters.Add(inIsInclude);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curAccountTypeData.OracleDbType = OracleDbType.RefCursor;
                curAccountTypeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curAccountTypeData);

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

        public DataSet InsertAccountTypeData(string accountTypeDesc, string displayOrder, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inAccoutCodeDesc = new OracleParameter();
                OracleParameter inDisplayOrder = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curAccountTypeData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_account_type.p_insert_account_type_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inAccoutCodeDesc.OracleDbType = OracleDbType.Varchar2;
                inAccoutCodeDesc.Direction = ParameterDirection.Input;
                inAccoutCodeDesc.Value = accountTypeDesc;
                orlCmd.Parameters.Add(inAccoutCodeDesc);

                inDisplayOrder.OracleDbType = OracleDbType.Varchar2;
                inDisplayOrder.Direction = ParameterDirection.Input;
                inDisplayOrder.Value = displayOrder;
                orlCmd.Parameters.Add(inDisplayOrder);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curAccountTypeData.OracleDbType = OracleDbType.RefCursor;
                curAccountTypeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curAccountTypeData);

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
