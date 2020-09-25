/*
File Name   :   MissingParticipants.cs
Purpose     :   to search products 

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     30-May-2016     Pratik(Infosys Limited)   Initial Creation
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

namespace WARS
{
    public partial class CatalogueSearch : System.Web.UI.Page
    {
        #region Global Declarations
        CatalogueSearchBL catalogueSearchBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Catalogue Search";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Catalogue Search";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlCatalogueDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtCatalogueNo.Focus();
                    Session["CatSrchCatDetails"] = null;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {

                        btnUpdateStatus.Enabled = false;
                        LoadData();
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

        protected void gvCatalogueDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[5].Visible = false;
                    LinkButton _dblClickButton = e.Row.FindControl("lnkBtnDblClk") as LinkButton;
                    string _jsDoubleClick = ClientScript.GetPostBackClientHyperlink(_dblClickButton, "");
                    e.Row.Attributes.Add("ondblclick", _jsDoubleClick);
                }

                //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[5].Visible = false;
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
        protected void gvCatalogueDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["CatSrchCatDetails"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvCatalogueDetails.PageIndex = 0;
                gvCatalogueDetails.DataSource = dataView;
                gvCatalogueDetails.DataBind();

                Session["CatSrchCatDetails"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvCatalogueDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                if (e.CommandName == "dblClk")
                {
                    int rowIndex = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    string catNo = ((Label)gvCatalogueDetails.Rows[rowIndex].FindControl("lblCatNo")).Text;

                    Response.Redirect("../Participants/CatalogueMaintenance.aspx?catNo=" + catNo + "", false);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to catalogue maintenance screen.", ex.Message);
            }
        }

        protected void gvCatalogueDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                if (Session["CatSrchCatDetails"] == null)
                    return;

                DataTable dtCatalogueDetails = Session["CatSrchCatDetails"] as DataTable;

                gvCatalogueDetails.PageIndex = e.NewPageIndex;
                BindGrid(dtCatalogueDetails);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        protected void btnAddCatDetails_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Participants/CatalogueMaintenance.aspx", false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to catalogue maintenance screen.", ex.Message);
            }
        }

        protected void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtCatNo = Session["CatSrchCatDetails"] as DataTable;

                if (dtCatNo == null || dtCatNo.Rows.Count == 0)
                {
                    msgView.SetMessage("No catalogues available to update!", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    ddlUpdateStatus.SelectedIndex = 0;
                    mpeUpdateStatus.Show();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating catalogue status", ex.Message);
            }
        }

        /// <summary>
        /// Bulk update catalogue status
        /// </summary>        
        protected void bthUpdateCatStatusPopup_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateCatalogueStatus(false, false, false);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating catalogue status", ex.Message);
            }
        }

        /// <summary>
        /// Bulk update catalogue status on Confirmation pop up
        /// </summary>        
        protected void btnUpdateCatStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnYesConfirm.Text == "Yes" && btnYesConfirm.CommandName == "Participants")
                {
                    hdnOverrideInvalidCatnoUpdate.Value = "Y";
                }
                else if (btnYesConfirm.Text == "Confirm")
                {
                    hdnOverrideParticipUpdate.Value = "Y";
                }
                else if (btnYesConfirm.Text == "Yes" && btnYesConfirm.CommandName == "Tracks") //WUIN-1005
                {
                    hdnOverrideTrackStatus0.Value = "Y";
                }

                bool overrideInvalidCatnoUpdate = hdnOverrideInvalidCatnoUpdate.Value == "Y" ? true : false;
                bool overrideParticipUpdate = hdnOverrideParticipUpdate.Value == "Y" ? true : false;
                bool overrideTrackStatus0 = hdnOverrideTrackStatus0.Value == "Y" ? true : false;

                UpdateCatalogueStatus(overrideInvalidCatnoUpdate, overrideParticipUpdate, overrideTrackStatus0);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating catalogue status", ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //Making this hidden field value to Y so that if we get a single record after search then it will redirect to next screen
                hdnIsNewRequest.Value = "Y";

                SearchData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in search.", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                txtCatalogueNo.Text = string.Empty;
                txtArtist.Text = string.Empty;
                txtTitle.Text = string.Empty;
                ddlConfiguration.SelectedIndex = 0;
                txtISRC.Text = string.Empty;
                ddlTeamResponsibility.SelectedIndex = 0;
                ddlTeamResponsibilitybyTrack.SelectedIndex = 0;
                ddlManagerResponsibility.SelectedIndex = 0;
                ddlManagerResponsibilitybyTrack.SelectedIndex = 0;
                ddlCatnoStatus.SelectedIndex = 0;
                ddlTrackStatus.SelectedIndex = 0;
                Session["CatSrchCatDetails"] = null;
                Session["CSSearchedFilters"] = null;
                dtEmpty = new DataTable();
                gvCatalogueDetails.EmptyDataText = "<br/>";
                BindGrid(dtEmpty);


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reseting.", ex.Message);
            }
        }

        protected void btnSearchBulkUpload_Click(object sender, EventArgs e)
        {
            try
            {
                //validate catnos entered 
                string[] arrCatNos = txtUploadPrdList.Text.ToUpper().Trim().Replace("\r\n", string.Empty).Split(';').ToArray();
                foreach (string catNo in arrCatNos)
                {
                    //check if entered cataloge is greater than the CATNO.CATNO column definition
                    if (catNo.Length > 30)
                    {
                        lblUploadPrdListError.Visible = true;
                        mpeUploadPrdList.Show();
                        return;

                    }
                }

                //Making this hidden field value to Y so that if we get a single record after search then it will redirect to next screen
                hdnIsNewRequest.Value = "Y";

                //clear search fields
                txtCatalogueNo.Text = string.Empty;
                txtArtist.Text = string.Empty;
                txtTitle.Text = string.Empty;
                ddlConfiguration.SelectedIndex = 0;
                txtISRC.Text = string.Empty;
                ddlTeamResponsibility.SelectedIndex = 0;
                ddlTeamResponsibilitybyTrack.SelectedIndex = 0;
                ddlManagerResponsibilitybyTrack.SelectedIndex = 0;
                ddlManagerResponsibility.SelectedIndex = 0;
                ddlCatnoStatus.SelectedIndex = 0;
                ddlTrackStatus.SelectedIndex = 0;

                SearchData();

                txtUploadPrdList.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in searching uploaded catalogue numbers", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {
            dtEmpty = new DataTable();
            catalogueSearchBL = new CatalogueSearchBL();
            DataSet initialData = catalogueSearchBL.GetInitialData(out errorId);
            gvCatalogueDetails.EmptyDataText = "<br />";
            gvCatalogueDetails.DataSource = dtEmpty;
            gvCatalogueDetails.DataBind();

            ddlConfiguration.DataTextField = "config_name";
            ddlConfiguration.DataValueField = "config_code";
            ddlConfiguration.DataSource = initialData.Tables[0];
            ddlConfiguration.DataBind();
            ddlConfiguration.Items.Insert(0, new ListItem("-", null));

            ddlTeamResponsibility.DataTextField = "responsibility_desc";
            ddlTeamResponsibility.DataValueField = "responsibility_code";
            ddlTeamResponsibility.DataSource = initialData.Tables[1];
            ddlTeamResponsibility.DataBind();
            ddlTeamResponsibility.Items.Insert(0, new ListItem("-", null));

            ddlTeamResponsibilitybyTrack.DataTextField = "responsibility_desc";
            ddlTeamResponsibilitybyTrack.DataValueField = "responsibility_code";
            ddlTeamResponsibilitybyTrack.DataSource = initialData.Tables[1];
            ddlTeamResponsibilitybyTrack.DataBind();
            ddlTeamResponsibilitybyTrack.Items.Insert(0, new ListItem("-", null));

            ddlManagerResponsibility.DataTextField = "responsibility_desc";
            ddlManagerResponsibility.DataValueField = "responsibility_code";
            ddlManagerResponsibility.DataSource = initialData.Tables[1];
            ddlManagerResponsibility.DataBind();
            ddlManagerResponsibility.Items.Insert(0, new ListItem("-", null));

            ddlManagerResponsibilitybyTrack.DataTextField = "responsibility_desc";
            ddlManagerResponsibilitybyTrack.DataValueField = "responsibility_code";
            ddlManagerResponsibilitybyTrack.DataSource = initialData.Tables[1];
            ddlManagerResponsibilitybyTrack.DataBind();
            ddlManagerResponsibilitybyTrack.Items.Insert(0, new ListItem("-", null));

            ddlCatnoStatus.DataTextField = "item_text";
            ddlCatnoStatus.DataValueField = "item_value";
            ddlCatnoStatus.DataSource = initialData.Tables[2];
            ddlCatnoStatus.DataBind();
            ddlCatnoStatus.Items.Insert(0, new ListItem("-", null));

            ddlTrackStatus.DataTextField = "item_text";
            ddlTrackStatus.DataValueField = "item_value";
            ddlTrackStatus.DataSource = initialData.Tables[3];
            ddlTrackStatus.DataBind();
            ddlTrackStatus.Items.Insert(0, new ListItem("-", null));

            ddlUpdateStatus.DataTextField = "item_text";
            ddlUpdateStatus.DataValueField = "item_value";
            ddlUpdateStatus.DataSource = initialData.Tables[2];
            ddlUpdateStatus.DataBind();
            ddlUpdateStatus.Items.Insert(0, new ListItem("-"));

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                hdnIsNewRequest.Value = Request.QueryString[0];

                if (hdnIsNewRequest.Value == "N")
                {
                    if (Session["CSSearchedFilters"] != null)
                    {
                        DataTable dtSearchedFilters = Session["CSSearchedFilters"] as DataTable;

                        foreach (DataRow dRow in dtSearchedFilters.Rows)
                        {
                            if (dRow["filter_name"].ToString() == "txtCatalogueNo")
                            {
                                txtCatalogueNo.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtTitle")
                            {
                                txtTitle.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtArtist")
                            {
                                txtArtist.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlConfiguration")
                            {
                                ddlConfiguration.SelectedValue = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtISRC")
                            {
                                txtISRC.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlTeamResponsibility")
                            {
                                ddlTeamResponsibility.SelectedValue = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlTeamResponsibilitybyTrack")
                            {
                                ddlTeamResponsibilitybyTrack.SelectedValue = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlManagerResponsibility")
                            {
                                ddlManagerResponsibility.SelectedValue = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlManagerResponsibilitybyTrack")
                            {
                                ddlManagerResponsibilitybyTrack.SelectedValue = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlCatnoStatus")
                            {
                                ddlCatnoStatus.SelectedValue = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlTrackStatus")
                            {
                                ddlTrackStatus.SelectedValue = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtUploadPrdList")
                            {
                                txtUploadPrdList.Text = dRow["filter_value"].ToString();
                            }
                        }

                        SearchData();
                    }
                }
            }
            else
            {
                Session["CSSearchedFilters"] = null;
            }
        }

        private void SearchData()
        {

            //Create a table to hold the filter values
            DataTable dtSearchedFilters = new DataTable();
            dtSearchedFilters.Columns.Add("filter_name", typeof(string));
            dtSearchedFilters.Columns.Add("filter_value", typeof(string));

            //Add the filter values to the above created table
            dtSearchedFilters.Rows.Add("txtCatalogueNo", txtCatalogueNo.Text);
            dtSearchedFilters.Rows.Add("txtTitle", txtTitle.Text);
            dtSearchedFilters.Rows.Add("txtArtist", txtArtist.Text);
            dtSearchedFilters.Rows.Add("ddlConfiguration", ddlConfiguration.SelectedValue);
            dtSearchedFilters.Rows.Add("txtISRC", txtISRC.Text);
            dtSearchedFilters.Rows.Add("ddlTeamResponsibility", ddlTeamResponsibility.SelectedValue);
            dtSearchedFilters.Rows.Add("ddlTeamResponsibilitybyTrack", ddlTeamResponsibilitybyTrack.SelectedValue);
            dtSearchedFilters.Rows.Add("ddlManagerResponsibility", ddlManagerResponsibility.SelectedValue);
            dtSearchedFilters.Rows.Add("ddlManagerResponsibilitybyTrack", ddlManagerResponsibilitybyTrack.SelectedValue);
            dtSearchedFilters.Rows.Add("ddlCatnoStatus", ddlCatnoStatus.SelectedValue);
            dtSearchedFilters.Rows.Add("ddlTrackStatus", ddlTrackStatus.SelectedValue);
            dtSearchedFilters.Rows.Add("txtUploadPrdList", txtUploadPrdList.Text);

            Session["CSSearchedFilters"] = dtSearchedFilters;

            catalogueSearchBL = new CatalogueSearchBL();
            //JIRA-1048 Changes to handle single quote while searching --Start
            DataSet searchedData = catalogueSearchBL.GetSearchedCatData(txtCatalogueNo.Text.Replace("'", "").Trim().ToUpper(), txtTitle.Text.Replace("'", "").Trim(),
                                    txtArtist.Text.Replace("'", "").Trim(), ddlConfiguration.SelectedValue, txtISRC.Text.Replace("'", "").Trim(), ddlTeamResponsibility.SelectedValue, ddlTeamResponsibilitybyTrack.SelectedValue, ddlManagerResponsibility.SelectedValue, ddlManagerResponsibilitybyTrack.SelectedValue,
                                    ddlCatnoStatus.SelectedValue, ddlTrackStatus.SelectedValue, txtUploadPrdList.Text.ToUpper().Trim().Replace("\r\n", string.Empty).Split(';').ToArray(), out errorId);
            //JIRA-1048 Changes to handle single quote while searching --End
            catalogueSearchBL = null;

            if (searchedData.Tables.Count != 0 && errorId != 2)
            {
                Session["CatSrchCatDetails"] = searchedData.Tables[0];
                gvCatalogueDetails.PageIndex = 0;
                if (searchedData.Tables[0].Rows.Count == 0)
                {
                    gvCatalogueDetails.EmptyDataText = "No data found for the selected filter criteria";
                }
                BindGrid(searchedData.Tables[0]);
                if (searchedData.Tables[0].Rows.Count == 1 && hdnIsNewRequest.Value == "Y")
                {
                    Response.Redirect("../Participants/CatalogueMaintenance.aspx?catNo=" + searchedData.Tables[0].Rows[0]["catno"].ToString() + "", false);
                }

                //WUIN-710 - reset bulk update flag
                hdnOverrideInvalidCatnoUpdate.Value = "N";
                hdnOverrideParticipUpdate.Value = "N";

            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        { 
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (gridData.Rows.Count > 0)
            {
                gvCatalogueDetails.DataSource = gridData;
                gvCatalogueDetails.DataBind();
            }
            else
            {
                dtEmpty = new DataTable();
                gvCatalogueDetails.DataSource = dtEmpty;
                gvCatalogueDetails.DataBind();
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        /// <summary>
        /// updates the status of catalogues fetched out of search criteria -
        /// (not just the one displayed in the current page of the gridbut all the catalogues fetched)
        /// </summary>
        private void UpdateCatalogueStatus(bool overrideInvalidCatnoUpdate, bool overrideParticipUpdate, bool overrideTrackStatus0)
        {
            bool isParticipUpdate;
            bool isTrackStatus0;
            Array UpdateCatnoList;

            if (ValidateCatToUpdateStatus(overrideInvalidCatnoUpdate, overrideParticipUpdate, overrideTrackStatus0, out isParticipUpdate, out isTrackStatus0, out UpdateCatnoList))
            {
                if (UpdateCatnoList.Length > 0)
                {
                    catalogueSearchBL = new CatalogueSearchBL();
                    //JIRA-1048 --Changes to handle single quote -- Start
                    DataSet searchedData = catalogueSearchBL.UpdateCatalogueDetails(txtCatalogueNo.Text.Replace("'", "").Trim().ToUpper(), txtTitle.Text.Replace("'", "").Trim(),
                                                    txtArtist.Text.Replace("'", "").Trim(), ddlConfiguration.SelectedValue, txtISRC.Text.Replace("'", "").Trim(), ddlTeamResponsibility.SelectedValue, ddlTeamResponsibilitybyTrack.SelectedValue,
                                                    ddlManagerResponsibility.SelectedValue, ddlManagerResponsibilitybyTrack.SelectedValue, ddlCatnoStatus.SelectedValue, ddlUpdateStatus.SelectedValue, ddlTrackStatus.SelectedValue, CatNoListForParticips(), UpdateCatnoList, Convert.ToString(Session["UserCode"]),
                                                    out errorId);
                    //JIRA-1048 --Changes to handle single quote --ENd
                    if (searchedData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["CatSrchCatDetails"] = searchedData.Tables[0];
                        gvCatalogueDetails.PageIndex = 0;
                        BindGrid(searchedData.Tables[0]);
                        msgView.SetMessage("Bulk status update successful", MessageType.Success, PositionType.Auto); // WUIN-1234
                    }
                    else
                    {
                        ExceptionHandler("Error in updating catalogue status", string.Empty);
                    }

                }
            }
            else
            {
                if (isTrackStatus0) //WUIN-1005 
                {
                    lblMessage.Text = "There are products in the list where some of the tracks have a status of No participants. Do you want to continue?";
                    btnYesConfirm.Text = "Yes";
                    btnNoConfirm.Text = "No";
                    btnYesConfirm.CommandName = "Tracks";
                }
                else
                {
                    //If the Participants will be updated, a message will be displayed ‘This will update the Status of all Participants and Tracks to this Catalogue Status. Confirm or Cancel’
                    if (isParticipUpdate)
                    {
                        lblMessage.Text = "This will update the Status of all Participants and Tracks to this Catalogue Status";
                        btnYesConfirm.Text = "Confirm";
                        btnNoConfirm.Text = "Cancel";
                    }
                    else
                    {
                        lblMessage.Text = "One or more catalogue status cannot be updated. Do you want to continue for the rest?";
                        btnYesConfirm.Text = "Yes";
                        btnNoConfirm.Text = "No";
                        btnYesConfirm.CommandName = "Participants";
                    }
                }

                mpeConfirmation.Show();
            }

        }

        /// <summary>        
        ///1.No update allowed if (legacy = 'Y' and no active Participants (PARTICIPATION_TYPE = 'A' and end_date null))  OR (legacy = 'N' and no active Track Participants(ISRC participants))       
        ///2. When updating the CATNO.STATUS_CODE,  									
        ///     1.The check for updating only one level at a time will be removed (eg will be able to update from Under Review to Manager Sign Off)									
        //      2.Participant Status will be updated to the Product Status if the existing Participant Status is < new Product Status									
        ///     3.Participant Status will not be updated to the Product Status if the existing Participant Status is > new Product Status									
        ///     4.Only update Participant Status if no end date 									
        //      5.If not legacy, Track Status will be updated to the Product Status if the existing Track Status is < new Product Status									
        ///     6.If not legacy, Track Status will not be updated to the Product Status if the existing Track Status is > new Product Status	
        ///3.If Status is moved from Manager Sign Off (3) then 						
        ///     display warning 'This update will prevent the generation of Statement details for all Participants'
        ///4.If the Participants will be updated, a message will be displayed ‘This will update the Status of all Participants and Tracks to this Catalogue Status. Confirm or Cancel’
        ///5. (WUIN-909) Cannot update status to 'No Participants' if there are active participants
        ///      if legacy = 'Y' and active Participants (PARTICIPATION_TYPE = 'A' and end_date null)
        ///      if legacy = 'N' and active Track Participants(ISRC participants)
        ///6. WUIN-1005 - if any tracks with status 'No Participants' found for the catno's for which status is being updated, display confirm box.     
        /// </summary>
        private bool ValidateCatToUpdateStatus(bool overrideInvalidCatnoUpdate, bool overrideParticipUpdate, bool overrideTrackStatus0, out bool isParticipUpdate, out bool isTrackStatus0, out Array UpdateCatnoList)
        {
            List<string> UpdateCatnos = new List<string>();
            UpdateCatnoList = UpdateCatnos.ToArray();
            string catno;
            string legacy;
            bool isValid;
            isParticipUpdate = false;
            isTrackStatus0 = false;
            DataTable dtCatDetails = Session["CatSrchCatDetails"] as DataTable;

            if (ddlUpdateStatus.SelectedIndex == 0)
            {
                msgView.SetMessage("Please select valid catalogue status", MessageType.Success, PositionType.Auto);
                return false;
            }

            if (dtCatDetails == null || dtCatDetails.Rows.Count == 0)
            {
                msgView.SetMessage("No catalogues to update the status", MessageType.Success, PositionType.Auto);
                return false;
            }

            //get list of participants and Track participants (ISRC participants) and tracks of the catnos            
            DataTable dtParticips;
            DataTable dtTrackParticips;
            DataTable dtTracks;

            catalogueSearchBL = new CatalogueSearchBL();
            DataSet dsCatnoParticipants = catalogueSearchBL.GetCatnoParticipants(CatNoListForParticips(), out errorId);
            catalogueSearchBL = null;

            if (errorId == 2 || dsCatnoParticipants.Tables.Count == 0)
            {
                ExceptionHandler("Error in fetching participation details of the selected catalogues", string.Empty);
                return false;
            }

            dtParticips = dsCatnoParticipants.Tables[0];
            dtTrackParticips = dsCatnoParticipants.Tables[1];
            dtTracks = dsCatnoParticipants.Tables[2]; //WUIN-1005
            // WUIN-1005 - No update allowed if status Participants and Tracks is No Participants.           
            if (!overrideTrackStatus0)
            {
                foreach (DataRow drCat in dtCatDetails.Rows)
                {
                    isTrackStatus0 = false;
                    catno = drCat["catno"].ToString();
                    legacy = drCat["legacy"].ToString();
                    if (legacy == "N" && (dtTracks.Select("catno = '" + catno + "' AND status_code = 0").Count() > 0))
                    {
                        isTrackStatus0 = true;
                        if (!overrideTrackStatus0)
                            return false;
                    }
                }
            }

            foreach (DataRow drCat in dtCatDetails.Rows)
            {
                isValid = true;
                isParticipUpdate = false;
                isTrackStatus0 = false;
                catno = drCat["catno"].ToString();
                legacy = drCat["legacy"].ToString();


                //1.No update allowed if (legacy = 'Y' and no active Participants (PARTICIPATION_TYPE = 'A' and end_date null))  OR (legacy = 'N' and no active Track Participants(ISRC participants))                
                if ((legacy == "Y" && (dtParticips.Select("catno = '" + catno + "' AND participation_type = 'A' AND end_date IS NULL").Count() == 0)) ||
                    (legacy == "N" && (dtTrackParticips.Select("catno = '" + catno + "' AND active = 'Y'").Count() == 0)))
                {
                    isValid = false;

                    if (!overrideInvalidCatnoUpdate)
                        return false;
                }

                //5.Cannot update status to 'No Participants' if there are active participants
                if (ddlUpdateStatus.SelectedValue == "0" &&
                    ((legacy == "Y" && (dtParticips.Select("catno = '" + catno + "' AND participation_type = 'A' AND end_date IS NULL").Count() > 0)) ||
                    (legacy == "N" && (dtTrackParticips.Select("catno = '" + catno + "' AND active = 'Y'").Count() > 0))))
                {
                    isValid = false;

                    if (!overrideInvalidCatnoUpdate)
                        return false;
                }

                //4.If the Participants(either participation participants or if not legacy and isrc participants) will be updated, 
                //a message will be displayed ‘This will update the Status of all Participants and Tracks to this Catalogue Status. Confirm or Cancel’
                if ((dtParticips.Select("catno = '" + catno + "' AND status_code < " + ddlUpdateStatus.SelectedValue + " AND end_date IS NULL").Count() > 0) ||
                    (legacy == "N" && (dtTrackParticips.Select("catno = '" + catno + "' AND status_code < " + ddlUpdateStatus.SelectedValue).Count() > 0))
                    )
                {
                    isParticipUpdate = true;

                    if (!overrideParticipUpdate)
                        return false;
                }


                if (isValid)
                {
                    UpdateCatnos.Add(catno + Global.DBDelimiter + legacy);
                }

            }

            UpdateCatnoList = UpdateCatnos.ToArray();

            //4.If Status is moved from Manager Sign Off (3) then 						
            //    display warning 'This update will prevent the generation of Statement details for all Participants'
            // (if selected status is not Manager sign off, display warning if any catalogue is Manager signed off)
            if ((ddlUpdateStatus.SelectedValue != "3" && ddlUpdateStatus.SelectedValue != "0") && (dtCatDetails.Select("status_code = 3").Count() > 0))
            {
                msgView.SetMessage("This update will prevent the generation of Statement details for all Participants of catalogues that are Manager signed off!",
                                    MessageType.Success, PositionType.Auto);
            }

            return true;

        }

        private Array CatNoListForParticips()
        {
            DataTable dtCatDetailsForParticips = Session["CatSrchCatDetails"] as DataTable;
            List<string> catNoList = new List<string>();

            foreach (DataRow dr in dtCatDetailsForParticips.Rows)
            {
                catNoList.Add(dr["catno"].ToString());
            }

            return catNoList.ToArray();

        }

        private void UserAuthorization()
        {
            //Validation:
            //JIRA-983 - Only SuperUser and Supervisor can select Update Status
            //WUIN-1096 Only Read access for Reaonly User
            if ((Session["UserRole"].ToString().ToLower() == UserRole.SuperUser.ToString().ToLower()) || (Session["UserRole"].ToString().ToLower() == UserRole.Supervisor.ToString().ToLower()))
            {
                hdnIsSuperUser.Value = "Y";
                btnUpdateStatus.Enabled = true;
            }
            else if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnAddCatDetails.Enabled = false;
                btnUpdateStatus.Enabled = false;

            }

        }

        #endregion Methods



    }
}