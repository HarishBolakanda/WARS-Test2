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
    public class RoyaltorContractDAL : IRoyaltorContractDAL
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
                OracleParameter pCompanyList = new OracleParameter();
                OracleParameter pLabelList = new OracleParameter();
                OracleParameter pResponsibilityList = new OracleParameter();
                OracleParameter pReportingScheduleList = new OracleParameter();
                OracleParameter pStmtPriorityList = new OracleParameter();
                OracleParameter pReserveBasisList = new OracleParameter();
                OracleParameter pUnitList = new OracleParameter();
                OracleParameter pStatusList = new OracleParameter();
                OracleParameter pContractTypeList = new OracleParameter();
                OracleParameter pStatementFormat = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCompanyList.OracleDbType = OracleDbType.RefCursor;
                pCompanyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCompanyList);

                pLabelList.OracleDbType = OracleDbType.RefCursor;
                pLabelList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pLabelList);

                pResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                pResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pResponsibilityList);

                pReportingScheduleList.OracleDbType = OracleDbType.RefCursor;
                pReportingScheduleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pReportingScheduleList);

                pStmtPriorityList.OracleDbType = OracleDbType.RefCursor;
                pStmtPriorityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtPriorityList);

                pReserveBasisList.OracleDbType = OracleDbType.RefCursor;
                pReserveBasisList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pReserveBasisList);

                pUnitList.OracleDbType = OracleDbType.RefCursor;
                pUnitList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pUnitList);

                pStatusList.OracleDbType = OracleDbType.RefCursor;
                pStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStatusList);

                pContractTypeList.OracleDbType = OracleDbType.RefCursor;
                pContractTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pContractTypeList);

                pStatementFormat.OracleDbType = OracleDbType.RefCursor;
                pStatementFormat.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStatementFormat);

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

        public DataSet GetSearchData(string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pSearchData = new OracleParameter();
                OracleParameter pReservePlan = new OracleParameter();

                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_get_search_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Direction = ParameterDirection.Input;
                pRoyaltor.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltor);

                pSearchData.OracleDbType = OracleDbType.RefCursor;
                pSearchData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSearchData);

                pReservePlan.OracleDbType = OracleDbType.RefCursor;
                pReservePlan.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pReservePlan);

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

        public DataSet AddUpateContract(string royaltor, string isRespChanged, string isOwnerChanged, Array contractParams, Array liquidationParams, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pIsRespChanged = new OracleParameter();
                OracleParameter pIsOwnerChanged = new OracleParameter();
                OracleParameter pContractParams = new OracleParameter();
                OracleParameter pLiquidationParams = new OracleParameter();
                OracleParameter pSearchData = new OracleParameter();
                OracleParameter pReservePlan = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_add_update_contract", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Direction = ParameterDirection.Input;
                if (royaltor == "-")
                    pRoyaltor.Value = DBNull.Value;
                else
                    pRoyaltor.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltor);

                pIsRespChanged.OracleDbType = OracleDbType.Varchar2;
                pIsRespChanged.Direction = ParameterDirection.Input;
                pIsRespChanged.Value = isRespChanged;
                orlCmd.Parameters.Add(pIsRespChanged);

                pIsOwnerChanged.OracleDbType = OracleDbType.Varchar2;
                pIsOwnerChanged.Direction = ParameterDirection.Input;
                pIsOwnerChanged.Value = isOwnerChanged;
                orlCmd.Parameters.Add(pIsOwnerChanged);

                pContractParams.OracleDbType = OracleDbType.Varchar2;
                pContractParams.Direction = ParameterDirection.Input;
                pContractParams.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (contractParams.Length == 0)
                {
                    pContractParams.Size = 1;
                    pContractParams.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pContractParams.Size = contractParams.Length;
                    pContractParams.Value = contractParams;
                }
                orlCmd.Parameters.Add(pContractParams);

                pLiquidationParams.OracleDbType = OracleDbType.Varchar2;
                pLiquidationParams.Direction = ParameterDirection.Input;
                pLiquidationParams.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (liquidationParams.Length == 0)
                {
                    pLiquidationParams.Size = 1;
                    pLiquidationParams.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pLiquidationParams.Size = liquidationParams.Length;
                    pLiquidationParams.Value = liquidationParams;
                }
                orlCmd.Parameters.Add(pLiquidationParams);

                pSearchData.OracleDbType = OracleDbType.RefCursor;
                pSearchData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSearchData);

                pReservePlan.OracleDbType = OracleDbType.RefCursor;
                pReservePlan.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pReservePlan);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

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

        public void CopyContract(string royaltorIdCopy, string royaltorIdNew, string royaltorName, string loggedUser, string optionCodes, string royRates, string subRates, string packRates, string escCodes, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pRoyaltorIdCopy = new OracleParameter();
                OracleParameter pRoyaltorIdNew = new OracleParameter();
                OracleParameter pRoyaltorName = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pOptionCodes = new OracleParameter();
                OracleParameter pRoyRates = new OracleParameter();
                OracleParameter pSubRates = new OracleParameter();
                OracleParameter pPackRates = new OracleParameter();
                OracleParameter pEscCodes = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_copy_contract", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorIdCopy.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorIdCopy.Direction = ParameterDirection.Input;
                pRoyaltorIdCopy.Value = royaltorIdCopy;
                orlCmd.Parameters.Add(pRoyaltorIdCopy);

                pRoyaltorIdNew.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorIdNew.Direction = ParameterDirection.Input;
                pRoyaltorIdNew.Value = royaltorIdNew;
                orlCmd.Parameters.Add(pRoyaltorIdNew);

                pRoyaltorName.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorName.Direction = ParameterDirection.Input;
                pRoyaltorName.Value = royaltorName;
                orlCmd.Parameters.Add(pRoyaltorName);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pOptionCodes.OracleDbType = OracleDbType.Varchar2;
                pOptionCodes.Direction = ParameterDirection.Input;
                pOptionCodes.Value = optionCodes;
                orlCmd.Parameters.Add(pOptionCodes);

                pRoyRates.OracleDbType = OracleDbType.Varchar2;
                pRoyRates.Direction = ParameterDirection.Input;
                pRoyRates.Value = royRates;
                orlCmd.Parameters.Add(pRoyRates);

                pSubRates.OracleDbType = OracleDbType.Varchar2;
                pSubRates.Direction = ParameterDirection.Input;
                pSubRates.Value = subRates;
                orlCmd.Parameters.Add(pSubRates);

                pPackRates.OracleDbType = OracleDbType.Varchar2;
                pPackRates.Direction = ParameterDirection.Input;
                pPackRates.Value = packRates;
                orlCmd.Parameters.Add(pPackRates);

                pEscCodes.OracleDbType = OracleDbType.Varchar2;
                pEscCodes.Direction = ParameterDirection.Input;
                pEscCodes.Value = escCodes;
                orlCmd.Parameters.Add(pEscCodes);

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
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataSet LockUnlockContact(string royaltor, string locked, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pLocked = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pContractData = new OracleParameter();
                OracleParameter pReservePlan = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_lock_unlock_contract", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Direction = ParameterDirection.Input;
                pRoyaltor.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltor);

                pLocked.OracleDbType = OracleDbType.Varchar2;
                pLocked.Direction = ParameterDirection.Input;
                pLocked.Value = locked;
                orlCmd.Parameters.Add(pLocked);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pContractData.OracleDbType = OracleDbType.RefCursor;
                pContractData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pContractData);

                pReservePlan.OracleDbType = OracleDbType.RefCursor;
                pReservePlan.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pReservePlan);

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

        public DataSet AddOwnerDetails(string ownerCode, string ownerName, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pOwnerCode = new OracleParameter();
                OracleParameter pOwnerName = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pOwnerList = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_add_owner_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pOwnerCode.Direction = ParameterDirection.Input;
                pOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(pOwnerCode);

                pOwnerName.OracleDbType = OracleDbType.Varchar2;
                pOwnerName.Direction = ParameterDirection.Input;
                pOwnerName.Value = ownerName;
                orlCmd.Parameters.Add(pOwnerName);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pOwnerList.OracleDbType = OracleDbType.RefCursor;
                pOwnerList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOwnerList);

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
        public string GetNewOwnerCode(out Int32 iErrorId)
        {
            string newOwnerCode = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pNewOwnerCode = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_get_new_owner_code", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;


                pNewOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pNewOwnerCode.Size = 200;
                pNewOwnerCode.Direction = ParameterDirection.Output;
                pNewOwnerCode.ParameterName = "NewOwnerCode";
                orlCmd.Parameters.Add(pNewOwnerCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteScalar();
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                newOwnerCode = orlCmd.Parameters["NewOwnerCode"].Value.ToString();

            }
            catch (Exception ex)
            {
                iErrorId = 2;
            }
            finally
            {
                CloseConnection();
            }
            return newOwnerCode;
        }


        public DataSet GetOptionsForCopyContract(string royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pOtionsData = new OracleParameter();
                OracleParameter pEcsCodes = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_get_options_copy_contract", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Direction = ParameterDirection.Input;
                pRoyaltor.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltor);

                pOtionsData.OracleDbType = OracleDbType.RefCursor;
                pOtionsData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOtionsData);

                pEcsCodes.OracleDbType = OracleDbType.RefCursor;
                pEcsCodes.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEcsCodes);

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

        public void UpdateScreenLockFlag(string royaltor, string screenLockFlag, string loggedUser, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pScreenLockDlag = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_roy_contract.p_update_screen_lock_flag", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Direction = ParameterDirection.Input;
                pRoyaltor.Value = royaltor;
                orlCmd.Parameters.Add(pRoyaltor);

                pScreenLockDlag.OracleDbType = OracleDbType.Varchar2;
                pScreenLockDlag.Direction = ParameterDirection.Input;
                pScreenLockDlag.Value = screenLockFlag;
                orlCmd.Parameters.Add(pScreenLockDlag);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteScalar();

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
