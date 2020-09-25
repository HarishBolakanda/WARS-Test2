/*
File Name   :   CatalogueMaintenance.cs
Purpose     :   to search products with missing participants

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     06-Jun-2017     Pratik(Infosys Limited)   Initial Creation
 *      23-Jan-2018     Harish                    WUIN-420   
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Net;
using System.IO;
using System.Xml;
using WARS.BusinessLayer;
using System.Threading;
using System.Drawing;

namespace WARS
{
    public partial class CatalogueMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        CatalogueMaintenanceBL catalogueMaintenanceBL;
        Utilities util;
        Int32 errorId;
        Int32 trackListingCount;
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Catalogue Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Catalogue Maintenence";
                }

                lblTab.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {
                    txtCatalogueNumber.Focus();
                    Session["CatalogueDetails"] = null;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //used in user role validations
                        hdnUserRole.Value = Session["UserRole"].ToString();

                        if (Request.QueryString != null && Request.QueryString.Count > 0)
                        {
                            string catalogueNumber = Request.QueryString[0];
                            hdnIsNewCatNo.Value = "N";
                            LoadData(catalogueNumber);
                            hdnCatalogueNo.Value = catalogueNumber;

                            txtCatalogueNumber.Font.Bold = true;
                            txtCatalogueNumber.ReadOnly = true;
                            txtCatalogueNumber.Attributes.Add("onFocus", "MoveCatNoFocus()");
                            txtCatalogueNumber.CssClass = "textboxStyle_readonly";

                        }
                        else
                        {
                            hdnIsNewCatNo.Value = "Y";
                            LoadData(string.Empty);
                        }

                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                }

                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("valGrpSave");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Catalogue details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                SaveChanges(false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving catalogue", ex.Message);
            }
        }

        protected void btnYesConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChanges(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving catalogue", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Artist")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtArtist.Text = string.Empty;
                        return;
                    }

                    txtArtist.Text = lbFuzzySearch.SelectedValue.ToString();
                    hdnIsValidArtist.Value = "Y";

                }
                else if (hdnFuzzySearchField.Value == "Project")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtProject.Text = string.Empty;
                        return;
                    }

                    txtProject.Text = lbFuzzySearch.SelectedValue.ToString();
                    hdnIsValidProject.Value = "Y";
                }
                else if (hdnFuzzySearchField.Value == "ExpRateProject")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtExceptionRateProject.Text = string.Empty;
                        return;
                    }

                    txtExceptionRateProject.Text = lbFuzzySearch.SelectedValue.ToString();
                    hdnIsValidExcRateProject.Value = "Y";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection.", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Artist")
                {
                    txtArtist.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "Project")
                {
                    txtProject.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "ExpRateProject")
                {
                    txtExceptionRateProject.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing fuzzy search pop up.", ex.Message);
            }
        }

        protected void fuzzySearchArtist_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchArtist();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in artist search.", ex.Message);
            }
        }

        protected void fuzzySearchProject_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchProject();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in project search.", ex.Message);
            }
        }

        protected void fuzzySearchExRatesProject_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchExpRateProject();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in exception rate project search.", ex.Message);
            }
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAudit.Text == "Audit")
                {
                    Response.Redirect("../Audit/CatalogueDetailsAudit.aspx?CatNo=" + hdnCatalogueNo.Value + "", false);
                }
                else
                {
                    Response.Redirect("../Participants/CatalogueSearch.aspx", false);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        protected void btnTrackListing_Click(object sender, EventArgs e)
        {
            try
            {

                Response.Redirect("../Participants/TrackListing.aspx?CatNo=" + hdnCatalogueNo.Value + "", false);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        protected void btnCatalogueSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Participants/CatalogueSearch.aspx?isNewRequest=N", false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        protected void btnPartSummary_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Participants/ParticipantSummary.aspx?CatNo=" + hdnCatalogueNo.Value + "", false);
        }

        protected void btnPartMaint_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Participants/ParticipantMaintenance.aspx?CatNo=" + hdnCatalogueNo.Value + "", false);
        }

        protected void btnMissingPart_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Participants/MissingParticipants.aspx?isNewRequest=N", false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        protected void valFirstSaleDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtFirstSaleDate.Text.Trim() != "__/__/____" && txtFirstSaleDate.Text.Trim() != string.Empty)
            {
                DateTime firstSaleDate = DateTime.MinValue;
                try
                {
                    firstSaleDate = Convert.ToDateTime(txtFirstSaleDate.Text);
                }
                catch
                {
                    args.IsValid = false;
                    valFirstSaleDate.ToolTip = "Please enter a valid date in DD/MM/YYYY format";
                    return;
                }
            }
        }

        //JIRA-961 Changes by Ravi -- Start
        protected void btnCatNotes_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Participants/CatalogueNotes.aspx?CatNo=" + hdnCatalogueNo.Value + "", false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }
        //JIRA-961 Changes by Ravi -- End

        #endregion Events

        #region Methods

        private void LoadData(string catNo)
        {
            catalogueMaintenanceBL = new CatalogueMaintenanceBL();
            DataSet initialData = catalogueMaintenanceBL.GetInitialData(catNo, out trackListingCount, out errorId);
            catalogueMaintenanceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {

                ddlMurOwner.DataSource = initialData.Tables[1];
                ddlMurOwner.DataTextField = "mur_owner_desc";
                ddlMurOwner.DataValueField = "mur_owner_code";
                ddlMurOwner.DataBind();
                ddlMurOwner.Items.Insert(0, new ListItem("-"));

                ddlConfiguration.DataSource = initialData.Tables[2];
                ddlConfiguration.DataTextField = "config_desc";
                ddlConfiguration.DataValueField = "config_group_code";
                ddlConfiguration.DataBind();
                ddlConfiguration.Items.Insert(0, new ListItem("-"));

                ddlTimeTrackShare.DataSource = initialData.Tables[3];
                ddlTimeTrackShare.DataTextField = "dropdown_desc";
                ddlTimeTrackShare.DataValueField = "dropdown_value";
                ddlTimeTrackShare.DataBind();
                ddlTimeTrackShare.Items.Insert(0, new ListItem("-"));

                ddlCatStatus.DataSource = initialData.Tables[4];
                ddlCatStatus.DataTextField = "item_text";
                ddlCatStatus.DataValueField = "item_value";
                ddlCatStatus.DataBind();
                ddlCatStatus.Items.Insert(0, new ListItem("-"));

                BindData(initialData.Tables[0], trackListingCount);

                //for new catalogue, select default value to 'No Participants'
                if (catNo == string.Empty)
                {
                    hdnStatusCode.Value = "0";
                    ddlCatStatus.SelectedValue = "0";
                }
            }
            else
            {
                ExceptionHandler("Error in loading catalogue data", string.Empty);
            }
        }

        private void BindData(DataTable catalogueData, Int32 trackListingCount)
        {
            //assign default values to the hidden fields
            hdnMurOwner.Value = "-";
            hdnTimeTrackShare.Value = "-";
            hdnStatusCode.Value = "-";
            hdnConfiguration.Value = "-";
            hdnCompilation.Value = "N";
            hdnLegacy.Value = "N";
            hdnLicensedOut.Value = "N";
            hdnTotalPlayLength.Value = "___:__:__";
            hdnIsValidArtist.Value = "Y";
            hdnIsValidProject.Value = "Y";
            hdnIsValidExcRateProject.Value = "Y";

            if (catalogueData.Rows.Count > 0)
            {
                hdnCatalogueNo.Value = catalogueData.Rows[0]["catno"].ToString();
                txtCatalogueNumber.Text = catalogueData.Rows[0]["catno"].ToString();
                txtTitle.Text = catalogueData.Rows[0]["catno_title"].ToString();
                txtTotalTracks.Text = catalogueData.Rows[0]["total_tracks"].ToString();
                txtUnlistedComponents.Text = catalogueData.Rows[0]["unlisted_components"].ToString();
                txtTotalPlayLength.Text = catalogueData.Rows[0]["total_play_length"].ToString();
                hdnTrackListingCount.Value = trackListingCount.ToString();
                hdnStatusCode.Value = catalogueData.Rows[0]["status_code"].ToString();
                //WUIN - 1000
                txtMarketingOwner.Text = catalogueData.Rows[0]["marketing_owner_code"].ToString();
                txtFirstSaleDate.Text = catalogueData.Rows[0]["first_sale_date"].ToString();
                txtWeaSaelsLabel.Text = catalogueData.Rows[0]["wea_sales_label_code"].ToString();

                txtLabel.Text = catalogueData.Rows[0]["label"].ToString();//WUIN - 1047

                if (catalogueData.Rows[0]["is_compilation"].ToString() == "Y")
                {
                    cbCompilation.Checked = true;
                }

                if (catalogueData.Rows[0]["is_licensed_out"].ToString() == "Y")
                {
                    cbLicensedOut.Checked = true;
                }
                if (catalogueData.Rows[0]["legacy"].ToString() == "Y")
                {
                    cbLegacy.Checked = true;
                }

                string configurationCode = catalogueData.Rows[0]["config_code"].ToString().Trim();
                ddlConfiguration.ClearSelection();
                if (ddlConfiguration.Items.FindByValue(configurationCode) != null)
                {
                    ddlConfiguration.Items.FindByValue(configurationCode).Selected = true;
                    hdnConfiguration.Value = configurationCode;
                }
                else
                {
                    ddlConfiguration.SelectedIndex = 0;
                    hdnConfiguration.Value = "-";
                }

                ddlCatStatus.ClearSelection();
                if (ddlCatStatus.Items.FindByValue(catalogueData.Rows[0]["status_code"].ToString()) != null)
                {
                    ddlCatStatus.Items.FindByValue(catalogueData.Rows[0]["status_code"].ToString()).Selected = true;
                }
                else
                {
                    ddlCatStatus.SelectedIndex = 0;
                }

                string murOwnerCode = catalogueData.Rows[0]["mur_owner_code"].ToString().Trim();
                ddlMurOwner.ClearSelection();
                if (ddlMurOwner.Items.FindByValue(murOwnerCode) != null)
                {
                    ddlMurOwner.Items.FindByValue(murOwnerCode).Selected = true;
                    hdnMurOwner.Value = murOwnerCode;
                }
                else
                {
                    ddlMurOwner.SelectedIndex = 0;
                    hdnMurOwner.Value = "-";
                }

                string timeTrack = catalogueData.Rows[0]["track_time_flag"].ToString().Trim();
                ddlTimeTrackShare.ClearSelection();
                if (ddlTimeTrackShare.Items.FindByValue(timeTrack) != null)
                {
                    ddlTimeTrackShare.Items.FindByValue(timeTrack).Selected = true;
                    hdnTimeTrackShare.Value = timeTrack;
                }
                else
                {
                    ddlTimeTrackShare.SelectedIndex = 0;
                    hdnTimeTrackShare.Value = "-";
                }



                //populate artist for the artist id
                string artistId = catalogueData.Rows[0]["artist_id"].ToString().Trim();
                if (Session["FuzzySearchAllArtistList"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllArtistList", out errorId);
                    Session["FuzzySearchAllArtistList"] = dsList.Tables[0];
                }

                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchAllArtistList"]).Select("artist_id = '" + artistId + "'");
                if (filteredRow.Length > 0)
                {
                    txtArtist.Text = filteredRow[0]["artist"].ToString();
                }
                else
                {
                    txtArtist.Text = string.Empty;
                }

                //populate project for the project id
                string projectId = catalogueData.Rows[0]["project_id"].ToString().Trim();
                if (Session["FuzzySearchAllProjectList"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllProjectList", out errorId);
                    Session["FuzzySearchAllProjectList"] = dsList.Tables[0];
                }

                filteredRow = ((DataTable)Session["FuzzySearchAllProjectList"]).Select("Convert(project_id,System.String) = '" + projectId + "'");
                if (filteredRow.Length > 0)
                {
                    txtProject.Text = filteredRow[0]["project"].ToString();
                }
                else
                {
                    txtProject.Text = string.Empty;
                }

                string excRateProjectId = catalogueData.Rows[0]["exception_rate_project_id"].ToString().Trim();
                filteredRow = ((DataTable)Session["FuzzySearchAllProjectList"]).Select("Convert(project_id,System.String) = '" + excRateProjectId + "'");
                if (filteredRow.Length > 0)
                {
                    txtExceptionRateProject.Text = filteredRow[0]["project"].ToString();
                }
                else
                {
                    txtExceptionRateProject.Text = string.Empty;
                }

                //assign hidden field values
                hdnCatalogueNumber.Value = catalogueData.Rows[0]["catno"].ToString();
                hdnTitle.Value = catalogueData.Rows[0]["catno_title"].ToString();
                hdnArtist.Value = txtArtist.Text;
                hdnProject.Value = txtProject.Text;
                hdnExceptionRateProject.Value = txtExceptionRateProject.Text;
                hdnTotalTracks.Value = catalogueData.Rows[0]["total_tracks"].ToString();
                hdnUnlistedComponents.Value = catalogueData.Rows[0]["unlisted_components"].ToString();
                hdnCompilation.Value = catalogueData.Rows[0]["is_compilation"].ToString();
                hdnLicensedOut.Value = catalogueData.Rows[0]["is_licensed_out"].ToString();
                hdnLegacy.Value = catalogueData.Rows[0]["legacy"].ToString();
                hdnIsEditable.Value = catalogueData.Rows[0]["is_editable"].ToString();
                //WUIN - 1000
                hdnMarketingOwner.Value = catalogueData.Rows[0]["marketing_owner_code"].ToString();
                hdnFirstSaleDate.Value = catalogueData.Rows[0]["first_sale_date"].ToString();
                hdnWeaSalesLabel.Value = catalogueData.Rows[0]["wea_sales_label_code"].ToString();

                hdnLabel.Value = catalogueData.Rows[0]["label"].ToString();//WUIN - 1047
                if (!string.IsNullOrEmpty(catalogueData.Rows[0]["total_play_length"].ToString()))
                {
                    //WUIN-420 changes
                    string totalTime = catalogueData.Rows[0]["total_play_length"].ToString();

                    if (!totalTime.Contains(':'))
                    {
                        totalTime = String.Format("{0:D7}", Convert.ToInt32(totalTime));//append leading zeros if the length is < 7
                        string seconds = totalTime.Substring((totalTime.Length - 2), 2);
                        string mins = totalTime.Substring((totalTime.Length - 4), 2);
                        string hrs = totalTime.Substring((totalTime.Length - 7), 3);
                        hdnTotalPlayLength.Value = hrs + ":" + mins + ":" + seconds;
                    }
                    else
                    {
                        hdnTotalPlayLength.Value = totalTime;
                    }
                }

                //WUIN-1167 Commenting below method
                //Any changes done on this screen should move the status to under review irrespective of the User role.

                //EnableDisableSaveOption();

                //WUIN-274 - handle navigation buttons
                HandleNavigationButtons(trackListingCount.ToString(), catalogueData.Rows[0]["status_code"].ToString(), catalogueData.Rows[0]["all_track_status"].ToString());

            }
            else
            {
                //Harish: 05-11-2018 - for a new catno, no need to display navigation buttons
                HandleNavigationButtons(string.Empty, string.Empty, string.Empty);
            }
        }

        /// <summary>
        /// WUIN-1167 Commenting below method
        /// Any changes done on this screen should move the status to under review irrespective of the User role.
        /// Disables Save options of the screen on validations        
        /// When catalogue status is at Manager sign off, only super user can edit catalogue details
        /// </summary>

        //private void EnableDisableSaveOption()
        //{
        //    btnSave.Enabled = true;
        //    btnSave.ToolTip = string.Empty;

        //    //JIRA-983 Changes by Ravi on 26/02/2019 -- Start
        //    if ((hdnUserRole.Value.ToLower() != UserRole.SuperUser.ToString().ToLower()) && (hdnUserRole.Value.ToLower() != UserRole.Supervisor.ToString().ToLower()) && hdnStatusCode.Value == "3")
        //    {
        //        btnSave.Enabled = false;
        //        btnSave.ToolTip = "This can be edited only by super user and Supervisor";
        //    }
        //    //JIRA-983 Changes by Ravi on 26/02/2019 -- End
        //}

        private void HandleNavigationButtons(string trackListingCount, string catnoStatusCode, string trackStatusNotZeroCount)
        {
            //WUIN-274 -- 
            //btnTrackListing --> if Track Listing exists for CATNO
            //btnPartSummary --> if Track Listing exists for CATNO except	if CATNO.STATUS_CODE > 0 and ISRC_DEAL.STATUS_CODE = 0 for all Tracks
            //btnPartMaint --> if no Track Listing or (CATNO.STATUS_CODE > 0 and ISRC_DEAL.STATUS_CODE = 0 for all Tracks)

            btnTrackListing.Visible = false;
            btnPartSummary.Visible = false;
            btnPartMaint.Visible = false;

            if (hdnIsNewCatNo.Value == "N")
            {
                btnAudit.Text = "Audit";
                btnTrackListing.Enabled = true;

                if ((trackListingCount != string.Empty && Convert.ToInt32(trackListingCount) > 0) && hdnLegacy.Value == "N")
                {
                    btnTrackListing.Visible = true;
                    btnPartSummary.Visible = true;
                }

                if ((hdnLegacy.Value == "N" && (trackListingCount != string.Empty && Convert.ToInt32(trackListingCount) <= 0)) || (hdnLegacy.Value == "Y"))
                {
                    btnPartMaint.Visible = true;
                }

            }
            else
            {
                btnAudit.Text = "Cancel";
            }

        }

        private void SaveChanges(bool overrideParticipUpdate)
        {

            if (hdnIsStatusChange.Value == "Y")
            {
                bool isParticipUpdate;

                if (!ValidateCatalogueDetails(overrideParticipUpdate, out isParticipUpdate))
                {
                    //If the Participants will be updated, a message will be displayed ‘This will update the Status of all Participants and Tracks to this Catalogue Status. Confirm or Cancel’
                    if (isParticipUpdate)
                    {
                        lblConfirmMsg.Text = "This will update the Status of all Participants and Tracks to this Catalogue Status";
                        mpeConfirmation.Show();
                    }

                    return;

                }
            }
            else
            {
                //WUIN-1167 Any changes made to the catalogue which is at manager sign off/team sign off should move back to under review irrespective of the user role
                if (hdnStatusCode.Value == "3" || hdnStatusCode.Value == "2")
                {
                    ddlCatStatus.SelectedValue = "1";
                }
            }

            string userCode = Convert.ToString(Session["UserCode"]);
            Array catalogueDetails = CatalogueDetails();

            if (hdnIsNewCatNo.Value == "Y")
            {
                catalogueMaintenanceBL = new CatalogueMaintenanceBL();
                DataSet updatedData = catalogueMaintenanceBL.SaveCatalogueDetails(string.Empty, catalogueDetails, userCode, out trackListingCount, out errorId);
                catalogueMaintenanceBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("Catalogue deatils exists with entered cataloge number!", MessageType.Warning, PositionType.Auto);
                }
                else if (updatedData.Tables.Count != 0 && errorId != 2)
                {
                    hdnIsNewCatNo.Value = "N";
                    txtCatalogueNumber.Font.Bold = true;
                    txtCatalogueNumber.ReadOnly = true;
                    txtCatalogueNumber.Attributes.Add("onFocus", "MoveCatNoFocus()");
                    txtCatalogueNumber.CssClass = "textboxStyle_readonly";

                    if (updatedData.Tables[0].Rows.Count != 0)
                    {
                        BindData(updatedData.Tables[0], trackListingCount);
                    }
                    else
                    {
                        ExceptionHandler("Error in saving catalogue", string.Empty);
                        return;
                    }

                    msgView.SetMessage("New catalogue created successfully.", MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    ExceptionHandler("Error in saving catalogue", string.Empty);
                }

            }
            else if (hdnIsNewCatNo.Value == "N")
            {
                catalogueMaintenanceBL = new CatalogueMaintenanceBL();
                DataSet updatedData = catalogueMaintenanceBL.SaveCatalogueDetails(txtCatalogueNumber.Text.Trim().ToUpper(), catalogueDetails, userCode, out trackListingCount, out errorId);
                catalogueMaintenanceBL = null;

                if (updatedData.Tables.Count != 0 && errorId != 2)
                {
                    if (updatedData.Tables[0].Rows.Count != 0)
                    {
                        BindData(updatedData.Tables[0], trackListingCount);
                        hdnIsStatusChange.Value = "N";
                    }
                    else
                    {
                        ExceptionHandler("Error in saving catalogue", string.Empty);
                        return;
                    }

                    msgView.SetMessage("Catalogue details updated successfully.", MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    ExceptionHandler("Error in saving catalogue", string.Empty);
                }
            }
        }

        /// <summary>         
        ///1. When updating the CATNO.STATUS_CODE,  
        ///     1.No update allowed if (legacy = 'Y' and no active Participants (PARTICIPATION_TYPE = 'A' and end_date null))  
        ///                         OR (legacy = 'N' and no active Track Participants(ISRC participants))
        ///     2.The check for updating only one level at a time will be removed (eg will be able to update from Under Review to Manager Sign Off)									
        //      3.Participant Status will be updated to the Product Status if the existing Participant Status is < new Product Status									
        ///     4.Participant Status will not be updated to the Product Status if the existing Participant Status is > new Product Status									
        ///     5.Only update Participant Status if no end date 									
        //      6.If not legacy, Track Status will be updated to the Product Status if the existing Track Status is < new Product Status									
        //      7.If not legacy, Track Status will not be updated to the Product Status if the existing Track Status is > new Product Status	
        ///2.(This is handled on client side)If Status is moved from Manager Sign Off (3) then 						
        ///     display warning 'This update will prevent the generation of Statement details for all Participants'
        ///3.If the Participants will be updated, a confirmation message will be displayed 
        ///     ‘This will update the Status of all Participants and Tracks to this Catalogue Status. Confirm or Cancel’
        ///4. (WUIN-909) Cannot change status to 'No Participants' if there are active participants
        ///      if legacy = 'Y' and active Participants (PARTICIPATION_TYPE = 'A' and end_date null)
        ///      if legacy = 'N' and active Track Participants(ISRC participants)
        /// </summary>
        private bool ValidateCatalogueDetails(bool overrideParticipUpdate, out bool isParticipUpdate)
        {
            bool isValid = true;
            isParticipUpdate = false;

            Page.Validate("valGrpSave");
            if (!Page.IsValid)
            {
                msgView.SetMessage("Catalogue details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                return false;
            }

            if (hdnIsValidArtist.Value != "Y")
            {
                msgView.SetMessage("Please select valid artist from list.", MessageType.Success, PositionType.Auto);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtProject.Text))
            {
                if (hdnIsValidProject.Value != "Y")
                {
                    msgView.SetMessage("Please select valid project from list.", MessageType.Success, PositionType.Auto);
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtExceptionRateProject.Text))
            {
                if (hdnIsValidExcRateProject.Value != "Y")
                {
                    msgView.SetMessage("Please select valid exception rate project from list.", MessageType.Success, PositionType.Auto);
                    return false;
                }

            }

            if (hdnIsNewCatNo.Value == "N" && hdnIsEditable.Value == "Y" && ddlCatStatus.SelectedValue == "0")
            {
                msgView.SetMessage("As there are active participants, status cannot be set to 'No Participants'", MessageType.Success, PositionType.Auto);
                return false;
            }

            //4.If the Participants(either participation participants or if not legacy and isrc participants) will be updated, 
            //a message will be displayed ‘This will update the Status of all Participants and Tracks to this Catalogue Status. Confirm or Cancel’  
            if (!overrideParticipUpdate)
            {
                catalogueMaintenanceBL = new CatalogueMaintenanceBL();
                DataSet dsParticips = catalogueMaintenanceBL.GetCatnoParticipants(hdnCatalogueNo.Value, out errorId);
                catalogueMaintenanceBL = null;

                if (errorId == 2 || dsParticips.Tables.Count == 0)
                {
                    ExceptionHandler("Error in saving catalogue at fetching participant list.", string.Empty);
                    return false;
                }

                DataTable dtParticips = dsParticips.Tables[0];
                DataTable dtTrackParticips = dsParticips.Tables[1];

                if ((dtParticips.Select("status_code < " + ddlCatStatus.SelectedValue + " AND end_date IS NULL").Count() > 0) ||
                    (!cbLegacy.Checked && (dtTrackParticips.Select("status_code < " + ddlCatStatus.SelectedValue).Count() > 0)))
                {
                    isParticipUpdate = true;

                    if (!overrideParticipUpdate)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }


            return isValid;
        }

        private Array CatalogueDetails()
        {
            List<string> catalogueDetails = new List<string>();

            catalogueDetails.Add("catno" + Global.DBDelimiter + txtCatalogueNumber.Text.Trim());
            catalogueDetails.Add("catno_title" + Global.DBDelimiter + txtTitle.Text.Trim());

            if (ddlConfiguration.SelectedIndex > 0)
            {
                catalogueDetails.Add("config_code" + Global.DBDelimiter + ddlConfiguration.SelectedValue);
            }
            else
            {
                catalogueDetails.Add("config_code" + Global.DBDelimiter + string.Empty);
            }

            catalogueDetails.Add("label" + Global.DBDelimiter + txtLabel.Text.Trim());

            catalogueDetails.Add("project_id" + Global.DBDelimiter + txtProject.Text.Split('-')[0].ToString().Trim());

            if (ddlMurOwner.SelectedIndex > 0)
            {
                catalogueDetails.Add("mur_owner_code" + Global.DBDelimiter + ddlMurOwner.SelectedValue);
            }
            else
            {
                catalogueDetails.Add("mur_owner_code" + Global.DBDelimiter + string.Empty);
            }


            catalogueDetails.Add("artist_id" + Global.DBDelimiter + txtArtist.Text.Split('-')[0].ToString().Trim());

            if (ddlTimeTrackShare.SelectedIndex > 0)
            {
                catalogueDetails.Add("track_time_flag" + Global.DBDelimiter + ddlTimeTrackShare.SelectedValue);
            }
            else
            {
                catalogueDetails.Add("track_time_flag" + Global.DBDelimiter + string.Empty);
            }
            catalogueDetails.Add("marketing_owner" + Global.DBDelimiter + txtMarketingOwner.Text.Trim());
            catalogueDetails.Add("first_sale_date" + Global.DBDelimiter + txtFirstSaleDate.Text.Trim());
            catalogueDetails.Add("wea_sales_label" + Global.DBDelimiter + txtWeaSaelsLabel.Text.Trim());

            catalogueDetails.Add("total_tracks" + Global.DBDelimiter + txtTotalTracks.Text.Trim());
            catalogueDetails.Add("total_play_length" + Global.DBDelimiter + (txtTotalPlayLength.Text.Trim() == "___:__:__" ? string.Empty : txtTotalPlayLength.Text.Trim()));
            catalogueDetails.Add("unlisted_components" + Global.DBDelimiter + txtUnlistedComponents.Text.Trim());

            if (cbCompilation.Checked)
            {
                catalogueDetails.Add("is_compilation" + Global.DBDelimiter + "Y");
            }
            else
            {
                catalogueDetails.Add("is_compilation" + Global.DBDelimiter + "N");
            }

            if (cbLicensedOut.Checked)
            {
                catalogueDetails.Add("is_licensed_out" + Global.DBDelimiter + "Y");
            }
            else
            {
                catalogueDetails.Add("is_licensed_out" + Global.DBDelimiter + "N");
            }


            catalogueDetails.Add("exception_rate_project_id" + Global.DBDelimiter + (txtExceptionRateProject.Text == string.Empty ? "-1" : txtExceptionRateProject.Text.Split('-')[0].ToString()));

            if (ddlCatStatus.SelectedIndex > 0)
            {
                catalogueDetails.Add("status_code" + Global.DBDelimiter + ddlCatStatus.SelectedValue);
            }
            else
            {
                catalogueDetails.Add("status_code" + Global.DBDelimiter + string.Empty);
            }


            if (cbLegacy.Checked)
            {
                catalogueDetails.Add("legacy" + Global.DBDelimiter + "Y");
            }
            else
            {
                catalogueDetails.Add("legacy" + Global.DBDelimiter + "N");
            }

            return catalogueDetails.ToArray();

        }

        private void FuzzySearchArtist()
        {
            if (txtArtist.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in artist search field!", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Artist";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllArtisList(txtArtist.Text.ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();


        }

        private void FuzzySearchProject()
        {
            if (txtProject.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in project search field!", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Project";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllProjectList(txtProject.Text.ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();


        }

        private void FuzzySearchExpRateProject()
        {
            if (txtExceptionRateProject.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in exception rate project search field!", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "ExpRateProject";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllProjectList(txtExceptionRateProject.Text.ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();



        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSave.Enabled = false;
                btnYesConfirm.Enabled = false;
                btnNoConfirm.Enabled = false;
            }

        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        #endregion Methods

    }
}