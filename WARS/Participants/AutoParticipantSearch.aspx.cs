/*
File Name   :   AutoParticipantSearch.cs
Purpose     :   to search and Add/Update Participant Groups

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     12-Apr-2019     Rakesh(Infosys Limited)   Initial Creation
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WARS.BusinessLayer;

namespace WARS.Participants
{
    public partial class AutoParticipantSearch : System.Web.UI.Page
    {

        #region Global Declarations

        AutoParticipantSearchBL autoParticipantSearchBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize1"].ToString());
        int newAutoPartId;
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
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Auto Participants Search";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Auto Participants Search";
                }

                pnlAutoParticipantDtls.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadAutoParticipantData();

                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }
                    util = null;
                }

                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void imgBtnAddParticipant_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAutoParticipant(0, txtMarketingOwnerAddRow.Text, txtWEASalesLabelAddRow.Text, txtArtistAddRow.Text, txtProjectTitleAddRow.Text);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving Participation Group.", ex.Message);
            }
        }

        protected void gvAutoParticipantDtls_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    e.Row.Cells[4].Visible = false;
                    ImageButton _dblClickButton = e.Row.FindControl("imgBtnDblClk") as ImageButton;
                    string _jsDoubleClick = ClientScript.GetPostBackClientHyperlink(_dblClickButton, "");
                    e.Row.Attributes.Add("ondblclick", _jsDoubleClick);

                }

                //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[4].Visible = false;
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
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }



        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvAutoParticipantDtls_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["AutoParticipantData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvAutoParticipantDtls, dtEmpty, rptPager);
                Session["AutoParticipantData"] = dataView.ToTable();
            }

            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvAutoParticipantDtls_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "saverow")
                {
                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    int rowIndex = gvr.RowIndex;

                    if (hdnChangeNotSaved.Value == "Y")
                    {
                        Int32 autoPartId = Convert.ToInt32(((HiddenField)gvAutoParticipantDtls.Rows[rowIndex].FindControl("hdnAutoParticipantId")).Value);
                        string marketingOwner = ((TextBox)gvAutoParticipantDtls.Rows[rowIndex].FindControl("txtMarketingOwnerGridRow")).Text;
                        string weaSalesLabel = ((TextBox)gvAutoParticipantDtls.Rows[rowIndex].FindControl("txtWEASalesLabelGridRow")).Text;
                        string artist = ((TextBox)gvAutoParticipantDtls.Rows[rowIndex].FindControl("txtArtistGridRow")).Text;
                        string projectTitle = ((TextBox)gvAutoParticipantDtls.Rows[rowIndex].FindControl("txtProjectTitleGridRow")).Text;

                        SaveAutoParticipant(autoPartId, marketingOwner, weaSalesLabel, artist, projectTitle);
                        SeachAutoParticipantData();
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        hdnFuzzyGridRowValidator.Value = "N";
                    }
                }
                if (e.CommandName == "dblClk")
                {
                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    int rowIndex = gvr.RowIndex;

                    Int32 autoPartId = Convert.ToInt32(((HiddenField)gvAutoParticipantDtls.Rows[rowIndex].FindControl("hdnAutoParticipantId")).Value);
                    if (hdnChangeNotSaved.Value != "Y")
                    {
                        Response.Redirect("../Participants/AutoParticipantMaintenance.aspx?autoPartId=" + autoPartId + "", false);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "RedirectToAutoParticipMaint", "RedirectToAutoParticipMaintOnDblClick(" + autoPartId + ");", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving auto participant data.", ex.Message);
            }
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["AutoParticipantData"] == null)
                    return;

                DataTable dtAutoParticipantData = Session["AutoParticipantData"] as DataTable;
                if (dtAutoParticipantData.Rows.Count == 0)
                    return;

                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);
                Utilities.PopulateGridPage(pageIndex, dtAutoParticipantData, gridDefaultPageSize, gvAutoParticipantDtls, dtEmpty, rptPager);
                hdnChangeNotSaved.Value = "N";
                UserAuthorization();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            hdnPageNumber.Value = "1";
            //Making this hidden field value to Y so that if we get a single record after search then it will redirect to next screen
            hdnIsNewRequest.Value = "Y";
            SeachAutoParticipantData();
            UserAuthorization();

        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;
            txtMarketingOwner.Text = string.Empty;
            txtWEASalesLabel.Text = string.Empty;
            txtArtist.Text = string.Empty;
            txtProject.Text = string.Empty;
            txtMarketingOwnerAddRow.Text = string.Empty;
            txtWEASalesLabelAddRow.Text = string.Empty;
            txtArtistAddRow.Text = string.Empty;
            txtProjectTitleAddRow.Text = string.Empty;
            Session["AutoParticipantFilters"] = null;
            hdnChangeNotSaved.Value = "N";
            hdnAddRowDataNotSaved.Value = "N";
            hdnPageNumber.Value = "1";
            hdnGridRowSelectedPrvious.Value = null;
            hdnFuzzyGridRowValidator.Value = "N";
            hdnFuzzyAddRowValidator.Value = "N";

            dtEmpty = new DataTable();
            gvAutoParticipantDtls.EmptyDataText = "<br />";
            gvAutoParticipantDtls.DataSource = dtEmpty;
            gvAutoParticipantDtls.DataBind();

            rptPager.DataSource = null;
            rptPager.DataBind();

        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                if (hdnAddRowDataNotSaved.Value == "Y")
                {
                    SaveAutoParticipant(0, txtMarketingOwnerAddRow.Text, txtWEASalesLabelAddRow.Text, txtArtistAddRow.Text, txtProjectTitleAddRow.Text);

                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                    Int32 autoPartId = Convert.ToInt32(((HiddenField)gvAutoParticipantDtls.Rows[rowIndex].FindControl("hdnAutoParticipantId")).Value);
                    string marketingOwner = ((TextBox)gvAutoParticipantDtls.Rows[rowIndex].FindControl("txtMarketingOwnerGridRow")).Text;
                    string weaSalesLabel = ((TextBox)gvAutoParticipantDtls.Rows[rowIndex].FindControl("txtWEASalesLabelGridRow")).Text;
                    string artist = ((TextBox)gvAutoParticipantDtls.Rows[rowIndex].FindControl("txtArtistGridRow")).Text;
                    string projectTitle = ((TextBox)gvAutoParticipantDtls.Rows[rowIndex].FindControl("txtProjectTitleGridRow")).Text;

                    SaveAutoParticipant(autoPartId, marketingOwner, weaSalesLabel, artist, projectTitle);

                    SeachAutoParticipantData();
                    hdnChangeNotSaved.Value = "N";
                    hdnAddRowDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    hdnFuzzyGridRowValidator.Value = "N";

                }
            }

            catch (Exception ex)
            {
                ExceptionHandler("Error in saving Participation Group data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnAddRowDataNotSaved.Value == "Y")
                {
                    txtMarketingOwnerAddRow.Text = string.Empty;
                    txtWEASalesLabelAddRow.Text = string.Empty;
                    txtArtistAddRow.Text = string.Empty;
                    txtProjectTitleAddRow.Text = string.Empty;
                    hdnAddRowDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    hdnFuzzyAddRowValidator.Value = "N";
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    hdnFuzzyGridRowValidator.Value = "N";
                    DataTable dtSearchedData = Session["AutoParticipantData"] as DataTable;
                    BindGrid(dtSearchedData);


                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data.", ex.Message);
            }
        }

        protected void btnCatalogueSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Participants/CatalogueSearch.aspx?isNewRequest=N", false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadAutoParticipantData()
        {
            dtEmpty = new DataTable();
            gvAutoParticipantDtls.EmptyDataText = "<br />";
            gvAutoParticipantDtls.DataSource = dtEmpty;
            gvAutoParticipantDtls.DataBind();

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                hdnIsNewRequest.Value = Request.QueryString[0];

                if (hdnIsNewRequest.Value == "N")
                {
                    if (Session["AutoParticipantFilters"] != null)
                    {
                        DataTable dtSearchedFilters = Session["AutoParticipantFilters"] as DataTable;

                        foreach (DataRow dRow in dtSearchedFilters.Rows)
                        {
                            if (dRow["filter_name"].ToString() == "txtMarketingOwner")
                            {
                                txtMarketingOwner.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtWEASalesLabel")
                            {
                                txtWEASalesLabel.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtArtist")
                            {
                                txtArtist.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtProject")
                            {
                                txtProject.Text = dRow["filter_value"].ToString();
                            }
                        }
                        SeachAutoParticipantData();
                    }
                }
            }
            else
            {
                Session["AutoParticipantFilters"] = null;
            }
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (hdnPageNumber.Value == "")
            {
                hdnPageNumber.Value = "1";
            }
            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), gridData, gridDefaultPageSize, gvAutoParticipantDtls, dtEmpty, rptPager);
        }


        private void AutoParticipantSession()
        {
            //Create a table to hold the filter values
            DataTable dtSearchedFilters = new DataTable();
            dtSearchedFilters.Columns.Add("filter_name", typeof(string));
            dtSearchedFilters.Columns.Add("filter_value", typeof(string));

            //Add the filter values to the above created table
            dtSearchedFilters.Rows.Add("txtMarketingOwner", txtMarketingOwner.Text.Trim());
            dtSearchedFilters.Rows.Add("txtWEASalesLabel", txtWEASalesLabel.Text.Trim());
            dtSearchedFilters.Rows.Add("txtArtist", txtArtist.Text.Trim());
            dtSearchedFilters.Rows.Add("txtProject", txtProject.Text.Trim());

            Session["AutoParticipantFilters"] = dtSearchedFilters;
        }


        private void SaveAutoParticipant(int autoPartId, string marketingOwner, string weaSalesLabel, string artist, string projectTitle)
        {
            string userCode = Convert.ToString(Session["UserCode"]);
            marketingOwner = marketingOwner == "" ? "-" : marketingOwner.Trim().ToUpper();
            weaSalesLabel = weaSalesLabel == "" ? "-" : weaSalesLabel.Trim().ToUpper();
            int artistId = artist == "" ? -1 : Convert.ToInt32(artist.Split('-')[0].Trim());
            int projectId = projectTitle == "" ? -1 : Convert.ToInt32(projectTitle.Split('-')[0].Trim());
            //projectTitle = projectTitle == "" ? "-" : projectTitle.Split('-')[1].Trim();
            projectTitle = projectTitle == "" ? "-" : projectTitle.Substring((projectTitle.IndexOf('-') + 2), (projectTitle.Length - projectTitle.IndexOf('-') - 2));

            //JIRA-1048 --Changes to handle single quote -- Start
            string marketingOnwerSearch = txtMarketingOwner.Text.Replace("'", "").Trim();
            string weaSalesLabelSearch = txtWEASalesLabel.Text.Replace("'", "").Trim();
            string artistSearch = txtArtist.Text.Replace("'", "").Trim();
            string projectSearch = txtProject.Text.Replace("'", "").Trim();
            //JIRA-1048 --Changes to handle single quote -- End

            autoParticipantSearchBL = new AutoParticipantSearchBL();
            DataSet autoParticipantData = autoParticipantSearchBL.AddUpdateAutoParticipant(autoPartId, marketingOwner, weaSalesLabel,
                artistId, projectId, projectTitle, marketingOnwerSearch, weaSalesLabelSearch, artistSearch, projectSearch, userCode, out newAutoPartId, out errorId);
            autoParticipantSearchBL = null;
            if (errorId == 0)
            {
                if (autoPartId == 0 && newAutoPartId != 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "NewAutoParticipSave", "RedirectToAutoParticipMaint(" + newAutoPartId + ");", true);
                }
                else
                {
                    if (autoParticipantData.Tables[0].Rows.Count == 0 && errorId != 2)
                    {
                        dtEmpty = new DataTable();
                        gvAutoParticipantDtls.EmptyDataText = "No data found for the selected filter criteria";
                        BindGrid(autoParticipantData.Tables[0]);
                        Session["AutoParticipantData"] = null;
                    }
                    else
                    {
                        Session["AutoParticipantData"] = autoParticipantData.Tables[0];
                        BindGrid(autoParticipantData.Tables[0]);
                    }

                    msgView.SetMessage("Participation Group updated successfully.", MessageType.Success, PositionType.Auto);

                }
            }
            else if (errorId == 1)
            {
                msgView.SetMessage("This Participant Group already exists.", MessageType.Warning, PositionType.Auto);
                return;
            }
            else
            {
                msgView.SetMessage("Failed to save Participation Group.", MessageType.Warning, PositionType.Auto);
                return;
            }

        }

        private void SeachAutoParticipantData()
        {
            AutoParticipantSession();
            autoParticipantSearchBL = new AutoParticipantSearchBL();
            //JIRA-1048 --Changes to handle single quote -- Start
            DataSet autoPartData = autoParticipantSearchBL.SearchAutoParticipantData(txtMarketingOwner.Text.Replace("'", "").Trim(), txtWEASalesLabel.Text.Replace("'", "").Trim(), txtArtist.Text.Replace("'", "").Trim(), txtProject.Text.Replace("'", "").Trim(), out errorId);
            //JIRA-1048 --Changes to handle single quote -- End
            autoParticipantSearchBL = null;
            if (autoPartData.Tables.Count != 0 && errorId != 2)
            {
                if (autoPartData.Tables[0].Rows.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvAutoParticipantDtls.EmptyDataText = "No data found for the selected filter criteria";
                    BindGrid(autoPartData.Tables[0]);
                    Session["AutoParticipantData"] = null;
                }
                else
                {
                    Session["AutoParticipantData"] = autoPartData.Tables[0];
                    BindGrid(autoPartData.Tables[0]);
                    if (autoPartData.Tables[0].Rows.Count == 1 && hdnIsNewRequest.Value == "Y")
                    {
                        Response.Redirect("../Participants/AutoParticipantMaintenance.aspx?autoPartId=" + autoPartData.Tables[0].Rows[0]["auto_participant_id"].ToString() + "", false);
                    }

                }

            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnAddParticipant.Enabled = false;
                imgBtnAddRowUndo.Enabled = false;
                btnSaveChanges.Enabled = false;

                foreach (GridViewRow rows in gvAutoParticipantDtls.Rows)
                {
                    (rows.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                }

            }

        }


        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }
        #endregion Methods

        #region Fuzzy Search

        protected void btnFuzzyArtistListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                FuzzySearchArtist(hdnFuzzySearchText.Value);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Artist fuzzy search popup", ex.Message);
            }

        }

        protected void btnFuzzyProjectTitleListPopup_Click(object sender, EventArgs e)
        {
            try
            {
                FuzzySearchProjectTitle(hdnFuzzySearchText.Value);
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in Artist fuzzy search popup", ex.Message);
            }

        }

        private void FuzzySearchProjectTitle(string searchText)
        {

            lblFuzzySearchPopUp.Text = "Project - Complete Search List";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllProjectList(searchText.ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();

        }

        private void FuzzySearchArtist(string searchText)
        {

            lblFuzzySearchPopUp.Text = "Artist - Complete Search List";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllArtisList(searchText.ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();

        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "ArtistAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtArtistAddRow.Text = string.Empty;
                        return;
                    }

                    txtArtistAddRow.Text = lbFuzzySearch.SelectedValue.ToString();
                    hdnFuzzyAddRowValidator.Value = "N";
                    hdnIsValidArtist.Value = "Y";
                }
                else if (hdnFuzzySearchField.Value == "ProjectTitleAddRow")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtProjectTitleAddRow.Text = string.Empty;
                        return;
                    }

                    txtProjectTitleAddRow.Text = lbFuzzySearch.SelectedValue.ToString();
                    hdnFuzzyAddRowValidator.Value = "N";
                    hdnIsValidProject.Value = "Y";
                }
                else if (hdnFuzzySearchField.Value == "ArtistGridRow")
                {
                    TextBox txtArtistGridRow;
                    foreach (GridViewRow gvr in gvAutoParticipantDtls.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnGridFuzzySearchRowId.Value)
                        {
                            txtArtistGridRow = (gvr.FindControl("txtArtistGridRow") as TextBox);
                            if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                            {
                                txtArtistGridRow.Text = string.Empty;
                                txtArtistGridRow.ToolTip = string.Empty;
                                return;
                            }

                            txtArtistGridRow.Text = lbFuzzySearch.SelectedValue.ToString();
                            txtArtistGridRow.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                            hdnFuzzyGridRowValidator.Value = "N";
                            hdnIsValidArtistGridRow.Value = "Y";
                            break;
                        }
                    }

                    ScriptManager.RegisterStartupScript(this, typeof(Page), "CompareGrid", "CompareGridData(" + Convert.ToInt32(hdnGridFuzzySearchRowId.Value) + ");", true);

                }
                else if (hdnFuzzySearchField.Value == "ProjectTitleGridRow")
                {
                    TextBox txtProjectTitleGridRow;
                    foreach (GridViewRow gvr in gvAutoParticipantDtls.Rows)
                    {
                        if (gvr.RowIndex.ToString() == hdnGridFuzzySearchRowId.Value)
                        {
                            txtProjectTitleGridRow = (gvr.FindControl("txtProjectTitleGridRow") as TextBox);
                            if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                            {
                                txtProjectTitleGridRow.Text = string.Empty;
                                txtProjectTitleGridRow.ToolTip = string.Empty;
                                return;
                            }

                            txtProjectTitleGridRow.Text = lbFuzzySearch.SelectedValue.ToString();
                            txtProjectTitleGridRow.ToolTip = lbFuzzySearch.SelectedValue.ToString();
                            hdnFuzzyGridRowValidator.Value = "N";
                            hdnIsValidProjectGridRow.Value = "Y";
                            break;
                        }
                    }

                    ScriptManager.RegisterStartupScript(this, typeof(Page), "CompareGrid", "CompareGridData(" + Convert.ToInt32(hdnGridFuzzySearchRowId.Value) + ");", true);

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting item from list", ex.Message);
            }
        }


        #endregion Fuzzy Search
    }
}