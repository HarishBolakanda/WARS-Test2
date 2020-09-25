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
    public class CustomerMaintenanceDAL : ICustomerMaintenanceDAL
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
                OracleParameter curCustomerData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_customer.p_get_customer_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curCustomerData.OracleDbType = OracleDbType.RefCursor;
                curCustomerData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCustomerData);

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

        public DataSet UpdateCustomerData(Int32 customerCode, string fixedMobile,
            string displayOnStatementGlobal, string displayOnStatementAcount, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCustomerCode = new OracleParameter();
                OracleParameter inFixedMobile = new OracleParameter();
                OracleParameter inDisplayOnStatementGlobal = new OracleParameter();
                OracleParameter inDisplayOnStatementAcount = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curCustomerData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_customer.p_update_customer_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCustomerCode.OracleDbType = OracleDbType.Int32;
                inCustomerCode.Direction = ParameterDirection.Input;
                inCustomerCode.Value = customerCode;
                orlCmd.Parameters.Add(inCustomerCode);

                inFixedMobile.OracleDbType = OracleDbType.Varchar2;
                inFixedMobile.Direction = ParameterDirection.Input;
                inFixedMobile.Value = fixedMobile;
                orlCmd.Parameters.Add(inFixedMobile);

                inDisplayOnStatementGlobal.OracleDbType = OracleDbType.Varchar2;
                inDisplayOnStatementGlobal.Direction = ParameterDirection.Input;
                inDisplayOnStatementGlobal.Value = displayOnStatementGlobal;
                orlCmd.Parameters.Add(inDisplayOnStatementGlobal);

                inDisplayOnStatementAcount.OracleDbType = OracleDbType.Varchar2;
                inDisplayOnStatementAcount.Direction = ParameterDirection.Input;
                inDisplayOnStatementAcount.Value = displayOnStatementAcount;
                orlCmd.Parameters.Add(inDisplayOnStatementAcount);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curCustomerData.OracleDbType = OracleDbType.RefCursor;
                curCustomerData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCustomerData);

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
