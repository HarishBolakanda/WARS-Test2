/*
File Name   :   RoyContractPkgRates.cs
Purpose     :   to maintain packaging rates of royaltor contract

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     05-May-2017     Harish(Infosys Limited)   Initial Creation
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
    public partial class RoyContractPkgRates : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractPkgRatesBL royContractPkgRatesBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtOptionPeriodList;
        string loggedUserID;
        string userRoleID;
        string royaltorId = string.Empty;
        string isNewRoyaltor = string.Empty;
        string wildCardChar = ".";
        string orderByTerritory = "seller_group_code_order,seller_group_code, score desc,catno,config_group_code";
        string orderByHierarchy = "score desc,seller_group_code_order,seller_group_code,catno,config_group_code";
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

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Packaging Rates";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Packaging Rates";
                }

                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //HtmlTableRow trPackagingRates = (HtmlTableRow)contractNavigationButtons.FindControl("trPackagingRates");
                        //trPackagingRates.Visible = false;
                        Button btnPackagingRates = (Button)contractNavigationButtons.FindControl("btnPackagingRates");
                        btnPackagingRates.Enabled = false;
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
                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.PackagingRates.ToString());
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
                                LoadPkgRatesData(optPrdSelected);
                            }
                            else
                            {
                                LoadPkgRatesData("-");
                            }

                        }
                        else
                        {
                            LoadPkgRatesData("-");
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

        protected void gvPkgRates_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Packaging rates not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                Array pkgRatesList = PkgRatesList();
                List<string> deleteList = new List<string>();
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                }

                //check if any changes to save
                if (pkgRatesList.Length == 0 && deleteList.Count == 0)
                {
                    if (isNewRoyaltor == "N")
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }
                    else if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.PackagingRates.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);
                    }

                    return;
                }

                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                userRoleID = Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower());
                string royaltor;
                string invalidCatno;
                royContractPkgRatesBL = new RoyContractPkgRatesBL();
                DataSet pkgRatesData = royContractPkgRatesBL.SavePkgRates(royaltorId, ddlOptionPeriod.SelectedValue, loggedUserID, userRoleID, pkgRatesList, deleteList.ToArray(), out royaltor, out invalidCatno, out errorId);
                royContractPkgRatesBL = null;

                //validate - catno should be valid one from warse.catno table
                if (errorId == 1)
                {
                    msgView.SetMessage("Packaging rates not saved – invalid Catalogue No. - " + invalidCatno, MessageType.Warning, PositionType.Auto);
                    return;
                }

                hdnGridDataChanged.Value = "N";
                hdnGridDataDeleted.Value = "N";
                ViewState["vsDeleteIds"] = null;
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (errorId == 0)
                {
                    txtRoyaltorId.Text = royaltor;
                    if (pkgRatesData.Tables.Count != 0)
                    {
                        DataTable dtGridDataSorted = GridDataSorted(pkgRatesData.Tables[0]);
                        dtOptionPeriodList = pkgRatesData.Tables[1];
                        Session["RoyContPkgRatesGridDataInitial"] = dtGridDataSorted;
                        Session["RoyContPkgRatesGridData"] = dtGridDataSorted;
                        Session["RoyContPkgRatesOptPeriod"] = dtOptionPeriodList;

                        gvPkgRates.DataSource = pkgRatesData.Tables[0];
                        gvPkgRates.DataBind();

                        if (pkgRatesData.Tables[0].Rows.Count == 0)
                        {
                            gvPkgRates.EmptyDataText = "No data found for the selected royaltor and option period";
                        }
                        else
                        {
                            gvPkgRates.EmptyDataText = string.Empty;
                        }


                    }
                    else if (pkgRatesData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvPkgRates.DataSource = dtEmpty;
                        gvPkgRates.EmptyDataText = "No data found for the selected royaltor and option period";
                        gvPkgRates.DataBind();
                    }

                    //new royaltor - redirect to Escalation rates screen
                    //existing royaltor - remain in same screen
                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.PackagingRates.ToString());

                        //redirect in javascript so that issue of data not saved validation would be handled
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId.Split('-')[0].Trim() + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Packaging rates saved", MessageType.Warning, PositionType.Auto);
                    }

                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving packaging rates data", string.Empty);
                }
                else
                {
                    ExceptionHandler("Error in saving packaging rates data", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving packaging rates data", ex.Message);
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

        protected void btnConfirmOptChange_Click(object sender, EventArgs e)
        {
            LoadPkgRatesData(ddlOptionPeriod.SelectedValue);
        }

       
        //JIRA-908 Changes by Ravi on 12/02/2019 -- Start
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

        //JIRA-908 Changes by Ravi on 12/02/2019 -- End  


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvPkgRates_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyContPkgRatesGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                AssignGridDropdownList();
                gvPkgRates.DataSource = dataView;
                gvPkgRates.DataBind();
                Session["RoyContPkgRatesGridData"] = dataView.ToTable();
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

                LoadPkgRatesData(ddlOptionPeriod.SelectedValue);
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

        #endregion Events

        #region Methods
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void LoadPkgRatesData(string optionPeriod)
        {
            string royaltor;
            userRoleID = Utilities.GetUserRoleId(Session["UserRole"].ToString().ToLower()); ;
            royContractPkgRatesBL = new RoyContractPkgRatesBL();
            DataSet pkgRatesData = royContractPkgRatesBL.GetPkgRatesData(royaltorId, optionPeriod, userRoleID, out royaltor, out errorId);
            royContractPkgRatesBL = null;

            if (pkgRatesData.Tables.Count != 0 && errorId != 2)
            {
                txtRoyaltorId.Text = royaltor;
                dtOptionPeriodList = pkgRatesData.Tables[1];
                Session["RoyContPkgRatesGridDataInitial"] = pkgRatesData.Tables[0];
                Session["RoyContPkgRatesGridData"] = pkgRatesData.Tables[0];
                Session["RoyContPkgRatesOptPeriod"] = dtOptionPeriodList;
                Session["RoyContPkgRatesCatno"] = pkgRatesData.Tables[2];

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
                    }
                }
                else
                {
                    ddlOptionPeriod.SelectedValue = optionPeriod;
                }

                if (pkgRatesData.Tables[0].Rows.Count == 0)
                {
                    gvPkgRates.EmptyDataText = "No data found for the selected royaltor and option period";
                }

                gvPkgRates.DataSource = pkgRatesData.Tables[0];
                gvPkgRates.DataBind();

            }
            else if (pkgRatesData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvPkgRates.DataSource = dtEmpty;
                gvPkgRates.EmptyDataText = "No data found for the selected royaltor and option period";
                gvPkgRates.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }

            hdnOptPeriodSelected.Value = ddlOptionPeriod.SelectedValue;
            hdnGridDataChanged.Value = "N";
            hdnGridDataDeleted.Value = "N";
            ViewState["vsDeleteIds"] = null;
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


            DataTable dtGridData = (DataTable)Session["RoyContPkgRatesGridData"];
            foreach (DataRow dr in dtGridData.Rows)
            {
                if (dr["pack_rate_id"].ToString() == rateIdToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData); ;
            Session["RoyContPkgRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvPkgRates.DataSource = dtGridChangedDataSorted;
            gvPkgRates.DataBind();

            //delete row from initial grid data session
            DataTable dtOriginalGridData = (DataTable)Session["RoyContPkgRatesGridDataInitial"];
            foreach (DataRow dr in dtOriginalGridData.Rows)
            {
                if (dr["pack_rate_id"].ToString() == rateIdToDelete)
                {
                    dr.Delete();
                    break;
                }

            }

            DataTable dtOriginalGridDataSorted = GridDataSorted(dtOriginalGridData); ;
            Session["RoyContPkgRatesGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private Array PkgRatesList()
        {
            if (Session["RoyContPkgRatesGridDataInitial"] == null)
            {
                ExceptionHandler("Error in saving packaging rates data", string.Empty);
            }

            DataTable pkgRatesData = (DataTable)Session["RoyContPkgRatesGridDataInitial"];
            List<string> pkgRatesList = new List<string>();
            string rateId;
            string territory;
            string catNo;
            string config;
            string pkgRate;
            string isModified;

            foreach (GridViewRow gvr in gvPkgRates.Rows)
            {
                rateId = (gvr.FindControl("hdnRateId") as HiddenField).Value;
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                catNo = (gvr.FindControl("txtCatNo") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                pkgRate = (gvr.FindControl("txtPkgRate") as TextBox).Text;
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;

                territory = (territory == string.Empty ? wildCardChar : territory.Substring(0, territory.IndexOf("-") - 1));
                config = (config == string.Empty ? wildCardChar : config.Substring(0, config.IndexOf("-") - 1));

                if (isModified == Global.DBNullParamValue)//new row
                {
                    isModified = Global.DBNullParamValue;
                    pkgRatesList.Add(rateId + "(;||;)" + territory + "(;||;)" +
                                        (catNo == string.Empty ? Global.DBNullParamValue : catNo.ToUpper()) + "(;||;)" +
                                        config + "(;||;)" +
                                        (pkgRate == string.Empty ? Global.DBNullParamValue : pkgRate) + "(;||;)" +
                                         isModified);

                }
                else
                {
                    DataTable dtPkgRate = pkgRatesData.Select("pack_rate_id=" + rateId).CopyToDataTable();
                    if (dtPkgRate.Rows.Count != 0)
                    {
                        if ((territory == string.Empty ? wildCardChar : territory) != dtPkgRate.Rows[0]["seller_group_code"].ToString()
                                || catNo.ToUpper() != dtPkgRate.Rows[0]["catno"].ToString()
                                || (config == string.Empty ? wildCardChar : config) != dtPkgRate.Rows[0]["config_group_code"].ToString()
                                || pkgRate != (dtPkgRate.Rows[0]["rate_value"].ToString() == "" ? "" : Convert.ToDouble(dtPkgRate.Rows[0]["rate_value"]).ToString()))//existing row - if data changed 
                        {
                            isModified = "Y";
                            pkgRatesList.Add(rateId + "(;||;)" + territory + "(;||;)" +
                                        (catNo == string.Empty ? Global.DBNullParamValue : catNo.ToUpper()) + "(;||;)" +
                                        config + "(;||;)" +
                                        (pkgRate == string.Empty ? Global.DBNullParamValue : pkgRate) + "(;||;)" +
                                         isModified);
                        }
                    }
                }


            }

            return pkgRatesList.ToArray();
        }

        private void ReOrderGridData()
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (Session["RoyContPkgRatesGridData"] == null)
            {
                ExceptionHandler("Error in re-ordering rates data", string.Empty);
                return;
            }

            DataTable dtGridData = (DataTable)Session["RoyContPkgRatesGridData"];
            DataTable dtGridChangedDataSorted = GridDataSorted(dtGridData);
            Session["RoyContPkgRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvPkgRates.DataSource = dtGridChangedDataSorted;
            gvPkgRates.DataBind();

        }

        private void AssignGridDropdownList()
        {
            dtOptionPeriodList = (DataTable)Session["RoyContPkgRatesOptPeriod"];
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
            string territory;
            string catNo;
            string config;
            string territorySelected = string.Empty;
            string configCodeSelected = string.Empty;

            if (txtTerritoryAddRow.Text != string.Empty)
            {
                territorySelected = txtTerritoryAddRow.Text.Substring(0, txtTerritoryAddRow.Text.IndexOf("-") - 1);
            }

            if (txtConfigAddRow.Text != string.Empty)
            {
                configCodeSelected = txtConfigAddRow.Text.Substring(0, txtConfigAddRow.Text.IndexOf("-") - 1);
            }

            foreach (GridViewRow gvr in gvPkgRates.Rows)
            {
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                catNo = (gvr.FindControl("txtCatNo") as TextBox).Text;

                if (territory == territorySelected && config == configCodeSelected && catNo == txtCatNoAddRow.Text)
                {
                    isRowExisting = true;
                    break;
                }

            }

            return isRowExisting;
        }

        private void ClearAddRow()
        {
            txtTerritoryAddRow.Text = string.Empty;
            txtCatNoAddRow.Text = string.Empty;
            txtConfigAddRow.Text = string.Empty;
            txtPkgRateAddRow.Text = string.Empty;

        }

        private void AppendRowToGrid()
        {
            if (Session["RoyContPkgRatesGridData"] == null || Session["RoyContPkgRatesGridDataInitial"] == null)
            {
                ExceptionHandler("Error in adding rate row to grid", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContPkgRatesGridData"];
            DataTable dtOriginalGridData = (DataTable)Session["RoyContPkgRatesGridDataInitial"];
            DataRow drNewRow = dtGridData.NewRow();
            DataRow drNewRowOriginal = dtOriginalGridData.NewRow();
            if (hdnRateIdAddRow.Value == string.Empty)
            {
                drNewRow["pack_rate_id"] = -1;
                hdnRateIdAddRow.Value = "-1";
            }
            else
            {
                int rateId = Convert.ToInt32(hdnRateIdAddRow.Value);
                rateId = rateId - 1;
                drNewRow["pack_rate_id"] = rateId;
                hdnRateIdAddRow.Value = rateId.ToString();
            }

            //drNewRow["seller_group_code"] = ddlTerritoryAddRow.SelectedValue;

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

            //drNewRow["config_group_code"] = ddlConfigCodeAddRow.SelectedValue;


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

            if (txtPkgRateAddRow.Text == string.Empty)
            {
                drNewRow["rate_value"] = DBNull.Value;
            }
            else
            {
                drNewRow["rate_value"] = txtPkgRateAddRow.Text;
            }

            drNewRow["score"] = 0;
            drNewRow["is_modified"] = Global.DBNullParamValue;

            dtGridData.Rows.Add(drNewRow);
            DataTable dtGridChangedDataSorted = (dtGridData);
            Session["RoyContPkgRatesGridData"] = dtGridChangedDataSorted;
            AssignGridDropdownList();
            gvPkgRates.DataSource = dtGridChangedDataSorted;
            gvPkgRates.DataBind();
            txtTerritoryAddRow.Focus(); //JIRA-451 CHanges

            drNewRowOriginal.ItemArray = drNewRow.ItemArray.Clone() as object[];
            dtOriginalGridData.Rows.Add(drNewRowOriginal);
            DataTable dtOriginalGridDataSorted = GridDataSorted(dtOriginalGridData);
            Session["RoyContPkgRatesGridDataInitial"] = dtOriginalGridDataSorted;


        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["RoyContPkgRatesGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            string territory;
            string config;
            string pkgRate;


            foreach (GridViewRow gvr in gvPkgRates.Rows)
            {
                territory = (gvr.FindControl("txtTerritory") as TextBox).Text;
                config = (gvr.FindControl("txtConfig") as TextBox).Text;
                pkgRate = (gvr.FindControl("txtPkgRate") as TextBox).Text;

                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["pack_rate_id"] = (gvr.FindControl("hdnRateId") as HiddenField).Value;
                //drGridRow["seller_group_code"] = (gvr.FindControl("ddlTerritory") as DropDownList).SelectedValue;
                drGridRow["seller_group_code_order"] = (gvr.FindControl("hdnSellerGrpCodeOrder") as HiddenField).Value;
                drGridRow["catno"] = (gvr.FindControl("txtCatNo") as TextBox).Text;
                //drGridRow["config_group_code"] = (gvr.FindControl("ddlConfigCode") as DropDownList).SelectedValue;
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

                if (pkgRate == string.Empty)
                {
                    drGridRow["rate_value"] = DBNull.Value;
                }
                else
                {
                    drGridRow["rate_value"] = pkgRate;
                }

                dtGridChangedData.Rows.Add(drGridRow);

            }

            Session["RoyContPkgRatesGridData"] = dtGridChangedData;

        }


        private void EnableReadonly()
        {
            btnSave.Enabled = false;
            btnAppendAddRow.Enabled = false;
            btnUndoAddRow.Enabled = false;

            //disable grid buttons
            foreach (GridViewRow gvr in gvPkgRates.Rows)
            {
                (gvr.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
            }
        }

        #endregion Methods

        #region Validations
        protected void valtxtCatNoAddRow_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (txtCatNoAddRow.Text != string.Empty)
                {
                    DataTable dtCatno = (DataTable)Session["RoyContPkgRatesCatno"];
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
                    DataTable dtCatno = (DataTable)Session["RoyContPkgRatesCatno"];
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

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Territory")
                {
                    TextBox txtTerritory;
                    foreach (GridViewRow gvr in gvPkgRates.Rows)
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
                    foreach (GridViewRow gvr in gvPkgRates.Rows)
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