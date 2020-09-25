
/*
File Name   :   RoyaltyActivity.cs
Purpose     :   Royaltoy Activity screen

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     18-Jul-2016     Harish(Infosys Limited)   Initial Creation
2.0     20-Apr-2017     Pratik(Infosys Limited)   Copied from v13
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
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Configuration;

namespace WARS
{
    public partial class RoyaltorActivity1 : System.Web.UI.Page
    {
        #region Global Declarations
        RoyaltorActivityBL royaltorActivityBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize2"].ToString());
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
            if (Session["DatabaseName"] != null)
            {
                this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Activity";
            }
            else
            {
                util = new Utilities();
                string dbName = util.GetDatabaseName();
                util = null;
                Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Activity";
            }

            try
            {
                lblTab.Focus(); //tabbing sequence starts here
                if (!IsPostBack)
                {
                    LoadData();
                }

                UserAuthorization();
                //set gridview panel height 
                PnlActivity.Style.Add("height", hdnGridPnlHeight.Value);
            }
            catch (ThreadAbortException ex1)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }

        protected void btnRemove_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                ImageButton btnRemove = (ImageButton)sender;
                GridViewRow gvr = ((ImageButton)sender).NamingContainer as GridViewRow;
                string levelFlag = (gvr.FindControl("hdnLevelFlag") as HiddenField).Value;
                string code = (gvr.FindControl("lblCode") as Label).Text;
                string stmtPeriodId = (gvr.FindControl("lblStmtPeriodId") as Label).Text;
                string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                royaltorActivityBL = new RoyaltorActivityBL();
                /* WOS-389 - changes - modified by Harish
                DataSet activityData = royaltorActivityBL.UpdateRemoveFromRun(levelFlag, code, loggedUserID, stmtPeriodId, out nextRunTime, out errorId);*/
                DataSet activityData = royaltorActivityBL.UpdateRemoveFromRun(levelFlag, code, loggedUserID, stmtPeriodId, out errorId);
                royaltorActivityBL = null;
                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else
                {
                    ExceptionHandler("Error in loading the activity screen grid data.", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing a royaltor/owner group from next run.", ex.Message);
            }
        }

        protected void gvRoyActivity_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                ImageButton btnRemove;
                ImageButton btnRetry;
                Label lblCanBeRemoved;
                Label lblRetry;
                string detailsFileFlag;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    btnRemove = (ImageButton)e.Row.FindControl("btnRemove");
                    lblCanBeRemoved = (Label)e.Row.FindControl("lblCanBeRemoved");
                    btnRetry = (ImageButton)e.Row.FindControl("btnRetry");
                    lblRetry = (Label)e.Row.FindControl("lblRetry");
                    detailsFileFlag = ((HiddenField)e.Row.FindControl("hdnDetailFlag")).Value;

                    if (lblCanBeRemoved.Text == "Y")
                        btnRemove.Visible = true;
                    else
                        btnRemove.Visible = false;

                    if (lblRetry.Text == "E" || detailsFileFlag == "E")
                        btnRetry.Visible = true;
                    else
                        btnRetry.Visible = false;

                }

                //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    foreach (TableCell tc in e.Row.Cells)
                    {
                        if (tc.HasControls() && tc.Controls.Count == 1)
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
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in refreshing data.", ex.Message);
            }
        }

        protected void btnRetry_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                hdnPageNumber.Value = "1";
                ImageButton btnRetry = (ImageButton)sender;
                GridViewRow gvr = ((ImageButton)sender).NamingContainer as GridViewRow;
                string levelFlag = (gvr.FindControl("hdnLevelFlag") as HiddenField).Value;
                string code = (gvr.FindControl("lblCode") as Label).Text;
                string stmtPeriodId = (gvr.FindControl("lblStmtPeriodId") as Label).Text;
                string retry = (gvr.FindControl("lblRetry") as Label).Text;
                string detailFlag = (gvr.FindControl("hdnDetailFlag") as HiddenField).Value;
                string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                royaltorActivityBL = new RoyaltorActivityBL();
                DataSet activityData = royaltorActivityBL.SetRetry(levelFlag, code, retry, detailFlag, loggedUserID, stmtPeriodId, out errorId);
                royaltorActivityBL = null;
                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else
                {
                    ExceptionHandler("Error in loading the activity screen grid data.", string.Empty);
                }


            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing a royaltor/owner group from next run.", ex.Message);
            }
        }
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["RoyActivityData"] == null)
                    return;

                DataTable activityData = Session["RoyActivityData"] as DataTable;
                if (activityData.Rows.Count == 0)
                    return;
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);
                Utilities.PopulateGridPage(pageIndex, activityData, gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                UserAuthorization();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        protected void btnRemoveAll_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
                List<string> stmtsToRemove = new List<string>();
                string levelFlag;
                string code;
                string stmtPeriodId;
                string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string canBeRemoved;

                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                //WUIN-38 
                if (Session["RoyActivityData"] == null)
                    return;

                DataTable dtRoyActivityData = Session["RoyActivityData"] as DataTable;
                if (dtRoyActivityData.Rows.Count == 0)
                    return;

                foreach (DataRow row in dtRoyActivityData.Rows)
                {
                    canBeRemoved = row["remove_from_run"].ToString();

                    if (canBeRemoved == "Y")
                    {
                        levelFlag = row["level_flag"].ToString();
                        code = row["code"].ToString();
                        stmtPeriodId = row["stmt_period_id"].ToString();
                        stmtsToRemove.Add(levelFlag + "," + code + "," + stmtPeriodId);
                    }

                }

                //foreach (GridViewRow row in gvRoyActivity.Rows)
                //{
                //    canBeRemoved = (row.FindControl("lblCanBeRemoved") as Label).Text;
                //    if (canBeRemoved == "Y")
                //    {
                //        levelFlag = (row.FindControl("hdnLevelFlag") as HiddenField).Value;
                //        code = (row.FindControl("lblCode") as Label).Text;
                //        stmtPeriodId = (row.FindControl("lblStmtPeriodId") as Label).Text;
                //        stmtsToRemove.Add(levelFlag + "," + code + "," + stmtPeriodId);
                //    }

                //}

                royaltorActivityBL = new RoyaltorActivityBL();
                DataSet activityData = royaltorActivityBL.RemoveAllFromRun(stmtsToRemove.ToArray(), loggedUserID, out errorId);
                royaltorActivityBL = null;
                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else
                {
                    ExceptionHandler("Error in loading the activity screen grid data.", string.Empty);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing  royaltor/owner groups from next run.", ex.Message);
            }

        }

        protected void btnRetryAll_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
                List<string> stmtsToRetry = new List<string>();
                string levelFlag;
                string code;
                string stmtPeriodId;
                string loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string retry;
                string detailFlag;


                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                //WUIN-38
                if (Session["RoyActivityData"] == null)
                    return;

                DataTable dtRoyActivityData = Session["RoyActivityData"] as DataTable;
                if (dtRoyActivityData.Rows.Count == 0)
                    return;

                foreach (DataRow row in dtRoyActivityData.Rows)
                {
                    retry = row["royaltor_stmt_flag"].ToString();
                    detailFlag = row["dtl_file_flag"].ToString();

                    if (retry == "E" || detailFlag == "E")
                    {
                        levelFlag = row["level_flag"].ToString();
                        code = row["code"].ToString();
                        stmtPeriodId = row["stmt_period_id"].ToString();
                        stmtsToRetry.Add(levelFlag + "," + code + "," + stmtPeriodId + "," + retry + "," + detailFlag);
                    }

                }

                //foreach (GridViewRow row in gvRoyActivity.Rows)
                //{
                //    retry = (row.FindControl("lblRetry") as Label).Text;
                //    if (retry == "E")
                //    {
                //        levelFlag = (row.FindControl("hdnLevelFlag") as HiddenField).Value;
                //        code = (row.FindControl("lblCode") as Label).Text;
                //        stmtPeriodId = (row.FindControl("lblStmtPeriodId") as Label).Text;
                //        stmtsToRetry.Add(levelFlag + "," + code + "," + stmtPeriodId);
                //    }

                //}

                royaltorActivityBL = new RoyaltorActivityBL();
                DataSet activityData = royaltorActivityBL.RetryAll(stmtsToRetry.ToArray(), loggedUserID, out errorId);
                royaltorActivityBL = null;
                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else
                {
                    ExceptionHandler("Error in loading the activity screen grid data.", string.Empty);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing  royaltor/owner groups from next run.", ex.Message);
            }
        }


        //JIRA-908 CHanges by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string message;
                CommonBL commonBl = new CommonBL();
                commonBl.RunRotaltyEngine("Full", out message, out errorId);
                commonBl = null;
                if (errorId == 0)
                {
                    msgView.SetMessage(message, MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    msgView.SetMessage("Error in triggering royalty engine.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in triggering royalty engine.", ex.Message);
            }
        }

        //JIRA-908 CHanges by Ravi on 13/02/2019 -- End

        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvRoyActivity_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyActivityData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                Utilities.PopulateGridPage(1, dataView.ToTable(), gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                Session["RoyActivityData"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion EVENTS

        #region METHODS
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnRoyaltyEngine.Enabled = false;
                btnRefresh.Enabled = false;
                (gvRoyActivity.Rows[0].FindControl("btnRemoveAll") as ImageButton).Enabled = false;
                (gvRoyActivity.Rows[0].FindControl("btnRetryAll") as ImageButton).Enabled = false;

                foreach (GridViewRow rows in gvRoyActivity.Rows)
                {
                    (rows.FindControl("btnRemove") as ImageButton).Enabled = false;
                    (rows.FindControl("btnRetry") as ImageButton).Enabled = false;
                }
            }

        }

        private void LoadData()
        {
            try
            {
                hdnPageNumber.Value = "1";
                royaltorActivityBL = new RoyaltorActivityBL();
                /* WOS-389 - changes - modified by Harish
                DataSet activityData = royaltorActivityBL.GetActivityData(out nextRunTime, out errorId);*/
                DataSet activityData = royaltorActivityBL.GetActivityData(out errorId);
                royaltorActivityBL = null;

                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(1, activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager); ;
                }
                else
                {
                    util = new Utilities();
                    util.GenericExceptionHandler("Error in loading the grid data.");
                    util = null;
                }

            }
            catch (Exception ex)
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in loading Next Run Statement Activity data - " + ex.Message);
                util = null;
            }

        }

        #endregion METHODS


    }
}