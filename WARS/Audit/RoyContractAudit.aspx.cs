/*
File Name   :   RoyContractAudit.cs
Purpose     :   to display Royaltor Contract and Reserve Audit data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     26-Apr-2017     Pratik(Infosys Limited)   Initial Creation
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

namespace WARS.Contract
{
    public partial class RoyContractAudit : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractAuditBL royContractAuditBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract Audit";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract Audit";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlRoyaltorAuditDetails.Style.Add("height", hdnGridPnlHeight.Value);
                pnlRoyaltorRsvAuditDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtRoyaltorSearch.Focus();
                    tdData.Style.Add("display", "none");
                    trRoyAudit.Style.Add("display", "none");
                    trRsvAudit.Style.Add("display", "none");
                    btnReserveAudit.Enabled = false;
                    btnContractAudit.Visible = false;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                       
                        LoadData();

                        this.Master.FindControl("lnkBtnHome").Visible = false; //WUIN-599 Hiding master page home button

                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        if (Convert.ToString(Session["UserCode"]) != Convert.ToString(Session["ContractScreenLockedUser"]) )
                        {
                            hdnOtherUserScreenLocked.Value = "Y";
                        }
                        
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

        protected void fuzzySearchRoyaltor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyaltor();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor search.", ex.Message);
            }
        }

        protected void btnReserveAudit_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    tdData.Style.Remove("display");
                    trRoyAudit.Style.Add("display", "none");
                    trRsvAudit.Style.Remove("display");
                    btnReserveAudit.Visible = false;
                    btnContractAudit.Visible = true;

                    royContractAuditBL = new RoyContractAuditBL();
                    DataSet auditData = royContractAuditBL.GetRoyReserveAuditData(Convert.ToInt32(txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim()), out errorId);
                    royContractAuditBL = null;

                    if (auditData.Tables.Count != 0 && errorId != 2)
                    {
                        Int32 id = 0;

                        //Datatable to hold the reserve audit data
                        DataTable dtTempRsvAudit = new DataTable();
                        dtTempRsvAudit.Columns.Add("id", typeof(Int32));
                        dtTempRsvAudit.Columns.Add("1", typeof(string));
                        dtTempRsvAudit.Columns.Add("2", typeof(string));
                        dtTempRsvAudit.Columns.Add("3", typeof(string));
                        dtTempRsvAudit.Columns.Add("4", typeof(string));
                        dtTempRsvAudit.Columns.Add("5", typeof(string));
                        dtTempRsvAudit.Columns.Add("6", typeof(string));
                        dtTempRsvAudit.Columns.Add("7", typeof(string));
                        dtTempRsvAudit.Columns.Add("8", typeof(string));
                        dtTempRsvAudit.Columns.Add("user_code", typeof(string));
                        dtTempRsvAudit.Columns.Add("last_modified", typeof(DateTime));

                        //Datatable to hold the royaltor audit data
                        DataTable dtTempRoyAudit = auditData.Tables[1].Clone();


                        if (auditData.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow drow in auditData.Tables[0].Rows)
                            {
                                if (id == 0)
                                {
                                    //Add a new row as id=0 (1st row returned from query)
                                    dtTempRsvAudit.Rows.Add(id, null, null, null, null, null, null, null, null, null, null);
                                    dtTempRsvAudit.Rows[id]["last_modified"] = drow["last_modified"];
                                    dtTempRsvAudit.Rows[id]["user_code"] = drow["user_code"];


                                    for (Int32 i = 1; i <= 8; i++)
                                    {
                                        if (i.ToString() == drow["reserve_interval"].ToString())
                                        {
                                            //if change type is D (delete) then assign empty else assign liqudation percentage
                                            if (drow["change_type"].ToString() == "D")
                                            {
                                                dtTempRsvAudit.Rows[id][i.ToString()] = string.Empty;
                                            }
                                            else
                                            {
                                                dtTempRsvAudit.Rows[id][i.ToString()] = drow["liquidation_pct"];
                                            }
                                        }
                                    }
                                    id++;
                                }
                                else
                                {
                                    //if last_modified of current row matches with previous row then update the liqudation percentages
                                    //no need to add a new row
                                    if (dtTempRsvAudit.Rows[id - 1]["last_modified"].ToString() == drow["last_modified"].ToString())
                                    {
                                        for (Int32 i = 1; i <= 8; i++)
                                        {
                                            if (i.ToString() == drow["reserve_interval"].ToString())
                                            {
                                                //if change type is D (delete) then assign empty else assign liqudation percentage
                                                if (drow["change_type"].ToString() == "D")
                                                {
                                                    dtTempRsvAudit.Rows[id - 1][i.ToString()] = string.Empty;
                                                }
                                                else
                                                {
                                                    dtTempRsvAudit.Rows[id - 1][i.ToString()] = drow["liquidation_pct"];
                                                }
                                            }
                                        }
                                    }
                                    //if last_modified of current row doesn't with previous row then add a new row
                                    else
                                    {
                                        dtTempRsvAudit.Rows.Add(id, null, null, null, null, null, null, null, null, null, null);
                                        dtTempRsvAudit.Rows[id]["last_modified"] = drow["last_modified"];
                                        dtTempRsvAudit.Rows[id]["user_code"] = drow["user_code"];

                                        for (Int32 i = 1; i <= 8; i++)
                                        {
                                            if (i.ToString() == drow["reserve_interval"].ToString())
                                            {
                                                //if change type is D (delete) then assign empty else assign liqudation percentage
                                                if (drow["change_type"].ToString() == "D")
                                                {
                                                    dtTempRsvAudit.Rows[id][i.ToString()] = string.Empty;
                                                }
                                                else
                                                {
                                                    dtTempRsvAudit.Rows[id][i.ToString()] = drow["liquidation_pct"];
                                                }
                                            }
                                            else
                                            {
                                                //Copy the liquidation percentage from previous row
                                                dtTempRsvAudit.Rows[id][i.ToString()] = dtTempRsvAudit.Rows[id - 1][i.ToString()];
                                            }
                                        }
                                        id++;
                                    }
                                }

                            }

                        }

                        if (auditData.Tables[1].Rows.Count > 0)
                        {
                            dtTempRoyAudit = auditData.Tables[1];

                            for (int i = 0; i < dtTempRoyAudit.Rows.Count - 1; )
                            {
                                DataRow currentRow = dtTempRoyAudit.Rows[i];
                                DataRow nextRow = dtTempRoyAudit.Rows[i + 1];

                                //if all the 4 values matches for current row and next row then remove next row as no changes in reserve columns
                                if (currentRow["reserve_prod_type"].ToString() == nextRow["reserve_prod_type"].ToString() && currentRow["reserve_pct"].ToString() == nextRow["reserve_pct"].ToString()
                                   && currentRow["reserve_end_date"].ToString() == nextRow["reserve_end_date"].ToString() && currentRow["reserve_sales_flag"].ToString() == nextRow["reserve_sales_flag"].ToString())
                                {
                                    dtTempRoyAudit.Rows.Remove(nextRow);
                                }
                                else
                                {
                                    i++;
                                }
                            }
                        }

                        //Datatable to hold the last_modified date in asc order from above 2 datatables 
                        DataTable dtTempLastModified = new DataTable();
                        dtTempLastModified.Columns.Add("last_modified", typeof(DateTime));
                        dtTempLastModified.Columns.Add("audit_flag", typeof(string));

                        //Get the last_modified date from the temporary datatables 
                        //Add "Roy" or "Rsv" to audit_flag to different royaltor change date from reserve change date 
                        foreach (DataRow royRow in dtTempRoyAudit.Rows)
                        {
                            dtTempLastModified.Rows.Add(royRow["last_modified"].ToString(), "Roy");
                        }

                        foreach (DataRow rsvRow in dtTempRsvAudit.Rows)
                        {
                            if (!string.IsNullOrWhiteSpace(rsvRow["last_modified"].ToString()))
                            {
                                dtTempLastModified.Rows.Add(rsvRow["last_modified"].ToString(), "Rsv");
                            }
                        }

                        //sort the last_modified dates and get the distinct values
                        dtTempLastModified.DefaultView.Sort = "last_modified asc";
                        dtTempLastModified = dtTempLastModified.DefaultView.ToTable(true, "last_modified", "audit_flag");

                        //Creating the final table 
                        DataTable dtFinalAudit = new DataTable();
                        dtFinalAudit.Columns.Add("id", typeof(Int32));
                        dtFinalAudit.Columns.Add("reserve_prod_type", typeof(string));
                        dtFinalAudit.Columns.Add("reserve_pct", typeof(string));
                        dtFinalAudit.Columns.Add("reserve_end_date", typeof(string));
                        dtFinalAudit.Columns.Add("reserve_sales_flag", typeof(string));
                        dtFinalAudit.Columns.Add("1", typeof(string));
                        dtFinalAudit.Columns.Add("2", typeof(string));
                        dtFinalAudit.Columns.Add("3", typeof(string));
                        dtFinalAudit.Columns.Add("4", typeof(string));
                        dtFinalAudit.Columns.Add("5", typeof(string));
                        dtFinalAudit.Columns.Add("6", typeof(string));
                        dtFinalAudit.Columns.Add("7", typeof(string));
                        dtFinalAudit.Columns.Add("8", typeof(string));
                        dtFinalAudit.Columns.Add("user_code", typeof(string));
                        dtFinalAudit.Columns.Add("last_modified", typeof(DateTime));

                        Int32 rowNum = 0;

                        foreach (DataRow dtRow in dtTempLastModified.Rows)
                        {
                            if (dtRow["audit_flag"].ToString() == "Roy")
                            {
                                //filter the data based on last_modified date
                                DataRow[] royFilteredRow = dtTempRoyAudit.Select("last_modified = '" + dtRow["last_modified"].ToString() + "'");

                                foreach (DataRow row in royFilteredRow)
                                {
                                    if (rowNum > 0)
                                    {
                                        //if last_modified of filtered row matches with last_modified of previous row of final datatable
                                        //then copy the data else add a new row
                                        DataRow previousRow = dtFinalAudit.Rows[rowNum - 1];
                                        if (previousRow["last_modified"].ToString() == row["last_modified"].ToString())
                                        {
                                            dtFinalAudit.Rows[rowNum - 1]["reserve_prod_type"] = row["reserve_prod_type"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["reserve_pct"] = row["reserve_pct"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["reserve_end_date"] = row["reserve_end_date"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["reserve_sales_flag"] = row["reserve_sales_flag"].ToString();
                                        }
                                        else
                                        {
                                            dtFinalAudit.Rows.Add(rowNum, row["reserve_prod_type"].ToString(), row["reserve_pct"].ToString(), row["reserve_end_date"].ToString(), row["reserve_sales_flag"].ToString(), previousRow["1"].ToString(), previousRow["2"].ToString(), previousRow["3"].ToString(), previousRow["4"].ToString(), previousRow["5"].ToString(), previousRow["6"].ToString(), previousRow["7"].ToString(), previousRow["8"].ToString(), row["user_code"].ToString(), row["last_modified"].ToString());
                                            rowNum++;
                                        }
                                    }
                                    else
                                    {
                                        //if no row presents in final datatable then add a new row with royaltor data
                                        dtFinalAudit.Rows.Add(rowNum, row["reserve_prod_type"].ToString(), row["reserve_pct"].ToString(), row["reserve_end_date"].ToString(), row["reserve_sales_flag"].ToString(), null, null, null, null, null, null, null, null, row["user_code"].ToString(), row["last_modified"].ToString());
                                        rowNum++;
                                    }

                                }
                            }
                            else if (dtRow["audit_flag"].ToString() == "Rsv")
                            {
                                //filter the data based on last_modified date
                                DataRow[] rsvFilteredRow = dtTempRsvAudit.Select("last_modified = '" + dtRow["last_modified"].ToString() + "'");

                                foreach (DataRow row in rsvFilteredRow)
                                {
                                    if (rowNum > 0)
                                    {
                                        //if last_modified of filtered row matches with last_modified of previous row of final datatable
                                        //then copy the data else add a new row
                                        DataRow previousRow = dtFinalAudit.Rows[rowNum - 1];
                                        if (previousRow["last_modified"].ToString() == row["last_modified"].ToString())
                                        {
                                            dtFinalAudit.Rows[rowNum - 1]["1"] = row["1"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["2"] = row["2"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["3"] = row["3"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["4"] = row["4"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["5"] = row["5"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["6"] = row["6"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["7"] = row["7"].ToString();
                                            dtFinalAudit.Rows[rowNum - 1]["8"] = row["8"].ToString();
                                        }
                                        else
                                        {
                                            dtFinalAudit.Rows.Add(rowNum, previousRow["reserve_prod_type"].ToString(), previousRow["reserve_pct"].ToString(), previousRow["reserve_end_date"].ToString(), previousRow["reserve_sales_flag"].ToString(), row["1"].ToString(), row["2"].ToString(), row["3"].ToString(), row["4"].ToString(), row["5"].ToString(), row["6"].ToString(), row["7"].ToString(), row["8"].ToString(), row["user_code"].ToString(), row["last_modified"].ToString());
                                            rowNum++;
                                        }
                                    }
                                    else
                                    {
                                        //if no row presents in final datatable then add a new row with reserve data
                                        dtFinalAudit.Rows.Add(rowNum, null, null, null, null, row["1"].ToString(), row["2"].ToString(), row["3"].ToString(), row["4"].ToString(), row["5"].ToString(), row["6"].ToString(), row["7"].ToString(), row["8"].ToString(), row["user_code"].ToString(), row["last_modified"].ToString());
                                        rowNum++;
                                    }
                                }
                            }
                        }

                        //sort the data in desc order
                        dtFinalAudit.DefaultView.Sort = "id desc";
                        gvRoyaltorRsvAuditDetails.DataSource = dtFinalAudit;
                        gvRoyaltorRsvAuditDetails.DataBind();

                        CompareRsvAuditRows();
                    }
                    else
                    {
                        ExceptionHandler("Error in fetching royaltordata", string.Empty);
                    }
                }
                else
                {
                    msgView.SetMessage("Please select a valid royaltor from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in displaying reserve audit data.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtRoyaltorSearch.Text = string.Empty;
                    return;
                }

                txtRoyaltorSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                RoyaltorSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }

        protected void btnHdnRoyaltorSearch_Click(object sender, EventArgs e)
        {
            try
            {
                RoyaltorSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading audit details.", ex.Message);
            }
        }

        protected void btnContractAudit_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    RoyaltorSelected();
                }
                else
                {
                    msgView.SetMessage("Please select a valid royaltor from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading audit details.", ex.Message);
            }
        }

        protected void btnContractMaint_Click(object sender, EventArgs e)
        {
            try
            {
                hdnIsMaintScreen.Value = "Y";

                if (hdnIsValidSearch.Value == "Y" && !string.IsNullOrWhiteSpace(txtRoyaltorSearch.Text))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "PreviousScreen", "RedirectToMaintScreen(" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + ");", true);
                   // Response.Redirect(@"~/Contract/RoyaltorContract.aspx?RoyaltorId=" + txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim() + "", false);
                }
                else
                {
                    msgView.SetMessage("Please select a valid royaltor from list", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in redirecting to contract maintenance screen.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        private void LoadData()
        {

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                int royaltorId = Convert.ToInt32(Request.QueryString[0]);

                //populate royaltor for the royaltor id
                if (Session["FuzzySearchAllRoyaltorList"] == null)
                {
                    DataSet dsList = new FuzzySearchBL().GetFuzzySearchList("GetAllRoyaltorList", out errorId);
                    Session["FuzzySearchAllRoyaltorList"] = dsList.Tables[0];
                }

                DataRow[] filteredRow = ((DataTable)Session["FuzzySearchAllRoyaltorList"]).Select("royaltor_id = '" + royaltorId + "'");
                txtRoyaltorSearch.Text = filteredRow[0]["royaltor"].ToString();
                RoyaltorSelected();
            }
        }

        private void RoyaltorSelected()
        {
            hdnIsValidSearch.Value = "Y";
            tdData.Style.Remove("display");
            trRoyAudit.Style.Remove("display");
            trRsvAudit.Style.Add("display", "none");
            btnReserveAudit.Visible = true;
            btnReserveAudit.Enabled = true;
            btnContractAudit.Visible = false;

            royContractAuditBL = new RoyContractAuditBL();
            DataSet auditData = royContractAuditBL.GetRoyContractAuditData(Convert.ToInt32(txtRoyaltorSearch.Text.Split('-')[0].ToString().Trim()), out errorId);
            royContractAuditBL = null;

            if (auditData.Tables.Count != 0 && errorId != 2)
            {
                DataTable dtRoyAuditData = auditData.Tables[0];

                for (int i = 0; i < dtRoyAuditData.Rows.Count - 1; )
                {
                    DataRow currentRow = dtRoyAuditData.Rows[i];
                    DataRow nextRow = dtRoyAuditData.Rows[i + 1];

                    //if all the values matche for current row and next row then remove next row as no changes in reserve columns
                    if (currentRow["royaltor_name"].ToString() == nextRow["royaltor_name"].ToString() && currentRow["company_name"].ToString() == nextRow["company_name"].ToString()
                       && currentRow["owner_name"].ToString() == nextRow["owner_name"].ToString() && currentRow["responsibility_desc"].ToString() == nextRow["responsibility_desc"].ToString()
                        && currentRow["label_description"].ToString() == nextRow["label_description"].ToString() && currentRow["chargeble_to"].ToString() == nextRow["chargeble_to"].ToString()
                        && currentRow["parent_contribution_pct"].ToString() == nextRow["parent_contribution_pct"].ToString() && currentRow["royaltor_plg_id"].ToString() == nextRow["royaltor_plg_id"].ToString()
                        && currentRow["royaltor_held"].ToString() == nextRow["royaltor_held"].ToString() && currentRow["royaltor_locked"].ToString() == nextRow["royaltor_locked"].ToString()
                        && currentRow["signed"].ToString() == nextRow["signed"].ToString() && currentRow["contract_expiry_date"].ToString() == nextRow["contract_expiry_date"].ToString()
                        && currentRow["statement_type_code"].ToString() == nextRow["statement_type_code"].ToString() && currentRow["priority_desc"].ToString() == nextRow["priority_desc"].ToString()
                        && currentRow["print_stream"].ToString() == nextRow["print_stream"].ToString() && currentRow["stmt_display_zero"].ToString() == nextRow["stmt_display_zero"].ToString()
                        && currentRow["stmt_producer_report"].ToString() == nextRow["stmt_producer_report"].ToString() && currentRow["stmt_cost_report"].ToString() == nextRow["stmt_cost_report"].ToString()
                        && currentRow["contract_type_desc"].ToString() == nextRow["contract_type_desc"].ToString()
                        && currentRow["statement_format_desc"].ToString() == nextRow["statement_format_desc"].ToString()
                        && currentRow["status"].ToString() == nextRow["status"].ToString()
                        && currentRow["contract_start_date"].ToString() == nextRow["contract_start_date"].ToString()
                        && currentRow["royaltor_type"].ToString() == nextRow["royaltor_type"].ToString()
                        && currentRow["soc_sec_no"].ToString() == nextRow["soc_sec_no"].ToString()
                        && currentRow["exclude_from_accrual"].ToString() == nextRow["exclude_from_accrual"].ToString()
                        && currentRow["send_to_portal"].ToString() == nextRow["send_to_portal"].ToString())
                    {
                        dtRoyAuditData.Rows.Remove(currentRow);
                    }
                    else
                    {
                        i++;
                    }
                }

                BindGrid(dtRoyAuditData);
            }
            else
            {
                ExceptionHandler("Error in fetching royaltordata", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            //tdData.Style.Remove("display");

            if (gridData.Rows.Count > 0)
            {
                gvRoyaltorAuditDetails.DataSource = gridData;
                gvRoyaltorAuditDetails.DataBind();
                CompareRoyAuditRows();
            }
            else
            {
                dtEmpty = new DataTable();
                gvRoyaltorAuditDetails.DataSource = dtEmpty;
                gvRoyaltorAuditDetails.DataBind();
            }
        }

        private void CompareRoyAuditRows()
        {
            for (int i = 0; i < gvRoyaltorAuditDetails.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvRoyaltorAuditDetails.Rows[i];
                GridViewRow nextRow = gvRoyaltorAuditDetails.Rows[i + 1];

                //Comapre Royaltor Name
                if ((currentRow.FindControl("lblRoyaltorName") as Label).Text != (nextRow.FindControl("lblRoyaltorName") as Label).Text)
                {
                    (currentRow.FindControl("lblRoyaltorName") as Label).ForeColor = Color.Red;
                }

                //Compare PLG Royaltor Number
                if ((currentRow.FindControl("lblPLGRoyaltorNumber") as Label).Text != (nextRow.FindControl("lblPLGRoyaltorNumber") as Label).Text)
                {
                    (currentRow.FindControl("lblPLGRoyaltorNumber") as Label).ForeColor = Color.Red;
                }

                //Compare Reporting Schedule
                if ((currentRow.FindControl("lblReportingSchedule") as Label).Text != (nextRow.FindControl("lblReportingSchedule") as Label).Text)
                {
                    (currentRow.FindControl("lblReportingSchedule") as Label).ForeColor = Color.Red;
                }


                //Compare Company
                if ((currentRow.FindControl("lblCompany") as Label).Text != (nextRow.FindControl("lblCompany") as Label).Text)
                {
                    (currentRow.FindControl("lblCompany") as Label).ForeColor = Color.Red;
                }

                //Compare Held
                if ((currentRow.FindControl("lblHeld") as Label).Text != (nextRow.FindControl("lblHeld") as Label).Text)
                {
                    (currentRow.FindControl("lblHeld") as Label).ForeColor = Color.Red;
                }

                //Compare Statement Priority
                if ((currentRow.FindControl("lblStatementPriority") as Label).Text != (nextRow.FindControl("lblStatementPriority") as Label).Text)
                {
                    (currentRow.FindControl("lblStatementPriority") as Label).ForeColor = Color.Red;
                }

                //Compare Owner
                if ((currentRow.FindControl("lblOwner") as Label).Text != (nextRow.FindControl("lblOwner") as Label).Text)
                {
                    (currentRow.FindControl("lblOwner") as Label).ForeColor = Color.Red;
                }

                //Compare Lock
                if ((currentRow.FindControl("lblLock") as Label).Text != (nextRow.FindControl("lblLock") as Label).Text)
                {
                    (currentRow.FindControl("lblLock") as Label).ForeColor = Color.Red;
                }

                //Compare Print Group
                if ((currentRow.FindControl("lblPrintGroup") as Label).Text != (nextRow.FindControl("lblPrintGroup") as Label).Text)
                {
                    (currentRow.FindControl("lblPrintGroup") as Label).ForeColor = Color.Red;
                }

                //Compare Responsibility
                if ((currentRow.FindControl("lblResponsibility") as Label).Text != (nextRow.FindControl("lblResponsibility") as Label).Text)
                {
                    (currentRow.FindControl("lblResponsibility") as Label).ForeColor = Color.Red;
                }

                //Compare Signed
                if ((currentRow.FindControl("lblSigned") as Label).Text != (nextRow.FindControl("lblSigned") as Label).Text)
                {
                    (currentRow.FindControl("lblSigned") as Label).ForeColor = Color.Red;
                }

                //Compare Display 0 Values on Stmts
                if ((currentRow.FindControl("lblZeroValue") as Label).Text != (nextRow.FindControl("lblZeroValue") as Label).Text)
                {
                    (currentRow.FindControl("lblZeroValue") as Label).ForeColor = Color.Red;
                }

                //Compare Label
                if ((currentRow.FindControl("lblLabel") as Label).Text != (nextRow.FindControl("lblLabel") as Label).Text)
                {
                    (currentRow.FindControl("lblLabel") as Label).ForeColor = Color.Red;
                }

                //Compare ExpiryDate
                if ((currentRow.FindControl("lblExpiryDate") as Label).Text != (nextRow.FindControl("lblExpiryDate") as Label).Text)
                {
                    (currentRow.FindControl("lblExpiryDate") as Label).ForeColor = Color.Red;
                }

                //Compare Producer Summary
                if ((currentRow.FindControl("lblProducerSummary") as Label).Text != (nextRow.FindControl("lblProducerSummary") as Label).Text)
                {
                    (currentRow.FindControl("lblProducerSummary") as Label).ForeColor = Color.Red;
                }

                //Compare Chargeable To
                if ((currentRow.FindControl("lblChargeableTo") as Label).Text != (nextRow.FindControl("lblChargeableTo") as Label).Text)
                {
                    (currentRow.FindControl("lblChargeableTo") as Label).ForeColor = Color.Red;
                }

                //Compare Chargeable Percent
                if ((currentRow.FindControl("lblChargeablePercent") as Label).Text != (nextRow.FindControl("lblChargeablePercent") as Label).Text)
                {
                    (currentRow.FindControl("lblChargeablePercent") as Label).ForeColor = Color.Red;
                }

                //Compare Cost Report
                if ((currentRow.FindControl("lblCostReport") as Label).Text != (nextRow.FindControl("lblCostReport") as Label).Text)
                {
                    (currentRow.FindControl("lblCostReport") as Label).ForeColor = Color.Red;
                }

                //Compare Contract Type
                if ((currentRow.FindControl("lblContractTypeDesc") as Label).Text != (nextRow.FindControl("lblContractTypeDesc") as Label).Text)
                {
                    (currentRow.FindControl("lblContractTypeDesc") as Label).ForeColor = Color.Red;
                }

                //Compare Statement Format
                if ((currentRow.FindControl("lblStatementFormatDesc") as Label).Text != (nextRow.FindControl("lblStatementFormatDesc") as Label).Text)
                {
                    (currentRow.FindControl("lblStatementFormatDesc") as Label).ForeColor = Color.Red;
                }

                //WUIN-858 - Adding new fields.
                //Compare Status
                if ((currentRow.FindControl("lblStatus") as Label).Text != (nextRow.FindControl("lblStatus") as Label).Text)
                {
                    (currentRow.FindControl("lblStatus") as Label).ForeColor = Color.Red;
                }

                //Compare Start Date
                if ((currentRow.FindControl("lblStartDate") as Label).Text != (nextRow.FindControl("lblStartDate") as Label).Text)
                {
                    (currentRow.FindControl("lblStartDate") as Label).ForeColor = Color.Red;
                }

                //Compare Royaltor Type
                if ((currentRow.FindControl("lblRoyaltorType") as Label).Text != (nextRow.FindControl("lblRoyaltorType") as Label).Text)
                {
                    (currentRow.FindControl("lblRoyaltorType") as Label).ForeColor = Color.Red;
                }

                //WUIN-936 - Adding new fields.
                //Compare Send to Portal
                if ((currentRow.FindControl("lblSendtoPortal") as Label).Text != (nextRow.FindControl("lblSendtoPortal") as Label).Text)
                {
                    (currentRow.FindControl("lblSendtoPortal") as Label).ForeColor = Color.Red;
                }

                //JIRA-970 Changes by Ravi on 15/02/2019 -- Start
                //Compare Social Security Number
                if ((currentRow.FindControl("lblSocSecNo") as Label).Text != (nextRow.FindControl("lblSocSecNo") as Label).Text)
                {
                    (currentRow.FindControl("lblSocSecNo") as Label).ForeColor = Color.Red;
                }
                //JIRA-970 Changes by Ravi on 15/02/2019 -- End

                //JIRA-1006 Changes by Ravi on 29/04/2019 -- Start
                //Compare Exclude from Accrual
                if ((currentRow.FindControl("lblExcludeFromAccrual") as Label).Text != (nextRow.FindControl("lblExcludeFromAccrual") as Label).Text)
                {
                    (currentRow.FindControl("lblExcludeFromAccrual") as Label).ForeColor = Color.Red;
                }
                //JIRA-1006 Changes by Ravi on 29/04/2019 -- End
            }
        }

        private void CompareRsvAuditRows()
        {
            for (int i = 0; i < gvRoyaltorRsvAuditDetails.Rows.Count - 1; i++)
            {
                GridViewRow currentRow = gvRoyaltorRsvAuditDetails.Rows[i];
                GridViewRow nextRow = gvRoyaltorRsvAuditDetails.Rows[i + 1];

                //Comapre Products Reserves Taken On
                if ((currentRow.FindControl("lblRsvTakenOn") as Label).Text != (nextRow.FindControl("lblRsvTakenOn") as Label).Text)
                {
                    (currentRow.FindControl("lblRsvTakenOn") as Label).ForeColor = Color.Red;
                }

                //Compare Default Royaltor Reserve %
                if ((currentRow.FindControl("lblRsvPercent") as Label).Text != (nextRow.FindControl("lblRsvPercent") as Label).Text)
                {
                    (currentRow.FindControl("lblRsvPercent") as Label).ForeColor = Color.Red;
                }

                //Compare Reserves End Date
                if ((currentRow.FindControl("lblRsvEndDate") as Label).Text != (nextRow.FindControl("lblRsvEndDate") as Label).Text)
                {
                    (currentRow.FindControl("lblRsvEndDate") as Label).ForeColor = Color.Red;
                }


                //Compare Nett units / Sales units (N/S)
                if ((currentRow.FindControl("lblUnits") as Label).Text != (nextRow.FindControl("lblUnits") as Label).Text)
                {
                    (currentRow.FindControl("lblUnits") as Label).ForeColor = Color.Red;
                }

                //Compare Liquidation Period 1
                if ((currentRow.FindControl("lblInterval1") as Label).Text != (nextRow.FindControl("lblInterval1") as Label).Text)
                {
                    (currentRow.FindControl("lblInterval1") as Label).ForeColor = Color.Red;
                }

                //Compare Liquidation Period 2
                if ((currentRow.FindControl("lblInterval2") as Label).Text != (nextRow.FindControl("lblInterval2") as Label).Text)
                {
                    (currentRow.FindControl("lblInterval2") as Label).ForeColor = Color.Red;
                }

                //Compare Liquidation Period 3
                if ((currentRow.FindControl("lblInterval3") as Label).Text != (nextRow.FindControl("lblInterval3") as Label).Text)
                {
                    (currentRow.FindControl("lblInterval3") as Label).ForeColor = Color.Red;
                }

                //Compare Liquidation Period 4
                if ((currentRow.FindControl("lblInterval4") as Label).Text != (nextRow.FindControl("lblInterval4") as Label).Text)
                {
                    (currentRow.FindControl("lblInterval4") as Label).ForeColor = Color.Red;
                }

                //Compare Liquidation Period 5
                if ((currentRow.FindControl("lblInterval5") as Label).Text != (nextRow.FindControl("lblInterval5") as Label).Text)
                {
                    (currentRow.FindControl("lblInterval5") as Label).ForeColor = Color.Red;
                }

                //Compare Liquidation Period 6
                if ((currentRow.FindControl("lblInterval6") as Label).Text != (nextRow.FindControl("lblInterval6") as Label).Text)
                {
                    (currentRow.FindControl("lblInterval6") as Label).ForeColor = Color.Red;
                }

                //Compare Liquidation Period 7
                if ((currentRow.FindControl("lblInterval7") as Label).Text != (nextRow.FindControl("lblInterval7") as Label).Text)
                {
                    (currentRow.FindControl("lblInterval7") as Label).ForeColor = Color.Red;
                }

                //Compare Liquidation Period 8
                if ((currentRow.FindControl("lblInterval8") as Label).Text != (nextRow.FindControl("lblInterval8") as Label).Text)
                {
                    (currentRow.FindControl("lblInterval8") as Label).ForeColor = Color.Red;
                }
            }
        }

        private void FuzzySearchRoyaltor()
        {
            if (txtRoyaltorSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(txtRoyaltorSearch.Text.Trim().ToUpper(), int.MaxValue);
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
