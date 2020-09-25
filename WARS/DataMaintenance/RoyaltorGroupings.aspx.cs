/*
File Name   :   RoyaltorGroupings.cs
Purpose     :   to maintain royaltor groupings based on group ID

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     18-Aug-2016     Pratik(Infosys Limited)   Initial Creation
2.0     07-Feb-2017     Pratik(Infosys Limited)   Added new fuzzy search functionality
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
using System.Configuration;

namespace WARS
{
    public partial class RoyaltorGroupings : System.Web.UI.Page
    {
        #region Global Declarations
        string loggedUserID;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        RoyaltorGroupingsBL royaltorGroupingBL;
        Int32 royaltorId;
        string royaltorName;
        Int32 groupId;
        string groupName;
        Int32 groupTypeCode;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["RoyaltorGroupingDefaultPageSize"].ToString());                
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Gouping";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Gouping";
                }

                txtRoyaltorGroupType.Focus();//tabbing sequence starts here
                //ddlRoyaltorGroupType.Focus();                

                if (!IsPostBack)
                {
                    //clear sessions
                    Session["royaltorGroupingData"] = null;
                    Session["FuzzyRoyGrpMaintRoyaltorList"] = null;
                    Session["FuzzyRoyGrpMaintGroupOutBoxList"] = null;
                    Session["groupOutDataPaging"] = null;
                    Session["FuzzyRoyGrpMaintGroupInBoxList"] = null;
                    Session["selectdRoyaltorId"] = null;
                    tdSummStmts.Visible = false;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadDropdowns();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                    btnAddRoyaltor.Enabled = false;
                    btnRemoveRoyaltor.Enabled = false;
                }
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void ddlRoyaltorGroupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet royaltorGroupingData = Session["royaltorGroupingData"] as DataSet;
                if (ddlRoyaltorGroupType.SelectedIndex > 0)
                {
                    //loading the group list according to group type
                    Session["FuzzyRoyGrpMaintRoyaltorList"] = royaltorGroupingData.Tables[Convert.ToInt32(ddlRoyaltorGroupType.SelectedValue)];
                    if (Convert.ToInt32(ddlRoyaltorGroupType.SelectedValue) == 2)
                    {
                        tdSummStmts.Visible = true;
                    }
                    else
                    {
                        tdSummStmts.Visible = false;
                    }
                }
                else
                {
                    Session["FuzzyRoyGrpMaintRoyaltorList"] = null;

                }
                if (string.IsNullOrWhiteSpace(txtRoyaltorGroupType.Text) == false)
                {
                    LoadInitialGridData();
                }

                txtRoyaltorGroupType.Text = string.Empty;
                txtGroupOutBox.Text = string.Empty;
                txtGroupInBox.Text = string.Empty;
                btnAddRoyaltor.Enabled = false;
                btnRemoveRoyaltor.Enabled = false;
                gvGroupIn.EmptyDataText = string.Empty;
                gvGroupOut.EmptyDataText = string.Empty;
                Session["FuzzyRoyGrpMaintGroupOutBoxList"] = null;
                Session["FuzzyRoyGrpMaintGroupInBoxList"] = null;
                Session["groupOutDataPaging"] = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor grouping type dropdown.", ex.Message);
            }
        }

        protected void txtRoyaltorGroupType_TextChanged(object sender, EventArgs e)
        {
            try
            {
                LoadInitialGridData();                
               
                if (hdnSearchListItemSelected.Value == "Y")
                {
                    RoyaltorGroupSelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchRoyaltorGroup();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpOutGridPnlHeight.Value == string.Empty ? "300" : hdnGrpOutGridPnlHeight.Value).ToString());
                }
                else
                {
                    btnAddRoyaltor.Enabled = false;
                    btnRemoveRoyaltor.Enabled = false;
                    Session["FuzzyRoyGrpMaintGroupOutBoxList"] = null;
                    Session["FuzzyRoyGrpMaintGroupInBoxList"] = null;
                    Session["groupOutDataPaging"] = null;
                }
                hdnSearchListItemSelected.Value = string.Empty;
                txtGroupOutBox.Text = string.Empty;
                txtGroupInBox.Text = string.Empty;
                Session["selectdRoyaltorId"] = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor group from textbox.", ex.Message);
            }
        }
        
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["groupOutDataPaging"] == null)
                    return;

                DataTable dtGroupOut = Session["groupOutDataPaging"] as DataTable;
                if (dtGroupOut.Rows.Count == 0)
                    return;
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);               
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtGroupOut, gridDefaultPageSize, gvGroupOut, dtEmpty, rptPager);
                //clear the selected row on page change
                UserAuthorization();
                txtGroupOutBox.Text = string.Empty;

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }
        
        protected void gvGroupIn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Disabling checkbox where royaltor Id is same as group Id 
                    groupId = Convert.ToInt32(txtRoyaltorGroupType.Text.Substring(0, txtRoyaltorGroupType.Text.IndexOf("-")));
                    royaltorId = Convert.ToInt32((e.Row.FindControl("lblRoyaltorId") as Label).Text);
                    if (groupId == royaltorId)
                    {
                        (e.Row.FindControl("cbRemoveRoyaltors") as CheckBox).Enabled = false;
                        (e.Row.FindControl("cbRemoveRoyaltors") as CheckBox).ToolTip = "Master royaltor cannot be removed from the group";
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
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }
        }

        protected void gvGroupOut_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void gvGroupOut_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyGroupingOutData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvGroupOut.DataSource = dataView;
                gvGroupOut.DataBind();
                Session["groupOutDataPaging"] = dataTable; 
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvGroupIn_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyGroupingInData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvGroupIn.DataSource = dataView;
                gvGroupIn.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End
        protected void txtGroupOutBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
               
                if (hdnSearchListItemSelected.Value == "Y")
                {
                    RoyaltorOutBoxSelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchRoyaltorOut();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpOutGridPnlHeight.Value == string.Empty ? "300" : hdnGrpOutGridPnlHeight.Value).ToString());
                }
                else
                {
                    txtGroupOutBox.Text = string.Empty;
                    gvGroupOut.SelectedIndex = -1;
                }
                hdnSearchListItemSelected.Value = string.Empty;
            }
            catch(Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from out box textbox.", ex.Message);
            }
        }

        protected void txtGroupInBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                
                if (hdnSearchListItemSelected.Value == "Y")
                {
                    RoyaltorInBoxSelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchRoyaltorIn();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpOutGridPnlHeight.Value == string.Empty ? "300" : hdnGrpOutGridPnlHeight.Value).ToString());
                }
                else
                {
                    txtGroupInBox.Text = string.Empty;
                    gvGroupIn.SelectedIndex = -1;
                }
                hdnSearchListItemSelected.Value = string.Empty;
            }
            catch(Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from textbox.", ex.Message);
            }
        }

        protected void btnRemoveRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                int selectdRoyaltorId;
                royaltorGroupingBL = new RoyaltorGroupingsBL();
                groupTypeCode = Convert.ToInt32(ddlRoyaltorGroupType.SelectedValue);
                groupId = Convert.ToInt32(txtRoyaltorGroupType.Text.Substring(0, txtRoyaltorGroupType.Text.IndexOf("-")));
                
                if (Session["selectdRoyaltorId"] == null)
                {
                    selectdRoyaltorId = Convert.ToInt32(txtRoyaltorGroupType.Text.Substring(0, txtRoyaltorGroupType.Text.IndexOf("-")));
                }
                else
                {                    
                    selectdRoyaltorId = Convert.ToInt32(Session["selectdRoyaltorId"]);
                }
                 

                List<Int32> royaltorIds = new List<Int32>();                
                CheckBox cbAddRoyaltors;

                foreach (GridViewRow gridrow in gvGroupIn.Rows)
                {
                    cbAddRoyaltors = (CheckBox)gridrow.FindControl("cbRemoveRoyaltors");
                    if (cbAddRoyaltors.Checked)
                    {
                        royaltorId = Convert.ToInt32((gridrow.FindControl("lblRoyaltorId") as Label).Text);
                        royaltorName = (gridrow.FindControl("lblRolaytorName") as Label).Text;
                        royaltorIds.Add(royaltorId);                        
                    }
                }

                if (royaltorIds.Count > 0)
                {
                    DataSet royaltorGroupingData = royaltorGroupingBL.RemoveRoyaltorFromGroup(groupTypeCode, groupId, selectdRoyaltorId, royaltorIds.ToArray(), userCode, out errorId);
                    royaltorGroupingBL = null;
                    
                    BindGrids(royaltorGroupingData);
                    UpdateSearchList(royaltorGroupingData);

                    Session["FuzzyRoyGrpMaintGroupInBoxList"] = royaltorGroupingData.Tables[0];
                    Session["FuzzyRoyGrpMaintGroupOutBoxList"] = royaltorGroupingData.Tables[7];//Harish 05-01-18 reverted it back to 7 from 1 as it was correct(as discussed with Janet)
                    Session["groupOutDataPaging"] = royaltorGroupingData.Tables[1];//Harish 05-01-2018 - WUIN-380 - paging changes

                    txtGroupOutBox.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                }
                else
                {
                    ScrollInGrid();
                    msgView.SetMessage("Please select a royaltor from 'In the Group' list",
                                        MessageType.Warning, PositionType.Auto);
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler("Error in removing royaltor from group", ex.Message);
            }
        }
        
        protected void btnAddRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                int selectdRoyaltorId;                
                royaltorGroupingBL = new RoyaltorGroupingsBL();
                groupTypeCode = Convert.ToInt32(ddlRoyaltorGroupType.SelectedValue);
                groupId = Convert.ToInt32(txtRoyaltorGroupType.Text.Substring(0, txtRoyaltorGroupType.Text.IndexOf("-")));
                
                if (Session["selectdRoyaltorId"] == null)
                {
                    selectdRoyaltorId = Convert.ToInt32(txtRoyaltorGroupType.Text.Substring(0, txtRoyaltorGroupType.Text.IndexOf("-")));
                }
                else
                {                    
                    selectdRoyaltorId = Convert.ToInt32(Session["selectdRoyaltorId"]);
                }

                List<Int32> royaltorIds = new List<Int32>();
                CheckBox cbAddRoyaltors;

                foreach (GridViewRow gridrow in gvGroupOut.Rows)
                {
                    cbAddRoyaltors = (CheckBox)gridrow.FindControl("cbAddRoyaltors");
                    if (cbAddRoyaltors.Checked)
                    {
                        royaltorId = Convert.ToInt32((gridrow.FindControl("lblRoyaltorId") as Label).Text);
                        royaltorIds.Add(royaltorId);
                    }
                }

                if (royaltorIds.Count > 0)
                {
                    DataSet royaltorGroupingData = royaltorGroupingBL.AddRoyaltorToGroup(groupTypeCode, groupId, selectdRoyaltorId, royaltorIds.ToArray(), userCode, out errorId);
                    royaltorGroupingBL = null;
                    
                    BindGrids(royaltorGroupingData);
                    UpdateSearchList(royaltorGroupingData);

                    Session["FuzzyRoyGrpMaintGroupInBoxList"] = royaltorGroupingData.Tables[0];
                    Session["FuzzyRoyGrpMaintGroupOutBoxList"] = royaltorGroupingData.Tables[7];//Harish 05-01-2018 - reverted it back to 2 as it was correct(as discussed with Janet) 
                    Session["groupOutDataPaging"] = royaltorGroupingData.Tables[1];//Harish 05-01-2018 - WUIN-380 - paging changes

                    txtGroupOutBox.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                }
                else
                {
                    ScrollOutGrid();
                    msgView.SetMessage("Please select a royaltor from 'Out of the Group' list",
                                        MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding royaltor to group", ex.Message);
            }
        }
         
        protected void btnGenSummForGroup_Click(object sender, EventArgs e)
        {
            try
            {
                //validation - check if in group grid has rows
                if (gvGroupIn.Rows.Count == 0)
                {
                    msgView.SetMessage("No Royaltors in the group to generate the Summary!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                groupId = Convert.ToInt32(txtRoyaltorGroupType.Text.Substring(0, txtRoyaltorGroupType.Text.IndexOf("-")));
                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                royaltorGroupingBL = new RoyaltorGroupingsBL();
                royaltorGroupingBL.GenerateGroupSummaries(loggedUserID, groupId, out errorId);
                royaltorGroupingBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("Summary cannot be generated. One or more Royaltors do not have the correct status.", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in setting flag to generate group summaries", string.Empty);
                }
                else
                {
                    msgView.SetMessage("Summary request is in progress. The reports will be available when generated.", MessageType.Warning, PositionType.Auto);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in setting flag to generate group summaries", ex.Message);
            }
        }

        protected void btnGenAllSummaries_Click(object sender, EventArgs e)
        {
            try
            {
                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                royaltorGroupingBL = new RoyaltorGroupingsBL();
                royaltorGroupingBL.GenerateAllGroupSummaries(loggedUserID, out errorId);
                royaltorGroupingBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("One or more Summaries cannot be generated. Summary request is in progress for the rest. The reports will be available when generated.", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in setting flag to generate all summaries", string.Empty);
                }
                else
                {
                    msgView.SetMessage("Summary request is in progress. The reports will be available when generated.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in setting flag to generate all summaries", ex.Message);
            }
        }

        protected void fuzzySearchRoyaltorGroup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltorGroup();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpOutGridPnlHeight.Value == string.Empty ? "300" : hdnGrpOutGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor group fuzzy search", ex.Message);
            }
        }

        protected void fuzzySearchRoyaltorOut_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltorOut();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpOutGridPnlHeight.Value == string.Empty ? "300" : hdnGrpOutGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void fuzzySearchRoyaltorIn_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltorIn();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpOutGridPnlHeight.Value == string.Empty ? "300" : hdnGrpOutGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "RoyaltorGroup")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtRoyaltorGroupType.Text = string.Empty;
                        btnAddRoyaltor.Enabled = false;
                        btnRemoveRoyaltor.Enabled = false;
                        Session["FuzzyRoyGrpMaintGroupOutBoxList"] = null;
                        Session["FuzzyRoyGrpMaintGroupInBoxList"] = null;
                        Session["groupOutDataPaging"] = null;
                        return;
                    }

                    txtRoyaltorGroupType.Text = lbFuzzySearch.SelectedValue.ToString();
                    RoyaltorGroupSelected();
                }
                else if (hdnFuzzySearchField.Value == "RoyaltorOut")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        gvGroupOut.SelectedIndex = -1;
                        txtGroupOutBox.Text = string.Empty;
                        return;
                    }

                    txtGroupOutBox.Text = lbFuzzySearch.SelectedValue.ToString();
                    RoyaltorOutBoxSelected();
                }
                else if (hdnFuzzySearchField.Value == "RoyaltorIn")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        gvGroupIn.SelectedIndex = -1;
                        txtGroupInBox.Text = string.Empty;
                        return;
                    }

                    txtGroupInBox.Text = lbFuzzySearch.SelectedValue.ToString();
                    RoyaltorInBoxSelected();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "RoyaltorGroup")
                {
                    LoadInitialGridData();
                    txtRoyaltorGroupType.Text = string.Empty;
                    btnAddRoyaltor.Enabled = false;
                    btnRemoveRoyaltor.Enabled = false;
                    Session["FuzzyRoyGrpMaintGroupOutBoxList"] = null;
                    Session["FuzzyRoyGrpMaintGroupInBoxList"] = null;
                    Session["groupOutDataPaging"] = null;

                }
                else if (hdnFuzzySearchField.Value == "RoyaltorOut")
                {
                    txtGroupOutBox.Text = string.Empty;
                    gvGroupOut.SelectedIndex = -1;
                }
                else if (hdnFuzzySearchField.Value == "RoyaltorIn")
                {
                    txtGroupInBox.Text = string.Empty;
                    gvGroupIn.SelectedIndex = -1;
                }
                mpeFuzzySearch.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing fuzzy search pop", ex.Message);
            }

        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "GroupType")
            {
                ddlRoyaltorGroupType_SelectedIndexChanged(sender, e);
            }
            if (hdnButtonSelection.Value == "RoyaltorGroup" || hdnButtonSelection.Value == "fuzzySearchRoyaltorGroup")
            {
                hdnSearchListItemSelected.Value = "Y";
                txtRoyaltorGroupType.Text = string.Empty;
                txtRoyaltorGroupType_TextChanged(sender, e);
            }

        } 

        #endregion EVENTS

        #region METHODS

        private void LoadDropdowns()
        {
            royaltorGroupingBL = new RoyaltorGroupingsBL();
            DataSet royaltorGroupingData = royaltorGroupingBL.GetRoyaltorGroupings(out errorId);
            royaltorGroupingBL = null;
            if (royaltorGroupingData.Tables.Count != 0 && errorId != 2)
            {
                ddlRoyaltorGroupType.DataTextField = "ROYALTOR_GROUP_TYPE_DESC";
                ddlRoyaltorGroupType.DataValueField = "ROYALTOR_GROUP_TYPE_CODE";
                ddlRoyaltorGroupType.DataSource = royaltorGroupingData.Tables[0];
                ddlRoyaltorGroupType.DataBind();
                ddlRoyaltorGroupType.Items.Insert(0, new ListItem("-"));                
                Session["royaltorGroupingData"] = royaltorGroupingData;
            }
            else if (royaltorGroupingData.Tables.Count == 0 && errorId != 2)
            {
                ddlRoyaltorGroupType.Items.Insert(0, new ListItem("-"));
            }
            else
            {
                ExceptionHandler("Error in loading grouping type dropdown.", string.Empty);
            }

            LoadInitialGridData();
        }

        private void LoadGridData(Int32 groupTypeCode, Int32 groupId)
        {
            royaltorGroupingBL = new RoyaltorGroupingsBL();
            DataSet royaltorGroupingData = royaltorGroupingBL.GetRoyaltorGroupingsInOutData(groupTypeCode, groupId, out errorId);
            royaltorGroupingBL = null;
            hdnPageNumber.Value = "1";
            BindGrids(royaltorGroupingData);
            Session["FuzzyRoyGrpMaintGroupInBoxList"] = royaltorGroupingData.Tables[0];
            Session["FuzzyRoyGrpMaintGroupOutBoxList"] = royaltorGroupingData.Tables[2];//Harish 05-01-2018 - reverted it back to 2 as it was correct(as discussed with Janet)
            Session["groupOutDataPaging"] = royaltorGroupingData.Tables[1];//Harish 05-01-2018 - WUIN-380 - paging changes
        }

        private void LoadInitialGridData()
        {
            hdnPageNumber.Value = "1";
            dtEmpty = new DataTable();
            gvGroupIn.DataSource = dtEmpty;
            gvGroupIn.DataBind();
            gvGroupOut.DataSource = dtEmpty;
            gvGroupOut.DataBind();
            Utilities.PopulateGridPage(1, dtEmpty, gridDefaultPageSize, gvGroupOut, dtEmpty, rptPager);
        }

        private bool ValidateSelectedRoyaltorGrpFilter()
        {
            if (txtRoyaltorGroupType.Text != "" && Session["FuzzyRoyGrpMaintRoyaltorList"] != null)
            {
                if (txtRoyaltorGroupType.Text != "No results found")
                {
                    DataTable dtRoyaltorGroupings;
                    dtRoyaltorGroupings = Session["FuzzyRoyGrpMaintRoyaltorList"] as DataTable;

                    foreach (DataRow dRow in dtRoyaltorGroupings.Rows)
                    {
                        if (dRow["grp_name"].ToString() == txtRoyaltorGroupType.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    txtRoyaltorGroupType.Text = string.Empty;
                    return false;
                }
            }
            else
            {
                txtRoyaltorGroupType.Text = string.Empty;
                return false;
            }
        }

        private bool ValidateSelectedRoyaltorOut()
        {
            if (txtGroupOutBox.Text != "" && Session["FuzzyRoyGrpMaintGroupOutBoxList"] != null)
            {
                if (txtGroupOutBox.Text != "No results found")
                {
                    DataTable dtRoyaltorGroupOutdata;
                    dtRoyaltorGroupOutdata = Session["FuzzyRoyGrpMaintGroupOutBoxList"] as DataTable;

                    foreach (DataRow dRow in dtRoyaltorGroupOutdata.Rows)
                    {
                        if (dRow["royaltor_value"].ToString() == txtGroupOutBox.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    txtGroupOutBox.Text = string.Empty;
                    return false;
                }
            }
            else
            {
                txtGroupOutBox.Text = string.Empty;
                return false;
            }
        }

        private bool ValidateSelectedRoyaltorIn()
        {
            if (txtGroupInBox.Text != "" && Session["FuzzyRoyGrpMaintGroupInBoxList"] != null)
            {
                if (txtGroupInBox.Text != "No results found")
                {
                    DataTable dtRoyaltorGroupIndata;
                    dtRoyaltorGroupIndata = Session["FuzzyRoyGrpMaintGroupInBoxList"] as DataTable;

                    foreach (DataRow dRow in dtRoyaltorGroupIndata.Rows)
                    {
                        if (dRow["royaltor_value"].ToString() == txtGroupInBox.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    txtGroupInBox.Text = string.Empty;
                    return false;
                }
            }
            else
            {
                txtGroupInBox.Text = string.Empty;
                return false;
            }
        }

        private void ScrollOutGrid()
        {
            //to scroll within the grid when a royaltor is selected in the textbox
            //int intScrollTo = this.gvGroupOut.SelectedIndex * (int)this.gvGroupOut.RowStyle.Height.Value;
            //int intScrollTo = this.gvGroupOut.SelectedIndex * 15;//WOS-402 - Modified by Harish 06-12-2016
            int intScrollTo = this.gvGroupOut.SelectedIndex * 22;
            string strScript = string.Empty;
            strScript += "var gvGroupOut = document.getElementById('" + this.gvGroupOut.ClientID + "');\n";
            strScript += "if (gvGroupOut != null && gvGroupOut.parentElement != null && gvGroupOut.parentElement.parentElement != null)\n";
            strScript += "  gvGroupOut.parentElement.parentElement.scrollTop = " + intScrollTo + ";\n";
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ScrollGridOut", strScript, true);
        }

        private void ScrollInGrid()
        {
            //to scroll within the grid when a royaltor is selected in the textbox
            //int intScrollTo = this.gvGroupIn.SelectedIndex * (int)this.gvGroupIn.RowStyle.Height.Value;
            int intScrollTo = this.gvGroupIn.SelectedIndex * 15;
            string strScript = string.Empty;
            strScript += "var gvGroupIn = document.getElementById('" + this.gvGroupIn.ClientID + "');\n";
            strScript += "if (gvGroupIn != null && gvGroupIn.parentElement != null && gvGroupIn.parentElement.parentElement != null)\n";
            strScript += "  gvGroupIn.parentElement.parentElement.scrollTop = " + intScrollTo + ";\n";
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ScrollGridIn", strScript, true);
        }

        private void BindGrids(DataSet royaltorGroupingData)
        {
            if (royaltorGroupingData.Tables.Count != 0 && errorId != 2)
            {
                Session["RoyGroupingInData"] = royaltorGroupingData.Tables[0];
                gvGroupIn.DataSource = royaltorGroupingData.Tables[0];
                if (royaltorGroupingData.Tables[0].Rows.Count == 0)
                {
                    gvGroupIn.EmptyDataText = "No data found for the selected royaltor group.";
                }
                gvGroupIn.DataBind();

                Session["RoyGroupingOutData"] = royaltorGroupingData.Tables[1];
                if (royaltorGroupingData.Tables[1].Rows.Count == 0)
                {
                    gvGroupOut.EmptyDataText = "No data found for the selected royaltor group.";
                }               
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), royaltorGroupingData.Tables[1], gridDefaultPageSize, gvGroupOut, dtEmpty, rptPager);
                gvGroupIn.SelectedIndex = -1;
                gvGroupOut.SelectedIndex = -1;
            }
            else if (royaltorGroupingData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvGroupIn.DataSource = dtEmpty;
                gvGroupIn.EmptyDataText = "No data found for the selected royaltor group.";
                gvGroupIn.DataBind();
                gvGroupOut.DataSource = dtEmpty;
                gvGroupOut.EmptyDataText = "No data found for the selected royaltor group.";
                gvGroupOut.DataBind();
                rptPager.Visible = false;
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

            if (gvGroupOut.Rows.Count == 0)
            {
                btnAddRoyaltor.Enabled = false;
            }
            else
            {
                btnAddRoyaltor.Enabled = true;
            }
            UserAuthorization();
        }

        private void UpdateSearchList(DataSet royaltorGroupingData)
        {
            DataSet updatedGroupingData = new DataSet();
            updatedGroupingData.Tables.Add(royaltorGroupingData.Tables[2].Copy());
            updatedGroupingData.Tables.Add(royaltorGroupingData.Tables[3].Copy());
            updatedGroupingData.Tables.Add(royaltorGroupingData.Tables[4].Copy());
            updatedGroupingData.Tables.Add(royaltorGroupingData.Tables[5].Copy());
            updatedGroupingData.Tables.Add(royaltorGroupingData.Tables[6].Copy());
            Session["royaltorGroupingData"] = updatedGroupingData;
            Session["FuzzyRoyGrpMaintRoyaltorList"] = royaltorGroupingData.Tables[Convert.ToInt32(ddlRoyaltorGroupType.SelectedValue) + 2];

        }
        
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        private void FuzzySearchRoyaltorGroup()
        {
            
            if (txtRoyaltorGroupType.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor group filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "RoyaltorGroup";

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyRoyGrpMaintRoyaltorList(txtRoyaltorGroupType.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchRoyaltorOut()
        {
            
            if (txtGroupOutBox.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "RoyaltorOut";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyRoyGrpMaintGroupOutBoxList(txtGroupOutBox.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchRoyaltorIn()
        {
            
            if (txtGroupInBox.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "RoyaltorIn";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyRoyGrpMaintGroupInBoxList(txtGroupInBox.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void RoyaltorGroupSelected()
        {
            if (ValidateSelectedRoyaltorGrpFilter())
            {
                groupTypeCode = Convert.ToInt32(ddlRoyaltorGroupType.SelectedValue);
                groupId = Convert.ToInt32(txtRoyaltorGroupType.Text.Substring(0, txtRoyaltorGroupType.Text.IndexOf("-")));
                LoadGridData(groupTypeCode, groupId);

                if (gvGroupIn.Rows.Count > 0)
                {
                    PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
                }
                if (gvGroupOut.Rows.Count > 0)
                {
                    PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
                }

                btnRemoveRoyaltor.Enabled = true;
            }
            else
            {
                btnAddRoyaltor.Enabled = false;
                btnRemoveRoyaltor.Enabled = false;
                Session["FuzzyRoyGrpMaintGroupOutBoxList"] = null;
                Session["FuzzyRoyGrpMaintGroupInBoxList"] = null;

            }
        }

        private void RoyaltorOutBoxSelected()
        {
            if (ValidateSelectedRoyaltorOut())
            {
                groupTypeCode = Convert.ToInt32(ddlRoyaltorGroupType.SelectedValue);
                groupId = Convert.ToInt32(txtRoyaltorGroupType.Text.Substring(0, txtRoyaltorGroupType.Text.IndexOf("-")));
                int selectdRoyaltorId = Convert.ToInt32(txtGroupOutBox.Text.Substring(0, txtGroupOutBox.Text.IndexOf("-")));
                Session["selectdRoyaltorId"] = selectdRoyaltorId;

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                royaltorGroupingBL = new RoyaltorGroupingsBL();
                DataSet royaltorGroupingData = royaltorGroupingBL.GetUpdatedOutData(groupTypeCode, groupId, selectdRoyaltorId, out errorId);
                royaltorGroupingBL = null;

                if (royaltorGroupingData.Tables.Count != 0 && errorId != 2)
                {
                   
                    gvGroupOut.DataSource = royaltorGroupingData.Tables[0];
                    Session["groupOutDataPaging"] = royaltorGroupingData.Tables[0];//Harish 09-01-2018 - WUIN-380 - paging changes
                    Session["RoyGroupingOutData"] = royaltorGroupingData.Tables[0];//Harish 09-01-2018 - WUIN-380 - paging changes
                    if (royaltorGroupingData.Tables[0].Rows.Count == 0)
                    {
                        gvGroupOut.EmptyDataText = "No data found for the selected royaltor group.";
                        gvGroupOut.DataBind();
                        gvGroupOut.SelectedIndex = -1;
                    }
                    else
                    {
                        //WOS-402 changes - Harish 06-12-2016 =============
                        //DataTable dt01 = royaltorGroupingData.Tables[0].Select("royaltor=" + selectdRoyaltorId).CopyToDataTable();
                        DataRow[] result = royaltorGroupingData.Tables[0].Select("royaltor_id = " + selectdRoyaltorId);
                        if (result.Length > 0)
                        {
                            //int SelectedIndex = royaltorGroupingData.Tables[0].Rows.IndexOf(result[0]);
                            //int pageIndex = Convert.ToInt32(Math.Truncate(Convert.ToDouble(SelectedIndex / 500)));
                            //gvGroupOut.PageIndex = pageIndex;
                            //gvGroupOut.DataBind();
                            //gvGroupOut.SelectedIndex = SelectedIndex - (pageIndex) * 500;
                            ScrollOutGrid();
                            ScrollInGrid();

                        }
                    }

                    //============== End
                }
                else if (royaltorGroupingData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvGroupOut.DataSource = dtEmpty;
                    gvGroupOut.EmptyDataText = "No data found for the selected royaltor group.";
                    gvGroupOut.DataBind();

                    Session["groupOutDataPaging"] = royaltorGroupingData.Tables[0];//Harish 09-01-2018 - WUIN-380 - paging changes
                                        
                }
                else
                {
                    ExceptionHandler("Error in loading grid data.", string.Empty);
                }

                if (gvGroupOut.Rows.Count == 0)
                {
                    btnAddRoyaltor.Enabled = false;
                }
                else
                {
                    btnAddRoyaltor.Enabled = true;
                }

                /*WOS-402 changes - Harish 06-12-2016 =============
                 * code before changes
                foreach (GridViewRow datarow in gvGroupOut.Rows)
                {
                    royaltorId = Convert.ToInt32((datarow.FindControl("lblRoyaltorId") as Label).Text);
                    if (royaltorId == selectdRoyaltorId)
                    {
                        gvGroupOut.SelectedIndex = datarow.RowIndex;
                        ScrollOutGrid();
                        ScrollInGrid();
                    }
                }
                 * */
                //============== End
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(1, royaltorGroupingData.Tables[0], gridDefaultPageSize, gvGroupOut, dtEmpty, rptPager);
                UserAuthorization();
            }
            else
            {
                gvGroupOut.SelectedIndex = -1;
                txtGroupOutBox.Text = string.Empty;
                //msgView.SetMessage("Please select a valid royaltor from the filter list",
                //                    MessageType.Warning, PositionType.Auto);
            }            
        }

        private void RoyaltorInBoxSelected()
        {
            if (ValidateSelectedRoyaltorIn())
            {
                int selectdRoyaltorId = Convert.ToInt32(txtGroupInBox.Text.Substring(0, txtGroupInBox.Text.IndexOf("-")));

                foreach (GridViewRow datarow in gvGroupIn.Rows)
                {
                    royaltorId = Convert.ToInt32((datarow.FindControl("lblRoyaltorId") as Label).Text);
                    if (royaltorId == selectdRoyaltorId)
                    {
                        gvGroupIn.SelectedIndex = datarow.RowIndex;
                        ScrollInGrid();
                        ScrollOutGrid();
                    }
                }

            }
            else
            {
                txtGroupInBox.Text = string.Empty;
                gvGroupIn.SelectedIndex = -1;
                //msgView.SetMessage("Please select a valid royaltor from the filter list",
                //                    MessageType.Warning, PositionType.Auto);

            }
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnGenSummForGroup.Enabled = false;
                btnGenAllSummaries.Enabled = false;
                btnRemoveRoyaltor.Enabled = false;
                btnAddRoyaltor.Enabled = false;
                foreach (GridViewRow rows in gvGroupOut.Rows)
                {
                    (rows.FindControl("cbAddRoyaltors") as CheckBox).Enabled = false;
                }
                foreach (GridViewRow rows in gvGroupIn.Rows)
                {
                    (rows.FindControl("cbRemoveRoyaltors") as CheckBox).Enabled = false;
                }

            }
        }

        #endregion METHODS        

       

       
     
            
   }
}