/*
File Name   :   RoyaltorStatementChanges.cs
Purpose     :   To add or remove royaltors from a statement run

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     11-Feb-2016     Harish(Infosys Limited)   Initial Creation
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

namespace WARS
{
    public partial class RoyaltorStatementChanges : System.Web.UI.Page
    {
        #region Global Declarations
        Utilities util;
        string userRole;
        Int32 errorId;
        DataTable dtEmpty;
        RoyaltorStatementBL royaltorStatementBL;
        Int32 royaltorId;
        #endregion Global Declarations

        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region PAGE EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Statement Changes";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Statement Changes";
                }

                hdnAddBtnEnable.Value = string.Empty;
                hdnRemoveBtnEnable.Value = string.Empty;

                if (Session["UserRole"] != null)
                {
                    userRole = Session["UserRole"].ToString();
                }

                if (IsPostBack && tdGrid.Visible == true)
                {
                    //set gridview panel height                    
                    PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                }

                txtRoyaltor.Focus();//tabbing sequence starts here
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        userRole = Session["UserRole"].ToString();
                        //Validation: only Super user can access this page
                        if (userRole.ToLower() == UserRole.SuperUser.ToString().ToLower())
                        {
                            tdGrid.Visible = false;
                            tdOwner.Visible = false;
                            tdButtons.Visible = false;
                            LoadDropdowns();
                            LoadInitialGridData();
                        }
                        else
                        {
                            ExceptionHandler(util.OnlySuperUserExpMessage.ToString(), string.Empty);
                        }
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;

                }

            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading Royaltor Statement Process screen.", ex.Message);
            }

        }

        protected void txtRoyaltor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnRoySearchSelected.Value == "Y")
                {
                    LoadData();
                }
                else if (hdnRoySearchSelected.Value == "N")
                {
                    FuzzySearchRoyaltor();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

                }
                else if (hdnRoySearchSelected.Value == string.Empty)
                {
                    LoadInitialGridData();
                    btnRemove.Enabled = false;
                }

                hdnRoySearchSelected.Value = string.Empty;


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from textbox.", ex.Message);
            }
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRoyaltor.Text != string.Empty)
                {
                    UpdateRoyStmtRun();
                    msgView.SetMessage("Job to remove statements has been triggered.", MessageType.Warning, PositionType.Auto);
                }
                else if (txtOwner.Text != string.Empty)
                {
                    UpdateOwnerStmtRun();
                    msgView.SetMessage("Job to remove statements has been triggered.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating data.", ex.Message);
            }
            finally
            {
                mpeConfirm.Hide();
            }

        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                PopUpConfirmation("Remove");
            }
            catch (ThreadAbortException ex1)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Remove button event.", ex.Message);
            }
        }

        protected void btnNextRunStmtActvitiy_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRoyaltor.Text != string.Empty)
                {
                    //add the selected stmts if any and pop up the activity list
                    bool cbCheckedValidation = false;
                    bool valAllOrRecentPeriodOnly = false;//WOS-300
                    int checkedRowCount = 0;
                    CheckBox cbAdd;

                    //validations                
                    //2.WOS-300 - allow all periods or recent period only to be selected if multiple periods
                    foreach (GridViewRow row in gvRoyStmt.Rows)
                    {
                        cbAdd = (CheckBox)row.FindControl("cbAdd");
                        if (cbAdd.Checked == true)
                        {
                            cbCheckedValidation = true;
                            checkedRowCount++;

                            if (gvRoyStmt.Rows.Count > 1)
                            {
                                if (checkedRowCount == 1)
                                {
                                    if (row.RowIndex == 0)
                                    {
                                        valAllOrRecentPeriodOnly = true;
                                    }
                                    else
                                    {
                                        valAllOrRecentPeriodOnly = false;
                                    }
                                }
                                else if (checkedRowCount > 1)
                                {
                                    if (checkedRowCount == gvRoyStmt.Rows.Count)
                                    {
                                        valAllOrRecentPeriodOnly = true;
                                    }
                                    else
                                    {
                                        valAllOrRecentPeriodOnly = false;
                                    }

                                }

                            }
                            else if (gvRoyStmt.Rows.Count == 1)
                            {
                                valAllOrRecentPeriodOnly = true;
                            }
                        }

                    }

                    if (cbCheckedValidation != true)
                    {
                        util = new Utilities();
                        actScreen.PopupScreen(hdnGridPnlHeight.Value);
                    }
                    else if (valAllOrRecentPeriodOnly != true)
                    {
                        msgView.SetMessage("Please select all periods or the recent period only.", MessageType.Warning, PositionType.Auto);
                    }
                    else
                    {
                        //update
                        UpdateRoyStmtRun();
                        util = new Utilities();
                        actScreen.PopupScreen(hdnGridPnlHeight.Value);
                    }

                }
                else if (txtOwner.Text != string.Empty)
                {
                    //add the selected stmts if any and pop up the activity list
                    bool cbCheckedValidation = false;
                    bool valAllOrRecentPeriodOnly = false;//WOS-300
                    int checkedRowCount = 0;
                    CheckBox cbAdd;

                    //validations                
                    //2.WOS-300 - allow all periods or recent period only to be selected if multiple periods
                    foreach (GridViewRow row in gvOwnerStmt.Rows)
                    {
                        cbAdd = (CheckBox)row.FindControl("cbAdd");
                        if (cbAdd.Checked == true)
                        {
                            cbCheckedValidation = true;
                            checkedRowCount++;

                            if (gvOwnerStmt.Rows.Count > 1)
                            {
                                if (checkedRowCount == 1)
                                {
                                    if (row.RowIndex == 0)
                                    {
                                        valAllOrRecentPeriodOnly = true;
                                    }
                                    else
                                    {
                                        valAllOrRecentPeriodOnly = false;
                                    }
                                }
                                else if (checkedRowCount > 1)
                                {
                                    if (checkedRowCount == gvOwnerStmt.Rows.Count)
                                    {
                                        valAllOrRecentPeriodOnly = true;
                                    }
                                    else
                                    {
                                        valAllOrRecentPeriodOnly = false;
                                    }

                                }

                            }
                            else if (gvOwnerStmt.Rows.Count == 1)
                            {
                                valAllOrRecentPeriodOnly = true;
                            }
                        }

                    }

                    if (cbCheckedValidation != true)
                    {
                        util = new Utilities();
                        actScreen.PopupScreen(hdnGridPnlHeight.Value);
                    }
                    else if (valAllOrRecentPeriodOnly != true)
                    {
                        msgView.SetMessage("Please select all periods or the recent period only.", MessageType.Warning, PositionType.Auto);
                    }
                    else
                    {
                        //update
                        UpdateOwnerStmtRun();
                        util = new Utilities();
                        actScreen.PopupScreen(hdnGridPnlHeight.Value);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in opening Next Run Statement Activity popup.", ex.Message);
            }

        }

        protected void fuzzySearchRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltor();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }

        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
            //    {                    
            //        txtRoyaltor.Text = string.Empty;
            //        btnRemove.Enabled = false;
            //        LoadInitialGridData();
            //        return;
            //    }

            //    txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
            //    LoadData();
            //}
            //catch (Exception ex)
            //{
            //    ExceptionHandler("Error in fuzzy search selection", ex.Message);
            //}
            try
            {
                if (hdnFuzzySearchField.Value == "Royaltor")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtRoyaltor.Text = string.Empty;
                        btnRemove.Enabled = false;
                        LoadInitialGridData();
                        return;
                    }

                    txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                    LoadData();
                }
                else if (hdnFuzzySearchField.Value == "Owner")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtOwner.Text = string.Empty;
                        btnRemove.Enabled = false;
                        LoadInitialOwnerGridData();
                        return;
                    }

                    txtOwner.Text = lbFuzzySearch.SelectedValue.ToString();
                    LoadOwnerData();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void txtOwner_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnOwnerSearchSelected.Value == "Y")
                {
                    LoadOwnerData();
                }
                else if (hdnOwnerSearchSelected.Value == "N")
                {
                    FuzzySearchOwner();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

                }
                else if (hdnOwnerSearchSelected.Value == string.Empty)
                {
                    LoadInitialOwnerGridData();
                    btnRemove.Enabled = false;
                }

                hdnOwnerSearchSelected.Value = string.Empty;


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting owner from textbox.", ex.Message);
            }
        }

        protected void fuzzySearchOwner_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchOwner();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in owner fuzzy search", ex.Message);
            }
        }

        #endregion PAGE EVENTS

        #region GRIDVIEW EVENTS

        protected void gvRoyStmt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblRoyaltor = (Label)e.Row.FindControl("lblRoyaltor");
                    Label lblName = (Label)e.Row.FindControl("lblName");
                    CheckBox cbAdd = (CheckBox)e.Row.FindControl("cbAdd");
                    CheckBox cbRemove = (CheckBox)e.Row.FindControl("cbRemove");

                    string addRoy = (e.Row.FindControl("hdnAddRoy") as Label).Text;
                    string removeRoy = (e.Row.FindControl("hdnRemoveRoy") as Label).Text;

                    //display royaltor id and name only in first row
                    if (e.Row.RowIndex > 0)
                    {
                        lblRoyaltor.Text = "";
                        lblName.Text = "";
                    }

                    //Enable/disable Add and Remove checkbox
                    if (addRoy == "Y")
                    {
                        cbAdd.Enabled = true;
                        cbRemove.Enabled = false;
                        hdnAddBtnEnable.Value = "Y";
                    }
                    else if (removeRoy == "Y")
                    {
                        cbRemove.Enabled = true;
                        cbAdd.Enabled = false;
                        hdnRemoveBtnEnable.Value = "Y";
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
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }

        }

        protected void gvOwnerStmt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblOwner = (Label)e.Row.FindControl("lblOwnerCode");
                    Label lblName = (Label)e.Row.FindControl("lblOwnerName");
                    CheckBox cbAdd = (CheckBox)e.Row.FindControl("cbAdd");
                    CheckBox cbRemove = (CheckBox)e.Row.FindControl("cbRemove");

                    string addOwner = (e.Row.FindControl("hdnAddOwner") as Label).Text;
                    string removeOwner = (e.Row.FindControl("hdnRemoveOwner") as Label).Text;

                    //display owner code and name only in first row
                    if (e.Row.RowIndex > 0)
                    {
                        lblOwner.Text = "";
                        lblName.Text = "";
                    }

                    //Enable/disable Add and Remove checkbox
                    if (addOwner == "Y")
                    {
                        cbAdd.Enabled = true;
                        cbRemove.Enabled = false;
                        hdnAddBtnEnable.Value = "Y";
                    }
                    else if (removeOwner == "Y")
                    {
                        cbRemove.Enabled = true;
                        cbAdd.Enabled = false;
                        hdnRemoveBtnEnable.Value = "Y";
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
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }
        }


        protected void gvOwnerStmt_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyStmtChangesOwnerData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvOwnerStmt.DataSource = dataView;
                gvOwnerStmt.DataBind();
            }
        }

        protected void gvRoyStmt_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyStmtChangesRoyData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvRoyStmt.DataSource = dataView;
                gvRoyStmt.DataBind();
            }
        }
      

        #endregion GRIDVIEW EVENTS

        #region METHODS

        private void LoadData()
        {
            tdGrid.Visible = true;
            tdOwner.Visible = false;
            tdButtons.Visible = true;
            txtOwner.Text = string.Empty;

            if (txtRoyaltor.Text != string.Empty)
            {
                if (ValidateSelectedRoyaltorFilter() == false)
                {
                    LoadInitialGridData();
                    msgView.SetMessage("Please select a valid royaltor from the filter list.",
                                MessageType.Warning, PositionType.Auto);
                    txtRoyaltor.Text = string.Empty;
                    btnRemove.Enabled = false;

                    return;
                }
                else
                {
                    royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));
                    LoadGridData(royaltorId);
                    if (gvRoyStmt.Rows.Count > 0)
                    {
                        PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                    }
                    EnableDisableButtons();
                }


            }
            else
            {
                LoadInitialGridData();
                tdGrid.Visible = false;
                tdButtons.Visible = false;
            }
        }

        private void LoadOwnerData()
        {
            tdGrid.Visible = false;
            tdOwner.Visible = true;
            tdButtons.Visible = true;
            txtRoyaltor.Text = string.Empty;

            if (txtOwner.Text != string.Empty)
            {
                if (ValidateSelectedOwnerFilter() == false)
                {
                    LoadInitialOwnerGridData();
                    msgView.SetMessage("Please select a valid owner from the filter list.",
                                MessageType.Warning, PositionType.Auto);
                    txtOwner.Text = string.Empty;
                    btnRemove.Enabled = false;

                    return;
                }
                else
                {
                    int ownerCode = Convert.ToInt32(txtOwner.Text.Substring(0, txtOwner.Text.IndexOf("-") - 1));
                    LoadOwnerGridData(ownerCode);
                    if (gvOwnerStmt.Rows.Count > 0)
                    {
                        pnlOwnerGrid.Style.Add("height", hdnGridPnlHeight.Value);
                    }
                    EnableDisableButtons();
                }


            }
            else
            {
                LoadInitialGridData();
                tdOwner.Visible = false;
                tdButtons.Visible = false;
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void LoadDropdowns()
        {
            royaltorStatementBL = new RoyaltorStatementBL();
            DataSet royaltors = royaltorStatementBL.GetRoyaltors(out errorId);
            royaltorStatementBL = null;
            if (royaltors.Tables.Count != 0 && errorId != 2)
            {                
                Session["StmtChangesOwnerList"] = royaltors.Tables[0];
            }
            else if (royaltors.Tables.Count == 0 && errorId != 2)
            {
                Session["StmtChangesOwnerList"] = royaltors.Tables[0];
            }
            else
            {
                ExceptionHandler("Error in loading royaltors dropdown.", string.Empty);
            }
        }

        private void LoadInitialGridData()
        {
            dtEmpty = new DataTable();
            gvRoyStmt.DataSource = dtEmpty;
            gvRoyStmt.EmptyDataText = "No Data Found";
            gvRoyStmt.DataBind();

        }

        private void LoadInitialOwnerGridData()
        {
            dtEmpty = new DataTable();
            gvOwnerStmt.DataSource = dtEmpty;
            gvOwnerStmt.EmptyDataText = "No Data Found";
            gvOwnerStmt.DataBind();

        }

        private void LoadGridData(Int32 royaltorId)
        { 
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            royaltorStatementBL = new RoyaltorStatementBL();
            DataSet royStmtData = royaltorStatementBL.GetRoyStmtData(royaltorId, out errorId);
            royaltorStatementBL = null;
            if (royStmtData.Tables.Count != 0 && errorId != 2)
            {
                Session["RoyStmtChangesRoyData"] = royStmtData.Tables[0];
                gvRoyStmt.DataSource = royStmtData.Tables[0];
                if (royStmtData.Tables[0].Rows.Count == 0)
                {
                    gvRoyStmt.EmptyDataText = "No data found for the selected royaltor.";
                    hdnAddBtnEnable.Value = string.Empty;
                    hdnRemoveBtnEnable.Value = string.Empty;
                }
                gvRoyStmt.DataBind();
            }
            else if (royStmtData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyStmt.DataSource = dtEmpty;
                gvRoyStmt.EmptyDataText = "No data found for the selected royaltor.";
                gvRoyStmt.DataBind();
                hdnAddBtnEnable.Value = string.Empty;
                hdnRemoveBtnEnable.Value = string.Empty;
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

        }

        private void LoadOwnerGridData(Int32 ownerCode)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            royaltorStatementBL = new RoyaltorStatementBL();
            DataSet ownerStmtData = royaltorStatementBL.GetOwnerStmtData(ownerCode, out errorId);
            royaltorStatementBL = null;
            if (ownerStmtData.Tables.Count != 0 && errorId != 2)
            {
                //If there are royaltors, stmt periods to be added or removed then show owner record
                if (ownerStmtData.Tables[0].Rows.Count > 0)
                {
                    DataTable tempDt = ownerStmtData.Tables[0].Copy();
                    DataView dv = new DataView(tempDt);
                    DataTable dtResults = dv.ToTable(true, "owner_code", "owner_name", "statement_period_id", "statement_type_code", "stmt_period", "add_owner", "remove_owner");
                    Session["RoyStmtChangesOwnerData"] = dtResults;
                    gvOwnerStmt.DataSource = dtResults;
                    if (dtResults.Rows.Count == 0)
                    {
                        gvOwnerStmt.EmptyDataText = "No data found for the selected owner.";
                        hdnAddBtnEnable.Value = string.Empty;
                        hdnRemoveBtnEnable.Value = string.Empty;
                    }
                    gvOwnerStmt.DataBind();
                    Session["StmtsToBeProcessed"] = ownerStmtData.Tables[0];
                }
                else
                {
                    dtEmpty = new DataTable();
                    gvOwnerStmt.DataSource = dtEmpty;
                    gvOwnerStmt.EmptyDataText = "No data found for the selected owner.";
                    gvOwnerStmt.DataBind();
                    hdnAddBtnEnable.Value = string.Empty;
                    hdnRemoveBtnEnable.Value = string.Empty;
                }
            }
            else if (ownerStmtData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvOwnerStmt.DataSource = dtEmpty;
                gvOwnerStmt.EmptyDataText = "No data found for the selected owner.";
                gvOwnerStmt.DataBind();
                hdnAddBtnEnable.Value = string.Empty;
                hdnRemoveBtnEnable.Value = string.Empty;
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

        }

        private void EnableDisableButtons()
        {
            //enable disable Add/Remove buttons           
            if (hdnRemoveBtnEnable.Value.ToString() == "Y")
            {
                btnRemove.Enabled = true;
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }

        //private void PopUpConfirmation(string action)
        //{
        //    try
        //    {
        //        bool cbCheckedValidation = false;

        //        if (action == "Remove")
        //        {
        //            CheckBox cbRemove;

        //            //validation
        //            foreach (GridViewRow row in gvRoyStmt.Rows)
        //            {
        //                cbRemove = (CheckBox)row.FindControl("cbRemove");
        //                if (cbRemove.Checked == true)
        //                    cbCheckedValidation = true;
        //            }

        //            if (cbCheckedValidation == true)
        //            {
        //                //pop up message for confirmation to process the statements                    
        //                mpeConfirm.Show();
        //                lblConfirmMsg.Text = "Do you want to process the selected statements?";
        //            }
        //            else
        //            {
        //                //pop up message 
        //                msgView.SetMessage("Please select a statement period to process.", MessageType.Warning, PositionType.Auto);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler("Error in confirmation popup.", ex.Message);
        //    }
        //    finally
        //    {

        //    }

        //}

        private void PopUpConfirmation(string action)
        {
            try
            {
                bool cbCheckedValidation = false;

                if (action == "Remove")
                {
                    CheckBox cbRemove;

                    if (txtRoyaltor.Text != string.Empty)
                    {
                        //validation
                        foreach (GridViewRow row in gvRoyStmt.Rows)
                        {
                            cbRemove = (CheckBox)row.FindControl("cbRemove");
                            if (cbRemove.Checked == true)
                                cbCheckedValidation = true;
                        }

                        if (cbCheckedValidation == true)
                        {
                            //pop up message for confirmation to process the statements                    
                            mpeConfirm.Show();
                            lblConfirmMsg.Text = "Do you want to process the selected statements?";
                        }
                        else
                        {
                            //pop up message 
                            msgView.SetMessage("Please select a statement period to process.", MessageType.Warning, PositionType.Auto);
                        }
                    }
                    else if (txtOwner.Text != string.Empty)
                    {
                        //validation
                        foreach (GridViewRow row in gvOwnerStmt.Rows)
                        {
                            cbRemove = (CheckBox)row.FindControl("cbRemove");
                            if (cbRemove.Checked == true)
                                cbCheckedValidation = true;
                        }

                        if (cbCheckedValidation == true)
                        {
                            //pop up message for confirmation to process the statements                    
                            mpeConfirm.Show();
                            lblConfirmMsg.Text = "Do you want to process the selected statements?";
                        }
                        else
                        {
                            //pop up message 
                            msgView.SetMessage("Please select a statement period to process.", MessageType.Warning, PositionType.Auto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in confirmation popup.", ex.Message);
            }
            finally
            {

            }

        }

        private void UpdateRoyStmtRun()
        {
            List<int> stmtsToAdd = new List<int>();
            List<int> stmtsToRemove = new List<int>();

            CheckBox cbAdd;
            CheckBox cbRemove;
            string stmtPeriodID;

            //loop thorugh grid and get selected list of statement ids to be add/removed
            foreach (GridViewRow row in gvRoyStmt.Rows)
            {
                cbAdd = (CheckBox)row.FindControl("cbAdd");
                cbRemove = (CheckBox)row.FindControl("cbRemove");
                stmtPeriodID = (row.FindControl("hdnStmtPeriodID") as Label).Text;

                if (cbAdd.Enabled == true && cbAdd.Checked == true)
                    stmtsToAdd.Add(Convert.ToInt32(stmtPeriodID));
                else if (cbRemove.Enabled == true && cbRemove.Checked == true)
                    stmtsToRemove.Add(Convert.ToInt32(stmtPeriodID));

            }

            royaltorStatementBL = new RoyaltorStatementBL();
            string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
            royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));
            DataSet royStmtData = royaltorStatementBL.UpdateRoyStmt(royaltorId.ToString(), stmtsToAdd.ToArray(), stmtsToRemove.ToArray(), loggedUserID, out errorId);
            royaltorStatementBL = null;

            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;
            if (errorId == 1)
            {
                ExceptionHandler("Error in triggering statement removal job.", string.Empty);
                return;
            }
            else if (errorId == 2)
            {
                ExceptionHandler("Error in updating data.", string.Empty);
                return;
            }
            else if (royStmtData.Tables.Count != 0)
            {
                gvRoyStmt.DataSource = royStmtData.Tables[0];
                if (royStmtData.Tables[0].Rows.Count == 0)
                {
                    gvRoyStmt.EmptyDataText = "Data updated and no data found to display for the selected royaltor.";
                    hdnAddBtnEnable.Value = string.Empty;
                    hdnRemoveBtnEnable.Value = string.Empty;
                }
                gvRoyStmt.DataBind();

            }
            else if (royStmtData.Tables.Count == 0)
            {
                dtEmpty = new DataTable();
                gvRoyStmt.DataSource = dtEmpty;
                gvRoyStmt.EmptyDataText = "Data updated and no data found to display for the selected royaltor.";
                gvRoyStmt.DataBind();
                hdnAddBtnEnable.Value = string.Empty;
                hdnRemoveBtnEnable.Value = string.Empty;
            }
            else
            {
                ExceptionHandler("Error in updating data.", string.Empty);
            }

            EnableDisableButtons();
        }

        private void UpdateOwnerStmtRun()
        {
            if (gvOwnerStmt.Rows.Count > 0)
            {
                if (Session["StmtsToBeProcessed"] == null)
                {
                    ExceptionHandler("Error in updating data.", string.Empty);
                    return;
                }

                List<int> royaltors = new List<int>();
                List<int> stmtsToAdd = new List<int>();
                List<int> stmtsToRemove = new List<int>();

                CheckBox cbAdd;
                CheckBox cbRemove;
                string stmtPeriodID;

                DataTable dtStmtsToBeProcessed = Session["StmtsToBeProcessed"] as DataTable;

                //foreach (DataRow dRow in dtStmtsToBeProcessed.Rows)
                //{
                //    royaltors.Add(Convert.ToInt32(dRow["royaltor_id"].ToString()));
                //    stmtsToAdd.Add(Convert.ToInt32(dRow["statement_period_id"].ToString()));
                //    stmtsToRemove.Add(Convert.ToInt32(dRow["statement_period_id"].ToString()));
                //}

                //GridViewRow gvRow = gvOwnerStmt.Rows[0];
                //if ((gvRow.FindControl("cbAdd") as CheckBox).Checked)
                //{
                //    stmtsToRemove.Clear();
                //}
                //else if ((gvRow.FindControl("cbRemove") as CheckBox).Checked)
                //{
                //    stmtsToAdd.Clear();
                //}

                //loop thorugh grid and get selected list of statement ids to be add/removed
                foreach (GridViewRow row in gvOwnerStmt.Rows)
                {
                    cbAdd = (CheckBox)row.FindControl("cbAdd");
                    cbRemove = (CheckBox)row.FindControl("cbRemove");
                    stmtPeriodID = (row.FindControl("hdnStmtPeriodID") as Label).Text;

                    if (cbAdd.Enabled == true && cbAdd.Checked == true)
                    {
                        //loop through the datatable to get royaltor id
                        foreach (DataRow dRow in dtStmtsToBeProcessed.Rows)
                        {
                            if (dRow["statement_period_id"].ToString() == stmtPeriodID)
                            {
                                royaltors.Add(Convert.ToInt32(dRow["royaltor_id"].ToString()));
                                stmtsToAdd.Add(Convert.ToInt32(stmtPeriodID));
                            }
                        }
                    }
                    else if (cbRemove.Enabled == true && cbRemove.Checked == true)
                    {
                        foreach (DataRow dRow in dtStmtsToBeProcessed.Rows)
                        {
                            if (dRow["statement_period_id"].ToString() == stmtPeriodID)
                            {
                                royaltors.Add(Convert.ToInt32(dRow["royaltor_id"].ToString()));
                                stmtsToRemove.Add(Convert.ToInt32(stmtPeriodID));
                            }
                        }

                    }
                }

                royaltorStatementBL = new RoyaltorStatementBL();
                string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                int ownerCode = Convert.ToInt32(txtOwner.Text.Substring(0, txtOwner.Text.IndexOf("-") - 1));
                DataSet ownerStmtData = royaltorStatementBL.UpdateOwnerStmt(ownerCode, royaltors.ToArray(), stmtsToAdd.ToArray(), stmtsToRemove.ToArray(), loggedUserID, out errorId);
                royaltorStatementBL = null;
                if (ownerStmtData.Tables.Count != 0 && errorId != 2)
                {
                    //WUIN-746 clearing sort hidden files
                    hdnSortExpression.Value = string.Empty;
                    hdnSortDirection.Value = string.Empty;

                    //If there are royaltors, stmt periods to be added or removed then show owner record
                    if (ownerStmtData.Tables[0].Rows.Count > 0)
                    {
                        DataTable tempDt = ownerStmtData.Tables[0].Copy();
                        DataView dv = new DataView(tempDt);
                        DataTable dtResults = dv.ToTable(true, "owner_code", "owner_name", "statement_period_id", "statement_type_code", "stmt_period", "add_owner", "remove_owner");

                        gvOwnerStmt.DataSource = dtResults;
                        if (dtResults.Rows.Count == 0)
                        {
                            gvOwnerStmt.EmptyDataText = "No data found for the selected owner.";
                            hdnAddBtnEnable.Value = string.Empty;
                            hdnRemoveBtnEnable.Value = string.Empty;
                        }
                        gvOwnerStmt.DataBind();
                        Session["StmtsToBeProcessed"] = ownerStmtData.Tables[0];
                    }
                    else
                    {
                        dtEmpty = new DataTable();
                        gvOwnerStmt.DataSource = dtEmpty;
                        gvOwnerStmt.EmptyDataText = "Data updated and no data found to display for the selected owner.";
                        gvOwnerStmt.DataBind();
                        hdnAddBtnEnable.Value = string.Empty;
                        hdnRemoveBtnEnable.Value = string.Empty;
                    }

                }
                else if (ownerStmtData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvOwnerStmt.DataSource = dtEmpty;
                    gvOwnerStmt.EmptyDataText = "Data updated and no data found to display for the selected owner.";
                    gvOwnerStmt.DataBind();
                    hdnAddBtnEnable.Value = string.Empty;
                    hdnRemoveBtnEnable.Value = string.Empty;
                }
                else
                {
                    ExceptionHandler("Error in updating data.", string.Empty);
                }

                EnableDisableButtons();
            }
        }

        private bool ValidateSelectedRoyaltorFilter()
        {
            if (txtRoyaltor.Text != "" && Session["FuzzySearchAllRoyaltorList"] != null)
            {
                if (txtRoyaltor.Text != "No results found")
                {
                    DataTable stmtChangesRoyaltors;
                    stmtChangesRoyaltors = Session["FuzzySearchAllRoyaltorList"] as DataTable;

                    foreach (DataRow dRow in stmtChangesRoyaltors.Rows)
                    {
                        if (dRow["royaltor"].ToString() == txtRoyaltor.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool ValidateSelectedOwnerFilter()
        {
            if (txtOwner.Text != "" && Session["StmtChangesOwnerList"] != null)
            {
                if (txtOwner.Text != "No results found")
                {
                    DataTable stmtChangesOwners;
                    stmtChangesOwners = Session["StmtChangesOwnerList"] as DataTable;

                    foreach (DataRow dRow in stmtChangesOwners.Rows)
                    {
                        if (dRow["owners"].ToString() == txtOwner.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void FuzzySearchRoyaltor()
        {
            
            if (txtRoyaltor.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                LoadInitialGridData();
                btnRemove.Enabled = false;
                return;
            }

            hdnFuzzySearchField.Value = "Royaltor";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(txtRoyaltor.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchOwner()
        {
            
            if (txtOwner.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in owner filter field", MessageType.Warning, PositionType.Auto);
                LoadInitialOwnerGridData();
                btnRemove.Enabled = false;
                return;
            }
                       
            hdnFuzzySearchField.Value = "Owner";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyStmtChangesOwnerList(txtOwner.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        #endregion METHODS

        
    }
}