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
    public class RoyaltorGroupingsDAL : IRoyaltorGroupingsDAL
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

        public DataSet GetRoyaltorGroupings(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter cur_royaltors_group_type = new OracleParameter();
                OracleParameter cur_grp_statement_summary = new OracleParameter();
                OracleParameter cur_grp_summary_statement = new OracleParameter();
                OracleParameter cur_grp_txt_detail_statement = new OracleParameter();
                OracleParameter cur_grp_gfs_royaltor = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_royaltor_groupings.p_get_royaltor_group_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                cur_royaltors_group_type.OracleDbType = OracleDbType.RefCursor;
                cur_royaltors_group_type.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltors_group_type);

                cur_grp_statement_summary.OracleDbType = OracleDbType.RefCursor;
                cur_grp_statement_summary.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_statement_summary);

                cur_grp_summary_statement.OracleDbType = OracleDbType.RefCursor;
                cur_grp_summary_statement.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_summary_statement);

                cur_grp_txt_detail_statement.OracleDbType = OracleDbType.RefCursor;
                cur_grp_txt_detail_statement.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_txt_detail_statement);

                cur_grp_gfs_royaltor.OracleDbType = OracleDbType.RefCursor;
                cur_grp_gfs_royaltor.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_gfs_royaltor);

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

        public DataSet GetRoyaltorGroupingsInOutData(Int32 grpTypeCode, Int32 grpId,out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_group_type_code = new OracleParameter();
                OracleParameter in_v_group_id = new OracleParameter();
                //OracleParameter in_v_group_name = new OracleParameter();
                OracleParameter cur_grp_in_data = new OracleParameter();
                OracleParameter cur_grp_out_data = new OracleParameter();
                OracleParameter cur_royaltor_out_list = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_royaltor_groupings.p_get_roy_in_out_grp_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_group_type_code.OracleDbType = OracleDbType.Int32;
                in_v_group_type_code.Direction = ParameterDirection.Input;
                in_v_group_type_code.Value = grpTypeCode;
                orlCmd.Parameters.Add(in_v_group_type_code);

                in_v_group_id.OracleDbType = OracleDbType.Int32;
                in_v_group_id.Direction = ParameterDirection.Input;
                in_v_group_id.Value = grpId;
                orlCmd.Parameters.Add(in_v_group_id);

                //in_v_group_name.OracleDbType = OracleDbType.Varchar2;
                //in_v_group_name.Direction = ParameterDirection.Input;
                //in_v_group_name.Value = grpName;
                //orlCmd.Parameters.Add(in_v_group_name);

                cur_grp_in_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_in_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_in_data);

                cur_grp_out_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_out_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_out_data);

                cur_royaltor_out_list.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_out_list.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_out_list);

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

        public DataSet AddRoyaltorToGroup(Int32 grpTypeCode, Int32 grpId, Int32 searchedRoyaltorId, Array royaltorId, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_group_type_code = new OracleParameter();
                OracleParameter in_v_group_id = new OracleParameter();
                //OracleParameter in_v_group_name = new OracleParameter();
                OracleParameter in_v_searched_royaltor_id = new OracleParameter();
                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_user_code = new OracleParameter();
                OracleParameter cur_grp_in_data = new OracleParameter();
                OracleParameter cur_grp_out_data = new OracleParameter();
                OracleParameter cur_royaltors_group_type = new OracleParameter();
                OracleParameter cur_grp_statement_summary = new OracleParameter();
                OracleParameter cur_grp_summary_statement = new OracleParameter();
                OracleParameter cur_grp_txt_detail_statement = new OracleParameter();
                OracleParameter cur_grp_gfs_royaltor = new OracleParameter();
                OracleParameter cur_royaltor_out_list = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_royaltor_groupings.p_add_royaltor_to_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_group_type_code.OracleDbType = OracleDbType.Int32;
                in_v_group_type_code.Direction = ParameterDirection.Input;
                in_v_group_type_code.Value = grpTypeCode;
                orlCmd.Parameters.Add(in_v_group_type_code);

                in_v_group_id.OracleDbType = OracleDbType.Int32;
                in_v_group_id.Direction = ParameterDirection.Input;
                in_v_group_id.Value = grpId;
                orlCmd.Parameters.Add(in_v_group_id);

                in_v_searched_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_searched_royaltor_id.Direction = ParameterDirection.Input;
                in_v_searched_royaltor_id.Value = searchedRoyaltorId;
                orlCmd.Parameters.Add(in_v_searched_royaltor_id);

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                in_v_royaltor_id.Size = royaltorId.Length;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                in_v_user_code.OracleDbType = OracleDbType.Varchar2;
                in_v_user_code.Direction = ParameterDirection.Input;
                in_v_user_code.Value = userCode;
                orlCmd.Parameters.Add(in_v_user_code);

                cur_grp_in_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_in_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_in_data);

                cur_grp_out_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_out_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_out_data);

                cur_royaltors_group_type.OracleDbType = OracleDbType.RefCursor;
                cur_royaltors_group_type.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltors_group_type);

                cur_grp_statement_summary.OracleDbType = OracleDbType.RefCursor;
                cur_grp_statement_summary.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_statement_summary);

                cur_grp_summary_statement.OracleDbType = OracleDbType.RefCursor;
                cur_grp_summary_statement.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_summary_statement);

                cur_grp_txt_detail_statement.OracleDbType = OracleDbType.RefCursor;
                cur_grp_txt_detail_statement.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_txt_detail_statement);

                cur_grp_gfs_royaltor.OracleDbType = OracleDbType.RefCursor;
                cur_grp_gfs_royaltor.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_gfs_royaltor);

                cur_royaltor_out_list.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_out_list.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_out_list);

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

        public DataSet RemoveRoyaltorFromGroup(Int32 grpTypeCode, Int32 grpId, Int32 searchedRoyaltorId, Array royaltorId, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_group_type_code = new OracleParameter();
                OracleParameter in_v_group_id = new OracleParameter();
                //OracleParameter in_v_group_name = new OracleParameter();
                OracleParameter in_v_searched_royaltor_id = new OracleParameter();
                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter in_v_user_code = new OracleParameter();
                //OracleParameter in_v_royaltor_name = new OracleParameter();
                OracleParameter cur_grp_in_data = new OracleParameter();
                OracleParameter cur_grp_out_data = new OracleParameter();
                OracleParameter cur_royaltors_group_type = new OracleParameter();
                OracleParameter cur_grp_statement_summary = new OracleParameter();
                OracleParameter cur_grp_summary_statement = new OracleParameter();
                OracleParameter cur_grp_txt_detail_statement = new OracleParameter();
                OracleParameter cur_grp_gfs_royaltor = new OracleParameter();
                OracleParameter cur_royaltor_out_list = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_royaltor_groupings.p_remove_royaltor_from_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_group_type_code.OracleDbType = OracleDbType.Int32;
                in_v_group_type_code.Direction = ParameterDirection.Input;
                in_v_group_type_code.Value = grpTypeCode;
                orlCmd.Parameters.Add(in_v_group_type_code);

                in_v_group_id.OracleDbType = OracleDbType.Int32;
                in_v_group_id.Direction = ParameterDirection.Input;
                in_v_group_id.Value = grpId;
                orlCmd.Parameters.Add(in_v_group_id);

                in_v_searched_royaltor_id.OracleDbType = OracleDbType.Varchar2;
                in_v_searched_royaltor_id.Direction = ParameterDirection.Input;
                in_v_searched_royaltor_id.Value = searchedRoyaltorId;
                orlCmd.Parameters.Add(in_v_searched_royaltor_id);

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                in_v_royaltor_id.Size = royaltorId.Length;
                in_v_royaltor_id.Value = royaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                //in_v_royaltor_name.OracleDbType = OracleDbType.Varchar2;
                //in_v_royaltor_name.Direction = ParameterDirection.Input;
                //in_v_royaltor_name.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                //in_v_royaltor_name.Size = royaltorId.Length;
                //in_v_royaltor_name.Value = royaltorName;
                //orlCmd.Parameters.Add(in_v_royaltor_name);

                in_v_user_code.OracleDbType = OracleDbType.Varchar2;
                in_v_user_code.Direction = ParameterDirection.Input;
                in_v_user_code.Value = userCode;
                orlCmd.Parameters.Add(in_v_user_code);

                cur_grp_in_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_in_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_in_data);

                cur_grp_out_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_out_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_out_data);

                cur_royaltors_group_type.OracleDbType = OracleDbType.RefCursor;
                cur_royaltors_group_type.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltors_group_type);

                cur_grp_statement_summary.OracleDbType = OracleDbType.RefCursor;
                cur_grp_statement_summary.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_statement_summary);

                cur_grp_summary_statement.OracleDbType = OracleDbType.RefCursor;
                cur_grp_summary_statement.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_summary_statement);

                cur_grp_txt_detail_statement.OracleDbType = OracleDbType.RefCursor;
                cur_grp_txt_detail_statement.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_txt_detail_statement);

                cur_grp_gfs_royaltor.OracleDbType = OracleDbType.RefCursor;
                cur_grp_gfs_royaltor.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_gfs_royaltor);

                cur_royaltor_out_list.OracleDbType = OracleDbType.RefCursor;
                cur_royaltor_out_list.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_royaltor_out_list);

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

        public DataSet GetUpdatedOutData(Int32 grpTypeCode, Int32 grpId, Int32 searchedRoyaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_group_type_code = new OracleParameter();
                OracleParameter in_v_group_id = new OracleParameter();
                OracleParameter in_v_royaltor_id = new OracleParameter();
                OracleParameter cur_grp_out_data = new OracleParameter();                

                orlCmd = new OracleCommand("pkg_maint_royaltor_groupings.p_get_roy_updated_out_grp_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_group_type_code.OracleDbType = OracleDbType.Int32;
                in_v_group_type_code.Direction = ParameterDirection.Input;
                in_v_group_type_code.Value = grpTypeCode;
                orlCmd.Parameters.Add(in_v_group_type_code);

                in_v_group_id.OracleDbType = OracleDbType.Int32;
                in_v_group_id.Direction = ParameterDirection.Input;
                in_v_group_id.Value = grpId;
                orlCmd.Parameters.Add(in_v_group_id);

                in_v_royaltor_id.OracleDbType = OracleDbType.Int32;
                in_v_royaltor_id.Direction = ParameterDirection.Input;
                in_v_royaltor_id.Value = searchedRoyaltorId;
                orlCmd.Parameters.Add(in_v_royaltor_id);

                cur_grp_out_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_out_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_out_data);

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

        public void GenerateGroupSummaries(string loggedUser, Int32 groupId, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pGroupId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_royaltor_groupings.p_generate_group_summary", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pGroupId.OracleDbType = OracleDbType.Int32;
                pGroupId.Direction = ParameterDirection.Input;
                pGroupId.Value = groupId;
                orlCmd.Parameters.Add(pGroupId);

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


        public void GenerateAllGroupSummaries(string loggedUser, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_royaltor_groupings.p_generate_all_summaries", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

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
