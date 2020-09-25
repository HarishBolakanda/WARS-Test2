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
    public class TrackListingDAL : ITrackListingDAL
    {

        #region Global Declarations
        ConnectionDAL connDAL;
        OracleConnection orlConn;
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        DataTable dt;
        OracleParameter ErrorId;
        string sErrorMsg;
        #endregion Global Declarations
        
        public DataSet GetTrackListing(string catNo, string filterStatus, string filterUnit, string filterSide, string loggedInUserRole, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pFilterStatus = new OracleParameter();
                OracleParameter pFilterUnit = new OracleParameter();
                OracleParameter pFilterSide = new OracleParameter();
                OracleParameter pLoggedInUserRole = new OracleParameter();
                OracleParameter pCatDetails = new OracleParameter();
                OracleParameter pCatStatusList = new OracleParameter();
                OracleParameter pISRCStatusList = new OracleParameter();
                OracleParameter pRoyaltorList = new OracleParameter();
                OracleParameter pOptionPeriodList = new OracleParameter();
                OracleParameter pSellerGroupList = new OracleParameter();
                OracleParameter pEscCodeList = new OracleParameter();
                OracleParameter pTrackListing = new OracleParameter();
                OracleParameter pParticipation = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_track_listing.p_get_track_listing_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pFilterStatus.OracleDbType = OracleDbType.Varchar2;
                pFilterStatus.Direction = ParameterDirection.Input;
                pFilterStatus.Value = (filterStatus == string.Empty ? "-" : filterStatus);
                orlCmd.Parameters.Add(pFilterStatus);

                pFilterUnit.OracleDbType = OracleDbType.Varchar2;
                pFilterUnit.Direction = ParameterDirection.Input;
                pFilterUnit.Value = (filterUnit == string.Empty ? "-" : filterUnit);
                orlCmd.Parameters.Add(pFilterUnit);

                pFilterSide.OracleDbType = OracleDbType.Varchar2;
                pFilterSide.Direction = ParameterDirection.Input;
                pFilterSide.Value = (filterSide == string.Empty ? "-" : filterSide.ToUpper());
                orlCmd.Parameters.Add(pFilterSide);

                pLoggedInUserRole.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUserRole.Direction = ParameterDirection.Input;
                pLoggedInUserRole.Value = loggedInUserRole;
                orlCmd.Parameters.Add(pLoggedInUserRole);

                pCatDetails.OracleDbType = OracleDbType.RefCursor;
                pCatDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatDetails);

                pCatStatusList.OracleDbType = OracleDbType.RefCursor;
                pCatStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatStatusList);

                pISRCStatusList.OracleDbType = OracleDbType.RefCursor;
                pISRCStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pISRCStatusList);

                //JIRA-1005 Changes --Start
                //pRoyaltorList.OracleDbType = OracleDbType.RefCursor;
                //pRoyaltorList.Direction = System.Data.ParameterDirection.Output;
                //orlCmd.Parameters.Add(pRoyaltorList);
                //JIRA-1005 Changes --End

                pOptionPeriodList.OracleDbType = OracleDbType.RefCursor;
                pOptionPeriodList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOptionPeriodList);

                pSellerGroupList.OracleDbType = OracleDbType.RefCursor;
                pSellerGroupList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSellerGroupList);

                pEscCodeList.OracleDbType = OracleDbType.RefCursor;
                pEscCodeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscCodeList);

                pTrackListing.OracleDbType = OracleDbType.RefCursor;
                pTrackListing.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTrackListing);

                pParticipation.OracleDbType = OracleDbType.RefCursor;
                pParticipation.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pParticipation);

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

        public DataTable GetFilteredData(string catNo, string filterStatus, string filterUnit, string filterSide, string loggedInUserRole, out Int32 iErrorId)
        {
            dt = new DataTable();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pFilterStatus = new OracleParameter();
                OracleParameter pFilterUnit = new OracleParameter();
                OracleParameter pFilterSide = new OracleParameter();
                OracleParameter pLoggedInUserRole = new OracleParameter();
                OracleParameter pTrackListing = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_track_listing.p_track_listing_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pFilterStatus.OracleDbType = OracleDbType.Varchar2;
                pFilterStatus.Direction = ParameterDirection.Input;
                pFilterStatus.Value = (filterStatus == string.Empty ? "-" : filterStatus);
                orlCmd.Parameters.Add(pFilterStatus);

                pFilterUnit.OracleDbType = OracleDbType.Varchar2;
                pFilterUnit.Direction = ParameterDirection.Input;
                pFilterUnit.Value = (filterUnit == string.Empty ? "-" : filterUnit);
                orlCmd.Parameters.Add(pFilterUnit);

                pFilterSide.OracleDbType = OracleDbType.Varchar2;
                pFilterSide.Direction = ParameterDirection.Input;
                pFilterSide.Value = (filterSide == string.Empty ? "-" : filterSide.ToUpper());
                orlCmd.Parameters.Add(pFilterSide);

                pLoggedInUserRole.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUserRole.Direction = ParameterDirection.Input;
                pLoggedInUserRole.Value = loggedInUserRole;
                orlCmd.Parameters.Add(pLoggedInUserRole);

                pTrackListing.OracleDbType = OracleDbType.RefCursor;
                pTrackListing.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTrackListing);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(dt);
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

            return dt;
        }

        public DataSet SaveComment(string catNo, string filterStatus, string filterUnit, string filterSide, string isrcDealId, string comment, string saveDelete, string loggedInUserRole, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pFilterStatus = new OracleParameter();
                OracleParameter pFilterUnit = new OracleParameter();
                OracleParameter pFilterSide = new OracleParameter();
                OracleParameter pIsrcDealId = new OracleParameter();
                OracleParameter pComment = new OracleParameter();
                OracleParameter pSaveDelete = new OracleParameter();
                OracleParameter pLoggedInUserRoleId = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pTrackListing = new OracleParameter();
                OracleParameter pCatDetails = new OracleParameter();


                orlCmd = new OracleCommand("pkg_maint_track_listing.p_save_comment", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pFilterStatus.OracleDbType = OracleDbType.Varchar2;
                pFilterStatus.Direction = ParameterDirection.Input;
                pFilterStatus.Value = (filterStatus == string.Empty ? "-" : filterStatus);
                orlCmd.Parameters.Add(pFilterStatus);

                pFilterUnit.OracleDbType = OracleDbType.Varchar2;
                pFilterUnit.Direction = ParameterDirection.Input;
                pFilterUnit.Value = (filterUnit == string.Empty ? "-" : filterUnit);
                orlCmd.Parameters.Add(pFilterUnit);

                pFilterSide.OracleDbType = OracleDbType.Varchar2;
                pFilterSide.Direction = ParameterDirection.Input;
                pFilterSide.Value = (filterSide == string.Empty ? "-" : filterSide.ToUpper());
                orlCmd.Parameters.Add(pFilterSide);

                pIsrcDealId.OracleDbType = OracleDbType.Varchar2;
                pIsrcDealId.Direction = ParameterDirection.Input;
                pIsrcDealId.Value = isrcDealId;
                orlCmd.Parameters.Add(pIsrcDealId);

                pComment.OracleDbType = OracleDbType.Varchar2;
                pComment.Direction = ParameterDirection.Input;
                pComment.Value = comment;
                orlCmd.Parameters.Add(pComment);

                pSaveDelete.OracleDbType = OracleDbType.Varchar2;
                pSaveDelete.Direction = ParameterDirection.Input;
                pSaveDelete.Value = saveDelete;
                orlCmd.Parameters.Add(pSaveDelete);

                pLoggedInUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUserRoleId.Direction = ParameterDirection.Input;
                pLoggedInUserRoleId.Value = loggedInUserRole;
                orlCmd.Parameters.Add(pLoggedInUserRoleId);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pTrackListing.OracleDbType = OracleDbType.RefCursor;
                pTrackListing.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTrackListing);

                pCatDetails.OracleDbType = OracleDbType.RefCursor;
                pCatDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatDetails);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "dtTrackListing";
                    ds.Tables[1].TableName = "dtCatDetails";
                }

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

        public DataSet SaveAllTrackDetails(string catNo, string filterStatus, string filterUnit, string filterSide, string catStatusCode, string trackTimeFlag, string catStatusChanged, string catFlagChanged, string userCode,
                                   Array trackList, Array trackParticipantList, string loggedInUserRole, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pFilterStatus = new OracleParameter();
                OracleParameter pFilterUnit = new OracleParameter();
                OracleParameter pFilterSide = new OracleParameter();
                OracleParameter pCatStatusCode = new OracleParameter();
                OracleParameter pTrackTimeFlag = new OracleParameter();
                OracleParameter pCatStatusChanged= new OracleParameter();
                OracleParameter pCatFlagChanged = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pTrackList = new OracleParameter();
                OracleParameter pTrackParticipantList = new OracleParameter();
                OracleParameter pLoggedInUserRoleId = new OracleParameter();
                OracleParameter pTrackListing = new OracleParameter();
                OracleParameter pCatDetails = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_track_listing.p_save_all_track_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pFilterStatus.OracleDbType = OracleDbType.Varchar2;
                pFilterStatus.Direction = ParameterDirection.Input;
                pFilterStatus.Value = (filterStatus == string.Empty ? "-" : filterStatus);
                orlCmd.Parameters.Add(pFilterStatus);

                pFilterUnit.OracleDbType = OracleDbType.Varchar2;
                pFilterUnit.Direction = ParameterDirection.Input;
                pFilterUnit.Value = (filterUnit == string.Empty ? "-" : filterUnit);
                orlCmd.Parameters.Add(pFilterUnit);

                pFilterSide.OracleDbType = OracleDbType.Varchar2;
                pFilterSide.Direction = ParameterDirection.Input;
                pFilterSide.Value = (filterSide == string.Empty ? "-" : filterSide.ToUpper());
                orlCmd.Parameters.Add(pFilterSide);

                pCatStatusCode.OracleDbType = OracleDbType.Varchar2;
                pCatStatusCode.Direction = ParameterDirection.Input;
                pCatStatusCode.Value = catStatusCode;
                orlCmd.Parameters.Add(pCatStatusCode);

                pTrackTimeFlag.OracleDbType = OracleDbType.Varchar2;
                pTrackTimeFlag.Direction = ParameterDirection.Input;
                pTrackTimeFlag.Value = trackTimeFlag;
                orlCmd.Parameters.Add(pTrackTimeFlag);

                pCatStatusChanged.OracleDbType = OracleDbType.Varchar2;
                pCatStatusChanged.Direction = ParameterDirection.Input;
                pCatStatusChanged.Value = catStatusChanged;
                orlCmd.Parameters.Add(pCatStatusChanged);

                pCatFlagChanged.OracleDbType = OracleDbType.Varchar2;
                pCatFlagChanged.Direction = ParameterDirection.Input;
                pCatFlagChanged.Value = catFlagChanged;
                orlCmd.Parameters.Add(pCatFlagChanged);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pTrackList.OracleDbType = OracleDbType.Varchar2;
                pTrackList.Direction = ParameterDirection.Input;
                pTrackList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (trackList.Length == 0)
                {
                    pTrackList.Size = 1;
                    pTrackList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pTrackList.Size = trackList.Length;
                    pTrackList.Value = trackList;
                }
                orlCmd.Parameters.Add(pTrackList);

                pTrackParticipantList.OracleDbType = OracleDbType.Varchar2;
                pTrackParticipantList.Direction = ParameterDirection.Input;
                pTrackParticipantList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (trackParticipantList.Length == 0)
                {
                    pTrackParticipantList.Size = 1;
                    pTrackParticipantList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pTrackParticipantList.Size = trackParticipantList.Length;
                    pTrackParticipantList.Value = trackParticipantList;
                }
                orlCmd.Parameters.Add(pTrackParticipantList);

                pLoggedInUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUserRoleId.Direction = ParameterDirection.Input;
                pLoggedInUserRoleId.Value = loggedInUserRole;
                orlCmd.Parameters.Add(pLoggedInUserRoleId);


                pTrackListing.OracleDbType = OracleDbType.RefCursor;
                pTrackListing.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTrackListing);

                pCatDetails.OracleDbType = OracleDbType.RefCursor;
                pCatDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatDetails);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "dtTrackListing";
                    ds.Tables[1].TableName = "dtCatDetails";
                }

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
        public DataSet CopyParticipant(string catNo, string filterStatus, string filterUnit, string filterSide, string userCode, string copyPart, string selectedTrackIds, string loggedInUserRole, out Int32 iErrorId)
        {
            ds = new DataSet();

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pFilterStatus = new OracleParameter();
                OracleParameter pFilterUnit = new OracleParameter();
                OracleParameter pFilterSide = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pCopyPart = new OracleParameter();
                OracleParameter pSelectedTrackIds = new OracleParameter();
                OracleParameter pLoggedInUserRoleId = new OracleParameter();
                OracleParameter pTrackListing = new OracleParameter();
                OracleParameter pCatDetails = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_track_listing.p_copy_participant", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pFilterStatus.OracleDbType = OracleDbType.Varchar2;
                pFilterStatus.Direction = ParameterDirection.Input;
                pFilterStatus.Value = (filterStatus == string.Empty ? "-" : filterStatus);
                orlCmd.Parameters.Add(pFilterStatus);

                pFilterUnit.OracleDbType = OracleDbType.Varchar2;
                pFilterUnit.Direction = ParameterDirection.Input;
                pFilterUnit.Value = (filterUnit == string.Empty ? "-" : filterUnit);
                orlCmd.Parameters.Add(pFilterUnit);

                pFilterSide.OracleDbType = OracleDbType.Varchar2;
                pFilterSide.Direction = ParameterDirection.Input;
                pFilterSide.Value = (filterSide == string.Empty ? "-" : filterSide.ToUpper());
                orlCmd.Parameters.Add(pFilterSide);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pCopyPart.OracleDbType = OracleDbType.Varchar2;
                pCopyPart.Direction = ParameterDirection.Input;
                pCopyPart.Value = copyPart;
                orlCmd.Parameters.Add(pCopyPart);

                pSelectedTrackIds.OracleDbType = OracleDbType.Varchar2;
                pSelectedTrackIds.Direction = ParameterDirection.Input;
                pSelectedTrackIds.Value = selectedTrackIds;
                orlCmd.Parameters.Add(pSelectedTrackIds);

                pLoggedInUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUserRoleId.Direction = ParameterDirection.Input;
                pLoggedInUserRoleId.Value = loggedInUserRole;
                orlCmd.Parameters.Add(pLoggedInUserRoleId);

                pTrackListing.OracleDbType = OracleDbType.RefCursor;
                pTrackListing.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTrackListing);

                pCatDetails.OracleDbType = OracleDbType.RefCursor;
                pCatDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatDetails);


                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "dtTrackListing";
                    ds.Tables[1].TableName = "dtCatDetails";
                }

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

        public void ConsolidateParticipants(string catNo, string userCode, out Int32 iErrorId, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pErrorMsg = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_track_listing.p_consolidate_particip", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                pErrorMsg.OracleDbType = OracleDbType.Varchar2;
                pErrorMsg.Size = 200;
                pErrorMsg.Direction = ParameterDirection.Output;
                pErrorMsg.ParameterName = "ErrorMsg";
                orlCmd.Parameters.Add(pErrorMsg);

                orlCmd.ExecuteNonQuery();

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                errorMsg = orlCmd.Parameters["ErrorMsg"].Value.ToString();

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
