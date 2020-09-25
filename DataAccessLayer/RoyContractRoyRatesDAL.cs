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
    public class RoyContractRoyRatesDAL : IRoyContractRoyRatesDAL
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

        public DataSet GetRoyaltyRatesData(string royaltorId, string optionPeriod, string userRoleId, out string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();                
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pOptionPeriod = new OracleParameter();
                OracleParameter pUserRoleId = new OracleParameter();
                OracleParameter pRoyaltyRatesData = new OracleParameter();
                OracleParameter pOptionPeriodList = new OracleParameter();                
                OracleParameter pSalesTypeList = new OracleParameter();
                OracleParameter pPriceFieldList = new OracleParameter();
                OracleParameter pCatnoList = new OracleParameter();                
                OracleParameter pRoyaltor = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_royalty_rates.p_get_contract_royalty_rates", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pOptionPeriod.OracleDbType = OracleDbType.Varchar2;
                pOptionPeriod.Direction = ParameterDirection.Input;
                pOptionPeriod.Value = optionPeriod;
                orlCmd.Parameters.Add(pOptionPeriod);

                pUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pUserRoleId.Direction = ParameterDirection.Input;
                pUserRoleId.Value = userRoleId;
                orlCmd.Parameters.Add(pUserRoleId);

                pRoyaltyRatesData.OracleDbType = OracleDbType.RefCursor;
                pRoyaltyRatesData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoyaltyRatesData);

                pOptionPeriodList.OracleDbType = OracleDbType.RefCursor;
                pOptionPeriodList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOptionPeriodList);

                pSalesTypeList.OracleDbType = OracleDbType.RefCursor;
                pSalesTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSalesTypeList);

                pPriceFieldList.OracleDbType = OracleDbType.RefCursor;
                pPriceFieldList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPriceFieldList);

                pCatnoList.OracleDbType = OracleDbType.RefCursor;
                pCatnoList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatnoList);

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

        public DataSet SaveRoyaltyRates(string royaltorId, string optionPeriod, string loggedUser, string userRoleId, Array addUpdateList, Array deleteList, out string royaltor, out string invalidCatno, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            invalidCatno = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pOptionPeriod = new OracleParameter();
                OracleParameter pUserRoleId = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pAddUpdateList = new OracleParameter();
                OracleParameter pDeleteList = new OracleParameter();
                OracleParameter pRoyaltyRatesData = new OracleParameter();
                OracleParameter pOptionPeriodList = new OracleParameter();                
                OracleParameter pSalesTypeList = new OracleParameter();
                OracleParameter pPriceFieldList = new OracleParameter();
                OracleParameter pCatnoList = new OracleParameter();                
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pInvalidCatno = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_royalty_rates.p_save_contract_royalty_rates", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pOptionPeriod.OracleDbType = OracleDbType.Varchar2;
                pOptionPeriod.Direction = ParameterDirection.Input;
                pOptionPeriod.Value = optionPeriod;
                orlCmd.Parameters.Add(pOptionPeriod);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pUserRoleId.Direction = ParameterDirection.Input;
                pUserRoleId.Value = userRoleId;
                orlCmd.Parameters.Add(pUserRoleId);

                pAddUpdateList.OracleDbType = OracleDbType.Varchar2;
                pAddUpdateList.Direction = ParameterDirection.Input;
                pAddUpdateList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (addUpdateList.Length == 0)
                {
                    pAddUpdateList.Size = 1;
                    pAddUpdateList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pAddUpdateList.Size = addUpdateList.Length;
                    pAddUpdateList.Value = addUpdateList;
                }
                orlCmd.Parameters.Add(pAddUpdateList);

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

                pRoyaltyRatesData.OracleDbType = OracleDbType.RefCursor;
                pRoyaltyRatesData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoyaltyRatesData);

                pOptionPeriodList.OracleDbType = OracleDbType.RefCursor;
                pOptionPeriodList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOptionPeriodList);
                
                pSalesTypeList.OracleDbType = OracleDbType.RefCursor;
                pSalesTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSalesTypeList);

                pPriceFieldList.OracleDbType = OracleDbType.RefCursor;
                pPriceFieldList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPriceFieldList);

                pCatnoList.OracleDbType = OracleDbType.RefCursor;
                pCatnoList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatnoList);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                pInvalidCatno.OracleDbType = OracleDbType.Varchar2;
                pInvalidCatno.Size = 500;
                pInvalidCatno.Direction = ParameterDirection.Output;
                pInvalidCatno.ParameterName = "out_v_invalid_catno";
                orlCmd.Parameters.Add(pInvalidCatno);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();
                invalidCatno = orlCmd.Parameters["out_v_invalid_catno"].Value.ToString();


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
