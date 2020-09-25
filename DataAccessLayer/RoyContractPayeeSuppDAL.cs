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
    public class RoyContractPayeeSuppDAL : IRoyContractPayeeSuppDAL
    {

        #region Global Declarations
        ConnectionDAL connDAL;
        OracleConnection orlConn;
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        OracleParameter ErrorId;
        string sErrorMsg;
        #endregion Global Declarations

        public DataSet GetDropdownData(string royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pPayeeList = new OracleParameter();
                OracleParameter pSupplierList = new OracleParameter();
                OracleParameter pCurrencyList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_payee_supp.p_get_dropdown_list_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);
                
                pPayeeList.OracleDbType = OracleDbType.RefCursor;
                pPayeeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeList);

                pSupplierList.OracleDbType = OracleDbType.RefCursor;
                pSupplierList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSupplierList);

                pCurrencyList.OracleDbType = OracleDbType.RefCursor;
                pCurrencyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCurrencyList);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "PayeeList";
                    ds.Tables[1].TableName = "SupplierList";
                    ds.Tables[2].TableName = "CurrencyList";
                }

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

        public DataSet GetInitialData(string royaltorId, out string royaltor, out string supplierSiteNameRegValue, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            supplierSiteNameRegValue = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pSupplierSiteNameRegValue = new OracleParameter();
                OracleParameter pPayeeList = new OracleParameter();
                OracleParameter pSupplierList = new OracleParameter();
                OracleParameter pPayeeSupplier = new OracleParameter();
                OracleParameter pCurrencyList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_payee_supp.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                pSupplierSiteNameRegValue.OracleDbType = OracleDbType.Varchar2;
                pSupplierSiteNameRegValue.Size = 250;
                pSupplierSiteNameRegValue.Direction = ParameterDirection.Output;
                pSupplierSiteNameRegValue.ParameterName = "out_v_site_name_reg_value";
                orlCmd.Parameters.Add(pSupplierSiteNameRegValue);

                pPayeeList.OracleDbType = OracleDbType.RefCursor;
                pPayeeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeList);

                pSupplierList.OracleDbType = OracleDbType.RefCursor;
                pSupplierList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSupplierList);

                pPayeeSupplier.OracleDbType = OracleDbType.RefCursor;
                pPayeeSupplier.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeSupplier);

                pCurrencyList.OracleDbType = OracleDbType.RefCursor;
                pCurrencyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCurrencyList);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();
                supplierSiteNameRegValue = orlCmd.Parameters["out_v_site_name_reg_value"].Value.ToString();
                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "PayeeList";
                    ds.Tables[1].TableName = "SupplierList";
                    ds.Tables[2].TableName = "PayeeSupplier";
                    ds.Tables[3].TableName = "CurrencyList";
                }

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

        public DataSet GetPayeeSupplier(string royaltorId, string intPartyId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pIntPartyId = new OracleParameter();
                OracleParameter pPayeeSupplier = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_payee_supp.p_get_payee_supplier", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pIntPartyId.OracleDbType = OracleDbType.Varchar2;
                pIntPartyId.Direction = ParameterDirection.Input;
                pIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(pIntPartyId);

                pPayeeSupplier.OracleDbType = OracleDbType.RefCursor;
                pPayeeSupplier.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeSupplier);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "PayeeSupplier";
                }

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

        public DataSet SavePayeeSupplier(string royaltorId, string intPartyId, string payeeCurrency, string supplierNumberOld, string supplierNumber, string supplierSiteNameOld, string supplierSiteName, string accountCompany, string mismatchFlag, string saveType, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pIntPartyId = new OracleParameter();
                OracleParameter pPayeeCurrency = new OracleParameter();
                OracleParameter pSupplierNumberOld = new OracleParameter();
                OracleParameter pSupplierNumber = new OracleParameter();
                OracleParameter pSupplierSiteNameOld = new OracleParameter();
                OracleParameter pSupplierSiteName = new OracleParameter();
                OracleParameter pAccountCompany = new OracleParameter();
                OracleParameter pMismatchFlag = new OracleParameter();
                OracleParameter pSaveType = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pPayeeSupplier = new OracleParameter();
                OracleParameter pSupplierList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_payee_supp.p_save_payee_supplier", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pIntPartyId.OracleDbType = OracleDbType.Varchar2;
                pIntPartyId.Direction = ParameterDirection.Input;
                pIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(pIntPartyId);

                pPayeeCurrency.OracleDbType = OracleDbType.Varchar2;
                pPayeeCurrency.Direction = ParameterDirection.Input;
                pPayeeCurrency.Value = payeeCurrency;
                orlCmd.Parameters.Add(pPayeeCurrency);

                pSupplierNumberOld.OracleDbType = OracleDbType.Varchar2;
                pSupplierNumberOld.Direction = ParameterDirection.Input;
                pSupplierNumberOld.Value = supplierNumberOld;
                orlCmd.Parameters.Add(pSupplierNumberOld);

                pSupplierNumber.OracleDbType = OracleDbType.Varchar2;
                pSupplierNumber.Direction = ParameterDirection.Input;
                pSupplierNumber.Value = supplierNumber;
                orlCmd.Parameters.Add(pSupplierNumber);

                pSupplierSiteNameOld.OracleDbType = OracleDbType.Varchar2;
                pSupplierSiteNameOld.Direction = ParameterDirection.Input;
                pSupplierSiteNameOld.Value = supplierSiteNameOld;
                orlCmd.Parameters.Add(pSupplierSiteNameOld);

                pSupplierSiteName.OracleDbType = OracleDbType.Varchar2;
                pSupplierSiteName.Direction = ParameterDirection.Input;
                pSupplierSiteName.Value = supplierSiteName;
                orlCmd.Parameters.Add(pSupplierSiteName);

                pAccountCompany.OracleDbType = OracleDbType.Varchar2;
                pAccountCompany.Direction = ParameterDirection.Input;
                pAccountCompany.Value = accountCompany;
                orlCmd.Parameters.Add(pAccountCompany);

                pMismatchFlag.OracleDbType = OracleDbType.Varchar2;
                pMismatchFlag.Direction = ParameterDirection.Input;
                pMismatchFlag.Value = mismatchFlag;
                orlCmd.Parameters.Add(pMismatchFlag);

                pSaveType.OracleDbType = OracleDbType.Varchar2;
                pSaveType.Direction = ParameterDirection.Input;
                pSaveType.Value = saveType;
                orlCmd.Parameters.Add(pSaveType);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pPayeeSupplier.OracleDbType = OracleDbType.RefCursor;
                pPayeeSupplier.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeSupplier);

                pSupplierList.OracleDbType = OracleDbType.RefCursor;
                pSupplierList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSupplierList);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "PayeeSupplier";
                    ds.Tables[1].TableName = "SupplierList";
                }

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
