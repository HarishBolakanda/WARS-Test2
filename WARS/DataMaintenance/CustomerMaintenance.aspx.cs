/*
File Name   :   CustomerMaintenance.cs
Purpose     :   to maintain Customer data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     15-May-2016     Pratik(Infosys Limited)   Initial Creation
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
    public partial class CustomerMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        CustomerMaintenanceBL customerMaintenanceBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize1"].ToString());
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Customer Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Customer Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlCustomerDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtCustomerSearch.Focus();
                    Session["CustomerData"] = null;
                    Session["FilteredCustomerData"] = null;


                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
                        UserAuthorization();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                    
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void btnHdnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                hdnPageNumber.Value = "1";                
                gvCustomerDetails.PageIndex = 0;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching search data.", ex.Message);
            }
        }

       

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                txtCustomerSearch.Text = string.Empty;
                txtLocalCustomerNo.Text = string.Empty;
                txtSourceCountry.Text = string.Empty;
                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;                
                gvCustomerDetails.PageIndex = 0;
                hdnPageNumber.Value = "1";
                hdnSortExpression.Value = "";
                hdnSortDirection.Value = "";
                customerMaintenanceBL = new CustomerMaintenanceBL();
                DataSet initialData = customerMaintenanceBL.GetInitialData(out errorId);
                customerMaintenanceBL = null;

                if (initialData.Tables.Count != 0 && errorId != 2)
                {
                    Session["CustomerData"] = initialData.Tables[0];

                    BindGrid(initialData.Tables[0]);
                }
                else
                {
                    ExceptionHandler("Error in fetching data", string.Empty);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reset.", ex.Message);
            }
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                msgView.SetMessage("screen to be developed.", MessageType.Success, PositionType.Auto);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to audit screen.", ex.Message);
            }
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);
                //hdnPageIndex.Value = pageIndex.ToString();
                DataTable dtGridData = (DataTable)Session["FilteredCustomerData"];
                Utilities.PopulateGridPage(pageIndex, dtGridData, gridDefaultPageSize, gvCustomerDetails, dtEmpty, rptPager);
                hdnChangeNotSaved.Value = "N";
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }
    
        protected void gvCustomerDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DataTable dtFixedMobile = new DataTable();
                dtFixedMobile.Columns.Add("dropdown_value", typeof(string));
                dtFixedMobile.Columns.Add("dropdown_text", typeof(string));
                dtFixedMobile.Rows.Add("F", "Fixed");
                dtFixedMobile.Rows.Add("M", "Mobile");

                DropDownList ddlFixedMobile;
                CheckBox cbDisplayOnGlobalStmt;
                CheckBox cbDisplayOnAccountStmt;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlFixedMobile = (e.Row.FindControl("ddlFixedMobile") as DropDownList);
                    string hdnFixedMobile = (e.Row.FindControl("hdnFixedMobile") as HiddenField).Value;

                    ddlFixedMobile.DataSource = dtFixedMobile;
                    ddlFixedMobile.DataTextField = "dropdown_text";
                    ddlFixedMobile.DataValueField = "dropdown_value";
                    ddlFixedMobile.DataBind();
                    ddlFixedMobile.Items.Insert(0, new ListItem("-"));

                    if (dtFixedMobile.Select("dropdown_value = '" + hdnFixedMobile + "'").Length != 0)
                    {
                        ddlFixedMobile.Items.FindByValue(hdnFixedMobile).Selected = true;
                    }
                    else
                    {
                        ddlFixedMobile.SelectedIndex = 0;
                    }

                    cbDisplayOnGlobalStmt = (e.Row.FindControl("cbDisplayOnGlobalStmt") as CheckBox);
                    string isDisplayOnGlobalStmt = (e.Row.FindControl("hdnDisplayOnGlobalStmt") as HiddenField).Value;

                    if (isDisplayOnGlobalStmt == "Y")
                    {
                        cbDisplayOnGlobalStmt.Checked = true;
                    }

                    cbDisplayOnAccountStmt = (e.Row.FindControl("cbDisplayOnAccountStmt") as CheckBox);
                    string isDisplayOnAccountStmt = (e.Row.FindControl("hdnDisplayOnAccountStmt") as HiddenField).Value;

                    if (isDisplayOnAccountStmt == "Y")
                    {
                        cbDisplayOnAccountStmt.Checked = true;
                    }


                    string localCustomerNo = (e.Row.FindControl("lblLocalCustomerNo") as Label).Text;
                    if (localCustomerNo.Length > 15)
                    {
                        (e.Row.FindControl("lblLocalCustomerNo") as Label).Text = localCustomerNo.Substring(0, 15) + "...";
                    }

                    string accountName = (e.Row.FindControl("lblAccountName") as Label).Text;
                    if (accountName.Length > 35)
                    {
                        (e.Row.FindControl("lblAccountName") as Label).Text = accountName.Substring(0, 35) + "...";
                    }

                    string globalCustomerName = (e.Row.FindControl("lblGlobalCustomerName") as Label).Text;
                    if (globalCustomerName.Length > 35)
                    {
                        (e.Row.FindControl("lblGlobalCustomerName") as Label).Text = globalCustomerName.Substring(0, 35) + "...";
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
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvCustomerDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["FilteredCustomerData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvCustomerDetails, dtEmpty, rptPager);
                Session["FilteredCustomerData"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvCustomerDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                if (e.CommandName == "saverow")
                {
                    if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
                    {
                        int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                        //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                        Int32 customerCode = Convert.ToInt32(((Label)gvCustomerDetails.Rows[rowIndex].FindControl("lblCustomerId")).Text);
                        string fixedMobileFlag = ((DropDownList)gvCustomerDetails.Rows[rowIndex].FindControl("ddlFixedMobile")).SelectedValue;

                        string isDisplayOnGlobalStatement;
                        if (((CheckBox)gvCustomerDetails.Rows[rowIndex].FindControl("cbDisplayOnGlobalStmt")).Checked)
                        {
                            isDisplayOnGlobalStatement = "Y";
                        }
                        else
                        {
                            isDisplayOnGlobalStatement = "N";
                        }

                        string isDisplayOnAccountStatement;
                        if (((CheckBox)gvCustomerDetails.Rows[rowIndex].FindControl("cbDisplayOnAccountStmt")).Checked)
                        {
                            isDisplayOnAccountStatement = "Y";
                        }
                        else
                        {
                            isDisplayOnAccountStatement = "N";
                        }

                        customerMaintenanceBL = new CustomerMaintenanceBL();
                        DataSet customerData = customerMaintenanceBL.UpdateCustomerData(customerCode, fixedMobileFlag, isDisplayOnGlobalStatement, isDisplayOnAccountStatement, userCode, out errorId);
                        customerMaintenanceBL = null;

                        if (customerData.Tables.Count != 0 && errorId != 2)
                        {
                            //WUIN-746 clearing sort hidden files
                            hdnSortExpression.Value = string.Empty;
                            hdnSortDirection.Value = string.Empty;

                            Session["CustomerData"] = customerData.Tables[0];

                            PopulateGrid();

                            hdnChangeNotSaved.Value = "N";
                            hdnGridRowSelectedPrvious.Value = null;
                            msgView.SetMessage("Customer details updated successfully.", MessageType.Success, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("Failed to updated customer details.", MessageType.Warning, PositionType.Auto);
                        }
                    }
                }                
                else if (e.CommandName == "cancelrow")
                {
                    //WUIN-746 clearing sort hidden files
                    hdnSortExpression.Value = string.Empty;
                    hdnSortDirection.Value = string.Empty;

                    PopulateGrid();

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;                   
                }
                
                
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving customer data.", ex.Message);
            }
        }
        
        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                //Calculate the rowindex for validation 
                int rowIndexValidation = (gvCustomerDetails.PageIndex * gvCustomerDetails.PageSize) + rowIndex;

                //Validate
                Page.Validate("GroupUpdate_" + rowIndexValidation + "");
                if (!Page.IsValid)
                {
                    mpeSaveUndo.Hide();
                    msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                    return;
                }

                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                Int32 customerCode = Convert.ToInt32(((Label)gvCustomerDetails.Rows[rowIndex].FindControl("lblCustomerId")).Text);
                string fixedMobileFlag = ((DropDownList)gvCustomerDetails.Rows[rowIndex].FindControl("ddlFixedMobile")).SelectedValue;

                string isDisplayOnGlobalStatement;
                if (((CheckBox)gvCustomerDetails.Rows[rowIndex].FindControl("cbDisplayOnGlobalStmt")).Checked)
                {
                    isDisplayOnGlobalStatement = "Y";
                }
                else
                {
                    isDisplayOnGlobalStatement = "N";
                }

                string isDisplayOnAccountStatement;
                if (((CheckBox)gvCustomerDetails.Rows[rowIndex].FindControl("cbDisplayOnAccountStmt")).Checked)
                {
                    isDisplayOnAccountStatement = "Y";
                }
                else
                {
                    isDisplayOnAccountStatement = "N";
                }

                customerMaintenanceBL = new CustomerMaintenanceBL();
                DataSet customerData = customerMaintenanceBL.UpdateCustomerData(customerCode, fixedMobileFlag, isDisplayOnGlobalStatement, isDisplayOnAccountStatement, userCode, out errorId);
                customerMaintenanceBL = null;

                if (customerData.Tables.Count != 0 && errorId != 2)
                {
                    Session["CustomerData"] = customerData.Tables[0];

                    //WUIN-746 clearing sort hidden files
                    hdnSortExpression.Value = string.Empty;
                    hdnSortDirection.Value = string.Empty;

                    PopulateGrid();

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Customer details updated successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to updated customer details.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving customer data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                PopulateGrid();

                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading data.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {
            customerMaintenanceBL = new CustomerMaintenanceBL();
            DataSet initialData = customerMaintenanceBL.GetInitialData(out errorId);
            customerMaintenanceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["CustomerData"] = initialData.Tables[0];
                Session["FilteredCustomerData"] = initialData.Tables[0];
                hdnPageNumber.Value = "1";
                BindGrid(initialData.Tables[0]);
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }
        }

        private void PopulateGrid()
        {
            if (Session["CustomerData"] != null)
            {
                DataTable dtCustomerData = Session["CustomerData"] as DataTable;
                DataTable dtSearched = dtCustomerData.Clone();

                dtSearched = dtCustomerData;

                if (txtCustomerSearch.Text != "")
                {
                    DataRow[] foundRows = dtSearched.Select("Convert(customer_id,System.String) like '%" + txtCustomerSearch.Text.Replace("'", "").Trim() + "%' or account_name like '%" + txtCustomerSearch.Text.Replace("'", "").Trim() + "%'");
                    if (foundRows.Length != 0)
                    {
                        dtSearched = foundRows.CopyToDataTable();
                    }
                }
                if (txtLocalCustomerNo.Text != "")
                {
                    DataRow[] foundRows = dtSearched.Select("local_customer_code like '%" + txtLocalCustomerNo.Text.Replace("'", "").Trim() + "%' ");
                    if (foundRows.Length != 0)
                    {
                        dtSearched = foundRows.CopyToDataTable();
                    }
                }
                if (txtSourceCountry.Text != "")
                {
                    DataRow[] foundRows = dtSearched.Select("source_country_code like '%" + txtSourceCountry.Text.Replace("'", "").Trim() + "%' ");
                    if (foundRows.Length != 0)
                    {
                        dtSearched = foundRows.CopyToDataTable();
                    }
                }
                Session["FilteredCustomerData"] = dtSearched;
                BindGrid(dtSearched);
            }
        }

        private void BindGrid(DataTable gridData)
        {

            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), gridData, gridDefaultPageSize, gvCustomerDetails, dtEmpty, rptPager);
            UserAuthorization();
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSaveChanges.Enabled = false;
                //disable grid buttons
                foreach (GridViewRow gvr in gvCustomerDetails.Rows)
                {
                    (gvr.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
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

              
    }
    
}