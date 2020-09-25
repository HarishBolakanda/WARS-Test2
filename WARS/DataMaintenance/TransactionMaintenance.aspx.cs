/*
File Name   :   TransactionMaintenance.cs
Purpose     :   To allow maintenance of Transactions

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0      28-May-2018     Bala(Infosys Limited)   Initial Creation
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
using System.Drawing;

namespace WARS
{
    public partial class TransactionMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        TransactionMaintenanceBL transactionMaintenanceBL;
        DataTable catNoList, currencyList, dtCountryCode, dtEmpty;
        Int32 errorId;
        Utilities util;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize1"].ToString());
        string gridDataSortOrder = "transaction_id";

        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Transaction Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Transaction Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlTransactions.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadInitialData();
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                txtSellerSearch.Text = string.Empty;
                txtCatalogueNumber.Text = string.Empty;
                txtSalesTypeSearch.Text = string.Empty;
                txtReportedDateSearch.Text = string.Empty;
                txtReceivedDateSearch.Text = string.Empty;
                if (hdnCompanyCode.Value != null)
                {
                    ddlCompany.SelectedValue = hdnCompanyCode.Value;
                }
                else
                {
                    ddlCompany.SelectedIndex = 0;
                }
                gvTransactions.DataSource = dtEmpty;
                gvTransactions.DataBind();
                hdnPageNumber.Value = "1";
                rptPager.Visible = false;
                hdnGridDataChanged.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reset event.", ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SearchedData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in search event.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet updateData;
                Page.Validate("valSave");
                if (!Page.IsValid)
                {
                    //WUIN-1023 Commented below code because validation marks '*' vanishing after colsing this message pop up. 
                    //msgView.SetMessage("Transactions not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }
                else
                {
                    Array modifiedRowList = ModifiedRowsList();
                    Array deletedRowsList = DeletedRowsList();

                    if (modifiedRowList.Length == 0 && deletedRowsList.Length == 0)
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    transactionMaintenanceBL = new TransactionMaintenanceBL();
                    updateData = transactionMaintenanceBL.UpdateTransactionDetails(txtSellerSearch.Text.Trim() == string.Empty ? "" : txtSellerSearch.Text.Split('-')[0].Trim(),
                        txtSalesTypeSearch.Text.Trim() == string.Empty ? "" : txtSalesTypeSearch.Text.Split('-')[0].Trim(), txtReceivedDateSearch.Text.Contains('_') ? "" : txtReceivedDateSearch.Text,
                        txtReportedDateSearch.Text.Contains('_') ? "" : txtReportedDateSearch.Text, txtCatalogueNumber.Text == string.Empty ? "" : txtCatalogueNumber.Text.ToUpper().Trim(),
                        ddlCompany.SelectedValue, modifiedRowList, deletedRowsList, Convert.ToString(Session["UserCode"]), out errorId);
                    transactionMaintenanceBL = null;

                    if (errorId == 2 || updateData.Tables.Count == 0)
                    {
                        ExceptionHandler("Error in saving transactions", string.Empty);
                    }
                    else if (errorId == 0 || errorId == 1)
                    {
                        LoadGridData(updateData.Tables[0], true);

                        //any of transactions selected to delete are present in TRANS_PART table and are not deleted
                        if (errorId == 1)
                        {
                            msgView.SetMessage("One or more selected transactions to be deleted have participants and cannot be deleted. Rest are deleted.", MessageType.Warning, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("Transaction updated Successfully.!", MessageType.Warning, PositionType.Auto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving transactions", ex.Message);
            }
        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "Search" || hdnButtonSelection.Value == "undefined")
            {
                btnSearch_Click(sender, e);
            }
            if (hdnButtonSelection.Value == "Reset")
            {
                btnReset_Click(sender, e);
            }

            if (hdnButtonSelection.Value == "AddTransaction")
            {
                btnAddTransaction_Click(sender, e);
            }

            hdnIsConfirmPopup.Value = "N";

        }

        protected void lbFuzzySales_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySales.SelectedValue.ToString() == "No results found")
                {
                    txtSalesTypeSearch.Text = string.Empty;
                    txtSalesTypeSearch.ToolTip = string.Empty;
                    return;
                }

                txtSalesTypeSearch.Text = lbFuzzySales.SelectedValue.ToString();
                txtSalesTypeSearch.ToolTip = lbFuzzySales.SelectedValue.ToString();
                hdnIsValidSalesType.Value = "Y";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting sales type.", ex.Message);
            }
        }

        protected void lbFuzzySalesTypeAddTrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySalesTypeAddTrans.SelectedValue.ToString() == "No results found")
                {
                    txtAddTransSalesType.Text = string.Empty;
                    txtAddTransSalesType.ToolTip = string.Empty;
                    return;
                }

                txtAddTransSalesType.Text = lbFuzzySalesTypeAddTrans.SelectedValue.ToString();
                txtAddTransSalesType.ToolTip = lbFuzzySalesTypeAddTrans.SelectedValue.ToString();
                mpeAddTransactionDetailsPopup.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting sales type.", ex.Message);
            }
        }

        protected void lbFuzzySeller_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySeller.SelectedValue.ToString() == "No results found")
                {
                    txtSellerSearch.Text = string.Empty;
                    txtSellerSearch.ToolTip = string.Empty;
                    return;
                }

                txtSellerSearch.Text = lbFuzzySeller.SelectedValue.ToString();
                txtSellerSearch.ToolTip = lbFuzzySeller.SelectedValue.ToString();
                hdnIsValidSeller.Value = "Y";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting seller.", ex.Message);
            }
        }

        protected void lbFuzzySellerAddTrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFuzzySellerAddTrans.SelectedValue.ToString() == "No results found")
            {
                txtAddTransTerritory.Text = string.Empty;
                txtAddTransTerritory.ToolTip = string.Empty;
                return;
            }

            txtAddTransTerritory.Text = lbFuzzySellerAddTrans.SelectedValue.ToString();
            txtAddTransTerritory.ToolTip = lbFuzzySellerAddTrans.SelectedValue.ToString();
            mpeAddTransactionDetailsPopup.Show();
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txtFuzzy;
                GridViewRowCollection gvRows = gvRows = gvTransactions.Rows;
                string fuzzyValue = hdnFuzzySearchField.Value == "SalesType" ? "txtSalesType" : "txtSeller";

                foreach (GridViewRow gvr in gvRows)
                {
                    if (gvr.RowIndex.ToString() == hdnGridFuzzySearchRowId.Value)
                    {
                        txtFuzzy = (gvr.FindControl(fuzzyValue) as TextBox);
                        (gvr.FindControl("hdnIsModified") as HiddenField).Value = "Y";
                        if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                        {
                            txtFuzzy.Text = string.Empty;
                            txtFuzzy.ToolTip = string.Empty;
                            return;
                        }

                        txtFuzzy.Text = lbFuzzySearch.SelectedValue.ToString();
                        txtFuzzy.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting sale type.", ex.Message);
            }
        }

        protected void valReceivedDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (txtReceivedDateSearch.Text.Trim() != "__/____" && txtReceivedDateSearch.Text.Trim() != string.Empty)
                {
                    if (txtReceivedDateSearch.Text.Contains("_"))
                    {
                        args.IsValid = false;
                        valReceivedDate.ToolTip = "Please enter a valid date in MM/YYYY format";
                        return;
                    }
                    else
                    {
                        Int32 dateYear = Convert.ToInt32(txtReceivedDateSearch.Text.Replace('_', ' ').Split('/')[1].Trim());
                        Int32 dateMonth = Convert.ToInt32(txtReceivedDateSearch.Text.Replace('_', ' ').Split('/')[0].Trim());
                        if (!(dateMonth > 0 && dateMonth < 13) || !(dateYear > 1900))
                        {
                            args.IsValid = false;
                            valReceivedDate.ToolTip = "Please enter a valid date in MM/YYYY format";
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in received date validation.", ex.Message);
            }
        }

        protected void valReportedDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (txtReportedDateSearch.Text.Trim() != "__/____" && txtReportedDateSearch.Text.Trim() != string.Empty)
                {
                    if (txtReportedDateSearch.Text.Contains("_"))
                    {
                        args.IsValid = false;
                        valReportedDate.ToolTip = "Please enter a valid date in MM/YYYY format";
                        return;
                    }
                    else
                    {
                        Int32 dateYear = Convert.ToInt32(txtReportedDateSearch.Text.Replace('_', ' ').Split('/')[1].Trim());
                        Int32 dateMonth = Convert.ToInt32(txtReportedDateSearch.Text.Replace('_', ' ').Split('/')[0].Trim());
                        if (!(dateMonth > 0 && dateMonth < 13) || !(dateYear > 1900))
                        {
                            args.IsValid = false;
                            valReportedDate.ToolTip = "Please enter a valid date in MM/YYYY format";
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reported date validation.", ex.Message);
            }
        }

        protected void valSalesEdit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                GridViewRow gvr = (GridViewRow)(source as Control).Parent.Parent;
                string salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;

                if (salesType != string.Empty && salesType != "No results found")
                {
                    DataTable dtSales = (DataTable)Session["FuzzySearchPriceGroupListTypeC"];
                    DataRow[] isSalesValid = dtSales.Select("price_group='" + salesType + "'");
                    if (isSalesValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sales type validation.", ex.Message);
            }
        }

        protected void valCurrencyEdit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                GridViewRow gvr = (GridViewRow)(source as Control).Parent.Parent;
                string currencyCode = (gvr.FindControl("txtCurrencyCode") as TextBox).Text;

                if (currencyCode != string.Empty)
                {
                    DataTable dtSales = (DataTable)Session["TransMaintCurrencyList"];
                    DataRow[] isCodeValid = dtSales.Select("currency_code='" + currencyCode + "'");
                    if (isCodeValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in currency code validation.", ex.Message);
            }
        }

        protected void valCountryEdit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                GridViewRow gvr = (GridViewRow)(source as Control).Parent.Parent;
                string countryCode = (gvr.FindControl("txtDestinationCountry") as TextBox).Text;

                if (countryCode != string.Empty)
                {
                    DataTable dtCountry = (DataTable)Session["TransMaintCountryList"];
                    DataRow[] isCodeValid = dtCountry.Select("country_code='" + countryCode + "'");
                    if (isCodeValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Country code validation.", ex.Message);
            }
        }

        protected void valCatNoEdit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                GridViewRow gvr = (GridViewRow)(source as Control).Parent.Parent;
                string catNo = (gvr.FindControl("txtCatNo") as TextBox).Text;

                if (catNo != string.Empty)
                {
                    DataTable dtSales = (DataTable)Session["TransMaintCatNoList"];
                    DataRow[] isCatNoValid = dtSales.Select("catno='" + catNo + "'");
                    if (isCatNoValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in catno validation.", ex.Message);
            }
        }

        protected void valCurrencyAddTrans_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                string currencyCode = txtAddTransCurrencyCode.Text;

                if (currencyCode != string.Empty)
                {
                    DataTable dtSales = (DataTable)Session["TransMaintCurrencyList"];
                    DataRow[] isCodeValid = dtSales.Select("currency_code='" + currencyCode + "'");
                    if (isCodeValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in currency code validation.", ex.Message);
            }
        }

        protected void valCountryAddTrans_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                string countryCode = txtAddTransDestinationCountry.Text;

                if (countryCode != string.Empty)
                {
                    DataTable dtCountry = (DataTable)Session["TransMaintCountryList"];
                    DataRow[] isCodeValid = dtCountry.Select("country_code='" + countryCode + "'");
                    if (isCodeValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Country code validation.", ex.Message);
            }
        }

        protected void valCatNoAddTrans_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                string catNo = txtAddTransCatNo.Text;

                if (catNo != string.Empty)
                {
                    DataTable dtSales = (DataTable)Session["TransMaintCatNoList"];
                    DataRow[] isCatNoValid = dtSales.Select("catno='" + catNo + "'");
                    if (isCatNoValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in catno validation.", ex.Message);
            }
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                GridView gridValue = gvTransactions;
                Repeater repeaterPage = rptPager;
                if (Session["TransMaint"] == null)
                    return;

                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);
                DataTable dtGridData = (DataTable)Session["TransMaint"];
                Utilities.PopulateGridPage(pageIndex, dtGridData, gridDefaultPageSize, gridValue, dtEmpty, repeaterPage);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        protected void btnAddTransaction_Click(object sender, EventArgs e)
        {
            ClearAddTransactionDetailsControls();
            mpeAddTransactionDetailsPopup.Show();
        }

        protected void btnSaveTransactions_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("valGrpAddTransactions");
                if (!Page.IsValid)
                {
                    mpeAddTransactionDetailsPopup.Show();
                }
                else
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string receivedDate = txtReceivedDateAddTrans.Text.Substring(0, 2) + "/" + mfi.GetMonthName(Convert.ToInt32(txtReceivedDateAddTrans.Text.Substring(3, 2))).Substring(0, 3) + "/" + txtReceivedDateAddTrans.Text.Substring(6, 4);
                    string reportedDate = txtAddTransReportedDate.Text.Substring(0, 2) + "/" + mfi.GetMonthName(Convert.ToInt32(txtAddTransReportedDate.Text.Substring(3, 2))).Substring(0, 3) + "/" + txtAddTransReportedDate.Text.Substring(6, 4);

                    transactionMaintenanceBL = new TransactionMaintenanceBL();
                    transactionMaintenanceBL.AddTransactionDetails(receivedDate,
                        txtAddTransSalesType.Text.Trim() == string.Empty ? "" : txtAddTransSalesType.Text.Split('-')[0].Trim(), txtAddTransSales1.Text == string.Empty ? "" : txtAddTransSales1.Text,
                       txtAddTransReceipts.Text == string.Empty ? "" : txtAddTransReceipts.Text, txtAddTransDolExchRate.Text == string.Empty ? "" : txtAddTransDolExchRate.Text,
                       reportedDate, txtAddTransPrice1.Text == string.Empty ? "" : txtAddTransPrice1.Text,
                       txtAddTransSales2.Text == string.Empty ? "" : txtAddTransSales2.Text, txtAddTransReceipts2.Text == string.Empty ? "" : txtAddTransReceipts2.Text,
                       txtAddTransCurrencyCode.Text == string.Empty ? "" : txtAddTransCurrencyCode.Text.ToUpper(), txtAddTransCatNo.Text == string.Empty ? "" : txtAddTransCatNo.Text.ToUpper().Trim(),
                       txtAddTransPrice2.Text == string.Empty ? "" : txtAddTransPrice2.Text, txtAddTransSales3.Text == string.Empty ? "" : txtAddTransSales3.Text,
                       txtAddTransReceipts3.Text == string.Empty ? "" : txtAddTransReceipts3.Text, txtAddTransWhtTax.Text == string.Empty ? "" : txtAddTransWhtTax.Text,
                        txtAddTransTerritory.Text.Trim() == string.Empty ? "" : txtAddTransTerritory.Text.Split('-')[0].Trim(), txtAddTransPrice3.Text == string.Empty ? "" : txtAddTransPrice3.Text,
                        txtAddTransDestinationCountry.Text == string.Empty ? "" : txtAddTransDestinationCountry.Text,
                        ddlAddTransCompanyCode.SelectedValue == "-" ? hdnCompanyCode.Value : ddlAddTransCompanyCode.SelectedValue, txtAddTransOwnerShare.Text == string.Empty ? "" : txtAddTransOwnerShare.Text,
                        txtAddTransTotalShare.Text == string.Empty ? "" : txtAddTransTotalShare.Text, Convert.ToString(Session["UserCode"]), out errorId);
                    transactionMaintenanceBL = null;

                    if (errorId == 2)
                    {
                        ExceptionHandler("Error in saving transactions", string.Empty);
                    }
                    else if (errorId == 0)
                    {
                        msgView.SetMessage("Transaction saved successfully.!", MessageType.Warning, PositionType.Auto);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving transactions", ex.Message);
            }
        }

        protected void btnFuzzyTerritoryListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchSellerGroupListTypeC(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
                lbFuzzySearch.DataSource = searchList;
                lbFuzzySearch.DataBind();
                mpeFuzzySearch.Show();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory fuzzy search popup", ex.Message);
            }
        }

        protected void btnFuzzySalesTypeListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchPriceGroupListTypeC(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
                lbFuzzySearch.DataSource = searchList;
                lbFuzzySearch.DataBind();
                mpeFuzzySearch.Show();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sales type fuzzy search popup", ex.Message);
            }
        }

        protected void btnFuzzyTerritoryListPopupAddTrans_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchSellerGroupListTypeC(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
                lbFuzzySellerAddTrans.DataSource = searchList;
                lbFuzzySellerAddTrans.DataBind();
                lbFuzzySellerAddTrans.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                mpeAddTransactionDetailsPopup.Show();
                mpeFuzzySellerAddTrans.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory fuzzy search popup", ex.Message);
            }
        }

        protected void btnFuzzySalesTypeListPopupAddTrans_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchPriceGroupListTypeC(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
                lbFuzzySalesTypeAddTrans.DataSource = searchList;
                lbFuzzySalesTypeAddTrans.DataBind();
                lbFuzzySalesTypeAddTrans.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                mpeAddTransactionDetailsPopup.Show();
                mpeFuzzySalesTypeAddTrans.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sales type fuzzy search popup", ex.Message);
            }
        }

        protected void btnHdnCatNoSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SearchedData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in search.", ex.Message);
            }
        }

        protected void btnHdnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SearchedData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in search.", ex.Message);
            }
        }

        protected void gvTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DropDownList ddlCompanyCode;
                string hdnCompanyCode;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlCompanyCode = (e.Row.FindControl("ddlCompanyCodeGrid") as DropDownList);
                    hdnCompanyCode = (e.Row.FindControl("hdnCompanyCodeGrid") as HiddenField).Value;

                    if ((DataTable)Session["TransactionMaintCompanyCodeList"] != null)
                    {
                        ddlCompanyCode.DataSource = (DataTable)Session["TransactionMaintCompanyCodeList"];
                        ddlCompanyCode.DataTextField = "company_name";
                        ddlCompanyCode.DataValueField = "company_code";
                        ddlCompanyCode.DataBind();
                        ddlCompanyCode.Items.Insert(0, new ListItem("-"));
                        if (ddlCompanyCode.Items.FindByValue(hdnCompanyCode) != null)
                        {
                            ddlCompanyCode.Items.FindByValue(hdnCompanyCode).Selected = true;
                        }
                        else
                        {
                            ddlCompanyCode.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private Array DeletedRowsList()
        {
            GridViewRowCollection gvRows = gvRows = gvTransactions.Rows;
            List<string> deleteList = new List<string>();
            CheckBox chkDeleteChecked;
            string transactionId;
            string catno;
            foreach (GridViewRow row in gvRows)
            {
                chkDeleteChecked = (CheckBox)row.FindControl("chkDelete");

                if (chkDeleteChecked.Checked == true)
                {
                    transactionId = ((HiddenField)row.FindControl("hdnTransactionId")).Value;
                    catno = ((TextBox)row.FindControl("txtCatNo")).Text;
                    deleteList.Add(transactionId + Global.DBDelimiter + catno);
                }
            }
            return deleteList.ToArray();
        }

        private void LoadInitialData()
        {
            try
            {
                tdTrans.Visible = false;
                transactionMaintenanceBL = new TransactionMaintenanceBL();
                DataSet fuzzySearchData = transactionMaintenanceBL.GetInitialData(out errorId);

                if (fuzzySearchData.Tables.Count != 0)
                {
                    catNoList = fuzzySearchData.Tables[0];
                    currencyList = fuzzySearchData.Tables[1];
                    dtCountryCode = fuzzySearchData.Tables[3];
                    Session["TransMaintCatnoList"] = catNoList;
                    Session["TransMaintCurrencyList"] = currencyList;
                    Session["TransMaintCountryList"] = dtCountryCode;
                    Session["TransactionMaintCompanyCodeList"] = fuzzySearchData.Tables[2];
                    DataRow[] foundRows = fuzzySearchData.Tables[2].Select("primary = 'Y'");
                    if (foundRows.Length != 0)
                    {
                        DataTable dtPrimary = foundRows.CopyToDataTable();
                        hdnCompanyCode.Value = dtPrimary.Rows[0]["company_code"].ToString();
                    }
                    if (Session["FuzzySearchPriceGroupListTypeC"] == null)
                    {
                        DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetPriceGroupListTypeC", out errorId);
                        Session["FuzzySearchPriceGroupListTypeC"] = dsList.Tables[0];
                    }

                    ddlAddTransCompanyCode.DataTextField = "company_name";
                    ddlAddTransCompanyCode.DataValueField = "company_code";
                    ddlAddTransCompanyCode.DataSource = fuzzySearchData.Tables[2];
                    ddlAddTransCompanyCode.DataBind();
                    ddlAddTransCompanyCode.Items.Insert(0, new ListItem("-"));
                    if (ddlAddTransCompanyCode.Items.FindByValue(hdnCompanyCode.Value) != null)
                    {
                        ddlAddTransCompanyCode.Items.FindByValue(hdnCompanyCode.Value).Selected = true;
                    }
                    else
                    {
                        ddlAddTransCompanyCode.SelectedIndex = 0;
                    }

                    ddlCompany.DataTextField = "company_name";
                    ddlCompany.DataValueField = "company_code";
                    ddlCompany.DataSource = fuzzySearchData.Tables[2];
                    ddlCompany.DataBind();
                    ddlCompany.Items.Insert(0, new ListItem("-"));
                    if (ddlCompany.Items.FindByValue(hdnCompanyCode.Value) != null)
                    {
                        ddlCompany.Items.FindByValue(hdnCompanyCode.Value).Selected = true;
                    }
                    else
                    {
                        ddlCompany.SelectedIndex = 0;
                    }
                }

                gvTransactions.DataSource = dtEmpty;
                gvTransactions.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in load initial data.", ex.Message);
            }
        }

        private Array ModifiedRowsList()
        {
            List<string> modifiedRowsList = new List<string>();
            string receivedDate;
            string reportedDate;
            string catNo;
            string territory;
            string salesType;
            string price1;
            string price2;
            string price3;
            string sales1;
            string sales2;
            string sales3;
            string receipts;
            string receipts2;
            string receipts3;
            string repDolExchRate;
            string currencyCode;
            string whtMultiplier;
            string transactionId;
            string hdnIsModified;
            string currencyModified;
            string hdnCurrencyCode;
            string companyCode;
            string destinationCountry;

            System.Globalization.DateTimeFormatInfo mfi = new
                       System.Globalization.DateTimeFormatInfo();
            foreach (GridViewRow gvr in gvTransactions.Rows)
            {
                hdnIsModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                if (hdnIsModified == "Y")
                {
                    transactionId = (gvr.FindControl("hdntransactionId") as HiddenField).Value;
                    receivedDate = (gvr.FindControl("txtReceivedDate") as TextBox).Text;
                    reportedDate = (gvr.FindControl("txtReportedDate") as TextBox).Text;
                    catNo = (gvr.FindControl("txtCatNo") as TextBox).Text;
                    territory = (gvr.FindControl("lblSeller") as Label).Text;
                    salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;
                    price1 = (gvr.FindControl("txtPrice1") as TextBox).Text;
                    price2 = (gvr.FindControl("txtPrice2") as TextBox).Text;
                    price3 = (gvr.FindControl("txtPrice3") as TextBox).Text;
                    sales1 = (gvr.FindControl("txtSales1") as TextBox).Text;
                    sales2 = (gvr.FindControl("txtSales2") as TextBox).Text;
                    sales3 = (gvr.FindControl("txtSales3") as TextBox).Text;
                    receipts = (gvr.FindControl("txtReceipts") as TextBox).Text;
                    receipts2 = (gvr.FindControl("txtReceipts2") as TextBox).Text;
                    receipts3 = (gvr.FindControl("txtReceipts3") as TextBox).Text;
                    repDolExchRate = (gvr.FindControl("txtDolExchRate") as TextBox).Text;
                    currencyCode = (gvr.FindControl("txtCurrencyCode") as TextBox).Text.ToUpper();
                    destinationCountry = (gvr.FindControl("txtDestinationCountry") as TextBox).Text;
                    whtMultiplier = (gvr.FindControl("txtWhtTax") as TextBox).Text;
                    hdnCurrencyCode = (gvr.FindControl("hdnCurrencyCode") as HiddenField).Value;

                    if ((gvr.FindControl("ddlCompanyCodeGrid") as DropDownList).SelectedIndex > 0)
                    {
                        companyCode = (gvr.FindControl("ddlCompanyCodeGrid") as DropDownList).SelectedValue;
                    }
                    else
                    {
                        companyCode = hdnCompanyCode.Value;
                    }

                    if (hdnCurrencyCode != currencyCode)
                    {
                        currencyModified = "Y";
                    }
                    else
                    {
                        currencyModified = "N";
                    }

                    //WHT tax % is displayed as (1- WHT_MULTIPLIER) * 100 eg multiplier = .85 displays as 15%
                    //Entered value is a % - converted to multiplier while saving (entered value / 100)
                    //screen field is non mandatory and when nothing is entered, it should save as 1 in database
                    //for any values other than empty it should be saved as (100-value)/100 in database
                    whtMultiplier = whtMultiplier.Trim() == string.Empty ? "0" : whtMultiplier.Trim();
                    receivedDate = receivedDate.Substring(0, 2) + "/" + mfi.GetMonthName(Convert.ToInt32(receivedDate.Substring(3, 2))).Substring(0, 3) + "/" + receivedDate.Substring(6, 4);
                    reportedDate = reportedDate.Substring(0, 2) + "/" + mfi.GetMonthName(Convert.ToInt32(reportedDate.Substring(3, 2))).Substring(0, 3) + "/" + reportedDate.Substring(6, 4);
                    modifiedRowsList.Add(transactionId + Global.DBDelimiter + receivedDate + Global.DBDelimiter + reportedDate + Global.DBDelimiter + catNo + Global.DBDelimiter + territory.Split('-')[0].Trim() + Global.DBDelimiter + salesType.Split('-')[0].Trim() + Global.DBDelimiter + price1 + Global.DBDelimiter + price2 + Global.DBDelimiter + price3 + Global.DBDelimiter + sales1 + Global.DBDelimiter + sales2 + Global.DBDelimiter + sales3 + Global.DBDelimiter + receipts + Global.DBDelimiter + receipts2 + Global.DBDelimiter + receipts3 + Global.DBDelimiter + repDolExchRate + Global.DBDelimiter + currencyCode + Global.DBDelimiter + whtMultiplier + Global.DBDelimiter + currencyModified + Global.DBDelimiter + companyCode + Global.DBDelimiter + destinationCountry);
                }
            }
            return modifiedRowsList.ToArray();
        }

        private void SearchedData()
        {
            DataSet searchedData;
            Page.Validate("valGrpSearch");
            if (!Page.IsValid)
            {
                return;
            }
            else
            {
                transactionMaintenanceBL = new TransactionMaintenanceBL();
                searchedData = transactionMaintenanceBL.GetSearchedTransactionsData(txtSellerSearch.Text.Trim() == string.Empty ? "" : txtSellerSearch.Text.Split('-')[0].Trim(), txtSalesTypeSearch.Text.Trim() == string.Empty ? "" : txtSalesTypeSearch.Text.Split('-')[0].Trim(), txtReceivedDateSearch.Text.Contains('_') ? "" : txtReceivedDateSearch.Text, txtReportedDateSearch.Text.Contains('_') ? "" : txtReportedDateSearch.Text, txtCatalogueNumber.Text == string.Empty ? "" : txtCatalogueNumber.Text.Replace("'", "").Trim().ToUpper(), ddlCompany.SelectedValue == "-" ? "" : ddlCompany.SelectedValue, out errorId);
            }
            transactionMaintenanceBL = null;

            if (searchedData.Tables.Count != 0 && errorId != 2)
            {
                LoadGridData(searchedData.Tables[0], false);
            }

        }

        private void LoadGridData(DataTable inputData, bool retainCurrentPage)
        {
            GridView gridValue;
            Repeater repeaterPage;

            tdTrans.Visible = true;
            gridValue = gvTransactions;
            repeaterPage = rptPager;
            DataView dv = inputData.DefaultView;
            dv.Sort = gridDataSortOrder;
            DataTable dtSorted = dv.ToTable();

            if (!retainCurrentPage)
            {
                hdnPageNumber.Value = "1";
            }

            Session["TransMaint"] = dtSorted;
            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtSorted, gridDefaultPageSize, gridValue, dtEmpty, repeaterPage);
        }

        private void ClearAddTransactionDetailsControls()
        {
            txtAddTransCatNo.Text = string.Empty;
            txtAddTransCurrencyCode.Text = string.Empty;
            txtAddTransDolExchRate.Text = string.Empty;
            txtAddTransPrice1.Text = string.Empty;
            txtAddTransPrice2.Text = string.Empty;
            txtAddTransPrice3.Text = string.Empty;
            txtAddTransReceipts.Text = string.Empty;
            txtAddTransReceipts2.Text = string.Empty;
            txtAddTransReceipts3.Text = string.Empty;
            txtAddTransReportedDate.Text = string.Empty;
            txtAddTransSales1.Text = string.Empty;
            txtAddTransSales2.Text = string.Empty;
            txtAddTransSales3.Text = string.Empty;
            txtAddTransTerritory.Text = string.Empty;
            txtAddTransWhtTax.Text = string.Empty;
            txtAddTransSalesType.Text = string.Empty;
            txtReceivedDateAddTrans.Text = string.Empty;
            txtAddTransDestinationCountry.Text = string.Empty;
            txtAddTransOwnerShare.Text = string.Empty;
            txtAddTransTotalShare.Text = string.Empty;
        }

        private void UserAuthorization()
        {
           
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSaveChanges.Enabled = false;
                btnAddTransaction.Enabled = false;
                btnCancelTransactions.Enabled = false;
            }
        }


        #endregion Methods

        #region Fuzzy Search

        protected void fuzzySearchSales_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchPriceGroupListTypeC(txtSalesTypeSearch.Text.Trim().ToUpper(), int.MaxValue);
                lbFuzzySales.DataSource = searchList;
                lbFuzzySales.DataBind();
                mpeFuzzySales.Show();
                lbFuzzySales.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sales type fuzzy search popup", ex.Message);
            }
        }

        protected void btnFuzzySearch_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchPriceGroupListTypeC(hdnFuzzySearchText.Value.Trim().ToUpper(), int.MaxValue);
                lbFuzzySearch.DataSource = searchList;
                lbFuzzySearch.DataBind();
                mpeFuzzySearch.Show();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Sales Type fuzzy search popup", ex.Message);
            }

        }

        protected void fuzzySearchSeller_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchSellerGroupListTypeC(txtSellerSearch.Text.Trim().ToUpper(), int.MaxValue);
                lbFuzzySeller.DataSource = searchList;
                lbFuzzySeller.DataBind();
                mpeFuzzySeller.Show();
                lbFuzzySeller.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Territory type fuzzy search popup", ex.Message);
            }
        }

        #endregion Fuzzy Search

    }
}