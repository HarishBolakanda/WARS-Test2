/*
File Name   :   RoyaltorActivity.ascx.cs
Purpose     :   Royaltoy Activity screen

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     18-Jul-2016     Harish(Infosys Limited)   Initial Creation
2.0     20-Apr-2017     Pratik(Infosys Limited)   Copied from v13
3.0     22-Aug-2018     Harish                    WUIN-609 - Modified Message pop up functionality
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
using System.Configuration;


namespace WARS
{
    public partial class RoyaltorActivity : System.Web.UI.UserControl
    {
        #region Global Declarations
        RoyaltorActivityBL royaltorActivityBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize2"].ToString());
        //int gridDefaultPageSize = 10; // Testing Purpose only
        #endregion Global Declarations

        #region PAGE EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {
            //WUIN-609 - Explicitly adding the page load event for this user control
            var script = "Sys.Application.add_init(pageLoadRoyActivity); Sys.WebForms.PageRequestManager.getInstance().add_endRequest(pageLoadRoyActivity)";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "thisControlPageLoad", script, true);
            UserAuthorization();
        }

        protected void btnClosePopupAct_Click(object sender, ImageClickEventArgs e)
        {
            if ((this.Parent.FindControl("hdnIsRecreateStmtUpdated") as HiddenField) != null)
            {
                if (hdnIsActivityChanged.Value != "" || (this.Parent.FindControl("hdnIsRecreateStmtUpdated") as HiddenField).Value != "")
                {
                    this.Page.GetType().InvokeMember("RefreshGridData", System.Reflection.BindingFlags.InvokeMethod, null, this.Page, null);
                }
            }
            mpeRoyActivity.Hide();
        }

        protected void btnRemove_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
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
                mpeRoyActivity.Show();

                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtEmpty, gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else
                {
                    util = new Utilities();
                    util.GenericExceptionHandler("Error in loading the activity screen grid data.");
                    util = null;
                }

                hdnIsActivityChanged.Value = "Y";


            }
            catch (Exception ex)
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in removing a royaltor/owner group from next run - " + ex.Message);
                util = null;
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
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    btnRemove = (ImageButton)e.Row.FindControl("btnRemove");
                    lblCanBeRemoved = (Label)e.Row.FindControl("lblCanBeRemoved");
                    btnRetry = (ImageButton)e.Row.FindControl("btnRetry");
                    lblRetry = (Label)e.Row.FindControl("lblRetry");

                    if (lblCanBeRemoved.Text == "Y")
                        btnRemove.Visible = true;
                    else
                        btnRemove.Visible = false;

                    if (lblRetry.Text == "E")
                        btnRetry.Visible = true;
                    else
                        btnRetry.Visible = false;

                }
            }
            catch (Exception ex)
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in binding grid data - " + ex.Message);
                util = null;
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData();
                mpeRoyActivity.Show();
            }
            catch (Exception ex)
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in refreshing data - " + ex.Message);
                util = null;
            }
        }

        protected void btnRoyaltyEngine_Click(object sender, EventArgs e)
        {
            try
            {
                string message;

                CommonBL commonBl = new CommonBL();
                commonBl.RunRotaltyEngine("Full", out message, out errorId); //JIRA-1133 Changes
                commonBl = null;

                if (errorId == 0)
                {
                    lblMessageRoyActivity.Text = message;
                    mpeRoyActivityMsgPopup.Show();
                    mpeRoyActivity.Show();
                }
                else if (errorId == 2)
                {
                    lblMessageRoyActivity.Text = "Error in triggering royalty engine";
                    mpeRoyActivityMsgPopup.Show();
                    mpeRoyActivity.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in triggering royalty engine.", ex.Message);
            }
        }

        protected void btnRetry_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
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
                mpeRoyActivity.Show();

                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);

                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtEmpty, gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
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
                //JIRA-1133 Changes by Ravi on 03-10-2019 -- Start
                mpeRoyActivity.Show();
                //JIRA-1133 Changes by Ravi on 03-10-2019 -- End
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change", ex.Message);
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
                //JIRA-1133 changes made by Harshika on 09-01-2020 --start
                mpeRoyActivity.Show();
                //JIRA-1133 changes made by Harshika on 09-01-2020 --end

                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);

                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtEmpty, gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
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

                    if (retry == "E" || detailFlag == "E" )
                    {
                        levelFlag = row["level_flag"].ToString();
                        code = row["code"].ToString();
                        stmtPeriodId = row["stmt_period_id"].ToString();
                        stmtsToRetry.Add(levelFlag + "," + code + "," + stmtPeriodId + "," + retry + "," + detailFlag );
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
                //JIRA-1133 changes made by Harshika on 09-01-2020 --start
                mpeRoyActivity.Show();
                //JIRA-1133 changes made by Harshika on 09-01-2020 --end

                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);

                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtEmpty, gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
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

        #endregion PAGE EVENTS

        #region METHODS

        private void UserAuthorization()
        {

            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnRefresh.Enabled = false;
                btnRemoveAll.Enabled = false;
                btnRoyaltyEngine.Enabled = false;
                foreach (GridViewRow rows in gvRoyActivity.Rows)
                {
                    (rows.FindControl("btnRemove") as ImageButton).Enabled = false;
                    (rows.FindControl("btnRetry") as ImageButton).Enabled = false;
                }
            }

        }

        public void PopupScreen(string popUpHeight)
        {
            try
            {
                hdnIsActivityChanged.Value = "";
                //double popUpHt = (Convert.ToDouble(popUpHeight) / 0.45) * 0.6;//Harish:10-08-16 - modified to solve PB overlapping issue
                double popUpHt = (Convert.ToDouble(popUpHeight) / 0.45) * 0.45;
                //set gridview panel height                    
                PnlActivity.Style.Add("height", (popUpHt - 100).ToString());
                gvRoyActivity.PageIndex = 0;
                LoadData();
                mpeRoyActivity.Show();

            }
            catch (Exception ex)
            {
                util = new Utilities();
                util.GenericExceptionHandler("Error in opening Next Run Statement Activity popup - " + ex.Message);
                util = null;
            }

        }

        private void LoadData()
        {
            try
            {
                hdnPageNumber.Value = "1";
                royaltorActivityBL = new RoyaltorActivityBL();
                /* WOS-389 - changes - removed by Harish
                DataSet activityData = royaltorActivityBL.GetActivityData(out nextRunTime, out errorId);*/
                DataSet activityData = royaltorActivityBL.GetActivityData(out errorId);
                royaltorActivityBL = null;

                if (activityData.Tables.Count != 0 && errorId != 2)
                {
                    Session["RoyActivityData"] = activityData.Tables[0];
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), activityData.Tables[0], gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
                }
                else if (activityData.Tables.Count == 0 && errorId != 2)
                {
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtEmpty, gridDefaultPageSize, gvRoyActivity, dtEmpty, rptPager);
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

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        #endregion METHODS

    }
}