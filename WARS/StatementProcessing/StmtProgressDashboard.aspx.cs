/*
File Name   :   StmtProgressDashboard.cs
Purpose     :   to view statement progress statistics

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     10-Aug-2016     Harish(Infosys Limited)   Initial Creation
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


namespace WARS
{
    public partial class StmtProgressDashboard : System.Web.UI.Page
    {
        #region Global Declarations
        string loggedUserID;
        StmtProgressDashboardBL stmtProgressDashboarBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        #endregion Global Declarations

        #region EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Statement Progress Dashboard";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Statement Progress Dashboard";
                }

                if (IsPostBack)
                {
                    //set gridview panel height                    
                    PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);

                }

                txtEarnings.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadInitialGridData();
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;
                    tdGvRemainingDaysHdr.Visible = false;
                    tdGvRemainingDays.Visible = false;
                }

            }
            catch (ThreadAbortException ex1)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }
        }
        
        protected void gvStatements_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string resp;
                string status;
                string statusCode;
                string rowNum;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    resp = (e.Row.FindControl("lblresp") as Label).Text;
                    status = (e.Row.FindControl("lblStatus") as Label).Text;
                    statusCode = (e.Row.FindControl("hdnStatusCode") as HiddenField).Value;
                    rowNum = (e.Row.FindControl("hdnRowNum") as HiddenField).Value;
                    if (status != "" && resp == "" && rowNum == "1")//Group header row
                    {
                        if (status == "Total")
                        {
                            e.Row.Cells[0].CssClass = "dashBoardScrnTotalRow_Center_Align";
                            e.Row.Cells[1].CssClass = "dashBoardScrnTotalRow_Text";
                            e.Row.Cells[2].CssClass = "dashBoardScrnTotalRow_Center_Align";
                            e.Row.Cells[3].CssClass = "dashBoardScrnTotalRow_Center_Align";
                            e.Row.Cells[4].CssClass = "dashBoardScrnTotalRow_Center_Align";
                            e.Row.Cells[5].CssClass = "dashBoardScrnTotalRow_Center_Align";
                            e.Row.Cells[6].CssClass = "dashBoardScrnTotalRow_Right_Align";

                        }
                        else if (status == "Grand Total")
                        {
                            e.Row.Cells[0].CssClass = "dashBoardScrnGrandTotalRow_Center_Align";
                            e.Row.Cells[1].CssClass = "dashBoardScrnGrandTotalRow_Text";
                            e.Row.Cells[2].CssClass = "dashBoardScrnGrandTotalRow_Center_Align";
                            e.Row.Cells[3].CssClass = "dashBoardScrnGrandTotalRow_Center_Align";
                            e.Row.Cells[4].CssClass = "dashBoardScrnGrandTotalRow_Center_Align";
                            e.Row.Cells[5].CssClass = "dashBoardScrnGrandTotalRow_Center_Align";
                            e.Row.Cells[6].CssClass = "dashBoardScrnGrandTotalRow_Right_Align";

                        }
                        else
                        {
                            if (statusCode != "9992")//no need to have expand/collapse button for below threshold
                            {
                                HideUnHideToggleButtons(e.Row.Cells[0], true, false);//hide collapse button
                            }
                            //e.Row.Cells[1].CssClass = "dashBoardScrnGroupHeader";
                            e.Row.Cells[1].Font.Bold = true;
                            e.Row.Cells[2].Font.Bold = true;
                            e.Row.Cells[3].Font.Bold = true;
                            e.Row.Cells[4].Font.Bold = true;
                            e.Row.Cells[5].Font.Bold = true;
                            e.Row.Cells[6].Font.Bold = true;
                        }


                    }
                    else //Group child row
                    {
                        (e.Row.FindControl("lblStatus") as Label).Visible = false;
                        //e.Row.Visible = false;

                        if (statusCode == "9991.60")//dummy separator row
                        {

                            e.Row.Cells[1].BackColor = System.Drawing.Color.Black;
                            e.Row.Cells[2].BackColor = System.Drawing.Color.Black;
                            e.Row.Cells[3].BackColor = System.Drawing.Color.Black;
                            e.Row.Cells[4].BackColor = System.Drawing.Color.Black;
                            e.Row.Cells[5].BackColor = System.Drawing.Color.Black;
                            e.Row.Cells[6].BackColor = System.Drawing.Color.Black;

                        }
                        else
                        {
                            //hide child rows initially
                            e.Row.Visible = false;
                        }


                    }

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }
        }

        protected void gvStatements_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int currRowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvStatements.Rows[currRowIndex];
                string hdrStatusCode = (row.FindControl("hdnStatusCode") as HiddenField).Value;
                string statusCode;

                if (e.CommandName == "Expand")
                {
                    for (int i = currRowIndex + 1; i < gvStatements.Rows.Count; i++)
                    {
                        statusCode = (gvStatements.Rows[i].FindControl("hdnStatusCode") as HiddenField).Value;

                        if (hdrStatusCode == statusCode)
                        {
                            gvStatements.Rows[i].Visible = true;
                        }
                        else
                        {
                            //we have reached the end of the current group
                            //make expand image invisible and collapse image visible
                            HideUnHideToggleButtons(row.Cells[0], false, true);
                            break;
                        }

                    }
                }
                else if (e.CommandName == "Collapse")
                {
                    for (int i = currRowIndex + 1; i < gvStatements.Rows.Count; i++)
                    {
                        statusCode = (gvStatements.Rows[i].FindControl("hdnStatusCode") as HiddenField).Value;

                        if (hdrStatusCode == statusCode)
                        {
                            gvStatements.Rows[i].Visible = false;
                        }
                        else
                        {
                            //we have reached the end of the current group
                            //make expand image invisible and collapse image visible
                            HideUnHideToggleButtons(row.Cells[0], true, false);
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in expanding/collapsing group in grid.", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlCompany.SelectedIndex = -1;
            ddlReportingSch.SelectedIndex = -1;
            ddlTeamResponsibility.SelectedIndex = -1;
            ddlMngrResponsibility.SelectedIndex = -1;
            ddlEarningsCompare.SelectedIndex = -1;
            txtEarnings.Text = string.Empty;
            tdGvRemainingDays.Visible = false;
            tdGvRemainingDaysHdr.Visible = false;

            DataTable intialData = (DataTable)Session["StmtDashboardInitialData"];
            if (intialData.Rows.Count == 0)
            {
                gvStatements.DataSource = intialData;
                gvStatements.EmptyDataText = "No Data Found";
                gvStatements.DataBind();
            }
            else
            {
                gvStatements.DataSource = intialData;
                gvStatements.DataBind();
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (!Page.IsValid)
            {
                dtEmpty = new DataTable();
                gvStatements.DataSource = dtEmpty;
                gvStatements.EmptyDataText = "No Data Found";
                gvStatements.DataBind();

                tdGvRemainingDays.Visible = false;
                tdGvRemainingDaysHdr.Visible = false;
                return;
            }

            LoadFilterGridData();
        }

        #endregion EVENTS

        #region METHODS
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void LoadInitialGridData()
        {
            //set gridview panel height                    
            PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);

            stmtProgressDashboarBL = new StmtProgressDashboardBL();
            DataSet intialData = stmtProgressDashboarBL.GetInitialData(out errorId);
            stmtProgressDashboarBL = null;

            if (intialData.Tables.Count != 0 && errorId != 2)
            {
                ddlCompany.DataTextField = "com_name";
                ddlCompany.DataValueField = "com_number";
                ddlCompany.DataSource = intialData.Tables[0];
                ddlCompany.DataBind();
                ddlCompany.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));

                ddlReportingSch.DataTextField = "stmt_period";
                ddlReportingSch.DataValueField = "statement_period_id";
                ddlReportingSch.DataSource = intialData.Tables[1];
                ddlReportingSch.DataBind();
                ddlReportingSch.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));

                ddlTeamResponsibility.DataTextField = "responsibility";
                ddlTeamResponsibility.DataValueField = "responsibility_code";
                ddlTeamResponsibility.DataSource = intialData.Tables[2];
                ddlTeamResponsibility.DataBind();
                ddlTeamResponsibility.Items.Insert(0, new ListItem("-"));

                ddlMngrResponsibility.DataTextField = "responsibility";
                ddlMngrResponsibility.DataValueField = "responsibility_code";
                ddlMngrResponsibility.DataSource = intialData.Tables[3];
                ddlMngrResponsibility.DataBind();
                ddlMngrResponsibility.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-"));

                Session["StmtDashboardInitialData"] = intialData.Tables[4];
                if (intialData.Tables[4].Rows.Count == 0)
                {
                    gvStatements.DataSource = intialData.Tables[4];
                    gvStatements.EmptyDataText = "No Data Found";
                    gvStatements.DataBind();
                }
                else
                {
                    gvStatements.DataSource = intialData.Tables[4];
                    gvStatements.DataBind();
                }

            }
            else if (intialData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvStatements.DataSource = dtEmpty;
                gvStatements.EmptyDataText = "No Data Found";
                gvStatements.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading search data.", string.Empty);
            }
        }

        private void LoadFilterGridData()
        {
            //set gridview panel height                    
            PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);

            if (ddlCompany.SelectedValue != "-" || ddlReportingSch.SelectedValue != "-" || ddlTeamResponsibility.SelectedValue != "-" || ddlMngrResponsibility.SelectedValue != "-" || txtEarnings.Text != "")
            {
                if (ddlReportingSch.SelectedValue != "-")
                {
                    tdGvRemainingDays.Visible = true;
                    tdGvRemainingDaysHdr.Visible = true;
                }
                else
                {
                    tdGvRemainingDays.Visible = false;
                    tdGvRemainingDaysHdr.Visible = false;
                }

                stmtProgressDashboarBL = new StmtProgressDashboardBL();
                DataSet stmtData = stmtProgressDashboarBL.GetFilterData(ddlCompany.SelectedValue, ddlReportingSch.SelectedValue, ddlTeamResponsibility.SelectedValue, ddlMngrResponsibility.SelectedValue, ddlEarningsCompare.SelectedValue, txtEarnings.Text, out errorId);
                stmtProgressDashboarBL = null;

                if (stmtData.Tables.Count != 0 && errorId != 2)
                {
                    if (stmtData.Tables[0].Rows.Count == 0)
                    {
                        gvStatements.DataSource = stmtData.Tables[0];
                        gvStatements.EmptyDataText = "No Data Found";
                        gvStatements.DataBind();
                    }
                    else
                    {

                        gvStatements.DataSource = stmtData.Tables[0];
                        gvStatements.DataBind();
                    }

                    if (stmtData.Tables[1].Rows.Count == 0)
                    {
                        gvRemainingDays.DataSource = stmtData.Tables[1];
                        gvRemainingDays.EmptyDataText = "No Data Found";
                        gvRemainingDays.DataBind();
                    }
                    else
                    {
                        gvRemainingDays.DataSource = stmtData.Tables[1];
                        gvRemainingDays.DataBind();
                    }

                }
                else if (stmtData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvStatements.DataSource = dtEmpty;
                    gvStatements.EmptyDataText = "No Data Found";
                    gvStatements.DataBind();
                }
                else
                {
                    ExceptionHandler("Error in loading search data.", string.Empty);
                }
            }
            else
            {
                DataTable intialData = (DataTable)Session["StmtDashboardInitialData"];
                if (intialData.Rows.Count == 0)
                {
                    gvStatements.DataSource = intialData;
                    gvStatements.EmptyDataText = "No Data Found";
                    gvStatements.DataBind();
                }
                else
                {
                    gvStatements.DataSource = intialData;
                    gvStatements.DataBind();
                }
            }


        }

        private void HideUnHideToggleButtons(TableCell cell, bool hideCollapseButton, bool hideExpandButton)
        {
            ImageButton imgExpand = (ImageButton)cell.FindControl("imgExpand");//+
            imgExpand.Visible = !hideExpandButton;
            ImageButton imgCollapse = (ImageButton)cell.FindControl("imgCollapse");//-
            imgCollapse.Visible = !hideCollapseButton;
        }
        #endregion METHODS




    }
}