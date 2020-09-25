/*
File Name   :   TrackListing.cs
Purpose     :   to enable Participants to be maintained on the Track Listing for a Product.

Version  Date            Modified By           Modification Log
______________________________________________________________________
1.0     21-Jun-2017      Harish                Initial Creation
1.1     12-Jan-2018      Harish                WUIN-350
        01-Feb-2018      Harish                WUIN-419 - Expand track participants after Copy to all tracks
        06-Feb-2018      Harish                WUIN-467 - Error consolidating Track participant changes(participation details are not refreshed after save)
        12-Feb-2018      Harish                WUIN-489 - Error consolidating track listing participants when total share tracks is emplty for catno
        14-Feb-2018      Harish                WUIN-444 - Track Listing - copy track participants only to tracks not signed off
        14-Feb-2018      Harish                WUIN-496 - Enable consolidate button only if all tracks are signed off (status_code >= 2)
        09-Mar-2018      Harish                WUIN-522 - Change royaltor option selection on Track Listing
        14-Mar-2018      Harish                WUIN-478 - Set UPDATED flag at Track level if Participants updated for consolidated CATNO
        06-Jun-2018      Harish                WUIN-499 - Change Track Listing Participant Consolidation to call common process
 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using System.Web.UI.HtmlControls;
using WARS.BusinessLayer;
using System.Net;
using System.Configuration;

namespace WARS
{
    public partial class TrackListing : System.Web.UI.Page
    {
        #region Global Declarations
        TrackListingBL trackListingBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataSet trackListingData;
        DataTable dtGridData;
        DataTable dtTrkListingPagingData;
        string catalogueNo = string.Empty;
        string trackStatus = string.Empty;
        string gridDataOrderBy = "seq_no, isrc, display_order";
        string royaltorFocus;
        bool valConsolidateIncInEsc = true;
        string wildCardChar = ".";
        string rowIndexOfTrackHeaderRow = string.Empty;//to handle track header level row from participant rows
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["TrackListingGridDefaultPageSize"].ToString());
        //int gridDefaultPageSize = 10;//**TESTING
        #endregion Global Declarations

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                catalogueNo = Request.QueryString["CatNo"];

                //TESTING ********************
                //catalogueNo = "A10302B00000178745";
                //catalogueNo = "A10302B00004635030";//one participant
                //catalogueNo = "0825646252824";//No participants                
                //catalogueNo = "5054196914356";                                
                //catalogueNo = "0825646513291";//many tracks
                //catalogueNo = "A10302B0002013979J";
                //catalogueNo = "A10302B0001709359S";

                //u14
                //catalogueNo = "A10302B000000001C3";
                //catalogueNo = "0825646032754";
                //catalogueNo = "A10302B0003308236Z"; //u14                
                //catalogueNo = "A10302B00023988650";
                //catalogueNo = "0190295971977"; //u14
                //catalogueNo = "A10302B0001401442N"; //u14

                //catalogueNo = "0094633262720"; //Q14

                if (catalogueNo == null || catalogueNo == string.Empty)
                {
                    msgView.SetMessage("Not a valid catalogue number!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Track Listing";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Track Listing";
                }

                lblTab.Focus();//tabbing sequence starts here                                
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        hdnUserRole.Value = Session["UserRole"].ToString();
                        LoadTrackListingData();

                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }


                }

                UserAuthorization();

            }
            catch (ThreadAbortException ex1)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void btnConsolidate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation:Validate Inc in Esc has same value for a Participant for all Tracks 
                // if different values, cannot consolidate (Display message 'One or more Participants has multiple values for Include in Escalation')
                valConsolidateIncInEsc = true;
                ValidateConsolidateIncInEsc();

                if (valConsolidateIncInEsc == false)
                {
                    msgView.SetMessage("Cannot consolidate. One or more Participants has multiple values for Include in Escalation", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //WUIN-499 - change
                //consolidate participants by calling common(used for Engine auto consolidation as well) consolidation procedure
                //participants auto consolidation procedure returns few validation messages that needs to be displayed 
                string errorMessage = string.Empty;
                trackListingBL = new TrackListingBL();
                trackListingBL.ConsolidateParticipants(lblCatNo.Text, Convert.ToString(Session["UserCode"]), out errorId, out errorMessage);
                trackListingBL = null;

                if (errorId == 0)
                {
                    //redirect to Participant summary screen
                    Response.Redirect("../Participants/ParticipantSummary.aspx?CatNo=" + lblCatNo.Text, false);
                }
                else if (errorId == 1)
                {
                    msgView.SetMessage(errorMessage, MessageType.Warning, PositionType.Auto);
                    LoadTrackListingData(); //WUIN-1100 Reloading the data after consolidation failed.
                }
                else
                {
                    ExceptionHandler("Error in consolidating participants", string.Empty);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in consolidating participants", ex.Message);
            }
            finally
            {
                mpeConfirmConsolidate.Hide();
            }
        }

        protected void btnPartSummary_Click(object sender, EventArgs e)
        {

        }

        protected void btnSaveAllChanges_Click(object sender, EventArgs e)
        {
            Page.Validate("valUpdate");
            if (!Page.IsValid)
            {
                msgView.SetMessage("Paricipant details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                return;
            }

            //check if cat details changed
            string catPerviousStatus = hdnCatStatusCode.Value;
            string catCurrentStatus = ddlCatStatus.SelectedValue;
            string trackTimeFlag = (rbCatTrackShare.Checked == true ? "T" : "M");
            string prevTrackTimeFlag = hdnTrackTimeFlag.Value;
            string catStatusChanged = (catPerviousStatus != catCurrentStatus) ? "Y" : "N";
            string catFlagChanged = (prevTrackTimeFlag != trackTimeFlag) ? "Y" : "N";


            List<Array> changeList = ModifiedRowsList();


            //check if any changes to save
            if (catStatusChanged == "N" && catFlagChanged == "N" && changeList[0].Length == 0 && changeList[1].Length == 0)
            {
                msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                return;
            }


            trackListingBL = new TrackListingBL();
            DataSet dsTrackListing = trackListingBL.SaveAllTrackDetails(lblCatNo.Text, (ddlTrackStatusFilter.SelectedIndex == 0 ? string.Empty : ddlTrackStatusFilter.SelectedValue),
                                                                     txtUnitFilter.Text, txtSideFilter.Text, catCurrentStatus, trackTimeFlag, catStatusChanged, catFlagChanged,
                                                                      Convert.ToString(Session["UserCode"]),
                                                                     changeList[0], changeList[1], Convert.ToString(Session["UserRoleId"]), out errorId);
            trackListingBL = null;


            if (errorId == 2)
            {
                ExceptionHandler("Error in saving track details", string.Empty);
                return;
            }

            Session["TrackListingGridData"] = dsTrackListing.Tables["dtTrackListing"];
            Session["TrackListingInitialData"] = dsTrackListing.Tables["dtTrackListing"];
            ViewState["vsDeleteIds"] = null;
            hdnGridDataDeleted.Value = "N";
            hdnBulkStatusUpdate.Value = "N";


            LoadGridData(dsTrackListing.Tables["dtTrackListing"]);

            PopulateCatalogueDetails(dsTrackListing.Tables["dtCatDetails"], null);

            EnableDisableConsolidateButton();

        }

        protected void btnSaveComment_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnSaveDelete = (Button)sender;
                string saveOrDelete = string.Empty;
                if (btnSaveDelete.Text == "Save")
                {
                    saveOrDelete = "S";
                }
                else if (btnSaveDelete.Text == "Delete")
                {
                    saveOrDelete = "D";
                }

                string comment = string.Empty;
                if (txtComment.Text.Trim().Length > 510)
                {
                    comment = txtComment.Text.Trim().Substring(0, 510);
                }
                else
                {
                    comment = txtComment.Text.Trim();
                }

                trackListingBL = new TrackListingBL();
                DataSet dsTrackListing = trackListingBL.SaveComment(lblCatNo.Text, (ddlTrackStatusFilter.SelectedIndex == 0 ? string.Empty : ddlTrackStatusFilter.SelectedValue),
                                                                            txtUnitFilter.Text, txtSideFilter.Text, hdnCommentISRCDealId.Value.ToString(), comment, saveOrDelete, Convert.ToString(Session["UserRoleId"]), Convert.ToString(Session["UserCode"]), out errorId);//JIRA-898
                trackListingBL = null;
                Session["TrackListingGridData"] = dsTrackListing.Tables["dtTrackListing"];
                Session["TrackListingInitialData"] = dsTrackListing.Tables["dtTrackListing"];

                if (errorId == 2)
                {
                    ExceptionHandler("Error in saving comment", string.Empty);
                    return;
                }


                if (dsTrackListing.Tables["dtTrackListing"].Rows.Count == 0)
                {
                    gvTrackListing.EmptyDataText = "No data found for the selected catalogue number";
                }

                LoadGridData(dsTrackListing.Tables["dtTrackListing"]);
                PopulateCatalogueDetails(dsTrackListing.Tables["dtCatDetails"], null);


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving comment", ex.Message);
            }

        }

        protected void gvTrackListing_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DropDownList ddlOptionPeriod;
                TextBox txtOptionPeriod;
                //DropDownList ddlTerritory;
                DropDownList ddlEscCode;
                DropDownList ddlStatus;
                CheckBox cbIncInEsc;
                CheckBox cbActive;
                CheckBox cbExclude;
                TextBox txtRoyaltor;
                TextBox txtTerritory;
                //RequiredFieldValidator rfvtxtRoyaltor;
                CustomValidator valtxtRoyaltor;
                string trackId;
                string displayOrder;
                string royaltorId;
                string optionPrdCode;
                string statusCode;
                string sellerGrpCode;
                string escCode;
                string incInEsc;
                string isActive;
                string isExclude;
                //string processFlag;
                string isTrackEditable;
                string isrcPartId;
                //string territory;
                DataTable optionPrdList;
                DataTable escCodeList;
                string hdnAutoSignoff;
                string isConsolidated;

                if (trackListingData == null)
                {
                    trackListingData = (DataSet)Session["TrackListingDropdownData"];
                }
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    displayOrder = (e.Row.FindControl("hdnDisplayOrder") as HiddenField).Value;
                    ddlOptionPeriod = (e.Row.FindControl("ddlOptionPeriod") as DropDownList);
                    txtOptionPeriod = (e.Row.FindControl("txtOptionPeriod") as TextBox);
                    ddlEscCode = (e.Row.FindControl("ddlEscCode") as DropDownList);
                    ddlStatus = (e.Row.FindControl("ddlStatus") as DropDownList);
                    cbIncInEsc = (e.Row.FindControl("cbIncInEsc") as CheckBox);
                    cbActive = (e.Row.FindControl("cbActive") as CheckBox);
                    cbExclude = (e.Row.FindControl("cbExclude") as CheckBox);
                    txtRoyaltor = (e.Row.FindControl("txtRoyaltor") as TextBox);
                    txtTerritory = (e.Row.FindControl("txtTerritory") as TextBox);
                    valtxtRoyaltor = (e.Row.FindControl("valtxtRoyaltor") as CustomValidator);
                    isTrackEditable = (e.Row.FindControl("hdnIsTrackEditable") as HiddenField).Value;
                    trackId = (e.Row.FindControl("hdnTrackListingId") as HiddenField).Value;
                    isrcPartId = (e.Row.FindControl("hdnISRCPartId") as HiddenField).Value;
                    hdnAutoSignoff = (e.Row.FindControl("hdnAutoSignoff") as HiddenField).Value; //953 Changes

                    //WUIN-634 - validations
                    //When Catalogue status is in Manager sign off, Editor user cannot edit any fields in the screen
                    //when Track is in Manager sign off, Editor user cannot edit Participants of the track

                    if (displayOrder == "1")//Track hearder row
                    {
                        isExclude = (e.Row.FindControl("hdnExclude") as HiddenField).Value;
                        statusCode = (e.Row.FindControl("hdnStatusCode") as HiddenField).Value;
                        (e.Row.FindControl("txtRoyaltor") as TextBox).Visible = false;
                        (e.Row.FindControl("imgBtnAddParticipant") as ImageButton).Visible = true;
                        (e.Row.FindControl("imgBtnCopy") as ImageButton).Visible = false;
                        (e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = false;
                        (e.Row.FindControl("txtTerritory") as TextBox).Visible = false;

                        valtxtRoyaltor.Visible = false;
                        ddlOptionPeriod.Visible = false;
                        txtOptionPeriod.Visible = false;
                        ddlEscCode.Visible = false;
                        cbIncInEsc.Visible = false;
                        cbActive.Visible = false;
                        trackStatus = string.Empty;

                        //JIRA-953 Changes by Ravi on 01/03/2019 --Start
                        if (hdnAutoSignoff == "Y" && isExclude != "Y")
                        {
                            (e.Row.FindControl("imgAutoSignoff") as Image).Visible = true;
                            ddlStatus.Style.Add("width", "80%");
                        }
                        //JIRA-953 Changes by Ravi on 01/03/2019 --End

                        //If ISRC_DEAL.STATUS_CODE  = 0, then display 'No Participants' in Royaltor
                        if ((e.Row.FindControl("txtRoyaltor") as TextBox).Text != string.Empty && isExclude != "Y")
                        {
                            (e.Row.FindControl("lblRoyaltor") as Label).Visible = true;
                        }

                        if (trackListingData.Tables["dtISRCStatusList"] != null)
                        {
                            ddlStatus.DataSource = trackListingData.Tables["dtISRCStatusList"];
                            ddlStatus.DataTextField = "item_text";
                            ddlStatus.DataValueField = "item_value";
                            ddlStatus.DataBind();
                            //ddlStatus.Items.Insert(0, new ListItem("-"));

                            if (ddlStatus.Items.FindByValue(statusCode) != null)
                            {
                                ddlStatus.Items.FindByValue(statusCode).Selected = true;
                            }

                            else
                            {
                                ddlStatus.SelectedIndex = 0;
                            }

                        }
                        else
                        {
                            ddlStatus.Items.Insert(0, new ListItem("-"));
                            ddlStatus.SelectedIndex = 0;
                        }

                        trackStatus = ddlStatus.SelectedValue;

                        //WUIN-1191 - Handling track status across pages when status changed on catalogue bulk status update.
                        if (hdnBulkStatusUpdate.Value == "Y")
                        {
                            ddlStatus.SelectedValue = ddlCatStatus.SelectedValue;
                        }

                        if ((e.Row.FindControl("hdnComments") as HiddenField).Value != string.Empty)
                        {
                            (e.Row.FindControl("imgBtnCommentWithOutLine") as ImageButton).Visible = false;
                        }
                        else
                        {
                            (e.Row.FindControl("imgBtnCommentWithLine") as ImageButton).Visible = false;
                        }

                        e.Row.Cells[0].Font.Bold = true;
                        e.Row.Cells[1].Font.Bold = true;
                        e.Row.Cells[2].Font.Bold = true;
                        e.Row.Cells[3].Font.Bold = true;
                        e.Row.Cells[4].Font.Bold = true;
                        e.Row.Cells[5].Font.Bold = true;

                        //WUIN-1167 Any changes made to the catalogue which is at manager sign off/team sign off should move back to under review irrespective of the user role
                        //COmmenting below as part of WUIN-1167

                        //validation - if catalogue status is 3, only super user can make any changes to participants
                        //             if track status is 3 then only super user can make any changes either to that track and participants in the track
                        //AUTO SIGN OFF for Tracks -  If CATNO.CONFIG_CODE in CONFIG_CONFIG_GROUP.CONFIG_CODE where CONFIG_CONFIG_GROUP.CONFIG_GROUP_CODE = 'AUTO SIGN'          
                        //             allow Editor user to make changes(to participants) at any status 
                        //JIRA-983 Changes by Ravi on 26/02/2019 -- Start
                        //if (((hdnUserRole.Value.ToLower() != UserRole.SuperUser.ToString().ToLower() && (hdnUserRole.Value.ToLower() != UserRole.Supervisor.ToString().ToLower())) && (ddlCatStatus.SelectedValue == "3" || trackStatus == "3")
                        //    && hdnCatAutoSign.Value != "Y"))
                        ////JIRA-983 Changes by Ravi on 26/02/2019 -- End
                        //{
                        //    (e.Row.FindControl("imgBtnSaveTrack") as ImageButton).Visible = false;
                        //    (e.Row.FindControl("imgBtnAddParticipant") as ImageButton).Visible = false;
                        //    (e.Row.FindControl("imgBtnUndo") as ImageButton).Visible = false;
                        //}

                        #region to handle Track header level row based on participant row
                        //WUIN-601 - Enable Exclude checkbox when no isrc participant exists for the track
                        //Enabled if there are participants but all are in-active
                        cbExclude.Enabled = true;//This will be disabled if validation fails for the participant rows
                        rowIndexOfTrackHeaderRow = e.Row.RowIndex.ToString();
                        #endregion to handle Track header level row based on participant row

                        //WUIN-601 - validation - Grey out Tracks with ARTIST.TEAM_RESPONSIBILITY where RESPONSIBILITY.RESPONSIBILITY_TYPE = 'N' for ISRC.ARTIST_ID or with EXCLUDE = 'Y'; no edit allowed
                        //Exclude checkbox should be visible,enabled if EXCLUDE = 'Y' so that it can be un checked and saved
                        if (isTrackEditable == "N")
                        {
                            (e.Row.FindControl("imgExpand") as ImageButton).Visible = false;
                            (e.Row.FindControl("imgCollapse") as ImageButton).Visible = false;

                            e.Row.Cells[1].Style.Add("background-color", "#DCDCDC");
                            e.Row.Cells[2].Style.Add("background-color", "#DCDCDC");
                            e.Row.Cells[3].Style.Add("background-color", "#DCDCDC");
                            e.Row.Cells[4].Style.Add("background-color", "#DCDCDC");
                            e.Row.Cells[5].Style.Add("background-color", "#DCDCDC");
                            e.Row.Cells[6].Style.Add("background-color", "#DCDCDC");

                            e.Row.Cells[1].Font.Bold = false;
                            e.Row.Cells[2].Font.Bold = false;
                            e.Row.Cells[3].Font.Bold = false;
                            e.Row.Cells[4].Font.Bold = false;
                            e.Row.Cells[5].Font.Bold = false;
                            e.Row.Cells[6].Font.Bold = false;

                            cbExclude.Visible = false;
                            ddlStatus.Visible = false;
                            (e.Row.FindControl("imgBtnAddParticipant") as ImageButton).Visible = false;
                            (e.Row.FindControl("imgBtnCommentWithOutLine") as ImageButton).Visible = false;
                            (e.Row.FindControl("imgBtnCommentWithLine") as ImageButton).Visible = false;
                            (e.Row.FindControl("imgBtnUndo") as ImageButton).Visible = false;

                            if (isExclude == "Y")
                            {
                                cbExclude.Visible = true;
                                cbExclude.Enabled = true;
                                cbExclude.Checked = true;
                            }

                        }


                    }
                    else//Track child row
                    {

                        (e.Row.FindControl("imgExpand") as ImageButton).Visible = false;
                        (e.Row.FindControl("imgCollapse") as ImageButton).Visible = false;
                        (e.Row.FindControl("lblTrack") as Label).Visible = false;
                        (e.Row.FindControl("lblISRC") as TextBox).Visible = false;
                        (e.Row.FindControl("lblTrackTitle") as Label).Visible = false;
                        (e.Row.FindControl("lblArtistName") as Label).Visible = false;
                        (e.Row.FindControl("lblResponsibility") as Label).Visible = false;
                        (e.Row.FindControl("lblTrackTime") as Label).Visible = false;
                        ddlStatus.Visible = false;
                        (e.Row.FindControl("imgBtnAddParticipant") as ImageButton).Visible = false;
                        (e.Row.FindControl("imgBtnCommentWithLine") as ImageButton).Visible = false;
                        (e.Row.FindControl("imgBtnCommentWithOutLine") as ImageButton).Visible = false;
                        cbExclude.Visible = false;
                        //Do not display copy option for newly added row which is not saved
                        if ((e.Row.FindControl("hdnIsModified") as HiddenField).Value != Global.DBNullParamValue)
                        {
                            (e.Row.FindControl("imgBtnCopy") as ImageButton).Visible = true;
                        }

                        royaltorId = (e.Row.FindControl("hdnRoyaltorId") as HiddenField).Value;
                        optionPrdCode = (e.Row.FindControl("hdnOptPeriodCode") as HiddenField).Value;
                        sellerGrpCode = (e.Row.FindControl("hdnSellerGrpCode") as HiddenField).Value;
                        escCode = (e.Row.FindControl("hdnEscCode") as HiddenField).Value;
                        isActive = (e.Row.FindControl("hdnActive") as HiddenField).Value;
                        incInEsc = (e.Row.FindControl("hdnIncInEsc") as HiddenField).Value;
                        isConsolidated = (e.Row.FindControl("hdnIsConsolidated") as HiddenField).Value;
                        if (royaltorId != string.Empty)
                        {
                            //get option period list for the royaltor
                            optionPrdList = GetOptionPeriodList(royaltorId);
                            if (optionPrdList != null)
                            {
                                ddlOptionPeriod.DataSource = optionPrdList;
                                ddlOptionPeriod.DataTextField = "item_text";
                                ddlOptionPeriod.DataValueField = "item_value";
                                ddlOptionPeriod.DataBind();
                                ddlOptionPeriod.Items.Insert(0, new ListItem("-"));

                                if (ddlOptionPeriod.Items.FindByValue(optionPrdCode) != null)
                                {
                                    ddlOptionPeriod.Items.FindByValue(optionPrdCode).Selected = true;
                                    txtOptionPeriod.Text = ddlOptionPeriod.SelectedItem.Text;
                                }
                                else
                                {
                                    ddlOptionPeriod.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                                ddlOptionPeriod.SelectedIndex = 0;
                            }

                            //get escalation code list for the royaltor
                            escCodeList = GetEscCodeList(royaltorId);
                            if (escCodeList != null)
                            {
                                ddlEscCode.DataSource = escCodeList;
                                ddlEscCode.DataTextField = "esc_code";
                                ddlEscCode.DataValueField = "esc_code";
                                ddlEscCode.DataBind();
                                ddlEscCode.Items.Insert(0, new ListItem("-"));

                                if (ddlEscCode.Items.FindByValue(escCode) != null)
                                {
                                    ddlEscCode.Items.FindByValue(escCode).Selected = true;
                                }
                                else
                                {
                                    ddlEscCode.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                ddlEscCode.Items.Insert(0, new ListItem("-"));
                                ddlEscCode.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                            ddlOptionPeriod.SelectedIndex = 0;

                            ddlEscCode.Items.Insert(0, new ListItem("-"));
                            ddlEscCode.SelectedIndex = 0;
                        }

                        if (incInEsc == "Y")
                        {
                            cbIncInEsc.Checked = true;
                        }
                        else
                        {
                            cbIncInEsc.Checked = false;
                        }

                        if (isActive == "Y")
                        {
                            cbActive.Checked = true;
                        }
                        else
                        {
                            cbActive.Checked = false;
                        }

                        //validation - Allow delete only if Participant details not saved to PARTICPATION table 
                        //Harish 15-11-2017: Royaltor and option period cannot be changed for participants that are present in PARTICIPATION table
                        if ((e.Row.FindControl("hdnParticipationId") as HiddenField).Value == string.Empty)
                        {
                            (e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = true;
                            txtOptionPeriod.Visible = false;
                        }
                        else
                        {
                            (e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = false;
                            txtRoyaltor.ReadOnly = true;
                            (e.Row.FindControl("aceRoyaltor") as AjaxControlToolkit.AutoCompleteExtender).Enabled = false;
                            ddlOptionPeriod.Visible = false;
                        }

                        //WUIN-1095 validation -  Delete Icon shouldn't be visible against a royaltor on a track once it is consolidated in any product
                        if (isConsolidated == "Y")
                        {
                            (e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = false;
                        }
                        else
                        {
                            (e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = true;
                        }
                        //JIRA-1095 Changes -- End

                        //WUIN-1167 Any changes made to the catalogue which is at manager sign off/team sign off should move back to under review irrespective of the user role
                        //COmmenting below as part of WUIN-1167

                        //validation - if catalogue status is 3, only super user can make any changes to participants
                        // if track status is 3 then only super user can make any changes either to that track and participants in the track
                        //AUTO SIGN OFF for Tracks -  If CATNO.CONFIG_CODE in CONFIG_CONFIG_GROUP.CONFIG_CODE where CONFIG_CONFIG_GROUP.CONFIG_GROUP_CODE = 'AUTO SIGN'          
                        //             allow Editor user to make changes(to participants) at any status 
                        //JIRA-983 Changes by Ravi on 26/02/2019 -- Start
                        //if (((hdnUserRole.Value.ToLower() != UserRole.SuperUser.ToString().ToLower()) && (hdnUserRole.Value.ToLower() != UserRole.Supervisor.ToString().ToLower())) && (ddlCatStatus.SelectedValue == "3" || trackStatus == "3")
                        //    && hdnCatAutoSign.Value != "Y")
                        ////JIRA-983 Changes by Ravi on 26/02/2019 -- End
                        //{
                        //    (e.Row.FindControl("imgBtnSaveTrack") as ImageButton).Visible = false;
                        //    (e.Row.FindControl("imgBtnAddParticipant") as ImageButton).Visible = false;
                        //    (e.Row.FindControl("imgBtnCopy") as ImageButton).Visible = false;
                        //    (e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = false;
                        //    (e.Row.FindControl("imgBtnUndo") as ImageButton).Visible = false;
                        //}

                        //WUIN-258 - The cursor should be positioned in the royaltor box when the user press + to add a participate to an ISRC
                        if (royaltorFocus == (trackId + ";" + isrcPartId))
                        {
                            txtRoyaltor.Focus();
                        }

                        #region to handle Track header level row based on participant row
                        //WUIN-601 - Enable Exclude checkbox when no isrc participant exists for the track
                        //Enabled if there are participants but all are in-active
                        //Enabled if it is 'No Participants' track
                        if (isActive == "Y")
                        {
                            //disalbe if atleast one active participant and not a 'No Participants' row
                            GridViewRow row = gvTrackListing.Rows[Convert.ToInt32(rowIndexOfTrackHeaderRow)];
                            if ((row.FindControl("hdnStatusCode") as HiddenField).Value != "0")
                            {
                                CheckBox cbExcludeTrack = (CheckBox)row.FindControl("cbExclude");//Track header row exclude checkbox
                                cbExcludeTrack.Checked = false;
                                cbExcludeTrack.Enabled = false;
                            }
                        }

                        #endregion to handle Track header level row based on participant row

                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }

        protected void gvTrackListing_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                string trackListingId = (row.FindControl("hdnTrackListingId") as HiddenField).Value;
                string seqNo = (row.FindControl("hdnSeqNo") as HiddenField).Value;
                string ISRC = (row.FindControl("hdnISRC") as HiddenField).Value;
                string displayOrder = (row.FindControl("hdnDisplayOrder") as HiddenField).Value;
                string isrcDealId = (row.FindControl("hdnISRCDealId") as HiddenField).Value;
                string isrcPartId = (row.FindControl("hdnISRCPartId") as HiddenField).Value;
                string isModified = (row.FindControl("hdnIsModified") as HiddenField).Value;
                string royaltor = (row.FindControl("hdnRoyaltor") as HiddenField).Value;

                if (e.CommandName == "addParicipant")
                {
                    AddParticipantToTrack(trackListingId, seqNo, ISRC, isrcDealId, royaltor);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in modifying grid data", ex.Message);
            }
        }

        /* WUIN-522
        protected void txtRoyaltor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gvr = ((TextBox)sender).NamingContainer as GridViewRow;
                PopulateOptPrdEscCode(gvr);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }
        */

        /// <summary>
        /// copies participant to all tracks that are not signed off
        /// </summary>  
        protected void btnCopyParticip1All_Click(object sender, EventArgs e)
        {
            try
            {
                //Validate if some tracks have status not < 2, display a warning
                dtGridData = (DataTable)Session["TrackListingInitialData"];
                DataRow[] dtTracksSignedOff = dtGridData.Select("display_order = 1 AND status_code >= 2 AND exclude = 'N' AND track_listing_id <> " + hdnCopyParticipTrackListingId.Value);
                if (dtTracksSignedOff.Count() > 0)
                {
                    hdnCopyParticip3PopUp.Value = "All";
                    lblmpeCopyParticip3.Text = "Cannot copy to all tracks. Some tracks are signed off. Do you want to copy Participants to all other tracks?";
                    mpeCopyParticip3.Show();
                    return;
                }

                foreach (GridViewRow gvr in gvTrackListing.Rows)
                {
                    if (gvr.RowIndex.ToString() == hdnCopyParticipRowIndex.Value)
                    {
                        CopyParticipant(gvr, string.Empty);
                        break;
                    }
                }

                mpeCopyParticip1.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in copying participant", ex.Message);
            }
        }

        /// <summary>
        /// copies participant to all tracks on current page that are not signed off
        /// </summary>        
        protected void btnCopyParticip1CurPage_Click(object sender, EventArgs e)
        {
            try
            {
                //get tracks ids of current page other than the selected copy participant track
                foreach (GridViewRow gvr in gvTrackListing.Rows)
                {
                    if (gvr.RowIndex.ToString() == hdnCopyParticipRowIndex.Value)
                    {
                        string displayOrder;
                        string trackId;
                        string curPageTrackIds = string.Empty;
                        string copySelectedTrackId = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;

                        foreach (GridViewRow gvr2 in gvTrackListing.Rows)
                        {
                            displayOrder = (gvr2.FindControl("hdnDisplayOrder") as HiddenField).Value;
                            trackId = (gvr2.FindControl("hdnTrackListingId") as HiddenField).Value;

                            //skip if it is a participant row or the copy selected track
                            if (displayOrder == "2" || trackId == copySelectedTrackId)
                            {
                                continue;
                            }

                            if (curPageTrackIds == string.Empty)
                            {
                                curPageTrackIds = trackId;
                            }
                            else
                            {
                                curPageTrackIds = curPageTrackIds + "," + trackId;
                            }
                        }

                        CopyParticipant(gvr, curPageTrackIds);
                        break;
                    }
                }

                mpeCopyParticip1.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in copying participant", ex.Message);
            }
        }

        /// <summary>
        /// displays list of tracks to select to be copied
        /// </summary>  
        protected void btnCopyParticip1Selected_Click(object sender, EventArgs e)
        {
            try
            {
                //populate grid CopyParticipTrackList with list of tracks
                dtGridData = (DataTable)Session["TrackListingInitialData"];

                if (dtGridData.Rows.Count == 0)
                {
                    gvCopyParticipTrackList.DataSource = dtEmpty;
                    gvCopyParticipTrackList.DataBind();

                    mpeCopyParticip1.Hide();
                    mpeCopyParticip2.Show();

                    return;
                }

                //get tracks other than the selected one

                DataTable dtTracks = dtGridData.Select("display_order = 1 AND track_listing_id <> " + hdnCopyParticipTrackListingId.Value).CopyToDataTable();

                //sort display list
                DataView dv = dtTracks.DefaultView;
                dv.Sort = "seq_no";
                DataTable dtTracksSorted = dv.ToTable();

                gvCopyParticipTrackList.DataSource = dtTracksSorted;
                gvCopyParticipTrackList.DataBind();

                mpeCopyParticip1.Hide();
                mpeCopyParticip2.Show();

                //set pop up panel and gridview panel height                    
                pnlCopyParticip2.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.75).ToString());
                plnGridCopyParticipTrackList.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.75 - 100).ToString());


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in opening tracks list to copy participant popup", ex.Message);
            }
        }

        /// <summary>
        /// copies participant to selected tracks that are not signed off
        /// </summary>        
        protected void btnCopyParticip2Copy_Click(object sender, EventArgs e)
        {
            try
            {

                foreach (GridViewRow gvr in gvTrackListing.Rows)
                {
                    if (gvr.RowIndex.ToString() == hdnCopyParticipRowIndex.Value)
                    {
                        //get selected tracks ids
                        string selectedTrackIds = string.Empty;
                        CheckBox cbCopyTrackSelect;
                        foreach (GridViewRow gvr2 in gvCopyParticipTrackList.Rows)
                        {
                            cbCopyTrackSelect = (gvr2.FindControl("cbCopyTrackSelect") as CheckBox);
                            if (cbCopyTrackSelect.Checked)
                            {
                                if (selectedTrackIds == string.Empty)
                                {
                                    selectedTrackIds = (gvr2.FindControl("hdnTrackListingId") as HiddenField).Value;
                                }
                                else
                                {
                                    selectedTrackIds = selectedTrackIds + "," + (gvr2.FindControl("hdnTrackListingId") as HiddenField).Value;
                                }

                            }
                        }

                        CopyParticipant(gvr, selectedTrackIds);
                        break;
                    }
                }

                mpeCopyParticip2.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in copying participant to selected tracks", ex.Message);
            }
        }

        /// <summary>
        /// copies participant to all/selected/current page(based on selected copy criteria) tracks that are not signed off
        /// this is fired when selected 'Yes' on warning pop up        
        /// </summary>        
        protected void btnYesCopyParticip3PopUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnCopyParticip3PopUp.Value == "All")
                {
                    foreach (GridViewRow gvr in gvTrackListing.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnCopyParticipRowIndex.Value)
                        {
                            CopyParticipant(gvr, string.Empty);
                            break;
                        }
                    }
                }
                else if (hdnCopyParticip3PopUp.Value == "CurrentPage")
                {
                    //get tracks ids of current page other than the selected copy participant track
                    foreach (GridViewRow gvr in gvTrackListing.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnCopyParticipRowIndex.Value)
                        {
                            string displayOrder;
                            string trackId;
                            string curPageTrackIds = string.Empty;
                            string copySelectedTrackId = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;

                            foreach (GridViewRow gvr2 in gvTrackListing.Rows)
                            {
                                displayOrder = (gvr2.FindControl("hdnDisplayOrder") as HiddenField).Value;
                                trackId = (gvr2.FindControl("hdnTrackListingId") as HiddenField).Value;

                                //skip if it is a participant row or the copy selected track
                                if (displayOrder == "2" || trackId == copySelectedTrackId)
                                {
                                    continue;
                                }

                                if (curPageTrackIds == string.Empty)
                                {
                                    curPageTrackIds = trackId;
                                }
                                else
                                {
                                    curPageTrackIds = curPageTrackIds + "," + trackId;
                                }
                            }

                            CopyParticipant(gvr, curPageTrackIds);
                            break;
                        }
                    }
                }
                else if (hdnCopyParticip3PopUp.Value == "Selected")
                {
                    foreach (GridViewRow gvr in gvTrackListing.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnCopyParticipRowIndex.Value)
                        {
                            //get selected tracks ids
                            string selectedTrackIds = string.Empty;
                            CheckBox cbCopyTrackSelect;
                            foreach (GridViewRow gvr2 in gvCopyParticipTrackList.Rows)
                            {
                                cbCopyTrackSelect = (gvr2.FindControl("cbCopyTrackSelect") as CheckBox);
                                if (cbCopyTrackSelect.Checked)
                                {
                                    if (selectedTrackIds == string.Empty)
                                    {
                                        selectedTrackIds = (gvr2.FindControl("hdnTrackListingId") as HiddenField).Value;
                                    }
                                    else
                                    {
                                        selectedTrackIds = selectedTrackIds + "," + (gvr2.FindControl("hdnTrackListingId") as HiddenField).Value;
                                    }

                                }
                            }

                            CopyParticipant(gvr, selectedTrackIds);
                            break;
                        }
                    }
                }

                mpeCopyParticip3.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in copying participant", ex.Message);
            }
        }

        protected void btnFilterTracks_Click(object sender, EventArgs e)
        {
            try
            {
                trackListingBL = new TrackListingBL();
                DataTable dtTrackListing = trackListingBL.GetFilteredData(lblCatNo.Text, (ddlTrackStatusFilter.SelectedIndex == 0 ? string.Empty : ddlTrackStatusFilter.SelectedValue),
                                                                            txtUnitFilter.Text, txtSideFilter.Text, Convert.ToString(Session["UserRoleId"]), out errorId);
                trackListingBL = null;
                Session["TrackListingGridData"] = dtTrackListing;
                Session["TrackListingInitialData"] = dtTrackListing;

                if (errorId == 2)
                {
                    ExceptionHandler("Error in loading filtered grid data", string.Empty);
                    return;
                }

                if (dtTrackListing.Rows.Count == 0)
                {
                    gvTrackListing.EmptyDataText = "No data found for the selected filter criteria";
                }

                hdnPageIndex.Value = string.Empty;//need to load from page 1 after applying filters
                LoadGridData(dtTrackListing);

                hdnGridDataDeleted.Value = "N";//reset to default

                //Handling expand/collapse all on
                if (hdnExpandCollapseAll.Value == "Collapse")//Grid expand all is selected
                {
                    //populate hdnExpandedTrackId with all track ids
                    hdnExpandedTrackId.Value = string.Empty;
                    DataTable dtTrackIds = dtTrackListing.DefaultView.ToTable(true, "track_listing_id");
                    foreach (DataRow drTrack in dtTrackIds.Rows)
                    {
                        //add track listing id
                        if (hdnExpandedTrackId.Value == string.Empty)
                        {
                            hdnExpandedTrackId.Value = drTrack["track_listing_id"].ToString();
                        }
                        else
                        {
                            hdnExpandedTrackId.Value = hdnExpandedTrackId.Value + ";" + drTrack["track_listing_id"].ToString();
                        }
                    }
                }
                UserAuthorization();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading filtered grid data", ex.Message);
            }
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            try
            {
                //clear filter fields
                ddlTrackStatusFilter.SelectedIndex = 0;
                txtUnitFilter.Text = string.Empty;
                txtSideFilter.Text = string.Empty;

                //reload grid data
                btnFilterTracks_Click(sender, e);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "FilterTracks")
            {
                btnFilterTracks_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "ClearFilters")
            {
                btnClearFilters_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "OnPageChange")
            {
                lnkPage_Click(sender, e);
            }

            hdnIsConfirmPopup.Value = "N";

        }

        //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string isrcDealId = Convert.ToString(hdnDeleteISRCDealId.Value);
                string isrcPartId = Convert.ToString(hdnDeleteISRCPartId.Value);
                string displayOrder = Convert.ToString(hdnDeletedisplayOrder.Value);
                string isModified = Convert.ToString(hdnDeleteisModified.Value);
                if (isModified != "-")
                {
                    hdnGridDataDeleted.Value = "Y";
                }
                DeleteRowFromGrid(displayOrder, isrcDealId, isrcPartId, isModified);
                hdnDeleteISRCDealId.Value = "";
                hdnDeleteISRCPartId.Value = "";
                hdnDeletedisplayOrder.Value = "";
                hdnDeleteisModified.Value = "";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting grid data", ex.Message);
            }
        }
        //JIRA-908 Changes by Ravi on 13/02/2019 -- End

        #endregion Events

        #region Methods
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSaveComment.Enabled = false;
                btnDeleteComment.Enabled = false;
                btnConsolidate.Enabled = false;
                foreach (GridViewRow rows in gvTrackListing.Rows)
                {
                    (rows.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnAddParticipant") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnCopy") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;

                }
            }
        }

        private void LoadTrackListingData()
        {
            trackListingBL = new TrackListingBL();
            trackListingData = trackListingBL.GetTrackListing(catalogueNo, (ddlTrackStatusFilter.SelectedIndex == 0 ? string.Empty : ddlTrackStatusFilter.SelectedValue),
                                                                txtUnitFilter.Text, txtSideFilter.Text, Convert.ToString(Session["UserRoleId"]), out errorId); //JIRA-898 CHange
            trackListingBL = null;

            //name the datatables so that they can be used appropriately     
            trackListingData.Tables[0].TableName = "dtCatDetails";
            trackListingData.Tables[1].TableName = "dtCatNoStatusList";
            trackListingData.Tables[2].TableName = "dtISRCStatusList";
            trackListingData.Tables[3].TableName = "dtOptionPrdList";
            trackListingData.Tables[4].TableName = "dtSellerGrpList";
            trackListingData.Tables[5].TableName = "dtEscCodeList";
            if (trackListingData.Tables[7].TableName == "dtTrackListing")
            {
                trackListingData.Tables[6].TableName = "dtParticipation";
            }
            else
            {
                trackListingData.Tables[7].TableName = "dtParticipation";
                trackListingData.Tables[6].TableName = "dtTrackListing";
            }

            Session["TrackListingDropdownData"] = trackListingData;
            Session["TrackListingGridData"] = trackListingData.Tables["dtTrackListing"];
            Session["TrackListingParticipation"] = trackListingData.Tables["dtParticipation"];
            Session["TrackListingInitialData"] = trackListingData.Tables["dtTrackListing"];//used for undo/reset 
            Session["TrackListingSellerGrpList"] = trackListingData.Tables["dtSellerGrpList"];

            if (errorId == 2)
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
                return;
            }

            if (trackListingData.Tables.Count == 0)
            {
                dtEmpty = new DataTable();
                gvTrackListing.DataSource = dtEmpty;
                gvTrackListing.EmptyDataText = "No data found for the selected catalogue number";
                gvTrackListing.DataBind();

            }
            else
            {
                //populate status search filter dropdown
                ddlTrackStatusFilter.DataSource = trackListingData.Tables["dtISRCStatusList"];
                ddlTrackStatusFilter.DataTextField = "item_text";
                ddlTrackStatusFilter.DataValueField = "item_value";
                ddlTrackStatusFilter.DataBind();
                ddlTrackStatusFilter.Items.Insert(0, new ListItem("-"));

                PopulateCatalogueDetails(trackListingData.Tables["dtCatDetails"], trackListingData.Tables["dtCatNoStatusList"]);
                LoadGridData(trackListingData.Tables["dtTrackListing"]);

            }

            EnableDisableConsolidateButton();

        }

        /// <summary>
        /// to load Tracklistin grid data
        /// </summary>        
        private void LoadGridData(DataTable gvDataSource)
        {
            Session["TrackListingInitialData"] = gvDataSource;//used for paging 

            if (gvDataSource.Rows.Count == 0)
            {
                gvTrackListing.EmptyDataText = "No data found for the selected catalogue number";
            }

            if (gvDataSource.Rows.Count > (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value)))
            {
                SetPageStartEndRowNum(gvDataSource.Rows.Count);
                PopulateGridPage(hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value));
            }
            else
            {
                //clear pager
                Session["TrackListingPageStartEnd"] = null;
                rptPager.DataSource = null;
                rptPager.DataBind();

                gvTrackListing.DataSource = gvDataSource;
                gvTrackListing.DataBind();
            }
            UserAuthorization();
        }

        private void PopulateCatalogueDetails(DataTable catDetails, DataTable catStatusList)
        {
            try
            {
                if (catStatusList != null)
                {
                    ddlCatStatus.DataSource = catStatusList;
                    ddlCatStatus.DataTextField = "item_text";
                    ddlCatStatus.DataValueField = "item_value";
                    ddlCatStatus.DataBind();
                    //ddlCatStatus.Items.Insert(0, new ListItem("-"));
                }

                if (catDetails.Rows.Count != 0)
                {
                    lblCatNo.Text = catDetails.Rows[0]["catno"].ToString();
                    lblCatTitle.Text = catDetails.Rows[0]["catno_title"].ToString();
                    lblCatArtist.Text = catDetails.Rows[0]["artist_name"].ToString();
                    lblCatDealType.Text = catDetails.Rows[0]["deal_type_desc"].ToString();
                    lblCatTotalTracks.Text = catDetails.Rows[0]["total_tracks"].ToString();
                    lblCatTotalTime.Text = catDetails.Rows[0]["total_play_length"].ToString();
                    hdnTrackTimeFlag.Value = catDetails.Rows[0]["track_time_flag"].ToString();
                    hdnCatStatusCode.Value = catDetails.Rows[0]["status_code"].ToString();
                    hdnCatAutoSign.Value = catDetails.Rows[0]["auto_sign"].ToString();


                    if (catDetails.Rows[0]["is_compilation"].ToString() == "Y")
                    {
                        cbCatCompilation.Checked = true;
                    }
                    else
                    {
                        cbCatCompilation.Checked = false;
                    }

                    if (catDetails.Rows[0]["track_time_flag"].ToString() == "T")
                    {
                        rbCatTrackShare.Checked = true;
                        rbCatTimeShare.Checked = false;
                    }
                    else
                    {
                        rbCatTrackShare.Checked = false;
                        rbCatTimeShare.Checked = true;
                    }

                    ddlCatStatus.ClearSelection();
                    if (ddlCatStatus.Items.FindByValue(catDetails.Rows[0]["status_code"].ToString()) != null)
                    {
                        ddlCatStatus.Items.FindByValue(catDetails.Rows[0]["status_code"].ToString()).Selected = true;
                    }
                    else
                    {
                        ddlCatStatus.SelectedIndex = 0;
                    }


                }



            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading Catalogue details", ex.Message);
            }
        }

        /// <summary>        
        /// Validation:Validate Inc in Esc has same value for a Participant for all Tracks 
        /// if different values, cannot consolidate (Display message 'One or more Participants has multiple values for Include in Escalation')
        /// </summary>
        private void ValidateConsolidateIncInEsc()
        {
            string royaltorId = string.Empty;
            string optionPeriodCode = string.Empty;
            string sellerGroupCode = string.Empty;
            string ISRC = string.Empty;
            string trackListingId = string.Empty;
            string escCode = string.Empty;
            string incInEsc = string.Empty;
            string partType = string.Empty;
            string query = string.Empty;
            DataTable dtQuery = new DataTable();

            GetGridData();
            dtGridData = (DataTable)Session["TrackListingGridData"];

            //a participant is identified with unique combination of Royaltor, Option Period, Territory, Esc Code, PARTICPATION_TYPE combination on ISRC_PARTICIPANT
            DataTable dtUniqueParticipants = dtGridData.DefaultView.ToTable(true, "royaltor_id", "option_period_code", "seller_group_code", "esc_code", "active");

            foreach (DataRow drParticip in dtUniqueParticipants.Rows)
            {
                //track header row
                if (drParticip["royaltor_id"].ToString() == string.Empty)
                {
                    continue;
                }

                royaltorId = drParticip["royaltor_id"].ToString();
                optionPeriodCode = drParticip["option_period_code"].ToString();
                sellerGroupCode = drParticip["seller_group_code"].ToString();
                escCode = drParticip["esc_code"].ToString();
                partType = (drParticip["active"].ToString() == "Y" ? "A" : "I");

                query = "royaltor_id='" + royaltorId + "' AND option_period_code= '" + optionPeriodCode + "' ";

                if (sellerGroupCode == string.Empty)
                {
                    query = query + " AND seller_group_code is null ";

                }
                else
                {
                    query = query + " AND seller_group_code= '" + sellerGroupCode + "' ";
                }

                if (escCode == string.Empty)
                {
                    query = query + " AND esc_code is null ";
                }
                else
                {
                    query = query + " AND esc_code= '" + escCode + "' ";
                }

                query = query + " AND active= '" + drParticip["active"].ToString() + "' ";

                dtQuery = dtGridData.Select(query).CopyToDataTable();

                if (dtQuery.DefaultView.ToTable(true, "inc_in_escalation").Rows.Count > 1)
                {
                    valConsolidateIncInEsc = false;
                    break;
                }

            }



        }

        private void CopyParticipant(GridViewRow gvr, string selectedTrackIds)
        {
            try
            {
                string isrcPartId = (gvr.FindControl("hdnISRCPartId") as HiddenField).Value;
                string isrcDealId = (gvr.FindControl("hdnISRCDealId") as HiddenField).Value;
                string royaltor = (gvr.FindControl("txtRoyaltor") as TextBox).Text;
                string royaltorId = royaltor.Substring(0, royaltor.IndexOf("-") - 1);
                DropDownList ddlOptionPeriod = (gvr.FindControl("ddlOptionPeriod") as DropDownList);
                string territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                DropDownList ddlEscCode = (gvr.FindControl("ddlEscCode") as DropDownList);
                CheckBox cbActive = (gvr.FindControl("cbActive") as CheckBox);
                CheckBox cbIncInEsc = (gvr.FindControl("cbIncInEsc") as CheckBox);

                territory = (territory == string.Empty ? wildCardChar : territory.Substring(0, territory.IndexOf("-") - 1));

                string copyPart = isrcPartId + "(;||;)" + isrcDealId + "(;||;)" + royaltorId + "(;||;)" + ddlOptionPeriod.SelectedValue + "(;||;)" +
                                                territory + "(;||;)" +
                                                (ddlEscCode.SelectedIndex == 0 ? Global.DBNullParamValue : ddlEscCode.SelectedValue) + "(;||;)" +
                                                (cbActive.Checked ? "Y" : "N") + "(;||;)" + (cbIncInEsc.Checked ? "Y" : "N");

                trackListingBL = new TrackListingBL();
                DataSet dsTrackListing = trackListingBL.CopyParticipant(lblCatNo.Text, (ddlTrackStatusFilter.SelectedIndex == 0 ? string.Empty : ddlTrackStatusFilter.SelectedValue),
                                                                          txtUnitFilter.Text, txtSideFilter.Text, Convert.ToString(Session["UserCode"]), copyPart, selectedTrackIds, Convert.ToString(Session["UserRoleId"]), out errorId);//JIRA-898
                trackListingBL = null;

                Session["TrackListingGridData"] = dsTrackListing.Tables["dtTrackListing"];
                Session["TrackListingInitialData"] = dsTrackListing.Tables["dtTrackListing"];
                hdnGridDataDeleted.Value = "N";
                hdnBulkStatusUpdate.Value = "N";
                ViewState["vsDeleteIds"] = null;

                if (errorId == 2)
                {
                    ExceptionHandler("Error in copying participant", string.Empty);
                    return;
                }

                LoadGridData(dsTrackListing.Tables["dtTrackListing"]);
                PopulateCatalogueDetails(dsTrackListing.Tables["dtCatDetails"], null);


                //WUIN-419 - Expand track participants after Copy to all tracks
                //populate hdnExpandedTrackId with all the track id's
                DataTable dtTrackIds = dsTrackListing.Tables["dtTrackListing"].DefaultView.ToTable(true, "track_listing_id");
                foreach (DataRow drTrack in dtTrackIds.Rows)
                {
                    //append track listing id only if it is not present
                    string[] trackIds = hdnExpandedTrackId.Value.Split(';');
                    if (!trackIds.Contains(drTrack["track_listing_id"].ToString()))
                    {
                        hdnExpandedTrackId.Value = hdnExpandedTrackId.Value + ";" + drTrack["track_listing_id"].ToString();
                    }
                }

                msgView.SetMessage("Participant copied", MessageType.Warning, PositionType.Auto);


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in copying participant", ex.Message);
            }

        }

        private DataTable GetOptionPeriodList(string royaltorId)
        {
            DataTable optionPrdList = new DataTable();
            optionPrdList = null;

            if (trackListingData == null)
            {
                trackListingData = (DataSet)Session["TrackListingDropdownData"];
            }

            if (trackListingData.Tables["dtOptionPrdList"] != null)
            {
                DataRow[] optionPrdExist = trackListingData.Tables["dtOptionPrdList"].Select("royaltor_id='" + royaltorId + "'");
                if (optionPrdExist.Count() != 0)
                {
                    optionPrdList = trackListingData.Tables["dtOptionPrdList"].Select("royaltor_id='" + royaltorId + "'").CopyToDataTable();
                }

            }

            return optionPrdList;
        }

        private DataTable GetEscCodeList(string royaltorId)
        {
            DataTable escCodeList = new DataTable();
            escCodeList = null;

            if (trackListingData.Tables["dtEscCodeList"] != null)
            {
                DataRow[] escCodeExist = trackListingData.Tables["dtEscCodeList"].Select("royaltor_id='" + royaltorId + "'");
                if (escCodeExist.Count() != 0)
                {
                    escCodeList = trackListingData.Tables["dtEscCodeList"].Select("royaltor_id='" + royaltorId + "'").CopyToDataTable();
                }

            }

            return escCodeList;
        }

        private void DeleteRowFromGrid(string displayOrder, string isrcDealId, string isrcPartId, string isModified)
        {
            try
            {
                //add to delete list only if the row is not a new one 
                List<string> deleteList = new List<string>();

                if (isModified != Global.DBNullParamValue)
                {
                    if (ViewState["vsDeleteIds"] != null)
                    {
                        deleteList = (List<string>)ViewState["vsDeleteIds"];
                        deleteList.Add(isrcDealId + ";" + isrcPartId);
                    }
                    else
                    {
                        deleteList.Add(isrcDealId + ";" + isrcPartId);
                    }

                    ViewState["vsDeleteIds"] = deleteList;
                }

                GetGridData();
                dtGridData = (DataTable)Session["TrackListingGridData"];
                foreach (DataRow dr in dtGridData.Rows)
                {
                    if (dr["display_order"].ToString() == displayOrder && dr["isrc_part_id"].ToString() == isrcPartId)
                    {
                        dr.Delete();
                        break;
                    }

                }

                DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData);
                Session["TrackListingGridData"] = dtGridChangedDataSorted;
                AssignGridDropdownList();
                gvTrackListing.DataSource = dtGridChangedDataSorted;
                gvTrackListing.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting grid row", ex.Message);
            }

        }

        private void AddParticipantToTrack(string trackListingId, string seqNo, string ISRC, string isrcDealId, string royaltor)
        {
            try
            {
                if (Session["TrackListingDropdownData"] == null)
                {
                    ExceptionHandler("Error in adding royaltor row to grid", string.Empty);
                }

                GetGridData();
                dtGridData = (DataTable)Session["TrackListingGridData"];

                bool addRow = true;

                /*
                //if it is a no participant then don’t add two empty rows
                if (royaltor == "No Participants")
                {
                    DataRow[] dtEmptyRow = dtGridData.Select("track_listing_id =" + trackListingId + " AND display_order = 2 AND royaltor_id is null");
                    if (dtEmptyRow.Count() > 0)
                    {
                        addRow = false;
                    }
                }
                 * */

                //Do not allow more than one unused 'new' empty participant row under a track
                DataRow[] dtEmptyRow = dtGridData.Select("track_listing_id =" + trackListingId + " AND display_order = 2 AND royaltor_id is null");
                if (dtEmptyRow.Count() > 0)
                {
                    addRow = false;
                }

                if (addRow)
                {
                    DataRow drNewRow = dtGridData.NewRow();
                    drNewRow["rownum"] = "0";
                    drNewRow["display_order"] = "2";
                    drNewRow["track_listing_id"] = trackListingId;
                    drNewRow["seq_no"] = seqNo;
                    drNewRow["isrc"] = ISRC;
                    drNewRow["track_title"] = DBNull.Value;
                    drNewRow["artist_name"] = DBNull.Value;
                    drNewRow["responsibility_desc"] = DBNull.Value;
                    drNewRow["play_length"] = DBNull.Value;
                    //drNewRow["isrc_part_id"] = DBNull.Value;
                    drNewRow["isrc_deal_id"] = isrcDealId;
                    drNewRow["royaltor_id"] = DBNull.Value;
                    drNewRow["royaltor"] = DBNull.Value;
                    drNewRow["option_period_code"] = DBNull.Value;
                    drNewRow["seller_group_code"] = DBNull.Value;
                    drNewRow["esc_code"] = DBNull.Value;
                    drNewRow["inc_in_escalation"] = "Y";//Default display to checked for new participant
                    drNewRow["active"] = "Y";//Set Active as checked by default
                    drNewRow["comments"] = DBNull.Value;
                    drNewRow["status_code"] = DBNull.Value;
                    drNewRow["participation_id"] = DBNull.Value;
                    drNewRow["exclude"] = "N";//Set as N by default
                    drNewRow["manual_override"] = DBNull.Value;
                    drNewRow["is_track_editable"] = "Y";
                    drNewRow["is_modified"] = Global.DBNullParamValue;
                    drNewRow["is_consolidated"] = "N";//Set as N by default - WUIN-1095

                    if (hdnISRCPartIdAddRow.Value == string.Empty)
                    {
                        drNewRow["isrc_part_id"] = -1;
                        hdnISRCPartIdAddRow.Value = "-1";
                    }
                    else
                    {
                        int partId = Convert.ToInt32(hdnISRCPartIdAddRow.Value);
                        partId = partId - 1;
                        drNewRow["isrc_part_id"] = partId;
                        hdnISRCPartIdAddRow.Value = partId.ToString();
                    }

                    dtGridData.Rows.Add(drNewRow);
                }

                //to focus the royaltor textbox on adding a new participant row
                royaltorFocus = trackListingId + ";" + hdnISRCPartIdAddRow.Value;

                DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData);
                Session["TrackListingGridData"] = dtGridChangedDataSorted;
                AssignGridDropdownList();
                gvTrackListing.DataSource = dtGridChangedDataSorted;
                gvTrackListing.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding participant to track", ex.Message);
            }

        }

        private void AssignGridDropdownList()
        {
            trackListingData = (DataSet)Session["TrackListingDropdownData"];
        }

        private DataTable GridDataSorted(DataTable inputData)
        {
            DataView dv = inputData.DefaultView;
            dv.Sort = gridDataOrderBy;
            DataTable dtSorted = dv.ToTable();

            return dtSorted;

        }

        private void GetGridData()
        {
            dtGridData = (DataTable)Session["TrackListingGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();

            foreach (GridViewRow gvr in gvTrackListing.Rows)
            {
                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["rownum"] = (gvr.FindControl("hdnRowNum") as HiddenField).Value;
                drGridRow["display_order"] = (gvr.FindControl("hdnDisplayOrder") as HiddenField).Value;
                drGridRow["track_listing_id"] = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;
                drGridRow["seq_no"] = (gvr.FindControl("hdnSeqNo") as HiddenField).Value;
                drGridRow["isrc"] = (gvr.FindControl("hdnISRC") as HiddenField).Value;
                if ((gvr.FindControl("hdnDisplayOrder") as HiddenField).Value == "1")
                {
                    drGridRow["track_title"] = (gvr.FindControl("lblTrackTitle") as Label).Text;
                    drGridRow["artist_name"] = (gvr.FindControl("lblArtistName") as Label).Text;
                    drGridRow["responsibility_desc"] = (gvr.FindControl("lblResponsibility") as Label).Text;
                    drGridRow["play_length"] = (gvr.FindControl("lblTrackTime") as Label).Text;
                    drGridRow["isrc_part_id"] = DBNull.Value;
                    drGridRow["isrc_deal_id"] = (gvr.FindControl("hdnISRCDealId") as HiddenField).Value;
                    drGridRow["participation_id"] = DBNull.Value;
                    drGridRow["manual_override"] = DBNull.Value;
                    drGridRow["royaltor_id"] = DBNull.Value;
                    if ((gvr.FindControl("hdnRoyaltor") as HiddenField).Value == "No Participants")
                    {
                        drGridRow["royaltor"] = (gvr.FindControl("hdnRoyaltor") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["royaltor"] = DBNull.Value;
                    }
                    drGridRow["option_period_code"] = DBNull.Value;
                    drGridRow["seller_group_code"] = DBNull.Value;
                    drGridRow["esc_code"] = DBNull.Value;
                    drGridRow["inc_in_escalation"] = DBNull.Value;
                    drGridRow["active"] = DBNull.Value;
                    drGridRow["exclude"] = (gvr.FindControl("hdnExclude") as HiddenField).Value;
                    drGridRow["status_code"] = (gvr.FindControl("hdnStatusCode") as HiddenField).Value;
                    drGridRow["comments"] = (gvr.FindControl("hdnComments") as HiddenField).Value;
                    //953  Changes by Ravi -- Start
                    drGridRow["auto_signoff"] = (gvr.FindControl("hdnAutoSignoff") as HiddenField).Value;
                    //953  Changes by Ravi -- End
                }
                else
                {
                    drGridRow["track_title"] = DBNull.Value;
                    drGridRow["artist_name"] = DBNull.Value;
                    drGridRow["responsibility_desc"] = DBNull.Value;
                    drGridRow["play_length"] = DBNull.Value;

                    if ((gvr.FindControl("hdnISRCPartId") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["isrc_part_id"] = (gvr.FindControl("hdnISRCPartId") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["isrc_part_id"] = DBNull.Value;
                    }

                    drGridRow["isrc_deal_id"] = (gvr.FindControl("hdnISRCDealId") as HiddenField).Value;

                    if ((gvr.FindControl("hdnRoyaltorId") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["royaltor_id"] = (gvr.FindControl("hdnRoyaltorId") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["royaltor_id"] = DBNull.Value;
                    }


                    if ((gvr.FindControl("txtRoyaltor") as TextBox).Text != string.Empty)
                    {
                        drGridRow["royaltor"] = (gvr.FindControl("txtRoyaltor") as TextBox).Text;
                    }
                    else
                    {
                        drGridRow["royaltor"] = DBNull.Value;
                    }

                    /* Harish : 03-11-2017 - undo is losing its initial value after adding new row
                    
                    if ((gvr.FindControl("hdnRoyaltor") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["royaltor"] = (gvr.FindControl("hdnRoyaltor") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["royaltor"] = DBNull.Value;
                    }
                     * */



                    //WUIN-258 changes - The option period is lost if the first participate added when the user presses + to add a second participate to an ISRC without saving the first one

                    /*
                    if ((gvr.FindControl("hdnOptPeriodCode") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["option_period_code"] = (gvr.FindControl("hdnOptPeriodCode") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["option_period_code"] = DBNull.Value;
                    }
                    
                    if ((gvr.FindControl("hdnSellerGrpCode") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["seller_group_code"] = (gvr.FindControl("hdnSellerGrpCode") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["seller_group_code"] = DBNull.Value;
                    }

                    if ((gvr.FindControl("hdnEscCode") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["esc_code"] = (gvr.FindControl("hdnEscCode") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["esc_code"] = DBNull.Value;
                    }

                    if ((gvr.FindControl("hdnActive") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["active"] = (gvr.FindControl("hdnActive") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["active"] = DBNull.Value;
                    }
                    */


                    //WUIN-522 changes - Begin
                    //as selected option period value is not getting hold on server side, hidden field value are populated on dropdown selection change
                    /* Before change
                    if ((gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedIndex != 0)
                    {
                        drGridRow["option_period_code"] = (gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedValue;
                    }
                    else
                    {
                        drGridRow["option_period_code"] = DBNull.Value;
                    }
                     * */

                    if ((gvr.FindControl("hdnOptPeriodCodeChanged") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["option_period_code"] = (gvr.FindControl("hdnOptPeriodCodeChanged") as HiddenField).Value;
                    }
                    else if ((gvr.FindControl("hdnOptPeriodCode") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["option_period_code"] = (gvr.FindControl("hdnOptPeriodCode") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["option_period_code"] = DBNull.Value;
                    }

                    //WUIN-522 changes - End


                    /*if ((gvr.FindControl("ddlTerritory") as DropDownList).SelectedIndex != 0)
                    {
                        drGridRow["seller_group_code"] = (gvr.FindControl("ddlTerritory") as DropDownList).SelectedValue;
                    }
                    else
                    {
                        drGridRow["seller_group_code"] = DBNull.Value;
                    }*/

                    string territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                    if (territory == string.Empty)
                    {
                        //drGridRow["seller_group_code"] = DBNull.Value;//Harish 13-03-2018
                        drGridRow["seller_group_code"] = wildCardChar;
                        drGridRow["seller_group"] = DBNull.Value;
                    }
                    else
                    {
                        drGridRow["seller_group_code"] = territory.Substring(0, territory.IndexOf("-") - 1);
                        drGridRow["seller_group"] = territory;
                    }

                    //WUIN-522 changes - Begin
                    //as selected Esc code value is not getting hold on server side, hidden field value are populated on dropdown selection change
                    /* Before change
                    if ((gvr.FindControl("ddlEscCode") as DropDownList).SelectedIndex != 0)
                    {
                        drGridRow["esc_code"] = (gvr.FindControl("ddlEscCode") as DropDownList).SelectedValue;
                    }
                    else
                    {
                        drGridRow["esc_code"] = DBNull.Value;
                    }
                     * */
                    if ((gvr.FindControl("hdnEscCodeChanged") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["esc_code"] = (gvr.FindControl("hdnEscCodeChanged") as HiddenField).Value;
                    }
                    else if ((gvr.FindControl("hdnEscCode") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["esc_code"] = (gvr.FindControl("hdnEscCode") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["esc_code"] = DBNull.Value;
                    }

                    //WUIN-522 changes - End

                    if ((gvr.FindControl("cbActive") as CheckBox).Checked)
                    {
                        drGridRow["active"] = "Y";
                    }
                    else
                    {
                        drGridRow["active"] = "N";
                    }

                    if ((gvr.FindControl("cbIncInEsc") as CheckBox).Checked)
                    {
                        drGridRow["inc_in_escalation"] = "Y";
                    }
                    else
                    {
                        drGridRow["inc_in_escalation"] = "N";
                    }


                    if ((gvr.FindControl("hdnParticipationId") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["participation_id"] = (gvr.FindControl("hdnParticipationId") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["participation_id"] = DBNull.Value;
                    }

                    if ((gvr.FindControl("hdnManualOverride") as HiddenField).Value != string.Empty)
                    {
                        drGridRow["manual_override"] = (gvr.FindControl("hdnManualOverride") as HiddenField).Value;
                    }
                    else
                    {
                        drGridRow["manual_override"] = DBNull.Value;
                    }

                    drGridRow["exclude"] = (gvr.FindControl("hdnExclude") as HiddenField).Value;
                    drGridRow["status_code"] = DBNull.Value;
                    drGridRow["comments"] = DBNull.Value;

                    drGridRow["is_consolidated"] = (gvr.FindControl("hdnIsConsolidated") as HiddenField).Value; //WUIN-1095 

                }

                if ((gvr.FindControl("hdnIsTrackEditable") as HiddenField).Value != string.Empty)
                {
                    drGridRow["is_track_editable"] = (gvr.FindControl("hdnIsTrackEditable") as HiddenField).Value;
                }
                else
                {
                    drGridRow["is_track_editable"] = DBNull.Value;
                }

                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["TrackListingGridData"] = dtGridChangedData;

        }

        private List<Array> ModifiedRowsList()
        {

            List<string> trackList = new List<string>();
            List<string> trackParicipantList = new List<string>();

            string displayOrder;
            string trackListingIdRow;
            string isrcDealId;
            string isrcPartId;
            string royaltor;
            string royaltorId;
            string participation_id;
            string isModified;
            string hdnRoyaltor;
            string hdnOptPeriodCode;
            string hdnOptPeriodCodeChanged;
            string hdnSellerGrpCode;
            string hdnEscCode;
            string hdnEscCodeChanged;
            string hdnActive;
            string isActive;
            string hdnIncInEsc;
            string isIncInEsc;
            string optionPeriodCode;
            string escCode;
            string hdnStatusCode;
            string statusCode;
            string hdnExclude;
            string exclude;
            DropDownList ddlOptionPeriod;
            string territory;
            DropDownList ddlEscCode;
            CheckBox cbActive;
            CheckBox cbIncInEsc;

            string trackIsrcDealId = "";
            string trackStatusCode = "";
            string trackExclude = "";
            string trackStatusChanged = "N";
            string trackExcludeChanged = "N";
            string trackDetailsChanged = "N";
            string trackParticipantAdded = "N";
            string trackParticipantModified = "N";
            string trackParticipantDeleted = "N";
            int deletedCount = 0;
            int addedCount = 0;
            int modifiedCount = 0;
            int trackListIndex = -1;
            foreach (GridViewRow gvr in gvTrackListing.Rows)
            {

                displayOrder = (gvr.FindControl("hdnDisplayOrder") as HiddenField).Value;
                trackListingIdRow = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;
                hdnStatusCode = (gvr.FindControl("hdnStatusCode") as HiddenField).Value;
                statusCode = (gvr.FindControl("ddlStatus") as DropDownList).SelectedValue;
                exclude = (gvr.FindControl("cbExclude") as CheckBox).Checked == true ? "Y" : "N";
                hdnExclude = (gvr.FindControl("hdnExclude") as HiddenField).Value;
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                isrcDealId = (gvr.FindControl("hdnISRCDealId") as HiddenField).Value;
                isrcPartId = (gvr.FindControl("hdnISRCPartId") as HiddenField).Value;

                if (displayOrder == "1")
                {
                    //WUIN-1191 - resetting temp variables on every track 
                    trackIsrcDealId = isrcDealId;
                    trackStatusCode = statusCode;
                    trackExclude = exclude;
                    trackStatusChanged = "N";
                    trackExcludeChanged = "N";
                    trackDetailsChanged = "N";
                    trackParticipantAdded = "N";
                    trackParticipantModified = "N";
                    trackParticipantDeleted = "N";
                    deletedCount = 0;
                    addedCount = 0;
                    modifiedCount = 0;

                    //Check if track details changed
                    //Adding to list if changed
                    if ((hdnStatusCode != statusCode) || (hdnExclude != exclude))
                    {
                        trackListIndex = trackListIndex + 1;
                        trackIsrcDealId = isrcDealId;
                        trackStatusCode = statusCode;
                        trackStatusChanged = (hdnStatusCode != statusCode ? "Y" : "N");
                        trackExclude = exclude;
                        trackExcludeChanged = (hdnExclude != exclude ? "Y" : "N");
                        trackDetailsChanged = "Y";
                        trackList.Add(trackListingIdRow + "(;||;)" + isrcDealId + "(;||;)" + trackDetailsChanged + "(;||;)" + trackStatusCode + "(;||;)" + trackStatusChanged + "(;||;)" + trackExclude + "(;||;)" + trackExcludeChanged + "(;||;)" + trackParticipantModified + "(;||;)" + trackParticipantAdded);

                    }

                    // Check if participant deleted for corresponding track
                    List<string> deleteList = new List<string>();
                    if (ViewState["vsDeleteIds"] != null)
                    {
                        deleteList = (List<string>)ViewState["vsDeleteIds"];
                    }

                    for (int i = 0; i < deleteList.Count; i++)
                    {
                        string[] deletedParticipant = deleteList[i].Split(";".ToCharArray());
                        if (deletedParticipant[0].ToString() == isrcDealId)
                        {
                            deletedCount = deletedCount + 1;
                            isrcPartId = deletedParticipant[1].ToString();
                            trackParticipantDeleted = "Y";
                            isModified = "D";
                            //Adding to list if deleted
                            trackParicipantList.Add(isrcPartId + "(;||;)" + isrcDealId + "(;||;)" + 0 + "(;||;)" + Global.DBNullParamValue + "(;||;)" +
                                               Global.DBNullParamValue + "(;||;)" + Global.DBNullParamValue + "(;||;)" +
                                               Global.DBNullParamValue + "(;||;)" + Global.DBNullParamValue + "(;||;)" +
                                               Global.DBNullParamValue + "(;||;)" + isModified);

                            //WUIN-1167 - Any Changes made participants  - track status will set to UR

                            //If participant deleted - adding particpant modified flag to track list
                            //If already track changed then removing track row and reformatting to track list for deleted row
                            if (trackDetailsChanged == "Y")
                            {
                                trackList.RemoveAt(trackListIndex);
                                trackListIndex = (trackListIndex - 1);
                            }

                            if (deletedCount == 1) // Adding to track list only for first deleted row for a track
                            {
                                trackListIndex = trackListIndex + 1;
                                trackIsrcDealId = isrcDealId;
                                trackList.Add(trackListingIdRow + "(;||;)" + isrcDealId + "(;||;)" + trackDetailsChanged + "(;||;)" + trackStatusCode + "(;||;)" + trackStatusChanged + "(;||;)" + trackExclude + "(;||;)" + trackExcludeChanged + "(;||;)" + trackParticipantModified + "(;||;)" + trackParticipantAdded);

                            }
                        }

                    }
                }
                else
                {
                    hdnRoyaltor = (gvr.FindControl("hdnRoyaltor") as HiddenField).Value;
                    hdnOptPeriodCode = (gvr.FindControl("hdnOptPeriodCode") as HiddenField).Value;
                    hdnOptPeriodCodeChanged = (gvr.FindControl("hdnOptPeriodCodeChanged") as HiddenField).Value;
                    hdnSellerGrpCode = (gvr.FindControl("hdnSellerGrpCode") as HiddenField).Value;
                    hdnEscCode = (gvr.FindControl("hdnEscCode") as HiddenField).Value;
                    hdnEscCodeChanged = (gvr.FindControl("hdnEscCodeChanged") as HiddenField).Value;
                    hdnActive = (gvr.FindControl("hdnActive") as HiddenField).Value;
                    hdnIncInEsc = (gvr.FindControl("hdnIncInEsc") as HiddenField).Value;

                    royaltor = (gvr.FindControl("txtRoyaltor") as TextBox).Text;
                    participation_id = (gvr.FindControl("hdnParticipationId") as HiddenField).Value;
                    isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                    ddlOptionPeriod = (gvr.FindControl("ddlOptionPeriod") as DropDownList);
                    territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                    ddlEscCode = (gvr.FindControl("ddlEscCode") as DropDownList);
                    cbActive = (gvr.FindControl("cbActive") as CheckBox);
                    cbIncInEsc = (gvr.FindControl("cbIncInEsc") as CheckBox);

                    royaltorId = royaltor == "" ? string.Empty : royaltor.Substring(0, royaltor.IndexOf("-") - 1);

                    territory = (territory == string.Empty ? wildCardChar : territory.Substring(0, territory.IndexOf("-") - 1));


                    //as option period/esc code dropdown is not reflecting the value selected on client side, need to get the value from hidden fields
                    if (ddlOptionPeriod.SelectedIndex == 0)
                    {
                        if (hdnOptPeriodCodeChanged != string.Empty)
                            optionPeriodCode = hdnOptPeriodCodeChanged;
                        else if (hdnOptPeriodCode != string.Empty)
                            optionPeriodCode = hdnOptPeriodCode;
                        else
                            optionPeriodCode = Global.DBNullParamValue;
                    }
                    else
                    {
                        optionPeriodCode = ddlOptionPeriod.SelectedValue;
                    }

                    if (ddlEscCode.SelectedIndex == 0)
                    {
                        if (hdnEscCodeChanged != string.Empty)
                            escCode = hdnEscCodeChanged;
                        else if (hdnEscCode != string.Empty)
                            escCode = hdnEscCode;
                        else
                            escCode = Global.DBNullParamValue;
                    }
                    else
                    {
                        escCode = ddlEscCode.SelectedValue;
                    }

                    if (cbActive.Checked)
                        isActive = "Y";
                    else
                        isActive = "N";

                    if (cbIncInEsc.Checked)
                        isIncInEsc = "Y";
                    else
                        isIncInEsc = "N";

                    //adding to list if new participant added
                    if (isModified == Global.DBNullParamValue && royaltorId != "" && optionPeriodCode != "")
                    {
                        isrcPartId = "-1";
                        isModified = "A";
                        trackParticipantAdded = "Y";
                        trackParticipantModified = "Y";
                        trackParicipantList.Add(isrcPartId + "(;||;)" + isrcDealId + "(;||;)" + royaltorId + "(;||;)" + optionPeriodCode + "(;||;)" +
                                                territory + "(;||;)" + escCode + "(;||;)" +
                                                (cbActive.Checked ? "Y" : "N") + "(;||;)" + (cbIncInEsc.Checked ? "Y" : "N") + "(;||;)" +
                                                (participation_id == string.Empty ? Global.DBNullParamValue : participation_id) + "(;||;)" + isModified);

                    }
                    else if (isModified != Global.DBNullParamValue)
                    {
                        //adding to list if participant modified
                        if (royaltor != hdnRoyaltor || (ddlOptionPeriod.SelectedIndex == 0 ? string.Empty : ddlOptionPeriod.SelectedValue) != hdnOptPeriodCode ||
                      territory != hdnSellerGrpCode ||
                      (ddlEscCode.SelectedIndex == 0 ? string.Empty : ddlEscCode.SelectedValue) != hdnEscCode || isActive != hdnActive || isIncInEsc != hdnIncInEsc)
                        {
                            isModified = "U";
                            trackParticipantModified = "Y";
                            trackParicipantList.Add(isrcPartId + "(;||;)" + isrcDealId + "(;||;)" + royaltorId + "(;||;)" + optionPeriodCode + "(;||;)" +
                                                territory + "(;||;)" + escCode + "(;||;)" +
                                                (cbActive.Checked ? "Y" : "N") + "(;||;)" + (cbIncInEsc.Checked ? "Y" : "N") + "(;||;)" +
                                                (participation_id == string.Empty ? Global.DBNullParamValue : participation_id) + "(;||;)" + isModified);
                        }
                    }

                    //WUIN-1167 - Any Changes made to participants  - track status will set to UR

                    //If participant added/modified - adding particpant modified flag to track list
                    //If already track changed then removing track row and reformatting to track list for  row

                    if ((trackIsrcDealId == isrcDealId) && (isModified == "A" || isModified == "U"))
                    {
                        addedCount = (isModified == "A") ? addedCount + 1 : addedCount;
                        modifiedCount = (isModified == "U") ? modifiedCount + 1 : modifiedCount;

                        if (trackListIndex != -1 && (trackDetailsChanged == "Y" || trackParticipantDeleted == "Y" || (addedCount == 1 && modifiedCount > 0)))
                        {
                            trackList.RemoveAt(trackListIndex);
                            trackListIndex = (trackListIndex - 1);
                        }
                        if (addedCount == 1 || modifiedCount == 1)
                        {
                            trackListIndex = (trackListIndex + 1);
                            trackList.Add(trackListingIdRow + "(;||;)" + isrcDealId + "(;||;)" + trackDetailsChanged + "(;||;)" + trackStatusCode + "(;||;)" + trackStatusChanged + "(;||;)" + trackExclude + "(;||;)" + trackExcludeChanged + "(;||;)" + trackParticipantModified + "(;||;)" + trackParticipantAdded);

                        }
                    }
                }
            }

            //WUIN-1191 -  To update status for track of other pages when catno status changed
            if (hdnBulkStatusUpdate.Value == "Y")
            {
                //Get tracks data other than current page
                DataTable dtTrackData = GetTracksOtherthanCurrentPage(hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value));

                if (dtTrackData != null)
                {
                    foreach (DataRow drTrack in dtTrackData.Rows)
                    {
                        displayOrder = drTrack["display_order"].ToString();
                        trackListingIdRow = drTrack["track_listing_id"].ToString();
                        isrcDealId = drTrack["isrc_deal_id"].ToString();
                        isrcPartId = drTrack["isrc_part_id"].ToString();
                        hdnStatusCode = drTrack["status_code"].ToString();
                        exclude = drTrack["exclude"].ToString();

                        if (displayOrder == "1" && ddlCatStatus.SelectedValue != hdnStatusCode)
                        {
                            trackIsrcDealId = isrcDealId;
                            trackStatusCode = ddlCatStatus.SelectedValue;
                            trackStatusChanged = "Y";
                            trackExclude = exclude;
                            trackExcludeChanged = "N";
                            trackDetailsChanged = "Y";
                            trackList.Add(trackListingIdRow + "(;||;)" + isrcDealId + "(;||;)" + trackDetailsChanged + "(;||;)" + trackStatusCode + "(;||;)" + trackStatusChanged + "(;||;)" + trackExclude + "(;||;)" + trackExcludeChanged + "(;||;)" + trackParticipantModified + "(;||;)" + trackParticipantAdded);

                        }
                    }
                }
            }

            List<Array> ChangeList = new List<Array>();
            ChangeList.Add(trackList.ToArray());
            ChangeList.Add(trackParicipantList.ToArray());

            return ChangeList;
        }

        private DataTable GetTracksOtherthanCurrentPage(int pageIndex)
        {
            int startingRowNum = 0;
            int endingRowNum = 0;
            DataTable dtTracksOtherthanCurrentPage = null;
            List<PageStartEnd> pages = (List<PageStartEnd>)Session["TrackListingPageStartEnd"];
            if (pages != null)
            {
                var page = pages.Where(p => p.PageNum == pageIndex);
                foreach (var p in page)
                {
                    startingRowNum = p.StartRowNum;
                    endingRowNum = p.EndRowNum;
                }
                DataTable dtStartToCurrentPage = new DataTable();
                DataTable dtCurrentPagetoLast = new DataTable();

                DataTable dtTrkListingIntialData = (DataTable)Session["TrackListingInitialData"];
                int maxRowNum = Convert.ToInt32(dtTrkListingIntialData.Rows[dtTrkListingIntialData.Rows.Count - 1]["rownum"]);
                DataRow[] dr1 = dtTrkListingIntialData.Select("rownum>=" + 1 + "AND rownum<" + startingRowNum); // Get data from 1st rownum to current page starting rownum
                DataRow[] dr2 = dtTrkListingIntialData.Select("rownum>" + endingRowNum + "AND rownum<=" + maxRowNum);// Get data from current max rownum to last rownum

                if (dr1.Count() > 0)
                {
                    dtStartToCurrentPage = dr1.CopyToDataTable();
                }
                if (dr2.Count() > 0)
                {
                    dtCurrentPagetoLast = dr2.CopyToDataTable();
                }

                dtStartToCurrentPage.Merge(dtCurrentPagetoLast);
                dtTracksOtherthanCurrentPage = dtStartToCurrentPage;
            }
            return dtTracksOtherthanCurrentPage;
        }

        protected void fuzzySearchRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GridViewRow gvr = ((ImageButton)sender).NamingContainer as GridViewRow;
                string searchText = (gvr.FindControl("txtRoyaltor") as TextBox).Text;
                hdnGridRoyFuzzySearchRowId.Value = gvr.RowIndex.ToString();

                if (searchText == string.Empty)
                {
                    msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                    return;
                }

                FuzzySearchRoyaltor(searchText);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        /* WUIN-522 change
        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Territory")
                {
                    TextBox txtTerritory;
                    foreach (GridViewRow gvr in gvTrackListing.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnGridFuzzySearchRowId.Value)
                        {
                            txtTerritory = (gvr.FindControl("txtTerritory") as TextBox);
                            if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                            {
                                txtTerritory.Text = string.Empty;
                                txtTerritory.ToolTip = string.Empty;
                                return;
                            }

                            txtTerritory.Text = lbFuzzySearch.SelectedValue.ToString();
                            txtTerritory.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                            break;
                        }
                    }

                    hdnFuzzySearchField.Value = string.Empty;
                }
                else
                {
                    TextBox txtRoy;
                    foreach (GridViewRow gvr in gvTrackListing.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnGridRoyFuzzySearchRowId.Value)
                        {
                            txtRoy = (gvr.FindControl("txtRoyaltor") as TextBox);
                            if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                            {
                                txtRoy.Text = string.Empty;
                                return;
                            }

                            txtRoy.Text = lbFuzzySearch.SelectedValue.ToString();
                            PopulateOptPrdEscCode(gvr);
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        */

        private void PopulateOptPrdEscCode(GridViewRow gvr)
        {
            DataTable optionPrdList;
            DataTable escCodeList;
            TextBox txtRoyaltor = (gvr.FindControl("txtRoyaltor") as TextBox);
            DropDownList ddlOptionPeriod = (gvr.FindControl("ddlOptionPeriod") as DropDownList);
            DropDownList ddlEscCode = (gvr.FindControl("ddlEscCode") as DropDownList);
            ddlOptionPeriod.Items.Clear();
            ddlEscCode.Items.Clear();
            //hdnExpandedTrackId.Value = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;

            if (txtRoyaltor.Text.Trim() == string.Empty || txtRoyaltor.Text.IndexOf("-") < 0)
            {
                ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                ddlOptionPeriod.SelectedIndex = 0;

                ddlEscCode.Items.Insert(0, new ListItem("-"));
                ddlEscCode.SelectedIndex = 0;

                return;
            }

            string royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);

            //WUIN-258 changes - The option period is lost in the first participate added when the user presses + to add a second participate to an ISRC without saving the first one
            (gvr.FindControl("hdnRoyaltorId") as HiddenField).Value = royaltorId;

            if (royaltorId != string.Empty)
            {
                //get option period list for the royaltor
                optionPrdList = GetOptionPeriodList(royaltorId);
                if (optionPrdList != null)
                {
                    ddlOptionPeriod.DataSource = optionPrdList;
                    ddlOptionPeriod.DataTextField = "item_text";
                    ddlOptionPeriod.DataValueField = "item_value";
                    ddlOptionPeriod.DataBind();
                    ddlOptionPeriod.Items.Insert(0, new ListItem("-"));

                    //Populate the Option period if only one option period for the selected royaltor
                    if (optionPrdList.Rows.Count == 1)
                    {
                        ddlOptionPeriod.SelectedIndex = 1;
                    }

                }
                else
                {
                    ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                    ddlOptionPeriod.SelectedIndex = 0;
                }

                //get escalation code list for the royaltor
                escCodeList = GetEscCodeList(royaltorId);
                if (escCodeList != null)
                {
                    ddlEscCode.DataSource = escCodeList;
                    ddlEscCode.DataTextField = "esc_code";
                    ddlEscCode.DataValueField = "esc_code";
                    ddlEscCode.DataBind();
                    ddlEscCode.Items.Insert(0, new ListItem("-"));
                }
                else
                {
                    ddlEscCode.Items.Insert(0, new ListItem("-"));
                    ddlEscCode.SelectedIndex = 0;
                }
            }
            else
            {
                ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                ddlOptionPeriod.SelectedIndex = 0;

                ddlEscCode.Items.Insert(0, new ListItem("-"));
                ddlEscCode.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// WUIN-496 - Enable consolidate button only if all tracks are signed off (status_code >= 2)
        /// </summary>
        private void EnableDisableConsolidateButton()
        {
            btnConsolidate.Enabled = false;

            //validation - if catalogue status is 3, only super user can consolidate
            //JIRA-983 Changes by Ravi on 26/02/2019 -- Start
            if (ddlCatStatus.SelectedValue == "3" && ((hdnUserRole.Value.ToLower() != UserRole.SuperUser.ToString().ToLower()) && (hdnUserRole.Value.ToLower() != UserRole.Supervisor.ToString().ToLower())))
            {
                //JIRA-983 Changes by Ravi on 26/02/2019 -- End
                return;
            }

            dtGridData = (DataTable)Session["TrackListingInitialData"];
            if (dtGridData.Rows.Count == 0)
            {
                return;
            }

            //disable if atleast one track is not signed off(if status_code < 2). 
            //WUIN-601 - validate only for tracks which are editable
            DataRow[] dtTracksNotSignedOff = dtGridData.Select("display_order = 1 AND  status_code < 2 AND is_track_editable = 'Y'");

            if (dtTracksNotSignedOff.Count() > 0)
            {
                btnConsolidate.Enabled = false;
            }
            else
            {
                //disable if all tracks are not editable
                DataRow[] dtTracksEditable = dtGridData.Select("display_order = 1 AND is_track_editable = 'Y'");
                if (dtTracksEditable.Count() == 0)
                {
                    btnConsolidate.Enabled = false;
                    return;
                }

                btnConsolidate.Enabled = true;

                //If an existing PARTICIPATION has MANUAL_OVERRIDE = 'Y', 
                //      then display message 'One or more Participants has a manual override, do you want to Continue?' Continue or Cancel
                //set hidden field value to validate on client side
                //DataRow[] dtManualOverride = dtGridData.Select("display_order = 2 AND manual_override = 'Y'" );
                DataTable dtParticip = (DataTable)Session["TrackListingParticipation"];
                DataRow[] dtManualOverride = dtParticip.Select("manual_override = 'Y'");
                if (dtManualOverride.Count() > 0)
                {
                    hdnManualOverride.Value = dtManualOverride.Count().ToString();
                }
                else
                {
                    hdnManualOverride.Value = "0";
                }

            }

        }


        #endregion Methods

        #region Validations

        #endregion Validations

        #region Web Methods

        //Populate option period dropdown on change of royaltor
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static List<DropdownListItems> GetOptionPeriods(string royaltor)
        {
            List<DropdownListItems> lstOptionPeriods = new List<DropdownListItems>();
            DataSet trackListingData;
            DropdownListItems listItem;

            if (royaltor.Trim() == string.Empty || royaltor.IndexOf("-") < 0)
            {
                //listItem = new DropdownListItems();
                //listItem.Text = "No results found";
                //listItem.Value = "0";
                //lstOptionPeriods.Add(listItem);
                return lstOptionPeriods;
            }

            string royaltorId = royaltor.Substring(0, royaltor.IndexOf("-") - 1);

            if (HttpContext.Current.Session["TrackListingDropdownData"] != null)
            {
                trackListingData = (DataSet)HttpContext.Current.Session["TrackListingDropdownData"];
                if (trackListingData.Tables["dtOptionPrdList"] != null)
                {
                    DataRow[] results = trackListingData.Tables["dtOptionPrdList"].Select("royaltor_id='" + royaltorId + "'");
                    if (results.Count() > 0)
                    {
                        foreach (DataRow dr in results)
                        {
                            listItem = new DropdownListItems();
                            listItem.Text = Convert.ToString(dr["item_text"]);
                            listItem.Value = Convert.ToString(dr["item_value"]);
                            lstOptionPeriods.Add(listItem);


                        }
                    }
                    //else
                    //{
                    //    listItem = new DropdownListItems();
                    //    listItem.Text = "No results found";
                    //    listItem.Value = "0";
                    //    lstOptionPeriods.Add(listItem);
                    //}


                }
            }

            return lstOptionPeriods;
        }

        //Populate Esc code dropdown on change of royaltor
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static List<DropdownListItems> GetEscCodes(string royaltor)
        {
            List<DropdownListItems> lstEscCodes = new List<DropdownListItems>();
            DataSet trackListingData;
            DropdownListItems listItem;

            if (royaltor.Trim() == string.Empty || royaltor.IndexOf("-") < 0)
            {
                //listItem = new DropdownListItems();
                //listItem.Text = "No results found";
                //listItem.Value = "0";
                //lstEscCodes.Add(listItem);
                return lstEscCodes;
            }

            string royaltorId = royaltor.Substring(0, royaltor.IndexOf("-") - 1);

            if (HttpContext.Current.Session["TrackListingDropdownData"] != null)
            {
                trackListingData = (DataSet)HttpContext.Current.Session["TrackListingDropdownData"];
                if (trackListingData.Tables["dtEscCodeList"] != null)
                {
                    DataRow[] results = trackListingData.Tables["dtEscCodeList"].Select("royaltor_id='" + royaltorId + "'");
                    if (results.Count() > 0)
                    {
                        foreach (DataRow dr in results)
                        {
                            listItem = new DropdownListItems();
                            listItem.Text = Convert.ToString(dr["esc_code"]);
                            listItem.Value = Convert.ToString(dr["esc_code"]);
                            lstEscCodes.Add(listItem);
                        }
                    }
                    //else
                    //{
                    //    listItem = new DropdownListItems();
                    //    listItem.Text = "No results found";
                    //    listItem.Value = "0";
                    //    lstEscCodes.Add(listItem);
                    //}


                }
            }

            return lstEscCodes;
        }

        #endregion Web Methods

        #region Class
        /// <summary>
        /// To populate dropdown list in client side
        /// </summary>        
        public class DropdownListItems
        {
            private string _value;
            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }

            private string _text;
            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }
        }
        #endregion Class

        #region Fuzzy search
        protected void btnFuzzyTerritoryListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> lstTerritories = new List<string>();
                DataTable dtTerritories;
                if (Session["TrackListingSellerGrpList"] != null)
                {
                    dtTerritories = (DataTable)Session["TrackListingSellerGrpList"];
                    if (dtTerritories.Rows.Count > 0)
                    {
                        var results = dtTerritories.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(hdnFuzzySearchText.Value.ToUpper()));
                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                lstTerritories.Add(Convert.ToString(dr["seller_group"]));
                            }
                        }
                        else
                        {
                            lstTerritories.Add("No results found");
                        }


                    }
                    else
                    {
                        lstTerritories.Add("No results found");
                    }

                }
                lblFuzzySearchPopUp.Text = "Territory - Complete Search List";
                lbFuzzySearch.DataSource = lstTerritories;
                lbFuzzySearch.DataBind();
                mpeFuzzySearch.Show();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory fuzzy search popup", ex.Message);
            }

        }

        protected void btnFuzzyRoyaltorListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = hdnFuzzySearchText.Value;

                if (searchText == string.Empty)
                {
                    msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                    return;
                }

                FuzzySearchRoyaltor(searchText);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search popup", ex.Message);
            }
        }

        private void FuzzySearchRoyaltor(string searchText)
        {

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(searchText.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();

        }

        #endregion

        #region GRID PAGING

        /// <summary>
        /// object to hold page number, start and end row numbers
        /// </summary>
        struct PageStartEnd
        {
            int pageNum;
            int startRowNum;
            int endRowNum;

            public int PageNum
            {
                get { return pageNum; }
                set
                {
                    pageNum = value;
                }
            }

            public int StartRowNum
            {
                get { return startRowNum; }
                set
                {
                    startRowNum = value;
                }
            }

            public int EndRowNum
            {
                get { return endRowNum; }
                set
                {
                    endRowNum = value;
                }
            }

        };

        private void SetPageStartEndRowNum(int recordCount)
        {
            dtTrkListingPagingData = (DataTable)Session["TrackListingInitialData"];

            /* //Harish 26-06-18: this may not be required as maximum participants in a track might not be greater than the page size(usually 50 or 100)
            //set the page size based on maximum participants in a track
            DataTable dt0 = GroupBy("track_listing_id", "rownum", dtTrkListingPagingData);
            if (dt0.Rows.Count != 0)
            {
                DataTable dt01 = dt0.Select("count=MAX(count)").CopyToDataTable();
                if (dt01.Rows.Count != 0)
                {
                    if (Convert.ToInt32(dt01.Rows[0]["count"]) > gridDefaultPageSize)
                    {
                        hdnGridPageSize.Value = (Convert.ToInt32(dt01.Rows[0]["count"]) + 10).ToString();
                    }
                    else
                    {
                        hdnGridPageSize.Value = gridDefaultPageSize.ToString();
                    }
                }
                else
                {
                    hdnGridPageSize.Value = gridDefaultPageSize.ToString();
                }
            }
            */

            hdnGridPageSize.Value = gridDefaultPageSize.ToString();
            double dblPageCount = (double)((decimal)recordCount / decimal.Parse(hdnGridPageSize.Value.ToString()));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<PageStartEnd> pages = new List<PageStartEnd>();
            int currentPageStartRowNum = 1;
            int currentPageEndRowNum;
            string query;
            PageStartEnd page;


            if (pageCount > 0)
            {

                //set the page number, start and end row numbers for each page
                for (int i = 1; i <= pageCount; i++)
                {
                    currentPageEndRowNum = i * (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value));
                    query = "(rownum>=" + currentPageStartRowNum + " AND rownum<=" + currentPageEndRowNum + ")";

                    //check if there are rows for the select query and exit if not
                    DataRow[] dt01 = dtTrkListingPagingData.Select(query);
                    if (dt01.Count() == 0)
                    {
                        break;
                    }

                    DataTable dt1 = dtTrkListingPagingData.Select(query).CopyToDataTable().Select("rownum=MAX(rownum)").CopyToDataTable();
                    string maxRowNum = dt1.Rows[0]["rownum"].ToString();
                    string maxRowNumTrakListingId = dt1.Rows[0]["track_listing_id"].ToString();

                    //check if there are rows for the tracklisting id of max(rownum)
                    //if exist, fetch all rows of the tracklisting id of max(rownum) next to max(rownum)
                    DataRow[] dt2 = dtTrkListingPagingData.Select("rownum > " + maxRowNum);
                    if (dt2.Count() != 0)
                    {
                        DataTable dt3 = dtTrkListingPagingData.Select("rownum > " + maxRowNum).CopyToDataTable();
                        foreach (DataRow dr3 in dt3.Rows)
                        {
                            if (maxRowNumTrakListingId == dr3["track_listing_id"].ToString())
                            {
                                currentPageEndRowNum = currentPageEndRowNum + 1;
                            }
                            else
                            {
                                //currentPageEndRowNum = Convert.ToInt32(maxRowNum);
                                break;
                            }

                        }
                    }
                    else
                    {
                        currentPageEndRowNum = Convert.ToInt32(maxRowNum);
                    }

                    page = new PageStartEnd();
                    page.PageNum = i;
                    page.StartRowNum = currentPageStartRowNum;
                    page.EndRowNum = currentPageEndRowNum;
                    pages.Add(page);

                    currentPageStartRowNum = currentPageEndRowNum + 1;
                }

            }

            Session["TrackListingPageStartEnd"] = pages;

        }

        private DataTable GroupBy(string groupByColumn, string aggregateColumn, DataTable sourceTable)
        {

            DataView dv = new DataView(sourceTable);

            //getting distinct values for group column
            DataTable dtGroup = dv.ToTable(true, new string[] { groupByColumn });

            //adding column for the row count
            dtGroup.Columns.Add("Count", typeof(int));

            //looping thru distinct values for the group, counting
            foreach (DataRow dr in dtGroup.Rows)
            {
                dr["Count"] = sourceTable.Compute("Count(" + aggregateColumn + ")", groupByColumn + " = '" + dr[groupByColumn] + "'");
            }

            //returning grouped/counted result
            return dtGroup;
        }

        private void PopulateGridPage(int pageIndex)
        {
            int startingRowNum = 0;
            int endingRowNum = 0;

            List<PageStartEnd> pages = (List<PageStartEnd>)Session["TrackListingPageStartEnd"];
            var page = pages.Where(p => p.PageNum == pageIndex);
            foreach (var p in page)
            {
                startingRowNum = p.StartRowNum;
                endingRowNum = p.EndRowNum;
            }

            dtTrkListingPagingData = (DataTable)Session["TrackListingInitialData"];
            DataTable dt3 = dtTrkListingPagingData.Select("rownum>=" + startingRowNum + "AND rownum<=" + endingRowNum).CopyToDataTable();

            if (dt3.Rows.Count != 0)
            {
                gvTrackListing.DataSource = dt3;
            }
            else
            {
                gvTrackListing.EmptyDataText = "No data found for the selected catalogue number";
            }
            gvTrackListing.DataBind();

            //display paging only if page count is > 1
            if (pages.Count() > 1)
            {
                PopulatePager(pages.Count(), pageIndex, (hdnGridPageSize.Value == "" ? gridDefaultPageSize : Convert.ToInt32(hdnGridPageSize.Value)));
            }
        }

        private void PopulatePager(int pageCount, int currentPage, int pageSize)
        {
            rptPager.Visible = true;
            List<ListItem> pages = new List<ListItem>();

            if (pageCount > 0)
            {
                //To display only 10 pages on screen at a time
                //Divide total pages into different groups with 10 pages on each group
                //for example 
                //page group 0 ->page no 1 to 10 
                //page group 1 ->page no 11 to 20 ... 
                //calculate page group of currentpage
                int pageGroup = (int)Math.Floor((decimal)(currentPage - 1) / 10);

                //Add First page 
                pages.Add(new ListItem("First", "1", currentPage > 1));

                //if page group > 0 means current page is >10. Add ... to screen 
                //And its page index is last page of previous page group
                if (pageGroup > 0)
                {
                    pages.Add(new ListItem("...", (pageGroup * 10).ToString(), true));
                }

                //Add pages based on page group
                //If selected page is 5, its pagegroup is 0 so add 1 to 10 page no
                //If selected page is 18, its pagegroup is 1 so add 11 to 20 page no
                for (int i = 1; i <= 10; i++)
                {
                    int pageIndex = (pageGroup * 10) + i;
                    if (pageIndex <= pageCount)
                    {
                        pages.Add(new ListItem(pageIndex.ToString(), pageIndex.ToString(), pageIndex != currentPage));
                    }
                }

                //If total page count is more than 10 and we are not on last page group then add ...
                //And its index is first page of next page group
                if (pageCount > 10 && ((pageCount - (pageGroup * 10)) > 10))
                {
                    pages.Add(new ListItem("...", ((pageGroup * 10) + 11).ToString(), currentPage < pageCount));
                }

                //Finally ad Last page
                pages.Add(new ListItem("Last", pageCount.ToString(), currentPage < pageCount));
            }
            rptPager.DataSource = pages;
            rptPager.DataBind();
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                //pageCount;
                int pageIndex;
                if (hdnIsConfirmPopup.Value != "Y")
                {
                    pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                }
                else
                {
                    if (hdnPageIndex.Value == "Last")
                    {
                        List<PageStartEnd> pages = (List<PageStartEnd>)Session["TrackListingPageStartEnd"];
                        pageIndex = pages.Count();
                    }
                    else
                    {
                        pageIndex = hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value);
                    }

                }

                hdnPageIndex.Value = pageIndex.ToString();
                PopulateGridPage(pageIndex);
                UserAuthorization();

                hdnGridDataDeleted.Value = "N";//reset to default

                //Handling expand all on paging
                if (hdnExpandCollapseAll.Value == "Collapse")//Grid expand all is selected
                {
                    //populate hdnExpandedTrackId with all track ids
                    hdnExpandedTrackId.Value = string.Empty;
                    DataTable dtTrackIds = ((DataTable)Session["TrackListingInitialData"]).DefaultView.ToTable(true, "track_listing_id");
                    foreach (DataRow drTrack in dtTrackIds.Rows)
                    {
                        //add track listing id
                        if (hdnExpandedTrackId.Value == string.Empty)
                        {
                            hdnExpandedTrackId.Value = drTrack["track_listing_id"].ToString();
                        }
                        else
                        {
                            hdnExpandedTrackId.Value = hdnExpandedTrackId.Value + ";" + drTrack["track_listing_id"].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change", ex.Message);
            }
        }

        #endregion GRID PAGING




    }
}