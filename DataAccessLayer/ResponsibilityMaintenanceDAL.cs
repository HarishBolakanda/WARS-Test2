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
    public class ResponsibilityMaintenanceDAL : IResponsibilityMaintenanceDAL
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

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter curResponsibilityData = new OracleParameter();
                OracleParameter curManagerRespList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_responsibility.p_get_responsibility_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curResponsibilityData.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityData);

                curManagerRespList.OracleDbType = OracleDbType.RefCursor;
                curManagerRespList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curManagerRespList);

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
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet UpdateResponsibilityData(string responsibilityCode, string responsibilityDesc, string managerResponsibility, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inResponsibilityCode = new OracleParameter();
                OracleParameter inResponsibilityDesc = new OracleParameter();
                OracleParameter inManagerResponsibility = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curResponsibilityData = new OracleParameter();
                OracleParameter curManagerRespList = new OracleParameter();
                OracleParameter curResponsibilityList = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_responsibility.p_update_responsibility_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inResponsibilityCode.OracleDbType = OracleDbType.Varchar2;
                inResponsibilityCode.Direction = ParameterDirection.Input;
                inResponsibilityCode.Value = responsibilityCode;
                orlCmd.Parameters.Add(inResponsibilityCode);

                inResponsibilityDesc.OracleDbType = OracleDbType.Varchar2;
                inResponsibilityDesc.Direction = ParameterDirection.Input;
                inResponsibilityDesc.Value = responsibilityDesc;
                orlCmd.Parameters.Add(inResponsibilityDesc);

                inManagerResponsibility.OracleDbType = OracleDbType.Varchar2;
                inManagerResponsibility.Direction = ParameterDirection.Input;
                inManagerResponsibility.Value = managerResponsibility;
                orlCmd.Parameters.Add(inManagerResponsibility);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curResponsibilityData.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityData);

                curManagerRespList.OracleDbType = OracleDbType.RefCursor;
                curManagerRespList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curManagerRespList);

                curResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityList);                

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
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet InsertResponsibilityData(string responsibilityCode, string responsibilityDesc, string managerResponsibility, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inResponsibilityCode = new OracleParameter();
                OracleParameter inResponsibilityDesc = new OracleParameter();
                OracleParameter inManagerResponsibility = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curResponsibilityData = new OracleParameter();
                OracleParameter curManagerRespList = new OracleParameter();
                OracleParameter curResponsibilityList = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_responsibility.p_insert_responsibility_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inResponsibilityCode.OracleDbType = OracleDbType.Varchar2;
                inResponsibilityCode.Direction = ParameterDirection.Input;
                inResponsibilityCode.Value = responsibilityCode;
                orlCmd.Parameters.Add(inResponsibilityCode);

                inResponsibilityDesc.OracleDbType = OracleDbType.Varchar2;
                inResponsibilityDesc.Direction = ParameterDirection.Input;
                inResponsibilityDesc.Value = responsibilityDesc;
                orlCmd.Parameters.Add(inResponsibilityDesc);

                inManagerResponsibility.OracleDbType = OracleDbType.Varchar2;
                inManagerResponsibility.Direction = ParameterDirection.Input;
                inManagerResponsibility.Value = managerResponsibility;
                orlCmd.Parameters.Add(inManagerResponsibility);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curResponsibilityData.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityData);

                curManagerRespList.OracleDbType = OracleDbType.RefCursor;
                curManagerRespList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curManagerRespList);

                curResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityList);

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
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet DeleteResponsibilityData(string responsibilityCode, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inResponsibilityCode = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curResponsibilityData = new OracleParameter();
                OracleParameter curManagerRespList = new OracleParameter();
                OracleParameter curResponsibilityList = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_responsibility.p_delete_responsibility_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inResponsibilityCode.OracleDbType = OracleDbType.Varchar2;
                inResponsibilityCode.Direction = ParameterDirection.Input;
                inResponsibilityCode.Value = responsibilityCode;
                orlCmd.Parameters.Add(inResponsibilityCode);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curResponsibilityData.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityData);

                curManagerRespList.OracleDbType = OracleDbType.RefCursor;
                curManagerRespList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curManagerRespList);

                curResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityList);                

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
                throw ex;
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
