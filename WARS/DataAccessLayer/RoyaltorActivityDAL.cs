using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;

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
                cur_activity_data = new OracleParameter();
                //next_run_time = new OracleParameter();
                ErrorId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_activity_screen.p_update_activity_data", orlConn);
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
                /*
                next_run_time.OracleDbType = OracleDbType.Varchar2;
                next_run_time.Direction = ParameterDirection.Output;
                next_run_time.Size = 200;
                next_run_time.ParameterName = "next_run_time";
                orlCmd.Parameters.Add(next_run_time);*/
                
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

        public DataSet SetRetry(string levelFlag, string code, string loggedUser, string stmtPeriodId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter level_flag = new OracleParameter();
                OracleParameter code_id = new OracleParameter();
                OracleParameter stmt_period_id = new OracleParameter();
                OracleParameter logged_user = new OracleParameter();
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
