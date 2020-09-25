/*
File Name   :   RoyContractOptionPeriods.cs
Purpose     :   to add/edit Option period details of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     10-Apr-2017     Harish(Infosys Limited)   Initial Creation
2.0     23-May-2017     Harish                    WUIN-639 - Users where role not = 4 should not be able to add Option period with -1 
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

namespace WARS.Contract
{
    public partial class RoyContractOptionPeriods : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractOptionPeriodsBL royContractOptionPeriodsBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtUnitType;
        DataTable dtPriceType;
        DataTable dtReceiptType;
        string loggedUserID;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        Int32 maxOptionPeriodCode;
        #endregion Global Declarations

        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];

                //royaltorId = "12340";
                //isNewRoyaltor = "N";

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Option Periods";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Option Periods";
                }

                //lblTab.Focus();//tabbing sequence starts here
                txtDescAddRow.Focus();
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //HtmlTableRow trOptionPeriods = (HtmlTableRow)contractNavigationButtons.FindControl("trOptionPeriods");
                        //trOptionPeriods.Visible = false;
                        Button btnOptionPeriods = (Button)contractNavigationButtons.FindControl("btnOptionPeriods");
                        btnOptionPeriods.Enabled = false;
                        HiddenField hdnRoyaltorId = (HiddenField)contractNavigationButtons.FindControl("hdnRoyaltorId");
                        hdnRoyaltorId.Value = royaltorId;
                        HiddenField hdnRoyaltorIdHdr = (HiddenField)contractHdrNavigation.FindControl("hdnRoyaltorId");
                        hdnRoyaltorIdHdr.Value = royaltorId;

                        //WUIN-599 - resetting the flags
                        ((HiddenField)Master.FindControl("hdnIsContractScreen")).Value = "N";
                        ((HiddenField)Master.FindControl("hdnIsNotContractScreen")).Value = "N";
                        this.Master.FindControl("lnkBtnHome").Visible = false;
                       

                        if (isNewRoyaltor == "Y")
                        {
                            btnSave.Text = "Save & Continue";
                            btnAudit.Text = "Back";
                            //contractNavigationButtons.Disable();
                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.OptionPeriod.ToString());
                        }

                        txtRoyaltorId.Text = royaltorId;
                        LoadOptionPeriodData();
                        SetAddRowDefaultValues();

                        //WUIN-450 - If a new Royaltor is set up with Lock checked it should continue to allow entry of the details (the Royaltor set up needs to be completed!)
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId))
                        {
                            btnSave.ToolTip = "Royaltor Locked";
                        }

                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        // If a contract is already using by another user then making the screen readonly.
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId))
                        {
                           hdnOtherUserScreenLocked.Value = "Y";
                        }

                        //WUIN-1096 - Only Read access for ReadonlyUser
                        //WUIN-599 If a contract is already using by another user then making the screen readonly.
                        //WUIN-450 -Only Read access for locked contracts
                        if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId)) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId)))
                        {
                            EnableReadonly();
                        }

                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                }

            }
            catch (ThreadAbortException ex1)
            {

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

                Array optionPeriodList = OptionPeriodList();
                List<string> deleteList = new List<string>();
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                }

                //check if any changes to save
                if (optionPeriodList.Length == 0 && deleteList.Count == 0)
                {
                    if (isNewRoyaltor == "N")
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }
                    else if (isNewRoyaltor == "Y")
                    {
                        if (gvContOptionPeriod.Rows.Count == 0)
                        {
                            msgView.SetMessage("Must be at least one option period!", MessageType.Warning, PositionType.Auto);
                        }
                        else
                        {
                            //WUIN-450
                            //set screen button enabled = Y
                            contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.OptionPeriod.ToString());

                            ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                        }

                    }

                    return;
                }

                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Option period details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string royaltor;
                royContractOptionPeriodsBL = new RoyContractOptionPeriodsBL();
                DataSet optPeriodData = royContractOptionPeriodsBL.SaveOptionPeriod(royaltorId, optionPeriodList, deleteList.ToArray(), loggedUserID,
                                        Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower()), out royaltor, out maxOptionPeriodCode, out errorId);
                royContractOptionPeriodsBL = null;
                hdnGridDataChanged.Value = "N";
                hdnGridDataDeleted.Value = "N";
                ViewState["vsDeleteIds"] = null;


                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;


                if (errorId == 1)
                {
                    msgView.SetMessage("Option periods not saved – option already exists for the royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (errorId == 0)
                {
                    if (optPeriodData.Tables.Count != 0)
                    {
                        txtRoyaltorId.Text = royaltor;
                        txtOptionAddRow.Text = maxOptionPeriodCode.ToString();
                        dtUnitType = optPeriodData.Tables[1];
                        dtPriceType = optPeriodData.Tables[2];
                        dtReceiptType = optPeriodData.Tables[3];
                        Session["RoyContOptPrdGridDataInitial"] = optPeriodData.Tables[0];
                        Session["RoyContOptPrdGridData"] = optPeriodData.Tables[0];
                        Session["RoyContOptPrdUnitType"] = dtUnitType;
                        Session["RoyContOptPrdPriceType"] = dtPriceType;
                        Session["RoyContOptPrdReceiptType"] = dtReceiptType;

                        gvContOptionPeriod.DataSource = optPeriodData.Tables[0];
                        gvContOptionPeriod.DataBind();

                        if (optPeriodData.Tables[0].Rows.Count == 0)
                        {
                            gvContOptionPeriod.EmptyDataText = "No data found for the selected royaltor";
                        }
                        else
                        {
                            gvContOptionPeriod.EmptyDataText = string.Empty;
                        }

                    }
                    else if (optPeriodData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvContOptionPeriod.DataSource = dtEmpty;
                        gvContOptionPeriod.EmptyDataText = "No data found for the selected royaltor";
                        gvContOptionPeriod.DataBind();
                    }

                    SetAddRowDefaultValues();

                    //new royaltor - redirect to Royalty rates screen
                    //existing royaltor - remain in same screen
                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.OptionPeriod.ToString());

                        //redirect to Bank details screen                                  
                        //Response.Redirect(@"~/Contract/RoyContractRoyRates.aspx?RoyaltorId=" + royaltorId.Split('-')[0].Trim() + "&isNewRoyaltor=Y", false);
                        //redirect in javascript so that issue of data not saved validation would be handled
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId.Split('-')[0].Trim() + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Option periods saved", MessageType.Warning, PositionType.Auto);
                    }

                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving option period data", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving option period data", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving option period data", ex.Message);
            }
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            hdnIsAuditScreen.Value = "Y";
            //redirect in javascript so that issue of data not saved validation would be handled
            if (isNewRoyaltor == "Y")
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "PreviousScreen", "RedirectToPreviousScreen(" + royaltorId + ");", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, typeof(Page), "AuditScreen", "RedirectToAuditScreen();", true);
            }
        }

        protected void gvContOptionPeriod_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DropDownList ddlUnitField;
                DropDownList ddlPriceField;
                DropDownList ddlReceiptField;
                string unitField;
                string priceField;
                string receiptField;
                string isModified;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlUnitField = (e.Row.FindControl("ddlUnitField") as DropDownList);
                    ddlPriceField = (e.Row.FindControl("ddlPriceField") as DropDownList);
                    ddlReceiptField = (e.Row.FindControl("ddlReceiptField") as DropDownList);
                    unitField = (e.Row.FindControl("hdnUnitField") as HiddenField).Value;
                    priceField = (e.Row.FindControl("hdnPriceField") as HiddenField).Value;
                    receiptField = (e.Row.FindControl("hdnReceiptField") as HiddenField).Value;
                    isModified = (e.Row.FindControl("hdnIsModified") as HiddenField).Value;

                    if (dtUnitType != null)
                    {
                        ddlUnitField.DataSource = dtUnitType;
                        ddlUnitField.DataTextField = "item_text";
                        ddlUnitField.DataValueField = "item_value";
                        ddlUnitField.DataBind();
                        ddlUnitField.Items.Insert(0, new ListItem("-"));

                        if (ddlUnitField.Items.FindByValue(unitField) != null)
                        {
                            ddlUnitField.Items.FindByValue(unitField).Selected = true;
                        }
                        else
                        {
                            ddlUnitField.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        ddlUnitField.Items.Insert(0, new ListItem("-"));
                        ddlUnitField.SelectedIndex = 0;
                    }

                    if (dtPriceType != null)
                    {
                        ddlPriceField.DataSource = dtPriceType;
                        ddlPriceField.DataTextField = "item_text";
                        ddlPriceField.DataValueField = "item_value";
                        ddlPriceField.DataBind();
                        ddlPriceField.Items.Insert(0, new ListItem("-"));

                        if (ddlPriceField.Items.FindByValue(priceField) != null)
                        {
                            ddlPriceField.Items.FindByValue(priceField).Selected = true;
                        }
                        else
                        {
                            ddlPriceField.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        ddlPriceField.Items.Insert(0, new ListItem("-"));
                        ddlPriceField.SelectedIndex = 0;
                    }

                    if (dtReceiptType != null)
                    {
                        ddlReceiptField.DataSource = dtReceiptType;
                        ddlReceiptField.DataTextField = "item_text";
                        ddlReceiptField.DataValueField = "item_value";
                        ddlReceiptField.DataBind();
                        ddlReceiptField.Items.Insert(0, new ListItem("-"));

                        if (ddlReceiptField.Items.FindByValue(receiptField) != null)
                        {
                            ddlReceiptField.Items.FindByValue(receiptField).Selected = true;
                        }
                        else
                        {
                            ddlReceiptField.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        ddlReceiptField.Items.Insert(0, new ListItem("-"));
                        ddlReceiptField.SelectedIndex = 0;
                    }

                    if (isModified == "-" || isModified == "C")
                    {
                        //(e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = false;
                        (e.Row.FindControl("ImageCopy") as ImageButton).Visible = false;
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

        protected void gvContOptionPeriod_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                

                if (e.CommandName == "cancelRow")
                {
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    string optionPeriodCode = (row.FindControl("lblOption") as Label).Text;
                    string optionPeriodCodeCopy = (row.FindControl("hdnOptPeriodCodeCopy") as HiddenField).Value;
                    string isModified = (row.FindControl("hdnIsModified") as HiddenField).Value;

                    RowCancelChanges(optionPeriodCode);
                }
                else if (e.CommandName == "copy")
                {
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    string optionPeriodCode = (row.FindControl("lblOption") as Label).Text;
                    string optionPeriodCodeCopy = (row.FindControl("hdnOptPeriodCodeCopy") as HiddenField).Value;
                    string isModified = (row.FindControl("hdnIsModified") as HiddenField).Value;

                    hdnCopyRowOptPrdCodeCopy.Value = optionPeriodCode;
                    hdnCopyRowUnitField.Value = (row.FindControl("ddlUnitField") as DropDownList).SelectedValue;
                    hdnCopyRowPriceField.Value = (row.FindControl("ddlPriceField") as DropDownList).SelectedValue;
                    hdnCopyRowReceiptField.Value = (row.FindControl("ddlReceiptField") as DropDownList).SelectedValue;
                    mpeCopyOptionPopup.Show();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting/cancelling grid data", ex.Message);
            }
        }

        protected void btnAppendAddRow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                if (!ValidateAppendRow())
                {
                    msgView.SetMessage("Option already exists", MessageType.Warning, PositionType.Auto);
                    return;
                }

                AppendRowToGrid("New");
                ClearAddRow();


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding option period row to grid", ex.Message);
            }
        }

        protected void btnCopyOptPrdContinue_Click(object sender, EventArgs e)
        {
            try
            {
                AppendRowToGrid("Copy");
                txtDescriptionCopyOptPrd.Text = string.Empty;
                txtPLGContCopyOptPrd.Text = string.Empty;
                hdnCopyRowUnitField.Value = null;
                hdnCopyRowOptPrdCodeCopy.Value = null;
                hdnCopyRowPriceField.Value = null;
                hdnCopyRowReceiptField.Value = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in copying option period", ex.Message);
            }
        }

        protected void btnCopyOptPrdCancel_Click(object sender, EventArgs e)
        {
            ClearCopyOptPrdControls();
            mpeCopyOptionPopup.Hide();
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //delete row from the grid
                DeleteRowFromGrid(hdnOptPrdToDelete.Value.ToString(), hdnOptPrdToDeleteIsModified.Value.ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting option period", ex.Message);
            }

        }
        //JIRA-908 Changes by Ravi in 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            if (hdnOptionDeleteRowIndex.Value != "")
            {
                int rowIndex = Convert.ToInt16(hdnOptionDeleteRowIndex.Value);

                string optionPeriodCode = ((Label)gvContOptionPeriod.Rows[rowIndex].FindControl("lblOption")).Text;
                string optionPeriodCodeCopy = ((HiddenField)gvContOptionPeriod.Rows[rowIndex].FindControl("hdnOptPeriodCodeCopy")).Value;
                string isModified = ((HiddenField)gvContOptionPeriod.Rows[rowIndex].FindControl("hdnIsModified")).Value;


                if (optionPeriodCodeCopy == string.Empty)
                {
                    //validation - 1. deletion not allowed if option period is in participation table for the royaltor  
                    //             2. warning if deleting row that has Rates set up 
                    royContractOptionPeriodsBL = new RoyContractOptionPeriodsBL();
                    royContractOptionPeriodsBL.ValidateDelete(royaltorId, optionPeriodCode, out errorId);
                    royContractOptionPeriodsBL = null;
                    if (errorId == 1)
                    {
                        msgView.SetMessage("Option period is used in participation - cannot be deleted!", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else if (errorId == 3)
                    {
                        hdnOptPrdToDelete.Value = optionPeriodCode;
                        hdnOptPrdToDeleteIsModified.Value = isModified;
                        mpeConfirmDelete.Show();
                        return;
                    }
                    DeleteRowFromGrid(optionPeriodCode, isModified);
                }
            }

        }
        //JIRA-908 Changes by Ravi in 13/02/2019 -- End

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvContOptionPeriod_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyContOptPrdGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                AssignGridDropdownList();
                gvContOptionPeriod.DataSource = dataView;
                gvContOptionPeriod.DataBind();
                Session["RoyContOptPrdGridData"] = dataView.ToTable();
            }
            //WUIN-1096 -  ReadOnlyUser
            if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()))
            {
                EnableReadonly();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion EVENTS

        #region METHODS

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void LoadOptionPeriodData()
        {
            string royaltor;

            royContractOptionPeriodsBL = new RoyContractOptionPeriodsBL();
            DataSet optPeriodData = royContractOptionPeriodsBL.GetOptionPeriodData(royaltorId, Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower()), out royaltor, out maxOptionPeriodCode, out errorId);
            royContractOptionPeriodsBL = null;

            if (optPeriodData.Tables.Count != 0 && errorId != 2)
            {
                txtRoyaltorId.Text = royaltor;
                txtOptionAddRow.Text = maxOptionPeriodCode.ToString();
                hdnOptionPeriodCode.Value = maxOptionPeriodCode.ToString();
                dtUnitType = optPeriodData.Tables[1];
                dtPriceType = optPeriodData.Tables[2];
                dtReceiptType = optPeriodData.Tables[3];
                Session["RoyContOptPrdGridDataInitial"] = optPeriodData.Tables[0];
                Session["RoyContOptPrdGridData"] = optPeriodData.Tables[0];
                Session["RoyContOptPrdUnitType"] = dtUnitType;
                Session["RoyContOptPrdPriceType"] = dtPriceType;
                Session["RoyContOptPrdReceiptType"] = dtReceiptType;

                ddlUnitFieldAddRow.DataSource = dtUnitType;
                ddlUnitFieldAddRow.DataTextField = "item_text";
                ddlUnitFieldAddRow.DataValueField = "item_value";
                ddlUnitFieldAddRow.DataBind();
                ddlUnitFieldAddRow.Items.Insert(0, new ListItem("-"));

                ddlPriceFieldAddRow.DataSource = dtPriceType;
                ddlPriceFieldAddRow.DataTextField = "item_text";
                ddlPriceFieldAddRow.DataValueField = "item_value";
                ddlPriceFieldAddRow.DataBind();
                ddlPriceFieldAddRow.Items.Insert(0, new ListItem("-"));

                ddlReceiptFieldAddRow.DataSource = dtReceiptType;
                ddlReceiptFieldAddRow.DataTextField = "item_text";
                ddlReceiptFieldAddRow.DataValueField = "item_value";
                ddlReceiptFieldAddRow.DataBind();
                ddlReceiptFieldAddRow.Items.Insert(0, new ListItem("-"));

                if (optPeriodData.Tables[0].Rows.Count == 0)
                {
                    gvContOptionPeriod.DataSource = optPeriodData.Tables[0];
                    gvContOptionPeriod.EmptyDataText = "No data found for the selected royaltor";
                    gvContOptionPeriod.DataBind();
                }
                else
                {
                    gvContOptionPeriod.DataSource = optPeriodData.Tables[0];
                    gvContOptionPeriod.DataBind();
                }

            }
            else if (optPeriodData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvContOptionPeriod.DataSource = dtEmpty;
                gvContOptionPeriod.EmptyDataText = "No data found for the selected royaltor";
                gvContOptionPeriod.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }

            hdnGridDataChanged.Value = "N";
            hdnGridDataDeleted.Value = "N";
            ViewState["vsDeleteIds"] = null;
        }

        /// <summary>
        /// validate if duplicate rows are not being added to the grid
        /// </summary>
        /// <returns></returns>
        private bool ValidateAppendRow()
        {
            bool isValid = true;
            string optPeriodCode;

            foreach (GridViewRow gvr in gvContOptionPeriod.Rows)
            {
                optPeriodCode = (gvr.FindControl("lblOption") as Label).Text;
                if (optPeriodCode == txtOptionAddRow.Text)
                {
                    isValid = false;
                    break;
                }

            }

            return isValid;
        }

        private void AppendRowToGrid(string addType)
        {
            if (Session["RoyContOptPrdGridData"] == null || Session["RoyContOptPrdGridDataInitial"] == null)
            {
                ExceptionHandler("Error in adding royalty rate row to grid", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContOptPrdGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyContOptPrdGridDataInitial"];
            DataRow drNewRow = dtGridData.NewRow();
            DataRow drNewRowOriginal = dtOriginalGridData.NewRow();

            if (addType == "New")//for appending new row data
            {
                drNewRow["option_period_code"] = txtOptionAddRow.Text;
                hdnOptionPeriodCode.Value = (Convert.ToInt32(hdnOptionPeriodCode.Value) + 1).ToString();
                drNewRow["option_period_code_copy"] = "-";
                drNewRow["option_period_desc"] = txtDescAddRow.Text;
                drNewRow["plg_option_period"] = txtPLGConAddRow.Text;
                drNewRow["unit_type"] = ddlUnitFieldAddRow.SelectedValue;
                drNewRow["price_type"] = ddlPriceFieldAddRow.SelectedValue;
                drNewRow["receipt_type"] = ddlReceiptFieldAddRow.SelectedValue;
                drNewRow["is_modified"] = "-";
            }
            else if (addType == "Copy")//for appending copy row data
            {
                //Generate next number for new Option Period
                int maxOptPeriodCode = Convert.ToInt32(dtGridData.AsEnumerable().Max(row => row["option_period_code"]));
                if (maxOptPeriodCode >= 99)
                {
                    msgView.SetMessage("Cannot create new option Period greater than 99. Please contact WMI Royalties Support", MessageType.Warning, PositionType.Auto);
                    mpeCopyOptionPopup.Hide();
                    return;
                }
                else
                {
                    drNewRow["option_period_code"] = (maxOptPeriodCode + 1).ToString();
                }
                drNewRow["option_period_code_copy"] = hdnCopyRowOptPrdCodeCopy.Value;
                drNewRow["option_period_desc"] = txtDescriptionCopyOptPrd.Text;
                drNewRow["plg_option_period"] = txtPLGContCopyOptPrd.Text;
                drNewRow["unit_type"] = hdnCopyRowUnitField.Value == "-" ? "0" : hdnCopyRowUnitField.Value;
                drNewRow["price_type"] = hdnCopyRowPriceField.Value == "-" ? "0" : hdnCopyRowPriceField.Value;
                drNewRow["receipt_type"] = hdnCopyRowReceiptField.Value == "-" ? "0" : hdnCopyRowReceiptField.Value;
                drNewRow["is_modified"] = "C";
            }

            dtGridData.Rows.Add(drNewRow);
            DataView dv = dtGridData.DefaultView;
            dv.Sort = "option_period_code asc";
            DataTable dtGridDataSorted = dv.ToTable();
            Session["RoyContOptPrdGridData"] = dtGridDataSorted;
            AssignGridDropdownList();
            gvContOptionPeriod.DataSource = dtGridDataSorted;
            gvContOptionPeriod.DataBind();

            drNewRowOriginal.ItemArray = drNewRow.ItemArray.Clone() as object[];
            dtOriginalGridData.Rows.Add(drNewRowOriginal);
            DataView dvInitialData = dtOriginalGridData.DefaultView;
            dvInitialData.Sort = "option_period_code asc";
            DataTable dtOriginalGridDataSorted = dvInitialData.ToTable();
            Session["RoyContOptPrdGridDataInitial"] = dtOriginalGridDataSorted;

        }

        private void ClearAddRow()
        {
            txtOptionAddRow.Text = hdnOptionPeriodCode.Value;
            txtDescAddRow.Text = string.Empty;
            txtPLGConAddRow.Text = string.Empty;
            ddlReceiptFieldAddRow.SelectedIndex = 0;
            SetAddRowDefaultValues();

        }

        private Array OptionPeriodList()
        {
            if (Session["RoyContOptPrdGridDataInitial"] == null)
            {
                ExceptionHandler("Error in saving option period data", string.Empty);
            }

            DataTable optPeriodData = (DataTable)Session["RoyContOptPrdGridDataInitial"];
            List<string> optPeriodDetails = new List<string>();
            string optPeriodCode;
            string optPeriodCodeCopy;
            string description;
            string PLGContract;
            string isModified;
            DropDownList ddlUnitField;
            DropDownList ddlPriceField;
            DropDownList ddlReceiptField;

            foreach (GridViewRow gvr in gvContOptionPeriod.Rows)
            {
                optPeriodCode = (gvr.FindControl("lblOption") as Label).Text;
                optPeriodCodeCopy = (gvr.FindControl("hdnOptPeriodCodeCopy") as HiddenField).Value;
                description = (gvr.FindControl("txtDescription") as TextBox).Text;
                PLGContract = (gvr.FindControl("txtPlgContractVersion") as TextBox).Text;
                ddlUnitField = (gvr.FindControl("ddlUnitField") as DropDownList);
                ddlPriceField = (gvr.FindControl("ddlPriceField") as DropDownList);
                ddlReceiptField = (gvr.FindControl("ddlReceiptField") as DropDownList);
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                if (isModified == Global.DBNullParamValue || isModified == "C")//new or copied row
                {
                    if (isModified == Global.DBNullParamValue)//new row
                    {
                        optPeriodCodeCopy = Global.DBNullParamValue;
                        isModified = Global.DBNullParamValue;
                        if (description == string.Empty)
                            description = Global.DBNullParamValue;

                        if (PLGContract == string.Empty)
                            PLGContract = Global.DBNullParamValue;
                        optPeriodDetails.Add(optPeriodCode + Global.DBDelimiter + optPeriodCodeCopy + Global.DBDelimiter + description + Global.DBDelimiter + PLGContract + Global.DBDelimiter + ddlUnitField.SelectedValue + Global.DBDelimiter +
                              ddlPriceField.SelectedValue + Global.DBDelimiter + ddlReceiptField.SelectedValue + Global.DBDelimiter + isModified);
                    }
                    else if (isModified == "C")//copied row
                    {

                        isModified = "C";
                        if (description == string.Empty)
                            description = Global.DBNullParamValue;

                        if (PLGContract == string.Empty)
                            PLGContract = Global.DBNullParamValue;
                        optPeriodDetails.Add(optPeriodCode + Global.DBDelimiter + optPeriodCodeCopy + Global.DBDelimiter + description + Global.DBDelimiter + PLGContract + Global.DBDelimiter + ddlUnitField.SelectedValue + Global.DBDelimiter +
                              ddlPriceField.SelectedValue + Global.DBDelimiter + ddlReceiptField.SelectedValue + Global.DBDelimiter + isModified);
                    }
                }
                else
                {
                    DataTable dtOptPeriod = optPeriodData.Select("option_period_code=" + optPeriodCode).CopyToDataTable();
                    if (description != dtOptPeriod.Rows[0]["option_period_desc"].ToString() || PLGContract != dtOptPeriod.Rows[0]["plg_option_period"].ToString()
                            || ddlUnitField.SelectedValue != dtOptPeriod.Rows[0]["unit_type"].ToString()
                            || ddlPriceField.SelectedValue != dtOptPeriod.Rows[0]["price_type"].ToString()
                            || ddlReceiptField.SelectedValue != dtOptPeriod.Rows[0]["receipt_type"].ToString())//existing row - if data changed 
                    {
                        optPeriodCodeCopy = Global.DBNullParamValue;
                        isModified = "Y";
                        if (description == string.Empty)
                            description = Global.DBNullParamValue;

                        if (PLGContract == string.Empty)
                            PLGContract = Global.DBNullParamValue;
                        optPeriodDetails.Add(optPeriodCode + Global.DBDelimiter + optPeriodCodeCopy + Global.DBDelimiter + description + Global.DBDelimiter + PLGContract + Global.DBDelimiter + ddlUnitField.SelectedValue + Global.DBDelimiter +
                             ddlPriceField.SelectedValue + Global.DBDelimiter + ddlReceiptField.SelectedValue + Global.DBDelimiter + isModified);
                    }

                }




            }

            return optPeriodDetails.ToArray();
        }

        private void RowCancelChanges(string optionPeriodCode)
        {
            if (Session["RoyContOptPrdGridData"] == null || Session["RoyContOptPrdGridDataInitial"] == null)
            {
                ExceptionHandler("Error in cancelling row changes", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContOptPrdGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyContOptPrdGridDataInitial"];
            //get original data row before changes for the selected row cancel
            DataTable dtOriginalGridDataRow = dtOriginalGridData.Select("option_period_code=" + optionPeriodCode).CopyToDataTable();
            DataRow drOriginalGridDataRow;

            if (dtOriginalGridDataRow.Rows.Count != 0)
            {
                drOriginalGridDataRow = dtOriginalGridDataRow.Rows[0];

                foreach (DataRow dr in dtGridData.Rows)
                {
                    if (dr["option_period_code"].ToString() == optionPeriodCode)
                    {
                        dr.Delete();
                        break;
                    }
                }

                dtGridData.ImportRow(drOriginalGridDataRow);
                DataView dv = dtGridData.DefaultView;
                dv.Sort = "option_period_code asc";
                DataTable dtGridChangedDataSorted = dv.ToTable();
                Session["RoyContOptPrdGridData"] = dtGridChangedDataSorted;
                AssignGridDropdownList();
                gvContOptionPeriod.DataSource = dtGridChangedDataSorted;
                gvContOptionPeriod.DataBind();

            }

        }

        private void DeleteRowFromGrid(string optionPeriodCode, string isModified)
        {
            GetGridData();
            //add to delete list only if the row is not a new one or a copied one
            List<string> deleteList = new List<string>();
            if (isModified != Global.DBNullParamValue && isModified != "C")
            {
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                    deleteList.Add(optionPeriodCode);
                }
                else
                {
                    deleteList.Add(optionPeriodCode);
                }

                ViewState["vsDeleteIds"] = deleteList;
                hdnGridDataDeleted.Value = "Y";

            }

            DataTable dtGridData = (DataTable)Session["RoyContOptPrdGridData"];
            foreach (DataRow dr in dtGridData.Rows)
            {
                if (dr["option_period_code"].ToString() == optionPeriodCode)
                {
                    dr.Delete();
                    break;
                }

            }

            DataView dv = dtGridData.DefaultView;
            dv.Sort = "option_period_code asc";
            DataTable dtGridChangedDataSorted = dv.ToTable();
            Session["RoyContOptPrdGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvContOptionPeriod.DataSource = dtGridChangedDataSorted;
            gvContOptionPeriod.DataBind();

            //delete row from initial grid data session
            DataTable dtOriginalGridData = (DataTable)Session["RoyContOptPrdGridDataInitial"];
            foreach (DataRow dr in dtOriginalGridData.Rows)
            {
                if (dr["option_period_code"].ToString() == optionPeriodCode)
                {
                    dr.Delete();
                    break;
                }

            }

            DataView dvInitialData = dtOriginalGridData.DefaultView;
            dvInitialData.Sort = "option_period_code asc";
            DataTable dtOriginalGridDataSorted = dvInitialData.ToTable();
            Session["RoyContOptPrdGridDataInitial"] = dtOriginalGridDataSorted;

        }

        private void AssignGridDropdownList()
        {
            dtUnitType = (DataTable)Session["RoyContOptPrdUnitType"];
            dtPriceType = (DataTable)Session["RoyContOptPrdPriceType"];
            dtReceiptType = (DataTable)Session["RoyContOptPrdReceiptType"];
        }

        private void ClearCopyOptPrdControls()
        {
            txtDescriptionCopyOptPrd.Text = string.Empty;
            txtPLGContCopyOptPrd.Text = string.Empty;
        }

        /// <summary>
        /// set default Unit field to value 1, Price field to value 2
        /// </summary>
        private void SetAddRowDefaultValues()
        {
            ddlUnitFieldAddRow.SelectedIndex = 1;
            ddlPriceFieldAddRow.SelectedIndex = 2;
            ddlReceiptFieldAddRow.SelectedIndex = 1;
        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["RoyContOptPrdGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            string unitType;
            string priceType;
            string receiptType;

            foreach (GridViewRow gvr in gvContOptionPeriod.Rows)
            {
                unitType = (gvr.FindControl("ddlUnitField") as DropDownList).SelectedValue;
                priceType = (gvr.FindControl("ddlPriceField") as DropDownList).SelectedValue;
                receiptType = (gvr.FindControl("ddlReceiptField") as DropDownList).SelectedValue;
                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["option_period_code"] = (gvr.FindControl("lblOption") as Label).Text;
                drGridRow["option_period_code_copy"] = (gvr.FindControl("hdnOptPeriodCodeCopy") as HiddenField).Value;
                drGridRow["option_period_desc"] = (gvr.FindControl("txtDescription") as TextBox).Text;
                drGridRow["plg_option_period"] = (gvr.FindControl("txtPlgContractVersion") as TextBox).Text;
                drGridRow["unit_type"] = unitType == "-" ? "0" : unitType;
                drGridRow["price_type"] = priceType == "-" ? "0" : priceType;
                drGridRow["receipt_type"] = receiptType == "-" ? "0" : receiptType;
                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["RoyContOptPrdGridData"] = dtGridChangedData;

        }

        private void EnableReadonly()
        {
            btnSave.Enabled = false;
            btnAppendAddRow.Enabled = false;
            btnUndoAddRow.Enabled = false;

            //disable grid buttons
            foreach (GridViewRow gvr in gvContOptionPeriod.Rows)
            {
                (gvr.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                (gvr.FindControl("ImageCopy") as ImageButton).Enabled = false;
            }
        }


        #endregion  METHODS



        #region Web Methods

        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static int UpdateScreenLockFlag()
        {
            int errorId;
            RoyaltorContractBL royContractBL = new RoyaltorContractBL();
            royContractBL.UpdateScreenLockFlag(HttpContext.Current.Session["ScreenLockedRoyaltorId"].ToString(), "N", HttpContext.Current.Session["UserCode"].ToString(), out errorId);
            royContractBL = null;
            return errorId;

        }

        #endregion Web Methods



    }
}