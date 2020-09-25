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
    public class ArtistAuditDAL : IArtistAuditDAL
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

        public DataSet GetArtistAuditData(Int32 artistId, out string artist, out Int32 iErrorId)
        {
            ds = new DataSet();
            artist = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter pArtistId = new OracleParameter();
                OracleParameter pArtistAuditDetails = new OracleParameter();
                OracleParameter pArtist = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_artist_audit.p_get_artist_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pArtistId.OracleDbType = OracleDbType.Int32;
                pArtistId.Direction = ParameterDirection.Input;
                pArtistId.Value = artistId;
                orlCmd.Parameters.Add(pArtistId);

                pArtistAuditDetails.OracleDbType = OracleDbType.RefCursor;
                pArtistAuditDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pArtistAuditDetails);

                pArtist.OracleDbType = OracleDbType.Varchar2;
                pArtist.Size = 250;
                pArtist.Direction = ParameterDirection.Output;
                pArtist.ParameterName = "out_v_artist";
                orlCmd.Parameters.Add(pArtist);


                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                artist = orlCmd.Parameters["out_v_artist"].Value.ToString();

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
