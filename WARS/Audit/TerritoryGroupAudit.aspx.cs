/*
File Name   :   TerritoryGroupAudit.cs
Purpose     :   To display Territory Group Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     11-Nov-2017     Pratik(Infosys Limited)   Initial Creation (WUIN-297)
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
    public partial class TerritoryGroupAudit : System.Web.UI.Page
    {
        #region Global Declarations
        TerritoryGroupAuditBL territoryGroupAuditBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Territory Group Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Territory Group Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlTerritoryGroupAudit.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtTerritoryGroupSearch.Focus();
                    tdData.Style.Add("display", "none");
                    trRoyAudit.Style.Add("display", "none");
                    
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
                SearchTerritoryGroupAuditData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in searching audit data.", ex.Message);
            }
        }

        protected void gvTerritoryGroupAudit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                Label lblTerritory;
                Label lblTerritoryName;
                Label lblTerritoryLoc;

                string changeType;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    lblTerritory = (e.Row.FindControl("lblTerritory") as Label);
                    lblTerritoryName = (e.Row.FindControl("lblTerritoryName") as Label);
                    lblTerritoryLoc = (e.Row.FindControl("lblTerritoryLoc") as Label);

                    changeType = (e.Row.FindControl("hdnChangeType") as HiddenField).Value;

                    //For deleted records 
                    //Change the color of details to red
                    if (changeType == "D")
                    {
                        lblTerritory.ForeColor = Color.Red;
                        lblTerritoryName.ForeColor = Color.Red;
                        lblTerritoryLoc.ForeColor = Color.Red;
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

        protected void fuzzySearchTerritoryGroup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchTerritoryGroup();
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
                    txtTerritoryGroupSearch.Text = string.Empty;
                    return;
                }

                txtTerritoryGroupSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                hdnIsValidSearch.Value = "Y";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }
        
        protected void btnTerritoryGroupMaint_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtTerritoryGroupSearch.Text))
                {
                    Response.Redirect(@"~/DataMaintenance/TerritoryGroup.aspx?terriroryGroupCode=" + txtTerritoryGroupSearch.Text.Split('-')[0].ToString().Trim() + "", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid territory group from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to territory group maintenence screen.", ex.Message);
            }
        }

        protected void valTerritoryGroup_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtTerritoryGroupSearch.Text))
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
        protected void gvTerritoryGroupAudit_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["TerritoryGroupAuditData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvTerritoryGroupAudit.DataSource = dataView;
                gvTerritoryGroupAudit.DataBind();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion Events

        #region Methods

        private void LoadData()
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
                txtTerritoryGroupSearch.Text = filteredRow[0]["seller_group"].ToString();             
                hdnTerritoryGroupCode.Value = txtTerritoryGroupSearch.Text.Split('-')[0].ToString().Trim();
                hdnIsValidSearch.Value = "Y";
                SearchTerritoryGroupAuditData();
            }
        }

        private void SearchTerritoryGroupAuditData()
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
            trRoyAudit.Style.Remove("display");

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


            territoryGroupAuditBL = new TerritoryGroupAuditBL();
            DataSet auditData = territoryGroupAuditBL.GetTerritoryGroupAuditData(txtTerritoryGroupSearch.Text.Split('-')[0].ToString().Trim(), fromDate, toDate, out errorId);
            territoryGroupAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {
                Session["TerritoryGroupAuditData"] = auditData.Tables[0]; 
                BindGrid(auditData.Tables[0]);
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
                gvTerritoryGroupAudit.DataSource = gridData;
                gvTerritoryGroupAudit.DataBind();
                //CompareRoyAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvTerritoryGroupAudit.DataSource = dtEmpty;
                gvTerritoryGroupAudit.DataBind();
            }
        }

        private void FuzzySearchTerritoryGroup()
        {
            if (txtTerritoryGroupSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in territory group search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchSellerGroupListTypeP(txtTerritoryGroupSearch.Text.Trim().ToUpper(), int.MaxValue);
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