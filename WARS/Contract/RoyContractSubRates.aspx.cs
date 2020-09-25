/*
File Name   :   RoyContractSubRates.cs
Purpose     :   to maintain subsidiary rates of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     03-May-2017     Harish(Infosys Limited)   Initial Creation
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
    public partial class RoyContractSubRates : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractSubRatesBL royContractSubRatesBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtOptionPeriodList;
        DataTable dtSalesTypeList;
        string loggedUserID;
        string userRoleID;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        string wildCardChar = ".";
        string orderByTerritory = "seller_group_code_order,seller_group_code, score desc,option_period_code,config_group_code,price_group_code";
        string orderByHierarchy = "score desc,option_period_code,seller_group_code_order,seller_group_code,config_group_code,price_group_code";
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

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Subsidiary Rates";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Subsidiary Rates";
                }

                lblTab.Focus();//tabbing sequence starts here                
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //HtmlTableRow trSubsidiaryRates = (HtmlTableRow)contractNavigationButtons.FindControl("trSubsidiaryRates");
                        //trSubsidiaryRates.Visible = false;
                        Button btnSubsidRates = (Button)contractNavigationButtons.FindControl("btnSubsidRates");
                        btnSubsidRates.Enabled = false;
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
                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.SubsidiaryRates.ToString());
                        }

                        txtRoyaltorId.Text = royaltorId;
                        LoadSubsidRatesData();

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
                Array subsidRatesList = SusidRatesList();
                List<string> deleteList = new List<string>();
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                }

                //check if any changes to save
                if (subsidRatesList.Length == 0 && deleteList.Count == 0)
                {
                    if (isNewRoyaltor == "N")
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }
                    else if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.SubsidiaryRates.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }

                    return;
                }

                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Subsidiary rates not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }


                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                userRoleID = Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower());
                string royaltor;
                royContractSubRatesBL = new RoyContractSubRatesBL();
                DataSet subRatesData = royContractSubRatesBL.SaveSubsidRates(royaltorId, loggedUserID, userRoleID, subsidRatesList, deleteList.ToArray(), out royaltor, out errorId);
                royContractSubRatesBL = null;
                hdnGridDataDeleted.Value = "N";
                ViewState["vsDeleteIds"] = null;
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (errorId == 0)
                {
                    txtRoyaltorId.Text = royaltor;
                    if (subRatesData.Tables.Count != 0)
                    {
                        DataTable dtGridDataSorted = GridDataSorted(subRatesData.Tables[0]);
                        dtOptionPeriodList = subRatesData.Tables[1];
                        dtSalesTypeList = subRatesData.Tables[2];
                        Session["RoyContSubRatesGridDataInitial"] = dtGridDataSorted;
                        Session["RoyContSubRatesGridData"] = dtGridDataSorted;
                        Session["RoyContSubRatesOptPeriod"] = dtOptionPeriodList;
                        Session["RoyContSubRatesSalesType"] = dtSalesTypeList;

                        gvSubRates.DataSource = subRatesData.Tables[0];
                        gvSubRates.DataBind();

                        if (subRatesData.Tables[0].Rows.Count == 0)
                        {
                            gvSubRates.EmptyDataText = "No data found for the selected royaltor and option period";
                        }
                        else
                        {
                            gvSubRates.EmptyDataText = string.Empty;
                        }

                        ddlOptionPeriodAddRow.DataSource = dtOptionPeriodList;
                        ddlOptionPeriodAddRow.DataTextField = "item_text";
                        ddlOptionPeriodAddRow.DataValueField = "item_value";
                        ddlOptionPeriodAddRow.DataBind();
                        ddlOptionPeriodAddRow.Items.Insert(0, new ListItem("-"));


                    }
                    else if (subRatesData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvSubRates.DataSource = dtEmpty;
                        gvSubRates.EmptyDataText = "No data found for the selected royaltor";
                        gvSubRates.DataBind();
                    }

                    //new royaltor - redirect to Royalty rates screen
                    //existing royaltor - remain in same screen
                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.SubsidiaryRates.ToString());

                        //redirect to Bank details screen                                                          
                        //redirect in javascript so that issue of data not saved validation would be handled
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId.Split('-')[0].Trim() + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Subsidiary rates saved", MessageType.Warning, PositionType.Auto);
                    }

                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving subsidiary rates data", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving subsidiary rates data", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving subsidiary rates data", ex.Message);
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
            if (hdnButtonSelection.Value == "HierarchyOrder")
            {
                btnDisplayOrder_Click(sender, e);
            }
        }

        protected void gvSubRates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DropDownList ddlOptionPeriod;
                string optionPeriodCode;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlOptionPeriod = (e.Row.FindControl("ddlOptionPeriod") as DropDownList);
                    optionPeriodCode = (e.Row.FindControl("hdnOptionPeriod") as HiddenField).Value;

                    if (dtOptionPeriodList != null)
                    {
                        ddlOptionPeriod.DataSource = dtOptionPeriodList;
                        ddlOptionPeriod.DataTextField = "item_text";
                        ddlOptionPeriod.DataValueField = "item_value";
                        ddlOptionPeriod.DataBind();
                        ddlOptionPeriod.Items.Insert(0, new ListItem("-"));

                        if (ddlOptionPeriod.Items.FindByValue(optionPeriodCode) != null)
                        {
                            ddlOptionPeriod.Items.FindByValue(optionPeriodCode).Selected = true;
                        }
                        else
                        {
                            ddlOptionPeriod.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                        ddlOptionPeriod.SelectedIndex = 0;
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

        protected void btnAppendAddRow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                if (IsRowExisting())
                {
                    msgView.SetMessage("Rate already exists", MessageType.Warning, PositionType.Auto);
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

        protected void btnUndoAddRow_Click(object sender, ImageClickEventArgs e)
        {
            ClearAddRow();
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
        protected void gvSubRates_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyContSubRatesGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                AssignGridDropdownList();

                gvSubRates.DataSource = dataView;
                gvSubRates.DataBind();
                Session["RoyContSubRatesGridData"] = dataView.ToTable();
            }
            //WUIN-1096 -  ReadOnlyUser
            if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()))
            {
                EnableReadonly();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End


        #endregion EVENTS

        #region METHODS
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void LoadSubsidRatesData()
        {
            userRoleID = Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower());
            string royaltor;
            royContractSubRatesBL = new RoyContractSubRatesBL();
            DataSet subRatesData = royContractSubRatesBL.GetSubsidRatesData(royaltorId, userRoleID, out royaltor, out errorId);
            royContractSubRatesBL = null;

            if (subRatesData.Tables.Count != 0 && errorId != 2)
            {
                txtRoyaltorId.Text = royaltor;
                dtOptionPeriodList = subRatesData.Tables[1];
                dtSalesTypeList = subRatesData.Tables[2];
                Session["RoyContSubRatesGridDataInitial"] = subRatesData.Tables[0];
                Session["RoyContSubRatesGridData"] = subRatesData.Tables[0];
                Session["RoyContSubRatesOptPeriod"] = dtOptionPeriodList;
                Session["RoyContSubRatesSalesType"] = dtSalesTypeList;

                ddlOptionPeriodAddRow.DataSource = dtOptionPeriodList;
                ddlOptionPeriodAddRow.DataTextField = "item_text";
                ddlOptionPeriodAddRow.DataValueField = "item_value";
                ddlOptionPeriodAddRow.DataBind();
                ddlOptionPeriodAddRow.Items.Insert(0, new ListItem("-"));

                if (subRatesData.Tables[0].Rows.Count == 0)
                {
                    gvSubRates.EmptyDataText = "No data found for the selected royaltor";
                }

                gvSubRates.DataSource = subRatesData.Tables[0];
                gvSubRates.DataBind();

            }
            else if (subRatesData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvSubRates.DataSource = dtEmpty;
                gvSubRates.EmptyDataText = "No data found for the selected royaltor";
                gvSubRates.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }

            hdnGridDataDeleted.Value = "N";
            ViewState["vsDeleteIds"] = null;
        }

        private Array SusidRatesList()
        {
            if (Session["RoyContSubRatesGridDataInitial"] == null)
            {
                ExceptionHandler("Error in saving subsidiary rates data", string.Empty);
            }

            DataTable subRatesData = (DataTable)Session["RoyContSubRatesGridDataInitial"];
            List<string> subRatesList = new List<string>();
            string rateId;
            string territory;
            string config;
            string salesType;
            string revenueRate;
            string royaltyRate;
            string isModified;
            DropDownList ddlOptionPeriod;

            foreach (GridViewRow gvr in gvSubRates.Rows)
            {
                rateId = (gvr.FindControl("hdnRateId") as HiddenField).Value;
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;
                revenueRate = (gvr.FindControl("txtRevenueRate") as TextBox).Text;
                royaltyRate = (gvr.FindControl("txtRoyaltyRate") as TextBox).Text;
                ddlOptionPeriod = (gvr.FindControl("ddlOptionPeriod") as DropDownList);
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                //WUIN - 1106 Commenting below code as it is not working as expected when territory code/config code/salesType code contains '-' 
                //territory = (territory == string.Empty ? wildCardChar : territory.Substring(0, territory.IndexOf("-") - 1));
                //config = (config == string.Empty ? wildCardChar : config.Substring(0, config.IndexOf("-") - 1));
                //salesType = (salesType == string.Empty ? wildCardChar : salesType.Substring(0, salesType.IndexOf("-") - 1));

                //WUIN - 1106 -  Splitting upto first " - " (hyphen with spaces both sides) and replacing spaces with \0 for comparing.
                territory = territory.Replace(" ", Global.DBDelimiter);
                territory = (territory == string.Empty ? wildCardChar : territory.Substring(0, territory.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                territory = territory.Replace(Global.DBDelimiter, " ");

                config = config.Replace(" ", Global.DBDelimiter);
                config = (config == string.Empty ? wildCardChar : config.Substring(0, config.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                config = config.Replace(Global.DBDelimiter, " ");

                salesType = salesType.Replace(" ", Global.DBDelimiter);
                salesType = (salesType == string.Empty ? wildCardChar : salesType.Substring(0, salesType.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                salesType = salesType.Replace(Global.DBDelimiter, " ");

                if (isModified == Global.DBNullParamValue)//new row
                {
                    isModified = Global.DBNullParamValue;
                    subRatesList.Add(rateId + "(;||;)" + (ddlOptionPeriod.SelectedIndex == 0 ? Global.DBNullParamValue : ddlOptionPeriod.SelectedValue) + "(;||;)" +
                                        territory + "(;||;)" + config + "(;||;)" + salesType + "(;||;)" +
                                        (revenueRate == string.Empty ? Global.DBNullParamValue : revenueRate) + "(;||;)" +
                                        (royaltyRate == string.Empty ? Global.DBNullParamValue : royaltyRate) + "(;||;)" +
                                         isModified);

                }
                else
                {
                    DataTable dtSubRate = subRatesData.Select("subsid_rate_id=" + rateId).CopyToDataTable();
                    if (dtSubRate.Rows.Count != 0)
                    {
                        if ((ddlOptionPeriod.SelectedIndex == 0 ? wildCardChar : ddlOptionPeriod.SelectedValue) != dtSubRate.Rows[0]["option_period_code"].ToString()
                                || (territory == string.Empty ? wildCardChar : territory) != dtSubRate.Rows[0]["seller_group_code"].ToString()
                                || (config == string.Empty ? wildCardChar : config) != dtSubRate.Rows[0]["config_group_code"].ToString()
                                || (salesType == string.Empty ? wildCardChar : salesType) != dtSubRate.Rows[0]["price_group_code"].ToString()
                                || revenueRate != (dtSubRate.Rows[0]["revenue_rate"].ToString() == "" ? "" : Convert.ToDouble(dtSubRate.Rows[0]["revenue_rate"]).ToString()) //WUIN-1154
                                || royaltyRate != (dtSubRate.Rows[0]["royalty_rate"].ToString() == "" ? "" : Convert.ToDouble(dtSubRate.Rows[0]["royalty_rate"]).ToString()))//existing row - if data changed 
                        {
                            isModified = "Y";
                            subRatesList.Add(rateId + "(;||;)" + (ddlOptionPeriod.SelectedIndex == 0 ? Global.DBNullParamValue : ddlOptionPeriod.SelectedValue) + "(;||;)" +
                                        territory + "(;||;)" + config + "(;||;)" + salesType + "(;||;)" +
                                        (revenueRate == string.Empty ? Global.DBNullParamValue : revenueRate) + "(;||;)" +
                                        (royaltyRate == string.Empty ? Global.DBNullParamValue : royaltyRate) + "(;||;)" +
                                         isModified);
                        }
                    }
                }


            }

            return subRatesList.ToArray();
        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["RoyContSubRatesGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            string territory;
            string config;
            string salesType;
            string territoryCode;
            string configCode;
            string salesTypeCode;
            string revenueRate;
            string royaltyRate;

            foreach (GridViewRow gvr in gvSubRates.Rows)
            {
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;
                revenueRate = (gvr.FindControl("txtRevenueRate") as TextBox).Text;
                royaltyRate = (gvr.FindControl("txtRoyaltyRate") as TextBox).Text;
                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["subsid_rate_id"] = (gvr.FindControl("hdnRateId") as HiddenField).Value;
                drGridRow["option_period_code"] = (gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedValue;
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
                    territoryCode = territory.Replace(" ", Global.DBDelimiter);
                    territoryCode = (territoryCode == string.Empty ? wildCardChar : territoryCode.Substring(0, territoryCode.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                    territoryCode = territoryCode.Replace(Global.DBDelimiter, " ");

                    drGridRow["seller_group_code"] = territoryCode;
                    drGridRow["seller_group"] = territory;
                }

                if (config == string.Empty)
                {
                    drGridRow["config_group_code"] = DBNull.Value;
                    drGridRow["config_group"] = DBNull.Value;
                }
                else
                {
                    configCode = config.Replace(" ", Global.DBDelimiter);
                    configCode = (configCode == string.Empty ? wildCardChar : configCode.Substring(0, configCode.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                    configCode = configCode.Replace(Global.DBDelimiter, " ");

                    drGridRow["config_group_code"] = configCode;
                    drGridRow["config_group"] = config;
                }

                if (salesType == string.Empty)
                {
                    drGridRow["price_group_code"] = DBNull.Value;
                    drGridRow["price_group"] = DBNull.Value;
                }
                else
                {
                    salesTypeCode = salesType.Replace(" ", Global.DBDelimiter);
                    salesTypeCode = (salesTypeCode == string.Empty ? wildCardChar : salesTypeCode.Substring(0, salesTypeCode.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                    salesTypeCode = salesTypeCode.Replace(Global.DBDelimiter, " ");

                    drGridRow["price_group_code"] = salesTypeCode;
                    drGridRow["price_group"] = salesType;
                }

                if (revenueRate == string.Empty)
                {
                    drGridRow["revenue_rate"] = DBNull.Value;
                }
                else
                {
                    drGridRow["revenue_rate"] = revenueRate;
                }

                if (royaltyRate == string.Empty)
                {
                    drGridRow["royalty_rate"] = DBNull.Value;
                }
                else
                {
                    drGridRow["royalty_rate"] = royaltyRate;
                }

                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["RoyContSubRatesGridData"] = dtGridChangedData;

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


            DataTable dtGridData = (DataTable)Session["RoyContSubRatesGridData"];
            foreach (DataRow dr in dtGridData.Rows)
            {
                if (dr["subsid_rate_id"].ToString() == rateIdToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData); ;
            Session["RoyContSubRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvSubRates.DataSource = dtGridChangedDataSorted;
            gvSubRates.DataBind();

            //delete row from initial grid data session
            DataTable dtOriginalGridData = (DataTable)Session["RoyContSubRatesGridDataInitial"];
            foreach (DataRow dr in dtOriginalGridData.Rows)
            {
                if (dr["subsid_rate_id"].ToString() == rateIdToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataTable dtOriginalGridDataSorted = GridDataSorted(dtOriginalGridData); ;
            Session["RoyContSubRatesGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private void AssignGridDropdownList()
        {
            dtOptionPeriodList = (DataTable)Session["RoyContSubRatesOptPeriod"];
            dtSalesTypeList = (DataTable)Session["RoyContSubRatesSalesType"];
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

        /// <summary>
        /// validate if the new row being added is not existing
        /// </summary>
        /// <returns></returns>
        private bool IsRowExisting()
        {
            bool isRowExisting = false;
            string optionPeriod;
            string territory;
            string salesType;
            string config;
            string optionPeriodSelected = ddlOptionPeriodAddRow.SelectedValue;
            string territorySelected = string.Empty;
            string configCodeSelected = string.Empty;
            string salesTypeSelected = string.Empty;

            if (txtTerritoryAddRow.Text != string.Empty)
            {
                territorySelected = txtTerritoryAddRow.Text.Replace(" ", Global.DBDelimiter);
                territorySelected = (territorySelected == string.Empty ? wildCardChar : territorySelected.Substring(0, territorySelected.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                territorySelected = territorySelected.Replace(Global.DBDelimiter, " ");

            }

            if (txtConfigAddRow.Text != string.Empty)
            {
                configCodeSelected = txtConfigAddRow.Text.Replace(" ", Global.DBDelimiter);
                configCodeSelected = (configCodeSelected == string.Empty ? wildCardChar : configCodeSelected.Substring(0, configCodeSelected.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                configCodeSelected = configCodeSelected.Replace(Global.DBDelimiter, " ");
            }

            if (txtSalesTypeAddRow.Text != string.Empty)
            {
                salesTypeSelected = txtSalesTypeAddRow.Text.Replace(" ", Global.DBDelimiter);
                salesTypeSelected = (salesTypeSelected == string.Empty ? wildCardChar : salesTypeSelected.Substring(0, salesTypeSelected.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                salesTypeSelected = salesTypeSelected.Replace(Global.DBDelimiter, " ");

            }

            foreach (GridViewRow gvr in gvSubRates.Rows)
            {
                optionPeriod = (gvr.FindControl("ddlOptionPeriod") as DropDownList).SelectedValue;
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                salesType = (gvr.FindControl("txtSalesType") as TextBox).Text;

                if (territory != string.Empty)
                {
                    territory = territory.Replace(" ", Global.DBDelimiter);
                    territory = (territory == string.Empty ? wildCardChar : territory.Substring(0, territory.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                    territory = territory.Replace(Global.DBDelimiter, " ");
                }

                if (config != string.Empty)
                {
                    config = config.Replace(" ", Global.DBDelimiter);
                    config = (config == string.Empty ? wildCardChar : config.Substring(0, config.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                    config = config.Replace(Global.DBDelimiter, " ");
                }

                if (salesType != string.Empty)
                {
                    salesType = salesType.Replace(" ", Global.DBDelimiter);
                    salesType = (salesType == string.Empty ? wildCardChar : salesType.Substring(0, salesType.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                    salesType = salesType.Replace(Global.DBDelimiter, " ");
                }

                if (optionPeriodSelected == optionPeriod && territory == territorySelected && salesType == salesTypeSelected && config == configCodeSelected)
                {
                    isRowExisting = true;
                    break;
                }

            }

            return isRowExisting;
        }

        private void ClearAddRow()
        {
            ddlOptionPeriodAddRow.SelectedIndex = 0;
            txtTerritoryAddRow.Text = string.Empty;
            txtConfigAddRow.Text = string.Empty;
            txtSalesTypeAddRow.Text = string.Empty;
            txtRoyaltyRateAddRow.Text = string.Empty;
            txtRevenueRateAddRow.Text = string.Empty;

        }

        private void AppendRowToGrid()
        {
            if (Session["RoyContSubRatesGridData"] == null || Session["RoyContSubRatesGridDataInitial"] == null)
            {
                ExceptionHandler("Error in adding row to grid", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContSubRatesGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyContSubRatesGridDataInitial"];
            DataRow drNewRow = dtGridData.NewRow();
            DataRow drNewRowOriginal = dtOriginalGridData.NewRow();
            string territoryCodeAddRow;
            string configCodeAddRow;
            string salesTypeCodeAddRow;
            if (hdnRateIdAddRow.Value == string.Empty)
            {
                drNewRow["subsid_rate_id"] = -1;
                hdnRateIdAddRow.Value = "-1";
            }
            else
            {
                int rateId = Convert.ToInt32(hdnRateIdAddRow.Value);
                rateId = rateId - 1;
                drNewRow["subsid_rate_id"] = rateId;
                hdnRateIdAddRow.Value = rateId.ToString();
            }

            drNewRow["option_period_code"] = ddlOptionPeriodAddRow.SelectedValue;
            //drNewRow["seller_group_code"] = ddlTerritoryAddRow.SelectedValue;
            //drNewRow["config_group_code"] = ddlConfigCodeAddRow.SelectedValue;
            //drNewRow["price_group_code"] = ddlSalesTypeAddRow.SelectedValue;

            //if (ddlTerritoryAddRow.SelectedIndex == 0)
            //{
            //    drNewRow["seller_group_code_order"] = "1";//to sort data for wildcard to display bottom
            //}
            //else
            //{
            //    drNewRow["seller_group_code_order"] = "0";//to sort data for wildcard to display bottom
            //}

            if (txtTerritoryAddRow.Text == string.Empty)
            {
                drNewRow["seller_group_code"] = DBNull.Value;
                drNewRow["seller_group"] = DBNull.Value;
                drNewRow["seller_group_code_order"] = "1";//to sort data for wildcard to display bottom
            }
            else
            {
                territoryCodeAddRow = txtTerritoryAddRow.Text.Replace(" ", Global.DBDelimiter);
                territoryCodeAddRow = (territoryCodeAddRow == string.Empty ? wildCardChar : territoryCodeAddRow.Substring(0, territoryCodeAddRow.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                territoryCodeAddRow = territoryCodeAddRow.Replace(Global.DBDelimiter, " ");

                drNewRow["seller_group_code"] = territoryCodeAddRow;
                drNewRow["seller_group"] = txtTerritoryAddRow.Text;
                drNewRow["seller_group_code_order"] = "0";//to sort data for wildcard to display bottom
            }

            if (txtConfigAddRow.Text == string.Empty)
            {
                drNewRow["config_group_code"] = DBNull.Value;
                drNewRow["config_group"] = DBNull.Value;
            }
            else
            {
                configCodeAddRow = txtConfigAddRow.Text.Replace(" ", Global.DBDelimiter);
                configCodeAddRow = (configCodeAddRow == string.Empty ? wildCardChar : configCodeAddRow.Substring(0, configCodeAddRow.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                configCodeAddRow = configCodeAddRow.Replace(Global.DBDelimiter, " ");

                drNewRow["config_group_code"] = configCodeAddRow;
                drNewRow["config_group"] = txtConfigAddRow.Text;
            }

            if (txtSalesTypeAddRow.Text == string.Empty)
            {
                drNewRow["price_group_code"] = DBNull.Value;
                drNewRow["price_group"] = DBNull.Value;
            }
            else
            {
                salesTypeCodeAddRow = txtSalesTypeAddRow.Text.Replace(" ", Global.DBDelimiter);
                salesTypeCodeAddRow = (salesTypeCodeAddRow == string.Empty ? wildCardChar : salesTypeCodeAddRow.Substring(0, salesTypeCodeAddRow.IndexOf(Global.DBDelimiter + "-" + Global.DBDelimiter)));
                salesTypeCodeAddRow = salesTypeCodeAddRow.Replace(Global.DBDelimiter, " ");

                drNewRow["price_group_code"] = salesTypeCodeAddRow;
                drNewRow["price_group"] = txtSalesTypeAddRow.Text;
            }

            if (txtRevenueRateAddRow.Text == string.Empty)
            {
                drNewRow["revenue_rate"] = DBNull.Value;
            }
            else
            {
                drNewRow["revenue_rate"] = txtRevenueRateAddRow.Text;
            }

            if (txtRoyaltyRateAddRow.Text == string.Empty)
            {
                drNewRow["royalty_rate"] = DBNull.Value;
            }
            else
            {
                drNewRow["royalty_rate"] = txtRoyaltyRateAddRow.Text;
            }

            drNewRow["score"] = 0;
            drNewRow["is_modified"] = Global.DBNullParamValue;

            dtGridData.Rows.Add(drNewRow);
            DataTable dtGridChangedDataSorted = (dtGridData); //JIRA -451 Changes
            Session["RoyContSubRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvSubRates.DataSource = dtGridChangedDataSorted;
            gvSubRates.DataBind();
            ddlOptionPeriodAddRow.Focus(); //JIRA-451 CHanges

            drNewRowOriginal.ItemArray = drNewRow.ItemArray.Clone() as object[];
            dtOriginalGridData.Rows.Add(drNewRowOriginal);
            DataTable dtOriginalGridDataSorted = GridDataSorted(dtOriginalGridData);
            Session["RoyContSubRatesGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private void ReOrderGridData()
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (Session["RoyContSubRatesGridData"] == null)
            {
                ExceptionHandler("Error in re-ordering rates data", string.Empty);
                return;
            }

            DataTable dtGridData = (DataTable)Session["RoyContSubRatesGridData"];
            DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData); ;
            Session["RoyContSubRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvSubRates.DataSource = dtGridChangedDataSorted;
            gvSubRates.DataBind();

        }


        private void EnableReadonly()
        {
            btnSave.Enabled = false;
            btnAppendAddRow.Enabled = false;
            btnUndoAddRow.Enabled = false;

            //disable grid buttons
            foreach (GridViewRow gvr in gvSubRates.Rows)
            {
                (gvr.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
            }
        }

        #endregion METHODS

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
                List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchContSubRatesSalesTypeList(hdnFuzzySearchText.Value.ToUpper(), int.MaxValue);
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
                    foreach (GridViewRow gvr in gvSubRates.Rows)
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
                    foreach (GridViewRow gvr in gvSubRates.Rows)
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
                    foreach (GridViewRow gvr in gvSubRates.Rows)
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