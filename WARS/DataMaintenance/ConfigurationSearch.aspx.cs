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
using System.Drawing;

namespace WARS
{
    public partial class ConfigurationSearch : System.Web.UI.Page
    {
        #region Global Declarations
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        ConfigurationSearchBL configurationSearchBL;
        #endregion Global Declarations

        #region Sorting
        string ascending = "Asc";
        string descending = "Desc";
        string sort_Up = "~/Images/Sort_up.png";
        string sort_Down = "~/Images/Sort_down.png";
        #endregion Sorting

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Configuration Search";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Configuration Search";
                }

                txtConfiguration.Focus();//tabbing sequence starts here
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadConfigTypeList();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                    tdGrid.Visible = false;
                }
                else
                {
                    PnlConfigurationGroup.Style.Add("height", hdnGridPnlGroupHeight.Value);
                }

                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void txtConfiguration_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnconfSearchSelected.Value == "Y")
                {

                    if (ValidateSelectedConfigCode())
                    {
                        string configurationCode = txtConfiguration.Text.Substring(0, txtConfiguration.Text.IndexOf("-") - 1);

                        LoadGridData(configurationCode);

                        tdGrid.Visible = true;
                    }
                    else
                    {
                        txtConfiguration.Text = string.Empty;
                    }
                }
                else if (hdnconfSearchSelected.Value == "N")
                {
                    FuzzySearchConfig();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());

                }
                else if (hdnconfSearchSelected.Value == string.Empty)
                {
                    LoadInitialGridData();
                }

                hdnconfSearchSelected.Value = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting the configuration.", ex.Message);
            }
        }

        protected void fuzzySearchConfig_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchConfig();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in configuration grouping fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtConfiguration.Text = string.Empty;
                    tdGrid.Visible = false;
                    return;
                }

                txtConfiguration.Text = lbFuzzySearch.SelectedValue.ToString();

                if (ValidateSelectedConfigCode())
                {
                    string configurationCode = txtConfiguration.Text.Substring(0, txtConfiguration.Text.IndexOf("-") - 1);

                    LoadGridData(configurationCode);

                    tdGrid.Visible = true;
                }
                else
                {
                    txtConfiguration.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void BtnAddConfigType_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("valAddConfigTypeGroup");
                if (!Page.IsValid)
                {
                    mpeAddConfigTypeCode.Show();
                }
                else
                {

                    string userCode = Convert.ToString(Session["UserCode"]);

                    configurationSearchBL = new ConfigurationSearchBL();
                    DataSet dtConfigTypeList = configurationSearchBL.SaveConfigGroup("I", txtConfigTypeCode.Text.ToUpper(), txtConfigTypeName.Text.ToUpper(), ddlConfigType.SelectedValue, userCode, out errorId);
                    configurationSearchBL = null;

                    if (dtConfigTypeList.Tables.Count > 0 && errorId == 0)
                    {
                        Session["FuzzySearchConfigGroupListTypeC"] = dtConfigTypeList.Tables[0];
                        msgView.SetMessage("Configuration added successfully.", MessageType.Warning, PositionType.Auto);
                        mpeAddConfigTypeCode.Hide();
                        txtConfigTypeCode.Text = "";
                        txtConfigTypeName.Text = "";
                        ddlConfigType.Text = "-";                        
                    }
                    else if (errorId == 1)
                    {
                        msgView.SetMessage("Configuration already exists.", MessageType.Warning, PositionType.Auto);
                        mpeAddConfigTypeCode.Show();
                    }
                    else
                    {
                        ExceptionHandler("Error in adding the new Configuration.", "");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding the new Configuration.", ex.Message);
            }

        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "fuzzySearchConfig" || hdnButtonSelection.Value == "ConfigSearch")
            {
                hdnconfSearchSelected.Value = string.Empty;
                txtConfiguration.Text = string.Empty;
                txtConfiguration_TextChanged(sender, e);
            }
                                   
        }

        protected void imgBtnConfigUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = Convert.ToString(Session["UserCode"]);

                Page.Validate("valUpdateConfigTypeGroup");
                if (!Page.IsValid)
                {
                    return;
                }

                configurationSearchBL = new ConfigurationSearchBL();
                DataSet dtConfigTypeList = configurationSearchBL.SaveConfigGroup("U", lblConfigTypeCode.Text.ToUpper(), txtGridConfigTypeName.Text.ToUpper(), ddlGridConfigType.SelectedValue, userCode, out errorId);
                configurationSearchBL = null;

                if (dtConfigTypeList.Tables.Count != 0 && errorId != 2)
                {
                    Session["FuzzySearchConfigGroupListTypeC"] = dtConfigTypeList.Tables[0];

                    DataTable dtConfigDtls = dtConfigTypeList.Tables[1];
                    lblConfigTypeCode.Text = dtConfigDtls.Rows[0]["config_code"].ToString();
                    hdnGridConfigTypeName.Value = txtGridConfigTypeName.Text = dtConfigDtls.Rows[0]["config_name"].ToString();
                    hdnGridConfigType.Value = ddlGridConfigType.SelectedValue = dtConfigDtls.Rows[0]["config_type"].ToString();

                    msgView.SetMessage("Configuration updated successfully.", MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to update Configuration.", MessageType.Warning, PositionType.Auto);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating Configuration.", ex.Message);
            }
        }

        protected void gvConfigurationGroup_RowDataBound(object sender, GridViewRowEventArgs e)
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


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvConfigurationGroup_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["ConfigGroupData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvConfigurationGroup.DataSource = dataView;
                gvConfigurationGroup.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion EVENTS

        #region METHODS

        private void LoadInitialGridData()
        {          
            tdGrid.Visible = false;
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnSave.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnAddNewConfigType.Enabled = false;
            }
        }

        private void LoadConfigTypeList()
        {
            configurationSearchBL = new ConfigurationSearchBL();
            DataSet configTypeData = configurationSearchBL.GetInitialData(out errorId);
            configurationSearchBL = null;

            if (configTypeData.Tables.Count != 0 && errorId != 2)
            {               
                ddlConfigType.DataSource = configTypeData.Tables[1];
                ddlConfigType.DataTextField = "config_type_desc";
                ddlConfigType.DataValueField = "config_type";
                ddlConfigType.DataBind();
                ddlConfigType.Items.Insert(0, new ListItem("-"));

                ddlGridConfigType.DataSource = configTypeData.Tables[1];
                ddlGridConfigType.DataTextField = "config_type_desc";
                ddlGridConfigType.DataValueField = "config_type";
                ddlGridConfigType.DataBind();
                ddlGridConfigType.Items.Insert(0, new ListItem("-"));
            }
            else if (configTypeData.Tables.Count == 0 && errorId != 2)
            {
                Session["FuzzySearchConfigGroupListTypeC"] = null;
            }
            else
            {
                ExceptionHandler("Error in loading Configuration Type", string.Empty);
            }
        }
       
        private void LoadGridData(string configurationCode)
        {
            configurationSearchBL = new ConfigurationSearchBL();
            DataSet configurationList = configurationSearchBL.GetConfigurationData(configurationCode, out errorId);
            configurationSearchBL = null;

            BindGrids(configurationList);
        }

        private void BindGrids(DataSet configurationList)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (configurationList.Tables.Count != 0 && errorId != 2)
            {
                DataTable dtconfigDtls = configurationList.Tables[0];

                lblConfigTypeCode.Text = dtconfigDtls.Rows[0]["config_code"].ToString();
                hdnGridConfigTypeName.Value = txtGridConfigTypeName.Text = dtconfigDtls.Rows[0]["config_name"].ToString();
                hdnGridConfigType.Value = ddlGridConfigType.SelectedValue = dtconfigDtls.Rows[0]["config_type"].ToString();

                Session["ConfigGroupData"] = configurationList.Tables[1];
                gvConfigurationGroup.DataSource = configurationList.Tables[1];
                if (configurationList.Tables[1].Rows.Count == 0)
                {
                    gvConfigurationGroup.EmptyDataText = "No data found for the selected Configuration.";
                }
                gvConfigurationGroup.DataBind();
            }
            else if (configurationList.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();                
                gvConfigurationGroup.DataSource = dtEmpty;
                gvConfigurationGroup.EmptyDataText = "No data found for the selected Configuration.";
                gvConfigurationGroup.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

        }

        private bool ValidateSelectedConfigCode()
        {
            if (txtConfiguration.Text != "" && Session["FuzzySearchConfigGroupListTypeC"] != null)
            {
                if (txtConfiguration.Text != "No results found")
                {
                    DataTable dtTerritoriesList;
                    dtTerritoriesList = Session["FuzzySearchConfigGroupListTypeC"] as DataTable;

                    foreach (DataRow dRow in dtTerritoriesList.Rows)
                    {
                        if (dRow["config_group"].ToString() == txtConfiguration.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        private void FuzzySearchConfig()
        {

            if (txtConfiguration.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in configuration filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchConfigGroupListTypeC(txtConfiguration.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        #endregion METHODS

            

       

    }
}