/*
File Name   :   RoyContractEscHistory.cs
Purpose     :   to maintain escalation history of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     07-Aug-2017     Harish(Infosys Limited)   Initial Creation
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
    public partial class RoyContractEscHistory : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractEscHistoryBL royContractEscHistoryBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        bool enableGridEdit = true;
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
                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];
                //royaltorId = "15565";

                if (royaltorId == string.Empty)
                {
                    royaltorId = null;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Escalation History";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Escalation History";
                }

                lblTab.Focus();//tabbing sequence starts here                
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        HiddenField hdnBtnContractMaint = (HiddenField)contractHdrNavigation.FindControl("hdnBtnContractMaint");

                        //WUIN-599 - resetting the flags
                        ((HiddenField)Master.FindControl("hdnIsContractScreen")).Value = "N";
                        ((HiddenField)Master.FindControl("hdnIsNotContractScreen")).Value = "N";
                        this.Master.FindControl("lnkBtnHome").Visible = false;

                        if (royaltorId == null || royaltorId == string.Empty)
                        {
                            //displayed from main menu(not contract menu)                            
                            hdnBtnContractMaint.Value = "Y";
                            trConNavBtns.Visible = false;
                            hdnIsRoyaltorNull.Value = "Y";
                            LoadInitialData(royaltorId);
                        }
                        else
                        {
                            //displayed from contract main menu
                            Button btnEscHistory = (Button)contractNavigationButtons.FindControl("btnEscHistory");
                            btnEscHistory.Enabled = false;
                            HiddenField hdnRoyaltorId = (HiddenField)contractNavigationButtons.FindControl("hdnRoyaltorId");
                            hdnRoyaltorId.Value = royaltorId;
                            HiddenField hdnRoyaltorIdHdr = (HiddenField)contractHdrNavigation.FindControl("hdnRoyaltorId");
                            hdnRoyaltorIdHdr.Value = royaltorId;

                            hdnBtnContractMaint.Value = "N";
                            LoadInitialData(royaltorId);

                            if (isNewRoyaltor == "Y")
                            {
                                contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.EscHistory.ToString());
                            }

                            //WUIN-599 -- Only one user can use contract screens at the same time.
                            // If a contract is already using by another user then making the screen readonly.
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

                        if (royaltorId == null)
                        {
                            txtRoyaltor.ReadOnly = false;
                            txtRoyaltor.Font.Bold = false;
                            txtRoyaltor.CssClass = "textboxStyle";
                            fuzzySearchRoyaltor.Visible = true;
                        }
                        else
                        {
                            fuzzySearchRoyaltor.Visible = false;
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

        protected void btnHistorySummary_Click(object sender, EventArgs e)
        {
            try
            {
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                DataSet escHistoryData;
                royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);

                if (btnHistorySummary.Text == "History Summary")
                {
                    btnHistorySummary.Text = "History Details";
                    enableGridEdit = false;
                    royContractEscHistoryBL = new RoyContractEscHistoryBL();
                    escHistoryData = royContractEscHistoryBL.GetEscHistorySummary(royaltorId, ddlEscCode.SelectedValue, out errorId);
                    royContractEscHistoryBL = null;
                }
                else
                {
                    btnHistorySummary.Text = "History Summary";

                    royContractEscHistoryBL = new RoyContractEscHistoryBL();
                    escHistoryData = royContractEscHistoryBL.GetEscHistory(royaltorId, ddlEscCode.SelectedValue, out errorId);
                    royContractEscHistoryBL = null;
                }


                if (errorId == 2)
                {
                    ExceptionHandler("Error in loading grid data", string.Empty);
                    return;
                }


                if (escHistoryData.Tables.Count != 0)
                {
                    if (escHistoryData.Tables[0].Rows.Count == 0)
                    {
                        gvEscHistory.EmptyDataText = "No data found for the selected royaltor and escalation code";
                    }


                    gvEscHistory.DataSource = escHistoryData.Tables[0];
                    gvEscHistory.DataBind();

                    PopulateTotalSales(escHistoryData.Tables[0]);

                }
                else if (escHistoryData.Tables.Count == 0)
                {
                    dtEmpty = new DataTable();
                    gvEscHistory.DataSource = dtEmpty;
                    gvEscHistory.EmptyDataText = "No data found for the selected royaltor and escalation code";
                    gvEscHistory.DataBind();
                }
                UserAuthorization();


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void gvEscHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    //validation : do not allow edit when 'History Summary' button is clicked
                    if (enableGridEdit == false)
                    {
                        (e.Row.FindControl("txtSales") as TextBox).ReadOnly = true;
                        (e.Row.FindControl("txtAdjSales") as TextBox).ReadOnly = true;
                        (e.Row.FindControl("imgBtnSave") as ImageButton).Visible = false;
                        (e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = false;
                        (e.Row.FindControl("imgBtnUndo") as ImageButton).Visible = false;
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

        protected void gvEscHistory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                
                if (e.CommandName == "SaveRow")
                {
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

                    SaveEscHistoryDetails(row);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving/deleting grid data", ex.Message);
            }
        }

        protected void ddlEscCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet escHistoryData;
                royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);

                if (btnHistorySummary.Text == "History Summary")
                {
                    royContractEscHistoryBL = new RoyContractEscHistoryBL();
                    escHistoryData = royContractEscHistoryBL.GetEscHistory(royaltorId, ddlEscCode.SelectedValue, out errorId);
                    royContractEscHistoryBL = null;
                }
                else
                {
                    enableGridEdit = false;
                    royContractEscHistoryBL = new RoyContractEscHistoryBL();
                    escHistoryData = royContractEscHistoryBL.GetEscHistorySummary(royaltorId, ddlEscCode.SelectedValue, out errorId);
                    royContractEscHistoryBL = null;
                }

                if (errorId == 2)
                {
                    ExceptionHandler("Error in loading grid data", string.Empty);
                    return;
                }


                if (escHistoryData.Tables.Count != 0)
                {
                    if (escHistoryData.Tables[0].Rows.Count == 0)
                    {
                        gvEscHistory.EmptyDataText = "No data found for the selected royaltor and escalation code";
                    }

                    gvEscHistory.DataSource = escHistoryData.Tables[0];
                    gvEscHistory.DataBind();

                    PopulateTotalSales(escHistoryData.Tables[0]);

                }
                else if (escHistoryData.Tables.Count == 0)
                {
                    dtEmpty = new DataTable();
                    gvEscHistory.DataSource = dtEmpty;
                    gvEscHistory.EmptyDataText = "No data found for the selected royaltor and escalation code";
                    gvEscHistory.DataBind();
                }
                UserAuthorization();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                //Validate
                Page.Validate("valGrpSave");
                if (!Page.IsValid)
                {
                    mpeSaveUndo.Hide();
                    msgView.SetMessage("Invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                SaveEscHistoryDetails(gvEscHistory.Rows[rowIndex]);
                hdnGridRowSelectedPrvious.Value = string.Empty;

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving grid data", ex.Message);
            }
        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "HistorySummary")
            {
                btnHistorySummary_Click(sender, e);
            }
            if (hdnButtonSelection.Value == "EscalationCode")
            {
                ddlEscCode_SelectedIndexChanged(sender, e);
            }
        }

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvEscHistory_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["EscHistoryData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvEscHistory.DataSource = dataView;
                gvEscHistory.DataBind();
                Session["EscHistoryData"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        //JIRA-908 Changes by Ravi on 12/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string royaltorId = hdnDeleteRoyaltorId.Value;
                string escCode = hdnDeleteEscCode.Value;
                string sellerGrpCode = hdnDeleteSellerGrpCode.Value;
                string priceGrpCode = hdnDeletePriceGrpCode.Value;
                string configGrpCode = hdnDeleteConfigGrpCode.Value;
                royContractEscHistoryBL = new RoyContractEscHistoryBL();
                DataSet escHistory = royContractEscHistoryBL.DeleteEscHistory(royaltorId, escCode, sellerGrpCode, priceGrpCode, configGrpCode, out errorId);
                royContractEscHistoryBL = null;
                LoadGridData(escHistory, errorId);
                hdnDeleteRoyaltorId.Value = "";
                hdnDeleteEscCode.Value = "";
                hdnDeleteSellerGrpCode.Value = "";
                hdnDeletePriceGrpCode.Value = "";
                hdnDeleteConfigGrpCode.Value = "";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting grid data", ex.Message);
            }
        }
        //JIRA-908 Changes by Ravi on 12/02/2019 -- End

        #endregion Events

        #region Methods

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                EnableReadonly();
            }
        }

        private void LoadInitialData(string royaltorId)
        {

            royContractEscHistoryBL = new RoyContractEscHistoryBL();
            DataSet initialData = royContractEscHistoryBL.GetInitialData((royaltorId == null ? Global.DBNullParamValue : royaltorId), out errorId);
            royContractEscHistoryBL = null;

            if (errorId != 2)
            {

                if (royaltorId != null)
                {
                    //populate royaltor for the royaltor id
                    if (Session["FuzzySearchAllRoyListWithOwnerCode"] == null)
                    {
                        DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllRoyaltorListWithOwnerCode", out errorId);
                        Session["FuzzySearchAllRoyListWithOwnerCode"] = dsList.Tables[0];
                    }

                    DataRow[] filteredRow = ((DataTable)Session["FuzzySearchAllRoyListWithOwnerCode"]).Select("royaltor_id = '" + royaltorId + "'");
                    txtRoyaltor.Text = filteredRow[0]["royaltor"].ToString();

                    ddlEscCode.DataSource = initialData.Tables["EscCodeList"];
                    ddlEscCode.DataTextField = "item_text";
                    ddlEscCode.DataValueField = "item_value";
                    ddlEscCode.DataBind();
                    if (initialData.Tables["EscCodeList"].Rows.Count == 0)
                    {
                        ddlEscCode.Items.Insert(0, new ListItem("-"));
                    }

                    if (initialData.Tables["EscCodeList"].Rows.Count > 0)
                    {
                        string minEscCode = Convert.ToString(initialData.Tables["EscCodeList"].Compute("min([item_value])", string.Empty));
                        ddlEscCode.SelectedValue = minEscCode.ToString();
                    }


                    if (initialData.Tables["EscHistory"].Rows.Count == 0)
                    {
                        gvEscHistory.EmptyDataText = "No data found for the selected royaltor and escalation code";
                    }
                    Session["EscHistoryData"] = initialData.Tables["EscHistory"];
                    gvEscHistory.DataSource = initialData.Tables["EscHistory"];
                    gvEscHistory.DataBind();

                    PopulateTotalSales(initialData.Tables["EscHistory"]);

                }
                else
                {
                    txtRoyaltor.Text = string.Empty;
                    LoadEmptyGrid();
                }
                UserAuthorization();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }



        }

        private void LoadEmptyGrid()
        {
            dtEmpty = new DataTable();
            gvEscHistory.DataSource = dtEmpty;
            gvEscHistory.EmptyDataText = "No data found for the selected royaltor and escalation code";
            gvEscHistory.DataBind();

            PopulateTotalSales(null);
        }

        private void LoadGridData(DataSet escHistory, int errorId)
        {
            if (errorId != 2 && escHistory.Tables.Count != 0)
            {

                if (escHistory.Tables["EscHistory"].Rows.Count == 0)
                {
                    gvEscHistory.EmptyDataText = "No data found for the selected royaltor and escalation code";
                }

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                gvEscHistory.DataSource = escHistory.Tables["EscHistory"];
                gvEscHistory.DataBind();

                PopulateTotalSales(escHistory.Tables["EscHistory"]);
                UserAuthorization();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }
        }

        private void SaveEscHistoryDetails(GridViewRow gvr)
        {
            if (!Page.IsValid)
            {
                msgView.SetMessage("Invalid or missing data!", MessageType.Warning, PositionType.Auto);
                return;
            }

            string royaltorId = (gvr.FindControl("hdnRoyaltorId") as HiddenField).Value;
            string escCode = (gvr.FindControl("hdnEscCode") as HiddenField).Value;
            string sellerGrpCode = (gvr.FindControl("hdnSellerGrpCode") as HiddenField).Value;
            string configGrpCode = (gvr.FindControl("hdnConfigGrpCode") as HiddenField).Value;
            string priceGrpCode = (gvr.FindControl("hdnPriceGrpCode") as HiddenField).Value;
            string sales = (gvr.FindControl("txtSales") as TextBox).Text;
            string adjSales = (gvr.FindControl("txtAdjSales") as TextBox).Text;

            royContractEscHistoryBL = new RoyContractEscHistoryBL();
            DataSet escHistory = royContractEscHistoryBL.SaveEscHistory(royaltorId, escCode, sellerGrpCode, priceGrpCode, configGrpCode, sales, adjSales, out errorId);
            royContractEscHistoryBL = null;

            LoadGridData(escHistory, errorId);
        }

        private void PopulateTotalSales(DataTable escHistoryData)
        {
            lblTotalSales.Text = "0";
            lblTotalAdjSales.Text = "0";

            if (escHistoryData != null && escHistoryData.Rows.Count > 0)
            {
                object objTotal;
                objTotal = escHistoryData.Compute("Sum(units)", "");
                lblTotalSales.Text = objTotal.ToString();
                objTotal = escHistoryData.Compute("Sum(adjusted_units)", "");
                lblTotalAdjSales.Text = objTotal.ToString();
            }

        }

        private void EnableReadonly()
        {
            btnSaveChanges.Enabled = false;
            //disable grid buttons
            foreach (GridViewRow gvr in gvEscHistory.Rows)
            {
                (gvr.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
            }
        }

        #endregion Methods

        #region Royaltor fuzzy search

        protected void fuzzySearchRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltor();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        private void FuzzySearchRoyaltor()
        {
            List<string> lstRoyaltors = new List<string>();
            if (txtRoyaltor.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                LoadEmptyGrid();
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyListWithOwnerCode(txtRoyaltor.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtRoyaltor.Text = string.Empty;
                    LoadEmptyGrid();
                    return;
                }

                txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                LoadInitialData(royaltorId);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void btnRoyaltorSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnRoySearchSelected.Value == "Y")
                {
                    royaltorId = txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1);
                    LoadInitialData(royaltorId);
                }
                else
                {

                }


                hdnRoySearchSelected.Value = string.Empty;


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from textbox", ex.Message);
            }
        }

        #endregion Royaltor auto complete textbox

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