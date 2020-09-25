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
   public  class CatalogueNotesDAL : ICatalogueNotesDAL
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

        public DataSet GetCatalogueNotes(string catNo, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pNotesData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_catalogue_notes.p_get_catalogue_notes", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

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

        public void SaveCatalogueNotes(string catNo, string notes, string loggedUser, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pcatNo = new OracleParameter();
                OracleParameter pNotes = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pNotesData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_catalogue_notes.p_save_catalogue_notes", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pcatNo.OracleDbType = OracleDbType.Varchar2;
                pcatNo.Direction = ParameterDirection.Input;
                pcatNo.Value = catNo;
                orlCmd.Parameters.Add(pcatNo);

                pNotes.OracleDbType = OracleDbType.Clob;
                pNotes.Direction = ParameterDirection.Input;
                pNotes.Value = notes;
                orlCmd.Parameters.Add(pNotes);

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
