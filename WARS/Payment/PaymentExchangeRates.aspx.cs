/*
File Name   :   PaymentExchangeRates.cs
Purpose     :   to maintain Payment Exchange Rates 

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     04-Apr-2018      Bala(Infosys Limited)   Initial Creation
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
    public partial class PaymentExchangeRates : System.Web.UI.Page
    {
        #region Global Declarations

        PaymentExchangeRatesBL paymentExchangeRatesBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Payment Exchange Rates";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Payment Exchange Rates";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlExchangeRateDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    Session["ExchangeRateFactorData"] = null;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        txtPaymentMonth.Focus();
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
                ExceptionHandler("Error in saving payment exchange rate data.", ex.Message);
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
                ExceptionHandler("Error in saving payment exchange rate data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtPaymentMonth.Text = string.Empty;
                    txtExchangeRate.Text = string.Empty;
                    txtCurrency.Text = string.Empty;
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
                ExceptionHandler("Error in creating payment exchange rate.", ex.Message);
            }
        }

        protected void valPaymentMonth_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtPaymentMonth.Text.Contains("_"))
                {
                    args.IsValid = false;
                    return;
                }
                else if (txtPaymentMonth.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtPaymentMonth.Text, out temp))
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

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCompany.SelectedIndex > 0)
                {
                    txtCurrency.Enabled = true;
                    txtExchangeRate.Enabled = true;
                    txtPaymentMonth.Enabled = true;

                    paymentExchangeRatesBL = new PaymentExchangeRatesBL();
                    DataSet paymentExchangeRateData = paymentExchangeRatesBL.GetPaymentExchangeRateData(ddlCompany.SelectedValue, out errorId);
                    paymentExchangeRatesBL = null;


                    if (paymentExchangeRateData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["ExchangeRateFactorData"] = paymentExchangeRateData.Tables[0];
                        gvExchangeRateDetails.PageIndex = 0;

                        //WUIN-746 clearing sort hidden files
                        hdnSortExpression.Value = string.Empty;
                        hdnSortDirection.Value = string.Empty;

                        BindGrid(paymentExchangeRateData.Tables[0]);

                    }
                    else
                    {
                        ExceptionHandler("Error in fetching data", string.Empty);
                    }
                }
                else
                {
                    txtCurrency.Enabled = false;
                    txtExchangeRate.Enabled = false;
                    txtPaymentMonth.Enabled = false;
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

        #endregion Events

        #region Methods

        private void LoadData()
        {
            string primaryCompanyCode = string.Empty;
            paymentExchangeRatesBL = new PaymentExchangeRatesBL();
            DataSet initialData = paymentExchangeRatesBL.GetInitialData(out primaryCompanyCode, out errorId);
            paymentExchangeRatesBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                hdnPageNumber.Value = "1";

                if (initialData.Tables.Count > 1)
                {
                    txtCompany.Text = primaryCompanyCode;
                    ddlCompany.Visible = false;
                    txtCompany.Visible = true;
                    rfddlCompany.Enabled = false;
                    if (initialData.Tables[1].Rows.Count > 0)
                    {
                        Session["ExchangeRateFactorData"] = initialData.Tables[1];
                        BindGrid(initialData.Tables[1]);
                    }
                    else
                    {
                        dtEmpty = new DataTable();
                        gvExchangeRateDetails.DataSource = dtEmpty;
                        gvExchangeRateDetails.DataBind();
                    }
                }
                else
                {

                    ddlCompany.DataSource = initialData.Tables[0];
                    ddlCompany.DataTextField = "company_data";
                    ddlCompany.DataValueField = "company_code";
                    ddlCompany.DataBind();
                    ddlCompany.Items.Insert(0, new ListItem("-"));

                    ddlCompany.Visible = true;
                    rfddlCompany.Enabled = true;
                    txtCompany.Visible = false;
                }
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
                string userCode = Convert.ToString(Session["UserCode"]);
                int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                //Calculate the rowindex for validation 
                int rowIndexValidation = (gvExchangeRateDetails.PageIndex * gvExchangeRateDetails.PageSize) + rowIndex;

                Page.Validate("GroupUpdate_" + rowIndexValidation + "");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Payment exchange rate details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                string companyCode = ((HiddenField)gvExchangeRateDetails.Rows[rowIndex].FindControl("hdnCompanyCode")).Value;
                string currencyCode = ((Label)gvExchangeRateDetails.Rows[rowIndex].FindControl("lblCurrencyCode")).Text;
                string monthId = ((HiddenField)gvExchangeRateDetails.Rows[rowIndex].FindControl("hdnMonthId")).Value;
                string exchangeRateFactor = ((TextBox)gvExchangeRateDetails.Rows[rowIndex].FindControl("txtExchangeRateFactor")).Text;

                paymentExchangeRatesBL = new PaymentExchangeRatesBL();
                DataSet updatedData = paymentExchangeRatesBL.UpdatePaymentExchangeRates(companyCode, currencyCode, Convert.ToInt32(monthId), Convert.ToDouble(exchangeRateFactor), userCode, out errorId);
                paymentExchangeRatesBL = null;

                if (updatedData.Tables.Count != 0 && errorId != 2)
                {
                    Session["ExchangeRateFactorData"] = updatedData.Tables[0];

                    //WUIN-746 clearing sort hidden files
                    hdnSortExpression.Value = string.Empty;
                    hdnSortDirection.Value = string.Empty;

                    BindGrid(updatedData.Tables[0]);

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Payment exchange rate details saved successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to save payment exchange rate details.", MessageType.Warning, PositionType.Auto);
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
                msgView.SetMessage("Payment exchange rate details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                return;
            }

            string userCode = Convert.ToString(Session["UserCode"]);
            int month = Convert.ToInt32(txtPaymentMonth.Text.Split('/')[0].ToString());
            int year = Convert.ToInt32(txtPaymentMonth.Text.Split('/')[1].ToString());
            string companyCode = ddlCompany.SelectedValue == "" ? txtCompany.Text.Split('-')[0].ToString().Trim() : ddlCompany.SelectedValue;

            paymentExchangeRatesBL = new PaymentExchangeRatesBL();
            DataSet updatedData = paymentExchangeRatesBL.InsertPaymentExchangeRates(companyCode, txtCurrency.Text.ToUpper(), month, year, Convert.ToDouble(txtExchangeRate.Text), userCode, out errorId);
            paymentExchangeRatesBL = null;

            if (errorId == 3)
            {
                msgView.SetMessage("Invalid month and year combination. Failed to save payment exchange rate details.", MessageType.Warning, PositionType.Auto);
            }
            else if (errorId == 4)
            {
                msgView.SetMessage("Invalid currency code. Failed to save payment exchange rate details. ", MessageType.Warning, PositionType.Auto);
            }
            else if (errorId == 1)
            {
                msgView.SetMessage("Payment exchange rate exists for this payment month and currency code.", MessageType.Success, PositionType.Auto);
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
                txtPaymentMonth.Text = string.Empty;
                txtExchangeRate.Text = string.Empty;
                txtCurrency.Text = string.Empty;
                gvExchangeRateDetails.PageIndex = 0;
                msgView.SetMessage("Payment exchange rate created successfully.", MessageType.Success, PositionType.Auto);
            }
            else
            {
                msgView.SetMessage("Failed to save payment exchange rate details.", MessageType.Warning, PositionType.Auto);
            }
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnSaveChanges.Enabled = false;
                foreach (GridViewRow rows in gvExchangeRateDetails.Rows)
                {
                    (rows.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                }

            }

        }

        #endregion Methods
        
    }
}