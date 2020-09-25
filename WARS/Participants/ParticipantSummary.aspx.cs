/*
File Name   :   ParticipantSummary.cs
Purpose     :   to maintain Participants status and active/inactive for a Product.

Version  Date            Modified By           Modification Log
______________________________________________________________________
1.0     27-Jul-2017      Harish                Initial Creation (WUIN-111)
        17-Nov-2017      Harish                WUIN-111 - changes - If update CATNO status - display a message asking if user wants all Participant status to be updated too
 *      25-Jan-2018      Harish                WUIN-443
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

namespace WARS
{
    public partial class ParticipantSummary : System.Web.UI.Page
    {
        #region Global Declarations
        ParticipantSummaryBL participantSummaryBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtPartStatusList;
        DataSet dsParticipantDropdownData;
        string catalogueNo = string.Empty;
        string wildCardChar = ".";
        DataTable dtGridData;
        #endregion Global Declarations


        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                catalogueNo = Request.QueryString["CatNo"];
                // catalogueNo = "A10302B00000178745";
                //catalogueNo = "A10302B00004635030";
                //catalogueNo = "A10302B0002959242Z";


                if (catalogueNo == null || catalogueNo == string.Empty)
                {
                    msgView.SetMessage("Not a valid catalogue number!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Participant Summary";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Participant Summary";
                }

                lblTab.Focus();//tabbing sequence starts here                                
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadParticipSummaryData();

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

        protected void gvPartSummary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Session["ParticipantDropdownData"] == null)
                {
                    return;
                }

                DropDownList ddlStatus;
                CheckBox cbActive;
                CheckBox cbOverride;
                CheckBox cbEscIncludeUnits;
                string statusCode;
                string isActive;
                string isOverride;
                string escIncludeUnits;
                dsParticipantDropdownData = (DataSet)Session["ParticipantDropdownData"];
                Label lblTime;
                Label lblTotalTime;
                Label lblEscCode;

                if (dtPartStatusList == null)
                {
                    dtPartStatusList = (DataTable)Session["PartcipSummPartStatusList"];
                }

                //if track_time_flag = 'T' then disable time and total time else disable share and total share 
                if (lblCatTrackTimeShare.Text == "Track")
                {
                    txtTimeInsert.Enabled = false;
                    txtTotalTimeInsert.Enabled = false;
                }
                else
                {
                    txtShareInsert.Enabled = false;
                    txtTotalShareInsert.Enabled = false;
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlStatus = (e.Row.FindControl("ddlStatus") as DropDownList);
                    cbActive = (e.Row.FindControl("cbActive") as CheckBox);
                    cbOverride = (e.Row.FindControl("cbOverride") as CheckBox);
                    isActive = (e.Row.FindControl("hdnActive") as HiddenField).Value;
                    isOverride = (e.Row.FindControl("hdnParticipantOverride") as HiddenField).Value;
                    statusCode = (e.Row.FindControl("hdnStatusCode") as HiddenField).Value;
                    cbEscIncludeUnits = (e.Row.FindControl("cbEscIncludeUnits") as CheckBox);
                    lblTime = (e.Row.FindControl("lblTime") as Label);
                    lblTotalTime = (e.Row.FindControl("lblTotalTime") as Label);
                    lblEscCode = (e.Row.FindControl("lblEscCode") as Label);
                    isActive = (e.Row.FindControl("hdnActive") as HiddenField).Value;
                    escIncludeUnits = (e.Row.FindControl("hdnEscIncludeUnits") as HiddenField).Value;
                    statusCode = (e.Row.FindControl("hdnStatusCode") as HiddenField).Value;

                    //format the time and total time fields as hhh:mm:ss
                    if (lblTime.Text != string.Empty && lblTime.Text != "_______")
                    {
                        lblTime.Text = TimeFormat(lblTime.Text);
                    }
                    else if (lblTime.Text == "_______")
                    {
                        lblTime.Text = "000:00:00";
                    }

                    if (lblTotalTime.Text != string.Empty && lblTotalTime.Text != "_______")
                    {
                        lblTotalTime.Text = TimeFormat(lblTotalTime.Text);
                    }
                    else if (lblTotalTime.Text == "_______")
                    {
                        lblTotalTime.Text = "000:00:00";
                    }
                    if (dtPartStatusList != null)
                    {
                        ddlStatus.DataSource = dtPartStatusList;
                        ddlStatus.DataTextField = "item_text";
                        ddlStatus.DataValueField = "item_value";
                        ddlStatus.DataBind();

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

                    if (isActive == "Y")
                    {
                        cbActive.Checked = true;
                    }
                    else
                    {
                        cbActive.Checked = false;
                    }

                    if (isOverride == "Y")
                    {
                        cbOverride.Checked = true;
                    }
                    else
                    {
                        cbOverride.Checked = false;
                    }

                    if (escIncludeUnits == "Y")
                    {
                        cbEscIncludeUnits.Checked = true;
                    }
                    else
                    {
                        cbEscIncludeUnits.Checked = false;
                    }

                    if (string.IsNullOrWhiteSpace(lblEscCode.Text))
                    {
                        cbEscIncludeUnits.Visible = false;
                    }
                    else
                    {
                        cbEscIncludeUnits.Visible = true;
                    }

                }

                //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    foreach (TableCell tc in e.Row.Cells)
                    {
                        if (tc.HasControls())
                        {
                            LinkButton lnkHeader = (LinkButton)tc.Controls[0];
                            lnkHeader.Style.Add("color", "black");
                            lnkHeader.Style.Add("text-decoration", "none");

                            if (lnkHeader != null && hdnSortExpression.Value == lnkHeader.CommandArgument)
                            {
                                // initialize a new image
                                Image imgSort = new Image();
                                imgSort.ImageUrl = (hdnSortDirection.Value == ascending) ? sort_Up : sort_Down;
                                // adding a space and the image to the header link
                                tc.Controls.Add(new LiteralControl(" "));
                                tc.Controls.Add(imgSort);
                            }

                        }
                    }
                }
                //JIRA-746 Changes by Ravi on 05/03/2019 -- End
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvPartSummary_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortingDirection = string.Empty;
            Utilities util = new Utilities();
            string sortDirec = util.SortingDirection(hdnSortDirection.Value);
            if (sortDirec == ascending)
            {
                sortingDirection = descending;
            }
            else
            {
                sortingDirection = ascending;
            }
            DataTable dataTable = (DataTable)Session["ParticipationGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvPartSummary.DataSource = dataView;
                gvPartSummary.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void btnDisplayActive_Click(object sender, EventArgs e)
        {
            try
            {
                //toggle the display button text
                if (btnDisplayActive.Text == "Display Active")
                {
                    btnDisplayActive.Text = "Display All";
                }
                else
                {
                    btnDisplayActive.Text = "Display Active";
                }

                LoadParticipSummaryData();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in displaying Active participants", ex.Message);
            }
        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "DisplayActive")
            {
                btnDisplayActive_Click(sender, e);
            }
            hdnIsConfirmPopup.Value = "N";
        }

        //JIRA-1049 Changes by Ravi on 24/06/2019 -- Start
        protected void btnCorrectMismatches_Click(object sender, EventArgs e)
        {
            try
            {
                //WUIN-1049 - change
                //Able to pull down the participants which are not available in Participant Summary when compared with that of Track Listing screen and Inactivate all the participants
                string errorMessage = string.Empty;
                participantSummaryBL = new ParticipantSummaryBL();
                participantSummaryBL.CorrectMismatches(lblCatNo.Text, Convert.ToString(Session["UserCode"]), out errorId, out errorMessage);
                participantSummaryBL = null;

                if (errorId == 2)
                {
                    ExceptionHandler("Error in Correcting Mismatches of Participants", string.Empty);
                }
                else
                {
                    msgView.SetMessage(errorMessage, MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Correcting Mismatches of Participants", ex.Message);
            }
        }

        //JIRA-1049 Changes by Ravi on 24/06/2019 -- End
        #endregion Events

        #region Methods

        /// <summary>
        /// To check if all or only active participant data to be fetched
        /// </summary>        
        private string LoadOnlyActiveParticipats()
        {
            if (btnDisplayActive.Text == "Display Active")
            {
                //fetch all
                return "N";
            }
            else
            {
                //fetch only active
                return "Y";
            }
        }

        private void LoadParticipSummaryData()
        {
            participantSummaryBL = new ParticipantSummaryBL();
            DataSet ParticipSummaryData = participantSummaryBL.GetParticipantSummary(catalogueNo, LoadOnlyActiveParticipats(), Convert.ToString(Session["UserRoleId"]), out errorId); //JIRA-898 Change
            participantSummaryBL = null;

            Session["PartcipSummCatStatusList"] = ParticipSummaryData.Tables["dtCatNoStatusList"];
            Session["PartcipSummPartStatusList"] = ParticipSummaryData.Tables["dtISRCStatusList"];

            //name the datatables so that they can be used appropriately            
            ParticipSummaryData.Tables[4].TableName = "dtOptionPrdList";
            ParticipSummaryData.Tables[5].TableName = "dtSellerGrpList";
            ParticipSummaryData.Tables[6].TableName = "dtEscCodeList";

            Session["ParticipantDropdownData"] = ParticipSummaryData;
            Session["ParticipationGridData"] = ParticipSummaryData.Tables[3];
            Session["ParticipMaintSellerGrpList"] = ParticipSummaryData.Tables[5];

            if (errorId == 2)
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
                return;
            }

            if (ParticipSummaryData.Tables.Count == 0)
            {
                dtEmpty = new DataTable();
                gvPartSummary.DataSource = dtEmpty;
                gvPartSummary.EmptyDataText = "No data found for the selected catalogue number";
                gvPartSummary.DataBind();

            }
            else
            {
                PopulateCatalogueDetails(ParticipSummaryData.Tables["dtCatDetails"], ParticipSummaryData.Tables["dtCatNoStatusList"]);

                if (ParticipSummaryData.Tables[0].Rows.Count == 0)
                {
                    gvPartSummary.EmptyDataText = "No data found for the selected catalogue number";
                }

                gvPartSummary.DataSource = ParticipSummaryData.Tables["dtParicipants"];
                gvPartSummary.DataBind();

                if (lblCatTrackTimeShare.Text == "Track")
                {
                    txtTimeInsert.Enabled = false;
                    txtTotalTimeInsert.Enabled = false;
                    txtShareInsert.Text = "1";
                    txtTotalShareInsert.Text = "1";
                    hdnShareInsert.Value = "1";
                    hdnTotalShareInsert.Value = "1";
                }
                else
                {
                    txtShareInsert.Enabled = false;
                    txtTotalShareInsert.Enabled = false;
                }
            }
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
                }

                if (catDetails.Rows.Count != 0)
                {
                    lblCatNo.Text = catDetails.Rows[0]["catno"].ToString();
                    lblCatTitle.Text = catDetails.Rows[0]["catno_title"].ToString();
                    lblCatArtist.Text = catDetails.Rows[0]["artist_name"].ToString();
                    lblCatDealType.Text = catDetails.Rows[0]["deal_type_desc"].ToString();
                    lblCatTotalTracks.Text = catDetails.Rows[0]["total_tracks"].ToString();
                    lblCatTotalTime.Text = catDetails.Rows[0]["total_play_length"].ToString();
                    lblCatTrackTimeShare.Text = catDetails.Rows[0]["track_time_flag"].ToString();
                    hdnCatStatusCode.Value = catDetails.Rows[0]["status_code"].ToString();
                    hdnTimeTrack.Value = catDetails.Rows[0]["track_time"].ToString();

                    if (lblCatTotalTime.Text != string.Empty)
                    {
                        lblCatTotalTime.Text = TimeFormat(lblCatTotalTime.Text);
                    }

                    if (catDetails.Rows[0]["is_compilation"].ToString() == "Y")
                    {
                        cbCatCompilation.Checked = true;
                    }
                    else
                    {
                        cbCatCompilation.Checked = false;
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

                if (catDetails.Rows.Count == 0)
                {
                    btnUndoSaveCat.Visible = false;
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading Catalogue details", ex.Message);
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        /// <summary>
        /// If participant Status not all = Catno Status, display popup message to update all to cat status
        /// </summary>

        private string TimeFormat(string timeVal)
        {
            if (!timeVal.Contains(':'))
            {
                string totalTime = String.Format("{0:D7}", Convert.ToInt32(timeVal));//append leading zeros if the length is < 7
                string seconds = totalTime.Substring((totalTime.Length - 2), 2);
                string mins = totalTime.Substring((totalTime.Length - 4), 2);
                string hrs = totalTime.Substring((totalTime.Length - 7), 3);
                return hrs + ":" + mins + ":" + seconds;
            }
            else
            {
                return timeVal;
            }

        }


        #endregion Methods

        // JIRA-1070 Changes by Ravi -- Start

        protected void btnHdnRoyaltorInsertSearch_Click(object sender, EventArgs e)
        {
            try
            {
                PopulateOptPrdInsert();
                PopulateEscDodeInsert();
                ddlOptionPeriodInsert.Focus();

                if (lblCatTrackTimeShare.Text == "Track")
                {
                    txtTimeInsert.Enabled = false;
                    txtTotalTimeInsert.Enabled = false;
                    txtShareInsert.Text = "1";
                    txtTotalShareInsert.Text = "1";
                    hdnShareInsert.Value = "1";
                    hdnTotalShareInsert.Value = "1";
                    txtTrackNoInsert.Text = "";
                    txtTrackTitleInsert.Text = "";
                    txtTrackTitleInsert.Attributes.Remove("readonly");
                }
                else
                {
                    txtShareInsert.Enabled = false;
                    txtTotalShareInsert.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from list.", ex.Message);
            }
        }

        private DataTable GetOptionPeriodList(string royaltorId)
        {
            DataTable optionPrdList = new DataTable();
            optionPrdList = null;

            if (dsParticipantDropdownData.Tables["dtOptionPrdList"] != null)
            {
                DataRow[] optionPrdExist = dsParticipantDropdownData.Tables["dtOptionPrdList"].Select("royaltor_id='" + royaltorId + "'");
                if (optionPrdExist.Count() != 0)
                {
                    optionPrdList = dsParticipantDropdownData.Tables["dtOptionPrdList"].Select("royaltor_id='" + royaltorId + "'").CopyToDataTable();
                }

            }

            return optionPrdList;
        }

        private DataTable GetEscCodeList(string royaltorId)
        {
            DataTable escCodeList = new DataTable();
            escCodeList = null;

            if (dsParticipantDropdownData.Tables["dtEscCodeList"] != null)
            {
                DataRow[] optionPrdExist = dsParticipantDropdownData.Tables["dtEscCodeList"].Select("royaltor_id='" + royaltorId + "'");
                if (optionPrdExist.Count() != 0)
                {
                    escCodeList = dsParticipantDropdownData.Tables["dtEscCodeList"].Select("royaltor_id='" + royaltorId + "'").CopyToDataTable();
                }

            }

            return escCodeList;
        }

        private void PopulateOptPrdInsert()
        {
            DataTable optionPrdList;
            ddlOptionPeriodInsert.Items.Clear();

            if (txtRoyaltorInsert.Text.Trim() == string.Empty || txtRoyaltorInsert.Text.IndexOf("-") < 0)
            {
                ddlOptionPeriodInsert.Items.Insert(0, new ListItem("-"));
                ddlOptionPeriodInsert.SelectedIndex = 0;

                return;
            }

            string royaltorId = txtRoyaltorInsert.Text.Substring(0, txtRoyaltorInsert.Text.IndexOf("-") - 1);

            if (royaltorId != string.Empty)
            {
                //get option period list for the royaltor
                dsParticipantDropdownData = (DataSet)Session["ParticipantDropdownData"];
                optionPrdList = GetOptionPeriodList(royaltorId);
                if (optionPrdList != null)
                {
                    ddlOptionPeriodInsert.DataSource = optionPrdList;
                    ddlOptionPeriodInsert.DataTextField = "item_text";
                    ddlOptionPeriodInsert.DataValueField = "item_value";
                    ddlOptionPeriodInsert.DataBind();
                    ddlOptionPeriodInsert.Items.Insert(0, new ListItem("-"));

                    //if only one option period for the royaltor then select it
                    if (optionPrdList.Rows.Count == 1)
                    {
                        ddlOptionPeriodInsert.SelectedIndex = 1;
                    }
                }
                else
                {
                    ddlOptionPeriodInsert.Items.Insert(0, new ListItem("-"));
                    ddlOptionPeriodInsert.SelectedIndex = 0;
                }
            }
            else
            {
                ddlOptionPeriodInsert.Items.Insert(0, new ListItem("-"));
                ddlOptionPeriodInsert.SelectedIndex = 0;
            }
        }

        private void PopulateEscDodeInsert()
        {
            DataTable escCodeList;
            ddlEscCodeInsert.Items.Clear();

            if (txtRoyaltorInsert.Text.Trim() == string.Empty || txtRoyaltorInsert.Text.IndexOf("-") < 0)
            {
                ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));
                ddlEscCodeInsert.SelectedIndex = 0;

                return;
            }

            string royaltorId = txtRoyaltorInsert.Text.Substring(0, txtRoyaltorInsert.Text.IndexOf("-") - 1);

            if (royaltorId != string.Empty)
            {
                //get option period list for the royaltor
                dsParticipantDropdownData = (DataSet)Session["ParticipantDropdownData"];
                escCodeList = GetEscCodeList(royaltorId);
                if (escCodeList != null)
                {
                    ddlEscCodeInsert.DataSource = escCodeList;
                    ddlEscCodeInsert.DataTextField = "esc_code";
                    ddlEscCodeInsert.DataValueField = "esc_code";
                    ddlEscCodeInsert.DataBind();
                    ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));

                }
                else
                {
                    ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));
                    ddlEscCodeInsert.SelectedIndex = 0;
                }
            }
            else
            {
                ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));
                ddlEscCodeInsert.SelectedIndex = 0;
            }
        }

        #region Fuzzy Search

        protected void btnFuzzyRoyaltorListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                FuzzySearchRoyaltor(txtRoyaltorInsert.Text);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search popup", ex.Message);
            }
        }

        protected void btnFuzzyTerritoryListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                FuzzySearchTerritory(hdnFuzzySearchText.Value);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory fuzzy search popup", ex.Message);
            }
        }

        private void FuzzySearchRoyaltor(string searchText)
        {
            lblFuzzySearchPopUp.Text = "Royaltor - Complete Search List";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(searchText.ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchTerritory(string searchText)
        {
            lblFuzzySearchPopUp.Text = "Territory - Complete Search List";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyPartMaintSellerGrpList(searchText.ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }


        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "RoyaltorAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtRoyaltorInsert.Text = string.Empty;
                        return;
                    }

                    txtRoyaltorInsert.Text = lbFuzzySearch.SelectedValue.ToString();
                    PopulateOptPrdInsert();
                    PopulateEscCodeInsert();
                }

                else if (hdnFuzzySearchField.Value == "TerritoryAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtTerritoryInsert.Text = string.Empty;
                        return;
                    }

                    txtTerritoryInsert.Text = lbFuzzySearch.SelectedValue.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting item from list", ex.Message);
            }
        }

        private void PopulateEscCodeInsert()
        {
            DataTable escCodeList;
            ddlEscCodeInsert.Items.Clear();

            if (txtRoyaltorInsert.Text.Trim() == string.Empty || txtRoyaltorInsert.Text.IndexOf("-") < 0)
            {
                ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));
                ddlEscCodeInsert.SelectedIndex = 0;

                return;
            }

            string royaltorId = txtRoyaltorInsert.Text.Substring(0, txtRoyaltorInsert.Text.IndexOf("-") - 1);

            if (royaltorId != string.Empty)
            {
                //get option period list for the royaltor
                dsParticipantDropdownData = (DataSet)Session["ParticipantDropdownData"];
                escCodeList = GetEscCodeList(royaltorId);
                if (escCodeList != null)
                {
                    ddlEscCodeInsert.DataSource = escCodeList;
                    ddlEscCodeInsert.DataTextField = "esc_code";
                    ddlEscCodeInsert.DataValueField = "esc_code";
                    ddlEscCodeInsert.DataBind();
                    ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));
                }
                else
                {
                    ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));
                    ddlEscCodeInsert.SelectedIndex = 0;
                }
            }
            else
            {
                ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));
                ddlEscCodeInsert.SelectedIndex = 0;
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "RoyaltorInsert")
                {
                    txtRoyaltorInsert.Text = string.Empty;
                    PopulateOptPrdInsert();
                    PopulateEscCodeInsert();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing full search popup", ex.Message);
            }
        }

        #endregion Fuzzy Search


        protected void btnSaveAllChanges_Click(object sender, EventArgs e)
        {
            try
            {
                Array modifiedRowList = ModifiedRowsList();
                string catPerviousStatus = hdnCatStatusCode.Value;
                string catCurrentStatus = ddlCatStatus.SelectedValue;

                //check if any changes to save
                if (modifiedRowList.Length == 0 && catPerviousStatus == catCurrentStatus)
                {
                    msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                participantSummaryBL = new ParticipantSummaryBL();
                DataSet updatedData = participantSummaryBL.SaveParticipantDetails(lblCatNo.Text, ddlCatStatus.SelectedValue, catPerviousStatus == catCurrentStatus ? "N" : "Y", modifiedRowList,
                                                                                        LoadOnlyActiveParticipats(), Convert.ToString(Session["UserCode"]), Convert.ToString(Session["UserRoleId"]), out errorId);//JIRA-898
                participantSummaryBL = null;


                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;


                if (errorId == 0)
                {
                    if (updatedData.Tables.Count != 0)
                    {
                        if (updatedData.Tables[0].Rows.Count > 0)
                        {
                            hdnCatStatusCode.Value = updatedData.Tables[0].Rows[0]["status_code"].ToString();

                            ddlCatStatus.ClearSelection();
                            if (ddlCatStatus.Items.FindByValue(updatedData.Tables[0].Rows[0]["status_code"].ToString()) != null)
                            {
                                ddlCatStatus.Items.FindByValue(updatedData.Tables[0].Rows[0]["status_code"].ToString()).Selected = true;
                            }
                            else
                            {
                                ddlCatStatus.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            hdnCatStatusCode.Value = null;
                            ddlCatStatus.SelectedIndex = 0;
                        }

                        Session["ParticipationGridData"] = updatedData.Tables[1];
                        gvPartSummary.DataSource = updatedData.Tables[1];
                        gvPartSummary.DataBind();

                        if (updatedData.Tables[1].Rows.Count == 0)
                        {
                            gvPartSummary.EmptyDataText = "No data found for the selected catalogue number";
                        }
                        else
                        {
                            gvPartSummary.EmptyDataText = string.Empty;
                        }

                    }
                    else if (updatedData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvPartSummary.DataSource = dtEmpty;
                        gvPartSummary.EmptyDataText = "No data found for the selected catalogue number";
                        gvPartSummary.DataBind();
                    }
                    msgView.SetMessage("Participant details saved.", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving participant data", string.Empty);
                }
                else if (errorId == 3)
                {
                    msgView.SetMessage("Failed to save changes! Duplicate rows exists for same Catalogue, Royaltor Id, Option Period, Territory, Esc Code , Tune Id and Participation Type combination", MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    ExceptionHandler("Error in saving participant data", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving paricipant details", ex.Message);
            }

        }

        protected void imgBtnSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Page.Validate("valAddRow");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (txtTimeInsert.Text != "___:__:__" && txtTotalTimeInsert.Text != "___:__:__")
                {
                    if (Convert.ToInt32(txtTimeInsert.Text.Replace(":", "")) > Convert.ToInt32(txtTotalTimeInsert.Text.Replace(":", "")))
                    {
                        msgView.SetMessage("Time must be less than or equal to Total Time", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                AppendRowToGrid();
                ClearAddRow();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving participant data", ex.Message);
            }
        }

        private void UserAuthorization()
        {
            hdnUserRole.Value = Session["UserRole"].ToString();
            //Validation: only Super user can access this page
            if (Session["UserRole"].ToString().ToLower() == UserRole.SuperUser.ToString().ToLower())
            {
                hdnIsSuperUser.Value = "Y";
            }
            //WUIN-1096 Only Read access for Reaonly User
            else if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnCorrectMismatches.Enabled = false;
                btnUndoSaveCat.Enabled = false;
                imgBtnSave.Enabled = false;
                imgBtnCancel.Enabled = false;
                foreach (GridViewRow rows in gvPartSummary.Rows)
                {
                    (rows.FindControl("imgBtnUpdateParticipant") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                }
            }

        }


        private Array ModifiedRowsList()
        {
            List<string> modifiedRowsList = new List<string>();
            string participId;
            string royaltorId;
            string optionPeriodCode;
            string sellerGroupCode;
            string shareTracks;
            string shareTotalTracks;
            string shareTime;
            string shareTotalTime;
            string tuneCode;
            string tuneTitle;
            string escCode;
            string escIncludeUnits;
            string participationType;
            string overRide;
            string statusCode;
            string isrcVal;
            string trackListingId;
            string isISRCTrackTitle;
            string isModified;
            string isStatusChanged = string.Empty;
            string isActiveChanged = string.Empty;
            string hdnParticipActive;
            string hdnParticipStatus;

            foreach (GridViewRow gvr in gvPartSummary.Rows)
            {

                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                isrcVal = (gvr.FindControl("hdnISRCVal") as HiddenField).Value;
                trackListingId = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;
                isISRCTrackTitle = (gvr.FindControl("hdnIsISRCTrackTitle") as HiddenField).Value;

                if (isModified == Global.DBNullParamValue || isModified == "Y")//new row
                {
                    participId = (gvr.FindControl("hdnParticipId") as HiddenField).Value;
                    royaltorId = (gvr.FindControl("hdnParticipRoyId") as HiddenField).Value;
                    optionPeriodCode = (gvr.FindControl("hdnParticipOptionPeriod") as HiddenField).Value;

                    sellerGroupCode = (gvr.FindControl("lblTerritory") as Label).Text;
                    if (sellerGroupCode == string.Empty)
                    {
                        sellerGroupCode = wildCardChar;
                    }
                    else
                    {
                        sellerGroupCode = (sellerGroupCode.Trim().IndexOf("-") - 1).ToString();
                    }

                    shareTracks = (gvr.FindControl("hdnShare") as HiddenField).Value;
                    shareTotalTracks = (gvr.FindControl("hdnTotalShare") as HiddenField).Value;
                    shareTime = (gvr.FindControl("hdnTime") as HiddenField).Value.Replace(":", "");
                    shareTotalTime = (gvr.FindControl("hdnTotalTime") as HiddenField).Value.Replace(":", "");
                    tuneCode = (gvr.FindControl("hdnTrackNo") as HiddenField).Value;
                    tuneTitle = (gvr.FindControl("hdnTrackTitle") as HiddenField).Value;
                    escCode = (gvr.FindControl("hdnEscalationCode") as HiddenField).Value;
                    if (escCode == "-")
                    {
                        escCode = string.Empty;
                    }

                    if ((gvr.FindControl("cbEscIncludeUnits") as CheckBox).Checked)
                    {
                        escIncludeUnits = "Y";
                    }
                    else
                    {
                        escIncludeUnits = "N";
                    }
                    hdnParticipActive = (gvr.FindControl("hdnParticipActive") as HiddenField).Value;
                    hdnParticipStatus = (gvr.FindControl("hdnParticipStatus") as HiddenField).Value;

                    statusCode = (gvr.FindControl("ddlStatus") as DropDownList).SelectedValue;
                    if ((gvr.FindControl("cbActive") as CheckBox).Checked)
                    {
                        participationType = "A";
                    }
                    else
                    {
                        participationType = "I";
                    }

                    if ((gvr.FindControl("cbOverride") as CheckBox).Checked)
                    {
                        overRide = "Y";
                    }
                    else
                    {
                        overRide = "N";
                    }
                    if (isModified == "Y")
                    {
                        if (hdnParticipActive != participationType)
                        {
                            isActiveChanged = "Y";
                        }
                        if (hdnParticipStatus != statusCode)
                        {
                            isStatusChanged = "Y";
                        }
                    }
                    else
                    {
                        isActiveChanged = "N";
                        isStatusChanged = "N";
                    }

                    modifiedRowsList.Add(participId + Global.DBDelimiter + royaltorId + Global.DBDelimiter + optionPeriodCode + Global.DBDelimiter + sellerGroupCode + Global.DBDelimiter
                        + shareTracks + Global.DBDelimiter + shareTotalTracks + Global.DBDelimiter + shareTime + Global.DBDelimiter + shareTotalTime + Global.DBDelimiter + tuneCode + Global.DBDelimiter +
                        tuneTitle + Global.DBDelimiter + escCode + Global.DBDelimiter + escIncludeUnits + Global.DBDelimiter + participationType + Global.DBDelimiter + overRide + Global.DBDelimiter + statusCode + Global.DBDelimiter +
                                        isModified + Global.DBDelimiter + isrcVal + Global.DBDelimiter + trackListingId + Global.DBDelimiter + isISRCTrackTitle + Global.DBDelimiter + isActiveChanged + Global.DBDelimiter + isStatusChanged);
                }

            }

            return modifiedRowsList.ToArray();
        }

        private void ClearAddRow()
        {
            txtRoyaltorInsert.Text = string.Empty;
            ddlOptionPeriodInsert.Items.Clear();
            txtTerritoryInsert.Text = string.Empty;
            txtTimeInsert.Text = string.Empty;
            txtTotalTimeInsert.Text = string.Empty;
            ddlEscCodeInsert.Items.Clear();
            txtShareInsert.Text = hdnShareInsert.Value;
            txtTotalShareInsert.Text = hdnTotalShareInsert.Value;
            txtTrackNoInsert.Text = string.Empty;
            txtTrackTitleInsert.Text = string.Empty;
            txtTrackTitleInsert.Attributes.Remove("readonly");
            cbEscIncludeUnitsInsert.Checked = false;
            hdnTrackTitlefromISRC.Value = "N";
            hdnISRC.Value = "";
            hdnTrackListingID.Value = "";
        }

        private void GetGridData()
        {

            DataTable dtGridData = (DataTable)Session["ParticipationGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            CheckBox cbActive;
            CheckBox cbEscIncludeUnits;
            CheckBox cbOverride;
            string territory;

            foreach (GridViewRow gvr in gvPartSummary.Rows)
            {
                DataRow drGridRow = dtGridChangedData.NewRow();
                cbActive = (gvr.FindControl("cbActive") as CheckBox);
                cbEscIncludeUnits = (gvr.FindControl("cbEscIncludeUnits") as CheckBox);
                cbOverride = (gvr.FindControl("cbOverride") as CheckBox);
                drGridRow["participation_id"] = (gvr.FindControl("hdnParticipId") as HiddenField).Value;
                drGridRow["royaltor_id"] = (gvr.FindControl("hdnParticipRoyId") as HiddenField).Value;
                drGridRow["royaltor"] = (gvr.FindControl("lblRoyaltor") as Label).Text;

                drGridRow["option_period_code"] = (gvr.FindControl("hdnParticipOptionPeriod") as HiddenField).Value;
                drGridRow["option_period"] = (gvr.FindControl("lblOptionPeriod") as Label).Text;

                territory = (gvr.FindControl("lblTerritory") as Label).Text.Trim();

                if (territory == "-")
                {
                    drGridRow["seller_group_code"] = wildCardChar;
                    drGridRow["territory"] = Global.DBNullParamValue;
                }
                else
                {
                    drGridRow["seller_group_code"] = territory.Substring(0, territory.IndexOf("-") - 1);
                    drGridRow["territory"] = territory;
                }

                if (!string.IsNullOrEmpty((gvr.FindControl("hdnShare") as HiddenField).Value))
                {
                    drGridRow["share_tracks"] = (gvr.FindControl("hdnShare") as HiddenField).Value;
                }

                if (!string.IsNullOrEmpty((gvr.FindControl("hdnTotalShare") as HiddenField).Value))
                {
                    drGridRow["share_total_tracks"] = (gvr.FindControl("hdnTotalShare") as HiddenField).Value;
                }

                drGridRow["share_time"] = (gvr.FindControl("hdnTime") as HiddenField).Value;
                drGridRow["share_total_time"] = (gvr.FindControl("hdnTotalTime") as HiddenField).Value;
                drGridRow["track_title"] = (gvr.FindControl("hdnTrackTitle") as HiddenField).Value;
                drGridRow["esc_code"] = (gvr.FindControl("hdnEscalationCode") as HiddenField).Value;
                drGridRow["inc_in_escalation"] = (gvr.FindControl("hdnEscIncludeUnits") as HiddenField).Value;

                drGridRow["inc_in_escalation"] = cbEscIncludeUnits.Checked ? "Y" : "N";
                drGridRow["participation_type"] = cbActive.Checked ? "A" : "I";
                drGridRow["override"] = cbOverride.Checked ? "Y" : "N";
                drGridRow["status_code"] = (gvr.FindControl("ddlStatus") as DropDownList).SelectedValue;
                drGridRow["is_active"] = (gvr.FindControl("hdnActive") as HiddenField).Value;
                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                if ((gvr.FindControl("hdnTrackNo") as HiddenField).Value != "")
                {
                    drGridRow["tune_code"] = (gvr.FindControl("hdnTrackNo") as HiddenField).Value;
                }
                else
                {
                    drGridRow["tune_code"] = DBNull.Value;
                }
                drGridRow["isrc_val"] = (gvr.FindControl("hdnISRCVal") as HiddenField).Value;
                if ((gvr.FindControl("hdnTrackListingId") as HiddenField).Value != "")
                {
                    drGridRow["tracklisting_id"] = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;
                }
                else
                {
                    drGridRow["tracklisting_id"] = DBNull.Value;
                }
                drGridRow["is_isrc_track_title"] = (gvr.FindControl("hdnIsISRCTrackTitle") as HiddenField).Value;


                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["ParticipationGridData"] = dtGridChangedData;

        }


        private void AppendRowToGrid()
        {
            GetGridData();

            if (Session["ParticipationGridData"] == null)
            {
                ExceptionHandler("Error in adding rate row to grid", string.Empty);
            }

            DataTable dtGridData = (DataTable)Session["ParticipationGridData"];
            DataRow drNewRow = dtGridData.NewRow();
            drNewRow["participation_id"] = Convert.ToInt32(hdnNewParticipId.Value);
            hdnNewParticipId.Value = (Convert.ToInt32(hdnNewParticipId.Value) - 1).ToString();
            if (!string.IsNullOrWhiteSpace(txtRoyaltorInsert.Text))
            {
                drNewRow["royaltor_id"] = Convert.ToInt32(txtRoyaltorInsert.Text.Split('-')[0].Trim());
                drNewRow["royaltor"] = txtRoyaltorInsert.Text;
            }
            if (ddlOptionPeriodInsert.SelectedIndex > 0)
            {
                drNewRow["option_period_code"] = ddlOptionPeriodInsert.SelectedValue;
                drNewRow["option_period"] = ddlOptionPeriodInsert.SelectedItem.Text;
            }
            else
            {
                drNewRow["option_period_code"] = string.Empty;
                drNewRow["option_period"] = string.Empty; ;
            }
            if (txtTerritoryInsert.Text == string.Empty)
            {
                drNewRow["seller_group_code"] = wildCardChar;
                drNewRow["territory"] = Global.DBNullParamValue;
            }
            else
            {
                drNewRow["seller_group_code"] = txtTerritoryInsert.Text.Substring(0, txtTerritoryInsert.Text.IndexOf("-") - 1);
                drNewRow["territory"] = txtTerritoryInsert.Text;
            }

            if (!string.IsNullOrWhiteSpace(txtShareInsert.Text))
            {
                drNewRow["share_tracks"] = txtShareInsert.Text.Trim();
            }
            else
            {
                drNewRow["share_tracks"] = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(txtTotalShareInsert.Text))
            {
                drNewRow["share_total_tracks"] = txtTotalShareInsert.Text.Trim();
            }
            else
            {
                drNewRow["share_total_tracks"] = DBNull.Value;
            }

            if (txtTimeInsert.Text == "___:__:__")
            {
                drNewRow["share_time"] = "0000000";
            }
            else
            {
                drNewRow["share_time"] = txtTimeInsert.Text.Trim().Replace(":", "");
            }

            if (txtTotalTimeInsert.Text == "___:__:__")
            {
                drNewRow["share_total_time"] = "0000000";
            }
            else
            {
                drNewRow["share_total_time"] = txtTotalTimeInsert.Text.Trim().Replace(":", "");
            }


            if (!string.IsNullOrWhiteSpace(txtTrackNoInsert.Text))
            {
                drNewRow["tune_code"] = txtTrackNoInsert.Text.Trim();
            }
            else
            {
                drNewRow["tune_code"] = DBNull.Value;
            }
            //JIRA-1074 Changes -- Start
            drNewRow["track_title"] = txtTrackTitleInsert.Text.Trim();

            if (ddlEscCodeInsert.SelectedIndex > 0)
            {
                drNewRow["esc_code"] = ddlEscCodeInsert.SelectedValue;
            }
            else
            {
                drNewRow["esc_code"] = string.Empty;
            }

            if (cbEscIncludeUnitsInsert.Checked)
            {
                drNewRow["inc_in_escalation"] = "Y";
            }
            else
            {
                drNewRow["inc_in_escalation"] = "N";
            }

            drNewRow["participation_type"] = "A";
            drNewRow["is_active"] = "Y";
            drNewRow["override"] = "Y";
            drNewRow["status_code"] = "1";
            drNewRow["isrc_val"] = hdnISRC.Value;
            if (hdnTrackListingID.Value != string.Empty)
            {
                drNewRow["tracklisting_id"] = hdnTrackListingID.Value;
            }
            else
            {
                drNewRow["tracklisting_id"] = DBNull.Value;
            }
            drNewRow["is_isrc_track_title"] = hdnTrackTitlefromISRC.Value;

            drNewRow["is_modified"] = Global.DBNullParamValue;

            dtGridData.Rows.Add(drNewRow);
            Session["ParticipationGridData"] = dtGridData;
            gvPartSummary.DataSource = dtGridData;
            gvPartSummary.DataBind();
        }

        protected void txtTrackNoInsert_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtTrackNoInsert.Text != "" && txtShareInsert.Text == "1" && Convert.ToInt16(txtTotalShareInsert.Text) > 1)
                {
                    string seqNo = txtTrackNoInsert.Text;
                    string trackTitle = string.Empty;
                    string isrc = string.Empty;
                    string track_listing_id = string.Empty;
                    participantSummaryBL = new ParticipantSummaryBL();
                    participantSummaryBL.GetTrackTitlefromISRC(lblCatNo.Text, seqNo, out errorId, out trackTitle, out isrc, out track_listing_id);
                    participantSummaryBL = null;

                    if (errorId == 0)
                    {

                        txtTrackTitleInsert.Text = trackTitle;
                        txtTrackTitleInsert.Attributes.Add("readonly", "readOnly");
                        hdnTrackTitlefromISRC.Value = "Y";
                        hdnISRC.Value = isrc;
                        hdnTrackListingID.Value = track_listing_id;
                        ddlEscCodeInsert.Focus();
                    }
                    else if (errorId == 1)
                    {
                        txtTrackNoInsert.Text = "";
                        txtTrackTitleInsert.Text = "";
                        txtTrackTitleInsert.Attributes.Add("readonly", "readOnly");
                        hdnTrackTitlefromISRC.Value = "N";
                        hdnISRC.Value = "";
                        hdnTrackListingID.Value = "";
                        mpeShowNoTrackMsg.Show();
                    }
                    else
                    {
                        ExceptionHandler("Error in fetching Track Title from ISRC for given Track No", string.Empty);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching Track Title from ISRC for given Track No", ex.Message);
            }
        }

        // JIRA-1070 Changes by Ravi -- End

    }
}