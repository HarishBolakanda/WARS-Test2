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
    public class PaymentExchangeRatesDAL : IPaymentExchangeRatesDAL
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

        public DataSet GetInitialData(out string primaryCompanyCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            primaryCompanyCode = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter curCompanyList = new OracleParameter();
                OracleParameter curPaymentExchangeRateData = new OracleParameter();
                OracleParameter outPrimaryCompanyCode = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_pack_exchange_rates.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curCompanyList.OracleDbType = OracleDbType.RefCursor;
                curCompanyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyList);
  

                curPaymentExchangeRateData.OracleDbType = OracleDbType.RefCursor;
                curPaymentExchangeRateData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPaymentExchangeRateData);

                outPrimaryCompanyCode.OracleDbType = OracleDbType.Varchar2;
                outPrimaryCompanyCode.Size = 200;
                outPrimaryCompanyCode.Direction = ParameterDirection.Output;
                outPrimaryCompanyCode.ParameterName = "PrimaryCompanyCode";
                orlCmd.Parameters.Add(outPrimaryCompanyCode);             

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                primaryCompanyCode = orlCmd.Parameters["PrimaryCompanyCode"].Value.ToString();
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


        public DataSet GetPaymentExchangeRateData(string companyCode,  out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter curCompanyData = new OracleParameter();
                OracleParameter curPaymentExchangeRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_pack_exchange_rates.p_get_exchange_rate_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                curPaymentExchangeRateData.OracleDbType = OracleDbType.RefCursor;
                curPaymentExchangeRateData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPaymentExchangeRateData);

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

        public DataSet UpdatePaymentExchangeRates(string companyCode, string currencyCode, Int32 monthId, double paymentExchangeRate, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter inCurrencyCode = new OracleParameter();
                OracleParameter inMonthId = new OracleParameter();
                OracleParameter inPaymentExchangeRate = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curExchangeRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_pack_exchange_rates.p_update_payment_exchange_rate", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                inCurrencyCode.OracleDbType = OracleDbType.Varchar2;
                inCurrencyCode.Direction = ParameterDirection.Input;
                inCurrencyCode.Value = currencyCode;
                orlCmd.Parameters.Add(inCurrencyCode);

                inMonthId.OracleDbType = OracleDbType.Int32;
                inMonthId.Direction = ParameterDirection.Input;
                inMonthId.Value = monthId;
                orlCmd.Parameters.Add(inMonthId);

                inPaymentExchangeRate.OracleDbType = OracleDbType.Double;
                inPaymentExchangeRate.Direction = ParameterDirection.Input;
                inPaymentExchangeRate.Value = paymentExchangeRate;
                orlCmd.Parameters.Add(inPaymentExchangeRate);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curExchangeRateData.OracleDbType = OracleDbType.RefCursor;
                curExchangeRateData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curExchangeRateData);

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

        public DataSet InsertPaymentExchangeRates(string companyCode, string currencyCode, Int32 month, Int32 year, double paymentExchangeRate, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter inCurrencyCode = new OracleParameter();
                OracleParameter inMonth = new OracleParameter();
                OracleParameter inYear = new OracleParameter();
                OracleParameter inExchangeRateFactor = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curExchangeRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_pack_exchange_rates.p_insert_payment_exchange_rate", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                inCurrencyCode.OracleDbType = OracleDbType.Varchar2;
                inCurrencyCode.Direction = ParameterDirection.Input;
                inCurrencyCode.Value = currencyCode;
                orlCmd.Parameters.Add(inCurrencyCode);

                inMonth.OracleDbType = OracleDbType.Int32;
                inMonth.Direction = ParameterDirection.Input;
                inMonth.Value = month;
                orlCmd.Parameters.Add(inMonth);

                inYear.OracleDbType = OracleDbType.Int32;
                inYear.Direction = ParameterDirection.Input;
                inYear.Value = year;
                orlCmd.Parameters.Add(inYear);

                inExchangeRateFactor.OracleDbType = OracleDbType.Double;
                inExchangeRateFactor.Direction = ParameterDirection.Input;
                inExchangeRateFactor.Value = paymentExchangeRate;
                orlCmd.Parameters.Add(inExchangeRateFactor);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curExchangeRateData.OracleDbType = OracleDbType.RefCursor;
                curExchangeRateData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curExchangeRateData);

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
