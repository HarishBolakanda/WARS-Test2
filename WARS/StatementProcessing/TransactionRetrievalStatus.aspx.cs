/*
File Name   :   TransactionRetrieval.cs
Purpose     :   used for selecting Catalogue numbers to be used to retrieve Transactions from the Transaction Archive

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     13-Feb-2017     Pratik(Infosys Limited)   Initial Creation
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
    public partial class TransactionRetrievalStatus : System.Web.UI.Page
    {
        #region Global Declarations

        TransactionRetrievalStatusBL transRetStatusBL;
        string loggedUserID;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string ASCENDING = " ASC";
        string DESCENDING = " DESC";
        #endregion Global Declarations

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["DatabaseName"] != null)
            {
                this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Transaction Retrieval Request Status";
            }
            else
            {
                util = new Utilities();
                string dbName = util.GetDatabaseName();
                util = null;
                Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                this.Title = dbName.Split('.')[0].ToString() + " - " + "Transaction Retrieval Request Status";
            }

            lblTab.Focus();//tabbing sequence starts here
            if (!IsPostBack)
            {                
                util = new Utilities();
                if (util.UserAuthentication())
                {
                    PopulateGrid();
                }
                else
                {
                    ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                }

                util = null;

            }
        }

        protected void txtRoyOptPrd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCatArtist.Text = string.Empty;
                txtCatNum.Text = string.Empty;
                txtProjCode.Text = string.Empty;

                if (hdnSelToRetSearchSelected.Value == "Y")
                {
                    RoyOptPrdSeleted();
                }
                else if (hdnSelToRetSearchSelected.Value == "N")
                {
                    FuzzySearchRoyOptPrd();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                }
                else
                {
                    PopulateGrid();
                }
                

                hdnSelToRetSearchSelected.Value = string.Empty;
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
                txtRoyOptPrd.Text = string.Empty;
                txtCatNum.Text = string.Empty;
                txtProjCode.Text = string.Empty;

                if (hdnSelToRetSearchSelected.Value == "Y")
                {
                    CatArtistSelected();
                }
                else if (hdnSelToRetSearchSelected.Value == "N")
                {
                    FuzzySearchCatArtist();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());
                }
                else
                {
                    PopulateGrid();
                }

                hdnSelToRetSearchSelected.Value = string.Empty;
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
                txtRoyOptPrd.Text = string.Empty;
                txtCatArtist.Text = string.Empty;
                txtCatNum.Text = string.Empty;

                if (hdnSelToRetSearchSelected.Value == "Y")
                {
                    ProjCodeSelected();
                }
                else if (hdnSelToRetSearchSelected.Value == "N")
                {
                    FuzzySearchProjCode();
                    lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlHeight.Value == string.Empty ? "300" : hdnGridPnlHeight.Value).ToString());

                }
                else
                {
                    PopulateGrid();
                }

                hdnSelToRetSearchSelected.Value = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
            }
        }

        protected void btnCatNumNotSelecSearch_Click(object sender, EventArgs e)
        {
            try
            {
                txtRoyOptPrd.Text = string.Empty;
                txtCatArtist.Text = string.Empty;
                txtProjCode.Text = string.Empty;

                if (txtCatNum.Text != string.Empty)
                {
                    LoadSearchedData();
                }
                else
                {
                    PopulateGrid();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading grid data", ex.Message);
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
                ExceptionHandler("Error in artist fuzzy search", ex.Message);
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
                ExceptionHandler("Error in project no fuzzy search", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (hdnFuzzySearchField.Value == "RoyOptPrd")
                {
                    txtRoyOptPrd.Text = lbFuzzySearch.SelectedValue.ToString();
                    RoyOptPrdSeleted();
                }
                else if (hdnFuzzySearchField.Value == "CatArtist")
                {
                    txtCatArtist.Text = lbFuzzySearch.SelectedValue.ToString();
                    CatArtistSelected();
                }
                else if (hdnFuzzySearchField.Value == "ProjCode")
                {
                    txtProjCode.Text = lbFuzzySearch.SelectedValue.ToString();
                    ProjCodeSelected();
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
                }
                else if (hdnFuzzySearchField.Value == "CatArtist")
                {
                    txtCatArtist.Text = string.Empty;
                }
                else if (hdnFuzzySearchField.Value == "ProjCode")
                {
                    txtProjCode.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing fuzzy search pop up", ex.Message);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                txtRoyOptPrd.Text = string.Empty;
                txtCatArtist.Text = string.Empty;
                txtCatNum.Text = string.Empty;
                txtProjCode.Text = string.Empty;               
                PopulateGrid();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in reseting.", ex.Message);
            }
        }

        #endregion EVENTS

        #region METHODS

        private void PopulateGrid()
        {
            transRetStatusBL = new TransactionRetrievalStatusBL();
            DataSet searchListData = transRetStatusBL.GetInitialTransactionData(out errorId);
            transRetStatusBL = null;

            if (searchListData.Tables.Count != 0 && errorId != 2)
            {
                BindGrids(searchListData);
            }
            else
            {
                ExceptionHandler("Error in fetching filter list data", string.Empty);
            }
        }

        private void LoadSearchedData()
        {
            string royaltorId = (txtRoyOptPrd.Text == "" ? string.Empty : txtRoyOptPrd.Text.Split('-')[0].Trim());
            string optionPeriodCode = (txtRoyOptPrd.Text == "" ? string.Empty : txtRoyOptPrd.Text.Split('-')[1].Trim());
            string artistId = (txtCatArtist.Text == "" ? string.Empty : txtCatArtist.Text.Split('-')[0].Trim());
            string catNumber = txtCatNum.Text.Trim();
            string projCode = (txtProjCode.Text == "" ? string.Empty : txtProjCode.Text.Split('-')[0].Trim());

            transRetStatusBL = new TransactionRetrievalStatusBL();
            DataSet searchedData = transRetStatusBL.GetSearchData(royaltorId, optionPeriodCode, artistId, catNumber.ToUpper(), projCode, out errorId);
            transRetStatusBL = null;

            BindGrids(searchedData);
        }

        private void BindGrids(DataSet transRetrieved)
        {
            if (transRetrieved.Tables.Count != 0 && errorId != 2)
            {
                Session["ActiveTransStatusData"] = transRetrieved.Tables[0];

                gvTransRetStatus.DataSource = transRetrieved.Tables[0];
                if (transRetrieved.Tables[0].Rows.Count == 0)
                {
                    gvTransRetStatus.EmptyDataText = "No data found.";
                }
                gvTransRetStatus.DataBind();


                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }
            }
            else if (transRetrieved.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();
                gvTransRetStatus.DataSource = dtEmpty;
                gvTransRetStatus.EmptyDataText = "No data found.";
                gvTransRetStatus.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        private void FuzzySearchRoyOptPrd()
        {
            if (txtRoyOptPrd.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in Royaltor & Option period filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "RoyOptPrd";
            txtCatArtist.Text = string.Empty;
            txtProjCode.Text = string.Empty;
            txtCatNum.Text = string.Empty;

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyTransRetRoyOpPrdList(txtRoyOptPrd.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchCatArtist()
        {
            if (txtCatArtist.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in Catalogue Artist / Name filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "CatArtist";
            txtRoyOptPrd.Text = string.Empty;
            txtProjCode.Text = string.Empty;
            txtCatNum.Text = string.Empty;

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzyTransRetArtistList(txtCatArtist.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void FuzzySearchProjCode()
        {
            if (txtProjCode.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in Project No. / Title filter field", MessageType.Warning, PositionType.Auto);
                return;
            }

            hdnFuzzySearchField.Value = "ProjCode";
            txtRoyOptPrd.Text = string.Empty;
            txtCatArtist.Text = string.Empty;
            txtCatNum.Text = string.Empty;

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllProjectList(txtProjCode.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        private void RoyOptPrdSeleted()
        {
            if (txtRoyOptPrd.Text == string.Empty || txtRoyOptPrd.Text == "No results found")
            {
                txtRoyOptPrd.Text = string.Empty;
                msgView.SetMessage("Please select a valid royaltor & option period from the list", MessageType.Warning, PositionType.Auto);
                return;
            }
            else
            {
                LoadSearchedData();
            }
        }

        private void CatArtistSelected()
        {
            if (txtCatArtist.Text == string.Empty || txtCatArtist.Text == "No results found")
            {
                txtCatArtist.Text = string.Empty;
                msgView.SetMessage("Please select a valid artist from the list", MessageType.Warning, PositionType.Auto);
                return;
            }
            else
            {
                LoadSearchedData();
            }
        }

        private void ProjCodeSelected()
        {
            if (txtProjCode.Text == string.Empty || txtProjCode.Text == "No results found")
            {
                txtProjCode.Text = string.Empty;
                msgView.SetMessage("Please select a valid project number from the list", MessageType.Warning, PositionType.Auto);
                return;
            }
            else
            {
                LoadSearchedData();
            }
        }

        #endregion METHODS

        #region Sorting grid data

        #region Enums for Sorting
        private enum GridSortDirections
        {
            Ascending = 0,
            Descending = 1
        }

        private enum GridSortColumns
        {
            None = 0,
            CatNo = 1,
            Title = 2,
            Artist = 3,
            Project = 4,
            Config = 5,
            StartDate = 6,
            EndDate = 7,
            RequestedBy = 8,
            RequestedOn = 9,
            TrnxReceived = 10,
            Status = 11
        }

        #endregion Enums for Sorting

        #region properties for Sorting
        /// <summary>
        /// Property to hold sort direction of not selected grid
        /// </summary>
        //private string NotSelGridSortDirection
        //{
        //    get
        //    {
        //        if (hdnNotSelGridSortDir.Value == string.Empty)
        //            hdnNotSelGridSortDir.Value = GridSortDirections.Ascending.ToString();
        //        return hdnNotSelGridSortDir.Value.ToString();
        //    }
        //    set { hdnNotSelGridSortDir.Value = value; }
        //}

        ///// <summary>
        ///// Property to hold sort column of not selected grid
        ///// </summary>
        //private string NotSelGridSortColumn
        //{
        //    get
        //    {
        //        if (hdnNotSelGridSortColumn.Value == string.Empty)
        //            hdnNotSelGridSortColumn.Value = NotSelGridSortColumns.None.ToString();
        //        return hdnNotSelGridSortColumn.Value;
        //    }
        //    set { hdnNotSelGridSortColumn.Value = value; }
        //}

        /// <summary>
        /// Property to hold sort direction of selected grid
        /// </summary>
        private string GridSortDirection
        {
            get
            {
                if (hdnGridSortDir.Value == string.Empty)
                    hdnGridSortDir.Value = GridSortDirections.Ascending.ToString();
                return hdnGridSortDir.Value.ToString();
            }
            set { hdnGridSortDir.Value = value; }
        }

        /// <summary>
        /// Property to hold sort column of selected grid
        /// </summary>
        private string GridSortColumn
        {
            get
            {
                if (hdnGridSortColumn.Value == string.Empty)
                    hdnGridSortColumn.Value = GridSortColumns.None.ToString();
                return hdnGridSortColumn.Value;
            }
            set { hdnGridSortColumn.Value = value; }
        }

        #endregion properties for Sorting

        protected void btnCatNoSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.CatNo.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.CatNo.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "catno" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnTitleSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.Title.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.Title.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "catno_title" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnArtistSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.Artist.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.Artist.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "artist_name" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnProject_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.Project.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.Project.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "project_title" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.Config.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.Config.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "config_code" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnStartDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.StartDate.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.StartDate.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "start_recdate" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnEndDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.EndDate.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.EndDate.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "end_recdate" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnReqstedBy_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.RequestedBy.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.RequestedBy.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "user_name" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnRqstedOn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.RequestedOn.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.RequestedOn.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "last_modified" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnTrnxRcvd_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.TrnxReceived.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.TrnxReceived.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "txns_retrieved" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        protected void btnStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["ActiveTransStatusData"] == null)
                    return;

                DataTable dtActiveTransStatusData = (DataTable)Session["ActiveTransStatusData"];

                if (GridSortColumn == GridSortColumns.None.ToString())
                    GridSortDirection = GridSortDirections.Descending.ToString();
                else if (GridSortColumn == GridSortColumns.Status.ToString())
                    GridSortDirection = GridSortDirection == GridSortDirections.Ascending.ToString() ? GridSortDirections.Descending.ToString() : GridSortDirections.Ascending.ToString();
                else
                    GridSortDirection = GridSortDirections.Ascending.ToString();

                GridSortColumn = GridSortColumns.Status.ToString();
                dtActiveTransStatusData.DefaultView.Sort = "status" + (GridSortDirection == GridSortDirections.Ascending.ToString() ? ASCENDING : DESCENDING);
                gvTransRetStatus.DataSource = dtActiveTransStatusData;
                gvTransRetStatus.DataBind();

                if (gvTransRetStatus.Rows.Count > 0)
                {
                    PnlGridTransRetStatus.Style.Add("height", hdnGridPnlHeight.Value);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sorting grid data", string.Empty);
            }
        }

        #endregion Sorting grid data
      
    }
}