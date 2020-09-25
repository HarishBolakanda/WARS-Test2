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
    public class RoyContractOptionPeriodsDAL : IRoyContractOptionPeriodsDAL
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

        public DataSet GetOptionPeriodData(string royaltorId, string userRoleId, out string royaltor, out Int32 maxOptionPeriodCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            maxOptionPeriodCode = 0;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pUserRoleId = new OracleParameter();
                OracleParameter pOptionPeriodData = new OracleParameter();
                OracleParameter pUnitFieldList = new OracleParameter();
                OracleParameter pPriceFieldList = new OracleParameter();
                OracleParameter pReceiptFieldList = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pMaxOptionPeriodCode = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_option_period.p_get_contract_option_period", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pUserRoleId.Direction = ParameterDirection.Input;
                pUserRoleId.Value = userRoleId;
                orlCmd.Parameters.Add(pUserRoleId);

                pOptionPeriodData.OracleDbType = OracleDbType.RefCursor;
                pOptionPeriodData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOptionPeriodData);

                pUnitFieldList.OracleDbType = OracleDbType.RefCursor;
                pUnitFieldList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pUnitFieldList);

                pPriceFieldList.OracleDbType = OracleDbType.RefCursor;
                pPriceFieldList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPriceFieldList);

                pReceiptFieldList.OracleDbType = OracleDbType.RefCursor;
                pReceiptFieldList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pReceiptFieldList);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                pMaxOptionPeriodCode.OracleDbType = OracleDbType.Int32;
                pMaxOptionPeriodCode.Size = 250;
                pMaxOptionPeriodCode.Direction = ParameterDirection.Output;
                pMaxOptionPeriodCode.ParameterName = "out_max_option_period_code";
                orlCmd.Parameters.Add(pMaxOptionPeriodCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();
                maxOptionPeriodCode = Convert.ToInt32(orlCmd.Parameters["out_max_option_period_code"].Value.ToString());
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

        public DataSet SaveOptionPeriod(string royaltorId, Array optionPeriodList, Array deleteList, string loggedUser, string userRoleId, out string royaltor, out Int32 outmaxOptionPeriodCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            outmaxOptionPeriodCode = 0;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pOptionPeriodList = new OracleParameter();
                OracleParameter pDeleteList = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pUserRoleId = new OracleParameter();
                OracleParameter pOptionPeriodData = new OracleParameter();
                OracleParameter pUnitFieldList = new OracleParameter();
                OracleParameter pPriceFieldList = new OracleParameter();
                OracleParameter pReceiptFieldList = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pOutMaxPeriodCode = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_option_period.p_save_option_period", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pOptionPeriodList.OracleDbType = OracleDbType.Varchar2;
                pOptionPeriodList.Direction = ParameterDirection.Input;
                pOptionPeriodList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (optionPeriodList.Length == 0)
                {
                    pOptionPeriodList.Size = 1;
                    pOptionPeriodList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pOptionPeriodList.Size = optionPeriodList.Length;
                    pOptionPeriodList.Value = optionPeriodList;
                }
                orlCmd.Parameters.Add(pOptionPeriodList);

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

                pUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pUserRoleId.Direction = ParameterDirection.Input;
                pUserRoleId.Value = userRoleId;
                orlCmd.Parameters.Add(pUserRoleId);

                pOptionPeriodData.OracleDbType = OracleDbType.RefCursor;
                pOptionPeriodData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOptionPeriodData);

                pUnitFieldList.OracleDbType = OracleDbType.RefCursor;
                pUnitFieldList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pUnitFieldList);

                pPriceFieldList.OracleDbType = OracleDbType.RefCursor;
                pPriceFieldList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPriceFieldList);

                pReceiptFieldList.OracleDbType = OracleDbType.RefCursor;
                pReceiptFieldList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pReceiptFieldList);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                pOutMaxPeriodCode.OracleDbType = OracleDbType.Int32;
                pOutMaxPeriodCode.Size = 250;
                pOutMaxPeriodCode.Direction = ParameterDirection.Output;
                pOutMaxPeriodCode.ParameterName = "out_max_option_period_code";
                orlCmd.Parameters.Add(pOutMaxPeriodCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();
                outmaxOptionPeriodCode = Convert.ToInt32(orlCmd.Parameters["out_max_option_period_code"].Value.ToString());
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

        public void ValidateDelete(string royaltorId, string optionPeriodCode, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pOptionPeriodCode = new OracleParameter();
                orlCmd = new OracleCommand("pkg_roy_contract_option_period.p_validate_delete", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pOptionPeriodCode.OracleDbType = OracleDbType.Varchar2;
                pOptionPeriodCode.Direction = ParameterDirection.Input;
                pOptionPeriodCode.Value = optionPeriodCode;
                orlCmd.Parameters.Add(pOptionPeriodCode);

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
