/*
File Name   :   TransactionRetrieval.cs
Purpose     :   used for selecting Catalogue numbers to be used to retrieve Transactions from the Transaction Archive

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     26-Oct-2016     Harish(Infosys Limited)   Initial Creation
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
    public partial class TransactionRetrieval : System.Web.UI.Page
    {
        #region Global Declarations

        TransactionRetrievalBL transRetBL;
        string loggedUserID;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string ASCENDING = " ASC";
        string DESCENDING = " DESC";
        int gridDefaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridDefaultPageSize2"].ToString());
        #endregion Global Declarations

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Transaction Retrieval";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Transaction Retrieval";
                }

                lblTab.Focus();//tabbing sequence starts here
                if (!IsPostBack)
                {
                    ClearSessions();
                    btnRetrieveTrans.Enabled = false;
                    btnAddToSelected.Enabled = false;
                    btnRemoveFromSelected.Enabled = false;
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

        protected void btnAddToSelected_Click(object sender, ImageClickEventArgs e)
        {
            try
            {

                DataTable dtNotSelectedToRet;
                DataTable dtSelectedToRet;

                if (Session["TransRetDtNotSelToRet"] == null)
                {
                    ExceptionHandler("Error in adding to selected group", string.Empty);
                }
                else
                {
                    dtNotSelectedToRet = (DataTable)Session["TransRetDtNotSelToRet"];

                    if (Session["TransRetDtSelectedToRet"] != null)
                    {
                        dtSelectedToRet = (DataTable)Session["TransRetDtSelectedToRet"];
                    }
                    else
                    {
                        dtSelectedToRet = new DataTable();
                        dtSelectedToRet = dtNotSelectedToRet.Clone();
                    }

                    bool isSelected = false;
                    Label lblCatNoNotSelec;
                    CheckBox cbCatNotSelec;
                    foreach (GridViewRow gvr in gvCatNotSelected.Rows)
                    {
                        cbCatNotSelec = (CheckBox)gvr.FindControl("cbCatNotSelec");

                        if (cbCatNotSelec.Checked == true)
                        {
                            isSelected = true;
                            lblCatNoNotSelec = (Label)gvr.FindControl("lblCatNoNotSelec");

                            DataRow[] dt01 = dtNotSelectedToRet.Select("catno='" + lblCatNoNotSelec.Text + "'");
                            foreach (DataRow dr01 in dt01)
                            {
                                if (dtSelectedToRet.Rows.Count == 0)
                                {
                                    dtSelectedToRet.ImportRow(dr01);
                                }
                                else
                                {
                                    //check if selected item already exist in selected grid
                                    DataRow[] dt02 = dtSelectedToRet.Select("catno='" + dr01["catno"].ToString() + "'");
                                    if (dt02.Count() == 0)
                                    {
                                        dtSelectedToRet.ImportRow(dr01);
                                    }

                                }

                                dtNotSelectedToRet.Rows.Remove(dr01);
                            }


                        }
                    }

                    if (!isSelected)
                    {
                        msgView.SetMessage("Please select a catalogue from the not selected list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {
                        Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtNotSelectedToRet, gridDefaultPageSize, gvCatNotSelected, dtEmpty, rptPager);
                        //set gridview panel height                    
                        PnlGridCatSelected.Style.Add("height", hdnGridPnlHeight.Value);

                        if (dtSelectedToRet.Rows.Count == 0)
                        {
                            ClearSelGridData();
                        }
                        else
                        {
                            //sort data according to the current sort order
                            if (SelGridSortColumn == NotSelGridSortColumns.None.ToString())
                            {
                                SelGridSortDirection = GridSortDirections.Ascending.ToString();
                                dtSelectedToRet.DefaultView.Sort = "catno" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                            }
                            else if (SelGridSortColumn == NotSelGridSortColumns.CatNo.ToString())
                            {
                                dtSelectedToRet.DefaultView.Sort = "catno" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                            }
                            else if (SelGridSortColumn == NotSelGridSortColumns.Title.ToString())
                            {
                                dtSelectedToRet.DefaultView.Sort = "catno_title" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                            }
                            else if (SelGridSortColumn == NotSelGridSortColumns.Artist.ToString())
                            {
                                dtSelectedToRet.DefaultView.Sort = "artist_name" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                            }

                            gvCatSelected.DataSource = dtSelectedToRet;
                            gvCatSelected.DataBind();
                        }

                        Session["TransRetDtNotSelToRet"] = dtNotSelectedToRet;
                        Session["TransRetDtSelectedToRet"] = dtSelectedToRet;

                        //removed by harish as it is causing issue with selected all and added - harish - 06-02-2017
                        //if (dtNotSelectedToRet.Rows.Count == 0)
                        //{
                        //    txtCatNum.Text = string.Empty;
                        //    txtRoyOptPrd.Text = string.Empty;
                        //    txtCatArtist.Text = string.Empty;
                        //    txtProjCode.Text = string.Empty;
                        //}

                    }
                }

                EnableButtons();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding to selected group", ex.Message);
            }
        }

        protected void btnRemoveFromSelected_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                DataTable dtNotSelectedToRet;
                DataTable dtSelectedToRet;

                if (Session["TransRetDtSelectedToRet"] == null)
                {
                    msgView.SetMessage("Please select a catalogue from the selected list", MessageType.Warning, PositionType.Auto);
                    return;
                }
                else
                {
                    dtSelectedToRet = (DataTable)Session["TransRetDtSelectedToRet"];

                    if (Session["TransRetDtNotSelToRet"] != null)
                    {
                        dtNotSelectedToRet = (DataTable)Session["TransRetDtNotSelToRet"];
                    }
                    else
                    {
                        dtNotSelectedToRet = new DataTable();
                        dtNotSelectedToRet = dtSelectedToRet.Clone();
                    }

                    bool isSelected = false;
                    Label lblCatNoSelec;
                    CheckBox cbCatSelec;
                    foreach (GridViewRow gvr in gvCatSelected.Rows)
                    {
                        cbCatSelec = (CheckBox)gvr.FindControl("cbCatSelec");

                        if (cbCatSelec.Checked == true)
                        {
                            isSelected = true;
                            lblCatNoSelec = (Label)gvr.FindControl("lblCatNoSelec");

                            DataRow[] dt01 = dtSelectedToRet.Select("catno='" + lblCatNoSelec.Text + "'");
                            foreach (DataRow dr01 in dt01)
                            {
                                dtNotSelectedToRet.ImportRow(dr01);
                                dtSelectedToRet.Rows.Remove(dr01);
                            }


                        }
                    }

                    if (!isSelected)
                    {
                        msgView.SetMessage("Please select a catalogue from the selected list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {
                        if (dtNotSelectedToRet.Rows.Count == 0)
                        {
                            ClearNotSelGridData();
                        }
                        else
                        {
                            //sort data according to the current sort order                            
                            if (NotSelGridSortColumn == NotSelGridSortColumns.None.ToString())
                            {
                                NotSelGridSortDirection = GridSortDirections.Ascending.ToString();
                                dtNotSelectedToRet.DefaultView.Sort = "catno" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                            }
                            else if (NotSelGridSortColumn == NotSelGridSortColumns.CatNo.ToString())
                            {
                                dtNotSelectedToRet.DefaultView.Sort = "catno" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                            }
                            else if (NotSelGridSortColumn == NotSelGridSortColumns.Title.ToString())
                            {
                                dtNotSelectedToRet.DefaultView.Sort = "catno_title" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                            }
                            else if (NotSelGridSortColumn == NotSelGridSortColumns.Artist.ToString())
                            {
                                dtNotSelectedToRet.DefaultView.Sort = "artist_name" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                            }
                            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dtNotSelectedToRet, gridDefaultPageSize, gvCatNotSelected, dtEmpty, rptPager);
                        }

                        gvCatSelected.DataSource = dtSelectedToRet;
                        gvCatSelected.DataBind();

                        Session["TransRetDtNotSelToRet"] = dtNotSelectedToRet;
                        Session["TransRetDtSelectedToRet"] = dtSelectedToRet;


                    }
                }

                EnableButtons();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in removing from selected group", ex.Message);
            }
        }

        protected void btnRetrieveTrans_Click(object sender, EventArgs e)
        {
            try
            {
                //validate 
                if (!Page.IsValid)
                {
                    return;
                }

                if (gvCatSelected.Rows.Count == 0)
                {
                    msgView.SetMessage("Please select a catalogue to the selected list", MessageType.Warning, PositionType.Auto);
                    return;
                }

                List<string> catsToRetrieve = new List<string>();

                Label lblCatNoSelec;
                foreach (GridViewRow gvr in gvCatSelected.Rows)
                {
                    lblCatNoSelec = (Label)gvr.FindControl("lblCatNoSelec");
                    catsToRetrieve.Add(lblCatNoSelec.Text);
                }

                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string royaltorId = (txtRoyOptPrd.Text == "" ? string.Empty : txtRoyOptPrd.Text.Split('-')[0].Trim());
                string optionPeriodCode = (txtRoyOptPrd.Text == "" ? string.Empty : txtRoyOptPrd.Text.Split('-')[1].Trim());
                string artistId = (txtCatArtist.Text == "" ? string.Empty : txtCatArtist.Text.Split('-')[0].Trim());
                string catNumber = txtCatNum.Text.Replace("'", "").Trim();//JIRA-1048 Changes to handle single quote  
                string projCode = (txtProjCode.Text == "" ? string.Empty : txtProjCode.Text.Split('-')[0].Trim());

                transRetBL = new TransactionRetrievalBL();
                DataSet catSearchData = transRetBL.AddToRetrieveTrans(catsToRetrieve.ToArray(), txtFromDate.Text, txtToDate.Text, royaltorId, optionPeriodCode, artistId, catNumber.ToUpper(),
                                                                        projCode, loggedUserID, "N", out errorId);
                transRetBL = null;
                if (errorId == 1)
                {
                    mpeConfirm.Show();
                    lblConfirmMsg.Text = "One or more Catalogue number already selected. Do you want to overwrite existing details?";
                    return;
                }
                else if (catSearchData.Tables.Count != 0 && errorId != 2)
                {
                    if (catSearchData.Tables[0].Rows.Count == 0)
                    {
                        gvCatNotSelected.EmptyDataText = "No data found for the selected filter criteria";
                    }
                    else
                    {
                        msgView.SetMessage("Transaction Retrieval request generated.  This will be processed prior to the next Scheduled Engine Process.", MessageType.Warning, PositionType.Auto);
                    }

                    Session["TransRetDtNotSelToRet"] = catSearchData.Tables[0];
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), catSearchData.Tables[0], gridDefaultPageSize, gvCatNotSelected, dtEmpty, rptPager);
                }
                else if (catSearchData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvCatNotSelected.DataSource = dtEmpty;
                    gvCatNotSelected.EmptyDataText = "No data found for the selected filter criteria";
                    gvCatNotSelected.DataBind();
                }
                else
                {
                    ExceptionHandler("Error in loading grid data.", string.Empty);
                }

                dtEmpty = new DataTable();
                gvCatSelected.EmptyDataText = "<br />";
                gvCatSelected.DataSource = dtEmpty;
                gvCatSelected.DataBind();

                Session["TransRetDtSelectedToRet"] = null;

                EnableButtons();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding to retrieve transactions", ex.Message);
            }
        }

        protected void txtRoyOptPrd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnSelToRetSearchSelected.Value == "Y")
                {
                    //clearing other filters so that at once only one filter is allowed to search
                    //txtCatArtist.Text = string.Empty;
                    //txtCatNum.Text = string.Empty;
                    //txtProjCode.Text = string.Empty;

                    if (( txtRoyOptPrd.Text == "No results found" || hdnNotSelGridFilterSelected.Value == "N")
                        && !IsNotSelGridFiltersEmpty())
                    {
                        txtRoyOptPrd.Text = string.Empty;
                        dtEmpty = new DataTable();
                        gvCatNotSelected.EmptyDataText = "<br />";
                        gvCatNotSelected.DataSource = dtEmpty;
                        gvCatNotSelected.DataBind();
                        msgView.SetMessage("Please select a valid royaltor & option period from the list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {
                        LoadNotSelecToRetGrid();
                    }
                }
                else if (hdnSelToRetSearchSelected.Value == "N")
                {
                    FuzzySearchRoyOptPrd();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

                }
                else if (hdnSelToRetSearchSelected.Value == string.Empty)
                {
                    LoadInitialGridData();
                }              
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void txtCatArtist_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnSelToRetSearchSelected.Value == "Y")
                {
                    //clearing other filters so that at once only one filter is allowed to search
                    //txtRoyOptPrd.Text = string.Empty;
                    //txtCatNum.Text = string.Empty;
                    //txtProjCode.Text = string.Empty;

                    if ((txtCatArtist.Text == "No results found" || hdnNotSelGridFilterSelected.Value == "N")
                        && !IsNotSelGridFiltersEmpty())
                    {
                        txtCatArtist.Text = string.Empty;
                        dtEmpty = new DataTable();
                        gvCatNotSelected.EmptyDataText = "<br />";
                        gvCatNotSelected.DataSource = dtEmpty;
                        gvCatNotSelected.DataBind();
                        msgView.SetMessage("Please select a valid artist from the list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {
                        LoadNotSelecToRetGrid();
                    }
                }
                else if (hdnSelToRetSearchSelected.Value == "N")
                {
                    FuzzySearchCatArtist();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

                }
                else if (hdnSelToRetSearchSelected.Value == string.Empty)
                {
                    LoadInitialGridData();
                }                
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void txtProjCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnSelToRetSearchSelected.Value == "Y")
                {
                    //clearing other filters so that at once only one filter is allowed to search
                    //txtCatArtist.Text = string.Empty;
                    //txtRoyOptPrd.Text = string.Empty;
                    //txtCatNum.Text = string.Empty;

                    if ((txtProjCode.Text == "No results found" || hdnNotSelGridFilterSelected.Value == "N")
                        && !IsNotSelGridFiltersEmpty())
                    {
                        txtProjCode.Text = string.Empty;
                        dtEmpty = new DataTable();
                        gvCatNotSelected.EmptyDataText = "<br />";
                        gvCatNotSelected.DataSource = dtEmpty;
                        gvCatNotSelected.DataBind();
                        msgView.SetMessage("Please select a valid project number from the list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {
                        LoadNotSelecToRetGrid();
                    }
                }
                else if (hdnSelToRetSearchSelected.Value == "N")
                {
                    FuzzySearchProjCode();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

                }
                else if (hdnSelToRetSearchSelected.Value == string.Empty)
                {
                    LoadInitialGridData();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void valFrmToDates_ServerValidate(object source, ServerValidateEventArgs args)
        {

            //WOS-450 - validation - The Transaction Retrieval should allow either one of the dates or both or none

            DateTime fromdate = DateTime.MinValue;
            DateTime todate = DateTime.MinValue;
            Int32 frmDateYear = int.MinValue;
            Int32 frmDateMonth = int.MinValue;
            Int32 toDateYear = int.MinValue;
            Int32 toDateMonth = int.MinValue;


            if (txtFromDate.Text.Trim() != "" && txtFromDate.Text.Trim() != "__/____")
            {
                frmDateYear = Convert.ToInt32(txtFromDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                frmDateMonth = Convert.ToInt32(txtFromDate.Text.Replace('_', ' ').Split('/')[0].Trim());
                //validate - month and year are valid
                if (!(frmDateMonth > 0 && frmDateMonth < 13) || !(frmDateYear > 1900))
                {
                    valFrmToDates.ErrorMessage = "Please enter valid from date in mm/yyyy format!";
                    args.IsValid = false;
                    return;
                }
                else
                {
                    fromdate = Convert.ToDateTime(txtFromDate.Text);
                }

            }

            if (txtToDate.Text.Trim() != "" && txtToDate.Text.Trim() != "__/____")
            {
                //validation - check for valid date mm/yyyy
                toDateYear = Convert.ToInt32(txtToDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                toDateMonth = Convert.ToInt32(txtToDate.Text.Replace('_', ' ').Split('/')[0].Trim());
                if (!(toDateMonth > 0 && toDateMonth < 13) || !(toDateYear > 1900))
                {
                    valFrmToDates.ErrorMessage = "Please enter valid to date in mm/yyyy format!";
                    args.IsValid = false;
                    return;
                }
                else
                {
                    todate = Convert.ToDateTime(txtToDate.Text);
                }

            }


            if ((txtFromDate.Text.Trim() != "" && txtToDate.Text.Trim() != "") && (txtFromDate.Text != "__/____" && txtToDate.Text != "__/____"))
            {
                //validate - from_date should be earlier than the to_date
                if ((frmDateYear > toDateYear) || (frmDateYear == toDateYear && frmDateMonth > toDateMonth))
                {
                    valFrmToDates.ErrorMessage = "Start date should be earlier than the end date!";
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = true;
                }

            }
        }

        protected void btnCofirmYes_Click(object sender, EventArgs e)
        {
            try
            {
                //validate 
                if (!Page.IsValid)
                {
                    return;
                }

                if (gvCatSelected.Rows.Count == 0)
                {
                    msgView.SetMessage("Please select a catalogue to the selected list", MessageType.Warning, PositionType.Auto);
                    return;
                }

                List<string> catsToRetrieve = new List<string>();

                Label lblCatNoSelec;
                foreach (GridViewRow gvr in gvCatSelected.Rows)
                {
                    lblCatNoSelec = (Label)gvr.FindControl("lblCatNoSelec");
                    catsToRetrieve.Add(lblCatNoSelec.Text);
                }

                loggedUserID = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                string royaltorId = (txtRoyOptPrd.Text == "" ? string.Empty : txtRoyOptPrd.Text.Split('-')[0].Trim());
                string optionPeriodCode = (txtRoyOptPrd.Text == "" ? string.Empty : txtRoyOptPrd.Text.Split('-')[1].Trim());
                string artistId = (txtCatArtist.Text == "" ? string.Empty : txtCatArtist.Text.Split('-')[0].Trim());
                string catNumber = txtCatNum.Text.Replace("'", "").Trim(); //JIRA-1048 Changes to handle single quote 
                string projCode = (txtProjCode.Text == "" ? string.Empty : txtProjCode.Text.Split('-')[0].Trim());



                transRetBL = new TransactionRetrievalBL();
                DataSet catSearchData = transRetBL.AddToRetrieveTrans(catsToRetrieve.ToArray(), txtFromDate.Text, txtToDate.Text, royaltorId, optionPeriodCode, artistId, catNumber,
                                                                        projCode, loggedUserID, "Y", out errorId);
                transRetBL = null;

                if (errorId == 1)
                {
                    mpeConfirm.Show();
                    lblConfirmMsg.Text = "One or more Catalogue number already selected. Do you want to overwrite existing details?";
                    return;
                }
                else if (catSearchData.Tables.Count != 0 && errorId != 2)
                {
                    if (catSearchData.Tables[0].Rows.Count == 0)
                    {
                        gvCatNotSelected.EmptyDataText = "No data found for the selected filter criteria";
                    }
                    else
                    {
                        msgView.SetMessage("Transaction Retrieval request generated.  This will be processed prior to the next Scheduled Engine Process.", MessageType.Warning, PositionType.Auto);
                    }

                    Session["TransRetDtNotSelToRet"] = catSearchData.Tables[0];
                    Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), catSearchData.Tables[0], gridDefaultPageSize, gvCatNotSelected, dtEmpty, rptPager);
                }
                else if (catSearchData.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvCatNotSelected.DataSource = dtEmpty;
                    gvCatNotSelected.EmptyDataText = "No data found for the selected filter criteria";
                    gvCatNotSelected.DataBind();
                }
                else
                {
                    ExceptionHandler("Error in loading grid data.", string.Empty);
                }

                dtEmpty = new DataTable();
                gvCatSelected.EmptyDataText = "<br />";
                gvCatSelected.DataSource = dtEmpty;
                gvCatSelected.DataBind();

                Session["TransRetDtSelectedToRet"] = null;

                EnableButtons();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding to retrieve transactions", ex.Message);
            }
        }

        protected void fuzzySearchRoyOptPrd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchRoyOptPrd();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void fuzzySearchCatArtist_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchCatArtist();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in royaltor fuzzy search", ex.Message);
            }
        }

        protected void fuzzySearchProjCode_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchProjCode();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

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
                if (hdnFuzzySearchField.Value == "RoyOptPrd")
                {
                    hdnNotSelGridFilterSelected.Value = "Y";
                    txtRoyOptPrd.Text = lbFuzzySearch.SelectedValue.ToString();
                    if ((txtRoyOptPrd.Text == string.Empty || txtRoyOptPrd.Text == "No results found" || hdnNotSelGridFilterSelected.Value == "N")
                        && !IsNotSelGridFiltersEmpty())
                    {
                        txtRoyOptPrd.Text = string.Empty;
                        msgView.SetMessage("Please select a valid royaltor & option period from the list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {

                        LoadNotSelecToRetGrid();
                    }
                }
                else if (hdnFuzzySearchField.Value == "CatArtist")
                {
                    hdnNotSelGridFilterSelected.Value = "Y";
                    txtCatArtist.Text = lbFuzzySearch.SelectedValue.ToString();
                    if ((txtCatArtist.Text == string.Empty || txtCatArtist.Text == "No results found" || hdnNotSelGridFilterSelected.Value == "N")
                        && !IsNotSelGridFiltersEmpty())
                    {
                        txtCatArtist.Text = string.Empty;
                        msgView.SetMessage("Please select a valid artist from the list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {
                        LoadNotSelecToRetGrid();
                    }

                }
                else if (hdnFuzzySearchField.Value == "ProjCode")
                {
                    hdnNotSelGridFilterSelected.Value = "Y";
                    txtProjCode.Text = lbFuzzySearch.SelectedValue.ToString();
                    if ((txtProjCode.Text == string.Empty || txtProjCode.Text == "No results found" || hdnNotSelGridFilterSelected.Value == "N")
                        && !IsNotSelGridFiltersEmpty())
                    {
                        txtProjCode.Text = string.Empty;
                        msgView.SetMessage("Please select a valid project number from the list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {
                        LoadNotSelecToRetGrid();
                    }

                }
                else if (hdnFuzzySearchField.Value == "CatNum")
                {
                    hdnNotSelGridFilterSelected.Value = "Y";
                    txtCatNum.Text = lbFuzzySearch.SelectedValue.ToString();
                    if ((txtCatNum.Text == string.Empty || txtCatNum.Text == "No results found" || hdnNotSelGridFilterSelected.Value == "N")
                        && !IsNotSelGridFiltersEmpty())
                    {
                        txtCatNum.Text = string.Empty;
                        msgView.SetMessage("Please select a valid catalogue number from the list", MessageType.Warning, PositionType.Auto);
                        return;
                    }
                    else
                    {
                        LoadNotSelecToRetGrid();
                    }

                }


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
                if (hdnFuzzySearchField.Value == "RoyOptPrd")
                {
                    txtRoyOptPrd.Text = string.Empty;
                    LoadNotSelecToRetGrid();
                }
                else if (hdnFuzzySearchField.Value == "CatArtist")
                {
                    txtCatArtist.Text = string.Empty;
                    LoadNotSelecToRetGrid();
                }
                else if (hdnFuzzySearchField.Value == "ProjCode")
                {
                    txtProjCode.Text = string.Empty;
                    LoadNotSelecToRetGrid();
                }
                else if (hdnFuzzySearchField.Value == "CatNum")
                {
                    txtCatNum.Text = string.Empty;
                    LoadNotSelecToRetGrid();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing fuzzy search pop up.", ex.Message);
            }
        }

        protected void cbCatNotSelecSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox cbCatNotSelecSelectAll = (CheckBox)sender;
                CheckBox cbCatNotSelec;
                foreach (GridViewRow gvr in gvCatNotSelected.Rows)
                {
                    cbCatNotSelec = (CheckBox)gvr.FindControl("cbCatNotSelec");
                    if (cbCatNotSelecSelectAll.Checked == true)
                        cbCatNotSelec.Checked = true;
                    else
                        cbCatNotSelec.Checked = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting all in not selected group", ex.Message);
            }

        }

        protected void cbCatSelecSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox cbCatSelecSelectAll = (CheckBox)sender;
                CheckBox cbCatSelec;
                foreach (GridViewRow gvr in gvCatSelected.Rows)
                {
                    cbCatSelec = (CheckBox)gvr.FindControl("cbCatSelec");
                    if (cbCatSelecSelectAll.Checked == true)
                        cbCatSelec.Checked = true;
                    else
                        cbCatSelec.Checked = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting all in selected group", ex.Message);
            }
        }

        protected void gvCatNotSelected_DataBound(object sender, EventArgs e)
        {
            try
            {
                cbCatNotSelecSelectAll.Checked = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data in not selected group grid", ex.Message);
            }
        }

        protected void gvCatSelected_DataBound(object sender, EventArgs e)
        {
            try
            {
                cbCatSelecSelectAll.Checked = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in binding data in selected group grid", ex.Message);
            }
        }

        protected void btnCatNumNotSelecSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnSelToRetSearchSelected.Value == "Y")
                {
                    //clearing other filters so that at once only one filter is allowed to search
                    //txtCatArtist.Text = string.Empty;
                    //txtRoyOptPrd.Text = string.Empty;
                    //txtProjCode.Text = string.Empty;
                    
                    LoadNotSelecToRetGrid();                   
                }        

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtNotSelToRet"] == null)
                    return;

                DataTable dtTransRetDtNotSelToRet = Session["TransRetDtNotSelToRet"] as DataTable;
                if (dtTransRetDtNotSelToRet.Rows.Count == 0)
                    return;
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = Convert.ToString(pageIndex);
                Utilities.PopulateGridPage(pageIndex, dtTransRetDtNotSelToRet, gridDefaultPageSize, gvCatNotSelected, dtEmpty, rptPager);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }
        protected void gvCatNotSelected_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //string catNoNotSelec = (e.Row.FindControl("lblCatNoNotSelec") as Label).Text;
                //if (catNoNotSelec.Length > 14)
                //{
                //    (e.Row.FindControl("lblCatNoNotSelec") as Label).Text = catNoNotSelec.Substring(0, 14) + "...";
                //}

                //string titleNotSelected = (e.Row.FindControl("lblTitleNotSelec") as Label).Text;
                //if (titleNotSelected.Length > 25)
                //{
                //    (e.Row.FindControl("lblTitleNotSelec") as Label).Text = titleNotSelected.Substring(0, 25) + "...";
                //}

                //string artistNotSelected = (e.Row.FindControl("lblArtistNotSelec") as Label).Text;
                //if (artistNotSelected.Length > 18)
                //{
                //    (e.Row.FindControl("lblArtistNotSelec") as Label).Text = artistNotSelected.Substring(0, 18) + "...";
                //}

                //string projectNotSelected = (e.Row.FindControl("lblProjCodeNotSelec") as Label).Text;
                //if (projectNotSelected.Length > 15)
                //{
                //    (e.Row.FindControl("lblProjCodeNotSelec") as Label).Text = projectNotSelected.Substring(0, 15) + "...";
                //}
            }
        }

        protected void gvCatSelected_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //string catNoSelec = (e.Row.FindControl("lblCatNoSelec") as Label).Text;
                //if (catNoSelec.Length > 14)
                //{
                //    (e.Row.FindControl("lblCatNoSelec") as Label).Text = catNoSelec.Substring(0, 14) + "...";
                //}

                //string titleSelected = (e.Row.FindControl("lblTitleSelec") as Label).Text;
                //if (titleSelected.Length > 25)
                //{
                //    (e.Row.FindControl("lblTitleSelec") as Label).Text = titleSelected.Substring(0, 25) + "...";
                //}

                //string artistSelected = (e.Row.FindControl("lblArtistSelec") as Label).Text;
                //if (artistSelected.Length > 18)
                //{
                //    (e.Row.FindControl("lblArtistSelec") as Label).Text = artistSelected.Substring(0, 18) + "...";
                //}

                //string projectSelected = (e.Row.FindControl("lblProjCodeSelec") as Label).Text;
                //if (projectSelected.Length > 15)
                //{
                //    (e.Row.FindControl("lblProjCodeSelec") as Label).Text = projectSelected.Substring(0, 15) + "...";
                //}
            }
        }

        protected void gvCatNotSelected_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                if (Session["TransRetDtNotSelToRet"] == null)
                    return;

                DataTable dtTransRetDtNotSelToRet = Session["TransRetDtNotSelToRet"] as DataTable;
                if (dtTransRetDtNotSelToRet.Rows.Count == 0)
                    return;

                gvCatNotSelected.PageIndex = e.NewPageIndex;
                gvCatNotSelected.DataSource = dtTransRetDtNotSelToRet;
                gvCatNotSelected.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        //protected void gvCatSelected_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    try
        //    {
        //        if (Session["TransRetDtSelectedToRet"] == null)
        //            return;

        //        DataTable dtTransRetDtSelectedToRet = Session["TransRetDtSelectedToRet"] as DataTable;
        //        if (dtTransRetDtSelectedToRet.Rows.Count == 0)
        //            return;

        //        gvCatSelected.PageIndex = e.NewPageIndex;
        //        gvCatSelected.DataSource = dtTransRetDtSelectedToRet;
        //        gvCatSelected.DataBind();

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler("Error in page change.", ex.Message);
        //    }
        //}

        #endregion EVENTS

        #region METHODS

        private void UserAuthorization()
        {

            //WUIN-1096 Only Read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnRetrieveTrans.Enabled = false;
                btnAddToSelected.Enabled = false;
                btnRemoveFromSelected.Enabled = false;
            }

        }

        private void EnableButtons()
        {
            if (gvCatNotSelected.Rows.Count > 0)
            {
                btnAddToSelected.Enabled = true;
            }
            else
            {
                btnAddToSelected.Enabled = false;
            }

            if (gvCatSelected.Rows.Count > 0)
            {
                btnRemoveFromSelected.Enabled = true;
                btnRetrieveTrans.Enabled = true;
            }
            else
            {
                btnRemoveFromSelected.Enabled = false;
                btnRetrieveTrans.Enabled = false;
            }
            UserAuthorization();

        }

        private void ClearSessions()
        {
            Session["TransRetDtNotSelToRet"] = null;
            Session["TransRetDtSelectedToRet"] = null;
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void LoadInitialGridData()
        {
            hdnPageNumber.Value = "1";
            dtEmpty = new DataTable();
            gvCatNotSelected.EmptyDataText = "<br />";
            gvCatNotSelected.DataSource = dtEmpty;
            gvCatNotSelected.DataBind();

            gvCatSelected.EmptyDataText = "<br />";
            gvCatSelected.DataSource = dtEmpty;
            gvCatSelected.DataBind();

        }

        private void ClearNotSelGridData()
        {
            dtEmpty = new DataTable();
            gvCatNotSelected.EmptyDataText = "<br />";
            gvCatNotSelected.DataSource = dtEmpty;
            gvCatNotSelected.DataBind();

            Session["TransRetDtNotSelToRet"] = null;
            EnableButtons();
        }

        private void ClearSelGridData()
        {
            dtEmpty = new DataTable();
            gvCatSelected.EmptyDataText = "<br />";
            gvCatSelected.DataSource = dtEmpty;
            gvCatSelected.DataBind();

            Session["TransRetDtSelectedToRet"] = null;
            EnableButtons();
        }

        private void LoadNotSelecToRetGrid()
        {
            if (txtRoyOptPrd.Text == "" && txtCatArtist.Text == "" && txtCatNum.Text == "" && txtProjCode.Text == "")
            {
                ClearNotSelGridData();
                return;
            }

            //set gridview panel height                    
            PnlGridCatNotSelected.Style.Add("height", hdnGridPnlHeight.Value);

            string royaltorId = (txtRoyOptPrd.Text == "" ? string.Empty : txtRoyOptPrd.Text.Split('-')[0].Trim());
            string optionPeriodCode = (txtRoyOptPrd.Text == "" ? string.Empty : txtRoyOptPrd.Text.Split('-')[1].Trim());
            string artistId = (txtCatArtist.Text == "" ? string.Empty : txtCatArtist.Text.Split('-')[0].Trim());
            string catNumber = txtCatNum.Text.Replace("'", "").Trim();//JIRA-1048 Changes to handle single quote 
            string projCode = (txtProjCode.Text == "" ? string.Empty : txtProjCode.Text.Split('-')[0].Trim());

            transRetBL = new TransactionRetrievalBL();
            DataSet catSearchData = transRetBL.GetSearchData(royaltorId, optionPeriodCode, artistId, catNumber.ToUpper(), projCode, out errorId);
            transRetBL = null;
            hdnPageNumber.Value = "1";
            if (catSearchData.Tables.Count != 0 && errorId != 2)
            {
                if (catSearchData.Tables[0].Rows.Count == 0)
                {
                    gvCatNotSelected.DataSource = catSearchData.Tables[0];
                    gvCatNotSelected.EmptyDataText = "No data found for the selected filter criteria";
                    gvCatNotSelected.DataBind();
                }
                else
                {
                    if (Session["TransRetDtSelectedToRet"] != null)
                    {
                        DataTable dtSelectedToRet = (DataTable)Session["TransRetDtSelectedToRet"];
                        DataTable dtNotSelectedToRet = catSearchData.Tables[0];

                        foreach (DataRow drow1 in dtSelectedToRet.Rows)
                        {
                            foreach (DataRow drow2 in dtNotSelectedToRet.Rows)
                            {
                                if (drow1["catno"].ToString() == drow2["catno"].ToString())
                                {
                                    dtNotSelectedToRet.Rows.Remove(drow2);
                                    break;
                                }
                            }
                        }

                        gvCatNotSelected.DataSource = dtNotSelectedToRet;
                        gvCatNotSelected.DataBind();
                    }
                    else
                    {
                        Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), catSearchData.Tables[0], gridDefaultPageSize, gvCatNotSelected, dtEmpty, rptPager);
                    }

                }

                Session["TransRetDtNotSelToRet"] = catSearchData.Tables[0];
            }
            else if (catSearchData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvCatNotSelected.DataSource = dtEmpty;
                gvCatNotSelected.EmptyDataText = "No data found for the selected filter criteria";
                gvCatNotSelected.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }

            EnableButtons();
            NotSelGridSortColumn = NotSelGridSortColumns.None.ToString();

        }

        private bool IsNotSelGridFiltersEmpty()
        {

            if (txtRoyOptPrd.Text == string.Empty && txtCatArtist.Text == string.Empty && txtProjCode.Text == string.Empty && txtCatNum.Text == string.Empty)
                return true;
            else
                return false;
        }

        private void FuzzySearchRoyOptPrd()
        {
            //if (txtRoyOptPrd.Text.Trim() == string.Empty)
            //{
            //    dtEmpty = new DataTable();
            //    gvCatNotSelected.EmptyDataText = "<br />";
            //    gvCatNotSelected.DataSource = dtEmpty;
            //    gvCatNotSelected.DataBind();
            //    msgView.SetMessage("Please enter a text in Royaltor & Option period filter field", MessageType.Warning, PositionType.Auto);
            //    return;
            //}

            hdnFuzzySearchField.Value = "RoyOptPrd";            
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyTransRetRoyOpPrdList(txtRoyOptPrd.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchCatArtist()
        {
            //if (txtCatArtist.Text.Trim() == string.Empty)
            //{
            //    dtEmpty = new DataTable();
            //    gvCatNotSelected.EmptyDataText = "<br />";
            //    gvCatNotSelected.DataSource = dtEmpty;
            //    gvCatNotSelected.DataBind();
            //    msgView.SetMessage("Please enter a text in Catalogue Artist / Name filter field", MessageType.Warning, PositionType.Auto);
            //    return;
            //}

            hdnFuzzySearchField.Value = "CatArtist";          
            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyTransRetArtistList(txtCatArtist.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchProjCode()
        {
            //if (txtProjCode.Text.Trim() == string.Empty)
            //{
            //    dtEmpty = new DataTable();
            //    gvCatNotSelected.EmptyDataText = "<br />";
            //    gvCatNotSelected.DataSource = dtEmpty;
            //    gvCatNotSelected.DataBind();
            //    msgView.SetMessage("Please enter a text in Project No. / Title filter field", MessageType.Warning, PositionType.Auto);
            //    return;
            //}

            hdnFuzzySearchField.Value = "ProjCode";            

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllProjectList(txtProjCode.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        #endregion METHODS

        #region Sorting grid data

        #region Enums for Sorting
        private enum GridSortDirections
        {
            Ascending = 0,
            Descending = 1
        }

        private enum NotSelGridSortColumns
        {
            None = 0,
            CatNo = 1,
            Title = 2,
            Artist = 3,
            Project = 4,
            Config = 5
        }

        #endregion Enums for Sorting

        #region properties for Sorting
        /// <summary>
        /// Property to hold sort direction of not selected grid
        /// </summary>
        private string NotSelGridSortDirection
        {
            get
            {
                if (hdnNotSelGridSortDir.Value == string.Empty)
                    hdnNotSelGridSortDir.Value = GridSortDirections.Ascending.ToString();
                return hdnNotSelGridSortDir.Value.ToString();
            }
            set { hdnNotSelGridSortDir.Value = value; }
        }

        /// <summary>
        /// Property to hold sort column of not selected grid
        /// </summary>
        private string NotSelGridSortColumn
        {
            get
            {
                if (hdnNotSelGridSortColumn.Value == string.Empty)
                    hdnNotSelGridSortColumn.Value = NotSelGridSortColumns.None.ToString();
                return hdnNotSelGridSortColumn.Value;
            }
            set { hdnNotSelGridSortColumn.Value = value; }
        }

        /// <summary>
        /// Property to hold sort direction of selected grid
        /// </summary>
        private string SelGridSortDirection
        {
            get
            {
                if (hdnSelGridSortDir.Value == string.Empty)
                    hdnSelGridSortDir.Value = GridSortDirections.Ascending.ToString();
                return hdnSelGridSortDir.Value.ToString();
            }
            set { hdnSelGridSortDir.Value = value; }
        }

        /// <summary>
        /// Property to hold sort column of selected grid
        /// </summary>
        private string SelGridSortColumn
        {
            get
            {
                if (hdnSelGridSortColumn.Value == string.Empty)
                    hdnSelGridSortColumn.Value = NotSelGridSortColumns.None.ToString();
                return hdnSelGridSortColumn.Value;
            }
            set { hdnSelGridSortColumn.Value = value; }
        }

        #endregion properties for Sorting

        protected void btnCatNoNotSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtNotSelToRet"] == null)
                    return;

                DataTable dtNotSelectedToRet = (DataTable)Session["TransRetDtNotSelToRet"];

                if (NotSelGridSortColumn == NotSelGridSortColumns.None.ToString())
                    NotSelGridSortDirection = GridSortDirections.Descending.ToString();
                else if (NotSelGridSortColumn == NotSelGridSortColumns.CatNo.ToString())
                    NotSelGridSortDirection = NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    NotSelGridSortDirection = GridSortDirections.Ascending.ToString();

                NotSelGridSortColumn = NotSelGridSortColumns.CatNo.ToString();
                dtNotSelectedToRet.DefaultView.Sort = "catno" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatNotSelected.DataSource = dtNotSelectedToRet;
                gvCatNotSelected.DataBind();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnTitleNotSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtNotSelToRet"] == null)
                    return;

                DataTable dtNotSelectedToRet = (DataTable)Session["TransRetDtNotSelToRet"];

                if (NotSelGridSortColumn == NotSelGridSortColumns.Title.ToString())
                    NotSelGridSortDirection = NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    NotSelGridSortDirection = GridSortDirections.Ascending.ToString();

                NotSelGridSortColumn = NotSelGridSortColumns.Title.ToString();
                dtNotSelectedToRet.DefaultView.Sort = "catno_title" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatNotSelected.DataSource = dtNotSelectedToRet;
                gvCatNotSelected.DataBind();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnArtistNotSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtNotSelToRet"] == null)
                    return;

                DataTable dtNotSelectedToRet = (DataTable)Session["TransRetDtNotSelToRet"];

                if (NotSelGridSortColumn == NotSelGridSortColumns.Artist.ToString())
                    NotSelGridSortDirection = NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    NotSelGridSortDirection = GridSortDirections.Ascending.ToString();

                NotSelGridSortColumn = NotSelGridSortColumns.Artist.ToString();
                dtNotSelectedToRet.DefaultView.Sort = "artist_name" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatNotSelected.DataSource = dtNotSelectedToRet;
                gvCatNotSelected.DataBind();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnProjectNotSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtNotSelToRet"] == null)
                    return;

                DataTable dtNotSelectedToRet = (DataTable)Session["TransRetDtNotSelToRet"];

                if (NotSelGridSortColumn == NotSelGridSortColumns.Project.ToString())
                    NotSelGridSortDirection = NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    NotSelGridSortDirection = GridSortDirections.Ascending.ToString();

                NotSelGridSortColumn = NotSelGridSortColumns.Project.ToString();
                dtNotSelectedToRet.DefaultView.Sort = "project_title" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatNotSelected.DataSource = dtNotSelectedToRet;
                gvCatNotSelected.DataBind();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnConfigNotSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtNotSelToRet"] == null)
                    return;

                DataTable dtNotSelectedToRet = (DataTable)Session["TransRetDtNotSelToRet"];

                if (NotSelGridSortColumn == NotSelGridSortColumns.Config.ToString())
                    NotSelGridSortDirection = NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    NotSelGridSortDirection = GridSortDirections.Ascending.ToString();

                NotSelGridSortColumn = NotSelGridSortColumns.Config.ToString();
                dtNotSelectedToRet.DefaultView.Sort = "config_code" + (NotSelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatNotSelected.DataSource = dtNotSelectedToRet;
                gvCatNotSelected.DataBind();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnCatNoSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtSelectedToRet"] == null)
                    return;

                DataTable dtSelectedToRet = (DataTable)Session["TransRetDtSelectedToRet"];

                if (SelGridSortColumn == NotSelGridSortColumns.None.ToString())
                    SelGridSortDirection = GridSortDirections.Descending.ToString();
                else if (SelGridSortColumn == NotSelGridSortColumns.CatNo.ToString())
                    SelGridSortDirection = SelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    SelGridSortDirection = GridSortDirections.Ascending.ToString();

                SelGridSortColumn = NotSelGridSortColumns.CatNo.ToString();
                dtSelectedToRet.DefaultView.Sort = "catno" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatSelected.DataSource = dtSelectedToRet;
                gvCatSelected.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnTitleSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtSelectedToRet"] == null)
                    return;

                DataTable dtSelectedToRet = (DataTable)Session["TransRetDtSelectedToRet"];

                if (SelGridSortColumn == NotSelGridSortColumns.Title.ToString())
                    SelGridSortDirection = SelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    SelGridSortDirection = GridSortDirections.Ascending.ToString();

                SelGridSortColumn = NotSelGridSortColumns.Title.ToString();
                dtSelectedToRet.DefaultView.Sort = "catno_title" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatSelected.DataSource = dtSelectedToRet;
                gvCatSelected.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnArtistSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtSelectedToRet"] == null)
                    return;

                DataTable dtSelectedToRet = (DataTable)Session["TransRetDtSelectedToRet"];

                if (SelGridSortColumn == NotSelGridSortColumns.Artist.ToString())
                    SelGridSortDirection = SelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    SelGridSortDirection = GridSortDirections.Ascending.ToString();

                SelGridSortColumn = NotSelGridSortColumns.Artist.ToString();
                dtSelectedToRet.DefaultView.Sort = "artist_name" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatSelected.DataSource = dtSelectedToRet;
                gvCatSelected.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnProjectSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtSelectedToRet"] == null)
                    return;

                DataTable dtSelectedToRet = (DataTable)Session["TransRetDtSelectedToRet"];

                if (SelGridSortColumn == NotSelGridSortColumns.Project.ToString())
                    SelGridSortDirection = SelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    SelGridSortDirection = GridSortDirections.Ascending.ToString();

                SelGridSortColumn = NotSelGridSortColumns.Project.ToString();
                dtSelectedToRet.DefaultView.Sort = "project_title" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatSelected.DataSource = dtSelectedToRet;
                gvCatSelected.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }


        protected void btnConfigSelecSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TransRetDtSelectedToRet"] == null)
                    return;

                DataTable dtSelectedToRet = (DataTable)Session["TransRetDtSelectedToRet"];

                if (SelGridSortColumn == NotSelGridSortColumns.Config.ToString())
                    SelGridSortDirection = SelGridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    SelGridSortDirection = GridSortDirections.Ascending.ToString();

                SelGridSortColumn = NotSelGridSortColumns.Config.ToString();
                dtSelectedToRet.DefaultView.Sort = "config_code" + (SelGridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvCatSelected.DataSource = dtSelectedToRet;
                gvCatSelected.DataBind();

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        #endregion Sorting grid data


    }
}