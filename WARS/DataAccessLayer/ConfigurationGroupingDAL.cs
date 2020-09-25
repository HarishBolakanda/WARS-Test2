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
    public class ConfigurationGroupingDAL : IConfigurationGroupingDAL
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

        public DataSet GetConfigurationGroupList(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter curConfigGroupList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_configuration_group.p_get_config_groups", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curConfigGroupList.OracleDbType = OracleDbType.RefCursor;
                curConfigGroupList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curConfigGroupList);

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

        public DataSet GetConfigurationGroupInOutData(string configGroupCode, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inGroupCode = new OracleParameter();
                OracleParameter curGroupInData = new OracleParameter();
                OracleParameter curGroupOutdata = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_configuration_group.p_get_config_in_out_grp_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inGroupCode.OracleDbType = OracleDbType.Varchar2;
                inGroupCode.Direction = ParameterDirection.Input;
                inGroupCode.Value = configGroupCode;
                orlCmd.Parameters.Add(inGroupCode);

                curGroupInData.OracleDbType = OracleDbType.RefCursor;
                curGroupInData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupInData);

                curGroupOutdata.OracleDbType = OracleDbType.RefCursor;
                curGroupOutdata.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupOutdata);

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

        public DataSet AddConfigurationToGroup(string configGroupCode, Array configcodes, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inGroupCode = new OracleParameter();
                OracleParameter inConfigCodes = new OracleParameter();
                OracleParameter curGroupInData = new OracleParameter();
                OracleParameter curGroupOutdata = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_configuration_group.p_add_config_to_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inGroupCode.OracleDbType = OracleDbType.Varchar2;
                inGroupCode.Direction = ParameterDirection.Input;
                inGroupCode.Value = configGroupCode;
                orlCmd.Parameters.Add(inGroupCode);

                inConfigCodes.OracleDbType = OracleDbType.Varchar2;
                inConfigCodes.Direction = ParameterDirection.Input;
                inConfigCodes.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (configcodes.Length == 0)
                {
                    inConfigCodes.Size = 0;
                    inConfigCodes.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inConfigCodes.Size = configcodes.Length;
                    inConfigCodes.Value = configcodes;
                }
                orlCmd.Parameters.Add(inConfigCodes);

                curGroupInData.OracleDbType = OracleDbType.RefCursor;
                curGroupInData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupInData);

                curGroupOutdata.OracleDbType = OracleDbType.RefCursor;
                curGroupOutdata.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupOutdata);

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

        public DataSet RemoveConfigurationFromGroup(string configGroupCode, Array configcodes, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inGroupCode = new OracleParameter();
                OracleParameter inConfigCodes = new OracleParameter();
                OracleParameter curGroupInData = new OracleParameter();
                OracleParameter curGroupOutData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_configuration_group.p_remove_config_from_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inGroupCode.OracleDbType = OracleDbType.Varchar2;
                inGroupCode.Direction = ParameterDirection.Input;
                inGroupCode.Value = configGroupCode;
                orlCmd.Parameters.Add(inGroupCode);

                inConfigCodes.OracleDbType = OracleDbType.Varchar2;
                inConfigCodes.Direction = ParameterDirection.Input;
                inConfigCodes.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (configcodes.Length == 0)
                {
                    inConfigCodes.Size = 0;
                    inConfigCodes.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inConfigCodes.Size = configcodes.Length;
                    inConfigCodes.Value = configcodes;
                }
                orlCmd.Parameters.Add(inConfigCodes);

                curGroupInData.OracleDbType = OracleDbType.RefCursor;
                curGroupInData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupInData);

                curGroupOutData.OracleDbType = OracleDbType.RefCursor;
                curGroupOutData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupOutData);

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

        public DataSet InsertConfigurationGroup(string configGroupCode, string configGroupName, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inGroupCode = new OracleParameter();
                OracleParameter inGroupName = new OracleParameter();
                OracleParameter curGroupInData = new OracleParameter();
                OracleParameter curGroupOutData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_configuration_group.p_insert_config_group", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inGroupCode.OracleDbType = OracleDbType.Varchar2;
                inGroupCode.Direction = ParameterDirection.Input;
                inGroupCode.Value = configGroupCode;
                orlCmd.Parameters.Add(inGroupCode);

                inGroupName.OracleDbType = OracleDbType.Varchar2;
                inGroupName.Direction = ParameterDirection.Input;
                inGroupName.Value = configGroupName;
                orlCmd.Parameters.Add(inGroupName);

                curGroupInData.OracleDbType = OracleDbType.RefCursor;
                curGroupInData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupInData);

                curGroupOutData.OracleDbType = OracleDbType.RefCursor;
                curGroupOutData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curGroupOutData);

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
