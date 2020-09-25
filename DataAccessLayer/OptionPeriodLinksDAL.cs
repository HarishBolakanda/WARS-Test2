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
    public class OptionPeriodLinksDAL : IOptionPeriodLinksDAL
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
               
        public DataSet GetOptionPeriods(Int32 royaltorId, string loggedInUserRole, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_logged_In_UserRole = new OracleParameter();
                OracleParameter cur_option_periods = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_option_period_links.p_get_option_periods", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_logged_In_UserRole.OracleDbType = OracleDbType.Int32;
                in_v_logged_In_UserRole.Direction = ParameterDirection.Input;
                in_v_logged_In_UserRole.Value = loggedInUserRole;
                orlCmd.Parameters.Add(in_v_logged_In_UserRole);

                cur_option_periods.OracleDbType = OracleDbType.RefCursor;
                cur_option_periods.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_option_periods);

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

        public DataSet GetIntialLinksData(Int32 royaltorId, Int32 optionPeriodCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_option_period = new OracleParameter();
                OracleParameter cur_initial_links_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_option_period_links.p_get_initial_links_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_option_period.OracleDbType = OracleDbType.Int32;
                in_v_option_period.Direction = ParameterDirection.Input;
                in_v_option_period.Value = optionPeriodCode;
                orlCmd.Parameters.Add(in_v_option_period);

                cur_initial_links_data.OracleDbType = OracleDbType.RefCursor;
                cur_initial_links_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_initial_links_data);

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

        public DataSet GetArtistLinksData(string artistName, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_royaltor_name = new OracleParameter();
                OracleParameter cur_artist_links_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_option_period_links.p_get_artist_links", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_royaltor_name.OracleDbType = OracleDbType.Varchar2;
                in_v_royaltor_name.Direction = ParameterDirection.Input;
                in_v_royaltor_name.Value = artistName;
                orlCmd.Parameters.Add(in_v_royaltor_name);

                cur_artist_links_data.OracleDbType = OracleDbType.RefCursor;
                cur_artist_links_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_artist_links_data);

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

        public DataSet OptionPeriodOperations(Array artistIdsToAdd, Array artistIdsToRemove, Array artistIdsToReplace, Int32 royaltorId, Int32 optionPeriodCode, string userCode, string displayArtistIds, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_artist_id_to_add = new OracleParameter();
                OracleParameter in_v_artist_id_to_remove = new OracleParameter();
                OracleParameter in_v_artist_id_to_replace = new OracleParameter();
                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_option_period = new OracleParameter();
                OracleParameter in_v_user_code = new OracleParameter();
                OracleParameter in_v_artistids_to_display = new OracleParameter();
                OracleParameter in_v_artist_name = new OracleParameter();
                OracleParameter cur_updated_links_data = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_option_period_links.p_option_period_operations", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_artist_id_to_add.OracleDbType = OracleDbType.Int32;
                in_v_artist_id_to_add.Direction = ParameterDirection.Input;
                in_v_artist_id_to_add.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (artistIdsToAdd.Length == 0)
                {
                    in_v_artist_id_to_add.Size = 0;
                    in_v_artist_id_to_add.Value = new OracleDecimal[1] { OracleDecimal.Null }; 
                }
                else
                {
                    in_v_artist_id_to_add.Size = artistIdsToAdd.Length;
                    in_v_artist_id_to_add.Value = artistIdsToAdd;
                }
                orlCmd.Parameters.Add(in_v_artist_id_to_add);

                in_v_artist_id_to_remove.OracleDbType = OracleDbType.Int32;
                in_v_artist_id_to_remove.Direction = ParameterDirection.Input;
                in_v_artist_id_to_remove.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (artistIdsToRemove.Length == 0)
                {
                    in_v_artist_id_to_remove.Size = 0;
                    in_v_artist_id_to_remove.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    in_v_artist_id_to_remove.Size = artistIdsToRemove.Length;
                    in_v_artist_id_to_remove.Value = artistIdsToRemove;
                }
                orlCmd.Parameters.Add(in_v_artist_id_to_remove);

                in_v_artist_id_to_replace.OracleDbType = OracleDbType.Int32;
                in_v_artist_id_to_replace.Direction = ParameterDirection.Input;
                in_v_artist_id_to_replace.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (artistIdsToReplace.Length == 0)
                {
                    in_v_artist_id_to_replace.Size = 0;
                    in_v_artist_id_to_replace.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    in_v_artist_id_to_replace.Size = artistIdsToReplace.Length;
                    in_v_artist_id_to_replace.Value = artistIdsToReplace;
                }
                orlCmd.Parameters.Add(in_v_artist_id_to_replace);

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_option_period.OracleDbType = OracleDbType.Int32;
                in_v_option_period.Direction = ParameterDirection.Input;
                in_v_option_period.Value = optionPeriodCode;
                orlCmd.Parameters.Add(in_v_option_period);

                in_v_user_code.OracleDbType = OracleDbType.Varchar2;
                in_v_user_code.Direction = ParameterDirection.Input;
                in_v_user_code.Value = userCode;
                orlCmd.Parameters.Add(in_v_user_code);

                in_v_artistids_to_display.OracleDbType = OracleDbType.Varchar2;
                in_v_artistids_to_display.Direction = ParameterDirection.Input;
                in_v_artistids_to_display.Value = displayArtistIds;
                orlCmd.Parameters.Add(in_v_artistids_to_display);

                //in_v_artist_name.OracleDbType = OracleDbType.Varchar2;
                //in_v_artist_name.Direction = ParameterDirection.Input;
                //if (string.IsNullOrEmpty(artistName))
                //{
                //    in_v_artist_name.Value = DBNull.Value;
                //}
                //else
                //{
                //    in_v_artist_name.Value = artistName;
                //}
                //orlCmd.Parameters.Add(in_v_artist_name);

                cur_updated_links_data.OracleDbType = OracleDbType.RefCursor;
                cur_updated_links_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_updated_links_data);

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
