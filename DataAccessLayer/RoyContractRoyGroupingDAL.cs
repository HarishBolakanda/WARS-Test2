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
    public class RoyContractRoyGroupingDAL : IRoyContractRoyGroupingDAL
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

        public DataSet GetInitialData(Int32 royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pRoyaltorGroupingData = new OracleParameter();                
                OracleParameter pAccountCompanyList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_roy_grouping.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Int32;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pRoyaltorGroupingData.OracleDbType = OracleDbType.RefCursor;
                pRoyaltorGroupingData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoyaltorGroupingData);

                pAccountCompanyList.OracleDbType = OracleDbType.RefCursor;
                pAccountCompanyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pAccountCompanyList);

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
        
        public DataSet UpdateRoyaltorGrouping(Int32 royaltorId, string summaryMasterRoyaltor, string txtMasterRoyaltor, string accrualRoyaltor, string dspAnalyticsRoyaltor, string gfsLabel, string gfsCompany, string printGroup, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pSummaryMasterRoyaltor = new OracleParameter();
                OracleParameter pTxtMasterRoyaltor = new OracleParameter();
                OracleParameter pAccrualRoyaltor = new OracleParameter();
                OracleParameter pDspAnalyticsRoyaltor = new OracleParameter();
                OracleParameter pGfsLabel = new OracleParameter();
                OracleParameter pGfsCompany = new OracleParameter();
                OracleParameter pPrintGroup = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pRoyaltorGroupingData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_roy_grouping.p_update_royaltor_grouping", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Int32;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pSummaryMasterRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pSummaryMasterRoyaltor.Direction = ParameterDirection.Input;
                pSummaryMasterRoyaltor.Value = summaryMasterRoyaltor;
                orlCmd.Parameters.Add(pSummaryMasterRoyaltor);

                pTxtMasterRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pTxtMasterRoyaltor.Direction = ParameterDirection.Input;
                pTxtMasterRoyaltor.Value = txtMasterRoyaltor;
                orlCmd.Parameters.Add(pTxtMasterRoyaltor);

                pAccrualRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pAccrualRoyaltor.Direction = ParameterDirection.Input;
                pAccrualRoyaltor.Value = accrualRoyaltor;
                orlCmd.Parameters.Add(pAccrualRoyaltor);

                pDspAnalyticsRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pDspAnalyticsRoyaltor.Direction = ParameterDirection.Input;
                pDspAnalyticsRoyaltor.Value = dspAnalyticsRoyaltor;
                orlCmd.Parameters.Add(pDspAnalyticsRoyaltor);

                pGfsLabel.OracleDbType = OracleDbType.Varchar2;
                pGfsLabel.Direction = ParameterDirection.Input;
                pGfsLabel.Value = gfsLabel;
                orlCmd.Parameters.Add(pGfsLabel);

                pGfsCompany.OracleDbType = OracleDbType.Varchar2;
                pGfsCompany.Direction = ParameterDirection.Input;
                pGfsCompany.Value = gfsCompany;
                orlCmd.Parameters.Add(pGfsCompany);

                pPrintGroup.OracleDbType = OracleDbType.Varchar2;
                pPrintGroup.Direction = ParameterDirection.Input;
                pPrintGroup.Value = printGroup;
                orlCmd.Parameters.Add(pPrintGroup);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pRoyaltorGroupingData.OracleDbType = OracleDbType.RefCursor;
                pRoyaltorGroupingData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoyaltorGroupingData);

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
