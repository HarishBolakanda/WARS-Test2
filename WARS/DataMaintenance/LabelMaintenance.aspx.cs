/*
File Name   :   LabelMaintenance.cs
Purpose     :   Used for maintaining Label data

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     13-Mar-2017     Pratik(Infosys Limited)   Initial Creation
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
    public partial class LabelMaintenance : System.Web.UI.Page
    {

        #region Global Declarations

        LabelMaintenanceBL labelMaintenanceBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Label Maintenance";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Label Maintenance";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlLabelDetails.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    txtLabelSearch.Focus();

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

        protected void fuzzySearchLabel_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchLabel();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in label search.", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;
                if (Session["LabelMaintData"] != null)
                {
                    DataTable dtLabelData = Session["LabelMaintData"] as DataTable;
                    BindGrid(dtLabelData);
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                }

                txtLabelSearch.Text = string.Empty;
                txtLabelDesc.Text = string.Empty;
                txtLabelCode.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reset.", ex.Message);
            }
        }

        protected void gvLabelDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                if (e.CommandName == "saverow")
                {
                    if (!string.IsNullOrEmpty(hdnGridRowSelectedPrvious.Value))
                    {
                        int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                        //int rowIndex = ((GridViewRow)((ImageButton)(e.CommandSource)).NamingContainer).RowIndex;
                        string labelCode = ((Label)gvLabelDetails.Rows[rowIndex].FindControl("lblLabelCode")).Text;
                        string labelDesc = ((TextBox)gvLabelDetails.Rows[rowIndex].FindControl("txtLabelName")).Text;

                        labelMaintenanceBL = new LabelMaintenanceBL();
                        DataSet labelData = labelMaintenanceBL.UpdateLabelData(labelCode, labelDesc, userCode, out errorId);
                        labelMaintenanceBL = null;

                        if (labelData.Tables.Count != 0 && errorId != 2)
                        {
                            Session["LabelMaintData"] = labelData.Tables[0];
                            Session["FuzzySearchAllLabelList"] = labelData.Tables[1];

                            //check if there is only one row in the grid before binding updated data.
                            //if count is 1 then only display that row
                            if (gvLabelDetails.Rows.Count == 1)
                            {
                                DataTable dtSearched = labelData.Tables[0].Clone();
                                DataRow[] foundRows = labelData.Tables[0].Select("label_code = '" + labelCode + "'");
                                if (foundRows.Length != 0)
                                {
                                    dtSearched = foundRows.CopyToDataTable();
                                    BindGrid(dtSearched);
                                }
                            }
                            else
                            {
                                BindGrid(labelData.Tables[0]);
                            }
                            hdnChangeNotSaved.Value = "N";
                            hdnGridRowSelectedPrvious.Value = null;
                            msgView.SetMessage("Label details updated successfully.", MessageType.Success, PositionType.Auto);
                        }
                        else
                        {
                            msgView.SetMessage("Failed to updated label details.", MessageType.Warning, PositionType.Auto);
                        }
                    }
                }
                else if (e.CommandName == "cancelrow")
                {

                    if (Session["LabelMaintData"] != null)
                    {
                        DataTable dtLabelData = Session["LabelMaintData"] as DataTable;
                        if (gvLabelDetails.Rows.Count == 1)
                        {
                            string labelDesc = ((Label)gvLabelDetails.Rows[0].FindControl("lblLabelName")).Text;

                            ((TextBox)gvLabelDetails.Rows[0].FindControl("txtLabelName")).Text = labelDesc;
                        }
                        else
                        {
                            BindGrid(dtLabelData);
                            txtLabelSearch.Text = string.Empty;
                        }
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving/deleting label data.", ex.Message);
            }
        }

        protected void btnHdnLabelSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LabelSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting label from list.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtLabelSearch.Text = string.Empty;
                    return;
                }

                txtLabelSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                LabelSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting label.", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtLabelSearch.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing search list.", ex.Message);
            }
        }

        protected void imgBtnInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["LabelMaintData"] != null)
                {
                    DataTable dtLabelData = Session["LabelMaintData"] as DataTable;
                    if (dtLabelData.Select("label_code = '" + txtLabelCode.Text + "'").Length != 0)
                    {
                        msgView.SetMessage("Label already exists with this label code.", MessageType.Success, PositionType.Auto);
                        return;
                    }
                }

                string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                labelMaintenanceBL = new LabelMaintenanceBL();
                DataSet labelData = labelMaintenanceBL.InsertLabelData(txtLabelCode.Text.Trim(), txtLabelDesc.Text.Trim(), userCode, out errorId);
                labelMaintenanceBL = null;

                if (labelData.Tables.Count != 0 && errorId != 2)
                {
                    Session["LabelMaintData"] = labelData.Tables[0];
                    Session["FuzzySearchAllLabelList"] = labelData.Tables[1];
                    BindGrid(labelData.Tables[0]);
                    txtLabelDesc.Text = string.Empty;
                    txtLabelCode.Text = string.Empty;
                    txtLabelSearch.Text = string.Empty;
                    hdnChangeNotSaved.Value = "N";
                    hdnInsertDataNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Label created successfully.", MessageType.Success, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to create label.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in creating label.", ex.Message);
            }
        }

        protected void imgBtnCancel_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtLabelDesc.Text = string.Empty;
                txtLabelCode.Text = string.Empty;
                hdnInsertDataNotSaved.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing label description.", ex.Message);
            }
        }

        protected void btnUndoChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    txtLabelDesc.Text = string.Empty;
                    txtLabelCode.Text = string.Empty;
                    hdnInsertDataNotSaved.Value = "N";
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    if (Session["LabelMaintData"] != null)
                    {
                        DataTable dtLabelData = (Session["LabelMaintData"] as DataTable).Copy();
                        BindGrid(dtLabelData);
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnInsertDataNotSaved.Value == "Y")
                {
                    //Validate
                    Page.Validate("valInsertLabel");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    if (Session["LabelMaintData"] != null)
                    {
                        DataTable dtLabelData = Session["LabelMaintData"] as DataTable;
                        if (dtLabelData.Select("label_code = '" + txtLabelCode.Text + "'").Length != 0)
                        {
                            msgView.SetMessage("Label already exists with this label code.", MessageType.Success, PositionType.Auto);
                            return;
                        }
                    }


                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());

                    labelMaintenanceBL = new LabelMaintenanceBL();
                    DataSet labelData = labelMaintenanceBL.InsertLabelData(txtLabelCode.Text.Trim(), txtLabelDesc.Text.Trim(), userCode, out errorId);
                    labelMaintenanceBL = null;

                    if (labelData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["LabelMaintData"] = labelData.Tables[0];
                        Session["FuzzySearchAllLabelList"] = labelData.Tables[1];
                        BindGrid(labelData.Tables[0]);
                        txtLabelDesc.Text = string.Empty;
                        txtLabelCode.Text = string.Empty;
                        txtLabelSearch.Text = string.Empty;
                        hdnChangeNotSaved.Value = "N";
                        hdnInsertDataNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Label created successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed create label.", MessageType.Warning, PositionType.Auto);
                    }
                }
                else if (hdnChangeNotSaved.Value == "Y")
                {
                    //Validate
                    Page.Validate("valUpdateLabel");
                    if (!Page.IsValid)
                    {
                        mpeSaveUndo.Hide();
                        msgView.SetMessage("Invalid data entered.Please correct.", MessageType.Warning, PositionType.Auto);
                        return;
                    }

                    string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                    int rowIndex = Convert.ToInt32(hdnGridRowSelectedPrvious.Value);
                    string labelCode = ((Label)gvLabelDetails.Rows[rowIndex].FindControl("lblLabelCode")).Text;
                    string labelDesc = ((TextBox)gvLabelDetails.Rows[rowIndex].FindControl("txtLabelName")).Text;

                    labelMaintenanceBL = new LabelMaintenanceBL();
                    DataSet labelData = labelMaintenanceBL.UpdateLabelData(labelCode, labelDesc, userCode, out errorId);
                    labelMaintenanceBL = null;

                    if (labelData.Tables.Count != 0 && errorId != 2)
                    {
                        Session["LabelMaintData"] = labelData.Tables[0];
                        Session["FuzzySearchAllLabelList"] = labelData.Tables[1];
                        //check if there is only one row in the grid before binding updated data.
                        //if count is 1 then only display that row
                        if (gvLabelDetails.Rows.Count == 1)
                        {
                            DataTable dtSearched = labelData.Tables[0].Clone();
                            DataRow[] foundRows = labelData.Tables[0].Select("label_code = '" + labelCode + "'");
                            if (foundRows.Length != 0)
                            {
                                dtSearched = foundRows.CopyToDataTable();
                                BindGrid(dtSearched);
                            }
                        }
                        else
                        {
                            BindGrid(labelData.Tables[0]);
                        }
                        hdnChangeNotSaved.Value = "N";
                        hdnGridRowSelectedPrvious.Value = null;
                        msgView.SetMessage("Label details updated successfully.", MessageType.Success, PositionType.Auto);
                    }
                    else
                    {
                        msgView.SetMessage("Failed to updated label details.", MessageType.Warning, PositionType.Auto);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving grid data", ex.Message);
            }
        }

        //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string labelCode = hdnLabelCode.Value;
                labelMaintenanceBL = new LabelMaintenanceBL();
                DataSet labelData = labelMaintenanceBL.DeleteLabelData(labelCode, Convert.ToString(Session["UserCode"]), out errorId);
                labelMaintenanceBL = null;

                if (errorId == 1)
                {
                    msgView.SetMessage("Can't delete label as it's being used in contract.", MessageType.Success, PositionType.Auto);
                }
                else if (labelData.Tables.Count != 0 && errorId == 0)
                {
                    Session["LabelList"] = labelData.Tables[0];
                    BindGrid(labelData.Tables[0]);
                    txtLabelSearch.Text = string.Empty;
                    hdnChangeNotSaved.Value = "N";
                    hdnGridRowSelectedPrvious.Value = null;
                    msgView.SetMessage("Label deleted successfully.", MessageType.Success, PositionType.Auto);
                    hdnLabelCode.Value = "";
                }
                else
                {
                    msgView.SetMessage("Failed to delete label.", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Failed to delete label.", ex.Message);
            }
        }
        //JIRA-908 Changes by Ravi on 13/02/2019 -- End


        protected void gvLabelDetails_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void gvLabelDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["LabelMaintData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvLabelDetails.DataSource = dataView;
                gvLabelDetails.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End
        #endregion Events

        #region Methods

        private void LoadData()
        {
            labelMaintenanceBL = new LabelMaintenanceBL();
            DataSet initialData = labelMaintenanceBL.GetInitialData(out errorId);
            labelMaintenanceBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                Session["LabelMaintData"] = initialData.Tables[0];

                BindGrid(initialData.Tables[0]);
            }
            else
            {
                ExceptionHandler("Error in fetching filter list data", string.Empty);
            }
        }

        private void LabelSelected()
        {
            if (Session["LabelMaintData"] != null)
            {
                hdnChangeNotSaved.Value = "N";
                hdnGridRowSelectedPrvious.Value = null;

                DataTable dtLabelData = (Session["LabelMaintData"] as DataTable).Copy();
                DataTable dtSearched = (Session["LabelMaintData"] as DataTable).Clone();

                DataRow[] foundRows = dtLabelData.Select("label_code = '" + txtLabelSearch.Text.Split('-')[0].ToString().Trim() + "'");
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

            if (gridData.Rows.Count > 0)
            {
                gvLabelDetails.DataSource = gridData;
                gvLabelDetails.DataBind();
                PnlLabelDetails.Style.Add("height", hdnGridPnlHeight.Value);
            }
            else
            {
                dtEmpty = new DataTable();
                gvLabelDetails.DataSource = dtEmpty;
                gvLabelDetails.DataBind();
                PnlLabelDetails.Style.Add("height", hdnGridPnlHeight.Value);
            }
            UserAuthorization();
        }

        private void FuzzySearchLabel()
        {
            if (txtLabelSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in label search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllLabelList(txtLabelSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
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
                foreach (GridViewRow rows in gvLabelDetails.Rows)
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

        #endregion Methods

        
    }
}