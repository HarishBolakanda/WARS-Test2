/*
File Name   :   RoyaltorSearch.cs
Purpose     :   to search, lock/unlock, create contracts of royaltors

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     14-Mar-2015     Harish(Infosys Limited)   Initial Creation
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
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text;
using System.Threading;


namespace WARS
{
    public partial class RoyaltorSearch : System.Web.UI.Page
    {

        #region Global Declarations
        RoyaltorSearchBL roySearchBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Search";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Search";
                }

                lblTab.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {
                    txtRoyaltor.Focus();
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        PopulateDropDowns();
                        LoadEmptyGrid();
                        UserAuthorization();
                        if (Request.QueryString != null && Request.QueryString.Count > 0)
                        {
                            hdnIsNewRequest.Value = Request.QueryString[0];

                            if (hdnIsNewRequest.Value == "N")
                            {
                                if (Session["RSSearchedFilters"] != null)
                                {
                                    DataTable dtSearchedFilters = Session["RSSearchedFilters"] as DataTable;

                                    foreach (DataRow dRow in dtSearchedFilters.Rows)
                                    {
                                        if (dRow["filter_name"].ToString() == "txtRoyaltor")
                                        {
                                            txtRoyaltor.Text = dRow["filter_value"].ToString();
                                        }
                                        else if (dRow["filter_name"].ToString() == "txtPlgRoyaltor")
                                        {
                                            txtPlgRoyaltor.Text = dRow["filter_value"].ToString();
                                        }
                                        else if (dRow["filter_name"].ToString() == "cbCompanySelected")
                                        {
                                            if (dRow["filter_value"].ToString() == "Y")
                                                cbCompanySelected.Checked = true;
                                            else
                                                cbCompanySelected.Checked = false;
                                        }
                                        else if (dRow["filter_name"].ToString() == "txtOwner")
                                        {
                                            txtOwner.Text = dRow["filter_value"].ToString();
                                        }
                                        else if (dRow["filter_name"].ToString() == "ddlResponsibility")
                                        {
                                            ddlResponsibility.SelectedValue = dRow["filter_value"].ToString();
                                        }
                                        else if (dRow["filter_name"].ToString() == "ddlStatus")
                                        {
                                            ddlStatus.SelectedValue = dRow["filter_value"].ToString();
                                        }
                                        else if (dRow["filter_name"].ToString() == "cbRoyaltorHeld")
                                        {
                                            if (dRow["filter_value"].ToString() == "Y")
                                                cbRoyaltorHeld.Checked = true;
                                            else
                                                cbRoyaltorHeld.Checked = false;
                                        }
                                        else if (dRow["filter_name"].ToString() == "ddlContractType")
                                        {
                                            ddlContractType.SelectedValue = dRow["filter_value"].ToString();
                                        }
                                        else if (dRow["filter_name"].ToString() == "txtUploadRoyList")
                                        {
                                            txtUploadRoyList.Text = dRow["filter_value"].ToString();
                                        }
                                    }
                                }
                                LoadGridData();
                            }
                        }

                        //WUIN-599 Clearning session values
                        Session["ScreenLockedRoyaltorId"] = null;
                        Session["ContractScreenLockedUser"] = null;
                        Session["ContractScreenLocked"] = null;
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

        protected void fuzzySearchOwner_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchOwner();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void btnAddRoyaltor_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Contract/RoyaltorContract.aspx", false);
        }

        protected void btnLockContracts_Click(object sender, EventArgs e)
        {
            if (gvRoyaltors.Rows.Count == 0)
            {
                msgView.SetMessage("Please select royaltors to be locked", MessageType.Warning, PositionType.Auto);
                return;
            }
            else
            {
                CheckBox cbRoyChecked;
                bool isChecked = false;

                foreach (GridViewRow row in gvRoyaltors.Rows)
                {
                    cbRoyChecked = (CheckBox)row.FindControl("cbRoyChecked");

                    if (cbRoyChecked.Checked == true)
                    {
                        isChecked = true;
                        break;
                    }
                }

                if (isChecked == false)
                {
                    msgView.SetMessage("Please select royaltors to be locked", MessageType.Warning, PositionType.Auto);
                    return;
                }
            }

            hdnLockUnlock.Value = "Lock";
            mpeConfirm.Show();
            lblConfirmMsg.Text = "This will lock all selected Contracts.  Do you want to continue?";
        }

        protected void btnUnLockContracts_Click(object sender, EventArgs e)
        {
            if (gvRoyaltors.Rows.Count == 0)
            {
                msgView.SetMessage("Please select royaltors to be unlocked", MessageType.Warning, PositionType.Auto);
                return;
            }
            else
            {
                CheckBox cbRoyChecked;
                bool isChecked = false;

                foreach (GridViewRow row in gvRoyaltors.Rows)
                {
                    cbRoyChecked = (CheckBox)row.FindControl("cbRoyChecked");

                    if (cbRoyChecked.Checked == true)
                    {
                        isChecked = true;
                        break;
                    }
                }

                if (isChecked == false)
                {
                    msgView.SetMessage("Please select royaltors to be unlocked", MessageType.Warning, PositionType.Auto);
                    return;
                }
            }

            hdnLockUnlock.Value = "UnLock";
            mpeConfirm.Show();
            lblConfirmMsg.Text = "This will unlock all selected Contracts.  Do you want to continue?";
        }

        protected void btnLockAllContracts_Click(object sender, EventArgs e)
        {
            hdnButtonText.Value = "LockAll";
            hdnLockUnlock.Value = "Y";
            mpeConfirm.Show();
            lblConfirmMsg.Text = "This will lock all Contracts.  Do you want to continue?";
        }

        protected void btnUnlockAllContracts_Click(object sender, EventArgs e)
        {
            hdnButtonText.Value = "UnLockAll";
            hdnLockUnlock.Value = "N";
            mpeConfirm.Show();
            lblConfirmMsg.Text = "This will unlock all Contracts.  Do you want to continue?";
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {

                List<string> royaltors = new List<string>();
                CheckBox cbRoyChecked;
                string royaltorId;

                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                string royaltor = (txtRoyaltor.Text == "" ? string.Empty : txtRoyaltor.Text.Replace("'", "").Trim()); //JIRA-1048 --Changes to handle single quote
                string plgRoyaltor = (txtPlgRoyaltor.Text == "" ? string.Empty : txtPlgRoyaltor.Text.Replace("'", "").Trim());//JIRA-1048 --Changes to handle single quote
                string ownerCode = (txtOwner.Text == "" ? string.Empty : txtOwner.Text.Split('-')[0].Trim());
                string isCompanySelected = cbCompanySelected.Checked == true ? "Y" : "N";
                string isRoyaltorHeld = cbRoyaltorHeld.Checked == true ? "Y" : "N";
                string lockUnlock = string.Empty;
                string lockUnlockAll = "N";
                if (!(hdnButtonText.Value == "LockAll" || hdnButtonText.Value == "UnLockAll" || hdnButtonText.Value == "UpdateStatus"))
                {
                    foreach (GridViewRow row in gvRoyaltors.Rows)
                    {
                        cbRoyChecked = (CheckBox)row.FindControl("cbRoyChecked");
                        royaltorId = (row.FindControl("hdnRoyaltorId") as HiddenField).Value;

                        if (cbRoyChecked.Checked == true)
                            royaltors.Add(royaltorId);

                    }

                    if (hdnLockUnlock.Value == "Lock")
                    {
                        lockUnlock = "Y";
                    }
                    else if (hdnLockUnlock.Value == "UnLock")
                    {
                        lockUnlock = "N";
                    }

                    roySearchBL = new RoyaltorSearchBL();
                    DataSet roySearchData = roySearchBL.UpdateRoyaltor(royaltor, plgRoyaltor, ownerCode, isCompanySelected, ddlResponsibility.SelectedValue, ddlStatus.SelectedValue, isRoyaltorHeld, ddlContractType.SelectedValue,
                                                                       royaltors.ToArray(), lockUnlock, lockUnlockAll, hdnUpdateStatus.Value, Convert.ToString(Session["UserCode"]), out errorId);
                    roySearchBL = null;

                    if (roySearchData.Tables.Count != 0 && errorId != 2)
                    {
                        if (roySearchData.Tables[0].Rows.Count == 0)
                        {
                            gvRoyaltors.DataSource = roySearchData.Tables[0];
                            gvRoyaltors.EmptyDataText = "No data found for the selected filter criteria";
                            gvRoyaltors.DataBind();
                        }
                        else
                        {
                            gvRoyaltors.DataSource = roySearchData.Tables[0];
                            gvRoyaltors.DataBind();
                        }

                    }
                    else if (roySearchData.Tables.Count == 0 && errorId != 2)
                    {
                        dtEmpty = new DataTable();
                        gvRoyaltors.DataSource = dtEmpty;
                        gvRoyaltors.EmptyDataText = "No data found for the selected filter criteria";
                        gvRoyaltors.DataBind();
                    }
                    else
                    {
                        ExceptionHandler("Error in loading grid data", string.Empty);
                        return;
                    }

                    if (hdnLockUnlock.Value == "Lock")
                    {
                        msgView.SetMessage("Selected royaltors have been locked", MessageType.Warning, PositionType.Auto);
                    }
                    else if (hdnLockUnlock.Value == "UnLock")
                    {
                        msgView.SetMessage("Selected royaltors have been unlocked", MessageType.Warning, PositionType.Auto);
                    }

                }
                else
                {
                    if (hdnButtonText.Value == "UpdateStatus")
                    {
                        foreach (GridViewRow row in gvRoyaltors.Rows)
                        {
                            royaltorId = (row.FindControl("hdnRoyaltorId") as HiddenField).Value;
                            royaltors.Add(royaltorId);
                        }
                    }
                    else
                    {
                        lockUnlockAll = "Y";
                    }


                    roySearchBL = new RoyaltorSearchBL();
                    DataSet roySearchData = roySearchBL.UpdateRoyaltor(royaltor, plgRoyaltor, ownerCode, isCompanySelected, ddlResponsibility.SelectedValue, ddlStatus.SelectedValue, isRoyaltorHeld, ddlContractType.SelectedValue,
                                                                        royaltors.ToArray(), hdnLockUnlock.Value, lockUnlockAll, hdnUpdateStatus.Value, Convert.ToString(Session["UserCode"]), out errorId);
                    roySearchBL = null;

                    if (errorId == 2)
                    {
                        ExceptionHandler("Error in updating royaltors", string.Empty);
                        return;
                    }

                    if (roySearchData.Tables.Count != 0)
                    {
                        if (roySearchData.Tables[0].Rows.Count == 0)
                        {
                            gvRoyaltors.DataSource = roySearchData.Tables[0];
                            gvRoyaltors.EmptyDataText = "No data found for the selected filter criteria";
                            gvRoyaltors.DataBind();
                        }
                        else
                        {
                            gvRoyaltors.DataSource = roySearchData.Tables[0];
                            gvRoyaltors.DataBind();
                        }

                    }
                    else if (roySearchData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvRoyaltors.DataSource = dtEmpty;
                        gvRoyaltors.EmptyDataText = "No data found for the selected filter criteria";
                        gvRoyaltors.DataBind();
                    }
                    else
                    {
                        ExceptionHandler("Error in loading grid data", string.Empty);
                        return;
                    }

                    if (hdnButtonText.Value == "LockAll")
                    {
                        msgView.SetMessage("All royaltors are locked", MessageType.Warning, PositionType.Auto);
                    }
                    else if (hdnButtonText.Value == "UnLockAll")
                    {
                        msgView.SetMessage("All royaltors are unlocked", MessageType.Warning, PositionType.Auto);
                    }

                    hdnButtonText.Value = string.Empty;
                }

                hdnLockUnlock.Value = string.Empty;

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in locking/unlocking royaltors", ex.Message);
            }
            finally
            {
                mpeConfirm.Hide();
            }
        }

        protected void cbCompanySelected_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //Making this hidden field value to Y so that if we get a single record after search then it will redirect to next screen
                hdnIsNewRequest.Value = "Y";

                //load grid data only if any of the other filters are selected
                if (txtRoyaltor.Text != string.Empty || txtPlgRoyaltor.Text != string.Empty || txtOwner.Text != string.Empty ||
                   ddlResponsibility.SelectedIndex != 0 || ddlStatus.SelectedIndex != 0 || ddlContractType.SelectedIndex != 0)
                {
                    LoadGridData();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void cbRoyaltorHeld_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //Making this hidden field value to Y so that if we get a single record after search then it will redirect to next screen
                hdnIsNewRequest.Value = "Y";

                //load grid data only if any of the other filters are selected
                if (txtRoyaltor.Text != string.Empty || txtPlgRoyaltor.Text != string.Empty || txtOwner.Text != string.Empty ||
                   ddlResponsibility.SelectedIndex != 0 || ddlStatus.SelectedIndex != 0 || ddlContractType.SelectedIndex != 0)
                {
                    LoadGridData();
                }



            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //Making this hidden field value to Y so that if we get a single record after search then it will redirect to next screen
                hdnIsNewRequest.Value = "Y";
                LoadGridData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtOwner.Text = string.Empty;
                    LoadEmptyGrid();
                    return;
                }

                txtOwner.Text = lbFuzzySearch.SelectedValue.ToString();
                txtRoyaltor.Text = string.Empty;
                txtPlgRoyaltor.Text = string.Empty;
                hdnIsValidOwner.Value = "Y";
                LoadGridData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void gvRoyaltors_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                e.Row.Cells[9].Visible = false;
                LinkButton _dblClickButton = e.Row.FindControl("lnkBtnDblClk") as LinkButton;
                string _jsDoubleClick = ClientScript.GetPostBackClientHyperlink(_dblClickButton, "");
                e.Row.Attributes.Add("ondblclick", _jsDoubleClick);
                HiddenField hdnRoyaltorLocked = e.Row.FindControl("hdnRoyaltorLocked") as HiddenField;
                Label lblRoyaltorId = e.Row.FindControl("lblRoyaltorId") as Label;
                Image imgLock = e.Row.FindControl("imgLock") as Image;

                if (hdnRoyaltorLocked.Value == "Y")
                {
                    imgLock.Visible = true;
                }
                else
                {
                    imgLock.Visible = false;
                }
            }
            //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[9].Visible = false;

                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.HasControls() && tc.Controls.Count == 1)
                    {

                        var obj = tc.Controls[0];
                        LinkButton lnkHeader = (LinkButton)tc.Controls[0];
                        var hear = lnkHeader.Text;
                        lnkHeader.Style.Add("color", "black");
                        lnkHeader.Style.Add("text-decoration", "none");

                        if (lnkHeader != null && hdnSortExpression.Value == lnkHeader.CommandArgument)
                        {
                            // initialize a new image
                            System.Web.UI.WebControls.Image imgSort = new System.Web.UI.WebControls.Image();
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

        protected void gvRoyaltors_RowCommand(object sender, GridViewCommandEventArgs e)
        {


            //if (e.CommandName == "clk")
            //{
            //    row.BackColor = System.Drawing.Color.LightBlue;

            //}

            if (e.CommandName == "dblClk")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                string royaltorId = (row.FindControl("hdnRoyaltorId") as HiddenField).Value;
                Response.Redirect(@"~/Contract/RoyaltorContract.aspx?RoyaltorId=" + royaltorId, false);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                txtRoyaltor.Text = string.Empty;
                txtPlgRoyaltor.Text = string.Empty;
                cbCompanySelected.Checked = false;
                txtOwner.Text = string.Empty;
                ddlResponsibility.SelectedIndex = 0;
                LoadEmptyGrid();
                ddlStatus.SelectedIndex = 0;
                cbRoyaltorHeld.Checked = false;
                ddlContractType.SelectedIndex = 0;
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                Session["RSSearchedFilters"] = null;
                Session["RoySrchRoyDetails"] = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reseting.", ex.Message);
            }
        }

        protected void gvRoyaltors_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                if (Session["RoySrchRoyDetails"] == null)
                    return;

                DataTable dtRoySearchDetails = Session["RoySrchRoyDetails"] as DataTable;

                gvRoyaltors.PageIndex = e.NewPageIndex;

                if (dtRoySearchDetails.Rows.Count > 0)
                {
                    gvRoyaltors.DataSource = dtRoySearchDetails;
                    gvRoyaltors.DataBind();
                }
                else
                {
                    LoadEmptyGrid();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }


        protected void btnSearchBulkUpload_Click(object sender, EventArgs e)
        {

            try
            {
                //validate Royaltor entered 
                string[] arrRoyNos = txtUploadRoyList.Text.ToUpper().Trim().Replace("\r\n", string.Empty).Split(';').ToArray();
                foreach (string royNo in arrRoyNos)
                {
                    //check if entered Royaltor is greater than the royaltor.royaltor_id column definition
                    if (royNo.Length > 5)
                    {
                        lblUploadRoyListError.Visible = true;
                        mpeUploadRoyList.Show();
                        return;

                    }
                }

                //Making this hidden field value to Y so that if we get a single record after search then it will redirect to next screen
                hdnIsNewRequest.Value = "Y";

                //clear search fields
                txtRoyaltor.Text = string.Empty;
                txtPlgRoyaltor.Text = string.Empty;
                cbCompanySelected.Checked = false;
                txtOwner.Text = string.Empty;
                ddlResponsibility.SelectedIndex = 0;
                ddlStatus.SelectedIndex = 0;
                ddlContractType.SelectedIndex = 0;
                cbRoyaltorHeld.Checked = false;
                LoadGridData();

                txtUploadRoyList.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in searching uploaded catalogue numbers", ex.Message);
            }
        }
        protected void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtRoySearch = Session["RoySrchRoyDetails"] as DataTable;

                if (dtRoySearch == null || dtRoySearch.Rows.Count == 0)
                {
                    msgView.SetMessage("No royaltors available to update!", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    ddlUpdateStatus.SelectedIndex = 0;
                    mpeUpdateStatus.Show();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating royaltor status", ex.Message);
            }
        }

        protected void bthUpdateRoyStatusPopup_Click(object sender, EventArgs e)
        {
            try
            {
                //WUIN - 985 Supervisor can select to Manager sigoff only.
                if (Session["UserRole"].ToString().ToLower() == UserRole.Supervisor.ToString().ToLower())
                {
                    if (ddlUpdateStatus.SelectedValue != "3")
                    {
                        msgView.SetMessage("Supervisor can change status to Manger Sign Off only!", MessageType.Success, PositionType.Auto);
                        return;
                    }
                }

                hdnButtonText.Value = "UpdateStatus";
                hdnUpdateStatus.Value = ddlUpdateStatus.SelectedValue;
                mpeConfirm.Show();
                lblConfirmMsg.Text = "This will update status to all searched contracts.  Do you want to continue?";

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating royaltor status", ex.Message);
            }
        }

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvRoyaltors_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoySrchRoyDetails"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;


                gvRoyaltors.PageIndex = 0;
                gvRoyaltors.DataSource = dataView;
                gvRoyaltors.DataBind();
                Session["RoySrchRoyDetails"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion Events

        #region Methods
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void LoadGridData()
        {
            //if (txtRoyaltor.Text == "" && txtOwner.Text == "" && txtPlgRoyaltor.Text == ""
            //    && ddlResponsibility.SelectedIndex == 0 && ddlStatus.SelectedIndex == 0)
            //{
            //    LoadEmptyGrid();
            //    return;
            //}

            //Create a table to hold the filter values
            DataTable dtSearchedFilters = new DataTable();
            dtSearchedFilters.Columns.Add("filter_name", typeof(string));
            dtSearchedFilters.Columns.Add("filter_value", typeof(string));

            //Add the filter values to the above created table
            dtSearchedFilters.Rows.Add("txtRoyaltor", txtRoyaltor.Text);
            dtSearchedFilters.Rows.Add("txtPlgRoyaltor", txtPlgRoyaltor.Text);
            if (cbCompanySelected.Checked)
            {
                dtSearchedFilters.Rows.Add("cbCompanySelected", "Y");
            }
            dtSearchedFilters.Rows.Add("txtOwner", txtOwner.Text);
            dtSearchedFilters.Rows.Add("ddlResponsibility", ddlResponsibility.SelectedValue);
            dtSearchedFilters.Rows.Add("ddlStatus", ddlStatus.SelectedValue);

            if (cbRoyaltorHeld.Checked)
            {
                dtSearchedFilters.Rows.Add("cbRoyaltorHeld", "Y");
            }
            dtSearchedFilters.Rows.Add("ddlContractType", ddlContractType.SelectedValue);

            dtSearchedFilters.Rows.Add("txtUploadRoyList", txtUploadRoyList.Text);

            Session["RSSearchedFilters"] = dtSearchedFilters;

            //set gridview panel height                    
            PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
            //JIRA-1048 Changes to handle single quote while searching --Start
            string royaltor = (txtRoyaltor.Text == "" ? string.Empty : txtRoyaltor.Text.Replace("'", "").Trim());
            string plgRoyaltor = (txtPlgRoyaltor.Text == "" ? string.Empty : txtPlgRoyaltor.Text.Replace("'", "").Trim());
            //JIRA-1048 Changes to handle single quote while searching --End
            string ownerCode = (txtOwner.Text == "" ? string.Empty : txtOwner.Text.Split('-')[0].Trim());
            string isCompanySelected = cbCompanySelected.Checked == true ? "Y" : "N";
            string isRoyaltorHeld = cbRoyaltorHeld.Checked == true ? "Y" : "N";

            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            roySearchBL = new RoyaltorSearchBL();
            DataSet roySearchData = roySearchBL.GetSearchData(royaltor, plgRoyaltor, ownerCode, isCompanySelected, ddlResponsibility.SelectedValue, ddlStatus.SelectedValue, isRoyaltorHeld, ddlContractType.SelectedValue, txtUploadRoyList.Text.ToUpper().Trim().Replace("\r\n", string.Empty).Split(';').ToArray(), out errorId);
            roySearchBL = null;

            
            if (roySearchData.Tables.Count != 0 && errorId != 2)
            {
                if (roySearchData.Tables[0].Rows.Count == 0)
                {
                    Session["RoySrchRoyDetails"] = null;
                    gvRoyaltors.DataSource = roySearchData.Tables[0];
                    gvRoyaltors.EmptyDataText = "No data found for the selected filter criteria";
                    gvRoyaltors.DataBind();
                }
                else
                {
                    Session["RoySrchRoyDetails"] = roySearchData.Tables[0];
                    gvRoyaltors.DataSource = roySearchData.Tables[0];
                    gvRoyaltors.DataBind();

                    if (gvRoyaltors.Rows.Count == 1 && hdnIsNewRequest.Value == "Y")
                    {
                        Response.Redirect(@"~/Contract/RoyaltorContract.aspx?RoyaltorId=" + roySearchData.Tables[0].Rows[0]["royaltor_id"], false);
                    }
                }

            }
            else if (roySearchData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyaltors.DataSource = dtEmpty;
                gvRoyaltors.EmptyDataText = "No data found for the selected filter criteria";
                gvRoyaltors.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }
        }

        private void LoadEmptyGrid()
        {
            dtEmpty = new DataTable();
            gvRoyaltors.EmptyDataText = "<br />";
            gvRoyaltors.DataSource = dtEmpty;
            gvRoyaltors.DataBind();

            //WUIN-985 Clearing Searched Details from Session
            Session["RoySrchRoyDetails"] = null;
        }

        private void PopulateDropDowns()
        {

            roySearchBL = new RoyaltorSearchBL();
            DataSet dropdownListData = roySearchBL.GetDropdownData(out errorId);
            roySearchBL = null;

            if (dropdownListData.Tables.Count != 0 && errorId != 2)
            {

                //responsibility dropdown
                ddlResponsibility.DataTextField = "responsibility";
                ddlResponsibility.DataValueField = "responsibility_code";
                ddlResponsibility.DataSource = dropdownListData.Tables[0];
                ddlResponsibility.DataBind();
                ddlResponsibility.Items.Insert(0, new ListItem("-"));

                //status dropdown
                ddlStatus.DataTextField = "status_desc";
                ddlStatus.DataValueField = "status_code";
                ddlStatus.DataSource = dropdownListData.Tables[1];
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0, new ListItem("-"));

                //contract type dropdown
                ddlContractType.DataTextField = "contract_type";
                ddlContractType.DataValueField = "contract_type_code";
                ddlContractType.DataSource = dropdownListData.Tables[2];
                ddlContractType.DataBind();
                ddlContractType.Items.Insert(0, new ListItem("-"));

                //Update status dropdown
                ddlUpdateStatus.DataTextField = "status_desc";
                ddlUpdateStatus.DataValueField = "status_code";
                ddlUpdateStatus.DataSource = dropdownListData.Tables[1];
                ddlUpdateStatus.DataBind();
                ddlUpdateStatus.Items.Insert(0, new ListItem("-"));

            }
            else
            {
                ExceptionHandler("Error in loading the dropdown list values", string.Empty);
            }

        }

        private void FuzzySearchOwner()
        {
            if (txtOwner.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text owner search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllOwnerList(txtOwner.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        /// <summary>
        ///WUIN-568 - Only superusers should be able to unlock and lock contracts        
        /// </summary>
        private void UserAuthorization()
        {

            if (Session["UserRole"].ToString().ToLower() != UserRole.SuperUser.ToString().ToLower())
            {
                btnLockContracts.Enabled = false;
                btnUnLockContracts.Enabled = false;
                btnLockAllContracts.Enabled = false;
                btnUnlockAllContracts.Enabled = false;
                btnUpdateStatus.Enabled = false;
                // WUIN-985 Only Superuser / Surpervisor can user bulk status update
                if (Session["UserRole"].ToString().ToLower() == UserRole.Supervisor.ToString().ToLower()) //supervisor
                {
                    btnUpdateStatus.Enabled = true;
                }
                else if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()) //supervisor
                {
                    btnAddRoyaltor.Enabled = false;
                }
            }
            else
            {
                btnUpdateStatus.Enabled = true;
            }


        }

        #endregion Methods



    }
}