/*
File Name   :   TerritorySearch.cs
Purpose     :   To get the territory groups associated with a territory 
                and add those groups to a new territory

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     19-Oct-2016     Pratik(Infosys Limited)   Initial Creation
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
using System.Drawing;

namespace WARS
{
    public partial class TerritorySearch : System.Web.UI.Page
    {

        #region Global Declarations
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        TerritorySearchBL territorySearchBL;
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
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Territory Search";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Territory Search";
                }

                lblTab.Focus();

                if (!IsPostBack)
                {
                    txtTerritorySearch.Focus();
                    //clear sessions
                    Session["FuzzyTerritorySearchSellerGrpList"] = null;
                    Session["TerritorySearchCode"] = null;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadTerritoryList();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                    tdGrid.Visible = false;
                    btnAddTerritories.Enabled = false;
                }
                else
                {
                    PnlTerritoryGroup.Style.Add("height", hdnGridPnlGroupHeight.Value);
                }
                UserAuthorization();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void txtTerritorySearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnSearchListItemSelected.Value == "Y")
                {
                    territorySelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchTerritory();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting the territory.", ex.Message);
            }
        }

        protected void txtTerritoryLocSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnSearchListItemSelected.Value == "Y")
                {
                    territoryLocSelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchTerritoryLoc();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting the territory location.", ex.Message);
            }
        }

        protected void txtNewTerritory_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnSearchListItemSelected.Value == "Y")
                {
                    newTerritorySelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchNewTerritory();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
                }

                mpeInsertGroup.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting the territory.", ex.Message);
            }
        }

        protected void txtNewTerritoryLoc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnSearchListItemSelected.Value == "Y")
                {
                    newterritoryLocSelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchNewTerritoryLoc();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
                }

                mpeInsertGroup.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting the territory location.", ex.Message);
            }
        }

        protected void btnAddTerritories_Click(object sender, EventArgs e)
        {
            try
            {
                txtNewTerritory.Text = string.Empty;
                txtNewTerritoryLoc.Text = string.Empty;
                mpeInsertGroup.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding the territory to groups.", ex.Message);
            }
        }

        protected void fuzzyTerritorySearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchTerritory();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory fuzzy search", ex.Message);
            }
        }

        protected void fuzzyTerritoryLocSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchTerritoryLoc();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory location fuzzy search", ex.Message);
            }
        }

        protected void fuzzySearchNewTerritory_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchNewTerritory();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
                mpeInsertGroup.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in new territory fuzzy search", ex.Message);
            }
        }

        protected void fuzzySearchNewTerritoryLoc_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchNewTerritoryLoc();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
                mpeInsertGroup.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in new territory location fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Territory")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtTerritorySearch.Text = string.Empty;
                        return;
                    }

                    txtTerritorySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                    territorySelected();
                }
                else if (hdnFuzzySearchField.Value == "TerritoryLoc")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtTerritoryLocSearch.Text = string.Empty;
                        return;
                    }

                    txtTerritoryLocSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                    territoryLocSelected();
                }
                else if (hdnFuzzySearchField.Value == "NewTerritory")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtNewTerritory.Text = string.Empty;
                        return;
                    }

                    txtNewTerritory.Text = lbFuzzySearch.SelectedValue.ToString();
                    newTerritorySelected();
                    mpeInsertGroup.Show();
                }
                else if (hdnFuzzySearchField.Value == "NewTerritoryLoc")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtNewTerritoryLoc.Text = string.Empty;
                        return;
                    }

                    txtNewTerritoryLoc.Text = lbFuzzySearch.SelectedValue.ToString();
                    newterritoryLocSelected();
                    mpeInsertGroup.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Territory")
                {
                    txtTerritorySearch.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "TerritoryLoc")
                {
                    txtTerritoryLocSearch.Text = string.Empty;

                }
                else if (hdnFuzzySearchField.Value == "NewTerritory")
                {
                    txtNewTerritory.Text = string.Empty;
                    mpeInsertGroup.Show();

                }
                else if (hdnFuzzySearchField.Value == "NewTerritoryLoc")
                {
                    txtNewTerritoryLoc.Text = string.Empty;
                    mpeInsertGroup.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing fuzzy search pop up", ex.Message);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string territoryCode;
                //changes maded by Harish - 27-10-16
                //if (!string.IsNullOrWhiteSpace(txtNewTerritory.Text) & !string.IsNullOrWhiteSpace(txtNewTerritoryLoc.Text))
                if (string.IsNullOrWhiteSpace(txtNewTerritory.Text) & string.IsNullOrWhiteSpace(txtNewTerritoryLoc.Text))
                {
                    msgView.SetMessage("Please select a territory to add to the groups",
                                    MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(txtNewTerritory.Text))
                    {
                        territoryCode = txtNewTerritory.Text.Substring(0, txtNewTerritory.Text.IndexOf("-") - 1);
                    }
                    else
                    {
                        territoryCode = txtNewTerritoryLoc.Text.Substring(0, txtNewTerritoryLoc.Text.IndexOf("-") - 1);
                    }
                    string territoryGroupCode;

                    List<string> territoryGroupCodes = new List<string>();

                    foreach (GridViewRow gridrow in gvTerritoryGroup.Rows)
                    {
                        territoryGroupCode = (gridrow.FindControl("lblTerritoryGroupCode") as Label).Text;
                        territoryGroupCodes.Add(territoryGroupCode);
                    }

                    if (territoryGroupCodes.Count > 0)
                    {
                        string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                        territorySearchBL = new TerritorySearchBL();
                        territorySearchBL.AddTerritoryToGroups(territoryCode, territoryGroupCodes.ToArray(), userCode, out errorId);
                        territorySearchBL = null;

                        if (errorId == 0)
                        {
                            msgView.SetMessage("New territory has been added to the groups",
                                    MessageType.Warning, PositionType.Auto);
                        }
                        else
                        {
                            ExceptionHandler("Error in adding the territory to groups.", "");
                        }
                    }
                }
                mpeInsertGroup.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding the territory to groups.", ex.Message);
            }
        }

        protected void btnAddTerritory_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("valAddTerritoryGroup");
                if (!Page.IsValid)
                {
                    mpeAddTerritoryCode.Show();
                }
                else
                {

                    string userCode = Convert.ToString(Session["UserCode"]);

                    territorySearchBL = new TerritorySearchBL();
                    DataSet dtTerritoryList = territorySearchBL.InsertUpdateTerritoryGroup("I", txtTerritoryCode.Text.ToUpper(), txtTerritoryName.Text.ToUpper(), txtTerritoryLocation.Text.ToUpper(), txtCountryCode.Text.ToUpper(), ddlTerritoryType.SelectedValue, userCode, out errorId);
                    territorySearchBL = null;

                    if (dtTerritoryList.Tables.Count > 0 && errorId == 0)
                    {
                        Session["FuzzyTerritorySearchSellerGrpList"] = dtTerritoryList.Tables[0];
                        msgView.SetMessage("Territory code added successfully.", MessageType.Warning, PositionType.Auto);
                        mpeAddTerritoryCode.Hide();
                        txtTerritoryCode.Text =  "";
                        txtTerritoryName.Text = "";
                        txtTerritoryLocation.Text = "";
                        txtCountryCode.Text = "";
                        ddlTerritoryType.SelectedIndex = 0;
                    }
                    else if (errorId == 1)
                    {
                        msgView.SetMessage("Territory code already exists.", MessageType.Warning, PositionType.Auto);
                        mpeAddTerritoryCode.Show();
                    }
                    else
                    {
                        ExceptionHandler("Error in adding the new territory code.", "");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding the new territory code.", ex.Message);
            }

        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "TerritorySearch" || hdnButtonSelection.Value == "fuzzyTerritorySearch")
            {
                hdnSearchListItemSelected.Value = "Y";
                txtTerritorySearch.Text = string.Empty;
                txtTerritorySearch_TextChanged(sender, e);
            }
            else if (hdnButtonSelection.Value == "TerritoryLocSearch" || hdnButtonSelection.Value == "fuzzyTerritoryLocSearch")
            {
                hdnSearchListItemSelected.Value = "Y";
                txtTerritoryLocSearch.Text = string.Empty;
                txtTerritoryLocSearch_TextChanged(sender, e);
            }
            else if (hdnButtonSelection.Value == "AddTerritories")
            {
                btnAddTerritories_Click(sender, e);
            }
        }

        protected void valCountryCode_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                string countryCode = txtCountryCode.Text.ToUpper();
                if (countryCode != string.Empty)
                {
                    DataTable dtCountry = (DataTable)Session["TerritoyrSearhCountryList"];
                    DataRow[] isCodeValid = dtCountry.Select("country_code='" + countryCode + "'");
                    if (isCodeValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Country code validation.", ex.Message);
            }
        }


        protected void valGridCountryCode_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                string countryCode = txtGridCountryCode.Text.ToUpper();

                if (countryCode != string.Empty)
                {
                    DataTable dtCountry = (DataTable)Session["TerritoyrSearhCountryList"];
                    DataRow[] isCodeValid = dtCountry.Select("country_code='" + countryCode + "'");
                    if (isCodeValid.Count() == 0)
                    {
                        args.IsValid = false;
                    }
                    else { args.IsValid = true; }
                }
                else
                {
                    args.IsValid = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Country code validation.", ex.Message);
            }
        }

        protected void imgBtnTerritoryUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = Convert.ToString(Session["UserCode"]);

                Page.Validate("valUpdateTerritoryGroup");
                if (!Page.IsValid)
                {
                    return;
                }
                
                territorySearchBL = new TerritorySearchBL();
                DataSet dtTerritoryList = territorySearchBL.InsertUpdateTerritoryGroup("U", lblTerritoryCode.Text.ToUpper(), txtGridTerritoryName.Text.ToUpper(), txtGridTerritoryLocation.Text.ToUpper(), txtGridCountryCode.Text.ToUpper(), ddlGridTerritoryType.SelectedValue, userCode, out errorId);
                territorySearchBL = null;

                if (dtTerritoryList.Tables.Count != 0 && errorId != 2)
                {
                    Session["FuzzyTerritorySearchSellerGrpList"] = dtTerritoryList.Tables[0];

                    DataTable dtTerritoryDtls = dtTerritoryList.Tables[1];
                    lblTerritoryCode.Text = dtTerritoryDtls.Rows[0]["seller_code"].ToString();
                    hdnGridTerritoryName.Value = txtGridTerritoryName.Text = dtTerritoryDtls.Rows[0]["seller_name"].ToString();
                    hdnGridTerritoryLocation.Value = txtGridTerritoryLocation.Text = dtTerritoryDtls.Rows[0]["seller_location"].ToString();
                    hdnGridCountryCode.Value = txtGridCountryCode.Text = dtTerritoryDtls.Rows[0]["country_code"].ToString();
                    hdnGridTerritoryType.Value = ddlGridTerritoryType.SelectedValue = dtTerritoryDtls.Rows[0]["territory_type"].ToString();

                    msgView.SetMessage("Territory code updated successfully.", MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to update territory code.", MessageType.Warning, PositionType.Auto);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating territory code.", ex.Message);
            }
        }

        protected void gvTerritoryGroup_RowDataBound(object sender, GridViewRowEventArgs e)
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

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvTerritoryGroup_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["TerritoryGroupData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvTerritoryGroup.DataSource = dataView;
                gvTerritoryGroup.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End


        #endregion EVENTS

        #region METHODS

        private void LoadTerritoryList()
        {
            territorySearchBL = new TerritorySearchBL();
            DataSet territoriesList = territorySearchBL.GetInitialData(out errorId);
            territorySearchBL = null;

            if (territoriesList.Tables.Count != 0 && errorId != 2)
            {
                Session["FuzzyTerritorySearchSellerGrpList"] = territoriesList.Tables[0];
                Session["TerritoyrSearhCountryList"] = territoriesList.Tables[2];

                ddlTerritoryType.DataSource = territoriesList.Tables[1];
                ddlTerritoryType.DataTextField = "territory_description";
                ddlTerritoryType.DataValueField = "territory_type";
                ddlTerritoryType.DataBind();
                ddlTerritoryType.Items.Insert(0, new ListItem("-"));

                ddlGridTerritoryType.DataSource = territoriesList.Tables[1];
                ddlGridTerritoryType.DataTextField = "territory_description";
                ddlGridTerritoryType.DataValueField = "territory_type";
                ddlGridTerritoryType.DataBind();
                ddlGridTerritoryType.Items.Insert(0, new ListItem("-"));
            }
            else if (territoriesList.Tables.Count == 0 && errorId != 2)
            {
                Session["FuzzyTerritorySearchSellerGrpList"] = null;
            }
            else
            {
                ExceptionHandler("Error in loading territories", string.Empty);
            }
        }

        private void LoadGridData(string territoryCode)
        {
            territorySearchBL = new TerritorySearchBL();
            DataSet territoryData = territorySearchBL.GetTerritoryData(territoryCode, out errorId);
            territorySearchBL = null;

            BindGrids(territoryData);
        }

        private void BindGrids(DataSet territoryData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (territoryData.Tables.Count != 0 && errorId != 2)
            {
                DataTable dtTerritoryDtls = territoryData.Tables[0];

                lblTerritoryCode.Text = dtTerritoryDtls.Rows[0]["seller_code"].ToString();
                hdnGridTerritoryName.Value = txtGridTerritoryName.Text = dtTerritoryDtls.Rows[0]["seller_name"].ToString();
                hdnGridTerritoryLocation.Value = txtGridTerritoryLocation.Text = dtTerritoryDtls.Rows[0]["seller_location"].ToString();
                hdnGridCountryCode.Value = txtGridCountryCode.Text = dtTerritoryDtls.Rows[0]["country_code"].ToString();
                hdnGridTerritoryType.Value = ddlGridTerritoryType.SelectedValue = dtTerritoryDtls.Rows[0]["territory_type"].ToString();

                Session["TerritoryGroupData"] = territoryData.Tables[1];
                gvTerritoryGroup.DataSource = territoryData.Tables[1];
                if (territoryData.Tables[1].Rows.Count == 0)
                {
                    gvTerritoryGroup.EmptyDataText = "No data found for the selected territory.";
                }
                gvTerritoryGroup.DataBind();
            }
            else if (territoryData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvTerritoryGroup.DataSource = dtEmpty;
                gvTerritoryGroup.EmptyDataText = "No data found for the selected territory.";
                gvTerritoryGroup.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

        }

        private bool ValidateSelectedTerritoryCode()
        {
            if (txtTerritorySearch.Text != "" && Session["FuzzyTerritorySearchSellerGrpList"] != null)
            {
                if (txtTerritorySearch.Text != "No results found")
                {
                    DataTable dtTerritoriesList;
                    dtTerritoriesList = Session["FuzzyTerritorySearchSellerGrpList"] as DataTable;

                    foreach (DataRow dRow in dtTerritoriesList.Rows)
                    {
                        if (dRow["seller_group"].ToString() == txtTerritorySearch.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool ValidateSelectedTerritoryLocCode()
        {
            if (txtTerritoryLocSearch.Text != "" && Session["FuzzyTerritorySearchSellerGrpList"] != null)
            {
                if (txtTerritoryLocSearch.Text != "No results found")
                {
                    DataTable dtTerritoriesList;
                    dtTerritoriesList = Session["FuzzyTerritorySearchSellerGrpList"] as DataTable;

                    foreach (DataRow dRow in dtTerritoriesList.Rows)
                    {
                        if (dRow["seller_group"].ToString() == txtTerritoryLocSearch.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool ValidateNewTerritoryCode()
        {
            if (txtNewTerritory.Text != "" && Session["FuzzyTerritorySearchSellerGrpList"] != null)
            {
                if (txtNewTerritory.Text != "No results found")
                {
                    DataTable dtTerritoriesList;
                    dtTerritoriesList = Session["FuzzyTerritorySearchSellerGrpList"] as DataTable;

                    foreach (DataRow dRow in dtTerritoriesList.Rows)
                    {
                        if (dRow["seller_group"].ToString() == txtNewTerritory.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool ValidateNewTerritoryLocCode()
        {
            if (txtNewTerritoryLoc.Text != "" && Session["FuzzyTerritorySearchSellerGrpList"] != null)
            {
                if (txtNewTerritoryLoc.Text != "No results found")
                {
                    DataTable dtTerritoriesList;
                    dtTerritoriesList = Session["FuzzyTerritorySearchSellerGrpList"] as DataTable;

                    foreach (DataRow dRow in dtTerritoriesList.Rows)
                    {
                        if (dRow["seller_group"].ToString() == txtNewTerritoryLoc.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        } 

        private void territorySelected()
        {
            if (ValidateSelectedTerritoryCode())
            {
                string territoryCode = txtTerritorySearch.Text.Substring(0, txtTerritorySearch.Text.IndexOf("-") - 1);

                Session["TerritorySearchCode"] = territoryCode;

                LoadGridData(territoryCode);

                tdGrid.Visible = true;
                btnAddTerritories.Enabled = true;
                txtTerritoryLocSearch.Text = string.Empty;
                txtNewTerritory.Text = string.Empty;
                txtNewTerritoryLoc.Text = string.Empty;

                UserAuthorization();
            }
            else
            {
                txtTerritorySearch.Text = string.Empty;
                txtTerritoryLocSearch.Text = string.Empty;
                tdGrid.Visible = false;
                hdnSearchListItemSelected.Value = "N";
            }
        }

        private void territoryLocSelected()
        {
            if (ValidateSelectedTerritoryLocCode())
            {
                string territoryCode = txtTerritoryLocSearch.Text.Substring(0, txtTerritoryLocSearch.Text.IndexOf("-") - 1);

                Session["TerritorySearchCode"] = territoryCode;

                LoadGridData(territoryCode);

                tdGrid.Visible = true;
                btnAddTerritories.Enabled = true;
                txtTerritorySearch.Text = string.Empty;
                txtNewTerritory.Text = string.Empty;
                txtNewTerritoryLoc.Text = string.Empty;

                UserAuthorization();
            }
            else
            {
                txtTerritorySearch.Text = string.Empty;
                txtTerritoryLocSearch.Text = string.Empty;
                tdGrid.Visible = false;
                hdnSearchListItemSelected.Value = "N";
            }
        }

        private void newTerritorySelected()
        {
            if (ValidateNewTerritoryCode())
            {
                string territoryCode = txtNewTerritory.Text.Substring(0, txtNewTerritory.Text.IndexOf("-") - 1);

                if (Session["TerritorySearchCode"] != null)
                {
                    if (territoryCode == Session["TerritorySearchCode"].ToString())
                    {
                        msgView.SetMessage("Please select a territory other than the searched one above",
                            MessageType.Warning, PositionType.Auto);

                        txtNewTerritory.Text = string.Empty;
                    }
                    else
                    {
                        txtNewTerritoryLoc.Text = string.Empty;
                    }
                }
            }
            else
            {
                txtNewTerritory.Text = string.Empty;
            }
        }

        private void newterritoryLocSelected()
        {
            if (ValidateNewTerritoryLocCode())
            {

                string territoryCode = txtNewTerritoryLoc.Text.Substring(0, txtNewTerritoryLoc.Text.IndexOf("-") - 1);

                if (Session["TerritorySearchCode"] != null)
                {
                    if (territoryCode == Session["TerritorySearchCode"].ToString())
                    {
                        msgView.SetMessage("Please select a territory other than the searched one above",
                            MessageType.Warning, PositionType.Auto);

                        txtNewTerritoryLoc.Text = string.Empty;
                    }
                    else
                    {
                        txtNewTerritory.Text = string.Empty;
                    }
                }
            }
            else
            {
                txtNewTerritoryLoc.Text = string.Empty;
            }
        }

        private void FuzzySearchTerritory()
        {

            if (txtTerritorySearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in territory field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Territory";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyTerritorySearchSellerGrpList(txtTerritorySearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchTerritoryLoc()
        {

            if (txtTerritoryLocSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in territory location field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "TerritoryLoc";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyTerritorySearchSellerGrpList(txtTerritoryLocSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchNewTerritory()
        {

            if (txtNewTerritory.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in new territory field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "NewTerritory";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyTerritorySearchSellerGrpList(txtNewTerritory.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchNewTerritoryLoc()
        {

            if (txtNewTerritoryLoc.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in new territory location field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "NewTerritoryLoc";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyTerritorySearchSellerGrpList(txtNewTerritoryLoc.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnAddTerritories.Enabled = false;
                btnAddNewTerritory.Enabled = false;
                imgBtnSave.Enabled = false;
                imgBtnCancel.Enabled = false;
               

            }
        }


        #endregion METHODS

       
    }
}