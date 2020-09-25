/*
File Name   :   RoyaltorBalance.cs
Purpose     :   To display Royaltor Balance

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     06-Mar-2018     Bala(Infosys Limited)   Initial Creation
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
    public partial class RoyaltorBalance : System.Web.UI.Page
    {
        #region Global Declarations
        DataSet royaltorList;       
        Int32 errorId;
        Utilities util;
        #endregion Global Declarations

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["DatabaseName"] != null)
                {
                    this.Title = Convert.ToString(Session["DatabaseName"]) + " - " + "Royaltor Earnings Enquiry";
                }
                else
                {
                    util = new Utilities();
                    string dbName = util.GetDatabaseName();
                    util = null;
                    Session["DatabaseName"] = dbName.Split('.')[0].ToString();
                    this.Title = dbName.Split('.')[0].ToString() + " - " + "Royaltor Earnings Enquiry";
                }

                lblTab.Focus();//tabbing sequence starts here


                if (!IsPostBack)
                {
                    util = new Utilities();
                    if (util.UserAuthentication())
                    {

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

        protected void valBalanceDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtBalanceDate.Text.Trim() != "__/____" && txtBalanceDate.Text.Trim() != string.Empty)
            {
                if (txtBalanceDate.Text.Contains("_"))
                {
                    args.IsValid = false;
                    msgView.SetMessage("Please enter a valid date in MM/YYYY format", MessageType.Warning, PositionType.Auto);
                    valBalanceDate.ToolTip = "Please enter a valid date in MM/YYYY format";
                    return;
                }
                else
                {
                    RoyaltorBalanceBL royaltorBalanceBL = new RoyaltorBalanceBL();
                    royaltorList = royaltorBalanceBL.GetRoyaltorDate(txtRoyaltor.Text.Split('-')[0].ToString().Trim(), out errorId);
                    string monthCode = Convert.ToString(royaltorList.Tables[0].Rows[0]["month_code"]);

                    Int32 dateYear = Convert.ToInt32(txtBalanceDate.Text.Replace('_', ' ').Split('/')[1].Trim());
                    Int32 dateMonth = Convert.ToInt32(txtBalanceDate.Text.Replace('_', ' ').Split('/')[0].Trim());
                    if (!(dateMonth > 0 && dateMonth < 13) || !(dateYear > 1900))
                    {
                        args.IsValid = false;
                        msgView.SetMessage("Please enter a valid date in MM/YYYY format", MessageType.Warning, PositionType.Auto);
                        valBalanceDate.ToolTip = "Please enter a valid date in MM/YYYY format";
                        return;
                    }
                    else if (dateYear > DateTime.Now.Year || (dateMonth > DateTime.Now.Month && dateYear >= DateTime.Now.Year))
                    {
                        args.IsValid = false;
                        msgView.SetMessage("Date should not be greater than Today", MessageType.Warning, PositionType.Auto);
                        valBalanceDate.ToolTip = "Date should not be greater than Today";
                        return;
                    }

                    else if (royaltorList.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(monthCode))
                    {

                        if (Convert.ToInt32(monthCode) >= Convert.ToInt32((txtBalanceDate.Text.Split('/')[1] + txtBalanceDate.Text.Split('/')[0]).Replace("/", "")))
                        {
                            args.IsValid = false;
                            msgView.SetMessage("Should be a date greater than latest end date of statement period from Balance History", MessageType.Warning, PositionType.Auto);
                            valBalanceDate.ToolTip = "Should be a date greater than latest end date of statement period from Balance History";
                            return;
                        }
                    }

                }
            }
        }

        protected void lbFuzzySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbFuzzySearch.SelectedValue.ToString() == "No results found")
                {
                    txtRoyaltor.Text = string.Empty;
                    txtBroughtForwardDate.Text = string.Empty;
                    txtBroughtForward.Text = string.Empty;
                    txtRoyaltyEarnings.Text = string.Empty;
                    txtRoyaltyReserves.Text = string.Empty;
                    txtCosts.Text = string.Empty;
                    txtBalanceFinalDate.Text = string.Empty;
                    txtBalanceFinal.Text = string.Empty;
                    hdnRoyaltorId.Value = string.Empty;
                    return;
                }

                txtRoyaltor.Text = lbFuzzySearch.SelectedValue.ToString();
                hdnIsvalidRoyaltor.Value = "Y";
                btnHdnRoyaltor_Click(sender, e);
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
                txtBroughtForwardDate.Text = string.Empty;
                txtBroughtForward.Text = string.Empty;
                txtRoyaltyEarnings.Text = string.Empty;
                txtRoyaltyReserves.Text = string.Empty;
                txtCosts.Text = string.Empty;
                txtBalanceFinalDate.Text = string.Empty;
                txtBalanceFinal.Text = string.Empty;
                hdnRoyaltorId.Value = string.Empty;
                hdnIsvalidRoyaltor.Value = "N";
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in selecting royaltor from searh list.", ex.Message);
            }
        }

        protected void btnHdnRoyaltor_Click(object sender, EventArgs e)
        {
            try
            {
                string voucherDate = string.Empty;
                string dateType = string.Empty;
                decimal broughtForward = 0, costs = 0, royaltyReserves = 0, royaltyEarnings = 0, balance = 0;

                System.Globalization.DateTimeFormatInfo mfi = new
                            System.Globalization.DateTimeFormatInfo();
                hdnRoyaltorId.Value = txtRoyaltor.Text.Split('-')[0].ToString().Trim();
                Page.Validate("valGrpSave");
                if (!Page.IsValid)
                {
                    return;
                }
                else
                {
                    ClearValues();
                    RoyaltorBalanceBL royaltorBalanceBL = new RoyaltorBalanceBL();
                    if (txtBalanceDate.Text.Contains("_") || txtBalanceDate.Text == string.Empty)
                    {
                        dateType = "default";
                        royaltorList = royaltorBalanceBL.GetSearchedData(txtRoyaltor.Text.Split('-')[0].ToString().Trim(), dateType, DateTime.Now.ToString("yyyyMM"), string.Empty, out errorId);
                    }
                    else
                    {
                        dateType = "date";
                        voucherDate = DateTime.DaysInMonth(Convert.ToInt32(txtBalanceDate.Text.Split('/')[1]), Convert.ToInt32(txtBalanceDate.Text.Split('/')[0])) + "-" + mfi.GetMonthName(Convert.ToInt32(txtBalanceDate.Text.Split('/')[0])).Substring(0, 3) + "-" + txtBalanceDate.Text.Split('/')[1];
                        royaltorList = royaltorBalanceBL.GetSearchedData(txtRoyaltor.Text.Split('-')[0].ToString().Trim(), "date", (txtBalanceDate.Text.Split('/')[1] + txtBalanceDate.Text.Split('/')[0]), voucherDate, out errorId);
                    }

                    if (royaltorList.Tables.Count > 0 && txtRoyaltor.Text.Split('-')[0].ToString().Trim() != string.Empty)
                    {
                        if (royaltorList.Tables[0].Rows.Count > 0)
                        {
                            txtBroughtForwardDate.Text = ConvertTime(Convert.ToString(royaltorList.Tables[0].Rows[0]["month_code"]));
                            broughtForward = Math.Round(Convert.ToDecimal(royaltorList.Tables[0].Rows[0]["closing_balance"]), 2);
                            txtBroughtForward.Text = ConvertAmount(broughtForward);
                        }

                        if (royaltorList.Tables[1].Rows.Count > 0 && dateType == "default")
                        {
                            costs = (Math.Round(Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(royaltorList.Tables[1].Rows[0]["total_costs"])) ? "0" : (royaltorList.Tables[1].Rows[0]["total_costs"])), 2));
                            royaltyReserves = (Math.Round(Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(royaltorList.Tables[1].Rows[0]["total_reserve_amount"])) ? "0" : (royaltorList.Tables[1].Rows[0]["total_reserve_amount"])), 2));
                            royaltyEarnings = Math.Round(Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(royaltorList.Tables[1].Rows[0]["total_royalty_amount"])) ? "0" : (royaltorList.Tables[1].Rows[0]["total_royalty_amount"])), 2);
                        }
                        else if (dateType == "date" && royaltorList.Tables[1].Rows.Count > 0 && royaltorList.Tables[2].Rows.Count > 0)
                        {
                            costs = (Math.Round(Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(royaltorList.Tables[2].Rows[0]["total_costs"])) ? "0" : (royaltorList.Tables[2].Rows[0]["total_costs"])), 2));
                            royaltyReserves = (Math.Round(Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(royaltorList.Tables[1].Rows[0]["total_reserve_amount"])) ? "0" : (royaltorList.Tables[1].Rows[0]["total_reserve_amount"])), 2));
                            royaltyEarnings = Math.Round(Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(royaltorList.Tables[1].Rows[0]["total_royalty_amount"])) ? "0" : (royaltorList.Tables[1].Rows[0]["total_royalty_amount"])), 2);
                        }

                        //Formula  Balance = Balance Brought Forward + Royalty Earnings - Royaltor Reserves - Costs                     
                        balance = broughtForward + royaltyEarnings - royaltyReserves - costs;

                        txtBalanceFinalDate.Text = txtBalanceDate.Text.Contains("_") ? DateTime.Now.ToString("MMM-yyyy") : (mfi.GetMonthName(Convert.ToInt32(txtBalanceDate.Text.Split('/')[0])).Substring(0, 3) + "-" + txtBalanceDate.Text.Split('/')[1]);
                        txtBalanceFinal.Text = ConvertAmount(balance);
                        txtRoyaltyEarnings.Text = ConvertAmount(royaltyEarnings);
                        //WUIN-556 Multipled value with -1 for costs and reserves
                        txtRoyaltyReserves.Text = ConvertAmount(royaltyReserves * -1);
                        txtCosts.Text = ConvertAmount(costs * -1);

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in search.", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        //WUIN-656 Clear previous values if no details on table for Royaltor
        private void ClearValues()
        {
            txtBroughtForwardDate.Text = string.Empty;
            txtBroughtForward.Text = string.Empty;
            txtCosts.Text = "0.00";
            txtRoyaltyReserves.Text="0.00";
            txtRoyaltyEarnings.Text = "0.00";
            txtBalanceFinalDate.Text = string.Empty;
            txtBalanceFinal.Text = "0.00";
        }
        
        private string ConvertTime(string time)
        {
            if (time.Length > 0)
            {
                System.Globalization.DateTimeFormatInfo mfi = new
                    System.Globalization.DateTimeFormatInfo();
                return mfi.GetMonthName(Convert.ToInt32(time.Substring(4, 2))).Substring(0, 3) + "-" + time.Substring(0, 4);
            }
            else
            { return string.Empty; }


        }

        private string ConvertAmount(decimal amount)
        {
            return amount.ToString("0.00");
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


    }
}