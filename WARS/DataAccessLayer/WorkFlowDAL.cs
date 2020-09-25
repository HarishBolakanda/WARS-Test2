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


        public DataSet GetDropDownData(string loggedUser, out string defaultThresholds, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                OracleParameter logged_user = new OracleParameter();
                OracleParameter cur_company = new OracleParameter();
                OracleParameter cur_statement_period = new OracleParameter();
                OracleParameter cur_owner = new OracleParameter();
                OracleParameter cur_royaltor = new OracleParameter();
                OracleParameter cur_status = new OracleParameter();
                OracleParameter cur_responsibility = new OracleParameter();
                OracleParameter cur_priority = new OracleParameter();
                OracleParameter pDefaultThresholds = new OracleParameter();
                
                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_dropdownlist_values", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

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

                cur_responsibility.OracleDbType = OracleDbType.RefCursor;
                cur_responsibility.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_responsibility);

                cur_priority.OracleDbType = OracleDbType.RefCursor;
                cur_priority.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_priority);

                pDefaultThresholds.OracleDbType = OracleDbType.Varchar2;
                pDefaultThresholds.Size = 100;
                pDefaultThresholds.Direction = System.Data.ParameterDirection.Output;
                pDefaultThresholds.ParameterName = "pDefaultThresholds";
                orlCmd.Parameters.Add(pDefaultThresholds);
                
                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                defaultThresholds = orlCmd.Parameters["pDefaultThresholds"].Value.ToString();
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

        public DataSet GetFiltersForResp(string respCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter oraRespCode = new OracleParameter();
                OracleParameter cur_owner = new OracleParameter();
                OracleParameter cur_royaltor = new OracleParameter();

                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_filters_for_resp", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                oraRespCode.OracleDbType = OracleDbType.Varchar2;
                oraRespCode.Direction = ParameterDirection.Input;
                oraRespCode.Value = respCode;
                orlCmd.Parameters.Add(oraRespCode);
                
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

        /* removed by harish (12-01-2017) as it is no longer being used
        public DataSet GetWorkflowData(string loggedUser, out string respCode, out Int32 iErrorId)        
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                OracleParameter cur_activity_data = new OracleParameter();
                OracleParameter logged_user = new OracleParameter();
                OracleParameter responsiblity_code = new OracleParameter();                
              
                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_workflow_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

                responsiblity_code.OracleDbType = OracleDbType.Varchar2;
                responsiblity_code.Size = 100;
                responsiblity_code.Direction = System.Data.ParameterDirection.Output;
                responsiblity_code.ParameterName = "reponsible_code";
                orlCmd.Parameters.Add(responsiblity_code);
                                
                cur_activity_data.OracleDbType = OracleDbType.RefCursor;
                cur_activity_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_activity_data);               
                
                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                respCode = orlCmd.Parameters["reponsible_code"].Value.ToString();                
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
        */

        public DataSet GetWorkflowSearchData(string compCode, string rep_schedule, string owner, string royaltor, string responsibility, string priority, string status,
            string recoupedThreshold, string unrecoupedThreshold, out Int32 iErrorId)
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
                OracleParameter responsibility_code = new OracleParameter();
                OracleParameter priority_code = new OracleParameter();
                OracleParameter stmt_status_code = new OracleParameter();
                OracleParameter recouped_threshold = new OracleParameter();
                OracleParameter unrecouped_threshold = new OracleParameter();
                
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

                responsibility_code.OracleDbType = OracleDbType.Varchar2;
                responsibility_code.Direction = ParameterDirection.Input;
                responsibility_code.Value = responsibility;
                orlCmd.Parameters.Add(responsibility_code);

                priority_code.OracleDbType = OracleDbType.Varchar2;
                priority_code.Direction = ParameterDirection.Input;
                priority_code.Value = priority;
                orlCmd.Parameters.Add(priority_code);

                stmt_status_code.OracleDbType = OracleDbType.Varchar2;
                stmt_status_code.Direction = ParameterDirection.Input;
                stmt_status_code.Value = status;
                orlCmd.Parameters.Add(stmt_status_code);

                recouped_threshold.OracleDbType = OracleDbType.Varchar2;
                recouped_threshold.Direction = ParameterDirection.Input;
                recouped_threshold.Value = recoupedThreshold;
                orlCmd.Parameters.Add(recouped_threshold);

                unrecouped_threshold.OracleDbType = OracleDbType.Varchar2;
                unrecouped_threshold.Direction = ParameterDirection.Input;
                unrecouped_threshold.Value = unrecoupedThreshold;
                orlCmd.Parameters.Add(unrecouped_threshold);
                                                              
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

        public DataSet UpdateWorkflowStatus(string pageMode, string royaltorID, string stmtPeriodId, string ownerCode, string status, string compCode, string rep_schedule, string owner, string royaltor, string responsibility,
                                     string priority, string status_search, string recoupedThreshold, string unrecoupedThreshold, string loggedUser,
                                     out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                

                OracleParameter page_mode = new OracleParameter();
                OracleParameter owner_code = new OracleParameter();
                OracleParameter royaltor_id = new OracleParameter();
                OracleParameter stmt_period_id = new OracleParameter();
                OracleParameter stmt_status_code = new OracleParameter();
                OracleParameter cur_activity_data = new OracleParameter();
                OracleParameter company_code_search = new OracleParameter();
                OracleParameter stmt_period_id_search = new OracleParameter();
                OracleParameter owner_code_search = new OracleParameter();
                OracleParameter royaltor_id_search = new OracleParameter();
                OracleParameter responsibility_code = new OracleParameter();
                OracleParameter priority_code = new OracleParameter();
                OracleParameter stmt_status_code_search = new OracleParameter();
                OracleParameter recouped_threshold = new OracleParameter();
                OracleParameter unrecouped_threshold = new OracleParameter();
                OracleParameter logged_user = new OracleParameter();                

                orlCmd = new OracleCommand("pkg_workflow_screen.p_update_workflow_status", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                page_mode.OracleDbType = OracleDbType.Varchar2;
                page_mode.Direction = ParameterDirection.Input;
                if (pageMode == "")
                    page_mode.Value = DBNull.Value;
                else
                    page_mode.Value = pageMode;
                orlCmd.Parameters.Add(page_mode);

                royaltor_id.OracleDbType = OracleDbType.Varchar2;
                royaltor_id.Direction = ParameterDirection.Input;
                royaltor_id.Value = royaltorID;
                orlCmd.Parameters.Add(royaltor_id);

                stmt_period_id.OracleDbType = OracleDbType.Varchar2;
                stmt_period_id.Direction = ParameterDirection.Input;
                stmt_period_id.Value = stmtPeriodId;
                orlCmd.Parameters.Add(stmt_period_id);

                owner_code.OracleDbType = OracleDbType.Varchar2;
                owner_code.Direction = ParameterDirection.Input;
                if (ownerCode == "")
                    owner_code.Value = DBNull.Value;
                else
                    owner_code.Value = ownerCode;
                orlCmd.Parameters.Add(owner_code);
                
                stmt_status_code.OracleDbType = OracleDbType.Varchar2;
                stmt_status_code.Direction = ParameterDirection.Input;
                stmt_status_code.Value = status;
                orlCmd.Parameters.Add(stmt_status_code);

                company_code_search.OracleDbType = OracleDbType.Varchar2;
                company_code_search.Direction = ParameterDirection.Input;
                company_code_search.Value = compCode;
                orlCmd.Parameters.Add(company_code_search);

                stmt_period_id_search.OracleDbType = OracleDbType.Varchar2;
                stmt_period_id_search.Direction = ParameterDirection.Input;
                stmt_period_id_search.Value = rep_schedule;
                orlCmd.Parameters.Add(stmt_period_id_search);

                owner_code_search.OracleDbType = OracleDbType.Varchar2;
                owner_code_search.Direction = ParameterDirection.Input;
                owner_code_search.Value = (owner == "" ? "-" : owner);
                orlCmd.Parameters.Add(owner_code_search);

                royaltor_id_search.OracleDbType = OracleDbType.Varchar2;
                royaltor_id_search.Direction = ParameterDirection.Input;
                royaltor_id_search.Value = (royaltor == "" ? "-" : royaltor);
                orlCmd.Parameters.Add(royaltor_id_search);

                responsibility_code.OracleDbType = OracleDbType.Varchar2;
                responsibility_code.Direction = ParameterDirection.Input;
                responsibility_code.Value = responsibility;
                orlCmd.Parameters.Add(responsibility_code);

                priority_code.OracleDbType = OracleDbType.Varchar2;
                priority_code.Direction = ParameterDirection.Input;
                priority_code.Value = priority;
                orlCmd.Parameters.Add(priority_code);

                stmt_status_code_search.OracleDbType = OracleDbType.Varchar2;
                stmt_status_code_search.Direction = ParameterDirection.Input;
                stmt_status_code_search.Value = status_search;
                orlCmd.Parameters.Add(stmt_status_code_search);

                recouped_threshold.OracleDbType = OracleDbType.Varchar2;
                recouped_threshold.Direction = ParameterDirection.Input;
                recouped_threshold.Value = recoupedThreshold;
                orlCmd.Parameters.Add(recouped_threshold);

                unrecouped_threshold.OracleDbType = OracleDbType.Varchar2;
                unrecouped_threshold.Direction = ParameterDirection.Input;
                unrecouped_threshold.Value = unrecoupedThreshold;
                orlCmd.Parameters.Add(unrecouped_threshold);
                               
                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

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

        public void UpdateSummaryStmtDetails(string royaltorId, string stmtPeriodId, out string createFrontSheet, out string masterGroupedRoyaltor,
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

        public DataSet GetCourtesySharIds(string royaltorID, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();
                OracleParameter royaltor_id = new OracleParameter();
                OracleParameter cur_company = new OracleParameter();

                orlCmd = new OracleCommand("pkg_workflow_screen.p_get_courtesy_share_ids", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                royaltor_id.OracleDbType = OracleDbType.Varchar2;
                royaltor_id.Direction = ParameterDirection.Input;
                royaltor_id.Value = royaltorID;
                orlCmd.Parameters.Add(royaltor_id);

                cur_company.OracleDbType = OracleDbType.RefCursor;
                cur_company.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_company);

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
