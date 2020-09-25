/*
File Name   :   RoyContractGrouping.cs
Purpose     :   Used for maintaining Royaltor Grouping details (WUIN-151)

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     28-Jul-2017     Pratik(Infosys Limited)   Initial Creation
        08-Dec-2017     Harish                    WUIN-387 changes
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using System.Web.UI.HtmlControls;
using WARS.BusinessLayer;
using System.Net;


namespace WARS.Contract
{
    public partial class RoyContractGrouping : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractRoyGroupingBL royContractRoyGroupingBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string loggedUserID;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];

                //royaltorId = "15";
                //isNewRoyaltor = "N";

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    btnSave.Enabled = false;
                    btnAudit.Enabled = false;
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Royaltor Contract Grouping";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Royaltor Contract Grouping";
                }

                //lblTab.Focus();//tabbing sequence starts here 
                txtRoyaltorId.Focus();
                //PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        
                        //HtmlTableRow trContractGroupings = (HtmlTableRow)contractNavigationButtons.FindControl("trContractGroupings");
                        //trContractGroupings.Visible = false;
                        Button btnContractGrps = (Button)contractNavigationButtons.FindControl("btnContractGrps");
                        btnContractGrps.Enabled = false; 
                        HiddenField hdnRoyaltorId = (HiddenField)contractNavigationButtons.FindControl("hdnRoyaltorId");
                        hdnRoyaltorId.Value = royaltorId;
                        HiddenField hdnRoyaltorIdHdr = (HiddenField)contractHdrNavigation.FindControl("hdnRoyaltorId");
                        hdnRoyaltorIdHdr.Value = royaltorId;

                        //WUIN-599 - resetting the flags
                        ((HiddenField)Master.FindControl("hdnIsContractScreen")).Value = "N";
                        ((HiddenField)Master.FindControl("hdnIsNotContractScreen")).Value = "N";
                        this.Master.FindControl("lnkBtnHome").Visible = false;
                       

                        if (isNewRoyaltor == "Y")
                        {
                            btnSave.Text = "Save & Continue";
                            btnAudit.Text = "Back";
                            hdnIsNewRoyaltor.Value = "Y";

                            //WUIN-387 change - enable contract buttons on this screen as this is the last screen of contract creation
                            //contractNavigationButtons.Disable();
                            //WUIN-367- as this is not the last screen, disabling the contract buttons
                            //contractNavigationButtons.Disable();

                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.ContractGroupings.ToString());
                        }

                        LoadData(royaltorId);

                        //WUIN-450 - If a new Royaltor is set up with Lock checked it should continue to allow entry of the details (the Royaltor set up needs to be completed!)
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId))
                        {
                            btnSave.ToolTip = "Royaltor Locked";
                        }

                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId))
                        {
                            hdnOtherUserScreenLocked.Value = "Y";
                        }

                        //WUIN-1096 - Only Read access for ReadonlyUser
                        //WUIN-599 If a contract is already using by another user then making the screen readonly.
                        //WUIN-450 -Only Read access for locked contracts
                        if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId)) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId)))
                        {
                            EnableReadonly();
                        }

                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                }

            }
            catch (ThreadAbortException ex1)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidRoyaltorGFS.Value == "N")
                {
                    msgView.SetMessage("Royaltor grouping deatils not saved – nota valid royaltor for 'GFS Grouping Royaltor'!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (hdnIsValidRoyaltorSummary.Value == "N")
                {
                    msgView.SetMessage("Royaltor grouping deatils not saved – nota valid royaltor for 'Summary Statements'!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (hdnIsValidRoyaltorTXT.Value == "N")
                {
                    msgView.SetMessage("Royaltor grouping deatils not saved – nota valid royaltor for 'TXT Detail Statements'!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (txtDSPAnalytics.Text != string.Empty && hdnIsValidRoyaltorDSP.Value == "N")
                {
                    msgView.SetMessage("Royaltor grouping deatils not saved – nota valid royaltor for 'DSP Analytics'!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                string gfsComapny;
                if (ddlGFSCompany.SelectedIndex > 0)
                {
                    gfsComapny = ddlGFSCompany.SelectedValue;
                }
                else
                {
                    gfsComapny = string.Empty;
                }

                if (hdnDataChanged.Value != "Y")
                {
                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.ContractGroupings.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }

                    return;
                }

                royContractRoyGroupingBL = new RoyContractRoyGroupingBL();
                DataSet updatedData = royContractRoyGroupingBL.UpdateRoyaltorGrouping(Convert.ToInt32(royaltorId), txtSummaryStatements.Text.Split('-')[0], txtTXTDetailStatements.Text.Split('-')[0], txtGFSGroupingRoyaltor.Text.Split('-')[0], txtDSPAnalytics.Text.Split('-')[0], txtGFSLabel.Text, gfsComapny, txtPrintGrp.Text, loggedUserID, out errorId);
                royContractRoyGroupingBL = null;

                if (updatedData.Tables.Count == 0 || errorId == 2)
                {
                    ExceptionHandler("Error in saving data", string.Empty);
                }
                else
                {
                    PopulateRoyGroupingDetails(updatedData.Tables[0]);
                    hdnDataChanged.Value = "N";

                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.ContractGroupings.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Royaltor grouping details saved successfully", MessageType.Warning, PositionType.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving data.", ex.Message);
            }

        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                hdnIsAuditScreen.Value = "Y";
                //redirect in javascript so that issue of data not saved validation would be handled
                if (isNewRoyaltor == "Y")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "PreviousScreen", "RedirectToPreviousScreen(" + royaltorId + ");", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "AuditScreen", "RedirectToAuditScreen(" + royaltorId + ");", true);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        protected void fuzzyGFSGroupingRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnFuzzySearchField.Value = "GFSGroupingRoyaltor";
                if (txtGFSGroupingRoyaltor.Text == string.Empty)
                {
                    msgView.SetMessage("Please enter a text in GFS Grouping Royaltor search field", MessageType.Warning, PositionType.Auto);
                    return;
                }

                FuzzySearchRoyaltor(txtGFSGroupingRoyaltor.Text);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void fuzzySummaryGroupingRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnFuzzySearchField.Value = "SummaryStatements";
                if (txtSummaryStatements.Text == string.Empty)
                {
                    msgView.SetMessage("Please enter a text in Summary Statements search field", MessageType.Warning, PositionType.Auto);
                    return;
                }

                FuzzySearchRoyaltor(txtSummaryStatements.Text);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void fuzzyTXTGroupingRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnFuzzySearchField.Value = "TXTDetailStatements";
                if (txtTXTDetailStatements.Text == string.Empty)
                {
                    msgView.SetMessage("Please enter a text in TXT Detail Statements search field", MessageType.Warning, PositionType.Auto);
                    return;
                }

                FuzzySearchRoyaltor(txtTXTDetailStatements.Text);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void fuzzyDSPGroupingRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnFuzzySearchField.Value = "DSPAnalytics";
                if (txtDSPAnalytics.Text == string.Empty)
                {
                    msgView.SetMessage("Please enter a text in DSP Analytics search field", MessageType.Warning, PositionType.Auto);
                    return;
                }

                FuzzySearchRoyaltor(txtDSPAnalytics.Text);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "GFSGroupingRoyaltor")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtGFSGroupingRoyaltor.Text = string.Empty;
                        return;
                    }

                    txtGFSGroupingRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                }
                else if (hdnFuzzySearchField.Value == "SummaryStatements")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtSummaryStatements.Text = string.Empty;
                        return;
                    }

                    txtSummaryStatements.Text = lbFuzzySearch.SelectedValue.ToString();
                }
                else if (hdnFuzzySearchField.Value == "TXTDetailStatements")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtTXTDetailStatements.Text = string.Empty;
                        return;
                    }

                    txtTXTDetailStatements.Text = lbFuzzySearch.SelectedValue.ToString();
                }
                else if (hdnFuzzySearchField.Value == "DSPAnalytics")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtDSPAnalytics.Text = string.Empty;
                        return;
                    }

                    txtDSPAnalytics.Text = lbFuzzySearch.SelectedValue.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting item from list", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "GFSGroupingRoyaltor")
                {
                    txtGFSGroupingRoyaltor.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "SummaryStatements")
                {
                    txtSummaryStatements.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "TXTDetailStatements")
                {
                    txtTXTDetailStatements.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "DSPAnalytics")
                {
                    txtDSPAnalytics.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing full search popup", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData(string royaltorId)
        {
            royContractRoyGroupingBL = new RoyContractRoyGroupingBL();
            DataSet initialData = royContractRoyGroupingBL.GetInitialData(Convert.ToInt32(royaltorId), out errorId);
            royContractRoyGroupingBL = null;
                        
            if (errorId == 2)
            {
                ExceptionHandler("Error in loading data", string.Empty);
                return;
            }

            if (initialData.Tables.Count == 0)
            {
                ExceptionHandler("Error in loading data", string.Empty);

            }
            else
            {
                //Popupate dropdown
                ddlGFSCompany.DataSource = initialData.Tables[1];
                ddlGFSCompany.DataTextField = "dropdown_text";
                ddlGFSCompany.DataValueField = "dropdown_value";
                ddlGFSCompany.DataBind();
                ddlGFSCompany.Items.Insert(0, new ListItem("-"));

                //Populate textfields
                PopulateRoyGroupingDetails(initialData.Tables[0]);

            }

        }

        private void PopulateRoyGroupingDetails(DataTable royGroupingDetails)
        {
            try
            {
                if (royGroupingDetails.Rows.Count != 0)
                {
                    txtRoyaltorId.Text = royGroupingDetails.Rows[0]["royaltor"].ToString();
                    txtSummaryStatements.Text = royGroupingDetails.Rows[0]["summary_master_royaltor"].ToString();
                    txtTXTDetailStatements.Text = royGroupingDetails.Rows[0]["txt_master_royaltor"].ToString();
                    txtGFSGroupingRoyaltor.Text = royGroupingDetails.Rows[0]["accrual_royaltor"].ToString();
                    txtDSPAnalytics.Text = royGroupingDetails.Rows[0]["dsp_analytics_royaltor"].ToString();
                    txtGFSLabel.Text = royGroupingDetails.Rows[0]["gfs_label"].ToString();
                    txtPrintGrp.Text = royGroupingDetails.Rows[0]["print_stream"].ToString();

                    hdnSummaryStatements.Value = royGroupingDetails.Rows[0]["summary_master_royaltor"].ToString();
                    hdnTXTDetailStatements.Value = royGroupingDetails.Rows[0]["txt_master_royaltor"].ToString();
                    hdnGFSGroupingRoyaltor.Value = royGroupingDetails.Rows[0]["accrual_royaltor"].ToString();
                    hdnDSPAnalytics.Value = royGroupingDetails.Rows[0]["dsp_analytics_royaltor"].ToString();
                    hdnGFSLabel.Value = royGroupingDetails.Rows[0]["gfs_label"].ToString();
                    hdnPrintGrp.Value = royGroupingDetails.Rows[0]["print_stream"].ToString();

                    if (ddlGFSCompany.Items.FindByValue(royGroupingDetails.Rows[0]["gfs_company"].ToString()) != null)
                    {
                        ddlGFSCompany.Items.FindByValue(royGroupingDetails.Rows[0]["gfs_company"].ToString()).Selected = true;
                        hdnGFSCompany.Value = royGroupingDetails.Rows[0]["gfs_company"].ToString();
                    }
                    else
                    {
                        ddlGFSCompany.SelectedIndex = 0;
                        hdnGFSCompany.Value = "-";
                    }


                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading Catalogue details", ex.Message);
            }
        }

        private void FuzzySearchRoyaltor(string searchText)
        {
            if (searchText == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(searchText.Trim().ToUpper(), int.MaxValue);
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

        private void EnableReadonly()
        {

            btnSave.Enabled = false;
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