/*
File Name   :   RoyaltorPayeeSuppAudit.cs
Purpose     :   To display Royaltor Payee Supplier Link Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     30-Oct-2017     Pratik(Infosys Limited)   Initial Creation
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
    public partial class RoyaltorPayeeSuppAudit : System.Web.UI.Page
    {
        #region Global Declarations
        RoyaltorPayeeSuppAuditBL royaltorPayeeSuppAuditBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Payee Supplier Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Payee Supplier Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlPayeeSuppAudit.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtRoyaltorSearch.Focus();
                    tdData.Style.Add("display", "none");

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

        protected void btnHdnRoyaltorSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ddlPayee.Items.Clear();
                SearchRoyPayeeSuppAuditData(string.Empty);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error loading audit data.", ex.Message);
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

        protected void ddlPayee_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    if (ddlPayee.SelectedValue != "-")
                    {
                        SearchRoyPayeeSuppAuditData(ddlPayee.SelectedValue);
                    }
                    else
                    {
                        dtEmpty = new DataTable();
                        gvPayeeSuppAudit.DataSource = dtEmpty;
                        gvPayeeSuppAudit.DataBind();
                    }
                }
                else
                {
                    msgView.SetMessage("Please select a valid royaltor from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error loading audit data.", ex.Message);
            }
        }

        protected void gvPayeeSuppAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                Label lblSupplierNumber;
                Label lblSupplierSiteName;
                Label lblName;
                Label lblAddress1;
                Label lblPostcode;
                Label lblMismatchAddress;
                Label lblPayeeCurrency;

                string isDeleted;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblSupplierNumber = (e.Row.FindControl("lblSupplierNumber") as Label);
                    lblSupplierSiteName = (e.Row.FindControl("lblSupplierSiteName") as Label);
                    lblName = (e.Row.FindControl("lblName") as Label);
                    lblAddress1 = (e.Row.FindControl("lblAddress1") as Label);
                    lblPostcode = (e.Row.FindControl("lblPostcode") as Label);
                    lblMismatchAddress = (e.Row.FindControl("lblMismatchAddress") as Label);
                    lblPayeeCurrency = (e.Row.FindControl("lblPayeeCurrency") as Label);
                    isDeleted = (e.Row.FindControl("hdnIsDeleted") as HiddenField).Value;

                    //Change the color of details to red
                    if (isDeleted == "Y")
                    {
                        lblMismatchAddress.ForeColor = Color.Red;
                        lblPayeeCurrency.ForeColor = Color.Red;
                        lblSupplierNumber.ForeColor = Color.Red;
                        lblSupplierSiteName.ForeColor = Color.Red;
                        lblName.ForeColor = Color.Red;
                        lblAddress1.ForeColor = Color.Red;
                        lblPostcode.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
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
                ddlPayee.Items.Clear();
                SearchRoyPayeeSuppAuditData(string.Empty);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }

        protected void btnSupplierLink_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    hdnIsMaintScreen.Value = "Y";
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "MaintScreen", "RedirectToMaintScreen(" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + ");", true);
                   
                   // Response.Redirect(@"~/Contract/RoyContractPayeeSupp.aspx?RoyaltorId=" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + "&isNewRoyaltor=N", false);
               
                }
                else
                {
                    msgView.SetMessage("Please select a valid royaltor from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to royalty payee supplier screen.", ex.Message);
            }
        }

        protected void valRoyaltor_ServerValidate(object source, ServerValidateEventArgs args)
        {

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
                hdnIsValidSearch.Value = "Y";
                SearchRoyPayeeSuppAuditData(Request.QueryString[1]);
                if (ddlPayee.Items.FindByValue(Request.QueryString[1]) != null)
                {
                    ddlPayee.ClearSelection();
                    ddlPayee.Items.FindByValue(Request.QueryString[1]).Selected = true;
                }
            }
        }

        private void SearchRoyPayeeSuppAuditData(string intPartyId)
        {

            Page.Validate("valSearch");
            if (!Page.IsValid)
            {
                dtEmpty = new DataTable();
                gvPayeeSuppAudit.DataSource = dtEmpty;
                gvPayeeSuppAudit.DataBind();

                msgView.SetMessage("Invalid search input!", MessageType.Warning, PositionType.Auto);
                return;
            }

            tdData.Style.Remove("display");
            string fromDate = string.Empty;
            string toDate = string.Empty;

            royaltorPayeeSuppAuditBL = new RoyaltorPayeeSuppAuditBL();
            DataSet auditData = royaltorPayeeSuppAuditBL.GetRoyPayeeSuppAuditData(txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim(), intPartyId, out errorId);
            royaltorPayeeSuppAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {
                //We need to show data where supplier was added for the 1st time. Need to hide all records before that.
                DataTable dtTemp = auditData.Tables[0].Copy();
                for (int i = dtTemp.Rows.Count - 1; i >= 0; i--)
                {
                    //Check if supplier number is null. If null means supplier is not linked to payee yet
                    if (string.IsNullOrEmpty(dtTemp.Rows[i]["supplier_number"].ToString()))
                    {
                        dtTemp.Rows.Remove(dtTemp.Rows[i]);
                        dtTemp.AcceptChanges();
                    }
                    else
                    {
                        //Break as supplier number is not null    
                        break;
                    }
                }

                BindGrid(dtTemp);

                if (ddlPayee.Items.Count == 0)
                {
                    //Populate the dropdown
                    ddlPayee.DataSource = auditData.Tables[1];
                    ddlPayee.DataTextField = "item_text";
                    ddlPayee.DataValueField = "item_value";
                    ddlPayee.DataBind();
                    ddlPayee.Items.Insert(0, new ListItem("-"));

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
                gvPayeeSuppAudit.DataSource = gridData;
                gvPayeeSuppAudit.DataBind();
                CompareRoyAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvPayeeSuppAudit.DataSource = dtEmpty;
                gvPayeeSuppAudit.DataBind();
            }
        }

        private void CompareRoyAuditRows()
        {
            for (int i = 0; i < gvPayeeSuppAudit.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvPayeeSuppAudit.Rows[i];
                GridViewRow nextRow = gvPayeeSuppAudit.Rows[i + 1];

                //Comapre Supplier Number
                if ((currentRow.FindControl("lblSupplierNumber") as Label).Text != (nextRow.FindControl("lblSupplierNumber") as Label).Text)
                {
                    (currentRow.FindControl("lblSupplierNumber") as Label).ForeColor = Color.Red;
                }

                //Comapre Supplier Site Name
                if ((currentRow.FindControl("lblSupplierSiteName") as Label).Text != (nextRow.FindControl("lblSupplierSiteName") as Label).Text)
                {
                    (currentRow.FindControl("lblSupplierSiteName") as Label).ForeColor = Color.Red;
                }

                //Compare Mismatch Address flag
                if ((currentRow.FindControl("lblMismatchAddress") as Label).Text != (nextRow.FindControl("lblMismatchAddress") as Label).Text)
                {
                    (currentRow.FindControl("lblMismatchAddress") as Label).ForeColor = Color.Red;
                }

                //Compare Payee Currency flag
                if ((currentRow.FindControl("lblPayeeCurrency") as Label).Text != (nextRow.FindControl("lblPayeeCurrency") as Label).Text)
                {
                    (currentRow.FindControl("lblPayeeCurrency") as Label).ForeColor = Color.Red;
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