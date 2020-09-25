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
    public class RoyaltorActivityDAL : IRoyaltorActivityDAL
    {

        #region Global Declarations        
        ConnectionDAL connDAL;
        OracleConnection orlConn;        
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        OracleParameter ErrorId;
        OracleParameter cur_activity_data;
        OracleParameter next_run_time;
        string sErrorMsg;
        #endregion Global Declarations


        //public DataSet GetActivityData(out string nextRunTime, out Int32 iErrorId)
        public DataSet GetActivityData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                cur_activity_data = new OracleParameter();
                //next_run_time = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_activity_screen.p_get_activity_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                cur_activity_data.OracleDbType = OracleDbType.RefCursor;
                cur_activity_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_activity_data);
                /*
                next_run_time.OracleDbType = OracleDbType.Varchar2;
                next_run_time.Direction = ParameterDirection.Output;
                next_run_time.Size = 200;
                next_run_time.ParameterName = "next_run_time";
                orlCmd.Parameters.Add(next_run_time);*/

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                //nextRunTime = orlCmd.Parameters["next_run_time"].Value.ToString();

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                //nextRunTime = string.Empty;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        //public DataSet UpdateRemoveFromRun(string levelFlag, string code, string loggedUser, string stmtPeriodId, out string nextRunTime, out Int32 iErrorId)
        public DataSet UpdateRemoveFromRun(string levelFlag, string code, string loggedUser, string stmtPeriodId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter level_flag = new OracleParameter();
                OracleParameter code_id = new OracleParameter();
                OracleParameter stmt_period_id = new OracleParameter();
                OracleParameter logged_user = new OracleParameter();
                OracleParameter remove_all = new OracleParameter();
                cur_activity_data = new OracleParameter();
                //next_run_time = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_activity_screen.p_remove_from_run", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                level_flag.OracleDbType = OracleDbType.Varchar2;
                level_flag.Direction = ParameterDirection.Input;
                level_flag.Value = levelFlag;
                orlCmd.Parameters.Add(level_flag);

                code_id.OracleDbType = OracleDbType.Varchar2;
                code_id.Direction = ParameterDirection.Input;
                code_id.Value = code;
                orlCmd.Parameters.Add(code_id);

                stmt_period_id.OracleDbType = OracleDbType.Varchar2;
                stmt_period_id.Direction = ParameterDirection.Input;
                stmt_period_id.Value = stmtPeriodId;
                orlCmd.Parameters.Add(stmt_period_id);

                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

                remove_all.OracleDbType = OracleDbType.Varchar2;
                remove_all.Direction = ParameterDirection.Input;
                remove_all.Value = "N";
                orlCmd.Parameters.Add(remove_all);

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
                //nextRunTime = orlCmd.Parameters["next_run_time"].Value.ToString();

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                //nextRunTime = string.Empty;                
            }
            finally
            {
                CloseConnection();
            }

            return ds;

        }

        public DataSet SetRetry(string levelFlag, string code, string royStmtFlag, string detailFileFlag, string loggedUser, string stmtPeriodId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter level_flag = new OracleParameter();
                OracleParameter code_id = new OracleParameter();
                OracleParameter stmt_period_id = new OracleParameter();
                OracleParameter inRoyStmtFlag = new OracleParameter();
                OracleParameter inDetailFileFlag = new OracleParameter();                
                OracleParameter logged_user = new OracleParameter();
                OracleParameter retry_all = new OracleParameter();
                cur_activity_data = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_activity_screen.p_set_retry", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                level_flag.OracleDbType = OracleDbType.Varchar2;
                level_flag.Direction = ParameterDirection.Input;
                level_flag.Value = levelFlag;
                orlCmd.Parameters.Add(level_flag);

                code_id.OracleDbType = OracleDbType.Varchar2;
                code_id.Direction = ParameterDirection.Input;
                code_id.Value = code;
                orlCmd.Parameters.Add(code_id);

                stmt_period_id.OracleDbType = OracleDbType.Varchar2;
                stmt_period_id.Direction = ParameterDirection.Input;
                stmt_period_id.Value = stmtPeriodId;
                orlCmd.Parameters.Add(stmt_period_id);

                inRoyStmtFlag.OracleDbType = OracleDbType.Varchar2;
                inRoyStmtFlag.Direction = ParameterDirection.Input;
                inRoyStmtFlag.Value = royStmtFlag;
                orlCmd.Parameters.Add(inRoyStmtFlag);

                inDetailFileFlag.OracleDbType = OracleDbType.Varchar2;
                inDetailFileFlag.Direction = ParameterDirection.Input;
                inDetailFileFlag.Value = detailFileFlag;
                orlCmd.Parameters.Add(inDetailFileFlag);

                logged_user.OracleDbType = OracleDbType.Varchar2;
                logged_user.Direction = ParameterDirection.Input;
                logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(logged_user);

                retry_all.OracleDbType = OracleDbType.Varchar2;
                retry_all.Direction = ParameterDirection.Input;
                retry_all.Value = "N";
                orlCmd.Parameters.Add(retry_all);

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
            }
            finally
            {
                CloseConnection();
            }

            return ds;

        }

        public DataSet RemoveAllFromRun(Array stmtsToRemove, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pStmtsToRemove = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                cur_activity_data = new OracleParameter();
                ErrorId = new OracleParameter();
                orlCmd = new OracleCommand("pkg_activity_screen.p_remove_all_from_run", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pStmtsToRemove.OracleDbType = OracleDbType.Varchar2;
                pStmtsToRemove.Direction = ParameterDirection.Input;
                pStmtsToRemove.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtsToRemove.Length == 0)
                {
                    pStmtsToRemove.Size = 1;
                    pStmtsToRemove.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pStmtsToRemove.Size = stmtsToRemove.Length;
                    pStmtsToRemove.Value = stmtsToRemove;
                }
                orlCmd.Parameters.Add(pStmtsToRemove);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

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
            }
            finally
            {
                CloseConnection();
            }

            return ds;

        }

        public DataSet RetryAll(Array stmtsToRetry, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter pStmtsToRetry = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                cur_activity_data = new OracleParameter();
                ErrorId = new OracleParameter();
                orlCmd = new OracleCommand("pkg_activity_screen.p_set_retry_all", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pStmtsToRetry.OracleDbType = OracleDbType.Varchar2;
                pStmtsToRetry.Direction = ParameterDirection.Input;
                pStmtsToRetry.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (stmtsToRetry.Length == 0)
                {
                    pStmtsToRetry.Size = 1;
                    pStmtsToRetry.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pStmtsToRetry.Size = stmtsToRetry.Length;
                    pStmtsToRetry.Value = stmtsToRetry;
                }
                orlCmd.Parameters.Add(pStmtsToRetry);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

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
