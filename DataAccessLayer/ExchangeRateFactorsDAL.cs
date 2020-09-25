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
    public class ExchangeRateFactorsDAL : IExchangeRateFactorsDAL
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
                OracleParameter curCompanyList = new OracleParameter();
                OracleParameter curPrimaryCompanyData = new OracleParameter();
                OracleParameter curExchangeRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_exchange_rates.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curCompanyList.OracleDbType = OracleDbType.RefCursor;
                curCompanyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyList);

                curPrimaryCompanyData.OracleDbType = OracleDbType.RefCursor;
                curPrimaryCompanyData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPrimaryCompanyData);

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

        public DataSet GetCompanyData(string companyCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter curCompanyData = new OracleParameter();
                OracleParameter curExchangeRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_exchange_rates.p_get_company_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                curCompanyData.OracleDbType = OracleDbType.RefCursor;
                curCompanyData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyData);

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

        public DataSet UpdateExchangeRateFactor(string companyCode, Int32 monthId, double exchangeRateFactor, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter inMonthId = new OracleParameter();
                OracleParameter inExchangeRateFactor = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curExchangeRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_exchange_rates.p_update_exchange_rate_factor", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                inMonthId.OracleDbType = OracleDbType.Int32;
                inMonthId.Direction = ParameterDirection.Input;
                inMonthId.Value = monthId;
                orlCmd.Parameters.Add(inMonthId);

                inExchangeRateFactor.OracleDbType = OracleDbType.Double;
                inExchangeRateFactor.Direction = ParameterDirection.Input;
                inExchangeRateFactor.Value = exchangeRateFactor;
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

        public DataSet InsertExchangeRateFactor(string companyCode, Int32 month, Int32 year, double exchangeRateFactor, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter inMonth = new OracleParameter();
                OracleParameter inYear = new OracleParameter();
                OracleParameter inExchangeRateFactor = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curExchangeRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_exchange_rates.p_insert_exchange_rate_factor", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

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
                inExchangeRateFactor.Value = exchangeRateFactor;
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

        //public DataSet UpdateTransactions(string companyCode, string currencyCode, string domesticCurrencyGroup, Array monthIds, Array exchangeRateFactor, string userCode, out Int32 iErrorId)
        //{
        //    ds = new DataSet();
        //    try
        //    {
        //        OpenConnection(out iErrorId, out sErrorMsg);

        //        ErrorId = new OracleParameter();
        //        OracleParameter inCompanyCode = new OracleParameter();
        //        OracleParameter inCurrencyCode = new OracleParameter();
        //        OracleParameter inDomesticCurrencyGroup = new OracleParameter();
        //        OracleParameter inMonthIds = new OracleParameter();
        //        OracleParameter inExchangeRateFactors = new OracleParameter();
        //        OracleParameter inUserCode = new OracleParameter();
        //        OracleParameter curExchangeRateData = new OracleParameter();
        //        orlDA = new OracleDataAdapter();

        //        orlCmd = new OracleCommand("pkg_maint_exchange_rates.p_trigger_trans_scheduler_job", orlConn);
        //        orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //        inCompanyCode.OracleDbType = OracleDbType.Varchar2;
        //        inCompanyCode.Direction = ParameterDirection.Input;
        //        inCompanyCode.Value = companyCode;
        //        orlCmd.Parameters.Add(inCompanyCode);

        //        inCurrencyCode.OracleDbType = OracleDbType.Varchar2;
        //        inCurrencyCode.Direction = ParameterDirection.Input;
        //        inCurrencyCode.Value = currencyCode;
        //        orlCmd.Parameters.Add(inCurrencyCode);

        //        inDomesticCurrencyGroup.OracleDbType = OracleDbType.Varchar2;
        //        inDomesticCurrencyGroup.Direction = ParameterDirection.Input;
        //        inDomesticCurrencyGroup.Value = domesticCurrencyGroup;
        //        orlCmd.Parameters.Add(inDomesticCurrencyGroup);

        //        inMonthIds.OracleDbType = OracleDbType.Int32;
        //        inMonthIds.Direction = ParameterDirection.Input;
        //        inMonthIds.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
        //        if (monthIds.Length == 0)
        //        {
        //            inMonthIds.Size = 0;
        //            inMonthIds.Value = new OracleDecimal[1] { OracleDecimal.Null };
        //        }
        //        else
        //        {
        //            inMonthIds.Size = monthIds.Length;
        //            inMonthIds.Value = monthIds;
        //        }
        //        orlCmd.Parameters.Add(inMonthIds);

        //        inExchangeRateFactors.OracleDbType = OracleDbType.Double;
        //        inExchangeRateFactors.Direction = ParameterDirection.Input;
        //        inExchangeRateFactors.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
        //        if (exchangeRateFactor.Length == 0)
        //        {
        //            inExchangeRateFactors.Size = 0;
        //            inExchangeRateFactors.Value = new OracleDecimal[1] { OracleDecimal.Null };
        //        }
        //        else
        //        {
        //            inExchangeRateFactors.Size = exchangeRateFactor.Length;
        //            inExchangeRateFactors.Value = exchangeRateFactor;
        //        }
        //        orlCmd.Parameters.Add(inExchangeRateFactors);

        //        inUserCode.OracleDbType = OracleDbType.Varchar2;
        //        inUserCode.Direction = ParameterDirection.Input;
        //        inUserCode.Value = userCode;
        //        orlCmd.Parameters.Add(inUserCode);

        //        curExchangeRateData.OracleDbType = OracleDbType.RefCursor;
        //        curExchangeRateData.Direction = System.Data.ParameterDirection.Output;
        //        orlCmd.Parameters.Add(curExchangeRateData);

        //        ErrorId.OracleDbType = OracleDbType.Int32;
        //        ErrorId.Direction = ParameterDirection.Output;
        //        ErrorId.ParameterName = "ErrorId";
        //        orlCmd.Parameters.Add(ErrorId);

        //        orlDA = new OracleDataAdapter(orlCmd);
        //        orlDA.Fill(ds);

        //        iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

        //    }
        //    catch (Exception ex)
        //    {
        //        iErrorId = 2;
        //        throw ex;
        //    }
        //    finally
        //    {
        //        CloseConnection();
        //    }
        //    return ds;

        //}

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
