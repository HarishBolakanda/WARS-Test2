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
    public class UserAccountMaintDAL : IUserAccountMaintDAL
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
        
        public DataSet GetData(string userFilter, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();
                OracleParameter pUserFilter = new OracleParameter();
                OracleParameter pResponsibilityList = new OracleParameter();
                OracleParameter pRoleList = new OracleParameter();
                OracleParameter pPaymentRoleList = new OracleParameter();
                OracleParameter pSearchFilterList = new OracleParameter();                                
                OracleParameter pUserAccountData = new OracleParameter();
                orlCmd = new OracleCommand("pkg_maint_user_account.p_get_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pUserFilter.OracleDbType = OracleDbType.Varchar2;
                pUserFilter.Direction = ParameterDirection.Input;
                if (userFilter == string.Empty)
                    pUserFilter.Value = DBNull.Value;
                else
                    pUserFilter.Value = userFilter;
                orlCmd.Parameters.Add(pUserFilter);

                pResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                pResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pResponsibilityList);

                pRoleList.OracleDbType = OracleDbType.RefCursor;
                pRoleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoleList);

                pPaymentRoleList.OracleDbType = OracleDbType.RefCursor;
                pPaymentRoleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPaymentRoleList);

                pSearchFilterList.OracleDbType = OracleDbType.RefCursor;
                pSearchFilterList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSearchFilterList);

                pUserAccountData.OracleDbType = OracleDbType.RefCursor;
                pUserAccountData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pUserAccountData);  
                
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


        public DataSet AddUserAccount(string userName, string userCode, string userAccId, string respCode, string roleId, string paymentRoleId, string isActive, string userFilter, string loggedUser,
                                       out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter pUserName = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pUserAccId = new OracleParameter();
                OracleParameter pRespCode = new OracleParameter();
                OracleParameter pRoleId = new OracleParameter();
                OracleParameter pPaymentRoleId = new OracleParameter();
                OracleParameter pIsActive = new OracleParameter();
                OracleParameter pUserFilter = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pResponsibilityList = new OracleParameter();
                OracleParameter pRoleList = new OracleParameter();
                OracleParameter pPaymentRoleList = new OracleParameter();
                OracleParameter pSearchFilterList = new OracleParameter();
                OracleParameter pUserAccountData = new OracleParameter();
                
                orlCmd = new OracleCommand("pkg_maint_user_account.p_add_user_account", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pUserName.OracleDbType = OracleDbType.Varchar2;
                pUserName.Direction = ParameterDirection.Input;
                pUserName.Value = userName;
                orlCmd.Parameters.Add(pUserName);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pUserAccId.OracleDbType = OracleDbType.Varchar2;
                pUserAccId.Direction = ParameterDirection.Input;
                pUserAccId.Value = userAccId;
                orlCmd.Parameters.Add(pUserAccId);

                pRespCode.OracleDbType = OracleDbType.Varchar2;
                pRespCode.Direction = ParameterDirection.Input;
                pRespCode.Value = respCode;
                orlCmd.Parameters.Add(pRespCode);

                pRoleId.OracleDbType = OracleDbType.Varchar2;
                pRoleId.Direction = ParameterDirection.Input;
                pRoleId.Value = roleId;
                orlCmd.Parameters.Add(pRoleId);

                pPaymentRoleId.OracleDbType = OracleDbType.Varchar2;
                pPaymentRoleId.Direction = ParameterDirection.Input;
                pPaymentRoleId.Value = paymentRoleId;
                orlCmd.Parameters.Add(pPaymentRoleId);

                pIsActive.OracleDbType = OracleDbType.Varchar2;
                pIsActive.Direction = ParameterDirection.Input;
                if (isActive == string.Empty)
                    pIsActive.Value = DBNull.Value;
                else
                    pIsActive.Value = isActive;
                orlCmd.Parameters.Add(pIsActive);

                pUserFilter.OracleDbType = OracleDbType.Varchar2;
                pUserFilter.Direction = ParameterDirection.Input;
                if (userFilter == string.Empty)
                    pUserFilter.Value = DBNull.Value;
                else
                    pUserFilter.Value = userFilter;
                orlCmd.Parameters.Add(pUserFilter);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);                

                pResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                pResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pResponsibilityList);

                pRoleList.OracleDbType = OracleDbType.RefCursor;
                pRoleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoleList);

                pPaymentRoleList.OracleDbType = OracleDbType.RefCursor;
                pPaymentRoleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPaymentRoleList);

                pSearchFilterList.OracleDbType = OracleDbType.RefCursor;
                pSearchFilterList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSearchFilterList);

                pUserAccountData.OracleDbType = OracleDbType.RefCursor;
                pUserAccountData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pUserAccountData);

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

        public DataSet UpdateUserAccount(string userAccId, string userCode, string respCode, string userName, string userCodeNew, string userAccIdNew, string respCodeNew,
                                    string roleId, string paymentRoleId, string isActive, string userFilter, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter pUserAccId = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();                
                OracleParameter pRespCode = new OracleParameter();
                OracleParameter pUserName = new OracleParameter();
                OracleParameter pUserCodeNew = new OracleParameter();
                OracleParameter pUserAccIdNew = new OracleParameter();
                OracleParameter pRespCodeNew = new OracleParameter();
                OracleParameter pRoleId = new OracleParameter();
                OracleParameter pPaymentRoleId = new OracleParameter();
                OracleParameter pIsActive = new OracleParameter();
                OracleParameter pUserFilter = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pResponsibilityList = new OracleParameter();
                OracleParameter pRoleList = new OracleParameter();
                OracleParameter pPaymentRoleList = new OracleParameter();
                OracleParameter pSearchFilterList = new OracleParameter();
                OracleParameter pUserAccountData = new OracleParameter();
                
                orlCmd = new OracleCommand("pkg_maint_user_account.p_update_user_account", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                
                pUserAccId.OracleDbType = OracleDbType.Varchar2;
                pUserAccId.Direction = ParameterDirection.Input;
                pUserAccId.Value = userAccId;
                orlCmd.Parameters.Add(pUserAccId);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pRespCode.OracleDbType = OracleDbType.Varchar2;
                pRespCode.Direction = ParameterDirection.Input;
                pRespCode.Value = respCode;
                orlCmd.Parameters.Add(pRespCode);

                pUserName.OracleDbType = OracleDbType.Varchar2;
                pUserName.Direction = ParameterDirection.Input;
                pUserName.Value = userName;
                orlCmd.Parameters.Add(pUserName);

                pUserCodeNew.OracleDbType = OracleDbType.Varchar2;
                pUserCodeNew.Direction = ParameterDirection.Input;
                pUserCodeNew.Value = userCodeNew;
                orlCmd.Parameters.Add(pUserCodeNew);

                pUserAccIdNew.OracleDbType = OracleDbType.Varchar2;
                pUserAccIdNew.Direction = ParameterDirection.Input;
                pUserAccIdNew.Value = userAccIdNew;
                orlCmd.Parameters.Add(pUserAccIdNew);

                pRespCodeNew.OracleDbType = OracleDbType.Varchar2;
                pRespCodeNew.Direction = ParameterDirection.Input;
                pRespCodeNew.Value = respCodeNew;
                orlCmd.Parameters.Add(pRespCodeNew);

                pRoleId.OracleDbType = OracleDbType.Varchar2;
                pRoleId.Direction = ParameterDirection.Input;
                pRoleId.Value = roleId;
                orlCmd.Parameters.Add(pRoleId);

                pPaymentRoleId.OracleDbType = OracleDbType.Varchar2;
                pPaymentRoleId.Direction = ParameterDirection.Input;
                pPaymentRoleId.Value = paymentRoleId;
                orlCmd.Parameters.Add(pPaymentRoleId);

                pIsActive.OracleDbType = OracleDbType.Varchar2;
                pIsActive.Direction = ParameterDirection.Input;
                if (isActive == string.Empty)
                    pIsActive.Value = DBNull.Value;
                else
                    pIsActive.Value = isActive;
                orlCmd.Parameters.Add(pIsActive);

                pUserFilter.OracleDbType = OracleDbType.Varchar2;
                pUserFilter.Direction = ParameterDirection.Input;
                if (userFilter == string.Empty)
                    pUserFilter.Value = DBNull.Value;
                else
                    pUserFilter.Value = userFilter;
                orlCmd.Parameters.Add(pUserFilter);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                pResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pResponsibilityList);

                pRoleList.OracleDbType = OracleDbType.RefCursor;
                pRoleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoleList);

                pPaymentRoleList.OracleDbType = OracleDbType.RefCursor;
                pPaymentRoleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPaymentRoleList);

                pSearchFilterList.OracleDbType = OracleDbType.RefCursor;
                pSearchFilterList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSearchFilterList);

                pUserAccountData.OracleDbType = OracleDbType.RefCursor;
                pUserAccountData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pUserAccountData);

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

        public void UpdateRespChanges(Int32 respToReplace, Int32 newResp, string loggedUser, out Int32 iErrorId)
        {            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter pRespToReplace = new OracleParameter();
                OracleParameter pNewResp = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_user_account.p_update_responsibility", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRespToReplace.OracleDbType = OracleDbType.Int32;
                pRespToReplace.Direction = ParameterDirection.Input;
                pRespToReplace.Value = respToReplace;
                orlCmd.Parameters.Add(pRespToReplace);

                pNewResp.OracleDbType = OracleDbType.Int32;
                pNewResp.Direction = ParameterDirection.Input;
                pNewResp.Value = newResp;
                orlCmd.Parameters.Add(pNewResp);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);
                                
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
        #endregion

    }
}
