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
    public class StmtProgressDashboardDAL : IStmtProgressDashboardDAL
    {

        #region Global Declarations        
        ConnectionDAL connDAL;
        OracleConnection orlConn;        
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
                orlDA = new OracleDataAdapter();
                                
                OracleParameter pStmtData = new OracleParameter();
                OracleParameter pResponsibility = new OracleParameter();
                orlCmd = new OracleCommand("pkg_stmt_dashboard_screen.p_get_initial_stmt_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                
                pStmtData.OracleDbType = OracleDbType.RefCursor;
                pStmtData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtData);

                pResponsibility.OracleDbType = OracleDbType.RefCursor;
                pResponsibility.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pResponsibility);

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

        public DataSet GetFilterData(string stmtRptDays, string rptDaysPlus,string respCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter pStmtRptDays = new OracleParameter();
                OracleParameter pRptDaysPlus = new OracleParameter();
                OracleParameter pStmtData = new OracleParameter();
                OracleParameter pRemainingDays = new OracleParameter();
                OracleParameter pRespCode = new OracleParameter();
                orlCmd = new OracleCommand("pkg_stmt_dashboard_screen.p_get_filter_stmt_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pStmtRptDays.OracleDbType = OracleDbType.Varchar2;
                pStmtRptDays.Direction = ParameterDirection.Input;
                if (stmtRptDays != string.Empty)
                {
                    pStmtRptDays.Value = stmtRptDays;
                }
                else
                {
                    pStmtRptDays.Value = "-";
                }
                orlCmd.Parameters.Add(pStmtRptDays);

                pRptDaysPlus.OracleDbType = OracleDbType.Varchar2;
                pRptDaysPlus.Direction = ParameterDirection.Input;
                pRptDaysPlus.Value = rptDaysPlus;
                orlCmd.Parameters.Add(pRptDaysPlus);

                pRespCode.OracleDbType = OracleDbType.Varchar2;
                pRespCode.Direction = ParameterDirection.Input;
                pRespCode.Value = respCode;
                orlCmd.Parameters.Add(pRespCode);

                pStmtData.OracleDbType = OracleDbType.RefCursor;
                pStmtData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtData);

                pRemainingDays.OracleDbType = OracleDbType.RefCursor;
                pRemainingDays.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRemainingDays);

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
