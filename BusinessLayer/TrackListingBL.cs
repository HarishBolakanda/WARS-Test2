using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;


namespace WARS.BusinessLayer
{
    public class TrackListingBL : ITrackListingBL
    {
        ITrackListingDAL trackListingDAL;
        #region Constructor
        public TrackListingBL()
        {
            trackListingDAL = new TrackListingDAL();
        }
        #endregion Constructor
        /* UserRole Added by RaviMulugu as a part of JIRA-898 on 15th November 2018 --Start */
        public DataSet GetTrackListing(string catNo, string filterStatus, string filterUnit, string filterSide, string loggedInUserRole, out Int32 iErrorId)
        {
            return ProcessTrackListingData(trackListingDAL.GetTrackListing(catNo, filterStatus, filterUnit, filterSide, loggedInUserRole, out iErrorId), 6);
        }
        /* UserRole Added by RaviMulugu as a part of JIRA-898 on 15th November 2018 --End */
        public DataTable GetFilteredData(string catNo, string filterStatus, string filterUnit, string filterSide, string loggedInUserRole, out Int32 iErrorId)
        {
            return ProcessTrackListingData(trackListingDAL.GetFilteredData(catNo, filterStatus, filterUnit, filterSide, loggedInUserRole, out iErrorId));
        }

        public DataSet SaveComment(string catNo, string filterStatus, string filterUnit, string filterSide, string isrcDealId, string comment, string saveDelete, string loggedInUserRole, string userCode, out Int32 iErrorId)
        {
            return ProcessTrackListingData(trackListingDAL.SaveComment(catNo, filterStatus, filterUnit, filterSide, isrcDealId, comment, saveDelete, loggedInUserRole, userCode, out iErrorId), 0);
        }

        public DataSet SaveAllTrackDetails(string catNo, string filterStatus, string filterUnit, string filterSide, string catStatusCode, string trackTimeFlag, string catStatusChanged, string catFlagChanged, string userCode,
                                   Array trackList, Array trackParticipantList, string loggedInUserRole, out Int32 iErrorId)
        {
            return ProcessTrackListingData(trackListingDAL.SaveAllTrackDetails(catNo, filterStatus, filterUnit, filterSide, catStatusCode, trackTimeFlag, catStatusChanged,catFlagChanged, userCode, trackList,
                                                                            trackParticipantList, loggedInUserRole, out iErrorId), 0);
        }

        public DataSet CopyParticipant(string catNo, string filterStatus, string filterUnit, string filterSide, string userCode, string copyPart, string selectedTrackIds, string loggedInUserRole, out Int32 iErrorId)
        {
            return ProcessTrackListingData(trackListingDAL.CopyParticipant(catNo, filterStatus, filterUnit, filterSide, userCode, copyPart, selectedTrackIds, loggedInUserRole, out iErrorId), 0);
        }

        public void ConsolidateParticipants(string catNo, string userCode, out Int32 iErrorId, out string errorMsg)
        {
            trackListingDAL.ConsolidateParticipants(catNo, userCode, out iErrorId, out errorMsg);
        }


        /// <summary>
        /// 1.If ISRC_DEAL.STATUS_CODE  = 0, then display 'No Participants' in Royaltor
        ///   Display 'new' line for Participant to be added
        /// 2.do not display participant rows with is_track_editable = N
        /// Tracks to be greyed out and be non editable when artist.Responsibility Code = 90 (TBA) on the ARTIST row for the ISRC ARTIST_ID                                               
        /// </summary>
        /// <param name="trackListingData"></param>
        /// <returns></returns>
        private DataSet ProcessTrackListingData(DataSet trackListingData, int tableNum)
        {
            if (trackListingData.Tables[tableNum].Rows.Count != 0)
            {
                trackListingData.Tables[tableNum].TableName = "dtTrackListing";

                //Adding 'No Participant' rows
                //process only if data contains isrc deals where status_code = 0 and is_track_editable = 'Y' 
                DataRow[] dtIsExist = trackListingData.Tables["dtTrackListing"].Select("display_order = 1 AND  status_code = 0 AND is_track_editable = 'Y'");
                if (dtIsExist.Count() > 0)
                {
                    DataTable dtChangedData = trackListingData.Tables["dtTrackListing"].Clone();
                    dtChangedData.TableName = "dtTrackListing";
                    foreach (DataRow dr in trackListingData.Tables["dtTrackListing"].Rows)
                    {

                        if (dr["display_order"].ToString() == "1" && dr["status_code"].ToString() == "0" && dr["is_track_editable"].ToString() == "Y")
                        {
                            dtChangedData.ImportRow(dr);
                            DataRow drInsert = dtChangedData.NewRow();
                            drInsert["rownum"] = dr["rownum"].ToString();
                            drInsert["display_order"] = "2";
                            drInsert["track_listing_id"] = dr["track_listing_id"].ToString();
                            drInsert["seq_no"] = dr["seq_no"].ToString();
                            drInsert["isrc"] = dr["isrc"].ToString();
                            drInsert["track_title"] = DBNull.Value;
                            drInsert["artist_name"] = DBNull.Value;
                            drInsert["responsibility_desc"] = DBNull.Value;
                            drInsert["play_length"] = DBNull.Value;
                            drInsert["isrc_part_id"] = DBNull.Value;
                            drInsert["isrc_deal_id"] = dr["isrc_deal_id"].ToString();
                            drInsert["royaltor_id"] = DBNull.Value;
                            drInsert["royaltor"] = DBNull.Value;
                            drInsert["option_period_code"] = DBNull.Value;
                            drInsert["seller_group_code"] = DBNull.Value;
                            drInsert["esc_code"] = DBNull.Value;
                            drInsert["inc_in_escalation"] = "Y";//default Y                            
                            drInsert["active"] = "Y";//Set Active as checked by default
                            drInsert["comments"] = DBNull.Value;
                            drInsert["status_code"] = DBNull.Value;
                            //drInsert["process_flag"] = "Y";
                            drInsert["exclude"] = "N";//Set as N by default
                            drInsert["is_track_editable"] = "Y";
                            drInsert["is_modified"] = "-";
                            drInsert["is_consolidated"] = "N";//Set as N by default - WUIN-1095

                            dtChangedData.Rows.Add(drInsert);
                        }
                        else
                        {
                            dtChangedData.ImportRow(dr);
                        }

                    }

                    trackListingData.Tables.RemoveAt(tableNum);
                    trackListingData.Tables.Add(dtChangedData);

                }

                //harish - 30-10-17 - handling where artist.Responsibility Code = 90 (TBA) on the ARTIST row for the ISRC.ARTIST_ID
                //removing participant rows where is_track_editable = 'N'
                DataRow[] dtIsExist2 = trackListingData.Tables["dtTrackListing"].Select("display_order = 2 AND  is_track_editable = 'N'");
                if (dtIsExist2.Count() > 0)
                {
                    foreach (DataRow dr in dtIsExist2)
                    {
                        trackListingData.Tables["dtTrackListing"].Rows.Remove(dr);
                    }
                }


            }


            return trackListingData;


        }



        /// <summary>
        /// 1.If ISRC_DEAL.STATUS_CODE  = 0, then display 'No Participants' in Royaltor
        ///   Display 'new' line for Participant to be added
        /// 2.do not display participant rows with is_track_editable = N
        /// Tracks to be greyed out and be non editable when artist.Responsibility Code = 90 (TBA) on the ARTIST row for the ISRC ARTIST_ID  
        /// </summary>
        /// <param name="trackListingData"></param>
        /// <returns></returns>
        private DataTable ProcessTrackListingData(DataTable trackListingData)
        {
            DataTable dtReturn = new DataTable();


            if (trackListingData.Rows.Count != 0)
            {
                //Adding 'No Participant' rows
                //process only if data contains isrc deals where status_code = 0 and is_track_editable = 'Y'
                DataRow[] dtIsExist = trackListingData.Select("display_order = 1 AND  status_code = 0 AND is_track_editable = 'Y'");
                if (dtIsExist.Count() > 0)
                {
                    DataTable dtChangedData = trackListingData.Clone();
                    foreach (DataRow dr in trackListingData.Rows)
                    {

                        if (dr["display_order"].ToString() == "1" && dr["status_code"].ToString() == "0" && dr["is_track_editable"].ToString() == "Y")
                        {
                            dtChangedData.ImportRow(dr);
                            DataRow drInsert = dtChangedData.NewRow();
                            drInsert["rownum"] = dr["rownum"].ToString();
                            drInsert["display_order"] = "2";
                            drInsert["track_listing_id"] = dr["track_listing_id"].ToString();
                            drInsert["seq_no"] = dr["seq_no"].ToString();
                            drInsert["isrc"] = dr["isrc"].ToString();
                            drInsert["track_title"] = DBNull.Value;
                            drInsert["artist_name"] = DBNull.Value;
                            drInsert["responsibility_desc"] = DBNull.Value;
                            drInsert["play_length"] = DBNull.Value;
                            drInsert["isrc_part_id"] = DBNull.Value;
                            drInsert["isrc_deal_id"] = dr["isrc_deal_id"].ToString();
                            drInsert["royaltor_id"] = DBNull.Value;
                            drInsert["royaltor"] = DBNull.Value;
                            drInsert["option_period_code"] = DBNull.Value;
                            drInsert["seller_group_code"] = DBNull.Value;
                            drInsert["esc_code"] = DBNull.Value;
                            drInsert["inc_in_escalation"] = "Y";//default Y
                            drInsert["active"] = "Y";//Set Active as checked by default
                            drInsert["comments"] = DBNull.Value;
                            drInsert["status_code"] = DBNull.Value;
                            //drInsert["process_flag"] = "Y";
                            drInsert["exclude"] = "N";//Set as N by default
                            drInsert["is_track_editable"] = "Y";
                            drInsert["is_modified"] = "-";
                            drInsert["is_consolidated"] = "N";//Set as N by default - WUIN-1095

                            dtChangedData.Rows.Add(drInsert);
                        }
                        else
                        {
                            dtChangedData.ImportRow(dr);
                        }

                    }

                    dtReturn = dtChangedData.Copy();

                }
                else
                {
                    dtReturn = trackListingData.Copy();
                }

                //harish - 30-10-17 - handling where artist.Responsibility Code = 90 (TBA) on the ARTIST row for the ISRC.ARTIST_ID
                //removing participant rows where is_track_editable = 'N'
                DataRow[] dtIsExist2 = dtReturn.Select("display_order = 2 AND  is_track_editable = 'N'");
                if (dtIsExist2.Count() > 0)
                {
                    foreach (DataRow dr in dtIsExist2)
                    {
                        dtReturn.Rows.Remove(dr);
                    }
                }

            }
            else
            {
                dtReturn = trackListingData.Copy();
            }


            return dtReturn;


        }




    }
}
