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
    public class StatementRunDAL : IStatementRunDAL
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


        public DataSet GetStmtRunData(string stmtEndPeriod, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();
                OracleParameter stmt_end_period = new OracleParameter();
                OracleParameter cur_statement_run_data = new OracleParameter();
                orlCmd = new OracleCommand("pkg_stmt_process_screen.p_get_statement_run_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                stmt_end_period.OracleDbType = OracleDbType.Varchar2;
                stmt_end_period.Direction = ParameterDirection.Input;
                stmt_end_period.Value = stmtEndPeriod;
                orlCmd.Parameters.Add(stmt_end_period);

                cur_statement_run_data.OracleDbType = OracleDbType.RefCursor;
                cur_statement_run_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_statement_run_data);
                
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

        public DataSet UpdatestmtRunData(Array stmtsToRun, Array stmtsToRerun, Array stmtsToArchive, string stmtEndPeriod, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();                
                orlDA = new OracleDataAdapter();
                OracleParameter stmts_to_run = new OracleParameter();
                OracleParameter stmts_to_rerun = new OracleParameter();
                OracleParameter stmts_to_archive = new OracleParameter();
                OracleParameter stmt_end_period = new OracleParameter();
                OracleParameter logged_user = new OracleParameter();
                OracleParameter cur_statement_run_data = new OracleParameter();
                orlCmd = new OracleCommand("pkg_stmt_process_screen.p_update_statement_run_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                stmts_to_run.OracleDbType = OracleDbType.Int32;
                stmts_to_run.Direction = ParameterDirection.Input;
                stmts_to_run.CollectionType = OracleCollectionType.PLSQLAssociativeArray;                
                if (stmtsToRun.Length == 0)
                {
                    stmts_to_run.Size = 1;
                    stmts_to_run.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    stmts_to_run.Size = stmtsToRun.Length;
                    stmts_to_run.Value = stmtsToRun;
                }
                orlCmd.Parameters.Add(stmts_to_run);

                stmts_to_rerun.OracleDbType = OracleDbType.Int32;
                stmts_to_rerun.Direction = ParameterDirection.Input;
                stmts_to_rerun.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtsToRerun.Length == 0)
                {
                    stmts_to_rerun.Size = 1;
                    stmts_to_rerun.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    stmts_to_rerun.Size = stmtsToRerun.Length;
                    stmts_to_rerun.Value = stmtsToRerun;
                }
                orlCmd.Parameters.Add(stmts_to_rerun);

                stmts_to_archive.OracleDbType = OracleDbType.Int32;
                stmts_to_archive.Direction = ParameterDirection.Input;
                stmts_to_archive.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtsToArchive.Length == 0)
                {
                    stmts_to_archive.Size = 1;
                    stmts_to_archive.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    stmts_to_archive.Size = stmtsToArchive.Length;
                    stmts_to_archive.Value = stmtsToArchive;
                }
                orlCmd.Parameters.Add(stmts_to_archive);

                stmt_end_period.OracleDbType = OracleDbType.Varchar2;
                stmt_end_period.Direction = ParameterDirection.Input;
                stmt_end_period.Value = stmtEndPeriod;
                orlCmd.Parameters.Add(stmt_end_period);

                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

                cur_statement_run_data.OracleDbType = OracleDbType.RefCursor;
                cur_statement_run_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_statement_run_data);

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
