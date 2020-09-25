/*
File Name   :   PaymentDetails.cs
Purpose     :   To display payment details from invoice and payment tables (WUIN-293)

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     03-Oct-2017     Pratik(Infosys Limited)   Initial Creation
        04-Dec-2017     Harish                    WUIN-291 - changes
 *                                                opening payment details from payment approval screen
 *      19-Dec-2017     Harish                    WUIN-291 - changes - removed opening of payment details from approval screen     
 *      08-Mar-2019     Rakesh                    WUIN-990 Renamed 'Invoice' to 'Payment' for filters and controld Ids.
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

namespace WARS.Payment
{
    public partial class PaymentDetails : System.Web.UI.Page
    {
        #region Global Declarations
        PaymentDetailsBL paymentDetailsBL;
        Utilities util;
        Int32 errorId;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Payment Details";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Payment Details";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);

                if (!IsPostBack)
                {
                    txtSupplier.Focus();
                    Session["dtPaymentDetails"] = null;
                    Session["PageStartEndPD"] = null;

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadData();
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

        protected void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                SearchPaymentDetails();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in payment details search.", ex.Message);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                txtRoyaltor.Text = string.Empty;
                txtSupplier.Text = string.Empty;
                txtPaymentDate.Text = string.Empty;
                txtPaymentThreshold.Text = string.Empty;
                txtPaymentFilename.Text = string.Empty;
                txtPaymentNo.Text = string.Empty;
                ddlStatus.SelectedIndex = 0;
                hdnPageNumber.Value = "1";
                hdnSupplier.Value = string.Empty;
                hdnIsvalidRoyaltor.Value = "N";

                dtEmpty = new DataTable();
                BindGrid(dtEmpty);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in clearing deatils.", ex.Message);
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

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtRoyaltor.Text = string.Empty;
                    return;
                }

                txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                hdnIsvalidRoyaltor.Value = "Y";
                SearchPaymentDetails();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor.", ex.Message);
            }
        }

        protected void btnCloseFuzzySearchPopup_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtRoyaltor.Text = string.Empty;
                hdnIsvalidRoyaltor.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from searh list.", ex.Message);
            }
        }

        protected void valPaymentDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                DateTime temp;
                if (txtPaymentDate.Text.Trim() != "__/__/____" && txtPaymentDate.Text.Trim() != "")
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
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in validating date.", ex.Message);
            }
        }

        protected void gvSupplierSearchList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                string supplierNumber = (row.FindControl("lblSupplierNumber") as Label).Text;
                string supplierSiteName = (row.FindControl("lblSupplierSiteName") as Label).Text;
                string supplierName = (row.FindControl("lblSupplierName") as Label).Text;

                if (e.CommandName == "dblClk")
                {
                    txtSupplier.Text = supplierNumber + " - " + supplierSiteName + " - " + supplierName;
                    hdnSupplier.Value = txtSupplier.Text;
                    SearchPaymentDetails();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading supplier data", ex.Message);
            }
        }

        protected void gvSupplierSearchList_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[7].Visible = false;
                    LinkButton _dblClickButton = e.Row.FindControl("lnkBtnDblClk") as LinkButton;
                    string _jsDoubleClick = ClientScript.GetPostBackClientHyperlink(_dblClickButton, "");
                    e.Row.Attributes.Add("ondblclick", _jsDoubleClick);

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in searching supplier data", ex.Message);
            }
        }

        protected void gvPaymentDetails_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void gvPaymentDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["dtPaymentDetails"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                hdnPageNumber.Value = "1";
                Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), dataView.ToTable(), gridDefaultPageSize, gvPaymentDetails, dtEmpty, rptPager);
                Session["dtPaymentDetails"] = dataView.ToTable();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        protected void btnSupplierSearch_Click(object sender, EventArgs e)
        {
            try
            {
                paymentDetailsBL = new PaymentDetailsBL();
                DataSet supplierList = paymentDetailsBL.GetSupplierList(txtSupplier.Text, out errorId);
                paymentDetailsBL = null;

                if (supplierList.Tables.Count != 0 && errorId != 2)
                {

                    if (supplierList.Tables[0].Rows.Count == 0)
                    {
                        gvSupplierSearchList.DataSource = supplierList.Tables[0];
                        gvSupplierSearchList.EmptyDataText = "No data found for the entered text";
                        gvSupplierSearchList.DataBind();
                    }
                    else
                    {
                        gvSupplierSearchList.DataSource = supplierList.Tables[0];
                        gvSupplierSearchList.DataBind();

                        //set pop up panel and gridview panel height                    
                        pnlSupplierSearchPopup.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.75).ToString());
                        plnGridSupplierSearch.Style.Add("height", (Convert.ToDouble(hdnGridPnlHeight.Value) * 0.75 - 100).ToString());
                    }

                }
                else if (supplierList.Tables.Count == 0 && errorId != 2)
                {
                    dtEmpty = new DataTable();
                    gvSupplierSearchList.DataSource = dtEmpty;
                    gvSupplierSearchList.EmptyDataText = "No data found for the entered text";
                    gvSupplierSearchList.DataBind();
                }
                else
                {
                    ExceptionHandler("Error in supplier search data", string.Empty);
                }

                mpeSupplierSearch.Show();
            }
            catch (Exception ex)
            {
                mpeSupplierSearch.Hide();
                ExceptionHandler("Error in supplier search data", ex.Message);
            }
        }

        protected void btnClosePopupSupplierSearch_Click(object sender, ImageClickEventArgs e)
        {
            txtSupplier.Text = string.Empty;
            hdnSupplier.Value = string.Empty;
        }

        protected void valSupplier_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtSupplier.Text == string.Empty || txtSupplier.Text == hdnSupplier.Value)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void valRoyaltor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtRoyaltor.Text == string.Empty || hdnIsvalidRoyaltor.Value == "Y")
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        #endregion Events

        #region Methods

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnApInterface.Enabled = false;
            }

        }
        private void LoadData()
        {
            paymentDetailsBL = new PaymentDetailsBL();
            DataSet initialData = paymentDetailsBL.GetInitialData(out errorId);
            paymentDetailsBL = null;

            if (initialData.Tables.Count != 0 && errorId != 2)
            {
                ddlStatus.DataSource = initialData.Tables[0];
                ddlStatus.DataTextField = "status_desc";
                ddlStatus.DataValueField = "status_code";
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0, new ListItem("-"));

                dtEmpty = new DataTable();
                gvPaymentDetails.EmptyDataText = "No data is displayed initially.";
                gvPaymentDetails.DataSource = dtEmpty;
                gvPaymentDetails.DataBind();

            }
            else
            {
                ExceptionHandler("Error in fetching filter list data", string.Empty);
            }
        }

        private void SearchPaymentDetails()
        {

            Page.Validate("valSearch");
            if (!Page.IsValid)
            {
                dtEmpty = new DataTable();
                BindGrid(dtEmpty);

                msgView.SetMessage("Invalid search input!", MessageType.Warning, PositionType.Auto);
                return;
            }


            string paymentDate = string.Empty;
            string statusCode = string.Empty;


            if (txtPaymentDate.Text.Trim() != "__/__/____")
            {
                paymentDate = txtPaymentDate.Text.Trim();
            }

            if (ddlStatus.SelectedIndex > 0)
            {
                statusCode = ddlStatus.SelectedValue;
            }

            //WUIN-1079
            string supplierNumber = "";
            string supplierSiteName = "";
            if (txtSupplier.Text != "")
            {
                supplierNumber = txtSupplier.Text.Split('-')[0].ToString().Trim();
                supplierSiteName = txtSupplier.Text.Split('-')[1].ToString().Trim();
            }

            paymentDetailsBL = new PaymentDetailsBL();
            //JIRA-1048 Changes to handle single quote while searching --Start
            DataSet searchedData = paymentDetailsBL.GetSearchedData(txtPaymentNo.Text.Replace("'", "").Trim(), paymentDate, statusCode, supplierNumber, supplierSiteName,
                                    txtPaymentThreshold.Text.Trim(), txtPaymentFilename.Text.Replace("'", "").Trim(), txtRoyaltor.Text.Split('-')[0].ToString().Trim(), out errorId);
            //JIRA-1048 Changes to handle single quote while searching --End
            paymentDetailsBL = null;

            if (searchedData.Tables.Count != 0 && errorId != 2)
            {
                BindGrid(searchedData.Tables[0]);
            }
            else
            {
                ExceptionHandler("Error in fetching payment details search data", string.Empty);
            }
        }

        private void BindGrid(DataTable gridData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            hdnPageNumber.Value = "1";
            Session["dtPaymentDetails"] = gridData;
            Utilities.PopulateGridPage(Convert.ToInt32(hdnPageNumber.Value), gridData, gridDefaultPageSize, gvPaymentDetails, dtEmpty, rptPager);
        }

        private void FuzzySearchRoyaltor()
        {
            if (txtRoyaltor.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in royaltor search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchAllRoyaltorList(txtRoyaltor.Text.Trim().ToUpper(), int.MaxValue);
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

        #region GRID PAGING
        /// <summary>
        /// object to hold page number, start and end row numbers
        /// </summary>


        protected void lnkPage_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtGridData = (DataTable)Session["dtPaymentDetails"];
                int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
                hdnPageNumber.Value = pageIndex.ToString();
                Utilities.PopulateGridPage(pageIndex, dtGridData, gridDefaultPageSize, gvPaymentDetails, dtEmpty, rptPager);

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in page change.", ex.Message);
            }
        }

        #endregion GRID PAGING

        //JIRA-908 Changes by Ravi on 13/02/2019 -- Start
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                string userRole = Session["UserRole"].ToString();
                if (userRole.ToLower() == UserRole.SuperUser.ToString().ToLower())
                {
                    paymentDetailsBL = new PaymentDetailsBL();
                    paymentDetailsBL.CreateAPInterface();
                    paymentDetailsBL = null;

                    if (errorId == 0)
                    {
                        msgView.SetMessage("Job to create AP interface has been triggered.", MessageType.Warning, PositionType.Auto);
                    }
                    else if (errorId == 2)
                    {
                        msgView.SetMessage("Failed to trigger job to create AP interface.", MessageType.Warning, PositionType.Auto);
                    }
                }
                else
                {
                    msgView.SetMessage("AP interface can only be created by a SuperUser", MessageType.Warning, PositionType.Auto);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in trigger job to create AP interface.", ex.Message);
            }
        }

        

        //JIRA-908 Changes by Ravi on 13/02/2019 -- End

    }
}