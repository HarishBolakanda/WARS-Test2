/*
File Name   :   RoyaltorCosts.cs
Purpose     :   to maintain royaltor costs

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     10-Jun-2016     Harish(Infosys Limited)   Initial Creation
2.0     08-Feb-2017     Pratik(Infosys Limited)   Added new fuzzy search functionality
3.0     17-Apr-2018     Harish                    Alter the cost maintenance screen for amending many existing rows
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
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Web.SessionState;
using System.Text;

namespace WARS
{
    public partial class RoyaltorCosts : System.Web.UI.Page
    {
        #region Global Declarations
        string loggedUserID;
        RoyaltorCostsBL royCostsBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string fromDate = string.Empty, toDate = string.Empty;
        string DBDelimiter = " " + Convert.ToChar(0); //Blank space is added to the delimiter so that two delimiters won't be considered as one by oracle if there is no data present between them
        #endregion Global Declarations

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Costs";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Costs";
                }

                //txtRoyaltor.Focus();//tabbing sequence starts here
                lblTab.Focus();

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        hdnUserRole.Value = Session["UserRole"].ToString();
                        tdGrid.Visible = false;
                        btnSave.Visible = false;
                        PopulateDropDowns();
                        LoadInitialGridData();
                        txtRoyaltor.Focus();//tabbing sequence starts here
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;

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

        protected void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                //validate 
                if (!Page.IsValid)
                {
                    LoadInitialGridData();
                    tdGrid.Visible = false;
                    btnSave.Visible = false;
                    return;
                }

                if (txtRoyaltor.Text == string.Empty || txtRoyaltor.Text == "No results found" || hdnRoyaltorSelected.Value == "N")
                {
                    msgView.SetMessage("Please select a valid royaltor from the list", MessageType.Warning, PositionType.Auto);
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialStmtPeriod();
                    LoadInitialGridData();
                    return;
                }

                if ((ddlStmtPeriod.SelectedIndex != 0 && ((txtFromDate.Text != string.Empty && txtFromDate.Text != "__/____") ||
                    (txtToDate.Text != string.Empty && txtToDate.Text != "__/____"))))
                {
                    msgView.SetMessage("Please select either statement period or date range", MessageType.Warning, PositionType.Auto);
                    LoadInitialGridData();
                    return;
                }

                LoadSearchData();
                ClearAddRowFields();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Go button click.", ex.Message);
            }


        }
        
        protected void btnYesSearch_Click(object sender, EventArgs e)
        {
            try
            {
                /* Harish - 27-04-18 - commented as this is not same as GO button validations
                if (txtRoyaltor.Text == "")
                {
                    msgView.SetMessage("Please select royaltor", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if ((ddlStmtPeriod.SelectedIndex != 0 && (txtFromDate.Text != string.Empty || txtToDate.Text != string.Empty)))
                {
                    msgView.SetMessage("Please select either statement period or date range", MessageType.Warning, PositionType.Auto);
                    return;
                }
                 * */

                if (txtRoyaltor.Text == string.Empty || txtRoyaltor.Text == "No results found" || hdnRoyaltorSelected.Value == "N")
                {
                    msgView.SetMessage("Please select a valid royaltor from the list", MessageType.Warning, PositionType.Auto);
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialStmtPeriod();
                    LoadInitialGridData();
                    return;
                }

                if ((ddlStmtPeriod.SelectedIndex != 0 && ((txtFromDate.Text != string.Empty && txtFromDate.Text != "__/____") ||
                    (txtToDate.Text != string.Empty && txtToDate.Text != "__/____"))))
                {
                    msgView.SetMessage("Please select either statement period or date range", MessageType.Warning, PositionType.Auto);
                    LoadInitialGridData();
                    return;
                }

                LoadSearchData();
                ClearAddRowFields();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in yes button click.", ex.Message);
            }

        }

        protected void txtRoyaltor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtToDate.Text = string.Empty;
                txtFromDate.Text = string.Empty;

                //if (txtRoyaltor.Text == string.Empty || txtRoyaltor.Text == "No results found" || hdnRoyaltorSelected.Value == "N")
                //{
                //    msgView.SetMessage("Please select a valid royaltor from the list",MessageType.Warning, PositionType.Auto);
                //    txtRoyaltor.Text = string.Empty;
                //    LoadInitialStmtPeriod();
                //    LoadInitialGridData();
                //    return;

                //}
                //else
                //{
                //    LoadStmtPeriodData();
                //}
                if (hdnRoyaltorSelected.Value == "Y")
                {
                    RoyaltorSelected();
                }
                else if (hdnRoyaltorSelected.Value == "N")
                {
                    FuzzySearchRoyaltor();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                }
                else
                {
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialStmtPeriod();
                    LoadInitialGridData();
                    return;
                }                
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }

        protected void gvRoyCosts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string accTypeId;
                string isDeleted;
                
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    accTypeId = (e.Row.FindControl("hdnAccTypeId") as HiddenField).Value;

                    if (accTypeId == "")//Group header row
                    {
                        e.Row.Cells[0].CssClass = "costGridHeaderrowStyle";
                        (e.Row.FindControl("txtTranDesc") as TextBox).Visible = false;
                        (e.Row.FindControl("txtDate") as TextBox).Visible = false;
                        (e.Row.FindControl("txtAmount") as TextBox).Visible = false;
                        (e.Row.FindControl("txtSuppName") as TextBox).Visible = false;
                        (e.Row.FindControl("txtProjCode") as TextBox).Visible = false;
                        (e.Row.FindControl("txtInvoiceNum") as TextBox).Visible = false;                        
                        (e.Row.FindControl("imgBtnCancel") as ImageButton).Visible = false;
                        (e.Row.FindControl("cbDisplay") as CheckBox).Visible = false;
                        (e.Row.FindControl("rfvDescEdit") as RequiredFieldValidator).Visible = false;
                        (e.Row.FindControl("rfvDateEdit") as RequiredFieldValidator).Visible = false;
                        (e.Row.FindControl("regDateEdit") as RegularExpressionValidator).Visible = false;                        
                        (e.Row.FindControl("valAmountEdit") as CustomValidator).Visible = false;//WUIN-511
                        
                    }

                    isDeleted = (e.Row.FindControl("hdnIsDeleted") as HiddenField).Value;
                    if (isDeleted == "N")
                    {
                        (e.Row.FindControl("cbDisplay") as CheckBox).Checked = true;
                    }
                    else
                    {
                        (e.Row.FindControl("cbDisplay") as CheckBox).Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data.", ex.Message);
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
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialGridData();
                    LoadStmtPeriodData();
                    return;
                }
                txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                hdnRoyaltorSelected.Value = "Y";
                RoyaltorSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from search list", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtRoyaltor.Text = string.Empty;
                LoadInitialStmtPeriod();
                LoadInitialGridData();
                mpeFuzzySearch.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing search list", ex.Message);
            }
        }
        
        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {  
                //validate 
                Page.Validate("valSearch");

                if (!Page.IsValid)
                {
                    LoadInitialGridData();
                    tdGrid.Visible = false;
                    btnSave.Visible = false;
                    return;
                }

                if (txtRoyaltor.Text == string.Empty || txtRoyaltor.Text == "No results found" || hdnRoyaltorSelected.Value == "N")
                {
                    msgView.SetMessage("Please select a valid royaltor from the list", MessageType.Warning, PositionType.Auto);
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialStmtPeriod();
                    LoadInitialGridData();
                    return;
                }

                if ((ddlStmtPeriod.SelectedIndex != 0 && ((txtFromDate.Text != string.Empty && txtFromDate.Text != "__/____") ||
                    (txtToDate.Text != string.Empty && txtToDate.Text != "__/____"))))
                {
                    msgView.SetMessage("Please select either statement period or date range", MessageType.Warning, PositionType.Auto);
                    LoadInitialGridData();
                    return;
                }


                LoadSearchData();
                ClearAddRowFields();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading data grid.", ex.Message);
            }
        }
               
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (txtRoyaltor.Text == string.Empty || txtRoyaltor.Text == "No results found" || hdnRoyaltorSelected.Value == "N")
                {
                    msgView.SetMessage("Cost details not saved – Please select a valid royaltor from the list", MessageType.Warning, PositionType.Auto);
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialStmtPeriod();                    
                    return;
                }

                //validate date fields
                Page.Validate("valSearch");

                if (!Page.IsValid)
                {
                    msgView.SetMessage("Cost details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                string userCode = Convert.ToString(Session["UserCode"]);
                Array costSaveList = CostDetailsList();                
                SetFromToDates();

                royCostsBL = new RoyaltorCostsBL();
                DataSet gridData = royCostsBL.SaveCost(txtRoyaltor.Text.Split('-')[0].ToString(), ddlStmtPeriod.SelectedValue, fromDate, toDate, userCode, costSaveList, out errorId);
                royCostsBL = null;

                if (gridData.Tables.Count != 0 && errorId != 2)
                {

                    if (gridData.Tables[0].Rows.Count == 0)
                    {
                        gvRoyCosts.DataSource = gridData.Tables[0];
                        gvRoyCosts.EmptyDataText = "No data found for the selected filter criteria.";
                        gvRoyCosts.DataBind();
                    }
                    else
                    {
                        gvRoyCosts.DataSource = gridData.Tables[0];
                        gvRoyCosts.DataBind();
                    }

                    Session["RoyCostsGridDataInitial"] = gridData.Tables[0];
                    Session["RoyCostsGridData"] = gridData.Tables[0];

                }
                else if (gridData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvRoyCosts.DataSource = dtEmpty;
                    gvRoyCosts.EmptyDataText = "No data found for the selected filter criteria.";
                    gvRoyCosts.DataBind();
                }
                else
                {
                    ExceptionHandler("Error in updating cost data.", string.Empty);
                }

                ClearAddRowFields();


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving cost data.", ex.Message);
            }
        }

        protected void valFrmToDates_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtFromDate.Text.Trim() != "" && txtToDate.Text.Trim() != "")
            {
                DateTime fromdate = DateTime.MinValue;
                DateTime todate = DateTime.MinValue;

                if (txtFromDate.Text.Trim() != "__/____")
                {
                    Int32 frmDateYear = Convert.ToInt32(txtFromDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                    Int32 frmDateMonth = Convert.ToInt32(txtFromDate.Text.Replace('_', ' ').Split('/')[0].Trim());
                    //validate - month and year are valid
                    if (!(frmDateMonth > 0 && frmDateMonth < 13) || !(frmDateYear > 1900))
                    {
                        valFrmToDates.ErrorMessage = "Please enter valid from date in mm/yyyy format";
                        args.IsValid = false;
                        return;
                    }
                    else
                    {
                        fromdate = Convert.ToDateTime(txtFromDate.Text);
                    }


                }

                if (txtToDate.Text.Trim() != "__/____")
                {
                    //validation - check for valid date mm/yyyy
                    Int32 toDateYear = Convert.ToInt32(txtToDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                    Int32 toDateMonth = Convert.ToInt32(txtToDate.Text.Replace('_', ' ').Split('/')[0].Trim());
                    if (!(toDateMonth > 0 && toDateMonth < 13) || !(toDateYear > 1900))
                    {
                        valFrmToDates.ErrorMessage = "Please enter valid to date in mm/yyyy format";
                        args.IsValid = false;
                        return;
                    }
                    else
                    {
                        todate = Convert.ToDateTime(txtToDate.Text);
                    }

                }

                //validation - from should be earlier than the to_date   
                if (fromdate != DateTime.MinValue && todate != DateTime.MinValue)
                {
                    if (fromdate > todate)
                    {
                        args.IsValid = false;
                        return;
                    }
                }
                
            }
        }

        protected void btnAppendAddRow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Not a valid data. Please correct", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (txtRoyaltor.Text == string.Empty || txtRoyaltor.Text == "No results found" || hdnRoyaltorSelected.Value == "N")
                {
                    msgView.SetMessage("Please select a valid royaltor from the list", MessageType.Warning, PositionType.Auto);
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialStmtPeriod();
                    return;
                }

                //Validate
                Page.Validate("valAdd");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                    return;
                }

                AppendRowToGrid();
                ClearAddRowFields();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding payee row to grid", ex.Message);
            }
        }

        #endregion EVENTS

        #region METHODS

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

            //util = new Utilities();
            //util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            //util = null;
        }

        private void PopulateDropDowns()
        {

            royCostsBL = new RoyaltorCostsBL();
            DataSet dropdownListData = royCostsBL.GetDropDownData(out errorId);
            royCostsBL = null;

            if (dropdownListData.Tables.Count != 0 && errorId != 2)
            {                
                Session["RoyCostsAccountType"] = dropdownListData.Tables[0];//used in appending row to grid

                ddlAccTypeAdd.DataTextField = "account_type_desc";
                ddlAccTypeAdd.DataValueField = "type_id";
                ddlAccTypeAdd.DataSource = dropdownListData.Tables[0];
                ddlAccTypeAdd.DataBind();
                ddlAccTypeAdd.Items.Insert(0, new ListItem("-"));

                LoadInitialStmtPeriod();

            }
            else
            {
                ExceptionHandler("Error in fetching royaltors", string.Empty);
            }

        }

        private void SetFromToDates()
        {
            if (txtFromDate.Text != "__/____" && txtFromDate.Text != string.Empty)
                fromDate = Convert.ToDateTime(txtFromDate.Text).ToShortDateString();
            else
                fromDate = string.Empty;

            if (txtToDate.Text != "__/____" && txtToDate.Text != string.Empty)
                toDate = Convert.ToDateTime(txtToDate.Text).AddMonths(1).AddDays(-1).ToShortDateString();
            else
                toDate = string.Empty;
        }

        private void LoadSearchData()
        {            
            hdnGridRowSelectedPrvious.Value = null;
            tdGrid.Visible = true;
            btnSave.Visible = true;
            //set gridview panel height                    
            PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
            SetFromToDates();
            royCostsBL = new RoyaltorCostsBL();
            DataSet gridData = royCostsBL.GetSearchData(txtRoyaltor.Text.Split('-')[0].ToString(), ddlStmtPeriod.SelectedValue, fromDate, toDate, out errorId);
            royCostsBL = null;

            if (gridData.Tables.Count != 0 && errorId != 2)
            {

                if (gridData.Tables[0].Rows.Count == 0)
                {
                    gvRoyCosts.DataSource = gridData.Tables[0];
                    gvRoyCosts.EmptyDataText = "No data found for the selected filter criteria";
                    gvRoyCosts.DataBind();
                }
                else
                {
                    gvRoyCosts.DataSource = gridData.Tables[0];
                    gvRoyCosts.DataBind();
                }

                Session["RoyCostsGridDataInitial"] = gridData.Tables[0];
                Session["RoyCostsGridData"] = gridData.Tables[0];

            }
            else if (gridData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyCosts.DataSource = dtEmpty;
                gvRoyCosts.EmptyDataText = "No data found for the selected filter criteria";
                gvRoyCosts.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

            UserAuthorization();


        }

        private void LoadInitialGridData()
        {
            dtEmpty = new DataTable();
            gvRoyCosts.DataSource = dtEmpty;
            gvRoyCosts.DataBind();
        }

        private void LoadStmtPeriodData()
        {
            //if (tdGrid.Visible == true)
            //{
            //    tdGrid.Visible = false;
            //}

            LoadStmtPeriodData(txtRoyaltor.Text.Split('-')[0].ToString());

        }

        private void LoadStmtPeriodData(string royaltorId)
        {
            royCostsBL = new RoyaltorCostsBL();
            DataSet stmtPeriods = royCostsBL.GetStmtPeriods(royaltorId, out errorId);
            royCostsBL = null;

            if (stmtPeriods.Tables.Count != 0 && errorId != 2)
            {
                ddlStmtPeriod.DataTextField = "stmt_period";
                ddlStmtPeriod.DataValueField = "statement_period_id";
                ddlStmtPeriod.DataSource = stmtPeriods.Tables[0];
                ddlStmtPeriod.DataBind();
                ddlStmtPeriod.Items.Insert(0, new ListItem("-"));

            }
            else
            {
                ExceptionHandler("Error in loading statement periods.", string.Empty);
            }
        }

        private void LoadInitialStmtPeriod()
        {
            ddlStmtPeriod.Items.Clear();
            ddlStmtPeriod.Items.Insert(0, new ListItem("-"));
        }

        private void ClearAddRowFields()
        {
            ddlAccTypeAdd.SelectedIndex = 0;
            txtDescAdd.Text = string.Empty;
            txtDateAdd.Text = string.Empty;
            txtAmountAdd.Text = string.Empty;
            txtSuppNameAdd.Text = string.Empty;
            txtProjCodeAdd.Text = string.Empty;
            txtInvNumAdd.Text = string.Empty;
        }
        
        private void FuzzySearchRoyaltor()
        {

            if (txtRoyaltor.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyListWithOwnerCode(txtRoyaltor.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        /// <summary>
        /// WUIN-641 changes - modified the way comparing initial data with the current screen data
        /// </summary>
        /// <returns></returns>
        private Array CostDetailsList()
        {           
            List<string> costDetails = new List<string>();

            //hidden field variables
            string accountTypeId;
            string journalEntryId;
            string hdnTranDesc;
            string hdnDate;
            string hdnAmount;
            string hdnSuppName;
            string hdnProjCode;
            string hdnInvoiceNum;
            string hdnIsDeleted;

            //screen field variables
            string txtTranDesc;
            string txtDate;
            string txtAmount;
            string txtSuppName;
            string txtProjCode;
            string txtInvoiceNum;

            //variables            
            string isModified;
            CheckBox cbDisplay;
            
            foreach (GridViewRow gvr in gvRoyCosts.Rows)
            {
                accountTypeId = (gvr.FindControl("hdnAccTypeId") as HiddenField).Value;

                if (accountTypeId == string.Empty)
                {
                    //check only for the group child rows
                    continue;
                }

                //hidden field values
                journalEntryId = (gvr.FindControl("hdnJournalEntryId") as HiddenField).Value;
                hdnTranDesc = (gvr.FindControl("hdnTranDesc") as HiddenField).Value;
                hdnDate = (gvr.FindControl("hdnDate") as HiddenField).Value;
                hdnAmount = (gvr.FindControl("hdnAmount") as HiddenField).Value;
                hdnSuppName = (gvr.FindControl("hdnSuppName") as HiddenField).Value;
                hdnProjCode = (gvr.FindControl("hdnProjCode") as HiddenField).Value;
                hdnInvoiceNum = (gvr.FindControl("hdnInvoiceNum") as HiddenField).Value;
                hdnIsDeleted = (gvr.FindControl("hdnIsDeleted") as HiddenField).Value;
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                //screen field values
                txtTranDesc = (gvr.FindControl("txtTranDesc") as TextBox).Text;
                txtDate = (gvr.FindControl("txtDate") as TextBox).Text;
                txtAmount = (gvr.FindControl("txtAmount") as TextBox).Text;
                txtSuppName = (gvr.FindControl("txtSuppName") as TextBox).Text;
                txtProjCode = (gvr.FindControl("txtProjCode") as TextBox).Text;
                txtInvoiceNum = (gvr.FindControl("txtInvoiceNum") as TextBox).Text;
                cbDisplay = (gvr.FindControl("cbDisplay") as CheckBox);
                               
                if (isModified == "-")
                {
                    //new row

                    costDetails.Add(accountTypeId + DBDelimiter + journalEntryId + DBDelimiter + txtTranDesc + DBDelimiter + txtDate + DBDelimiter + txtAmount + DBDelimiter +
                            txtSuppName + DBDelimiter + txtProjCode + DBDelimiter + txtInvoiceNum + DBDelimiter + (cbDisplay.Checked ? "N" : "Y") + DBDelimiter + isModified);

                }
                else
                {
                    //existing row, check if modified and add to the list

                    if (txtTranDesc != hdnTranDesc || txtDate != hdnDate || txtAmount != hdnAmount || txtSuppName != hdnSuppName || txtProjCode != hdnProjCode ||
                        txtInvoiceNum != hdnInvoiceNum || hdnIsDeleted != (cbDisplay.Checked ? "N" : "Y"))
                    {
                        costDetails.Add(accountTypeId + DBDelimiter + journalEntryId + DBDelimiter + txtTranDesc + DBDelimiter + txtDate + DBDelimiter + txtAmount + DBDelimiter +
                            txtSuppName + DBDelimiter + txtProjCode + DBDelimiter + txtInvoiceNum + DBDelimiter + (cbDisplay.Checked ? "N" : "Y") + DBDelimiter + isModified);
                    }
                }
            }


            return costDetails.ToArray();

        }

        private void RoyaltorSelected()
        {
            if (txtRoyaltor.Text == string.Empty || txtRoyaltor.Text == "No results found")
            {
                msgView.SetMessage("Please select a valid royaltor from the list", MessageType.Warning, PositionType.Auto);
                txtRoyaltor.Text = string.Empty;
                LoadInitialStmtPeriod();
                LoadInitialGridData();
                return;

            }
            else
            {
                LoadInitialGridData();
                LoadStmtPeriodData();
            }
        }
        
        private void AppendRowToGrid()
        {
            if (Session["RoyCostsGridData"] == null || Session["RoyCostsGridDataInitial"] == null)
            {
                ExceptionHandler("Error in adding royaltor cost row to grid", string.Empty);
            }

            GetGridData();

            DataTable dtGridData = (DataTable)Session["RoyCostsGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyCostsGridDataInitial"];
            DataRow drNewRow = dtGridData.NewRow();
            DataRow drNewRowOriginal = dtOriginalGridData.NewRow();

            //get display_order from account_type selected
            DataTable dtAccType = (DataTable)Session["RoyCostsAccountType"];
            string displayOrder = dtAccType.Select("type_id='" + ddlAccTypeAdd.SelectedValue.Replace("'","''") + "'").CopyToDataTable().Rows[0]["display_order"].ToString();            

            //check if there are rows in grid for the selected account type, if not add the payment description row as well
            DataRow[] dtGridRows = dtGridData.Select("account_type_id='" + ddlAccTypeAdd.SelectedValue.ToString().Split('-')[0] + "'");
            if (dtGridRows.Count() == 0)
            {
                drNewRow["sno"] = "1";
                drNewRow["account_type_desc"] = ddlAccTypeAdd.SelectedItem.Text;
                drNewRow["display_order"] = displayOrder;
                drNewRow["account_type_id"] = DBNull.Value;
                drNewRow["journal_entry_id"] = DBNull.Value;
                drNewRow["royaltor_id"] = DBNull.Value;
                drNewRow["journal_voucher_desc"] = DBNull.Value;
                drNewRow["journal_voucher_date"] = DBNull.Value;
                drNewRow["journal_voucher_amount"] = DBNull.Value;
                drNewRow["journal_supplier_name"] = DBNull.Value;
                drNewRow["project_code"] = DBNull.Value;
                drNewRow["invoice_no"] = DBNull.Value;
                drNewRow["is_deleted"] = "N";
                drNewRow["is_modified"] = "N";//as no need to add this explicitly                
                dtGridData.Rows.Add(drNewRow);
                drNewRowOriginal.ItemArray = drNewRow.ItemArray.Clone() as object[];
                dtOriginalGridData.Rows.Add(drNewRowOriginal);

                drNewRow = dtGridData.NewRow();
                drNewRowOriginal = dtOriginalGridData.NewRow();
                drNewRow["sno"] = "2";
                drNewRow["account_type_desc"] = DBNull.Value;
                drNewRow["display_order"] = displayOrder;
                drNewRow["account_type_id"] = ddlAccTypeAdd.SelectedValue.ToString().Split('-')[0];
                drNewRow["journal_entry_id"] = DBNull.Value;
                drNewRow["royaltor_id"] = txtRoyaltor.Text.Split('-')[0].ToString();
                drNewRow["journal_voucher_desc"] = txtDescAdd.Text.Trim();
                drNewRow["journal_voucher_date"] = txtDateAdd.Text.Trim();
                drNewRow["journal_voucher_amount"] = txtAmountAdd.Text.Trim();
                drNewRow["journal_supplier_name"] = txtSuppNameAdd.Text.Trim();
                drNewRow["project_code"] = txtProjCodeAdd.Text.Trim();
                drNewRow["invoice_no"] = txtInvNumAdd.Text.Trim();
                drNewRow["is_deleted"] = "N";//default as N for new cost(as per previous add functionality)
                drNewRow["is_modified"] = "-";
            }
            else
            {
                drNewRow["sno"] = "2";
                drNewRow["account_type_desc"] = DBNull.Value;
                drNewRow["display_order"] = displayOrder;
                drNewRow["account_type_id"] = ddlAccTypeAdd.SelectedValue.ToString().Split('-')[0];
                drNewRow["journal_entry_id"] = DBNull.Value;
                drNewRow["royaltor_id"] = txtRoyaltor.Text.Split('-')[0].ToString();
                drNewRow["journal_voucher_desc"] = txtDescAdd.Text.Trim();
                drNewRow["journal_voucher_date"] = txtDateAdd.Text.Trim();
                drNewRow["journal_voucher_amount"] = txtAmountAdd.Text.Trim();
                drNewRow["journal_supplier_name"] = txtSuppNameAdd.Text.Trim();
                drNewRow["project_code"] = txtProjCodeAdd.Text.Trim();
                drNewRow["invoice_no"] = txtInvNumAdd.Text.Trim();
                drNewRow["is_deleted"] = "N";//default as N for new cost(as per previous add functionality)
                drNewRow["is_modified"] = "-";

            }

            dtGridData.Rows.Add(drNewRow);

            DataView dv = dtGridData.DefaultView;
            dv.Sort = "display_order, sno, journal_entry_id";
            DataTable dtGridDataSorted = dv.ToTable();
            Session["RoyCostsGridData"] = dtGridDataSorted;
            gvRoyCosts.DataSource = dtGridDataSorted;
            gvRoyCosts.DataBind();

            drNewRowOriginal.ItemArray = drNewRow.ItemArray.Clone() as object[];
            dtOriginalGridData.Rows.Add(drNewRowOriginal);
            DataView dvInitialData = dtOriginalGridData.DefaultView;
            dvInitialData.Sort = "display_order, sno, journal_entry_id";
            DataTable dtOriginalGridDataSorted = dvInitialData.ToTable();
            Session["RoyCostsGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["RoyCostsGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            string accountTypeId;
            string journalEntryId;
            string royaltorId;
            CheckBox cbDisplay;

            foreach (GridViewRow gvr in gvRoyCosts.Rows)
            {

                cbDisplay = (gvr.FindControl("cbDisplay") as CheckBox);
                accountTypeId = (gvr.FindControl("hdnAccTypeId") as HiddenField).Value;
                journalEntryId = (gvr.FindControl("hdnJournalEntryId") as HiddenField).Value;
                royaltorId = (gvr.FindControl("hdnRoyaltorId") as HiddenField).Value;

                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["sno"] = (gvr.FindControl("hdnSno") as HiddenField).Value;
                drGridRow["account_type_desc"] = (gvr.FindControl("lblAccType") as Label).Text;
                drGridRow["display_order"] = (gvr.FindControl("hdnDisplayOrder") as HiddenField).Value;

                if (accountTypeId == string.Empty)
                {
                    drGridRow["account_type_id"] = DBNull.Value;
                }
                else
                {
                    drGridRow["account_type_id"] = accountTypeId;
                }

                if (journalEntryId == string.Empty)
                {
                    drGridRow["journal_entry_id"] = DBNull.Value;
                }
                else
                {
                    drGridRow["journal_entry_id"] = journalEntryId;
                }

                if (royaltorId == string.Empty)
                {
                    drGridRow["royaltor_id"] = DBNull.Value;
                }
                else
                {
                    drGridRow["royaltor_id"] = royaltorId;
                }

                drGridRow["journal_voucher_desc"] = (gvr.FindControl("txtTranDesc") as TextBox).Text;
                drGridRow["journal_voucher_date"] = (gvr.FindControl("txtDate") as TextBox).Text;
                drGridRow["journal_voucher_amount"] = (gvr.FindControl("txtAmount") as TextBox).Text;
                drGridRow["journal_supplier_name"] = (gvr.FindControl("txtSuppName") as TextBox).Text;
                drGridRow["project_code"] = (gvr.FindControl("txtProjCode") as TextBox).Text;
                drGridRow["invoice_no"] = (gvr.FindControl("txtInvoiceNum") as TextBox).Text;
                drGridRow["is_deleted"] = (cbDisplay.Checked ? "N" : "Y");
                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value; ;

                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["RoyCostsGridData"] = dtGridChangedData;

        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSave.Enabled = false;
                btnConfirmSaveChanges.Enabled = false;
                btnAppendAddRow.Enabled = false;
                imgBtnCancel.Enabled = false;
                //disable grid buttons
                foreach (GridViewRow rows in gvRoyCosts.Rows)
                {
                    (rows.FindControl("imgBtnCancel") as ImageButton).Enabled = false;
                }

            }
        }

        #endregion METHODS

       
    }
}