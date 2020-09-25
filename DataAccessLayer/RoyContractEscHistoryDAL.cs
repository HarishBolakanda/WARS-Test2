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
    public class RoyContractEscHistoryDAL : IRoyContractEscHistoryDAL
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

        public DataSet GetInitialData(string royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();                
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();                                
                OracleParameter pEscCodeList = new OracleParameter();
                OracleParameter pEscHistory = new OracleParameter();
                
                orlCmd = new OracleCommand("pkg_roy_contract_esc_history.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId); 

                pEscCodeList.OracleDbType = OracleDbType.RefCursor;
                pEscCodeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscCodeList);

                pEscHistory.OracleDbType = OracleDbType.RefCursor;
                pEscHistory.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscHistory);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);                                               
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                
                if (iErrorId != 2)
                {
                    if (royaltorId != "-")
                    {                        
                        ds.Tables[0].TableName = "EscCodeList";
                        ds.Tables[1].TableName = "EscHistory";
                    }
                   
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

        public DataSet GetEscHistory(string royaltorId, string escCode, out Int32 iErrorId)
        {
            ds = new DataSet();            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pEscCode = new OracleParameter();                
                OracleParameter pEscHistory = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_esc_history.p_get_esc_history", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pEscCode.OracleDbType = OracleDbType.Varchar2;
                pEscCode.Direction = ParameterDirection.Input;
                pEscCode.Value = escCode;
                orlCmd.Parameters.Add(pEscCode);

                pEscHistory.OracleDbType = OracleDbType.RefCursor;
                pEscHistory.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscHistory);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "EscHistory";
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

        public DataSet GetEscHistorySummary(string royaltorId, string escCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pEscCode = new OracleParameter();
                OracleParameter pEscHistory = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_esc_history.p_get_esc_history_summary", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pEscCode.OracleDbType = OracleDbType.Varchar2;
                pEscCode.Direction = ParameterDirection.Input;
                pEscCode.Value = escCode;
                orlCmd.Parameters.Add(pEscCode);

                pEscHistory.OracleDbType = OracleDbType.RefCursor;
                pEscHistory.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscHistory);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "EscHistory";
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

        public DataSet SaveEscHistory(string royaltorId, string escCode, string sellerGrpCode, string priceGrpCode, string configGrpCode, string sales, string adjSales, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pEscCode = new OracleParameter();
                OracleParameter pSellerGrpCode = new OracleParameter();
                OracleParameter pPriceGrpCode = new OracleParameter();
                OracleParameter pConfigGrpCode = new OracleParameter();                
                OracleParameter pSales = new OracleParameter();
                OracleParameter pAdjSales = new OracleParameter();
                OracleParameter pEscHistory = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_esc_history.p_update_esc_history", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pEscCode.OracleDbType = OracleDbType.Varchar2;
                pEscCode.Direction = ParameterDirection.Input;
                pEscCode.Value = escCode;
                orlCmd.Parameters.Add(pEscCode);

                pSellerGrpCode.OracleDbType = OracleDbType.Varchar2;
                pSellerGrpCode.Direction = ParameterDirection.Input;
                pSellerGrpCode.Value = sellerGrpCode;
                orlCmd.Parameters.Add(pSellerGrpCode);

                pPriceGrpCode.OracleDbType = OracleDbType.Varchar2;
                pPriceGrpCode.Direction = ParameterDirection.Input;
                pPriceGrpCode.Value = priceGrpCode;
                orlCmd.Parameters.Add(pPriceGrpCode);

                pConfigGrpCode.OracleDbType = OracleDbType.Varchar2;
                pConfigGrpCode.Direction = ParameterDirection.Input;
                pConfigGrpCode.Value = configGrpCode;
                orlCmd.Parameters.Add(pConfigGrpCode);              

                pSales.OracleDbType = OracleDbType.Varchar2;
                pSales.Direction = ParameterDirection.Input;
                pSales.Value = sales;
                orlCmd.Parameters.Add(pSales);

                pAdjSales.OracleDbType = OracleDbType.Varchar2;
                pAdjSales.Direction = ParameterDirection.Input;
                pAdjSales.Value = adjSales;
                orlCmd.Parameters.Add(pAdjSales);

                pEscHistory.OracleDbType = OracleDbType.RefCursor;
                pEscHistory.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscHistory);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "EscHistory";
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

        public DataSet DeleteEscHistory(string royaltorId, string escCode, string sellerGrpCode, string priceGrpCode, string configGrpCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pEscCode = new OracleParameter();
                OracleParameter pSellerGrpCode = new OracleParameter();
                OracleParameter pPriceGrpCode = new OracleParameter();
                OracleParameter pConfigGrpCode = new OracleParameter();                
                OracleParameter pEscHistory = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_esc_history.p_delete_esc_history", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pEscCode.OracleDbType = OracleDbType.Varchar2;
                pEscCode.Direction = ParameterDirection.Input;
                pEscCode.Value = escCode;
                orlCmd.Parameters.Add(pEscCode);

                pSellerGrpCode.OracleDbType = OracleDbType.Varchar2;
                pSellerGrpCode.Direction = ParameterDirection.Input;
                pSellerGrpCode.Value = sellerGrpCode;
                orlCmd.Parameters.Add(pSellerGrpCode);

                pPriceGrpCode.OracleDbType = OracleDbType.Varchar2;
                pPriceGrpCode.Direction = ParameterDirection.Input;
                pPriceGrpCode.Value = priceGrpCode;
                orlCmd.Parameters.Add(pPriceGrpCode);

                pConfigGrpCode.OracleDbType = OracleDbType.Varchar2;
                pConfigGrpCode.Direction = ParameterDirection.Input;
                pConfigGrpCode.Value = configGrpCode;
                orlCmd.Parameters.Add(pConfigGrpCode);

                pEscHistory.OracleDbType = OracleDbType.RefCursor;
                pEscHistory.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscHistory);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "EscHistory";
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
