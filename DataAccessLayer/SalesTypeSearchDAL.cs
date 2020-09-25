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
    public class SalesTypeSearchDAL : ISalesTypeSearchDAL
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
                orlDA = new OracleDataAdapter();

                OracleParameter curSalesTypeList = new OracleParameter();
                OracleParameter curPriceTypeList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_sales_type_search.p_get_intial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curSalesTypeList.OracleDbType = OracleDbType.RefCursor;
                curSalesTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSalesTypeList);

                curPriceTypeList.OracleDbType = OracleDbType.RefCursor;
                curPriceTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPriceTypeList);                

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

        public DataSet GetSalesTypeData(string salesTypeCode, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inSalesTypeCode = new OracleParameter();
                OracleParameter curSalesTypeData = new OracleParameter();
                OracleParameter curSalesTypeGroupData = new OracleParameter();                 

                orlCmd = new OracleCommand("pkg_sales_type_search.p_get_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inSalesTypeCode.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeCode.Direction = ParameterDirection.Input;
                inSalesTypeCode.Value = salesTypeCode;
                orlCmd.Parameters.Add(inSalesTypeCode);

                curSalesTypeData.OracleDbType = OracleDbType.RefCursor;
                curSalesTypeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSalesTypeData);

                curSalesTypeGroupData.OracleDbType = OracleDbType.RefCursor;
                curSalesTypeGroupData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSalesTypeGroupData);                

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

        public DataSet SaveSalesTypeGroup(string flag, string salesTypeCode, string salesTypeName, string salesTypeType, string escalationProrata, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inFlag = new OracleParameter();
                OracleParameter inSalesTypeCode = new OracleParameter();
                OracleParameter inSalesTypeName = new OracleParameter();
                OracleParameter inSalesTypeType = new OracleParameter();
                OracleParameter inEscalationProrata = new OracleParameter();
                OracleParameter inUsercode = new OracleParameter();
                OracleParameter curSalesTypeList = new OracleParameter();
                OracleParameter curSalesTypeData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_sales_type_search.p_save_sales_type_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inFlag.OracleDbType = OracleDbType.Varchar2;
                inFlag.Direction = ParameterDirection.Input;
                inFlag.Value = flag;
                orlCmd.Parameters.Add(inFlag);

                inSalesTypeCode.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeCode.Direction = ParameterDirection.Input;
                inSalesTypeCode.Value = salesTypeCode;
                orlCmd.Parameters.Add(inSalesTypeCode);

                inSalesTypeName.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeName.Direction = ParameterDirection.Input;
                inSalesTypeName.Value = salesTypeName;
                orlCmd.Parameters.Add(inSalesTypeName);

                inSalesTypeType.OracleDbType = OracleDbType.Varchar2;
                inSalesTypeType.Direction = ParameterDirection.Input;
                inSalesTypeType.Value = salesTypeType;
                orlCmd.Parameters.Add(inSalesTypeType);

                inEscalationProrata.OracleDbType = OracleDbType.Varchar2;
                inEscalationProrata.Direction = ParameterDirection.Input;
                inEscalationProrata.Value = escalationProrata;
                orlCmd.Parameters.Add(inEscalationProrata);

                inUsercode.OracleDbType = OracleDbType.Varchar2;
                inUsercode.Direction = ParameterDirection.Input;
                inUsercode.Value = userCode;
                orlCmd.Parameters.Add(inUsercode);

                curSalesTypeList.OracleDbType = OracleDbType.RefCursor;
                curSalesTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSalesTypeList);

                curSalesTypeData.OracleDbType = OracleDbType.RefCursor;
                curSalesTypeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSalesTypeData);

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
