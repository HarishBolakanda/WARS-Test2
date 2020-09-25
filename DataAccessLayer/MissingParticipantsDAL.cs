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
    public class MissingParticipantsDAL : IMissingParticipantsDAL
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
                //OracleParameter curMissingParticipants = new OracleParameter();
                OracleParameter curResponsibilityist = new OracleParameter();
                OracleParameter curCatnoStatusList = new OracleParameter();
                OracleParameter curConfigTypeList = new OracleParameter();
                OracleParameter curTrackStatusList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_missing_participants.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                //curMissingParticipants.OracleDbType = OracleDbType.RefCursor;
                //curMissingParticipants.Direction = System.Data.ParameterDirection.Output;
                //orlCmd.Parameters.Add(curMissingParticipants);

                curResponsibilityist.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityist.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityist);

                curCatnoStatusList.OracleDbType = OracleDbType.RefCursor;
                curCatnoStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCatnoStatusList);

                curConfigTypeList.OracleDbType = OracleDbType.RefCursor;
                curConfigTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curConfigTypeList);

                curTrackStatusList.OracleDbType = OracleDbType.RefCursor;
                curTrackStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curTrackStatusList);

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

        public DataSet GetSearchedData(string catNo, string title, string artist, string configCode, string teamResponsibility, string teamResponsibilityTrack,
           string mngrResponsibility, string managerResponsibilityTrack, string trnxStartDate, string trnxStartDateCompare, string trnxEndDate, string trnxEndDateCompare, string catnoStatusCode, string trackStatusCode, string trnxValue, string isrc, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCatNo = new OracleParameter();
                OracleParameter inTitle = new OracleParameter();
                OracleParameter inArtist = new OracleParameter();
                OracleParameter inConfigCode = new OracleParameter();
                OracleParameter inTeamResponsibility = new OracleParameter();
                OracleParameter inTeamResponsibilityTrack = new OracleParameter();
                OracleParameter inMngrResponsibility = new OracleParameter();
                OracleParameter inMngrResponsibilityTrack = new OracleParameter();
                OracleParameter inTrnxStartDate = new OracleParameter();
                OracleParameter inTrnxStartDateCompare = new OracleParameter();
                OracleParameter inTrnxEndDate = new OracleParameter();
                OracleParameter inTrnxEndDateCompare = new OracleParameter();
                OracleParameter inCatnoStatusCode = new OracleParameter();
                OracleParameter inTrackStatusCode = new OracleParameter();
                OracleParameter inTrnxValue = new OracleParameter();
                OracleParameter inISRC = new OracleParameter();
                OracleParameter curMissingParticipants = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_missing_participants.p_get_searched_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCatNo.OracleDbType = OracleDbType.Varchar2;
                inCatNo.Direction = ParameterDirection.Input;
                inCatNo.Value = catNo;
                orlCmd.Parameters.Add(inCatNo);

                inTitle.OracleDbType = OracleDbType.Varchar2;
                inTitle.Direction = ParameterDirection.Input;
                inTitle.Value = title;
                orlCmd.Parameters.Add(inTitle);

                inArtist.OracleDbType = OracleDbType.Varchar2;
                inArtist.Direction = ParameterDirection.Input;
                inArtist.Value = artist;
                orlCmd.Parameters.Add(inArtist);

                inConfigCode.OracleDbType = OracleDbType.Varchar2;
                inConfigCode.Direction = ParameterDirection.Input;
                inConfigCode.Value = configCode;
                orlCmd.Parameters.Add(inConfigCode);

                inTeamResponsibility.OracleDbType = OracleDbType.Varchar2;
                inTeamResponsibility.Direction = ParameterDirection.Input;
                inTeamResponsibility.Value = teamResponsibility;
                orlCmd.Parameters.Add(inTeamResponsibility);

                inTeamResponsibilityTrack.OracleDbType = OracleDbType.Varchar2;
                inTeamResponsibilityTrack.Direction = ParameterDirection.Input;
                inTeamResponsibilityTrack.Value = teamResponsibilityTrack;
                orlCmd.Parameters.Add(inTeamResponsibilityTrack);

                inMngrResponsibility.OracleDbType = OracleDbType.Varchar2;
                inMngrResponsibility.Direction = ParameterDirection.Input;
                inMngrResponsibility.Value = mngrResponsibility;
                orlCmd.Parameters.Add(inMngrResponsibility);

                inMngrResponsibilityTrack.OracleDbType = OracleDbType.Varchar2;
                inMngrResponsibilityTrack.Direction = ParameterDirection.Input;
                inMngrResponsibilityTrack.Value = managerResponsibilityTrack;
                orlCmd.Parameters.Add(inMngrResponsibilityTrack);

                inTrnxStartDate.OracleDbType = OracleDbType.Varchar2;
                inTrnxStartDate.Direction = ParameterDirection.Input;
                inTrnxStartDate.Value = trnxStartDate;
                orlCmd.Parameters.Add(inTrnxStartDate);

                inTrnxStartDateCompare.OracleDbType = OracleDbType.Varchar2;
                inTrnxStartDateCompare.Direction = ParameterDirection.Input;
                inTrnxStartDateCompare.Value = trnxStartDateCompare;
                orlCmd.Parameters.Add(inTrnxStartDateCompare);

                inTrnxEndDate.OracleDbType = OracleDbType.Varchar2;
                inTrnxEndDate.Direction = ParameterDirection.Input;
                inTrnxEndDate.Value = trnxEndDate;
                orlCmd.Parameters.Add(inTrnxEndDate);

                inTrnxEndDateCompare.OracleDbType = OracleDbType.Varchar2;
                inTrnxEndDateCompare.Direction = ParameterDirection.Input;
                inTrnxEndDateCompare.Value = trnxEndDateCompare;
                orlCmd.Parameters.Add(inTrnxEndDateCompare);

                inCatnoStatusCode.OracleDbType = OracleDbType.Varchar2;
                inCatnoStatusCode.Direction = ParameterDirection.Input;
                inCatnoStatusCode.Value = catnoStatusCode;
                orlCmd.Parameters.Add(inCatnoStatusCode);

                inTrackStatusCode.OracleDbType = OracleDbType.Varchar2;
                inTrackStatusCode.Direction = ParameterDirection.Input;
                inTrackStatusCode.Value = trackStatusCode;
                orlCmd.Parameters.Add(inTrackStatusCode);

                inTrnxValue.OracleDbType = OracleDbType.Varchar2;
                inTrnxValue.Direction = ParameterDirection.Input;
                inTrnxValue.Value = trnxValue;
                orlCmd.Parameters.Add(inTrnxValue);

                inISRC.OracleDbType = OracleDbType.Varchar2;
                inISRC.Direction = ParameterDirection.Input;
                inISRC.Value = isrc;
                orlCmd.Parameters.Add(inISRC);

                curMissingParticipants.OracleDbType = OracleDbType.RefCursor;
                curMissingParticipants.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curMissingParticipants);

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

        public string IsTrackListingExist(string catNo, out string catnoLegacy, out Int32 iErrorId)
        {
            string isTrackListingExist = string.Empty;

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                orlCmd = new OracleCommand();

                OracleParameter pCatno = new OracleParameter();
                OracleParameter pTrackListingId = new OracleParameter();
                OracleParameter pCatnoLegacy = new OracleParameter();

                orlCmd = new OracleCommand("pkg_missing_participants.p_is_track_listing_exist", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatno.OracleDbType = OracleDbType.Varchar2;
                pCatno.Direction = ParameterDirection.Input;
                pCatno.Value = catNo.ToUpper();
                orlCmd.Parameters.Add(pCatno);

                pTrackListingId.OracleDbType = OracleDbType.Varchar2;
                pTrackListingId.Size = 200;
                pTrackListingId.Direction = System.Data.ParameterDirection.Output;
                pTrackListingId.ParameterName = "isTrackListingExist";
                orlCmd.Parameters.Add(pTrackListingId);

                pCatnoLegacy.OracleDbType = OracleDbType.Varchar2;
                pCatnoLegacy.Size = 200;
                pCatnoLegacy.Direction = System.Data.ParameterDirection.Output;
                pCatnoLegacy.ParameterName = "catnoLegacy";
                orlCmd.Parameters.Add(pCatnoLegacy);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

                isTrackListingExist = orlCmd.Parameters["isTrackListingExist"].Value.ToString();
                catnoLegacy = orlCmd.Parameters["catnoLegacy"].Value.ToString();
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

            return isTrackListingExist;
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
