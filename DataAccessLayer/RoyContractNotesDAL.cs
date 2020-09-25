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
    public class RoyContractNotesDAL : IRoyContractNotesDAL
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

        public DataSet GetRoyContractNotes(string royaltorId, out string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();                
                OracleParameter pNotesData = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();


                orlCmd = new OracleCommand("pkg_roy_contract_notes.p_get_contract_notes", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pNotesData.OracleDbType = OracleDbType.RefCursor;
                pNotesData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pNotesData);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();


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

        public void SaveRoyContractNotes(string royaltorId, string notes, string loggedUser, out Int32 iErrorId)
        {            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pNotes = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pNotesData = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();


                orlCmd = new OracleCommand("pkg_roy_contract_notes.p_save_contract_notes", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

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
