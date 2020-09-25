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
    public class ParticipantMaintenanceDAL : IParticipantMaintenanceDAL
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

        public DataSet GetInitialData(string catNo, string activeParticips, string userRoleId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pActiveParticips = new OracleParameter();
                OracleParameter pCatDetails = new OracleParameter();
                OracleParameter pParticipantDetails = new OracleParameter();                
                OracleParameter pOptionPeriodList = new OracleParameter();
                OracleParameter pSellerGroupList = new OracleParameter();
                OracleParameter pTuneTitleList = new OracleParameter();
                OracleParameter pStatusList = new OracleParameter();
                OracleParameter pEscCodeList = new OracleParameter();
                OracleParameter pLoggedInUserRoleId = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_participant.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pActiveParticips.OracleDbType = OracleDbType.Varchar2;
                pActiveParticips.Direction = ParameterDirection.Input;
                pActiveParticips.Value = activeParticips;
                orlCmd.Parameters.Add(pActiveParticips);

                //JIRA-898 Changes --Start
                pLoggedInUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUserRoleId.Direction = ParameterDirection.Input;
                pLoggedInUserRoleId.Value = userRoleId;
                orlCmd.Parameters.Add(pLoggedInUserRoleId);
                //JIRA-898 Changes --End

                pCatDetails.OracleDbType = OracleDbType.RefCursor;
                pCatDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatDetails);

                pParticipantDetails.OracleDbType = OracleDbType.RefCursor;
                pParticipantDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pParticipantDetails);
                              
                pOptionPeriodList.OracleDbType = OracleDbType.RefCursor;
                pOptionPeriodList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOptionPeriodList);

                pSellerGroupList.OracleDbType = OracleDbType.RefCursor;
                pSellerGroupList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pSellerGroupList);

                pTuneTitleList.OracleDbType = OracleDbType.RefCursor;
                pTuneTitleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTuneTitleList);

                pStatusList.OracleDbType = OracleDbType.RefCursor;
                pStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pStatusList);

                pEscCodeList.OracleDbType = OracleDbType.RefCursor;
                pEscCodeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscCodeList);

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

        public DataSet GetAllOrActiveParticips(string catNo, string activeParticips, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pActiveParticips = new OracleParameter();                
                OracleParameter pParticipantDetails = new OracleParameter();                

                orlCmd = new OracleCommand("pkg_maint_participant.p_get_participant_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pActiveParticips.OracleDbType = OracleDbType.Varchar2;
                pActiveParticips.Direction = ParameterDirection.Input;
                pActiveParticips.Value = activeParticips;
                orlCmd.Parameters.Add(pActiveParticips);

                pParticipantDetails.OracleDbType = OracleDbType.RefCursor;
                pParticipantDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pParticipantDetails);

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

        public DataSet GetFuzzySearchLists(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();                
                OracleParameter pOptionPeriodList = new OracleParameter();
                OracleParameter pTuneTitleList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_participant.p_get_fuzzy_search_list", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                
                pOptionPeriodList.OracleDbType = OracleDbType.RefCursor;
                pOptionPeriodList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pOptionPeriodList);

                pTuneTitleList.OracleDbType = OracleDbType.RefCursor;
                pTuneTitleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTuneTitleList);

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

        public DataSet SaveParticipantDetails(string catNo, string catStatusCode, string isCatModified, Array participantsToAddUpdate,
                                                string activeParticips, string userCode, string userRoleId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pCatStatusCode = new OracleParameter();
                OracleParameter pIsCatModified = new OracleParameter();
                OracleParameter pAddUpdateList = new OracleParameter();
                OracleParameter pActiveParticips = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pLoggedInUserRoleId = new OracleParameter();
                OracleParameter pCatlogueData = new OracleParameter();
                OracleParameter pParticipantsData = new OracleParameter();
                OracleParameter pTuneTitleList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_participant.p_save_participant_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pCatStatusCode.OracleDbType = OracleDbType.Varchar2;
                pCatStatusCode.Direction = ParameterDirection.Input;
                pCatStatusCode.Value = catStatusCode;
                orlCmd.Parameters.Add(pCatStatusCode);

                pIsCatModified.OracleDbType = OracleDbType.Varchar2;
                pIsCatModified.Direction = ParameterDirection.Input;
                pIsCatModified.Value = isCatModified;
                orlCmd.Parameters.Add(pIsCatModified);

                pAddUpdateList.OracleDbType = OracleDbType.Varchar2;
                pAddUpdateList.Direction = ParameterDirection.Input;
                pAddUpdateList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (participantsToAddUpdate.Length == 0)
                {
                    pAddUpdateList.Size = 1;
                    pAddUpdateList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pAddUpdateList.Size = participantsToAddUpdate.Length;
                    pAddUpdateList.Value = participantsToAddUpdate;
                }
                orlCmd.Parameters.Add(pAddUpdateList);

                pActiveParticips.OracleDbType = OracleDbType.Varchar2;
                pActiveParticips.Direction = ParameterDirection.Input;
                pActiveParticips.Value = activeParticips;
                orlCmd.Parameters.Add(pActiveParticips);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = userCode;
                orlCmd.Parameters.Add(pUserCode);

                pLoggedInUserRoleId.OracleDbType = OracleDbType.Varchar2;
                pLoggedInUserRoleId.Direction = ParameterDirection.Input;
                pLoggedInUserRoleId.Value = userRoleId;
                orlCmd.Parameters.Add(pLoggedInUserRoleId);

                pCatlogueData.OracleDbType = OracleDbType.RefCursor;
                pCatlogueData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pCatlogueData);

                pParticipantsData.OracleDbType = OracleDbType.RefCursor;
                pParticipantsData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pParticipantsData);

                pTuneTitleList.OracleDbType = OracleDbType.RefCursor;
                pTuneTitleList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTuneTitleList);

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

        public void GetTrackTitlefromISRC(string catNo, string seqNo, out Int32 iErrorId, out string trackTitle, out string isrc, out string track_Listing_Id)
        {
            trackTitle = string.Empty;
            isrc = string.Empty;
            track_Listing_Id = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                ErrorId = new OracleParameter();
                OracleParameter pCatNo = new OracleParameter();
                OracleParameter pSeqNo = new OracleParameter();
                OracleParameter pErrorMsg = new OracleParameter();
                OracleParameter ptrackTitle = new OracleParameter();
                OracleParameter pISRC = new OracleParameter();
                OracleParameter ptrack_Listing_Id = new OracleParameter();

                orlCmd = new OracleCommand("pkg_maint_participant.p_get_track_title_isrc", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pCatNo.OracleDbType = OracleDbType.Varchar2;
                pCatNo.Direction = ParameterDirection.Input;
                pCatNo.Value = catNo;
                orlCmd.Parameters.Add(pCatNo);

                pSeqNo.OracleDbType = OracleDbType.Int32;
                pSeqNo.Direction = ParameterDirection.Input;
                pSeqNo.Value = seqNo;
                orlCmd.Parameters.Add(pSeqNo);

                ptrackTitle.OracleDbType = OracleDbType.Varchar2;
                ptrackTitle.Size = 200;
                ptrackTitle.Direction = ParameterDirection.Output;
                ptrackTitle.ParameterName = "track_title";
                orlCmd.Parameters.Add(ptrackTitle);

                pISRC.OracleDbType = OracleDbType.Varchar2;
                pISRC.Size = 200;
                pISRC.Direction = ParameterDirection.Output;
                pISRC.ParameterName = "isrc";
                orlCmd.Parameters.Add(pISRC);

                ptrack_Listing_Id.OracleDbType = OracleDbType.Varchar2;
                ptrack_Listing_Id.Size = 200;
                ptrack_Listing_Id.Direction = ParameterDirection.Output;
                ptrack_Listing_Id.ParameterName = "track_listing_id";
                orlCmd.Parameters.Add(ptrack_Listing_Id);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();
                trackTitle = orlCmd.Parameters["track_title"].Value.ToString();
                isrc = orlCmd.Parameters["isrc"].Value.ToString();
                track_Listing_Id = orlCmd.Parameters["track_listing_id"].Value.ToString();
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
