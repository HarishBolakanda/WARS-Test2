/*
File Name   :   StatementTextMaintenance.cs
Purpose     :   Used for maintaining statement text details.

Version     Date           Modified By        Modification Log
______________________________________________________________________
1.0       24-Aug-2018      Sreelekha          Initial Creation
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
    public partial class StatementTextMaintenance : System.Web.UI.Page
    {

        #region Global Declarations
        StatementTextMaintenanceBL statementTextMaintenanceBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Statement Text Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Statement Text Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlStatementTextDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        PopulateDropDowns();
                        LoadGridData();
                        util = null;
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                gvStatementTextDetails.DataSource = dtEmpty;
                gvStatementTextDetails.DataBind();
                ddlCompany.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reset.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                Array modifiedRowList = ModifiedRowsList();
                if (modifiedRowList.Length == 0)
                {
                    msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                StatementTextMaintenanceBL statementTextMaintenanceBL = new StatementTextMaintenanceBL();
                DataSet saveStatementTextDetails = statementTextMaintenanceBL.SaveStatementTextDetails(ddlCompany.SelectedValue, modifiedRowList, Convert.ToString(Session["UserCode"]), out errorId);
                statementTextMaintenanceBL = null;

                if (errorId == 2 || saveStatementTextDetails.Tables.Count == 0)
                {
                    ExceptionHandler("Error in saving statement text details", string.Empty);
                }
                else if (saveStatementTextDetails.Tables[0].Rows.Count == 0)
                {
                    LoadGridData();
                }
                else
                {
                    Session["StmntTextData"] = saveStatementTextDetails;
                    gvStatementTextDetails.DataSource = saveStatementTextDetails;
                    gvStatementTextDetails.DataBind();
                }

            }

            catch (Exception ex)
            {
                ExceptionHandler("Error in saving Statement Text Details", ex.Message);
            }
        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "Reset")
            {
                btnReset_Click(sender, e);
            }
            else
            {
                BindGridViewData();
            }
            hdnIsConfirmPopup.Value = "N";
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCompany.SelectedIndex > 0)
                {
                    BindGridViewData();
                }
                else
                {
                    dtEmpty = new DataTable();
                    gvStatementTextDetails.DataSource = dtEmpty;
                    gvStatementTextDetails.DataBind();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting company.", ex.Message);
            }
        }

        protected void btnYesConfirm_Click(object sender, EventArgs e)
        {
            btnSaveChanges_Click(sender, e);
        }

        protected void gvStatementTextDetails_RowDataBound(object sender, GridViewRowEventArgs e)
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



        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvStatementTextDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["StmntTextData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvStatementTextDetails.DataSource = dataView;
                gvStatementTextDetails.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion Events

        #region Methods

        private void LoadGridData()
        {
            dtEmpty = new DataTable();
            gvStatementTextDetails.EmptyDataText = "No data is displayed initially.";
            gvStatementTextDetails.DataSource = dtEmpty;
            gvStatementTextDetails.DataBind();
        }

        private void PopulateDropDowns()
        {

            statementTextMaintenanceBL = new StatementTextMaintenanceBL();
            DataSet dropdownListData = statementTextMaintenanceBL.GetDropDownData(out errorId);
            statementTextMaintenanceBL = null;

            if (dropdownListData.Tables.Count != 0 && errorId != 2)
            {
                ddlCompany.DataTextField = "com_name";
                ddlCompany.DataValueField = "com_number";
                ddlCompany.DataSource = dropdownListData.Tables[0];
                ddlCompany.DataBind();
                ddlCompany.Items.Insert(0, new ListItem("-"));
            }
            else
            {
                ExceptionHandler("Error in loading the dropdown list values.", string.Empty);
            }

        }

        private Array ModifiedRowsList()
        {
            List<string> modifiedRowsList = new List<string>();
            foreach (GridViewRow row in gvStatementTextDetails.Rows)
            {
                string hdnFieldId = (row.FindControl("hdnFieldId") as HiddenField).Value;
                string hdnFieldText = (row.FindControl("hdnFieldText") as HiddenField).Value;
                string txtStatementText = (row.FindControl("txtStatementText") as TextBox).Text;
                if (txtStatementText != hdnFieldText)
                {
                    modifiedRowsList.Add(hdnFieldId + Global.DBDelimiter + txtStatementText);
                }
            }

            return modifiedRowsList.ToArray();

        }

        private void BindGridViewData()
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            statementTextMaintenanceBL = new StatementTextMaintenanceBL();
            DataSet statementTextDetails = statementTextMaintenanceBL.GetStatementTextDetails(ddlCompany.SelectedValue, out errorId);
            statementTextMaintenanceBL = null;
            if (statementTextDetails.Tables.Count != 0 && errorId != 2)
            {
                Session["StmntTextData"] = statementTextDetails.Tables[0];
                gvStatementTextDetails.DataSource = statementTextDetails.Tables[0];
                gvStatementTextDetails.DataBind();
            }
            else
            {
                ExceptionHandler("Error in fetching data", string.Empty);
            }
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnSave.Enabled = false;
                foreach (GridViewRow rows in gvStatementTextDetails.Rows)
                {
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
