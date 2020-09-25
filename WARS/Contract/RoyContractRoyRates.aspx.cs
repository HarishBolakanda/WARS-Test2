/*
File Name   :   RoyContractRoyRates.cs
Purpose     :   to maintain royalty rates of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     13-Apr-2017     Harish(Infosys Limited)   Initial Creation
 *      29-Jan-2018     Harish                    WUIN-372    
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
    public partial class RoyContractRoyRates : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractRoyRatesBL royContractRoyRatesBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtOptionPeriodList;
        DataTable dtSalesTypeList;
        DataTable dtPriceFieldList;
        string loggedUserID;
        string userRoleID;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        string wildCardChar = ".";
        string orderByTerritory = "seller_group_code_order,seller_group_code, score desc,catno,price_group_code, config_group_code";
        string orderByHierarchy = "score desc,catno,seller_group_code_order,seller_group_code, price_group_code, config_group_code";
        #endregion Global Declarations

        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];

                //royaltorId = "12340";//14838
                //isNewRoyaltor = "N";

                //royaltorId = "55109";
                //isNewRoyaltor = "N";

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Royalty Rates";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Royalty Rates";
                }

                lblTab.Focus();//tabbing sequence starts here                
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //HtmlTableRow trRoyaltyRates = (HtmlTableRow)contractNavigationButtons.FindControl("trRoyaltyRates");
                        //trRoyaltyRates.Visible = false;
                        Button btnRoyRates = (Button)contractNavigationButtons.FindControl("btnRoyRates");
                        btnRoyRates.Enabled = false;
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
                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.RoyaltyRates.ToString());
                            hdnIsNewRoyaltor.Value = "Y";
                        }

                        txtRoyaltorId.Text = royaltorId;

                        //WUIN-285 - Use current selected option period as default for other rates
                        //      check if screen is opened with new royaltor or not and decide to select option period accordingly
                        if (Session["ContractRatesOptPrdSelected"] != null)
                        {
                            string royaltorOptPrd = Convert.ToString(Session["ContractRatesOptPrdSelected"]);
                            string royaltorSelected = royaltorOptPrd.Split(';')[0];
                            string optPrdSelected = royaltorOptPrd.Split(';')[1];

                            if (royaltorId == royaltorSelected)
                            {
                                LoadRoyaltyRatesData(optPrdSelected);
                            }
                            else
                            {
                                LoadRoyaltyRatesData("-");
                            }

                        }
                        else
                        {
                            LoadRoyaltyRatesData("-");
                        }

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
                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Royalty rates not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                Array royRatesList = RoyaltorRatesList();

                List<string> deleteList = new List<string>();
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                }

                //check if any changes to save
                if (royRatesList.Length == 0 && deleteList.Count == 0)
                {
                    if (isNewRoyaltor == "N")
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }
                    else if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.RoyaltyRates.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }

                    return;
                }


                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                userRoleID = Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower()); ;
                string royaltor;
                string invalidCatno;
                royContractRoyRatesBL = new RoyContractRoyRatesBL();
                DataSet royRatesData = royContractRoyRatesBL.SaveRoyaltyRates(royaltorId, ddlOptionPeriod.SelectedValue, loggedUserID, userRoleID, royRatesList, deleteList.ToArray(), out royaltor, out invalidCatno, out errorId);
                royContractRoyRatesBL = null;

                //validate - catno should be valid one from warse.catno table
                if (errorId == 1)
                {
                    msgView.SetMessage("Royalty rates not saved – invalid Catalogue No. - " + invalidCatno, MessageType.Warning, PositionType.Auto);
                    return;
                }

                hdnGridDataDeleted.Value = "N";
                ViewState["vsDeleteIds"] = null;
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (errorId == 0)
                {
                    txtRoyaltorId.Text = royaltor;
                    if (royRatesData.Tables.Count != 0)
                    {
                        DataTable dtGridDataSorted = GridDataSorted(royRatesData.Tables[0]);
                        dtOptionPeriodList = royRatesData.Tables[1];
                        dtSalesTypeList = royRatesData.Tables[2];
                        dtPriceFieldList = royRatesData.Tables[3];
                        Session["RoyContRoyRatesGridDataInitial"] = dtGridDataSorted;
                        Session["RoyContRoyRatesGridData"] = dtGridDataSorted;
                        Session["RoyContRoyRatesOptPeriod"] = dtOptionPeriodList;
                        Session["RoyContRoyRatesSalesType"] = dtSalesTypeList;
                        Session["RoyContRoyRatesPriceFld"] = dtPriceFieldList;
                        Session["RoyContRoyRatesCatno"] = royRatesData.Tables[4];

                        gvRoyaltyRates.DataSource = dtGridDataSorted;
                        gvRoyaltyRates.DataBind();

                        if (royRatesData.Tables[0].Rows.Count == 0)
                        {
                            gvRoyaltyRates.EmptyDataText = "No data found for the selected royaltor and option period";
                        }
                        else
                        {
                            gvRoyaltyRates.EmptyDataText = string.Empty;
                        }

                        ddlPriceFieldAddRow.DataSource = dtPriceFieldList;
                        ddlPriceFieldAddRow.DataTextField = "item_text";
                        ddlPriceFieldAddRow.DataValueField = "item_value";
                        ddlPriceFieldAddRow.DataBind();
                        ddlPriceFieldAddRow.Items.Insert(0, new ListItem("-"));
                        SetAddRowDefaultPriceType();


                    }
                    else if (royRatesData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvRoyaltyRates.DataSource = dtEmpty;
                        gvRoyaltyRates.EmptyDataText = "No data found for the selected royaltor and option period";
                        gvRoyaltyRates.DataBind();
                    }

                    //new royaltor - redirect to Subsidiary rates screen
                    //existing royaltor - remain in same screen
                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.RoyaltyRates.ToString());

                        //redirect in javascript so that issue of data not saved validation would be handled
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Royalty rates saved", MessageType.Warning, PositionType.Auto);
                    }

                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving royalty rates data", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving royalty rates data", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving royalty rates data", ex.Message);
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
                if (ddlOptionPeriod.SelectedIndex > 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "AuditScreen", "RedirectToAuditScreen(" + royaltorId + "," + ddlOptionPeriod.SelectedValue + ");", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "AuditScreen", "RedirectToAuditScreen(" + royaltorId + ",'');", true);
                }
            }
        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "HierarchyOrder")
            {
                btnDisplayOrder_Click(sender, e);
            }
            if (hdnButtonSelection.Value == "OptionPeriod")
            {
                ddlOptionPeriod_SelectedIndexChanged(sender, e);
            }
        }

        protected void gvRoyaltyRates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DropDownList ddlPriceField;
                string priceType;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    ddlPriceField = (e.Row.FindControl("ddlPriceField") as DropDownList);
                    priceType = (e.Row.FindControl("hdnPriceType") as HiddenField).Value;

                    if (dtPriceFieldList != null)
                    {
                        ddlPriceField.DataSource = dtPriceFieldList;
                        ddlPriceField.DataTextField = "item_text";
                        ddlPriceField.DataValueField = "item_value";
                        ddlPriceField.DataBind();
                        ddlPriceField.Items.Insert(0, new ListItem("-"));

                        if (ddlPriceField.Items.FindByValue(priceType) != null)
                        {
                            ddlPriceField.Items.FindByValue(priceType).Selected = true;
                        }
                        else
                        {
                            ddlPriceField.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        ddlPriceField.Items.Insert(0, new ListItem("-"));
                        ddlPriceField.SelectedIndex = 0;
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


        protected void btnAppendAddRow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (IsRowExisting())
                {
                    msgView.SetMessage("Rate already exists", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (ddlOptionPeriod.SelectedValue == "-")
                {
                    msgView.SetMessage("Please select valid option period to add the rates", MessageType.Warning, PositionType.Auto);
                    return;
                }

                AppendRowToGrid();
                ClearAddRow();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding option period row to grid", ex.Message);
            }
        }

        protected void btnDisplayOrder_Click(object sender, EventArgs e)
        {
            if (hdnDefaultDisplayOrder.Value == "Y")
            {
                btnDisplayOrder.Text = "Hierarchy with Territory Order";
                hdnDefaultDisplayOrder.Value = "N";
            }
            else
            {
                btnDisplayOrder.Text = "Hierarchy Order";
                hdnDefaultDisplayOrder.Value = "Y";
            }

            ReOrderGridData();
        }

        protected void btnConfirmOptChange_Click(object sender, EventArgs e)
        {
            LoadRoyaltyRatesData(ddlOptionPeriod.SelectedValue);
        }

        //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string rateId = Convert.ToString(hdnDeleteRateId.Value);
                string isModified = Convert.ToString(hdnDeleteIsModified.Value);
                DeleteRowFromGrid(rateId, isModified);
                hdnDeleteRateId.Value = "";
                hdnDeleteIsModified.Value = "";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting grid data", ex.Message);
            }
        }
        //JIRA-908 Changes by Ravi on 13/02/2019 -- End

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvRoyaltyRates_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyContRoyRatesGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                AssignGridDropdownList();
                gvRoyaltyRates.DataSource = dataView;
                gvRoyaltyRates.DataBind();
                Session["RoyContRoyRatesGridData"] = dataView.ToTable();
            }
            //WUIN-1096 -  ReadOnlyUser
            if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()))
            {
                EnableReadonly();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void ddlOptionPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //WUIN-285 - Use current selected option period as default for other rates
                //        hold selected royaltorId and  option period in a session                        
                Session["ContractRatesOptPrdSelected"] = royaltorId + ";" + ddlOptionPeriod.SelectedValue.ToString();

                LoadRoyaltyRatesData(ddlOptionPeriod.SelectedValue);
                if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
                {
                    EnableReadonly();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting option period.", ex.Message);
            }
        }

        #endregion EVENTS
        
        #region METHODS
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void LoadRoyaltyRatesData(string optionPeriod)
        {
            string royaltor;
            userRoleID = Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower()); ;
            royContractRoyRatesBL = new RoyContractRoyRatesBL();
            DataSet royRatesData = royContractRoyRatesBL.GetRoyaltyRatesData(royaltorId, optionPeriod, userRoleID, out royaltor, out errorId);
            royContractRoyRatesBL = null;

            if (royRatesData.Tables.Count != 0 && errorId != 2)
            {
                txtRoyaltorId.Text = royaltor;
                dtOptionPeriodList = royRatesData.Tables[1];
                dtSalesTypeList = royRatesData.Tables[2];
                dtPriceFieldList = royRatesData.Tables[3];
                Session["RoyContRoyRatesGridDataInitial"] = royRatesData.Tables[0];
                Session["RoyContRoyRatesGridData"] = royRatesData.Tables[0];
                Session["RoyContRoyRatesOptPeriod"] = dtOptionPeriodList;
                Session["RoyContRoyRatesSalesType"] = dtSalesTypeList;
                Session["RoyContRoyRatesPriceFld"] = dtPriceFieldList;
                Session["RoyContRoyRatesCatno"] = royRatesData.Tables[4];

                ddlOptionPeriod.DataSource = dtOptionPeriodList;
                ddlOptionPeriod.DataTextField = "item_text";
                ddlOptionPeriod.DataValueField = "item_value";
                ddlOptionPeriod.DataBind();
                ddlOptionPeriod.Items.Insert(0, new ListItem("-"));

                //on initial load populate option period dropdown with minimum/first option period code
                if (optionPeriod == "-")
                {
                    if (dtOptionPeriodList.Rows.Count > 0)
                    {
                        int minOptionPeriodCode = Convert.ToInt32(dtOptionPeriodList.Compute("min([item_value])", string.Empty));
                        ddlOptionPeriod.SelectedValue = minOptionPeriodCode.ToString();

                        //WUIN-285 - Use current selected option period as default for other rates
                        //        hold selected royaltorId and  option period in a session                        
                        Session["ContractRatesOptPrdSelected"] = royaltorId + ";" + minOptionPeriodCode.ToString();

                        //set default price type code for the selected option period
                        DataRow[] dtDefaultPriceType = dtOptionPeriodList.Select("item_value='" + minOptionPeriodCode + "'");
                        if (dtDefaultPriceType.Count() > 0)
                        {
                            hdnDefaultPriceType.Value = dtDefaultPriceType[0]["price_type"].ToString();
                        }
                        else
                        {
                            hdnDefaultPriceType.Value = string.Empty;
                        }

                    }
                    else
                    {
                        //set default price type code to null if option periods are not present for the royaltor
                        hdnDefaultPriceType.Value = string.Empty;
                    }

                }
                else
                {
                    ddlOptionPeriod.SelectedValue = optionPeriod;

                    //set default price type code for the selected option period
                    //WUIN-598 - change
                    //DataRow[] dtDefaultPriceType = dtOptionPeriodList.Select("item_value=" + optionPeriod);
                    DataRow[] dtDefaultPriceType = dtOptionPeriodList.Select("item_value='" + optionPeriod + "'");
                    if (dtDefaultPriceType.Count() > 0)
                    {
                        hdnDefaultPriceType.Value = dtDefaultPriceType[0]["price_type"].ToString();
                    }
                    else
                    {
                        hdnDefaultPriceType.Value = string.Empty;
                    }
                }

                ddlPriceFieldAddRow.DataSource = dtPriceFieldList;
                ddlPriceFieldAddRow.DataTextField = "item_text";
                ddlPriceFieldAddRow.DataValueField = "item_value";
                ddlPriceFieldAddRow.DataBind();
                ddlPriceFieldAddRow.Items.Insert(0, new ListItem("-"));
                SetAddRowDefaultPriceType();

                if (royRatesData.Tables[0].Rows.Count == 0)
                {
                    gvRoyaltyRates.DataSource = royRatesData.Tables[0];
                    gvRoyaltyRates.EmptyDataText = "No data found for the selected royaltor and option period";
                    gvRoyaltyRates.DataBind();
                }
                else
                {
                    gvRoyaltyRates.DataSource = royRatesData.Tables[0];
                    gvRoyaltyRates.DataBind();
                }

            }
            else if (royRatesData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyaltyRates.DataSource = dtEmpty;
                gvRoyaltyRates.EmptyDataText = "No data found for the selected royaltor";
                gvRoyaltyRates.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }

            hdnOptPeriodSelected.Value = ddlOptionPeriod.SelectedValue;
            hdnGridDataDeleted.Value = "N";
            ViewState["vsDeleteIds"] = null;
        }

        private Array RoyaltorRatesList()
        {
            if (Session["RoyContRoyRatesGridDataInitial"] == null)
            {
                ExceptionHandler("Error in saving royalty rates data", string.Empty);
            }

            DataTable royRatesData = (DataTable)Session["RoyContRoyRatesGridDataInitial"];
            List<string> royRatesList = new List<string>();
            string rateId;
            string territory;
            string catNo;
            string config;
            string salesType;
            string salesPct;
            string royaltyRate;
            string unitRate;
            string isModified;
            DropDownList ddlPriceField;

            foreach (GridViewRow gvr in gvRoyaltyRates.Rows)
            {
                rateId = (gvr.FindControl("hdnRateId") as HiddenField).Value;
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                catNo = (gvr.FindControl("txtCatNo") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;
                salesPct = (gvr.FindControl("txtSalesPct") as TextBox).Text;
                royaltyRate = (gvr.FindControl("txtRoyaltyRate") as TextBox).Text;
                unitRate = (gvr.FindControl("txtUnitRate") as TextBox).Text;
                ddlPriceField = (gvr.FindControl("ddlPriceField") as DropDownList);
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                territory = (territory == string.Empty ? wildCardChar : territory.Substring(0, territory.IndexOf("-") - 1));
                config = (config == string.Empty ? wildCardChar : config.Substring(0, config.IndexOf("-") - 1));
                salesType = (salesType == string.Empty ? wildCardChar : salesType.Substring(0, salesType.IndexOf("-") - 1));

                if (isModified == Global.DBNullParamValue)//new row
                {
                    isModified = Global.DBNullParamValue;
                    royRatesList.Add(rateId + "(;||;)" + ddlOptionPeriod.SelectedValue + "(;||;)" + territory + "(;||;)" +
                                        (catNo == string.Empty ? Global.DBNullParamValue : catNo.ToUpper()) + "(;||;)" +
                                        config + "(;||;)" + salesType + "(;||;)" +
                                        (salesPct == string.Empty ? Global.DBNullParamValue : salesPct) + "(;||;)" +
                                        (royaltyRate == string.Empty ? Global.DBNullParamValue : royaltyRate) + "(;||;)" +
                                        (unitRate == string.Empty ? Global.DBNullParamValue : unitRate) + "(;||;)" +
                                        (ddlPriceField.SelectedValue) + "(;||;)" + isModified);
                }
                else
                {
                    DataTable dtRoyRate = royRatesData.Select("standard_rate_id=" + rateId).CopyToDataTable();
                    if (dtRoyRate.Rows.Count != 0)
                    {
                        if ((territory == string.Empty ? wildCardChar : territory) != dtRoyRate.Rows[0]["seller_group_code"].ToString() || catNo.ToUpper() != dtRoyRate.Rows[0]["catno"].ToString()
                                || (config == string.Empty ? wildCardChar : config) != dtRoyRate.Rows[0]["config_group_code"].ToString()
                                || (salesType == string.Empty ? wildCardChar : salesType) != dtRoyRate.Rows[0]["price_group_code"].ToString()
                                || salesPct != dtRoyRate.Rows[0]["sales_pct"].ToString()
                                || royaltyRate != (dtRoyRate.Rows[0]["royalty_rate"].ToString() == "" ? "" : Convert.ToDouble(dtRoyRate.Rows[0]["royalty_rate"]).ToString())  //WUIN-1154
                                || unitRate != (dtRoyRate.Rows[0]["unit_rate"].ToString() == "" ? "" : Convert.ToDouble(dtRoyRate.Rows[0]["unit_rate"]).ToString())
                                || ddlPriceField.SelectedValue != dtRoyRate.Rows[0]["price_type"].ToString()
                                 )//existing row - if data changed 
                        {
                            isModified = "Y";
                            royRatesList.Add(rateId + "(;||;)" + ddlOptionPeriod.SelectedValue + "(;||;)" + territory + "(;||;)" +
                                        (catNo.ToUpper() == string.Empty ? Global.DBNullParamValue : catNo.ToUpper()) + "(;||;)" +
                                        config + "(;||;)" + salesType + "(;||;)" +
                                        (salesPct == string.Empty ? Global.DBNullParamValue : salesPct) + "(;||;)" +
                                        (royaltyRate == string.Empty ? Global.DBNullParamValue : royaltyRate) + "(;||;)" +
                                        (unitRate == string.Empty ? Global.DBNullParamValue : unitRate) + "(;||;)" +
                                        (ddlPriceField.SelectedValue) + "(;||;)" + isModified);
                        }
                    }
                }


            }

            return royRatesList.ToArray();
        }

        private DataTable GridDataSorted(DataTable inputData)
        {
            DataView dv = inputData.DefaultView;
            if (hdnDefaultDisplayOrder.Value == "Y")
            {
                dv.Sort = orderByTerritory;
            }
            else
            {
                dv.Sort = orderByHierarchy;
            }
            DataTable dtSorted = dv.ToTable();

            return dtSorted;

        }

        private void AppendRowToGrid()
        {
            if (Session["RoyContRoyRatesGridData"] == null || Session["RoyContRoyRatesGridDataInitial"] == null)
            {
                ExceptionHandler("Error in adding royalty rate row to grid", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContRoyRatesGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyContRoyRatesGridDataInitial"];
            DataRow drNewRow = dtGridData.NewRow();
            DataRow drNewRowOriginal = dtOriginalGridData.NewRow();
            if (hdnRateIdAddRow.Value == string.Empty)
            {
                drNewRow["standard_rate_id"] = -1;
                hdnRateIdAddRow.Value = "-1";
            }
            else
            {
                int rateId = Convert.ToInt32(hdnRateIdAddRow.Value);
                rateId = rateId - 1;
                drNewRow["standard_rate_id"] = rateId;
                hdnRateIdAddRow.Value = rateId.ToString();
            }

            if (txtTerritoryAddRow.Text == string.Empty)
            {
                drNewRow["seller_group_code"] = DBNull.Value;
                drNewRow["seller_group"] = DBNull.Value;
                drNewRow["seller_group_code_order"] = "1";//to sort data for wildcard to display bottom
            }
            else
            {
                drNewRow["seller_group_code"] = txtTerritoryAddRow.Text.Substring(0, txtTerritoryAddRow.Text.IndexOf("-") - 1);
                drNewRow["seller_group"] = txtTerritoryAddRow.Text;
                drNewRow["seller_group_code_order"] = "0";//to sort data for wildcard to display bottom
            }

            if (txtCatNoAddRow.Text == string.Empty)
            {
                drNewRow["catno"] = DBNull.Value;
            }
            else
            {
                drNewRow["catno"] = txtCatNoAddRow.Text;
            }

            if (txtConfigAddRow.Text == string.Empty)
            {
                drNewRow["config_group_code"] = DBNull.Value;
                drNewRow["config_group"] = DBNull.Value;
            }
            else
            {
                drNewRow["config_group_code"] = txtConfigAddRow.Text.Substring(0, txtConfigAddRow.Text.IndexOf("-") - 1);
                drNewRow["config_group"] = txtConfigAddRow.Text;
            }

            if (txtSalesTypeAddRow.Text == string.Empty)
            {
                drNewRow["price_group_code"] = DBNull.Value;
                drNewRow["price_group"] = DBNull.Value;
            }
            else
            {
                drNewRow["price_group_code"] = txtSalesTypeAddRow.Text.Substring(0, txtSalesTypeAddRow.Text.IndexOf("-") - 1);
                drNewRow["price_group"] = txtSalesTypeAddRow.Text;
            }

            if (txtSalesPctAddRow.Text == string.Empty)
            {
                drNewRow["sales_pct"] = DBNull.Value;
            }
            else
            {
                drNewRow["sales_pct"] = txtSalesPctAddRow.Text;
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

            drNewRow["price_type"] = ddlPriceFieldAddRow.SelectedValue;
            drNewRow["score"] = 0;
            drNewRow["is_modified"] = Global.DBNullParamValue;

            dtGridData.Rows.Add(drNewRow);
            DataTable dtGridChangedDataSorted = dtGridData; //JIRA-451 Changes
            Session["RoyContRoyRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvRoyaltyRates.DataSource = dtGridChangedDataSorted;
            gvRoyaltyRates.DataBind();
            txtTerritoryAddRow.Focus();//JIRA-451 CHanges

            drNewRowOriginal.ItemArray = drNewRow.ItemArray.Clone() as object[];
            dtOriginalGridData.Rows.Add(drNewRowOriginal);
            DataTable dtOriginalGridDataSorted = GridDataSorted(dtOriginalGridData);
            Session["RoyContRoyRatesGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private void ClearAddRow()
        {
            txtTerritoryAddRow.Text = string.Empty;
            txtCatNoAddRow.Text = string.Empty;
            txtConfigAddRow.Text = string.Empty;
            txtSalesTypeAddRow.Text = string.Empty;
            txtSalesPctAddRow.Text = "100";
            txtRoyaltyRateAddRow.Text = string.Empty;
            txtUnitRateAddRow.Text = string.Empty;
            SetAddRowDefaultPriceType();

        }

        /// <summary>
        /// set default price type as the price type for the selected option period
        /// </summary>
        private void SetAddRowDefaultPriceType()
        {
            if (hdnDefaultPriceType.Value != string.Empty)
            {
                ddlPriceFieldAddRow.SelectedValue = hdnDefaultPriceType.Value.ToString();
            }
            else
            {
                ddlPriceFieldAddRow.SelectedIndex = 1;
            }

        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["RoyContRoyRatesGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            string territory;
            string config;
            string salesType;
            string unitRate;
            string royaltyRate;
            string salesPct;

            foreach (GridViewRow gvr in gvRoyaltyRates.Rows)
            {
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;
                unitRate = (gvr.FindControl("txtUnitRate") as TextBox).Text;
                royaltyRate = (gvr.FindControl("txtRoyaltyRate") as TextBox).Text;
                salesPct = (gvr.FindControl("txtSalesPct") as TextBox).Text;

                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["standard_rate_id"] = (gvr.FindControl("hdnRateId") as HiddenField).Value;
                drGridRow["catno"] = (gvr.FindControl("txtCatNo") as TextBox).Text;
                drGridRow["price_type"] = (gvr.FindControl("ddlPriceField") as DropDownList).SelectedValue;
                drGridRow["seller_group_code_order"] = (gvr.FindControl("hdnSellerGrpCodeOrder") as HiddenField).Value;
                drGridRow["score"] = (gvr.FindControl("hdnScore") as HiddenField).Value;
                drGridRow["is_modified"] = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                if (territory == string.Empty)
                {
                    drGridRow["seller_group_code"] = DBNull.Value;
                    drGridRow["seller_group"] = DBNull.Value;
                }
                else
                {
                    drGridRow["seller_group_code"] = territory.Substring(0, territory.IndexOf("-") - 1);
                    drGridRow["seller_group"] = territory;
                }

                if (config == string.Empty)
                {
                    drGridRow["config_group_code"] = DBNull.Value;
                    drGridRow["config_group"] = DBNull.Value;
                }
                else
                {
                    drGridRow["config_group_code"] = config.Substring(0, config.IndexOf("-") - 1);
                    drGridRow["config_group"] = config;
                }

                if (salesType == string.Empty)
                {
                    drGridRow["price_group_code"] = DBNull.Value;
                    drGridRow["price_group"] = DBNull.Value;
                }
                else
                {
                    drGridRow["price_group_code"] = salesType.Substring(0, salesType.IndexOf("-") - 1);
                    drGridRow["price_group"] = salesType;
                }


                if (unitRate == string.Empty)
                {
                    drGridRow["unit_rate"] = DBNull.Value;
                }
                else
                {
                    drGridRow["unit_rate"] = unitRate;
                }

                if (royaltyRate == string.Empty)
                {
                    drGridRow["royalty_rate"] = DBNull.Value;
                }
                else
                {
                    drGridRow["royalty_rate"] = royaltyRate;
                }

                if (salesPct == string.Empty)
                {
                    drGridRow["sales_pct"] = DBNull.Value;
                }
                else
                {
                    drGridRow["sales_pct"] = salesPct;
                }

                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["RoyContRoyRatesGridData"] = dtGridChangedData;

        }

        private void DeleteRowFromGrid(string rateIdToDelete, string isModified)
        {
            GetGridData();
            //add to delete list only if the row is not a new one 
            List<string> deleteList = new List<string>();
            if (isModified != Global.DBNullParamValue)
            {
                if (Convert.ToInt32(rateIdToDelete) > 0)
                {
                    if (ViewState["vsDeleteIds"] != null)
                    {
                        deleteList = (List<string>)ViewState["vsDeleteIds"];
                        deleteList.Add(rateIdToDelete);
                    }
                    else
                    {
                        deleteList.Add(rateIdToDelete);
                    }

                    ViewState["vsDeleteIds"] = deleteList;

                }
            }


            DataTable dtGridData = (DataTable)Session["RoyContRoyRatesGridData"];
            foreach (DataRow dr in dtGridData.Rows)
            {
                if (dr["standard_rate_id"].ToString() == rateIdToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData);
            Session["RoyContRoyRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvRoyaltyRates.DataSource = dtGridChangedDataSorted;
            gvRoyaltyRates.DataBind();

            //delete row from initial grid data session
            DataTable dtOriginalGridData = (DataTable)Session["RoyContRoyRatesGridDataInitial"];
            foreach (DataRow dr in dtOriginalGridData.Rows)
            {
                if (dr["standard_rate_id"].ToString() == rateIdToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataTable dtOriginalGridDataSorted = GridDataSorted(dtOriginalGridData);
            Session["RoyContRoyRatesGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private void AssignGridDropdownList()
        {
            dtOptionPeriodList = (DataTable)Session["RoyContRoyRatesOptPeriod"];
            dtSalesTypeList = (DataTable)Session["RoyContRoyRatesSalesType"];
            dtPriceFieldList = (DataTable)Session["RoyContRoyRatesPriceFld"];
        }

        /// <summary>
        /// validate if the new row being added is not existing
        /// </summary>
        /// <returns></returns>
        private bool IsRowExisting()
        {
            bool isRowExisting = false;
            string territory;
            string salesType;
            string config;
            string catNo;
            string territorySelected = string.Empty;
            string configCodeSelected = string.Empty;
            string salesTypeSelected = string.Empty;

            if (txtTerritoryAddRow.Text != string.Empty)
            {
                territorySelected = txtTerritoryAddRow.Text.Substring(0, txtTerritoryAddRow.Text.IndexOf("-") - 1);
            }

            if (txtConfigAddRow.Text != string.Empty)
            {
                configCodeSelected = txtConfigAddRow.Text.Substring(0, txtConfigAddRow.Text.IndexOf("-") - 1);
            }

            if (txtSalesTypeAddRow.Text != string.Empty)
            {
                salesTypeSelected = txtSalesTypeAddRow.Text.Substring(0, txtSalesTypeAddRow.Text.IndexOf("-") - 1);
            }

            foreach (GridViewRow gvr in gvRoyaltyRates.Rows)
            {
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;
                catNo = (gvr.FindControl("txtCatNo") as TextBox).Text;

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


                if (territory == territorySelected && salesType == salesTypeSelected && config == configCodeSelected &&
                    catNo == txtCatNoAddRow.Text)
                {
                    isRowExisting = true;
                    break;
                }

            }

            return isRowExisting;
        }

        private void ReOrderGridData()
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (Session["RoyContRoyRatesGridData"] == null)
            {
                ExceptionHandler("Error in re-ordering rates data", string.Empty);
                return;
            }

            DataTable dtGridData = (DataTable)Session["RoyContRoyRatesGridData"];
            DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData);
            Session["RoyContRoyRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvRoyaltyRates.DataSource = dtGridChangedDataSorted;
            gvRoyaltyRates.DataBind();

        }

        private void EnableReadonly()
        {
            btnSave.Enabled = false;
            btnAppendAddRow.Enabled = false;
            btnUndoAddRow.Enabled = false;

            //disable grid buttons
            foreach (GridViewRow gvr in gvRoyaltyRates.Rows)
            {
                (gvr.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
            }
        }

        #endregion METHODS

        #region Validations
        protected void valtxtCatNoAddRow_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (txtCatNoAddRow.Text != string.Empty)
                {
                    DataTable dtCatno = (DataTable)Session["RoyContRoyRatesCatno"];
                    DataRow[] isCatnoValid = dtCatno.Select("catno='" + txtCatNoAddRow.Text + "'");
                    if (isCatnoValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                }
                else
                {
                    args.IsValid = true;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in catno validation", ex.Message);
            }
        }

        protected void valtxtCatNo_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                GridViewRow gvr = (GridViewRow)(source as Control).Parent.Parent;
                string catno = (gvr.FindControl("txtCatNo") as TextBox).Text;

                if (catno != string.Empty)
                {
                    DataTable dtCatno = (DataTable)Session["RoyContRoyRatesCatno"];
                    DataRow[] isCatnoValid = dtCatno.Select("catno='" + catno + "'");
                    if (isCatnoValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                }
                else
                {
                    args.IsValid = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in catno validation", ex.Message);
            }
        }
        #endregion Validations

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
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchContRoyRatesSalesTypeList(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
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
                    foreach (GridViewRow gvr in gvRoyaltyRates.Rows)
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
                    foreach (GridViewRow gvr in gvRoyaltyRates.Rows)
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
                    foreach (GridViewRow gvr in gvRoyaltyRates.Rows)
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