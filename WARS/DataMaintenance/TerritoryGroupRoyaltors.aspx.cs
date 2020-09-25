/*
File Name   :   TerritoryGroupRoyaltors.cs
Purpose     :   To display royaltors for territory group 

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     11-Nov-2017     Pratik(Infosys Limited)   Initial Creation (WUIN-299)
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

namespace WARS.DataMaintenance
{
    public partial class TerritoryGroupRoyaltors : System.Web.UI.Page
    {
        #region Global Declarations
        TerritoryGroupRoyaltorsBL territoryGroupRoyaltorsBL;
        Utilities util;
        Int32 errorId;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize2"].ToString());
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltors for Territory Group";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltors for Territory Group";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlRoyaltors.Style.Add("height", hdnGridPnlGroupHeight.Value);
                PnlTerritoryGroup.Style.Add("height", hdnGridPnlTerritoryHeight.Value);

                if (!IsPostBack)
                {
                    txtTerritoryGroupSearch.Focus();
                    
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

        protected void fuzzyTerritoryGroupSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchTerritoryGroup();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in territory group search.", ex.Message);
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

                SearchTerritoryGroupRoyaltors();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting territory group.", ex.Message);
            }
        }

        protected void btnHdnTerritoryGroupSearch_Click(object sender, EventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
                SearchTerritoryGroupRoyaltors();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching royaltor list for selected territory group.", ex.Message);
            }
        }
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TerritoryGroupRoyaltors"] == null)
                    return;

                DataTable dtTerritoryGroupRoyaltors = Session["TerritoryGroupRoyaltors"] as DataTable;
                if (dtTerritoryGroupRoyaltors.Rows.Count == 0)
                    return;
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);            
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtTerritoryGroupRoyaltors, gridDefaultPageSize, gvRoyaltors, dtEmpty, rptPager);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        protected void gvRoyaltors_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void gvRoyaltors_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["TerritoryGroupRoyaltors"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvRoyaltors, dtEmpty, rptPager);
                Session["TerritoryGroupRoyaltors"] = dataView.ToTable();
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
                hdnIsValidSearch.Value = "Y";
                SearchTerritoryGroupRoyaltors();
            }
        }

        private void SearchTerritoryGroupRoyaltors()
        {
            territoryGroupRoyaltorsBL = new TerritoryGroupRoyaltorsBL();
            DataSet auditData = territoryGroupRoyaltorsBL.GetRoyaltorList(txtTerritoryGroupSearch.Text.Split('-')[0].ToString().Trim(), out errorId);
            territoryGroupRoyaltorsBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {
                hdnPageNumber.Value = "1";
                Session["TerritoryGroupRoyaltors"] = auditData.Tables[1];
                BindGrid(auditData.Tables[0], auditData.Tables[1]);
            }
            else
            {
                ExceptionHandler("Error in fetching royaltor list", string.Empty);
            }
        }

        private void BindGrid(DataTable territoryData, DataTable royaltorList)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (territoryData.Rows.Count > 0)
            {
                gvTerritoryGroup.DataSource = territoryData;
                gvTerritoryGroup.DataBind();
            }
            else
            {
                dtEmpty = new DataTable();
                gvTerritoryGroup.DataSource = dtEmpty;
                gvTerritoryGroup.DataBind();
            }
            
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), royaltorList, gridDefaultPageSize, gvRoyaltors, dtEmpty, rptPager);            
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