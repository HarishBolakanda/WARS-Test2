/*
File Name   :   AutoParticipantMaintenance.cs
Purpose     :   To maintain Auto Participant details

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     17-Apr-2019     Ravi Mulugu(Infosys Limited)   Initial Creation
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using WARS.BusinessLayer;
using System.Net;

namespace WARS.Participants
{
    public partial class AutoParticipantMaintenance : System.Web.UI.Page
    {
        #region Global Declarations
        AutoParticipantMaintenanceBL autoParticipantMaintenanceBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string autoParticipantId = string.Empty;
        DataSet dsAutoParticipantDropdownData;
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
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Auto Participant Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Auto Participant Maintenance";
                }
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        
                        autoParticipantId = Request.QueryString["autoPartId"];
                        Session["autoParticipantId"] = autoParticipantId;
                        hdnAutoPartId.Value = autoParticipantId;
                        if (autoParticipantId != "")
                        {
                            LoadInitialData(autoParticipantId);
                        }
                        else
                        {
                            ExceptionHandler("Sorry! Invalid Auto Participant Id", string.Empty);
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

        protected void gvAutoParticipantDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Session["AutoParticipantData"] == null)
                {
                    return;
                }

                DropDownList ddlOptionPeriod;
                DropDownList ddlEscalationCode;
                CheckBox cbActive;
                CheckBox cbEscIncludeUnits;
                string royaltorId;
                string territory;
                string optionPrdCode;
                string escalationCode;
                string isActive;
                string escIncludeUnits;
                DataTable optionPrdList;
                DataTable escalationCodeList;
                //get option period list for the royaltor
                dsAutoParticipantDropdownData = (DataSet)Session["AutoParticipantData"];

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlOptionPeriod = (e.Row.FindControl("ddlOptionPeriod") as DropDownList);
                    ddlEscalationCode = (e.Row.FindControl("ddlEscalationCode") as DropDownList);
                    cbActive = (e.Row.FindControl("cbActive") as CheckBox);
                    cbEscIncludeUnits = (e.Row.FindControl("cbEscIncludeUnits") as CheckBox);

                    royaltorId = (e.Row.FindControl("hdnAutoParticipantRoyId") as HiddenField).Value;
                    optionPrdCode = (e.Row.FindControl("hdnAutoParticipOptionPeriod") as HiddenField).Value;
                    escalationCode = (e.Row.FindControl("hdnEscalationCode") as HiddenField).Value;
                    territory = (e.Row.FindControl("hdnSellerGrp") as HiddenField).Value;                    
                    royaltorId = royaltorId.Split('-')[0].ToString().Trim();

                    if (royaltorId != string.Empty)
                    {
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
                        dsAutoParticipantDropdownData = (DataSet)Session["AutoParticipantData"];
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

                    isActive = (e.Row.FindControl("hdnAutoParticipantActive") as HiddenField).Value;

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

                    //if share=1 and totalshare>1 then enable trackno and tracktitle else disable
                    if (((e.Row.FindControl("txtShare") as TextBox).Text.Trim() == "1" && Convert.ToInt32((e.Row.FindControl("txtTotalShare") as TextBox).Text.Trim()) > 1)
                        && (((((e.Row.FindControl("txtTime") as TextBox).Text.Trim() == "_______") || ((e.Row.FindControl("txtTime") as TextBox).Text.Trim() == "___:__:__")) || ((e.Row.FindControl("txtTime") as TextBox).Text.Trim() == ""))
                        && ((((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == "_______")) || ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == "___:__:__") || ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == ""))))
                    {
                        (e.Row.FindControl("txtTrackNo") as TextBox).Enabled = true;
                        (e.Row.FindControl("txtTrackTitle") as TextBox).Enabled = true;
                        (e.Row.FindControl("txtTime") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtTotalTime") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtShare") as TextBox).Enabled = true;
                        (e.Row.FindControl("txtTotalShare") as TextBox).Enabled = true;
                    }
                    else if ((((e.Row.FindControl("txtShare") as TextBox).Text.Trim() == "1") && ((e.Row.FindControl("txtTotalShare") as TextBox).Text.Trim() == "1"))
                            && ((((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == "") && ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == "")
                            || ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == "_______") && ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == "_______"))
                            || ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == "___:__:__") && ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() == "___:__:__")))
                    {
                        (e.Row.FindControl("txtTime") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtTotalTime") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtTrackNo") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtTrackTitle") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtShare") as TextBox).Enabled = true;
                        (e.Row.FindControl("txtTotalShare") as TextBox).Enabled = true;
                    }
                    else if ((((e.Row.FindControl("txtShare") as TextBox).Text.Trim() == "") && ((e.Row.FindControl("txtTotalShare") as TextBox).Text.Trim() == ""))
                        && (((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() != "") && ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() != "")
                       || ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() != "_______") && ((e.Row.FindControl("txtTotalTime") as TextBox).Text.Trim() != "_______")))
                    {
                        (e.Row.FindControl("txtTime") as TextBox).Enabled = true;
                        (e.Row.FindControl("txtTotalTime") as TextBox).Enabled = true;
                        (e.Row.FindControl("txtTrackNo") as TextBox).Enabled = true;
                        (e.Row.FindControl("txtTrackTitle") as TextBox).Enabled = true;
                        (e.Row.FindControl("txtShare") as TextBox).Enabled = false;
                        (e.Row.FindControl("txtTotalShare") as TextBox).Enabled = false;
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
        protected void gvAutoParticipantDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["AutoParticipationGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvAutoParticipantDetails.DataSource = dataView;
                gvAutoParticipantDetails.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        protected void gvAutoParticipantDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
               
                if (e.CommandName == "cancelRow")
                {
                    GridViewRow gvr = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    string autoParticipationId = (gvr.FindControl("hdnAutoParticipantDetailId") as HiddenField).Value;
                    string isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                    DropDownList ddlOptionPeriod;
                    DropDownList ddlEscalationCode;
                    CheckBox cbActive;
                    CheckBox cbEscIncludeUnits;

                    string royaltorId;
                    string optionPrdCode;
                    string escalationCode;
                    string isActive;
                    string escIncludeUnits;
                    DataTable optionPrdList;

                    ddlOptionPeriod = (gvr.FindControl("ddlOptionPeriod") as DropDownList);
                    ddlEscalationCode = (gvr.FindControl("ddlEscalationCode") as DropDownList);
                    cbActive = (gvr.FindControl("cbActive") as CheckBox);
                    cbEscIncludeUnits = (gvr.FindControl("cbEscIncludeUnits") as CheckBox);
                    royaltorId = (gvr.FindControl("hdnAutoParticipantRoyId") as HiddenField).Value;
                    optionPrdCode = (gvr.FindControl("hdnAutoParticipOptionPeriod") as HiddenField).Value;

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

                    isActive = (gvr.FindControl("hdnAutoParticipantActive") as HiddenField).Value;
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
                    (gvr.FindControl("txtTrackNo") as TextBox).Text = (gvr.FindControl("hdnTrackNo") as HiddenField).Value;
                    (gvr.FindControl("txtTrackTitle") as TextBox).Text = (gvr.FindControl("hdnTrackTitle") as HiddenField).Value;

                    if (Session["AutoParticipantData"] == null)
                    {
                        return;
                    }

                    ddlOptionPeriod.Items.Clear();
                    royaltorId = royaltorId.Split('-')[0].ToString().Trim();
                    if (royaltorId != string.Empty)
                    {
                        //get option period list for the royaltor
                        dsAutoParticipantDropdownData = (DataSet)Session["AutoParticipantData"];
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
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in cancelling grid data", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Page.Validate("valGrpAppendAddRow");
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
                string userRole = Session["UserCode"].ToString();
                Array modifiedRowList = ModifiedRowsList();
                autoParticipantId = Convert.ToString(Session["autoParticipantId"]);
                //validate 
                Page.Validate("valUpdate");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Participant details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //check if any changes to save
                if (modifiedRowList.Length == 0)
                {
                    msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                autoParticipantMaintenanceBL = new AutoParticipantMaintenanceBL();
                DataSet savedData = autoParticipantMaintenanceBL.SaveAutoParticipantDetails(autoParticipantId, modifiedRowList, userRole, Convert.ToString(Session["UserRoleId"]), out errorId); //JIRA-898 Changes
                autoParticipantMaintenanceBL = null;

                hdnGridDataChanged.Value = "N";

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (errorId == 0)
                {
                    if (savedData.Tables.Count != 0)
                    {
                        Session["AutoParticipationGridData"] = savedData.Tables[1];
                        gvAutoParticipantDetails.DataSource = savedData.Tables[1];
                        gvAutoParticipantDetails.DataBind();

                        if (savedData.Tables[1].Rows.Count == 0)
                        {
                            gvAutoParticipantDetails.EmptyDataText = "No data found for the selected Auto Participant";
                        }
                        else
                        {
                            gvAutoParticipantDetails.EmptyDataText = string.Empty;
                        }
                    }
                    else if (savedData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvAutoParticipantDetails.DataSource = dtEmpty;
                        gvAutoParticipantDetails.EmptyDataText = "No data found for the selected  Auto Participant";
                        gvAutoParticipantDetails.DataBind();
                    }

                    msgView.SetMessage("Auto Participant details saved.", MessageType.Warning, PositionType.Auto);
                    hdnAddRowDataChanged.Value = "N";
                }
                else if (errorId == 3)
                {
                    msgView.SetMessage("Failed to save changes! Duplicate rows exists for same royaltor, option period, seller group code, esc code and Inc in Escalation combination", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving Auto participant data", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving Auto participant data", string.Empty);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving Auto participant data", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadInitialData(string autoParticipantId)
        {
            autoParticipantMaintenanceBL = new AutoParticipantMaintenanceBL();
            DataSet initialData = autoParticipantMaintenanceBL.GetDropDownData(autoParticipantId, Convert.ToString(Session["UserRoleId"]), out errorId); //JIRA-898 Change
            autoParticipantMaintenanceBL = null;

            if (errorId == 2)
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
                return;
            }

            else if (errorId != 2 && initialData.Tables[4] != null)
            {
                lblMarketingOwner.Text = initialData.Tables[4].Rows[0]["marketing_owner_code"].ToString();
                lblArtist.Text = initialData.Tables[4].Rows[0]["artist_id"].ToString();
                txtProjectTitle.Text = initialData.Tables[4].Rows[0]["project_title"].ToString();
                lblWEASales.Text = initialData.Tables[4].Rows[0]["wea_sales_label_code"].ToString();
                Session["AutoParticipantData"] = initialData;
                Session["AutoParticipationGridData"] = initialData.Tables[5];
                dsAutoParticipantDropdownData = initialData;
                LoadDropDownData();
                BindGridData(initialData.Tables[5]);
            }
        }

        private void LoadDropDownData()
        {
            if (errorId == 2)
            {
                ExceptionHandler("Error in loading Auto Participant DropDown data", string.Empty);
                return;
            }
            dsAutoParticipantDropdownData.Tables[0].TableName = "dtOptionPrdList";
            dsAutoParticipantDropdownData.Tables[1].TableName = "dtEscCodeList";
            Session["ParticipMaintSellerGrpList"] = dsAutoParticipantDropdownData.Tables[2];
            Session["ParticipMaintTrackTitleList"] = dsAutoParticipantDropdownData.Tables[3];
        }

        private void BindGridData(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (gridData.Rows.Count > 0)
            {
                gvAutoParticipantDetails.DataSource = gridData;
                gvAutoParticipantDetails.DataBind();
            }
            else if (gridData.Rows.Count == 0)
            {
                LoadEmptyGrid();
            }
            else
            {
                ExceptionHandler("Error in loading Auto Participant grid data", string.Empty);
            }
        }

        private void LoadEmptyGrid()
        {
            dtEmpty = new DataTable();
            gvAutoParticipantDetails.DataSource = dtEmpty;
            gvAutoParticipantDetails.EmptyDataText = "No data found for the selected Auto Participant";
            gvAutoParticipantDetails.DataBind();
        }

        private DataTable GetOptionPeriodList(string royaltorId)
        {
            DataTable optionPrdList = new DataTable();
            optionPrdList = null;

            if (dsAutoParticipantDropdownData.Tables["dtOptionPrdList"] != null)
            {
                DataRow[] optionPrdExist = dsAutoParticipantDropdownData.Tables["dtOptionPrdList"].Select("royaltor_id='" + royaltorId + "'");
                if (optionPrdExist.Count() != 0)
                {
                    optionPrdList = dsAutoParticipantDropdownData.Tables["dtOptionPrdList"].Select("royaltor_id='" + royaltorId + "'").CopyToDataTable();
                }

            }
            return optionPrdList;
        }

        private DataTable GetEscCodeList(string royaltorId)
        {
            DataTable escCodeList = new DataTable();
            escCodeList = null;

            if (dsAutoParticipantDropdownData.Tables["dtEscCodeList"] != null)
            {
                DataRow[] optionPrdExist = dsAutoParticipantDropdownData.Tables["dtEscCodeList"].Select("royaltor_id='" + royaltorId + "'");
                if (optionPrdExist.Count() != 0)
                {
                    escCodeList = dsAutoParticipantDropdownData.Tables["dtEscCodeList"].Select("royaltor_id='" + royaltorId + "'").CopyToDataTable();
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
                dsAutoParticipantDropdownData = (DataSet)Session["AutoParticipantData"];
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
                dsAutoParticipantDropdownData = (DataSet)Session["AutoParticipantData"];
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

            if (Session["AutoParticipationGridData"] == null)
            {
                ExceptionHandler("Error in adding rate row to grid", string.Empty);
            }

            DataTable dtGridData = (DataTable)Session["AutoParticipationGridData"];
            DataRow drNewRow = dtGridData.NewRow();

            drNewRow["auto_participant_detail_id"] = Convert.ToInt32(hdnNewAutoParticipId.Value);
            hdnNewAutoParticipId.Value = (Convert.ToInt32(hdnNewAutoParticipId.Value) - 1).ToString();

            if (!string.IsNullOrWhiteSpace(txtRoyaltorInsert.Text))
            {
                drNewRow["royaltor_id"] = txtRoyaltorInsert.Text;
            }

            drNewRow["option_period_code"] = ddlOptionPeriodInsert.SelectedValue;

            drNewRow["seller_group"] = txtTerritoryAddRow.Text;

            if ((!string.IsNullOrWhiteSpace(txtShareInsert.Text)) && ((txtTimeInsert.Text != "___:__:__") || (txtTotalTimeInsert.Text == "___:__:__")))
            {
                drNewRow["share_tracks"] = txtShareInsert.Text.Trim();
            }

            if ((!string.IsNullOrWhiteSpace(txtTotalShareInsert.Text)) && ((txtTimeInsert.Text != "___:__:__") || (txtTotalTimeInsert.Text == "___:__:__")))
            {
                drNewRow["share_total_tracks"] = txtTotalShareInsert.Text.Trim();
            }

            if ((txtTimeInsert.Text == "___:__:__") && (txtShareInsert.Text.Trim() == "1" && txtTotalShareInsert.Text.Trim() == "1"))
            {
                drNewRow["share_time"] = "";
            }
            else
            {
                drNewRow["share_time"] = txtTimeInsert.Text.Trim().Replace(":", "");
            }

            if (txtTotalTimeInsert.Text == "___:__:__" && (txtShareInsert.Text.Trim() == "1" && txtTotalShareInsert.Text.Trim() == "1"))
            {
                drNewRow["share_total_time"] = "";
            }
            else
            {
                drNewRow["share_total_time"] = txtTotalTimeInsert.Text.Trim().Replace(":", "");
            }

            if (!string.IsNullOrWhiteSpace(txtTrackNoInsert.Text))
            {
                drNewRow["tune_code"] = txtTrackNoInsert.Text.Trim();
            }

            drNewRow["tune_title"] = txtTrackTitleInsert.Text.Trim();

            if (ddlEscCodeInsert.SelectedIndex > 0)
            {
                drNewRow["esc_code"] = ddlEscCodeInsert.SelectedValue;
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

            drNewRow["is_modified"] = Global.DBNullParamValue;

            dtGridData.Rows.Add(drNewRow);
            Session["AutoParticipationGridData"] = dtGridData;
            gvAutoParticipantDetails.DataSource = dtGridData;
            gvAutoParticipantDetails.DataBind();

        }

        private void ClearAddRow()
        {
            txtRoyaltorInsert.Text = string.Empty;
            ddlOptionPeriodInsert.Items.Clear();
            txtTerritoryAddRow.Text = string.Empty;
            txtTimeInsert.Text = string.Empty;
            txtTotalTimeInsert.Text = string.Empty;
            txtTrackNoInsert.Text = string.Empty;
            txtTrackTitleInsert.Text = string.Empty;
            ddlEscCodeInsert.Items.Clear();
            txtShareInsert.Text = "1";
            txtTotalShareInsert.Text = "1";
        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["AutoParticipationGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            CheckBox cbActive;
            CheckBox cbEscIncludeUnits;
            string trackno;
            string territory;
            string autoParticipantDetailId;

            foreach (GridViewRow gvr in gvAutoParticipantDetails.Rows)
            {
                DataRow drGridRow = dtGridChangedData.NewRow();
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                cbActive = (gvr.FindControl("cbActive") as CheckBox);
                cbEscIncludeUnits = (gvr.FindControl("cbEscIncludeUnits") as CheckBox);
                autoParticipantDetailId = (gvr.FindControl("hdnAutoParticipantDetailId") as HiddenField).Value;

                if (autoParticipantDetailId == string.Empty)
                {
                    drGridRow["auto_participant_detail_id"] = "";
                }
                else
                {
                    drGridRow["auto_participant_detail_id"] = autoParticipantDetailId;
                }

                drGridRow["royaltor_id"] = (gvr.FindControl("hdnAutoParticipantRoyId") as HiddenField).Value;

                if ((gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedIndex > 0)
                {
                    drGridRow["option_period_code"] = (gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedValue;
                }
                else
                {
                    drGridRow["option_period_code"] = string.Empty;
                }

                if (territory == "")
                {
                    drGridRow["seller_group"] = string.Empty;
                }
                else
                {
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
                    drGridRow["tune_code"] = (gvr.FindControl("txtTrackNo") as TextBox).Text;
                }

                drGridRow["tune_title"] = (gvr.FindControl("txtTrackTitle") as TextBox).Text;

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

                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                dtGridChangedData.Rows.Add(drGridRow);
            }
            Session["AutoParticipationGridData"] = dtGridChangedData;
        }

        private Array ModifiedRowsList()
        {
            List<string> modifiedRowsList = new List<string>();
            string autoParticipntDetailId;
            string royaltorId;
            string optionPeriodCode;
            string sellerGroupCode;
            string shareTracks;
            string shareTotalTracks;
            string shareTime = string.Empty;
            string shareTotalTime = string.Empty;
            string tuneCode;
            string tuneTitle;
            string escCode;
            string escIncludeUnits;
            string participationType;
            Int32 autoParticipantDetailId = 0;
            Int32 royaltorID;
            string isModified;

            foreach (GridViewRow gvr in gvAutoParticipantDetails.Rows)
            {
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                if (isModified == Global.DBNullParamValue)//new row
                {
                    royaltorId = (gvr.FindControl("hdnAutoParticipantRoyId") as HiddenField).Value;

                    royaltorID = Convert.ToInt32(royaltorId.Substring(0, royaltorId.IndexOf("-") - 1));

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
                    if ((gvr.FindControl("txtTime") as TextBox).Text == "___:__:__")
                    {
                        shareTime = "";
                    }
                    else
                    {
                        shareTime = (gvr.FindControl("txtTime") as TextBox).Text.Replace(":", "");
                    }
                    if ((gvr.FindControl("txtTotalTime") as TextBox).Text == "___:__:__")
                    {
                        shareTotalTime = "";
                    }
                    else
                    {
                        shareTotalTime = (gvr.FindControl("txtTotalTime") as TextBox).Text.Replace(":", "");
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
                    modifiedRowsList.Add(0 + Global.DBDelimiter + royaltorID + Global.DBDelimiter + optionPeriodCode + Global.DBDelimiter + sellerGroupCode + Global.DBDelimiter
                                        + shareTracks + Global.DBDelimiter + shareTotalTracks + Global.DBDelimiter + shareTime + Global.DBDelimiter +
                                        shareTotalTime + Global.DBDelimiter + tuneCode + Global.DBDelimiter +
                                          tuneTitle + Global.DBDelimiter + escCode + Global.DBDelimiter + escIncludeUnits + Global.DBDelimiter + participationType);
                }
                else if (isModified == "Y")
                {
                    autoParticipntDetailId = (gvr.FindControl("hdnAutoParticipantDetailId") as HiddenField).Value;
                    royaltorId = (gvr.FindControl("hdnAutoParticipantRoyId") as HiddenField).Value;
                    royaltorID = Convert.ToInt32(royaltorId.Substring(0, royaltorId.IndexOf("-") - 1));

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
                    if ((gvr.FindControl("txtTime") as TextBox).Text == "___:__:__")
                    {
                        shareTime = "";
                    }
                    else
                    {
                        shareTime = (gvr.FindControl("txtTime") as TextBox).Text.Replace(":", "");
                    }
                    if ((gvr.FindControl("txtTotalTime") as TextBox).Text == "___:__:__")
                    {
                        shareTotalTime = "";
                    }
                    else
                    {
                        shareTotalTime = (gvr.FindControl("txtTotalTime") as TextBox).Text.Replace(":", "");
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
                    autoParticipantDetailId = Convert.ToInt32(autoParticipntDetailId);
                    hdnAutoPartDetailId.Value = autoParticipntDetailId.ToString();
                    modifiedRowsList.Add(autoParticipantDetailId + Global.DBDelimiter + royaltorID + Global.DBDelimiter + optionPeriodCode + Global.DBDelimiter + sellerGroupCode + Global.DBDelimiter
                                        + shareTracks + Global.DBDelimiter + shareTotalTracks + Global.DBDelimiter + shareTime + Global.DBDelimiter +
                                        shareTotalTime + Global.DBDelimiter + tuneCode + Global.DBDelimiter +
                                          tuneTitle + Global.DBDelimiter + escCode + Global.DBDelimiter + escIncludeUnits + Global.DBDelimiter + participationType);
                }
            }
            return modifiedRowsList.ToArray();
        }

        private void UserAuthorization()
        {
            string userRole = Session["UserRole"].ToString();
            //Validation: Getting Super user access this page
            if (userRole.ToLower() == UserRole.SuperUser.ToString().ToLower())
            {
                hdnIsSuperUser.Value = "Y";
            }

            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnSaveChanges.Enabled = false;
                foreach (GridViewRow rows in gvAutoParticipantDetails.Rows)
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

        protected void btnFuzzyTrackTitleListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                FuzzySearchTrackTitle(hdnFuzzySearchText.Value);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in track title fuzzy search", ex.Message);
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

        private void FuzzySearchTrackTitle(string searchText)
        {
            lblFuzzySearchPopUp.Text = "Track Title - Complete Search List";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyPartMaintTrackTitleList(searchText.ToUpper(), int.MaxValue);
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
                PopulateOptPrdInsert();
                PopulateEscCodeInsert();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from list.", ex.Message);
            }
        }

        protected void fuzzySearchTrackTitleInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // To keep Enabled the Tracktitle and TrackNo fields when Track title fuzzy search popup is clicked
                if (hdnTrackTitleInsertEnabled.Value == "Y")
                {
                    txtTrackNoInsert.Enabled = true;
                    txtTrackTitleInsert.Enabled = true;
                }
                else
                {
                    txtTrackNoInsert.Enabled = false;
                    txtTrackTitleInsert.Enabled = false;
                }

                hdnFuzzySearchField.Value = "TrackTitleInsert";
                if (txtTrackTitleInsert.Text == string.Empty)
                {
                    msgView.SetMessage("Please enter a text in track title search field", MessageType.Warning, PositionType.Auto);
                    return;
                }

                FuzzySearchTrackTitle(txtTrackTitleInsert.Text);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in track title fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "TrackTitle")
                {
                    TextBox txtTrackTitle;
                    foreach (GridViewRow gvr in gvAutoParticipantDetails.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnGridFuzzySearchRowId.Value)
                        {
                            txtTrackTitle = (gvr.FindControl("txtTrackTitle") as TextBox);
                            if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                            {
                                txtTrackTitle.Text = string.Empty;
                                txtTrackTitle.ToolTip = string.Empty;
                                return;
                            }

                            txtTrackTitle.Text = lbFuzzySearch.SelectedValue.ToString();
                            txtTrackTitle.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                            break;
                        }
                    }

                    ScriptManager.RegisterStartupScript(this, typeof(Page), "CompareGrid", "CompareGridData(" + Convert.ToInt32(hdnGridFuzzySearchRowId.Value) + ");", true);

                }
                else if (hdnFuzzySearchField.Value == "Territory")
                {
                    TextBox txtTerritory;
                    foreach (GridViewRow gvr in gvAutoParticipantDetails.Rows)
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
                    PopulateEscCodeInsert();
                }
                else if (hdnFuzzySearchField.Value == "TrackTitleAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtTrackTitleInsert.Text = string.Empty;
                        return;
                    }

                    txtTrackTitleInsert.Text = lbFuzzySearch.SelectedValue.ToString();
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
                if (hdnFuzzySearchField.Value == "TrackTitle")
                {
                    TextBox txtTrackTitle;

                    GridViewRow gvr = gvAutoParticipantDetails.Rows[Convert.ToInt32(hdnGridRoyFuzzySearchRowId.Value)];
                    txtTrackTitle = (gvr.FindControl("txtTrackTitle") as TextBox);
                    txtTrackTitle.Text = string.Empty;
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "CompareGrid", "CompareGridData(" + Convert.ToInt32(hdnGridRoyFuzzySearchRowId.Value) + ");", true);
                }
                else if (hdnFuzzySearchField.Value == "RoyaltorInsert")
                {
                    txtRoyaltorInsert.Text = string.Empty;
                    PopulateOptPrdInsert();
                    PopulateEscCodeInsert();
                }
                else if (hdnFuzzySearchField.Value == "TrackTitleInsert")
                {
                    txtTrackTitleInsert.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing full search popup", ex.Message);
            }
        }

        #endregion Fuzzy Search

    }
}