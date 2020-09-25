using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;
using Oracle.DataAccess.Types;
using WARS.DataAccessLayer;

namespace WARS.DataAccessLayer
{
    public class SalesTypeGroupingDAL : ISalesTypeGroupingDAL
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
                
        public DataSet GetSalesTypeGroupInOutData(string salesTypeGroupCode, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inGroupCode = new OracleParameter();
                OracleParameter curGroupInData = new OracleParameter();
                OracleParameter curGroupOutdata = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_sales_type_group.p_get_salestype_inout_grp_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inGroupCode.OracleDbType = OracleDbType.Varchar2;
                inGroupCode.Direction = ParameterDirection.Input;
                inGroupCode.Value = salesTypeGroupCode;
                orlCmd.Parameters.Add(inGroupCode);

                curGroupInData.OracleDbType = OracleDbType.RefCursor;
                curGroupInData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupInData);

                curGroupOutdata.OracleDbType = OracleDbType.RefCursor;
                curGroupOutdata.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupOutdata);

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
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }

        public DataSet AddSalesTypeToGroup(string salesTypeGroupCode, Array salesTypecodes, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inSalesTypeGroupCode = new OracleParameter();
                OracleParameter inSalesTypeCodes = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curGroupInData = new OracleParameter();
                OracleParameter curGroupOutdata = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_sales_type_group.p_add_salestype_to_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inSalesTypeGroupCode.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeGroupCode.Direction = ParameterDirection.Input;
                inSalesTypeGroupCode.Value = salesTypeGroupCode;
                orlCmd.Parameters.Add(inSalesTypeGroupCode);

                inSalesTypeCodes.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeCodes.Direction = ParameterDirection.Input;
                inSalesTypeCodes.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (salesTypecodes.Length == 0)
                {
                    inSalesTypeCodes.Size = 0;
                    inSalesTypeCodes.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inSalesTypeCodes.Size = salesTypecodes.Length;
                    inSalesTypeCodes.Value = salesTypecodes;
                }
                orlCmd.Parameters.Add(inSalesTypeCodes);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curGroupInData.OracleDbType = OracleDbType.RefCursor;
                curGroupInData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupInData);

                curGroupOutdata.OracleDbType = OracleDbType.RefCursor;
                curGroupOutdata.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupOutdata);

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
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet RemoveSalesTypeFromGroup(string salesTypeGroupCode, Array salesTypecodes,string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inSalesTypeGroupCode = new OracleParameter();
                OracleParameter inSalesTypeCodes = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curGroupInData = new OracleParameter();
                OracleParameter curGroupOutdata = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_sales_type_group.p_remove_salestype_from_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inSalesTypeGroupCode.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeGroupCode.Direction = ParameterDirection.Input;
                inSalesTypeGroupCode.Value = salesTypeGroupCode;
                orlCmd.Parameters.Add(inSalesTypeGroupCode);

                inSalesTypeCodes.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeCodes.Direction = ParameterDirection.Input;
                inSalesTypeCodes.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (salesTypecodes.Length == 0)
                {
                    inSalesTypeCodes.Size = 0;
                    inSalesTypeCodes.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inSalesTypeCodes.Size = salesTypecodes.Length;
                    inSalesTypeCodes.Value = salesTypecodes;
                }
                orlCmd.Parameters.Add(inSalesTypeCodes);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curGroupInData.OracleDbType = OracleDbType.RefCursor;
                curGroupInData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupInData);

                curGroupOutdata.OracleDbType = OracleDbType.RefCursor;
                curGroupOutdata.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupOutdata);

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
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet InsertSalesTypeGroup(string salesTypeGroupCode, string salesTypeGroupName, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inSaleTypeGroupCode = new OracleParameter();
                OracleParameter inSaleTypeGroupName = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curGroupInData = new OracleParameter();
                OracleParameter curGroupOutdata = new OracleParameter();
                OracleParameter curGroupList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_sales_type_group.p_insert_salestype_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inSaleTypeGroupCode.OracleDbType = OracleDbType.Varchar2;
                inSaleTypeGroupCode.Direction = ParameterDirection.Input;
                inSaleTypeGroupCode.Value = salesTypeGroupCode;
                orlCmd.Parameters.Add(inSaleTypeGroupCode);

                inSaleTypeGroupName.OracleDbType = OracleDbType.Varchar2;
                inSaleTypeGroupName.Direction = ParameterDirection.Input;
                inSaleTypeGroupName.Value = salesTypeGroupName;
                orlCmd.Parameters.Add(inSaleTypeGroupName);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curGroupInData.OracleDbType = OracleDbType.RefCursor;
                curGroupInData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupInData);

                curGroupOutdata.OracleDbType = OracleDbType.RefCursor;
                curGroupOutdata.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupOutdata);

                curGroupList.OracleDbType = OracleDbType.RefCursor;
                curGroupList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupList);

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
