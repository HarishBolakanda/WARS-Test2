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
    public class WorkFlowDAL: IWorkFlowDAL
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


        public DataSet GetDropDownData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();
                                
                OracleParameter cur_company = new OracleParameter();
                OracleParameter cur_statement_period = new OracleParameter();
                OracleParameter cur_owner = new OracleParameter();
                OracleParameter cur_royaltor = new OracleParameter();
                OracleParameter cur_status = new OracleParameter();
                OracleParameter cur_TeamResponsibility = new OracleParameter();
                OracleParameter cur_priority = new OracleParameter();
                OracleParameter cur_status_visibility = new OracleParameter();
                OracleParameter cur_MngrResponsibility = new OracleParameter();
                                
                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_dropdownlist_values", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                cur_company.OracleDbType = OracleDbType.RefCursor;
                cur_company.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_company);

                cur_statement_period.OracleDbType = OracleDbType.RefCursor;
                cur_statement_period.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_statement_period);
                
                cur_owner.OracleDbType = OracleDbType.RefCursor;
                cur_owner.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_owner);

                cur_royaltor.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor);

                cur_status.OracleDbType = OracleDbType.RefCursor;
                cur_status.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_status);

                cur_TeamResponsibility.OracleDbType = OracleDbType.RefCursor;
                cur_TeamResponsibility.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_TeamResponsibility);

                cur_priority.OracleDbType = OracleDbType.RefCursor;
                cur_priority.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_priority);

                cur_status_visibility.OracleDbType = OracleDbType.RefCursor;
                cur_status_visibility.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_status_visibility);

                cur_MngrResponsibility.OracleDbType = OracleDbType.RefCursor;
                cur_MngrResponsibility.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_MngrResponsibility);

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

        public DataSet GetFiltersForResp(string teamRespCode, string mngrRespCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter oraTeamRespCode = new OracleParameter();
                OracleParameter oraMngrRespCode = new OracleParameter();
                OracleParameter cur_owner = new OracleParameter();
                OracleParameter cur_royaltor = new OracleParameter();

                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_filters_for_resp", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                oraTeamRespCode.OracleDbType = OracleDbType.Varchar2;
                oraTeamRespCode.Direction = ParameterDirection.Input;
                oraTeamRespCode.Value = teamRespCode;
                orlCmd.Parameters.Add(oraTeamRespCode);

                oraMngrRespCode.OracleDbType = OracleDbType.Varchar2;
                oraMngrRespCode.Direction = ParameterDirection.Input;
                oraMngrRespCode.Value = mngrRespCode;
                orlCmd.Parameters.Add(oraMngrRespCode);
                
                cur_owner.OracleDbType = OracleDbType.RefCursor;
                cur_owner.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_owner);

                cur_royaltor.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor);

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

        public DataSet GetWorkflowSearchData(string compCode, string rep_schedule, string owner, string royaltor, string teamResponsibility, string mngrResponsibility, string priority, string producer, string status,
                                             string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                OracleParameter cur_activity_data = new OracleParameter();
                OracleParameter company_code = new OracleParameter();
                OracleParameter statement_period_id = new OracleParameter();
                OracleParameter owner_code = new OracleParameter();
                OracleParameter royaltor_id = new OracleParameter();
                OracleParameter teamResponsibility_code = new OracleParameter();
                OracleParameter mngrResponsibility_code = new OracleParameter();
                OracleParameter priority_code = new OracleParameter();
                OracleParameter producer_text = new OracleParameter();
                OracleParameter stmt_status_code = new OracleParameter();
                OracleParameter pEarnings = new OracleParameter();                
                OracleParameter pEarningsCompare = new OracleParameter();
                OracleParameter pClosingBalance = new OracleParameter();
                OracleParameter pClosingBalanceCompare = new OracleParameter();
                
                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_workflow_search_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                company_code.OracleDbType = OracleDbType.Varchar2;
                company_code.Direction = ParameterDirection.Input;
                company_code.Value = compCode;
                orlCmd.Parameters.Add(company_code);

                statement_period_id.OracleDbType = OracleDbType.Varchar2;
                statement_period_id.Direction = ParameterDirection.Input;
                statement_period_id.Value = rep_schedule;
                orlCmd.Parameters.Add(statement_period_id);

                owner_code.OracleDbType = OracleDbType.Varchar2;
                owner_code.Direction = ParameterDirection.Input;
                owner_code.Value = (owner == "" ? "-" : owner);
                orlCmd.Parameters.Add(owner_code);

                royaltor_id.OracleDbType = OracleDbType.Varchar2;
                royaltor_id.Direction = ParameterDirection.Input;
                royaltor_id.Value = (royaltor == "" ? "-" : royaltor);
                orlCmd.Parameters.Add(royaltor_id);

                teamResponsibility_code.OracleDbType = OracleDbType.Varchar2;
                teamResponsibility_code.Direction = ParameterDirection.Input;
                teamResponsibility_code.Value = teamResponsibility;
                orlCmd.Parameters.Add(teamResponsibility_code);

                mngrResponsibility_code.OracleDbType = OracleDbType.Varchar2;
                mngrResponsibility_code.Direction = ParameterDirection.Input;
                mngrResponsibility_code.Value = mngrResponsibility;
                orlCmd.Parameters.Add(mngrResponsibility_code);

                priority_code.OracleDbType = OracleDbType.Varchar2;
                priority_code.Direction = ParameterDirection.Input;
                priority_code.Value = priority;
                orlCmd.Parameters.Add(priority_code);

                producer_text.OracleDbType = OracleDbType.Varchar2;
                producer_text.Direction = ParameterDirection.Input;
                producer_text.Value = (producer == "" ? "-" : producer);
                orlCmd.Parameters.Add(producer_text);

                stmt_status_code.OracleDbType = OracleDbType.Varchar2;
                stmt_status_code.Direction = ParameterDirection.Input;
                stmt_status_code.Value = status;
                orlCmd.Parameters.Add(stmt_status_code);

                pEarnings.OracleDbType = OracleDbType.Varchar2;
                pEarnings.Direction = ParameterDirection.Input;
                pEarnings.Value = earnings;
                orlCmd.Parameters.Add(pEarnings);

                pEarningsCompare.OracleDbType = OracleDbType.Varchar2;
                pEarningsCompare.Direction = ParameterDirection.Input;
                pEarningsCompare.Value = earningsCompare;
                orlCmd.Parameters.Add(pEarningsCompare);

                pClosingBalance.OracleDbType = OracleDbType.Varchar2;
                pClosingBalance.Direction = ParameterDirection.Input;
                pClosingBalance.Value = closingBalance;
                orlCmd.Parameters.Add(pClosingBalance);

                pClosingBalanceCompare.OracleDbType = OracleDbType.Varchar2;
                pClosingBalanceCompare.Direction = ParameterDirection.Input;
                pClosingBalanceCompare.Value = closingBalanceCompare;
                orlCmd.Parameters.Add(pClosingBalanceCompare);
                                                              
                cur_activity_data.OracleDbType = OracleDbType.RefCursor;
                cur_activity_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_activity_data);

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

        public DataSet UpdateWorkflowStatus(string pageMode, string status, Array stmtRowsToUpdate, string compCode, string repSchedule,
                                            string owner, string royaltor, string teamResponsibility, string mngrResponsibility, string priority, string producer, string statusSearch,
                                            string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, string loggedUser, out Int32 iErrorId)                                            
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();

                OracleParameter pPageMode = new OracleParameter();
                OracleParameter pStatus = new OracleParameter();
                OracleParameter pStmtRowsToUpdate = new OracleParameter();
                OracleParameter pCompCodeSearch = new OracleParameter();
                OracleParameter pRepScheduleSearch = new OracleParameter();
                OracleParameter pOwnerSearch = new OracleParameter();
                OracleParameter pRoyaltorSearch = new OracleParameter();
                OracleParameter pTeamResponsibilitySearch = new OracleParameter();
                OracleParameter pMngrResponsibilitySearch = new OracleParameter();
                OracleParameter pPrioritySearch = new OracleParameter();
                OracleParameter pProducerSearch = new OracleParameter();
                OracleParameter pStatusSearch = new OracleParameter();
                OracleParameter pEarningsSearch = new OracleParameter();
                OracleParameter pEarningsCompareSearch = new OracleParameter();
                OracleParameter pClosingBalanceSearch = new OracleParameter();
                OracleParameter pClosingBalanceCompareSearch = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pCurActivityData = new OracleParameter();                

                orlCmd = new OracleCommand("pkg_workflow_screen.p_update_workflow_status", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pPageMode.OracleDbType = OracleDbType.Varchar2;
                pPageMode.Direction = ParameterDirection.Input;
                if (pageMode == "")
                    pPageMode.Value = DBNull.Value;
                else
                    pPageMode.Value = pageMode;
                orlCmd.Parameters.Add(pPageMode);

                pStatus.OracleDbType = OracleDbType.Varchar2;
                pStatus.Direction = ParameterDirection.Input;
                pStatus.Value = status;
                orlCmd.Parameters.Add(pStatus);

                pStmtRowsToUpdate.OracleDbType = OracleDbType.Varchar2;
                pStmtRowsToUpdate.Direction = ParameterDirection.Input;
                pStmtRowsToUpdate.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtRowsToUpdate.Length == 0)
                {
                    pStmtRowsToUpdate.Size = 1;
                    pStmtRowsToUpdate.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pStmtRowsToUpdate.Size = stmtRowsToUpdate.Length;
                    pStmtRowsToUpdate.Value = stmtRowsToUpdate;
                }
                orlCmd.Parameters.Add(pStmtRowsToUpdate);

                pCompCodeSearch.OracleDbType = OracleDbType.Varchar2;
                pCompCodeSearch.Direction = ParameterDirection.Input;
                pCompCodeSearch.Value = compCode;
                orlCmd.Parameters.Add(pCompCodeSearch);

                pRepScheduleSearch.OracleDbType = OracleDbType.Varchar2;
                pRepScheduleSearch.Direction = ParameterDirection.Input;
                pRepScheduleSearch.Value = repSchedule;
                orlCmd.Parameters.Add(pRepScheduleSearch);

                pOwnerSearch.OracleDbType = OracleDbType.Varchar2;
                pOwnerSearch.Direction = ParameterDirection.Input;
                pOwnerSearch.Value = (owner == "" ? "-" : owner);
                orlCmd.Parameters.Add(pOwnerSearch);

                pRoyaltorSearch.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorSearch.Direction = ParameterDirection.Input;
                pRoyaltorSearch.Value = (royaltor == "" ? "-" : royaltor);
                orlCmd.Parameters.Add(pRoyaltorSearch);

                pTeamResponsibilitySearch.OracleDbType = OracleDbType.Varchar2;
                pTeamResponsibilitySearch.Direction = ParameterDirection.Input;
                pTeamResponsibilitySearch.Value = teamResponsibility;
                orlCmd.Parameters.Add(pTeamResponsibilitySearch);

                pMngrResponsibilitySearch.OracleDbType = OracleDbType.Varchar2;
                pMngrResponsibilitySearch.Direction = ParameterDirection.Input;
                pMngrResponsibilitySearch.Value = mngrResponsibility;
                orlCmd.Parameters.Add(pMngrResponsibilitySearch);

                pPrioritySearch.OracleDbType = OracleDbType.Varchar2;
                pPrioritySearch.Direction = ParameterDirection.Input;
                pPrioritySearch.Value = priority;
                orlCmd.Parameters.Add(pPrioritySearch);

                pProducerSearch.OracleDbType = OracleDbType.Varchar2;
                pProducerSearch.Direction = ParameterDirection.Input;
                pProducerSearch.Value = (producer == "" ? "-" : producer);
                orlCmd.Parameters.Add(pProducerSearch);

                pStatusSearch.OracleDbType = OracleDbType.Varchar2;
                pStatusSearch.Direction = ParameterDirection.Input;
                pStatusSearch.Value = statusSearch;
                orlCmd.Parameters.Add(pStatusSearch);

                pEarningsSearch.OracleDbType = OracleDbType.Varchar2;
                pEarningsSearch.Direction = ParameterDirection.Input;
                pEarningsSearch.Value = earnings;
                orlCmd.Parameters.Add(pEarningsSearch);

                pEarningsCompareSearch.OracleDbType = OracleDbType.Varchar2;
                pEarningsCompareSearch.Direction = ParameterDirection.Input;
                pEarningsCompareSearch.Value = earningsCompare;
                orlCmd.Parameters.Add(pEarningsCompareSearch);

                pClosingBalanceSearch.OracleDbType = OracleDbType.Varchar2;
                pClosingBalanceSearch.Direction = ParameterDirection.Input;
                pClosingBalanceSearch.Value = closingBalance;
                orlCmd.Parameters.Add(pClosingBalanceSearch);

                pClosingBalanceCompareSearch.OracleDbType = OracleDbType.Varchar2;
                pClosingBalanceCompareSearch.Direction = ParameterDirection.Input;
                pClosingBalanceCompareSearch.Value = closingBalanceCompare;
                orlCmd.Parameters.Add(pClosingBalanceCompareSearch);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pCurActivityData.OracleDbType = OracleDbType.RefCursor;
                pCurActivityData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCurActivityData);

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

        public DataSet GetComment(string royaltorId, string stmtPeriodId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pStmtPeriodId = new OracleParameter();
                OracleParameter pCurComment = new OracleParameter();

                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_comment", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pStmtPeriodId.OracleDbType = OracleDbType.Varchar2;
                pStmtPeriodId.Direction = ParameterDirection.Input;
                pStmtPeriodId.Value = stmtPeriodId;
                orlCmd.Parameters.Add(pStmtPeriodId);

                pCurComment.OracleDbType = OracleDbType.RefCursor;
                pCurComment.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCurComment);

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
        
        public DataSet SaveComment(string royaltorID, string stmtPeriodId, string comment, string saveDelete, string compCodeSearch, string repScheduleSearch, string ownerSearch, string royaltorSearch,
                                   string teamResponsibility, string mngrResponsibility, string prioritySearch, string producerSearch, string statusSearch,
                                   string earnings, string earningsCompare, string closingBalance, string closingBalanceCompare, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pStmtPeriodId = new OracleParameter();
                OracleParameter pComment = new OracleParameter();
                OracleParameter pSaveDelete = new OracleParameter();
                OracleParameter pCompCodeSearch = new OracleParameter();
                OracleParameter pRepScheduleSearch = new OracleParameter();
                OracleParameter pOwnerSearch = new OracleParameter();
                OracleParameter pRoyaltorSearch = new OracleParameter();
                OracleParameter pTeamResponsibilitySearch = new OracleParameter();
                OracleParameter pMngrResponsibilitySearch = new OracleParameter();
                OracleParameter pPrioritySearch = new OracleParameter();
                OracleParameter pProducerSearch = new OracleParameter();
                OracleParameter pStatusSearch = new OracleParameter();
                OracleParameter pEarnings = new OracleParameter();
                OracleParameter pEarningsCompare = new OracleParameter();
                OracleParameter pClosingBalance = new OracleParameter();
                OracleParameter pClosingBalanceCompare = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pCurWorkFlowData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_workflow_screen.p_save_comment", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorID;
                orlCmd.Parameters.Add(pRoyaltorId);

                pStmtPeriodId.OracleDbType = OracleDbType.Varchar2;
                pStmtPeriodId.Direction = ParameterDirection.Input;
                pStmtPeriodId.Value = stmtPeriodId;
                orlCmd.Parameters.Add(pStmtPeriodId);

                pComment.OracleDbType = OracleDbType.Varchar2;
                pComment.Direction = ParameterDirection.Input;
                pComment.Value = comment;
                orlCmd.Parameters.Add(pComment);

                pSaveDelete.OracleDbType = OracleDbType.Varchar2;
                pSaveDelete.Direction = ParameterDirection.Input;
                pSaveDelete.Value = saveDelete;
                orlCmd.Parameters.Add(pSaveDelete);

                pCompCodeSearch.OracleDbType = OracleDbType.Varchar2;
                pCompCodeSearch.Direction = ParameterDirection.Input;
                pCompCodeSearch.Value = compCodeSearch;
                orlCmd.Parameters.Add(pCompCodeSearch);

                pRepScheduleSearch.OracleDbType = OracleDbType.Varchar2;
                pRepScheduleSearch.Direction = ParameterDirection.Input;
                pRepScheduleSearch.Value = repScheduleSearch;
                orlCmd.Parameters.Add(pRepScheduleSearch);

                pOwnerSearch.OracleDbType = OracleDbType.Varchar2;
                pOwnerSearch.Direction = ParameterDirection.Input;
                pOwnerSearch.Value = (ownerSearch == "" ? "-" : ownerSearch);
                orlCmd.Parameters.Add(pOwnerSearch);

                pRoyaltorSearch.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorSearch.Direction = ParameterDirection.Input;
                pRoyaltorSearch.Value = (royaltorSearch == "" ? "-" : royaltorSearch);
                orlCmd.Parameters.Add(pRoyaltorSearch);

                pTeamResponsibilitySearch.OracleDbType = OracleDbType.Varchar2;
                pTeamResponsibilitySearch.Direction = ParameterDirection.Input;
                pTeamResponsibilitySearch.Value = teamResponsibility;
                orlCmd.Parameters.Add(pTeamResponsibilitySearch);

                pMngrResponsibilitySearch.OracleDbType = OracleDbType.Varchar2;
                pMngrResponsibilitySearch.Direction = ParameterDirection.Input;
                pMngrResponsibilitySearch.Value = mngrResponsibility;
                orlCmd.Parameters.Add(pMngrResponsibilitySearch);

                pPrioritySearch.OracleDbType = OracleDbType.Varchar2;
                pPrioritySearch.Direction = ParameterDirection.Input;
                pPrioritySearch.Value = prioritySearch;
                orlCmd.Parameters.Add(pPrioritySearch);

                pProducerSearch.OracleDbType = OracleDbType.Varchar2;
                pProducerSearch.Direction = ParameterDirection.Input;
                pProducerSearch.Value = (producerSearch == "" ? "-" : producerSearch);
                orlCmd.Parameters.Add(pProducerSearch);

                pStatusSearch.OracleDbType = OracleDbType.Varchar2;
                pStatusSearch.Direction = ParameterDirection.Input;
                pStatusSearch.Value = statusSearch;
                orlCmd.Parameters.Add(pStatusSearch);

                pEarnings.OracleDbType = OracleDbType.Varchar2;
                pEarnings.Direction = ParameterDirection.Input;
                pEarnings.Value = earnings;
                orlCmd.Parameters.Add(pEarnings);

                pEarningsCompare.OracleDbType = OracleDbType.Varchar2;
                pEarningsCompare.Direction = ParameterDirection.Input;
                pEarningsCompare.Value = earningsCompare;
                orlCmd.Parameters.Add(pEarningsCompare);

                pClosingBalance.OracleDbType = OracleDbType.Varchar2;
                pClosingBalance.Direction = ParameterDirection.Input;
                pClosingBalance.Value = closingBalance;
                orlCmd.Parameters.Add(pClosingBalance);

                pClosingBalanceCompare.OracleDbType = OracleDbType.Varchar2;
                pClosingBalanceCompare.Direction = ParameterDirection.Input;
                pClosingBalanceCompare.Value = closingBalanceCompare;
                orlCmd.Parameters.Add(pClosingBalanceCompare);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pCurWorkFlowData.OracleDbType = OracleDbType.RefCursor;
                pCurWorkFlowData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCurWorkFlowData);

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
        
        public void UpdateRecreateStat(Array rowsToUpdate, string loggedUser, out Int32 iErrorId)
        {
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();

                OracleParameter oraRowsToUpdate = new OracleParameter();
                OracleParameter oraLoggedUser = new OracleParameter();

                orlCmd = new OracleCommand("pkg_workflow_screen.p_update_recreate_stat", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                oraRowsToUpdate.OracleDbType = OracleDbType.Varchar2;
                oraRowsToUpdate.Direction = ParameterDirection.Input;
                oraRowsToUpdate.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (rowsToUpdate.Length == 0)
                {
                    oraRowsToUpdate.Size = 1;
                    oraRowsToUpdate.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    oraRowsToUpdate.Size = rowsToUpdate.Length;
                    oraRowsToUpdate.Value = rowsToUpdate;
                }
                orlCmd.Parameters.Add(oraRowsToUpdate);

                oraLoggedUser.OracleDbType = OracleDbType.Varchar2;
                oraLoggedUser.Direction = ParameterDirection.Input;
                oraLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(oraLoggedUser);

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

        public void UpdateSummaryStmtDetails(string royaltorId, string stmtPeriodId, out string createFrontSheet, out string masterGroupedRoyaltor, out string summaryMasterRotaltor,
                                            out string stmtPeriodSortCode, out string createInvoice, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter royaltor_id = new OracleParameter();
                OracleParameter stmt_period_id = new OracleParameter();
                OracleParameter oraCreateFrontSheet = new OracleParameter();
                OracleParameter oraMasterGroupedRoyaltor = new OracleParameter();
                OracleParameter oraSummaryMasterRotaltor = new OracleParameter();
                OracleParameter oraStmtPeriodSortCode = new OracleParameter();
                OracleParameter oraCreateInvoice = new OracleParameter();
                orlCmd = new OracleCommand("pkg_workflow_screen.p_update_summary_stmt_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                royaltor_id.OracleDbType = OracleDbType.Varchar2;
                royaltor_id.Direction = ParameterDirection.Input;
                royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(royaltor_id);

                stmt_period_id.OracleDbType = OracleDbType.Varchar2;
                stmt_period_id.Direction = ParameterDirection.Input;
                stmt_period_id.Value = stmtPeriodId;
                orlCmd.Parameters.Add(stmt_period_id);

                oraCreateFrontSheet.OracleDbType = OracleDbType.Varchar2;
                oraCreateFrontSheet.Size = 200;
                oraCreateFrontSheet.Direction = System.Data.ParameterDirection.Output;
                oraCreateFrontSheet.ParameterName = "oraCreateFrontSheet";
                orlCmd.Parameters.Add(oraCreateFrontSheet);

                oraMasterGroupedRoyaltor.OracleDbType = OracleDbType.Varchar2;
                oraMasterGroupedRoyaltor.Size = 200;
                oraMasterGroupedRoyaltor.Direction = System.Data.ParameterDirection.Output;
                oraMasterGroupedRoyaltor.ParameterName = "masterGroupedRoyaltor";
                orlCmd.Parameters.Add(oraMasterGroupedRoyaltor);

                oraSummaryMasterRotaltor.OracleDbType = OracleDbType.Varchar2;
                oraSummaryMasterRotaltor.Size = 200;
                oraSummaryMasterRotaltor.Direction = System.Data.ParameterDirection.Output;
                oraSummaryMasterRotaltor.ParameterName = "summaryMasterRotaltor";
                orlCmd.Parameters.Add(oraSummaryMasterRotaltor);

                oraStmtPeriodSortCode.OracleDbType = OracleDbType.Varchar2;
                oraStmtPeriodSortCode.Size = 200;
                oraStmtPeriodSortCode.Direction = System.Data.ParameterDirection.Output;
                oraStmtPeriodSortCode.ParameterName = "stmtPeriodSortCode";
                orlCmd.Parameters.Add(oraStmtPeriodSortCode);

                oraCreateInvoice.OracleDbType = OracleDbType.Varchar2;
                oraCreateInvoice.Size = 200;
                oraCreateInvoice.Direction = System.Data.ParameterDirection.Output;
                oraCreateInvoice.ParameterName = "oraCreateInvoice";
                orlCmd.Parameters.Add(oraCreateInvoice);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                createFrontSheet = orlCmd.Parameters["oraCreateFrontSheet"].Value.ToString();
                masterGroupedRoyaltor = orlCmd.Parameters["masterGroupedRoyaltor"].Value.ToString();
                summaryMasterRotaltor = orlCmd.Parameters["summaryMasterRotaltor"].Value.ToString();
                stmtPeriodSortCode = orlCmd.Parameters["stmtPeriodSortCode"].Value.ToString();
                createInvoice = orlCmd.Parameters["oraCreateInvoice"].Value.ToString();
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

        public DataSet GetIntPartyIds(string royaltorID, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter royaltor_id = new OracleParameter();
                OracleParameter curIntpartyIds = new OracleParameter();

                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_intparty_ids", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                royaltor_id.OracleDbType = OracleDbType.Varchar2;
                royaltor_id.Direction = ParameterDirection.Input;
                royaltor_id.Value = royaltorID;
                orlCmd.Parameters.Add(royaltor_id);

                curIntpartyIds.OracleDbType = OracleDbType.RefCursor;
                curIntpartyIds.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curIntpartyIds);

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

        public void GetStmtDirectory(out string stmtDirectory, out Int32 iErrorId)
        {
            stmtDirectory = string.Empty;

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();                
                orlCmd = new OracleCommand();
                OracleParameter oraStmtDirectory = new OracleParameter();

                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_stmt_directory", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                oraStmtDirectory.OracleDbType = OracleDbType.Varchar2;
                oraStmtDirectory.Size = 200;
                oraStmtDirectory.Direction = System.Data.ParameterDirection.Output;
                oraStmtDirectory.ParameterName = "oraStmtDirectory";
                orlCmd.Parameters.Add(oraStmtDirectory);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                stmtDirectory = orlCmd.Parameters["oraStmtDirectory"].Value.ToString();
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

        public void RecalSummaryBulk(Array royaltorStmts, string loggedUser, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlCmd = new OracleCommand();
                OracleParameter pRoyaltorStmts = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                
                orlCmd = new OracleCommand("pkg_workflow_screen.p_trigger_recalculate_bulk_job", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorStmts.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorStmts.Direction = ParameterDirection.Input;
                pRoyaltorStmts.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (royaltorStmts.Length == 0)
                {
                    pRoyaltorStmts.Size = 1;
                    pRoyaltorStmts.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pRoyaltorStmts.Size = royaltorStmts.Length;
                    pRoyaltorStmts.Value = royaltorStmts;
                }
                orlCmd.Parameters.Add(pRoyaltorStmts);

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
