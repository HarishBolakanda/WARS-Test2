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
    public class RoyContractBankDAL : IRoyContractBankDAL
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

        public DataSet GetInitialData(Int32 royaltorId, out string royaltor, out string primaryPayee, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter curIntPartyList = new OracleParameter();
                OracleParameter curPaymentMethodList = new OracleParameter();
                OracleParameter curCurrencyList = new OracleParameter();
                OracleParameter curPayeeBankData = new OracleParameter();
                OracleParameter outRoyaltor = new OracleParameter();
                OracleParameter outPrimaryPayee = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_bank.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inRoyaltorId.OracleDbType = OracleDbType.Int32;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                curIntPartyList.OracleDbType = OracleDbType.RefCursor;
                curIntPartyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curIntPartyList);

                curPaymentMethodList.OracleDbType = OracleDbType.RefCursor;
                curPaymentMethodList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPaymentMethodList);

                curCurrencyList.OracleDbType = OracleDbType.RefCursor;
                curCurrencyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCurrencyList);

                curPayeeBankData.OracleDbType = OracleDbType.RefCursor;
                curPayeeBankData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeBankData);

                outRoyaltor.OracleDbType = OracleDbType.Varchar2;
                outRoyaltor.Size = 250;
                outRoyaltor.Direction = ParameterDirection.Output;
                outRoyaltor.ParameterName = "Royaltor";
                orlCmd.Parameters.Add(outRoyaltor);

                outPrimaryPayee.OracleDbType = OracleDbType.Varchar2;
                outPrimaryPayee.Size = 250;
                outPrimaryPayee.Direction = ParameterDirection.Output;
                outPrimaryPayee.ParameterName = "PrimaryPayee";
                orlCmd.Parameters.Add(outPrimaryPayee);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["Royaltor"].Value.ToString();
                primaryPayee = orlCmd.Parameters["PrimaryPayee"].Value.ToString();
            }
            catch (Exception ex)
            {
                iErrorId = 2;
                royaltor = string.Empty;
                primaryPayee = string.Empty;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet GetPayeeBankDetails(Int32 interestedPartyId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter inInterestedPartyId = new OracleParameter();
                OracleParameter curPayeeBankData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_bank.p_get_payee_bank_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inInterestedPartyId.OracleDbType = OracleDbType.Int32;
                inInterestedPartyId.Direction = ParameterDirection.Input;
                inInterestedPartyId.Value = interestedPartyId;
                orlCmd.Parameters.Add(inInterestedPartyId);

                curPayeeBankData.OracleDbType = OracleDbType.RefCursor;
                curPayeeBankData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeBankData);

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

        public DataSet MergePayeeBankDomesticDetails(Int32 interestedPartyId, string vatNumber, string supplierNumber, string paymentMethodCode,
           string bankName, string bankAddress, string accountName, string sortCode, string accountNumber, string vendorSiteCode, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter inInterestedPartyId = new OracleParameter();
                OracleParameter inVatNumber = new OracleParameter();
                OracleParameter inSupplierNumber = new OracleParameter();
                OracleParameter inPaymentMethodCode = new OracleParameter();
                OracleParameter inBankName = new OracleParameter();
                OracleParameter inBankAddress = new OracleParameter();
                OracleParameter inAccountName = new OracleParameter();
                OracleParameter inSortCode = new OracleParameter();
                OracleParameter inAccountNumber = new OracleParameter();
                OracleParameter inVendorSiteCode = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curPayeeBankData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_bank.p_merge_domestic_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inInterestedPartyId.OracleDbType = OracleDbType.Int32;
                inInterestedPartyId.Direction = ParameterDirection.Input;
                inInterestedPartyId.Value = interestedPartyId;
                orlCmd.Parameters.Add(inInterestedPartyId);

                inVatNumber.OracleDbType = OracleDbType.Varchar2;
                inVatNumber.Direction = ParameterDirection.Input;
                inVatNumber.Value = vatNumber;
                orlCmd.Parameters.Add(inVatNumber);

                inSupplierNumber.OracleDbType = OracleDbType.Varchar2;
                inSupplierNumber.Direction = ParameterDirection.Input;
                inSupplierNumber.Value = supplierNumber;
                orlCmd.Parameters.Add(inSupplierNumber);

                inPaymentMethodCode.OracleDbType = OracleDbType.Varchar2;
                inPaymentMethodCode.Direction = ParameterDirection.Input;
                inPaymentMethodCode.Value = paymentMethodCode;
                orlCmd.Parameters.Add(inPaymentMethodCode);

                inBankName.OracleDbType = OracleDbType.Varchar2;
                inBankName.Direction = ParameterDirection.Input;
                inBankName.Value = bankName;
                orlCmd.Parameters.Add(inBankName);

                inBankAddress.OracleDbType = OracleDbType.Varchar2;
                inBankAddress.Direction = ParameterDirection.Input;
                inBankAddress.Value = bankAddress;
                orlCmd.Parameters.Add(inBankAddress);

                inAccountName.OracleDbType = OracleDbType.Varchar2;
                inAccountName.Direction = ParameterDirection.Input;
                inAccountName.Value = accountName;
                orlCmd.Parameters.Add(inAccountName);

                inSortCode.OracleDbType = OracleDbType.Varchar2;
                inSortCode.Direction = ParameterDirection.Input;
                inSortCode.Value = sortCode;
                orlCmd.Parameters.Add(inSortCode);

                inAccountNumber.OracleDbType = OracleDbType.Varchar2;
                inAccountNumber.Direction = ParameterDirection.Input;
                inAccountNumber.Value = accountNumber;
                orlCmd.Parameters.Add(inAccountNumber);

                inVendorSiteCode.OracleDbType = OracleDbType.Varchar2;
                inVendorSiteCode.Direction = ParameterDirection.Input;
                inVendorSiteCode.Value = vendorSiteCode;
                orlCmd.Parameters.Add(inVendorSiteCode);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curPayeeBankData.OracleDbType = OracleDbType.RefCursor;
                curPayeeBankData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeBankData);

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

        public DataSet MergePayeeBankForeignDetails(Int32 interestedPartyId, string vatNumber, string supplierNumber, string paymentMethodCode,
           string iban, string swiftCode, string abaRouting, string accountName, string accountNumber, string bankAddress, string currencyCode, string vendorSiteCode, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter inInterestedPartyId = new OracleParameter();
                OracleParameter inVatNumber = new OracleParameter();
                OracleParameter inSupplierNumber = new OracleParameter();
                OracleParameter inPaymentMethodCode = new OracleParameter();
                OracleParameter inIban = new OracleParameter();
                OracleParameter inSwiftCode = new OracleParameter();
                OracleParameter inAbaRouting = new OracleParameter();
                OracleParameter inAccountName = new OracleParameter();
                OracleParameter inAccountNumber = new OracleParameter();
                OracleParameter inBankAddress = new OracleParameter();
                OracleParameter inCurrencyCode = new OracleParameter();
                OracleParameter inVendorSiteCode = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curPayeeBankData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_bank.p_merge_foreign_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inInterestedPartyId.OracleDbType = OracleDbType.Int32;
                inInterestedPartyId.Direction = ParameterDirection.Input;
                inInterestedPartyId.Value = interestedPartyId;
                orlCmd.Parameters.Add(inInterestedPartyId);

                inVatNumber.OracleDbType = OracleDbType.Varchar2;
                inVatNumber.Direction = ParameterDirection.Input;
                inVatNumber.Value = vatNumber;
                orlCmd.Parameters.Add(inVatNumber);

                inSupplierNumber.OracleDbType = OracleDbType.Varchar2;
                inSupplierNumber.Direction = ParameterDirection.Input;
                inSupplierNumber.Value = supplierNumber;
                orlCmd.Parameters.Add(inSupplierNumber);

                inPaymentMethodCode.OracleDbType = OracleDbType.Varchar2;
                inPaymentMethodCode.Direction = ParameterDirection.Input;
                inPaymentMethodCode.Value = paymentMethodCode;
                orlCmd.Parameters.Add(inPaymentMethodCode);

                inIban.OracleDbType = OracleDbType.Varchar2;
                inIban.Direction = ParameterDirection.Input;
                inIban.Value = iban;
                orlCmd.Parameters.Add(inIban);

                inSwiftCode.OracleDbType = OracleDbType.Varchar2;
                inSwiftCode.Direction = ParameterDirection.Input;
                inSwiftCode.Value = swiftCode;
                orlCmd.Parameters.Add(inSwiftCode);

                inAbaRouting.OracleDbType = OracleDbType.Varchar2;
                inAbaRouting.Direction = ParameterDirection.Input;
                inAbaRouting.Value = abaRouting;
                orlCmd.Parameters.Add(inAbaRouting);

                inAccountName.OracleDbType = OracleDbType.Varchar2;
                inAccountName.Direction = ParameterDirection.Input;
                inAccountName.Value = accountName;
                orlCmd.Parameters.Add(inAccountName);

                inAccountNumber.OracleDbType = OracleDbType.Varchar2;
                inAccountNumber.Direction = ParameterDirection.Input;
                inAccountNumber.Value = accountNumber;
                orlCmd.Parameters.Add(inAccountNumber);

                inBankAddress.OracleDbType = OracleDbType.Varchar2;
                inBankAddress.Direction = ParameterDirection.Input;
                inBankAddress.Value = bankAddress;
                orlCmd.Parameters.Add(inBankAddress);

                inCurrencyCode.OracleDbType = OracleDbType.Varchar2;
                inCurrencyCode.Direction = ParameterDirection.Input;
                inCurrencyCode.Value = currencyCode;
                orlCmd.Parameters.Add(inCurrencyCode);

                inVendorSiteCode.OracleDbType = OracleDbType.Varchar2;
                inVendorSiteCode.Direction = ParameterDirection.Input;
                inVendorSiteCode.Value = vendorSiteCode;
                orlCmd.Parameters.Add(inVendorSiteCode);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curPayeeBankData.OracleDbType = OracleDbType.RefCursor;
                curPayeeBankData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeBankData);

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
