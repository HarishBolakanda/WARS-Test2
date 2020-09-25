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
    public class InterestedPartyMaintenanceDAL : IInterestedPartyMaintenanceDAL
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
                OracleParameter curInterestedPartyData = new OracleParameter();
                OracleParameter curInterestedPartyType = new OracleParameter();
                OracleParameter curTaxType = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_interested_party.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curInterestedPartyData.OracleDbType = OracleDbType.RefCursor;
                curInterestedPartyData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curInterestedPartyData);

                curInterestedPartyType.OracleDbType = OracleDbType.RefCursor;
                curInterestedPartyType.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curInterestedPartyType);

                curTaxType.OracleDbType = OracleDbType.RefCursor;
                curTaxType.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTaxType);

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

        public DataSet UpdateInterestedPartyData(Int32 intPartyId, string intPartyName, string intPartyAdd1,
            string intPartyAdd2, string intPartyAdd3, string intPartyAdd4, string intPartyPostcode, string email, string vatNumber, string taxType, string isGenerateInvoice, string isSendStatement, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                //OracleParameter inIntPartyType = new OracleParameter();
                OracleParameter inIntPartyName = new OracleParameter();
                OracleParameter inIntPartyAdd1 = new OracleParameter();
                OracleParameter inIntPartyAdd2 = new OracleParameter();
                OracleParameter inIntPartyAdd3 = new OracleParameter();
                OracleParameter inIntPartyAdd4 = new OracleParameter();
                OracleParameter inIntPartyPostcode = new OracleParameter();
                OracleParameter inIntPartyEmail = new OracleParameter();
                OracleParameter inVatNumber = new OracleParameter();
                OracleParameter inTaxType = new OracleParameter();
                OracleParameter inIsSendStmt = new OracleParameter();
                OracleParameter inIsGenerateInvoice = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curlabelData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_interested_party.p_update_interested_party_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyId.OracleDbType = OracleDbType.Int32;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                //inIntPartyType.OracleDbType = OracleDbType.Varchar2;
                //inIntPartyType.Direction = ParameterDirection.Input;
                //inIntPartyType.Value = intPartyType;
                //orlCmd.Parameters.Add(inIntPartyType);

                inIntPartyName.OracleDbType = OracleDbType.Varchar2;
                inIntPartyName.Direction = ParameterDirection.Input;
                inIntPartyName.Value = intPartyName;
                orlCmd.Parameters.Add(inIntPartyName);

                inIntPartyAdd1.OracleDbType = OracleDbType.Varchar2;
                inIntPartyAdd1.Direction = ParameterDirection.Input;
                inIntPartyAdd1.Value = intPartyAdd1;
                orlCmd.Parameters.Add(inIntPartyAdd1);

                inIntPartyAdd2.OracleDbType = OracleDbType.Varchar2;
                inIntPartyAdd2.Direction = ParameterDirection.Input;
                inIntPartyAdd2.Value = intPartyAdd2;
                orlCmd.Parameters.Add(inIntPartyAdd2);

                inIntPartyAdd3.OracleDbType = OracleDbType.Varchar2;
                inIntPartyAdd3.Direction = ParameterDirection.Input;
                inIntPartyAdd3.Value = intPartyAdd3;
                orlCmd.Parameters.Add(inIntPartyAdd3);

                inIntPartyAdd4.OracleDbType = OracleDbType.Varchar2;
                inIntPartyAdd4.Direction = ParameterDirection.Input;
                inIntPartyAdd4.Value = intPartyAdd4;
                orlCmd.Parameters.Add(inIntPartyAdd4);

                inIntPartyPostcode.OracleDbType = OracleDbType.Varchar2;
                inIntPartyPostcode.Direction = ParameterDirection.Input;
                inIntPartyPostcode.Value = intPartyPostcode;
                orlCmd.Parameters.Add(inIntPartyPostcode);

                inIntPartyEmail.OracleDbType = OracleDbType.Varchar2;
                inIntPartyEmail.Direction = ParameterDirection.Input;
                inIntPartyEmail.Value = email;
                orlCmd.Parameters.Add(inIntPartyEmail);

                inVatNumber.OracleDbType = OracleDbType.Varchar2;
                inVatNumber.Direction = ParameterDirection.Input;
                inVatNumber.Value = vatNumber;
                orlCmd.Parameters.Add(inVatNumber);

                inTaxType.OracleDbType = OracleDbType.Varchar2;
                inTaxType.Direction = ParameterDirection.Input;
                inTaxType.Value = taxType;
                orlCmd.Parameters.Add(inTaxType);

                inIsGenerateInvoice.OracleDbType = OracleDbType.Varchar2;
                inIsGenerateInvoice.Direction = ParameterDirection.Input;
                inIsGenerateInvoice.Value = isGenerateInvoice;
                orlCmd.Parameters.Add(inIsGenerateInvoice);

                inIsSendStmt.OracleDbType = OracleDbType.Varchar2;
                inIsSendStmt.Direction = ParameterDirection.Input;
                inIsSendStmt.Value = isSendStatement;
                orlCmd.Parameters.Add(inIsSendStmt);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curlabelData.OracleDbType = OracleDbType.RefCursor;
                curlabelData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curlabelData);

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

        public DataSet InsertInterestedPartyData(string intPartyType, string intPartyName, string intPartyAdd1,
            string intPartyAdd2, string intPartyAdd3, string intPartyAdd4, string intPartyPostcode, string email, string vatNumber, string taxType, string isGenerateInvoice, string isSendStatement, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyType = new OracleParameter();
                OracleParameter inIntPartyName = new OracleParameter();
                OracleParameter inIntPartyAdd1 = new OracleParameter();
                OracleParameter inIntPartyAdd2 = new OracleParameter();
                OracleParameter inIntPartyAdd3 = new OracleParameter();
                OracleParameter inIntPartyAdd4 = new OracleParameter();
                OracleParameter inIntPartyPostcode = new OracleParameter();
                OracleParameter inIntPartyEmail = new OracleParameter();
                OracleParameter inVatNumber = new OracleParameter();
                OracleParameter inTaxType = new OracleParameter();
                OracleParameter inIsGenerateInvoice = new OracleParameter();
                OracleParameter inIsSendStmt = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curlabelData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_interested_party.p_insert_interested_party_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyType.OracleDbType = OracleDbType.Varchar2;
                inIntPartyType.Direction = ParameterDirection.Input;
                inIntPartyType.Value = intPartyType;
                orlCmd.Parameters.Add(inIntPartyType);

                inIntPartyName.OracleDbType = OracleDbType.Varchar2;
                inIntPartyName.Direction = ParameterDirection.Input;
                inIntPartyName.Value = intPartyName;
                orlCmd.Parameters.Add(inIntPartyName);

                inIntPartyAdd1.OracleDbType = OracleDbType.Varchar2;
                inIntPartyAdd1.Direction = ParameterDirection.Input;
                inIntPartyAdd1.Value = intPartyAdd1;
                orlCmd.Parameters.Add(inIntPartyAdd1);

                inIntPartyAdd2.OracleDbType = OracleDbType.Varchar2;
                inIntPartyAdd2.Direction = ParameterDirection.Input;
                inIntPartyAdd2.Value = intPartyAdd2;
                orlCmd.Parameters.Add(inIntPartyAdd2);

                inIntPartyAdd3.OracleDbType = OracleDbType.Varchar2;
                inIntPartyAdd3.Direction = ParameterDirection.Input;
                inIntPartyAdd3.Value = intPartyAdd3;
                orlCmd.Parameters.Add(inIntPartyAdd3);

                inIntPartyAdd4.OracleDbType = OracleDbType.Varchar2;
                inIntPartyAdd4.Direction = ParameterDirection.Input;
                inIntPartyAdd4.Value = intPartyAdd4;
                orlCmd.Parameters.Add(inIntPartyAdd4);

                inIntPartyPostcode.OracleDbType = OracleDbType.Varchar2;
                inIntPartyPostcode.Direction = ParameterDirection.Input;
                inIntPartyPostcode.Value = intPartyPostcode;
                orlCmd.Parameters.Add(inIntPartyPostcode);

                inIntPartyEmail.OracleDbType = OracleDbType.Varchar2;
                inIntPartyEmail.Direction = ParameterDirection.Input;
                inIntPartyEmail.Value = email;
                orlCmd.Parameters.Add(inIntPartyEmail);

                inVatNumber.OracleDbType = OracleDbType.Varchar2;
                inVatNumber.Direction = ParameterDirection.Input;
                inVatNumber.Value = vatNumber;
                orlCmd.Parameters.Add(inVatNumber);

                inTaxType.OracleDbType = OracleDbType.Varchar2;
                inTaxType.Direction = ParameterDirection.Input;
                inTaxType.Value = taxType;
                orlCmd.Parameters.Add(inTaxType);

                inIsGenerateInvoice.OracleDbType = OracleDbType.Varchar2;
                inIsGenerateInvoice.Direction = ParameterDirection.Input;
                inIsGenerateInvoice.Value = isGenerateInvoice;
                orlCmd.Parameters.Add(inIsGenerateInvoice);

                inIsSendStmt.OracleDbType = OracleDbType.Varchar2;
                inIsSendStmt.Direction = ParameterDirection.Input;
                inIsSendStmt.Value = isSendStatement;
                orlCmd.Parameters.Add(inIsSendStmt);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curlabelData.OracleDbType = OracleDbType.RefCursor;
                curlabelData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curlabelData);

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

        public DataSet DeleteInterestedPartyData(Int32 intPartyId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                OracleParameter curlabelData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_interested_party.p_delete_interested_party_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyId.OracleDbType = OracleDbType.Int32;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                curlabelData.OracleDbType = OracleDbType.RefCursor;
                curlabelData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curlabelData);

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

        public DataSet GetLinkedRoyaltorDetails(Int32 intPartyId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                OracleParameter curlabelData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_interested_party.p_get_linked_royaltor_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyId.OracleDbType = OracleDbType.Int32;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                curlabelData.OracleDbType = OracleDbType.RefCursor;
                curlabelData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curlabelData);

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

        //public DataSet GetPayeeBankDetails(Int32 intPartyId, out Int32 iErrorId)
        //{
        //    ds = new DataSet();
        //    try
        //    {
        //        OpenConnection(out iErrorId, out sErrorMsg);

        //        ErrorId = new OracleParameter();
        //        OracleParameter inIntPartyId = new OracleParameter();
        //        OracleParameter curlabelData = new OracleParameter();
        //        orlDA = new OracleDataAdapter();

        //        orlCmd = new OracleCommand("pkg_maint_interested_party.p_get_payee_bank_details", orlConn);
        //        orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //        inIntPartyId.OracleDbType = OracleDbType.Int32;
        //        inIntPartyId.Direction = ParameterDirection.Input;
        //        inIntPartyId.Value = intPartyId;
        //        orlCmd.Parameters.Add(inIntPartyId);

        //        curlabelData.OracleDbType = OracleDbType.RefCursor;
        //        curlabelData.Direction = System.Data.ParameterDirection.Output;
        //        orlCmd.Parameters.Add(curlabelData);

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

        public DataSet GetSupplierDetails(Int32 intPartyId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                OracleParameter curSupplierData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_interested_party.p_get_supplier_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyId.OracleDbType = OracleDbType.Int32;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                curSupplierData.OracleDbType = OracleDbType.RefCursor;
                curSupplierData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSupplierData);

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
