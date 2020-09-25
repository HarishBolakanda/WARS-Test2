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
                OracleParameter pCompany = new OracleParameter();
                OracleParameter pStatementPeriod = new OracleParameter();
                OracleParameter pTeamResponsibility = new OracleParameter();
                OracleParameter pMngrResponsibility = new OracleParameter();
                OracleParameter pStmtData = new OracleParameter();
                orlCmd = new OracleCommand("pkg_stmt_dashboard_screen.p_get_initial_stmt_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCompany.OracleDbType = OracleDbType.RefCursor;
                pCompany.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCompany);

                pStatementPeriod.OracleDbType = OracleDbType.RefCursor;
                pStatementPeriod.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStatementPeriod);

                pTeamResponsibility.OracleDbType = OracleDbType.RefCursor;
                pTeamResponsibility.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTeamResponsibility);

                pMngrResponsibility.OracleDbType = OracleDbType.RefCursor;
                pMngrResponsibility.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pMngrResponsibility);
                
                pStmtData.OracleDbType = OracleDbType.RefCursor;
                pStmtData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtData);

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

        public DataSet GetFilterData(string companyCode, string repSchedule, string teamResponsibility, string mngrResponsibility, string earningsCompare, string earnings, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter pCompanyCode = new OracleParameter();
                OracleParameter pRepSchedule = new OracleParameter();
                OracleParameter pTeamResponsibility = new OracleParameter();
                OracleParameter pMngrResponsibility = new OracleParameter();
                OracleParameter pEarningsCompare = new OracleParameter();
                OracleParameter pEarnings = new OracleParameter();
                OracleParameter pStmtData = new OracleParameter();
                OracleParameter pRemainingDays = new OracleParameter();
                orlCmd = new OracleCommand("pkg_stmt_dashboard_screen.p_get_filter_stmt_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;


                pCompanyCode.OracleDbType = OracleDbType.Varchar2;
                pCompanyCode.Direction = ParameterDirection.Input;
                pCompanyCode.Value = companyCode;
                orlCmd.Parameters.Add(pCompanyCode);

                pRepSchedule.OracleDbType = OracleDbType.Varchar2;
                pRepSchedule.Direction = ParameterDirection.Input;
                pRepSchedule.Value = repSchedule;
                orlCmd.Parameters.Add(pRepSchedule);

                pTeamResponsibility.OracleDbType = OracleDbType.Varchar2;
                pTeamResponsibility.Direction = ParameterDirection.Input;
                pTeamResponsibility.Value = teamResponsibility;
                orlCmd.Parameters.Add(pTeamResponsibility);

                pMngrResponsibility.OracleDbType = OracleDbType.Varchar2;
                pMngrResponsibility.Direction = ParameterDirection.Input;
                pMngrResponsibility.Value = mngrResponsibility;
                orlCmd.Parameters.Add(pMngrResponsibility);

                pEarningsCompare.OracleDbType = OracleDbType.Varchar2;
                pEarningsCompare.Direction = ParameterDirection.Input;
                pEarningsCompare.Value = earningsCompare;
                orlCmd.Parameters.Add(pEarningsCompare);

                pEarnings.OracleDbType = OracleDbType.Varchar2;
                pEarnings.Direction = ParameterDirection.Input;
                pEarnings.Value = earnings;
                orlCmd.Parameters.Add(pEarnings);

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
