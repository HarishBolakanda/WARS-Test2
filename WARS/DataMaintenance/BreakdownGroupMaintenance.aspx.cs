/*
File Name   :   BreakdownGroupMaintenance.cs
Purpose     :   to maintain Breakdown Group data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     29-Jun-2016     Pratik(Infosys Limited)   Initial Creation
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
    public partial class BreakdownGroupMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        BreakdownGroupMaintenanceBL breakdownGroupMaintenanceBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Breakdown Group Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Breakdown Group Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlBreakdownGroupDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtBreakdownGroupSearch.Focus();
                    Session["BreakdownGroupData"] = null;
                    Session["BGTerritoryList"] = null;
                    Session["BGConfigurationList"] = null;
                    Session["BGSalesTypeList"] = null;

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

        protected void btnHdnBreakdownGroupSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //JIRA-1048 Changes to handle single quote while searching --Start
                hdnSearchText.Value = txtBreakdownGroupSearch.Text.Replace("'", "").Trim();
                //JIRA-1048 Changes to handle single quote while searching --End
                PopulateGrid();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in breakdown group search.", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                LoadData();

                txtBreakdownGroupSearch.Text = string.Empty;
                hdnSearchText.Value = null;

                txtBreakdownGroupCodeInsert.Text = string.Empty;
                txtBreakdownGroupDescInsert.Text = string.Empty;
                ddlTerritoryInsert.SelectedIndex = 0;
                ddlConfigurationInsert.SelectedIndex = 0;
                ddlSalesTypeInsert.SelectedIndex = 0;
                txtGFSPLAccountInsert.Text = string.Empty;
                txtGFSBLAccountInsert.Text = string.Empty;
                hdnChangeNotSaved.Value = "N";
                hdnInsertDataNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;
                
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reseting page.", ex.Message);
            }
        }

        protected void gvBreakdownGroupDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Session["BGTerritoryList"] == null || Session["BGConfigurationList"] == null || Session["BGSalesTypeList"] == null)
                {
                    return;
                }

                DataTable dtTerritoryList = Session["BGTerritoryList"] as DataTable;
                DataTable dtConfigurationList = Session["BGConfigurationList"] as DataTable;
                DataTable dtSalesTypeList = Session["BGSalesTypeList"] as DataTable;
                DropDownList ddlTerritory;
                DropDownList ddlConfiguration;
                DropDownList ddlSalesType;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlTerritory = (e.Row.FindControl("ddlTerritory") as DropDownList);
                    string territory = (e.Row.FindControl("hdnTerritory") as HiddenField).Value;

                    ddlTerritory.DataSource = dtTerritoryList;
                    ddlTerritory.DataTextField = "seller_name";
                    ddlTerritory.DataValueField = "seller_group_code";
                    ddlTerritory.DataBind();
                    ddlTerritory.Items.Insert(0, new ListItem("-"));

                    if (dtTerritoryList.Select("seller_group_code = '" + territory + "'").Length != 0)
                    {
                        ddlTerritory.Items.FindByValue(territory).Selected = true;
                    }
                    else
                    {
                        ddlTerritory.SelectedIndex = 0;
                        (e.Row.FindControl("hdnTerritory") as HiddenField).Value = "-";
                    }

                    ddlConfiguration = (e.Row.FindControl("ddlConfiguration") as DropDownList);
                    string configuration = (e.Row.FindControl("hdnConfiguration") as HiddenField).Value;

                    ddlConfiguration.DataSource = dtConfigurationList;
                    ddlConfiguration.DataTextField = "config_name";
                    ddlConfiguration.DataValueField = "config_group_code";
                    ddlConfiguration.DataBind();
                    ddlConfiguration.Items.Insert(0, new ListItem("-"));

                    if (dtConfigurationList.Select("config_group_code = '" + configuration + "'").Length != 0)
                    {
                        ddlConfiguration.Items.FindByValue(configuration).Selected = true;
                    }
                    else
                    {
                        ddlConfiguration.SelectedIndex = 0;
                        (e.Row.FindControl("hdnConfiguration") as HiddenField).Value = "-";
                    }

                    ddlSalesType = (e.Row.FindControl("ddlSalesType") as DropDownList);
                    string salesType = (e.Row.FindControl("hdnSalesType") as HiddenField).Value;

                    ddlSalesType.DataSource = dtSalesTypeList;
                    ddlSalesType.DataTextField = "price_name";
                    ddlSalesType.DataValueField = "price_group_code";
                    ddlSalesType.DataBind();
                    ddlSalesType.Items.Insert(0, new ListItem("-"));

                    if (dtSalesTypeList.Select("price_group_code = '" + salesType + "'").Length != 0)
                    {
                        ddlSalesType.Items.FindByValue(salesType).Selected = true;
                    }
                    else
                    {
                        ddlSalesType.SelectedIndex = 0;
                        salesType = (e.Row.FindControl("hdnSalesType") as HiddenField).Value = "-";
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
        protected void gvBreakdownGroupDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["BreakdownGroupData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvBreakdownGroupDetails.DataSource = dataView;
                gvBreakdownGroupDetails.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void gvBreakdownGroupDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "saverow")
                {
                    UpdateBreakdownGroup();
                }
                else if (e.CommandName == "cancelrow")
                {
                    PopulateGrid();

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving breakdown group data.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    InsertBreakdownGroup();
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    UpdateBreakdownGroup();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving breakdown group data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtBreakdownGroupCodeInsert.Text = string.Empty;
                    txtBreakdownGroupDescInsert.Text = string.Empty;
                    ddlTerritoryInsert.SelectedIndex = 0;
                    ddlConfigurationInsert.SelectedIndex = 0;
                    ddlSalesTypeInsert.SelectedIndex = 0;
                    txtGFSPLAccountInsert.Text = string.Empty;
                    txtGFSBLAccountInsert.Text = string.Empty;
                    hdnInsertDataNotSaved.Value = "N";
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    PopulateGrid();

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading data grid.", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                InsertBreakdownGroup();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in creating breakdown group.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {
            breakdownGroupMaintenanceBL = new BreakdownGroupMaintenanceBL();
            DataSet initialData = breakdownGroupMaintenanceBL.GetInitialData(out errorId);
            breakdownGroupMaintenanceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["BreakdownGroupData"] = initialData.Tables[0];
                Session["BGTerritoryList"] = initialData.Tables[1];
                Session["BGConfigurationList"] = initialData.Tables[2];
                Session["BGSalesTypeList"] = initialData.Tables[3];

                BindGrid(initialData.Tables[0]);

                ddlTerritoryInsert.DataSource = initialData.Tables[1];
                ddlTerritoryInsert.DataTextField = "seller_name";
                ddlTerritoryInsert.DataValueField = "seller_group_code";
                ddlTerritoryInsert.DataBind();
                ddlTerritoryInsert.Items.Insert(0, new ListItem("-"));

                ddlConfigurationInsert.DataSource = initialData.Tables[2];
                ddlConfigurationInsert.DataTextField = "config_name";
                ddlConfigurationInsert.DataValueField = "config_group_code";
                ddlConfigurationInsert.DataBind();
                ddlConfigurationInsert.Items.Insert(0, new ListItem("-"));

                ddlSalesTypeInsert.DataSource = initialData.Tables[3];
                ddlSalesTypeInsert.DataTextField = "price_name";
                ddlSalesTypeInsert.DataValueField = "price_group_code";
                ddlSalesTypeInsert.DataBind();
                ddlSalesTypeInsert.Items.Insert(0, new ListItem("-"));
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }
        }

        private void PopulateGrid()
        {
            if (Session["BreakdownGroupData"] != null)
            {
                DataTable dtBreakdownGroupData = Session["BreakdownGroupData"] as DataTable;
                DataTable dtSearched = dtBreakdownGroupData.Clone();

                if (string.IsNullOrWhiteSpace(hdnSearchText.Value))
                {
                    dtSearched = dtBreakdownGroupData;
                }
                else
                {
                    DataRow[] foundRows = dtBreakdownGroupData.Select("breakdown_group like '%" + hdnSearchText.Value.Trim() + "%' or breakdown_desc like '%" + hdnSearchText.Value.Trim() + "%'");
                    if (foundRows.Length != 0)
                    {
                        dtSearched = foundRows.CopyToDataTable();
                    }
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
                gvBreakdownGroupDetails.DataSource = gridData;
                gvBreakdownGroupDetails.DataBind();
            }
            else
            {
                dtEmpty = new DataTable();
                gvBreakdownGroupDetails.DataSource = dtEmpty;
                gvBreakdownGroupDetails.DataBind();
            }
            UserAuthorization();
        }

        private void UpdateBreakdownGroup()
        {
            string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

            if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
            {
                int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                Page.Validate("GroupUpdate_" + rowIndex + "");
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Breakdown group details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                string breakdownGroupCode = ((Label)gvBreakdownGroupDetails.Rows[rowIndex].FindControl("lblBreakdownGroupCode")).Text;
                string breakdownGroupDesc = ((TextBox)gvBreakdownGroupDetails.Rows[rowIndex].FindControl("txtBreakdownGroupDesc")).Text;
                string territory = ((DropDownList)gvBreakdownGroupDetails.Rows[rowIndex].FindControl("ddlTerritory")).SelectedValue;
                string configuration = ((DropDownList)gvBreakdownGroupDetails.Rows[rowIndex].FindControl("ddlConfiguration")).SelectedValue;
                string salesType = ((DropDownList)gvBreakdownGroupDetails.Rows[rowIndex].FindControl("ddlSalesType")).SelectedValue;
                string GFSPLAccount = ((TextBox)gvBreakdownGroupDetails.Rows[rowIndex].FindControl("txtGFSPLAccount")).Text;
                string GFSBLAccount = ((TextBox)gvBreakdownGroupDetails.Rows[rowIndex].FindControl("txtGFSBLAccount")).Text;

                breakdownGroupMaintenanceBL = new BreakdownGroupMaintenanceBL();
                DataSet updatedData = breakdownGroupMaintenanceBL.UpdateBreakdownGroupData(breakdownGroupCode, breakdownGroupDesc, territory, configuration, salesType, GFSPLAccount, GFSBLAccount, userCode, out errorId);
                breakdownGroupMaintenanceBL = null;

                if (updatedData.Tables.Count != 0 && errorId != 2)
                {
                    Session["BreakdownGroupData"] = updatedData.Tables[0];

                    PopulateGrid();

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Breakdown group details saved successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to save breakdown group details.", MessageType.Warning, PositionType.Auto);
                }
            }
        }

        private void InsertBreakdownGroup()
        {
            Page.Validate("valInsertBreakdownGroup");
            if (!Page.IsValid)
            {
                msgView.SetMessage("Breakdown group details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                return;
            }

            string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

            breakdownGroupMaintenanceBL = new BreakdownGroupMaintenanceBL();
            DataSet updatedData = breakdownGroupMaintenanceBL.InsertBreakdownGroupData(txtBreakdownGroupCodeInsert.Text.Trim(), txtBreakdownGroupDescInsert.Text.Trim(), ddlTerritoryInsert.SelectedValue, ddlConfigurationInsert.SelectedValue, ddlSalesTypeInsert.SelectedValue, txtGFSPLAccountInsert.Text.Trim(), txtGFSBLAccountInsert.Text.Trim(), userCode, out errorId);
            breakdownGroupMaintenanceBL = null;

            if (errorId == 1)
            {
                msgView.SetMessage("Breakdown group exists with this group code.", MessageType.Success, PositionType.Auto);
            }
            else if (updatedData.Tables.Count != 0 && errorId != 2)
            {
                Session["BreakdownGroupData"] = updatedData.Tables[0];
                txtBreakdownGroupSearch.Text = string.Empty;
                hdnSearchText.Value = null;
                PopulateGrid();

                txtBreakdownGroupCodeInsert.Text = string.Empty;
                txtBreakdownGroupDescInsert.Text = string.Empty;
                ddlTerritoryInsert.SelectedIndex = 0;
                ddlConfigurationInsert.SelectedIndex = 0;
                ddlSalesTypeInsert.SelectedIndex = 0;
                txtGFSPLAccountInsert.Text = string.Empty;
                txtGFSBLAccountInsert.Text = string.Empty;

                hdnChangeNotSaved.Value = "N";
                hdnInsertDataNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;


                msgView.SetMessage("Breakdown group created successfully.", MessageType.Success, PositionType.Auto);
            }
            else
            {
                msgView.SetMessage("Failed to create breakdown group.", MessageType.Warning, PositionType.Auto);
            }
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnSaveChanges.Enabled = false;
                foreach (GridViewRow rows in gvBreakdownGroupDetails.Rows)
                {
                    (rows.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
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

        //JIRA-908 Changes by Ravi in 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string breakdownGroupCode = Convert.ToString(hdnDeleteBreakDownGrpCode.Value);
                breakdownGroupMaintenanceBL = new BreakdownGroupMaintenanceBL();
                DataSet updatedData = breakdownGroupMaintenanceBL.DeleteBreakdownGroupData(breakdownGroupCode, out errorId);
                breakdownGroupMaintenanceBL = null;

                if (updatedData.Tables.Count != 0 && errorId == 0)
                {
                    Session["BreakdownGroupData"] = updatedData.Tables[0];
                    txtBreakdownGroupSearch.Text = string.Empty;
                    hdnSearchText.Value = null;
                    PopulateGrid();

                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Breakdown group deleted successfully.", MessageType.Success, PositionType.Auto);
                    hdnDeleteBreakDownGrpCode.Value = "";
                }
                else
                {
                    msgView.SetMessage("Failed to delete breakdown group.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Failed to delete breakdown group.", ex.Message);
            }
        }
        //JIRA-908 Changes by Ravi in 13/02/2019 -- End
    }
}