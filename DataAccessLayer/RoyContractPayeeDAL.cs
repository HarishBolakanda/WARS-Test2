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
    public class RoyContractPayeeDAL : IRoyContractPayeeDAL
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

        public DataSet GetPayeeData(string royaltorId, out string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();                
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pPayeeData = new OracleParameter();
                OracleParameter pPayeeType = new OracleParameter();
                OracleParameter pTaxType = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_payee.p_get_contract_payee_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pPayeeData.OracleDbType = OracleDbType.RefCursor;
                pPayeeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeData);

                pPayeeType.OracleDbType = OracleDbType.RefCursor;
                pPayeeType.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeType);

                pTaxType.OracleDbType = OracleDbType.RefCursor;
                pTaxType.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTaxType);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);                                               
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();
                
                
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

        public DataSet GetIntPartySearchList(string intPartyType, string intPartyName, string intPartyIds, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pIntPartyType = new OracleParameter();
                OracleParameter pIntPartyName = new OracleParameter();
                OracleParameter pIntPartyIds = new OracleParameter();
                OracleParameter pIntPartyList = new OracleParameter();


                orlCmd = new OracleCommand("pkg_roy_contract_payee.p_get_interested_party_search", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pIntPartyType.OracleDbType = OracleDbType.Varchar2;
                pIntPartyType.Direction = ParameterDirection.Input;
                pIntPartyType.Value = intPartyType;
                orlCmd.Parameters.Add(pIntPartyType);

                pIntPartyName.OracleDbType = OracleDbType.Varchar2;
                pIntPartyName.Direction = ParameterDirection.Input;
                pIntPartyName.Value = intPartyName;
                orlCmd.Parameters.Add(pIntPartyName);

                pIntPartyIds.OracleDbType = OracleDbType.Varchar2;
                pIntPartyIds.Direction = ParameterDirection.Input;
                pIntPartyIds.Value = intPartyIds;
                orlCmd.Parameters.Add(pIntPartyIds);

                pIntPartyList.OracleDbType = OracleDbType.RefCursor;
                pIntPartyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pIntPartyList);

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
        
        public DataSet SavePayee(string royaltorId, Array payeeList, Array deleteList, string loggedUser, out string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pPayeeList = new OracleParameter();
                OracleParameter pDeleteList = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pPayeeData = new OracleParameter();
                OracleParameter pPayeeType = new OracleParameter();
                OracleParameter pTaxType = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();
                
                orlCmd = new OracleCommand("pkg_roy_contract_payee.p_save_payee", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pPayeeList.OracleDbType = OracleDbType.Varchar2;
                pPayeeList.Direction = ParameterDirection.Input;
                pPayeeList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (payeeList.Length == 0)
                {
                    pPayeeList.Size = 1;
                    pPayeeList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pPayeeList.Size = payeeList.Length;
                    pPayeeList.Value = payeeList;
                }
                orlCmd.Parameters.Add(pPayeeList);

                pDeleteList.OracleDbType = OracleDbType.Varchar2;
                pDeleteList.Direction = ParameterDirection.Input;
                pDeleteList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (deleteList.Length == 0)
                {
                    pDeleteList.Size = 1;
                    pDeleteList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pDeleteList.Size = deleteList.Length;
                    pDeleteList.Value = deleteList;
                }
                orlCmd.Parameters.Add(pDeleteList);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pPayeeData.OracleDbType = OracleDbType.RefCursor;
                pPayeeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeData);

                pPayeeType.OracleDbType = OracleDbType.RefCursor;
                pPayeeType.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeType);

                pTaxType.OracleDbType = OracleDbType.RefCursor;
                pTaxType.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTaxType);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);               

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();

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

        public DataSet DeletePayee(string royaltorId, string intPartyId, string intPartyType, string loggedUser, out string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pIntPartyId = new OracleParameter();
                OracleParameter pIntPartyType = new OracleParameter();                
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pPayeeData = new OracleParameter();
                OracleParameter pPayeeType = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_payee.p_delete_payee", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pIntPartyId.OracleDbType = OracleDbType.Varchar2;
                pIntPartyId.Direction = ParameterDirection.Input;
                pIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(pIntPartyId);

                pIntPartyType.OracleDbType = OracleDbType.Varchar2;
                pIntPartyType.Direction = ParameterDirection.Input;
                pIntPartyType.Value = intPartyType;
                orlCmd.Parameters.Add(pIntPartyType);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pPayeeData.OracleDbType = OracleDbType.RefCursor;
                pPayeeData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeData);

                pPayeeType.OracleDbType = OracleDbType.RefCursor;
                pPayeeType.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeType);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();

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

        public void AddInterestedParty(string partyType, string partyName, string address1, string address2, string address3, string address4, string postCode, string email, string VATNum, string taxType,
            string generateInvoice, string loggedUser, out string intPartyId, out string ipNumber, out Int32 iErrorId)
        {
            intPartyId = string.Empty;
            ipNumber = string.Empty;
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pPartyType = new OracleParameter();
                OracleParameter pPartyName = new OracleParameter();
                OracleParameter pAddress1 = new OracleParameter();
                OracleParameter pAddress2 = new OracleParameter();
                OracleParameter pAddress3 = new OracleParameter();
                OracleParameter pAddress4 = new OracleParameter();
                OracleParameter pPostCode = new OracleParameter();
                OracleParameter pEmail = new OracleParameter();
                OracleParameter pVATNum = new OracleParameter();
                OracleParameter pTaxType = new OracleParameter();
                OracleParameter pGenerateInvoice = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pIntPartyId = new OracleParameter();
                OracleParameter pIPNumber = new OracleParameter();
                

                orlCmd = new OracleCommand("pkg_roy_contract_payee.p_add_interested_party", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pPartyType.OracleDbType = OracleDbType.Varchar2;
                pPartyType.Direction = ParameterDirection.Input;
                pPartyType.Value = partyType;
                orlCmd.Parameters.Add(pPartyType);

                pPartyName.OracleDbType = OracleDbType.Varchar2;
                pPartyName.Direction = ParameterDirection.Input;
                pPartyName.Value = partyName;
                orlCmd.Parameters.Add(pPartyName);

                pAddress1.OracleDbType = OracleDbType.Varchar2;
                pAddress1.Direction = ParameterDirection.Input;
                pAddress1.Value = address1;
                orlCmd.Parameters.Add(pAddress1);

                pAddress2.OracleDbType = OracleDbType.Varchar2;
                pAddress2.Direction = ParameterDirection.Input;
                pAddress2.Value = address2;
                orlCmd.Parameters.Add(pAddress2);

                pAddress3.OracleDbType = OracleDbType.Varchar2;
                pAddress3.Direction = ParameterDirection.Input;
                pAddress3.Value = address3;
                orlCmd.Parameters.Add(pAddress3);

                pAddress4.OracleDbType = OracleDbType.Varchar2;
                pAddress4.Direction = ParameterDirection.Input;
                pAddress4.Value = address4;
                orlCmd.Parameters.Add(pAddress4);

                pPostCode.OracleDbType = OracleDbType.Varchar2;
                pPostCode.Direction = ParameterDirection.Input;
                pPostCode.Value = postCode;
                orlCmd.Parameters.Add(pPostCode);

                pEmail.OracleDbType = OracleDbType.Varchar2;
                pEmail.Direction = ParameterDirection.Input;
                pEmail.Value = email;
                orlCmd.Parameters.Add(pEmail);

                pVATNum.OracleDbType = OracleDbType.Varchar2;
                pVATNum.Direction = ParameterDirection.Input;
                pVATNum.Value = VATNum;
                orlCmd.Parameters.Add(pVATNum);

                pTaxType.OracleDbType = OracleDbType.Varchar2;
                pTaxType.Direction = ParameterDirection.Input;
                pTaxType.Value = taxType;
                orlCmd.Parameters.Add(pTaxType);

                pGenerateInvoice.OracleDbType = OracleDbType.Varchar2;
                pGenerateInvoice.Direction = ParameterDirection.Input;
                pGenerateInvoice.Value = generateInvoice;
                orlCmd.Parameters.Add(pGenerateInvoice);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pIntPartyId.OracleDbType = OracleDbType.Varchar2;
                pIntPartyId.Size = 250;
                pIntPartyId.Direction = ParameterDirection.Output;
                pIntPartyId.ParameterName = "intPartyId";
                orlCmd.Parameters.Add(pIntPartyId);

                pIPNumber.OracleDbType = OracleDbType.Varchar2;
                pIPNumber.Size = 250;
                pIPNumber.Direction = ParameterDirection.Output;
                pIPNumber.ParameterName = "ipNumber";
                orlCmd.Parameters.Add(pIPNumber);  

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                intPartyId = orlCmd.Parameters["intPartyId"].Value.ToString();
                ipNumber = orlCmd.Parameters["ipNumber"].Value.ToString();

            }
            catch (Exception ex)
            {
                iErrorId = 2;
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
        #endregion

    }
}
