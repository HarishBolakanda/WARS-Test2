/*
File Name   :   SalesTypeSearch.cs
Purpose     :   to display Sales Type Groups based on sales type code

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     04-Apr-2017     Pratik(Infosys Limited)   Initial Creation
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

namespace WARS
{
    public partial class SalesTypeSearch : System.Web.UI.Page
    {
        #region Global Declarations
        SalesTypeSearchBL salesTypeSearchBL;
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
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Sales Type Enquiry";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Sales Type Enquiry";
                }

                lblTab.Focus();//tabbing sequence starts here

                if (!IsPostBack)
                {
                    txtSalestypeSearch.Focus();
                    tdGrid.Visible = false;                    

                    util = new Utilities();
                    if (util.UserAuthentication())
                    {
                        LoadSalesTypeList();
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

        protected void fuzzySalestypeSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FuzzySearchSalesType();
                lbFuzzySearch.Style.Add("height", Convert.ToDouble(hdnGridPnlGroupHeight.Value == string.Empty ? "300" : hdnGridPnlGroupHeight.Value).ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in sales type search.", ex.Message);
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtSalestypeSearch.Text = string.Empty;
                    return;
                }

                txtSalestypeSearch.Text = lbFuzzySearch.SelectedValue.ToString();
                SalesTypeSelected();
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
                txtSalestypeSearch.Text = string.Empty;

                dtEmpty = new DataTable();

                gvSalestypeGroup.DataSource = dtEmpty;
                gvSalestypeGroup.EmptyDataText = "No data found.";
                gvSalestypeGroup.DataBind();

                tdGrid.Visible = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in closing sales type search list.", ex.Message);
            }
        }

        protected void btnHdnSalesTypeSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SalesTypeSelected();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting sales type.", ex.Message);
            }
        }

        protected void BtnAddSalesType_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("valAddSalesTypeGroup");
                if (!Page.IsValid)
                {
                    mpeAddSalesTypeCode.Show();
                }
                else
                {

                    string userCode = Convert.ToString(Session["UserCode"]);
                    string escalationProrata = txtEscalationProrata.Text == string.Empty ? "1" : txtEscalationProrata.Text;

                    salesTypeSearchBL = new SalesTypeSearchBL();
                    DataSet dtSalesTypeList = salesTypeSearchBL.SaveSalesTypeGroup("I", txtSalesTypeCode.Text.ToUpper(), txtSalesTypeName.Text.ToUpper(), ddlSalesTypeType.SelectedValue, escalationProrata, userCode, out errorId);
                    salesTypeSearchBL = null;

                    if (dtSalesTypeList.Tables.Count > 0 && errorId == 0)
                    {
                        Session["FuzzySearchPriceGroupListTypeC"] = dtSalesTypeList.Tables[0];
                        msgView.SetMessage("Sales type added successfully.", MessageType.Warning, PositionType.Auto);
                        mpeAddSalesTypeCode.Hide();
                        txtSalesTypeCode.Text = "";
                        txtSalesTypeName.Text = "";
                        ddlSalesTypeType.Text = "-";
                        txtEscalationProrata.Text = "";
                    }
                    else if (errorId == 1)
                    {
                        msgView.SetMessage("Sales type already exists.", MessageType.Warning, PositionType.Auto);
                        mpeAddSalesTypeCode.Show();
                    }
                    else
                    {
                        ExceptionHandler("Error in adding the new Sales type.", "");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in adding the new sales type code.", ex.Message);
            }

        }

        protected void btnUnSavedDataExit_Click(object sender, EventArgs e)
        {
            if (hdnButtonSelection.Value == "SalesTypeSearch" || hdnButtonSelection.Value == "fuzzySalestypeSearch")
            {
                txtSalestypeSearch.Text = string.Empty;
                tdGrid.Visible = false;
            }
        }

        protected void imgBtnSalesTypeUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = Convert.ToString(Session["UserCode"]);

                Page.Validate("valUpdateSalesTypeGroup");
                if (!Page.IsValid)
                {
                    return;
                }

                salesTypeSearchBL = new SalesTypeSearchBL();
                DataSet dtSalesTypeList = salesTypeSearchBL.SaveSalesTypeGroup("U", lblSalesTypeCode.Text.ToUpper(), txtGridSalesTypeName.Text.ToUpper(), ddlGridSalesTypeType.SelectedValue, txtGridEscalationProrata.Text, userCode, out errorId);
                salesTypeSearchBL = null;

                if (dtSalesTypeList.Tables.Count != 0 && errorId != 2)
                {
                    Session["FuzzySearchPriceGroupListTypeC"] = dtSalesTypeList.Tables[0];

                    DataTable dtSalesTypeDtls = dtSalesTypeList.Tables[1];
                    lblSalesTypeCode.Text = dtSalesTypeDtls.Rows[0]["price_group_code"].ToString();
                    hdnGridSalesTypeName.Value = txtGridSalesTypeName.Text = dtSalesTypeDtls.Rows[0]["price_name"].ToString();
                    hdnGridSalesTypeType.Value = ddlGridSalesTypeType.SelectedValue = dtSalesTypeDtls.Rows[0]["price_type"].ToString(); 
                    hdnGridEscalationProrata.Value = txtGridEscalationProrata.Text = dtSalesTypeDtls.Rows[0]["escalation_prorata"].ToString();

                    msgView.SetMessage("Sales type updated successfully.", MessageType.Warning, PositionType.Auto);
                }
                else
                {
                    msgView.SetMessage("Failed to update Sales type.", MessageType.Warning, PositionType.Auto);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in updating Sales type.", ex.Message);
            }
        }

        protected void gvSalestypeGroup_RowDataBound(object sender, GridViewRowEventArgs e)
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


        //JIRA-746 Changes by Ravi on 05/03/2019 -- Start
        protected void gvSalestypeGroup_Sorting(object sender, GridViewSortEventArgs e)
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
            DataTable dataTable = (DataTable)Session["SaleTypeGroupData"];
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + sortingDirection;

                hdnSortExpression.Value = e.SortExpression;
                hdnSortDirection.Value = sortingDirection;
                gvSalestypeGroup.DataSource = dataView;
                gvSalestypeGroup.DataBind();
            }
            UserAuthorization();
        }
        //JIRA-746 CHanges by Ravi on 05/03/2019 -- End

        #endregion Events

        #region Methods

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            //redirecting to exception page from javascript. to avoid prompt on window close when exception occured
            Session["Exception_Reason"] = errorMsg + "<br />" + expMsg;
            ScriptManager.RegisterStartupScript(this, typeof(Page), "ErrorPage", "RedirectToErrorPage();", true);
        } 

        private void LoadSalesTypeList()
        {
            salesTypeSearchBL = new SalesTypeSearchBL();
            DataSet salesTypeData = salesTypeSearchBL.GetInitialData(out errorId);
            salesTypeSearchBL = null;

            if (salesTypeData.Tables.Count != 0 && errorId != 2)
            {                
                ddlSalesTypeType.DataSource = salesTypeData.Tables[1];
                ddlSalesTypeType.DataTextField = "price_description";
                ddlSalesTypeType.DataValueField = "price_type";
                ddlSalesTypeType.DataBind();
                ddlSalesTypeType.Items.Insert(0, new ListItem("-"));

                ddlGridSalesTypeType.DataSource = salesTypeData.Tables[1];
                ddlGridSalesTypeType.DataTextField = "price_description";
                ddlGridSalesTypeType.DataValueField = "price_type";
                ddlGridSalesTypeType.DataBind();
                ddlGridSalesTypeType.Items.Insert(0, new ListItem("-"));
            }
            else if (salesTypeData.Tables.Count == 0 && errorId != 2)
            {
                Session["FuzzySearchPriceGroupListTypeC"] = null;
            }
            else
            {
                ExceptionHandler("Error in loading Sales Type", string.Empty);
            }
        }

        private void SalesTypeSelected()
        {
            salesTypeSearchBL = new SalesTypeSearchBL();
            DataSet salesTypeData = salesTypeSearchBL.GetSalesTypeData(txtSalestypeSearch.Text.Split('-')[0].ToString().Trim(), out errorId);
            salesTypeSearchBL = null;

            BindGrids(salesTypeData);

            tdGrid.Visible = true;
        }

        private void BindGrids(DataSet salesTypeInOutData)
        {
            //WUIN-746 clearing sort hidden files
            hdnSortExpression.Value = string.Empty;
            hdnSortDirection.Value = string.Empty;

            if (salesTypeInOutData.Tables.Count != 0 && errorId != 2)
            {
                DataTable dtSalesTypeDtls = salesTypeInOutData.Tables[0];

                lblSalesTypeCode.Text = dtSalesTypeDtls.Rows[0]["price_group_code"].ToString();
                hdnGridSalesTypeName.Value = txtGridSalesTypeName.Text = dtSalesTypeDtls.Rows[0]["price_name"].ToString();
                hdnGridSalesTypeType.Value = dtSalesTypeDtls.Rows[0]["price_type"].ToString();                
                
                if (ddlGridSalesTypeType.Items.FindByValue(hdnGridSalesTypeType.Value) != null)
                {                    
                    ddlGridSalesTypeType.SelectedValue = hdnGridSalesTypeType.Value;
                }
                else
                {
                    ddlGridSalesTypeType.SelectedIndex = 0;
                }  
                       
                hdnGridEscalationProrata.Value = txtGridEscalationProrata.Text = dtSalesTypeDtls.Rows[0]["escalation_prorata"].ToString();

                Session["SaleTypeGroupData"] = salesTypeInOutData.Tables[1];
                gvSalestypeGroup.DataSource = salesTypeInOutData.Tables[1];
                if (salesTypeInOutData.Tables[1].Rows.Count == 0)
                {
                    gvSalestypeGroup.EmptyDataText = "No data found.";
                }
                gvSalestypeGroup.DataBind();
            }
            else if (salesTypeInOutData.Tables.Count == 0 && errorId != 2)
            {
                dtEmpty = new DataTable();

                gvSalestypeGroup.DataSource = dtEmpty;
                gvSalestypeGroup.EmptyDataText = "No data found.";
                gvSalestypeGroup.DataBind();
            }
            else
            {
                ExceptionHandler("Error in loading grid data.", string.Empty);
            }
            if (gvSalestypeGroup.Rows.Count > 0)
            {
                PnlSalestypeGroup.Style.Add("height", hdnGridPnlGroupHeight.Value);
            }

        }

        private void UserAuthorization()
        {
            //WUIN-1096 Only read access for Reaonly User
            if (Session["UserRole"].ToString().ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                btnAddNewSalesType.Enabled = false;
                imgBtnCancel.Enabled = false;
                imgBtnSave.Enabled = false;

            }
        }

        private void FuzzySearchSalesType()
        {
            if (txtSalestypeSearch.Text.Trim() == string.Empty)
            {
                msgView.SetMessage("Please enter a text in sales type search field", MessageType.Warning, PositionType.Auto);
                return;
            }

            List<string> searchList = (new WARS.Services.FuzzySearch()).FuzzySearchPriceGroupListTypeC(txtSalestypeSearch.Text.Trim().ToUpper(), int.MaxValue);
            lbFuzzySearch.DataSource = searchList;
            lbFuzzySearch.DataBind();
            mpeFuzzySearch.Show();
        }

        #endregion Methods

        


    }
}