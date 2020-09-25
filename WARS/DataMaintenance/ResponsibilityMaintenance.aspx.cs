/*
File Name   :   ResponsibilityMaintenance.cs
Purpose     :   Used for maintaining Responsibility data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     11-Apr-2017     Pratik(Infosys Limited)   Initial Creation
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
    public partial class ResponsibilityMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        ResponsibilityMaintenanceBL responsibilityMaintenanceBL;
        string loggedUserID;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Responsibility Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Responsibility Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlResponsibilityDetails.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    txtResponsibilitySearch.Focus();

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

        protected void fuzzySearchResponsibility_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchResponsibility();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in responsibility search.", ex.Message);
            }
        }

        protected void gvResponsibilityDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                Label lblResponsibilityType;
                DropDownList ddlGridManagerResp;
                string hdnManagerResp;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblResponsibilityType = (e.Row.FindControl("lblResponsibilityType") as Label);
                    ddlGridManagerResp = (e.Row.FindControl("ddlGridManagerResp") as DropDownList);
                    hdnManagerResp = (e.Row.FindControl("hdnManagerResp") as HiddenField).Value;

                    DataTable dtManagerResp = (DataTable)Session["RespMaintMngrResp"];
                    ddlGridManagerResp.DataSource = dtManagerResp;
                    ddlGridManagerResp.DataTextField = "responsibility";
                    ddlGridManagerResp.DataValueField = "responsibility_code";
                    ddlGridManagerResp.DataBind();
                    ddlGridManagerResp.Items.Insert(0, new ListItem("-"));
                    ddlGridManagerResp.SelectedValue = hdnManagerResp;

                    //WUIN-719-Only super user can edit/delete changes if type is not null
                    if (!string.IsNullOrEmpty(lblResponsibilityType.Text))
                    {
                        (e.Row.FindControl("imgBtnDelete") as ImageButton).Visible = false;
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


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvResponsibilityDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RespMaintData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvResponsibilityDetails.DataSource = dataView;
                gvResponsibilityDetails.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvResponsibilityDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                if (e.CommandName == "saverow")
                {
                    //Validate
                    Page.Validate("valUpdateResponsibility");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
                    {
                        int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                        //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                        string responsibilityCode = ((Label)gvResponsibilityDetails.Rows[rowIndex].FindControl("lblResponsibilityCode")).Text;
                        string responsibilityDesc = ((TextBox)gvResponsibilityDetails.Rows[rowIndex].FindControl("txtResponsibilityName")).Text;
                        string managerResponsibility = ((DropDownList)gvResponsibilityDetails.Rows[rowIndex].FindControl("ddlGridManagerResp")).SelectedValue;

                        responsibilityMaintenanceBL = new ResponsibilityMaintenanceBL();
                        DataSet responsibilityData = responsibilityMaintenanceBL.UpdateResponsibilityData(responsibilityCode, responsibilityDesc, managerResponsibility, userCode, out errorId);
                        responsibilityMaintenanceBL = null;

                        if (responsibilityData.Tables.Count != 0 && errorId != 2)
                        {
                            Session["RespMaintData"] = responsibilityData.Tables[0];
                            Session["RespMaintMngrResp"] = responsibilityData.Tables[1];
                            Session["FuzzySearchAllResponsibilityList"] = responsibilityData.Tables[2];

                            //check if there is only one row in the grid before binding updated data.
                            //if count is 1 then only display that row
                            if (gvResponsibilityDetails.Rows.Count == 1)
                            {
                                DataTable dtSearched = responsibilityData.Tables[0].Clone();
                                DataRow[] foundRows = responsibilityData.Tables[0].Select("responsibility_code = '" + responsibilityCode + "'");
                                if (foundRows.Length != 0)
                                {
                                    dtSearched = foundRows.CopyToDataTable();
                                    BindGrid(dtSearched);
                                }
                            }
                            else
                            {
                                BindGrid(responsibilityData.Tables[0]);
                            }
                            PopulateDropdowns(responsibilityData.Tables[1]);
                            hdnChangeNotSaved.Value = "N";
                            hdnGridRowSelectedPrvious.Value = null;
                            msgView.SetMessage("Responsibility details updated successfully.", MessageType.Success, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("Failed to updated responsibility details.", MessageType.Warning, PositionType.Auto);
                        }
                    }
                }
                else if (e.CommandName == "cancelrow")
                {
                    if (Session["RespMaintData"] != null)
                    {
                        DataTable dtResponsibilityData = Session["RespMaintData"] as DataTable;
                        if (gvResponsibilityDetails.Rows.Count == 1)
                        {
                            string responsibilityDesc = ((Label)gvResponsibilityDetails.Rows[0].FindControl("lblResponsibilityName")).Text;

                            ((TextBox)gvResponsibilityDetails.Rows[0].FindControl("txtResponsibilityName")).Text = responsibilityDesc;
                        }
                        else
                        {
                            BindGrid(dtResponsibilityData);
                            txtResponsibilitySearch.Text = string.Empty;
                        }
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving/deleting responsibility data.", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                if (Session["RespMaintData"] != null)
                {
                    DataTable dtResponsibilityData = Session["RespMaintData"] as DataTable;
                    BindGrid(dtResponsibilityData);
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }

                txtResponsibilitySearch.Text = string.Empty;
                ddlManagerResponsibility.SelectedIndex = 0;
                txtResponsibilityDesc.Text = string.Empty;
                txtResponsibilityCode.Text = string.Empty;
                ddlNewManagerResp.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reset.", ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ResponsibilitySearch();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting responsibility from the filters.", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["RespMaintData"] != null)
                {
                    DataTable dtResponsibilityData = Session["RespMaintData"] as DataTable;
                    if (dtResponsibilityData.Select("responsibility_code = '" + txtResponsibilityCode.Text + "'").Length != 0)
                    {
                        msgView.SetMessage("Cannot save - Responsibility already exists with this Code.", MessageType.Success, PositionType.Auto);
                        return;
                    }
                    else if (dtResponsibilityData.Select("responsibility_desc = '" + txtResponsibilityDesc.Text + "'").Length != 0)
                    {
                        msgView.SetMessage("Cannot save - Responsibility already exists with this Name.", MessageType.Success, PositionType.Auto);
                        return;
                    }
                }

                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                responsibilityMaintenanceBL = new ResponsibilityMaintenanceBL();
                DataSet responsibilityData = responsibilityMaintenanceBL.InsertResponsibilityData(txtResponsibilityCode.Text.Trim(), txtResponsibilityDesc.Text.Trim(), ddlNewManagerResp.SelectedValue, userCode, out errorId);
                responsibilityMaintenanceBL = null;

                if (responsibilityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RespMaintData"] = responsibilityData.Tables[0];
                    Session["RespMaintMngrResp"] = responsibilityData.Tables[1];
                    Session["FuzzySearchAllResponsibilityList"] = responsibilityData.Tables[2];

                    BindGrid(responsibilityData.Tables[0]);
                    PopulateDropdowns(responsibilityData.Tables[1]);
                    txtResponsibilityDesc.Text = string.Empty;
                    txtResponsibilityCode.Text = string.Empty;
                    txtResponsibilitySearch.Text = string.Empty;
                    ddlNewManagerResp.SelectedIndex = 0;
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Responsibility created successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to create responsibility.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in creating responsibility.", ex.Message);
            }
        }

        protected void imgBtnCancel_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtResponsibilityDesc.Text = string.Empty;
                txtResponsibilityCode.Text = string.Empty;
                ddlNewManagerResp.SelectedIndex = 0;
                hdnInsertDataNotSaved.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing responsibility data.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtResponsibilitySearch.Text = string.Empty;
                    return;
                }

                txtResponsibilitySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                ResponsibilitySearch();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting responsibility.", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtResponsibilitySearch.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing search list.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    //Validate
                    Page.Validate("valInsertResponsibility");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    if (Session["RespMaintData"] != null)
                    {
                        DataTable dtResponsibilityData = Session["RespMaintData"] as DataTable;
                        if (dtResponsibilityData.Select("responsibility_code = '" + txtResponsibilityCode.Text + "'").Length != 0)
                        {
                            msgView.SetMessage("Responsibility already exists with this responsibility code.", MessageType.Success, PositionType.Auto);
                            return;
                        }
                        else if (dtResponsibilityData.Select("responsibility_desc = '" + txtResponsibilityDesc.Text + "'").Length != 0)
                        {
                            msgView.SetMessage("Responsibility already exists with this responsibility desc.", MessageType.Success, PositionType.Auto);
                            return;
                        }
                    }


                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                    responsibilityMaintenanceBL = new ResponsibilityMaintenanceBL();
                    DataSet responsibilityData = responsibilityMaintenanceBL.InsertResponsibilityData(txtResponsibilityCode.Text.Trim(), txtResponsibilityDesc.Text.Trim(), ddlNewManagerResp.SelectedValue, userCode, out errorId);
                    responsibilityMaintenanceBL = null;

                    if (responsibilityData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["RespMaintData"] = responsibilityData.Tables[0];
                        Session["RespMaintMngrResp"] = responsibilityData.Tables[1];
                        Session["FuzzySearchAllResponsibilityList"] = responsibilityData.Tables[2];

                        BindGrid(responsibilityData.Tables[0]);
                        PopulateDropdowns(responsibilityData.Tables[1]);
                        txtResponsibilityDesc.Text = string.Empty;
                        txtResponsibilityCode.Text = string.Empty;
                        txtResponsibilitySearch.Text = string.Empty;
                        ddlNewManagerResp.SelectedIndex = 0;
                        hdnChangeNotSaved.Value = "N";
                        hdnInsertDataNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Responsibility created successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed create responsibility.", MessageType.Warning, PositionType.Auto);
                    }
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    //Validate
                    Page.Validate("valUpdateResponsibility");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                    string responsibilityCode = ((Label)gvResponsibilityDetails.Rows[rowIndex].FindControl("lblResponsibilityCode")).Text;
                    string responsibilityDesc = ((TextBox)gvResponsibilityDetails.Rows[rowIndex].FindControl("txtResponsibilityName")).Text;
                    string managerResponsibility = ((DropDownList)gvResponsibilityDetails.Rows[rowIndex].FindControl("ddlGridManagerResp")).SelectedValue;

                    responsibilityMaintenanceBL = new ResponsibilityMaintenanceBL();
                    DataSet responsibilityData = responsibilityMaintenanceBL.UpdateResponsibilityData(responsibilityCode, responsibilityDesc, managerResponsibility, userCode, out errorId);
                    responsibilityMaintenanceBL = null;

                    if (responsibilityData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["RespMaintData"] = responsibilityData.Tables[0];
                        Session["RespMaintMngrResp"] = responsibilityData.Tables[1];
                        Session["FuzzySearchAllResponsibilityList"] = responsibilityData.Tables[2];

                        //check if there is only one row in the grid before binding updated data.
                        //if count is 1 then only display that row
                        if (gvResponsibilityDetails.Rows.Count == 1)
                        {
                            DataTable dtSearched = responsibilityData.Tables[0].Clone();
                            DataRow[] foundRows = responsibilityData.Tables[0].Select("responsibility_code = '" + responsibilityCode + "'");
                            if (foundRows.Length != 0)
                            {
                                dtSearched = foundRows.CopyToDataTable();
                                BindGrid(dtSearched);
                            }
                        }
                        else
                        {
                            BindGrid(responsibilityData.Tables[0]);
                        }
                        PopulateDropdowns(responsibilityData.Tables[1]);
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Responsibility details updated successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to updated responsibility details.", MessageType.Warning, PositionType.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving grid data", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtResponsibilityDesc.Text = string.Empty;
                    txtResponsibilityCode.Text = string.Empty;
                    ddlNewManagerResp.SelectedIndex = 0;
                    hdnInsertDataNotSaved.Value = "N";
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    if (Session["RespMaintData"] != null)
                    {
                        DataTable dtResponsibilityData = (Session["RespMaintData"] as DataTable).Copy();
                        BindGrid(dtResponsibilityData);
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void btnHdnResponsibilitySearch_Click(object sender, EventArgs e)
        {
            try
            {
                ResponsibilitySearch();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting responsibility from list.", ex.Message);
            }
        }

        //JIRA-908 CHanges by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string responsibilityCode = hdnRespCode.Value;
                responsibilityMaintenanceBL = new ResponsibilityMaintenanceBL();
                DataSet responsibilityData = responsibilityMaintenanceBL.DeleteResponsibilityData(responsibilityCode, Convert.ToString(Session["UserCode"]), out errorId);
                responsibilityMaintenanceBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("Cannot delete this Responsibility as it is linked to Artists/Contracts.", MessageType.Success, PositionType.Auto);
                }
                else if (responsibilityData.Tables.Count != 0 && errorId == 0)
                {
                    Session["RespMaintData"] = responsibilityData.Tables[0];
                    Session["RespMaintMngrResp"] = responsibilityData.Tables[1];
                    Session["FuzzySearchAllResponsibilityList"] = responsibilityData.Tables[2];

                    BindGrid(responsibilityData.Tables[0]);
                    PopulateDropdowns(responsibilityData.Tables[1]);
                    txtResponsibilitySearch.Text = string.Empty;
                    ddlManagerResponsibility.SelectedIndex = 0;
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Responsibility deleted successfully.", MessageType.Success, PositionType.Auto);
                    hdnRespCode.Value = "";
                }
                else
                {
                    msgView.SetMessage("Failed to delete responsibility.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Failed to delete responsibility.", ex.Message);
            }
        }
        //JIRA-908 CHanges by Ravi on 13/02/2019 -- End
        #endregion Events

        #region Methods

        private void LoadData()
        {
            responsibilityMaintenanceBL = new ResponsibilityMaintenanceBL();
            DataSet initialData = responsibilityMaintenanceBL.GetInitialData(out errorId);
            responsibilityMaintenanceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["RespMaintData"] = initialData.Tables[0];
                Session["RespMaintMngrResp"] = initialData.Tables[1];

                BindGrid(initialData.Tables[0]);
                PopulateDropdowns(initialData.Tables[1]);
            }
            else
            {
                ExceptionHandler("Error in fetching filter list data", string.Empty);
            }
        }

        private void ResponsibilitySearch()
        {
            if (Session["RespMaintData"] != null)
            {
                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;

                DataTable dtResponsibilityData = (Session["RespMaintData"] as DataTable).Copy();
                DataTable dtSearched = (Session["RespMaintData"] as DataTable).Clone();
                string searchquery = "1=1";
                if (txtResponsibilitySearch.Text != "" && ddlManagerResponsibility.SelectedIndex != 0)
                {
                    searchquery = searchquery + " AND (responsibility_code = '" + txtResponsibilitySearch.Text.Split('-')[0].ToString().Trim() + "' and manager_responsibility = '" + ddlManagerResponsibility.SelectedValue + "')";

                }
                else if (txtResponsibilitySearch.Text != "" && ddlManagerResponsibility.SelectedIndex == 0)
                {
                    searchquery = searchquery + " AND (responsibility_code = '" + txtResponsibilitySearch.Text.Split('-')[0].ToString().Trim() + "')";

                }
                else if (txtResponsibilitySearch.Text == "" && ddlManagerResponsibility.SelectedIndex != 0)
                {
                    searchquery = searchquery + " AND (manager_responsibility = '" + ddlManagerResponsibility.SelectedValue + "')";

                }
                DataRow[] foundRows = dtResponsibilityData.Select(searchquery);
                if (foundRows.Length != 0)
                {
                    dtSearched = foundRows.CopyToDataTable();
                }
                BindGrid(dtSearched);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (gridData.Rows.Count > 0)
            {
                gvResponsibilityDetails.DataSource = gridData;
                gvResponsibilityDetails.DataBind();
                PnlResponsibilityDetails.Style.Add("height", hdnGridPnlHeight.Value);
            }
            else
            {
                dtEmpty = new DataTable();
                gvResponsibilityDetails.DataSource = dtEmpty;
                gvResponsibilityDetails.EmptyDataText = "No data found for the selected filter criteria";
                gvResponsibilityDetails.DataBind();
                PnlResponsibilityDetails.Style.Add("height", hdnGridPnlHeight.Value);
            }
            UserAuthorization();
        }

        private void PopulateDropdowns(DataTable MngrRespList)
        {
            ddlNewManagerResp.DataSource = MngrRespList;
            ddlNewManagerResp.DataTextField = "responsibility";
            ddlNewManagerResp.DataValueField = "responsibility_code";
            ddlNewManagerResp.DataBind();
            ddlNewManagerResp.Items.Insert(0, new ListItem("-"));

            ddlManagerResponsibility.DataSource = MngrRespList;
            ddlManagerResponsibility.DataTextField = "responsibility";
            ddlManagerResponsibility.DataValueField = "responsibility_code";
            ddlManagerResponsibility.DataBind();
            ddlManagerResponsibility.Items.Insert(0, new ListItem("-"));
        }

        private void FuzzySearchResponsibility()
        {
            if (txtResponsibilitySearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in responsibility search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllResponsibilityList(txtResponsibilitySearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void UserAuthorization()
        {
            //Validation: Only SuperUser can select Update Status
            if (Session["UserRole"].ToString().ToLower() == UserRole.SuperUser.ToString().ToLower())
            {
                hdnIsSuperUser.Value = "Y";
            }

            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnSaveChanges.Enabled = false;
                //disable grid buttons
                foreach (GridViewRow rows in gvResponsibilityDetails.Rows)
                {
                    (rows.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
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