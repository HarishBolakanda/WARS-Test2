/*
File Name   :   RoyaltorEscalationRatesAudit.cs
Purpose     :   To display Royaltor Escalation Rates  Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     20-Oct-2017     Pratik(Infosys Limited)   Initial Creation (WUIN-295)
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
    public partial class RoyaltorEscalationRatesAudit : System.Web.UI.Page
    {
        #region Global Declarations
        RoyaltorEscalationRatesAuditBL royaltorEscalationRatesAuditBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Escalation Rates Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Escalation Rates Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlEscalationRatesAudit.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtRoyaltorSearch.Focus();
                    tdData.Style.Add("display", "none");
                    tdDataHeader.Style.Add("display", "none");
                    trRoyAudit.Style.Add("display", "none");



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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SearchEscalationRatesAuditData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in searching audit data.", ex.Message);
            }
        }

        protected void btnEscalationRates_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    hdnIsMaintScreen.Value = "Y";
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "MaintScreen", "RedirectToMaintScreen(" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + ");", true);

                    // Response.Redirect(@"~/Contract/RoyContractEscRates.aspx?RoyaltorId=" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + "&isNewRoyaltor=N", false);
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

        protected void gvEscalationRatesAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                Label lblEscCode;
                Label lblTerritory;
                Label lblConfigCode;
                Label lblSalesType;
                Label lblSalesAbove;
                Label lblSalesPerc;
                Label lblRoyaltyRate;
                Label lblUnitRate;
                Label lblRevenueRate;

                HiddenField hdnClrEscCode ;
                HiddenField hdnClrSeller ;
                HiddenField hdnClrConfig ;
                HiddenField hdnClrPrice;
                HiddenField hdnClrThreshUnits;
                HiddenField hdnClrSalesPct;
                HiddenField hdnClrRoyRate;
                HiddenField hdnClrUnitRate;
                HiddenField hdnClrRevenueRate;

                string changeType;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblEscCode = (e.Row.FindControl("lblEscCode") as Label);
                    lblTerritory = (e.Row.FindControl("lblTerritory") as Label);
                    lblConfigCode = (e.Row.FindControl("lblConfigCode") as Label);
                    lblSalesType = (e.Row.FindControl("lblSalesType") as Label);
                    lblSalesAbove = (e.Row.FindControl("lblSalesAbove") as Label);
                    lblSalesPerc = (e.Row.FindControl("lblSalesPerc") as Label);
                    lblRoyaltyRate = (e.Row.FindControl("lblRoyaltyRate") as Label);
                    lblUnitRate = (e.Row.FindControl("lblUnitRate") as Label);
                    lblRevenueRate = (e.Row.FindControl("lblRevenueRate") as Label);

                    hdnClrEscCode = (e.Row.FindControl("hdnClrEscCode") as HiddenField);
                    hdnClrSeller = (e.Row.FindControl("hdnClrSeller") as HiddenField);
                    hdnClrConfig = (e.Row.FindControl("hdnClrConfig") as HiddenField);
                    hdnClrPrice = (e.Row.FindControl("hdnClrPrice") as HiddenField);
                    hdnClrThreshUnits = (e.Row.FindControl("hdnClrThreshUnits") as HiddenField);
                    hdnClrSalesPct = (e.Row.FindControl("hdnClrSalesPct") as HiddenField);
                    hdnClrRoyRate = (e.Row.FindControl("hdnClrRoyRate") as HiddenField);
                    hdnClrUnitRate = (e.Row.FindControl("hdnClrUnitRate") as HiddenField);
                    hdnClrRevenueRate = (e.Row.FindControl("hdnClrRevenueRate") as HiddenField);

                    changeType = (e.Row.FindControl("hdnChangeType") as HiddenField).Value;


                     lblEscCode.ForeColor = hdnClrEscCode.Value == "R" ? Color.Red : Color.Black;
                     lblTerritory.ForeColor = hdnClrSeller.Value == "R" ? Color.Red : Color.Black;
                     lblConfigCode.ForeColor = hdnClrConfig.Value == "R" ? Color.Red : Color.Black;
                     lblSalesType.ForeColor = hdnClrPrice.Value == "R" ? Color.Red : Color.Black;
                     lblSalesAbove.ForeColor = hdnClrThreshUnits.Value == "R" ? Color.Red : Color.Black;
                     lblSalesPerc.ForeColor = hdnClrSalesPct.Value == "R" ? Color.Red : Color.Black;
                     lblRoyaltyRate.ForeColor = hdnClrRoyRate.Value == "R" ? Color.Red : Color.Black;
                     lblUnitRate.ForeColor = hdnClrUnitRate.Value == "R" ? Color.Red : Color.Black;
                     lblRevenueRate.ForeColor = hdnClrRevenueRate.Value == "R" ? Color.Red : Color.Black;


                    //For deleted records 
                    //Change the color of details to red
                    if (changeType == "D")
                    {
                        lblEscCode.ForeColor = Color.Red;
                        lblRoyaltyRate.ForeColor = Color.Red;
                        lblTerritory.ForeColor = Color.Red;
                        lblConfigCode.ForeColor = Color.Red;
                        lblSalesType.ForeColor = Color.Red;
                        lblSalesAbove.ForeColor = Color.Red;
                        lblSalesPerc.ForeColor = Color.Red;
                        lblRoyaltyRate.ForeColor = Color.Red;
                        lblUnitRate.ForeColor = Color.Red;
                        lblRevenueRate.ForeColor = Color.Red;
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

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvEscalationRatesAudit_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyEscRatesAuditData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvEscalationRatesAudit.DataSource = dataView;
                gvEscalationRatesAudit.DataBind();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

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
                SearchEscalationRatesAuditData();
            }
        }

        private void SearchEscalationRatesAuditData()
        {

            Page.Validate("valSearch");
            if (!Page.IsValid)
            {
                dtEmpty = new DataTable();
                BindGrid(dtEmpty);

                msgView.SetMessage("Invalid search input!", MessageType.Warning, PositionType.Auto);
                return;
            }

            tdData.Style.Remove("display");
            tdDataHeader.Style.Remove("display");
            trRoyAudit.Style.Remove("display");

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


            royaltorEscalationRatesAuditBL = new RoyaltorEscalationRatesAuditBL();
            DataSet auditData = royaltorEscalationRatesAuditBL.GetEscalationRatesAuditData(txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim(), fromDate, toDate, out errorId);
            royaltorEscalationRatesAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {
                auditData.Tables[0].Columns.AddRange(new DataColumn[] {
                new DataColumn("clr_esc_code", typeof(string)), 
                new DataColumn("clr_seller", typeof(string)),
                new DataColumn("clr_config", typeof(string)),
                new DataColumn("clr_price", typeof(string)),
                new DataColumn("clr_threshold_units", typeof(string)),
                new DataColumn("clr_sales_pct", typeof(string)),
                new DataColumn("clr_royalty_rate", typeof(string)),
                new DataColumn("clr_unit_rate", typeof(string)),
                new DataColumn("clr_revenue_rate", typeof(string))
                });
                Session["RoyEscRatesAuditData"] = auditData.Tables[0];
                BindGrid(auditData.Tables[0]);

            }
            else
            {
                ExceptionHandler("Error in fetching royaltor payee audit data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (gridData.Rows.Count > 0)
            {
                gvEscalationRatesAudit.DataSource = gridData;
                gvEscalationRatesAudit.DataBind();
                CompareRoyAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvEscalationRatesAudit.DataSource = dtEmpty;
                gvEscalationRatesAudit.DataBind();
            }
        }

        private void CompareRoyAuditRows()
        {
            DataTable dtAuditData = (DataTable)Session["RoyEscRatesAuditData"];

            for (int i = 0; i < gvEscalationRatesAudit.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvEscalationRatesAudit.Rows[i];

                for (int j = i + 1; j < gvEscalationRatesAudit.Rows.Count; j++)
                {
                    GridViewRow nextRow = gvEscalationRatesAudit.Rows[j];

                    if ((currentRow.FindControl("hdnRateId") as HiddenField).Value == (nextRow.FindControl("hdnRateId") as HiddenField).Value)
                    {

                        //Compare Esc Code
                        if ((currentRow.FindControl("lblEscCode") as Label).Text != (nextRow.FindControl("lblEscCode") as Label).Text)
                        {
                            (currentRow.FindControl("lblEscCode") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_esc_code"] = "R";
                        }

                        //Comapre Territory
                        if ((currentRow.FindControl("lblTerritory") as Label).Text != (nextRow.FindControl("lblTerritory") as Label).Text)
                        {
                            (currentRow.FindControl("lblTerritory") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_seller"] = "R";

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

                        //Compare Sales Above
                        if ((currentRow.FindControl("lblSalesAbove") as Label).Text != (nextRow.FindControl("lblSalesAbove") as Label).Text)
                        {
                            (currentRow.FindControl("lblSalesAbove") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_threshold_units"] = "R";

                        }

                        //Compare Sales Percentage
                        if ((currentRow.FindControl("lblSalesPerc") as Label).Text != (nextRow.FindControl("lblSalesPerc") as Label).Text)
                        {
                            (currentRow.FindControl("lblSalesPerc") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_sales_pct"] = "R";

                        }

                        //Compare Royalty Rate
                        if ((currentRow.FindControl("lblRoyaltyRate") as Label).Text != (nextRow.FindControl("lblRoyaltyRate") as Label).Text)
                        {
                            (currentRow.FindControl("lblRoyaltyRate") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_royalty_rate"] = "R";

                        }

                        //Compare Unit Rate
                        if ((currentRow.FindControl("lblUnitRate") as Label).Text != (nextRow.FindControl("lblUnitRate") as Label).Text)
                        {
                            (currentRow.FindControl("lblUnitRate") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_unit_rate"] = "R";

                        }

                        //Comapre Revenue Rate
                        if ((currentRow.FindControl("lblRevenueRate") as Label).Text != (nextRow.FindControl("lblRevenueRate") as Label).Text)
                        {
                            (currentRow.FindControl("lblRevenueRate") as Label).ForeColor = Color.Red;
                            dtAuditData.Rows[i]["clr_revenue_rate"] = "R";

                        }


                    }
                }
            }
            Session["RoyEscRatesAuditData"] = dtAuditData;
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