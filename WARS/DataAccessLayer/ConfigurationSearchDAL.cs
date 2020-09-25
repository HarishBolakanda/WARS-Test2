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
    public class ConfigurationSearchDAL : IConfigurationSearchDAL
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

        public DataSet GetConfigurationsList(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter curConfigurationList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_configuration_search.p_get_configurations", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curConfigurationList.OracleDbType = OracleDbType.RefCursor;
                curConfigurationList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curConfigurationList);

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

        public DataSet GetConfigurationData(string ConfigurationCode, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter inConfigurationCode = new OracleParameter();
                OracleParameter curConfigurationData = new OracleParameter();
                OracleParameter curConfigurationGroupData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_configuration_search.p_get_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inConfigurationCode.OracleDbType = OracleDbType.Varchar2;
                inConfigurationCode.Direction = ParameterDirection.Input;
                inConfigurationCode.Value = ConfigurationCode;
                orlCmd.Parameters.Add(inConfigurationCode);

                curConfigurationData.OracleDbType = OracleDbType.RefCursor;
                curConfigurationData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curConfigurationData);

                curConfigurationGroupData.OracleDbType = OracleDbType.RefCursor;
                curConfigurationGroupData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curConfigurationGroupData);

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
