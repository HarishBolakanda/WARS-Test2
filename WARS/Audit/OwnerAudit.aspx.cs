/*
File Name   :   OwnerAudit.cs
Purpose     :   to display Owner Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     10-Apr-2017     Pratik(Infosys Limited)   Initial Creation
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
    public partial class OwnerAudit : System.Web.UI.Page
    {
        #region Global Declarations
        OwnerAuditBL ownerAuditBL;
        string loggedUserID;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Owner Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Owner Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlOwnerDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtOwnerSearch.Focus();
                    tdData.Style.Add("display", "none");

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

        protected void fuzzySearchOwner_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchOwner();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in company search.", ex.Message);
            }
        }

        protected void btnHdnOwnerSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string ownerCode = txtOwnerSearch.Text.Split('-')[0].ToString().Trim();
                LoadOwnerAuditData(ownerCode);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading company details.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtOwnerSearch.Text = string.Empty;
                    return;
                }

                txtOwnerSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                string ownerCode = txtOwnerSearch.Text.Split('-')[0].ToString().Trim();
                LoadOwnerAuditData(ownerCode);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting owner.", ex.Message);
            }
        }

        protected void btnOwnerMaint_Click(object sender, EventArgs e)
        {
            if (hdnIsValidSearch.Value == "Y")
            {
                Response.Redirect("../DataMaintenance/OwnerMaintenance.aspx?ownerData=" + txtOwnerSearch.Text + "");
            }
            else
            {
                Response.Redirect("../DataMaintenance/OwnerMaintenance.aspx");
            }
        }

        protected void gvOwnerDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Label lblOwnerDesc;
            HiddenField hdnClrOwnerDesc;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                lblOwnerDesc = (e.Row.FindControl("lblOwnerDesc") as Label);
                hdnClrOwnerDesc = (e.Row.FindControl("hdnClrOwnerDesc") as HiddenField);
                if (hdnClrOwnerDesc.Value == "R")
                {
                    lblOwnerDesc.ForeColor = Color.Red;
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

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvOwnerDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["OwnerAuditData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvOwnerDetails.DataSource = dataView;
                gvOwnerDetails.DataBind();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End


        #endregion Events

        #region Methods

        private void LoadData()
        {
            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                string ownerValue = Request.QueryString[0];
                string ownerCode = ownerValue.Split('-')[0].ToString().Trim();

                //populate configuration group for the group code
                if (Session["FuzzySearchAllOwnerList"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllOwnerList", out errorId);
                    Session["FuzzySearchAllOwnerList"] = dsList.Tables[0];
                }
                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchAllOwnerList"]).Select("owner_code = '" + ownerCode + "'");
                txtOwnerSearch.Text = filteredRow[0]["owner"].ToString();
                LoadOwnerAuditData(ownerCode);
            }
        }

        private void LoadOwnerAuditData(string ownerCode)
        {
            hdnIsValidSearch.Value = "Y";

            ownerAuditBL = new OwnerAuditBL();
            DataSet ownerData = ownerAuditBL.GetOwnerAuditData(ownerCode, out errorId);
            ownerAuditBL = null;

            if (ownerData.Tables.Count != 0 && errorId != 2)
            {
                ownerData.Tables[0].Columns.Add("owner_name_color");
                Session["OwnerAuditData"] = ownerData.Tables[0];
                BindGrid(ownerData.Tables[0]);
                
            }
            else
            {
                ExceptionHandler("Error in fetching ownerdata", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            tdData.Style.Remove("display");
            if (gridData.Rows.Count > 0)
            {
                gvOwnerDetails.DataSource = gridData;
                gvOwnerDetails.DataBind();
                CompareRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvOwnerDetails.DataSource = dtEmpty;
                gvOwnerDetails.DataBind();
            }
        }

        private void CompareRows()
        {
            DataTable dtOwnerData = (DataTable)Session["OwnerAuditData"];
            for (int i = 0; i < gvOwnerDetails.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvOwnerDetails.Rows[i];
                GridViewRow nextRow = gvOwnerDetails.Rows[i + 1];

                //Comapre Owner Name
                if ((currentRow.FindControl("lblOwnerDesc") as Label).Text != (nextRow.FindControl("lblOwnerDesc") as Label).Text)
                {
                    (currentRow.FindControl("lblOwnerDesc") as Label).ForeColor = Color.Red;
                    dtOwnerData.Rows[i]["owner_name_color"] = "R";

                }

            }
            Session["OwnerAuditData"] = dtOwnerData;
        }

        private void FuzzySearchOwner()
        {
            if (txtOwnerSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text owner search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllOwnerList(txtOwnerSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        #endregion Methods





    }
}