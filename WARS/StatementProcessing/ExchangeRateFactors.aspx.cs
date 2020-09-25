/*
File Name   :   ExchangeRateFactors.cs
Purpose     :   to maintain Exchange Rate Factors and Trasanction data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     15-Jun-2016     Pratik(Infosys Limited)   Initial Creation
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
    public partial class ExchangeRateFactors : System.Web.UI.Page
    {
        #region Global Declarations

        ExchangeRateFactorsBL exchangeRateFactorsBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Exchange Rates";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Exchange Rates";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlExchangeRateDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    Session["ExchangeRateFactorData"] = null;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        txtTransactionReceiptDate.Focus();
                        LoadData();

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

        //protected void btnUpdateTransactions_Click(object sender, EventArgs e)
        //{
        //    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

        //    List<int> monthIds = new List<int>();
        //    List<double> exchangeRateFactors = new List<double>();
        //    CheckBox cbSelected;

        //    foreach (GridViewRow gridrow in gvExchangeRateDetails.Rows)
        //    {
        //        cbSelected = (CheckBox)gridrow.FindControl("cbSelected");
        //        if (cbSelected.Checked)
        //        {
        //            monthIds.Add(Convert.ToInt32((gridrow.FindControl("hdnMonthId") as HiddenField).Value));
        //            exchangeRateFactors.Add(Convert.ToDouble((gridrow.FindControl("txtExchangeRateFactor") as TextBox).Text));
        //        }
        //    }

        //    if (monthIds.Count > 0)
        //    {
        //        exchangeRateFactorsBL = new ExchangeRateFactorsBL();
        //        DataSet updatedData = exchangeRateFactorsBL.UpdateTransactions(ddlCompany.SelectedValue, txtDomesticCurrency.Text.Trim(), txtDomesticCurrencyGrouping.Text.Trim(), monthIds.ToArray(), exchangeRateFactors.ToArray(), userCode, out errorId);
        //        exchangeRateFactorsBL = null;

        //        if (updatedData.Tables.Count != 0 && errorId != 2)
        //        {
        //            Session["ExchangeRateFactorData"] = updatedData.Tables[0];

        //            BindGrid(updatedData.Tables[0]);

        //            msgView.SetMessage("Transaction details updated successfully.", MessageType.Success, PositionType.Auto);
        //        }
        //        else
        //        {
        //            msgView.SetMessage("Failed to update transaction details details.", MessageType.Warning, PositionType.Auto);
        //        }
        //    }
        //    else
        //    {
        //        msgView.SetMessage("Please select a exception rate factor!",
        //                            MessageType.Warning, PositionType.Auto);
        //    }
        //}
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["ExchangeRateFactorData"] == null)
                    return;

                DataTable dtExchangeRateFactorData = Session["ExchangeRateFactorData"] as DataTable;
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);

                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;
                BindGrid(dtExchangeRateFactorData);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }
        protected void gvExchangeRateDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {


                if (e.CommandName == "saverow")
                {
                    UpdateExchangeRateFactor();
                }
                else if (e.CommandName == "cancelrow")
                {
                    if (Session["ExchangeRateFactorData"] != null)
                    {
                        DataTable dtExchangeRateFactorData = Session["ExchangeRateFactorData"] as DataTable;
                        BindGrid(dtExchangeRateFactorData);
                    }

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving exchange rate factor data.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    InsertExchangeRateFactor();
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    UpdateExchangeRateFactor();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving exchange rate factor data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtTransactionReceiptDate.Text = string.Empty;
                    txtExchangeRateFactor.Text = string.Empty;
                    hdnInsertDataNotSaved.Value = "N";
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    if (Session["ExchangeRateFactorData"] != null)
                    {
                        DataTable dtExchangeRateFactorData = Session["ExchangeRateFactorData"] as DataTable;
                        BindGrid(dtExchangeRateFactorData);
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

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                InsertExchangeRateFactor();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in creating exchange rate factor.", ex.Message);
            }
        }

        protected void valTransactionReceiptDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtTransactionReceiptDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtTransactionReceiptDate.Text, out temp))
                    {

                    }
                    else
                    {
                        args.IsValid = false;
                        return;
                    }
                }
                else
                {
                    args.IsValid = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCompany.SelectedIndex > 0)
                {
                    txtExchangeRateFactor.Enabled = true;
                    txtTransactionReceiptDate.Enabled = true;
                    imgBtnInsert.Enabled = true;

                    exchangeRateFactorsBL = new ExchangeRateFactorsBL();
                    DataSet companyData = exchangeRateFactorsBL.GetCompanyData(ddlCompany.SelectedValue, out errorId);
                    exchangeRateFactorsBL = null;

                    if (companyData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["ExchangeRateFactorData"] = companyData.Tables[1];
                        gvExchangeRateDetails.PageIndex = 0;

                        //WUIN-746 clearing sort hidden files
                        hdnSortExpression.Value = string.Empty;
                        hdnSortDirection.Value = string.Empty;

                        BindGrid(companyData.Tables[1]);

                        ddlCompany.Items.FindByValue(companyData.Tables[0].Rows[0]["company_code"].ToString()).Selected = true;
                        txtDomesticCurrency.Text = companyData.Tables[0].Rows[0]["currency_code"].ToString();
                        txtDomesticCurrencyGrouping.Text = companyData.Tables[0].Rows[0]["domestic_currency_group"].ToString();
                    }
                    else
                    {
                        ExceptionHandler("Error in fetching data", string.Empty);
                    }
                }
                else
                {
                    txtDomesticCurrency.Text = string.Empty;
                    txtDomesticCurrencyGrouping.Text = string.Empty;
                    txtExchangeRateFactor.Enabled = false;
                    txtTransactionReceiptDate.Enabled = false;
                    imgBtnInsert.Enabled = false;

                    dtEmpty = new DataTable();
                    gvExchangeRateDetails.DataSource = dtEmpty;
                    gvExchangeRateDetails.DataBind();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting company.", ex.Message);
            }
        }

        protected void gvExchangeRateDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvExchangeRateDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["ExchangeRateFactorData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvExchangeRateDetails, dtEmpty, rptPager);
                Session["ExchangeRateFactorData"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End


        #endregion Events

        #region Methods

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSaveChanges.Enabled = false;
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                foreach (GridViewRow rows in gvExchangeRateDetails.Rows)
                {
                    (rows.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                }
            }

        }

        private void LoadData()
        {
            exchangeRateFactorsBL = new ExchangeRateFactorsBL();
            DataSet initialData = exchangeRateFactorsBL.GetInitialData(out errorId);
            exchangeRateFactorsBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                hdnPageNumber.Value = "1";
                ddlCompany.DataSource = initialData.Tables[0];
                ddlCompany.DataTextField = "company_data";
                ddlCompany.DataValueField = "company_code";
                ddlCompany.DataBind();
                ddlCompany.Items.Insert(0, new ListItem("-"));

                if (initialData.Tables.Count > 1)
                {
                    Session["ExchangeRateFactorData"] = initialData.Tables[2];
                    BindGrid(initialData.Tables[2]);

                    ddlCompany.Items.FindByValue(initialData.Tables[1].Rows[0]["company_code"].ToString()).Selected = true;
                    txtDomesticCurrency.Text = initialData.Tables[1].Rows[0]["currency_code"].ToString();
                    txtDomesticCurrencyGrouping.Text = initialData.Tables[1].Rows[0]["domestic_currency_group"].ToString();
                    txtCompany.Text = ddlCompany.SelectedItem.ToString();

                    ddlCompany.Visible = false;
                    txtCompany.Visible = true;
                }
                else
                {
                    txtExchangeRateFactor.Enabled = false;
                    txtTransactionReceiptDate.Enabled = false;
                    imgBtnInsert.Enabled = false;

                    ddlCompany.Visible = true;
                    txtCompany.Visible = false;
                }
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), gridData, gridDefaultPageSize, gvExchangeRateDetails, dtEmpty, rptPager);
            UserAuthorization();
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        private void UpdateExchangeRateFactor()
        {
            if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                //Calculate the rowindex for validation 
                int rowIndexValidation = (gvExchangeRateDetails.PageIndex * gvExchangeRateDetails.PageSize) + rowIndex;

                Page.Validate("GroupUpdate_" + rowIndexValidation + "");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Exchange rate factor details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                string companyCode = ((HiddenField)gvExchangeRateDetails.Rows[rowIndex].FindControl("hdnCompanyCode")).Value;
                string monthId = ((HiddenField)gvExchangeRateDetails.Rows[rowIndex].FindControl("hdnMonthId")).Value;
                string exchangeRateFactor = ((TextBox)gvExchangeRateDetails.Rows[rowIndex].FindControl("txtExchangeRateFactor")).Text;

                exchangeRateFactorsBL = new ExchangeRateFactorsBL();
                DataSet updatedData = exchangeRateFactorsBL.UpdateExchangeRateFactor(companyCode, Convert.ToInt32(monthId), Convert.ToDouble(exchangeRateFactor), userCode, out errorId);
                exchangeRateFactorsBL = null;

                if (updatedData.Tables.Count != 0 && errorId != 2)
                {
                    Session["ExchangeRateFactorData"] = updatedData.Tables[0];

                    //WUIN-746 clearing sort hidden files
                    hdnSortExpression.Value = string.Empty;
                    hdnSortDirection.Value = string.Empty;

                    BindGrid(updatedData.Tables[0]);

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Exchange rate factor details saved successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to save exchange rate factor details.", MessageType.Warning, PositionType.Auto);
                }
            }
        }

        private void InsertExchangeRateFactor()
        {
            //Validate
            Page.Validate("valInsertExchangeRates");
            if (!Page.IsValid)
            {
                mpeSaveUndo.Hide();
                msgView.SetMessage("Exchange rate factor details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                return;
            }

            string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
            int month = Convert.ToInt32(txtTransactionReceiptDate.Text.Split('/')[0].ToString());
            int year = Convert.ToInt32(txtTransactionReceiptDate.Text.Split('/')[1].ToString());

            exchangeRateFactorsBL = new ExchangeRateFactorsBL();
            DataSet updatedData = exchangeRateFactorsBL.InsertExchangeRateFactor(ddlCompany.SelectedValue, month, year, Convert.ToDouble(txtExchangeRateFactor.Text), userCode, out errorId);
            exchangeRateFactorsBL = null;

            if (errorId == 3)
            {
                msgView.SetMessage("Invalid month. Failed to save exchange rate factor details.", MessageType.Warning, PositionType.Auto);
            }
            else if (errorId == 1)
            {
                msgView.SetMessage("Exchange rate factor exists for this month and company.", MessageType.Success, PositionType.Auto);
            }
            else if (updatedData.Tables.Count != 0 && errorId != 2)
            {
                Session["ExchangeRateFactorData"] = updatedData.Tables[0];
                gvExchangeRateDetails.PageIndex = 0;

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                BindGrid(updatedData.Tables[0]);

                hdnInsertDataNotSaved.Value = "N";
                txtTransactionReceiptDate.Text = string.Empty;
                txtExchangeRateFactor.Text = string.Empty;
                gvExchangeRateDetails.PageIndex = 0;
                msgView.SetMessage("Exchange rate factor created successfully.", MessageType.Success, PositionType.Auto);
            }
            else
            {
                msgView.SetMessage("Failed to save exchange rate factor details.", MessageType.Warning, PositionType.Auto);
            }
        }

        #endregion Methods




    }
}