/*
File Name   :   AccountTypeMaintenance.cs
Purpose     :   to maintain Account Type data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     19-May-2016     Pratik(Infosys Limited)   Initial Creation
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
    public partial class AccountTypeMaintenance : System.Web.UI.Page
    {
        #region Global Declarations

        AccountTypeMaintenanceBL accountTypeMaintenanceBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Account Type Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Account Type Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlAccountTypeDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtAccountTypeSearch.Focus();
                    Session["AccountTypeData"] = null;
                    Session["AccountTypeList"] = null;
                    Session["AccSourceTypeList"] = null;

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

        protected void btnHdnAccountTypeSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //JIRA-1048 Changes to handle single quote while searching --Start
                hdnSearchText.Value = txtAccountTypeSearch.Text.Replace("'", "").Trim();
                //JIRA-1048 Changes to handle single quote while searching --End
                PopulateGrid();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in account type search.", ex.Message);
            }
        }

        protected void gvAccountTypeDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Session["AccSourceTypeList"] == null)
                {
                    return;
                }

                DataTable dtSourceTypeList = Session["AccSourceTypeList"] as DataTable;
                DropDownList ddlSourceType;
                CheckBox cbConsolid;
                CheckBox cbIsInclude;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlSourceType = (e.Row.FindControl("ddlSourceType") as DropDownList);
                    string sourceType = (e.Row.FindControl("hdnSourceType") as HiddenField).Value;

                    ddlSourceType.DataSource = dtSourceTypeList;
                    ddlSourceType.DataTextField = "dropdown_text";
                    ddlSourceType.DataValueField = "dropdown_value";
                    ddlSourceType.DataBind();
                    ddlSourceType.Items.Insert(0, new ListItem("-"));

                    if (dtSourceTypeList.Select("dropdown_value = '" + sourceType + "'").Length != 0)
                    {
                        ddlSourceType.Items.FindByValue(sourceType).Selected = true;
                    }
                    else
                    {
                        ddlSourceType.SelectedIndex = 0;
                    }

                    cbConsolid = (e.Row.FindControl("cbConsolid") as CheckBox);
                    string consolid = (e.Row.FindControl("hdnConsolid") as HiddenField).Value;

                    if (consolid == "Y")
                    {
                        cbConsolid.Checked = true;
                    }

                    cbIsInclude = (e.Row.FindControl("cbIsInclude") as CheckBox);
                    string isInclude = (e.Row.FindControl("hdnIsInclude") as HiddenField).Value;

                    if (isInclude == "Y")
                    {
                        cbIsInclude.Checked = true;
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

        protected void gvAccountTypeDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

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

                        //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                        string accountTypeCode = ((Label)gvAccountTypeDetails.Rows[rowIndex].FindControl("lblAccountTypeCode")).Text;
                        string isInclude = ((HiddenField)gvAccountTypeDetails.Rows[rowIndex].FindControl("hdnIsInclude")).Value;
                        string sourceType = ((DropDownList)gvAccountTypeDetails.Rows[rowIndex].FindControl("ddlSourceType")).SelectedValue;

                        if (sourceType == "-")
                        {
                            sourceType = string.Empty;
                        }

                        string isConsolid;
                        if (((CheckBox)gvAccountTypeDetails.Rows[rowIndex].FindControl("cbConsolid")).Checked)
                        {
                            isConsolid = "Y";
                        }
                        else
                        {
                            isConsolid = "N";
                        }

                        string isIncludeNew;
                        if (((CheckBox)gvAccountTypeDetails.Rows[rowIndex].FindControl("cbIsInclude")).Checked)
                        {
                            isIncludeNew = "Y";
                        }
                        else
                        {
                            isIncludeNew = "N";
                        }

                        accountTypeMaintenanceBL = new AccountTypeMaintenanceBL();
                        DataSet accountTypeData = accountTypeMaintenanceBL.UpdateAccountTypeMapData(accountTypeCode, isInclude, sourceType, isConsolid, isIncludeNew, userCode, out errorId);
                        accountTypeMaintenanceBL = null;

                        if (errorId == 1)
                        {
                            msgView.SetMessage("Account type exists with this account code and include value combination.", MessageType.Success, PositionType.Auto);
                        }
                        else if (accountTypeData.Tables.Count != 0 && errorId != 2)
                        {
                            Session["AccountTypeData"] = accountTypeData.Tables[0];

                            PopulateGrid();

                            hdnChangeNotSaved.Value = "N";
                            hdnGridRowSelectedPrvious.Value = null;
                            msgView.SetMessage("Account Type details saved successfully.", MessageType.Success, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("Failed to save account type details.", MessageType.Warning, PositionType.Auto);
                        }
                    }
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
                ExceptionHandler("Error in saving account type data.", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                string accountTypeId;
                string sourceType;                
                string isConsolid;
                string isInclude;

                if (ddlAccountTypeDescInsert.SelectedIndex > 0)
                {
                    accountTypeId = ddlAccountTypeDescInsert.SelectedValue;
                }
                else
                {
                    accountTypeId = null;
                }

                if (ddlSourceTypeInsert.SelectedIndex > 0)
                {
                    sourceType = ddlSourceTypeInsert.SelectedValue;
                }
                else
                {
                    sourceType = string.Empty;
                }

                if (cbConsolidInsert.Checked)
                {
                    isConsolid = "Y";
                }
                else
                {
                    isConsolid = "N";
                }
                
                if (cbInIncludeInsert.Checked)
                {
                    isInclude = "Y";
                }
                else
                {
                    isInclude = "N";
                }

                accountTypeMaintenanceBL = new AccountTypeMaintenanceBL();
                DataSet accountTypeData = accountTypeMaintenanceBL.InsertAccountTypeMapData(txtAccountTypeCode.Text, accountTypeId, sourceType,
                    isConsolid, isInclude, userCode, out errorId);
                accountTypeMaintenanceBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("Account type exists with this account code and include value combination.", MessageType.Success, PositionType.Auto);
                }
                else if (accountTypeData.Tables.Count != 0 && errorId != 2)
                {
                    Session["AccountTypeData"] = accountTypeData.Tables[0];
                    BindGrid(accountTypeData.Tables[0]);
                    txtAccountTypeCode.Text = string.Empty;
                    lblDisplayOrderInsert.Text = string.Empty;
                    cbConsolidInsert.Checked = false;
                    cbInIncludeInsert.Checked = false;
                    ddlAccountTypeDescInsert.SelectedIndex = 0;
                    ddlSourceTypeInsert.SelectedIndex = 0;
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    hdnSearchText.Value = null;
                    txtAccountTypeSearch.Text = string.Empty;
                    msgView.SetMessage("Account Type details saved successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to save account type details.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in inserting account type details.", ex.Message);
            }
        }

        protected void imgBtnCancel_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtAccountTypeCode.Text = string.Empty;
                lblDisplayOrderInsert.Text = string.Empty;
                cbConsolidInsert.Checked = false;
                cbInIncludeInsert.Checked = false;
                ddlAccountTypeDescInsert.SelectedIndex = 0;
                ddlSourceTypeInsert.SelectedIndex = 0;
                hdnInsertDataNotSaved.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing insert fields.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    //Validate
                    Page.Validate("valInsertAccountType");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                    string accountTypeId;
                    string sourceType;
                    string isConsolid;
                    string isInclude;

                    if (ddlAccountTypeDescInsert.SelectedIndex > 0)
                    {
                        accountTypeId = ddlAccountTypeDescInsert.SelectedValue;
                    }
                    else
                    {
                        accountTypeId = null;
                    }

                    if (ddlSourceTypeInsert.SelectedIndex > 0)
                    {
                        sourceType = ddlSourceTypeInsert.SelectedValue;
                    }
                    else
                    {
                        sourceType = string.Empty;
                    }

                    if (cbConsolidInsert.Checked)
                    {
                        isConsolid = "Y";
                    }
                    else
                    {
                        isConsolid = "N";
                    }

                    if (cbInIncludeInsert.Checked)
                    {
                        isInclude = "Y";
                    }
                    else
                    {
                        isInclude = "N";
                    }

                    accountTypeMaintenanceBL = new AccountTypeMaintenanceBL();
                    DataSet accountTypeData = accountTypeMaintenanceBL.InsertAccountTypeMapData(txtAccountTypeCode.Text, accountTypeId, sourceType,
                        isConsolid, isInclude, userCode, out errorId);
                    accountTypeMaintenanceBL = null;

                    if (errorId == 1)
                    {
                        msgView.SetMessage("Account type exists with this account code and include value combination.", MessageType.Success, PositionType.Auto);
                    }
                    else if (accountTypeData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["AccountTypeData"] = accountTypeData.Tables[0];
                        BindGrid(accountTypeData.Tables[0]);
                        txtAccountTypeCode.Text = string.Empty;
                        lblDisplayOrderInsert.Text = string.Empty;
                        cbConsolidInsert.Checked = false;
                        cbInIncludeInsert.Checked = false;
                        ddlAccountTypeDescInsert.SelectedIndex = 0;
                        ddlSourceTypeInsert.SelectedIndex = 0;
                        hdnChangeNotSaved.Value = "N";
                        hdnInsertDataNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        hdnSearchText.Value = null;
                        txtAccountTypeSearch.Text = string.Empty;
                        msgView.SetMessage("Account Type details saved successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to save account type details.", MessageType.Warning, PositionType.Auto);
                    }
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);

                    //Validate
                    Page.Validate("GroupUpdate_" + rowIndex + "");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                    string accountTypeCode = ((Label)gvAccountTypeDetails.Rows[rowIndex].FindControl("lblAccountTypeCode")).Text;
                    string isInclude = ((HiddenField)gvAccountTypeDetails.Rows[rowIndex].FindControl("hdnIsInclude")).Value;
                    string sourceType = ((DropDownList)gvAccountTypeDetails.Rows[rowIndex].FindControl("ddlSourceType")).SelectedValue;

                    if (sourceType == "-")
                    {
                        sourceType = string.Empty;
                    }

                    string isConsolid;
                    if (((CheckBox)gvAccountTypeDetails.Rows[rowIndex].FindControl("cbConsolid")).Checked)
                    {
                        isConsolid = "Y";
                    }
                    else
                    {
                        isConsolid = "N";
                    }

                    string isIncludeNew;
                    if (((CheckBox)gvAccountTypeDetails.Rows[rowIndex].FindControl("cbIsInclude")).Checked)
                    {
                        isIncludeNew = "Y";
                    }
                    else
                    {
                        isIncludeNew = "N";
                    }

                    accountTypeMaintenanceBL = new AccountTypeMaintenanceBL();
                    DataSet accountTypeData = accountTypeMaintenanceBL.UpdateAccountTypeMapData(accountTypeCode, isInclude, sourceType, isConsolid, isIncludeNew, userCode, out errorId);
                    accountTypeMaintenanceBL = null;

                    if (errorId == 1)
                    {
                        msgView.SetMessage("Account type exists with this account code and include value combination.", MessageType.Success, PositionType.Auto);
                    }
                    else if (accountTypeData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["AccountTypeData"] = accountTypeData.Tables[0];

                        PopulateGrid();

                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Account Type details updated successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to updated account type details.", MessageType.Warning, PositionType.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving account type data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtAccountTypeCode.Text = string.Empty;
                    lblDisplayOrderInsert.Text = string.Empty;
                    cbConsolidInsert.Checked = false;
                    cbInIncludeInsert.Checked = false;
                    ddlAccountTypeDescInsert.SelectedIndex = 0;
                    ddlSourceTypeInsert.SelectedIndex = 0;
                    hdnInsertDataNotSaved.Value = "N";
                }

                if (hdnChangeNotSaved.Value == "Y")
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                accountTypeMaintenanceBL = new AccountTypeMaintenanceBL();
                DataSet initialData = accountTypeMaintenanceBL.GetInitialData(out errorId);
                accountTypeMaintenanceBL = null;

                if (initialData.Tables.Count != 0 && errorId != 2)
                {
                    Session["AccountTypeData"] = initialData.Tables[0];
                    Session["AccountTypeList"] = initialData.Tables[1];

                    BindGrid(initialData.Tables[0]);

                    ddlAccountTypeDescInsert.DataSource = initialData.Tables[1];
                    ddlAccountTypeDescInsert.DataTextField = "account_type_desc";
                    ddlAccountTypeDescInsert.DataValueField = "account_type_id";
                    ddlAccountTypeDescInsert.DataBind();
                    ddlAccountTypeDescInsert.Items.Insert(0, new ListItem("-"));
                }
                else
                {
                    ExceptionHandler("Error in fetching data", string.Empty);
                }

                txtAccountTypeSearch.Text = string.Empty;
                txtAccountTypeCode.Text = string.Empty;
                lblDisplayOrderInsert.Text = string.Empty;
                cbConsolidInsert.Checked = false;
                cbInIncludeInsert.Checked = false;
                ddlAccountTypeDescInsert.SelectedIndex = 0;
                ddlSourceTypeInsert.SelectedIndex = 0;
                hdnChangeNotSaved.Value = "N";
                hdnInsertDataNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;
                hdnSearchText.Value = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reseting page.", ex.Message);
            }
        }

        protected void btnAccountTypeAudit_Click(object sender, EventArgs e)
        {
            msgView.SetMessage("Screen to be developed.", MessageType.Warning, PositionType.Auto);
        }
                                      
        protected void btnAddAccountType_Click(object sender, EventArgs e)
        {
            try
            {
                txtDesciption.Text = string.Empty;
                txtDisplayOrder.Text = string.Empty;
                mpeAddAccountType.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding new account type.", ex.Message);
            }
        }

        protected void btnInsertAccountType_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                accountTypeMaintenanceBL = new AccountTypeMaintenanceBL();
                DataSet accountTypeData = accountTypeMaintenanceBL.InsertAccountTypeData(txtDesciption.Text, txtDisplayOrder.Text, userCode, out errorId);
                accountTypeMaintenanceBL = null;

                if (accountTypeData.Tables.Count != 0 && errorId != 2)
                {
                    Session["AccountTypeList"] = accountTypeData.Tables[0];
                    ddlAccountTypeDescInsert.DataSource = accountTypeData.Tables[0];
                    ddlAccountTypeDescInsert.DataTextField = "account_type_desc";
                    ddlAccountTypeDescInsert.DataValueField = "account_type_id";
                    ddlAccountTypeDescInsert.DataBind();
                    ddlAccountTypeDescInsert.Items.Insert(0, new ListItem("-"));
                    txtDesciption.Text = string.Empty;
                    txtDisplayOrder.Text = string.Empty;
                    msgView.SetMessage("Account type created successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to create account type.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding new account type.", ex.Message);
            }
        }

        protected void ddlAccountTypeDescInsert_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Session["AccountTypeList"] == null)
                {
                    return;
                }

                DataTable dtAccountTypeList = Session["AccountTypeList"] as DataTable;

                if (ddlAccountTypeDescInsert.SelectedIndex > 0)
                {
                    if (dtAccountTypeList.Select("Convert(account_type_id,System.String) like '%" + ddlAccountTypeDescInsert.SelectedValue + "%'").Length > 0)
                    {
                        lblDisplayOrderInsert.Text = dtAccountTypeList.Select("Convert(account_type_id,System.String) like '%" + ddlAccountTypeDescInsert.SelectedValue + "%'")[0]["display_order"].ToString();
                    }
                }
                else
                {
                    lblDisplayOrderInsert.Text = string.Empty;
                }

                ddlAccountTypeDescInsert.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in displaying display order.", ex.Message);
            }
        }

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvAccountTypeDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["AccountTypeData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvAccountTypeDetails.DataSource = dataView;
                gvAccountTypeDetails.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion Events

        #region Methods

        private void LoadData()
        {
            accountTypeMaintenanceBL = new AccountTypeMaintenanceBL();
            DataSet initialData = accountTypeMaintenanceBL.GetInitialData(out errorId);
            accountTypeMaintenanceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["AccountTypeData"] = initialData.Tables[0];
                Session["AccountTypeList"] = initialData.Tables[1];
                Session["AccSourceTypeList"] = initialData.Tables[2];

                BindGrid(initialData.Tables[0]);

                ddlAccountTypeDescInsert.DataSource = initialData.Tables[1];
                ddlAccountTypeDescInsert.DataTextField = "account_type_desc";
                ddlAccountTypeDescInsert.DataValueField = "account_type_id";
                ddlAccountTypeDescInsert.DataBind();
                ddlAccountTypeDescInsert.Items.Insert(0, new ListItem("-"));

                ddlSourceTypeInsert.DataSource = initialData.Tables[2];
                ddlSourceTypeInsert.DataTextField = "dropdown_text";
                ddlSourceTypeInsert.DataValueField = "dropdown_value";
                ddlSourceTypeInsert.DataBind();
                ddlSourceTypeInsert.Items.Insert(0, new ListItem("-"));
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }
        }

        private void PopulateGrid()
        {
            if (Session["AccountTypeData"] != null)
            {
                DataTable dtAccountTypeData = Session["AccountTypeData"] as DataTable;
                DataTable dtSearched = dtAccountTypeData.Clone();

                if (string.IsNullOrWhiteSpace(hdnSearchText.Value))
                {
                    dtSearched = dtAccountTypeData;
                }
                else
                {
                    DataRow[] foundRows = dtAccountTypeData.Select("account_code_type like '%" + hdnSearchText.Value.Trim() + "%' or account_type_desc like '%" + hdnSearchText.Value.Trim() + "%'");
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
                gvAccountTypeDetails.DataSource = gridData;
                gvAccountTypeDetails.DataBind();

            }
            else
            {
                dtEmpty = new DataTable();
                gvAccountTypeDetails.DataSource = dtEmpty;
                gvAccountTypeDetails.DataBind();
            }
            UserAuthorization();
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnAddAccountType.Enabled = false;
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnSaveChanges.Enabled = false;
                //disable grid buttons
                foreach (GridViewRow gvr in gvAccountTypeDetails.Rows)
                {
                    (gvr.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
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