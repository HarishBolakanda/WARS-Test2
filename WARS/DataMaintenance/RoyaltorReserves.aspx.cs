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

namespace WARS
{
    public partial class RoyaltorReserves : System.Web.UI.Page
    {
        #region Global Declarations
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        RoyaltorReservesBL royaltorReserveBL;
        Int32 royaltorId;
        string userCode;
        string isNewRoyaltor = string.Empty;
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
                royaltorId = Convert.ToInt32(Request.QueryString["RoyaltorId"]);
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];               

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Reserves";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Reserves";
                }

                userCode = Convert.ToString(Session["UserCode"]);
                PnlRsvGrid.Style.Add("height", hdnRsvGridPnlHeight.Value);
                PnlBalGrid.Style.Add("height", hdnBalGridPnlHeight.Value);
                //txtRoyaltor.Focus();//tabbing sequence starts here                    
                lblTab.Focus();

                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        HiddenField hdnBtnTerritoryGrp = (HiddenField)contractHdrNavigation.FindControl("hdnBtnTerritoryGrp");
                        HiddenField hdnBtnConfigGrp = (HiddenField)contractHdrNavigation.FindControl("hdnBtnConfigGrp");  

                        //WUIN-599 - resetting the flags
                        ((HiddenField)Master.FindControl("hdnIsContractScreen")).Value = "N";
                        ((HiddenField)Master.FindControl("hdnIsNotContractScreen")).Value = "N";
                        this.Master.FindControl("lnkBtnHome").Visible = false;

                        LoadInitialGridData();
                        imgBtnAddRsvRow.Enabled = false;
                        lblTab.Focus();//tabbing sequence starts here

                        if (Convert.ToInt32(royaltorId) == 0)
                        {
                            //displayed from main menu(not contract menu) 
                            tdConNavBtns.Visible = false;
                            hdnBtnTerritoryGrp.Value = "N";
                            hdnBtnConfigGrp.Value = "N";
                            hdnIsRoyaltorNull.Value = "Y";
                            txtRoyaltor.ReadOnly = false;
                            txtRoyaltor.Font.Bold = false;
                            txtRoyaltor.CssClass = "textboxStyle";
                            fuzzySearchRoyaltor.Visible = true;
                        }
                        else
                        {
                            //displayed from contract main menu 
                            Button btnBalResvHistory = (Button)contractNavigationButtons.FindControl("btnBalResvHistory");
                            btnBalResvHistory.Enabled = false;
                            HiddenField hdnRoyaltorId = (HiddenField)contractNavigationButtons.FindControl("hdnRoyaltorId");
                            hdnRoyaltorId.Value = Convert.ToString(royaltorId);
                            HiddenField hdnRoyaltorIdHdr = (HiddenField)contractHdrNavigation.FindControl("hdnRoyaltorId");
                            hdnRoyaltorIdHdr.Value = Convert.ToString(royaltorId);

                            fuzzySearchRoyaltor.Visible = false;
                            txtRoyaltor.Text = Convert.ToString(royaltorId);
                            LoadGridData(royaltorId);

                            if (isNewRoyaltor == "Y")
                            {
                                contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.BalResvHistory.ToString());
                            }

                            //WUIN-599 -- Only one user can use contract screens at the same time.
                            // If a contract is already using by another user then making the screen readonly.
                            if (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(Convert.ToString(royaltorId)))
                            {
                                hdnOtherUserScreenLocked.Value = "Y";
                            }

                            //WUIN-1096 - Only Read access for ReadonlyUser
                            //WUIN-599 If a contract is already using by another user then making the screen readonly.
                            //WUIN-450 -Only Read access for locked contracts
                            if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()) ||
                                (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(Convert.ToString(royaltorId))) ||
                                (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(Convert.ToString(royaltorId))))
                            {
                                EnableReadonly();
                            }
                        }               
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

        protected void btnYesSearch_Click(object sender, EventArgs e)
        {
            try
            {
                royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));
                //LoadGridData(Convert.ToInt32(txtRoy.Text.Trim()));
                LoadGridData(royaltorId);

                if (gvRoyBal.Rows.Count > 0)
                {
                    if (Session["UserRole"].ToString().ToLower() == UserRole.SuperUser.ToString().ToLower())
                    {
                        imgBtnAddRsvRow.Enabled = true;
                    }
                    PnlBalGrid.Style.Add("height", hdnBalGridPnlHeight.Value);
                }
                else
                {
                    imgBtnAddRsvRow.Enabled = false;
                }

                if (gvRoyRsv.Rows.Count > 0)
                {
                    PnlRsvGrid.Style.Add("height", hdnRsvGridPnlHeight.Value);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }

        }

        protected void txtRoyaltor_TextChanged(object sender, EventArgs e)
        {
            try
            {

                if (hdnRoySearchSelected.Value == "Y")
                {
                    RoyaltorSelected();
                }
                else if (hdnRoySearchSelected.Value == "N")
                {
                    FuzzySearchRoyaltor();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnRsvGridPnlHeight.Value == string.Empty ? "300" : hdnRsvGridPnlHeight.Value).ToString());

                }
                else if (hdnRoySearchSelected.Value == string.Empty)
                {
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialGridData();
                    imgBtnAddRsvRow.Enabled = false;
                }

                hdnRoySearchSelected.Value = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting Royaltor", ex.Message);
            }
        }


        protected void gvRoyBal_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void gvRoyRsv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                DataTable statementPeriods = Session["stmtPeriod"] as DataTable;
                DropDownList ddlRsvPeriod;
                DropDownList ddlLqdPeriod;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //WOS-364 - added by harish
                    if (Session["stmtPeriod"] == null)
                        return;

                    ddlRsvPeriod = (e.Row.FindControl("ddlRsvPeriod") as DropDownList);
                    ddlLqdPeriod = (e.Row.FindControl("ddlLqdPeriod") as DropDownList);

                    if (statementPeriods != null)
                    {
                        ddlRsvPeriod.DataSource = statementPeriods;
                        ddlRsvPeriod.DataTextField = "STMT_PERIOD";
                        ddlRsvPeriod.DataValueField = "STMT_PERIOD_END_VALUE";
                        ddlRsvPeriod.DataBind();
                        ddlRsvPeriod.Items.Insert(0, new ListItem("-"));

                        string rsv_period = (e.Row.FindControl("lblRsvPeriodValue") as Label).Text;
                        if (string.IsNullOrEmpty(rsv_period))
                        {
                            rsv_period = "-";
                            ddlRsvPeriod.Items.FindByText(rsv_period).Selected = true;
                        }
                        else
                        {
                            ddlRsvPeriod.Items.FindByValue(rsv_period).Selected = true;
                        }

                        ddlLqdPeriod.DataSource = statementPeriods;
                        ddlLqdPeriod.DataTextField = "STMT_PERIOD";
                        ddlLqdPeriod.DataValueField = "STMT_PERIOD_START_VALUE";
                        ddlLqdPeriod.DataBind();
                        ddlLqdPeriod.Items.Insert(0, new ListItem("-"));

                        string lqd_period = (e.Row.FindControl("lblLqdtPeriod") as Label).Text;
                        if (string.IsNullOrEmpty(lqd_period))
                        {
                            lqd_period = "-";
                            ddlLqdPeriod.Items.FindByText(lqd_period).Selected = true;
                        }
                        else
                        {
                            //lqd_period.Substring(0, lqd_period.LastIndexOf("_") - 1);
                            ddlLqdPeriod.Items.FindByValue(lqd_period).Selected = true;
                        }
                    }

                    int flag = Convert.ToInt32((e.Row.FindControl("lblflag") as Label).Text);

                    if (flag == 1)
                    {
                        (e.Row.FindControl("ddlLqdPeriod") as DropDownList).Visible = false;
                        (e.Row.FindControl("txtLqAmount") as TextBox).Visible = false;
                        (e.Row.FindControl("txtLqPeriod") as TextBox).Font.Bold = true;
                        (e.Row.FindControl("rfvStmtPeriod") as RequiredFieldValidator).Visible = false;
                        (e.Row.FindControl("rfvLqAmount") as RequiredFieldValidator).Visible = false;
                        (e.Row.FindControl("revLqAmount") as RegularExpressionValidator).Visible = false;

                        if (string.IsNullOrEmpty((e.Row.FindControl("lblRsvPeriodValue") as Label).Text))
                        {
                            (e.Row.FindControl("btnInsert") as ImageButton).Visible = false;
                            (e.Row.FindControl("btnDelete") as ImageButton).Visible = false;
                            (e.Row.FindControl("imgBtnCancel") as ImageButton).Visible = true;
                        }
                    }
                    else
                    {
                        (e.Row.FindControl("ddlRsvPeriod") as DropDownList).Visible = false;
                        (e.Row.FindControl("txtRsvTaken") as TextBox).Visible = false;
                        (e.Row.FindControl("btnInsert") as ImageButton).Visible = false;
                        (e.Row.FindControl("rfvResEdit") as RequiredFieldValidator).Visible = false;
                        (e.Row.FindControl("revRsvTaken") as RegularExpressionValidator).Visible = false;
                        if (string.IsNullOrEmpty((e.Row.FindControl("lblLqdtPeriod") as Label).Text))
                        {
                            (e.Row.FindControl("btnDelete") as ImageButton).Visible = false;
                            (e.Row.FindControl("imgBtnCancel") as ImageButton).Visible = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }
        }

        protected void gvRoyBal_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                royaltorReserveBL = new RoyaltorReservesBL();
                if (e.CommandName == "saverow")
                {
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = "";
                    hdnPreviousSelectedGrid.Value = "";
                    int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;

                    int stmt_period_id = Convert.ToInt32(((Label)gvRoyBal.Rows[rowIndex].FindControl("lblBalPeriodId")).Text);
                    double period_bal = Convert.ToDouble(((TextBox)gvRoyBal.Rows[rowIndex].FindControl("txtBalance")).Text);
                    //int royaltor_id = Convert.ToInt32(txtRoy.Text.Trim());
                    int royaltor_id = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));

                    DataSet royBalData = royaltorReserveBL.UpdateRoyaltorBalData(royaltor_id, stmt_period_id, period_bal, userCode, out errorId);
                    BindBalanceGrid(royBalData);

                }
                
                royaltorReserveBL = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving/deleting royaltor balance data.", ex.Message);
            }
        }

        protected void gvRoyRsv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                royaltorReserveBL = new RoyaltorReservesBL();
                DataSet royRsvData = null;
                if (e.CommandName == "saverow")
                {
                    //hdnChangeNotSaved.Value = "N";
                    //hdnGridRowSelectedPrvious.Value = "";
                    //hdnPreviousSelectedGrid.Value = "";
                    int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                    //int royaltor_id = Convert.ToInt32(txtRoy.Text.Trim());
                    int royaltor_id = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));

                    int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblflag")).Text);
                    if (flag == 1)
                    {
                        string old_rsv_period_value = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblRsvPeriodValue")).Text;
                        string new_rsv_period_value = ((DropDownList)gvRoyRsv.Rows[rowIndex].FindControl("ddlRsvPeriod")).SelectedValue;
                        int new_rsv_period_id = Convert.ToInt32(new_rsv_period_value.Substring(0, new_rsv_period_value.IndexOf("_")));
                        int liqInterval = Convert.ToInt32(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtLqPeriod")).Text);
                        double rsv_taken = Convert.ToDouble(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtRsvTaken")).Text);
                        if (string.IsNullOrEmpty(old_rsv_period_value))
                        {
                            if (ValidateNewRsvPeriod(new_rsv_period_value))
                            {
                                hdnChangeNotSaved.Value = "N";
                                hdnGridRowSelectedPrvious.Value = "";
                                hdnPreviousSelectedGrid.Value = "";

                                royRsvData = royaltorReserveBL.InsertRoyaltorRsvData(royaltor_id, new_rsv_period_id, liqInterval, rsv_taken, userCode, out errorId);
                                BindReserveGrid(royRsvData);
                            }
                            else
                            {
                                msgView.SetMessage("Selected reserve period already exists for the selected royaltor", MessageType.Warning, PositionType.Auto);
                            }
                        }
                        else
                        {
                            if (ValidateRsvPeriod(old_rsv_period_value, new_rsv_period_value))
                            {
                                if (ValidateUpdatedRsvPeriod(new_rsv_period_value, rowIndex))
                                {
                                    if (ValidateRsvAmount(old_rsv_period_value))
                                    {
                                        hdnChangeNotSaved.Value = "N";
                                        hdnGridRowSelectedPrvious.Value = "";
                                        hdnPreviousSelectedGrid.Value = "";

                                        int old_rsv_period_id = Convert.ToInt32(old_rsv_period_value.Substring(0, old_rsv_period_value.IndexOf("_")));
                                        royRsvData = royaltorReserveBL.UpdateRoyaltorRsvData(royaltor_id, old_rsv_period_id, new_rsv_period_id, liqInterval, rsv_taken, userCode, out errorId);
                                        BindReserveGrid(royRsvData);
                                    }
                                    else
                                    {
                                        msgView.SetMessage("Reserve amount can not be less than sum of liquidated amounts", MessageType.Warning, PositionType.Auto);
                                    }
                                }
                                else
                                {
                                    msgView.SetMessage("Selected reserve period already exists for the selected royaltor", MessageType.Warning, PositionType.Auto);
                                }
                            }
                            else
                            {
                                msgView.SetMessage("Period reserve released can not be prior/same as period reserve taken", MessageType.Warning, PositionType.Auto);
                            }
                        }


                    }
                    else
                    {
                        string rsv_period_value = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblRsvPeriodValue")).Text;
                        int rsv_period_id = Convert.ToInt32(rsv_period_value.Substring(0, rsv_period_value.IndexOf("_")));
                        int rsv_period_end_month = Convert.ToInt32(rsv_period_value.Substring(rsv_period_value.IndexOf("_") + 1));
                        string old_lqd_period = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblLqdtPeriod")).Text;
                        //to change
                        string new_lqd_period = ((DropDownList)gvRoyRsv.Rows[rowIndex].FindControl("ddlLqdPeriod")).SelectedValue;
                        int new_lqd_period_end_month = Convert.ToInt32(new_lqd_period.Substring(new_lqd_period.LastIndexOf("_") + 1));
                        new_lqd_period.Substring(0, new_lqd_period.LastIndexOf("_") - 1);
                        int new_lqd_period_id = Convert.ToInt32(new_lqd_period.Substring(0, new_lqd_period.IndexOf("_")));
                        //int new_lqd_period_start_month = Convert.ToInt32(new_lqd_period.Substring(new_lqd_period.IndexOf("_") + 1));
                        //int lqd_interval = Convert.ToInt32(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtLqdInterval")).Text);
                        int lqd_interval = Convert.ToInt32(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtLqPeriod")).Text);
                        double lqd_amount = Convert.ToDouble(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtLqAmount")).Text);

                        if (new_lqd_period_end_month > rsv_period_end_month)
                        {
                            if (ValidateLqdAmount(rsv_period_value, rowIndex))
                            {

                                if (string.IsNullOrEmpty(old_lqd_period))
                                {
                                    if (ValidateNewLQdPeriod(rsv_period_value, new_lqd_period))
                                    {
                                        hdnChangeNotSaved.Value = "N";
                                        hdnGridRowSelectedPrvious.Value = "";
                                        hdnPreviousSelectedGrid.Value = "";

                                        royRsvData = royaltorReserveBL.InsertRoyaltorLqdData(royaltor_id, rsv_period_id, new_lqd_period_id, lqd_interval, lqd_amount, userCode, out errorId);
                                        BindReserveGrid(royRsvData);
                                    }
                                    else
                                    {
                                        msgView.SetMessage("Selected reserve period released already exists for the reserve period taken", MessageType.Warning, PositionType.Auto);
                                    }
                                }
                                else
                                {
                                    int old_lqd_period_id = Convert.ToInt32(old_lqd_period.Substring(0, old_lqd_period.IndexOf("_")));
                                    if (ValidateUpdatedLQdPeriod(rsv_period_value, new_lqd_period, rowIndex))
                                    {
                                        hdnChangeNotSaved.Value = "N";
                                        hdnGridRowSelectedPrvious.Value = "";
                                        hdnPreviousSelectedGrid.Value = "";

                                        royRsvData = royaltorReserveBL.UpdateRoyaltorLqdData(royaltor_id, rsv_period_id, old_lqd_period_id, new_lqd_period_id, lqd_interval, lqd_amount, userCode, out errorId);
                                        BindReserveGrid(royRsvData);
                                    }
                                    else
                                    {
                                        msgView.SetMessage("Selected reserve period released already exists for the reserve period taken", MessageType.Warning, PositionType.Auto);
                                    }
                                }
                            }
                            else
                            {
                                msgView.SetMessage("Sum of liquidated amounts can not be greater than reserve amount", MessageType.Warning, PositionType.Auto);
                            }
                        }
                        else
                        {
                            msgView.SetMessage("Period reserve released can not be prior/same as period reserve taken", MessageType.Warning, PositionType.Auto);
                        }

                    }

                }
                else if (e.CommandName == "insertrow")
                {
                    DataTable rsvData = Session["rsvData"] as DataTable;
                    int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                    string rsv_period_value = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblRsvPeriodValue")).Text;
                    DataRow newRow = rsvData.NewRow();
                    newRow["RSV_PERIOD_VALUE"] = rsv_period_value;
                    newRow["flag"] = 2;
                    rsvData.Rows.InsertAt(newRow, rowIndex + 1);
                    Session["rsvData"] = rsvData;
                    gvRoyRsv.DataSource = rsvData;
                    gvRoyRsv.DataBind();

                    //for handling save/undo changes functionality
                    hdnPreviousSelectedGrid.Value = "Resv";
                    hdnGridRowSelectedPrvious.Value = (rowIndex + 1).ToString();
                    hdnChangeNotSaved.Value = "Y";

                }
                else if (e.CommandName == "cancelrow")
                {
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = "";
                    hdnPreviousSelectedGrid.Value = "";
                    DataTable rsvData = Session["rsvData"] as DataTable;
                    int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                    rsvData.Rows.RemoveAt(rowIndex);
                    Session["rsvData"] = rsvData;
                    gvRoyRsv.DataSource = rsvData;
                    gvRoyRsv.DataBind();
                }

                royaltorReserveBL = null;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving/deleting reserve data.", ex.Message);
            }
        }

        protected void imgBtnAddRsvRow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                DataTable rsvData = Session["rsvData"] as DataTable;
                if (rsvData.Rows.Count == 0)
                {
                    rsvData = new DataTable();
                    rsvData.Columns.Add("flag", typeof(int));
                    rsvData.Columns.Add("RSV_PERIOD_VALUE", typeof(string));
                    rsvData.Columns.Add("Period_Reserve_Taken", typeof(string));
                    rsvData.Columns.Add("Reserve_taken", typeof(double));
                    rsvData.Columns.Add("Liquidation_period", typeof(int));
                    rsvData.Columns.Add("liquidated_interval", typeof(int));
                    rsvData.Columns.Add("Liquidation_period_value", typeof(string));
                    rsvData.Columns.Add("Period_Reserve_Released", typeof(string));
                    rsvData.Columns.Add("Liquidated_Amount", typeof(double));
                    rsvData.Columns.Add("Reserve_Held_Balance", typeof(double));
                    DataRow newRow = rsvData.NewRow();
                    newRow["flag"] = 1;
                    newRow["RSV_PERIOD_VALUE"] = null;
                    rsvData.Rows.InsertAt(newRow, 0);
                }
                else
                {
                    DataRow newRow = rsvData.NewRow();
                    newRow["flag"] = 1;
                    newRow["RSV_PERIOD_VALUE"] = null;
                    rsvData.Rows.InsertAt(newRow, 0);
                }

                Session["rsvData"] = rsvData;//Harish
                gvRoyRsv.DataSource = rsvData;
                gvRoyRsv.DataBind();

                //for handling save/undo changes functionality
                hdnPreviousSelectedGrid.Value = "Resv";
                hdnGridRowSelectedPrvious.Value = "0";
                hdnChangeNotSaved.Value = "Y";
                imgBtnAddRsvRow.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding new row to reserve grid.", ex.Message);
            }
        }

        protected void BtnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));
                //LoadGridData(Convert.ToInt32(txtRoy.Text.Trim()));
                LoadGridData(royaltorId);

                if (gvRoyBal.Rows.Count > 0)
                {
                    if (Session["UserRole"].ToString().ToLower() == UserRole.SuperUser.ToString().ToLower())
                    {
                        imgBtnAddRsvRow.Enabled = true;
                    }
                    PnlBalGrid.Style.Add("height", hdnBalGridPnlHeight.Value);
                }
                else
                {
                    imgBtnAddRsvRow.Enabled = false;
                }

                if (gvRoyRsv.Rows.Count > 0)
                {
                    PnlRsvGrid.Style.Add("height", hdnRsvGridPnlHeight.Value);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvRoyBal_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["baldata"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvRoyBal.DataSource = dataView;
                gvRoyBal.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            string prevChangedGrid = hdnPreviousSelectedGrid.Value;
            int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
            royaltorReserveBL = new RoyaltorReservesBL();
            DataSet royRsvData = null;

            if (prevChangedGrid == "Bal")
            {
                //Validate
                Page.Validate();
                if (!Page.IsValid)
                {
                    mpeSaveUndo.Hide();
                    msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                    return;
                }

                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = "";
                hdnPreviousSelectedGrid.Value = "";

                int stmt_period_id = Convert.ToInt32(((Label)gvRoyBal.Rows[rowIndex].FindControl("lblBalPeriodId")).Text);
                double period_bal = Convert.ToDouble(((TextBox)gvRoyBal.Rows[rowIndex].FindControl("txtBalance")).Text);
                //int royaltor_id = Convert.ToInt32(txtRoy.Text.Trim());
                int royaltor_id = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));

                DataSet royBalData = royaltorReserveBL.UpdateRoyaltorBalData(royaltor_id, stmt_period_id, period_bal, userCode, out errorId);
                BindBalanceGrid(royBalData);
            }
            else if (prevChangedGrid == "Resv")
            {
                //Validate
                Page.Validate();
                if (!Page.IsValid)
                {
                    mpeSaveUndo.Hide();
                    msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                    return;
                }

                #region save reserves
                //int royaltor_id = Convert.ToInt32(txtRoy.Text.Trim());
                int royaltor_id = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));

                int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblflag")).Text);
                if (flag == 1)
                {
                    string old_rsv_period_value = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblRsvPeriodValue")).Text;
                    string new_rsv_period_value = ((DropDownList)gvRoyRsv.Rows[rowIndex].FindControl("ddlRsvPeriod")).SelectedValue;
                    int new_rsv_period_id = Convert.ToInt32(new_rsv_period_value.Substring(0, new_rsv_period_value.IndexOf("_")));
                    int liqInterval = Convert.ToInt32(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtLqPeriod")).Text);
                    double rsv_taken = Convert.ToDouble(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtRsvTaken")).Text);
                    if (string.IsNullOrEmpty(old_rsv_period_value))
                    {
                        if (ValidateNewRsvPeriod(new_rsv_period_value))
                        {
                            hdnChangeNotSaved.Value = "N";
                            hdnGridRowSelectedPrvious.Value = "";
                            hdnPreviousSelectedGrid.Value = "";

                            royRsvData = royaltorReserveBL.InsertRoyaltorRsvData(royaltor_id, new_rsv_period_id, liqInterval, rsv_taken, userCode, out errorId);
                            BindReserveGrid(royRsvData);
                        }
                        else
                        {
                            msgView.SetMessage("Selected reserve period already exists for the selected royaltor", MessageType.Warning, PositionType.Auto);
                        }
                    }
                    else
                    {
                        if (ValidateRsvPeriod(old_rsv_period_value, new_rsv_period_value))
                        {
                            if (ValidateUpdatedRsvPeriod(new_rsv_period_value, rowIndex))
                            {
                                if (ValidateRsvAmount(old_rsv_period_value))
                                {
                                    hdnChangeNotSaved.Value = "N";
                                    hdnGridRowSelectedPrvious.Value = "";
                                    hdnPreviousSelectedGrid.Value = "";

                                    int old_rsv_period_id = Convert.ToInt32(old_rsv_period_value.Substring(0, old_rsv_period_value.IndexOf("_")));
                                    royRsvData = royaltorReserveBL.UpdateRoyaltorRsvData(royaltor_id, old_rsv_period_id, new_rsv_period_id, liqInterval, rsv_taken, userCode, out errorId);
                                    BindReserveGrid(royRsvData);
                                }
                                else
                                {
                                    msgView.SetMessage("Reserve amount can not be less than sum of liquidated amounts", MessageType.Warning, PositionType.Auto);
                                }
                            }
                            else
                            {
                                msgView.SetMessage("Selected reserve period already exists for the selected royaltor", MessageType.Warning, PositionType.Auto);
                            }
                        }
                        else
                        {
                            msgView.SetMessage("Period reserve released can not be prior/same as period reserve taken", MessageType.Warning, PositionType.Auto);
                        }
                    }


                }
                else
                {
                    string rsv_period_value = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblRsvPeriodValue")).Text;
                    int rsv_period_id = Convert.ToInt32(rsv_period_value.Substring(0, rsv_period_value.IndexOf("_")));
                    int rsv_period_end_month = Convert.ToInt32(rsv_period_value.Substring(rsv_period_value.IndexOf("_") + 1));
                    string old_lqd_period = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblLqdtPeriod")).Text;
                    //to change
                    string new_lqd_period = ((DropDownList)gvRoyRsv.Rows[rowIndex].FindControl("ddlLqdPeriod")).SelectedValue;
                    int new_lqd_period_end_month = Convert.ToInt32(new_lqd_period.Substring(new_lqd_period.LastIndexOf("_") + 1));
                    new_lqd_period.Substring(0, new_lqd_period.LastIndexOf("_") - 1);
                    int new_lqd_period_id = Convert.ToInt32(new_lqd_period.Substring(0, new_lqd_period.IndexOf("_")));
                    //int new_lqd_period_start_month = Convert.ToInt32(new_lqd_period.Substring(new_lqd_period.IndexOf("_") + 1));
                    int lqd_interval = Convert.ToInt32(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtLqPeriod")).Text);
                    double lqd_amount = Convert.ToDouble(((TextBox)gvRoyRsv.Rows[rowIndex].FindControl("txtLqAmount")).Text);

                    if (new_lqd_period_end_month > rsv_period_end_month)
                    {
                        if (ValidateLqdAmount(rsv_period_value, rowIndex))
                        {

                            if (string.IsNullOrEmpty(old_lqd_period))
                            {
                                if (ValidateNewLQdPeriod(rsv_period_value, new_lqd_period))
                                {
                                    hdnChangeNotSaved.Value = "N";
                                    hdnGridRowSelectedPrvious.Value = "";
                                    hdnPreviousSelectedGrid.Value = "";

                                    royRsvData = royaltorReserveBL.InsertRoyaltorLqdData(royaltor_id, rsv_period_id, new_lqd_period_id, lqd_interval, lqd_amount, userCode, out errorId);
                                    BindReserveGrid(royRsvData);
                                }
                                else
                                {
                                    msgView.SetMessage("Selected reserve period released already exists for the reserve period taken", MessageType.Warning, PositionType.Auto);
                                }
                            }
                            else
                            {
                                int old_lqd_period_id = Convert.ToInt32(old_lqd_period.Substring(0, old_lqd_period.IndexOf("_")));
                                if (ValidateUpdatedLQdPeriod(rsv_period_value, new_lqd_period, rowIndex))
                                {
                                    hdnChangeNotSaved.Value = "N";
                                    hdnGridRowSelectedPrvious.Value = "";
                                    hdnPreviousSelectedGrid.Value = "";

                                    royRsvData = royaltorReserveBL.UpdateRoyaltorLqdData(royaltor_id, rsv_period_id, old_lqd_period_id, new_lqd_period_id, lqd_interval, lqd_amount, userCode, out errorId);
                                    BindReserveGrid(royRsvData);
                                }
                                else
                                {
                                    msgView.SetMessage("Selected reserve period released already exists for the reserve period taken", MessageType.Warning, PositionType.Auto);
                                }
                            }
                        }
                        else
                        {
                            msgView.SetMessage("Sum of liquidated amounts can not be greater than reserve amount", MessageType.Warning, PositionType.Auto);
                        }
                    }
                    else
                    {
                        msgView.SetMessage("Period reserve released can not be prior/same as period reserve taken", MessageType.Warning, PositionType.Auto);
                    }

                }
                #endregion save reserves
            }



        }

        protected void fuzzySearchRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltor();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnRsvGridPnlHeight.Value == string.Empty ? "300" : hdnRsvGridPnlHeight.Value).ToString());

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
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtRoyaltor.Text = string.Empty;
                    LoadInitialGridData();
                    return;
                }

                txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                RoyaltorSelected();
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
                mpeFuzzySearch.Hide();
                txtRoyaltor.Text = string.Empty;
                LoadInitialGridData();
                imgBtnAddRsvRow.Enabled = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing search list", ex.Message);
            }
        }

        //JIRA-908 CHanges by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet royRsvData = null;
                royaltorReserveBL = new RoyaltorReservesBL();
                if (hdnRsvDeleteRowIndex.Value != "")
                {
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = "";
                    hdnPreviousSelectedGrid.Value = "";

                    int rowIndex = Convert.ToInt16(hdnRsvDeleteRowIndex.Value);
                    int royaltor_id = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));
                    string rsv_period_value = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblRsvPeriodValue")).Text;
                    int rsv_period_id = Convert.ToInt32(rsv_period_value.Substring(0, rsv_period_value.IndexOf("_")));
                    string lqd_period = ((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblLqdtPeriod")).Text;
                    int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[rowIndex].FindControl("lblflag")).Text);

                    string lqd_period_value = ((DropDownList)gvRoyRsv.Rows[rowIndex].FindControl("ddlLqdPeriod")).SelectedValue;

                    if (flag == 1)
                    {
                        royRsvData = royaltorReserveBL.DeleteRoyaltorRsvData(royaltor_id, rsv_period_id, userCode, out errorId);
                        BindReserveGrid(royRsvData);
                    }
                    else
                    {   //WUIN-631-Handling null index scenario
                        lqd_period_value.Substring(0, lqd_period_value.LastIndexOf("_") - 1);
                        int lqd_period_id = Convert.ToInt32(lqd_period_value.Substring(0, lqd_period_value.IndexOf("_")));
                        royRsvData = royaltorReserveBL.DeleteRoyaltorLqdData(royaltor_id, rsv_period_id, lqd_period_id, userCode, out errorId);
                        BindReserveGrid(royRsvData);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving/deleting royaltor balance data.", ex.Message);
            }
        }
        //JIRA-908 CHanges by Ravi on 13/02/2019 -- End 

        #endregion EVENTS

        #region METHODS
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

            //util = new Utilities();
            //util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            //util = null;
        }

        private void LoadInitialGridData()
        {
            dtEmpty = new DataTable();
            gvRoyRsv.DataSource = dtEmpty;
            gvRoyRsv.EmptyDataText = "";
            gvRoyRsv.DataBind();
            gvRoyBal.DataSource = dtEmpty;
            gvRoyBal.EmptyDataText = "";
            gvRoyBal.DataBind();
        }

        //WUIN-572-Enabled role based access
        private void UserAuthorization()
        {
            if (Session["UserRole"].ToString().ToLower() != UserRole.SuperUser.ToString().ToLower())
            {
                EnableReadonly();

                if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
                {
                    btnSaveChanges.Enabled = false;
                }
            }
        }


        private bool ValidateSelectedRoyaltorFilter()
        {
            if (txtRoyaltor.Text != "" && Session["FuzzySearchAllRoyListWithOwnerCode"] != null)
            {
                if (txtRoyaltor.Text != "No results found")
                {
                    DataTable dtRoyaltorsList;
                    dtRoyaltorsList = Session["FuzzySearchAllRoyListWithOwnerCode"] as DataTable;

                    foreach (DataRow dRow in dtRoyaltorsList.Rows)
                    {
                        if (dRow["royaltor"].ToString() == txtRoyaltor.Text)
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



        private void LoadGridData(Int32 royaltorId)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            hdnChangeNotSaved.Value = "N";
            hdnGridRowSelectedPrvious.Value = "";
            hdnPreviousSelectedGrid.Value = "";

            if (royaltorId != 0)
            {
                //populate royaltor for the royaltor id
                if (Session["FuzzySearchAllRoyListWithOwnerCode"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllRoyaltorListWithOwnerCode", out errorId);
                    Session["FuzzySearchAllRoyListWithOwnerCode"] = dsList.Tables[0];
                }

                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchAllRoyListWithOwnerCode"]).Select("royaltor_id = '" + royaltorId + "'");
                txtRoyaltor.Text = filteredRow[0]["royaltor"].ToString();
            }

            royaltorReserveBL = new RoyaltorReservesBL();
            DataSet royData = royaltorReserveBL.GetRoyaltorData(royaltorId, out errorId);
            royaltorReserveBL = null;

            if (royData.Tables.Count != 0 && errorId != 2)
            {
                if (royData.Tables[2].Rows.Count > 0)
                {
                    Session["stmtPeriod"] = royData.Tables[2];
                }
                //WOS-364 - added by harish
                else
                {
                    Session["stmtPeriod"] = null;
                }
                //stmtPeriod = royData.Tables[2];
                //Session["stmtPeriod"] = stmtPeriod;
                Session["rsvdata"] = royData.Tables[1];
                Session["baldata"] = royData.Tables[0];
                gvRoyBal.DataSource = royData.Tables[0];
                if (royData.Tables[0].Rows.Count == 0)
                {
                    gvRoyBal.EmptyDataText = "No data found for the selected royaltor.";
                }
                gvRoyBal.DataBind();

                gvRoyRsv.DataSource = royData.Tables[1];
                if (royData.Tables[1].Rows.Count == 0)
                {
                    gvRoyRsv.EmptyDataText = "No data found for the selected royaltor.";
                }
                gvRoyRsv.DataBind();

            }
            else if (royData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyBal.DataSource = dtEmpty;
                gvRoyBal.EmptyDataText = "No data found for the selected royaltor.";
                gvRoyBal.DataBind();
                gvRoyRsv.DataSource = dtEmpty;
                gvRoyRsv.EmptyDataText = "No data found for the selected royaltor.";
                gvRoyRsv.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }
            UserAuthorization();
        }

        private Boolean ValidateLqdAmount(string rsvPeriodValue, int rowIndex)
        {
            double rsvAmount = 0;
            double lqdAmount = 0;
            double lqdAmountSumm = 0;
            for (int i = 0; i < gvRoyRsv.Rows.Count; i++)
            {
                string rsv_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblRsvPeriodValue")).Text;
                int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[i].FindControl("lblflag")).Text);
                if (rsv_period_value == rsvPeriodValue)
                {
                    if (flag == 1)
                    {
                        rsvAmount = Convert.ToDouble(((Label)gvRoyRsv.Rows[i].FindControl("lblRsvTaken")).Text);
                    }
                    else
                    {
                        if (i == rowIndex)
                        {
                            lqdAmount = Convert.ToDouble(((TextBox)gvRoyRsv.Rows[i].FindControl("txtLqAmount")).Text);
                        }
                        else
                        {
                            //modified by Harish - 25-08-16
                            //lqdAmount = Convert.ToDouble(((Label)gvRoyRsv.Rows[i].FindControl("lblLqdAmount")).Text);
                            lqdAmount = ((Label)gvRoyRsv.Rows[i].FindControl("lblLqdAmount")).Text == "" ? 0 : Convert.ToDouble(((Label)gvRoyRsv.Rows[i].FindControl("lblLqdAmount")).Text);
                        }

                        lqdAmountSumm = lqdAmountSumm + lqdAmount;
                    }
                }

                if (rsvAmount < lqdAmountSumm)
                {
                    return false;
                }
            }
            //if (rsvAmount >= lqdAmountSumm)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            return true;
        }

        private Boolean ValidateRsvAmount(string rsvPeriodValue)
        {
            double rsvAmount = 0;
            double lqdAmount = 0;
            double lqdAmountSumm = 0;

            for (int i = 0; i < gvRoyRsv.Rows.Count; i++)
            {
                string rsv_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblRsvPeriodValue")).Text;
                int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[i].FindControl("lblflag")).Text);
                if (rsv_period_value == rsvPeriodValue)
                {
                    if (flag == 1)
                    {
                        rsvAmount = Convert.ToDouble(((TextBox)gvRoyRsv.Rows[i].FindControl("txtRsvTaken")).Text);
                    }
                    else
                    {
                        lqdAmount = Convert.ToDouble(((Label)gvRoyRsv.Rows[i].FindControl("lblLqdAmount")).Text);
                        lqdAmountSumm = lqdAmountSumm + lqdAmount;
                    }
                }

                if (rsvAmount < lqdAmountSumm)
                {
                    return false;
                }

            }
            //if (rsvAmount >= lqdAmountSumm)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            return true;
        }

        private Boolean ValidateRsvPeriod(string oldRsvPeriodValue, string newRsvPeriodValue)
        {
            int count = 0;
            for (int i = 0; i < gvRoyRsv.Rows.Count; i++)
            {
                int lqd_period_start_month = 0;
                string rsv_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblRsvPeriodValue")).Text;
                int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[i].FindControl("lblflag")).Text);
                if (rsv_period_value == oldRsvPeriodValue)
                {
                    int rsv_period_end_month = Convert.ToInt32(newRsvPeriodValue.Substring(newRsvPeriodValue.IndexOf("_") + 1));

                    if (flag == 2)
                    {   //WUIN-631 changes
                        //string lqd_period_value = ((DropDownList)gvRoyRsv.Rows[i].FindControl("ddlLqdPeriod")).SelectedValue;
                        string lqd_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblLqdtPeriod")).Text;
                        if (lqd_period_value != string.Empty)
                        {
                            int lastIndex = lqd_period_value.LastIndexOf("_") - 1;
                            int firstIndex = lqd_period_value.IndexOf("_");
                            lqd_period_start_month = Convert.ToInt32(lqd_period_value.Substring(firstIndex + 1, (lastIndex - firstIndex)));
                        }

                        if (rsv_period_end_month >= lqd_period_start_month)
                        {
                            count++;
                            return false;
                        }
                    }
                }
            }
            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean ValidateNewRsvPeriod(string newRsvPeriodValue)
        {
            int count = 0;
            for (int i = 0; i < gvRoyRsv.Rows.Count; i++)
            {
                string rsv_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblRsvPeriodValue")).Text;
                int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[i].FindControl("lblflag")).Text);
                if (flag == 1)
                {
                    if (rsv_period_value == newRsvPeriodValue)
                    {
                        count++;
                        return false;
                    }
                }
            }
            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean ValidateUpdatedRsvPeriod(string newRsvPeriodValue, int rowIndex)
        {
            int count = 0;
            for (int i = 0; i < gvRoyRsv.Rows.Count; i++)
            {
                string rsv_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblRsvPeriodValue")).Text;
                int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[i].FindControl("lblflag")).Text);
                if (flag == 1)
                {
                    if (i != rowIndex)
                    {
                        if (rsv_period_value == newRsvPeriodValue)
                        {
                            count++;
                            return false;
                        }
                    }
                }
            }
            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean ValidateNewLQdPeriod(string rsvPeriodValue, string newLqdPeriodValue)
        {
            int count = 0;
            for (int i = 0; i < gvRoyRsv.Rows.Count; i++)
            {
                string rsv_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblRsvPeriodValue")).Text;
                string lqd_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblLqdtPeriod")).Text;
                int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[i].FindControl("lblflag")).Text);
                if (flag == 2)
                {
                    if (rsv_period_value == rsvPeriodValue)
                    {
                        if (lqd_period_value == newLqdPeriodValue)
                        {
                            count++;
                            return false;
                        }
                    }
                }
            }
            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean ValidateUpdatedLQdPeriod(string rsvPeriodValue, string newLqdPeriodValue, int rowIndex)
        {
            int count = 0;
            for (int i = 0; i < gvRoyRsv.Rows.Count; i++)
            {
                string rsv_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblRsvPeriodValue")).Text;
                string lqd_period_value = ((Label)gvRoyRsv.Rows[i].FindControl("lblLqdtPeriod")).Text;
                int flag = Convert.ToInt32(((Label)gvRoyRsv.Rows[i].FindControl("lblflag")).Text);
                if (flag == 2)
                {
                    if (i != rowIndex)
                    {
                        if (rsv_period_value == rsvPeriodValue)
                        {
                            if (lqd_period_value == newLqdPeriodValue)
                            {
                                count++;
                                return false;
                            }
                        }
                    }
                }
            }
            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BindReserveGrid(DataSet royRsvData)
        {
            if (royRsvData.Tables.Count != 0 && errorId != 2)
            {
                Session["rsvdata"] = royRsvData.Tables[0];
                gvRoyRsv.DataSource = royRsvData.Tables[0];
                if (royRsvData.Tables[0].Rows.Count == 0)
                {
                    gvRoyRsv.EmptyDataText = "No data found for the selected royaltor.";
                }
                gvRoyRsv.DataBind();
            }
            else if (royRsvData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyRsv.DataSource = dtEmpty;
                gvRoyRsv.EmptyDataText = "No data found for the selected royaltor.";
                gvRoyRsv.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading reserves grid data.", string.Empty);
            }
            UserAuthorization();
        }

        private void BindBalanceGrid(DataSet royBalData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (royBalData.Tables.Count != 0 && errorId != 2)
            {
                gvRoyBal.DataSource = royBalData.Tables[0];
                if (royBalData.Tables[0].Rows.Count == 0)
                {
                    gvRoyBal.EmptyDataText = "No data found for the selected royaltor.";
                }
                gvRoyBal.DataBind();

            }
            else if (royBalData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvRoyBal.DataSource = dtEmpty;
                gvRoyBal.EmptyDataText = "No data found for the selected royaltor.";
                gvRoyBal.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading balance grid data.", string.Empty);
            }
            UserAuthorization();
        }

        private void FuzzySearchRoyaltor()
        {

            if (txtRoyaltor.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                LoadInitialGridData();
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyListWithOwnerCode(txtRoyaltor.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void RoyaltorSelected()
        {
            if (ValidateSelectedRoyaltorFilter())
            {
                royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-") - 1));
                LoadGridData(royaltorId);

                if (gvRoyBal.Rows.Count > 0)
                {
                    if (Session["UserRole"].ToString().ToLower() == UserRole.SuperUser.ToString().ToLower())
                    {
                        imgBtnAddRsvRow.Enabled = true;
                    }
                    PnlBalGrid.Style.Add("height", hdnBalGridPnlHeight.Value);
                }
                else
                {
                    imgBtnAddRsvRow.Enabled = false;
                }

                if (gvRoyRsv.Rows.Count > 0)
                {
                    PnlRsvGrid.Style.Add("height", hdnRsvGridPnlHeight.Value);
                }
            }
            else
            {
                LoadInitialGridData();
                txtRoyaltor.Text = string.Empty;
                imgBtnAddRsvRow.Enabled = false;
                msgView.SetMessage("Please select a royaltor",
                        MessageType.Warning, PositionType.Auto);
            }
        }

        private void EnableReadonly()
        {
            imgBtnAddRsvRow.Enabled = false;
            foreach (GridViewRow rows in gvRoyBal.Rows)
            {
                (rows.FindControl("btnUpdate") as ImageButton).Enabled = false;
            }
            foreach (GridViewRow rows in gvRoyRsv.Rows)
            {
                (rows.FindControl("btnInsert") as ImageButton).Enabled = false;
                (rows.FindControl("btnUpdate") as ImageButton).Enabled = false;
                (rows.FindControl("btnDelete") as ImageButton).Enabled = false;
                (rows.FindControl("imgBtnCancel") as ImageButton).Enabled = false;
            }

        }

        #endregion METHODS

        #region Web Methods
                    
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static int UpdateScreenLockFlag()
            {
            int errorId;
            RoyaltorContractBL royContractBL = new RoyaltorContractBL();
            royContractBL.UpdateScreenLockFlag(HttpContext.Current.Session["ScreenLockedRoyaltorId"].ToString(), "N", HttpContext.Current.Session["UserCode"].ToString(), out errorId);
            royContractBL = null;
            return errorId;

        }

        #endregion Web Methods        

    }
}