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
    public class OwnerMaintenanceDAL : IOwnerMaintenanceDAL
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

        public DataSet GetInitialData(out Int32 newOwnerCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            newOwnerCode = 0;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter curOwnerData = new OracleParameter();
                OracleParameter pNewOwnerCode = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_owner.p_get_owner_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curOwnerData.OracleDbType = OracleDbType.RefCursor;
                curOwnerData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curOwnerData);

                pNewOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pNewOwnerCode.Size = 200;
                pNewOwnerCode.Direction = ParameterDirection.Output;
                pNewOwnerCode.ParameterName = "NewOwnerCode";
                orlCmd.Parameters.Add(pNewOwnerCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                newOwnerCode = Convert.ToInt32(orlCmd.Parameters["NewOwnerCode"].Value.ToString());
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

        public DataSet UpdateOwnerData(Int32 ownerCode, string ownerName, string userCode, out Int32 newOwnerCode, out Int32 iErrorId)
        {
            ds = new DataSet(); 
            newOwnerCode = 0;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inOwnerCode = new OracleParameter();
                OracleParameter inOwnerName = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curOwnerData = new OracleParameter();
                OracleParameter curOwnerList = new OracleParameter();
                OracleParameter pNewOwnerCode = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_owner.p_update_owner_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inOwnerCode.OracleDbType = OracleDbType.Int32;
                inOwnerCode.Direction = ParameterDirection.Input;
                inOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(inOwnerCode);

                inOwnerName.OracleDbType = OracleDbType.Varchar2;
                inOwnerName.Direction = ParameterDirection.Input;
                inOwnerName.Value = ownerName;
                orlCmd.Parameters.Add(inOwnerName);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curOwnerData.OracleDbType = OracleDbType.RefCursor;
                curOwnerData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curOwnerData);

                curOwnerList.OracleDbType = OracleDbType.RefCursor;
                curOwnerList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curOwnerList);

                pNewOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pNewOwnerCode.Size = 200;
                pNewOwnerCode.Direction = ParameterDirection.Output;
                pNewOwnerCode.ParameterName = "NewOwnerCode";
                orlCmd.Parameters.Add(pNewOwnerCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                newOwnerCode = Convert.ToInt32(orlCmd.Parameters["NewOwnerCode"].Value.ToString());
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

        public DataSet InsertOwnerData(Int32 ownerCode, string ownerName, string userCode, out Int32 newOwnerCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            newOwnerCode = 0;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inOwnerCode = new OracleParameter();
                OracleParameter inOwnerName = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curOwnerData = new OracleParameter();
                OracleParameter curOwnerList = new OracleParameter();
                OracleParameter pNewOwnerCode = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_owner.p_insert_owner_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inOwnerCode.OracleDbType = OracleDbType.Varchar2;
                inOwnerCode.Direction = ParameterDirection.Input;
                inOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(inOwnerCode);

                inOwnerName.OracleDbType = OracleDbType.Varchar2;
                inOwnerName.Direction = ParameterDirection.Input;
                inOwnerName.Value = ownerName;
                orlCmd.Parameters.Add(inOwnerName);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curOwnerData.OracleDbType = OracleDbType.RefCursor;
                curOwnerData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curOwnerData);

                curOwnerList.OracleDbType = OracleDbType.RefCursor;
                curOwnerList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curOwnerList);

                pNewOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pNewOwnerCode.Size = 200;
                pNewOwnerCode.Direction = ParameterDirection.Output;
                pNewOwnerCode.ParameterName = "NewOwnerCode";
                orlCmd.Parameters.Add(pNewOwnerCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                newOwnerCode = Convert.ToInt32(orlCmd.Parameters["NewOwnerCode"].Value.ToString());
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

        public DataSet DeleteOwnerData(Int32 ownerCode, string userCode, out Int32 newOwnerCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            newOwnerCode = 0;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inOwnerCode = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curOwnerData = new OracleParameter();
                OracleParameter curOwnerList = new OracleParameter();
                OracleParameter pNewOwnerCode = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_owner.p_delete_owner_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inOwnerCode.OracleDbType = OracleDbType.Int32;
                inOwnerCode.Direction = ParameterDirection.Input;
                inOwnerCode.Value = ownerCode;
                orlCmd.Parameters.Add(inOwnerCode);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curOwnerData.OracleDbType = OracleDbType.RefCursor;
                curOwnerData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curOwnerData);

                curOwnerList.OracleDbType = OracleDbType.RefCursor;
                curOwnerList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curOwnerList);

                pNewOwnerCode.OracleDbType = OracleDbType.Varchar2;
                pNewOwnerCode.Size = 200;
                pNewOwnerCode.Direction = ParameterDirection.Output;
                pNewOwnerCode.ParameterName = "NewOwnerCode";
                orlCmd.Parameters.Add(pNewOwnerCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                newOwnerCode = Convert.ToInt32(orlCmd.Parameters["NewOwnerCode"].Value.ToString());
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
