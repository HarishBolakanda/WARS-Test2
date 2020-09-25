/*
File Name   :   RoyaltorPayeeAudit.cs
Purpose     :   To display Royaltor Payee  Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     24-Sep-2017     Pratik(Infosys Limited)   Initial Creation
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
    public partial class RoyaltorPayeeAudit : System.Web.UI.Page
    {
        #region Global Declarations
        RoyaltorPayeeAuditBL royaltorPayeeAuditBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Payee Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Payee Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlRoyaltorPayeeAudit.Style.Add("height", hdnGridPnlHeight.Value);

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

        protected void btnHdnRoyaltorSearch_Click(object sender, EventArgs e)
        {
            try
            {
                RoyaltorSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading audit details.", ex.Message);
            }
        }

        protected void btnPayeeDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    hdnIsMaintScreen.Value = "Y";
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "MaintScreen", "RedirectToMaintScreen(" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + ");", true);
                   
                    //Response.Redirect(@"~/Contract/RoyContractPayee.aspx?RoyaltorId=" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + "&isNewRoyaltor=N", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid royaltor from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to contract maintenance screen.", ex.Message);
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
                RoyaltorSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }

        protected void gvRoyaltorPayeeAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                Label lblPrimaryHdr;
                Label lblPercentageShareHdr;
                Label lblPayHdr;
                   Label lblSupplierNumberHdr;
                Label lblSupplierSiteNameHdr;
                Label lblUpdatedByHrd;
                Label lblUpdatedOnHdr;

                Label lblPrimary;
                Label lblIntPartyName;
                Label lblPercentageShare;
                Label lblAddress1;
                Label lblPay;
                Label lblPostcode;
                Label lblSupplierNumber;
                Label lblSupplierSiteName;

                string intPartyType;
                string changeType;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblPrimaryHdr = (e.Row.FindControl("lblPrimaryHdr") as Label);
                    lblPercentageShareHdr = (e.Row.FindControl("lblPercentageShareHdr") as Label);
                    lblPayHdr = (e.Row.FindControl("lblPayHdr") as Label);
                    lblSupplierNumberHdr = (e.Row.FindControl("lblSupplierNumberHdr") as Label);
                    lblSupplierSiteNameHdr = (e.Row.FindControl("lblSupplierSiteNameHdr") as Label);
                    lblUpdatedByHrd = (e.Row.FindControl("lblUpdatedByHrd") as Label);
                    lblUpdatedOnHdr = (e.Row.FindControl("lblUpdatedOnHdr") as Label);

                    lblPrimary = (e.Row.FindControl("lblPrimary") as Label);
                    lblIntPartyName = (e.Row.FindControl("lblIntPartyName") as Label);
                    lblPercentageShare = (e.Row.FindControl("lblPercentageShare") as Label);
                    lblAddress1 = (e.Row.FindControl("lblAddress1") as Label);
                    lblPay = (e.Row.FindControl("lblPay") as Label);
                    lblPostcode = (e.Row.FindControl("lblPostcode") as Label);
                    lblSupplierNumber = (e.Row.FindControl("lblSupplierNumber") as Label);
                    lblSupplierSiteName = (e.Row.FindControl("lblSupplierSiteName") as Label);

                    intPartyType = (e.Row.FindControl("hdnIntPartyType") as HiddenField).Value;
                    changeType = (e.Row.FindControl("hdnChangeType") as HiddenField).Value;

                    //For courtesy type hide Primary Payee, Percentage Share, Pay, supplier or VAT Number details
                    if (intPartyType == "C")
                    {
                        lblPrimaryHdr.Visible = false;
                        lblPercentageShareHdr.Visible = false;
                        lblPayHdr.Visible = false;
                        lblSupplierNumberHdr.Visible = false;
                        lblSupplierSiteNameHdr.Visible = false;

                        lblPrimary.Visible = false;
                        lblPercentageShare.Visible = false;
                        lblPay.Visible = false;
                        lblSupplierNumber.Visible = false;
                        lblSupplierSiteName.Visible = false;
                        
                    }

                    //For deleted records cahnge updated by, updated on to deleted by and deleted on
                    //Change the color of details to red
                    if (changeType == "D")
                    {
                        lblUpdatedByHrd.Text = "Deleted by";
                        lblUpdatedOnHdr.Text = "Deleted on";
                        lblPrimary.ForeColor = Color.Red;
                        lblIntPartyName.ForeColor = Color.Red;
                        lblPercentageShare.ForeColor = Color.Red;
                        lblAddress1.ForeColor = Color.Red;
                        lblPay.ForeColor = Color.Red;
                        lblPostcode.ForeColor = Color.Red;
                        lblSupplierNumber.ForeColor = Color.Red;
                        lblSupplierSiteName.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
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
                RoyaltorSelected();
            }
        }

        private void RoyaltorSelected()
        {
            hdnIsValidSearch.Value = "Y";
            tdData.Style.Remove("display");

            royaltorPayeeAuditBL = new RoyaltorPayeeAuditBL();
            DataSet auditData = royaltorPayeeAuditBL.GetRoyPayeeAuditData(Convert.ToInt32(txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim()), out errorId);
            royaltorPayeeAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {
                BindGrid(auditData.Tables[0]);
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
                gvRoyaltorPayeeAudit.DataSource = gridData;
                gvRoyaltorPayeeAudit.DataBind();
                CompareRoyAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvRoyaltorPayeeAudit.DataSource = dtEmpty;
                gvRoyaltorPayeeAudit.DataBind();
            }
        }

        private void CompareRoyAuditRows()
        {
            for (int i = 0; i < gvRoyaltorPayeeAudit.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvRoyaltorPayeeAudit.Rows[i];
                GridViewRow nextRow = gvRoyaltorPayeeAudit.Rows[i + 1];

                if ((currentRow.FindControl("hdnIntPartyId") as HiddenField).Value == (nextRow.FindControl("hdnIntPartyId") as HiddenField).Value)
                {

                    //Comapre Primary
                    if ((currentRow.FindControl("lblPrimary") as Label).Text != (nextRow.FindControl("lblPrimary") as Label).Text)
                    {
                        (currentRow.FindControl("lblPrimary") as Label).ForeColor = Color.Red;
                    }

                    //Compare Percentage Share
                    if ((currentRow.FindControl("lblPercentageShare") as Label).Text != (nextRow.FindControl("lblPercentageShare") as Label).Text)
                    {
                        (currentRow.FindControl("lblPercentageShare") as Label).ForeColor = Color.Red;
                    }

                    //Compare Pay
                    if ((currentRow.FindControl("lblPay") as Label).Text != (nextRow.FindControl("lblPay") as Label).Text)
                    {
                        (currentRow.FindControl("lblPay") as Label).ForeColor = Color.Red;
                    }

                    //Supplier Number
                    if ((currentRow.FindControl("lblSupplierNumber") as Label).Text != (nextRow.FindControl("lblSupplierNumber") as Label).Text)
                    {
                        (currentRow.FindControl("lblSupplierNumber") as Label).ForeColor = Color.Red;
                    }

                    //Supplier Site Name
                    if ((currentRow.FindControl("lblSupplierSiteName") as Label).Text != (nextRow.FindControl("lblSupplierSiteName") as Label).Text)
                    {
                        (currentRow.FindControl("lblSupplierSiteName") as Label).ForeColor = Color.Red;
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