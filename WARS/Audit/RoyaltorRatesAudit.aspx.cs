﻿/*
File Name   :   RoyaltorRatesAudit.cs
Purpose     :   To display Royaltor Rates  Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     19-Sep-2017     Pratik(Infosys Limited)   Initial Creation
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


namespace WARS.Audit
{
    public partial class RoyaltorRatesAudit : System.Web.UI.Page
    {

        #region Global Declarations
        RoyaltorRatesAuditBL royaltorRatesAuditBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Rates Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Rates Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlRoyaltorRatesAudit.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtRoyaltorSearch.Focus();
                    tdData.Style.Add("display", "none");
                    tdDataHeader.Style.Add("display", "none");

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();

                        this.Master.FindControl("lnkBtnHome").Visible = false; //WUIN-599 Hiding master page home button

                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        if (Convert.ToString(Session["UserCode"]) != Convert.ToString(Session["ContractScreenLockedUser"]))
                        {
                            hdnOtherUserScreenLocked.Value = "Y";
                        }
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

        protected void gvRoyaltorRatesAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                Label lblTerritory;
                Label lblCatno;
                Label lblConfigCode;
                Label lblSalesType;
                Label lblPercentageSales;
                Label lblRoyaltyRate;
                Label lblUnitRate;
                Label lblPricefield;
                Label lblUpdatedby;
                Label lblUpdatedon;
                Label lblDeletedby;
                Label lblDeletedon;

                string intRateType;
                string changeType;

                HiddenField hdnClrSeller;
                HiddenField hdnClrCatno;
                HiddenField hdnClrPrice;
                HiddenField hdnClrConfig;
                HiddenField hdnClrSalesPct;
                HiddenField hdnClrRoyRate;
                HiddenField hdnClrUnitRate;
                HiddenField hdnClrPriceField;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblTerritory = (e.Row.FindControl("lblTerritory") as Label);
                    lblCatno = (e.Row.FindControl("lblCatno") as Label);
                    lblConfigCode = (e.Row.FindControl("lblConfigCode") as Label);
                    lblSalesType = (e.Row.FindControl("lblSalesType") as Label);
                    lblPercentageSales = (e.Row.FindControl("lblPercentageSales") as Label);
                    lblRoyaltyRate = (e.Row.FindControl("lblRoyaltyRate") as Label);
                    lblUnitRate = (e.Row.FindControl("lblUnitRate") as Label);
                    lblPricefield = (e.Row.FindControl("lblPricefield") as Label);
                    lblUpdatedby = (e.Row.FindControl("lblUpdatedby") as Label);
                    lblUpdatedon = (e.Row.FindControl("lblUpdatedon") as Label);
                    lblDeletedby = (e.Row.FindControl("lblDeletedby") as Label);
                    lblDeletedon = (e.Row.FindControl("lblDeletedon") as Label);

                    intRateType = (e.Row.FindControl("hdnRateType") as HiddenField).Value;
                    changeType = (e.Row.FindControl("hdnChangeType") as HiddenField).Value;


                    hdnClrSeller = (e.Row.FindControl("hdnClrSeller") as HiddenField);
                    hdnClrCatno = (e.Row.FindControl("hdnClrCatno") as HiddenField); ;
                    hdnClrPrice = (e.Row.FindControl("hdnClrPrice") as HiddenField); ;
                    hdnClrConfig = (e.Row.FindControl("hdnClrConfig") as HiddenField); ;
                    hdnClrSalesPct = (e.Row.FindControl("hdnClrSalesPct") as HiddenField); ;
                    hdnClrRoyRate = (e.Row.FindControl("hdnClrRoyRate") as HiddenField); ;
                    hdnClrUnitRate = (e.Row.FindControl("hdnClrUnitRate") as HiddenField); ;
                    hdnClrPriceField = (e.Row.FindControl("hdnClrPriceField") as HiddenField); ;

                    lblTerritory.ForeColor = hdnClrSeller.Value == "R" ? Color.Red : Color.Black;
                    lblCatno.ForeColor = hdnClrCatno.Value == "R" ? Color.Red : Color.Black;
                    lblSalesType.ForeColor = hdnClrPrice.Value == "R" ? Color.Red : Color.Black;
                    lblConfigCode.ForeColor = hdnClrConfig.Value == "R" ? Color.Red : Color.Black;
                    lblPercentageSales.ForeColor = hdnClrSalesPct.Value == "R" ? Color.Red : Color.Black;
                    lblRoyaltyRate.ForeColor = hdnClrRoyRate.Value == "R" ? Color.Red : Color.Black;
                    lblUnitRate.ForeColor = hdnClrUnitRate.Value == "R" ? Color.Red : Color.Black;
                    lblPricefield.ForeColor = hdnClrPriceField.Value == "R" ? Color.Red : Color.Black;

                    //For rate_type = 'P' display Royalty Rate
                    //For rate_type = 'U' display Unit Rate
                    //Else hide both Royalty Rate & Unit Rate
                    if (intRateType == "P")
                    {
                        lblUnitRate.Text = string.Empty;
                    }
                    else if (intRateType == "U")
                    {
                        lblRoyaltyRate.Text = string.Empty;
                    }
                    else
                    {
                        lblRoyaltyRate.Text = string.Empty;
                        lblUnitRate.Text = string.Empty;
                    }

                    //Change the color of details to red
                    if (changeType == "D")
                    {
                        lblRoyaltyRate.ForeColor = Color.Red;
                        lblTerritory.ForeColor = Color.Red;
                        lblCatno.ForeColor = Color.Red;
                        lblConfigCode.ForeColor = Color.Red;
                        lblSalesType.ForeColor = Color.Red;
                        lblPercentageSales.ForeColor = Color.Red;
                        lblRoyaltyRate.ForeColor = Color.Red;
                        lblUnitRate.ForeColor = Color.Red;
                        lblPricefield.ForeColor = Color.Red;
                        lblUpdatedby.ForeColor = Color.Red;
                        lblUpdatedon.ForeColor = Color.Red;
                        lblDeletedby.ForeColor = Color.Red;
                        lblDeletedon.ForeColor = Color.Red;

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
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }

        protected void btnRoyaltorRatesDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    hdnIsMaintScreen.Value = "Y";
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "MaintScreen", "RedirectToMaintScreen(" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + ");", true);

                    //Response.Redirect(@"~/Contract/RoyContractRoyRates.aspx?RoyaltorId=" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + "&isNewRoyaltor=N", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid royaltor from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to royalty rate screen.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtRoyaltorSearch.Text = string.Empty;
                    return;
                }

                txtRoyaltorSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                hdnIsValidSearch.Value = "Y";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnRoyaltorId.Value.Trim() == txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim())
                {
                    if (ddlOptionPeriod.SelectedIndex > 0)
                    {
                        SearchRoyRatesAuditData(ddlOptionPeriod.SelectedValue);
                    }
                    else
                    {
                        SearchRoyRatesAuditData(string.Empty);
                    }
                }
                else
                {
                    ddlOptionPeriod.Items.Clear();
                    SearchRoyRatesAuditData(string.Empty);
                    hdnRoyaltorId.Value = txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in searching audit data.", ex.Message);
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
                ExceptionHandler("Error in royaltor search.", ex.Message);
            }
        }

        protected void ddlOptionPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlOptionPeriod.SelectedIndex > 0)
            {
                SearchRoyRatesAuditData(ddlOptionPeriod.SelectedValue);
            }
            else
            {
                ddlOptionPeriod.SelectedIndex = 1;
                SearchRoyRatesAuditData(ddlOptionPeriod.SelectedValue);
            }
        }

        protected void valRoyaltor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating royaltor.", ex.Message);
            }
        }

        protected void valFromDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtFromDate.Text.Trim() != "__/__/____" && txtFromDate.Text.Trim() != "")
                {
                    if (DateTime.TryParse(txtFromDate.Text, out temp))
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

        protected void valToDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtToDate.Text.Trim() != "__/__/____" && txtToDate.Text.Trim() != "")
                {
                    if (DateTime.TryParse(txtToDate.Text, out temp))
                    {
                        if (txtFromDate.Text.Trim() != "__/__/____" && txtFromDate.Text.Trim() != "")
                        {
                            if (DateTime.TryParse(txtFromDate.Text, out temp))
                            {
                                if (Convert.ToDateTime(txtFromDate.Text) > Convert.ToDateTime(txtToDate.Text))
                                {
                                    valToDate.ToolTip = "From date should be earlier than To date";
                                    args.IsValid = false;
                                    return;
                                }
                            }
                        }
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

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvRoyaltorRatesAudit_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortingDirection = string.Empty;
            string sortingExpression = string.Empty;
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
            DataTable dataTable = (DataTable)Session["RoyRoyRatesAuditData"];
            if (dataTable != null)
            {
                if (e.SortExpression == "roy_rate" || e.SortExpression == "unit_rate")
                {
                    sortingExpression = "rate_value";
                }
                else
                {
                    sortingExpression = e.SortExpression;
                }
                DataView dataView = new DataView(dataTable);
                dataView.Sort = sortingExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvRoyaltorRatesAudit.DataSource = dataView;
                gvRoyaltorRatesAudit.DataBind();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End
        #endregion Events

        #region Methods

        private void LoadData()
        {

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                int royaltorId = Convert.ToInt32(Request.QueryString[0]);

                //populate royaltor for the royaltor id
                if (Session["FuzzySearchAllRoyaltorList"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllRoyaltorList", out errorId);
                    Session["FuzzySearchAllRoyaltorList"] = dsList.Tables[0];
                }

                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchAllRoyaltorList"]).Select("royaltor_id = '" + royaltorId + "'");
                txtRoyaltorSearch.Text = filteredRow[0]["royaltor"].ToString();
                hdnRoyaltorId.Value = txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim();
                hdnIsValidSearch.Value = "Y";
                SearchRoyRatesAuditData(Request.QueryString[1]);
                if (ddlOptionPeriod.Items.FindByValue(Request.QueryString[1]) != null)
                {
                    ddlOptionPeriod.ClearSelection();
                    ddlOptionPeriod.Items.FindByValue(Request.QueryString[1]).Selected = true;
                }
            }
        }

        private void SearchRoyRatesAuditData(string optionPeriodCode)
        {

            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            Page.Validate("valSearch");
            if (!Page.IsValid)
            {
                dtEmpty = new DataTable();
                gvRoyaltorRatesAudit.DataSource = dtEmpty;
                gvRoyaltorRatesAudit.DataBind();

                msgView.SetMessage("Invalid search input!", MessageType.Warning, PositionType.Auto);
                return;
            }

            tdData.Style.Remove("display");
            tdDataHeader.Style.Remove("display");

            string fromDate = string.Empty;
            string toDate = string.Empty;

            if (txtFromDate.Text.Trim() != "__/__/____")
            {
                fromDate = txtFromDate.Text.Trim();
            }

            if (txtToDate.Text.Trim() != "__/__/____")
            {
                toDate = txtToDate.Text.Trim();
            }

            string userRoleID = Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower());
            royaltorRatesAuditBL = new RoyaltorRatesAuditBL();
            DataSet auditData = royaltorRatesAuditBL.GetRoyRatesAuditData(txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim(), optionPeriodCode, fromDate, toDate, userRoleID, out errorId);
            royaltorRatesAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {

                auditData.Tables[0].Columns.AddRange(new DataColumn[] { 
                new DataColumn("clr_seller", typeof(string)),
                new DataColumn("clr_catno", typeof(string)),
                new DataColumn("clr_price", typeof(string)),
                new DataColumn("clr_config", typeof(string)),
                new DataColumn("clr_sales_pct", typeof(string)),
                new DataColumn("clr_roy_rate", typeof(string)),
                new DataColumn("clr_unit_rate", typeof(string)),
                new DataColumn("clr_price_field", typeof(string))
                });
                Session["RoyRoyRatesAuditData"] = auditData.Tables[0];
                BindGrid(auditData.Tables[0]);

                if (ddlOptionPeriod.Items.Count == 0)
                {
                    //Populate the dropdown
                    ddlOptionPeriod.DataSource = auditData.Tables[1];
                    ddlOptionPeriod.DataTextField = "item_text";
                    ddlOptionPeriod.DataValueField = "item_value";
                    ddlOptionPeriod.DataBind();
                    ddlOptionPeriod.Items.Insert(0, new ListItem("-"));

                    if (auditData.Tables[1].Rows.Count > 0)
                    {
                        ddlOptionPeriod.SelectedIndex = 1;
                    }
                }
            }
            else
            {
                ExceptionHandler("Error in fetching royaltor payee audit data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            if (gridData.Rows.Count > 0)
            {
                gvRoyaltorRatesAudit.DataSource = gridData;
                gvRoyaltorRatesAudit.DataBind();
                CompareRoyAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvRoyaltorRatesAudit.DataSource = dtEmpty;
                gvRoyaltorRatesAudit.DataBind();
            }
        }

        private void CompareRoyAuditRows()
        {
            DataTable dtAuditData = (DataTable)Session["RoyRoyRatesAuditData"];
            for (int i = 0; i < gvRoyaltorRatesAudit.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvRoyaltorRatesAudit.Rows[i];

                //WUIN-611 - Rows from main table are displayed on top. Need to ignore them for coloring the changes
                if ((currentRow.FindControl("hdnDisplayOrder") as HiddenField).Value == "1")
                {
                    continue;
                }

                for (int j = i + 1; j < gvRoyaltorRatesAudit.Rows.Count; j++)
                {
                    GridViewRow nextRow = gvRoyaltorRatesAudit.Rows[j];

                    if ((currentRow.FindControl("hdnRateId") as HiddenField).Value == (nextRow.FindControl("hdnRateId") as HiddenField).Value)
                    {

                        //Comapre Territory
                        if ((currentRow.FindControl("lblTerritory") as Label).Text != (nextRow.FindControl("lblTerritory") as Label).Text)
                        {
                            (currentRow.FindControl("lblTerritory") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_seller"] = "R";
                        }

                        //Compare Catno
                        if ((currentRow.FindControl("lblCatno") as Label).Text != (nextRow.FindControl("lblCatno") as Label).Text)
                        {
                            (currentRow.FindControl("lblCatno") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_catno"] = "R";
                        }

                        //Compare Config Code
                        if ((currentRow.FindControl("lblConfigCode") as Label).Text != (nextRow.FindControl("lblConfigCode") as Label).Text)
                        {
                            (currentRow.FindControl("lblConfigCode") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_config"] = "R";
                        }

                        //Compare Sales Type
                        if ((currentRow.FindControl("lblSalesType") as Label).Text != (nextRow.FindControl("lblSalesType") as Label).Text)
                        {
                            (currentRow.FindControl("lblSalesType") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_price"] = "R";
                        }

                        //Comapre Sales Percentage
                        if ((currentRow.FindControl("lblPercentageSales") as Label).Text != (nextRow.FindControl("lblPercentageSales") as Label).Text)
                        {
                            (currentRow.FindControl("lblPercentageSales") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_sales_pct"] = "R";
                        }

                        //Compare Royalty Rate
                        if ((currentRow.FindControl("lblRoyaltyRate") as Label).Text != (nextRow.FindControl("lblRoyaltyRate") as Label).Text)
                        {
                            (currentRow.FindControl("lblRoyaltyRate") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_roy_rate"] = "R";
                        }

                        //Compare Unit Rate
                        if ((currentRow.FindControl("lblUnitRate") as Label).Text != (nextRow.FindControl("lblUnitRate") as Label).Text)
                        {
                            (currentRow.FindControl("lblUnitRate") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_unit_rate"] = "R";
                        }

                        //Compare Price Field
                        if ((currentRow.FindControl("lblPricefield") as Label).Text != (nextRow.FindControl("lblPricefield") as Label).Text)
                        {
                            (currentRow.FindControl("lblPricefield") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_price_field"] = "R";
                        }

                        break;
                    }
                }
            }
        }

        private void FuzzySearchRoyaltor()
        {
            if (txtRoyaltorSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(txtRoyaltorSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        #endregion Methods


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