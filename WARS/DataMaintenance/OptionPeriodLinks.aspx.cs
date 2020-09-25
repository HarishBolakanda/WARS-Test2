/*
File Name   :   OptionPeriodLinks.cs
Purpose     :   to maintain artist, royaltor and option period links 

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     07-Sep-2016     Pratik(Infosys Limited)   Initial Creation
2.0     07-Feb-2017     Pratik(Infosys Limited)   Added new fuzzy search functionality
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
using System.Drawing;
using System.Text;

namespace WARS
{
    public partial class OperationPeriodLinks : System.Web.UI.Page
    {
        #region Global Declarations
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        OptionPeriodLinksBL optionPeriodLinksBL;
        Int32 royaltorId;
        Int32 optionPeriodCode;
        string artistName;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Artist Royaltor Option Link";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Artist Royaltor Option Link";
                }

                txtRoyaltor.Focus();//tabbing sequence starts here
                if (!IsPostBack)
                {

                    //clear sessions                    
                    Session["artistName"] = null;
                    util = new Utilities();

                    if (util.UserAuthentication())
                    {
                        btnSaveChanges.Enabled = false;
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

        protected void txtRoyaltor_TextChanged(object sender, EventArgs e)
        {
            try
            {

                if (hdnSearchListItemSelected.Value == "Y")
                {
                    RoyaltorSelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchRoyaltor();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                }
                else
                {
                    ddlOptionPeriod.Items.Clear();
                    ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                    txtRoyaltor.Text = string.Empty;
                }
                hdnSearchListItemSelected.Value = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting Royaltor", ex.Message);
            }
        }

        protected void ddlOptionPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlOptionPeriod.SelectedIndex > 0)
                {
                    royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-")));
                    optionPeriodCode = Convert.ToInt32(ddlOptionPeriod.SelectedValue);
                    LoadGridData(royaltorId, optionPeriodCode);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting option period", ex.Message);
            }
        }

        protected void txtArtist_TextChanged(object sender, EventArgs e)
        {
            try
            {

                if (hdnSearchListItemSelected.Value == "Y")
                {
                    ArtistSelected();
                }
                else if (hdnSearchListItemSelected.Value == "N")
                {
                    FuzzySearchArtist();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                }
                else
                {
                    txtArtist.Text = string.Empty;
                }
                hdnSearchListItemSelected.Value = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting artist", ex.Message);
            }
        }

        protected void gvArtistLinks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    if ((e.Row.FindControl("lblRoyaltorId") as Label).Text == string.Empty)
                    {
                        //Disabling remove and replace checkboxs where royaltor Id is null 
                        (e.Row.FindControl("cbRemoveLinks") as CheckBox).Enabled = false;
                        (e.Row.FindControl("cbReplaceLinks") as CheckBox).Enabled = false;
                    }
                    else
                    {
                        //Disabling add checkbox where royaltor Id is not null
                        (e.Row.FindControl("cbAddLinks") as CheckBox).Enabled = false;
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
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding grid data.", ex.Message);
            }
        }



        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvArtistLinks_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["OptionPeriodLinksData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvArtistLinks.DataSource = dataView;
                gvArtistLinks.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                saveChanges();
                //btnSaveChanges.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving data", ex.Message);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                txtRoyaltor.Text = string.Empty;
                ddlOptionPeriod.Items.Clear();
                txtArtist.Text = string.Empty;
                btnSaveChanges.Enabled = false;
                LoadInitialGridData();
                gvArtistLinks.EmptyDataText = string.Empty;
                Session["artistName"] = null;
                hdnChangeNotSaved.Value = "N";
                isGridPopulated.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing data", ex.Message);
            }
        }

        protected void btnSaveChangedData_Click(object sender, EventArgs e)
        {
            try
            {
                saveChanges();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving data", ex.Message);
            }
        }

        protected void btnDiscard_Click(object sender, EventArgs e)
        {
            DataSet gridData = Session["Griddata"] as DataSet;
            BindGrids(gridData);
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

        protected void fuzzySearchArtist_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchArtist();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in artist fuzzy search", ex.Message);
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
                        ddlOptionPeriod.Items.Clear();
                        ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                        txtRoyaltor.Text = string.Empty;
                        return;
                    }

                    txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();

                    RoyaltorSelected();
                }
                else if (hdnFuzzySearchField.Value == "Artist")
                {
                    if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                    {
                        txtArtist.Text = string.Empty;
                        return;
                    }

                    txtArtist.Text = lbFuzzySearch.SelectedValue.ToString();

                    ArtistSelected();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting item from search list", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "Royaltor")
                {
                    ddlOptionPeriod.Items.Clear();
                    ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                    txtRoyaltor.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "Artist")
                {
                    txtArtist.Text = string.Empty;
                }
                mpeConfirmation.Hide();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing search list", ex.Message);
            }
        }

        #endregion EVENTS

        #region METHODS

        private void LoadDropdowns()
        {
            optionPeriodLinksBL = new OptionPeriodLinksBL();
            royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-")));
            DataSet optionPeriods = optionPeriodLinksBL.GetOptionPeriods(royaltorId, Convert.ToString(Session["UserRoleId"]), out errorId); //JIRA-898 CHange
            optionPeriodLinksBL = null;
            if (optionPeriods.Tables.Count != 0 && errorId != 2)
            {
                ddlOptionPeriod.DataTextField = "option_period";
                ddlOptionPeriod.DataValueField = "opd_option";
                ddlOptionPeriod.DataSource = optionPeriods.Tables[0];
                ddlOptionPeriod.DataBind();
                ddlOptionPeriod.Items.Insert(0, new ListItem("-"));

                if (optionPeriods.Tables[0].Rows.Count == 0)
                {
                    //btnSaveChanges.Enabled = false;
                    msgView.SetMessage("No option period available for the royaltor",
                                    MessageType.Warning, PositionType.Auto);
                }
                else if (optionPeriods.Tables[0].Rows.Count == 1)
                {
                    ddlOptionPeriod.SelectedIndex = 1;
                }
            }
            else if (optionPeriods.Tables.Count == 0 && errorId != 2)
            {
                ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
            }
            else
            {
                ExceptionHandler("Error in loading option period dropdown.", string.Empty);

            }
        }

        private void LoadGridData(Int32 royaltorID, Int32 optionPeriodCode)
        {
            //Loading initial grid after selecting a period option from dropdown list
            //Checking whether the grid is empty. If yes then populate the grid. 
            if (gvArtistLinks.Rows.Count == 0)
            {
                optionPeriodLinksBL = new OptionPeriodLinksBL();
                DataSet linksData = optionPeriodLinksBL.GetIntialLinksData(royaltorId, optionPeriodCode, out errorId);
                optionPeriodLinksBL = null;
                BindGrids(linksData);
            }
        }

        private void LoadArtistGridData(string artistName)
        {
            //Populating grid when a artist is searched in artist textbox
            optionPeriodLinksBL = new OptionPeriodLinksBL();
            DataSet linksData = optionPeriodLinksBL.GetArtistLinksData(artistName, out errorId);
            optionPeriodLinksBL = null;
            BindGrids(linksData);
        }

        private void saveChanges()
        {
            optionPeriodLinksBL = new OptionPeriodLinksBL();

            CheckBox cbAddPeriod;
            CheckBox cbRemovePeriod;
            CheckBox cbReplacePeriod;

            string userCode = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
            int artistId;
            int flag = 0;
            int checkedCount = 0;

            List<Int32> addArtistIds = new List<Int32>();
            List<Int32> removeArtistIds = new List<Int32>();
            List<Int32> replaceArtistIds = new List<Int32>();

            StringBuilder displayArtistIds = new StringBuilder();

            foreach (GridViewRow gridrow in gvArtistLinks.Rows)
            {
                cbAddPeriod = (CheckBox)gridrow.FindControl("cbAddLinks");
                cbRemovePeriod = (CheckBox)gridrow.FindControl("cbRemoveLinks");
                cbReplacePeriod = (CheckBox)gridrow.FindControl("cbReplaceLinks");
                artistId = Convert.ToInt32(((Label)gridrow.FindControl("lblArtistId")).Text);

                displayArtistIds.Append(artistId + ",");

                if (cbAddPeriod.Checked)
                {
                    addArtistIds.Add(artistId);
                    checkedCount++;
                }
                else if (cbRemovePeriod.Checked & cbReplacePeriod.Checked)
                {
                    flag++;
                    checkedCount++;
                }
                else if (cbRemovePeriod.Checked & cbReplacePeriod.Checked == false)
                {
                    removeArtistIds.Add(artistId);
                    checkedCount++;
                }
                else if (cbReplacePeriod.Checked & cbRemovePeriod.Checked == false)
                {
                    replaceArtistIds.Add(artistId);
                    checkedCount++;
                }

            }

            if (checkedCount > 0)
            {
                if (flag > 0)
                {
                    msgView.SetMessage("Can't check both remove and replace checkboxes in a row",
                                        MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    if (addArtistIds.Count() > 0 || replaceArtistIds.Count() > 0)
                    {
                        if (txtRoyaltor.Text != string.Empty)
                        {
                            if (ddlOptionPeriod.SelectedIndex != 0)
                            {
                                royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-")));
                                optionPeriodCode = Convert.ToInt32(ddlOptionPeriod.SelectedValue);

                                DataSet linksData = optionPeriodLinksBL.OptionPeriodOperations(addArtistIds.ToArray(), removeArtistIds.ToArray(), replaceArtistIds.ToArray(), royaltorId, optionPeriodCode, userCode, displayArtistIds.ToString(), out errorId);
                                optionPeriodLinksBL = null;
                                BindGrids(linksData);

                                hdnChangeNotSaved.Value = "N";
                            }
                            else
                            {
                                msgView.SetMessage("Please select an option period ",
                                                            MessageType.Warning, PositionType.Auto);
                            }
                        }
                        else
                        {
                            msgView.SetMessage("Please select a royaltor and an option period",
                                                            MessageType.Warning, PositionType.Auto);
                        }

                    }
                    else
                    {
                        royaltorId = 0;
                        optionPeriodCode = 0;

                        DataSet linksData = optionPeriodLinksBL.OptionPeriodOperations(addArtistIds.ToArray(), removeArtistIds.ToArray(), replaceArtistIds.ToArray(), royaltorId, optionPeriodCode, userCode, displayArtistIds.ToString(), out errorId);
                        optionPeriodLinksBL = null;
                        BindGrids(linksData);

                        hdnChangeNotSaved.Value = "N";
                    }
                }
            }
            else
            {
                msgView.SetMessage("Please check at least one checkbox",
                                        MessageType.Warning, PositionType.Auto);
            }
            //    }
            //    else
            //    {
            //        msgView.SetMessage("Please select an option period ",
            //                                    MessageType.Warning, PositionType.Auto);
            //    }
            //}
            //else
            //{
            //    msgView.SetMessage("Please select a royaltor",
            //                                    MessageType.Warning, PositionType.Auto);
            //}
        }

        private bool ValidateSelectedRoyaltor()
        {
            if (txtRoyaltor.Text != "" && Session["FuzzySearchAllRoyaltorList"] != null)
            {
                if (txtRoyaltor.Text != "No results found")
                {
                    DataTable dtRoyaltorGroupings;
                    dtRoyaltorGroupings = Session["FuzzySearchAllRoyaltorList"] as DataTable;

                    foreach (DataRow dRow in dtRoyaltorGroupings.Rows)
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

        private bool ValidateSelectedArtist()
        {
            if (txtArtist.Text != "" && Session["FuzzySearchAllArtistList"] != null)
            {
                if (txtArtist.Text != "No results found")
                {
                    DataTable dtRoyaltorGroupings;
                    dtRoyaltorGroupings = Session["FuzzySearchAllArtistList"] as DataTable;

                    foreach (DataRow dRow in dtRoyaltorGroupings.Rows)
                    {
                        if (dRow["artist"].ToString() == txtArtist.Text)
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

        private void LoadInitialGridData()
        {
            dtEmpty = new DataTable();
            gvArtistLinks.DataSource = dtEmpty;
            gvArtistLinks.DataBind();
        }

        private void BindGrids(DataSet linksData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (linksData.Tables.Count != 0 && errorId != 2)
            {
                Session["OptionPeriodLinksData"] = linksData.Tables[0];
                gvArtistLinks.DataSource = linksData.Tables[0];
                if (linksData.Tables[0].Rows.Count == 0)
                {
                    gvArtistLinks.EmptyDataText = "No data found.";
                }
                gvArtistLinks.DataBind();

                Session["Griddata"] = linksData;

                if (gvArtistLinks.Rows.Count > 0)
                {
                    PnlArtistLinks.Style.Add("height", hdnGridPnlHeight.Value);
                    btnSaveChanges.Enabled = true;
                }
                else
                {
                    btnSaveChanges.Enabled = false;
                }

                isGridPopulated.Value = "Y";
            }
            else if (linksData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvArtistLinks.DataSource = dtEmpty;
                gvArtistLinks.EmptyDataText = "No data found.";
                gvArtistLinks.DataBind();

                btnSaveChanges.Enabled = false;
                isGridPopulated.Value = "N";
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }
            UserAuthorization();
        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnCancel.Enabled = false;
                btnSaveChangedData.Enabled = false;
                btnSaveChanges.Enabled = false;
                btnDiscard.Enabled = false;
              
            }
        }


        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);

            //util = new Utilities();
            //util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            //util = null;
        }

        private void FuzzySearchRoyaltor()
        {

            if (txtRoyaltor.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Royaltor";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(txtRoyaltor.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchArtist()
        {
            if (txtArtist.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in artist filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "Artist";
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllArtisList(txtArtist.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void RoyaltorSelected()
        {
            if (ValidateSelectedRoyaltor())
            {
                LoadDropdowns();

                //if the option period count is 1, then populate the grid
                if (ddlOptionPeriod.Items.Count == 2)
                {
                    royaltorId = Convert.ToInt32(txtRoyaltor.Text.Substring(0, txtRoyaltor.Text.IndexOf("-")));
                    optionPeriodCode = Convert.ToInt32(ddlOptionPeriod.SelectedValue);
                    LoadGridData(royaltorId, optionPeriodCode);
                    txtArtist.Enabled = true;
                }

            }
            else
            {
                ddlOptionPeriod.Items.Clear();
                ddlOptionPeriod.Items.Insert(0, new ListItem("-"));
                txtRoyaltor.Text = string.Empty;
            }

        }

        private void ArtistSelected()
        {
            if (ValidateSelectedArtist())
            {
                artistName = txtArtist.Text.Substring(txtArtist.Text.IndexOf("-") + 1);
                LoadArtistGridData(artistName);
                Session["artistName"] = artistName;
            }
            else
            {
                txtArtist.Text = string.Empty;
            }
            //btnSaveChanges.Focus();
        }

        #endregion METHODS


    }
}