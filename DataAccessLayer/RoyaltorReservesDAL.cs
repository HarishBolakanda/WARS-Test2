using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;
using Oracle.DataAccess.Types;
using WARS.DataAccessLayer;

namespace WARS.DataAccessLayer
{

    public class RoyaltorReservesDAL : IRoyaltorReservesDAL
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

        public DataSet GetRoyaltorData(Int32 royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter cur_royaltor_bal_data = new OracleParameter();
                OracleParameter cur_royaltor_rsv_data = new OracleParameter();
                OracleParameter cur_stmt_period_data = new OracleParameter();


                orlCmd = new OracleCommand("pkg_maint_reserves.p_get_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                cur_royaltor_bal_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_bal_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_bal_data);

                cur_royaltor_rsv_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_rsv_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_rsv_data);

                cur_stmt_period_data.OracleDbType = OracleDbType.RefCursor;
                cur_stmt_period_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_stmt_period_data);

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

        //public DataSet GetRoyaltorRsvData(Int32 royaltorId, out Int32 iErrorId)
        //{
        //    ds = new DataSet();
        //    try
        //    {
        //        OpenConnection(out iErrorId, out sErrorMsg);
        //        ErrorId = new OracleParameter();
        //        orlDA = new OracleDataAdapter();

        //        OracleParameter in_v_royaltor_id = new OracleParameter();
        //        OracleParameter cur_royaltor_rsv_data = new OracleParameter();
        //        OracleParameter cur_stmt_period_data = new OracleParameter();

        //        orlCmd = new OracleCommand("pkg_maint_reserves.p_get_reserves", orlConn);
        //        orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //        in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
        //        in_v_royaltor_id.Direction = ParameterDirection.Input;
        //        in_v_royaltor_id.Value = royaltorId;
        //        orlCmd.Parameters.Add(in_v_royaltor_id);

        //        cur_royaltor_rsv_data.OracleDbType = OracleDbType.RefCursor;
        //        cur_royaltor_rsv_data.Direction = System.Data.ParameterDirection.Output;
        //        orlCmd.Parameters.Add(cur_royaltor_rsv_data);

        //        cur_stmt_period_data.OracleDbType = OracleDbType.RefCursor;
        //        cur_stmt_period_data.Direction = System.Data.ParameterDirection.Output;
        //        orlCmd.Parameters.Add(cur_stmt_period_data);

        //        ErrorId.OracleDbType = OracleDbType.Int32;
        //        ErrorId.Direction = ParameterDirection.Output;
        //        ErrorId.ParameterName = "ErrorId";
        //        orlCmd.Parameters.Add(ErrorId);

        //        orlDA = new OracleDataAdapter(orlCmd);
        //        orlDA.Fill(ds);

        //        iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

        //    }
        //    catch (Exception ex)
        //    {
        //        iErrorId = 2;
        //    }
        //    finally
        //    {
        //        CloseConnection();
        //    }
        //    return ds;

        //}

        public DataSet UpdateRoyaltorBalData(Int32 royaltorId, Int32 balancePeriodID, double updatedBalance, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_balance_period = new OracleParameter();
                OracleParameter in_v_updated_balance = new OracleParameter();
                OracleParameter in_v_logged_user = new OracleParameter();
                OracleParameter cur_royaltor_bal_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_reserves.p_update_balance_history", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_balance_period.OracleDbType = OracleDbType.Int32;
                in_v_balance_period.Direction = ParameterDirection.Input;
                in_v_balance_period.Value = balancePeriodID;
                orlCmd.Parameters.Add(in_v_balance_period);

                in_v_updated_balance.OracleDbType = OracleDbType.Double;
                in_v_updated_balance.Direction = ParameterDirection.Input;
                in_v_updated_balance.Value = updatedBalance;
                orlCmd.Parameters.Add(in_v_updated_balance);

                in_v_logged_user.OracleDbType = OracleDbType.Varchar2;
                in_v_logged_user.Direction = ParameterDirection.Input;
                in_v_logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(in_v_logged_user);

                cur_royaltor_bal_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_bal_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_bal_data);

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

        public DataSet DeleteRoyaltorBalData(Int32 royaltorId, Int32 balancePeriodID, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_balance_period = new OracleParameter();
                OracleParameter in_v_logged_user = new OracleParameter();
                OracleParameter cur_royaltor_bal_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_reserves.p_delete_balance_history", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_balance_period.OracleDbType = OracleDbType.Int32;
                in_v_balance_period.Direction = ParameterDirection.Input;
                in_v_balance_period.Value = balancePeriodID;
                orlCmd.Parameters.Add(in_v_balance_period);

                in_v_logged_user.OracleDbType = OracleDbType.Varchar2;
                in_v_logged_user.Direction = ParameterDirection.Input;
                in_v_logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(in_v_logged_user);

                cur_royaltor_bal_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_bal_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_bal_data);

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

        public DataSet DeleteRoyaltorRsvData(Int32 royaltorId, Int32 reservePeriodId, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_reserve_period_id = new OracleParameter();
                OracleParameter in_v_logged_user = new OracleParameter();
                OracleParameter cur_royaltor_rsv_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_reserves.p_delete_royaltor_rsv", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_reserve_period_id.OracleDbType = OracleDbType.Int32;
                in_v_reserve_period_id.Direction = ParameterDirection.Input;
                in_v_reserve_period_id.Value = reservePeriodId;
                orlCmd.Parameters.Add(in_v_reserve_period_id);

                in_v_logged_user.OracleDbType = OracleDbType.Varchar2;
                in_v_logged_user.Direction = ParameterDirection.Input;
                in_v_logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(in_v_logged_user);

                cur_royaltor_rsv_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_rsv_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_rsv_data);

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

        public DataSet InsertRoyaltorRsvData(Int32 royaltorId, Int32 reservePeriodId, Int32 lqdInterval, double rsvAmount, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_reserve_period_id = new OracleParameter();
                OracleParameter in_v_liq_interval = new OracleParameter();
                OracleParameter in_v_rsv_amount = new OracleParameter();
                OracleParameter in_v_user_code = new OracleParameter();
                OracleParameter cur_royaltor_rsv_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_reserves.p_insert_royaltor_rsv", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_reserve_period_id.OracleDbType = OracleDbType.Int32;
                in_v_reserve_period_id.Direction = ParameterDirection.Input;
                in_v_reserve_period_id.Value = reservePeriodId;
                orlCmd.Parameters.Add(in_v_reserve_period_id);

                in_v_liq_interval.OracleDbType = OracleDbType.Int32;
                in_v_liq_interval.Direction = ParameterDirection.Input;
                in_v_liq_interval.Value = lqdInterval;
                orlCmd.Parameters.Add(in_v_liq_interval);

                in_v_rsv_amount.OracleDbType = OracleDbType.Double;
                in_v_rsv_amount.Direction = ParameterDirection.Input;
                in_v_rsv_amount.Value = rsvAmount;
                orlCmd.Parameters.Add(in_v_rsv_amount);

                in_v_user_code.OracleDbType = OracleDbType.Varchar2;
                in_v_user_code.Direction = ParameterDirection.Input;
                in_v_user_code.Value = userCode;
                orlCmd.Parameters.Add(in_v_user_code);

                cur_royaltor_rsv_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_rsv_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_rsv_data);

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

        public DataSet UpdateRoyaltorRsvData(Int32 royaltorId, Int32 oldReservePeriodID, Int32 newReservePeriodID, Int32 lqdInterval, double rsvAmount, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_old_reserve_period_id = new OracleParameter();
                OracleParameter in_v_new_reserve_period_id = new OracleParameter();
                OracleParameter in_v_liq_interval = new OracleParameter();
                OracleParameter in_v_rsv_amount = new OracleParameter();
                OracleParameter in_v_logged_user = new OracleParameter();
                OracleParameter cur_royaltor_rsv_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_reserves.p_update_royaltor_rsv", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_old_reserve_period_id.OracleDbType = OracleDbType.Int32;
                in_v_old_reserve_period_id.Direction = ParameterDirection.Input;
                in_v_old_reserve_period_id.Value = oldReservePeriodID;
                orlCmd.Parameters.Add(in_v_old_reserve_period_id);

                in_v_new_reserve_period_id.OracleDbType = OracleDbType.Int32;
                in_v_new_reserve_period_id.Direction = ParameterDirection.Input;
                in_v_new_reserve_period_id.Value = newReservePeriodID;
                orlCmd.Parameters.Add(in_v_new_reserve_period_id);

                in_v_liq_interval.OracleDbType = OracleDbType.Int32;
                in_v_liq_interval.Direction = ParameterDirection.Input;
                in_v_liq_interval.Value = lqdInterval;
                orlCmd.Parameters.Add(in_v_liq_interval);

                in_v_rsv_amount.OracleDbType = OracleDbType.Double;
                in_v_rsv_amount.Direction = ParameterDirection.Input;
                in_v_rsv_amount.Value = rsvAmount;
                orlCmd.Parameters.Add(in_v_rsv_amount);

                in_v_logged_user.OracleDbType = OracleDbType.Varchar2;
                in_v_logged_user.Direction = ParameterDirection.Input;
                in_v_logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(in_v_logged_user);

                cur_royaltor_rsv_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_rsv_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_rsv_data);

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

        public DataSet DeleteRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodId,Int32 lqdPeriodId, string loggedUser, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_reserve_period_id = new OracleParameter();
                OracleParameter in_v_liqudity_period = new OracleParameter();
                OracleParameter in_v_logged_user = new OracleParameter();
                OracleParameter cur_royaltor_rsv_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_reserves.p_delete_lqd_rsv_summ", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_reserve_period_id.OracleDbType = OracleDbType.Int32;
                in_v_reserve_period_id.Direction = ParameterDirection.Input;
                in_v_reserve_period_id.Value = reservePeriodId;
                orlCmd.Parameters.Add(in_v_reserve_period_id);

                in_v_liqudity_period.OracleDbType = OracleDbType.Int32;
                in_v_liqudity_period.Direction = ParameterDirection.Input;
                in_v_liqudity_period.Value = lqdPeriodId;
                orlCmd.Parameters.Add(in_v_liqudity_period);

                in_v_logged_user.OracleDbType = OracleDbType.Varchar2;
                in_v_logged_user.Direction = ParameterDirection.Input;
                in_v_logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(in_v_logged_user);

                cur_royaltor_rsv_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_rsv_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_rsv_data);

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

        public DataSet InsertRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodId, Int32 lqdPeriodId,Int32 lqdInterval, double lqdAmount, string userCode, out Int32 iErrorId)        
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_reserve_period_id = new OracleParameter();
                OracleParameter in_v_liquidity_period = new OracleParameter();
                OracleParameter in_v_liquidity_interval = new OracleParameter();
                OracleParameter in_v_liquidity_amount = new OracleParameter();
                OracleParameter in_v_user_code = new OracleParameter();
                OracleParameter cur_royaltor_rsv_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_reserves.p_insert_lqd_rsv_summ", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_reserve_period_id.OracleDbType = OracleDbType.Int32;
                in_v_reserve_period_id.Direction = ParameterDirection.Input;
                in_v_reserve_period_id.Value = reservePeriodId;
                orlCmd.Parameters.Add(in_v_reserve_period_id);

                in_v_liquidity_period.OracleDbType = OracleDbType.Int32;
                in_v_liquidity_period.Direction = ParameterDirection.Input;
                in_v_liquidity_period.Value = lqdPeriodId;
                orlCmd.Parameters.Add(in_v_liquidity_period);

                in_v_liquidity_interval.OracleDbType = OracleDbType.Int32;
                in_v_liquidity_interval.Direction = ParameterDirection.Input;
                in_v_liquidity_interval.Value = lqdInterval;
                orlCmd.Parameters.Add(in_v_liquidity_interval);

                in_v_liquidity_amount.OracleDbType = OracleDbType.Double;
                in_v_liquidity_amount.Direction = ParameterDirection.Input;
                in_v_liquidity_amount.Value = lqdAmount;
                orlCmd.Parameters.Add(in_v_liquidity_amount);

                in_v_user_code.OracleDbType = OracleDbType.Varchar2;
                in_v_user_code.Direction = ParameterDirection.Input;
                in_v_user_code.Value = userCode;
                orlCmd.Parameters.Add(in_v_user_code);

                cur_royaltor_rsv_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_rsv_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_rsv_data);

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

        public DataSet UpdateRoyaltorLqdData(Int32 royaltorId, Int32 reservePeriodID, Int32 oldLqdPeriod, Int32 newLqdPeriod, Int32 lqdInterval, double lqdAmount, string loggedUser, out Int32 iErrorId)        
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_reserve_period_id = new OracleParameter();
                OracleParameter in_v_old_lqd_period_id = new OracleParameter();
                OracleParameter in_v_new_lqd_period_id = new OracleParameter();
                OracleParameter in_v_lqd_interval = new OracleParameter();
                OracleParameter in_v_lqd_amount = new OracleParameter();
                OracleParameter in_v_logged_user = new OracleParameter();
                OracleParameter cur_royaltor_rsv_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_reserves.p_update_lqd_rsv_summ", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_reserve_period_id.OracleDbType = OracleDbType.Int32;
                in_v_reserve_period_id.Direction = ParameterDirection.Input;
                in_v_reserve_period_id.Value = reservePeriodID;
                orlCmd.Parameters.Add(in_v_reserve_period_id);

                in_v_old_lqd_period_id.OracleDbType = OracleDbType.Int32;
                in_v_old_lqd_period_id.Direction = ParameterDirection.Input;
                in_v_old_lqd_period_id.Value = oldLqdPeriod;
                orlCmd.Parameters.Add(in_v_old_lqd_period_id);

                in_v_new_lqd_period_id.OracleDbType = OracleDbType.Int32;
                in_v_new_lqd_period_id.Direction = ParameterDirection.Input;
                in_v_new_lqd_period_id.Value = newLqdPeriod;
                orlCmd.Parameters.Add(in_v_new_lqd_period_id);

                in_v_lqd_interval.OracleDbType = OracleDbType.Double;
                in_v_lqd_interval.Direction = ParameterDirection.Input;
                in_v_lqd_interval.Value = lqdInterval;
                orlCmd.Parameters.Add(in_v_lqd_interval);

                in_v_lqd_amount.OracleDbType = OracleDbType.Double;
                in_v_lqd_amount.Direction = ParameterDirection.Input;
                in_v_lqd_amount.Value = lqdAmount;
                orlCmd.Parameters.Add(in_v_lqd_amount);

                in_v_logged_user.OracleDbType = OracleDbType.Varchar2;
                in_v_logged_user.Direction = ParameterDirection.Input;
                in_v_logged_user.Value = loggedUser;
                orlCmd.Parameters.Add(in_v_logged_user);

                cur_royaltor_rsv_data.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_rsv_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_rsv_data);

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
