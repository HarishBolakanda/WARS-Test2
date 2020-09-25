/*
File Name   :   RoyContractEscRates.cs
Purpose     :   to maintain escalation rates of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     15-May-2017     Harish(Infosys Limited)   Initial Creation
 *      13-Dec-2017     Harish                    WUIN-384 changes
 *      31-May-2018     Harish                    WUIN-378 changes
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
    public partial class RoyContractEscRates : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractEscRatesBL royContractEscRatesBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string loggedUserID;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        string wildCardChar = ".";
        string sortOrder = "esc_code,seller_group_code_order, seller_group_code,config_group_code,price_group_code,sales_trigger,value_trigger";
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

                //royaltorId = "12340";
                //isNewRoyaltor = "N";

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Escalation Rates";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Escalation Rates";
                }

                //lblTab.Focus();//tabbing sequence starts here                                             
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //HtmlTableRow trEscalationRates = (HtmlTableRow)contractNavigationButtons.FindControl("trEscalationRates");
                        //trEscalationRates.Visible = false;
                        Button btnEscalationRates = (Button)contractNavigationButtons.FindControl("btnEscalationRates");
                        btnEscalationRates.Enabled = false;
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
                            //contractNavigationButtons.Disable();
                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.EscalationRates.ToString());
                        }

                        txtRoyaltorId.Text = royaltorId;
                        LoadEscRatesData();

                        //WUIN-450 - If a new Royaltor is set up with Lock checked it should continue to allow entry of the details (the Royaltor set up needs to be completed!)
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId))
                        {
                            btnSave.ToolTip = "Royaltor Locked";
                        }

                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        // If a contract is already using by another user then making the screen readonly.
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId))
                        {
                            hdnOtherUserScreenLocked.Value = "Y";
                        }

                        //WUIN-1096 -  ReadOnlyUser
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

        protected void gvEscRates_RowDataBound(object sender, GridViewRowEventArgs e)
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

                if (e.Row.RowType == DataControlRowType.DataRow)
                {


                    /*
                    if (salesPct == "0")
                    {
                        (e.Row.FindControl("txtPctSales") as TextBox).Text = string.Empty;
                    }

                    if (royaltyRate == "0")
                    {
                        (e.Row.FindControl("txtRoyaltyRate") as TextBox).Text = string.Empty;
                    }

                    if (unitRate == "0")
                    {
                        (e.Row.FindControl("txtUnitRate") as TextBox).Text = string.Empty;
                    }

                    if (revenueRate == "0")
                    {
                        (e.Row.FindControl("txtRevenueRate") as TextBox).Text = string.Empty;
                    }
                     * */

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Escalation rates not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //WUIN-557 validation - check unique row
                if (ValidateDuplicateRateRow() == true)
                {
                    msgView.SetMessage("Rate already exists", MessageType.Warning, PositionType.Auto);
                    return;
                
                }

                List<Array> escRatesList = EscRateChangeList();
                List<string> deleteProfileList = new List<string>();
                List<string> deleteRateList = new List<string>();


                if (ViewState["vsDeleteProfileIds"] != null)
                {
                    deleteProfileList = (List<string>)ViewState["vsDeleteProfileIds"];
                }

                if (ViewState["vsDeleteRateIds"] != null)
                {
                    deleteRateList = (List<string>)ViewState["vsDeleteRateIds"];
                }

                //check if any changes to save
                if (escRatesList[0].Length == 0 && escRatesList[1].Length == 0 && deleteProfileList.Count == 0 && deleteRateList.Count == 0)
                {
                    if (isNewRoyaltor == "N")
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }
                    else if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.EscalationRates.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }

                    return;
                }

                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string royaltor;
                royContractEscRatesBL = new RoyContractEscRatesBL();
                DataSet escRatesData = royContractEscRatesBL.SaveEscRates(royaltorId, loggedUserID, escRatesList[0], escRatesList[1], deleteProfileList.ToArray(), out royaltor, out errorId);
                royContractEscRatesBL = null;
                hdnGridDataDeleted.Value = "N";
                ViewState["vsDeleteProfileIds"] = null;
                ViewState["vsDeleteRateIds"] = null;

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (errorId == 0 || errorId == 1)
                {
                    txtRoyaltorId.Text = royaltor;
                    if (escRatesData.Tables.Count != 0)
                    {

                        Session["RoyContEscRatesGridDataInitial"] = escRatesData.Tables[0];
                        Session["RoyContEscRatesGridData"] = escRatesData.Tables[0];

                        //hold initial data in a session for processing profiles and rates
                        DataTable dtInitialData = escRatesData.Tables[0].Clone();
                        foreach (DataRow drInitial in escRatesData.Tables[0].Rows)
                        {
                            dtInitialData.ImportRow(drInitial);
                        }
                        Session["RoyContEscRatesInitUnChanged"] = dtInitialData;

                        gvEscRates.DataSource = escRatesData.Tables[0];
                        gvEscRates.DataBind();

                        if (escRatesData.Tables[0].Rows.Count == 0)
                        {
                            gvEscRates.EmptyDataText = "No data found for the selected royaltor";
                        }
                        else
                        {
                            gvEscRates.EmptyDataText = string.Empty;
                        }

                    }
                    else if (escRatesData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvEscRates.DataSource = dtEmpty;
                        gvEscRates.EmptyDataText = "No data found for the selected royaltor and option period";
                        gvEscRates.DataBind();
                    }

                    if (errorId == 1)
                    {
                        //WUIN-378
                        //display confirmation popup to add default prorata rows for the escalation codes not present 
                        mpeProrataConfirm.Show();
                        return;

                        //**functionality to display save confirmation and redirecting to next page for new royaltor should be handled in confirmation popup as well
                    }

                    /*WUIN-378
                     * moved this logic to a method so that it can be re used in prorata confirmation popup
                    //new royaltor - redirect to Packaging rates screen
                    //existing royaltor - remain in same screen
                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.EscalationRates.ToString());

                        //redirect in javascript so that issue of data not saved validation would be handled
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId.Split('-')[0].Trim() + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Escalation rates saved", MessageType.Warning, PositionType.Auto);
                    }
                     * */

                    NotifyPostSavingChanges();

                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving escalation rates data", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving escalation rates data", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving escalation rates data", ex.Message);
            }
        }

        protected void btnAudit_Click(object sender, EventArgs e)
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

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "SalesProRata")
            {
                btnProRata_Click(sender, e);
            }            
        }

        protected void btnAppendAddRow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (IsRowExisting(-1,txtEscCodeAddRow.Text,txtTerritoryAddRow.Text,txtConfigAddRow.Text,txtSalesTypeAddRow.Text,txtSalesTriggerAddRow.Text,txtValueTriggerAddRow.Text))
                {
                    msgView.SetMessage("Rate already exists", MessageType.Warning, PositionType.Auto);
                    return;
                }

                AppendRowToGrid();
                ClearAddRow();

            }
            catch (Exception ex)
            {
                //ExceptionHandler("Error in adding option period row to grid", ex.Message);
                ExceptionHandler("Error in adding option period row to grid", ex.ToString());
            }
        }

        protected void btnSaveProRata_Click(object sender, EventArgs e)
        {
            try
            {
                Array salesProRataList = SalesProRataList();

                if (salesProRataList.Length == 0)
                {
                    msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                royContractEscRatesBL = new RoyContractEscRatesBL();
                DataSet proRataData = royContractEscRatesBL.SaveSalesCategoryProRata(royaltorId, Convert.ToString(Session["UserCode"]), salesProRataList, out errorId);
                royContractEscRatesBL = null;

                if (errorId == 2)
                {
                    ExceptionHandler("Error in saving prorata data", string.Empty);
                    return;
                }
                else if (proRataData.Tables.Count == 0)
                {
                    dtEmpty = new DataTable();
                    gvSalesCategoryProRata.DataSource = dtEmpty;
                    gvSalesCategoryProRata.EmptyDataText = "No data found for the selected royaltor";
                    gvSalesCategoryProRata.DataBind();
                }
                else
                {
                    if (proRataData.Tables[0].Rows.Count == 0)
                    {
                        gvSalesCategoryProRata.DataSource = proRataData.Tables[0];
                        gvSalesCategoryProRata.EmptyDataText = "No data found for the selected royaltor";
                        gvSalesCategoryProRata.DataBind();
                    }
                    else
                    {
                        gvSalesCategoryProRata.DataSource = FormatProRataData(proRataData);
                        gvSalesCategoryProRata.DataBind();

                        //Format prorata data display
                        FormatProRataDataDisplay();

                    }
                }

                mpeProRataPopup.Show();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving prorata data", ex.ToString());
            }
        }

        protected void btnProRata_Click(object sender, EventArgs e)
        {
            try
            {
                royContractEscRatesBL = new RoyContractEscRatesBL();
                DataSet proRataData = royContractEscRatesBL.GetSalesCategoryProRata(royaltorId, out errorId);
                royContractEscRatesBL = null;

                if (errorId == 2)
                {
                    ExceptionHandler("Error in loading prorata popup", string.Empty);
                    return;
                }
                else if (proRataData.Tables.Count == 0)
                {
                    dtEmpty = new DataTable();
                    gvSalesCategoryProRata.DataSource = dtEmpty;
                    gvSalesCategoryProRata.EmptyDataText = "No data found for the selected royaltor";
                    gvSalesCategoryProRata.DataBind();
                }
                else
                {
                    if (proRataData.Tables[0].Rows.Count == 0)
                    {
                        gvSalesCategoryProRata.DataSource = proRataData.Tables[0];
                        gvSalesCategoryProRata.EmptyDataText = "No data found for the selected royaltor";
                        gvSalesCategoryProRata.DataBind();
                    }
                    else
                    {
                        gvSalesCategoryProRata.DataSource = FormatProRataData(proRataData);
                        gvSalesCategoryProRata.DataBind();

                        //Format prorata data display
                        FormatProRataDataDisplay();

                        //set pop up panel and gridview panel height                    
                        pnlProRataPopup.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.75).ToString());
                        plnGridProRataPopup.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.75 - 100).ToString());//pnlProRataPopup

                    }
                }

                mpeProRataPopup.Show();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading prorata popup", ex.ToString());
            }
        }

        /// <summary>
        ///For each ESC_CODE and for distinct PRICE_GROUP_ESCALATIONS.PRICE_TYPE 
        ///display prorata data from royaltor_escalations for existing ones in the table. And from price_group_escalations for not existing ones                      
        /// </summary>        
        private DataTable FormatProRataData(DataSet dsProRataData)
        {
            string escCode;
            string priceType;
            string escProRata;

            DataTable dtEscProfiles = dsProRataData.Tables["dtEscProfiles"];
            DataTable dtProRataPriceGroup = dsProRataData.Tables["dtProRataPriceGroup"];
            DataTable dtProRataEscalations = dsProRataData.Tables["dtProRataEscalations"];

            DataTable dtProRataData = new DataTable();
            dtProRataData.Columns.Add("esc_code", typeof(string));
            dtProRataData.Columns.Add("price_type", typeof(string));
            dtProRataData.Columns.Add("escalation_prorata", typeof(Int32));

            foreach (DataRow drEscProfile in dtEscProfiles.Rows)
            {
                escCode = drEscProfile["esc_code"].ToString();

                foreach (DataRow drPriceType in dtProRataPriceGroup.Rows)
                {
                    priceType = drPriceType["price_type"].ToString();
                    escProRata = drPriceType["escalation_prorata"].ToString();

                    foreach (DataRow drEscProRata in dtProRataEscalations.Rows)
                    {
                        //check if prorata exist for this esc profile and price type
                        if (drEscProRata["esc_code"].ToString() == escCode && drEscProRata["price_type"].ToString() == priceType)
                        {
                            //Consider this prorata data from royaltor_escalations
                            escProRata = drEscProRata["escalation_prorata"].ToString();
                        }
                    }

                    dtProRataData.Rows.Add(escCode, priceType, escProRata);

                }
            }

            dtProRataData.DefaultView.Sort = "esc_code,escalation_prorata,price_type";
            dtProRataData = dtProRataData.DefaultView.ToTable();

            return dtProRataData;

        }

        private void FormatProRataDataDisplay()
        {
            for (int i = 0; i < gvSalesCategoryProRata.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvSalesCategoryProRata.Rows[i];

                if ((currentRow.FindControl("ldlEscCode") as Label).Text == string.Empty)
                {
                    continue;
                }

                for (int j = i + 1; j < gvSalesCategoryProRata.Rows.Count; j++)
                {
                    GridViewRow nextRow = gvSalesCategoryProRata.Rows[j];

                    if ((currentRow.FindControl("ldlEscCode") as Label).Text == (nextRow.FindControl("ldlEscCode") as Label).Text)
                    {
                        (nextRow.FindControl("ldlEscCode") as Label).Text = string.Empty;
                    }
                    else
                    {
                        break;
                    }
                }

            }

        }

        /// <summary>
        /// adds default prorata for the esc code of the royaltor where values are not present in royaltor_escalations table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnYesProrataConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                royContractEscRatesBL = new RoyContractEscRatesBL();
                royContractEscRatesBL.AddDefaultProrata(royaltorId, Convert.ToString(Session["UserCode"]), out errorId);
                royContractEscRatesBL = null;

                if (errorId == 2)
                {
                    ExceptionHandler("Error in adding default prorata", string.Empty);
                    return;
                }

                mpeProrataConfirm.Hide();
                NotifyPostSavingChanges();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding default prorata", ex.ToString());
            }
        }

        /// <summary>
        /// closes prorata confirmation pop up
        /// notifies user on save changes/redirects to next page for new royaltor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNoProrataConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                NotifyPostSavingChanges();
                mpeProrataConfirm.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding default prorata", ex.ToString());
            }
        }


        //JIRA-908 Changes by Ravi on 12/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string escProfileId = hdnDeleteEscProfileId.Value;
                string esscLevel = hdnDeleteEscLevel.Value;
                string isModified = hdnDeleteIsModified.Value;
                DeleteRowFromGrid(escProfileId, esscLevel, isModified);
                hdnDeleteEscProfileId.Value = "";
                hdnDeleteEscLevel.Value = "";
                hdnDeleteIsModified.Value = "";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting grid data", ex.Message);
            }
        }
        //JIRA-908 Changes by Ravi on 12/02/2019 -- End


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvEscRates_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyContEscRatesGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvEscRates.DataSource = dataView;
                gvEscRates.DataBind();
                Session["RoyContEscRatesGridData"] = dataView.ToTable();
            }
            //WUIN-1096 -  ReadOnlyUser
            if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()))
            {
                EnableReadonly();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion Events

        #region Methods
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void LoadEscRatesData()
        {
            string royaltor;
            royContractEscRatesBL = new RoyContractEscRatesBL();
            DataSet escRatesData = royContractEscRatesBL.GetEscRatesData(royaltorId, out royaltor, out errorId);
            royContractEscRatesBL = null;

            if (escRatesData.Tables.Count != 0 && errorId != 2)
            {
                txtRoyaltorId.Text = royaltor;

                Session["RoyContEscRatesGridDataInitial"] = escRatesData.Tables[0];
                Session["RoyContEscRatesGridData"] = escRatesData.Tables[0];

                //hold initial data in a session for processing profiles and rates
                DataTable dtInitialData = escRatesData.Tables[0].Clone();
                foreach (DataRow drInitial in escRatesData.Tables[0].Rows)
                {
                    dtInitialData.ImportRow(drInitial);
                }
                Session["RoyContEscRatesInitUnChanged"] = dtInitialData;

                if (escRatesData.Tables[0].Rows.Count == 0)
                {
                    gvEscRates.EmptyDataText = "No data found for the selected royaltor";
                }

                gvEscRates.DataSource = escRatesData.Tables[0];
                gvEscRates.DataBind();

            }
            else if (escRatesData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvEscRates.DataSource = dtEmpty;
                gvEscRates.EmptyDataText = "No data found for the selected royaltor";
                gvEscRates.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }

            hdnGridDataDeleted.Value = "N";
            ViewState["vsDeleteProfileIds"] = null;
            ViewState["vsDeleteRateIds"] = null;
        }

        private void ReOrderGridData()
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (Session["RoyContEscRatesGridData"] == null)
            {
                ExceptionHandler("Error in re-ordering rates data", string.Empty);
                return;
            }

            DataTable dtGridData = (DataTable)Session["RoyContEscRatesGridData"];
            DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData);
            Session["RoyContEscRatesGridData"] = dtGridChangedDataSorted;
            gvEscRates.DataSource = dtGridChangedDataSorted;
            gvEscRates.DataBind();

        }

        private DataTable GridDataSorted(DataTable inputData)
        {
            DataView dv = inputData.DefaultView;
            dv.Sort = sortOrder;
            DataTable dtSorted = dv.ToTable();
            return dtSorted;

        }
        
        private List<Array> EscRateChangeList()
        {
            if (Session["RoyContEscRatesGridDataInitial"] == null)
            {
                ExceptionHandler("Error in saving escalation rates data", string.Empty);
            }

            List<string> escProfileList = new List<string>();
            List<string> escRateList = new List<string>();

            #region Formulate Dataset to Save

            DataTable escRatesData = (DataTable)Session["RoyContEscRatesInitUnChanged"];
            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContEscRatesGridData"];

            DataTable dtEscProfile = new DataTable();
            dtEscProfile.Columns.Add("escalation_profile_id");
            dtEscProfile.Columns.Add("esc_code");
            dtEscProfile.Columns.Add("seller_group_code");
            dtEscProfile.Columns.Add("config_group_code");
            dtEscProfile.Columns.Add("price_group_code");
            dtEscProfile.Columns.Add("max_rate");
            dtEscProfile.Columns.Add("escalation_type");
            dtEscProfile.Columns.Add("is_modified");

            DataTable dtEscRate = new DataTable();
            dtEscRate.Columns.Add("escalation_profile_id");
            dtEscRate.Columns.Add("escalation_level");
            dtEscRate.Columns.Add("esc_code");
            dtEscRate.Columns.Add("seller_group_code");
            dtEscRate.Columns.Add("config_group_code");
            dtEscRate.Columns.Add("price_group_code");
            dtEscRate.Columns.Add("threshold_units", typeof(Double));
            dtEscRate.Columns.Add("ceiling_units");
            dtEscRate.Columns.Add("sales_pct");
            dtEscRate.Columns.Add("royalty_rate");
            dtEscRate.Columns.Add("unit_rate");
            dtEscRate.Columns.Add("revenue_rate");
            dtEscRate.Columns.Add("threshold_amount");
            dtEscRate.Columns.Add("ceiling_amount");
            dtEscRate.Columns.Add("is_modified");

            //profile or rate changes            
            #region looping grid
            string escProfileId;
            string escLevel;
            string escCode;
            string territory;
            string config;
            string salesType;            
            string salesTrigger;
            string valueTrigger;
            string salesPct;
            string royaltyRate;
            string unitRate;
            string revenueRate;
            string isModified;
            string maxRoyaltyRate;
            string maxUnitRate;
            string maxRevenueRate;
            Double maxRoyaltyRateConverted;
            Double maxUnitRateConverted;
            Double maxRevenueRateConverted;

            foreach (GridViewRow gvr in gvEscRates.Rows)
            {
                escProfileId = (gvr.FindControl("hdnEscProfileId") as HiddenField).Value;
                escLevel = (gvr.FindControl("hdnEscLevel") as HiddenField).Value;
                escCode = (gvr.FindControl("txtEscCode") as TextBox).Text;
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;                
                salesTrigger = (gvr.FindControl("txtSalesTrigger") as TextBox).Text;
                valueTrigger = (gvr.FindControl("txtValueTrigger") as TextBox).Text;
                salesPct = (gvr.FindControl("txtPctSales") as TextBox).Text;
                royaltyRate = (gvr.FindControl("txtRoyaltyRate") as TextBox).Text;
                unitRate = (gvr.FindControl("txtUnitRate") as TextBox).Text;
                revenueRate = (gvr.FindControl("txtRevenueRate") as TextBox).Text;
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                territory = (territory == string.Empty ? Global.DBNullParamValue : territory.Substring(0, territory.IndexOf("-") - 1));
                config = (config == string.Empty ? Global.DBNullParamValue : config.Substring(0, config.IndexOf("-") - 1));
                salesType = (salesType == string.Empty ? Global.DBNullParamValue : salesType.Substring(0, salesType.IndexOf("-") - 1));

                if (isModified == Global.DBNullParamValue)//new row
                {   
                    //new profile or rate
                    #region new profile or rate
                    if (escProfileId == "0")
                    {
                        bool addProfRow = true;
                        //check if this profile is already added to the deriving datatable
                        if (dtEscProfile.Rows.Count != 0)
                        {
                            DataRow[] isProfileAdded = dtEscProfile.Select("esc_code='" + escCode + "' AND seller_group_code='" + territory
                                                            + "' AND config_group_code='" + config
                                                            + "' AND price_group_code='" + salesType + "'");
                            if (isProfileAdded.Count() != 0)
                            {
                                addProfRow = false;
                            }
                        }

                        if (addProfRow)
                        {
                            //find max_rate & escalation type of the profile                            
                            string escType = Global.DBNullParamValue;//default value as nothing selected
                            Double maxRate = 0;
                            DataTable dtMaxRate = dtGridData.Select("escalation_profile_id='" + escProfileId + "' AND esc_code='" + escCode + "' AND seller_group_code='" + territory
                                                            + "' AND config_group_code='" + config + "' AND price_group_code='" + salesType + "'").CopyToDataTable();


                            if (dtMaxRate.Rows.Count != 0)
                            {
                                maxRoyaltyRate = dtMaxRate.Compute("max([royalty_rate])", string.Empty).ToString();
                                maxUnitRate = dtMaxRate.Compute("max([unit_rate])", string.Empty).ToString();
                                maxRevenueRate = dtMaxRate.Compute("max([revenue_rate])", string.Empty).ToString();

                                maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                                maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                                maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                                maxRate = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                                //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                                if (dtMaxRate.Rows[0]["sales_trigger"].ToString() != string.Empty)
                                {
                                    escType = "U";
                                }
                                else
                                {
                                    escType = "V";
                                }

                            }


                            DataRow drNewProfRow = dtEscProfile.NewRow();
                            drNewProfRow["escalation_profile_id"] = escProfileId;
                            drNewProfRow["esc_code"] = escCode;
                            drNewProfRow["seller_group_code"] = territory;
                            drNewProfRow["config_group_code"] = config;
                            drNewProfRow["price_group_code"] = salesType;
                            drNewProfRow["max_rate"] = (maxRate == int.MinValue ? "0" : maxRate.ToString());
                            drNewProfRow["escalation_type"] = escType;
                            drNewProfRow["is_modified"] = Global.DBNullParamValue;
                            dtEscProfile.Rows.Add(drNewProfRow);
                        }

                    }
                    else
                    {

                        //existing profile - check if max_rate is changed or escalation type changed. if changed then update profile                        
                        //find max_rate & escalation type of the profile for the initial values                        
                        string escType1 = Global.DBNullParamValue;//default value as nothing selected
                        Double maxRate1 = 0;
                        DataTable dtMaxRate1 = escRatesData.Select("escalation_profile_id='" + escProfileId + "' AND esc_code='" + escCode + "' AND seller_group_code='" + territory
                                                        + "' AND config_group_code='" + config + "' AND price_group_code='" + salesType + "'").CopyToDataTable();
                        if (dtMaxRate1.Rows.Count != 0)
                        {
                            maxRoyaltyRate = dtMaxRate1.Compute("max([royalty_rate])", string.Empty).ToString();
                            maxUnitRate = dtMaxRate1.Compute("max([unit_rate])", string.Empty).ToString();
                            maxRevenueRate = dtMaxRate1.Compute("max([revenue_rate])", string.Empty).ToString();

                            maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                            maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                            maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                            maxRate1 = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                            //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                            if (dtMaxRate1.Rows[0]["sales_trigger"].ToString() != string.Empty)
                            {
                                escType1 = "U";
                            }
                            else
                            {
                                escType1 = "V";
                            }

                        }


                        //find max_rate & escalation type of the profile for the grid values                        
                        string escType2 = Global.DBNullParamValue;//default value as nothing selected
                        Double maxRate2 = 0;
                        DataTable dtMaxRate2 = dtGridData.Select("escalation_profile_id='" + escProfileId + "' AND esc_code='" + escCode + "' AND seller_group_code='" + territory
                                                        + "' AND config_group_code='" + config + "' AND price_group_code='" + salesType + "'").CopyToDataTable();
                        if (dtMaxRate2.Rows.Count != 0)
                        {
                            maxRoyaltyRate = dtMaxRate2.Compute("max([royalty_rate])", string.Empty).ToString();
                            maxUnitRate = dtMaxRate2.Compute("max([unit_rate])", string.Empty).ToString();
                            maxRevenueRate = dtMaxRate2.Compute("max([revenue_rate])", string.Empty).ToString();

                            maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                            maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                            maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                            maxRate2 = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                            //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                            if (dtMaxRate2.Rows[0]["sales_trigger"].ToString() != string.Empty)
                            {
                                escType2 = "U";
                            }
                            else
                            {
                                escType2 = "V";
                            }
                        }

                        //find escalation_type for the initial values


                        if (maxRate1 != maxRate2 || escType1 != escType2)
                        {
                            //update profile
                            bool addProfRow = true;
                            //check if this profile is already added to the deriving datatable
                            if (dtEscProfile.Rows.Count != 0)
                            {
                                DataRow[] isProfileAdded = dtEscProfile.Select("esc_code='" + escCode + "' AND seller_group_code='" + territory
                                                                + "' AND config_group_code='" + config
                                                                + "' AND price_group_code='" + salesType + "'");
                                if (isProfileAdded.Count() != 0)
                                {
                                    addProfRow = false;
                                }
                            }

                            if (addProfRow)
                            {
                                //find max_rate & escalation type of the profile                            
                                string escType = Global.DBNullParamValue;//default value as nothing selected
                                Double maxRate = 0;
                                DataTable dtMaxRate = dtGridData.Select("escalation_profile_id='" + escProfileId + "' AND esc_code='" + escCode + "' AND seller_group_code='" + territory
                                                                + "' AND config_group_code='" + config + "' AND price_group_code='" + salesType + "'").CopyToDataTable();
                                if (dtMaxRate.Rows.Count != 0)
                                {
                                    maxRoyaltyRate = dtMaxRate.Compute("max([royalty_rate])", string.Empty).ToString();
                                    maxUnitRate = dtMaxRate.Compute("max([unit_rate])", string.Empty).ToString();
                                    maxRevenueRate = dtMaxRate.Compute("max([revenue_rate])", string.Empty).ToString();

                                    maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                                    maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                                    maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                                    maxRate = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                                    //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                                    if (dtMaxRate.Rows[0]["sales_trigger"].ToString() != string.Empty)
                                    {
                                        escType = "U";
                                    }
                                    else
                                    {
                                        escType = "V";
                                    }
                                }



                                DataRow drNewProfRow = dtEscProfile.NewRow();
                                drNewProfRow["escalation_profile_id"] = escProfileId;
                                drNewProfRow["esc_code"] = escCode;
                                drNewProfRow["seller_group_code"] = territory;
                                drNewProfRow["config_group_code"] = config;
                                drNewProfRow["price_group_code"] = salesType;
                                drNewProfRow["max_rate"] = (maxRate == int.MinValue ? "0" : maxRate.ToString());
                                drNewProfRow["escalation_type"] = escType;
                                drNewProfRow["is_modified"] = "Y";
                                dtEscProfile.Rows.Add(drNewProfRow);
                            }
                        }

                    }


                    //new rate row for a new or an existing profile
                    DataRow drNewRateRow = dtEscRate.NewRow();
                    drNewRateRow["escalation_profile_id"] = escProfileId;
                    drNewRateRow["escalation_level"] = 0;
                    drNewRateRow["esc_code"] = escCode;
                    drNewRateRow["seller_group_code"] = territory;
                    drNewRateRow["config_group_code"] = config;
                    drNewRateRow["price_group_code"] = salesType;
                    drNewRateRow["threshold_units"] = ((salesTrigger == string.Empty || valueTrigger != string.Empty) ? "0" : salesTrigger);//generated from Sales Trigger, or low values for level 0; 0 if Value Trigger entered
                    drNewRateRow["ceiling_units"] = ((salesTrigger == string.Empty || valueTrigger != string.Empty) ? "0" : salesTrigger);//generated from Sales Trigger of next level or high values if top level; 0 if Value Trigger entered
                    drNewRateRow["sales_pct"] = (salesPct == string.Empty ? Global.DBNullParamValue : salesPct);
                    drNewRateRow["royalty_rate"] = (royaltyRate == string.Empty ? Global.DBNullParamValue : royaltyRate);
                    drNewRateRow["unit_rate"] = (unitRate == string.Empty ? Global.DBNullParamValue : unitRate);
                    drNewRateRow["revenue_rate"] = (revenueRate == string.Empty ? Global.DBNullParamValue : revenueRate);
                    drNewRateRow["threshold_amount"] = ((valueTrigger == string.Empty || salesTrigger != string.Empty) ? "0" : valueTrigger);//generated from Value Trigger, or low values for level 0; 0 if Sales Trigger entered
                    drNewRateRow["ceiling_amount"] = ((valueTrigger == string.Empty || salesTrigger != string.Empty) ? "0" : valueTrigger);//generated from Value Trigger of next level or high values if top level; 0 if Sales Trigger entered
                    drNewRateRow["is_modified"] = Global.DBNullParamValue;
                    dtEscRate.Rows.Add(drNewRateRow);

                    #endregion new profile or rate

                }
                else
                {
                    //modified profile or rate
                    #region modified profile or rate
                    DataTable dtRateModified = escRatesData.Select("escalation_profile_id='" + escProfileId + "' AND escalation_level='" + escLevel + "'").CopyToDataTable();
                    if (dtRateModified.Rows.Count != 0)
                    {
                        if (escCode != dtRateModified.Rows[0]["esc_code"].ToString()
                                      || territory != dtRateModified.Rows[0]["seller_group_code"].ToString()
                                      || config != dtRateModified.Rows[0]["config_group_code"].ToString()
                                      || salesType != dtRateModified.Rows[0]["price_group_code"].ToString()                            
                                      || salesTrigger != dtRateModified.Rows[0]["sales_trigger"].ToString()
                                      || valueTrigger != dtRateModified.Rows[0]["value_trigger"].ToString()
                                      || salesPct != dtRateModified.Rows[0]["sales_pct"].ToString()
                                      || royaltyRate != (dtRateModified.Rows[0]["royalty_rate"].ToString() == "" ? "" : Convert.ToDouble(dtRateModified.Rows[0]["royalty_rate"]).ToString()) //WUIN-1154
                                      || unitRate != (dtRateModified.Rows[0]["unit_rate"].ToString() == "" ? "" : Convert.ToDouble(dtRateModified.Rows[0]["unit_rate"]).ToString())
                                      || revenueRate != (dtRateModified.Rows[0]["revenue_rate"].ToString() == "" ? "" : Convert.ToDouble(dtRateModified.Rows[0]["revenue_rate"]).ToString()))//existing row - if data changed
                        {
                            
                            //add to profile 
                            bool addProfRow = true;
                            //check if this profile is already added to the deriving datatable
                            if (dtEscProfile.Rows.Count != 0)
                            {
                                DataRow[] isProfileAdded = dtEscProfile.Select("esc_code='" + escCode
                                                            + "' AND seller_group_code='" + territory
                                                            + "' AND config_group_code='" + config
                                                            + "' AND price_group_code='" + salesType + "'");
                                if (isProfileAdded.Count() != 0)
                                {
                                    addProfRow = false;
                                }
                            }

                            if (addProfRow)
                            {
                                //find max_rate & escalation type of the profile                            
                                string escType = Global.DBNullParamValue;//default value as nothing selected
                                Double maxRate = 0;
                                DataTable dtMaxRate = dtGridData.Select("esc_code='" + escCode + "' AND seller_group_code='" + territory
                                                                + "' AND config_group_code='" + config + "' AND price_group_code='" + salesType + "'").CopyToDataTable();
                                if (dtMaxRate.Rows.Count != 0)
                                {
                                    maxRoyaltyRate = dtMaxRate.Compute("max([royalty_rate])", string.Empty).ToString();
                                    maxUnitRate = dtMaxRate.Compute("max([unit_rate])", string.Empty).ToString();
                                    maxRevenueRate = dtMaxRate.Compute("max([revenue_rate])", string.Empty).ToString();

                                    maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                                    maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                                    maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                                    maxRate = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                                    //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                                    if (dtMaxRate.Rows[0]["sales_trigger"].ToString() != string.Empty)
                                    {
                                        escType = "U";
                                    }
                                    else
                                    {
                                        escType = "V";
                                    }
                                }


                                DataRow drNewProfRow = dtEscProfile.NewRow();
                                drNewProfRow["escalation_profile_id"] = escProfileId;
                                drNewProfRow["esc_code"] = escCode;
                                drNewProfRow["seller_group_code"] = territory;
                                drNewProfRow["config_group_code"] = config;
                                drNewProfRow["price_group_code"] = salesType;
                                drNewProfRow["max_rate"] = (maxRate == int.MinValue ? "0" : maxRate.ToString());
                                drNewProfRow["escalation_type"] = escType;
                                drNewProfRow["is_modified"] = "Y";
                                dtEscProfile.Rows.Add(drNewProfRow);


                            }

                            {
                                DataRow drNewRateRow = dtEscRate.NewRow();
                                drNewRateRow["escalation_profile_id"] = escProfileId;
                                drNewRateRow["escalation_level"] = escLevel;
                                drNewRateRow["esc_code"] = escCode;
                                drNewRateRow["seller_group_code"] = territory;
                                drNewRateRow["config_group_code"] = config;
                                drNewRateRow["price_group_code"] = salesType;
                                drNewRateRow["threshold_units"] = ((salesTrigger == string.Empty || valueTrigger != string.Empty) ? "0" : salesTrigger);//generated from Sales Trigger, or low values for level 0; 0 if Value Trigger entered;
                                drNewRateRow["ceiling_units"] = ((salesTrigger == string.Empty || valueTrigger != string.Empty) ? "0" : salesTrigger);//generated from Sales Trigger of next level or high values if top level; 0 if Value Trigger entered                                
                                drNewRateRow["sales_pct"] = (salesPct == string.Empty ? Global.DBNullParamValue : salesPct);
                                drNewRateRow["royalty_rate"] = (royaltyRate == string.Empty ? Global.DBNullParamValue : royaltyRate);
                                drNewRateRow["unit_rate"] = (unitRate == string.Empty ? Global.DBNullParamValue : unitRate);
                                drNewRateRow["revenue_rate"] = (revenueRate == string.Empty ? Global.DBNullParamValue : revenueRate);
                                drNewRateRow["threshold_amount"] = ((valueTrigger == string.Empty || salesTrigger != string.Empty) ? "0" : valueTrigger);//generated from Value Trigger, or low values for level 0; 0 if Sales Trigger entered
                                drNewRateRow["ceiling_amount"] = ((valueTrigger == string.Empty || salesTrigger != string.Empty) ? "0" : valueTrigger);//generated from Value Trigger of next level or high values if top level; 0 if Sales Trigger entered
                                drNewRateRow["is_modified"] = "Y";
                                dtEscRate.Rows.Add(drNewRateRow);

                            }


                            // if profile is modified - add the profile and rates >= to the level of the initial profile which has been modified
                            if (escCode != dtRateModified.Rows[0]["esc_code"].ToString()
                                    || territory != dtRateModified.Rows[0]["seller_group_code"].ToString()
                                    || config != dtRateModified.Rows[0]["config_group_code"].ToString()
                                    || salesType != dtRateModified.Rows[0]["price_group_code"].ToString())
                            {
                                //add to profile 
                                addProfRow = true;
                                //check if this profile is already added to the deriving datatable
                                if (dtEscProfile.Rows.Count != 0)
                                {
                                    DataRow[] isProfileAdded = dtEscProfile.Select("esc_code='" + dtRateModified.Rows[0]["esc_code"].ToString()
                                        + "' AND seller_group_code='" + (dtRateModified.Rows[0]["seller_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : dtRateModified.Rows[0]["seller_group_code"].ToString())
                                        + "' AND config_group_code='" + (dtRateModified.Rows[0]["config_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : dtRateModified.Rows[0]["config_group_code"].ToString())
                                        + "' AND price_group_code='" + (dtRateModified.Rows[0]["price_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : dtRateModified.Rows[0]["price_group_code"].ToString()) + "'");
                                    if (isProfileAdded.Count() != 0)
                                    {
                                        addProfRow = false;
                                    }
                                }

                                if (addProfRow)
                                {

                                    //find max_rate & escalation type of the profile                            
                                    string escType = Global.DBNullParamValue;//default value as nothing selected
                                    Double maxRate = 0;
                                    DataTable dtMaxRate = new DataTable();
                                    DataRow[] dtMaxRate1 = dtGridData.Select("esc_code='" + dtRateModified.Rows[0]["esc_code"].ToString() + "' AND seller_group_code='" + dtRateModified.Rows[0]["seller_group_code"].ToString()
                                                                    + "' AND config_group_code='" + dtRateModified.Rows[0]["config_group_code"].ToString() + "' AND price_group_code='" + dtRateModified.Rows[0]["price_group_code"].ToString() + "'");
                                    if (dtMaxRate1.Count() != 0)
                                    {
                                        dtMaxRate = dtGridData.Select("esc_code='" + dtRateModified.Rows[0]["esc_code"].ToString() + "' AND seller_group_code='" + dtRateModified.Rows[0]["seller_group_code"].ToString()
                                                                        + "' AND config_group_code='" + dtRateModified.Rows[0]["config_group_code"].ToString() + "' AND price_group_code='" + dtRateModified.Rows[0]["price_group_code"].ToString() + "'").CopyToDataTable();
                                    }
                                    else
                                    {
                                        dtMaxRate = dtRateModified.Clone();
                                        foreach (DataRow dr in dtRateModified.Rows)
                                        {
                                            dtMaxRate.ImportRow(dr);
                                        }
                                    }

                                    if (dtMaxRate.Rows.Count != 0)
                                    {
                                        maxRoyaltyRate = dtMaxRate.Compute("max([royalty_rate])", string.Empty).ToString();
                                        maxUnitRate = dtMaxRate.Compute("max([unit_rate])", string.Empty).ToString();
                                        maxRevenueRate = dtMaxRate.Compute("max([revenue_rate])", string.Empty).ToString();

                                        maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                                        maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                                        maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                                        maxRate = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                                        //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                                        if (dtMaxRate.Rows[0]["sales_trigger"].ToString() != string.Empty)
                                        {
                                            escType = "U";
                                        }
                                        else
                                        {
                                            escType = "V";
                                        }
                                    }


                                    DataRow drNewProfRow = dtEscProfile.NewRow();
                                    drNewProfRow["escalation_profile_id"] = escProfileId;
                                    drNewProfRow["esc_code"] = dtRateModified.Rows[0]["esc_code"].ToString();
                                    drNewProfRow["seller_group_code"] = (dtRateModified.Rows[0]["seller_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : dtRateModified.Rows[0]["seller_group_code"].ToString());
                                    drNewProfRow["config_group_code"] = (dtRateModified.Rows[0]["config_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : dtRateModified.Rows[0]["config_group_code"].ToString());
                                    drNewProfRow["price_group_code"] = (dtRateModified.Rows[0]["price_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : dtRateModified.Rows[0]["price_group_code"].ToString());
                                    drNewProfRow["max_rate"] = (maxRate == int.MinValue ? "0" : maxRate.ToString());
                                    drNewProfRow["escalation_type"] = escType;
                                    drNewProfRow["is_modified"] = "Y";
                                    dtEscProfile.Rows.Add(drNewProfRow);

                                }

                                DataRow[] dtRateDeleted = escRatesData.Select("escalation_profile_id='" + dtRateModified.Rows[0]["escalation_profile_id"].ToString() + "' AND escalation_level >='" + dtRateModified.Rows[0]["escalation_level"].ToString() + "'");
                                foreach (DataRow drToDelete in dtRateDeleted)
                                {
                                    DataRow drNewRateRow = dtEscRate.NewRow();
                                    drNewRateRow["escalation_profile_id"] = drToDelete["escalation_profile_id"].ToString();
                                    drNewRateRow["esc_code"] = drToDelete["esc_code"].ToString();
                                    drNewRateRow["escalation_level"] = drToDelete["escalation_level"].ToString();
                                    drNewRateRow["seller_group_code"] = (drToDelete["seller_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drToDelete["seller_group_code"].ToString());
                                    drNewRateRow["config_group_code"] = (drToDelete["config_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drToDelete["config_group_code"].ToString());
                                    drNewRateRow["price_group_code"] = (drToDelete["price_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drToDelete["price_group_code"].ToString());
                                    drNewRateRow["threshold_units"] = (drToDelete["sales_trigger"].ToString() == string.Empty ? "0" : drToDelete["sales_trigger"].ToString());
                                    drNewRateRow["ceiling_units"] = (drToDelete["sales_trigger"].ToString() == string.Empty ? "0" : drToDelete["sales_trigger"].ToString());
                                    drNewRateRow["sales_pct"] = (drToDelete["sales_pct"].ToString() == string.Empty ? Global.DBNullParamValue : drToDelete["sales_pct"].ToString());
                                    drNewRateRow["royalty_rate"] = (drToDelete["royalty_rate"].ToString() == string.Empty ? Global.DBNullParamValue : drToDelete["royalty_rate"].ToString());
                                    drNewRateRow["unit_rate"] = (drToDelete["unit_rate"].ToString() == string.Empty ? Global.DBNullParamValue : drToDelete["unit_rate"].ToString());
                                    drNewRateRow["revenue_rate"] = (drToDelete["revenue_rate"].ToString() == string.Empty ? Global.DBNullParamValue : drToDelete["revenue_rate"].ToString());
                                    drNewRateRow["threshold_amount"] = (drToDelete["value_trigger"].ToString() == string.Empty ? "0" : drToDelete["value_trigger"].ToString());
                                    drNewRateRow["ceiling_amount"] = (drToDelete["value_trigger"].ToString() == string.Empty ? "0" : drToDelete["value_trigger"].ToString());
                                    drNewRateRow["is_modified"] = "D";
                                    dtEscRate.Rows.Add(drNewRateRow);
                                }


                            }

                        }
                    }

                    #endregion modified profile or rate
                }

            }
            #endregion looping grid

            //adding deleted rates
            List<string> deleteRateList = new List<string>();
            if (ViewState["vsDeleteRateIds"] != null)
            {
                deleteRateList = (List<string>)ViewState["vsDeleteRateIds"];
                foreach (string rateDeleted in deleteRateList)
                {
                    DataTable dtRateDeleted = escRatesData.Select("escalation_profile_id='" + rateDeleted.Split(';')[0].Trim() + "' AND escalation_level >='" + rateDeleted.Split(';')[1].Trim() + "'").CopyToDataTable();
                    //if (dtRateDeleted.Rows.Count != 0)
                    foreach (DataRow drToDelete in dtRateDeleted.Rows)
                    {
                        DataRow drNewRateRow = dtEscRate.NewRow();
                        drNewRateRow["escalation_profile_id"] = drToDelete["escalation_profile_id"].ToString();
                        drNewRateRow["esc_code"] = drToDelete["esc_code"].ToString();
                        drNewRateRow["escalation_level"] = drToDelete["escalation_level"].ToString();
                        drNewRateRow["seller_group_code"] = (drToDelete["seller_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drToDelete["seller_group_code"].ToString());
                        drNewRateRow["config_group_code"] = (drToDelete["config_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drToDelete["config_group_code"].ToString());
                        drNewRateRow["price_group_code"] = (drToDelete["price_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drToDelete["price_group_code"].ToString());
                        drNewRateRow["threshold_units"] = (drToDelete["sales_trigger"].ToString() == string.Empty ? "0" : drToDelete["sales_trigger"].ToString());
                        drNewRateRow["ceiling_units"] = (drToDelete["sales_trigger"].ToString() == string.Empty ? "0" : drToDelete["sales_trigger"].ToString());
                        drNewRateRow["sales_pct"] = (drToDelete["sales_pct"].ToString() == string.Empty ? Global.DBNullParamValue : drToDelete["sales_pct"].ToString());
                        drNewRateRow["royalty_rate"] = (drToDelete["royalty_rate"].ToString() == string.Empty ? Global.DBNullParamValue : drToDelete["royalty_rate"].ToString());
                        drNewRateRow["unit_rate"] = (drToDelete["unit_rate"].ToString() == string.Empty ? Global.DBNullParamValue : drToDelete["unit_rate"].ToString());
                        drNewRateRow["revenue_rate"] = (drToDelete["revenue_rate"].ToString() == string.Empty ? Global.DBNullParamValue : dtRateDeleted.Rows[0]["revenue_rate"].ToString());
                        drNewRateRow["threshold_amount"] = (drToDelete["value_trigger"].ToString() == string.Empty ? "0" : drToDelete["value_trigger"].ToString());
                        drNewRateRow["ceiling_amount"] = (drToDelete["value_trigger"].ToString() == string.Empty ? "0" : drToDelete["value_trigger"].ToString());
                        drNewRateRow["is_modified"] = "D";
                        dtEscRate.Rows.Add(drNewRateRow);
                    }
                }
            }

            //adding rest of rows of the modified profile(either adding a new rate or modifing an existing rate)
            #region adding rest of rows
            DataTable dtEscRateFinal = dtEscRate.Clone();
            string escProfileIdOfProf;
            string escLevelOfProf;
            string escCodeOfProf;
            string sellerGrpCodeOfProf;
            string configGrpCodeOfProf;
            string priceGrpCodeOfProf;
            //string salesAboveOfProf;
            string salesTrigOfProf;
            string valueTrigOfProf;
            string salesPctOfProf;
            string royaltyRateOfProf;
            string unitRateOfProf;
            string revenueRateOfProf;
            string isModifiedOfProf;
            DataTable dtRatesProfiles1 = dtEscRate.DefaultView.ToTable(true, "esc_code", "seller_group_code", "config_group_code", "price_group_code");
            foreach (DataRow drProf in dtRatesProfiles1.Rows)
            {
                DataRow[] dtRates = dtGridData.Select("esc_code='" + drProf["esc_code"] + "' AND seller_group_code='" +
                        drProf["seller_group_code"] + "' AND config_group_code='" + drProf["config_group_code"] + "' AND price_group_code='" + drProf["price_group_code"] + "'");
                if (dtRates.Count() > 0)
                {
                    foreach (DataRow dr in dtRates)
                    {

                        escProfileIdOfProf = dr["escalation_profile_id"].ToString();
                        escLevelOfProf = dr["escalation_level"].ToString();
                        escCodeOfProf = dr["esc_code"].ToString();
                        sellerGrpCodeOfProf = dr["seller_group_code"].ToString();
                        configGrpCodeOfProf = dr["config_group_code"].ToString();
                        priceGrpCodeOfProf = dr["price_group_code"].ToString();
                        //salesAboveOfProf = dr["sales_above"].ToString();
                        salesTrigOfProf = (dr["sales_trigger"].ToString() == string.Empty ? "0" : dr["sales_trigger"].ToString());
                        valueTrigOfProf = (dr["value_trigger"].ToString() == string.Empty ? "0" : dr["value_trigger"].ToString());
                        salesPctOfProf = dr["sales_pct"].ToString();
                        royaltyRateOfProf = dr["royalty_rate"].ToString();
                        unitRateOfProf = dr["unit_rate"].ToString();
                        revenueRateOfProf = dr["revenue_rate"].ToString();
                        isModifiedOfProf = dr["is_modified"].ToString();

                        //check if the row already exists                         
                        DataRow[] isRateRowAdded = dtEscRateFinal.Select("esc_code='" + escCodeOfProf + "' AND seller_group_code='" + sellerGrpCodeOfProf
                                                           + "' AND config_group_code='" + configGrpCodeOfProf + "' AND price_group_code='" + priceGrpCodeOfProf +
                                                             "' AND threshold_units='" + salesTrigOfProf + "' AND ceiling_units='" + salesTrigOfProf + "' AND sales_pct='"
                                                             + salesPctOfProf + "' AND royalty_rate='" + royaltyRateOfProf + "' AND unit_rate='" + unitRateOfProf + "' AND revenue_rate='" + revenueRateOfProf + "'"
                                                             + " AND threshold_amount='" + valueTrigOfProf + "' AND ceiling_amount='" + valueTrigOfProf + "'"
                                                           );

                        if (isRateRowAdded.Count() == 0)
                        {
                            //new rate row for a new or an existing profile
                            DataRow drNewRateRow = dtEscRateFinal.NewRow();
                            drNewRateRow["escalation_profile_id"] = escProfileIdOfProf;
                            drNewRateRow["escalation_level"] = escLevelOfProf;
                            drNewRateRow["esc_code"] = escCodeOfProf;
                            drNewRateRow["seller_group_code"] = sellerGrpCodeOfProf;
                            drNewRateRow["config_group_code"] = configGrpCodeOfProf;
                            drNewRateRow["price_group_code"] = priceGrpCodeOfProf;
                            drNewRateRow["threshold_units"] = salesTrigOfProf;
                            drNewRateRow["ceiling_units"] = salesTrigOfProf;
                            drNewRateRow["sales_pct"] = (salesPctOfProf == string.Empty ? Global.DBNullParamValue : salesPctOfProf);
                            drNewRateRow["royalty_rate"] = (royaltyRateOfProf == string.Empty ? Global.DBNullParamValue : royaltyRateOfProf);
                            drNewRateRow["unit_rate"] = (unitRateOfProf == string.Empty ? Global.DBNullParamValue : unitRateOfProf);
                            drNewRateRow["revenue_rate"] = (revenueRateOfProf == string.Empty ? Global.DBNullParamValue : revenueRateOfProf);
                            drNewRateRow["threshold_amount"] = valueTrigOfProf;
                            drNewRateRow["ceiling_amount"] = valueTrigOfProf;
                            //check if this rate has been created by change of profile. if so delete and add this rate row
                            DataRow[] isRateRowDeleted = dtEscRate.Select("esc_code='" + escCodeOfProf + "' AND seller_group_code='" + sellerGrpCodeOfProf
                                                           + "' AND config_group_code='" + configGrpCodeOfProf + "' AND price_group_code='" + priceGrpCodeOfProf +
                                                             "' AND threshold_units='" + salesTrigOfProf + "' AND ceiling_units='" + salesTrigOfProf + "' AND sales_pct='"
                                                             + salesPctOfProf + "' AND royalty_rate='" + royaltyRateOfProf + "' AND unit_rate='" + unitRateOfProf + "' AND revenue_rate='" + revenueRateOfProf + "'"
                                                             + " AND threshold_amount='" + valueTrigOfProf + "' AND ceiling_amount='" + valueTrigOfProf + "'"
                                                             + " AND is_modified = 'D'"
                                                           );
                            if (isRateRowDeleted.Count() == 0)
                            {
                                drNewRateRow["is_modified"] = (isModifiedOfProf != Global.DBNullParamValue ? "Y" : Global.DBNullParamValue);
                            }
                            else
                            {
                                drNewRateRow["is_modified"] = Global.DBNullParamValue;
                            }

                            dtEscRateFinal.Rows.Add(drNewRateRow);
                        }

                    }
                }

            }


            #endregion adding rest of rows

            //adjust level, threshold units, ceiling units, threshold_amount and ceiling_amount  accordingly for rates   
            /*THRESHOLD_UNITS	- generated from Sales Trigger, or low values for level 0; 0 if Value Trigger entered								
            CEILING_UNITS	- generated from Sales Trigger of next level or high values if top level; 0 if Value Trigger entered								
            THRESHOLD_AMOUNT - generated from Value Trigger, or low values for level 0; 0 if Sales Trigger entered								
            CEILING_AMOUNT	- generated from Value Trigger of next level or high values if top level; 0 if Sales Trigger entered								
            */
            #region adjust level, threshold units, ceiling units, threshold_amount and ceiling_amount
            DataTable dtRatesProfiles = dtEscRateFinal.DefaultView.ToTable(true, "esc_code", "seller_group_code", "config_group_code", "price_group_code");
            DataTable dtEscRateAdjusted = dtEscRateFinal.Clone();
            int adjustedDtRowId = 0;
            foreach (DataRow dr in dtRatesProfiles.Rows)
            {
                DataRow[] dtRateProfile1 = dtEscRateFinal.Select("esc_code='" + dr["esc_code"] + "' AND seller_group_code='" + dr["seller_group_code"] + "' AND config_group_code='" + dr["config_group_code"] +
                                                            "' AND price_group_code='" + dr["price_group_code"] + "' AND is_modified <> 'D'");

                if (dtRateProfile1.Count() != 0)
                {
                    DataTable dtRateProfile = dtEscRateFinal.Select("esc_code='" + dr["esc_code"] + "' AND seller_group_code='" + dr["seller_group_code"] + "' AND config_group_code='" + dr["config_group_code"] +
                                                            "' AND price_group_code='" + dr["price_group_code"] + "' AND is_modified <> 'D'").CopyToDataTable();

                    //adjust level, threshold units, ceiling units, threshold_amount and ceiling_amount accordingly for rates 
                    DataView dv = dtRateProfile.DefaultView;
                    //dv.Sort = "threshold_units asc";
                    dv.Sort = "threshold_units, threshold_amount asc";
                    DataTable dtSorted = dv.ToTable();

                    for (int i = 0; i < dtSorted.Rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            dtEscRateAdjusted.ImportRow(dtSorted.Rows[i]);
                            dtEscRateAdjusted.Rows[adjustedDtRowId]["escalation_level"] = "0";
                            //dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"] = int.MinValue;
                            //dtEscRateAdjusted.Rows[adjustedDtRowId]["sales_pct"] = "0";

                            if (dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_amount"].ToString() == "0" &&
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"].ToString() != "0")
                            {
                                //sales trigger entered

                                dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"] = int.MinValue;
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["sales_pct"] = "0";

                                dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_amount"] = "0";
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_amount"] = "0";
                            }
                            else
                            {
                                //value trigger entered

                                dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"] = "0";
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["sales_pct"] = "0";

                                dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_amount"] = int.MinValue;

                            }

                            dtEscRateAdjusted.Rows[adjustedDtRowId]["royalty_rate"] = "0";
                            dtEscRateAdjusted.Rows[adjustedDtRowId]["unit_rate"] = "0";
                            dtEscRateAdjusted.Rows[adjustedDtRowId]["revenue_rate"] = "0";

                            dtEscRateAdjusted.ImportRow(dtSorted.Rows[i]);
                            adjustedDtRowId++;
                            dtEscRateAdjusted.Rows[adjustedDtRowId]["escalation_level"] = (i + 1).ToString();

                            if (dtSorted.Rows.Count == 1)
                            {
                                if (dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_amount"].ToString() == "0" &&
                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"].ToString() != "0")
                                {
                                    //sales trigger entered

                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"] = dtSorted.Rows[i]["ceiling_units"];
                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_units"] = int.MaxValue;

                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_amount"] = "0";
                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_amount"] = "0";
                                }
                                else
                                {
                                    //value trigger entered

                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_amount"] = dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_amount"];
                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_amount"] = int.MaxValue;

                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"] = "0";
                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_units"] = "0";
                                }
                            }

                        }
                        else
                        {
                            dtEscRateAdjusted.ImportRow(dtSorted.Rows[i]);
                            adjustedDtRowId++;
                            /*
                            dtEscRateAdjusted.Rows[adjustedDtRowId - 1]["ceiling_units"] = dtSorted.Rows[i]["threshold_units"];
                            dtEscRateAdjusted.Rows[adjustedDtRowId]["escalation_level"] = (i + 1).ToString();

                            if ((i + 1) == dtSorted.Rows.Count)
                            {
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_units"] = int.MaxValue;
                            }
                             * */
                            if (dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_amount"].ToString() == "0" &&
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"].ToString() != "0")
                            {
                                //sales trigger entered

                                dtEscRateAdjusted.Rows[adjustedDtRowId - 1]["ceiling_units"] = dtSorted.Rows[i]["threshold_units"];
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["escalation_level"] = (i + 1).ToString();

                                if ((i + 1) == dtSorted.Rows.Count)
                                {
                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_units"] = int.MaxValue;
                                }

                                dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_amount"] = "0";
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_amount"] = "0";

                            }
                            else
                            {
                                //value trigger entered

                                dtEscRateAdjusted.Rows[adjustedDtRowId - 1]["ceiling_amount"] = dtSorted.Rows[i]["threshold_amount"];
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["escalation_level"] = (i + 1).ToString();

                                if ((i + 1) == dtSorted.Rows.Count)
                                {
                                    dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_amount"] = int.MaxValue;
                                }

                                dtEscRateAdjusted.Rows[adjustedDtRowId]["threshold_units"] = "0";
                                dtEscRateAdjusted.Rows[adjustedDtRowId]["ceiling_units"] = "0";

                            }

                        }

                    }

                    adjustedDtRowId++;
                }

            }
            
            //add deleted rows - when a rate is deleted or a profile is changed
            #region deletion
            DataRow[] isRateRowDeleted2 = dtEscRate.Select("is_modified = 'D'");
            if (isRateRowDeleted2.Count() > 0)
            {
                foreach (DataRow dr in isRateRowDeleted2)
                {
                    //adjust profile info only if there are any other rows in the grid for which profile rate is deleted
                    DataRow[] dtAddDeletedRate = dtGridData.Select("esc_code='" + dr["esc_code"].ToString() + "' AND seller_group_code='" + dr["seller_group_code"].ToString() +
                                                                    "' AND config_group_code='" + dr["config_group_code"].ToString() +
                                                                    "' AND price_group_code='" + dr["price_group_code"].ToString() + "'");
                    if (dtAddDeletedRate.Count() != 0)
                    {
                        dtEscRateAdjusted.ImportRow(dr);

                        //existing profile - check if max_rate is changed or escalation type changed. if changed then update profile                        
                        //find max_rate & escalation type of the profile for the initial values 
                        string escType1 = Global.DBNullParamValue;//default value as nothing selected
                        Double maxRate1 = 0;
                        DataTable dtMaxRate1 = escRatesData.Select("escalation_profile_id='" + dr["escalation_profile_id"].ToString() + "' AND esc_code='" + dr["esc_code"].ToString() +
                                                                    "' AND seller_group_code='" + dr["seller_group_code"].ToString() + "' AND config_group_code='" + dr["config_group_code"].ToString() +
                                                                    "' AND price_group_code='" + dr["price_group_code"].ToString() + "'").CopyToDataTable();
                        if (dtMaxRate1.Rows.Count != 0)
                        {
                            maxRoyaltyRate = dtMaxRate1.Compute("max([royalty_rate])", string.Empty).ToString();
                            maxUnitRate = dtMaxRate1.Compute("max([unit_rate])", string.Empty).ToString();
                            maxRevenueRate = dtMaxRate1.Compute("max([revenue_rate])", string.Empty).ToString();

                            maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                            maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                            maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                            maxRate1 = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                            //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                            if (dtMaxRate1.Rows[0]["sales_trigger"].ToString() != string.Empty)
                            {
                                escType1 = "U";
                            }
                            else
                            {
                                escType1 = "V";
                            }
                        }


                        //find max_rate & escalation type of the profile for the grid values                        
                        string escType2 = Global.DBNullParamValue;//default value as nothing selected
                        Double maxRate2 = 0;
                        DataTable dtMaxRate2 = dtGridData.Select("escalation_profile_id='" + dr["escalation_profile_id"].ToString() + "' AND esc_code='" + dr["esc_code"].ToString() +
                                                                    "' AND seller_group_code='" + dr["seller_group_code"].ToString() + "' AND config_group_code='" + dr["config_group_code"].ToString() +
                                                                    "' AND price_group_code='" + dr["price_group_code"].ToString() + "'").CopyToDataTable();
                        if (dtMaxRate2.Rows.Count != 0)
                        {
                            maxRoyaltyRate = dtMaxRate2.Compute("max([royalty_rate])", string.Empty).ToString();
                            maxUnitRate = dtMaxRate2.Compute("max([unit_rate])", string.Empty).ToString();
                            maxRevenueRate = dtMaxRate2.Compute("max([revenue_rate])", string.Empty).ToString();

                            maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                            maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                            maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                            maxRate2 = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                            //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                            if (dtMaxRate2.Rows[0]["sales_trigger"].ToString() != string.Empty)
                            {
                                escType2 = "U";
                            }
                            else
                            {
                                escType2 = "V";
                            }
                        }

                        if (maxRate1 != maxRate2 || escType1 != escType2)
                        {
                            //update profile
                            bool addProfRow = true;
                            //check if this profile is already added to the deriving datatable
                            if (dtEscProfile.Rows.Count != 0)
                            {
                                DataRow[] isProfileAdded = dtEscProfile.Select("esc_code='" + dr["esc_code"].ToString() + "' AND seller_group_code='" + dr["seller_group_code"].ToString()
                                                                + "' AND config_group_code='" + dr["config_group_code"].ToString()
                                                                + "' AND price_group_code='" + dr["price_group_code"].ToString() + "'");
                                if (isProfileAdded.Count() != 0)
                                {
                                    addProfRow = false;
                                }
                            }

                            if (addProfRow)
                            {
                                //find max_rate & escalation type of the profile                            
                                string escType = Global.DBNullParamValue;//default value as nothing selected
                                Double maxRate = 0;
                                DataTable dtMaxRate = dtGridData.Select("escalation_profile_id='" + dr["escalation_profile_id"].ToString() + "' AND esc_code='" + dr["esc_code"].ToString() +
                                                                    "' AND seller_group_code='" + dr["seller_group_code"].ToString() + "' AND config_group_code='" + dr["config_group_code"].ToString() +
                                                                    "' AND price_group_code='" + dr["price_group_code"].ToString() + "'").CopyToDataTable();
                                if (dtMaxRate.Rows.Count != 0)
                                {
                                    maxRoyaltyRate = dtMaxRate.Compute("max([royalty_rate])", string.Empty).ToString();
                                    maxUnitRate = dtMaxRate.Compute("max([unit_rate])", string.Empty).ToString();
                                    maxRevenueRate = dtMaxRate.Compute("max([revenue_rate])", string.Empty).ToString();

                                    maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                                    maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                                    maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                                    maxRate = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));

                                    //If Value Trigger entered, set to 'V'; if Sales Trigger entered, set to 'U'
                                    if (dtMaxRate.Rows[0]["sales_trigger"].ToString() != string.Empty)
                                    {
                                        escType = "U";
                                    }
                                    else
                                    {
                                        escType = "V";
                                    }
                                }


                                DataRow drNewProfRow = dtEscProfile.NewRow();
                                drNewProfRow["escalation_profile_id"] = dr["escalation_profile_id"].ToString();
                                drNewProfRow["esc_code"] = dr["esc_code"].ToString();
                                drNewProfRow["seller_group_code"] = dr["seller_group_code"].ToString();
                                drNewProfRow["config_group_code"] = dr["config_group_code"].ToString();
                                drNewProfRow["price_group_code"] = dr["price_group_code"].ToString();
                                drNewProfRow["max_rate"] = (maxRate == int.MinValue ? "0" : maxRate.ToString());
                                drNewProfRow["escalation_type"] = escType;
                                drNewProfRow["is_modified"] = "Y";
                                dtEscProfile.Rows.Add(drNewProfRow);
                            }
                        }
                    }
                }

            }
            #endregion deletion

            DataView dvEscRatesFinal = dtEscRateAdjusted.DefaultView;
            dvEscRatesFinal.Sort = "esc_code,escalation_level,esc_code";
            DataTable dtEscRatesFinal = dvEscRatesFinal.ToTable();

            #endregion adjust level, threshold units, ceiling units, threshold_amount and ceiling_amount

            #endregion Formulate Dataset to Save


            foreach (DataRow drAddUpdProf in dtEscProfile.Rows)
            {
                escProfileId = drAddUpdProf["escalation_profile_id"].ToString();
                isModified = drAddUpdProf["is_modified"].ToString();
                Double maxRate = Convert.ToDouble(drAddUpdProf["max_rate"].ToString());

                //for a modified profile
                if (isModified != Global.DBNullParamValue)
                {

                    //assign is_modified = D for a modified profile rows so that it can be deleted
                    DataRow[] dtIsModified = dtGridData.Select("esc_code='" + drAddUpdProf["esc_code"].ToString() +
                                                                "' AND seller_group_code='" + (drAddUpdProf["seller_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["seller_group_code"].ToString()) +
                                                                "' AND config_group_code='" + (drAddUpdProf["config_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["config_group_code"].ToString()) +
                                                                "' AND price_group_code='" + (drAddUpdProf["price_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["price_group_code"].ToString()) + "'");
                    if (dtIsModified.Count() == 0)
                    {
                        isModified = "D";
                    }
                    else
                    {
                        //check if max rate is correct - to handle for the modified profiles
                        DataTable dtMaxRate = dtGridData.Select("esc_code='" + drAddUpdProf["esc_code"].ToString()
                            + "' AND seller_group_code='" + (drAddUpdProf["seller_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["seller_group_code"].ToString())
                            + "' AND config_group_code='" + (drAddUpdProf["config_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["config_group_code"].ToString())
                            + "' AND price_group_code='" + (drAddUpdProf["price_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["price_group_code"].ToString()) + "'").CopyToDataTable();
                        if (dtMaxRate.Rows.Count != 0)
                        {
                            maxRoyaltyRate = dtMaxRate.Compute("max([royalty_rate])", string.Empty).ToString();
                            maxUnitRate = dtMaxRate.Compute("max([unit_rate])", string.Empty).ToString();
                            maxRevenueRate = dtMaxRate.Compute("max([revenue_rate])", string.Empty).ToString();

                            maxRoyaltyRateConverted = Convert.ToDouble(maxRoyaltyRate == string.Empty ? int.MinValue.ToString() : maxRoyaltyRate);
                            maxUnitRateConverted = Convert.ToDouble(maxUnitRate == string.Empty ? int.MinValue.ToString() : maxUnitRate);
                            maxRevenueRateConverted = Convert.ToDouble(maxRevenueRate == string.Empty ? int.MinValue.ToString() : maxRevenueRate);
                            maxRate = Math.Max(maxRoyaltyRateConverted, Math.Max(maxUnitRateConverted, maxRevenueRateConverted));
                        }

                        //check if modified profile exists in initial data. If not exist then it is a new one and set profile_id and is_modified accordingly
                        DataRow[] dtIsNewProf = escRatesData.Select("esc_code='" + drAddUpdProf["esc_code"].ToString() +
                                                                "' AND seller_group_code='" + (drAddUpdProf["seller_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["seller_group_code"].ToString()) +
                                                                "' AND config_group_code='" + (drAddUpdProf["config_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["config_group_code"].ToString()) +
                                                                "' AND price_group_code='" + (drAddUpdProf["price_group_code"].ToString() == wildCardChar ? Global.DBNullParamValue : drAddUpdProf["price_group_code"].ToString()) + "'");

                        if (dtIsNewProf.Count() == 0)
                        {
                            escProfileId = "0";
                            isModified = Global.DBNullParamValue;
                        }
                        else
                        {
                            isModified = "Y";
                        }

                    }
                }

                escProfileList.Add(escProfileId + "(;||;)" + drAddUpdProf["esc_code"].ToString() + "(;||;)" +
                                    drAddUpdProf["seller_group_code"].ToString() + "(;||;)" + drAddUpdProf["config_group_code"].ToString() + "(;||;)" +
                                    drAddUpdProf["price_group_code"].ToString() + "(;||;)" + (maxRate == int.MinValue ? "0" : maxRate.ToString()) + "(;||;)" + drAddUpdProf["escalation_type"].ToString() + "(;||;)" + isModified);

            }

            foreach (DataRow drAddUpdRate in dtEscRatesFinal.Rows)
            {
                escProfileId = drAddUpdRate["escalation_profile_id"].ToString();
                isModified = drAddUpdRate["is_modified"].ToString();

                //check if profile changed or not. If changed then it is a new one and set profile_id and is_modified accordingly               
                DataRow[] dtIsNewProfileRate = escRatesData.Select("esc_code='" + drAddUpdRate["esc_code"].ToString() +
                                                        "' AND seller_group_code='" + drAddUpdRate["seller_group_code"].ToString() +
                                                        "' AND config_group_code='" + drAddUpdRate["config_group_code"].ToString() +
                                                        "' AND price_group_code='" + drAddUpdRate["price_group_code"].ToString() + "'");
                if (dtIsNewProfileRate.Count() == 0)
                {
                    //profile changed - new profile                    
                    escProfileId = "0";
                    isModified = Global.DBNullParamValue;
                }
                else
                {
                    //profile not changed
                    //set is_modified
                    if (drAddUpdRate["escalation_level"].ToString() == "0")
                    {
                        isModified = "Y";
                    }
                    else
                    {
                        //check if profile_id exist initially. if exists, set is_modified = Y else to new one
                        DataRow[] dtIsNewRate = escRatesData.Select("escalation_level='" + drAddUpdRate["escalation_level"].ToString() +
                                                        "' AND esc_code='" + drAddUpdRate["esc_code"].ToString() +
                                                        "' AND seller_group_code='" + drAddUpdRate["seller_group_code"].ToString() +
                                                        "' AND config_group_code='" + drAddUpdRate["config_group_code"].ToString() +
                                                        "' AND price_group_code='" + drAddUpdRate["price_group_code"].ToString() + "'");
                        if (dtIsNewRate.Count() == 0)
                        {
                            //new rate row
                            isModified = Global.DBNullParamValue;
                        }
                        else if (isModified != "D")
                        {
                            DataRow[] dtIsDeleteExist = dtEscRatesFinal.Select("escalation_profile_id='" + drAddUpdRate["escalation_profile_id"].ToString() +
                                                        "' AND escalation_level='" + drAddUpdRate["escalation_level"].ToString() +
                                                        "' AND esc_code='" + drAddUpdRate["esc_code"].ToString() +
                                                        "' AND seller_group_code='" + drAddUpdRate["seller_group_code"].ToString() +
                                                        "' AND config_group_code='" + drAddUpdRate["config_group_code"].ToString() +
                                                        "' AND price_group_code='" + drAddUpdRate["price_group_code"].ToString() + "' AND is_modified='D'");
                            if (dtIsDeleteExist.Count() == 0)
                            {
                                isModified = "Y";
                            }
                            else
                            {
                                isModified = Global.DBNullParamValue;
                            }

                        }

                    }


                }


                escRateList.Add(escProfileId + "(;||;)" + drAddUpdRate["escalation_level"].ToString() + "(;||;)" +
                                    drAddUpdRate["esc_code"].ToString() + "(;||;)" + drAddUpdRate["seller_group_code"].ToString() + "(;||;)" +
                                    drAddUpdRate["config_group_code"].ToString() + "(;||;)" + drAddUpdRate["price_group_code"].ToString() + "(;||;)" +
                                    drAddUpdRate["threshold_units"].ToString() + "(;||;)" + drAddUpdRate["ceiling_units"].ToString() + "(;||;)" +
                                    drAddUpdRate["sales_pct"].ToString() + "(;||;)" + drAddUpdRate["royalty_rate"].ToString() + "(;||;)" +
                                    drAddUpdRate["unit_rate"].ToString() + "(;||;)" + drAddUpdRate["revenue_rate"].ToString() + "(;||;)" +
                                    drAddUpdRate["threshold_amount"].ToString() + "(;||;)" + drAddUpdRate["ceiling_amount"].ToString() + "(;||;)" +
                                    isModified);

            }

            List<Array> ChangeList = new List<Array>();
            ChangeList.Add(escProfileList.ToArray());
            ChangeList.Add(escRateList.ToArray());

            return ChangeList;

        }

        private void AppendRowToGrid()
        {
            if (Session["RoyContEscRatesGridData"] == null || Session["RoyContEscRatesGridDataInitial"] == null)
            {
                ExceptionHandler("Error in adding rate row to grid", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContEscRatesGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyContEscRatesGridDataInitial"];
            DataRow drNewRow = dtGridData.NewRow();
            DataRow drNewRowOriginal = dtOriginalGridData.NewRow();

            //check if the profile exists or a new one for the row being added
            string sellerGrpCode = string.Empty;
            string configCode = string.Empty;
            string priceGrpCode = string.Empty;

            if (txtTerritoryAddRow.Text != string.Empty)
            {
                sellerGrpCode = txtTerritoryAddRow.Text.Substring(0, txtTerritoryAddRow.Text.IndexOf("-") - 1);
            }

            if (txtConfigAddRow.Text != string.Empty)
            {
                configCode = txtConfigAddRow.Text.Substring(0, txtConfigAddRow.Text.IndexOf("-") - 1);
            }

            if (txtSalesTypeAddRow.Text != string.Empty)
            {
                priceGrpCode = txtSalesTypeAddRow.Text.Substring(0, txtSalesTypeAddRow.Text.IndexOf("-") - 1);
            }

            DataRow[] isProfileExists = dtGridData.Select("esc_code='" + txtEscCodeAddRow.Text.ToUpper() +
                "' AND seller_group_code='" + (sellerGrpCode == string.Empty ? "-" : sellerGrpCode) +
                "' AND config_group_code='" + (configCode == string.Empty ? "-" : configCode) +
                "' AND price_group_code='" + (priceGrpCode == string.Empty ? "-" : priceGrpCode) + "'");

            if (isProfileExists.Count() == 0)//new profile
            {
                drNewRow["escalation_profile_id"] = 0;
            }
            else
            {
                drNewRow["escalation_profile_id"] = isProfileExists[0]["escalation_profile_id"].ToString();
            }

            drNewRow["escalation_level"] = 0;
            drNewRow["esc_code"] = txtEscCodeAddRow.Text.ToUpper();

            if (txtTerritoryAddRow.Text == string.Empty)
            {
                drNewRow["seller_group_code"] = Global.DBNullParamValue;
                drNewRow["seller_group"] = DBNull.Value;
                drNewRow["seller_group_code_order"] = "1";//to sort data for wildcard to display bottom
            }
            else
            {
                drNewRow["seller_group_code"] = txtTerritoryAddRow.Text.Substring(0, txtTerritoryAddRow.Text.IndexOf("-") - 1);
                drNewRow["seller_group"] = txtTerritoryAddRow.Text;
                drNewRow["seller_group_code_order"] = "0";//to sort data for wildcard to display bottom
            }

            if (txtConfigAddRow.Text == string.Empty)
            {
                drNewRow["config_group_code"] = Global.DBNullParamValue;
                drNewRow["config_group"] = DBNull.Value;
            }
            else
            {
                drNewRow["config_group_code"] = txtConfigAddRow.Text.Substring(0, txtConfigAddRow.Text.IndexOf("-") - 1);
                drNewRow["config_group"] = txtConfigAddRow.Text;
            }

            if (txtSalesTypeAddRow.Text == string.Empty)
            {
                drNewRow["price_group_code"] = Global.DBNullParamValue;
                drNewRow["price_group"] = DBNull.Value;
            }
            else
            {
                drNewRow["price_group_code"] = txtSalesTypeAddRow.Text.Substring(0, txtSalesTypeAddRow.Text.IndexOf("-") - 1);
                drNewRow["price_group"] = txtSalesTypeAddRow.Text;
            }

            if (txtSalesTriggerAddRow.Text == string.Empty)
            {
                drNewRow["sales_trigger"] = DBNull.Value;
            }
            else
            {
                drNewRow["sales_trigger"] = txtSalesTriggerAddRow.Text;
            }

            if (txtValueTriggerAddRow.Text == string.Empty)
            {
                drNewRow["value_trigger"] = DBNull.Value;
            }
            else
            {
                drNewRow["value_trigger"] = txtValueTriggerAddRow.Text;
            }

            if (txtPctSalesAddRow.Text == string.Empty)
            {
                drNewRow["sales_pct"] = DBNull.Value;
            }
            else
            {
                drNewRow["sales_pct"] = txtPctSalesAddRow.Text;
            }

            if (txtRoyaltyRateAddRow.Text == string.Empty)
            {
                drNewRow["royalty_rate"] = DBNull.Value;
            }
            else
            {
                drNewRow["royalty_rate"] = txtRoyaltyRateAddRow.Text;
            }

            if (txtUnitRateAddRow.Text == string.Empty)
            {
                drNewRow["unit_rate"] = DBNull.Value;
            }
            else
            {
                drNewRow["unit_rate"] = txtUnitRateAddRow.Text;
            }

            if (txtRevenueRateAddRow.Text == string.Empty)
            {
                drNewRow["revenue_rate"] = DBNull.Value;
            }
            else
            {
                drNewRow["revenue_rate"] = txtRevenueRateAddRow.Text;
            }


            drNewRow["is_modified"] = Global.DBNullParamValue;

            dtGridData.Rows.Add(drNewRow);
            DataTable dtGridChangedDataSorted = dtGridData;
            Session["RoyContEscRatesGridData"] = dtGridChangedDataSorted;
            gvEscRates.DataSource = dtGridChangedDataSorted;
            gvEscRates.DataBind();
            txtEscCodeAddRow.Focus(); //JIRA-451 CHanges

            drNewRowOriginal.ItemArray = drNewRow.ItemArray.Clone() as object[];
            dtOriginalGridData.Rows.Add(drNewRowOriginal);
            DataTable dtOriginalGridDataSorted = GridDataSorted(dtOriginalGridData);
            Session["RoyContEscRatesGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["RoyContEscRatesGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            string territory;
            string config;
            string salesType;
            //string salesAbove;
            string salesTrigger;
            string valueTrigger;
            string salesPct;
            string royaltyRate;
            string unitRate;
            string revenueRate;

            foreach (GridViewRow gvr in gvEscRates.Rows)
            {
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;
                //salesAbove = (gvr.FindControl("txtSalesAbove") as TextBox).Text;
                salesTrigger = (gvr.FindControl("txtSalesTrigger") as TextBox).Text;
                valueTrigger = (gvr.FindControl("txtValueTrigger") as TextBox).Text;
                salesPct = (gvr.FindControl("txtPctSales") as TextBox).Text;
                royaltyRate = (gvr.FindControl("txtRoyaltyRate") as TextBox).Text;
                unitRate = (gvr.FindControl("txtUnitRate") as TextBox).Text;
                revenueRate = (gvr.FindControl("txtRevenueRate") as TextBox).Text;

                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["escalation_profile_id"] = (gvr.FindControl("hdnEscProfileId") as HiddenField).Value;
                drGridRow["escalation_level"] = (gvr.FindControl("hdnEscLevel") as HiddenField).Value;
                drGridRow["esc_code"] = (gvr.FindControl("txtEscCode") as TextBox).Text;
                drGridRow["seller_group_code_order"] = (gvr.FindControl("hdnSellerGrpCodeOrder") as HiddenField).Value;
                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                if (territory == string.Empty)
                {
                    drGridRow["seller_group_code"] = Global.DBNullParamValue;
                    drGridRow["seller_group"] = DBNull.Value;
                }
                else
                {
                    drGridRow["seller_group_code"] = territory.Substring(0, territory.IndexOf("-") - 1);
                    drGridRow["seller_group"] = territory;
                }

                if (config == string.Empty)
                {
                    drGridRow["config_group_code"] = Global.DBNullParamValue;
                    drGridRow["config_group"] = DBNull.Value;
                }
                else
                {
                    drGridRow["config_group_code"] = config.Substring(0, config.IndexOf("-") - 1);
                    drGridRow["config_group"] = config;
                }

                if (salesType == string.Empty)
                {
                    drGridRow["price_group_code"] = Global.DBNullParamValue;
                    drGridRow["price_group"] = DBNull.Value;
                }
                else
                {
                    drGridRow["price_group_code"] = salesType.Substring(0, salesType.IndexOf("-") - 1);
                    drGridRow["price_group"] = salesType;
                }

                /*
                if (salesAbove == string.Empty)
                {
                    drGridRow["sales_above"] = 0;
                }
                else
                {
                    drGridRow["sales_above"] = salesAbove;
                }
                 * */

                if (salesTrigger == string.Empty)
                {
                    drGridRow["sales_trigger"] = DBNull.Value;
                }
                else
                {
                    drGridRow["sales_trigger"] = salesTrigger;
                }

                if (valueTrigger == string.Empty)
                {
                    drGridRow["value_trigger"] = DBNull.Value;
                }
                else
                {
                    drGridRow["value_trigger"] = valueTrigger;
                }


                if (salesPct == string.Empty)
                {
                    drGridRow["sales_pct"] = DBNull.Value;
                }
                else
                {
                    drGridRow["sales_pct"] = salesPct;
                }

                if (royaltyRate == string.Empty)
                {
                    drGridRow["royalty_rate"] = DBNull.Value;
                }
                else
                {
                    drGridRow["royalty_rate"] = royaltyRate;
                }

                if (unitRate == string.Empty)
                {
                    drGridRow["unit_rate"] = DBNull.Value;
                }
                else
                {
                    drGridRow["unit_rate"] = unitRate;
                }

                if (revenueRate == string.Empty)
                {
                    drGridRow["revenue_rate"] = DBNull.Value;
                }
                else
                {
                    drGridRow["revenue_rate"] = revenueRate;
                }

                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["RoyContEscRatesGridData"] = dtGridChangedData;

        }

        /// <summary>
        /// WUIN-557 - validation - duplicate rate row
        /// rate is duplicate when esccode, territory, configcode, salestype and sales trigger/value trigger are same
        /// </summary>
        /// <returns></returns>
        private bool ValidateDuplicateRateRow()
        {
            string escCode;
            string territory;
            string config;
            string salesType;            
            string salesTrigger;
            string valueTrigger;

            foreach (GridViewRow gvr in gvEscRates.Rows)
            {                
                escCode = (gvr.FindControl("txtEscCode") as TextBox).Text;
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;                
                salesTrigger = (gvr.FindControl("txtSalesTrigger") as TextBox).Text;
                valueTrigger = (gvr.FindControl("txtValueTrigger") as TextBox).Text;

                if (IsRowExisting(gvr.RowIndex, escCode,territory,config,salesType,salesTrigger,valueTrigger))
                {
                    return true;
                                    
                }
                

            }

            return false;
        }

        /// <summary>
        /// validate if the current row(either a new row added from add row or an existing grid row) is not existing
        /// </summary>
        /// <returns></returns>
        private bool IsRowExisting(int rowIndexCompare, string escCodeCompare, string territoryCompare, 
                                    string configCompare, string salesTypeCompare, string salesTriggerCompare, string valueTriggerCompare)
        {
            bool isRowExisting = false;               
            string territorySelected = string.Empty;
            string configCodeSelected = string.Empty;
            string salesTypeSelected = string.Empty;

            string escCode;
            string territory;
            string config;
            string salesType;
            string salesTrigger;
            string valueTrigger;

            if (territoryCompare != string.Empty)
            {
                territorySelected = territoryCompare.Substring(0, territoryCompare.IndexOf("-") - 1);
            }

            if (configCompare != string.Empty)
            {
                configCodeSelected = configCompare.Substring(0, configCompare.IndexOf("-") - 1);
            }

            if (salesTypeCompare != string.Empty)
            {
                salesTypeSelected = salesTypeCompare.Substring(0, salesTypeCompare.IndexOf("-") - 1);
            }

            foreach (GridViewRow gvr in gvEscRates.Rows)
            {
                if (gvr.RowIndex == rowIndexCompare)
                {
                    continue;
                }

                escCode = (gvr.FindControl("txtEscCode") as TextBox).Text;
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;                
                salesTrigger = (gvr.FindControl("txtSalesTrigger") as TextBox).Text;
                valueTrigger = (gvr.FindControl("txtValueTrigger") as TextBox).Text;
                
                if (territory != string.Empty)
                {
                    territory = territory.Substring(0, territory.IndexOf("-") - 1);
                }

                if (config != string.Empty)
                {
                    config = config.Substring(0, config.IndexOf("-") - 1);
                }

                if (salesType != string.Empty)
                {
                    salesType = salesType.Substring(0, salesType.IndexOf("-") - 1);
                }
                //JIRA-557 Changes -- Start
                if (escCode.ToUpper() == escCodeCompare.ToUpper() && territory == territorySelected && config == configCodeSelected && salesType == salesTypeSelected &&
                    salesTrigger == salesTriggerCompare && valueTrigger == valueTriggerCompare)                    
                {
                    isRowExisting = true;
                    break;
                }
                //JIRA-557 Changes -- End
            }

            return isRowExisting;
        }

        private void ClearAddRow()
        {
            txtEscCodeAddRow.Text = string.Empty;
            txtTerritoryAddRow.Text = string.Empty;
            txtConfigAddRow.Text = string.Empty;
            txtSalesTypeAddRow.Text = string.Empty;
            txtSalesTriggerAddRow.Text = string.Empty;
            txtValueTriggerAddRow.Text = string.Empty;
            txtPctSalesAddRow.Text = string.Empty;
            txtRoyaltyRateAddRow.Text = string.Empty;
            txtUnitRateAddRow.Text = string.Empty;
            txtRevenueRateAddRow.Text = string.Empty;

        }

        private void DeleteRowFromGrid(string profIdToDelete, string escLevelToDelete, string isModified)
        {
            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContEscRatesGridData"];

            //add to delete list only if the row is not a new one 
            List<string> deleteProfileList = new List<string>();
            List<string> deleteRateList = new List<string>();

            if (isModified != Global.DBNullParamValue)
            {
                if (ViewState["vsDeleteRateIds"] != null)
                {
                    deleteRateList = (List<string>)ViewState["vsDeleteRateIds"];
                    deleteRateList.Add(profIdToDelete + ";" + escLevelToDelete);
                }
                else
                {
                    deleteRateList.Add(profIdToDelete + ";" + escLevelToDelete);
                }

                ViewState["vsDeleteRateIds"] = deleteRateList;

            }


            foreach (DataRow dr in dtGridData.Rows)
            {
                if (dr["escalation_profile_id"].ToString() == profIdToDelete && dr["escalation_level"].ToString() == escLevelToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            //add to profile delete list if all the rates of the profile are deleted
            //check if there are any rates for the profile id being deleted. Add to delete profile list if there are not any.
            if (isModified != Global.DBNullParamValue)
            {
                DataRow[] dtRateProfile = dtGridData.Select("escalation_profile_id='" + profIdToDelete + "'");
                if (dtRateProfile.Count() == 0)
                {
                    if (ViewState["vsDeleteProfileIds"] != null)
                    {
                        deleteProfileList = (List<string>)ViewState["vsDeleteProfileIds"];
                        deleteProfileList.Add(profIdToDelete);
                    }
                    else
                    {
                        deleteProfileList.Add(profIdToDelete);
                    }

                    ViewState["vsDeleteProfileIds"] = deleteProfileList;
                }
            }

            DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData);
            Session["RoyContEscRatesGridData"] = dtGridChangedDataSorted;
            gvEscRates.DataSource = dtGridChangedDataSorted;
            gvEscRates.DataBind();

            //delete row from initial grid data session
            DataTable dtOriginalGridData = (DataTable)Session["RoyContEscRatesGridDataInitial"];
            foreach (DataRow dr in dtOriginalGridData.Rows)
            {
                if (dr["escalation_profile_id"].ToString() == profIdToDelete && dr["escalation_level"].ToString() == escLevelToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataTable dtOriginalGridDataSorted = GridDataSorted(dtOriginalGridData); ;
            Session["RoyContEscRatesGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private Array SalesProRataList()
        {
            List<string> salesProRataList = new List<string>();
            string escCode;
            string priceCode;
            string escProRata;

            foreach (GridViewRow gvr in gvSalesCategoryProRata.Rows)
            {
                escCode = (gvr.FindControl("hdnEscCode") as HiddenField).Value;
                priceCode = (gvr.FindControl("lenSalesCategory") as Label).Text;
                escProRata = (gvr.FindControl("txtProRata") as TextBox).Text;

                salesProRataList.Add(escCode + Global.DBDelimiter + priceCode + Global.DBDelimiter + escProRata);
            }


            return salesProRataList.ToArray();
        }

        /// <summary>
        /// WUIN-378
        /// moved this logic to a method so that it can be re used in save button and prorata confirmation popup
        /// </summary>
        private void NotifyPostSavingChanges()
        {
            //new royaltor - redirect to Packaging rates screen
            //existing royaltor - remain in same screen
            if (isNewRoyaltor == "Y")
            {
                //WUIN-450
                //set screen button enabled = Y
                contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.EscalationRates.ToString());

                //redirect in javascript so that issue of data not saved validation would be handled
                ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId.Split('-')[0].Trim() + ");", true);
            }
            else
            {
                msgView.SetMessage("Escalation rates saved", MessageType.Warning, PositionType.Auto);
            }
        }

        private void EnableReadonly()
        {

            btnSave.Enabled = false;
            btnAppendAddRow.Enabled = false;
            btnUndoAddRow.Enabled = false;
            btnSaveProRata.Enabled = false;
            //disable grid buttons
            foreach (GridViewRow gvr in gvEscRates.Rows)
            {
                (gvr.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
            }
        }

        #endregion Methods

        #region Fuzzy Search

        protected void btnFuzzyTerritoryListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllSellerGroupList(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
                lblFuzzySearchPopUp.Text = "Territory - Complete Search List";
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

        protected void btnFuzzyConfigListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllConfigGroupList(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
                lblFuzzySearchPopUp.Text = "Configuration - Complete Search List";
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
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllPriceGroupList(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
                lblFuzzySearchPopUp.Text = "Sales Type - Complete Search List";
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

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Territory")
                {
                    TextBox txtTerritory;
                    foreach (GridViewRow gvr in gvEscRates.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnGridFuzzySearchRowId.Value)
                        {
                            txtTerritory = (gvr.FindControl("txtTerritory") as TextBox);
                            if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                            {
                                txtTerritory.Text = string.Empty;
                                txtTerritory.ToolTip = string.Empty;
                                return;
                            }

                            txtTerritory.Text = lbFuzzySearch.SelectedValue.ToString();
                            txtTerritory.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                            break;
                        }
                    }
                }
                else if (hdnFuzzySearchField.Value == "Config")
                {
                    TextBox txtConfig;
                    foreach (GridViewRow gvr in gvEscRates.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnGridFuzzySearchRowId.Value)
                        {
                            txtConfig = (gvr.FindControl("txtConfig") as TextBox);
                            if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                            {
                                txtConfig.Text = string.Empty;
                                txtConfig.ToolTip = string.Empty;
                                return;
                            }

                            txtConfig.Text = lbFuzzySearch.SelectedValue.ToString();
                            txtConfig.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                            break;
                        }
                    }
                }
                else if (hdnFuzzySearchField.Value == "SalesType")
                {
                    TextBox txtSalesType;
                    foreach (GridViewRow gvr in gvEscRates.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnGridFuzzySearchRowId.Value)
                        {
                            txtSalesType = (gvr.FindControl("txtSalesType") as TextBox);
                            if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                            {
                                txtSalesType.Text = string.Empty;
                                txtSalesType.ToolTip = string.Empty;
                                return;
                            }

                            txtSalesType.Text = lbFuzzySearch.SelectedValue.ToString();
                            txtSalesType.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                            break;
                        }
                    }
                }
                else if (hdnFuzzySearchField.Value == "TerritoryAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtTerritoryAddRow.Text = string.Empty;
                        txtTerritoryAddRow.ToolTip = string.Empty;
                        return;
                    }

                    txtTerritoryAddRow.Text = lbFuzzySearch.SelectedValue.ToString();
                    txtTerritoryAddRow.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                }
                else if (hdnFuzzySearchField.Value == "ConfigAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtConfigAddRow.Text = string.Empty;
                        txtConfigAddRow.ToolTip = string.Empty;
                        return;
                    }

                    txtConfigAddRow.Text = lbFuzzySearch.SelectedValue.ToString();
                    txtConfigAddRow.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                }
                else if (hdnFuzzySearchField.Value == "SalesTypeAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtSalesTypeAddRow.Text = string.Empty;
                        txtSalesTypeAddRow.ToolTip = string.Empty;
                        return;
                    }

                    txtSalesTypeAddRow.Text = lbFuzzySearch.SelectedValue.ToString();
                    txtSalesTypeAddRow.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                }



            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory fuzzy search selection", ex.Message);
            }
        }

        #endregion Fuzzy Search



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