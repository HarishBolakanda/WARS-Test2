/*
File Name   :   ArtistResponsibilityMaintenance.cs
Purpose     :   To maintain Artist responsibility data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     13-Oct-2016     Pratik(Infosys Limited)   Initial Creation (WUIN-303)
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

namespace WARS.DataMaintenance
{
    public partial class ArtistResponsibilityMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        ArtistResponsibilityMaintenanceBL artistResponsibilityMaintenanceBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Artist Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Artist Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlArtistDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtArtist.Focus();

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
                        UserAuthorization();
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
        protected void ddlDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                btnHdnArtistSearch_Click(sender, e);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting responsibility dropdown.", ex.Message);
            }
        }
        protected void btnHdnArtistSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadArtistData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in artist search.", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                artistResponsibilityMaintenanceBL = new ArtistResponsibilityMaintenanceBL();
                DataSet initialData = artistResponsibilityMaintenanceBL.GetInitialData(out errorId);
                artistResponsibilityMaintenanceBL = null;

                if (initialData.Tables.Count != 0 && errorId != 2)
                {
                    Session["ArtistResData"] = initialData.Tables[0];
                    Session["ArtistResList"] = initialData.Tables[1];

                    gvArtistDetails.PageIndex = 0;
                    hdnPageNumber.Value = "1";
                    BindGrid(initialData.Tables[0]);
                }
                else
                {
                    ExceptionHandler("Error in fetching data", string.Empty);
                }

                txtArtist.Text = string.Empty;
                ddlDealType.SelectedIndex = 0;
                ddlResponsibility.SelectedIndex = 0;
                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;
                hdnSearchText.Value = null;

                txtArtistNameInsert.Text = "";
                ddlDealTypeInsert.SelectedIndex = -1;
                ddlTeamRespInsert.SelectedIndex = -1;
                ddlManagerRespInsert.SelectedIndex = -1;
                hdnInsertDataNotSaved.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reseting page..", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnChangeNotSaved.Value == "Y")
                {
                    int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                    //Calculate the rowindex for validation 
                    int rowIndexValidation = (gvArtistDetails.PageIndex * gvArtistDetails.PageSize) + rowIndex;

                    //Validate
                    Page.Validate("GroupUpdate_" + rowIndexValidation + "");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    string artistId = ((HiddenField)gvArtistDetails.Rows[rowIndex].FindControl("hdnArtistId")).Value;
                    string teamResponsibility = ((DropDownList)gvArtistDetails.Rows[rowIndex].FindControl("ddlTeamResponsibility")).SelectedValue;
                    string managerResponsibility = ((DropDownList)gvArtistDetails.Rows[rowIndex].FindControl("ddlManagerResponsibility")).SelectedValue;

                    if (teamResponsibility == "-")
                    {
                        teamResponsibility = string.Empty;
                    }

                    if (managerResponsibility == "-")
                    {
                        managerResponsibility = string.Empty;
                    }

                    artistResponsibilityMaintenanceBL = new ArtistResponsibilityMaintenanceBL();
                    DataSet updatedData = artistResponsibilityMaintenanceBL.UpdateArtistData(artistId, teamResponsibility, managerResponsibility, Convert.ToString(Session["UserCode"]), hdnSearchText.Value, out errorId);
                    artistResponsibilityMaintenanceBL = null;

                    if (updatedData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["ArtistResData"] = updatedData.Tables[0];

                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Artist details saved successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to save artist details.", MessageType.Warning, PositionType.Auto);
                    }
                }
                else if (hdnInsertDataNotSaved.Value == "Y")
                {
                    Page.Validate("valInsertArtist");
                    if (!Page.IsValid)
                    {
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    artistResponsibilityMaintenanceBL = new ArtistResponsibilityMaintenanceBL();
                    DataSet updatedData = artistResponsibilityMaintenanceBL.InsertArtistData(txtArtistNameInsert.Text.Trim().ToUpper(), ddlDealTypeInsert.SelectedValue, ddlTeamRespInsert.SelectedValue, ddlManagerRespInsert.SelectedValue, Convert.ToString(Session["UserCode"]), hdnSearchText.Value, ddlDealType.SelectedValue, ddlResponsibility.SelectedValue, out errorId);
                    artistResponsibilityMaintenanceBL = null;

                    if (updatedData.Tables.Count != 0 && errorId == 0)
                    {
                        Session["ArtistResData"] = updatedData.Tables[0];
                        txtArtistNameInsert.Text = string.Empty;
                        ddlDealTypeInsert.SelectedIndex = -1;
                        ddlTeamRespInsert.SelectedIndex = -1;
                        ddlManagerRespInsert.SelectedIndex = -1;

                        gvArtistDetails.PageIndex = 0;
                        BindGrid(updatedData.Tables[0]);
                        hdnInsertDataNotSaved.Value = "N";
                        msgView.SetMessage("Artist details saved successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else if (errorId == 1)
                    {
                        msgView.SetMessage("Artist with same name and deal type already exist.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to save artist details.", MessageType.Warning, PositionType.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving artist data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnChangeNotSaved.Value == "Y")
                {
                    if (Session["ArtistResData"] == null)
                        return;

                    DataTable dtArtistResData = Session["ArtistResData"] as DataTable;
                    BindGrid(dtArtistResData);

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }
                else if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtArtistNameInsert.Text = "";
                    ddlDealTypeInsert.SelectedIndex = -1;
                    ddlTeamRespInsert.SelectedIndex = -1;
                    ddlManagerRespInsert.SelectedIndex = -1;
                    hdnInsertDataNotSaved.Value = "N";

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading data grid.", ex.Message);
            }
        }
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["ArtistResData"] == null)
                    return;

                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);
                //hdnPageIndex.Value = pageIndex.ToString();
                DataTable dtGridData = (DataTable)Session["ArtistResData"];
                Utilities.PopulateGridPage(pageIndex, dtGridData, gridDefaultPageSize, gvArtistDetails, dtEmpty, rptPager);
                hdnChangeNotSaved.Value = "N";
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }
        protected void gvArtistDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Session["ArtistResList"] == null)
                {
                    return;
                }

                DataTable dtResponsibilityList = Session["ArtistResList"] as DataTable;
                DropDownList ddlTeamResponsibility;
                DropDownList ddlManagerResponsibility;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlTeamResponsibility = (e.Row.FindControl("ddlTeamResponsibility") as DropDownList);
                    string teamResponsibility = (e.Row.FindControl("hdnTeamResponsibility") as HiddenField).Value;

                    ddlTeamResponsibility.DataSource = dtResponsibilityList;
                    ddlTeamResponsibility.DataTextField = "responsibility_desc";
                    ddlTeamResponsibility.DataValueField = "responsibility_code";
                    ddlTeamResponsibility.DataBind();
                    ddlTeamResponsibility.Items.Insert(0, new ListItem("-"));

                    if (dtResponsibilityList.Select("responsibility_code = '" + teamResponsibility + "'").Length != 0)
                    {
                        ddlTeamResponsibility.Items.FindByValue(teamResponsibility).Selected = true;
                    }
                    else
                    {
                        ddlTeamResponsibility.SelectedIndex = 0;
                    }

                    ddlManagerResponsibility = (e.Row.FindControl("ddlManagerResponsibility") as DropDownList);
                    string managerResponsibility = (e.Row.FindControl("hdnManagerResponsibility") as HiddenField).Value;

                    ddlManagerResponsibility.DataSource = dtResponsibilityList;
                    ddlManagerResponsibility.DataTextField = "responsibility_desc";
                    ddlManagerResponsibility.DataValueField = "responsibility_code";
                    ddlManagerResponsibility.DataBind();
                    ddlManagerResponsibility.Items.Insert(0, new ListItem("-"));

                    if (dtResponsibilityList.Select("responsibility_code = '" + managerResponsibility + "'").Length != 0)
                    {
                        ddlManagerResponsibility.Items.FindByValue(managerResponsibility).Selected = true;
                    }
                    else
                    {
                        ddlManagerResponsibility.SelectedIndex = 0;
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
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvArtistDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["ArtistResData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvArtistDetails, dtEmpty, rptPager);
                Session["ArtistResData"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvArtistDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "saverow")
                {
                    if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
                    {
                        int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                        Page.Validate("GroupUpdate_" + rowIndex + "");
                        if (!Page.IsValid)
                        {
                            msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                            return;
                        }

                        string artistId = ((HiddenField)gvArtistDetails.Rows[rowIndex].FindControl("hdnArtistId")).Value;
                        string teamResponsibility = ((DropDownList)gvArtistDetails.Rows[rowIndex].FindControl("ddlTeamResponsibility")).SelectedValue;
                        string managerResponsibility = ((DropDownList)gvArtistDetails.Rows[rowIndex].FindControl("ddlManagerResponsibility")).SelectedValue;

                        if (teamResponsibility == "-")
                        {
                            teamResponsibility = string.Empty;
                        }

                        if (managerResponsibility == "-")
                        {
                            managerResponsibility = string.Empty;
                        }

                        artistResponsibilityMaintenanceBL = new ArtistResponsibilityMaintenanceBL();
                        DataSet updatedData = artistResponsibilityMaintenanceBL.UpdateArtistData(artistId, teamResponsibility, managerResponsibility, Convert.ToString(Session["UserCode"]), hdnSearchText.Value, out errorId);
                        artistResponsibilityMaintenanceBL = null;

                        if (updatedData.Tables.Count != 0 && errorId != 2)
                        {
                            Session["ArtistResData"] = updatedData.Tables[0];

                            hdnChangeNotSaved.Value = "N";
                            hdnGridRowSelectedPrvious.Value = null;
                            msgView.SetMessage("Artist details saved successfully.", MessageType.Success, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("Failed to save artist details.", MessageType.Warning, PositionType.Auto);
                        }
                    }
                }
                else if (e.CommandName == "cancelrow")
                {
                    if (Session["ArtistResData"] == null)
                        return;

                    DataTable dtArtistResData = Session["ArtistResData"] as DataTable;
                    BindGrid(dtArtistResData);

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving artist data.", ex.Message);
            }
        }

        protected void imgBtnArtistInsert_Click(object sender, EventArgs e)
        {
            Page.Validate("valInsertArtist");
            if (!Page.IsValid)
            {
                msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                return;
            }

            artistResponsibilityMaintenanceBL = new ArtistResponsibilityMaintenanceBL();
            DataSet updatedData = artistResponsibilityMaintenanceBL.InsertArtistData(txtArtistNameInsert.Text.Trim().ToUpper(), ddlDealTypeInsert.SelectedValue, ddlTeamRespInsert.SelectedValue, ddlManagerRespInsert.SelectedValue, Convert.ToString(Session["UserCode"]), hdnSearchText.Value, ddlDealType.SelectedValue, ddlResponsibility.SelectedValue, out errorId);
            artistResponsibilityMaintenanceBL = null;

            if (updatedData.Tables.Count != 0 && errorId == 0)
            {
                Session["ArtistResData"] = updatedData.Tables[0];
                txtArtistNameInsert.Text = string.Empty;
                ddlDealTypeInsert.SelectedIndex = -1;
                ddlTeamRespInsert.SelectedIndex = -1;
                ddlManagerRespInsert.SelectedIndex = -1;

                gvArtistDetails.PageIndex = 0;
                BindGrid(updatedData.Tables[0]);

                hdnInsertDataNotSaved.Value = "N";
                msgView.SetMessage("Artist details saved successfully.", MessageType.Success, PositionType.Auto);
            }
            else if (errorId == 1)
            {
                msgView.SetMessage("Artist with same name and deal type already exist.", MessageType.Success, PositionType.Auto);
            }
            else
            {
                msgView.SetMessage("Failed to save artist details.", MessageType.Warning, PositionType.Auto);
            }

        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                ArtistAuditSession();

                if (gvArtistDetails.Rows.Count == 1)
                {
                    string artistId = (gvArtistDetails.Rows.Count > 1) ? string.Empty : ((HiddenField)gvArtistDetails.Rows[0].FindControl("hdnArtistId")).Value;
                    Response.Redirect("../Audit/ArtistAudit.aspx?artistData=" + artistId + "", false);
                }
                else
                {
                    Response.Redirect("../Audit/ArtistAudit.aspx", false);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting.", ex.Message);
            }
        }

        private void ArtistAuditSession()
        {
            //Create a table to hold the filter values
            DataTable dtSearchedFilters = new DataTable();
            dtSearchedFilters.Columns.Add("filter_name", typeof(string));
            dtSearchedFilters.Columns.Add("filter_value", typeof(string));

            //Add the filter values to the above created table
            dtSearchedFilters.Rows.Add("txtArtist", txtArtist.Text.Trim());
            dtSearchedFilters.Rows.Add("ddlDealType", ddlDealType.SelectedValue);
            dtSearchedFilters.Rows.Add("ddlResponsibility", ddlResponsibility.SelectedValue);

            Session["ArtistMaintFilters"] = dtSearchedFilters;
        }
        #endregion Events

        #region Methods

        private void LoadData()
        {
            artistResponsibilityMaintenanceBL = new ArtistResponsibilityMaintenanceBL();
            DataSet initialData = artistResponsibilityMaintenanceBL.GetInitialData(out errorId);
            artistResponsibilityMaintenanceBL = null;

            ddlDealType.DataTextField = "deal_type_desc";
            ddlDealType.DataValueField = "deal_type_id";
            ddlDealType.DataSource = initialData.Tables[2];
            ddlDealType.DataBind();
            ddlDealType.Items.Insert(0, new ListItem("-", null));

            ddlResponsibility.DataTextField = "responsibility_desc";
            ddlResponsibility.DataValueField = "responsibility_code";
            ddlResponsibility.DataSource = initialData.Tables[1];
            ddlResponsibility.DataBind();
            ddlResponsibility.Items.Insert(0, new ListItem("-", null));

            ddlDealTypeInsert.DataTextField = "deal_type_desc";
            ddlDealTypeInsert.DataValueField = "deal_type_id";
            ddlDealTypeInsert.DataSource = initialData.Tables[2];
            ddlDealTypeInsert.DataBind();
            ddlDealTypeInsert.Items.Insert(0, new ListItem("-", null));

            ddlTeamRespInsert.DataTextField = "responsibility_desc";
            ddlTeamRespInsert.DataValueField = "responsibility_code";
            ddlTeamRespInsert.DataSource = initialData.Tables[1];
            ddlTeamRespInsert.DataBind();
            ddlTeamRespInsert.Items.Insert(0, new ListItem("-", null));

            ddlManagerRespInsert.DataTextField = "responsibility_desc";
            ddlManagerRespInsert.DataValueField = "responsibility_code";
            ddlManagerRespInsert.DataSource = initialData.Tables[1];
            ddlManagerRespInsert.DataBind();
            ddlManagerRespInsert.Items.Insert(0, new ListItem("-", null));

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["ArtistResData"] = initialData.Tables[0];
                Session["ArtistResList"] = initialData.Tables[1];
                hdnPageNumber.Value = "1";

                //load grid as per the search criteria held. This to be done only when navigating from Audit screen
                if (Request.QueryString["fromAudit"] == "Y")
                {
                    if (Session["ArtistMaintFilters"] != null)
                    {
                        DataTable dtSearchedFilters = Session["ArtistMaintFilters"] as DataTable;
                        foreach (DataRow dRow in dtSearchedFilters.Rows)
                        {
                            if (dRow["filter_name"].ToString() == "txtArtist")
                            {
                                txtArtist.Text = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlDealType")
                            {
                                ddlDealType.SelectedValue = dRow["filter_value"].ToString();
                            }
                            else if (dRow["filter_name"].ToString() == "ddlResponsibility")
                            {
                                ddlResponsibility.SelectedValue = dRow["filter_value"].ToString();
                            }
                        }

                        LoadArtistData();
                    }

                }
                else
                {
                    BindGrid(initialData.Tables[0]);
                }
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }


        }

        private void LoadArtistData()
        {
            hdnSearchText.Value = txtArtist.Text.Replace("'", "").Trim();//JIRA-1048 --Changes to handle single quote

            artistResponsibilityMaintenanceBL = new ArtistResponsibilityMaintenanceBL();
            DataSet searchedData = artistResponsibilityMaintenanceBL.GetSearchedArtistData(txtArtist.Text.Replace("'", "").Trim(), ddlDealType.SelectedValue, ddlResponsibility.SelectedValue, out errorId);//JIRA-1048 --Changes to handle single quote
            artistResponsibilityMaintenanceBL = null;
            hdnPageNumber.Value = "1";

            if (searchedData.Tables.Count != 0 && errorId != 2)
            {
                Session["ArtistResData"] = searchedData.Tables[0];

                gvArtistDetails.PageIndex = 0;
                BindGrid(searchedData.Tables[0]);
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
            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), gridData, gridDefaultPageSize, gvArtistDetails, dtEmpty, rptPager);
            UserAuthorization();
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnSaveChanges.Enabled = false;

                foreach (GridViewRow rows in gvArtistDetails.Rows)
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


    }
}