using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.IDAL;

namespace WARS.DataAccessLayer
{
    public class TaxRateMaintananceDAL : ITaxRateMaintananceDAL
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

        public DataSet GetInitialData( out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter curCompanyList = new OracleParameter();
                OracleParameter curTaxTypeList = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_tax_rates.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curCompanyList.OracleDbType = OracleDbType.RefCursor;
                curCompanyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCompanyList);

                curTaxTypeList.OracleDbType = OracleDbType.RefCursor;
                curTaxTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTaxTypeList);

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


        public DataSet GetTaxRateData(string companyCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter curTaxRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_tax_rates.p_get_tax_rate_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                curTaxRateData.OracleDbType = OracleDbType.RefCursor;
                curTaxRateData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTaxRateData);

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

        public DataSet SaveTaxRate(string companyCode, string startDate, string endDate, string taxType ,double taxRate, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter inStartDate = new OracleParameter();
                OracleParameter inEndDate = new OracleParameter();
                OracleParameter intaxType = new OracleParameter();
                OracleParameter inTaxRate = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curTaxRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_tax_rates.p_insert_tax_rate", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                inStartDate.OracleDbType = OracleDbType.Varchar2;
                inStartDate.Direction = ParameterDirection.Input;
                inStartDate.Value = startDate;
                orlCmd.Parameters.Add(inStartDate);

                inEndDate.OracleDbType = OracleDbType.Varchar2;
                inEndDate.Direction = ParameterDirection.Input;
                inEndDate.Value = endDate;
                orlCmd.Parameters.Add(inEndDate);

                intaxType.OracleDbType = OracleDbType.Varchar2;
                intaxType.Direction = ParameterDirection.Input;
                intaxType.Value = taxType;
                orlCmd.Parameters.Add(intaxType);

                inTaxRate.OracleDbType = OracleDbType.Double;
                inTaxRate.Direction = ParameterDirection.Input;
                inTaxRate.Value = taxRate;
                orlCmd.Parameters.Add(inTaxRate);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curTaxRateData.OracleDbType = OracleDbType.RefCursor;
                curTaxRateData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTaxRateData);

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

        public DataSet UpdateTaxRate(string companyCodeSearch,string companyCode, string taxNo, string endDate, double taxRate, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCompanyCodeSearch = new OracleParameter();
                OracleParameter inCompanyCode = new OracleParameter();
                OracleParameter inTaxNo = new OracleParameter();
                OracleParameter inEndDate= new OracleParameter();
                OracleParameter inTaxRate = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curTaxRateData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_tax_rates.p_update_tax_rate", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCompanyCodeSearch.OracleDbType = OracleDbType.Varchar2;
                inCompanyCodeSearch.Direction = ParameterDirection.Input;
                inCompanyCodeSearch.Value = companyCodeSearch;
                orlCmd.Parameters.Add(inCompanyCodeSearch);

                inCompanyCode.OracleDbType = OracleDbType.Varchar2;
                inCompanyCode.Direction = ParameterDirection.Input;
                inCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(inCompanyCode);

                inTaxNo.OracleDbType = OracleDbType.Varchar2;
                inTaxNo.Direction = ParameterDirection.Input;
                inTaxNo.Value = taxNo;
                orlCmd.Parameters.Add(inTaxNo);

                inEndDate.OracleDbType = OracleDbType.Varchar2;
                inEndDate.Direction = ParameterDirection.Input;
                inEndDate.Value = endDate;
                orlCmd.Parameters.Add(inEndDate);

                inTaxRate.OracleDbType = OracleDbType.Double;
                inTaxRate.Direction = ParameterDirection.Input;
                inTaxRate.Value = taxRate;
                orlCmd.Parameters.Add(inTaxRate);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curTaxRateData.OracleDbType = OracleDbType.RefCursor;
                curTaxRateData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTaxRateData);

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
