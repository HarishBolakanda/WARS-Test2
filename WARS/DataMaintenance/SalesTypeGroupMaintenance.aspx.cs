/*
File Name   :   SalesTypeGroupMaintenance.cs
Purpose     :   to maintain Sales Type Groups based on group code

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     31-Mar-2017     Pratik(Infosys Limited)   Initial Creation
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

namespace WARS
{
    public partial class PriceGroupMaintenance : System.Web.UI.Page
    {
        #region Global Declarations
        SalesTypeGroupingBL salesTypeGroupingBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string ASCENDING = " ASC";
        string DESCENDING = " DESC";
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Sales Type Group Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Sales Type Group Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {
                    txtSalesTypeGroup.Focus();
                                        
                    Session["SalesTypeGroupInData"] = null;
                    Session["SalesTypeGroupOutData"] = null;
                    Session["ActiveSalesTypeGroupInData"] = null;
                    Session["ActiveSalesTypeGroupOutData"] = null;
                    cbInSelectAll.Enabled = false;
                    cbOutSelectAll.Enabled = false;
                    btnRemoveSalesType.Enabled = false;
                    btnAddSalesType.Enabled = false;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        //do nothing on initial load
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                }
                if (Session["UserRole"].ToString().ToLower() != UserRole.SuperUser.ToString().ToLower())
                {
                    btnAddSalesTypeGroup.Enabled = false; 
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void fuzzySearchSalesTypeGrp_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchSalesTypeGroup();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGrpInGridPnlHeight.Value == string.Empty ? "300" : hdnGrpInGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sales type group search.", ex.Message);
            }
        }

        protected void btnAddSalesTypeGroup_Click(object sender, EventArgs e)
        {            
            try
            {
                txtGroupCode.Text = string.Empty;
                txtGroupDesc.Text = string.Empty;
                mpeInsertGroup.Show();
                txtGroupCode.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding a new sales type group.", ex.Message);
            }
        }
       
        protected void btnRemoveSalesType_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtSalesTypeGroup.Text))
                {
                    string selectdSalesTypeGroupCode = txtSalesTypeGroup.Text.Split('-')[0].ToString().Trim();
                    string salesTypeCode;
                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                    List<string> salesTypeCodesToRemove = new List<string>();
                    CheckBox cbRemoveSalesType;

                    foreach (GridViewRow gridrow in gvGroupIn.Rows)
                    {
                        cbRemoveSalesType = (CheckBox)gridrow.FindControl("cbRemoveSalesType");
                        if (cbRemoveSalesType.Checked)
                        {
                            salesTypeCode = (gridrow.FindControl("lblSalesTypeCode") as Label).Text;
                            salesTypeCodesToRemove.Add(salesTypeCode);
                        }
                    }

                    if (salesTypeCodesToRemove.Count > 0)
                    {
                        salesTypeGroupingBL = new SalesTypeGroupingBL();
                        DataSet salesTypeGroupInOutData = salesTypeGroupingBL.RemoveSalesTypeFromGroup(selectdSalesTypeGroupCode, salesTypeCodesToRemove.ToArray(),userCode, out errorId);
                        salesTypeGroupingBL = null;

                        BindGrids(salesTypeGroupInOutData);

                        txtGroupOutBox.Text = string.Empty;
                        txtGroupInBox.Text = string.Empty;

                        hdnisItemSelectedIn.Value = "N";
                        hdnisItemSelectedOut.Value = "N";
                    }
                    else
                    {
                        msgView.SetMessage("Please select a sales type from 'In the Group' list",
                                            MessageType.Warning, PositionType.Auto);
                    }
                }
                else
                {
                    msgView.SetMessage("Please select a sales type group from searched list",
                                            MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing sales type from group.", ex.Message);
            }
        }

        protected void btnAddSalesType_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtSalesTypeGroup.Text))
                {
                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    string selectdSalesTypeGroupCode = txtSalesTypeGroup.Text.Split('-')[0].ToString().Trim();
                    string salesTypeCode;

                    List<string> salesTypeCodesToAdd = new List<string>();
                    CheckBox cbAddSalesType;

                    foreach (GridViewRow gridrow in gvGroupOut.Rows)
                    {
                        cbAddSalesType = (CheckBox)gridrow.FindControl("cbAddSalesType");
                        if (cbAddSalesType.Checked)
                        {
                            salesTypeCode = (gridrow.FindControl("lblSalesTypeCode") as Label).Text;
                            salesTypeCodesToAdd.Add(salesTypeCode);
                        }
                    }

                    if (salesTypeCodesToAdd.Count > 0)
                    {
                        salesTypeGroupingBL = new SalesTypeGroupingBL();
                        DataSet salesTypeGroupInOutData = salesTypeGroupingBL.AddSalesTypeToGroup(selectdSalesTypeGroupCode, salesTypeCodesToAdd.ToArray(), userCode, out errorId);
                        salesTypeGroupingBL = null;

                        BindGrids(salesTypeGroupInOutData);

                        txtGroupOutBox.Text = string.Empty;
                        txtGroupInBox.Text = string.Empty;

                        hdnisItemSelectedIn.Value = "N";
                        hdnisItemSelectedOut.Value = "N";
                    }
                    else
                    {
                        msgView.SetMessage("Please select a sales type from 'Out of the Group' list",
                                            MessageType.Warning, PositionType.Auto);
                    }
                }
                else
                {
                    msgView.SetMessage("Please select a sales type group from searched list",
                                            MessageType.Warning, PositionType.Auto);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding sales type to group.", ex.Message);
            }
        }
        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                salesTypeGroupingBL = new SalesTypeGroupingBL();
                DataSet salesTypeGroupInOutData = salesTypeGroupingBL.InsertSalesTypeGroup(txtGroupCode.Text.Trim(), txtGroupDesc.Text.Trim(), userCode, out errorId);
                salesTypeGroupingBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("The sales type group code already exists",
                        MessageType.Warning, PositionType.Auto);
                    mpeInsertGroup.Show();
                    txtGroupCode.Focus();
                }

                else
                {
                    BindGrids(salesTypeGroupInOutData);
                    if (salesTypeGroupInOutData.Tables.Count == 3)
                    {
                        Session["FuzzySearchPriceGroupListTypeP"] = salesTypeGroupInOutData.Tables[2];
                    }
                    txtSalesTypeGroup.Text = txtGroupCode.Text.Trim() + " - " + txtGroupDesc.Text.Trim();
                    hdnIsValidSearch.Value = "Y";
                    txtGroupCode.Text = string.Empty;
                    txtGroupDesc.Text = string.Empty;
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
                    txtSalesTypeGroup.Text = string.Empty;
                    return;
                }

                txtSalesTypeGroup.Text = lbFuzzySearch.SelectedValue.ToString();
                SalesTypeGroupSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting label.", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtSalesTypeGroup.Text = string.Empty;
                txtGroupInBox.Text = string.Empty;
                txtGroupOutBox.Text = string.Empty;
                hdnIsValidSearch.Value = "N";
                hdnisItemSelectedIn.Value = "N";
                hdnisItemSelectedOut.Value = "N";

                dtEmpty = new DataTable();
                gvGroupIn.DataSource = dtEmpty;
                gvGroupIn.EmptyDataText = "No data found.";
                gvGroupIn.DataBind();
                gvGroupOut.DataSource = dtEmpty;
                gvGroupOut.EmptyDataText = "No data found.";
                gvGroupOut.DataBind();

                cbInSelectAll.Enabled = false;
                cbOutSelectAll.Enabled = false;
                btnAddSalesType.Enabled = false;
                btnRemoveSalesType.Enabled = false;

                Session["SalesTypeGroupInData"] = null;
                Session["SalesTypeGroupOutData"] = null;
                Session["ActiveSalesTypeGroupInData"] = null;
                Session["ActiveSalesTypeGroupOutData"] = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing sales type group search list.", ex.Message);
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

        protected void btnSalesTypeSearchOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["SalesTypeGroupOutData"] != null)
                {
                    //Copy the out of group data to a datatable so that any change to datatable wont affect the original data
                    DataTable groupOutData = (Session["SalesTypeGroupOutData"] as DataTable).Copy();
                    DataTable dtSearched = (Session["SalesTypeGroupOutData"] as DataTable).Clone();

                    DataRow[] foundRows = groupOutData.Select("price_group_code Like '%" + txtGroupOutBox.Text.Replace("'", "").Trim() + "%' or price_name Like '%" + txtGroupOutBox.Text.Replace("'", "").Trim() + "%'");
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
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sales type search.", ex.Message);
            }
        }

        protected void btnSalesTypeSearchIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["SalesTypeGroupInData"] != null)
                {
                    //Copy the out of group data to a datatable so that any change to datatable wont affect the original data
                    DataTable groupInData = (Session["SalesTypeGroupInData"] as DataTable).Copy();
                    DataTable dtSearched = (Session["SalesTypeGroupInData"] as DataTable).Clone();

                    DataRow[] foundRows = groupInData.Select("price_group_code Like '%" + txtGroupInBox.Text.Replace("'", "").Trim() + "%' or price_name Like '%" + txtGroupInBox.Text.Replace("'", "").Trim() + "%'");
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
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sales type search.", ex.Message);
            }
        }

        protected void btnHdnSalesTypeSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SalesTypeGroupSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting sales type group.", ex.Message);
            }
        }

        protected void btnDiscard_Click(object sender, EventArgs e)
        {
            try
            {
                hdnisItemSelectedIn.Value = "N";
                hdnisItemSelectedOut.Value = "N";

                DataTable dtActiveSalesTypeGroupInData = Session["ActiveSalesTypeGroupInData"] as DataTable;
                DataTable dtActiveSalesTypeGroupOutData = Session["ActiveSalesTypeGroupOutData"] as DataTable;

                BindInGrid(dtActiveSalesTypeGroupInData);
                BindOutGrid(dtActiveSalesTypeGroupOutData);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in discarding changes.", ex.Message);
            }
        }

        #endregion Events 

        #region Methods

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true); 
        }

        private void SalesTypeGroupSelected()
        {
            hdnIsValidSearch.Value = "Y";
            salesTypeGroupingBL = new SalesTypeGroupingBL();
            DataSet salesTypeGroupInOutData = salesTypeGroupingBL.GetSalesTypeGroupInOutData(txtSalesTypeGroup.Text.Split('-')[0].ToString().Trim(), out errorId);
            salesTypeGroupingBL = null;

            BindGrids(salesTypeGroupInOutData);
            gvGroupIn.DataSource = salesTypeGroupInOutData.Tables[0];
            gvGroupIn.DataBind();

            txtGroupInBox.Text = string.Empty;
            txtGroupOutBox.Text = string.Empty;
            UserAuthorization();
        }

        private void BindGrids(DataSet salesTypeGroupInOutData)
        {
            if (salesTypeGroupInOutData.Tables.Count != 0 && errorId != 2)
            {
                Session["SalesTypeGroupInData"] = salesTypeGroupInOutData.Tables[0];
                Session["SalesTypeGroupOutData"] = salesTypeGroupInOutData.Tables[1];
                Session["ActiveSalesTypeGroupInData"] = salesTypeGroupInOutData.Tables[0];
                Session["ActiveSalesTypeGroupOutData"] = salesTypeGroupInOutData.Tables[1];

                gvGroupIn.DataSource = salesTypeGroupInOutData.Tables[0];
                if (salesTypeGroupInOutData.Tables[0].Rows.Count == 0)
                {
                    btnRemoveSalesType.Enabled = false;
                    gvGroupIn.EmptyDataText = "No data found.";
                }
                else
                {
                    btnRemoveSalesType.Enabled = true;
                }
                gvGroupIn.DataBind();

                gvGroupOut.DataSource = salesTypeGroupInOutData.Tables[1];
                if (salesTypeGroupInOutData.Tables[1].Rows.Count == 0)
                {
                    btnAddSalesType.Enabled = false;
                    gvGroupOut.EmptyDataText = "No data found.";
                }
                else
                {
                    btnAddSalesType.Enabled = true;
                }
                gvGroupOut.DataBind();
            }
            else if (salesTypeGroupInOutData.Tables.Count == 0 && errorId != 2)
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
            UserAuthorization();
        }

        private void BindOutGrid(DataTable groupOutData)
        {
            Session["ActiveSalesTypeGroupOutData"] = groupOutData;

            if (groupOutData.Rows.Count != 0)
            {
                gvGroupOut.DataSource = groupOutData;
                gvGroupOut.DataBind();
            }
            else if (groupOutData.Rows.Count == 0)
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

            if (gvGroupOut.Rows.Count > 0)
            {
                PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
            }
            UserAuthorization();
        }

        private void BindInGrid(DataTable groupInData)
        {
            Session["ActiveSalesTypeGroupInData"] = groupInData;

            if (groupInData.Rows.Count != 0)
            {
                gvGroupIn.DataSource = groupInData;
                gvGroupIn.DataBind();
            }
            else if (groupInData.Rows.Count == 0)
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

            if (gvGroupIn.Rows.Count > 0)
            {
                PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
            }
            UserAuthorization();
        }
        private void UserAuthorization()
        {
            if (Session["UserRole"].ToString().ToLower() != UserRole.SuperUser.ToString().ToLower())
            {
                foreach (GridViewRow rows in gvGroupOut.Rows)
                {
                    (rows.FindControl("cbAddSalesType") as CheckBox).Enabled = false;
                }
                foreach (GridViewRow rows in gvGroupIn.Rows)
                {
                    (rows.FindControl("cbRemoveSalesType") as CheckBox).Enabled = false;
                }
                cbOutSelectAll.Enabled = false;
                cbInSelectAll.Enabled = false;
                btnRemoveSalesType.Enabled = false;
                btnAddSalesType.Enabled = false;
                btnAddSalesTypeGroup.Enabled = false;
            }       
        }
        private void FuzzySearchSalesTypeGroup()
        {
            if (txtSalesTypeGroup.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in sales type group search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchPriceGroupListTypeP(txtSalesTypeGroup.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        #endregion Methods

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
            SalesTypeCode = 1,
            SalesTypeDesc = 2
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

        protected void btnOutCodeSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveSalesTypeGroupOutData"] == null)
                    return;

                DataTable dtActiveConfigData = (DataTable)Session["ActiveSalesTypeGroupOutData"];

                if (GridSortColumnOutGroup == GridSortColumns.None.ToString())
                    GridSortDirectionOutGroup = GridSortDirections.Descending.ToString();
                else if (GridSortColumnOutGroup == GridSortColumns.SalesTypeCode.ToString())
                    GridSortDirectionOutGroup = GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionOutGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnOutGroup = GridSortColumns.SalesTypeCode.ToString();
                dtActiveConfigData.DefaultView.Sort = "price_group_code" + (GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupOut.DataSource = dtActiveConfigData;
                gvGroupOut.DataBind();

                if (gvGroupOut.Rows.Count > 0)
                {
                    PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
                }
                UserAuthorization();
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
                if (Session["ActiveSalesTypeGroupOutData"] == null)
                    return;

                DataTable dtActiveConfigData = (DataTable)Session["ActiveSalesTypeGroupOutData"];

                if (GridSortColumnOutGroup == GridSortColumns.None.ToString())
                    GridSortDirectionOutGroup = GridSortDirections.Descending.ToString();
                else if (GridSortColumnOutGroup == GridSortColumns.SalesTypeDesc.ToString())
                    GridSortDirectionOutGroup = GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionOutGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnOutGroup = GridSortColumns.SalesTypeDesc.ToString();
                dtActiveConfigData.DefaultView.Sort = "price_name" + (GridSortDirectionOutGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupOut.DataSource = dtActiveConfigData;
                gvGroupOut.DataBind();

                if (gvGroupOut.Rows.Count > 0)
                {
                    PnlGroupOut.Style.Add("height", hdnGrpOutGridPnlHeight.Value);
                }
                UserAuthorization();
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
                if (Session["ActiveSalesTypeGroupInData"] == null)
                    return;

                DataTable dtActiveConfigData = (DataTable)Session["ActiveSalesTypeGroupInData"];

                if (GridSortColumnInGroup == GridSortColumns.None.ToString())
                    GridSortDirectionInGroup = GridSortDirections.Descending.ToString();
                else if (GridSortColumnInGroup == GridSortColumns.SalesTypeCode.ToString())
                    GridSortDirectionInGroup = GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionInGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnInGroup = GridSortColumns.SalesTypeCode.ToString();
                dtActiveConfigData.DefaultView.Sort = "price_group_code" + (GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupIn.DataSource = dtActiveConfigData;
                gvGroupIn.DataBind();

                if (gvGroupIn.Rows.Count > 0)
                {
                    PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
                }
                UserAuthorization();
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
                if (Session["ActiveSalesTypeGroupInData"] == null)
                    return;

                DataTable dtActiveConfigData = (DataTable)Session["ActiveSalesTypeGroupInData"];

                if (GridSortColumnInGroup == GridSortColumns.None.ToString())
                    GridSortDirectionInGroup = GridSortDirections.Descending.ToString();
                else if (GridSortColumnInGroup == GridSortColumns.SalesTypeDesc.ToString())
                    GridSortDirectionInGroup = GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirectionInGroup = GridSortDirections.Ascending.ToString();

                GridSortColumnInGroup = GridSortColumns.SalesTypeDesc.ToString();
                dtActiveConfigData.DefaultView.Sort = "price_name" + (GridSortDirectionInGroup == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvGroupIn.DataSource = dtActiveConfigData;
                gvGroupIn.DataBind();

                if (gvGroupIn.Rows.Count > 0)
                {
                    PnlGroupIn.Style.Add("height", hdnGrpInGridPnlHeight.Value);
                }
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        #endregion Sorting grid data

        
    }
}