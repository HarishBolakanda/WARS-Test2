/*
File Name   :   ConfigurationGrouping.cs
Purpose     :   to maintain Configuration groups based on group ID

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     25-Oct-2016     Pratik(Infosys Limited)   Initial Creation
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
using System.Drawing;

namespace WARS
{
    public partial class ConfigurationGrouping : System.Web.UI.Page
    {
        #region Global Declarations
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        ConfigurationGroupingBL configurationGroupingBL;
        string ASCENDING = " ASC";
        string DESCENDING = " DESC";
        #endregion Global Declarations

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Configuration Group";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Configuration Group";
                }

                PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
                PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);

                txtConfigurationGroup.Focus();//tabbing sequence starts here
                if (!IsPostBack)
                {
                    
                    //clear sessions                    
                    Session["ConfigGroupInData"] = null;
                    Session["ConfigGroupOutData"] = null;                    
                    cbOutSelectAll.Enabled = false;
                    cbInSelectAll.Enabled = false;
                    btnAddConfiguration.Enabled = false;
                    btnRemoveConfiguration.Enabled = false;
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadConfigurationGroups();
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
        
        //WUIN-569 To enable role based access
        private void UserAuthorization()
        {
            if (Session["UserRole"].ToString().ToLower() != UserRole.SuperUser.ToString().ToLower())
            {
                foreach (GridViewRow rows in gvGroupOut.Rows)
                {
                    (rows.FindControl("cbAddConfiguration") as CheckBox).Enabled = false;
                }
                foreach (GridViewRow rows in gvGroupIn.Rows)
                {
                    (rows.FindControl("cbRemoveConfiguration") as CheckBox).Enabled = false;
                }
                cbOutSelectAll.Enabled = false;
                cbInSelectAll.Enabled = false;
                btnRemoveConfiguration.Enabled = false;
                btnAddConfiguration.Enabled = false;
                btnAddConfigurationGroup.Enabled = false;
            }        
        }
        
        protected void txtConfigurationGroup_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnconfGrpSearchSelected.Value == "Y")
                {
                    LoadInitialGridData();

                    if (ValidateSelectedConfigGrpFilter())
                    {
                        string groupCode = txtConfigurationGroup.Text.Substring(0, txtConfigurationGroup.Text.IndexOf("-") - 1);

                        LoadGridData(groupCode);
                    }
                    else
                    {
                        btnAddConfiguration.Enabled = false;
                        btnRemoveConfiguration.Enabled = false;
                        txtConfigurationGroup.Text = string.Empty;
                        Session["ConfigGroupInData"] = null;
                        Session["ConfigGroupOutData"] = null;
                    }
                    
                }
                else if (hdnconfGrpSearchSelected.Value == "N")
                {
                    FuzzySearchConfigGrp();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpInGridPnlHeight.Value == string.Empty ? "300" : hdnGrpInGridPnlHeight.Value).ToString());

                }
                else if (hdnconfGrpSearchSelected.Value == string.Empty)
                {
                    LoadInitialGridData();
                    Session["ConfigGroupInData"] = null;
                    Session["ConfigGroupOutData"] = null;
                    cbOutSelectAll.Enabled = false;
                    cbInSelectAll.Enabled = false;
                }

                hdnconfGrpSearchSelected.Value = string.Empty;
                txtGroupOutBox.Text = string.Empty;
                txtGroupInBox.Text = string.Empty;
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting configuration group from textbox.", ex.Message);
            }
        }

        protected void txtGroupOutBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSelectedConfigOut())
                {
                    string selectedConfigurationCode = txtGroupOutBox.Text.Substring(0, txtGroupOutBox.Text.IndexOf("-") - 1);
                    string configurationCode;
                    CheckBox addConfiguration;

                    //Copy the out of group data to a datatable so that any change to datatable wont affect the original data
                    DataTable groupOutData = (Session["ConfigGroupOutData"] as DataTable).Copy();

                    //create a new datatable to hold the code for which checkbox is checked
                    DataTable checkedRows = new DataTable();
                    checkedRows.Columns.Add("config_code", typeof(string));

                    foreach (GridViewRow datarow in gvGroupOut.Rows)
                    {
                        addConfiguration = datarow.FindControl("cbAddConfiguration") as CheckBox;
                        configurationCode = (datarow.FindControl("lblConfigCode") as Label).Text;

                        //if the checkbox is checked add the loaction and code to checkedRows datatable
                        if (addConfiguration.Checked)
                        {
                            checkedRows.Rows.Add(configurationCode);
                        }

                        if (configurationCode == selectedConfigurationCode)
                        {

                            DataTable groupOutDataNew = groupOutData.Copy();

                            foreach (DataRow drow in groupOutDataNew.Rows)
                            {
                                //get the rows for which code is matched and move them to the top
                                if (drow["config_code"].ToString() == configurationCode)
                                {
                                    DataRow selectedRow = groupOutData.Rows[groupOutDataNew.Rows.IndexOf(drow)];
                                    DataRow newRow = groupOutData.NewRow();
                                    newRow.ItemArray = drow.ItemArray; // copy data
                                    groupOutData.Rows.Remove(selectedRow);
                                    groupOutData.Rows.InsertAt(newRow, 0);
                                    break;
                                }

                            }

                            gvGroupOut.DataSource = groupOutData;
                            if (groupOutData.Rows.Count == 0)
                            {
                                btnAddConfiguration.Enabled = false;
                                gvGroupOut.EmptyDataText = "No data found.";
                            }
                            else
                            {
                                btnAddConfiguration.Enabled = true;
                            }
                            gvGroupOut.DataBind();

                        }
                    }

                    //Re-check the checkboxes which were checked before binding the data
                    foreach (DataRow drow in checkedRows.Rows)
                    {
                        string configCode;
                        CheckBox addConfig;
                        foreach (GridViewRow row in gvGroupOut.Rows)
                        {
                            addConfig = row.FindControl("cbAddConfiguration") as CheckBox;
                            configCode = (row.FindControl("lblConfigCode") as Label).Text;

                            if (drow["config_code"].ToString() == configCode)
                            {
                                addConfig.Checked = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    txtGroupOutBox.Text = string.Empty;
                }
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting configuration from textbox.", ex.Message);
            }
        }

        protected void txtGroupInBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSelectedConfigIn())
                {
                    string selectedConfigurationCode = txtGroupInBox.Text.Substring(0, txtGroupInBox.Text.IndexOf("-") - 1);
                    string configurationCode;
                    CheckBox removeConfiguration;

                    //Copy the out of group data to a datatable so that any change to datatable wont affect the original data
                    DataTable groupInData = (Session["ConfigGroupInData"] as DataTable).Copy();

                    //create a new datatable to hold the code for which checkbox is checked
                    DataTable checkedRows = new DataTable();
                    checkedRows.Columns.Add("config_code", typeof(string));

                    foreach (GridViewRow datarow in gvGroupIn.Rows)
                    {
                        removeConfiguration = datarow.FindControl("cbRemoveConfiguration") as CheckBox;
                        configurationCode = (datarow.FindControl("lblConfigCode") as Label).Text;

                        //if the checkbox is checked add the loaction and code to checkedRows datatable
                        if (removeConfiguration.Checked)
                        {
                            checkedRows.Rows.Add(configurationCode);
                        }

                        if (configurationCode == selectedConfigurationCode)
                        {

                            DataTable groupInDataNew = groupInData.Copy();

                            foreach (DataRow drow in groupInDataNew.Rows)
                            {
                                //get the rows for which code is matched and move them to the top
                                if (drow["config_code"].ToString() == configurationCode)
                                {
                                    DataRow selectedRow = groupInData.Rows[groupInDataNew.Rows.IndexOf(drow)];
                                    DataRow newRow = groupInData.NewRow();
                                    newRow.ItemArray = drow.ItemArray; // copy data
                                    groupInData.Rows.Remove(selectedRow);
                                    groupInData.Rows.InsertAt(newRow, 0);
                                    break;
                                }

                            }

                            gvGroupIn.DataSource = groupInData;
                            if (groupInData.Rows.Count == 0)
                            {
                                btnAddConfiguration.Enabled = false;
                                gvGroupIn.EmptyDataText = "No data found.";
                            }
                            else
                            {
                                btnAddConfiguration.Enabled = true;
                            }
                            gvGroupIn.DataBind();
                        }
                    }

                    //Re-check the checkboxes which were checked before binding the data
                    foreach (DataRow drow in checkedRows.Rows)
                    {
                        string configCode;
                        CheckBox removeConfig;
                        foreach (GridViewRow row in gvGroupIn.Rows)
                        {
                            removeConfig = row.FindControl("cbRemoveConfiguration") as CheckBox;
                            configCode = (row.FindControl("lblConfigCode") as Label).Text;

                            if (drow["config_code"].ToString() == configCode)
                            {
                                removeConfig.Checked = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    txtGroupInBox.Text = string.Empty;
                }
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting configuration from textbox.", ex.Message);
            }
        }

        protected void btnRemoveConfiguration_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string selectdConfigGroupCode = txtConfigurationGroup.Text.Substring(0, txtConfigurationGroup.Text.IndexOf("-") - 1);
                string configCode;
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                List<string> configCodesToRemove = new List<string>();
                CheckBox cbRemoveConfig;

                foreach (GridViewRow gridrow in gvGroupIn.Rows)
                {
                    cbRemoveConfig = (CheckBox)gridrow.FindControl("cbRemoveConfiguration");
                    if (cbRemoveConfig.Checked)
                    {
                        configCode = (gridrow.FindControl("lblConfigCode") as Label).Text;
                        configCodesToRemove.Add(configCode);
                    }
                }

                if (configCodesToRemove.Count > 0)
                {

                    configurationGroupingBL = new ConfigurationGroupingBL();
                    DataSet configurationGroupData = configurationGroupingBL.RemoveConfigurationFromGroup(selectdConfigGroupCode, configCodesToRemove.ToArray(),userCode, out errorId);
                    configurationGroupingBL = null;

                    BindGrids(configurationGroupData);

                    txtGroupOutBox.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                }
                else
                {
                    msgView.SetMessage("Please select a configuration from 'In the Group' list",
                                        MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing configuration from group.", ex.Message);
            }
        }

        protected void btnAddConfiguration_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string selectdConfigGroupCode = txtConfigurationGroup.Text.Substring(0, txtConfigurationGroup.Text.IndexOf("-") - 1);
                string configCode;

                List<string> configCodesToAdd = new List<string>();
                CheckBox cbAddConfig;

                foreach (GridViewRow gridrow in gvGroupOut.Rows)
                {
                    cbAddConfig = (CheckBox)gridrow.FindControl("cbAddConfiguration");
                    if (cbAddConfig.Checked)
                    {
                        configCode = (gridrow.FindControl("lblConfigCode") as Label).Text;
                        configCodesToAdd.Add(configCode);
                    }
                }

                if (configCodesToAdd.Count > 0)
                {

                    configurationGroupingBL = new ConfigurationGroupingBL();
                    DataSet configurationGroupData = configurationGroupingBL.AddConfigurationToGroup(selectdConfigGroupCode, configCodesToAdd.ToArray(), userCode, out errorId);
                    configurationGroupingBL = null;

                    BindGrids(configurationGroupData);

                    txtGroupOutBox.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                }
                else
                {
                    msgView.SetMessage("Please select a configuration from 'Out of the Group' list",
                                        MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding configuration to group.", ex.Message);
            }
        }

        protected void btnAddConfigurationGroup_Click(object sender, EventArgs e)
        {
            try
            {                
                txtgroupCode.Text = string.Empty;
                txtGroupName.Text = string.Empty;
                mpeInsertGroup.Show();
                txtgroupCode.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding a new configuration group.", ex.Message);
            }
        }

        protected void btnConfigurationGroupAudit_Click(object sender, EventArgs e)
        {
            try
            {

                if (ValidateSelectedConfigGrpFilter())
                {
                    Response.Redirect(@"~/Audit/ConfigurationGroupAudit.aspx?configurationGroupCode=" + txtConfigurationGroup.Text.Split('-')[0].ToString().Trim() + "", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid configuration group from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to configuration group audit screen.", ex.Message);
            }
        }
        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                configurationGroupingBL = new ConfigurationGroupingBL();
                DataSet configurationGroupData = configurationGroupingBL.InsertConfigurationGroup(txtgroupCode.Text.Trim(), txtGroupName.Text.Trim(), userCode, out errorId);
                configurationGroupingBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("The configuration group code already exists",
                        MessageType.Warning, PositionType.Auto);
                    mpeInsertGroup.Show();
                    txtgroupCode.Focus();
                }

                else
                {
                    BindGrids(configurationGroupData);
                    txtConfigurationGroup.Text = txtgroupCode.Text.Trim() + " - " + txtGroupName.Text.Trim();
                    txtgroupCode.Text = string.Empty;
                    txtGroupName.Text = string.Empty;
                    txtGroupOutBox.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                    mpeInsertGroup.Hide();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding a new configuration group.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtConfigurationGroup.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                    txtGroupOutBox.Text = string.Empty;
                    LoadInitialGridData();
                    Session["ConfigGroupInData"] = null;
                    Session["ConfigGroupOutData"] = null;
                    cbOutSelectAll.Enabled = false;
                    cbInSelectAll.Enabled = false;
                    return;
                }

                txtConfigurationGroup.Text = lbFuzzySearch.SelectedValue.ToString();

                if (ValidateSelectedConfigGrpFilter())
                {
                    string groupCode = txtConfigurationGroup.Text.Substring(0, txtConfigurationGroup.Text.IndexOf("-") - 1);

                    LoadGridData(groupCode);
                }
                else
                {
                    btnAddConfiguration.Enabled = false;
                    btnRemoveConfiguration.Enabled = false;
                    txtConfigurationGroup.Text = string.Empty;
                    Session["ConfigGroupInData"] = null;
                    Session["ConfigGroupOutData"] = null;
                }
                txtGroupOutBox.Text = string.Empty;
                txtGroupInBox.Text = string.Empty;
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void fuzzySearchConfigGrp_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchConfigGrp();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpInGridPnlHeight.Value == string.Empty ? "300" : hdnGrpInGridPnlHeight.Value).ToString());
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in configuration grouping fuzzy search", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            txtConfigurationGroup.Text = string.Empty;
            txtGroupInBox.Text = string.Empty;
            txtGroupOutBox.Text = string.Empty;
            LoadInitialGridData();
            Session["ConfigGroupInData"] = null;
            Session["ConfigGroupOutData"] = null;
            cbOutSelectAll.Enabled = false;
            cbInSelectAll.Enabled = false;
            UserAuthorization();
        }

        protected void gvGroupOut_DataBound(object sender, EventArgs e)
        {
            try
            {
                cbOutSelectAll.Enabled = true;
                cbOutSelectAll.Checked = false;

                hdnisItemSelectedIn.Value = "N";
                hdnisItemSelectedOut.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }

        protected void gvGroupIn_DataBound(object sender, EventArgs e)
        {
            try
            {
                cbInSelectAll.Enabled = true;
                cbInSelectAll.Checked = false;

                hdnisItemSelectedIn.Value = "N";
                hdnisItemSelectedOut.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid", ex.Message);
            }
        }

        protected void btnConfigSearchOut_Click(object sender, EventArgs e)
        {
            if (Session["ConfigGroupOutData"] != null)
            {
                //Copy the out of group data to a datatable so that any change to datatable wont affect the original data
                DataTable groupOutData = (Session["ConfigGroupOutData"] as DataTable).Copy();
                DataTable dtSearched = (Session["ConfigGroupOutData"] as DataTable).Clone();

                DataRow[] foundRows = groupOutData.Select("config_code Like '%" + txtGroupOutBox.Text.Replace("'", "").Trim() + "%' or config_name Like '%" + txtGroupOutBox.Text.Replace("'", "").Trim() + "%'");
                if (foundRows.Length != 0)
                {
                    dtSearched = foundRows.CopyToDataTable();
                }

                BindOutGrid(dtSearched);
            }
            else
            {
                txtGroupOutBox.Text = string.Empty;
            }
            txtGroupOutBox.Focus();
            UserAuthorization();
        }

        protected void btnConfigSearchIn_Click(object sender, EventArgs e)
        {
            if (Session["ConfigGroupInData"] != null)
            {
                //Copy the out of group data to a datatable so that any change to datatable wont affect the original data
                DataTable groupInData = (Session["ConfigGroupInData"] as DataTable).Copy();
                DataTable dtSearched = (Session["ConfigGroupInData"] as DataTable).Clone();

                DataRow[] foundRows = groupInData.Select("config_code Like '%" + txtGroupInBox.Text.Replace("'", "").Trim() + "%' or config_name Like '%" + txtGroupInBox.Text.Replace("'", "").Trim() + "%'");
                if (foundRows.Length != 0)
                {
                    dtSearched = foundRows.CopyToDataTable();
                }

                BindInGrid(dtSearched);
            }
            else
            {
                txtGroupInBox.Text = string.Empty;
            }
            txtGroupInBox.Focus();
            UserAuthorization();
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            DataTable dtActiveInGroupConfigData = Session["ActiveInGroupConfigData"] as DataTable;
            DataTable dtActiveOutGroupConfigData = Session["ActiveOutGroupConfigData"] as DataTable;

            BindInGrid(dtActiveInGroupConfigData);
            BindOutGrid(dtActiveOutGroupConfigData);

            if (hdnButtonSelection.Value == "ConfigurationAudit")
            {
                if (ValidateSelectedConfigGrpFilter())
                {
                    Response.Redirect(@"~/Audit/ConfigurationGroupAudit.aspx?configurationGroupCode=" + txtConfigurationGroup.Text.Split('-')[0].ToString().Trim() + "", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid configuration group from list", MessageType.Warning, PositionType.Auto);
                }
            }
            else if (hdnButtonSelection.Value == "AddConfiguration")
            {
                btnAddConfigurationGroup_Click(sender, e);
            }
            else if (hdnButtonSelection.Value == "FuzzySearchConfig")
            {
                fuzzySearchConfigGrp_Click(sender, e as ImageClickEventArgs);
            }

            hdnButtonSelection.Value = string.Empty;
        }

        #endregion EVENTS

        #region METHODS

        private void LoadConfigurationGroups()
        {           

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                string configGrpCode = Request.QueryString[0];                
                //populate configuration group for the group code
                if (Session["FuzzySearchConfigGroupListTypeP"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetConfigGroupListTypeP", out errorId);
                    Session["FuzzySearchConfigGroupListTypeP"] = dsList.Tables[0];
                }

                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchConfigGroupListTypeP"]).Select("config_group_code = '" + configGrpCode + "'");
                txtConfigurationGroup.Text = filteredRow[0]["config_group"].ToString();
                LoadGridData(configGrpCode);
                lblTab.Focus();
            }
        }

        private void LoadGridData(string groupCode)
        {
            configurationGroupingBL = new ConfigurationGroupingBL();
            DataSet configurationGroupInOutData = configurationGroupingBL.GetConfigurationGroupInOutData(groupCode, out errorId);
            configurationGroupingBL = null;

            BindGrids(configurationGroupInOutData);
        }

        private void BindGrids(DataSet configurationGroupInOutData)
        {
            if (configurationGroupInOutData.Tables.Count != 0 && errorId != 2)
            {
                Session["ConfigGroupInData"] = configurationGroupInOutData.Tables[0];
                Session["ConfigGroupOutData"] = configurationGroupInOutData.Tables[1];
                Session["ActiveInGroupConfigData"] = configurationGroupInOutData.Tables[0];
                Session["ActiveOutGroupConfigData"] = configurationGroupInOutData.Tables[1];

                gvGroupIn.DataSource = configurationGroupInOutData.Tables[0];
                if (configurationGroupInOutData.Tables[0].Rows.Count == 0)
                {
                    btnRemoveConfiguration.Enabled = false;
                    gvGroupIn.EmptyDataText = "No data found.";
                }
                else
                {
                    btnRemoveConfiguration.Enabled = true;
                }
                gvGroupIn.DataBind();

                gvGroupOut.DataSource = configurationGroupInOutData.Tables[1];
                if (configurationGroupInOutData.Tables[1].Rows.Count == 0)
                {
                    btnAddConfiguration.Enabled = false;
                    gvGroupOut.EmptyDataText = "No data found.";
                }
                else
                {
                    btnAddConfiguration.Enabled = true;
                }
                gvGroupOut.DataBind();
            }
            else if (configurationGroupInOutData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvGroupIn.DataSource = dtEmpty;
                gvGroupIn.EmptyDataText = "No data found.";
                gvGroupIn.DataBind();
                gvGroupOut.DataSource = dtEmpty;
                gvGroupOut.EmptyDataText = "No data found.";
                gvGroupOut.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

            if (gvGroupIn.Rows.Count > 0)
            {
                PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
            }
            if (gvGroupOut.Rows.Count > 0)
            {
                PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
            }

        }

        private void BindOutGrid(DataTable groupOutData)
        {
            Session["ActiveOutGroupConfigData"] = groupOutData;

            if (groupOutData!=null && groupOutData.Rows.Count != 0)
            {
                gvGroupOut.DataSource = groupOutData;
                gvGroupOut.DataBind();
            }
            else if (groupOutData == null || groupOutData.Rows.Count == 0)
            {
                dtEmpty = new DataTable();
                gvGroupOut.DataSource = dtEmpty;
                gvGroupOut.EmptyDataText = "No data found.";
                gvGroupOut.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

            if (groupOutData != null && gvGroupOut.Rows.Count > 0)
            {
                PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
            }

        }

        private void BindInGrid(DataTable groupInData)
        {
            Session["ActiveInGroupConfigData"] = groupInData;

            if (groupInData!=null && groupInData.Rows.Count != 0)
            {
                gvGroupIn.DataSource = groupInData;
                gvGroupIn.DataBind();
            }
            else if (groupInData == null || groupInData.Rows.Count == 0)
            {
                dtEmpty = new DataTable();
                gvGroupIn.DataSource = dtEmpty;
                gvGroupIn.EmptyDataText = "No data found.";
                gvGroupIn.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

            if (groupInData != null && gvGroupIn.Rows.Count > 0)
            {
                PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
            }

        }
        
        private void LoadInitialGridData()
        {
            dtEmpty = new DataTable();
            gvGroupIn.DataSource = dtEmpty;
            gvGroupIn.DataBind();
            gvGroupOut.DataSource = dtEmpty;
            gvGroupOut.DataBind();
        }

        private bool ValidateSelectedConfigGrpFilter()
        {
            if (txtConfigurationGroup.Text != "" && Session["FuzzySearchConfigGroupListTypeP"] != null)
            {
                if (txtConfigurationGroup.Text != "No results found")
                {
                    DataTable dtConfigurationGroupList;
                    dtConfigurationGroupList = Session["FuzzySearchConfigGroupListTypeP"] as DataTable;

                    foreach (DataRow dRow in dtConfigurationGroupList.Rows)
                    {
                        if (dRow["config_group"].ToString() == txtConfigurationGroup.Text)
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

        private bool ValidateSelectedConfigOut()
        {
            if (txtGroupOutBox.Text != "" && Session["ConfigGroupOutData"] != null)
            {
                if (txtGroupOutBox.Text != "No results found")
                {
                    DataTable dtConfigOutList;
                    dtConfigOutList = Session["ConfigGroupOutData"] as DataTable;

                    foreach (DataRow dRow in dtConfigOutList.Rows)
                    {
                        if (dRow["config_out_data"].ToString() == txtGroupOutBox.Text)
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

        private bool ValidateSelectedConfigIn()
        {
            if (txtGroupInBox.Text != "" && Session["ConfigGroupInData"] != null)
            {
                if (txtGroupInBox.Text != "No results found")
                {
                    DataTable dtConfigInList;
                    dtConfigInList = Session["ConfigGroupInData"] as DataTable;

                    foreach (DataRow dRow in dtConfigInList.Rows)
                    {
                        if (dRow["config_in_data"].ToString() == txtGroupInBox.Text)
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

        private void FuzzySearchConfigGrp()
        {            
            if (txtConfigurationGroup.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in configuration group filter field", MessageType.Warning, PositionType.Auto);
                LoadInitialGridData();
                return;
            }

            List<string> lstConfigGroups = (new WARS.Services.FuzzySearch()).FuzzySearchConfigGroupListTypeP(txtConfigurationGroup.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = lstConfigGroups;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
            UserAuthorization();
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);               
        }

        #endregion METHODS

        #region Auto complete textbox


        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod()]
        public static List<string> SearchGroupOutBox(string prefixText, int count)
        {

            List<string> lstGroupOutData = new List<string>();
            try
            {

                DataTable dtGroupOutdata;
                if (HttpContext.Current.Session["ConfigGroupOutData"] != null)
                {
                    dtGroupOutdata = (DataTable)HttpContext.Current.Session["ConfigGroupOutData"];

                    if (dtGroupOutdata.Rows.Count > 0)
                    {
                        var results = dtGroupOutdata.AsEnumerable().Where(dr => dr.Field<string>("config_out_data").ToUpper().Contains(prefixText.ToUpper()));

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                lstGroupOutData.Add(Convert.ToString(dr["config_out_data"]));
                            }
                        }
                        else
                        {
                            lstGroupOutData.Add("No results found");
                        }

                    }
                    else
                    {
                        lstGroupOutData.Add("No results found");
                    }
                }
                else
                {
                    lstGroupOutData.Add("No results found");
                }

            }
            catch (Exception ex)
            {

            }
            return lstGroupOutData;
        }

        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod()]
        public static List<string> SearchGroupInBox(string prefixText, int count)
        {

            List<string> lstGroupInData = new List<string>();
            try
            {

                DataTable dtGroupIndata;
                if (HttpContext.Current.Session["ConfigGroupInData"] != null)
                {
                    dtGroupIndata = (DataTable)HttpContext.Current.Session["ConfigGroupInData"];

                    if (dtGroupIndata.Rows.Count > 0)
                    {
                        var results = dtGroupIndata.AsEnumerable().Where(dr => dr.Field<string>("config_in_data").ToUpper().Contains(prefixText.ToUpper()));

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                lstGroupInData.Add(Convert.ToString(dr["config_in_data"]));
                            }
                        }
                        else
                        {
                            lstGroupInData.Add("No results found");
                        }

                    }
                    else
                    {
                        lstGroupInData.Add("No results found");
                    }
                }
                else
                {
                    lstGroupInData.Add("No results found");
                }

            }
            catch (Exception ex)
            {

            }
            return lstGroupInData;
        }

        #endregion Auto complete textbox         

        #region Sorting grid data

        #region Enums for Sorting
        private enum GridSortDirections
        {
            Ascending = 0,
            Descending = 1
        }

        private enum GridSortColumns
        {
            None = 0,
            ConfigCode = 1,
            ConfigDesc = 2
        }

        #endregion Enums for Sorting

        #region properties for Sorting
        /// <summary>
        /// Property to hold sort direction of not selected grid
        /// </summary>
        private string GridSortDirectionOutGroup
        {
            get
            {
                if (hdnGridSortDirOut.Value == string.Empty)
                    hdnGridSortDirOut.Value = GridSortDirections.Ascending.ToString();
                return hdnGridSortDirOut.Value.ToString();
            }
            set { hdnGridSortDirOut.Value = value; }
        }

        /// <summary>
        /// Property to hold sort column of not selected grid
        /// </summary>
        private string GridSortColumnOutGroup
        {
            get
            {
                if (hdnGridSortColumnOut.Value == string.Empty)
                    hdnGridSortColumnOut.Value = GridSortColumns.None.ToString();
                return hdnGridSortColumnOut.Value;
            }
            set { hdnGridSortColumnOut.Value = value; }
        }

        /// <summary>
        /// Property to hold sort direction of IN Box grid
        /// </summary>
        private string GridSortDirectionInGroup
        {
            get
            {
                if (hdnGridSortDirIn.Value == string.Empty)
                    hdnGridSortDirIn.Value = GridSortDirections.Ascending.ToString();
                return hdnGridSortDirIn.Value.ToString();
            }
            set { hdnGridSortDirIn.Value = value; }
        }

        /// <summary>
        /// Property to hold sort column of In Box grid
        /// </summary>
        private string GridSortColumnInGroup
        {
            get
            {
                if (hdnGridSortColumnIn.Value == string.Empty)
                    hdnGridSortColumnIn.Value = GridSortColumns.None.ToString();
                return hdnGridSortColumnIn.Value;
            }
            set { hdnGridSortColumnIn.Value = value; }
        }

        #endregion properties for Sorting



        //protected void btnTerritoryLocSort_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Session["ActiveInGroupConfigData"] == null)
        //            return;

        //        DataTable dtActiveTransStatusData = (DataTable)Session["ActiveInGroupConfigData"];

        //        if (GridSortColumn == GridSortColumns.None.ToString())
        //            GridSortDirection = GridSortDirections.Descending.ToString();
        //        else if (GridSortColumn == GridSortColumns.TerritoryLocation.ToString())
        //            GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
        //        else
        //            GridSortDirection = GridSortDirections.Ascending.ToString();

        //        GridSortColumn = GridSortColumns.TerritoryLocation.ToString();
        //        dtActiveTransStatusData.DefaultView.Sort = "seller_location" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
        //        gvGroupIn.DataSource = dtActiveTransStatusData;
        //        gvGroupIn.DataBind();

        //        if (gvGroupIn.Rows.Count > 0)
        //        {
        //            PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler("Error in sorting grid data", string.Empty);
        //    }
        //}

        protected void btnOutCodeSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveOutGroupConfigData"] == null)
                    return;

                DataTable dtActiveConfigData = (DataTable)Session["ActiveOutGroupConfigData"];

                if (GridSortColumnOutGroup == GridSortColumns.None.ToString())
                    GridSortDirectionOutGroup = GridSortDirections.Descending.ToString();
                else if (GridSortColumnOutGroup == GridSortColumns.ConfigCode.ToString())
                    GridSortDirectionOutGroup = GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionOutGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnOutGroup = GridSortColumns.ConfigCode.ToString();
                dtActiveConfigData.DefaultView.Sort = "config_code" + (GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupOut.DataSource = dtActiveConfigData;
                gvGroupOut.DataBind();

                if (gvGroupOut.Rows.Count > 0)
                {
                    PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnOutDescSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveOutGroupConfigData"] == null)
                    return;

                DataTable dtActiveConfigData = (DataTable)Session["ActiveOutGroupConfigData"];

                if (GridSortColumnOutGroup == GridSortColumns.None.ToString())
                    GridSortDirectionOutGroup = GridSortDirections.Ascending.ToString();
                else if (GridSortColumnOutGroup == GridSortColumns.ConfigDesc.ToString())
                    GridSortDirectionOutGroup = GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionOutGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnOutGroup = GridSortColumns.ConfigDesc.ToString();
                dtActiveConfigData.DefaultView.Sort = "config_name" + (GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupOut.DataSource = dtActiveConfigData;
                gvGroupOut.DataBind();

                if (gvGroupOut.Rows.Count > 0)
                {
                    PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnInCodeSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveInGroupConfigData"] == null)
                    return;

                DataTable dtActiveConfigData = (DataTable)Session["ActiveInGroupConfigData"];

                if (GridSortColumnInGroup == GridSortColumns.None.ToString())
                    GridSortDirectionInGroup = GridSortDirections.Descending.ToString();
                else if (GridSortColumnInGroup == GridSortColumns.ConfigCode.ToString())
                    GridSortDirectionInGroup = GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionInGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnInGroup = GridSortColumns.ConfigCode.ToString();
                dtActiveConfigData.DefaultView.Sort = "config_code" + (GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupIn.DataSource = dtActiveConfigData;
                gvGroupIn.DataBind();

                if (gvGroupIn.Rows.Count > 0)
                {
                    PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnInDescSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveInGroupConfigData"] == null)
                    return;

                DataTable dtActiveConfigData = (DataTable)Session["ActiveInGroupConfigData"];

                if (GridSortColumnInGroup == GridSortColumns.None.ToString())
                    GridSortDirectionInGroup = GridSortDirections.Ascending.ToString();
                else if (GridSortColumnInGroup == GridSortColumns.ConfigDesc.ToString())
                    GridSortDirectionInGroup = GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionInGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnInGroup = GridSortColumns.ConfigDesc.ToString();
                dtActiveConfigData.DefaultView.Sort = "config_name" + (GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupIn.DataSource = dtActiveConfigData;
                gvGroupIn.DataBind();

                if (gvGroupIn.Rows.Count > 0)
                {
                    PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        #endregion Sorting grid data              
        
                              
    }
}