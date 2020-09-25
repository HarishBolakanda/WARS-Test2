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
    public class RoyaltorSearchDAL : IRoyaltorSearchDAL
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

        public DataSet GetDropdownData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();                
                ErrorId = new OracleParameter();                
                OracleParameter pResponsibilityList = new OracleParameter();
                OracleParameter pStatusList = new OracleParameter();
                OracleParameter pContractTypeList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_search_screen.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                pResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pResponsibilityList);

                pStatusList.OracleDbType = OracleDbType.RefCursor;
                pStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStatusList);

                pContractTypeList.OracleDbType = OracleDbType.RefCursor;
                pContractTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pContractTypeList);
                
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

        public DataSet GetSearchData(string royaltor, string plgRoyaltor, string owner, string isCompanySelected, string responsibility, string status, string isRoyaltorHeld, string contractType, Array royaltorBulkSearchList, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pPlgRoyaltor = new OracleParameter();
                OracleParameter pOwner = new OracleParameter();
                OracleParameter pIsCompanySelected = new OracleParameter();
                OracleParameter pResponsibility= new OracleParameter();
                OracleParameter pStatus = new OracleParameter();
                OracleParameter pIsRoyaltorHeld = new OracleParameter();
                OracleParameter pContractType = new OracleParameter();
                OracleParameter pRoyaltorBulkSearchList = new OracleParameter();
                OracleParameter pSearchData = new OracleParameter();                
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_search_screen.p_get_search_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Direction = ParameterDirection.Input;
                pRoyaltor.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltor);

                pPlgRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pPlgRoyaltor.Direction = ParameterDirection.Input;
                pPlgRoyaltor.Value = plgRoyaltor;
                orlCmd.Parameters.Add(pPlgRoyaltor);

                pOwner.OracleDbType = OracleDbType.Varchar2;
                pOwner.Direction = ParameterDirection.Input;
                pOwner.Value = owner;
                orlCmd.Parameters.Add(pOwner);

                pIsCompanySelected.OracleDbType = OracleDbType.Varchar2;
                pIsCompanySelected.Direction = ParameterDirection.Input;
                pIsCompanySelected.Value = isCompanySelected;
                orlCmd.Parameters.Add(pIsCompanySelected);

                pResponsibility.OracleDbType = OracleDbType.Varchar2;
                pResponsibility.Direction = ParameterDirection.Input;
                pResponsibility.Value = (responsibility == "-" ? string.Empty : responsibility);
                orlCmd.Parameters.Add(pResponsibility);

                pStatus.OracleDbType = OracleDbType.Varchar2;
                pStatus.Direction = ParameterDirection.Input;
                pStatus.Value = (status == "-" ? string.Empty : status);
                orlCmd.Parameters.Add(pStatus);

                pIsRoyaltorHeld.OracleDbType = OracleDbType.Varchar2;
                pIsRoyaltorHeld.Direction = ParameterDirection.Input;
                pIsRoyaltorHeld.Value = isRoyaltorHeld;
                orlCmd.Parameters.Add(pIsRoyaltorHeld);

                pContractType.OracleDbType = OracleDbType.Varchar2;
                pContractType.Direction = ParameterDirection.Input;
                pContractType.Value = (contractType == "-" ? string.Empty : contractType);
                orlCmd.Parameters.Add(pContractType);

                pRoyaltorBulkSearchList.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorBulkSearchList.Direction = ParameterDirection.Input;
                pRoyaltorBulkSearchList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (royaltorBulkSearchList.Length == 0)
                {
                    pRoyaltorBulkSearchList.Size = 0;
                    pRoyaltorBulkSearchList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pRoyaltorBulkSearchList.Size = royaltorBulkSearchList.Length;
                    pRoyaltorBulkSearchList.Value = royaltorBulkSearchList;
                }
                orlCmd.Parameters.Add(pRoyaltorBulkSearchList);

                pSearchData.OracleDbType = OracleDbType.RefCursor;
                pSearchData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSearchData);

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

        public DataSet UpdateRoyaltor(string royaltor, string plgRoyaltor, string owner, string isCompanySelected, string responsibility, string status, string isRoyaltorHeld, string contractType, Array royaltors,
                               string isLockUnlock, string lockUnlockAll, string updateStatusCode, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pPlgRoyaltor = new OracleParameter();
                OracleParameter pOwner = new OracleParameter();
                OracleParameter pIsCompanySelected = new OracleParameter();
                OracleParameter pResponsibility = new OracleParameter();
                OracleParameter pStatus = new OracleParameter();
                OracleParameter pIsRoyaltorHeld = new OracleParameter();
                OracleParameter pContractType = new OracleParameter(); 
                OracleParameter pRoyaltors = new OracleParameter();
                OracleParameter pIsLockUnlock = new OracleParameter();
                OracleParameter pLockUnlockAll = new OracleParameter();
                OracleParameter pUpdateStatusCode = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pSearchData = new OracleParameter();                    
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_search_screen.p_update_royaltor", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Direction = ParameterDirection.Input;
                pRoyaltor.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltor);

                pPlgRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pPlgRoyaltor.Direction = ParameterDirection.Input;
                pPlgRoyaltor.Value = plgRoyaltor;
                orlCmd.Parameters.Add(pPlgRoyaltor);

                pOwner.OracleDbType = OracleDbType.Varchar2;
                pOwner.Direction = ParameterDirection.Input;
                pOwner.Value = owner;
                orlCmd.Parameters.Add(pOwner);

                pIsCompanySelected.OracleDbType = OracleDbType.Varchar2;
                pIsCompanySelected.Direction = ParameterDirection.Input;
                pIsCompanySelected.Value = isCompanySelected;
                orlCmd.Parameters.Add(pIsCompanySelected);

                pResponsibility.OracleDbType = OracleDbType.Varchar2;
                pResponsibility.Direction = ParameterDirection.Input;
                pResponsibility.Value = (responsibility == "-" ? string.Empty : responsibility);
                orlCmd.Parameters.Add(pResponsibility);

                pStatus.OracleDbType = OracleDbType.Varchar2;
                pStatus.Direction = ParameterDirection.Input;
                pStatus.Value = (status == "-" ? string.Empty : status);
                orlCmd.Parameters.Add(pStatus);

                pIsRoyaltorHeld.OracleDbType = OracleDbType.Varchar2;
                pIsRoyaltorHeld.Direction = ParameterDirection.Input;
                pIsRoyaltorHeld.Value = isRoyaltorHeld;
                orlCmd.Parameters.Add(pIsRoyaltorHeld);

                pContractType.OracleDbType = OracleDbType.Varchar2;
                pContractType.Direction = ParameterDirection.Input;
                pContractType.Value = (contractType == "-" ? string.Empty : contractType);
                orlCmd.Parameters.Add(pContractType);

                pRoyaltors.OracleDbType = OracleDbType.Varchar2;
                pRoyaltors.Direction = ParameterDirection.Input;
                pRoyaltors.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (royaltors.Length == 0)
                {
                    pRoyaltors.Size = 1;
                    pRoyaltors.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pRoyaltors.Size = royaltors.Length;
                    pRoyaltors.Value = royaltors;
                }
                orlCmd.Parameters.Add(pRoyaltors);

                pIsLockUnlock.OracleDbType = OracleDbType.Varchar2;
                pIsLockUnlock.Direction = ParameterDirection.Input;
                pIsLockUnlock.Value = isLockUnlock;
                orlCmd.Parameters.Add(pIsLockUnlock);

                pLockUnlockAll.OracleDbType = OracleDbType.Varchar2;
                pLockUnlockAll.Direction = ParameterDirection.Input;
                pLockUnlockAll.Value = lockUnlockAll;
                orlCmd.Parameters.Add(pLockUnlockAll);

                pUpdateStatusCode.OracleDbType = OracleDbType.Varchar2;
                pUpdateStatusCode.Direction = ParameterDirection.Input;
                pUpdateStatusCode.Value = updateStatusCode;
                orlCmd.Parameters.Add(pUpdateStatusCode);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pSearchData.OracleDbType = OracleDbType.RefCursor;
                pSearchData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSearchData);

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
