/*
File Name   :   SellerGroup.cs
Purpose     :   to maintain seller/territory groups based on group ID

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     03-Oct-2016     Pratik(Infosys Limited)   Initial Creation
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
    public partial class SellerGroup : System.Web.UI.Page
    {
        #region Global Declarations
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        SellerGroupBL sellerGroupBL;
        string groupCode;
        string sellerCode;
        int count;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Territory Group";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Territory Group";
                }

                PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
                PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtSellerGroup.Focus();//tabbing sequence starts here

                    //clear sessions                    
                    Session["groupOutData"] = null;
                    Session["groupInData"] = null;                    
                    Session["groupTerritoryLocationOut"] = null;
                    Session["groupTerritoryLocationIn"] = null;
                    cbOutSelectAll.Enabled = false;
                    cbInSelectAll.Enabled = false;
                    btnAddTerritory.Enabled = false;
                    btnRemoveTerritory.Enabled = false;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadSellerGroups();
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

        protected void txtSellerGroup_TextChanged(object sender, EventArgs e)
        {
            try
            {
                LoadInitialGridData();

                if (hdnSearchListItemSelected.Value == "Y")
                {
                    TerritoryGroupSelected();                    
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchTerritoryGroup();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpOutGridPnlHeight.Value == string.Empty ? "300" : hdnGrpOutGridPnlHeight.Value).ToString());
                }
                else
                {
                    Session["groupOutData"] = null;
                    Session["groupInData"] = null;                    
                    Session["groupTerritoryLocationOut"] = null;
                    Session["groupTerritoryLocationIn"] = null;
                    cbOutSelectAll.Enabled = false;
                    cbInSelectAll.Enabled = false;
                }
                txtGroupOutBox.Text = string.Empty;
                txtGroupInBox.Text = string.Empty;
                txtGroupInLoc.Text = string.Empty;
                txtGroupOutLoc.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting territory group from textbox.", ex.Message);
            }
        }
                
        protected void gvGroupOut_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string territoryLocation;
                string territoryCode;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    territoryLocation = (e.Row.FindControl("lblSellerLocation") as Label).Text;
                    territoryCode = (e.Row.FindControl("lblSellerCode") as Label).Text;
                    if (string.IsNullOrWhiteSpace(territoryCode))
                    {
                        HideUnHideToggleButtons(e.Row.Cells[0], true, false);//hide collapse button
                        e.Row.Font.Bold = true;
                        Session["groupTerritoryLocationOut"] = territoryLocation;
                    }
                    else
                    {
                        if (Session["groupTerritoryLocationOut"] != null)
                        {
                            if (territoryLocation == Session["groupTerritoryLocationOut"].ToString())
                            {
                                e.Row.Visible = false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }

        protected void gvGroupOut_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string territoryLocation;
                string territoryCode;
                int currRowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvGroupOut.Rows[currRowIndex];
                string hdrterritoryLocation = (row.FindControl("lblSellerLocation") as Label).Text;
                string hdrterritoryCode = (row.FindControl("lblSellerCode") as Label).Text;
                if (e.CommandName == "Expand")
                {
                    for (int i = currRowIndex + 1; i < gvGroupOut.Rows.Count; i++)
                    {
                        territoryLocation = (gvGroupOut.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                        territoryCode = (gvGroupOut.Rows[i].FindControl("lblSellerCode") as Label).Text;
                        if (hdrterritoryLocation == territoryLocation)
                        {
                            gvGroupOut.Rows[i].Visible = true;
                        }
                        else
                        {
                            //we have reached the end of the current group
                            //make expand image invisible and collapse image visible
                            HideUnHideToggleButtons(row.Cells[0], false, true);
                            break;
                        }

                        //if we are dealing with the last row,
                        //hide/unhide collapse/expand logic needs to be
                        //handled here
                        if (i + 1 == gvGroupOut.Rows.Count)
                            HideUnHideToggleButtons(row.Cells[0], false, true);
                    }
                }
                if (e.CommandName == "Collapse")
                {
                    for (int i = currRowIndex + 1; i < gvGroupOut.Rows.Count; i++)
                    {
                        territoryLocation = (gvGroupOut.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                        territoryCode = (gvGroupOut.Rows[i].FindControl("lblSellerCode") as Label).Text;
                        if (hdrterritoryLocation == territoryLocation)
                        {
                            gvGroupOut.Rows[i].Visible = false;
                        }
                        else
                        {
                            //we have reached the end of the current group
                            //make expand image invisible and collapse image visible
                            HideUnHideToggleButtons(row.Cells[0], true, false);
                            break;
                        }

                        //if we are dealing with the last row,
                        //hide/unhide collapse/expand logic needs to be
                        //handled here
                        if (i + 1 == gvGroupOut.Rows.Count)
                            HideUnHideToggleButtons(row.Cells[0], true, false);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in expanding/collapsing group in grid.", ex.Message);
            }
        }

        protected void gvGroupIn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string territoryLocation;
                string territoryCode;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    territoryLocation = (e.Row.FindControl("lblSellerLocation") as Label).Text;
                    territoryCode = (e.Row.FindControl("lblSellerCode") as Label).Text;
                    if (string.IsNullOrWhiteSpace(territoryCode))
                    {
                        HideUnHideToggleButtons(e.Row.Cells[0], true, false);//hide collapse button
                        e.Row.Font.Bold = true;
                        Session["groupTerritoryLocationIn"] = territoryLocation;
                    }
                    else
                    {
                        if (Session["groupTerritoryLocationIn"] != null)
                        {
                            if (territoryLocation == Session["groupTerritoryLocationIn"].ToString())
                            {
                                e.Row.Visible = false;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data to grid.", ex.Message);
            }
        }

        protected void gvGroupIn_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string territoryLocation;
                string territoryCode;
                int currRowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvGroupIn.Rows[currRowIndex];
                string hdrterritoryLocation = (row.FindControl("lblSellerLocation") as Label).Text;
                string hdrterritoryCode = (row.FindControl("lblSellerCode") as Label).Text;
                if (e.CommandName == "Expand")
                {
                    for (int i = currRowIndex + 1; i < gvGroupIn.Rows.Count; i++)
                    {
                        territoryLocation = (gvGroupIn.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                        territoryCode = (gvGroupIn.Rows[i].FindControl("lblSellerCode") as Label).Text;
                        if (hdrterritoryLocation == territoryLocation)
                        {
                            gvGroupIn.Rows[i].Visible = true;
                        }
                        else
                        {
                            //we have reached the end of the current group
                            //make expand image invisible and collapse image visible
                            HideUnHideToggleButtons(row.Cells[0], false, true);
                            break;
                        }

                        //if we are dealing with the last row,
                        //hide/unhide collapse/expand logic needs to be
                        //handled here
                        if (i + 1 == gvGroupIn.Rows.Count)
                            HideUnHideToggleButtons(row.Cells[0], false, true);
                    }
                }
                if (e.CommandName == "Collapse")
                {
                    for (int i = currRowIndex + 1; i < gvGroupIn.Rows.Count; i++)
                    {
                        territoryLocation = (gvGroupIn.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                        territoryCode = (gvGroupIn.Rows[i].FindControl("lblSellerCode") as Label).Text;
                        if (hdrterritoryLocation == territoryLocation)
                        {
                            gvGroupIn.Rows[i].Visible = false;
                        }
                        else
                        {
                            //we have reached the end of the current group
                            //make expand image invisible and collapse image visible
                            HideUnHideToggleButtons(row.Cells[0], true, false);
                            break;
                        }

                        //if we are dealing with the last row,
                        //hide/unhide collapse/expand logic needs to be
                        //handled here
                        if (i + 1 == gvGroupIn.Rows.Count)
                            HideUnHideToggleButtons(row.Cells[0], true, false);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in expanding/collapsing group in grid.", ex.Message);
            }

        }

        protected void btnRemoveTerritory_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string selectdTerritoryGorupCode = txtSellerGroup.Text.Substring(0, txtSellerGroup.Text.IndexOf("-") - 1);
                string territoryCode;
                string territoryLocation;
                sellerGroupBL = new SellerGroupBL();

                List<string> territotyCodes = new List<string>();
                CheckBox cbRemoveTerritory;

                foreach (GridViewRow gridrow in gvGroupIn.Rows)
                {
                    cbRemoveTerritory = (CheckBox)gridrow.FindControl("cbRemoveSeller");
                    if (cbRemoveTerritory.Checked)
                    {
                        territoryCode = (gridrow.FindControl("lblSellerCode") as Label).Text;
                        
                        if (!string.IsNullOrWhiteSpace(territoryCode))
                        {
                            territotyCodes.Add(territoryCode);
                        }
                    }
                }

                if (territotyCodes.Count > 0)
                {
                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

               
                    DataSet sellerGroupList = sellerGroupBL.RemoveTerritoryToGroup(selectdTerritoryGorupCode, territotyCodes.Distinct().ToArray(), userCode, out errorId);
                    sellerGroupBL = null;

                    BindGrids(sellerGroupList);

                    txtGroupOutBox.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                    txtGroupInLoc.Text = string.Empty;
                    txtGroupOutLoc.Text = string.Empty;
                }
                else
                {
                    //ScrollInGrid();
                    msgView.SetMessage("Please select a territory from 'In the Group' list",
                                        MessageType.Warning, PositionType.Auto);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing territory from group.", ex.Message);
            }
        }

        protected void btnAddTerritory_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string selectdTerritoryGorupCode = txtSellerGroup.Text.Substring(0, txtSellerGroup.Text.IndexOf("-") - 1);
                string territoryCode;
                //string territoryLocation;
                sellerGroupBL = new SellerGroupBL();

                List<string> territotyCodes = new List<string>();
                CheckBox cbAddTerritory;

                foreach (GridViewRow gridrow in gvGroupOut.Rows)
                {
                    cbAddTerritory = (CheckBox)gridrow.FindControl("cbAddSeller");
                    if (cbAddTerritory.Checked)
                    {
                        territoryCode = (gridrow.FindControl("lblSellerCode") as Label).Text;
                        //territoryLocation = (gridrow.FindControl("lblSellerLocation") as Label).Text;
                        //if (string.IsNullOrWhiteSpace(territoryCode))
                        //{
                        //    string territoryCd;
                        //    string territoryLoc;
                        //    foreach (GridViewRow row in gvGroupOut.Rows)
                        //    {
                        //        territoryCd = (row.FindControl("lblSellerCode") as Label).Text;
                        //        territoryLoc = (row.FindControl("lblSellerLocation") as Label).Text;
                        //        if (territoryLocation == territoryLoc & !string.IsNullOrWhiteSpace(territoryCd))
                        //        {
                        //            territotyCodes.Add(territoryCd);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    territotyCodes.Add(territoryCode);
                        //}

                        if (!string.IsNullOrWhiteSpace(territoryCode))
                        {
                            territotyCodes.Add(territoryCode);
                        }
                    }
                }

                if (territotyCodes.Count > 0)
                {
                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

          
                    DataSet sellerGroupList = sellerGroupBL.AddTerritoryToGroup(selectdTerritoryGorupCode, territotyCodes.Distinct().ToArray(), userCode, out errorId);
                    sellerGroupBL = null;

                    BindGrids(sellerGroupList);                

                    txtGroupOutBox.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                    txtGroupInLoc.Text = string.Empty;
                    txtGroupOutLoc.Text = string.Empty;
                }
                else
                {
                    //ScrollOutGrid();
                    msgView.SetMessage("Please select a territory from 'Out of the Group' list",
                                        MessageType.Warning, PositionType.Auto);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding territory to group.", ex.Message);
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnisItemSelectedIn.Value == "Y" || hdnisItemSelectedOut.Value == "Y")
                {
                    DataTable dtActiveInGroupData = Session["ActiveInGroupData"] as DataTable;
                    DataTable dtActiveOutGroupData = Session["ActiveOutGroupData"] as DataTable;

                    BindInGrid(dtActiveInGroupData);
                    BindOutGrid(dtActiveOutGroupData);

                    if (hdnButtonSelection.Value == "TerritoryAudit")
                    {
                        if ((ValidateSelectedSellerGrpFilter()))
                        {
                            Response.Redirect(@"~/Audit/TerritoryGroupAudit.aspx?territoryGroupCode=" + txtSellerGroup.Text.Split('-')[0].ToString().Trim() + "", false);
                        }
                        else
                        {
                            msgView.SetMessage("Please select a valid territory group from list", MessageType.Warning, PositionType.Auto);
                        }
                    }
                    else if (hdnButtonSelection.Value == "CheckRoyaltors")
                    {
                        if (ValidateSelectedSellerGrpFilter())
                        {
                            Response.Redirect(@"~/DataMaintenance/TerritoryGroupRoyaltors.aspx?terriroryGroupCode=" + txtSellerGroup.Text.Split('-')[0].ToString().Trim() + "", false);
                        }
                        else
                        {
                            msgView.SetMessage("Please select a valid territory group from list", MessageType.Warning, PositionType.Auto);
                        }
                    }
                    else if (hdnButtonSelection.Value == "AddConfiguration")
                    {
                        btnAddTerritoryGroup_Click(sender, e);
                    }
                    else if (hdnButtonSelection.Value == "FuzzySearchTerritory")
                    {
                        fuzzySearchTerritoryGroup_Click(sender, e as ImageClickEventArgs);
                    }

                    hdnButtonSelection.Value = string.Empty;
                }             
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in continue.", ex.Message);
            }
        }

        protected void btnAddTerritoryGroup_Click(object sender, EventArgs e)
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
                ExceptionHandler("Error in adding a new territory group.", ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                sellerGroupBL = new SellerGroupBL();
                DataSet sellerGroupData = sellerGroupBL.InsertTerritoryGroup(txtgroupCode.Text.Trim(), txtGroupName.Text.Trim(), userCode, out errorId);
                sellerGroupBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("The territory group code already exists",
                        MessageType.Warning, PositionType.Auto);
                    mpeInsertGroup.Show();
                    txtgroupCode.Focus();
                }

                else
                {
                    BindGrids(sellerGroupData);
                    txtSellerGroup.Text = txtgroupCode.Text.Trim() + " - " + txtGroupName.Text.Trim();
                    txtgroupCode.Text = string.Empty;
                    txtGroupName.Text = string.Empty;
                    txtGroupOutBox.Text = string.Empty;
                    txtGroupOutLoc.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                    txtGroupInLoc.Text = string.Empty;
                    mpeInsertGroup.Hide();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding a new territory group.", ex.Message);
            }
        }

        protected void cbRemoveSeller_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
                int currRowIndex = row.RowIndex;

                CheckBox hrdRemoveTerritories;
                string hdrTerritoryLocation = (gvGroupIn.Rows[currRowIndex].FindControl("lblSellerLocation") as Label).Text;
                string hdrTerritoryCode = (gvGroupIn.Rows[currRowIndex].FindControl("lblSellerCode") as Label).Text;
                hrdRemoveTerritories = (CheckBox)gvGroupIn.Rows[currRowIndex].FindControl("cbRemoveSeller");

                CheckBox removeTerritories;
                string territoryLocation;
                string territoryCode;

                if (string.IsNullOrWhiteSpace(hdrTerritoryCode))
                {
                    if (hrdRemoveTerritories.Checked)
                    {

                        for (int i = currRowIndex + 1; i < gvGroupIn.Rows.Count; i++)
                        {
                            territoryLocation = (gvGroupIn.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                            territoryCode = (gvGroupIn.Rows[i].FindControl("lblSellerCode") as Label).Text;
                            removeTerritories = (CheckBox)gvGroupIn.Rows[i].FindControl("cbRemoveSeller");

                            if (hdrTerritoryLocation == territoryLocation & !string.IsNullOrWhiteSpace(territoryCode))
                            {
                                gvGroupIn.Rows[i].Visible = true;
                                removeTerritories.Checked = true;
                            }
                            else
                            {
                                //we have reached the end of the current group
                                //make expand image invisible and collapse image visible
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                                break;
                            }

                            //if we are dealing with the last row,
                            //hide/unhide collapse/expand logic needs to be
                            //handled here
                            if (i + 1 == gvGroupIn.Rows.Count)
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                        }
                    }
                    else
                    {
                        for (int i = currRowIndex + 1; i < gvGroupIn.Rows.Count; i++)
                        {
                            territoryLocation = (gvGroupIn.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                            territoryCode = (gvGroupIn.Rows[i].FindControl("lblSellerCode") as Label).Text;
                            removeTerritories = (CheckBox)gvGroupIn.Rows[i].FindControl("cbRemoveSeller");

                            if (hdrTerritoryLocation == territoryLocation & !string.IsNullOrWhiteSpace(territoryCode))
                            {
                                gvGroupIn.Rows[i].Visible = true;
                                removeTerritories.Checked = false;
                            }
                            else
                            {
                                //we have reached the end of the current group
                                //make expand image invisible and collapse image visible
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                                break;
                            }

                            //if we are dealing with the last row,
                            //hide/unhide collapse/expand logic needs to be
                            //handled here
                            if (i + 1 == gvGroupIn.Rows.Count)
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                        }
                    }
                }
                else
                {
                    if (!hrdRemoveTerritories.Checked)
                    {

                        for (int i = currRowIndex - 1; i >= 0; i--)
                        {
                            territoryLocation = (gvGroupIn.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                            territoryCode = (gvGroupIn.Rows[i].FindControl("lblSellerCode") as Label).Text;
                            removeTerritories = (CheckBox)gvGroupIn.Rows[i].FindControl("cbRemoveSeller");

                            if (hdrTerritoryLocation == territoryLocation & string.IsNullOrWhiteSpace(territoryCode))
                            {
                                removeTerritories.Checked = false;
                                break;
                            }
                        }
                    }
                }

                CheckBox cbRemoveTettritory;
                int checkCount = 0;
                foreach (GridViewRow gdRow in gvGroupIn.Rows)
                {
                    cbRemoveTettritory = (CheckBox)(gdRow.FindControl("cbRemoveSeller"));
                    if (cbRemoveTettritory.Checked)
                    {
                        checkCount++;
                    }
                }

                if (!hrdRemoveTerritories.Checked)
                {
                    cbInSelectAll.Checked = false;
                    if (checkCount == 0)
                    {
                        hdnisItemSelectedIn.Value = "N";
                    }
                }
                else
                {
                    hdnisItemSelectedIn.Value = "Y";
                    if (gvGroupIn.Rows.Count == checkCount)
                    {
                        cbInSelectAll.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error selecting checkbox.", ex.Message);
            }
        }

        protected void cbAddSeller_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
                int currRowIndex = row.RowIndex;

                CheckBox hrdRemoveTerritories;
                string hdrTerritoryLocation = (gvGroupOut.Rows[currRowIndex].FindControl("lblSellerLocation") as Label).Text;
                string hdrTerritoryCode = (gvGroupOut.Rows[currRowIndex].FindControl("lblSellerCode") as Label).Text;
                hrdRemoveTerritories = (CheckBox)gvGroupOut.Rows[currRowIndex].FindControl("cbAddSeller");

                CheckBox removeTerritories;
                string territoryLocation;
                string territoryCode;

                if (string.IsNullOrWhiteSpace(hdrTerritoryCode))
                {
                    if (hrdRemoveTerritories.Checked)
                    {

                        for (int i = currRowIndex + 1; i < gvGroupOut.Rows.Count; i++)
                        {
                            territoryLocation = (gvGroupOut.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                            territoryCode = (gvGroupOut.Rows[i].FindControl("lblSellerCode") as Label).Text;
                            removeTerritories = (CheckBox)gvGroupOut.Rows[i].FindControl("cbAddSeller");

                            if (hdrTerritoryLocation == territoryLocation & !string.IsNullOrWhiteSpace(territoryCode))
                            {
                                gvGroupOut.Rows[i].Visible = true;
                                removeTerritories.Checked = true;
                            }
                            else
                            {
                                //we have reached the end of the current group
                                //make expand image invisible and collapse image visible
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                                break;
                            }

                            //if we are dealing with the last row,
                            //hide/unhide collapse/expand logic needs to be
                            //handled here
                            if (i + 1 == gvGroupOut.Rows.Count)
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                        }
                    }
                    else
                    {
                        for (int i = currRowIndex + 1; i < gvGroupOut.Rows.Count; i++)
                        {
                            territoryLocation = (gvGroupOut.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                            territoryCode = (gvGroupOut.Rows[i].FindControl("lblSellerCode") as Label).Text;
                            removeTerritories = (CheckBox)gvGroupOut.Rows[i].FindControl("cbAddSeller");

                            if (hdrTerritoryLocation == territoryLocation & !string.IsNullOrWhiteSpace(territoryCode))
                            {
                                gvGroupOut.Rows[i].Visible = true;
                                removeTerritories.Checked = false;
                            }
                            else
                            {
                                //we have reached the end of the current group
                                //make expand image invisible and collapse image visible
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                                break;
                            }

                            //if we are dealing with the last row,
                            //hide/unhide collapse/expand logic needs to be
                            //handled here
                            if (i + 1 == gvGroupOut.Rows.Count)
                                HideUnHideToggleButtons(row.Cells[0], false, true);
                        }
                    }
                }
                else
                {
                    if (!hrdRemoveTerritories.Checked)
                    {

                        for (int i = currRowIndex - 1; i >= 0; i--)
                        {
                            territoryLocation = (gvGroupOut.Rows[i].FindControl("lblSellerLocation") as Label).Text;
                            territoryCode = (gvGroupOut.Rows[i].FindControl("lblSellerCode") as Label).Text;
                            removeTerritories = (CheckBox)gvGroupOut.Rows[i].FindControl("cbAddSeller");

                            if (hdrTerritoryLocation == territoryLocation & string.IsNullOrWhiteSpace(territoryCode))
                            {
                                removeTerritories.Checked = false;
                                break;
                            }
                        }
                    }
                }

                CheckBox cbAddTettritory;
                int checkCount = 0;
                foreach (GridViewRow gdRow in gvGroupOut.Rows)
                {
                    cbAddTettritory = (CheckBox)(gdRow.FindControl("cbAddSeller"));
                    if (cbAddTettritory.Checked)
                    {
                        checkCount++;
                    }
                }


                if (!hrdRemoveTerritories.Checked)
                {
                    cbOutSelectAll.Checked = false;
                    if (checkCount == 0)
                    {
                        hdnisItemSelectedOut.Value = "N";
                    }
                }
                else
                {
                    hdnisItemSelectedOut.Value = "Y";
                    if (gvGroupOut.Rows.Count == checkCount)
                    {
                        cbOutSelectAll.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error selecting checkbox.", ex.Message);
            }
        }

        protected void fuzzySearchTerritoryGroup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchTerritoryGroup();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpOutGridPnlHeight.Value == string.Empty ? "300" : hdnGrpOutGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory group fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "TerritoryGrp")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtSellerGroup.Text = string.Empty;
                        LoadInitialGridData();
                        btnAddTerritory.Enabled = false;
                        btnRemoveTerritory.Enabled = false;
                        txtSellerGroup.Text = string.Empty;
                        txtGroupOutBox.Text = string.Empty;
                        txtGroupInBox.Text = string.Empty;
                        txtGroupInLoc.Text = string.Empty;
                        txtGroupOutLoc.Text = string.Empty;
                        Session["groupOutData"] = null;
                        Session["groupInData"] = null;                        
                        Session["groupTerritoryLocationOut"] = null;
                        Session["groupTerritoryLocationIn"] = null;
                        cbOutSelectAll.Enabled = false;
                        cbInSelectAll.Enabled = false;
                        return;
                    }

                    txtSellerGroup.Text = lbFuzzySearch.SelectedValue.ToString();

                    TerritoryGroupSelected();
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
                if (hdnFuzzySearchField.Value == "TerritoryGrp")
                {
                    LoadInitialGridData();
                    btnAddTerritory.Enabled = false;
                    btnRemoveTerritory.Enabled = false;
                    txtSellerGroup.Text = string.Empty;
                    txtGroupOutBox.Text = string.Empty;
                    txtGroupInBox.Text = string.Empty;
                    txtGroupInLoc.Text = string.Empty;
                    txtGroupOutLoc.Text = string.Empty;
                    Session["groupOutData"] = null;
                    Session["groupInData"] = null;                    
                    Session["groupTerritoryLocationOut"] = null;
                    Session["groupTerritoryLocationIn"] = null;
                    cbOutSelectAll.Enabled = false;
                    cbInSelectAll.Enabled = false;
                }
               
                mpeFuzzySearch.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing search list pop up", ex.Message);
            }
        }

        protected void btnTerritoryNameSearchOut_Click(object sender, EventArgs e)
        {
            try
            {
                string territoryCode;
                string hdrTerritoryLocation;
                Session["groupTerritoryLocationOut"] = null;
                if (Session["groupOutData"] != null)
                {
                    //Copy the out of group data to a datatable so that any change to datatable wont affect the original data
                    DataTable groupOutData = (Session["groupOutData"] as DataTable).Copy();
                    DataTable dtSearched = (Session["groupOutData"] as DataTable).Clone();
                    DataTable dtSearchedComplete = (Session["groupOutData"] as DataTable).Clone();

                    if (string.IsNullOrWhiteSpace(txtGroupOutBox.Text))
                    {
                        BindOutGrid(groupOutData);
                    }
                    else
                    {
                        DataRow[] foundRows = groupOutData.Select("seller_name Like '%" + txtGroupOutBox.Text.Trim().Replace("'", "''") + "%' or seller_code Like '%" + txtGroupOutBox.Text.Trim().Replace("'", "''") + "%' ");
                        if (foundRows.Length != 0)
                        {
                            dtSearched = foundRows.CopyToDataTable();
                        }



                        foreach (DataRow dRow in dtSearched.Rows)
                        {
                            DataRow[] locationRows = groupOutData.Select("seller_location = '" + dRow["seller_location"].ToString().Replace("'", "''") + "' and (seller_name = '" + dRow["seller_name"].ToString().Replace("'", "''") + "' or seller_name ='' )");
                            if (locationRows.Length != 0)
                            {
                                foreach (DataRow dataRow in locationRows)
                                {
                                    dtSearchedComplete.ImportRow(dataRow);
                                }
                            }
                        }

                        dtSearchedComplete = dtSearchedComplete.DefaultView.ToTable(true, "seller_location", "seller_code", "seller_name");
                        BindOutGrid(dtSearchedComplete);

                        foreach (GridViewRow datarow in gvGroupOut.Rows)
                        {
                            hdrTerritoryLocation = (datarow.FindControl("lblSellerLocation") as Label).Text;
                            territoryCode = (datarow.FindControl("lblSellerCode") as Label).Text;

                            datarow.Visible = true;

                            if (string.IsNullOrWhiteSpace(territoryCode))
                            {
                                HideUnHideToggleButtons(gvGroupOut.Rows[datarow.RowIndex].Cells[0], false, true);
                            }
                        }
                    }
                }
                txtGroupOutLoc.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory search", ex.Message);
            }
        }

        protected void btnTerritoryLocSearchOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["groupOutData"] != null)
                {
                    DataTable groupOutData = (Session["groupOutData"] as DataTable).Copy();
                    DataTable dtSearched = (Session["groupOutData"] as DataTable).Clone();

                    DataRow[] foundRows = groupOutData.Select("seller_location Like '%" + txtGroupOutLoc.Text.Trim().Replace("'", "''") + "%'");
                    if (foundRows.Length != 0)
                    {
                        dtSearched = foundRows.CopyToDataTable();
                    }

                    BindOutGrid(dtSearched);
                }
                txtGroupOutBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory location search", ex.Message);
            }
        }

        protected void btnTerritoryNameSearchIn_Click(object sender, EventArgs e)
        {
            try
            {
                string territoryCode;
                string hdrTerritoryLocation;
                Session["groupTerritoryLocationIn"] = null;
                //Copy the out of group data to a datatable so that any change to datatable wont affect the original data
                if (Session["groupInData"] != null)
                {
                    DataTable groupInData = (Session["groupInData"] as DataTable).Copy();
                    DataTable dtSearched = (Session["groupInData"] as DataTable).Clone();
                    DataTable dtSearchedComplete = (Session["groupInData"] as DataTable).Clone();

                    if (string.IsNullOrWhiteSpace(txtGroupInBox.Text))
                    {
                        BindInGrid(groupInData);
                    }
                    else
                    {
                        DataRow[] foundRows = groupInData.Select("seller_name Like '%" + txtGroupInBox.Text.Trim().Replace("'", "''") + "%' or seller_code Like '%" + txtGroupInBox.Text.Trim().Replace("'", "''") + "%'");
                        if (foundRows.Length != 0)
                        {
                            dtSearched = foundRows.CopyToDataTable();
                        }

                        foreach (DataRow dRow in dtSearched.Rows)
                        {
                            DataRow[] locationRows = groupInData.Select("seller_location = '" + dRow["seller_location"].ToString().Replace("'", "''") + "' and (seller_name = '" + dRow["seller_name"].ToString().Replace("'", "''") + "' or seller_name ='' )");
                            if (locationRows.Length != 0)
                            {
                                foreach (DataRow dataRow in locationRows)
                                {
                                    dtSearchedComplete.ImportRow(dataRow);
                                }
                            }
                        }

                        dtSearchedComplete = dtSearchedComplete.DefaultView.ToTable(true, "seller_location", "seller_code", "seller_name");
                        BindInGrid(dtSearchedComplete);

                        foreach (GridViewRow datarow in gvGroupIn.Rows)
                        {
                            hdrTerritoryLocation = (datarow.FindControl("lblSellerLocation") as Label).Text;
                            territoryCode = (datarow.FindControl("lblSellerCode") as Label).Text;

                            datarow.Visible = true;

                            if (string.IsNullOrWhiteSpace(territoryCode))
                            {
                                HideUnHideToggleButtons(gvGroupIn.Rows[datarow.RowIndex].Cells[0], false, true);
                            }
                        }
                    }

                }
                txtGroupInLoc.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory search", ex.Message);
            }
        }

        protected void btnTerritoryLocSearchIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["groupInData"] != null)
                {
                    DataTable groupInData = (Session["groupInData"] as DataTable).Copy();
                    DataTable dtSearched = (Session["groupInData"] as DataTable).Clone();

                    DataRow[] foundRows = groupInData.Select("seller_location Like '%" + txtGroupInLoc.Text.Trim().Replace("'", "''") + "%'");
                    if (foundRows.Length != 0)
                    {
                        dtSearched = foundRows.CopyToDataTable();
                    }

                    BindInGrid(dtSearched);
                }
                txtGroupInBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory location search", ex.Message);
            }
        }

        protected void cbOutSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox addTerritory;
                if (cbOutSelectAll.Checked)
                {
                    hdnisItemSelectedOut.Value = "Y";
                    foreach (GridViewRow datarow in gvGroupOut.Rows)
                    {
                        addTerritory = datarow.FindControl("cbAddSeller") as CheckBox;
                        addTerritory.Checked = true;
                    }
                }
                else
                {
                    hdnisItemSelectedOut.Value = "N";
                    foreach (GridViewRow datarow in gvGroupOut.Rows)
                    {
                        addTerritory = datarow.FindControl("cbAddSeller") as CheckBox;
                        addTerritory.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting all checkboxes of grid", ex.Message);
            }
        }

        protected void cbInSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox removeTerritory;
                if (cbInSelectAll.Checked)
                {
                    hdnisItemSelectedIn.Value = "Y";
                    foreach (GridViewRow datarow in gvGroupIn.Rows)
                    {
                        removeTerritory = datarow.FindControl("cbRemoveSeller") as CheckBox;
                        removeTerritory.Checked = true;
                    }
                }
                else
                {
                    hdnisItemSelectedIn.Value = "N";
                    foreach (GridViewRow datarow in gvGroupIn.Rows)
                    {
                        removeTerritory = datarow.FindControl("cbRemoveSeller") as CheckBox;
                        removeTerritory.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting all checkboxes of grid", ex.Message);
            }
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

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSelectedSellerGrpFilter())
                {
                    Response.Redirect(@"~/Audit/TerritoryGroupAudit.aspx?terriroryGroupCode=" + txtSellerGroup.Text.Split('-')[0].ToString().Trim() + "", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid territory group from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to territory group audit screen.", ex.Message);
            }
        }

        protected void btnCheckRoyaltors_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSelectedSellerGrpFilter())
                {
                    Response.Redirect(@"~/DataMaintenance/TerritoryGroupRoyaltors.aspx?terriroryGroupCode=" + txtSellerGroup.Text.Split('-')[0].ToString().Trim() + "", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid territory group from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to territory group audit screen.", ex.Message);
            }
        }

        #endregion EVENTS

        #region METHODS

        private void HideUnHideToggleButtons(TableCell cell, bool hideCollapseButton, bool hideExpandButton)
        {
            ImageButton imgExpand = (ImageButton)cell.FindControl("imgExpand");//+
            imgExpand.Visible = !hideExpandButton;
            ImageButton imgCollapse = (ImageButton)cell.FindControl("imgCollapse");//-
            imgCollapse.Visible = !hideCollapseButton;
        }

        private void LoadSellerGroups()
        {
           
            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                string territoryGroupCode = Request.QueryString[0];

                //populate seller group for the seller group code
                if (Session["FuzzySearchSellerGroupListTypeP"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetSellerGroupListTypeP", out errorId);
                    Session["FuzzySearchSellerGroupListTypeP"] = dsList.Tables[0];
                }

                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchSellerGroupListTypeP"]).Select("seller_group_code = '" + territoryGroupCode + "'");
                txtSellerGroup.Text = filteredRow[0]["seller_group"].ToString();
                LoadGridData(territoryGroupCode);
                lblTab.Focus();
            }
        }

        private void LoadGridData(string groupCode)
        {          
            sellerGroupBL = new SellerGroupBL();
            DataSet sellerGroupList = sellerGroupBL.GetSellerGroupInOutData(groupCode, out errorId);
            sellerGroupBL = null;
            BindGrids(sellerGroupList);        
        }

        private void LoadInitialGridData()
        {
            dtEmpty = new DataTable();
            gvGroupIn.DataSource = dtEmpty;
            gvGroupIn.DataBind();
            gvGroupOut.DataSource = dtEmpty;
            gvGroupOut.DataBind();
        }

        private bool ValidateSelectedSellerGrpFilter()
        {
            if (txtSellerGroup.Text != "" && Session["FuzzySearchSellerGroupListTypeP"] != null)
            {
                if (txtSellerGroup.Text != "No results found")
                {
                    DataTable dtSellerGroupList;
                    dtSellerGroupList = Session["FuzzySearchSellerGroupListTypeP"] as DataTable;

                    foreach (DataRow dRow in dtSellerGroupList.Rows)
                    {
                        if (dRow["seller_group"].ToString() == txtSellerGroup.Text)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    //txtSellerGroup.Text = string.Empty;
                    return false;
                }
            }
            else
            {
                //txtSellerGroup.Text = string.Empty;
                return false;
            }
        }

        private bool ValidateSelectedSellerOut()
        {
            if (txtGroupOutBox.Text != "" && Session["groupOutData"] != null)
            {
                if (txtGroupOutBox.Text != "No results found")
                {
                    DataTable dtSellerOutList;
                    dtSellerOutList = Session["groupOutData"] as DataTable;

                    foreach (DataRow dRow in dtSellerOutList.Rows)
                    {
                        if (dRow["seller_data"].ToString() == txtGroupOutBox.Text)
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

        private bool ValidateSelectedSellerIn()
        {
            if (txtGroupInBox.Text != "" && Session["groupInData"] != null)
            {
                if (txtGroupInBox.Text != "No results found")
                {
                    DataTable dtSellerOutList;
                    dtSellerOutList = Session["groupInData"] as DataTable;

                    foreach (DataRow dRow in dtSellerOutList.Rows)
                    {
                        if (dRow["seller_data"].ToString() == txtGroupInBox.Text)
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

        private void BindGrids(DataSet sellerGroupList)
        {
            if (sellerGroupList.Tables.Count != 0 && errorId != 2)
            {
                Session["groupInData"] = sellerGroupList.Tables[0];
                Session["groupOutData"] = sellerGroupList.Tables[1];                
                Session["ActiveInGroupData"] = sellerGroupList.Tables[0];
                Session["ActiveOutGroupData"] = sellerGroupList.Tables[1];

                gvGroupIn.DataSource = sellerGroupList.Tables[0];
                if (sellerGroupList.Tables[0].Rows.Count == 0)
                {
                    btnRemoveTerritory.Enabled = false;
                    gvGroupIn.EmptyDataText = "No data found for the selected territory group.";
                }
                else
                {
                    btnRemoveTerritory.Enabled = true;
                }
                gvGroupIn.DataBind();

                gvGroupOut.DataSource = sellerGroupList.Tables[1];
                if (sellerGroupList.Tables[1].Rows.Count == 0)
                {
                    btnAddTerritory.Enabled = false;
                    gvGroupOut.EmptyDataText = "No data found for the selected territory group.";
                }
                else
                {
                    btnAddTerritory.Enabled = true;
                }
                gvGroupOut.DataBind();
               
            }
            else if (sellerGroupList.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvGroupIn.DataSource = dtEmpty;
                gvGroupIn.EmptyDataText = "No data found for the selected territory group.";
                gvGroupIn.DataBind();
                gvGroupOut.DataSource = dtEmpty;
                gvGroupOut.EmptyDataText = "No data found for the selected territory group.";
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
            UserAuthorization();
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        private void FuzzySearchTerritoryGroup()
        {            
            if (txtSellerGroup.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in territory group field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "TerritoryGrp";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchSellerGroupListTypeP(txtSellerGroup.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void TerritoryGroupSelected()
        {
            if (ValidateSelectedSellerGrpFilter())
            {
                groupCode = txtSellerGroup.Text.Substring(0, txtSellerGroup.Text.IndexOf("-") - 1);

                LoadGridData(groupCode);

                btnRemoveTerritory.Enabled = true;
            }
            else
            {
                btnAddTerritory.Enabled = false;
                btnRemoveTerritory.Enabled = false;
                txtSellerGroup.Text = string.Empty;
                Session["groupOutData"] = null;
                Session["groupInData"] = null;                
                Session["groupTerritoryLocationOut"] = null;
                Session["groupTerritoryLocationIn"] = null;
            }
        }

        private void BindOutGrid(DataTable groupOutData)
        {
            Session["ActiveOutGroupData"] = groupOutData;

            if (groupOutData.Rows.Count != 0)
            {
                gvGroupOut.DataSource = groupOutData;
                gvGroupOut.DataBind();
            }
            else if (groupOutData.Rows.Count == 0)
            {
                dtEmpty = new DataTable();
                gvGroupOut.DataSource = dtEmpty;
                gvGroupOut.EmptyDataText = "No data found for the selected territory.";
                gvGroupOut.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

            if (gvGroupOut.Rows.Count > 0)
            {
                PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
            }
        }

        private void BindInGrid(DataTable groupInData)
        {
            Session["ActiveInGroupData"] = groupInData;

            if (groupInData.Rows.Count != 0)
            {
                gvGroupIn.DataSource = groupInData;
                gvGroupIn.DataBind();
            }
            else if (groupInData.Rows.Count == 0)
            {
                dtEmpty = new DataTable();
                gvGroupIn.DataSource = dtEmpty;
                gvGroupIn.EmptyDataText = "No data found for the selected territory.";
                gvGroupIn.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

            if (gvGroupIn.Rows.Count > 0)
            {
                PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
            }

        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnAddTerritory.Enabled = false;
                btnAddTerritoryGroup.Enabled = false;
                btnRemoveTerritory.Enabled = false;
                btnCancel.Enabled = false;
                cbOutSelectAll.Enabled = false;
                cbInSelectAll.Enabled = false;
                foreach (GridViewRow rows in gvGroupIn.Rows)
                {
                    (rows.FindControl("cbRemoveSeller") as CheckBox).Enabled = false;
                }
                foreach (GridViewRow rows in gvGroupOut.Rows)
                {
                    (rows.FindControl("cbAddSeller") as CheckBox).Enabled = false;
                }

            }
        }


        #endregion METHODS

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
            TerritoryLocation = 1
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

        protected void btnTerritoryLocSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveInGroupData"] == null)
                    return;

                DataTable dtActiveInGroupData = (DataTable)Session["ActiveInGroupData"];

                if (GridSortColumnInGroup == GridSortColumns.None.ToString())
                    GridSortDirectionInGroup = GridSortDirections.Descending.ToString();
                else if (GridSortColumnInGroup == GridSortColumns.TerritoryLocation.ToString())
                    GridSortDirectionInGroup = GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionInGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnInGroup = GridSortColumns.TerritoryLocation.ToString();
                dtActiveInGroupData.DefaultView.Sort = "seller_location" + (GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupIn.DataSource = dtActiveInGroupData;
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

        protected void btnTerritoryLocOutSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveOutGroupData"] == null)
                    return;

                DataTable dtActiveOutGroupData = (DataTable)Session["ActiveOutGroupData"];

                if (GridSortColumnOutGroup == GridSortColumns.None.ToString())
                    GridSortDirectionOutGroup = GridSortDirections.Descending.ToString();
                else if (GridSortColumnOutGroup == GridSortColumns.TerritoryLocation.ToString())
                    GridSortDirectionOutGroup = GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionOutGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnOutGroup = GridSortColumns.TerritoryLocation.ToString();
                dtActiveOutGroupData.DefaultView.Sort = "seller_location" + (GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupOut.DataSource = dtActiveOutGroupData;
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

        #endregion Sorting grid data
               


    }
}