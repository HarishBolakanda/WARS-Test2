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
    public class SellerGroupDAL : ISellerGroupDAL
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

        public DataSet GetSellerGroupList(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter cur_seller_group_list = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_seller_group.p_get_seller_groups", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                cur_seller_group_list.OracleDbType = OracleDbType.RefCursor;
                cur_seller_group_list.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_group_list);

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

        public DataSet GetSellerGroupInOutData(string sellerGroupCode, out Int32 locationCount, out Int32 iErrorId)
        {
            ds = new DataSet();
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_group_code = new OracleParameter();
                OracleParameter cur_grp_in_data = new OracleParameter();
                OracleParameter cur_grp_out_data = new OracleParameter();
                OracleParameter cur_seller_locations_in = new OracleParameter();
                OracleParameter cur_seller_locations_out = new OracleParameter();
                OracleParameter out_v_loction_count = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_seller_group.p_get_seller_in_out_grp_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_group_code.OracleDbType = OracleDbType.Varchar2;
                in_v_group_code.Direction = ParameterDirection.Input;
                in_v_group_code.Value = sellerGroupCode;
                orlCmd.Parameters.Add(in_v_group_code);

                cur_grp_in_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_in_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_in_data);

                cur_grp_out_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_out_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_out_data);

                cur_seller_locations_in.OracleDbType = OracleDbType.RefCursor;
                cur_seller_locations_in.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_locations_in);

                cur_seller_locations_out.OracleDbType = OracleDbType.RefCursor;
                cur_seller_locations_out.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_locations_out);

                out_v_loction_count.OracleDbType = OracleDbType.Int32;
                out_v_loction_count.Direction = ParameterDirection.Output;
                out_v_loction_count.ParameterName = "LoactionCount";
                orlCmd.Parameters.Add(out_v_loction_count);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                locationCount = Convert.ToInt32(orlCmd.Parameters["LoactionCount"].Value.ToString());                
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

            }
            catch (Exception ex)
            {
                locationCount = 0;
                iErrorId = 2;                
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }

        public DataSet AddTerritoryToGroup(string territoryGroupCode, Array territorycode, out Int32 locationCount, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_group_code = new OracleParameter();
                OracleParameter in_v_territory_code = new OracleParameter();
                OracleParameter cur_grp_in_data = new OracleParameter();
                OracleParameter cur_grp_out_data = new OracleParameter();
                OracleParameter cur_seller_locations_in = new OracleParameter();
                OracleParameter cur_seller_locations_out = new OracleParameter();
                OracleParameter out_v_loction_count = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_seller_group.p_add_seller_to_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_group_code.OracleDbType = OracleDbType.Varchar2;
                in_v_group_code.Direction = ParameterDirection.Input;
                in_v_group_code.Value = territoryGroupCode;
                orlCmd.Parameters.Add(in_v_group_code);

                in_v_territory_code.OracleDbType = OracleDbType.Varchar2;
                in_v_territory_code.Direction = ParameterDirection.Input;
                in_v_territory_code.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (territorycode.Length == 0)
                {
                    in_v_territory_code.Size = 0;
                    in_v_territory_code.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    in_v_territory_code.Size = territorycode.Length;
                    in_v_territory_code.Value = territorycode;
                }
                orlCmd.Parameters.Add(in_v_territory_code);

                cur_grp_in_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_in_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_in_data);

                cur_grp_out_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_out_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_out_data);

                cur_seller_locations_in.OracleDbType = OracleDbType.RefCursor;
                cur_seller_locations_in.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_locations_in);

                cur_seller_locations_out.OracleDbType = OracleDbType.RefCursor;
                cur_seller_locations_out.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_locations_out);

                out_v_loction_count.OracleDbType = OracleDbType.Int32;
                out_v_loction_count.Direction = ParameterDirection.Output;
                out_v_loction_count.ParameterName = "LoactionCount";
                orlCmd.Parameters.Add(out_v_loction_count);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                locationCount = Convert.ToInt32(orlCmd.Parameters["LoactionCount"].Value.ToString());  
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                locationCount = 0;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet RemoveTerritoryToGroup(string territoryGroupCode, Array territorycode, out Int32 locationCount, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_group_code = new OracleParameter();
                OracleParameter in_v_territory_code = new OracleParameter();
                OracleParameter cur_grp_in_data = new OracleParameter();
                OracleParameter cur_grp_out_data = new OracleParameter();
                OracleParameter cur_seller_locations_in = new OracleParameter();
                OracleParameter cur_seller_locations_out = new OracleParameter();
                OracleParameter out_v_loction_count = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_seller_group.p_remove_seller_from_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_group_code.OracleDbType = OracleDbType.Varchar2;
                in_v_group_code.Direction = ParameterDirection.Input;
                in_v_group_code.Value = territoryGroupCode;
                orlCmd.Parameters.Add(in_v_group_code);

                in_v_territory_code.OracleDbType = OracleDbType.Varchar2;
                in_v_territory_code.Direction = ParameterDirection.Input;
                in_v_territory_code.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (territorycode.Length == 0)
                {
                    in_v_territory_code.Size = 0;
                    in_v_territory_code.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    in_v_territory_code.Size = territorycode.Length;
                    in_v_territory_code.Value = territorycode;
                }
                orlCmd.Parameters.Add(in_v_territory_code);

                cur_grp_in_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_in_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_in_data);

                cur_grp_out_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_out_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_out_data);

                cur_seller_locations_in.OracleDbType = OracleDbType.RefCursor;
                cur_seller_locations_in.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_locations_in);

                cur_seller_locations_out.OracleDbType = OracleDbType.RefCursor;
                cur_seller_locations_out.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_locations_out);

                out_v_loction_count.OracleDbType = OracleDbType.Int32;
                out_v_loction_count.Direction = ParameterDirection.Output;
                out_v_loction_count.ParameterName = "LoactionCount";
                orlCmd.Parameters.Add(out_v_loction_count);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                locationCount = Convert.ToInt32(orlCmd.Parameters["LoactionCount"].Value.ToString());  
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                locationCount = 0;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet InsertTerritoryGroup(string territoryGroupCode, string territoryGroupName, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter in_v_group_code = new OracleParameter();
                OracleParameter in_v_group_name = new OracleParameter();
                OracleParameter cur_grp_in_data = new OracleParameter();
                OracleParameter cur_grp_out_data = new OracleParameter();
                OracleParameter cur_seller_locations_in = new OracleParameter();
                OracleParameter cur_seller_locations_out = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_seller_group.p_insert_seller_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                in_v_group_code.OracleDbType = OracleDbType.Varchar2;
                in_v_group_code.Direction = ParameterDirection.Input;
                in_v_group_code.Value = territoryGroupCode;
                orlCmd.Parameters.Add(in_v_group_code);

                in_v_group_name.OracleDbType = OracleDbType.Varchar2;
                in_v_group_name.Direction = ParameterDirection.Input;
                in_v_group_name.Value = territoryGroupName;
                orlCmd.Parameters.Add(in_v_group_name);

                cur_grp_in_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_in_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_in_data);

                cur_grp_out_data.OracleDbType = OracleDbType.RefCursor;
                cur_grp_out_data.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_grp_out_data);

                cur_seller_locations_in.OracleDbType = OracleDbType.RefCursor;
                cur_seller_locations_in.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_locations_in);

                cur_seller_locations_out.OracleDbType = OracleDbType.RefCursor;
                cur_seller_locations_out.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(cur_seller_locations_out);

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
