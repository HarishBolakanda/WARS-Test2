/*
File Name   :   InterestedPartyAudit.cs
Purpose     :   to maintain interested party data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     16-Mar-2016     Sreelekha   Initial Creation
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
    public partial class InterestedPartyAudit : System.Web.UI.Page
    {
        #region Global Declarations

        InterestedPartyAuditBL interestedPartyAuditBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string sessionIntParty = "";
        string sessionIntPartyType = "";
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Interested Party Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Interested Party Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlInterestedPartyDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtInterestedPartySearch.Focus();
                    tdData.Style.Add("display", "none");
                    txtData.Style.Add("display", "none");

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
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

        protected void gvInterestedPartyDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string changeType;
                Label lblSendStatement;
                Label lblGenerateInvoice;
                Label lblMisMatchAddress;
                Label lblInterestedPartyType;
                Label lblInterestedPartyName;
                Label lblInterestedPartyAdd1;
                Label lblInterestedPartyAdd2;
                Label lblInterestedPartyAdd3;
                Label lblInterestedPartyAdd4;
                Label lblInterestedPartyPostcode;
                Label lblInterestedPartyEmail;
                Label lblTaxNumber;
                Label lblUpdatedby;
                Label lblUpdatedon;
                Label lblAppTax;//JIRA-1144 CHanges

                HiddenField hdnClrIntPartName;
                HiddenField hdnClrIntPartyAdd1;
                HiddenField hdnClrIntPartyAdd2;
                HiddenField hdnClrIntPartyAdd3;
                HiddenField hdnClrIntPartyAdd4;
                HiddenField hdnClrPostCode;
                HiddenField hdnClrEmail;
                HiddenField hdnClrTaxNumber;
                HiddenField hdnClrApplicalbeTax;
                HiddenField hdnClrMismatchAdd;
                HiddenField hdnClrSendStmt;
                HiddenField hdnClrGenerateInv;


                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    changeType = (e.Row.FindControl("hdnChangeType") as HiddenField).Value;
                    lblSendStatement = (e.Row.FindControl("lblSendStatement") as Label);
                    lblGenerateInvoice = (e.Row.FindControl("lblGenerateInvoice") as Label);
                    lblMisMatchAddress = (e.Row.FindControl("lblMisMatchAddress") as Label);
                    lblInterestedPartyType = (e.Row.FindControl("lblInterestedPartyType") as Label);
                    lblInterestedPartyName = (e.Row.FindControl("lblInterestedPartyName") as Label);
                    lblInterestedPartyAdd1 = (e.Row.FindControl("lblInterestedPartyAdd1") as Label);
                    lblInterestedPartyAdd2 = (e.Row.FindControl("lblInterestedPartyAdd2") as Label);
                    lblInterestedPartyAdd3 = (e.Row.FindControl("lblInterestedPartyAdd3") as Label);
                    lblInterestedPartyAdd4 = (e.Row.FindControl("lblInterestedPartyAdd4") as Label);
                    lblInterestedPartyPostcode = (e.Row.FindControl("lblInterestedPartyPostcode") as Label);
                    lblInterestedPartyEmail = (e.Row.FindControl("lblInterestedPartyEmail") as Label);
                    lblTaxNumber = (e.Row.FindControl("lblTaxNumber") as Label);
                    lblAppTax = (e.Row.FindControl("lblApplicableTax") as Label);//JIRA-1144 CHanges
                    lblUpdatedby = (e.Row.FindControl("lblUpdatedby") as Label);
                    lblUpdatedon = (e.Row.FindControl("lblUpdatedon") as Label);

                    hdnClrIntPartName = (e.Row.FindControl("hdnClrIntPartName") as HiddenField);
                    hdnClrIntPartyAdd1 = (e.Row.FindControl("hdnClrIntPartyAdd1") as HiddenField);
                    hdnClrIntPartyAdd2 = (e.Row.FindControl("hdnClrIntPartyAdd2") as HiddenField);
                    hdnClrIntPartyAdd3 = (e.Row.FindControl("hdnClrIntPartyAdd3") as HiddenField);
                    hdnClrIntPartyAdd4 = (e.Row.FindControl("hdnClrIntPartyAdd4") as HiddenField);
                    hdnClrPostCode = (e.Row.FindControl("hdnClrPostCode") as HiddenField);
                    hdnClrEmail = (e.Row.FindControl("hdnClrEmail") as HiddenField);
                    hdnClrTaxNumber = (e.Row.FindControl("hdnClrTaxNumber") as HiddenField);
                    hdnClrApplicalbeTax = (e.Row.FindControl("hdnClrApplicalbeTax") as HiddenField);
                    hdnClrMismatchAdd = (e.Row.FindControl("hdnClrMismatchAdd") as HiddenField);
                    hdnClrSendStmt = (e.Row.FindControl("hdnClrSendStmt") as HiddenField);
                    hdnClrGenerateInv = (e.Row.FindControl("hdnClrGenerateInv") as HiddenField);

                    lblInterestedPartyName.ForeColor = hdnClrIntPartName.Value == "R" ? Color.Red : Color.Black;
                    lblInterestedPartyAdd1.ForeColor = hdnClrIntPartyAdd1.Value == "R" ? Color.Red : Color.Black;
                    lblInterestedPartyAdd2.ForeColor = hdnClrIntPartyAdd2.Value == "R" ? Color.Red : Color.Black;
                    lblInterestedPartyAdd3.ForeColor = hdnClrIntPartyAdd3.Value == "R" ? Color.Red : Color.Black;
                    lblInterestedPartyAdd4.ForeColor = hdnClrIntPartyAdd4.Value == "R" ? Color.Red : Color.Black;
                    lblInterestedPartyPostcode.ForeColor = hdnClrPostCode.Value == "R" ? Color.Red : Color.Black;
                    lblInterestedPartyEmail.ForeColor = hdnClrEmail.Value == "R" ? Color.Red : Color.Black;
                    lblTaxNumber.ForeColor = hdnClrTaxNumber.Value == "R" ? Color.Red : Color.Black;
                    lblAppTax.ForeColor = hdnClrApplicalbeTax.Value == "R" ? Color.Red : Color.Black;
                    lblSendStatement.ForeColor = hdnClrSendStmt.Value == "R" ? Color.Red : Color.Black;
                    lblGenerateInvoice.ForeColor = hdnClrGenerateInv.Value == "R" ? Color.Red : Color.Black;
                    lblMisMatchAddress.ForeColor = hdnClrMismatchAdd.Value == "R" ? Color.Red : Color.Black;

                    //For deleted records 
                    //Change the color of details to red
                    if (changeType == "D")
                    {

                        lblInterestedPartyType.ForeColor = Color.Red;
                        lblInterestedPartyName.ForeColor = Color.Red;
                        lblInterestedPartyAdd1.ForeColor = Color.Red;
                        lblInterestedPartyAdd2.ForeColor = Color.Red;
                        lblInterestedPartyAdd3.ForeColor = Color.Red;
                        lblInterestedPartyAdd4.ForeColor = Color.Red;
                        lblInterestedPartyPostcode.ForeColor = Color.Red;
                        lblTaxNumber.ForeColor = Color.Red;
                        lblAppTax.ForeColor = Color.Red;  //JIRA-1144 CHanges
                        lblSendStatement.ForeColor = Color.Red;
                        lblGenerateInvoice.ForeColor = Color.Red;
                        lblMisMatchAddress.ForeColor = Color.Red;
                        lblUpdatedby.ForeColor = Color.Red;
                        lblUpdatedon.ForeColor = Color.Red;

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

        protected void btnInterestedPartyMaint_Click(object sender, EventArgs e)
        {
            if (hdnIsValidSearch.Value == "Y")
            {
                IntPartySession();
            }

            Response.Redirect("../DataMaintenance/InterestedPartyMaintenance.aspx?fromAudit=Y");
        }

        protected void btnHdnInterestedPartySearch_Click(object sender, EventArgs e)
        {
            try
            {
                int intPartyId = Convert.ToInt32(txtInterestedPartySearch.Text.Split('-')[0].ToString().Trim());
                LoadInterestedPartyAuditData(intPartyId);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting interested party.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtInterestedPartySearch.Text = string.Empty;
                    return;
                }

                txtInterestedPartySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                int intPartyId = Convert.ToInt32(txtInterestedPartySearch.Text.Split('-')[0].ToString().Trim());
                LoadInterestedPartyAuditData(intPartyId);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting interested party.", ex.Message);
            }
        }

        protected void fuzzySearchIntParty_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchInterestedParty();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in interested party search.", ex.Message);
            }
        }

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvInterestedPartyDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["IntPartyAuditData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvInterestedPartyDetails.DataSource = dataView;
                gvInterestedPartyDetails.DataBind();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End


        #endregion Events

        #region Methods

        private void LoadData()
        {

            if (Request.QueryString.Count > 0 && !string.IsNullOrEmpty(Request.QueryString[0]))
            {
                int intPartyId = Convert.ToInt32(Request.QueryString[0]);

                if (intPartyId != 0)
                {
                    LoadInterestedPartyAuditData(intPartyId);
                }
                else
                {
                    dtEmpty = new DataTable();
                    gvInterestedPartyDetails.DataSource = dtEmpty;
                    gvInterestedPartyDetails.DataBind();
                }

            }
        }

        private void LoadInterestedPartyAuditData(int intPartyId)
        {
            hdnIsValidSearch.Value = "Y";

            string intParty;
            interestedPartyAuditBL = new InterestedPartyAuditBL();
            DataSet interestedPartyData = interestedPartyAuditBL.GetInterestedPartyAuditData(intPartyId, out intParty, out errorId);
            interestedPartyAuditBL = null;

            txtInterestedPartySearch.Text = intParty;

            if (interestedPartyData.Tables.Count != 0 && errorId != 2)
            {
                interestedPartyData.Tables[0].Columns.AddRange(new DataColumn[] {
                new DataColumn("clr_int_party_name", typeof(string)), 
                new DataColumn("clr_int_party_add1", typeof(string)),
                new DataColumn("clr_int_party_add2", typeof(string)),
                new DataColumn("clr_int_party_add3", typeof(string)),
                new DataColumn("clr_int_party_add4", typeof(string)),
                new DataColumn("clr_int_party_postcode", typeof(string)),
                new DataColumn("clr_email", typeof(string)),
                new DataColumn("clr_vat_number", typeof(string)),
                new DataColumn("clr_applicable_tax_type", typeof(string)),
                new DataColumn("clr_mismatch_address", typeof(string)),
                new DataColumn("clr_send_statement", typeof(string)),
                new DataColumn("clr_generate_invoice", typeof(string))
                });

                Session["IntPartyAuditData"] = interestedPartyData.Tables[0];
                BindGrid(interestedPartyData.Tables[0]);
               
            }
            else
            {
                ExceptionHandler("Error in fetching interested data", string.Empty);
            }
        }

        private void IntPartySession()
        {
            if (Session["intPartyMaintFilters"] != null)
            {
                DataTable dtIntPartySearchedFilters = Session["intPartyMaintFilters"] as DataTable;

                foreach (DataRow dRow in dtIntPartySearchedFilters.Rows)
                {
                    if (dRow["filter_name"].ToString() == "txtInterestedPartySearch")
                    {
                        sessionIntParty = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "ddlIntPartyType")
                    {
                        sessionIntPartyType = dRow["filter_value"].ToString();
                    }
                }
            }

            DataTable dtSearchedFilters = new DataTable();
            dtSearchedFilters.Columns.Add("filter_name", typeof(string));
            dtSearchedFilters.Columns.Add("filter_value", typeof(string));

            string intPartyName = txtInterestedPartySearch.Text.Split('-')[1].ToString().Trim();
            dtSearchedFilters.Rows.Add("txtInterestedPartySearch", intPartyName);

            if (sessionIntParty == intPartyName)
            {
                dtSearchedFilters.Rows.Add("ddlIntPartyType", sessionIntPartyType);
            }

            Session["intPartyMaintFilters"] = dtSearchedFilters;
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            tdData.Style.Remove("display");
            txtData.Style.Remove("display");
            if (gridData.Rows.Count > 0)
            {
                gvInterestedPartyDetails.DataSource = gridData;
                gvInterestedPartyDetails.DataBind();
                CompareRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvInterestedPartyDetails.EmptyDataText = "No data is displayed initially.";
                gvInterestedPartyDetails.DataSource = dtEmpty;
                gvInterestedPartyDetails.DataBind();
            }
        }

        private void CompareRows()
        {
            DataTable dtIntPartyAuditData = (DataTable)Session["IntPartyAuditData"];
            for (int i = 0; i < gvInterestedPartyDetails.Rows.Count - 1; i++)
            {
                //exit if it is last row
                if (i == (gvInterestedPartyDetails.Rows.Count - 1))
                {
                    break;
                }

                GridViewRow currentRow = gvInterestedPartyDetails.Rows[i];

                if ((currentRow.FindControl("hdnDisplayOrder") as HiddenField).Value == "1")
                {
                    continue;
                }

                GridViewRow nextRow = gvInterestedPartyDetails.Rows[i + 1];

                //Compare  Name
                if ((currentRow.FindControl("lblInterestedPartyName") as Label).Text != (nextRow.FindControl("lblInterestedPartyName") as Label).Text)
                {
                    (currentRow.FindControl("lblInterestedPartyName") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_int_party_name"] = "R";
                }
                //Compare Address1
                if ((currentRow.FindControl("lblInterestedPartyAdd1") as Label).Text != (nextRow.FindControl("lblInterestedPartyAdd1") as Label).Text)
                {
                    (currentRow.FindControl("lblInterestedPartyAdd1") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_int_party_add1"] = "R";
                }
                //Compare Address2
                if ((currentRow.FindControl("lblInterestedPartyAdd2") as Label).Text != (nextRow.FindControl("lblInterestedPartyAdd2") as Label).Text)
                {
                    (currentRow.FindControl("lblInterestedPartyAdd2") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_int_party_add2"] = "R";
                }
                //Compare Address3
                if ((currentRow.FindControl("lblInterestedPartyAdd3") as Label).Text != (nextRow.FindControl("lblInterestedPartyAdd3") as Label).Text)
                {
                    (currentRow.FindControl("lblInterestedPartyAdd3") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_int_party_add3"] = "R";
                }
                //Compare Address4
                if ((currentRow.FindControl("lblInterestedPartyAdd4") as Label).Text != (nextRow.FindControl("lblInterestedPartyAdd4") as Label).Text)
                {
                    (currentRow.FindControl("lblInterestedPartyAdd4") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_int_party_add4"] = "R";
                }
                //Compare Post code
                if ((currentRow.FindControl("lblInterestedPartyPostcode") as Label).Text != (nextRow.FindControl("lblInterestedPartyPostcode") as Label).Text)
                {
                    (currentRow.FindControl("lblInterestedPartyPostcode") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_int_party_postcode"] = "R";
                }
                //Compare Email
                if ((currentRow.FindControl("lblInterestedPartyEmail") as Label).Text != (nextRow.FindControl("lblInterestedPartyEmail") as Label).Text)
                {
                    (currentRow.FindControl("lblInterestedPartyEmail") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_email"] = "R";
                }
                //Compare Tax Number
                if ((currentRow.FindControl("lblTaxNumber") as Label).Text != (nextRow.FindControl("lblTaxNumber") as Label).Text)
                {
                    (currentRow.FindControl("lblTaxNumber") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_vat_number"] = "R";
                }
                //JIRA-1144 Changes -- STart
                //Compare Applicable Tax
                if ((currentRow.FindControl("lblApplicableTax") as Label).Text != (nextRow.FindControl("lblApplicableTax") as Label).Text)
                {
                    (currentRow.FindControl("lblApplicableTax") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_applicable_tax_type"] = "R";
                }
                //JIRA-1144 Changes -- End

                //Compare send stmt
                if ((currentRow.FindControl("lblSendStatement") as Label).Text != (nextRow.FindControl("lblSendStatement") as Label).Text)
                {
                    (currentRow.FindControl("lblSendStatement") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_send_statement"] = "R";
                }
                //Compare mismatch address
                if ((currentRow.FindControl("lblMisMatchAddress") as Label).Text != (nextRow.FindControl("lblMisMatchAddress") as Label).Text)
                {
                    (currentRow.FindControl("lblMisMatchAddress") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_mismatch_address"] = "R";
                }

                //WUIN-1030
                //Compare Generate Invoice
                if ((currentRow.FindControl("lblGenerateInvoice") as Label).Text != (nextRow.FindControl("lblGenerateInvoice") as Label).Text)
                {
                    (currentRow.FindControl("lblGenerateInvoice") as Label).ForeColor = Color.Red;
                    dtIntPartyAuditData.Rows[i]["clr_generate_invoice"] = "R";
                }

            }
            Session["IntPartyAuditData"] = dtIntPartyAuditData;

        }

        private void FuzzySearchInterestedParty()
        {
            if (txtInterestedPartySearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text intereseted party search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchIntPartyList(txtInterestedPartySearch.Text.Trim().ToUpper(), int.MaxValue);
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



    }
}