/*
File Name   :   UserAccountMaint.cs
Purpose     :   to maintain User account data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     05-Oct-2016     Harish(Infosys Limited)   Initial Creation
2.0     07-Feb-2017     Pratik(Infosys Limited)   Added new fuzzy search functionality
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WARS.BusinessLayer;
using System.Data;
using System.Net;

namespace WARS
{
    public partial class UserAccountMaint : System.Web.UI.Page
    {
        #region Global Declarations        
        UserAccountMaintBL userAccountMaintBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtResponsibilityList;
        DataTable dtRoleList;
        DataTable dtPaymentRoleList;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "User Account Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "User Account Maintenance";
                }

                if (IsPostBack)
                {
                    //set gridview panel height                    
                    PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                }

                lblTab.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        hdnUserRole.Value = Session["UserRole"].ToString();
                        LoadData();
                        util = null;

                       
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                }
                UserAuthorization();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }

        }

        protected void txtUserSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnSearchListItemSelected.Value == "Y")
                {
                    UserSelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchUser();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                }
                else
                {
                    txtUserSearch.Text = string.Empty;
                }
                hdnSearchListItemSelected.Value = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data", ex.Message);
            }

        }

        protected void gvUserAccount_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DropDownList ddlResponsibility;
                DropDownList ddlrole;
                DropDownList ddlPaymentRole;
                CheckBox cbActive;
                ImageButton btnUpdate;
                string respCode;
                string roleId;
                string paymentRoleId;
                string isEnabled;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ddlResponsibility = (e.Row.FindControl("ddlResponsibility") as DropDownList);
                    ddlrole = (e.Row.FindControl("ddlrole") as DropDownList);
                    ddlPaymentRole = (e.Row.FindControl("ddlPaymentRole") as DropDownList);
                    cbActive = (e.Row.FindControl("cbActive") as CheckBox);
                    btnUpdate = (e.Row.FindControl("btnUpdate") as ImageButton);
                    isEnabled = (e.Row.FindControl("hdnIsEnabled") as HiddenField).Value;

                    dtResponsibilityList = (DataTable)Session["UserAccountResponsibility"];
                    if (dtResponsibilityList != null)
                    {
                        ddlResponsibility.DataSource = dtResponsibilityList;
                        ddlResponsibility.DataTextField = "responsibility_desc";
                        ddlResponsibility.DataValueField = "responsibility_code";
                        ddlResponsibility.DataBind();
                        ddlResponsibility.Items.Insert(0, new ListItem("-"));

                        respCode = (e.Row.FindControl("hdnRespCode") as HiddenField).Value;
                        if (ddlResponsibility.Items.FindByValue(respCode) != null)
                        {
                            ddlResponsibility.Items.FindByValue(respCode).Selected = true;
                        }
                        else
                        {
                            ddlResponsibility.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        ddlResponsibility.Items.Insert(0, new ListItem("-"));
                        ddlResponsibility.SelectedIndex = 0;
                    }

                    dtRoleList = (DataTable)Session["UserAccountRole"];

                    if (dtRoleList != null)
                    {
                        ddlrole.DataSource = dtRoleList;
                        ddlrole.DataTextField = "role_desc";
                        ddlrole.DataValueField = "role_id";
                        ddlrole.DataBind();
                        ddlrole.Items.Insert(0, new ListItem("-"));

                        roleId = (e.Row.FindControl("hdnRoleId") as HiddenField).Value;
                        if (ddlrole.Items.FindByValue(roleId) != null)
                        {
                            ddlrole.Items.FindByValue(roleId).Selected = true;
                        }
                        else
                        {
                            ddlrole.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        ddlrole.Items.Insert(0, new ListItem("-"));
                        ddlrole.SelectedIndex = 0;
                    }

                    dtPaymentRoleList = (DataTable)Session["UserAccountPaymentRole"];
                    if (dtPaymentRoleList != null)
                    {
                        ddlPaymentRole.DataSource = dtPaymentRoleList;
                        ddlPaymentRole.DataTextField = "role_desc";
                        ddlPaymentRole.DataValueField = "role_id";
                        ddlPaymentRole.DataBind();
                        ddlPaymentRole.Items.Insert(0, new ListItem("-"));

                        paymentRoleId = (e.Row.FindControl("hdnPaymentRoleId") as HiddenField).Value;
                        if (ddlPaymentRole.Items.FindByValue(paymentRoleId) != null)
                        {
                            ddlPaymentRole.Items.FindByValue(paymentRoleId).Selected = true;
                        }
                        else
                        {
                            ddlPaymentRole.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        ddlPaymentRole.Items.Insert(0, new ListItem("-"));
                        ddlPaymentRole.SelectedIndex = 0;
                    }

                    if (isEnabled == "Y")
                    {
                        cbActive.Checked = true;
                    }
                    else
                    {
                        cbActive.Checked = false;
                    }

                    //validation: only Super user can add/edit
                    if (hdnUserRole.Value.ToLower() != UserRole.SuperUser.ToString().ToLower())
                    {
                        btnUpdate.ToolTip = "Only super user can edit";
                        btnUpdate.Enabled = false;
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
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }
        }


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvUserAccount_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["UserAccountData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvUserAccount.DataSource = dataView;
                gvUserAccount.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void btnAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = "";
                hdnChangeNotSavedAdd.Value = "N";
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;


                userAccountMaintBL = new UserAccountMaintBL();
                DataSet gridData = userAccountMaintBL.AddUserAccount(txtUserNameAdd.Text.Trim(), txtUserCodeAdd.Text.Trim(), txtUserAccIdAdd.Text.Trim(),
                                   ddlResponsibilityAdd.SelectedValue, ddlroleAdd.SelectedValue, ddlPaymentRoleAdd.SelectedValue, cbActiveAdd.Checked == true ? "Y" : string.Empty,
                                   txtUserSearch.Text == string.Empty ? string.Empty : txtUserSearch.Text.Split('-')[1].ToString().Trim(), Convert.ToString(Session["UserCode"]), out errorId);
                userAccountMaintBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("User account alredy exists", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (gridData.Tables.Count != 0 && errorId != 2)
                {
                    dtResponsibilityList = gridData.Tables[0];
                    dtRoleList = gridData.Tables[1];
                    dtPaymentRoleList = gridData.Tables[2];
                    Session["FuzzyUserMaintUserList"] = gridData.Tables[3];

                    if (gridData.Tables[4].Rows.Count == 0)
                    {
                        gvUserAccount.DataSource = gridData.Tables[4];
                        gvUserAccount.EmptyDataText = "No data found";
                        gvUserAccount.DataBind();
                    }
                    else
                    {
                        gvUserAccount.DataSource = gridData.Tables[4];
                        gvUserAccount.DataBind();
                    }

                    txtNewResp.Text = ddlResponsibilityAdd.SelectedItem.Text;
                    hdnNewResp.Value = ddlResponsibilityAdd.SelectedValue;

                    ddlResponsibilityAdd.DataSource = dtResponsibilityList;
                    ddlResponsibilityAdd.DataTextField = "responsibility_desc";
                    ddlResponsibilityAdd.DataValueField = "responsibility_code";
                    ddlResponsibilityAdd.DataBind();
                    ddlResponsibilityAdd.Items.Insert(0, new ListItem("-"));

                    ddlroleAdd.DataSource = dtRoleList;
                    ddlroleAdd.DataTextField = "role_desc";
                    ddlroleAdd.DataValueField = "role_id";
                    ddlroleAdd.DataBind();
                    ddlroleAdd.Items.Insert(0, new ListItem("-"));

                    ddlRespToReplace.DataSource = dtResponsibilityList;
                    ddlRespToReplace.DataTextField = "responsibility_desc";
                    ddlRespToReplace.DataValueField = "responsibility_code";
                    ddlRespToReplace.DataBind();
                    ddlRespToReplace.Items.Insert(0, new ListItem("-"));

                    ClearAddFields();

                    mpeRespUpdate.Show();


                }
                else if (gridData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvUserAccount.DataSource = dtEmpty;
                    gvUserAccount.EmptyDataText = "No data found";
                    gvUserAccount.DataBind();

                    ClearAddFields();
                }
                else
                {
                    ExceptionHandler("Error in adding user", string.Empty);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding user", ex.Message);
            }
        }

        protected void btnUpdate_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = "";
                hdnChangeNotSavedAdd.Value = "N";

                ImageButton btnUpdate = (ImageButton)sender;
                GridViewRow gvr = ((ImageButton)sender).NamingContainer as GridViewRow;
                string userAccIdToUpdate = (gvr.FindControl("hdnUserAccId") as HiddenField).Value;
                string userCodeToUpdate = (gvr.FindControl("hdnUserCode") as HiddenField).Value;
                string respCodeToUpdate = (gvr.FindControl("hdnRespCode") as HiddenField).Value;
                string userName = (gvr.FindControl("txtUserName") as TextBox).Text;
                string userCode = (gvr.FindControl("txtUserCode") as TextBox).Text;
                string userAccId = (gvr.FindControl("txtUserAccId") as TextBox).Text;
                string respCode = (gvr.FindControl("ddlResponsibility") as DropDownList).SelectedValue;
                string roleId = (gvr.FindControl("ddlrole") as DropDownList).SelectedValue;
                string paymentRoleId = (gvr.FindControl("ddlPaymentRole") as DropDownList).SelectedValue;
                string isActive = (gvr.FindControl("cbActive") as CheckBox).Checked == true ? "Y" : string.Empty;

                userAccountMaintBL = new UserAccountMaintBL();
                DataSet gridData = userAccountMaintBL.UpdateUserAccount(userAccIdToUpdate, userCodeToUpdate, respCodeToUpdate, userName, userCode, userAccId, respCode, roleId, paymentRoleId, isActive,
                                   txtUserSearch.Text == string.Empty ? string.Empty : txtUserSearch.Text.Split('-')[1].ToString().Trim(), Convert.ToString(Session["UserCode"]), out errorId);
                userAccountMaintBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("User account alredy exists", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (gridData.Tables.Count != 0 && errorId != 2)
                {
                    dtResponsibilityList = gridData.Tables[0];
                    dtRoleList = gridData.Tables[1];
                    dtPaymentRoleList = gridData.Tables[2];
                    Session["FuzzyUserMaintUserList"] = gridData.Tables[3];

                    if (gridData.Tables[4].Rows.Count == 0)
                    {
                        gvUserAccount.DataSource = gridData.Tables[4];
                        gvUserAccount.EmptyDataText = "No data found";
                        gvUserAccount.DataBind();
                    }
                    else
                    {
                        gvUserAccount.DataSource = gridData.Tables[4];
                        gvUserAccount.DataBind();
                    }

                    if (respCodeToUpdate != respCode)
                    {
                        txtNewResp.Text = (gvr.FindControl("ddlResponsibility") as DropDownList).SelectedItem.Text;
                        hdnNewResp.Value = (gvr.FindControl("ddlResponsibility") as DropDownList).SelectedValue;

                        ddlRespToReplace.DataSource = dtResponsibilityList;
                        ddlRespToReplace.DataTextField = "responsibility_desc";
                        ddlRespToReplace.DataValueField = "responsibility_code";
                        ddlRespToReplace.DataBind();
                        ddlRespToReplace.Items.Insert(0, new ListItem("-"));

                        mpeRespUpdate.Show();
                    }


                    ddlResponsibilityAdd.DataSource = dtResponsibilityList;
                    ddlResponsibilityAdd.DataTextField = "responsibility_desc";
                    ddlResponsibilityAdd.DataValueField = "responsibility_code";
                    ddlResponsibilityAdd.DataBind();
                    ddlResponsibilityAdd.Items.Insert(0, new ListItem("-"));

                    ddlroleAdd.DataSource = dtRoleList;
                    ddlroleAdd.DataTextField = "role_desc";
                    ddlroleAdd.DataValueField = "role_id";
                    ddlroleAdd.DataBind();
                    ddlroleAdd.Items.Insert(0, new ListItem("-"));


                }
                else if (gridData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvUserAccount.DataSource = dtEmpty;
                    gvUserAccount.EmptyDataText = "No data found";
                    gvUserAccount.DataBind();
                }
                else
                {
                    ExceptionHandler("Error in updating user", string.Empty);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating user", ex.Message);
            }

            ClearAddFields();
        }

        protected void btnRespUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                userAccountMaintBL = new UserAccountMaintBL();
                userAccountMaintBL.UpdateRespChanges(Convert.ToInt32(ddlRespToReplace.SelectedValue), Convert.ToInt32(hdnNewResp.Value), Convert.ToString(Session["UserCode"]), out errorId);
                userAccountMaintBL = null;

                if (errorId != 2)
                {
                    mpeRespUpdate.Hide();
                }
                else
                {
                    ExceptionHandler("Error in updating responsibility changes", string.Empty);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating responsibility changes", ex.Message);
            }


        }

        protected void btnYesConfirm_Click(object sender, EventArgs e)
        {
            hdnChangeNotSaved.Value = "N";
            hdnGridRowSelectedPrvious.Value = "";
            hdnChangeNotSavedAdd.Value = "N";
            ClearAddFields();

            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            userAccountMaintBL = new UserAccountMaintBL();
            DataSet gridData = userAccountMaintBL.GetData(string.Empty, out errorId);
            userAccountMaintBL = null;

            if (gridData.Tables.Count != 0 && errorId != 2)
            {
                dtResponsibilityList = gridData.Tables[0];
                dtRoleList = gridData.Tables[1];
                dtPaymentRoleList = gridData.Tables[2];
                Session["FuzzyUserMaintUserList"] = gridData.Tables[3];

                if (gridData.Tables[4].Rows.Count == 0)
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.EmptyDataText = "No data found";
                    gvUserAccount.DataBind();
                }
                else
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.DataBind();
                }


            }
            else if (gridData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvUserAccount.DataSource = dtEmpty;
                gvUserAccount.EmptyDataText = "No data found";
                gvUserAccount.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }
        }

        protected void fuzzySearchUser_Click(object sender, ImageClickEventArgs e)
        {
            FuzzySearchUser();
            lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtUserSearch.Text = string.Empty;
                    return;
                }
                hdnUserSearchSelected.Value = "Y";
                txtUserSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                UserSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting item from search list", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtUserSearch.Text = string.Empty;
                mpeFuzzySearch.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing search list", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;
            hdnChangeNotSaved.Value = "N";
            hdnGridRowSelectedPrvious.Value = "";
            hdnChangeNotSavedAdd.Value = "N";
            ClearAddFields();
            txtUserSearch.Text = string.Empty;


            userAccountMaintBL = new UserAccountMaintBL();
            DataSet gridData = userAccountMaintBL.GetData(string.Empty, out errorId);
            userAccountMaintBL = null;

            if (gridData.Tables.Count != 0 && errorId != 2)
            {
                dtResponsibilityList = gridData.Tables[0];
                dtRoleList = gridData.Tables[1];
                dtPaymentRoleList = gridData.Tables[2];
                Session["FuzzyUserMaintUserList"] = gridData.Tables[3];

                if (gridData.Tables[4].Rows.Count == 0)
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.EmptyDataText = "No data found";
                    gvUserAccount.DataBind();
                }
                else
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.DataBind();
                }


            }
            else if (gridData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvUserAccount.DataSource = dtEmpty;
                gvUserAccount.EmptyDataText = "No data found";
                gvUserAccount.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }
        }

        protected void imgBtnUndo_Click(object sender, ImageClickEventArgs e)
        {
            hdnChangeNotSaved.Value = "N";
            hdnGridRowSelectedPrvious.Value = "";

            userAccountMaintBL = new UserAccountMaintBL();
            DataSet gridData = userAccountMaintBL.GetData(string.Empty, out errorId);
            userAccountMaintBL = null;

            if (gridData.Tables.Count != 0 && errorId != 2)
            {
                dtResponsibilityList = gridData.Tables[0];
                dtRoleList = gridData.Tables[1];
                dtPaymentRoleList = gridData.Tables[2];
                Session["FuzzyUserMaintUserList"] = gridData.Tables[3];

                if (gridData.Tables[4].Rows.Count == 0)
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.EmptyDataText = "No data found";
                    gvUserAccount.DataBind();
                }
                else
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.DataBind();
                }


            }
            else if (gridData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvUserAccount.DataSource = dtEmpty;
                gvUserAccount.EmptyDataText = "No data found";
                gvUserAccount.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }
        }

        protected void imgBtnCancel_Click(object sender, ImageClickEventArgs e)
        {
            hdnChangeNotSavedAdd.Value = "N";
            ClearAddFields();
        }

        #endregion Events

        #region Methods

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void LoadData()
        {
            userAccountMaintBL = new UserAccountMaintBL();
            DataSet gridData = userAccountMaintBL.GetData(string.Empty, out errorId);
            userAccountMaintBL = null;

            if (gridData.Tables.Count != 0 && errorId != 2)
            {
                dtResponsibilityList = gridData.Tables[0];
                dtRoleList = gridData.Tables[1];
                dtPaymentRoleList = gridData.Tables[2];
                Session["FuzzyUserMaintUserList"] = gridData.Tables[3];

                Session["UserAccountResponsibility"] = dtResponsibilityList;
                ddlResponsibilityAdd.DataSource = dtResponsibilityList;
                ddlResponsibilityAdd.DataTextField = "responsibility_desc";
                ddlResponsibilityAdd.DataValueField = "responsibility_code";
                ddlResponsibilityAdd.DataBind();
                ddlResponsibilityAdd.Items.Insert(0, new ListItem("-"));

                Session["UserAccountRole"] = dtRoleList;
                ddlroleAdd.DataSource = dtRoleList;
                ddlroleAdd.DataTextField = "role_desc";
                ddlroleAdd.DataValueField = "role_id";
                ddlroleAdd.DataBind();
                ddlroleAdd.Items.Insert(0, new ListItem("-"));

                Session["UserAccountPaymentRole"] = dtPaymentRoleList;
                ddlPaymentRoleAdd.DataSource = dtPaymentRoleList;
                ddlPaymentRoleAdd.DataTextField = "role_desc";
                ddlPaymentRoleAdd.DataValueField = "role_id";
                ddlPaymentRoleAdd.DataBind();
                ddlPaymentRoleAdd.Items.Insert(0, new ListItem("-"));

                if (gridData.Tables[4].Rows.Count == 0)
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.EmptyDataText = "No data found";
                    gvUserAccount.DataBind();
                }
                else
                {
                    Session["UserAccountData"] = gridData.Tables[4];
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.DataBind();
                }

            }
            else if (gridData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvUserAccount.DataSource = dtEmpty;
                gvUserAccount.EmptyDataText = "No data found";
                gvUserAccount.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }


        }

        private void ClearAddFields()
        {
            txtUserNameAdd.Text = string.Empty;
            txtUserCodeAdd.Text = string.Empty;
            txtUserAccIdAdd.Text = string.Empty;
            ddlResponsibilityAdd.ClearSelection();
            ddlroleAdd.ClearSelection();
            ddlPaymentRoleAdd.ClearSelection();
            cbActiveAdd.Checked = false;
        }

        private void UserSelected()
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            hdnChangeNotSaved.Value = "N";
            hdnGridRowSelectedPrvious.Value = "";
            hdnChangeNotSavedAdd.Value = "N";
            if (txtUserSearch.Text != string.Empty && (txtUserSearch.Text == "No results found" || hdnUserSearchSelected.Value == "N"))
            {
                msgView.SetMessage("Please select a valid user from the list", MessageType.Warning, PositionType.Auto);
                txtUserSearch.Text = string.Empty;
                return;

            }

            userAccountMaintBL = new UserAccountMaintBL();
            DataSet gridData = userAccountMaintBL.GetData(txtUserSearch.Text == string.Empty ? string.Empty : txtUserSearch.Text.Split('-')[1].ToString().Trim(), out errorId);
            userAccountMaintBL = null;

            if (gridData.Tables.Count != 0 && errorId != 2)
            {

                dtResponsibilityList = gridData.Tables[0];
                dtRoleList = gridData.Tables[1];
                dtPaymentRoleList = gridData.Tables[2];
                Session["FuzzyUserMaintUserList"] = gridData.Tables[3];

                if (gridData.Tables[4].Rows.Count == 0)
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.EmptyDataText = "No data found";
                    gvUserAccount.DataBind();
                }
                else
                {
                    gvUserAccount.DataSource = gridData.Tables[4];
                    gvUserAccount.DataBind();
                }

            }
            else if (gridData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvUserAccount.DataSource = dtEmpty;
                gvUserAccount.EmptyDataText = "No data found for the selected filter criteria";
                gvUserAccount.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }
        }

        private void FuzzySearchUser()
        {            
            if (txtUserSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in user search filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyUserMaintUserList(txtUserSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void UserAuthorization()
        {
            //validation: only Super user can add/edit
            if (hdnUserRole.Value.ToLower() != UserRole.SuperUser.ToString().ToLower())
            {
                btnAdd.ToolTip = "Only super user can add";
                btnAdd.Enabled = false;

                //WUIN-1096 Only Read access for Reaonly User
                if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
                {
                    imgBtnCancel.Enabled = false;
                    btnRespUpdate.Enabled = false;
                    foreach (GridViewRow rows in gvUserAccount.Rows)
                    {
                        (rows.FindControl("btnUpdate") as ImageButton).Enabled = false;
                        (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                    }
                }

            } 

            
        }

        #endregion Methods

                
    }
}