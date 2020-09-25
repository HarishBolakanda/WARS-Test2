/*
File Name   :   OwnerMaintenance.cs
Purpose     :   Used for maintaining owner data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     07-Mar-2017     Pratik(Infosys Limited)   Initial Creation
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
    public partial class OwnerMaintenance : System.Web.UI.Page
    {

        #region Global Declarations

        OwnerMaintenanceBL ownerMaintenanceBL;
        string loggedUserID;
        Utilities util;
        Int32 errorId;
        Int32 newOwnerCode;
        DataTable dtEmpty;
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize1"].ToString());
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Owner Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Owner Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlOwnerDetails.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtOwnerSearch.Focus();
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
                        UserAuthorization();
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

        protected void btnHdnOwnerSearch_Click(object sender, EventArgs e)
        {
            try
            {
                hdnPageNumber.Value = "1";
                OwnerSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting owner from list.", ex.Message);
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
                ExceptionHandler("Error in owner search.", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtOwnerSearch.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing searh list.", ex.Message);
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
                OwnerSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting owner.", ex.Message);
            }
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["OwnerMaintData"] == null)
                    return;

                DataTable dtOwnerData = Session["OwnerMaintData"] as DataTable;
                if (dtOwnerData.Rows.Count == 0)
                    return;

                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);
                Utilities.PopulateGridPage(pageIndex, dtOwnerData, gridDefaultPageSize, gvOwnerDetails, dtEmpty, rptPager);
                hdnChangeNotSaved.Value = "N";
                UserAuthorization();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        protected void gvOwnerDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                if (e.CommandName == "saverow")
                {
                    if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
                    {
                        int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                        int ownerCode = Convert.ToInt32(((Label)gvOwnerDetails.Rows[rowIndex].FindControl("lblOwnerCode")).Text);
                        string ownerName = ((TextBox)gvOwnerDetails.Rows[rowIndex].FindControl("txtOwnerName")).Text;

                        ownerMaintenanceBL = new OwnerMaintenanceBL();
                        DataSet ownerData = ownerMaintenanceBL.UpdateOwnerData(ownerCode, ownerName.Trim().ToUpper(), userCode, out newOwnerCode, out errorId);
                        ownerMaintenanceBL = null;

                        if (ownerData.Tables.Count != 0 && errorId != 2)
                        {
                            Session["OwnerMaintData"] = ownerData.Tables[0];
                            Session["FuzzySearchAllOwnerList"] = ownerData.Tables[1];

                            //check if there is only one row in the grid before binding updated data.
                            //if count is 1 then only display that row
                            if (gvOwnerDetails.Rows.Count == 1)
                            {
                                DataTable dtSearched = ownerData.Tables[0].Clone();
                                DataRow[] foundRows = ownerData.Tables[0].Select("owner_code = '" + ownerCode + "'");
                                if (foundRows.Length != 0)
                                {
                                    dtSearched = foundRows.CopyToDataTable();
                                    BindGrid(dtSearched);
                                }
                            }
                            else
                            {
                                BindGrid(ownerData.Tables[0]);
                            }
                            hdnChangeNotSaved.Value = "N";
                            hdnGridRowSelectedPrvious.Value = null;
                            msgView.SetMessage("Owner details updated successfully.", MessageType.Success, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("Failed to updated owner details.", MessageType.Warning, PositionType.Auto);
                        }
                    }
                }
                else if (e.CommandName == "cancelrow")
                {

                    if (Session["OwnerMaintData"] != null)
                    {
                        DataTable dtOwnerData = Session["OwnerMaintData"] as DataTable;

                        if (gvOwnerDetails.Rows.Count == 1)
                        {
                            string ownerName = ((Label)gvOwnerDetails.Rows[0].FindControl("lblOwnerName")).Text;
                            ((TextBox)gvOwnerDetails.Rows[0].FindControl("txtOwnerName")).Text = ownerName;
                        }
                        else
                        {
                            BindGrid(dtOwnerData);
                            txtOwnerSearch.Text = string.Empty;
                        }
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving/deleting owner data.", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["OwnerMaintData"] != null)
                {
                    DataTable dtOwnerData = Session["OwnerMaintData"] as DataTable;
                    if (dtOwnerData.Select("owner_code = '" + txtOwnerCode.Text + "'").Length != 0)
                    {
                        msgView.SetMessage("Owner already exists with this owner code.", MessageType.Success, PositionType.Auto);
                        return;
                    }
                }

                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                ownerMaintenanceBL = new OwnerMaintenanceBL();
                DataSet ownerData = ownerMaintenanceBL.InsertOwnerData(Convert.ToInt32(txtOwnerCode.Text.Trim()), txtOwnerDesc.Text.Trim().ToUpper(), userCode, out newOwnerCode, out errorId);
                ownerMaintenanceBL = null;

                if (ownerData.Tables.Count != 0 && errorId != 2 && newOwnerCode != 0)
                {
                    Session["OwnerMaintData"] = ownerData.Tables[0];
                    Session["FuzzySearchAllOwnerList"] = ownerData.Tables[1];
                    gvOwnerDetails.PageIndex = gvOwnerDetails.PageCount - 1;
                    BindGrid(ownerData.Tables[0]);
                    txtOwnerDesc.Text = string.Empty;
                    txtOwnerCode.Text = string.Empty;
                    txtOwnerSearch.Text = string.Empty;
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    txtOwnerCode.Text = newOwnerCode.ToString();
                    msgView.SetMessage("Owner created successfully.", MessageType.Success, PositionType.Auto);
                }
                else if (newOwnerCode == 0)
                {
                    msgView.SetMessage("Owner already exists with this owner code.", MessageType.Success, PositionType.Auto);
                    return;

                }
                else
                {
                    msgView.SetMessage("Failed to create owner.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in creating owner.", ex.Message);
            }
        }

        protected void imgBtnCancel_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtOwnerCode.Text = string.Empty;
                txtOwnerDesc.Text = string.Empty;
                hdnInsertDataNotSaved.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing owner description.", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                if (Session["OwnerMaintData"] != null)
                {
                    DataTable dtOwnerData = Session["OwnerMaintData"] as DataTable;
                    gvOwnerDetails.PageIndex = 0;
                    hdnPageNumber.Value = "1";
                    BindGrid(dtOwnerData);
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }
                hdnIsValidSearch.Value = "N";
                txtOwnerDesc.Text = string.Empty;
                txtOwnerCode.Text = string.Empty;
                txtOwnerSearch.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reset.", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    //Validate
                    Page.Validate("valInsertOwner");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    if (Session["OwnerMaintData"] != null)
                    {
                        DataTable dtOwnerData = Session["OwnerMaintData"] as DataTable;
                        if (dtOwnerData.Select("owner_code = '" + txtOwnerCode.Text + "'").Length != 0)
                        {
                            msgView.SetMessage("Owner already exists with this owner code.", MessageType.Success, PositionType.Auto);
                            return;
                        }
                    }

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                    ownerMaintenanceBL = new OwnerMaintenanceBL();
                    DataSet ownerData = ownerMaintenanceBL.InsertOwnerData(Convert.ToInt32(txtOwnerCode.Text.Trim()), txtOwnerDesc.Text.Trim().ToUpper(), userCode, out newOwnerCode, out errorId);
                    ownerMaintenanceBL = null;

                    if (ownerData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["OwnerMaintData"] = ownerData.Tables[0];
                        Session["FuzzySearchAllOwnerList"] = ownerData.Tables[1];
                        gvOwnerDetails.PageIndex = gvOwnerDetails.PageCount - 1;
                        BindGrid(ownerData.Tables[0]);
                        txtOwnerDesc.Text = string.Empty;
                        txtOwnerCode.Text = string.Empty;
                        txtOwnerSearch.Text = string.Empty;
                        hdnChangeNotSaved.Value = "N";
                        hdnInsertDataNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Owner created successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to create owner.", MessageType.Warning, PositionType.Auto);
                    }
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    //Validate
                    //WUIN-932 -corrections - moved the validation to blow logic as this is not working on save/undo popup
                    //Page.Validate("valUpdateOwner");
                    //if (!Page.IsValid)
                    //{
                    //    mpeSaveUndo.Hide();
                    //    msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                    //    return;
                    //}

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                    int ownerCode = Convert.ToInt32(((Label)gvOwnerDetails.Rows[rowIndex].FindControl("lblOwnerCode")).Text);
                    string ownerName = ((TextBox)gvOwnerDetails.Rows[rowIndex].FindControl("txtOwnerName")).Text;
                    
                    //Validate
                    if (ownerName == string.Empty)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    ownerMaintenanceBL = new OwnerMaintenanceBL();
                    DataSet ownerData = ownerMaintenanceBL.UpdateOwnerData(ownerCode, ownerName, userCode, out newOwnerCode, out errorId);
                    ownerMaintenanceBL = null;

                    if (ownerData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["OwnerMaintData"] = ownerData.Tables[0];
                        Session["FuzzySearchAllOwnerList"] = ownerData.Tables[1];
                        //check if there is only one row in the grid before binding updated data.
                        //if count is 1 then only display that row
                        if (gvOwnerDetails.Rows.Count == 1)
                        {
                            DataTable dtSearched = ownerData.Tables[0].Clone();
                            DataRow[] foundRows = ownerData.Tables[0].Select("owner_code = '" + ownerCode + "'");
                            if (foundRows.Length != 0)
                            {
                                dtSearched = foundRows.CopyToDataTable();
                                BindGrid(dtSearched);
                            }
                        }
                        else
                        {
                            BindGrid(ownerData.Tables[0]);
                        }
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Owner details updated successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to updated owner details.", MessageType.Warning, PositionType.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving grid data.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtOwnerDesc.Text = string.Empty;
                    txtOwnerCode.Text = string.Empty;
                    hdnInsertDataNotSaved.Value = "N";
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    if (Session["OwnerMaintData"] != null)
                    {
                        DataTable dtOwnerData = Session["OwnerMaintData"] as DataTable;
                        BindGrid(dtOwnerData);
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data.", ex.Message);
            }
        }

        protected void btnOwnerAudit_Click(object sender, EventArgs e)
        {
            if (hdnIsValidSearch.Value == "Y")
            {
                Response.Redirect("../Audit/OwnerAudit.aspx?ownerData=" + txtOwnerSearch.Text + "");
            }
            else
            {
                Response.Redirect("../Audit/OwnerAudit.aspx");
            }
        }

        //JIRA-908 Changes by ravi in 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                int ownerCode = Convert.ToInt32(hdnOwnerCode.Value);
                ownerMaintenanceBL = new OwnerMaintenanceBL();
                DataSet ownerData = ownerMaintenanceBL.DeleteOwnerData(ownerCode, Convert.ToString(Session["UserCode"]), out newOwnerCode, out errorId);
                ownerMaintenanceBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("Can't delete owner as it's being used in contract.", MessageType.Success, PositionType.Auto);
                }
                else if (ownerData.Tables.Count != 0 && errorId == 0)
                {
                    Session["OwnerList"] = ownerData.Tables[0];
                    BindGrid(ownerData.Tables[0]);
                    txtOwnerSearch.Text = string.Empty;
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Owner deleted successfully.", MessageType.Success, PositionType.Auto);
                    hdnOwnerCode.Value = "";
                }
                else
                {
                    msgView.SetMessage("Failed to delete owner.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Failed to delete owner.", ex.Message);
            }
        }
        //JIRA-908 Changes by ravi in 13/02/2019 --End



        #endregion Events

        #region Methods

        private void LoadData()
        {
            ownerMaintenanceBL = new OwnerMaintenanceBL();
            DataSet initialData = ownerMaintenanceBL.GetInitialData(out newOwnerCode, out errorId);
            ownerMaintenanceBL = null;

            if (newOwnerCode != 0)
            {
                txtOwnerCode.Text = newOwnerCode.ToString();
            }

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                hdnPageNumber.Value = "1";
                Session["OwnerMaintData"] = initialData.Tables[0];
                BindGrid(initialData.Tables[0]);
            }
            else
            {
                ExceptionHandler("Error in fetching filter list data", string.Empty);
            }

            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                txtOwnerSearch.Text = Request.QueryString[0];
                OwnerSelected();
            }
        }

        private void OwnerSelected()
        {
            if (Session["OwnerMaintData"] != null)
            {
                hdnIsValidSearch.Value = "Y";

                DataTable dtOwnerData = (Session["OwnerMaintData"] as DataTable).Copy();
                DataTable dtSearched = (Session["OwnerMaintData"] as DataTable).Clone();

                DataRow[] foundRows = dtOwnerData.Select("owner_code = '" + txtOwnerSearch.Text.Split('-')[0].ToString().Trim() + "'");
                if (foundRows.Length != 0)
                {
                    dtSearched = foundRows.CopyToDataTable();
                    BindGrid(dtSearched);
                }

            }
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), gridData, gridDefaultPageSize, gvOwnerDetails, dtEmpty, rptPager);
            UserAuthorization();
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                imgBtnInsert.Enabled = false;
                imgBtnCancel.Enabled = false;
                btnSaveChanges.Enabled = false;
                //disable grid buttons
                foreach (GridViewRow rows in gvOwnerDetails.Rows)
                {
                    (rows.FindControl("imgBtnSave") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
                    (rows.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                }

            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
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

        #endregion Methods

        protected void gvOwnerDetails_RowDataBound(object sender, GridViewRowEventArgs e)
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
            DataTable dataTable = (DataTable)Session["OwnerMaintData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvOwnerDetails, dtEmpty, rptPager);
                Session["OwnerMaintData"] = dataView.ToTable();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End


    }
}