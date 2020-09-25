/*
File Name   :   MissingParticipants.cs
Purpose     :   to search products with missing participants

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     25-May-2016     Pratik(Infosys Limited)   Initial Creation
2.0     15-Mar-2018     Harish                    WUIN-523 - Change Missing Participants grid setup
3.0     16-Mar-2018     Harish                    WUIN-502 - Use Legacy flag in selection criteria for Track Listing or Participant Maintenance
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
    public partial class MissingParticipants : System.Web.UI.Page
    {
        #region Global Declarations

        MissingParticipantsBL missingParticipantsBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize1"].ToString());
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Missing Participants";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Missing Participants";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlCatalogueDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtCatalogueNo.Focus();
                    Session["MissingPartcipantsData"] = null;
                    tdData.Style.Add("display", "none");

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
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
                txtStartDate.Text = string.Empty;
                txtEndDate.Text = string.Empty;
                txtValueThreshold.Text = string.Empty;
                //JIRA-911 Changes done by Ravi on  31-01-2019 -- Start
                txtISRC.Text = string.Empty;
                //JIRA-911 Changes done by Ravi on  31-01-2019 -- End

                ddlTeamResponsibility.SelectedIndex = 0;
                ddlTeamResponsibilitybyTrackLevel.SelectedIndex = 0;
                ddlManagerResponsibility.SelectedIndex = 0;
                ddlManagerResponsibilitybyTrackLevel.SelectedIndex = 0;
                ddlCatnoStatus.SelectedIndex = 0;
                ddlTrackStatus.SelectedIndex = 0;

                ddlCompareStartDate.SelectedValue = "=";
                ddlCompareEndDate.SelectedValue = "=";

                //gvCatalogueDetails.PageIndex = 0;

                Session["MPSearchedFilters"] = null;
                Session["MissingPartcipantsData"] = null;

                dtEmpty = new DataTable();
                BindGrid(dtEmpty);

                //missingParticipantsBL = new MissingParticipantsBL();
                //DataSet initialData = missingParticipantsBL.GetInitialData(out errorId);
                //missingParticipantsBL = null;

                //if (initialData.Tables.Count != 0 && errorId != 2)
                //{
                //    Session["MissingPartcipantsData"] = initialData.Tables[0];

                //    gvCatalogueDetails.PageIndex = 0;
                //    BindGrid(initialData.Tables[0]);

                //    ddlTeamResponsibility.DataSource = initialData.Tables[1];
                //    ddlTeamResponsibility.DataTextField = "responsibility_desc";
                //    ddlTeamResponsibility.DataValueField = "responsibility_code";
                //    ddlTeamResponsibility.DataBind();
                //    ddlTeamResponsibility.Items.Insert(0, new ListItem("-"));

                //    ddlManagerResponsibility.DataSource = initialData.Tables[1];
                //    ddlManagerResponsibility.DataTextField = "responsibility_desc";
                //    ddlManagerResponsibility.DataValueField = "responsibility_code";
                //    ddlManagerResponsibility.DataBind();
                //    ddlManagerResponsibility.Items.Insert(0, new ListItem("-"));

                //    ddlStatus.DataSource = initialData.Tables[2];
                //    ddlStatus.DataTextField = "status_desc";
                //    ddlStatus.DataValueField = "status_code";
                //    ddlStatus.DataBind();
                //    ddlStatus.Items.Insert(0, new ListItem("-"));
                //}
                //else
                //{
                //    ExceptionHandler("Error in fetching data", string.Empty);
                //}


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reseting.", ex.Message);
            }
        }

        //protected void gvCatalogueDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    try
        //    {
        //        if (Session["MissingPartcipantsData"] == null)
        //            return;

        //        DataTable dtMissingPartcipantsData = Session["MissingPartcipantsData"] as DataTable;

        //        gvCatalogueDetails.PageIndex = e.NewPageIndex;
        //        BindGrid(dtMissingPartcipantsData);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler("Error in page change.", ex.Message);
        //    }
        //}

        protected void gvCatalogueDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[8].Visible = false;
                    LinkButton _dblClickButton = e.Row.FindControl("lnkBtnDblClk") as LinkButton;
                    string _jsDoubleClick = ClientScript.GetPostBackClientHyperlink(_dblClickButton, "");
                    e.Row.Attributes.Add("ondblclick", _jsDoubleClick);
                    Label transactionvalue = e.Row.FindControl("lblTransactionValue") as Label;
                    transactionvalue.Text = Convert.ToDecimal(transactionvalue.Text).ToString("0.00");
                }
                
                //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[8].Visible = false;
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
            DataTable dataTable = (DataTable)Session["MissingPartcipantsData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                PopulateGridPage(1, dataView.ToTable());
                Session["MissingPartcipantsData"] = dataView.ToTable();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvCatalogueDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "dblClk")
                {
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    string catNo = (row.FindControl("lblCatNo") as Label).Text;
                    //string trackListingId = (row.FindControl("hdnTrackListingId") as HiddenField).Value;

                    string catnoLegacy = string.Empty;
                    missingParticipantsBL = new MissingParticipantsBL();
                    string isTrackListingExist = missingParticipantsBL.IsTrackListingExist(catNo, out catnoLegacy, out errorId);
                    missingParticipantsBL = null;

                    //WUIN-502 - change
                    //display Track Listing screen if CATNO.LEGACY = 'N' and Track Listing exists for CATNO
                    //Else display Catalogue Maintenance	
                    if (isTrackListingExist == "Y" && catnoLegacy == "N")
                    {
                        Response.Redirect("../Participants/TrackListing.aspx?CatNo=" + catNo + "", false);
                    }
                    else
                    {
                        Response.Redirect("../Participants/CatalogueMaintenance.aspx?CatNo=" + catNo + "", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to track listing screen.", ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Invalid search input!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //Making this hidden field value to Y so that if we get a single record after search then it will redirect to next screen
                hdnIsNewRequest.Value = "Y";

                //Create a table to hold the filter values
                DataTable dtSearchedFilters = new DataTable();
                dtSearchedFilters.Columns.Add("filter_name", typeof(string));
                dtSearchedFilters.Columns.Add("filter_value", typeof(string));

                //Add the filter values to the above created table
                dtSearchedFilters.Rows.Add("txtCatalogueNo", txtCatalogueNo.Text);
                dtSearchedFilters.Rows.Add("txtTitle", txtTitle.Text);
                dtSearchedFilters.Rows.Add("txtISRC", txtISRC.Text);
                dtSearchedFilters.Rows.Add("txtArtist", txtArtist.Text);
                dtSearchedFilters.Rows.Add("ddlConfiguration", ddlConfiguration.SelectedValue);
                dtSearchedFilters.Rows.Add("ddlTeamResponsibility", ddlTeamResponsibility.SelectedValue);
                dtSearchedFilters.Rows.Add("ddlTeamResponsibilitybyTrackLevel", ddlTeamResponsibilitybyTrackLevel.SelectedValue);
                dtSearchedFilters.Rows.Add("ddlManagerResponsibility", ddlManagerResponsibility.SelectedValue);
                dtSearchedFilters.Rows.Add("ddlManagerResponsibilitybyTrackLevel", ddlManagerResponsibilitybyTrackLevel.SelectedValue);
                dtSearchedFilters.Rows.Add("ddlCatnoStatus", ddlCatnoStatus.SelectedValue);
                dtSearchedFilters.Rows.Add("ddlTrackStatus", ddlTrackStatus.SelectedValue);
                dtSearchedFilters.Rows.Add("ddlCompareStartDate", ddlCompareStartDate.SelectedValue);
                dtSearchedFilters.Rows.Add("txtStartDate", txtStartDate.Text);
                dtSearchedFilters.Rows.Add("ddlCompareEndDate", ddlCompareEndDate.SelectedValue);
                dtSearchedFilters.Rows.Add("txtEndDate", txtEndDate.Text);
                dtSearchedFilters.Rows.Add("txtValueThreshold", txtValueThreshold.Text);

                Session["MPSearchedFilters"] = dtSearchedFilters;

                LoadSearchedData();

                //string teamResponsibility = string.Empty;
                //string mngrResponsibility = string.Empty;
                //string statusCode = string.Empty;
                //string startDate = string.Empty;
                //string endDate = string.Empty;

                //if (ddlTeamResponsibility.SelectedIndex > 0)
                //{
                //    teamResponsibility = ddlTeamResponsibility.SelectedValue;
                //}

                //if (ddlManagerResponsibility.SelectedIndex > 0)
                //{
                //    mngrResponsibility = ddlManagerResponsibility.SelectedValue;
                //}

                //if (ddlStatus.SelectedIndex > 0)
                //{
                //    statusCode = ddlStatus.SelectedValue;
                //}

                //if (txtStartDate.Text != "__/____")
                //{
                //    startDate = txtStartDate.Text.Split('/')[1].ToString() + txtStartDate.Text.Split('/')[0].ToString();
                //}

                //if (txtEndDate.Text != "__/____")
                //{
                //    endDate = txtEndDate.Text.Split('/')[1].ToString() + txtEndDate.Text.Split('/')[0].ToString(); ;
                //}


                //missingParticipantsBL = new MissingParticipantsBL();
                //DataSet initialData = missingParticipantsBL.GetSearchedData(txtCatalogueNo.Text.Trim().Replace("'", "''"), txtTitle.Text.Trim().Replace("'", "''"), txtArtist.Text.Trim().Replace("'", "''"),
                //    txtConfiguration.Text.Trim().Replace("'", "''"), teamResponsibility, mngrResponsibility, startDate, endDate, statusCode, txtValueThreshold.Text.Trim(), txtISRC.Text.Trim(), out errorId);
                //missingParticipantsBL = null;

                //if (initialData.Tables.Count != 0 && errorId != 2)
                //{
                //    Session["MissingPartcipantsData"] = initialData.Tables[0];

                //    gvCatalogueDetails.PageIndex = 0;
                //    BindGrid(initialData.Tables[0]);
                //}
                //else
                //{
                //    ExceptionHandler("Error in fetching data", string.Empty);
                //}
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in search.", ex.Message);
            }
        }

        protected void valStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtStartDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtStartDate.Text, out temp))
                    {

                    }
                    else
                    {
                        args.IsValid = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }
        }

        protected void valEndDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtEndDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtEndDate.Text, out temp))
                    {
                        if (txtStartDate.Text.Trim() != "__/____")
                        {
                            if (DateTime.TryParse("01/" + txtStartDate.Text, out temp))
                            {
                                if (Convert.ToDateTime("01/" + txtEndDate.Text) < Convert.ToDateTime("01/" + txtStartDate.Text))
                                {
                                    valEndDate.ToolTip = "Start date should be earlier than End date";
                                    args.IsValid = false;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        args.IsValid = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {
            missingParticipantsBL = new MissingParticipantsBL();
            DataSet initialData = missingParticipantsBL.GetInitialData(out errorId);
            missingParticipantsBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                //Session["MissingPartcipantsData"] = initialData.Tables[0];

                //BindGrid(initialData.Tables[0]);

                ddlTeamResponsibility.DataSource = initialData.Tables[0];
                ddlTeamResponsibility.DataTextField = "responsibility_desc";
                ddlTeamResponsibility.DataValueField = "responsibility_code";
                ddlTeamResponsibility.DataBind();
                ddlTeamResponsibility.Items.Insert(0, new ListItem("-"));

                ddlTeamResponsibilitybyTrackLevel.DataSource = initialData.Tables[0];
                ddlTeamResponsibilitybyTrackLevel.DataTextField = "responsibility_desc";
                ddlTeamResponsibilitybyTrackLevel.DataValueField = "responsibility_code";
                ddlTeamResponsibilitybyTrackLevel.DataBind();
                ddlTeamResponsibilitybyTrackLevel.Items.Insert(0, new ListItem("-"));

                ddlManagerResponsibility.DataSource = initialData.Tables[0];
                ddlManagerResponsibility.DataTextField = "responsibility_desc";
                ddlManagerResponsibility.DataValueField = "responsibility_code";
                ddlManagerResponsibility.DataBind();
                ddlManagerResponsibility.Items.Insert(0, new ListItem("-"));

                ddlManagerResponsibilitybyTrackLevel.DataSource = initialData.Tables[0];
                ddlManagerResponsibilitybyTrackLevel.DataTextField = "responsibility_desc";
                ddlManagerResponsibilitybyTrackLevel.DataValueField = "responsibility_code";
                ddlManagerResponsibilitybyTrackLevel.DataBind();
                ddlManagerResponsibilitybyTrackLevel.Items.Insert(0, new ListItem("-"));

                ddlCatnoStatus.DataSource = initialData.Tables[1];
                ddlCatnoStatus.DataTextField = "status_desc";
                ddlCatnoStatus.DataValueField = "status_code";
                ddlCatnoStatus.DataBind();
                ddlCatnoStatus.Items.Insert(0, new ListItem("-"));

                ddlTrackStatus.DataSource = initialData.Tables[3];
                ddlTrackStatus.DataTextField = "status_desc";
                ddlTrackStatus.DataValueField = "status_code";
                ddlTrackStatus.DataBind();
                ddlTrackStatus.Items.Insert(0, new ListItem("-"));

                ddlConfiguration.DataTextField = "config_desc";
                ddlConfiguration.DataValueField = "config_group_code";
                ddlConfiguration.DataSource = initialData.Tables[2];
                ddlConfiguration.DataBind();
                ddlConfiguration.Items.Insert(0, new ListItem("-", null));
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                hdnIsNewRequest.Value = Request.QueryString[0];

                if (hdnIsNewRequest.Value == "N")
                {
                    if (Session["MPSearchedFilters"] != null)
                    {
                        DataTable dtSearchedFilters = Session["MPSearchedFilters"] as DataTable;

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
                            else if (dRow["filter_name"].ToString() == "txtISRC")
                            {
                                txtISRC.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtArtist")
                            {
                                txtArtist.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlConfiguration")
                            {
                                ddlConfiguration.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "ddlTeamResponsibility")
                            {
                                ddlTeamResponsibility.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "ddlTeamResponsibilitybyTrackLevel")
                            {
                                ddlTeamResponsibilitybyTrackLevel.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "ddlManagerResponsibility")
                            {
                                ddlManagerResponsibility.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "ddlManagerResponsibilitybyTrackLevel")
                            {
                                ddlManagerResponsibilitybyTrackLevel.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "ddlCatnoStatus")
                            {
                                ddlCatnoStatus.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "ddlTrackStatus")
                            {
                                ddlTrackStatus.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "ddlCompareStartDate")
                            {
                                ddlCompareStartDate.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "txtStartDate")
                            {
                                txtStartDate.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlCompareEndDate")
                            {
                                ddlCompareEndDate.Items.FindByValue(dRow["filter_value"].ToString()).Selected = true;
                            }
                            else if (dRow["filter_name"].ToString() == "txtEndDate")
                            {
                                txtEndDate.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "txtValueThreshold")
                            {
                                txtValueThreshold.Text = dRow["filter_value"].ToString();
                            }
                        }

                        LoadSearchedData();
                    }
                }
            }
        }

        private void LoadSearchedData()
        {
            //validate 
            //if (!Page.IsValid)
            //{
            //    msgView.SetMessage("Invalid search input!", MessageType.Warning, PositionType.Auto);
            //    return;
            //}

            string teamResponsibility = string.Empty;
            string teamResponsibilityTrack = string.Empty;
            string mngrResponsibility = string.Empty;
            string mngrResponsibilityTrack = string.Empty;
            string catnoStatusCode = string.Empty;
            string trackStatusCode = string.Empty;
            string startDate = string.Empty;
            string endDate = string.Empty;
            string configuration = string.Empty;

            if (ddlTeamResponsibility.SelectedIndex > 0)
            {
                teamResponsibility = ddlTeamResponsibility.SelectedValue;
            }

            if (ddlTeamResponsibilitybyTrackLevel.SelectedIndex > 0)
            {
                teamResponsibilityTrack = ddlTeamResponsibilitybyTrackLevel.SelectedValue;
            }

            if (ddlManagerResponsibility.SelectedIndex > 0)
            {
                mngrResponsibility = ddlManagerResponsibility.SelectedValue;
            }

            if (ddlManagerResponsibilitybyTrackLevel.SelectedIndex > 0)
            {
                mngrResponsibilityTrack = ddlManagerResponsibilitybyTrackLevel.SelectedValue;
            }
            if (ddlCatnoStatus.SelectedIndex > 0)
            {
                catnoStatusCode = ddlCatnoStatus.SelectedValue;
            }
            if (ddlTrackStatus.SelectedIndex > 0)
            {
                trackStatusCode = ddlTrackStatus.SelectedValue;
            }

            if (ddlConfiguration.SelectedIndex > 0)
            {
                configuration = ddlConfiguration.SelectedValue;
            }

            if (txtStartDate.Text != "__/____")
            {
                startDate = txtStartDate.Text.Split('/')[1].ToString() + txtStartDate.Text.Split('/')[0].ToString();
            }

            if (txtEndDate.Text != "__/____")
            {
                endDate = txtEndDate.Text.Split('/')[1].ToString() + txtEndDate.Text.Split('/')[0].ToString(); ;
            }

            missingParticipantsBL = new MissingParticipantsBL();
            //JIRA-1048 Changes to handle single quote while searching --Start
            DataSet initialData = missingParticipantsBL.GetSearchedData(txtCatalogueNo.Text.Replace("'", "").Trim().ToUpper(), txtTitle.Text.Replace("'", "").Trim(), txtArtist.Text.Replace("'", "").Trim(),
                configuration, teamResponsibility, teamResponsibilityTrack, mngrResponsibility, mngrResponsibilityTrack, startDate, ddlCompareStartDate.SelectedValue, endDate, ddlCompareEndDate.SelectedValue, catnoStatusCode, trackStatusCode, txtValueThreshold.Text.Trim(), txtISRC.Text.Replace("'", "").Trim(), out errorId);
            //JIRA-1048 Changes to handle single quote while searching --End
            missingParticipantsBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["MissingPartcipantsData"] = initialData.Tables[0];

                gvCatalogueDetails.PageIndex = 0;
                BindGrid(initialData.Tables[0]);
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            tdData.Style.Remove("display");

            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            //WUIN-215 - changes
            //If search returns only one result - display Track Listing or CATNO depending on data (don't display grid for only one product)
            if (gridData.Rows.Count == 1)
            {
                if (hdnIsNewRequest.Value != "N") //If redirected from other screen (hdnIsNewRequest.Value = "N") then just bind the data
                {
                    string catNo = Convert.ToString(gridData.Rows[0]["catno"]);

                    //string trackListingId = Convert.ToString(gridData.Rows[0]["track_listing_id"]);
                    string catnoLegacy = string.Empty;
                    missingParticipantsBL = new MissingParticipantsBL();
                    string isTrackListingExist = missingParticipantsBL.IsTrackListingExist(catNo, out catnoLegacy, out errorId);
                    missingParticipantsBL = null;

                    //WUIN-502 - change
                    //display Track Listing screen if CATNO.LEGACY = 'N' and Track Listing exists for CATNO
                    //Else display Catalogue Maintenance	
                    if (isTrackListingExist == "Y" && catnoLegacy == "N")
                    {
                        Response.Redirect("../Participants/TrackListing.aspx?CatNo=" + catNo + "", false);
                    }
                    else
                    {
                        Response.Redirect("../Participants/CatalogueMaintenance.aspx?CatNo=" + catNo + "", false);
                    }
                }
                else
                {
                    gvCatalogueDetails.DataSource = gridData;
                    gvCatalogueDetails.DataBind();
                }
            }
            else
            {
                PopulateGridPage(1, gridData);
                //PopulateGridPage(hdnPageIndex.Value == "" ? 1 : Convert.ToInt32(hdnPageIndex.Value));
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        #endregion Methods


        #region GRID PAGING

        private void PopulateGridPage(int pageIndex, DataTable dtGridData)
        {
            if (dtGridData.Rows.Count > 0)
            {
                DataTable dtFiltered = dtGridData.Rows.Cast<System.Data.DataRow>().Skip((pageIndex - 1) * gridDefaultPageSize).Take(gridDefaultPageSize).CopyToDataTable();

                if (dtFiltered.Rows.Count != 0)
                {
                    gvCatalogueDetails.DataSource = dtFiltered;
                }
                else
                {
                    gvCatalogueDetails.EmptyDataText = "No data found.";
                }
                gvCatalogueDetails.DataBind();
            }
            else
            {
                dtEmpty = new DataTable();
                gvCatalogueDetails.DataSource = dtEmpty;
                gvCatalogueDetails.DataBind();
            }
            PopulatePager(dtGridData.Rows.Count, pageIndex, gridDefaultPageSize);
        }

        private void PopulatePager(int recordCount, int currentPage, int pageSize)
        {
            rptPager.Visible = true;
            double dblPageCount = (double)((decimal)recordCount / decimal.Parse(pageSize.ToString()));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<ListItem> pages = new List<ListItem>();
            if (pageCount > 0)
            {
                //To display only 10 pages on screen at a time
                //Divide total pages into different groups with 10 pages on each group
                //for example 
                //page group 0 ->page no 1 to 10 
                //page group 1 ->page no 11 to 20 ... 
                //calculate page group of currentpage
                int pageGroup = (int)Math.Floor((decimal)(currentPage - 1) / 10);

                //Add First page 
                pages.Add(new ListItem("First", "1", currentPage > 1));

                //if page group > 0 means current page is >10. Add ... to screen 
                //And its page index is last page of previous page group
                if (pageGroup > 0)
                {
                    pages.Add(new ListItem("...", (pageGroup * 10).ToString(), true));
                }

                //Add pages based on page group
                //If selected page is 5, its pagegroup is 0 so add 1 to 10 page no
                //If selected page is 18, its pagegroup is 1 so add 11 to 20 page no
                for (int i = 1; i <= 10; i++)
                {
                    int pageIndex = (pageGroup * 10) + i;
                    if (pageIndex <= pageCount)
                    {
                        pages.Add(new ListItem(pageIndex.ToString(), pageIndex.ToString(), pageIndex != currentPage));
                    }
                }

                //If total page count is more than 10 and we are not on last page group then add ...
                //And its index is first page of next page group
                if (pageCount > 10 && ((pageCount - (pageGroup * 10)) > 10))
                {
                    pages.Add(new ListItem("...", ((pageGroup * 10) + 11).ToString(), currentPage < pageCount));
                }

                //Finally ad Last page
                pages.Add(new ListItem("Last", pageCount.ToString(), currentPage < pageCount));
            }

            rptPager.DataSource = pages;
            rptPager.DataBind();

            if (pageCount == 1)
            {
                rptPager.Visible = false;
            }
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["MissingPartcipantsData"] == null)
                    return;

                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                //hdnPageIndex.Value = pageIndex.ToString();
                DataTable dtGridData = (DataTable)Session["MissingPartcipantsData"];
                PopulateGridPage(pageIndex, dtGridData);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        #endregion GRID PAGING
    }
}