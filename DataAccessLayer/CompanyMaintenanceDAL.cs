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
    public class CompanyMaintenanceDAL : ICompanyMaintenanceDAL
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
                OracleParameter curCurrencyList = new OracleParameter();
                OracleParameter curCoountCompanyList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_company.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                              
                curCurrencyList.OracleDbType = OracleDbType.RefCursor;
                curCurrencyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCurrencyList);

                curCoountCompanyList.OracleDbType = OracleDbType.RefCursor;
                curCoountCompanyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCoountCompanyList);

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

        public DataSet GetSearchedCompanyData(Int32 companyCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter curCompanyData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_company.p_get_company_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Int32;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                curCompanyData.OracleDbType = OracleDbType.RefCursor;
                curCompanyData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyData);

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

        public DataSet UpdateCompanyData(Int32 companyCode, string companyName, string companyDesc, string companyAddress1, string companyAddress2, string companyAddress3,
                string companyAddress4, string currencyCode, string domesticCurrencyGroup, string paymentThreshold, string thresholdRecouped, string thresholdUnrecouped,
                string isPrimary, string isDisplayVat, string accountCompany, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter inCompanyName = new OracleParameter();
                OracleParameter inCompanyDesc = new OracleParameter();
                OracleParameter inCompanyAdd1 = new OracleParameter();
                OracleParameter inCompanyAdd2 = new OracleParameter();
                OracleParameter inCompanyAdd3 = new OracleParameter();
                OracleParameter inCompanyAdd4 = new OracleParameter();
                OracleParameter inCurrencyCode = new OracleParameter();
                OracleParameter inDomesticCurrencyGroup = new OracleParameter();
                OracleParameter inPaymentThreshold = new OracleParameter();
                OracleParameter inThresholdRecouped = new OracleParameter();                
                OracleParameter inThresholdUnRecouped = new OracleParameter();
                OracleParameter inIsPrimary = new OracleParameter();
                OracleParameter inIsDisplayVat = new OracleParameter();
                OracleParameter inAccountCompany = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curCompanyData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_company.p_update_company_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Int32;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                inCompanyName.OracleDbType = OracleDbType.Varchar2;
                inCompanyName.Direction = ParameterDirection.Input;
                inCompanyName.Value = companyName;
                orlCmd.Parameters.Add(inCompanyName);

                inCompanyDesc.OracleDbType = OracleDbType.Varchar2;
                inCompanyDesc.Direction = ParameterDirection.Input;
                inCompanyDesc.Value = companyDesc;
                orlCmd.Parameters.Add(inCompanyDesc);

                inCompanyAdd1.OracleDbType = OracleDbType.Varchar2;
                inCompanyAdd1.Direction = ParameterDirection.Input;
                inCompanyAdd1.Value = companyAddress1;
                orlCmd.Parameters.Add(inCompanyAdd1);

                inCompanyAdd2.OracleDbType = OracleDbType.Varchar2;
                inCompanyAdd2.Direction = ParameterDirection.Input;
                inCompanyAdd2.Value = companyAddress2;
                orlCmd.Parameters.Add(inCompanyAdd2);

                inCompanyAdd3.OracleDbType = OracleDbType.Varchar2;
                inCompanyAdd3.Direction = ParameterDirection.Input;
                inCompanyAdd3.Value = companyAddress3;
                orlCmd.Parameters.Add(inCompanyAdd3);

                inCompanyAdd4.OracleDbType = OracleDbType.Varchar2;
                inCompanyAdd4.Direction = ParameterDirection.Input;
                inCompanyAdd4.Value = companyAddress4;
                orlCmd.Parameters.Add(inCompanyAdd4);

                inCurrencyCode.OracleDbType = OracleDbType.Varchar2;
                inCurrencyCode.Direction = ParameterDirection.Input;
                inCurrencyCode.Value = currencyCode;
                orlCmd.Parameters.Add(inCurrencyCode);

                inDomesticCurrencyGroup.OracleDbType = OracleDbType.Varchar2;
                inDomesticCurrencyGroup.Direction = ParameterDirection.Input;
                inDomesticCurrencyGroup.Value = domesticCurrencyGroup;
                orlCmd.Parameters.Add(inDomesticCurrencyGroup);

                inPaymentThreshold.OracleDbType = OracleDbType.Varchar2;
                inPaymentThreshold.Direction = ParameterDirection.Input;
                inPaymentThreshold.Value = paymentThreshold;
                orlCmd.Parameters.Add(inPaymentThreshold);

                inThresholdRecouped.OracleDbType = OracleDbType.Varchar2;
                inThresholdRecouped.Direction = ParameterDirection.Input;
                inThresholdRecouped.Value = thresholdRecouped;
                orlCmd.Parameters.Add(inThresholdRecouped);

                inThresholdUnRecouped.OracleDbType = OracleDbType.Varchar2;
                inThresholdUnRecouped.Direction = ParameterDirection.Input;
                inThresholdUnRecouped.Value = thresholdUnrecouped;
                orlCmd.Parameters.Add(inThresholdUnRecouped);

                inIsPrimary.OracleDbType = OracleDbType.Varchar2;
                inIsPrimary.Direction = ParameterDirection.Input;
                inIsPrimary.Value = isPrimary;
                orlCmd.Parameters.Add(inIsPrimary);

                inIsDisplayVat.OracleDbType = OracleDbType.Varchar2;
                inIsDisplayVat.Direction = ParameterDirection.Input;
                inIsDisplayVat.Value = isDisplayVat;
                orlCmd.Parameters.Add(inIsDisplayVat);

                inAccountCompany.OracleDbType = OracleDbType.Varchar2;
                inAccountCompany.Direction = ParameterDirection.Input;
                inAccountCompany.Value = accountCompany;
                orlCmd.Parameters.Add(inAccountCompany);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curCompanyData.OracleDbType = OracleDbType.RefCursor;
                curCompanyData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyData);

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

        public DataSet InsertCompanyData(string companyName, string companyDesc, string companyAddress1, string companyAddress2, string companyAddress3,
               string companyAddress4, string currencyCode, string domesticCurrencyGroup, string paymentThreshold, string thresholdRecouped, string thresholdUnrecouped,
               string isPrimary, string isDisplayVat, string accountCompany, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyName = new OracleParameter();
                OracleParameter inCompanyDesc = new OracleParameter();
                OracleParameter inCompanyAdd1 = new OracleParameter();
                OracleParameter inCompanyAdd2 = new OracleParameter();
                OracleParameter inCompanyAdd3 = new OracleParameter();
                OracleParameter inCompanyAdd4 = new OracleParameter();
                OracleParameter inCurrencyCode = new OracleParameter();
                OracleParameter inDomesticCurrencyGroup = new OracleParameter();
                OracleParameter inPaymentThreshold = new OracleParameter();
                OracleParameter inThresholdRecouped = new OracleParameter();
                OracleParameter inThresholdUnRecouped = new OracleParameter();
                OracleParameter inIsPrimary = new OracleParameter();
                OracleParameter inIsDisplayVat = new OracleParameter();
                OracleParameter inAccountCompany = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curCompanyData = new OracleParameter();
                OracleParameter curCompanyList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_company.p_insert_company_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyName.OracleDbType = OracleDbType.Varchar2;
                inCompanyName.Direction = ParameterDirection.Input;
                inCompanyName.Value = companyName;
                orlCmd.Parameters.Add(inCompanyName);

                inCompanyDesc.OracleDbType = OracleDbType.Varchar2;
                inCompanyDesc.Direction = ParameterDirection.Input;
                inCompanyDesc.Value = companyDesc;
                orlCmd.Parameters.Add(inCompanyDesc);

                inCompanyAdd1.OracleDbType = OracleDbType.Varchar2;
                inCompanyAdd1.Direction = ParameterDirection.Input;
                inCompanyAdd1.Value = companyAddress1;
                orlCmd.Parameters.Add(inCompanyAdd1);

                inCompanyAdd2.OracleDbType = OracleDbType.Varchar2;
                inCompanyAdd2.Direction = ParameterDirection.Input;
                inCompanyAdd2.Value = companyAddress2;
                orlCmd.Parameters.Add(inCompanyAdd2);

                inCompanyAdd3.OracleDbType = OracleDbType.Varchar2;
                inCompanyAdd3.Direction = ParameterDirection.Input;
                inCompanyAdd3.Value = companyAddress3;
                orlCmd.Parameters.Add(inCompanyAdd3);

                inCompanyAdd4.OracleDbType = OracleDbType.Varchar2;
                inCompanyAdd4.Direction = ParameterDirection.Input;
                inCompanyAdd4.Value = companyAddress4;
                orlCmd.Parameters.Add(inCompanyAdd4);

                inCurrencyCode.OracleDbType = OracleDbType.Varchar2;
                inCurrencyCode.Direction = ParameterDirection.Input;
                inCurrencyCode.Value = currencyCode;
                orlCmd.Parameters.Add(inCurrencyCode);

                inDomesticCurrencyGroup.OracleDbType = OracleDbType.Varchar2;
                inDomesticCurrencyGroup.Direction = ParameterDirection.Input;
                inDomesticCurrencyGroup.Value = domesticCurrencyGroup;
                orlCmd.Parameters.Add(inDomesticCurrencyGroup);

                inPaymentThreshold.OracleDbType = OracleDbType.Varchar2;
                inPaymentThreshold.Direction = ParameterDirection.Input;
                inPaymentThreshold.Value = paymentThreshold;
                orlCmd.Parameters.Add(inPaymentThreshold);

                inThresholdRecouped.OracleDbType = OracleDbType.Varchar2;
                inThresholdRecouped.Direction = ParameterDirection.Input;
                inThresholdRecouped.Value = thresholdRecouped;
                orlCmd.Parameters.Add(inThresholdRecouped);

                inThresholdUnRecouped.OracleDbType = OracleDbType.Varchar2;
                inThresholdUnRecouped.Direction = ParameterDirection.Input;
                inThresholdUnRecouped.Value = thresholdUnrecouped;
                orlCmd.Parameters.Add(inThresholdUnRecouped);

                inIsPrimary.OracleDbType = OracleDbType.Varchar2;
                inIsPrimary.Direction = ParameterDirection.Input;
                inIsPrimary.Value = isPrimary;
                orlCmd.Parameters.Add(inIsPrimary);

                inIsDisplayVat.OracleDbType = OracleDbType.Varchar2;
                inIsDisplayVat.Direction = ParameterDirection.Input;
                inIsDisplayVat.Value = isDisplayVat;
                orlCmd.Parameters.Add(inIsDisplayVat);

                inAccountCompany.OracleDbType = OracleDbType.Varchar2;
                inAccountCompany.Direction = ParameterDirection.Input;
                inAccountCompany.Value = accountCompany;
                orlCmd.Parameters.Add(inAccountCompany);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curCompanyData.OracleDbType = OracleDbType.RefCursor;
                curCompanyData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyData);

                curCompanyList.OracleDbType = OracleDbType.RefCursor;
                curCompanyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyList);

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
