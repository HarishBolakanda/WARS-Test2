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
    public class ArtistResponsibilityMaintenanceDAL : IArtistResponsibilityMaintenanceDAL
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
                OracleParameter curArtistData = new OracleParameter();
                OracleParameter curResponsibilityList = new OracleParameter();
                OracleParameter curDataTypeList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_artist.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curArtistData.OracleDbType = OracleDbType.RefCursor;
                curArtistData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curArtistData);

                curResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityList);

                curDataTypeList.OracleDbType = OracleDbType.RefCursor;
                curDataTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curDataTypeList);

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

        public DataSet GetSearchedArtistData(string artistName,string dealType,string teamResponsibility, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inArtistName = new OracleParameter();
                OracleParameter inDealType = new OracleParameter();
                OracleParameter inTeamResponsibility = new OracleParameter();
                OracleParameter curArtistData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_artist.p_get_artist_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inArtistName.OracleDbType = OracleDbType.Varchar2;
                inArtistName.Direction = ParameterDirection.Input;
                inArtistName.Value = artistName;
                orlCmd.Parameters.Add(inArtistName);

                inDealType.OracleDbType = OracleDbType.Varchar2;
                inDealType.Direction = ParameterDirection.Input;
                inDealType.Value = dealType;
                orlCmd.Parameters.Add(inDealType);

                inTeamResponsibility.OracleDbType = OracleDbType.Varchar2;
                inTeamResponsibility.Direction = ParameterDirection.Input;
                inTeamResponsibility.Value = teamResponsibility;
                orlCmd.Parameters.Add(inTeamResponsibility);

                curArtistData.OracleDbType = OracleDbType.RefCursor;
                curArtistData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curArtistData);

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

        public DataSet UpdateArtistData(string artistId, string teamResponsibility, string mngrResponsibility,
            string userCode, string artistName, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inArtistId = new OracleParameter();
                OracleParameter inTeamResponsibility = new OracleParameter();
                OracleParameter inMngrResponsibility = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter inArtistName = new OracleParameter();
                OracleParameter curArtistData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_artist.p_update_artist_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inArtistId.OracleDbType = OracleDbType.Varchar2;
                inArtistId.Direction = ParameterDirection.Input;
                inArtistId.Value = artistId;
                orlCmd.Parameters.Add(inArtistId);

                inTeamResponsibility.OracleDbType = OracleDbType.Varchar2;
                inTeamResponsibility.Direction = ParameterDirection.Input;
                inTeamResponsibility.Value = teamResponsibility;
                orlCmd.Parameters.Add(inTeamResponsibility);

                inMngrResponsibility.OracleDbType = OracleDbType.Varchar2;
                inMngrResponsibility.Direction = ParameterDirection.Input;
                inMngrResponsibility.Value = mngrResponsibility;
                orlCmd.Parameters.Add(inMngrResponsibility);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                inArtistName.OracleDbType = OracleDbType.Varchar2;
                inArtistName.Direction = ParameterDirection.Input;
                inArtistName.Value = artistName;
                orlCmd.Parameters.Add(inArtistName);

                curArtistData.OracleDbType = OracleDbType.RefCursor;
                curArtistData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curArtistData);

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


        public DataSet InsertArtistData(string artistName, string dealType, string teamResponsibility, string mngrResponsibility,
           string userCode, string artistNameSearch, string dealTypeSearch, string responsibilitySearch, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inArtistName = new OracleParameter();
                OracleParameter inDealType = new OracleParameter();
                OracleParameter inTeamResponsibility = new OracleParameter();
                OracleParameter inMngrResponsibility = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter inArtistNameSearch = new OracleParameter();
                OracleParameter inDealTypeSearch = new OracleParameter();
                OracleParameter inResponsibilitySearch = new OracleParameter();
                OracleParameter curArtistData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_maint_artist.p_insert_artist_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inArtistName.OracleDbType = OracleDbType.Varchar2;
                inArtistName.Direction = ParameterDirection.Input;
                inArtistName.Value = artistName;
                orlCmd.Parameters.Add(inArtistName);

                inDealType.OracleDbType = OracleDbType.Varchar2;
                inDealType.Direction = ParameterDirection.Input;
                inDealType.Value = dealType;
                orlCmd.Parameters.Add(inDealType);

                inTeamResponsibility.OracleDbType = OracleDbType.Varchar2;
                inTeamResponsibility.Direction = ParameterDirection.Input;
                inTeamResponsibility.Value = teamResponsibility;
                orlCmd.Parameters.Add(inTeamResponsibility);

                inMngrResponsibility.OracleDbType = OracleDbType.Varchar2;
                inMngrResponsibility.Direction = ParameterDirection.Input;
                inMngrResponsibility.Value = mngrResponsibility;
                orlCmd.Parameters.Add(inMngrResponsibility);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                inArtistNameSearch.OracleDbType = OracleDbType.Varchar2;
                inArtistNameSearch.Direction = ParameterDirection.Input;
                inArtistNameSearch.Value = artistNameSearch;
                orlCmd.Parameters.Add(inArtistNameSearch);

                inDealTypeSearch.OracleDbType = OracleDbType.Varchar2;
                inDealTypeSearch.Direction = ParameterDirection.Input;
                inDealTypeSearch.Value = dealTypeSearch;
                orlCmd.Parameters.Add(inDealTypeSearch);

                inResponsibilitySearch.OracleDbType = OracleDbType.Varchar2;
                inResponsibilitySearch.Direction = ParameterDirection.Input;
                inResponsibilitySearch.Value = responsibilitySearch;
                orlCmd.Parameters.Add(inResponsibilitySearch);

                curArtistData.OracleDbType = OracleDbType.RefCursor;
                curArtistData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curArtistData);

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
