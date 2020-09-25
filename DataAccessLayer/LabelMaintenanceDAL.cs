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
    public class LabelMaintenanceDAL : ILabelMaintenanceDAL
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
                OracleParameter curLabelData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_label.p_get_label_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curLabelData.OracleDbType = OracleDbType.RefCursor;
                curLabelData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curLabelData);

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

        public DataSet UpdateLabelData(string labelCode, string labelDesc, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inlabelCode = new OracleParameter();
                OracleParameter inlabelDesc = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curlabelData = new OracleParameter();
                OracleParameter curLabelList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_label.p_update_label_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inlabelCode.OracleDbType = OracleDbType.Varchar2;
                inlabelCode.Direction = ParameterDirection.Input;
                inlabelCode.Value = labelCode;
                orlCmd.Parameters.Add(inlabelCode);

                inlabelDesc.OracleDbType = OracleDbType.Varchar2;
                inlabelDesc.Direction = ParameterDirection.Input;
                inlabelDesc.Value = labelDesc;
                orlCmd.Parameters.Add(inlabelDesc);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curlabelData.OracleDbType = OracleDbType.RefCursor;
                curlabelData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curlabelData);

                curLabelList.OracleDbType = OracleDbType.RefCursor;
                curLabelList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curLabelList);

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

        public DataSet InsertLabelData(string labelCode, string labelDesc, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inlabelCode = new OracleParameter();
                OracleParameter inlabelDesc = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curLabelList = new OracleParameter();
                OracleParameter curlabelData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_label.p_insert_label_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inlabelCode.OracleDbType = OracleDbType.Varchar2;
                inlabelCode.Direction = ParameterDirection.Input;
                inlabelCode.Value = labelCode;
                orlCmd.Parameters.Add(inlabelCode);

                inlabelDesc.OracleDbType = OracleDbType.Varchar2;
                inlabelDesc.Direction = ParameterDirection.Input;
                inlabelDesc.Value = labelDesc;
                orlCmd.Parameters.Add(inlabelDesc);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curlabelData.OracleDbType = OracleDbType.RefCursor;
                curlabelData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curlabelData);

                curLabelList.OracleDbType = OracleDbType.RefCursor;
                curLabelList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curLabelList);

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

        public DataSet DeleteLabelData(string labelCode, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inlabelCode = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curlabelData = new OracleParameter();
                OracleParameter curLabelList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_label.p_delete_label_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inlabelCode.OracleDbType = OracleDbType.Varchar2;
                inlabelCode.Direction = ParameterDirection.Input;
                inlabelCode.Value = labelCode;
                orlCmd.Parameters.Add(inlabelCode);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curlabelData.OracleDbType = OracleDbType.RefCursor;
                curlabelData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curlabelData);

                curLabelList.OracleDbType = OracleDbType.RefCursor;
                curLabelList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curLabelList);

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
