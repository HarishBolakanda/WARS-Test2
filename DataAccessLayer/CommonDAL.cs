using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using WARS.IDAL;

namespace WARS.DataAccessLayer
{
    public class CommonDAL : ICommonDAL
    {

        #region Global Declarations        
        ConnectionDAL connDAL;
        OracleConnection orlConn;
        OracleTransaction txn;
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;        
        OracleParameter ErrorId;
        OracleParameter ErrorMsg;
        string sErrorMsg;
        #endregion Global Declarations

        /* WOS-389 - changes - removed by Harish
        public void GetNextRuntime(out string nextRunTime, out string estCompletionTime, out Int32 iErrorId, out string sErrorMsg)
        {
            nextRunTime = string.Empty;
            estCompletionTime = string.Empty;

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                ErrorId = new OracleParameter();
                ErrorMsg = new OracleParameter();
                orlCmd = new OracleCommand();

                OracleParameter next_run_time = new OracleParameter();
                OracleParameter est_completion_time = new OracleParameter();

                orlCmd = new OracleCommand("pkg_common_screens.p_get_stmt_next_run_time", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                next_run_time.OracleDbType = OracleDbType.Varchar2;
                next_run_time.Size = 200;
                next_run_time.Direction = System.Data.ParameterDirection.Output;
                next_run_time.ParameterName = "next_run_time";
                orlCmd.Parameters.Add(next_run_time);

                est_completion_time.OracleDbType = OracleDbType.Varchar2;
                est_completion_time.Size = 200;
                est_completion_time.Direction = System.Data.ParameterDirection.Output;
                est_completion_time.ParameterName = "est_completion_time";
                orlCmd.Parameters.Add(est_completion_time);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                ErrorMsg.OracleDbType = OracleDbType.Varchar2;
                ErrorMsg.Size = 200;
                ErrorMsg.Direction = System.Data.ParameterDirection.Output;
                ErrorMsg.ParameterName = "ErrorMsg";
                orlCmd.Parameters.Add(ErrorMsg);

                orlCmd.ExecuteNonQuery();

                nextRunTime = orlCmd.Parameters["next_run_time"].Value.ToString();
                estCompletionTime = orlCmd.Parameters["est_completion_time"].Value.ToString();
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                sErrorMsg = orlCmd.Parameters["ErrorMsg"].Value.ToString();
            }
            catch (Exception ex)
            {
                iErrorId = 2;
                sErrorMsg = "ERROR: Unable to getstatement next run time data. <br><br>" + ex.Message;
                throw ex;
            }
            finally
            {
                CloseConnection();
            }            
        
        }
        */
        public void GetSummaryStmtCUID(out string summaryStmtCUID, out Int32 iErrorId)
        {
            summaryStmtCUID = string.Empty;            

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                ErrorMsg = new OracleParameter();
                orlCmd = new OracleCommand();
                OracleParameter oraSummaryStmtCUID = new OracleParameter();

                orlCmd = new OracleCommand("pkg_stmt_report_cuid.p_get_summary_stmt_cuid", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                oraSummaryStmtCUID.OracleDbType = OracleDbType.Varchar2;
                oraSummaryStmtCUID.Size = 200;
                oraSummaryStmtCUID.Direction = System.Data.ParameterDirection.Output;
                oraSummaryStmtCUID.ParameterName = "oraSummaryStmtCUID";
                orlCmd.Parameters.Add(oraSummaryStmtCUID);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                summaryStmtCUID = orlCmd.Parameters["oraSummaryStmtCUID"].Value.ToString();                
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
        
        public string GetDatabaseName()
        {            
            //string sConnStr = ConfigurationManager.ConnectionStrings["WARSConnectionString"].ConnectionString;
            string sConnStr = Convert.ToString(System.Web.HttpContext.Current.Session["WARSDBConnectionString"]);             
            OracleConnection connObj = new OracleConnection(sConnStr);
            return connObj.DataSource;
        }

        public void RunRotaltyEngine(string refreshType, out string message, out Int32 iErrorId)
        {
            message = string.Empty;

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pRefreshType = new OracleParameter();
                OracleParameter oracleMessage = new OracleParameter();

                orlCmd = new OracleCommand("pkg_common_screens.run_royalty_engine", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRefreshType.OracleDbType = OracleDbType.Varchar2;
                pRefreshType.Direction = ParameterDirection.Input;
                pRefreshType.Value = refreshType;
                orlCmd.Parameters.Add(pRefreshType);

                oracleMessage.OracleDbType = OracleDbType.Varchar2;
                oracleMessage.Size = 500;
                oracleMessage.Direction = System.Data.ParameterDirection.Output;
                oracleMessage.ParameterName = "oracleMessage";
                orlCmd.Parameters.Add(oracleMessage);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                message = orlCmd.Parameters["oracleMessage"].Value.ToString();
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

        public void RunStatementArchive(out Int32 iErrorId)
        {
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();                

                orlCmd = new OracleCommand("pkg_common_screens.run_statement_archive", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                             
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

        public void LoadAdhocCostFile(out string message, out Int32 iErrorId)
        {
            message = string.Empty;

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter oracleMessage = new OracleParameter();

                orlCmd = new OracleCommand("pkg_common_screens.p_load_cost_adhoc_file", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                oracleMessage.OracleDbType = OracleDbType.Varchar2;
                oracleMessage.Size = 500;
                oracleMessage.Direction = System.Data.ParameterDirection.Output;
                oracleMessage.ParameterName = "oracleMessage";
                orlCmd.Parameters.Add(oracleMessage);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                message = orlCmd.Parameters["oracleMessage"].Value.ToString();
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
        
        public void RunAccrualProcess(string flag,out string message, out Int32 iErrorId)
        {
            message = string.Empty;

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter Pflag = new OracleParameter();
                OracleParameter oracleMessage = new OracleParameter();

                orlCmd = new OracleCommand("pkg_common_screens.p_run_accrual_process", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                Pflag.OracleDbType = OracleDbType.Varchar2;
                Pflag.Direction = ParameterDirection.Input;
                Pflag.Value = flag;
                orlCmd.Parameters.Add(Pflag);

                oracleMessage.OracleDbType = OracleDbType.Varchar2;
                oracleMessage.Size = 500;
                oracleMessage.Direction = System.Data.ParameterDirection.Output;
                oracleMessage.ParameterName = "oracleMessage";
                orlCmd.Parameters.Add(oracleMessage);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                message = orlCmd.Parameters["oracleMessage"].Value.ToString();
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

        public void GeneratePayments(out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter oracleMessage = new OracleParameter();

                orlCmd = new OracleCommand("pkg_common_screens.p_trigger_process_payments_job", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                
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

        public void RunAutoConsolidate(out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter oracleMessage = new OracleParameter();

                orlCmd = new OracleCommand("pkg_common_screens.p_run_auto_consolidate", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

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

        public void GetSharedPaths(out string pdfStatementPath, out string fileUploadPath, out Int32 iErrorId)
        {
            pdfStatementPath = string.Empty;
            fileUploadPath= string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter outPdfStatementPath = new OracleParameter();
                OracleParameter outFileUploadPath = new OracleParameter();

                orlCmd = new OracleCommand("pkg_common_screens.p_get_shared_paths", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                outPdfStatementPath.OracleDbType = OracleDbType.Varchar2;
                outPdfStatementPath.Size = 500;
                outPdfStatementPath.Direction = System.Data.ParameterDirection.Output;
                outPdfStatementPath.ParameterName = "outPdfStatementPath";
                orlCmd.Parameters.Add(outPdfStatementPath);

                outFileUploadPath.OracleDbType = OracleDbType.Varchar2;
                outFileUploadPath.Size = 500;
                outFileUploadPath.Direction = System.Data.ParameterDirection.Output;
                outFileUploadPath.ParameterName = "outFileUploadPath";
                orlCmd.Parameters.Add(outFileUploadPath);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                pdfStatementPath = orlCmd.Parameters["outPdfStatementPath"].Value.ToString();
                fileUploadPath = orlCmd.Parameters["outFileUploadPath"].Value.ToString();
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

        public void GetAdhocStatementOption( out string adhocStatementOption, out Int32 iErrorId)
        {
            adhocStatementOption = string.Empty;            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter outAdhocStmtOpt = new OracleParameter();                

                orlCmd = new OracleCommand("pkg_common_screens.p_get_adhoc_statement_option", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                outAdhocStmtOpt.OracleDbType = OracleDbType.Varchar2;
                outAdhocStmtOpt.Size = 500;
                outAdhocStmtOpt.Direction = System.Data.ParameterDirection.Output;
                outAdhocStmtOpt.ParameterName = "outAdhocStmtOpt";
                orlCmd.Parameters.Add(outAdhocStmtOpt);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                adhocStatementOption = orlCmd.Parameters["outAdhocStmtOpt"].Value.ToString();                
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
