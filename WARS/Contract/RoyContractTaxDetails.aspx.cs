/*
File Name   :   RoyContractTaxDetails.cs
Purpose     :   to add/edit Tax details of royaltor contract

Version  Date            Modified By                   Modification Log
______________________________________________________________________
1.0     22-Oct-2019     RaviMulugu(Infosys Limited)   Initial Creation
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using System.Web.UI.HtmlControls;
using WARS.BusinessLayer;
using System.Net;
using System.Configuration;

namespace WARS.Contract
{
    public partial class RoyContractTaxDetails : System.Web.UI.Page
    {
        #region Global Declarations
        RoyContractTaxDetailsBL royContractTaxDetailsBL;
        Utilities util;
        Int32 errorId;
        DataTable dtEmpty;
        string royaltorId = string.Empty;
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
                royaltorId = Request.QueryString["RoyaltorId"];
                isNewRoyaltor = Request.QueryString["isNewRoyaltor"];

                if (royaltorId == null || isNewRoyaltor == null)
                {
                    msgView.SetMessage("Not a valid royaltor!", MessageType.Warning, PositionType.Auto);
                    return;
                }

                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Contract - Tax Details";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Contract - Tax Details";
                }

                lblTab.Focus();//tabbing sequence starts here
                PnlGrid.Style.Add("height", hdnGridPnlHeight.Value);
                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        Button btnTaxDetails = (Button)contractNavigationButtons.FindControl("btnTaxDetails");
                        btnTaxDetails.Enabled = false;
                        HiddenField hdnRoyaltorIdNav = (HiddenField)contractNavigationButtons.FindControl("hdnRoyaltorId");
                        hdnRoyaltorIdNav.Value = royaltorId;
                        HiddenField hdnRoyaltorIdHdr = (HiddenField)contractHdrNavigation.FindControl("hdnRoyaltorId");
                        hdnRoyaltorIdHdr.Value = royaltorId;

                        //WUIN-599 - resetting the flags
                        ((HiddenField)Master.FindControl("hdnIsContractScreen")).Value = "N";
                        ((HiddenField)Master.FindControl("hdnIsNotContractScreen")).Value = "N";
                        this.Master.FindControl("lnkBtnHome").Visible = false;
                       

                        hdnRoyaltorId.Value = royaltorId;
                        if (isNewRoyaltor == "Y")
                        {
                            btnSave.Text = "Save & Continue";
                            btnAudit.Text = "Back";
                            contractNavigationButtons.EnableNewRoyNavButtons(ContractScreens.TaxDetails.ToString());
                        }

                        txtRoyaltorId.Text = royaltorId;
                        LoadRoyTaxDetailsData();
                        PopulateDropDowns();
                        //WUIN-450 - If a new Royaltor is set up with Lock checked it should continue to allow entry of the details (the Royaltor set up needs to be completed!)
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId))
                        {
                            btnSave.ToolTip = "Royaltor Locked";
                        }

                        //WUIN-599 -- Only one user can use contract screens at the same time.
                        // If a contract is already using by another user then making the screen readonly.
                        if (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId))
                        {
                            hdnOtherUserScreenLocked.Value = "Y";
                        }

                        //WUIN-1096 - Only Read access for ReadonlyUser
                        //WUIN-599 If a contract is already using by another user then making the screen readonly.
                        //WUIN-450 -Only Read access for locked contracts
                        if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsRoyaltorLocked(royaltorId)) ||
                            (isNewRoyaltor != "Y" && contractNavigationButtons.IsScreenLocked(royaltorId)))
                        {
                            EnableReadonly();
                        }
                    }
                    else
                    {
                        ExceptionHandler(util.InvalidUserExpMessage.ToString(), string.Empty);
                    }
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

        protected void ddlIPNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dtGridData = (DataTable)Session["RoyContractTaxPayeeList"];
            DataRow[] dtIpName = dtGridData.Select("ip_number='" + ddlIPNumber.SelectedValue + "'");
            if (dtIpName.Count() != 0)
            {
                txtInterestedPartyName.Text = dtIpName[0]["ip_name"].ToString();
                hdnInterestedPartyID.Value = dtIpName[0]["int_party_id"].ToString();
            }
            else
            {
                txtInterestedPartyName.Text = "";
                hdnInsertDataNotSaved.Value = "N";
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Array royTaxDetailsList = RoyTaxDetails();
                List<string> deleteList = new List<string>();
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                }

                //check if any changes to save
                if (royTaxDetailsList.Length == 0 && deleteList.Count == 0)
                {
                    if (isNewRoyaltor == "N")
                    {
                        msgView.SetMessage("No changes made to save!", MessageType.Warning, PositionType.Auto);
                    }
                    else if (isNewRoyaltor == "Y")
                    {
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.TaxDetails.ToString());

                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId + ");", true);

                    }
                    return;
                }

                //validate 
                if (!Page.IsValid)
                {
                    msgView.SetMessage("Tax details not saved – invalid or missing data!", MessageType.Warning, PositionType.Auto);
                    return;
                }
                string royaltor;
                royContractTaxDetailsBL = new RoyContractTaxDetailsBL();
                DataSet savedRoyContTaxData = royContractTaxDetailsBL.SaveRoyaltorTaxDetailsData(royaltorId, royTaxDetailsList, deleteList.ToArray(), Convert.ToString(Session["UserCode"]), out royaltor, out errorId);
                royContractTaxDetailsBL = null;
                ViewState["vsDeleteIds"] = null;
                //WUIN-746 clearing sort hidden files
                hdnSortExpression.Value = string.Empty;
                hdnSortDirection.Value = string.Empty;

                if (errorId == 0)
                {
                    txtRoyaltorId.Text = royaltor;
                    if (savedRoyContTaxData.Tables.Count != 0)
                    {
                        Session["RoyContTaxDetailsGridData"] = savedRoyContTaxData.Tables[0];
                        gvContTaxDetails.DataSource = savedRoyContTaxData.Tables[0];
                        gvContTaxDetails.DataBind();

                        if (savedRoyContTaxData.Tables[0].Rows.Count == 0)
                        {
                            gvContTaxDetails.EmptyDataText = "No data found for the selected royaltor";
                        }
                        else
                        {
                            gvContTaxDetails.EmptyDataText = string.Empty;
                        }

                    }
                    else if (savedRoyContTaxData.Tables.Count == 0)
                    {
                        dtEmpty = new DataTable();
                        gvContTaxDetails.DataSource = dtEmpty;
                        gvContTaxDetails.EmptyDataText = "No data found for the selected royaltor";
                        gvContTaxDetails.DataBind();
                    }

                    if (isNewRoyaltor == "Y")
                    {
                        //WUIN-450
                        //set screen button enabled = Y
                        contractNavigationButtons.SetNewRoyButtonStatus(ContractScreens.TaxDetails.ToString());

                        //redirect to Notes Overview screen                                  
                        //redirect in javascript so that issue of data not saved validation would be handled
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "NewRoySave", "RedirectOnNewRoyaltorSave(" + royaltorId.Split('-')[0].Trim() + ");", true);
                    }
                    else
                    {
                        msgView.SetMessage("Roy Contract Tax details saved", MessageType.Warning, PositionType.Auto);
                        hdnGridDataDeleted.Value = "N";
                        hdnChangeNotSaved.Value = "N";
                        hdnInsertDataNotSaved.Value = "N";
                        hdnAddRowDataChanged.Value = "N";
                    }

                }
                else if (errorId == 1)
                {
                    msgView.SetMessage("Tax record already exists with the same combination", MessageType.Warning, PositionType.Auto);
                }
                else if (errorId == 2)
                {
                    ExceptionHandler("Error in saving Roy Contract Tax details", string.Empty);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in saving Roy Contract Tax details", ex.Message);
            }
        }

        protected void btnAppendAddRow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                if (AppendRowToGrid())
                {
                    ClearAddRow();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding Royaltor Tax row to grid", ex.Message);
            }
        }


        //JIRA-908 CHanges by Ravi on 01/01/2020 -- STart
        protected void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteRowFromGrid(hdnDeleteIntPartyId.Value, hdnDeleteRoyTaxType.Value, hdnDeleteIsModified.Value);
                hdnDeleteIntPartyId.Value = "";
                hdnDeleteRoyTaxType.Value = "";
                hdnDeleteIsModified.Value = "";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in deleting grid data", ex.Message);
            }
        }
        //JIRA-908 CHanges by Ravi on 01/01/2020 -- STart

        protected void gvContTaxDetails_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void gvContTaxDetails_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["RoyContTaxDetailsGridData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;

                gvContTaxDetails.DataSource = dataView;
                gvContTaxDetails.DataBind();
                Session["RoyContTaxDetailsGridData"] = dataView.ToTable();
            }
            //WUIN-1096 -  ReadOnlyUser
            if ((Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower()))
            {
                EnableReadonly();
            }
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion EVENTS

        #region METHODS

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        }

        private void LoadRoyTaxDetailsData()
        {
            string royaltor;

            royContractTaxDetailsBL = new RoyContractTaxDetailsBL();
            DataSet royContractTaxDetailsData = royContractTaxDetailsBL.GetInitialData(royaltorId, out royaltor, out errorId);
            royContractTaxDetailsBL = null;

            if (royContractTaxDetailsData.Tables.Count != 0 && errorId != 2)
            {
                txtRoyaltorId.Text = royaltor;
                Session["RoyContTaxDetailsGridData"] = royContractTaxDetailsData.Tables[0];

                if (royContractTaxDetailsData.Tables[0].Rows.Count == 0)
                {
                    gvContTaxDetails.DataSource = royContractTaxDetailsData.Tables[0];
                    gvContTaxDetails.EmptyDataText = "No data found for the selected royaltor";
                    gvContTaxDetails.DataBind();
                }
                else
                {
                    gvContTaxDetails.DataSource = royContractTaxDetailsData.Tables[0];
                    gvContTaxDetails.DataBind();
                }
            }
            else
            {
                ExceptionHandler("Error in loading grid data", string.Empty);
            }

        }

        private void PopulateDropDowns()
        {
            royContractTaxDetailsBL = new RoyContractTaxDetailsBL();
            DataSet royContractTaxDetailsDropDownData = royContractTaxDetailsBL.GetDropdownData(royaltorId, out errorId);
            royContractTaxDetailsBL = null;

            if (royContractTaxDetailsDropDownData.Tables.Count != 0 && errorId != 2)
            {
                Session["RoyContractTaxPayeeList"] = royContractTaxDetailsDropDownData.Tables["PayeeList"];
                ddlIPNumber.DataSource = royContractTaxDetailsDropDownData.Tables["PayeeList"];
                ddlIPNumber.DataTextField = "ip_number";
                ddlIPNumber.DataValueField = "ip_number";
                ddlIPNumber.DataBind();
                ddlIPNumber.Items.Insert(0, new ListItem("-"));

                ddlTaxType.DataSource = royContractTaxDetailsDropDownData.Tables["TaxTypeList"]; ;
                ddlTaxType.DataTextField = "tax_type_desc";
                ddlTaxType.DataValueField = "tax_type";
                ddlTaxType.DataBind();
                ddlTaxType.Items.Insert(0, new ListItem("-"));
            }
            else
            {
                ExceptionHandler("Error in loading DropDown data", string.Empty);
            }

        }

        private void ClearAddRow()
        {
            txtInterestedPartyName.Text = "";
            txtTaxRate.Text = "";
            ddlIPNumber.SelectedIndex = 0;
            ddlTaxType.SelectedIndex = 0;
        }

        private Array RoyTaxDetails()
        {
            if (Session["RoyContTaxDetailsGridData"] == null)
            {
                ExceptionHandler("Error in saving Royaltor Tax data", string.Empty);
            }

            DataTable dtRoyTaxData = (DataTable)Session["RoyContTaxDetailsGridData"];
            List<string> royTaxDetails = new List<string>();
            string royIntPartyId;
            string royTaxType;
            string IPNumber;
            string isModified;
            string royTaxRate;
            string hdnTaxRate;

            foreach (GridViewRow gvr in gvContTaxDetails.Rows)
            {
                royIntPartyId = (gvr.FindControl("hdnRoyaltorTaxIntPartyId") as HiddenField).Value;
                royTaxType = (gvr.FindControl("hdnRoyaltorTaxType") as HiddenField).Value;
                isModified = (gvr.FindControl("hdnIsModified") as HiddenField).Value;
                IPNumber = (gvr.FindControl("hdnIPNumber") as HiddenField).Value;
                hdnTaxRate = (gvr.FindControl("hdnTaxRate") as HiddenField).Value;
                royTaxRate = (gvr.FindControl("txtTaxRate") as TextBox).Text;
                royTaxType = royTaxType.Split('-')[0].ToString().Trim();

                if (isModified == Global.DBNullParamValue)//new row
                {
                    isModified = Global.DBNullParamValue;
                    royTaxDetails.Add(royIntPartyId + "(;||;)" + royTaxType + "(;||;)" + royTaxRate + "(;||;)" + isModified);
                }
                else
                {
                    if (royTaxRate != hdnTaxRate)
                    {
                        isModified = "Y";
                        royTaxDetails.Add(royIntPartyId + "(;||;)" + royTaxType + "(;||;)" + royTaxRate + "(;||;)" + isModified);
                    }

                }
            }

            return royTaxDetails.ToArray();
        }


        private void DeleteRowFromGrid(string intPartyIdToDelete, string intPartyType, string isModified)
        {
            GetGridData();
            //add to delete list only if the row is not a new one 
            List<string> deleteList = new List<string>();
            if (isModified != Global.DBNullParamValue)
            {
                if (ViewState["vsDeleteIds"] != null)
                {
                    deleteList = (List<string>)ViewState["vsDeleteIds"];
                    deleteList.Add(intPartyIdToDelete + "(;||;)" + intPartyType.Split('-')[0].ToString().Trim());
                }
                else
                {
                    deleteList.Add(intPartyIdToDelete + "(;||;)" + intPartyType.Split('-')[0].ToString().Trim());
                }

                ViewState["vsDeleteIds"] = deleteList;
            }

            DataTable dtGridData = (DataTable)Session["RoyContTaxDetailsGridData"];
            foreach (DataRow dr in dtGridData.Rows)
            {
                if (dr["rt_int_party_id"].ToString() == intPartyIdToDelete && dr["royaltor_tax_type"].ToString() == intPartyType)
                {
                    dr.Delete();
                    break;
                }
            }

            DataView dv = dtGridData.DefaultView;
            dv.Sort = "rt_int_party_id asc";
            DataTable dtGridChangedDataSorted = dv.ToTable();
            Session["RoyContTaxDetailsGridData"] = dtGridChangedDataSorted;
            gvContTaxDetails.DataSource = dtGridChangedDataSorted;
            gvContTaxDetails.DataBind();

            DataTable dtOriginalGridData = (DataTable)Session["RoyContTaxDetailsGridData"];
            foreach (DataRow dr in dtOriginalGridData.Rows)
            {
                if (dr["rt_int_party_id"].ToString() == intPartyIdToDelete && dr["royaltor_tax_type"].ToString() == intPartyType)
                {
                    dr.Delete();
                    break;
                }
            }

            DataView dvInitialData = dtOriginalGridData.DefaultView;
            dvInitialData.Sort = "rt_int_party_id asc";
            DataTable dtOriginalGridDataSorted = dvInitialData.ToTable();
            Session["RoyContTaxDetailsGridData"] = dtOriginalGridDataSorted;

        }

        private void GetGridData()
        {
            DataTable dtGridData = (DataTable)Session["RoyContTaxDetailsGridData"];
            DataTable dtGridChangedData = dtGridData.Clone();
            foreach (GridViewRow gvr in gvContTaxDetails.Rows)
            {
                DataRow drGridRow = dtGridChangedData.NewRow();
                drGridRow["royaltor_tax_type"] = (gvr.FindControl("lblTaxType") as Label).Text;
                drGridRow["royaltor_tax_rate"] = (gvr.FindControl("txtTaxRate") as TextBox).Text;
                drGridRow["rt_int_party_id"] = (gvr.FindControl("hdnRoyaltorTaxIntPartyId") as HiddenField).Value;
                drGridRow["ip_number"] = (gvr.FindControl("lblIpNumber") as Label).Text;
                drGridRow["ip_name"] = (gvr.FindControl("lblInterestedPartyName") as Label).Text;
                drGridRow["is_modified"] = "";
                dtGridChangedData.Rows.Add(drGridRow);
            }
            Session["RoyContTaxDetailsGridData"] = dtGridChangedData;
        }

        private bool AppendRowToGrid()
        {
            if (Session["RoyContTaxDetailsGridData"] == null)
            {
                ExceptionHandler("Error in adding Tax Details row to grid", string.Empty);
            }

            GetGridData();
            DataTable dtGridData = (DataTable)Session["RoyContTaxDetailsGridData"];

            //check if row already exist
            DataRow[] dtTaxRate = dtGridData.Select("ip_number = '" + ddlIPNumber.SelectedValue + "' AND royaltor_tax_type = '" + ddlTaxType.SelectedItem.Text + "'");

            if (dtTaxRate.Count() != 0)
            {
                msgView.SetMessage("Tax record already exists with the same combination", MessageType.Warning, PositionType.Auto);
                return false;
            }

            DataTable dtOriginalGridData = (DataTable)Session["RoyContTaxDetailsGridData"];
            DataRow drNewRow = dtGridData.NewRow();
            DataRow drNewRowOriginal = dtOriginalGridData.NewRow();

            drNewRow["royaltor_tax_type"] = ddlTaxType.SelectedItem.Text;
            drNewRow["royaltor_tax_rate"] = txtTaxRate.Text;
            drNewRow["ip_number"] = ddlIPNumber.SelectedValue;
            drNewRow["ip_name"] = txtInterestedPartyName.Text;
            drNewRow["rt_int_party_id"] = hdnInterestedPartyID.Value;

            drNewRow["is_modified"] = "-";
            dtGridData.Rows.Add(drNewRow);

            DataView dv = dtGridData.DefaultView;
            dv.Sort = "rt_int_party_id asc";
            DataTable dtGridDataSorted = dv.ToTable();
            Session["RoyContTaxDetailsGridData"] = dtGridDataSorted;
            gvContTaxDetails.DataSource = dtGridDataSorted;
            gvContTaxDetails.DataBind();

            return true;

        }

        private void EnableReadonly()
        {
            btnSave.Enabled = false;
            btnUndoAddRow.Enabled = false;
            btnAppendAddRow.Enabled = false;
            //disable grid buttons
            foreach (GridViewRow gvr in gvContTaxDetails.Rows)
            {
                (gvr.FindControl("imgBtnDelete") as ImageButton).Enabled = false;
                (gvr.FindControl("imgBtnUndo") as ImageButton).Enabled = false;
            }
        }


        #endregion METHODS

        #region Web Methods

        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static void UpdateScreenLockFlag()
        {
            try
            {
                int errorId;
                RoyaltorContractBL royContractBL = new RoyaltorContractBL();
                royContractBL.UpdateScreenLockFlag(HttpContext.Current.Session["ScreenLockedRoyaltorId"].ToString(), "N", HttpContext.Current.Session["UserCode"].ToString(), out errorId);
                royContractBL = null;
            }
            catch { }


        }

        #endregion Web Methods

       
    }
}