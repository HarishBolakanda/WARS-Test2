/*
File Name   :   AutoParticipantSearch.cs
Purpose     :   to search and Add/Update Participant Groups

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     02-Nov-2019     Rakesh(Infosys Limited)   Initial Creation
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WARS.BusinessLayer;

namespace WARS.DataMaintenance
{
    public partial class TaxRateMaintanance : System.Web.UI.Page
    {
        #region Global Declarations

        TaxRateMaintananceBL taxRateMaintananceBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Tax Rates";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Tax Rates";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlTaxRateDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    Session["TaxRateData"] = null;

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

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                imgBtnInsert.Enabled = true;
                taxRateMaintananceBL = new TaxRateMaintananceBL();
                DataSet taxRateData = taxRateMaintananceBL.GetTaxRateData(ddlCompany.SelectedValue, out errorId);
                taxRateMaintananceBL = null;


                if (taxRateData.Tables.Count != 0 && errorId != 2)
                {
                    Session["TaxRateData"] = taxRateData.Tables[0];
                    gvTaxRateDetails.PageIndex = 0;
                    BindGrid(taxRateData.Tables[0]);
                }
                else
                {
                    ExceptionHandler("Error in fetching data", string.Empty);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting company.", ex.Message);
            }

        }

        protected void gvTaxRateDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "saverow")
                {
                    if (hdnChangeNotSaved.Value == "Y")
                    {
                        UpdateTaxRate();
                    }
                    else
                    {
                        msgView.SetMessage("No changes made to save.", MessageType.Success, PositionType.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving tax rate data.", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                SaveTaxRate();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in creating tax rate.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    SaveTaxRate();
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    UpdateTaxRate();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving tax rate data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtStartDate.Text = string.Empty;
                    txtEndDate.Text = string.Empty;
                    ddlTaxType.SelectedIndex = -1;
                    txtTaxRate.Text = string.Empty;
                    hdnInsertDataNotSaved.Value = "N";
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    if (Session["TaxRateData"] != null)
                    {
                        DataTable dtTaxRateData = Session["TaxRateData"] as DataTable;
                        BindGrid(dtTaxRateData);
                    }

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading data grid.", ex.Message);
            }
        }




        protected void valStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtStartDate.Text.Contains("_"))
                {
                    args.IsValid = false;
                    return;
                }
                else if (txtStartDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtStartDate.Text, out temp))
                    {

                    }
                    else
                    {
                        args.IsValid = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }
        }

        protected void valEndDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtEndDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse(GetLastDateofMonth(txtEndDate.Text) + "/" + txtEndDate.Text, out temp))
                    {
                        if (txtStartDate.Text == string.Empty)
                        {
                            return;
                        }
                        DateTime endDate = DateTime.MinValue;
                        endDate = Convert.ToDateTime(GetLastDateofMonth(txtEndDate.Text) + "/" + txtEndDate.Text);
                        DateTime startDate = DateTime.MinValue;
                        if (DateTime.TryParse("01/" + txtStartDate.Text, out temp))
                        {
                            startDate = Convert.ToDateTime("01/" + txtStartDate.Text);
                        }
                        //validate - start date should be earlier than the exipty date 
                        if (endDate < startDate)
                        {
                            args.IsValid = false;
                            valEndDate.ToolTip = "End date cannot be earlier to start date!";
                            return;
                        }
                    }
                    else
                    {
                        args.IsValid = false;
                        valEndDate.ToolTip = "Please enter a valid date in MM/YYYY format";
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }
        }

        protected void valEndDateGridRow_ServerValidate(object sender, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            try
            {
                CustomValidator valEndDateGridRow = (CustomValidator)sender;
                GridViewRow grdrow = (GridViewRow)((CustomValidator)sender).NamingContainer;
                TextBox endDateGridRow = grdrow.FindControl("txtEndDateGridRow") as TextBox;
                Label startDateGridRow = grdrow.FindControl("lblStartDateGridRow") as Label;

                DateTime temp;
                if (endDateGridRow.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse(GetLastDateofMonth(endDateGridRow.Text) + "/" + endDateGridRow.Text, out temp))
                    {
                        DateTime endDate = DateTime.MinValue;
                        DateTime startDate = DateTime.MinValue;
                        startDate = Convert.ToDateTime("01/" + startDateGridRow.Text);
                        endDate = Convert.ToDateTime(GetLastDateofMonth(endDateGridRow.Text) + "/" + endDateGridRow.Text);
                        //validate - start date should be earlier than the exipty date 
                        if (endDate < startDate)
                        {
                            args.IsValid = false;
                            valEndDateGridRow.ToolTip = "End date cannot be earlier to start date!";
                            return;
                        }
                    }
                    else
                    {
                        args.IsValid = false;
                        valEndDate.ToolTip = "Please enter a valid date in MM/YYYY format";
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }

        }

        protected void gvTaxRateDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
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


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvTaxRateDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["TaxRateData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvTaxRateDetails, dtEmpty, rptPager);
                Session["TaxRateData"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End



        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["TaxRateData"] == null)
                    return;

                DataTable dtTaxRateData = Session["TaxRateData"] as DataTable;
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);

                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;
                BindGrid(dtTaxRateData);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }


        #endregion Events

        #region Methods

        private void LoadData()
        {
            string primaryCompanyCode = string.Empty;
            taxRateMaintananceBL = new TaxRateMaintananceBL();
            DataSet initialData = taxRateMaintananceBL.GetInitialData(out errorId);
            taxRateMaintananceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                hdnPageNumber.Value = "1";

                ddlCompany.DataSource = initialData.Tables[0];
                ddlCompany.DataTextField = "company_data";
                ddlCompany.DataValueField = "company_code";
                ddlCompany.DataBind();
                ddlCompany.Items.Insert(0, new ListItem("-"));

                ddlTaxType.DataSource = initialData.Tables[1];
                ddlTaxType.DataTextField = "tax_type_desc";
                ddlTaxType.DataValueField = "tax_type";
                ddlTaxType.DataBind();
                ddlTaxType.Items.Insert(0, new ListItem("-"));

                dtEmpty = new DataTable();
                gvTaxRateDetails.EmptyDataText = "<br />";
                gvTaxRateDetails.DataSource = dtEmpty;
                gvTaxRateDetails.DataBind();

            }
        }
        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (gridData.Rows.Count > 0)
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), gridData, gridDefaultPageSize, gvTaxRateDetails, dtEmpty, rptPager);
            }
            else
            {
                dtEmpty = new DataTable();
                gvTaxRateDetails.EmptyDataText = "No data found for the selected company";
                gvTaxRateDetails.DataSource = dtEmpty;
                gvTaxRateDetails.DataBind();
            }
            UserAuthorization();
        }
        private void SaveTaxRate()
        {
            //Validate
            Page.Validate("valInsertTaxRate");
            if (!Page.IsValid)
            {
                mpeSaveUndo.Hide();
                msgView.SetMessage("Tax rate details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                return;
            }

            string startDate = txtStartDate.Text.Trim() != "__/____" ? "01/" + txtStartDate.Text : string.Empty;
            string endDate = string.Empty;
            if (txtEndDate.Text.Trim() != "__/____")
            {
                endDate = GetLastDateofMonth(txtEndDate.Text) + "/" + txtEndDate.Text;
            }
            string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

            taxRateMaintananceBL = new TaxRateMaintananceBL();
            DataSet updatedData = taxRateMaintananceBL.SaveTaxRate(ddlCompany.SelectedValue, startDate, endDate, ddlTaxType.SelectedValue, Convert.ToDouble(txtTaxRate.Text), userCode, out errorId);
            taxRateMaintananceBL = null;

            if (errorId == 1)
            {
                msgView.SetMessage("Tax rate exists for this company with this start date and tax type.", MessageType.Success, PositionType.Auto);
                return;
            }
            else if (updatedData.Tables.Count != 0 && errorId != 2)
            {
                Session["TaxRateData"] = updatedData.Tables[0];
                gvTaxRateDetails.PageIndex = 0;
                BindGrid(updatedData.Tables[0]);

                hdnInsertDataNotSaved.Value = "N";
                txtStartDate.Text = string.Empty;
                txtEndDate.Text = string.Empty;
                ddlTaxType.SelectedIndex = -1;
                txtTaxRate.Text = string.Empty;
                gvTaxRateDetails.PageIndex = 0;
                msgView.SetMessage("Tax rate created successfully.", MessageType.Success, PositionType.Auto);
            }
            else
            {
                msgView.SetMessage("Failed to save tax rate details.", MessageType.Warning, PositionType.Auto);
            }
        }

        private void UpdateTaxRate()
        {
            if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                //Calculate the rowindex for validation 
                int rowIndexValidation = (gvTaxRateDetails.PageIndex * gvTaxRateDetails.PageSize) + rowIndex;
                Page.Validate("GroupUpdate_" + rowIndexValidation + "");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Tax rate details not saved  – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                string companyCode = ((HiddenField)gvTaxRateDetails.Rows[rowIndex].FindControl("hdnCompanyCode")).Value;
                string txtNo = ((HiddenField)gvTaxRateDetails.Rows[rowIndex].FindControl("hdnTaxNo")).Value;
                string endDate = ((TextBox)gvTaxRateDetails.Rows[rowIndex].FindControl("txtEndDateGridRow")).Text;
                string taxRate = ((TextBox)gvTaxRateDetails.Rows[rowIndex].FindControl("txtTaxRateGridRow")).Text;
                if (endDate != "__/____" && endDate != "")
                {
                    endDate = GetLastDateofMonth(endDate) + "/" + endDate;
                }
                else
                {
                    endDate = string.Empty;
                }

                taxRateMaintananceBL = new TaxRateMaintananceBL();
                DataSet updatedData = taxRateMaintananceBL.UpdateTaxRate(ddlCompany.SelectedValue, companyCode, txtNo, endDate, Convert.ToDouble(taxRate), userCode, out errorId);
                taxRateMaintananceBL = null;

                if (updatedData.Tables.Count != 0 && errorId != 2)
                {
                    Session["TaxRateDate"] = updatedData.Tables[0];

                    BindGrid(updatedData.Tables[0]);

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Tax rate details saved successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to save tax rate details.", MessageType.Warning, PositionType.Auto);
                }
            }
        }

        private string GetLastDateofMonth(string date)
        {
            try
            {
                int year = Convert.ToInt16(date.Trim().Split('/')[1]);
                int month = Convert.ToInt16(date.Trim().Split('/')[0]);
                return DateTime.DaysInMonth(year, month).ToString();
            }
            catch
            {
                return "0";
            }
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSaveChanges.Enabled = false;
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;

                foreach (GridViewRow rows in gvTaxRateDetails.Rows)
                {
                    (rows.FindControl("imgBtnSave") as ImageButton).Enabled = false;
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

        
    }
}