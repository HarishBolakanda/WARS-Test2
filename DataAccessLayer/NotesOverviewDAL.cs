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
    public class NotesOverviewDAL : INotesOverviewDAL
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


        public DataSet GetNotesData (string royaltorId,string notesType,string statementPeriodId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pNotesType = new OracleParameter();
                OracleParameter pStatementPeriodId = new OracleParameter();
                OracleParameter pNotesData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_notes_overview.p_get_notes", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pNotesType.OracleDbType = OracleDbType.Varchar2;
                pNotesType.Direction = ParameterDirection.Input;
                pNotesType.Value = notesType;
                orlCmd.Parameters.Add(pNotesType);

                pStatementPeriodId.OracleDbType = OracleDbType.Varchar2;
                pStatementPeriodId.Direction = ParameterDirection.Input;
                pStatementPeriodId.Value = statementPeriodId;
                orlCmd.Parameters.Add(pStatementPeriodId);

                pNotesData.OracleDbType = OracleDbType.RefCursor;
                pNotesData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pNotesData);

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

        public DataSet GetWorkflowDropdown(string royaltorId,out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlDA = new OracleDataAdapter();

                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pWorkData = new OracleParameter();
                OracleParameter pStmtPeriod = new OracleParameter();

                orlCmd = new OracleCommand("pkg_notes_overview.p_get_statement_period_list", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pWorkData.OracleDbType = OracleDbType.RefCursor;
                pWorkData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pWorkData);

                pStmtPeriod.OracleDbType = OracleDbType.RefCursor;
                pStmtPeriod.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStmtPeriod);

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
