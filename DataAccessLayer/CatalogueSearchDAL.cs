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
    public class CatalogueSearchDAL : ICatalogueSearchDAL
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
                OracleParameter curConfigData = new OracleParameter();
                OracleParameter curResponsibilityList = new OracleParameter();
                OracleParameter curCatnoStatusList = new OracleParameter();
                OracleParameter curTrackStatusList = new OracleParameter();

                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_catalogue_search.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curConfigData.OracleDbType = OracleDbType.RefCursor;
                curConfigData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curConfigData);

                curResponsibilityList.OracleDbType = OracleDbType.RefCursor;
                curResponsibilityList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curResponsibilityList);

                curCatnoStatusList.OracleDbType = OracleDbType.RefCursor;
                curCatnoStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCatnoStatusList);

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

        public DataSet GetSearchedCatData(string catNo, string title, string artist, string configCode, string isrc, string teamResp, string teamRespTrack, string managerResp, string managerRespTrack, string catnoStatus, string trackStatus,
                                            Array catnoBulkSearchList, out Int32 iErrorId)
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
                OracleParameter inISRC = new OracleParameter();
                OracleParameter inTeamResp = new OracleParameter();
                OracleParameter inTeamRespTrack = new OracleParameter();
                OracleParameter inManagerResp = new OracleParameter();
                OracleParameter inManagerRespTrack = new OracleParameter();
                OracleParameter inCatnoStatus = new OracleParameter();
                OracleParameter inTrackStatus = new OracleParameter();
                OracleParameter inCatnoBulkSearchList = new OracleParameter();
                OracleParameter curCatalogueData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_catalogue_search.p_get_searched_data", orlConn);
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
                inConfigCode.Value = (configCode == "-" ? string.Empty : configCode);
                orlCmd.Parameters.Add(inConfigCode);

                inISRC.OracleDbType = OracleDbType.Varchar2;
                inISRC.Direction = ParameterDirection.Input;
                inISRC.Value = isrc;
                orlCmd.Parameters.Add(inISRC);

                inTeamResp.OracleDbType = OracleDbType.Varchar2;
                inTeamResp.Direction = ParameterDirection.Input;
                inTeamResp.Value = (teamResp == "-" ? string.Empty : teamResp);
                orlCmd.Parameters.Add(inTeamResp);

                inTeamRespTrack.OracleDbType = OracleDbType.Varchar2;
                inTeamRespTrack.Direction = ParameterDirection.Input;
                inTeamRespTrack.Value = (teamRespTrack == "-" ? string.Empty : teamRespTrack);
                orlCmd.Parameters.Add(inTeamRespTrack);

                inManagerResp.OracleDbType = OracleDbType.Varchar2;
                inManagerResp.Direction = ParameterDirection.Input;
                inManagerResp.Value = (managerResp == "-" ? string.Empty : managerResp);
                orlCmd.Parameters.Add(inManagerResp);

                inManagerRespTrack.OracleDbType = OracleDbType.Varchar2;
                inManagerRespTrack.Direction = ParameterDirection.Input;
                inManagerRespTrack.Value = (managerRespTrack == "-" ? string.Empty : managerRespTrack);
                orlCmd.Parameters.Add(inManagerRespTrack);

                inCatnoStatus.OracleDbType = OracleDbType.Varchar2;
                inCatnoStatus.Direction = ParameterDirection.Input;
                inCatnoStatus.Value = (catnoStatus == "-" ? string.Empty : catnoStatus);
                orlCmd.Parameters.Add(inCatnoStatus);

                inTrackStatus.OracleDbType = OracleDbType.Varchar2;
                inTrackStatus.Direction = ParameterDirection.Input;
                inTrackStatus.Value = (trackStatus == "-" ? string.Empty : trackStatus);
                orlCmd.Parameters.Add(inTrackStatus);

                inCatnoBulkSearchList.OracleDbType = OracleDbType.Varchar2;
                inCatnoBulkSearchList.Direction = ParameterDirection.Input;
                inCatnoBulkSearchList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (catnoBulkSearchList.Length == 0)
                {
                    inCatnoBulkSearchList.Size = 0;
                    inCatnoBulkSearchList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inCatnoBulkSearchList.Size = catnoBulkSearchList.Length;
                    inCatnoBulkSearchList.Value = catnoBulkSearchList;
                }
                orlCmd.Parameters.Add(inCatnoBulkSearchList);

                curCatalogueData.OracleDbType = OracleDbType.RefCursor;
                curCatalogueData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCatalogueData);

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

        public DataSet GetCatnoParticipants(Array catnoList, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inCatNoList = new OracleParameter();
                OracleParameter curParticipants = new OracleParameter();
                OracleParameter curIsrcParticipants = new OracleParameter();
                OracleParameter curCantoTracks = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_catalogue_search.p_get_catno_participants", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inCatNoList.OracleDbType = OracleDbType.Varchar2;
                inCatNoList.Direction = ParameterDirection.Input;
                inCatNoList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (catnoList.Length == 0)
                {
                    inCatNoList.Size = 0;
                    inCatNoList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inCatNoList.Size = catnoList.Length;
                    inCatNoList.Value = catnoList;
                }
                orlCmd.Parameters.Add(inCatNoList);

                curParticipants.OracleDbType = OracleDbType.RefCursor;
                curParticipants.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curParticipants);

                curIsrcParticipants.OracleDbType = OracleDbType.RefCursor;
                curIsrcParticipants.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curIsrcParticipants);

                curCantoTracks.OracleDbType = OracleDbType.RefCursor;
                curCantoTracks.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCantoTracks);

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

        //public DataSet UpdateCatalogueDetails(string catNo, string title, string artist, string configCode, string isrc, string teamResp, string teamRespTrack, string managerResp, string statusFilter, string statusCode,
        //                                      Array catnoBulkSearchList, Array catalogueDetails, string userCode, out Int32 iErrorId)
        //{
        //    ds = new DataSet();
        //    try
        //    {
        //        OpenConnection(out iErrorId, out sErrorMsg);

        //        ErrorId = new OracleParameter();
        //        OracleParameter inCatNo = new OracleParameter();
        //        OracleParameter inTitle = new OracleParameter();
        //        OracleParameter inArtist = new OracleParameter();
        //        OracleParameter inConfigCode = new OracleParameter();
        //        OracleParameter inISRC = new OracleParameter();
        //        OracleParameter inTeamResp = new OracleParameter();
        //        OracleParameter inTeamRespTrack = new OracleParameter();
        //        OracleParameter inManagerResp = new OracleParameter();
        //        OracleParameter inStatusFilter = new OracleParameter();
        //        OracleParameter inStatusCode = new OracleParameter();
        //        OracleParameter inCatnoBulkSearchList = new OracleParameter();
        //        OracleParameter inCatalogueDetails = new OracleParameter();
        //        OracleParameter inUserCode = new OracleParameter();                
        //        OracleParameter curCatalogueData = new OracleParameter();
        //        OracleParameter curParticipData = new OracleParameter();
        //        orlDA = new OracleDataAdapter();

        //        orlCmd = new OracleCommand("pkg_catalogue_search.p_update_catalogue_details", orlConn);
        //        orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //        inCatNo.OracleDbType = OracleDbType.Varchar2;
        //        inCatNo.Direction = ParameterDirection.Input;
        //        inCatNo.Value = catNo;
        //        orlCmd.Parameters.Add(inCatNo);

        //        inTitle.OracleDbType = OracleDbType.Varchar2;
        //        inTitle.Direction = ParameterDirection.Input;
        //        inTitle.Value = title;
        //        orlCmd.Parameters.Add(inTitle);

        //        inArtist.OracleDbType = OracleDbType.Varchar2;
        //        inArtist.Direction = ParameterDirection.Input;
        //        inArtist.Value = artist;
        //        orlCmd.Parameters.Add(inArtist);

        //        inConfigCode.OracleDbType = OracleDbType.Varchar2;
        //        inConfigCode.Direction = ParameterDirection.Input;
        //        inConfigCode.Value = (configCode == "-" ? string.Empty : configCode);
        //        orlCmd.Parameters.Add(inConfigCode);

        //        inISRC.OracleDbType = OracleDbType.Varchar2;
        //        inISRC.Direction = ParameterDirection.Input;
        //        inISRC.Value = isrc;
        //        orlCmd.Parameters.Add(inISRC);

        //        inTeamResp.OracleDbType = OracleDbType.Varchar2;
        //        inTeamResp.Direction = ParameterDirection.Input;
        //        inTeamResp.Value = (teamResp == "-" ? string.Empty : teamResp);
        //        orlCmd.Parameters.Add(inTeamResp);

        //        inTeamRespTrack.OracleDbType = OracleDbType.Varchar2;
        //        inTeamRespTrack.Direction = ParameterDirection.Input;
        //        inTeamRespTrack.Value = (teamRespTrack == "-" ? string.Empty : teamResp);
        //        orlCmd.Parameters.Add(inTeamRespTrack);

        //        inManagerResp.OracleDbType = OracleDbType.Varchar2;
        //        inManagerResp.Direction = ParameterDirection.Input;
        //        inManagerResp.Value = (managerResp == "-" ? string.Empty : managerResp);
        //        orlCmd.Parameters.Add(inManagerResp);

        //        inStatusFilter.OracleDbType = OracleDbType.Varchar2;
        //        inStatusFilter.Direction = ParameterDirection.Input;
        //        inStatusFilter.Value = (statusFilter == "-" ? string.Empty : statusFilter);
        //        orlCmd.Parameters.Add(inStatusFilter);

        //        inStatusCode.OracleDbType = OracleDbType.Varchar2;
        //        inStatusCode.Direction = ParameterDirection.Input;
        //        inStatusCode.Value = statusCode;
        //        orlCmd.Parameters.Add(inStatusCode);

        //        inCatnoBulkSearchList.OracleDbType = OracleDbType.Varchar2;
        //        inCatnoBulkSearchList.Direction = ParameterDirection.Input;
        //        inCatnoBulkSearchList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
        //        if (catnoBulkSearchList.Length == 0)
        //        {
        //            inCatnoBulkSearchList.Size = 0;
        //            inCatnoBulkSearchList.Value = new OracleDecimal[1] { OracleDecimal.Null };
        //        }
        //        else
        //        {
        //            inCatnoBulkSearchList.Size = catnoBulkSearchList.Length;
        //            inCatnoBulkSearchList.Value = catnoBulkSearchList;
        //        }
        //        orlCmd.Parameters.Add(inCatnoBulkSearchList);

        //        inCatalogueDetails.OracleDbType = OracleDbType.Varchar2;
        //        inCatalogueDetails.Direction = ParameterDirection.Input;
        //        inCatalogueDetails.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
        //        if (catalogueDetails.Length == 0)
        //        {
        //            inCatalogueDetails.Size = 0;
        //            inCatalogueDetails.Value = new OracleDecimal[1] { OracleDecimal.Null };
        //        }
        //        else
        //        {
        //            inCatalogueDetails.Size = catalogueDetails.Length;
        //            inCatalogueDetails.Value = catalogueDetails;
        //        }
        //        orlCmd.Parameters.Add(inCatalogueDetails);

        //        inUserCode.OracleDbType = OracleDbType.Varchar2;
        //        inUserCode.Direction = ParameterDirection.Input;
        //        inUserCode.Value = userCode;
        //        orlCmd.Parameters.Add(inUserCode);

        //        curCatalogueData.OracleDbType = OracleDbType.RefCursor;
        //        curCatalogueData.Direction = System.Data.ParameterDirection.Output;
        //        orlCmd.Parameters.Add(curCatalogueData);

        //        curParticipData.OracleDbType = OracleDbType.RefCursor;
        //        curParticipData.Direction = System.Data.ParameterDirection.Output;
        //        orlCmd.Parameters.Add(curParticipData);

        //        ErrorId.OracleDbType = OracleDbType.Int32;
        //        ErrorId.Direction = ParameterDirection.Output;
        //        ErrorId.ParameterName = "ErrorId";
        //        orlCmd.Parameters.Add(ErrorId);

        //        orlDA = new OracleDataAdapter(orlCmd);
        //        orlDA.Fill(ds);

        //        iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        iErrorId = 2;
        //        throw ex;
        //    }
        //    finally
        //    {
        //        CloseConnection();
        //    }
        //    return ds;

        //}

        public DataSet UpdateCatalogueDetails(string catNo, string title, string artist, string configCode, string isrc, string teamResp, string teamRespTrack, string managerResp, string managerRespTrack, string statusFilter, string statusCode, string trackStatus,
                                             Array catnoBulkSearchList, Array catalogueDetails, string userCode, out Int32 iErrorId)
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
                OracleParameter inISRC = new OracleParameter();
                OracleParameter inTeamResp = new OracleParameter();
                OracleParameter inTeamRespTrack = new OracleParameter();
                OracleParameter inManagerResp = new OracleParameter();
                OracleParameter inManagerRespTrack = new OracleParameter();
                OracleParameter inStatusFilter = new OracleParameter();
                OracleParameter inStatusCode = new OracleParameter();
                OracleParameter inTrackStatus = new OracleParameter();
                OracleParameter inCatnoBulkSearchList = new OracleParameter();
                OracleParameter inCatalogueDetails = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curCatalogueData = new OracleParameter();
                OracleParameter curParticipData = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_catalogue_search.p_update_catalogue_details", orlConn);
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
                inConfigCode.Value = (configCode == "-" ? string.Empty : configCode);
                orlCmd.Parameters.Add(inConfigCode);

                inISRC.OracleDbType = OracleDbType.Varchar2;
                inISRC.Direction = ParameterDirection.Input;
                inISRC.Value = isrc;
                orlCmd.Parameters.Add(inISRC);

                inTeamResp.OracleDbType = OracleDbType.Varchar2;
                inTeamResp.Direction = ParameterDirection.Input;
                inTeamResp.Value = (teamResp == "-" ? string.Empty : teamResp);
                orlCmd.Parameters.Add(inTeamResp);

                inTeamRespTrack.OracleDbType = OracleDbType.Varchar2;
                inTeamRespTrack.Direction = ParameterDirection.Input;
                inTeamRespTrack.Value = (teamRespTrack == "-" ? string.Empty : teamResp);
                orlCmd.Parameters.Add(inTeamRespTrack);

                inManagerResp.OracleDbType = OracleDbType.Varchar2;
                inManagerResp.Direction = ParameterDirection.Input;
                inManagerResp.Value = (managerResp == "-" ? string.Empty : managerResp);
                orlCmd.Parameters.Add(inManagerResp);

                inManagerRespTrack.OracleDbType = OracleDbType.Varchar2;
                inManagerRespTrack.Direction = ParameterDirection.Input;
                inManagerRespTrack.Value = (managerRespTrack == "-" ? string.Empty : managerRespTrack);
                orlCmd.Parameters.Add(inManagerRespTrack);

                inStatusFilter.OracleDbType = OracleDbType.Varchar2;
                inStatusFilter.Direction = ParameterDirection.Input;
                inStatusFilter.Value = (statusFilter == "-" ? string.Empty : statusFilter);
                orlCmd.Parameters.Add(inStatusFilter);

                inStatusCode.OracleDbType = OracleDbType.Varchar2;
                inStatusCode.Direction = ParameterDirection.Input;
                inStatusCode.Value = statusCode;
                orlCmd.Parameters.Add(inStatusCode);

                inTrackStatus.OracleDbType = OracleDbType.Varchar2;
                inTrackStatus.Direction = ParameterDirection.Input;
                inTrackStatus.Value = (trackStatus == "-" ? string.Empty : trackStatus);
                orlCmd.Parameters.Add(inTrackStatus);

                inCatnoBulkSearchList.OracleDbType = OracleDbType.Varchar2;
                inCatnoBulkSearchList.Direction = ParameterDirection.Input;
                inCatnoBulkSearchList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (catnoBulkSearchList.Length == 0)
                {
                    inCatnoBulkSearchList.Size = 0;
                    inCatnoBulkSearchList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inCatnoBulkSearchList.Size = catnoBulkSearchList.Length;
                    inCatnoBulkSearchList.Value = catnoBulkSearchList;
                }
                orlCmd.Parameters.Add(inCatnoBulkSearchList);

                inCatalogueDetails.OracleDbType = OracleDbType.Varchar2;
                inCatalogueDetails.Direction = ParameterDirection.Input;
                inCatalogueDetails.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (catalogueDetails.Length == 0)
                {
                    inCatalogueDetails.Size = 0;
                    inCatalogueDetails.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    inCatalogueDetails.Size = catalogueDetails.Length;
                    inCatalogueDetails.Value = catalogueDetails;
                }
                orlCmd.Parameters.Add(inCatalogueDetails);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curCatalogueData.OracleDbType = OracleDbType.RefCursor;
                curCatalogueData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curCatalogueData);

                curParticipData.OracleDbType = OracleDbType.RefCursor;
                curParticipData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curParticipData);

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
