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
    public class PaymentApprovalDAL : IPaymentApprovalDAL
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

        public DataSet GetInitialLoadData(string userAccountId, out string userRoleApprovalLevel, out Int32 iErrorId)
        {
            ds = new DataSet();
            userRoleApprovalLevel = string.Empty;
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                OracleParameter pUserAccountId = new OracleParameter();
                OracleParameter pUserRoleApprovalLevel = new OracleParameter();               
                OracleParameter pCompanyList = new OracleParameter();
                OracleParameter pStmtStatusList = new OracleParameter();
                OracleParameter pPayeeStatusList = new OracleParameter();
                OracleParameter pPaymentStatusList = new OracleParameter();                
                OracleParameter pResponsibilityList = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_payment_approval.p_get_intial_load_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pUserAccountId.OracleDbType = OracleDbType.Varchar2;
                pUserAccountId.Direction = ParameterDirection.Input;
                pUserAccountId.Value = userAccountId;
                orlCmd.Parameters.Add(pUserAccountId);

                pUserRoleApprovalLevel.OracleDbType = OracleDbType.Varchar2;
                pUserRoleApprovalLevel.Size = 10;
                pUserRoleApprovalLevel.Direction = System.Data.ParameterDirection.Output;
                pUserRoleApprovalLevel.ParameterName = "pUserRoleApprovalLevel";
                orlCmd.Parameters.Add(pUserRoleApprovalLevel);

                pCompanyList.OracleDbType = OracleDbType.RefCursor;
                pCompanyList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCompanyList);

                pStmtStatusList.OracleDbType = OracleDbType.RefCursor;
                pStmtStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtStatusList);

                pPayeeStatusList.OracleDbType = OracleDbType.RefCursor;
                pPayeeStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeStatusList);

                pPaymentStatusList.OracleDbType = OracleDbType.RefCursor;
                pPaymentStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPaymentStatusList);

                pResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                pResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pResponsibilityList);
              
                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                userRoleApprovalLevel = orlCmd.Parameters["pUserRoleApprovalLevel"].Value.ToString();                             
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

        public DataSet GetSearchData(string companyCode,string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                               string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                OracleParameter pCompanyCode = new OracleParameter();
                OracleParameter pStmtEndPeriod = new OracleParameter();
                OracleParameter pReportedDays = new OracleParameter();
                OracleParameter pStmtStatus = new OracleParameter();
                OracleParameter pPayeeStatus = new OracleParameter();
                OracleParameter pPaymentStatus = new OracleParameter();
                OracleParameter pOwnerCode = new OracleParameter();
                OracleParameter pIntPartyId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pBalThreshold = new OracleParameter();
                OracleParameter pRespCode = new OracleParameter();
                OracleParameter pPayments = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_payment_approval.p_get_search_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCompanyCode.OracleDbType = OracleDbType.Varchar2;
                pCompanyCode.Direction = ParameterDirection.Input;
                pCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(pCompanyCode);

                pStmtEndPeriod.OracleDbType = OracleDbType.Varchar2;
                pStmtEndPeriod.Direction = ParameterDirection.Input;
                pStmtEndPeriod.Value = stmtEndPeriod;
                orlCmd.Parameters.Add(pStmtEndPeriod);

                pReportedDays.OracleDbType = OracleDbType.Varchar2;
                pReportedDays.Direction = ParameterDirection.Input;
                pReportedDays.Value = reportedDays;
                orlCmd.Parameters.Add(pReportedDays);

                pStmtStatus.OracleDbType = OracleDbType.Varchar2;
                pStmtStatus.Direction = ParameterDirection.Input;
                pStmtStatus.Value = stmtStatus;
                orlCmd.Parameters.Add(pStmtStatus);

                pPayeeStatus.OracleDbType = OracleDbType.Varchar2;
                pPayeeStatus.Direction = ParameterDirection.Input;
                pPayeeStatus.Value = payeeStatus;
                orlCmd.Parameters.Add(pPayeeStatus);

                pPaymentStatus.OracleDbType = OracleDbType.Varchar2;
                pPaymentStatus.Direction = ParameterDirection.Input;
                pPaymentStatus.Value = paymentStatus;
                orlCmd.Parameters.Add(pPaymentStatus);

                pOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pOwnerCode.Direction = ParameterDirection.Input;
                pOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(pOwnerCode);

                pIntPartyId.OracleDbType = OracleDbType.Varchar2;
                pIntPartyId.Direction = ParameterDirection.Input;
                pIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(pIntPartyId);

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pBalThreshold.OracleDbType = OracleDbType.Varchar2;
                pBalThreshold.Direction = ParameterDirection.Input;
                pBalThreshold.Value = balThreshold;
                orlCmd.Parameters.Add(pBalThreshold);

                pRespCode.OracleDbType = OracleDbType.Varchar2;
                pRespCode.Direction = ParameterDirection.Input;
                pRespCode.Value = respCode;
                orlCmd.Parameters.Add(pRespCode);

                pPayments.OracleDbType = OracleDbType.RefCursor;
                pPayments.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayments);

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

        public DataSet SavePaymentApproval(Array paymentList, string userCode, string companyCode, string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                                    string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_payment_approval.p_save_payment_approval", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                OracleParameter pPaymentList = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pCompanyCode = new OracleParameter();
                OracleParameter pStmtEndPeriod = new OracleParameter();
                OracleParameter pReportedDays = new OracleParameter();
                OracleParameter pStmtStatus = new OracleParameter();
                OracleParameter pPayeeStatus = new OracleParameter();
                OracleParameter pPaymentStatus = new OracleParameter();
                OracleParameter pOwnerCode = new OracleParameter();
                OracleParameter pIntPartyId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pBalThreshold = new OracleParameter();
                OracleParameter pRespCode = new OracleParameter();
                OracleParameter pPayments = new OracleParameter();

                pPaymentList.OracleDbType = OracleDbType.Varchar2;
                pPaymentList.Direction = ParameterDirection.Input;
                pPaymentList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (paymentList.Length == 0)
                {
                    pPaymentList.Size = 1;
                    pPaymentList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pPaymentList.Size = paymentList.Length;
                    pPaymentList.Value = paymentList;
                }
                orlCmd.Parameters.Add(pPaymentList);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pCompanyCode.OracleDbType = OracleDbType.Varchar2;
                pCompanyCode.Direction = ParameterDirection.Input;
                pCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(pCompanyCode);

                pStmtEndPeriod.OracleDbType = OracleDbType.Varchar2;
                pStmtEndPeriod.Direction = ParameterDirection.Input;
                pStmtEndPeriod.Value = stmtEndPeriod;
                orlCmd.Parameters.Add(pStmtEndPeriod);

                pReportedDays.OracleDbType = OracleDbType.Varchar2;
                pReportedDays.Direction = ParameterDirection.Input;
                pReportedDays.Value = reportedDays;
                orlCmd.Parameters.Add(pReportedDays);

                pStmtStatus.OracleDbType = OracleDbType.Varchar2;
                pStmtStatus.Direction = ParameterDirection.Input;
                pStmtStatus.Value = stmtStatus;
                orlCmd.Parameters.Add(pStmtStatus);

                pPayeeStatus.OracleDbType = OracleDbType.Varchar2;
                pPayeeStatus.Direction = ParameterDirection.Input;
                pPayeeStatus.Value = payeeStatus;
                orlCmd.Parameters.Add(pPayeeStatus);

                pPaymentStatus.OracleDbType = OracleDbType.Varchar2;
                pPaymentStatus.Direction = ParameterDirection.Input;
                pPaymentStatus.Value = paymentStatus;
                orlCmd.Parameters.Add(pPaymentStatus);

                pOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pOwnerCode.Direction = ParameterDirection.Input;
                pOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(pOwnerCode);

                pIntPartyId.OracleDbType = OracleDbType.Varchar2;
                pIntPartyId.Direction = ParameterDirection.Input;
                pIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(pIntPartyId);

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pBalThreshold.OracleDbType = OracleDbType.Varchar2;
                pBalThreshold.Direction = ParameterDirection.Input;
                pBalThreshold.Value = balThreshold;
                orlCmd.Parameters.Add(pBalThreshold);

                pRespCode.OracleDbType = OracleDbType.Varchar2;
                pRespCode.Direction = ParameterDirection.Input;
                pRespCode.Value = respCode;
                orlCmd.Parameters.Add(pRespCode);

                pPayments.OracleDbType = OracleDbType.RefCursor;
                pPayments.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayments);                

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

        public DataSet UpdateInvoices(string userCode, string companyCode, string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                                    string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId)

        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_payment_approval.p_update_invoices", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;                
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pCompanyCode = new OracleParameter();
                OracleParameter pStmtEndPeriod = new OracleParameter();
                OracleParameter pReportedDays = new OracleParameter();
                OracleParameter pStmtStatus = new OracleParameter();
                OracleParameter pPayeeStatus = new OracleParameter();
                OracleParameter pPaymentStatus = new OracleParameter();
                OracleParameter pOwnerCode = new OracleParameter();
                OracleParameter pIntPartyId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pBalThreshold = new OracleParameter();
                OracleParameter pRespCode = new OracleParameter();
                OracleParameter pPayments = new OracleParameter();

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pCompanyCode.OracleDbType = OracleDbType.Varchar2;
                pCompanyCode.Direction = ParameterDirection.Input;
                pCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(pCompanyCode);

                pStmtEndPeriod.OracleDbType = OracleDbType.Varchar2;
                pStmtEndPeriod.Direction = ParameterDirection.Input;
                pStmtEndPeriod.Value = stmtEndPeriod;
                orlCmd.Parameters.Add(pStmtEndPeriod);

                pReportedDays.OracleDbType = OracleDbType.Varchar2;
                pReportedDays.Direction = ParameterDirection.Input;
                pReportedDays.Value = reportedDays;
                orlCmd.Parameters.Add(pReportedDays);

                pStmtStatus.OracleDbType = OracleDbType.Varchar2;
                pStmtStatus.Direction = ParameterDirection.Input;
                pStmtStatus.Value = stmtStatus;
                orlCmd.Parameters.Add(pStmtStatus);

                pPayeeStatus.OracleDbType = OracleDbType.Varchar2;
                pPayeeStatus.Direction = ParameterDirection.Input;
                pPayeeStatus.Value = payeeStatus;
                orlCmd.Parameters.Add(pPayeeStatus);

                pPaymentStatus.OracleDbType = OracleDbType.Varchar2;
                pPaymentStatus.Direction = ParameterDirection.Input;
                pPaymentStatus.Value = paymentStatus;
                orlCmd.Parameters.Add(pPaymentStatus);

                pOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pOwnerCode.Direction = ParameterDirection.Input;
                pOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(pOwnerCode);

                pIntPartyId.OracleDbType = OracleDbType.Varchar2;
                pIntPartyId.Direction = ParameterDirection.Input;
                pIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(pIntPartyId);

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pBalThreshold.OracleDbType = OracleDbType.Varchar2;
                pBalThreshold.Direction = ParameterDirection.Input;
                pBalThreshold.Value = balThreshold;
                orlCmd.Parameters.Add(pBalThreshold);

                pRespCode.OracleDbType = OracleDbType.Varchar2;
                pRespCode.Direction = ParameterDirection.Input;
                pRespCode.Value = respCode;
                orlCmd.Parameters.Add(pRespCode);

                pPayments.OracleDbType = OracleDbType.RefCursor;
                pPayments.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayments);

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
