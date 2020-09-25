/*
File Name   :   PaymentApproval.cs
Purpose     :   to approve royalty payments to GFS team

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     20-Nov-2017     Harish(Infosys Limited)   Initial Creation
        
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using WARS.BusinessLayer;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Net;
using AjaxControlToolkit;


namespace WARS.Payment
{
    public partial class PaymentApproval : System.Web.UI.Page
    {

        #region Global Declarations
        PaymentApprovalBL paymentApprovalBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtSearchFilters;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize1"].ToString());
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Payment Approval";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Payment Approval";
                }

                lblTab.Focus();//tabbing sequence starts here                                
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        hdnPaymentRoleId.Value = Session["UserPaymentRoleId"].ToString();
                        //load initial screen data upon request is a new one or coming from any screen where search details to be retained
                        if (Request.QueryString != null && Request.QueryString.Count > 0 && Request.QueryString["RetainSearch"] == "Y")
                        {
                            LoadInitialData();
                            PopulateSearchFields();
                            LoadSearchData();
                        }
                        else
                        {
                            LoadInitialData();
                        }
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }
                }

                UserAuthorization();

            }
            catch (ThreadAbortException ex1)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void gvPaymentApproval_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string hdnIsApprovalEnabled;
                string hdnApprovalLevel;
                string hdnAppUserCode1;
                string hdnAppUserCode2;
                string hdnAppUserCode3;
                string hdnAppUserCode4;
                string hdnAppUserCode5;
                string hdnCanUserCode;
                string hdnPayBelowThreshold;
                string hdnPaymentStatusCode;

                string hdnBulkApproval1;
                string hdnBulkApproval2;
                string hdnBulkApproval3;
                string hdnBulkApproval4;
                string hdnBulkApproval5;
                string hdnBulkCancel;

                CheckBox cbApproval1;
                CheckBox cbApproval2;
                CheckBox cbApproval3;
                CheckBox cbApproval4;
                CheckBox cbApproval5;
                CheckBox cbCancelPayment;
                Label lblstatus;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    hdnIsApprovalEnabled = (e.Row.FindControl("hdnIsApprovalEnabled") as HiddenField).Value;
                    hdnApprovalLevel = (e.Row.FindControl("hdnApprovalLevel") as HiddenField).Value;
                    hdnAppUserCode1 = (e.Row.FindControl("hdnAppUserCode1") as HiddenField).Value;
                    hdnAppUserCode2 = (e.Row.FindControl("hdnAppUserCode2") as HiddenField).Value;
                    hdnAppUserCode3 = (e.Row.FindControl("hdnAppUserCode3") as HiddenField).Value;
                    hdnAppUserCode4 = (e.Row.FindControl("hdnAppUserCode4") as HiddenField).Value;
                    hdnAppUserCode5 = (e.Row.FindControl("hdnAppUserCode5") as HiddenField).Value;
                    hdnCanUserCode = (e.Row.FindControl("hdnCanUserCode") as HiddenField).Value;
                    hdnPayBelowThreshold = (e.Row.FindControl("hdnPayBelowThreshold") as HiddenField).Value;
                    hdnPaymentStatusCode = (e.Row.FindControl("hdnPaymentStatusCode") as HiddenField).Value;

                    cbApproval1 = (e.Row.FindControl("cbApproval1") as CheckBox);
                    cbApproval2 = (e.Row.FindControl("cbApproval2") as CheckBox);
                    cbApproval3 = (e.Row.FindControl("cbApproval3") as CheckBox);
                    cbApproval4 = (e.Row.FindControl("cbApproval4") as CheckBox);
                    cbApproval5 = (e.Row.FindControl("cbApproval5") as CheckBox);
                    cbCancelPayment = (e.Row.FindControl("cbCancelPayment") as CheckBox);
                    lblstatus = (e.Row.FindControl("lblPaymentStatus") as Label);

                    hdnBulkApproval1 = (e.Row.FindControl("hdnBulkApproval1") as HiddenField).Value;
                    hdnBulkApproval2 = (e.Row.FindControl("hdnBulkApproval2") as HiddenField).Value;
                    hdnBulkApproval3 = (e.Row.FindControl("hdnBulkApproval3") as HiddenField).Value;
                    hdnBulkApproval4 = (e.Row.FindControl("hdnBulkApproval4") as HiddenField).Value;
                    hdnBulkApproval5 = (e.Row.FindControl("hdnBulkApproval5") as HiddenField).Value;
                    hdnBulkCancel = (e.Row.FindControl("hdnBulkCancel") as HiddenField).Value;

                    //check approval checkbox if corresponding user_code is not null
                    if (hdnAppUserCode1 != string.Empty)
                    {
                        cbApproval1.Checked = true;
                    }

                    if (hdnAppUserCode2 != string.Empty)
                    {
                        cbApproval2.Checked = true;
                    }

                    if (hdnAppUserCode3 != string.Empty)
                    {
                        cbApproval3.Checked = true;
                    }

                    if (hdnAppUserCode4 != string.Empty)
                    {
                        cbApproval4.Checked = true;
                    }

                    if (hdnAppUserCode5 != string.Empty)
                    {
                        cbApproval5.Checked = true;
                    }

                    if (hdnCanUserCode != string.Empty)
                    {
                        cbCancelPayment.Checked = true;

                        //disable cancel checkbox if payment is cancelled
                        cbCancelPayment.Enabled = false;
                    }


                    //Enable/disable approval checkbox                    
                    if (hdnIsApprovalEnabled == "N")
                    {
                        //disable all when is_approval_enabled = N                   
                        cbApproval1.Enabled = false;
                        cbApproval2.Enabled = false;
                        cbApproval3.Enabled = false;
                        cbApproval4.Enabled = false;
                        cbApproval5.Enabled = false;

                    }
                    else
                    {
                        //authorization: user role level enable/disable approval checkbox
                        //WUIN-410 - Check that the User approving the payment has a valid role for the approval level.													
                        //USER_ACCOUNT.PAYMENT_ROLE_ID = PAYMENT_APPROVAL.APPROVAL_ROLE_ID set for PAYMENT_APPROVAL.APPROVAL_LEVEL not < level being approved												
                        //eg if role id is set against level 3, user can approve levels 1,2 and 3	
                        if (Convert.ToInt16(hdnUserRoleApprovalLevel.Value) < 1)
                        {
                            cbApproval1.Enabled = false;
                        }

                        if (Convert.ToInt16(hdnUserRoleApprovalLevel.Value) < 2)
                        {
                            cbApproval2.Enabled = false;
                        }

                        if (Convert.ToInt16(hdnUserRoleApprovalLevel.Value) < 3)
                        {
                            cbApproval3.Enabled = false;
                        }

                        if (Convert.ToInt16(hdnUserRoleApprovalLevel.Value) < 4)
                        {
                            cbApproval4.Enabled = false;
                        }

                        if (Convert.ToInt16(hdnUserRoleApprovalLevel.Value) < 5)
                        {
                            cbApproval5.Enabled = false;
                        }

                        //Enable approval checkbox on PAYMENT.APPROVAL_LEVEL status
                        //Approved 1		Disable if PAYMENT.APPROVAL_LEVEL < 1	
                        //Approved 2		Disable if PAYMENT.APPROVAL_LEVEL < 2	
                        //Approved 3		Disable if PAYMENT.APPROVAL_LEVEL < 3
                        //Approved 4		Disable if PAYMENT.APPROVAL_LEVEL < 4
                        //Approved 5		Disable if PAYMENT.APPROVAL_LEVEL < 5
                        if (Convert.ToInt16(hdnApprovalLevel) < 1)
                        {
                            cbApproval1.Enabled = false;
                        }

                        if (Convert.ToInt16(hdnApprovalLevel) < 2)
                        {
                            cbApproval2.Enabled = false;
                        }

                        if (Convert.ToInt16(hdnApprovalLevel) < 3)
                        {
                            cbApproval3.Enabled = false;
                        }

                        if (Convert.ToInt16(hdnApprovalLevel) < 4)
                        {
                            cbApproval4.Enabled = false;
                        }

                        if (Convert.ToInt16(hdnApprovalLevel) < 5)
                        {
                            cbApproval5.Enabled = false;
                        }

                        //Harish: 11-12-2017: validation to allow approval if Below threshold
                        //Enable the first approval. Display a warning if selected 
                        if (hdnPayBelowThreshold == "Y")
                        {
                            cbApproval2.Enabled = false;
                            cbApproval3.Enabled = false;
                            cbApproval4.Enabled = false;
                            cbApproval5.Enabled = false;
                        }

                        if (hdnPayBelowThreshold == "Y" && hdnIsApprovalEnabled == "Y")
                        {
                            cbApproval1.Enabled = true;
                        }
                    }

                    //Any payment role id greater than 10 (Supervisor) should be able to cancel the payments
                    //Disable the Cancel Payment checkbox if the payment role is <= 10 (Supervisor)
                    //Enable cancellation when payment.status_code <= 7
                    if (hdnPaymentRoleId.Value == string.Empty || Convert.ToInt32(hdnPaymentRoleId.Value) <= 10
                        || !(Convert.ToInt16(hdnPaymentStatusCode) <= 7))
                    {
                        cbCancelPayment.Enabled = false;
                    }

                    //when payment.status_code = 8, display 'Payment Sent' on hover of Cancel checkbox
                    if (hdnPaymentStatusCode == "8")
                    {
                        cbCancelPayment.ToolTip = "Payment Sent";
                    }

                    if (hdnSelectedBulkApproval.Value != "")
                    {
                        if (hdnSelectedBulkApproval.Value == "1" && hdnBulkApproval1 == "Y" && cbApproval1.Enabled)
                        {
                            cbApproval1.Checked = true;
                        }
                        else if (hdnSelectedBulkApproval.Value == "2" && hdnBulkApproval2 == "Y" && cbApproval2.Enabled)
                        {
                            cbApproval2.Checked = true;
                        }
                        else if (hdnSelectedBulkApproval.Value == "3" && hdnBulkApproval3 == "Y" && cbApproval3.Enabled)
                        {
                            cbApproval3.Checked = true;
                        }
                        else if (hdnSelectedBulkApproval.Value == "4" && hdnBulkApproval4 == "Y" && cbApproval4.Enabled)
                        {
                            cbApproval4.Checked = true;
                        }
                        else if (hdnSelectedBulkApproval.Value == "5" && hdnBulkApproval5 == "Y" && cbApproval5.Enabled)
                        {
                            cbApproval5.Checked = true;
                        }
                    }
                    if (hdnBulkCancelSelected.Value == "Y")
                    {
                        if (hdnBulkCancel == "Y" && cbCancelPayment.Enabled)
                        {
                            cbCancelPayment.Checked = true;
                        }

                    }


                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payment approval loading grid data", ex.Message);
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                //WUIN-1018 - Clearing header check boxes on search changed.
                ClearBulkApproveCancel();

                LoadSearchData();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payment approval search.", ex.Message);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ddlCompany.SelectedIndex = 0;
                ddlStmtStatus.SelectedIndex = 0;
                ddlPayeeStatus.SelectedIndex = 0;
                ddlPaymentStatus.SelectedIndex = 0;
                ddlResponsibility.SelectedIndex = 0;
                txtStmtEndPeriod.Text = string.Empty;
                txtReportedDays.Text = string.Empty;
                txtOwnFuzzySearch.Text = string.Empty;
                txtPayeeFuzzySearch.Text = string.Empty;
                txtRoyaltorFuzzySearch.Text = string.Empty;
                txtBalThreshold.Text = string.Empty;
                dtEmpty = new DataTable();

                gvPaymentApproval.EmptyDataText = "No data is displayed initially.";
                gvPaymentApproval.DataSource = dtEmpty;
                gvPaymentApproval.DataBind();
                Session["PayApprSearchFilters"] = null;

                //clear pager
                Session["PayApprovalData"] = null;
                rptPager.DataSource = null;
                rptPager.DataBind();
                //WUIN-1018 - Clearing header check boxes on search changed.
                ClearBulkApproveCancel();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing and reloading the data.", ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Array paymentList = PaymentApprovalList();


                if (paymentList.Length == 0)
                {
                    msgView.SetMessage("No changes made to save", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Payment approval details not saved – invalid or missing data", MessageType.Warning, PositionType.Auto);
                    return;
                }

                string owner = string.Empty;
                string payee = string.Empty;
                string royaltor = string.Empty;

                if (txtOwnFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        owner = txtOwnFuzzySearch.Text.Substring(0, txtOwnFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the owner from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                if (txtPayeeFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        payee = txtPayeeFuzzySearch.Text.Substring(0, txtPayeeFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the payee from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                if (txtRoyaltorFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        royaltor = txtRoyaltorFuzzySearch.Text.Substring(0, txtRoyaltorFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the royaltor from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                paymentApprovalBL = new PaymentApprovalBL();
                DataSet dsPaymentData = paymentApprovalBL.SavePaymentApproval(paymentList, Convert.ToString(Session["UserCode"]), ddlCompany.SelectedValue,
                                                                                (txtStmtEndPeriod.Text == "__/____" ? "-" : txtStmtEndPeriod.Text),
                                                                                (txtReportedDays.Text == string.Empty ? "-" : txtReportedDays.Text),
                                                                                 ddlStmtStatus.SelectedValue, ddlPayeeStatus.SelectedValue, ddlPaymentStatus.SelectedValue,
                                                                                (owner == string.Empty ? "-" : owner),
                                                                                (payee == string.Empty ? "-" : payee),
                                                                                (royaltor == string.Empty ? "-" : royaltor),
                                                                                (txtBalThreshold.Text == string.Empty ? "-" : txtBalThreshold.Text),
                                                                                 ddlResponsibility.SelectedValue,
                                                                                 out errorId);
                paymentApprovalBL = null;


                if (errorId == 2 || dsPaymentData.Tables.Count == 0)
                {
                    msgView.SetMessage("Error in saving payment approvals", MessageType.Warning, PositionType.Auto);
                    return;
                }
                else if (dsPaymentData.Tables[0].Rows.Count == 0)
                {
                    dtEmpty = new DataTable();
                    gvPaymentApproval.EmptyDataText = "No data found for the selected search criteria";
                    Session["PayApprovalData"] = dtEmpty;
                    Utilities.PopulateGridPage((hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value)), dsPaymentData.Tables[0], gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);
                }
                else
                {
                    Session["PayApprovalData"] = dsPaymentData.Tables[0];
                    ClearBulkApproveCancel();
                    Session["PayBulkApprovalData"] = null;
                    Utilities.PopulateGridPage((hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value)), dsPaymentData.Tables[0], gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);
                }

                msgView.SetMessage("Changes saved successfully", MessageType.Warning, PositionType.Auto);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving payment approvals", ex.Message);
            }
        }


        protected void btnUpdateInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                string owner = string.Empty;
                string payee = string.Empty;
                string royaltor = string.Empty;

                if (txtOwnFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        owner = txtOwnFuzzySearch.Text.Substring(0, txtOwnFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the owner from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                if (txtPayeeFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        payee = txtPayeeFuzzySearch.Text.Substring(0, txtPayeeFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the payee from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                if (txtRoyaltorFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        royaltor = txtRoyaltorFuzzySearch.Text.Substring(0, txtRoyaltorFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the royaltor from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                paymentApprovalBL = new PaymentApprovalBL();
                DataSet dsPaymentData = paymentApprovalBL.UpdateInvoices(Convert.ToString(Session["UserCode"]), ddlCompany.SelectedValue,
                                                                                (txtStmtEndPeriod.Text == "__/____" ? "-" : txtStmtEndPeriod.Text),
                                                                                (txtReportedDays.Text == string.Empty ? "-" : txtReportedDays.Text),
                                                                                 ddlStmtStatus.SelectedValue, ddlPayeeStatus.SelectedValue, ddlPaymentStatus.SelectedValue,
                                                                                (owner == string.Empty ? "-" : owner),
                                                                                (payee == string.Empty ? "-" : payee),
                                                                                (royaltor == string.Empty ? "-" : royaltor),
                                                                                (txtBalThreshold.Text == string.Empty ? "-" : txtBalThreshold.Text),
                                                                                 ddlResponsibility.SelectedValue,
                                                                                 out errorId);
                paymentApprovalBL = null;

                if (errorId == 2 || dsPaymentData.Tables.Count == 0)
                {
                    msgView.SetMessage("Error in updating payment invoices", MessageType.Warning, PositionType.Auto);
                    return;
                }
                else if (dsPaymentData.Tables[0].Rows.Count == 0)
                {
                    dtEmpty = new DataTable();
                    gvPaymentApproval.EmptyDataText = "No data found for the selected search criteria";
                    Session["PayApprovalData"] = dsPaymentData.Tables[0];
                    Utilities.PopulateGridPage((hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value)), dsPaymentData.Tables[0], gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);
                }
                else
                {
                    Session["PayApprovalData"] = dsPaymentData.Tables[0];
                    Utilities.PopulateGridPage((hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value)), dsPaymentData.Tables[0], gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);
                }

                msgView.SetMessage("Payments updated successfully", MessageType.Warning, PositionType.Auto);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating payment invoices", ex.Message);
            }
        }

        protected void btnGeneratePayment_Click(object sender, EventArgs e)
        {
            try
            {
                CommonBL commonBl = new CommonBL();
                commonBl.GeneratePayments(out errorId);
                commonBl = null;
                if (errorId == 0)
                {
                    msgView.SetMessage("Job to generate payments is scheduled. ", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    msgView.SetMessage("Error in triggering payment generation job.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in generating payment details", ex.Message);
            }
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["PayApprovalData"] == null)
                    return;

                DataTable paymentData = null;
                if (hdnSelectedBulkApproval.Value != "" || hdnBulkCancelSelected.Value != "N")
                {
                    paymentData = Session["PayBulkApprovalData"] as DataTable;
                }
                else
                {
                    paymentData = Session["PayApprovalData"] as DataTable;
                }

                if (paymentData.Rows.Count == 0)
                {
                    return;
                }

                int pageIndex;
                if (hdnIsConfirmPopup.Value != "Y")
                {
                    pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                }
                else
                {
                    if (hdnPageIndex.Value == "Last")
                    {
                        double dblPageCount = (double)((decimal)paymentData.Rows.Count / decimal.Parse(gridDefaultPageSize.ToString()));
                        int pageCount = (int)Math.Ceiling(dblPageCount);

                        pageIndex = pageCount;
                    }
                    else
                    {
                        pageIndex = (hdnPageIndex.Value == "" || hdnPageIndex.Value == "First") ? 1 : Convert.ToInt32(hdnPageIndex.Value);
                    }
                }

                hdnPageIndex.Value = Convert.ToString(pageIndex);
                DataView dvGridData = new DataView(paymentData);
                Utilities.PopulateGridPage(pageIndex, dvGridData.ToTable(), gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }
        // WUIN - 1018 Bulk Approval and Cancel Events -- Start
        protected void cbHdrApproval_OnCheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkbox = (CheckBox)sender;
            string chkboxId = chkbox.ID;
            string approvalLevel = chkboxId.Substring(chkboxId.Length - 1, 1);
            if (chkbox.Checked)
            {
                if (hdnSelectedBulkApproval.Value != "")
                {
                    if (!(chkboxId.Contains(hdnSelectedBulkApproval.Value)))
                    {
                        var mainContent = Master.FindControl("ContentPlaceHolderBody");
                        CheckBox cbHdrApproval = (CheckBox)mainContent.FindControl("cbHdrApproval" + hdnSelectedBulkApproval.Value);
                        cbHdrApproval.Checked = false;


                    }
                }
                hdnSelectedBulkApproval.Value = approvalLevel;
                cbHdrCancel.Checked = false;

                if (Session["PayApprovalData"] != null)
                {
                    SetBulkUpdateFlag();
                }
            }
            else
            {
                ClearBulkApproveCancel();
                DataTable dtApproval = (DataTable)Session["PayApprovalData"];
                Utilities.PopulateGridPage(1, dtApproval, gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);

            }
        }

        protected void btnBulkConfirmYes_Click(object sender, EventArgs e)
        {

            DataTable dtBulkApproval = (DataTable)Session["PayBulkApprovalData"];
            Utilities.PopulateGridPage(1, dtBulkApproval, gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);

        }
        protected void btnBulkConfirmNo_Click(object sender, EventArgs e)
        {
            ClearBulkApproveCancel();
        }


        protected void cbHdrCancel_OnCheckedChanged(object sender, EventArgs e)
        {
            if (cbHdrCancel.Checked)
            {
                if (hdnSelectedBulkApproval.Value != "")
                {
                    var mainContent = Master.FindControl("ContentPlaceHolderBody");
                    CheckBox cbHdrApproval = (CheckBox)mainContent.FindControl("cbHdrApproval" + hdnSelectedBulkApproval.Value);
                    cbHdrApproval.Checked = false;
                }
                hdnSelectedBulkApproval.Value = "";
                if (Session["PayApprovalData"] != null)
                {
                    hdnBulkCancelSelected.Value = "Y";
                    Session["BulkCancelSelected"] = "Y";
                    SetBulkUpdateFlag();
                }
            }
            else
            {
                ClearBulkApproveCancel();
                DataTable dtApproval = (DataTable)Session["PayApprovalData"];
                Utilities.PopulateGridPage(1, dtApproval, gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);

            }

        }

        // WUIN - 1018 Bulk Approval and Cancel Events -- End
        #endregion Events

        #region Methods

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        /// <summary>
        /// No data is displayed in grid on initial load of the screen
        /// </summary>
        private void LoadInitialData()
        {
            string userRoleApprovalLevel = string.Empty;

            string sUser = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

            paymentApprovalBL = new PaymentApprovalBL();
            DataSet dsDropdownListData = paymentApprovalBL.GetInitialLoadData(sUser, out userRoleApprovalLevel, out errorId);
            paymentApprovalBL = null;

            if (errorId == 2 || dsDropdownListData.Tables.Count == 0)
            {
                return;
            }

            hdnUserRoleApprovalLevel.Value = userRoleApprovalLevel;

            ddlCompany.DataTextField = "item_text";
            ddlCompany.DataValueField = "item_value";
            ddlCompany.DataSource = dsDropdownListData.Tables[0];
            ddlCompany.DataBind();
            ddlCompany.Items.Insert(0, new ListItem("-"));

            ddlStmtStatus.DataTextField = "item_text";
            ddlStmtStatus.DataValueField = "item_value";
            ddlStmtStatus.DataSource = dsDropdownListData.Tables[1];
            ddlStmtStatus.DataBind();
            ddlStmtStatus.Items.Insert(0, new ListItem("-"));

            ddlPayeeStatus.DataTextField = "item_text";
            ddlPayeeStatus.DataValueField = "item_value";
            ddlPayeeStatus.DataSource = dsDropdownListData.Tables[2];
            ddlPayeeStatus.DataBind();
            ddlPayeeStatus.Items.Insert(0, new ListItem("-"));

            ddlPaymentStatus.DataTextField = "item_text";
            ddlPaymentStatus.DataValueField = "item_value";
            ddlPaymentStatus.DataSource = dsDropdownListData.Tables[3];
            ddlPaymentStatus.DataBind();
            ddlPaymentStatus.Items.Insert(0, new ListItem("-"));

            ddlResponsibility.DataTextField = "item_text";
            ddlResponsibility.DataValueField = "item_value";
            ddlResponsibility.DataSource = dsDropdownListData.Tables[4];
            ddlResponsibility.DataBind();
            ddlResponsibility.Items.Insert(0, new ListItem("-"));

            dtEmpty = new DataTable();
            gvPaymentApproval.EmptyDataText = "No data is displayed initially.";
            gvPaymentApproval.DataSource = dtEmpty;
            gvPaymentApproval.DataBind();

            //WUIN-1018 Clearing session data on page load
            Session["PayApprovalData"] = null;

        }

        private void PopulateSearchFields()
        {
            if (Session["PayApprSearchFilters"] != null)
            {
                dtSearchFilters = Session["PayApprSearchFilters"] as DataTable;

                foreach (DataRow dRow in dtSearchFilters.Rows)
                {
                    if (dRow["filter_name"].ToString() == "ddlCompany")
                    {
                        ddlCompany.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                    }
                    else if (dRow["filter_name"].ToString() == "txtStmtEndPeriod")
                    {
                        txtStmtEndPeriod.Text = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "txtReportedDays")
                    {
                        txtReportedDays.Text = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "ddlStmtStatus")
                    {
                        ddlStmtStatus.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                    }
                    else if (dRow["filter_name"].ToString() == "ddlPayeeStatus")
                    {
                        ddlPayeeStatus.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                    }
                    else if (dRow["filter_name"].ToString() == "ddlPaymentStatus")
                    {
                        ddlPaymentStatus.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                    }
                    else if (dRow["filter_name"].ToString() == "txtOwnFuzzySearch")
                    {
                        txtOwnFuzzySearch.Text = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "txtPayeeFuzzySearch")
                    {
                        txtPayeeFuzzySearch.Text = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "txtRoyaltorFuzzySearch")
                    {
                        txtRoyaltorFuzzySearch.Text = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "txtBalThreshold")
                    {
                        txtBalThreshold.Text = dRow["filter_value"].ToString();
                    }
                    else if (dRow["filter_name"].ToString() == "ddlResponsibility")
                    {
                        ddlResponsibility.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                    }
                }
            }
        }

        private void LoadSearchData()
        {
            try
            {            
                string owner = string.Empty;
                string payee = string.Empty;
                string royaltor = string.Empty;

                if (txtOwnFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        owner = txtOwnFuzzySearch.Text.Substring(0, txtOwnFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the owner from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                if (txtPayeeFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        payee = txtPayeeFuzzySearch.Text.Substring(0, txtPayeeFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the payee from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                if (txtRoyaltorFuzzySearch.Text != string.Empty)
                {
                    try
                    {
                        royaltor = txtRoyaltorFuzzySearch.Text.Substring(0, txtRoyaltorFuzzySearch.Text.IndexOf("-") - 1);
                    }
                    catch (Exception ex)
                    {
                        msgView.SetMessage("Please select the royaltor from search list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                }

                paymentApprovalBL = new PaymentApprovalBL();
                DataSet dsPaymentSearchData = paymentApprovalBL.GetSearchData(ddlCompany.SelectedValue,
                                                                                (txtStmtEndPeriod.Text == "__/____" ? "-" : txtStmtEndPeriod.Text),
                                                                                (txtReportedDays.Text == string.Empty ? "-" : txtReportedDays.Text),
                                                                                 ddlStmtStatus.SelectedValue, ddlPayeeStatus.SelectedValue, ddlPaymentStatus.SelectedValue,
                                                                                (owner == string.Empty ? "-" : owner),
                                                                                (payee == string.Empty ? "-" : payee),
                                                                                (royaltor == string.Empty ? "-" : royaltor),
                                                                                (txtBalThreshold.Text == string.Empty ? "-" : txtBalThreshold.Text),
                                                                                 ddlResponsibility.SelectedValue,
                                                                                 out errorId);
                paymentApprovalBL = null;

                if (errorId == 2 || dsPaymentSearchData.Tables.Count == 0)
                {
                    ExceptionHandler("Error in payment approval search", string.Empty);
                    return;
                }
                else if (dsPaymentSearchData.Tables[0].Rows.Count == 0)
                {
                    dtEmpty = new DataTable();
                    gvPaymentApproval.EmptyDataText = "No data found for the selected search criteria";
                    Session["PayApprovalData"] = dsPaymentSearchData.Tables[0];
                    Utilities.PopulateGridPage(1, dsPaymentSearchData.Tables[0], gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);
                }
                else
                {
                    Session["PayApprovalData"] = dsPaymentSearchData.Tables[0];
                    Utilities.PopulateGridPage(1, dsPaymentSearchData.Tables[0], gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);
                }
                //To retain search filter values when navigating between payment approval and details screens
                //Create a table to hold the filter values
                dtSearchFilters = new DataTable();
                dtSearchFilters.Columns.Add("filter_name", typeof(string));
                dtSearchFilters.Columns.Add("filter_value", typeof(string));

                //Add the filter values to the above created table
                dtSearchFilters.Rows.Add("ddlCompany", ddlCompany.SelectedValue);
                dtSearchFilters.Rows.Add("txtStmtEndPeriod", txtStmtEndPeriod.Text);
                dtSearchFilters.Rows.Add("txtReportedDays", txtReportedDays.Text);
                dtSearchFilters.Rows.Add("ddlStmtStatus", ddlStmtStatus.SelectedValue);
                dtSearchFilters.Rows.Add("ddlPayeeStatus", ddlPayeeStatus.SelectedValue);
                dtSearchFilters.Rows.Add("ddlPaymentStatus", ddlPaymentStatus.SelectedValue);
                dtSearchFilters.Rows.Add("txtOwnFuzzySearch", txtOwnFuzzySearch.Text);
                dtSearchFilters.Rows.Add("txtPayeeFuzzySearch", txtPayeeFuzzySearch.Text);
                dtSearchFilters.Rows.Add("txtRoyaltorFuzzySearch", txtRoyaltorFuzzySearch.Text);
                dtSearchFilters.Rows.Add("txtBalThreshold", txtBalThreshold.Text);
                dtSearchFilters.Rows.Add("ddlResponsibility", ddlResponsibility.SelectedValue);

                Session["PayApprSearchFilters"] = dtSearchFilters;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payment approval search", ex.Message);
            }
        }        

        private Array PaymentApprovalList()
        {
            List<string> paymentList = new List<string>();
            string paymentId;
            string approval1Modified;
            string approval1Checked;
            string approval2Modified;
            string approval2Checked;
            string approval3Modified;
            string approval3Checked;
            string approval4Modified;
            string approval4Checked;
            string approval5Modified;
            string approval5Checked;
            string cancelModified;
            string cancelChecked;

            string hdnPayBelowThreshold;
            string hdnAppUserCode1;
            string hdnAppUserCode2;
            string hdnAppUserCode3;
            string hdnAppUserCode4;
            string hdnAppUserCode5;
            string hdnCanUserCode;

            CheckBox cbApproval1;
            CheckBox cbApproval2;
            CheckBox cbApproval3;
            CheckBox cbApproval4;
            CheckBox cbApproval5;
            CheckBox cbCancelPayment;

            if ((hdnSelectedBulkApproval.Value == "" || hdnBulkCancelSelected.Value == "") && (hdnBulkDataModified.Value == "N"))
            {

                foreach (GridViewRow gvr in gvPaymentApproval.Rows)
                {
                    paymentId = (gvr.FindControl("hdnPaymentId") as HiddenField).Value;
                    hdnPayBelowThreshold = (gvr.FindControl("hdnPayBelowThreshold") as HiddenField).Value;
                    hdnAppUserCode1 = (gvr.FindControl("hdnAppUserCode1") as HiddenField).Value;
                    hdnAppUserCode2 = (gvr.FindControl("hdnAppUserCode2") as HiddenField).Value;
                    hdnAppUserCode3 = (gvr.FindControl("hdnAppUserCode3") as HiddenField).Value;
                    hdnAppUserCode4 = (gvr.FindControl("hdnAppUserCode4") as HiddenField).Value;
                    hdnAppUserCode5 = (gvr.FindControl("hdnAppUserCode5") as HiddenField).Value;
                    hdnCanUserCode = (gvr.FindControl("hdnCanUserCode") as HiddenField).Value;

                    cbApproval1 = (gvr.FindControl("cbApproval1") as CheckBox);
                    cbApproval2 = (gvr.FindControl("cbApproval2") as CheckBox);
                    cbApproval3 = (gvr.FindControl("cbApproval3") as CheckBox);
                    cbApproval4 = (gvr.FindControl("cbApproval4") as CheckBox);
                    cbApproval5 = (gvr.FindControl("cbApproval5") as CheckBox);
                    cbCancelPayment = (gvr.FindControl("cbCancelPayment") as CheckBox);

                    approval1Modified = "N";
                    approval1Checked = "N";
                    approval2Modified = "N";
                    approval2Checked = "N";
                    approval3Modified = "N";
                    approval3Checked = "N";
                    approval4Modified = "N";
                    approval4Checked = "N";
                    approval5Modified = "N";
                    approval5Checked = "N";
                    cancelModified = "N";
                    cancelChecked = "N";

                    if ((hdnAppUserCode1 == string.Empty && cbApproval1.Checked) || (hdnAppUserCode1 != string.Empty && !cbApproval1.Checked))
                    {
                        approval1Modified = "Y";
                    }

                    if ((hdnAppUserCode2 == string.Empty && cbApproval2.Checked) || (hdnAppUserCode2 != string.Empty && !cbApproval2.Checked))
                    {
                        approval2Modified = "Y";
                    }

                    if ((hdnAppUserCode3 == string.Empty && cbApproval3.Checked) || (hdnAppUserCode3 != string.Empty && !cbApproval3.Checked))
                    {
                        approval3Modified = "Y";
                    }

                    if ((hdnAppUserCode4 == string.Empty && cbApproval4.Checked) || (hdnAppUserCode4 != string.Empty && !cbApproval4.Checked))
                    {
                        approval4Modified = "Y";
                    }

                    if ((hdnAppUserCode5 == string.Empty && cbApproval5.Checked) || (hdnAppUserCode5 != string.Empty && !cbApproval5.Checked))
                    {
                        approval5Modified = "Y";
                    }

                    if ((hdnCanUserCode == string.Empty && cbCancelPayment.Checked) || (hdnCanUserCode != string.Empty && !cbCancelPayment.Checked))
                    {
                        cancelModified = "Y";
                    }

                    if (cbApproval1.Checked)
                    {
                        approval1Checked = "Y";
                    }

                    if (cbApproval2.Checked)
                    {
                        approval2Checked = "Y";
                    }

                    if (cbApproval3.Checked)
                    {
                        approval3Checked = "Y";
                    }

                    if (cbApproval4.Checked)
                    {
                        approval4Checked = "Y";
                    }

                    if (cbApproval5.Checked)
                    {
                        approval5Checked = "Y";
                    }

                    if (cbCancelPayment.Checked)
                    {
                        cancelChecked = "Y";
                    }

                    if (approval1Modified == "Y" || approval2Modified == "Y" || approval3Modified == "Y" || approval4Modified == "Y" || approval5Modified == "Y"
                        || cancelModified == "Y")
                    {
                        paymentList.Add(paymentId + "(;||;)" +
                                        approval1Modified + "(;||;)" + approval1Checked + "(;||;)" +
                                        approval2Modified + "(;||;)" + approval2Checked + "(;||;)" +
                                        approval3Modified + "(;||;)" + approval3Checked + "(;||;)" +
                                        approval4Modified + "(;||;)" + approval4Checked + "(;||;)" +
                                        approval5Modified + "(;||;)" + approval5Checked + "(;||;)" +
                                        cancelModified + "(;||;)" + cancelChecked
                                        );
                    }
                }
            }
            else
            {
                DataTable dtBulkApprovalData = (DataTable)Session["PayBulkApprovalData"];
                approval1Modified = "N";
                approval1Checked = "N";
                approval2Modified = "N";
                approval2Checked = "N";
                approval3Modified = "N";
                approval3Checked = "N";
                approval4Modified = "N";
                approval4Checked = "N";
                approval5Modified = "N";
                approval5Checked = "N";
                cancelModified = "N";
                cancelChecked = "N";
                if (hdnSelectedBulkApproval.Value != "")
                {
                    foreach (DataRow datarow in dtBulkApprovalData.Rows)
                    {
                        paymentId = datarow["payment_id"].ToString();
                        if (hdnSelectedBulkApproval.Value == "1")
                        {
                            approval1Modified = "Y";
                            approval1Checked = datarow["bulk_approval_1"].ToString(); ;
                        }
                        else if (hdnSelectedBulkApproval.Value == "2")
                        {
                            approval2Modified = "Y";
                            approval2Checked = datarow["bulk_approval_2"].ToString(); ;
                        }
                        else if (hdnSelectedBulkApproval.Value == "3")
                        {
                            approval3Modified = "Y";
                            approval3Checked = datarow["bulk_approval_3"].ToString(); ;
                        }
                        else if (hdnSelectedBulkApproval.Value == "4")
                        {
                            approval4Modified = "Y";
                            approval4Checked = datarow["bulk_approval_4"].ToString(); ;
                        }
                        else if (hdnSelectedBulkApproval.Value == "5")
                        {
                            approval5Modified = "Y";
                            approval5Checked = datarow["bulk_approval_5"].ToString(); ;
                        }

                        if (approval1Checked == "Y" || approval2Checked == "Y" || approval3Checked == "Y" || approval4Checked == "Y" || approval5Checked == "Y")
                        {
                            paymentList.Add(paymentId + "(;||;)" +
                                            approval1Modified + "(;||;)" + approval1Checked + "(;||;)" +
                                            approval2Modified + "(;||;)" + approval2Checked + "(;||;)" +
                                            approval3Modified + "(;||;)" + approval3Checked + "(;||;)" +
                                            approval4Modified + "(;||;)" + approval4Checked + "(;||;)" +
                                            approval5Modified + "(;||;)" + approval5Checked + "(;||;)" +
                                            cancelModified + "(;||;)" + cancelChecked
                                            );
                        }
                    }
                }
                if (hdnBulkCancelSelected.Value == "Y")
                {
                    foreach (DataRow datarow in dtBulkApprovalData.Rows)
                    {
                        paymentId = datarow["payment_id"].ToString();
                        cancelModified = "Y";
                        cancelChecked = datarow["bulk_cancel"].ToString();
                        if (cancelChecked == "Y")
                        {
                            paymentList.Add(paymentId + "(;||;)" +
                                            approval1Modified + "(;||;)" + approval1Checked + "(;||;)" +
                                            approval2Modified + "(;||;)" + approval2Checked + "(;||;)" +
                                            approval3Modified + "(;||;)" + approval3Checked + "(;||;)" +
                                            approval4Modified + "(;||;)" + approval4Checked + "(;||;)" +
                                            approval5Modified + "(;||;)" + approval5Checked + "(;||;)" +
                                            cancelModified + "(;||;)" + cancelChecked
                                            );
                        }
                    }
                }
            }
            return paymentList.ToArray();
        }

        // WUIN - 1018 Bulk Approval and Cancel Methods -- Start
        protected void SetBulkUpdateFlag()
        {
            DataTable approvalData = (DataTable)Session["PayApprovalData"];
            DataTable bulkApprovalData = approvalData.Clone();
            foreach (DataRow dr in approvalData.Rows)
            {
                bulkApprovalData.ImportRow(dr);
            }

            string hdnIsApprovalEnabled;
            string hdnApprovalLevel;
            string hdnAppUserCode1;
            string hdnAppUserCode2;
            string hdnAppUserCode3;
            string hdnAppUserCode4;
            string hdnAppUserCode5;
            string hdnCanUserCode;
            string hdnPayBelowThreshold;
            string hdnPaymentStatusCode;
            int belowThresholdCount = 0;
            int otherApprovalCount = 0;
            int thirdApprPendingCount = 0;
            int cancelCount = 0;
            foreach (DataRow row in bulkApprovalData.Rows)
            {

                hdnIsApprovalEnabled = row["is_approval_enabled"].ToString();
                hdnApprovalLevel = row["approval_level"].ToString();
                hdnAppUserCode1 = row["approval_user_code_1"].ToString();
                hdnAppUserCode2 = row["approval_user_code_2"].ToString();
                hdnAppUserCode3 = row["approval_user_code_3"].ToString();
                hdnAppUserCode4 = row["approval_user_code_4"].ToString();
                hdnAppUserCode5 = row["approval_user_code_5"].ToString();
                hdnCanUserCode = row["cancelled_user_code"].ToString();
                hdnPayBelowThreshold = row["payment_below_threshold"].ToString();
                hdnPaymentStatusCode = row["payment_status_code"].ToString();
                if (hdnSelectedBulkApproval.Value == "1" && hdnAppUserCode1 == string.Empty && hdnIsApprovalEnabled != "N" && Convert.ToInt16(hdnUserRoleApprovalLevel.Value) >= 1 && Convert.ToInt16(hdnApprovalLevel) >= 1)
                {
                    if (hdnPayBelowThreshold == "Y")
                    {
                        belowThresholdCount += 1;
                    }
                    row["bulk_approval_1"] = "Y";
                    hdnBulkDataModified.Value = "Y";
                    otherApprovalCount += 1;
                }
                else if (hdnSelectedBulkApproval.Value == "2" && hdnAppUserCode2 == string.Empty && hdnIsApprovalEnabled != "N" && Convert.ToInt16(hdnUserRoleApprovalLevel.Value) >= 2 && Convert.ToInt16(hdnApprovalLevel) >= 2 && hdnPayBelowThreshold == "N")
                {
                    row["bulk_approval_2"] = "Y";
                    hdnBulkDataModified.Value = "Y";
                    otherApprovalCount += 1;
                }
                else if (hdnSelectedBulkApproval.Value == "3" && hdnAppUserCode3 == string.Empty && hdnIsApprovalEnabled != "N" && Convert.ToInt16(hdnUserRoleApprovalLevel.Value) >= 3 && Convert.ToInt16(hdnApprovalLevel) >= 3 && hdnPayBelowThreshold == "N")
                {
                    row["bulk_approval_3"] = "Y";
                    hdnBulkDataModified.Value = "Y";
                    otherApprovalCount += 1;
                }
                else if (hdnSelectedBulkApproval.Value == "4" && hdnAppUserCode4 == string.Empty && hdnIsApprovalEnabled != "N" && Convert.ToInt16(hdnUserRoleApprovalLevel.Value) >= 4 && Convert.ToInt16(hdnApprovalLevel) >= 4 && hdnPayBelowThreshold == "N")
                {
                    if (hdnAppUserCode3 != string.Empty)
                    {
                        row["bulk_approval_4"] = "Y";
                        hdnBulkDataModified.Value = "Y";
                        otherApprovalCount += 1;
                    }
                    else
                    {
                        thirdApprPendingCount += 1;
                    }
                }
                else if (hdnSelectedBulkApproval.Value == "5" && hdnAppUserCode5 == string.Empty && hdnIsApprovalEnabled != "N" && Convert.ToInt16(hdnUserRoleApprovalLevel.Value) >= 5 && Convert.ToInt16(hdnApprovalLevel) >= 5 && hdnPayBelowThreshold == "N")
                {
                    if (hdnAppUserCode3 != string.Empty)
                    {
                        row["bulk_approval_5"] = "Y";
                        hdnBulkDataModified.Value = "Y";
                        otherApprovalCount += 1;
                    }
                    else
                    {
                        thirdApprPendingCount += 1;
                    }
                }
                else if (hdnBulkCancelSelected.Value == "Y" && hdnCanUserCode == string.Empty)
                {
                    if (!(hdnPaymentRoleId.Value == string.Empty || Convert.ToInt32(hdnPaymentRoleId.Value) <= 10
                       || !(Convert.ToInt16(hdnPaymentStatusCode) <= 7)))
                    {
                        row["bulk_cancel"] = "Y";
                        hdnBulkDataModified.Value = "Y";
                        cancelCount += 1;
                    }
                }
            }
            Session["PayBulkApprovalData"] = bulkApprovalData;
            if (hdnSelectedBulkApproval.Value != "")
            {
                if (belowThresholdCount > 0)
                {
                    lblMsgBulkConfirmPopup.Text = "Only eligible payments will be approved to the selected approval level, some payments are below the threshold these will not be approved. Do you want to continue?";
                }
                else if (thirdApprPendingCount > 1)
                {
                    lblMsgBulkConfirmPopup.Text = "Only eligible payments will be approved to the selected approval level, some payments still require a 3rd Approval these will not be approved. Do you want to continue?";
                }
                else if (otherApprovalCount > 0)
                {
                    lblMsgBulkConfirmPopup.Text = "Only eligible payments will be approved to the selected approval level. Do you want to continue?";
                }
                else
                {
                    ClearBulkApproveCancel();
                    msgView.SetMessage("There are no eligible payments to be approved.", MessageType.Warning, PositionType.Auto);
                    return;
                }

            }
            if (hdnBulkCancelSelected.Value == "Y")
            {
                if (cancelCount > 0)
                {
                    lblMsgBulkConfirmPopup.Text = "Only Payments which are below Payment Sent will be cancelled. Do you want to continue?";

                }
                else
                {
                    msgView.SetMessage("There are no eligible payments to be cancelled.", MessageType.Warning, PositionType.Auto);
                    ClearBulkApproveCancel();
                    return;
                }

            }

            mpeBulkConfirmPopup.Show();

        }


        public void ClearBulkApproveCancel()
        {
            if (hdnSelectedBulkApproval.Value != "")
            {
                var mainContent = Master.FindControl("ContentPlaceHolderBody");
                CheckBox cbHdrApproval = (CheckBox)mainContent.FindControl("cbHdrApproval" + hdnSelectedBulkApproval.Value);
                cbHdrApproval.Checked = false;
            }
            cbHdrCancel.Checked = false;
            hdnSelectedBulkApproval.Value = "";
            hdnBulkCancelSelected.Value = "N";
            hdnBulkDataModified.Value = "N";
            Session["PayBulkApprovalData"] = null;
        }

        // WUIN - 1018 Bulk Approval and Cancel Methods -- End


        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSave.Enabled = false;
                btnUpdateInvoice.Enabled = false;
                btnGeneratePaymentDetails.Enabled = false;

            }

        }

        #endregion Methods

        #region Fuzzy Search

        protected void btnFuzzySearchOwner_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchOwner();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payment approval Owner fuzzy search", ex.Message);
            }
        }

        protected void btnFuzzySearchPayee_Click1(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchPayee();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payment approval Payee fuzzy search", ex.Message);
            }
        }

        protected void btnFuzzySearchRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltor();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payment approval Royaltor fuzzy search", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Royaltor")
                {
                    txtRoyaltorFuzzySearch.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "Owner")
                {
                    txtOwnFuzzySearch.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "Payee")
                {
                    txtPayeeFuzzySearch.Text = string.Empty;
                }

                mpeFuzzySearch.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing complete search list pop up", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Royaltor")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtRoyaltorFuzzySearch.Text = string.Empty;
                        return;
                    }
                    txtRoyaltorFuzzySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                }
                else if (hdnFuzzySearchField.Value == "Owner")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtOwnFuzzySearch.Text = string.Empty;
                        return;
                    }
                    txtOwnFuzzySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                }
                else if (hdnFuzzySearchField.Value == "Payee")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtPayeeFuzzySearch.Text = string.Empty;
                        return;
                    }
                    txtPayeeFuzzySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "GoButton")
            {
                btnGo_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "ClearButton")
            {
                btnClear_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "PaymentReady")
            {
                btnUpdateInvoice_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "OnPageChange")
            {
                lnkPage_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "btnGeneratePaymentDetails")
            {
                btnGeneratePayment_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "BulkUpdate")
            {
                //WUIN-1018 - Clearing header check boxes and binding Acutal approval data
                ClearBulkApproveCancel();
                DataTable dtApproval = (DataTable)Session["PayApprovalData"];
                Utilities.PopulateGridPage(1, dtApproval, gridDefaultPageSize, gvPaymentApproval, dtEmpty, rptPager);

            }
            hdnIsConfirmPopup.Value = "N";
        }

        private void FuzzySearchRoyaltor()
        {
            if (txtRoyaltorFuzzySearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                return;
            }
            hdnFuzzySearchField.Value = "Royaltor";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyListWithOwnerCode(txtRoyaltorFuzzySearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchOwner()
        {
            if (txtOwnFuzzySearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in owner search field", MessageType.Warning, PositionType.Auto);
                return;
            }
            hdnFuzzySearchField.Value = "Owner";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllOwnerList(txtOwnFuzzySearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchPayee()
        {
            if (txtPayeeFuzzySearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in payee search field", MessageType.Warning, PositionType.Auto);
                return;
            }
            hdnFuzzySearchField.Value = "Payee";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllPayeeList(txtPayeeFuzzySearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }
        #endregion Fuzzy Search

    }
}