/*
File Name   :   ConfigurationGroupAudit.cs
Purpose     :   Configuration group audit screen

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     13-Jul-2018      Bala(Infosys Limited)   Initial Creation
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
using System.Drawing;

namespace WARS.Audit
{
    public partial class ConfigurationGroupAudit : System.Web.UI.Page
    {
        #region Global Declarations
        ConfigurationGroupAuditBL ConfigurationGroupAuditBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Configuration Group Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Configuration Group Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlConfigurationGroupAudit.Style.Add("height", hdnGridPnlHeight.Value);
                
                if (!IsPostBack)
                {
                    txtConfigurationGroupSearch.Focus();
                    tdData.Style.Add("display", "none");
                    trConfigGrpAudit.Style.Add("display", "none");
                    
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SearchConfigurationGroupAuditData(txtConfigurationGroupSearch.Text);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in searching audit data.", ex.Message);
            }
        }

        protected void gvConfigurationGroupAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                Label lblConfiguration;
                Label lblConfigurationName;

                string changeType;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblConfiguration = (e.Row.FindControl("lblConfiguration") as Label);
                    lblConfigurationName = (e.Row.FindControl("lblConfigurationName") as Label);

                    changeType = (e.Row.FindControl("hdnChangeType") as HiddenField).Value;

                    //For deleted records 
                    //Change the color of details to red
                    if (changeType == "D")
                    {
                        lblConfiguration.ForeColor = Color.Red;
                        lblConfigurationName.ForeColor = Color.Red;
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
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }

        protected void fuzzySearchConfigurationGroup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchConfigurationGroup();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor search.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtConfigurationGroupSearch.Text = string.Empty;
                    return;
                }

                txtConfigurationGroupSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                hdnIsValidSearch.Value = "Y";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }
        
        protected void btnConfigurationGroupMaint_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtConfigurationGroupSearch.Text))
                {
                    Response.Redirect(@"~/DataMaintenance/ConfigurationGrouping.aspx?configurationGroupCode=" + txtConfigurationGroupSearch.Text.Split('-')[0].ToString().Trim() + "", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid Configuration group from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to Configuration group maintenence screen.", ex.Message);
            }
        }

        protected void valConfigurationGroup_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtConfigurationGroupSearch.Text))
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating royaltor.", ex.Message);
            }
        }

        protected void valFromDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtFromDate.Text.Trim() != "__/__/____" && txtFromDate.Text.Trim() != "")
                {
                    if (DateTime.TryParse(txtFromDate.Text, out temp))
                    {
                        args.IsValid = true;
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

        protected void valToDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtToDate.Text.Trim() != "__/__/____" && txtToDate.Text.Trim() != "")
                {
                    if (DateTime.TryParse(txtToDate.Text, out temp))
                    {
                        if (txtFromDate.Text.Trim() != "__/__/____" && txtFromDate.Text.Trim() != "")
                        {
                            if (DateTime.TryParse(txtFromDate.Text, out temp))
                            {
                                if (Convert.ToDateTime(txtFromDate.Text) > Convert.ToDateTime(txtToDate.Text))
                                {
                                    valToDate.ToolTip = "From date should be earlier than To date";
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

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvConfigurationGroupAudit_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["ConfigGroupAuditData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvConfigurationGroupAudit.DataSource = dataView;
                gvConfigurationGroupAudit.DataBind();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion Events

        #region Methods

        private void LoadData()
        {  

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                string configGrpCode = Request.QueryString[0];
                hdnConfigurationGroupCode.Value = configGrpCode.ToString();
                hdnIsValidSearch.Value = "Y";
                //populate configuration group for the group code
                if (Session["FuzzySearchConfigGroupListTypeP"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetConfigGroupListTypeP", out errorId);
                    Session["FuzzySearchConfigGroupListTypeP"] = dsList.Tables[0];
                }
                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchConfigGroupListTypeP"]).Select("config_group_code = '" + configGrpCode + "'");
                txtConfigurationGroupSearch.Text = filteredRow[0]["config_group"].ToString();
                SearchConfigurationGroupAuditData(configGrpCode);
            }
        }

        private void SearchConfigurationGroupAuditData(string pConfigGrpCode)
        {

            Page.Validate("valSearch");
            if (!Page.IsValid)
            {
                dtEmpty = new DataTable();
                BindGrid(dtEmpty);

                msgView.SetMessage("Invalid search input!", MessageType.Warning, PositionType.Auto);
                return;
            }

            tdData.Style.Remove("display");
            trConfigGrpAudit.Style.Remove("display");

            string fromDate = string.Empty;
            string toDate = string.Empty;

            if (txtFromDate.Text.Trim() != "__/__/____")
            {
                fromDate = txtFromDate.Text.Trim();
            }

            if (txtToDate.Text.Trim() != "__/__/____")
            {
                toDate = txtToDate.Text.Trim();
            }
                        
            string configGrpCode = (txtConfigurationGroupSearch.Text == string.Empty ? pConfigGrpCode : txtConfigurationGroupSearch.Text.Split('-')[0].ToString().Trim());
            
            ConfigurationGroupAuditBL = new ConfigurationGroupAuditBL();
            DataSet auditData = ConfigurationGroupAuditBL.GetConfigurationGroupAuditData(configGrpCode, fromDate, toDate, out errorId);
            ConfigurationGroupAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {                
                BindGrid(auditData.Tables[0]);
                Session["ConfigGroupAuditData"] = auditData.Tables[0];
            }
            else
            {
                ExceptionHandler("Error in fetching royaltor payee audit data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (gridData.Rows.Count > 0)
            {
                gvConfigurationGroupAudit.DataSource = gridData;
                gvConfigurationGroupAudit.DataBind();
            }
            else
            {
                dtEmpty = new DataTable();
                gvConfigurationGroupAudit.DataSource = dtEmpty;
                gvConfigurationGroupAudit.DataBind();
            }
        }

        private void FuzzySearchConfigurationGroup()
        {
            if (txtConfigurationGroupSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in Configuration group search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchConfigGroupListTypeP(txtConfigurationGroupSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
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