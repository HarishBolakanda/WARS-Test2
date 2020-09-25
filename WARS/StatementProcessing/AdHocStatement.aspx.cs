/*
File Name   :   AdHocStatement.cs
Purpose     :   to create new statement periods associated with royaltors/owners

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     02-Aug-2016     Harish(Infosys Limited)   Initial Creation
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
using System.Globalization;


namespace WARS
{
    public partial class AdHocStatement : System.Web.UI.Page
    {
        #region Global Declarations
        string loggedUserID;
        AdHocStatementBL adHocStatementBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        DataTable dtRoyaltors;
        DataTable dtOwners;
        DataTable dtGridData;
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
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "AdHoc Statement";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "AdHoc Statement";
                }

                if (!IsPostBack)
                {
                    txtStmtDescStartDate.Focus();
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        PopulateDropDowns();
                        LoadInitialGridData();

                        //defing the datatable to bind to grid
                        if (gvStatements.Rows.Count == 0)
                        {
                            dtGridData = new DataTable();
                            dtGridData.Columns.Add("OwnerCode", typeof(string));
                            dtGridData.Columns.Add("OwnerName", typeof(string));
                            dtGridData.Columns.Add("RoyaltorId", typeof(string));
                            dtGridData.Columns.Add("RoyaltorName", typeof(string));
                            dtGridData.Columns.Add("LevelFlag", typeof(string));
                        }

                        Session["AdHocStmtGridData"] = dtGridData;

                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }

                    util = null;

                }

                UserAuthorization();

            }
            catch (ThreadAbortException ex1)
            {

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page load.", ex.Message);
            }

        }

        protected void txtRoySearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtRoySearch.Text == string.Empty)
                    return;

                if (hdnSearchListItemSelected.Value == "Y")
                {
                    if (ValidateSelectedRoyaltorFilter() == false)
                    {
                        msgView.SetMessage("Please select a valid royaltor from the list.",
                                    MessageType.Warning, PositionType.Auto);
                        txtRoySearch.Text = string.Empty;
                        return;
                    }
                    else
                    {
                        LoadGridData(txtRoySearch.Text.Split('-')[0].ToString().Trim(), string.Empty);
                        if (gvStatements.Rows.Count > 0)
                        {
                            PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                        }

                    }
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchRoyaltor();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                }

                hdnSearchListItemSelected.Value = string.Empty;
                txtRoySearch.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from search box.", ex.Message);
            }
        }

        protected void txtOwnerSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtOwnSearch.Text == string.Empty)
                    return;

                if (hdnSearchListItemSelected.Value == "Y")
                {
                    if (ValidateSelectedOwnerFilter() == false)
                    {
                        msgView.SetMessage("Please select a valid owner from the list.",
                                    MessageType.Warning, PositionType.Auto);
                        txtOwnSearch.Text = string.Empty;
                        return;
                    }
                    else
                    {
                        LoadGridData(string.Empty, txtOwnSearch.Text.Split('-')[0].ToString().Trim());
                        if (gvStatements.Rows.Count > 0)
                        {
                            PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                        }

                    }
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchOwner();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                }

                hdnSearchListItemSelected.Value = string.Empty;
                txtOwnSearch.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting owner from search box.", ex.Message);
            }
        }

        protected void gvStatements_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ownerCode = (e.Row.FindControl("lblOwner") as Label).Text;
                string royaltorId = (e.Row.FindControl("lblRoyaltor") as Label).Text;
                if (ownerCode != string.Empty && royaltorId == string.Empty)//owner level row
                {
                    e.Row.Cells[0].Font.Bold = true;
                    e.Row.Cells[1].Font.Bold = true;
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
        protected void gvStatements_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["AdHocStmtGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvStatements.DataSource = dataView;
                gvStatements.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void valStmtDesc_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //Commented by Pratik WUIN-220
            //if (txtStmtDesc.Text != string.Empty)
            //{
            //    if ((txtStmtDescStartDate.Text != string.Empty || txtStmtDescEndDate.Text != string.Empty)
            //        && (txtStmtDescStartDate.Text != "__/____" || txtStmtDescEndDate.Text != "__/____"))
            //    {
            //        valStmtDesc.ErrorMessage = "Please enter either start & end date or a free text.";
            //        args.IsValid = false;
            //    }
            //    else
            //    {
            //        args.IsValid = true;
            //    }
            //}
            //else
            //{
            if (txtStmtDescStartDate.Text != string.Empty && txtStmtDescEndDate.Text != string.Empty
                && txtStmtDescStartDate.Text != "__/____" && txtStmtDescEndDate.Text != "__/____")
            {
                //validate - month and year are valid
                DateTime temp;
                if (txtStmtDescStartDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtStmtDescStartDate.Text, out temp))
                    {

                    }
                    else
                    {
                        valStmtDesc.ErrorMessage = "Not a valid start date!";
                        args.IsValid = false;
                        return;
                    }
                }

                if (txtStmtDescEndDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtStmtDescEndDate.Text, out temp))
                    {

                    }
                    else
                    {
                        valStmtDesc.ErrorMessage = "Not a valid end date!";
                        args.IsValid = false;
                        return;
                    }
                }

                //validate - from_date should be earlier than the to_date 
                if (Convert.ToDateTime("01/" + txtStmtDescEndDate.Text) < Convert.ToDateTime("01/" + txtStmtDescStartDate.Text))
                {
                    valStmtDesc.ErrorMessage = "Start date should be earlier than the end date!";
                    args.IsValid = false;
                    return;
                }

                //Int32 frmDateYear = Convert.ToInt32(txtStmtDescStartDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                //Int32 frmDateMonth = Convert.ToInt32(txtStmtDescStartDate.Text.Replace('_', ' ').Split('/')[0].Trim());
                //Int32 toDateYear = Convert.ToInt32(txtStmtDescEndDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                //Int32 toDateMonth = Convert.ToInt32(txtStmtDescEndDate.Text.Replace('_', ' ').Split('/')[0].Trim());

                ////validate - month and year are valid
                //if (!(frmDateMonth > 0 && frmDateMonth < 13))
                //{
                //    valStmtDesc.ErrorMessage = "Not a valid start date!";
                //    args.IsValid = false;
                //    return;
                //}
                //else if (!(toDateMonth > 0 && toDateMonth < 13))
                //{
                //    valStmtDesc.ErrorMessage = "Not a valid end date!";
                //    args.IsValid = false;
                //    return;

                //}//validate - from_date should be earlier than the to_date                    
                //else if ((frmDateYear > toDateYear) || (frmDateYear == toDateYear && frmDateMonth > toDateMonth))
                //{
                //    valStmtDesc.ErrorMessage = "Start date should be earlier than the end date!";
                //    args.IsValid = false;
                //    return;
                //}
                //else
                //{
                //    args.IsValid = true;
                //}
            }
            else
            {
                valStmtDesc.ErrorMessage = "Please enter either start & end date or a free text.";
                args.IsValid = false;
                return;
            }
            //}

        }

        protected void valStmtPriod_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ((txtStmtDateStartDate.Text != "" && txtStmtDateEndDate.Text != "") && (txtStmtDateStartDate.Text != "__/____" && txtStmtDateEndDate.Text != "__/____"))
            {
                //validate - month and year are valid
                DateTime temp;
                if (txtStmtDateStartDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtStmtDateStartDate.Text, out temp))
                    {

                    }
                    else
                    {
                        valStmtPriod.ErrorMessage = "Not a valid start date!";
                        args.IsValid = false;
                        return;
                    }
                }

                if (txtStmtDateEndDate.Text.Trim() != "__/____")
                {
                    if (DateTime.TryParse("01/" + txtStmtDateEndDate.Text, out temp))
                    {

                    }
                    else
                    {
                        valStmtPriod.ErrorMessage = "Not a valid end date!";
                        args.IsValid = false;
                        return;
                    }
                }

                //validate - from_date should be earlier than the to_date 
                if (Convert.ToDateTime("01/" + txtStmtDateEndDate.Text) < Convert.ToDateTime("01/" + txtStmtDateStartDate.Text))
                {
                    valStmtPriod.ErrorMessage = "Start date should be earlier than the end date!";
                    args.IsValid = false;
                    return;
                }

                //Int32 frmDateYear = Convert.ToInt32(txtStmtDateStartDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                //Int32 frmDateMonth = Convert.ToInt32(txtStmtDateStartDate.Text.Replace('_', ' ').Split('/')[0].Trim());
                //Int32 toDateYear = Convert.ToInt32(txtStmtDateEndDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                //Int32 toDateMonth = Convert.ToInt32(txtStmtDateEndDate.Text.Replace('_', ' ').Split('/')[0].Trim());

                ////validate - month and year are valid
                //if (!(frmDateMonth > 0 && frmDateMonth < 13))
                //{
                //    valStmtPriod.ErrorMessage = "Not a valid start date!";
                //    args.IsValid = false;
                //    return;
                //}
                //else if (!(toDateMonth > 0 && toDateMonth < 13))
                //{
                //    valStmtPriod.ErrorMessage = "Not a valid end date!";
                //    args.IsValid = false;
                //    return;

                //}
                ////validate - from_date should be earlier than the to_date
                //else if ((frmDateYear > toDateYear) || (frmDateYear == toDateYear && frmDateMonth > toDateMonth))
                //{
                //    valStmtPriod.ErrorMessage = "Start date should be earlier than the end date!";
                //    args.IsValid = false;
                //    return;
                //}
                //else
                //{
                //    args.IsValid = true;
                //}
            }
            else
            {
                valStmtPriod.ErrorMessage = "Please enter both start & end dates.";
                args.IsValid = false;
                return;
            }

        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox cbRemoveFromStmt;
                string ownerCode;
                string royaltorId;
                dtGridData = (DataTable)Session["AdHocStmtGridData"];
                int removedRowCount = 0;

                foreach (GridViewRow row in gvStatements.Rows)
                {
                    if (dtGridData.Rows.Count > 0)
                    {
                        cbRemoveFromStmt = (CheckBox)row.FindControl("cbRemoveFromStmt");
                        if (cbRemoveFromStmt.Checked == true)
                        {
                            ownerCode = (row.FindControl("lblOwner") as Label).Text;
                            royaltorId = (row.FindControl("lblRoyaltor") as Label).Text;

                            if (royaltorId != string.Empty)//royaltor level row
                            {
                                dtGridData.Rows.RemoveAt(row.RowIndex - removedRowCount);
                                removedRowCount++;
                            }
                            else if (ownerCode != string.Empty && royaltorId == string.Empty)//owner level row
                            {
                                if (dtGridData.Select("OwnerCode<>'" + ownerCode + "'").Count() != 0)
                                {
                                    DataTable dtRestOfData = dtGridData.Select("OwnerCode<>'" + ownerCode + "'").CopyToDataTable();
                                    removedRowCount = removedRowCount + (dtGridData.Rows.Count - dtRestOfData.Rows.Count);
                                    dtGridData = dtRestOfData.Copy();
                                }
                                else
                                {
                                    dtGridData.Clear();
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                BindGridData();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing royaltor from grid.", ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //validate 

                if (!Page.IsValid)
                    return;

                if (gvStatements.Rows.Count == 0)
                {
                    msgView.SetMessage("Please add a royaltor/owner group for the statement to be created.", MessageType.Warning, PositionType.Auto);
                    return;
                }

                string royaltorId;
                string levelFlag;
                List<string> roysToAdd = new List<string>();
                foreach (GridViewRow gvr in gvStatements.Rows)
                {
                    royaltorId = (gvr.FindControl("lblRoyaltor") as Label).Text;
                    levelFlag = (gvr.FindControl("lblLelvelFlag") as Label).Text;
                    if (royaltorId != string.Empty)//royaltor level row
                    {
                        roysToAdd.Add(royaltorId + "," + levelFlag);
                    }
                }

                if (roysToAdd.Count > 0)
                {
                    string stmtDescription;
                    //if (txtStmtDesc.Text != string.Empty) //Commented by Pratik WUIN-220
                    //{
                    //    stmtDescription = txtStmtDesc.Text.Trim();
                    //}
                    //else
                    //{
                    //    string stmtDescStartDate = DateTime.Parse("01/" + txtStmtDescStartDate.Text.Trim()).ToString("MMM-yyyy", CultureInfo.InvariantCulture).ToUpper();
                    //    string stmtDescEndDate = DateTime.Parse("01/" + txtStmtDescEndDate.Text.Trim()).ToString("MMM-yyyy", CultureInfo.InvariantCulture).ToUpper();
                    //    stmtDescription = stmtDescStartDate + " to " + stmtDescEndDate;
                    //}

                    loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    string stmtTypeCode = string.Empty;
                    adHocStatementBL = new AdHocStatementBL();
                    adHocStatementBL.SaveStatement(txtStmtDescStartDate.Text, txtStmtDescEndDate.Text, txtStmtDateStartDate.Text, txtStmtDateEndDate.Text, txtPaymentDate.Text, roysToAdd.ToArray(), loggedUserID, out stmtTypeCode, out errorId);
                    adHocStatementBL = null;

                    if (errorId == 1)
                    {
                        msgView.SetMessage("Invalid start and end months", MessageType.Warning, PositionType.Auto);
                    }
                    else if (errorId == 0)
                    {
                        //clear screen data
                        txtStmtDescStartDate.Text = string.Empty;
                        txtStmtDescEndDate.Text = string.Empty;
                        //txtStmtDesc.Text = string.Empty;
                        txtStmtDateStartDate.Text = string.Empty;
                        txtStmtDateEndDate.Text = string.Empty;
                        txtPaymentDate.Text = string.Empty;
                        txtRoySearch.Text = string.Empty;
                        txtOwnSearch.Text = string.Empty;
                        dtGridData = (DataTable)Session["AdHocStmtGridData"];
                        dtGridData.Clear();
                        BindGridData();
                        PopulateDropDowns();

                        msgView.SetMessage("Ad Hoc Statement details generated with Statement Identifier '" + stmtTypeCode + "'. The Statement will be processed in the next scheduled run. ",
                                            MessageType.Warning, PositionType.Auto);
                    }
                    else
                    {
                        ExceptionHandler("Error in saving statement details.", string.Empty);
                    }

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving ad hoc statement period.", ex.Message);
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
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
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
                ExceptionHandler("Error in owner fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Royaltor")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtRoySearch.Text = string.Empty;
                        return;
                    }

                    txtRoySearch.Text = lbFuzzySearch.SelectedValue.ToString();
                    LoadGridData(txtRoySearch.Text.Split('-')[0].ToString().Trim(), string.Empty);
                    if (gvStatements.Rows.Count > 0)
                    {
                        PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                    }
                }
                else if (hdnFuzzySearchField.Value == "Owner")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtOwnSearch.Text = string.Empty;
                        return;
                    }

                    txtOwnSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                    LoadGridData(string.Empty, txtOwnSearch.Text.Split('-')[0].ToString().Trim());
                    if (gvStatements.Rows.Count > 0)
                    {
                        PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                    }


                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fuzzy search selection", ex.Message);
            }
        }

        protected void valPaymentDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //validate - payment date
            DateTime temp;
            if (txtPaymentDate.Text.Trim() != "__/__/____")
            {
                if (DateTime.TryParse(txtPaymentDate.Text, out temp))
                {

                }
                else
                {
                    args.IsValid = false;
                    return;
                }
            }
            else
            {
                args.IsValid = false;
                return;
            }
        }

        #endregion EVENTS

        #region METHODS
        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

        }

        private void UserAuthorization()
        {
            
            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnRemove.Enabled = false;
                btnSave.Enabled = false;
            }

        }

        private void PopulateDropDowns()
        {
            adHocStatementBL = new AdHocStatementBL();
            DataSet dropdownListData = adHocStatementBL.GetDropDownData(out errorId);
            adHocStatementBL = null;

            if (dropdownListData.Tables.Count != 0 && errorId != 2)
            {
                //for royaltor search
                Session["AdHocStmtsRoyaltorList"] = dropdownListData.Tables[0];

            }
            else
            {
                ExceptionHandler("Error in fetching the search list values.", string.Empty);
            }

        }

        private void LoadInitialGridData()
        {
            dtEmpty = new DataTable();
            gvStatements.DataSource = dtEmpty;
            gvStatements.DataBind();
        }

        private bool ValidateSelectedRoyaltorFilter()
        {
            if (txtRoySearch.Text != string.Empty)
            {
                if (txtRoySearch.Text == "No results found" || txtRoySearch.Text == string.Empty)
                {
                    return false;
                }
                else
                {
                    //validate if entered text is a valid royaltor Id
                    int num;
                    bool isNumeric = int.TryParse(txtRoySearch.Text.Split('-')[0].ToString().Trim(), out num);
                    if (isNumeric == false)
                        return false;
                    else
                        return true;
                }
            }
            /*
        else if (txtRoyaltor.Text != string.Empty)
        {
            //validate if entered text is a valid royaltor Id
            int num;
            bool isNumeric = int.TryParse(txtRoyaltor.Text, out num);
            if (isNumeric == false)
                return false;

            dtRoyaltors = (DataTable)Session["AdHocStmtsRoyaltorList"];
            if (dtRoyaltors.Rows.Count > 0 && (dtRoyaltors.Select("royaltor_id='" + txtRoyaltor.Text.Trim() + "'").Count() != 0))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
             * */
            else
            {
                return true;
            }

        }

        private bool ValidateSelectedOwnerFilter()
        {
            if (txtOwnSearch.Text != string.Empty)
            {
                if (txtOwnSearch.Text == "No results found" || txtOwnSearch.Text == string.Empty)
                {
                    return false;
                }
                else
                {
                    //validate if entered text is a valid owner Id
                    int num;
                    bool isNumeric = int.TryParse(txtOwnSearch.Text.Split('-')[0].ToString().Trim(), out num);
                    if (isNumeric == false)
                        return false;
                    else
                        return true;
                }
            }
            /*else if (txtOwner.Text != string.Empty)
            {
                //validate if entered text is a valid owner Id
                int num;
                bool isNumeric = int.TryParse(txtOwner.Text, out num);
                if (isNumeric == false)
                    return false;

                dtOwners = (DataTable)Session["AdHocStmtsOwners"];
                if (dtOwners.Rows.Count > 0 && (dtOwners.Select("owner_code='" + txtOwner.Text.Trim() + "'").Count() != 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }*/
            else
            {
                return true;
            }
        }

        private void LoadGridData(string royaltorId, string ownerCode)
        {
            dtGridData = (DataTable)Session["AdHocStmtGridData"];

            if (royaltorId != string.Empty)//populate royaltor
            {
                //validate if the selected royaltor is not present in grid
                if (dtGridData.Select("RoyaltorId='" + royaltorId + "'").Count() == 0)
                {
                    DataTable dtRoyaltors;
                    if (Session["AdHocStmtsRoyaltorList"] != null)
                    {
                        dtRoyaltors = (DataTable)Session["AdHocStmtsRoyaltorList"];
                        if (dtRoyaltors.Rows.Count > 0)
                        {

                            //validation - 
                            //WUIN-890 - Do not allow Royaltor to be selected for Adhoc statement if
                            //           there is an existing ROYALTOR_ACTIVITY record with Status not = 4 
                            //           there is an existing ROYALTOR_ACTIVITY record with Status = 5
                            if (dtRoyaltors.Select("royaltor_id='" + royaltorId + "' AND is_allowed='Y'").Count() == 0)
                            {                                
                                msgView.SetMessage("Royaltor cannot be selected for Adhoc Statement as it has a Statement not archived or already requested", MessageType.Warning, PositionType.Auto);
                                return;
                            }

                            var results = dtRoyaltors.AsEnumerable().Where(dr => dr.Field<Int32>("royaltor_id").Equals(Convert.ToInt32(royaltorId)));
                            if (results.Count() > 0)
                            {
                                foreach (DataRow dr in results)
                                {
                                    dtGridData.Rows.Add((Convert.ToString(dr["owner_data"]).Split(',')[0]), (Convert.ToString(dr["owner_data"]).Split(',')[1]),
                                        (Convert.ToString(dr["royaltors"]).Split('-')[0].Trim()), (Convert.ToString(dr["royaltors"]).Split('-')[1].Trim()), "R");

                                }
                            }

                        }

                    }
                    BindGridData();
                    txtRoySearch.Text = string.Empty;
                }
                else
                {
                    //message
                    msgView.SetMessage("Selected royaltor is already present in the list to be added.", MessageType.Warning, PositionType.Auto);
                }

                txtOwnSearch.Text = string.Empty;

            }
            else if (ownerCode != string.Empty)//populate owner group
            {
                //validate if the selected owner group is not present in grid
                if (dtGridData.Select("OwnerCode='" + ownerCode + "' AND RoyaltorId is null").Count() == 0)
                {
                    string isAllowed;
                    //fetch list of royaltors for the owner code
                    adHocStatementBL = new AdHocStatementBL();
                    DataSet ownerGroupData = adHocStatementBL.GetOwnerGroupData(ownerCode, out isAllowed, out errorId);
                    adHocStatementBL = null;
                                       
                    //validation - 
                    //WUIN-890 - Do not allow Royaltor to be selected for Adhoc statement if
                    //           there is an existing ROYALTOR_ACTIVITY record with Status not = 4 
                    //           there is an existing ROYALTOR_ACTIVITY record with Status = 5 
                    if (isAllowed == "N")
                    {                        
                        msgView.SetMessage("Owner cannot be selected for Adhoc Statement as one or more Royaltor has a Statement not archived or already requested", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    if (ownerGroupData.Tables.Count != 0 && errorId != 2)
                    {
                        DataTable dtOwnerGroup = ownerGroupData.Tables[0];
                        //append to existing grid data
                        foreach (DataRow dr in dtOwnerGroup.Rows)
                        {
                            if (dtGridData.Select("RoyaltorId='" + dr["RoyaltorId"].ToString() + "'").Count() == 0)
                            {
                                dtGridData.ImportRow(dr);
                            }
                        }

                        //bind to grid
                        BindGridData();
                        txtOwnSearch.Text = string.Empty;

                    }
                    else
                    {
                        ExceptionHandler("Error in fetching owner group data.", string.Empty);
                    }
                }
                else
                {
                    //message
                    msgView.SetMessage("Selected owner group is already present in the list to be added.", MessageType.Warning, PositionType.Auto);
                }

                txtRoySearch.Text = string.Empty;

            }

        }

        private void BindGridData()
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            //set gridview panel height                    
            PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
            gvStatements.DataSource = dtGridData;
            gvStatements.DataBind();
            Session["AdHocStmtGridData"] = dtGridData;
        }

        private void FuzzySearchRoyaltor()
        {            
            if (txtRoySearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Royaltor";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyAdHocStmtRoyaltorList(txtRoySearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchOwner()
        {            
            if (txtOwnSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in owner filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Owner";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllOwnerList(txtOwnSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }
        #endregion METHODS

             

    }
}