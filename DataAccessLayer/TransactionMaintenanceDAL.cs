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
    public class TransactionMaintenanceDAL : ITransactionMaintenanceDAL
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
                OracleParameter curCatnoList = new OracleParameter();
                OracleParameter curCurrencyList = new OracleParameter();
                OracleParameter curCompanyCodeList = new OracleParameter();
                OracleParameter curCountryCodeList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_transactions.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curCatnoList.OracleDbType = OracleDbType.RefCursor;
                curCatnoList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCatnoList);

                curCurrencyList.OracleDbType = OracleDbType.RefCursor;
                curCurrencyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCurrencyList);

                curCompanyCodeList.OracleDbType = OracleDbType.RefCursor;
                curCompanyCodeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyCodeList);

                curCountryCodeList.OracleDbType = OracleDbType.RefCursor;
                curCountryCodeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCountryCodeList);

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

        public DataSet GetSearchedTransactionsData(string seller, string salesType, string receivedDate, string reportedDate, string catno, string companyCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inSeller = new OracleParameter();
                OracleParameter inSalesType = new OracleParameter();
                OracleParameter inReceivedDate = new OracleParameter();
                OracleParameter inReportedDate = new OracleParameter();
                OracleParameter inCatno = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter curTransDetails = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_transactions.p_get_search_transactions", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inSeller.OracleDbType = OracleDbType.Varchar2;
                inSeller.Direction = ParameterDirection.Input;
                inSeller.Value = seller;
                orlCmd.Parameters.Add(inSeller);

                inSalesType.OracleDbType = OracleDbType.Varchar2;
                inSalesType.Direction = ParameterDirection.Input;
                inSalesType.Value = salesType;
                orlCmd.Parameters.Add(inSalesType);

                inReceivedDate.OracleDbType = OracleDbType.Varchar2;
                inReceivedDate.Direction = ParameterDirection.Input;
                inReceivedDate.Value = receivedDate;
                orlCmd.Parameters.Add(inReceivedDate);

                inReportedDate.OracleDbType = OracleDbType.Varchar2;
                inReportedDate.Direction = ParameterDirection.Input;
                inReportedDate.Value = reportedDate;
                orlCmd.Parameters.Add(inReportedDate);

                inCatno.OracleDbType = OracleDbType.Varchar2;
                inCatno.Direction = ParameterDirection.Input;
                inCatno.Value = catno;
                orlCmd.Parameters.Add(inCatno);

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                curTransDetails.OracleDbType = OracleDbType.RefCursor;
                curTransDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTransDetails);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "out_n_status_code";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["out_n_status_code"].Value.ToString());

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

        public DataSet UpdateTransactionDetails(string seller, string salesType, string receivedDate, string reportedDate, string catno, string companyCode, Array transToUpdate, Array transToDelete, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inSeller = new OracleParameter();
                OracleParameter inSalesType = new OracleParameter();
                OracleParameter inReceivedDate = new OracleParameter();
                OracleParameter inReportedDate = new OracleParameter();
                OracleParameter inCatno = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter pUpdateList = new OracleParameter();
                OracleParameter pDeleteList = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter curTransMaint = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_transactions.p_update_trans_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inSeller.OracleDbType = OracleDbType.Varchar2;
                inSeller.Direction = ParameterDirection.Input;
                inSeller.Value = seller;
                orlCmd.Parameters.Add(inSeller);

                inSalesType.OracleDbType = OracleDbType.Varchar2;
                inSalesType.Direction = ParameterDirection.Input;
                inSalesType.Value = salesType;
                orlCmd.Parameters.Add(inSalesType);

                inReceivedDate.OracleDbType = OracleDbType.Varchar2;
                inReceivedDate.Direction = ParameterDirection.Input;
                inReceivedDate.Value = receivedDate;
                orlCmd.Parameters.Add(inReceivedDate);

                inReportedDate.OracleDbType = OracleDbType.Varchar2;
                inReportedDate.Direction = ParameterDirection.Input;
                inReportedDate.Value = reportedDate;
                orlCmd.Parameters.Add(inReportedDate);

                inCatno.OracleDbType = OracleDbType.Varchar2;
                inCatno.Direction = ParameterDirection.Input;
                inCatno.Value = catno;
                orlCmd.Parameters.Add(inCatno);

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                pUpdateList.OracleDbType = OracleDbType.Varchar2;
                pUpdateList.Direction = ParameterDirection.Input;
                pUpdateList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (transToUpdate.Length == 0)
                {
                    pUpdateList.Size = 1;
                    pUpdateList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pUpdateList.Size = transToUpdate.Length;
                    pUpdateList.Value = transToUpdate;
                }
                orlCmd.Parameters.Add(pUpdateList);

                pDeleteList.OracleDbType = OracleDbType.Varchar2;
                pDeleteList.Direction = ParameterDirection.Input;
                pDeleteList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (transToDelete.Length == 0)
                {
                    pDeleteList.Size = 1;
                    pDeleteList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pDeleteList.Size = transToDelete.Length;
                    pDeleteList.Value = transToDelete;
                }
                orlCmd.Parameters.Add(pDeleteList);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                curTransMaint.OracleDbType = OracleDbType.RefCursor;
                curTransMaint.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTransMaint);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "out_n_status_code";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["out_n_status_code"].Value.ToString());

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

        public void AddTransactionDetails(string addTransReceivedDate, string addTransSalesType, string addTransSales1, string addTransReceipts, string addTransDolExchRate,
                                           string addTransReportedDate, string addTransPrice1, string addTransSales2, string addTransReceipts2, string addTransCurrencyCode,
                                           string addTransCatNo, string addTransPrice2, string addTransSales3, string addTransReceipts3, string addTransWhtTax,
                                           string addTransSeller, string addTransPrice3, string addTransDestinationCountry, string addCompanyCode, string addOwnerShare, string addTotalShare, string userCode, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter paddTransReceivedDate = new OracleParameter();
                OracleParameter paddTransSalesType = new OracleParameter();
                OracleParameter paddTransCatNo = new OracleParameter();
                OracleParameter paddTransDestCountry = new OracleParameter();
                OracleParameter paddTransCurrencyCode = new OracleParameter();
                OracleParameter paddTransDolExchRate = new OracleParameter();
                OracleParameter paddTransPrice1 = new OracleParameter();
                OracleParameter paddTransPrice2 = new OracleParameter();
                OracleParameter paddTransPrice3 = new OracleParameter();
                OracleParameter paddTransReceipts = new OracleParameter();
                OracleParameter paddTransReceipts2 = new OracleParameter();
                OracleParameter paddTransReceipts3 = new OracleParameter();
                OracleParameter paddTransReportedDate = new OracleParameter();
                OracleParameter paddTransSales1 = new OracleParameter();
                OracleParameter paddTransSales2 = new OracleParameter();
                OracleParameter paddTransSales3 = new OracleParameter();
                OracleParameter paddTransSeller = new OracleParameter();
                OracleParameter paddTransWhtTax = new OracleParameter();
                OracleParameter paddCompanyCode = new OracleParameter();
                OracleParameter paddOwnerShare = new OracleParameter();
                OracleParameter paddTotalShare = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();

                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_transactions.p_add_trans_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                paddTransReceivedDate.OracleDbType = OracleDbType.Varchar2;
                paddTransReceivedDate.Direction = ParameterDirection.Input;
                paddTransReceivedDate.Value = addTransReceivedDate;
                orlCmd.Parameters.Add(paddTransReceivedDate);

                paddTransSalesType.OracleDbType = OracleDbType.Varchar2;
                paddTransSalesType.Direction = ParameterDirection.Input;
                paddTransSalesType.Value = addTransSalesType;
                orlCmd.Parameters.Add(paddTransSalesType);

                paddTransSales1.OracleDbType = OracleDbType.Varchar2;
                paddTransSales1.Direction = ParameterDirection.Input;
                paddTransSales1.Value = addTransSales1;
                orlCmd.Parameters.Add(paddTransSales1);

                paddTransReceipts.OracleDbType = OracleDbType.Varchar2;
                paddTransReceipts.Direction = ParameterDirection.Input;
                paddTransReceipts.Value = addTransReceipts;
                orlCmd.Parameters.Add(paddTransReceipts);

                paddTransDolExchRate.OracleDbType = OracleDbType.Varchar2;
                paddTransDolExchRate.Direction = ParameterDirection.Input;
                paddTransDolExchRate.Value = addTransDolExchRate;
                orlCmd.Parameters.Add(paddTransDolExchRate);

                paddTransReportedDate.OracleDbType = OracleDbType.Varchar2;
                paddTransReportedDate.Direction = ParameterDirection.Input;
                paddTransReportedDate.Value = addTransReportedDate;
                orlCmd.Parameters.Add(paddTransReportedDate);

                paddTransPrice1.OracleDbType = OracleDbType.Varchar2;
                paddTransPrice1.Direction = ParameterDirection.Input;
                paddTransPrice1.Value = addTransPrice1;
                orlCmd.Parameters.Add(paddTransPrice1);

                paddTransSales2.OracleDbType = OracleDbType.Varchar2;
                paddTransSales2.Direction = ParameterDirection.Input;
                paddTransSales2.Value = addTransSales2;
                orlCmd.Parameters.Add(paddTransSales2);

                paddTransReceipts2.OracleDbType = OracleDbType.Varchar2;
                paddTransReceipts2.Direction = ParameterDirection.Input;
                paddTransReceipts2.Value = addTransReceipts2;
                orlCmd.Parameters.Add(paddTransReceipts2);

                paddTransCurrencyCode.OracleDbType = OracleDbType.Varchar2;
                paddTransCurrencyCode.Direction = ParameterDirection.Input;
                paddTransCurrencyCode.Value = addTransCurrencyCode;
                orlCmd.Parameters.Add(paddTransCurrencyCode);

                paddTransCatNo.OracleDbType = OracleDbType.Varchar2;
                paddTransCatNo.Direction = ParameterDirection.Input;
                paddTransCatNo.Value = addTransCatNo;
                orlCmd.Parameters.Add(paddTransCatNo);

                paddTransPrice2.OracleDbType = OracleDbType.Varchar2;
                paddTransPrice2.Direction = ParameterDirection.Input;
                paddTransPrice2.Value = addTransPrice2;
                orlCmd.Parameters.Add(paddTransPrice2);

                paddTransSales3.OracleDbType = OracleDbType.Varchar2;
                paddTransSales3.Direction = ParameterDirection.Input;
                paddTransSales3.Value = addTransSales3;
                orlCmd.Parameters.Add(paddTransSales3);

                paddTransReceipts3.OracleDbType = OracleDbType.Varchar2;
                paddTransReceipts3.Direction = ParameterDirection.Input;
                paddTransReceipts3.Value = addTransReceipts3;
                orlCmd.Parameters.Add(paddTransReceipts3);

                paddTransWhtTax.OracleDbType = OracleDbType.Varchar2;
                paddTransWhtTax.Direction = ParameterDirection.Input;
                paddTransWhtTax.Value = addTransWhtTax;
                orlCmd.Parameters.Add(paddTransWhtTax);

                paddTransSeller.OracleDbType = OracleDbType.Varchar2;
                paddTransSeller.Direction = ParameterDirection.Input;
                paddTransSeller.Value = addTransSeller;
                orlCmd.Parameters.Add(paddTransSeller);

                paddTransPrice3.OracleDbType = OracleDbType.Varchar2;
                paddTransPrice3.Direction = ParameterDirection.Input;
                paddTransPrice3.Value = addTransPrice3;
                orlCmd.Parameters.Add(paddTransPrice3);

                paddTransDestCountry.OracleDbType = OracleDbType.Varchar2;
                paddTransDestCountry.Direction = ParameterDirection.Input;
                paddTransDestCountry.Value = addTransDestinationCountry;
                orlCmd.Parameters.Add(paddTransDestCountry);

                paddCompanyCode.OracleDbType = OracleDbType.Varchar2;
                paddCompanyCode.Direction = ParameterDirection.Input;
                paddCompanyCode.Value = addCompanyCode;
                orlCmd.Parameters.Add(paddCompanyCode);

                paddOwnerShare.OracleDbType = OracleDbType.Varchar2;
                paddOwnerShare.Direction = ParameterDirection.Input;
                paddOwnerShare.Value = addOwnerShare;
                orlCmd.Parameters.Add(paddOwnerShare);

                paddTotalShare.OracleDbType = OracleDbType.Varchar2;
                paddTotalShare.Direction = ParameterDirection.Input;
                paddTotalShare.Value = addTotalShare;
                orlCmd.Parameters.Add(paddTotalShare);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();   
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
