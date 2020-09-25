/*
File Name   :   ParticipantMaintenance.cs
Purpose     :   to maintain Participants data (WUIN-127)

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     05-July-2017     Pratik(Infosys Limited)   Initial Creation
 *      26-Jan-2018      Harish                    WUIN-440
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
using AjaxControlToolkit;
using System.Globalization;

namespace WARS
{
    public partial class ParticipantMaintenance : System.Web.UI.Page
    {
        #region Global Declarations
        ParticipantMaintenanceBL participantMaintenanceBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataSet dsParticipantDropdownData;
        string catalogueNo = string.Empty;
        string wildCardChar = ".";
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
                //catalogueNo = "0825646003709";//150 participants
                //catalogueNo = "A10302B0001712161X";
                //catalogueNo = "A10302B00009075136"; // UAT
                //catalogueNo = "0075992585040";
                //catalogueNo = "A10302B0000665603T";

                if (catalogueNo == null)
                {
                    ExceptionHandler("Invalid catalogue number.", "");
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Participant Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Participant Maintenance";
                }

                //lblTab.Focus();//tabbing sequence starts here   
                txtRoyaltorInsert.Focus();
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {

                        LoadInitialData();
                        if (hdnTimeTrack.Value == "M" || hdnConfigCode.Value == "S")
                        {
                            txtTrackNoInsert.Enabled = true;
                            txtTrackTitleInsert.Enabled = true;
                        }
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

        protected void gvParicipantDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Session["ParticipantDropdownData"] == null)
                {
                    return;
                }

                DropDownList ddlOptionPeriod;
                DropDownList ddlEscalationCode;
                DropDownList ddlStatus;
                CheckBox cbActive;
                CheckBox cbEscIncludeUnits;
                string royaltorId;
                string optionPrdCode;
                string escalationCode;
                string statusCode;
                string isActive;
                string escIncludeUnits;
                DataTable optionPrdList;
                DataTable escalationCodeList;
                //JIRA-1074 Changes --Start
                string trackNo;
                string trackTitle;
                string txtShare;
                string txtTotalShare;
                string isModified;

                dsParticipantDropdownData = (DataSet)Session["ParticipantDropdownData"];

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlOptionPeriod = (e.Row.FindControl("ddlOptionPeriod") as DropDownList);
                    ddlEscalationCode = (e.Row.FindControl("ddlEscalationCode") as DropDownList);
                    ddlStatus = (e.Row.FindControl("ddlStatus") as DropDownList);
                    cbActive = (e.Row.FindControl("cbActive") as CheckBox);
                    cbEscIncludeUnits = (e.Row.FindControl("cbEscIncludeUnits") as CheckBox);
                    trackNo = (e.Row.FindControl("txtTrackNo") as TextBox).Text;
                    trackTitle = (e.Row.FindControl("txtTrackTitle") as TextBox).Text;
                    txtShare = (e.Row.FindControl("txtShare") as TextBox).Text;
                    txtTotalShare = (e.Row.FindControl("txtTotalShare") as TextBox).Text;
                    isModified = (e.Row.FindControl("hdnIsModified") as HiddenField).Value;

                    
                    //JIRA-1074 Changes --End
                    royaltorId = (e.Row.FindControl("hdnParticipRoyId") as HiddenField).Value;
                    optionPrdCode = (e.Row.FindControl("hdnParticipOptionPeriod") as HiddenField).Value;
                    escalationCode = (e.Row.FindControl("hdnEscalationCode") as HiddenField).Value;

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

                        //get esc code list for the royaltor
                        escalationCodeList = GetEscCodeList(royaltorId);
                        if (optionPrdList != null)
                        {
                            ddlEscalationCode.DataSource = escalationCodeList;
                            ddlEscalationCode.DataTextField = "esc_code";
                            ddlEscalationCode.DataValueField = "esc_code";
                            ddlEscalationCode.DataBind();
                            ddlEscalationCode.Items.Insert(0, new ListItem("-"));

                            if (ddlEscalationCode.Items.FindByValue(escalationCode) != null)
                            {
                                ddlEscalationCode.Items.FindByValue(escalationCode).Selected = true;
                            }
                            else
                            {
                                ddlEscalationCode.SelectedIndex = 0;
                                (e.Row.FindControl("hdnEscalationCode") as HiddenField).Value = "-";
                            }
                        }
                        else
                        {
                            ddlEscalationCode.Items.Insert(0, new ListItem("-"));
                            ddlEscalationCode.SelectedIndex = 0;
                            (e.Row.FindControl("hdnEscalationCode") as HiddenField).Value = "-";
                        }
                    }

                    statusCode = (e.Row.FindControl("hdnParticipStatus") as HiddenField).Value;

                    if (dsParticipantDropdownData.Tables["dtStatusList"] != null)
                    {
                        ddlStatus.DataSource = dsParticipantDropdownData.Tables["dtStatusList"];
                        ddlStatus.DataTextField = "item_text";
                        ddlStatus.DataValueField = "item_value";
                        ddlStatus.DataBind();
                        ddlStatus.Items.Insert(0, new ListItem("-"));

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

                    isActive = (e.Row.FindControl("hdnParticipActive") as HiddenField).Value;

                    if (isActive == "A")
                    {
                        cbActive.Checked = true;
                    }
                    else
                    {
                        cbActive.Checked = false;
                    }

                    escIncludeUnits = (e.Row.FindControl("hdnEscIncludeUnits") as HiddenField).Value;

                    if (escIncludeUnits == "Y")
                    {
                        cbEscIncludeUnits.Checked = true;
                    }
                    else
                    {
                        cbEscIncludeUnits.Checked = false;
                    }

                    if (ddlEscalationCode.SelectedIndex > 0)
                    {
                        cbEscIncludeUnits.Enabled = true;
                    }
                    else
                    {
                        cbEscIncludeUnits.Enabled = false;
                    }

                    //if track_time_flag = 'T' then disable time and total time else disable share and total share 
                    if (lblTimeTrackShare.Text == "Track")
                    {
                        (e.Row.FindControl("txtTime") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtTotalTime") as TextBox).Enabled = false;
                    }
                    else
                    {
                        (e.Row.FindControl("txtShare") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtTotalShare") as TextBox).Enabled = false;
                        (e.Row.FindControl("revTotalShare") as RegularExpressionValidator).Enabled = false;
                        (e.Row.FindControl("revShare") as RegularExpressionValidator).Enabled = false;
                    }

                    //JIRA-1270 Changes -- Start

                    //if share=1 and totalshare > 1 then enable trackno and disable tracktitle 
                    if ((txtShare.Trim() == "1" && Convert.ToInt16(txtTotalShare.Trim()) > 1 && hdnTimeTrack.Value == "T") || (hdnTimeTrack.Value == "M") || (hdnConfigCode.Value == "S"))
                    {
                        if ((e.Row.FindControl("hdnIsISRCTrackTitle") as HiddenField).Value == "Y")
                        {
                            (e.Row.FindControl("txtTrackTitle") as TextBox).Attributes.Add("readonly", "readOnly"); ;
                        }
                        else
                        {
                            (e.Row.FindControl("txtTrackTitle") as TextBox).Attributes.Remove("readonly"); ;

                        }
                    }
                    //if share=1 and totalshare = 1 then disable trackno and tracktitle 
                    //if share >= 1 and totalshare >= 1 then enable trackno and enable tracktitle 
                    else if ((Convert.ToInt16(txtShare.Trim()) >= 1 && Convert.ToInt16(txtTotalShare.Trim()) >= 1 && hdnTimeTrack.Value == "T") || (hdnTimeTrack.Value == "M") || (hdnConfigCode.Value == "S"))
                    {
                        (e.Row.FindControl("txtTrackTitle") as TextBox).Attributes.Remove("readonly"); ;

                    }
                    //JIRA-1270 Changes -- End
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

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                if (catalogueNo.Trim() != string.Empty)
                {
                    Response.Redirect("../Audit/ParticipantAudit.aspx?CatNo=" + catalogueNo + "&pageName=" + "ParticipantMaint" + "", false);
                }
                else
                {
                    Response.Redirect("../Audit/ParticipantAudit.aspx", false);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvParicipantDetails_Sorting(object sender, GridViewSortEventArgs e)
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
                gvParicipantDetails.DataSource = dataView;
                gvParicipantDetails.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvParicipantDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

                if (e.CommandName == "cancelRow")
                {
                    GridViewRow gvr = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    string participationId = (gvr.FindControl("hdnParticipId") as HiddenField).Value;
                    string isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;


                    DropDownList ddlOptionPeriod;
                    DropDownList ddlStatus;
                    DropDownList ddlEscalationCode;
                    CheckBox cbActive;
                    CheckBox cbEscIncludeUnits;

                    string royaltorId;
                    string optionPrdCode;
                    string statusCode;
                    string escalationCode;
                    string isActive;
                    string escIncludeUnits;
                    DataTable optionPrdList;
                    string trackNo;
                    string trackTitle;
                    string hdnShare;
                    string hdnTotalShare;

                    ddlOptionPeriod = (gvr.FindControl("ddlOptionPeriod") as DropDownList);
                    ddlEscalationCode = (gvr.FindControl("ddlEscalationCode") as DropDownList);
                    ddlStatus = (gvr.FindControl("ddlStatus") as DropDownList);
                    cbActive = (gvr.FindControl("cbActive") as CheckBox);
                    cbEscIncludeUnits = (gvr.FindControl("cbEscIncludeUnits") as CheckBox);
                    royaltorId = (gvr.FindControl("hdnParticipRoyId") as HiddenField).Value;
                    optionPrdCode = (gvr.FindControl("hdnParticipOptionPeriod") as HiddenField).Value;
                    trackNo = (gvr.FindControl("hdnTrackNo") as HiddenField).Value;
                    trackTitle = (gvr.FindControl("hdnTrackTitle") as HiddenField).Value;
                    hdnShare = (gvr.FindControl("hdnShare") as HiddenField).Value;
                    hdnTotalShare = (gvr.FindControl("hdnTotalShare") as HiddenField).Value;
                    statusCode = (gvr.FindControl("hdnParticipStatus") as HiddenField).Value;

                    ddlStatus.ClearSelection();
                    if (ddlStatus.Items.FindByValue(statusCode) != null)
                    {
                        ddlStatus.Items.FindByValue(statusCode).Selected = true;
                    }
                    else
                    {
                        ddlStatus.SelectedIndex = 0;
                    }

                    escalationCode = (gvr.FindControl("hdnEscalationCode") as HiddenField).Value;
                    ddlEscalationCode.ClearSelection();
                    if (ddlEscalationCode.Items.FindByValue(escalationCode) != null)
                    {
                        ddlEscalationCode.Items.FindByValue(escalationCode).Selected = true;
                    }
                    else
                    {
                        ddlEscalationCode.SelectedIndex = 0;
                    }

                    isActive = (gvr.FindControl("hdnParticipActive") as HiddenField).Value;
                    if (isActive == "A")
                    {
                        cbActive.Checked = true;
                    }
                    else
                    {
                        cbActive.Checked = false;
                    }

                    escIncludeUnits = (gvr.FindControl("hdnEscIncludeUnits") as HiddenField).Value;
                    if (escIncludeUnits == "Y")
                    {
                        cbEscIncludeUnits.Checked = true;
                    }
                    else
                    {
                        cbEscIncludeUnits.Checked = false;
                    }

                    (gvr.FindControl("txtTerritory") as TextBox).Text = (gvr.FindControl("hdnSellerGrp") as HiddenField).Value;
                    (gvr.FindControl("txtShare") as TextBox).Text = (gvr.FindControl("hdnShare") as HiddenField).Value;
                    (gvr.FindControl("txtTotalShare") as TextBox).Text = (gvr.FindControl("hdnTotalShare") as HiddenField).Value;
                    (gvr.FindControl("txtTime") as TextBox).Text = (gvr.FindControl("hdnTime") as HiddenField).Value;
                    (gvr.FindControl("txtTotalTime") as TextBox).Text = (gvr.FindControl("hdnTotalTime") as HiddenField).Value;

                    if (Session["ParticipantDropdownData"] == null)
                    {
                        return;
                    }

                    dsParticipantDropdownData = (DataSet)Session["ParticipantDropdownData"];
                    ddlOptionPeriod.Items.Clear();
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

                    }
                    if ((gvr.FindControl("hdnIsModified") as HiddenField).Value != Global.DBNullParamValue)
                    {
                        (gvr.FindControl("hdnIsModified") as HiddenField).Value = "N";
                    }

                    //JIRA-1270 Changes -- Start

                    //if share=1 and totalshare > 1 then enable trackno and make tracktitle read only
                    if ((hdnShare.Trim() == "1" && Convert.ToInt16(hdnTotalShare.Trim()) > 1 && hdnTimeTrack.Value == "T") || (hdnTimeTrack.Value == "M") || (hdnConfigCode.Value == "S"))
                    {
                        (gvr.FindControl("txtTrackNo") as TextBox).Enabled = true;
                        (gvr.FindControl("txtTrackTitle") as TextBox).Enabled = true;
                        if ((gvr.FindControl("hdnIsISRCTrackTitle") as HiddenField).Value == "Y")
                        {
                            (gvr.FindControl("txtTrackTitle") as TextBox).Attributes.Add("readonly", "readOnly"); ;
                        }
                        else
                        {
                            (gvr.FindControl("txtTrackTitle") as TextBox).Attributes.Remove("readonly"); ;

                        }
                    }
                    //if share >= 1 and totalshare >= 1 then enable trackno and enable tracktitle 
                    else if ((Convert.ToInt16(hdnShare.Trim()) > 1 && Convert.ToInt16(hdnTotalShare.Trim()) > 1 && hdnTimeTrack.Value == "T") || (hdnTimeTrack.Value == "M") || (hdnConfigCode.Value == "S"))
                    {
                        (gvr.FindControl("txtTrackTitle") as TextBox).Attributes.Remove("readonly"); ;

                    }

                    //JIRA-1270 Changes -- End

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting/cancelling grid data", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Page.Validate("valAddRow");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                AppendRowToGrid();
                ClearAddRow();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding paricipant row to grid", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                //validate 
                Page.Validate("valUpdate");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Paricipant details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;


                Array modifiedRowList = ModifiedRowsList();
                string catPerviousStatus = hdnStatusCode.Value;
                string catCurrentStatus = ddlCatStatus.SelectedValue;

                //check if any changes to save
                if (modifiedRowList.Length == 0 && catPerviousStatus == catCurrentStatus)
                {
                    msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //WUIN-1167 - Commenting as we update cat stautus to minimum of participant statuses.
                //if (!ValidateCatStatusCode())
                //{
                //    msgView.SetMessage("Paricipant details not saved – Not a valid status for catalogue!", MessageType.Warning, PositionType.Auto);
                //    return;
                //}

                participantMaintenanceBL = new ParticipantMaintenanceBL();
                DataSet updatedData = participantMaintenanceBL.SaveParticipantDetails(lblCatNo.Text, ddlCatStatus.SelectedValue, catPerviousStatus == catCurrentStatus ? "N" : "Y", modifiedRowList,
                                                                                        LoadOnlyActiveParticipats(), Convert.ToString(Session["UserCode"]), Convert.ToString(Session["UserRoleId"]), out errorId);//JIRA-898
                participantMaintenanceBL = null;

                hdnGridDataChanged.Value = "N";

                if (errorId == 0)
                {
                    if (updatedData.Tables.Count != 0)
                    {
                        if (updatedData.Tables[0].Rows.Count > 0)
                        {
                            hdnStatusCode.Value = updatedData.Tables[0].Rows[0]["status_code"].ToString();

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
                            hdnStatusCode.Value = null;
                            ddlCatStatus.SelectedIndex = 0;
                        }

                        Session["ParticipationGridData"] = updatedData.Tables[1];
                        gvParicipantDetails.DataSource = updatedData.Tables[1];
                        gvParicipantDetails.DataBind();

                        if (updatedData.Tables[1].Rows.Count == 0)
                        {
                            gvParicipantDetails.EmptyDataText = "No data found for the selected catalogue number";
                        }
                        else
                        {
                            gvParicipantDetails.EmptyDataText = string.Empty;
                        }

                    }
                    else if (updatedData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvParicipantDetails.DataSource = dtEmpty;
                        gvParicipantDetails.EmptyDataText = "No data found for the selected catalogue number";
                        gvParicipantDetails.DataBind();
                    }


                    msgView.SetMessage("Participant details saved.", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 3)
                {
                    msgView.SetMessage("Failed to save changes! Duplicate rows exists for same catalogue, royaltor, option period, territory, track no & track title combination", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving participant data", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving participant data", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving participant data", ex.Message);
            }
        }

        protected void btnCatMaintenance_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Participants/CatalogueMaintenance.aspx?catNo=" + catalogueNo + "", false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to catalogue maintenance screen.", ex.Message);
            }
        }

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

                LoadGridData();

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


        #endregion Events

        #region Methods

        private void LoadInitialData()
        {
            participantMaintenanceBL = new ParticipantMaintenanceBL();
            DataSet initialData = participantMaintenanceBL.GetInitialData(catalogueNo, LoadOnlyActiveParticipats(), Convert.ToString(Session["UserRoleId"]), out errorId); //JIRA-898 Changes
            participantMaintenanceBL = null;

            //name the datatables so that they can be used appropriately            
            initialData.Tables[2].TableName = "dtOptionPrdList";
            initialData.Tables[3].TableName = "dtSellerGrpList";
            initialData.Tables[5].TableName = "dtStatusList";
            initialData.Tables[6].TableName = "dtEscCodeList";

            Session["ParticipantDropdownData"] = initialData;
            Session["ParticipationGridData"] = initialData.Tables[1];
            Session["ParticipMaintSellerGrpList"] = initialData.Tables[3];

            if (errorId == 2)
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
                return;
            }

            if (initialData.Tables.Count == 0)
            {
                dtEmpty = new DataTable();
                gvParicipantDetails.DataSource = dtEmpty;
                gvParicipantDetails.EmptyDataText = "No data found for the selected catalogue number";
                gvParicipantDetails.DataBind();

            }
            else
            {
                PopulateCatalogueDetails(initialData.Tables[0], initialData.Tables[5]);


                if (initialData.Tables[1].Rows.Count == 0)
                {
                    gvParicipantDetails.EmptyDataText = "No data found for the selected catalogue number";
                }

                gvParicipantDetails.DataSource = initialData.Tables[1];
                gvParicipantDetails.DataBind();

                ddlOptionPeriodInsert.Items.Insert(0, new ListItem("-"));
                ddlEscCodeInsert.Items.Insert(0, new ListItem("-"));

                if (lblTimeTrackShare.Text == "Track")
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
                ddlCatStatus.DataSource = catStatusList;
                ddlCatStatus.DataTextField = "item_text";
                ddlCatStatus.DataValueField = "item_value";
                ddlCatStatus.DataBind();
                ddlCatStatus.Items.Insert(0, new ListItem("-"));

                if (catDetails.Rows.Count != 0)
                {
                    lblCatNo.Text = catDetails.Rows[0]["catno"].ToString();
                    lblCatTitle.Text = catDetails.Rows[0]["catno_title"].ToString();
                    lblCatArtist.Text = catDetails.Rows[0]["artist_name"].ToString();
                    lblCatDealType.Text = catDetails.Rows[0]["deal_type_desc"].ToString();
                    lblCatTotalTracks.Text = catDetails.Rows[0]["total_tracks"].ToString();
                    lblCatTotalTime.Text = catDetails.Rows[0]["total_play_length"].ToString();
                    lblTimeTrackShare.Text = catDetails.Rows[0]["track_time_flag"].ToString();
                    hdnStatusCode.Value = catDetails.Rows[0]["status_code"].ToString();
                    hdnConfigCode.Value = catDetails.Rows[0]["config_code"].ToString();
                    hdnTimeTrack.Value = catDetails.Rows[0]["track_time"].ToString();

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

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading Catalogue details", ex.Message);
            }
        }

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

        /// <summary>
        /// Loads grid with either all or only Active participants
        /// </summary>
        private void LoadGridData()
        {
            participantMaintenanceBL = new ParticipantMaintenanceBL();
            DataSet dtGridData = participantMaintenanceBL.GetInitialData(catalogueNo, LoadOnlyActiveParticipats(), Convert.ToString(Session["UserRoleId"]), out errorId); //JIRA-898 Changes
            participantMaintenanceBL = null;

            hdnGridDataChanged.Value = "N";
            Session["ParticipationGridData"] = dtGridData.Tables[1];

            //reset Catalogue status to initial value. This is needed if changes made not saved and opted to reload the gird display
            if (hdnStatusCode.Value != string.Empty)
            {
                ddlCatStatus.SelectedValue = hdnStatusCode.Value;
            }
            else
            {
                ddlCatStatus.SelectedIndex = 0;
            }


            if (errorId == 2)
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
                return;
            }

            if (dtGridData.Tables.Count == 0)
            {
                dtEmpty = new DataTable();
                gvParicipantDetails.DataSource = dtEmpty;
                gvParicipantDetails.EmptyDataText = "No data found for the selected catalogue number";
                gvParicipantDetails.DataBind();

            }
            else
            {
                if (dtGridData.Tables[1].Rows.Count == 0)
                {
                    gvParicipantDetails.EmptyDataText = "No data found for the selected catalogue number";
                }

                gvParicipantDetails.DataSource = dtGridData.Tables[1];
                gvParicipantDetails.DataBind();
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
            }
            else
            {
                drNewRow["option_period_code"] = string.Empty;
            }


            if (txtTerritoryAddRow.Text == string.Empty)
            {
                drNewRow["seller_group_code"] = string.Empty;
                drNewRow["seller_group"] = string.Empty;
            }
            else
            {
                drNewRow["seller_group_code"] = txtTerritoryAddRow.Text.Substring(0, txtTerritoryAddRow.Text.IndexOf("-") - 1);
                drNewRow["seller_group"] = txtTerritoryAddRow.Text;
            }

            if (!string.IsNullOrWhiteSpace(txtShareInsert.Text))
            {
                drNewRow["share_tracks"] = txtShareInsert.Text.Trim();
            }

            if (!string.IsNullOrWhiteSpace(txtTotalShareInsert.Text))
            {
                drNewRow["share_total_tracks"] = txtTotalShareInsert.Text.Trim();
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
                drNewRow["track_no"] = txtTrackNoInsert.Text.Trim();
            }
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
            drNewRow["status_code"] = 1;
            drNewRow["end_date"] = DBNull.Value;
            drNewRow["isrc_val"] = hdnISRCValInsert.Value;
            if (hdnTrackListingIdInsert.Value != string.Empty)
            {
                drNewRow["tracklisting_id"] = hdnTrackListingIdInsert.Value;
            }
            else
            {
                drNewRow["tracklisting_id"] = DBNull.Value;
            }
            drNewRow["is_isrc_track_title"] = hdnIsISRCTrackTitleInsert.Value;

            drNewRow["is_modified"] = Global.DBNullParamValue;


            dtGridData.Rows.Add(drNewRow);
            Session["ParticipationGridData"] = dtGridData;
            gvParicipantDetails.DataSource = dtGridData;
            gvParicipantDetails.DataBind();

        }

        private void ClearAddRow()
        {
            txtRoyaltorInsert.Text = string.Empty;
            ddlOptionPeriodInsert.SelectedIndex = 0;
            txtTerritoryAddRow.Text = string.Empty;
            txtTimeInsert.Text = string.Empty;
            txtTotalTimeInsert.Text = string.Empty;
            txtTrackNoInsert.Text = string.Empty;
            txtTrackTitleInsert.Text = string.Empty;
            ddlEscCodeInsert.SelectedIndex = 0;
            txtShareInsert.Text = hdnShareInsert.Value;
            txtTotalShareInsert.Text = hdnTotalShareInsert.Value;

        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["ParticipationGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            CheckBox cbActive;
            CheckBox cbEscIncludeUnits;
            string trackno;
            string territory;

            foreach (GridViewRow gvr in gvParicipantDetails.Rows)
            {
                DataRow drGridRow = dtGridChangedData.NewRow();
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                cbActive = (gvr.FindControl("cbActive") as CheckBox);
                cbEscIncludeUnits = (gvr.FindControl("cbEscIncludeUnits") as CheckBox);
                drGridRow["participation_id"] = (gvr.FindControl("hdnParticipId") as HiddenField).Value;
                drGridRow["royaltor_id"] = (gvr.FindControl("hdnParticipRoyId") as HiddenField).Value;
                drGridRow["royaltor"] = (gvr.FindControl("lblRoyaltor") as Label).Text;

                if ((gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedIndex > 0)
                {
                    drGridRow["option_period_code"] = (gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedValue;
                }
                else
                {
                    drGridRow["option_period_code"] = string.Empty;
                }

                if (territory == string.Empty)
                {
                    drGridRow["seller_group_code"] = wildCardChar;
                    drGridRow["seller_group"] = string.Empty;
                }
                else
                {
                    drGridRow["seller_group_code"] = territory.Substring(0, territory.IndexOf("-") - 1);
                    drGridRow["seller_group"] = territory;
                }

                if (!string.IsNullOrEmpty((gvr.FindControl("txtShare") as TextBox).Text))
                {
                    drGridRow["share_tracks"] = (gvr.FindControl("txtShare") as TextBox).Text;
                }

                if (!string.IsNullOrEmpty((gvr.FindControl("txtTotalShare") as TextBox).Text))
                {
                    drGridRow["share_total_tracks"] = (gvr.FindControl("txtTotalShare") as TextBox).Text;
                }

                drGridRow["share_time"] = (gvr.FindControl("txtTime") as TextBox).Text;
                drGridRow["share_total_time"] = (gvr.FindControl("txtTotalTime") as TextBox).Text;

                trackno = (gvr.FindControl("txtTrackNo") as TextBox).Text;

                if (!string.IsNullOrWhiteSpace(trackno))
                {
                    drGridRow["track_no"] = (gvr.FindControl("txtTrackNo") as TextBox).Text;
                }
                //JIRA-1074 Changes -- Start
                if (!string.IsNullOrWhiteSpace((gvr.FindControl("txtTrackTitle") as TextBox).Text))
                {
                    drGridRow["track_title"] = (gvr.FindControl("txtTrackTitle") as TextBox).Text;
                }
                //JIRA-1074 Changes -- End
                if ((gvr.FindControl("ddlEscalationCode") as DropDownList).SelectedIndex > 0)
                {
                    drGridRow["esc_code"] = (gvr.FindControl("ddlEscalationCode") as DropDownList).SelectedValue;
                }
                else
                {
                    drGridRow["esc_code"] = string.Empty;
                }

                drGridRow["inc_in_escalation"] = cbEscIncludeUnits.Checked ? "Y" : "N";
                drGridRow["participation_type"] = cbActive.Checked ? "A" : "I";
                drGridRow["status_code"] = (gvr.FindControl("ddlStatus") as DropDownList).SelectedValue;

                if ((gvr.FindControl("hdnEndDate") as HiddenField).Value == string.Empty)
                {
                    drGridRow["end_date"] = DBNull.Value;
                }

                drGridRow["isrc_val"] = (gvr.FindControl("hdnISRCVal") as HiddenField).Value;
                if ((gvr.FindControl("hdnTrackListingId") as HiddenField).Value != string.Empty)
                {
                    drGridRow["tracklisting_id"] = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;
                }
                else
                {
                    drGridRow["tracklisting_id"] = DBNull.Value;
                }

                drGridRow["is_isrc_track_title"] = (gvr.FindControl("hdnIsISRCTrackTitle") as HiddenField).Value;
                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["ParticipationGridData"] = dtGridChangedData;

        }
        /* - WUIN-1167 Commenting below code as we are updating catno status to minimum of participant statuses
        private Boolean ValidateCatStatusCode()
        {
            Int32 catStatusCode = Convert.ToInt32(ddlCatStatus.SelectedValue);
            string statusCode;
            string isModified;
            int activeParticipantCount = 0;
            int statusCode1 = 0;
            int statusCode2 = 0;
            int statusCode3 = 0;
            int newrowCount = 0;
            foreach (GridViewRow gvr in gvParicipantDetails.Rows)
            {
                if ((gvr.FindControl("cbActive") as CheckBox).Checked)
                {
                    activeParticipantCount++;
                    statusCode = (gvr.FindControl("ddlStatus") as DropDownList).SelectedValue;
                    isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                    if (statusCode == "1")
                    {
                        statusCode1++;
                    }
                    else if (statusCode == "2")
                    {
                        statusCode2++;
                    }
                    else if (statusCode == "3")
                    {
                        statusCode3++;
                    }

                    if (isModified == Global.DBNullParamValue)
                    {
                        newrowCount++;
                    }
                }

            }

            //if (activeParticipantCount == 0 && catStatusCode == 0)
            //{
            //    return true;
            //} 
            if (activeParticipantCount == 0) //If no active participants then status of catalogue will be updated automatically
            {
                return true;
            }
            else if (activeParticipantCount == newrowCount) //if all the active paritipants are new participants then status of catalogue will be updated automatically
            {
                return true;
            }
            else if (activeParticipantCount > 0 && statusCode1 > 0 && catStatusCode == 1)
            {
                return true;
            }
            else if (activeParticipantCount > 0 && statusCode2 > 0 && catStatusCode > 0 && catStatusCode < 3)
            {
                return true;
            }
            else if (activeParticipantCount > 0 && statusCode3 == activeParticipantCount && catStatusCode > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        */

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
            string statusCode;
            string previousStatusCode;
            string isrcVal;
            string trackListingId;
            string isISRCTrackTitle;
            string isModified;


            foreach (GridViewRow gvr in gvParicipantDetails.Rows)
            {

                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                isrcVal = (gvr.FindControl("hdnISRCVal") as HiddenField).Value;
                trackListingId = (gvr.FindControl("hdnTrackListingId") as HiddenField).Value;
                isISRCTrackTitle = (gvr.FindControl("hdnIsISRCTrackTitle") as HiddenField).Value;

                if (isModified == Global.DBNullParamValue)//new row
                {

                    participId = (gvr.FindControl("hdnParticipId") as HiddenField).Value;
                    royaltorId = (gvr.FindControl("hdnParticipRoyId") as HiddenField).Value;

                    if ((gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedIndex > 0)
                    {
                        optionPeriodCode = (gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedValue;
                    }
                    else
                    {
                        optionPeriodCode = string.Empty;
                    }

                    sellerGroupCode = (gvr.FindControl("txtTerritory") as TextBox).Text;
                    if (sellerGroupCode == string.Empty)
                    {
                        sellerGroupCode = wildCardChar;
                    }
                    else
                    {
                        sellerGroupCode = sellerGroupCode.Substring(0, sellerGroupCode.IndexOf("-") - 1);
                    }

                    shareTracks = (gvr.FindControl("txtShare") as TextBox).Text;
                    shareTotalTracks = (gvr.FindControl("txtTotalShare") as TextBox).Text;
                    shareTime = (gvr.FindControl("txtTime") as TextBox).Text.Replace(":", "");
                    shareTotalTime = (gvr.FindControl("txtTotalTime") as TextBox).Text.Replace(":", "");

                    if (lblTimeTrackShare.Text == "Track")
                    {
                        if (shareTracks == shareTotalTracks)
                        {
                            shareTracks = "1";
                            shareTotalTracks = "1";
                        }
                        shareTime = "0000000";
                        shareTotalTime = "0000000";
                    }
                    else
                    {
                        shareTracks = "0";
                        shareTotalTracks = "0";
                    }

                    tuneCode = (gvr.FindControl("txtTrackNo") as TextBox).Text;
                    tuneTitle = (gvr.FindControl("txtTrackTitle") as TextBox).Text;
                    escCode = (gvr.FindControl("ddlEscalationCode") as DropDownList).Text;
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

                    if ((gvr.FindControl("cbActive") as CheckBox).Checked)
                    {
                        participationType = "A";
                    }
                    else
                    {
                        participationType = "I";
                    }

                    statusCode = (gvr.FindControl("ddlStatus") as DropDownList).SelectedValue;

                    modifiedRowsList.Add(participId + Global.DBDelimiter + royaltorId + Global.DBDelimiter + optionPeriodCode + Global.DBDelimiter + sellerGroupCode + Global.DBDelimiter
                        + shareTracks + Global.DBDelimiter + shareTotalTracks + Global.DBDelimiter + shareTime + Global.DBDelimiter + shareTotalTime + Global.DBDelimiter + tuneCode + Global.DBDelimiter +
                        tuneTitle + Global.DBDelimiter + escCode + Global.DBDelimiter + escIncludeUnits + Global.DBDelimiter + participationType + Global.DBDelimiter + statusCode + Global.DBDelimiter +
                                        isModified + Global.DBDelimiter + isrcVal + Global.DBDelimiter + trackListingId + Global.DBDelimiter + isISRCTrackTitle);
                }
                else if (isModified == "Y")
                {

                    participId = (gvr.FindControl("hdnParticipId") as HiddenField).Value;
                    royaltorId = (gvr.FindControl("hdnParticipRoyId") as HiddenField).Value;

                    if ((gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedIndex > 0)
                    {
                        optionPeriodCode = (gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedValue;
                    }
                    else
                    {
                        optionPeriodCode = string.Empty;
                    }

                    sellerGroupCode = (gvr.FindControl("txtTerritory") as TextBox).Text;
                    if (sellerGroupCode == string.Empty)
                    {
                        sellerGroupCode = wildCardChar;
                    }
                    else
                    {
                        sellerGroupCode = sellerGroupCode.Substring(0, sellerGroupCode.IndexOf("-") - 1);
                    }

                    shareTracks = (gvr.FindControl("txtShare") as TextBox).Text;
                    shareTotalTracks = (gvr.FindControl("txtTotalShare") as TextBox).Text;
                    shareTime = (gvr.FindControl("txtTime") as TextBox).Text.Replace(":", "");
                    shareTotalTime = (gvr.FindControl("txtTotalTime") as TextBox).Text.Replace(":", "");

                    if (lblTimeTrackShare.Text == "Track")
                    {
                        if (shareTracks == shareTotalTracks)
                        {
                            shareTracks = "1";
                            shareTotalTracks = "1";
                        }
                        shareTime = "0000000";
                        shareTotalTime = "0000000";
                    }
                    else
                    {
                        shareTracks = "0";
                        shareTotalTracks = "0";
                    }

                    tuneCode = (gvr.FindControl("txtTrackNo") as TextBox).Text;
                    tuneTitle = (gvr.FindControl("txtTrackTitle") as TextBox).Text;
                    escCode = (gvr.FindControl("ddlEscalationCode") as DropDownList).Text;
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

                    if ((gvr.FindControl("cbActive") as CheckBox).Checked)
                    {
                        participationType = "A";
                    }
                    else
                    {
                        participationType = "I";
                    }

                    //WUIN-1167 Any changes made to the partifipant which is at manager sign off/team sign off should move back to under review irrespective of the user role

                    previousStatusCode = (gvr.FindControl("hdnParticipStatus") as HiddenField).Value;
                    statusCode = (gvr.FindControl("ddlStatus") as DropDownList).SelectedValue;
                    if (previousStatusCode == statusCode && (previousStatusCode == "2" || previousStatusCode == "3"))
                    {
                        statusCode = "1";
                    }

                    modifiedRowsList.Add(participId + Global.DBDelimiter + royaltorId + Global.DBDelimiter + optionPeriodCode + Global.DBDelimiter + sellerGroupCode + Global.DBDelimiter
                        + shareTracks + Global.DBDelimiter + shareTotalTracks + Global.DBDelimiter + shareTime + Global.DBDelimiter + shareTotalTime + Global.DBDelimiter + tuneCode + Global.DBDelimiter +
                        tuneTitle + Global.DBDelimiter + escCode + Global.DBDelimiter + escIncludeUnits + Global.DBDelimiter + participationType + Global.DBDelimiter + statusCode + Global.DBDelimiter +
                                         isModified + Global.DBDelimiter + isrcVal + Global.DBDelimiter + trackListingId + Global.DBDelimiter + isISRCTrackTitle);
                }


            }

            return modifiedRowsList.ToArray();
        }

        private void UserAuthorization()
        {
            //Validation: only Super user can access this page
            if (Session["UserRole"].ToString().ToLower() == UserRole.SuperUser.ToString().ToLower())
            {
                hdnIsSuperUser.Value = "Y";
            }
            //JIRA-983 Changes by Ravi on 26/02/2019 -- Start
            //Validation: Getting Supervisor access this page
            else if (Session["UserRole"].ToString().ToLower() == UserRole.Supervisor.ToString().ToLower())
            {
                hdnIsSupervisor.Value = "Y";
            }
            //JIRA-983 Changes by Ravi on 26/02/2019 -- End
            //WUIN-1096 Only Read access for Reaonly User
            else if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSaveChanges.Enabled = false;
                imgBtnCancel.Enabled = false;
                imgBtnInsert.Enabled = false;
                btnYes.Enabled = false;
                foreach (GridViewRow rows in gvParicipantDetails.Rows)
                {
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;

                }
            }

        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        #endregion Methods

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

        protected void btnHdnRoyaltorInsertSearch_Click(object sender, EventArgs e)
        {
            try
            {
                txtTerritoryAddRow.Text = string.Empty;
                txtTimeInsert.Text = string.Empty;
                txtTotalTimeInsert.Text = string.Empty;
                txtTrackNoInsert.Text = string.Empty;
                txtTrackTitleInsert.Text = string.Empty;
                txtTrackTitleInsert.Attributes.Remove("readonly");
                ddlEscCodeInsert.SelectedIndex = 0;
                txtShareInsert.Text = hdnShareInsert.Value;
                txtTotalShareInsert.Text = hdnTotalShareInsert.Value;
                PopulateOptPrdInsert();
                PopulateEscDodeInsert();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from list.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Territory")
                {
                    TextBox txtTerritory;
                    foreach (GridViewRow gvr in gvParicipantDetails.Rows)
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

                    ScriptManager.RegisterStartupScript(this, typeof(Page), "CompareGrid", "CompareGridData(" + Convert.ToInt32(hdnGridFuzzySearchRowId.Value) + ");", true);
                }
                else if (hdnFuzzySearchField.Value == "RoyaltorAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtRoyaltorInsert.Text = string.Empty;
                        return;
                    }

                    txtRoyaltorInsert.Text = lbFuzzySearch.SelectedValue.ToString();
                    PopulateOptPrdInsert();
                    PopulateEscDodeInsert();
                }
                else if (hdnFuzzySearchField.Value == "TerritoryAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtTerritoryAddRow.Text = string.Empty;
                        return;
                    }

                    txtTerritoryAddRow.Text = lbFuzzySearch.SelectedValue.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting item from list", ex.Message);
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
                    PopulateEscDodeInsert();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing full search popup", ex.Message);
            }
        }

        #endregion Fuzzy Search

        //JIRA-1074 Changes -- Start
        protected void txtTrackNo_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                string seqNo = hdnTrackNoChange.Value;
                string trackTitle = string.Empty;
                string isrc = string.Empty;
                string trackListingId = string.Empty;
                hdnFocusControl.Value = "";

                if (hdnTrackNoRow.Value != "")
                {
                    int rowIndex = Convert.ToInt16(hdnTrackNoRow.Value);
                    int share = Convert.ToInt16((gvParicipantDetails.Rows[rowIndex].FindControl("txtShare") as TextBox).Text);
                    int totalShare = Convert.ToInt16((gvParicipantDetails.Rows[rowIndex].FindControl("txtTotalShare") as TextBox).Text);
                    (gvParicipantDetails.Rows[rowIndex].FindControl("hdnTrackNoTemp") as HiddenField).Value = "";

                    if (seqNo != "" && share == 1 && totalShare > 1)
                    {
                        participantMaintenanceBL = new ParticipantMaintenanceBL();
                        participantMaintenanceBL.GetTrackTitlefromISRC(lblCatNo.Text, seqNo, out errorId, out trackTitle, out isrc, out trackListingId);
                        participantMaintenanceBL = null;
                        if (errorId != 1 && trackTitle == "null")
                        {
                            (gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackTitle") as TextBox).Text = "";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackNo") as TextBox).Text = "";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackTitle") as TextBox).Attributes.Add("readonly", "readOnly"); ;
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnIsModified") as HiddenField).Value = "Y";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnISRCVal") as HiddenField).Value = "";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnTrackListingId") as HiddenField).Value = "";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnIsISRCTrackTitle") as HiddenField).Value = "N";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnTrackNoTemp") as HiddenField).Value = hdnTrackNoChange.Value;
                            hdnFocusControl.Value = "txtTrackNo";
                            mpeShowNoTrackMsg.Show();
                        }
                        else if (errorId == 1 && trackTitle == "null")
                        {
                            (gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackTitle") as TextBox).Attributes.Remove("readonly");
                            (gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackTitle") as TextBox).Text = "";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnIsModified") as HiddenField).Value = "Y";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackTitle") as TextBox).Focus();
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnISRCVal") as HiddenField).Value = "";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnTrackListingId") as HiddenField).Value = "";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnIsISRCTrackTitle") as HiddenField).Value = "N";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnTrackNoTemp") as HiddenField).Value = hdnTrackNoChange.Value;
                            hdnTrackNoChange.Value = "";

                        }
                        else if (errorId == 0 && trackTitle != "")
                        {
                            (gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackTitle") as TextBox).Text = trackTitle;
                            (gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackTitle") as TextBox).Attributes.Add("readonly", "readOnly");
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnIsModified") as HiddenField).Value = "Y";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnISRCVal") as HiddenField).Value = isrc;
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnTrackListingId") as HiddenField).Value = trackListingId;
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnIsISRCTrackTitle") as HiddenField).Value = "Y";
                            (gvParicipantDetails.Rows[rowIndex].FindControl("ddlEscalationCode") as DropDownList).Focus();
                            Page.SetFocus(gvParicipantDetails.Rows[rowIndex].FindControl("txtTrackTitle") as TextBox);
                            (gvParicipantDetails.Rows[rowIndex].FindControl("hdnTrackNoTemp") as HiddenField).Value = hdnTrackNoChange.Value;
                            hdnTrackNoChange.Value = "";

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching Track Title from ISRC for given Track No", ex.Message);
            }
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
                    string trackListingId = string.Empty;
                    hdnFocusControl.Value = "";

                    participantMaintenanceBL = new ParticipantMaintenanceBL();
                    participantMaintenanceBL.GetTrackTitlefromISRC(lblCatNo.Text, seqNo, out errorId, out trackTitle, out isrc, out trackListingId);
                    participantMaintenanceBL = null;

                    if (errorId != 1 && trackTitle == "null")
                    {
                        txtTrackTitleInsert.Text = "";
                        txtTrackNoInsert.Text = "";
                        txtTrackTitleInsert.Attributes.Add("readonly", "readOnly");
                        hdnISRCValInsert.Value = "";
                        hdnTrackListingIdInsert.Value = "";
                        hdnIsISRCTrackTitleInsert.Value = "N";
                        hdnFocusControl.Value = "txtTrackNoInsert";
                        mpeShowNoTrackMsg.Show();
                    }
                    else if (errorId == 1 && trackTitle == "null")
                    {
                        txtTrackTitleInsert.Attributes.Remove("readonly");
                        txtTrackTitleInsert.Text = "";
                        hdnISRCValInsert.Value = "";
                        hdnTrackListingIdInsert.Value = "";
                        hdnIsISRCTrackTitleInsert.Value = "N";
                        txtTrackTitleInsert.Focus();
                    }
                    else if (errorId == 0 && trackTitle != "")
                    {
                        txtTrackTitleInsert.Text = trackTitle;
                        txtTrackTitleInsert.Attributes.Add("readonly", "readOnly");
                        hdnISRCValInsert.Value = isrc;
                        hdnTrackListingIdInsert.Value = trackListingId;
                        hdnIsISRCTrackTitleInsert.Value = "Y";
                        txtTrackTitleInsert.Focus();

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching Track Title from ISRC for given Track No", ex.Message);
            }
        }
        //JIRA-1074 Changes-- End

    }
}