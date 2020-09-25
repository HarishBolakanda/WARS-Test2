using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Data;
using WARS.BusinessLayer;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WARS
{
    public enum MessageType : int
    {
        Error = 0,
        Warning = 1,
        Success = 2,
        Confirm = 3
    }

    public enum PositionType : int
    {
        Auto = 0,
        Top = 1,
        Left = 2,
        TopLeft = 3
    }

    /// <summary>
    /// holds user role description and corresponding role id
    /// </summary>
    public enum UserRole
    {
        SuperUser = 1,
        EditorUser = 2,
        PaymentApprover = 3,
        RoyaltiesFinance = 4,
        //JIRA - 983 Changes by Ravi on 20/02/2019 -- Start
        Supervisor = 5,
        //JIRA - 983 Changes by Ravi on 20/02/2019 -- End
        //WUIN-1096
        ReadOnlyUser = 6
    }

    public enum PageMode : int
    {
        InitialLoad = 0,
        Search = 1,
        Add = 2,
        Update = 3
    }

    public enum StmtType : int
    {
        DetailStmt = 0,
        SummaryStmtPrimaryPayee = 1,
        SummaryStmtNonPrimaryPayee = 2,
        InvoiceStmtPrimaryPayee = 3,
        InvoiceStmtNonPrimaryPayee = 4
    }

    /// <summary>
    /// WUIN-450 - On new Contract, enable side bar buttons that have already been completed
    /// </summary>
    public enum ContractScreens : int
    {
        RoyaltorSearch = 0,
        ContractDetails = 1,
        PayeeDetails = 2,
        PayeeSupplier = 3,
        OptionPeriod = 4,
        RoyaltyRates = 5,
        SubsidiaryRates = 6,
        PackagingRates = 7,
        EscalationRates = 8,
        ContractGroupings = 9,
        TaxDetails = 10, //JIRA-1146 CHanges
        Notes = 11,
        BalResvHistory = 12,
        EscHistory = 13
    }

    /// <summary>
    /// WUIN-930 - Functionality to have single application set up for all WARS affiliates
    /// </summary>
    public enum AffiliateName
    {
        Benelux = 1,
        CentralEurope = 2,
        CzechRepublic = 3,
        Denmark = 4,
        Finland = 5,
        Italy = 6,
        MiddleEast = 7,
        Norway = 8,
        Poland = 9,
        Portugal = 10,
        Russia = 11,
        SouthAfrica = 12,
        Spain = 13,
        Sweden = 14,
        UKADA = 15,
        UKERATO = 16,
        UKGB = 17,
        UKNVC = 18,
        UKPLG = 19,
        UKTELDEC = 20,
        UKWEA = 21,
        AustraliaNZ = 22,
        China = 23,
        HongKong = 24,
        Indonesia = 25,
        Japan = 26,
        Korea = 27,
        Malaysia = 28,
        Philippines = 29,
        Singapore = 30,
        Taiwan = 31,
        Thailand = 32,
        Argentina = 33,
        Brazil = 34,
        Canada = 35,
        Latina = 36,
        Mexico = 37,
        DEV = 38,
        UAT = 39,
        QA = 40,

    }

    /// <summary>
    /// WUIN-1015 - to get affiliate code for formulating SharePoint pdf folder name
    /// </summary>
    public enum AffiliateCode
    {
        BNLX = 1,
        GSA = 2,
        CZE = 3,
        DNK = 4,
        FIN = 5,
        ITA = 6,
        MiddleEast = 7,
        NOR = 8,
        POL = 9,
        PRT = 10,
        RUS = 11,
        ZAF = 12,
        ESP = 13,
        SWE = 14,
        ADA = 15,
        ERATO = 16,
        GB = 17,
        NVC = 18,
        PLG = 19,
        TELDEC = 20,
        WEA = 21,
        AUS = 22,
        CHN = 23,
        HKG = 24,
        IND = 25,
        JAP = 26,
        KOR = 27,
        MYS = 28,
        PHL = 29,
        SGP = 30,
        TWN = 31,
        THA = 32,
        ARG = 33,
        BRA = 34,
        CAN = 35,
        LAT = 36,
        MEX = 37,
        DEV = 38,
        UAT = 39,
        QA = 40,

    }

    public class Utilities
    {
        public string InvalidUserExpMessage = "Sorry! You do not have access to the application/page.";
        public string OnlySuperUserExpMessage = "Sorry! Only Super user can access the screen.";

        Int32 iErrorId;
        string sErrorMsg;

        public bool UserAuthentication()
        {
            bool validUser = false;

            if (HttpContext.Current.Session["UserRole"] != null)
            {
                validUser = true;
            }
            else
            {
                LoginBL loginBL = new LoginBL();
                DataTable dtUserInfo = new DataTable();
                string sUser = WebUtility.HtmlDecode(System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString());
                dtUserInfo = loginBL.UserAuthentication(sUser, out iErrorId, out sErrorMsg);
                if (dtUserInfo.Rows.Count > 0)
                {
                    HttpContext.Current.Session["UserCode"] = dtUserInfo.Rows[0]["user_code"].ToString();
                    HttpContext.Current.Session["UserRole"] = dtUserInfo.Rows[0]["role_desc"].ToString();
                    HttpContext.Current.Session["UserRoleId"] = dtUserInfo.Rows[0]["role_id"].ToString();
                    HttpContext.Current.Session["UserPaymentRoleId"] = dtUserInfo.Rows[0]["payment_role_id"].ToString();

                    validUser = true;
                }
                else
                {
                    //WUIN-1082
                    string WARSSupportUserList = ConfigurationManager.AppSettings["WARSSupportUserList"].ToString();

                    if (Array.IndexOf(WARSSupportUserList.ToLower().Split(','), sUser.ToLower()) >= 0)
                    {
                        sUser = ConfigurationManager.AppSettings["WARSSupportUserId"].ToString();
                    }

                    dtUserInfo = loginBL.UserAuthentication(sUser, out iErrorId, out sErrorMsg);
                    if (dtUserInfo.Rows.Count > 0)
                    {
                        HttpContext.Current.Session["UserCode"] = dtUserInfo.Rows[0]["user_code"].ToString();
                        HttpContext.Current.Session["UserRole"] = dtUserInfo.Rows[0]["role_desc"].ToString();
                        HttpContext.Current.Session["UserRoleId"] = dtUserInfo.Rows[0]["role_id"].ToString();
                        HttpContext.Current.Session["UserPaymentRoleId"] = dtUserInfo.Rows[0]["payment_role_id"].ToString();

                        validUser = true;
                    }
                }
                loginBL = null;
            }

            return validUser;
        }

        /// <summary>
        /// returns user role id from role description
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public static string GetUserRoleId(string userRole)
        {
            if (userRole.ToLower() == UserRole.EditorUser.ToString().ToLower())
            {
                return ((int)UserRole.EditorUser).ToString();
            }
            else if (userRole.ToLower() == UserRole.SuperUser.ToString().ToLower())
            {
                return ((int)UserRole.SuperUser).ToString();
            }
            else if (userRole.ToLower() == UserRole.PaymentApprover.ToString().ToLower())
            {
                return ((int)UserRole.PaymentApprover).ToString();
            }
            else if (userRole.ToLower() == UserRole.RoyaltiesFinance.ToString().ToLower())
            {
                return ((int)UserRole.RoyaltiesFinance).ToString();
            }
            else if (userRole.ToLower() == UserRole.ReadOnlyUser.ToString().ToLower())
            {
                return ((int)UserRole.ReadOnlyUser).ToString();
            }
            else
            {
                return "0";
            }
        }

        /* WOS-389 - changes - modified by Harish
        public void GetNextRuntime(out string nextRunTime, out string estCompletionTime,out Int32 errorId, out string errorMsg)
        {
            nextRunTime = string.Empty;
            estCompletionTime = string.Empty;
            CommonBL commonBL = new CommonBL();
            commonBL.GetNextRuntime(out nextRunTime, out estCompletionTime, out errorId, out errorMsg);
            commonBL = null;
        }
         * */

        //public void GenericExceptionHandler(string pageName,string expMsg)
        //{
        //    HttpContext.Current.Session["Exception_Reason"] = pageName + " - " + expMsg;
        //    HttpContext.Current.Response.Redirect(@"~\ExceptionPage.aspx",false);
        //}
        //WUIN-380-To implement custom paging in gridview
        public static void PopulateGridPage(int pageIndex, DataTable dtGridData, int gridDefaultPageSize, GridView gvDetails, DataTable dtEmpty, Repeater rptPager)
        {
            if (dtGridData.Rows.Count > 0)
            {
                DataTable dtFiltered = dtGridData.Rows.Cast<System.Data.DataRow>().Skip((pageIndex - 1) * gridDefaultPageSize).Take(gridDefaultPageSize).CopyToDataTable();

                if (dtFiltered.Rows.Count != 0)
                {
                    gvDetails.DataSource = dtFiltered;
                }
                else
                {
                    gvDetails.EmptyDataText = "No data found.";
                }
                gvDetails.DataBind();
            }
            else
            {
                dtEmpty = new DataTable();
                gvDetails.DataSource = dtEmpty;
                gvDetails.DataBind();
            }
            PopulatePager(dtGridData.Rows.Count, pageIndex, gridDefaultPageSize, rptPager);
        }

        //WUIN-380-To implement custom paging in gridview
        public static void PopulatePager(int recordCount, int currentPage, int pageSize, Repeater rptPager)
        {
            rptPager.Visible = true;
            double dblPageCount = (double)((decimal)recordCount / decimal.Parse(pageSize.ToString()));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<ListItem> pages = new List<ListItem>();
            if (pageCount > 0)
            {
                //To display only 10 pages on screen at a time
                //Divide total pages into different groups with 10 pages on each group
                //for example 
                //page group 0 ->page no 1 to 10 
                //page group 1 ->page no 11 to 20 ... 
                //calculate page group of currentpage
                int pageGroup = (int)Math.Floor((decimal)(currentPage - 1) / 10);

                //Add First page 
                pages.Add(new ListItem("First", "1", currentPage > 1));

                //if page group > 0 means current page is >10. Add ... to screen 
                //And its page index is last page of previous page group
                if (pageGroup > 0)
                {
                    pages.Add(new ListItem("...", (pageGroup * 10).ToString(), true));
                }

                //Add pages based on page group
                //If selected page is 5, its pagegroup is 0 so add 1 to 10 page no
                //If selected page is 18, its pagegroup is 1 so add 11 to 20 page no
                for (int i = 1; i <= 10; i++)
                {
                    int pageIndex = (pageGroup * 10) + i;
                    if (pageIndex <= pageCount)
                    {
                        pages.Add(new ListItem(pageIndex.ToString(), pageIndex.ToString(), pageIndex != currentPage));
                    }
                }

                //If total page count is more than 10 and we are not on last page group then add ...
                //And its index is first page of next page group
                if (pageCount > 10 && ((pageCount - (pageGroup * 10)) > 10))
                {
                    pages.Add(new ListItem("...", ((pageGroup * 10) + 11).ToString(), currentPage < pageCount));
                }

                //Finally ad Last page
                pages.Add(new ListItem("Last", pageCount.ToString(), currentPage < pageCount));
            }

            rptPager.DataSource = pages;
            rptPager.DataBind();

            if (pageCount == 1)
            {
                rptPager.Visible = false;
            }
        }

        public void GenericExceptionHandler(string expMsg)
        {
            HttpContext.Current.Session["Exception_Reason"] = expMsg;
            HttpContext.Current.Response.Redirect(@"~/Common/ExceptionPage.aspx", false);
        }

        public string GetDatabaseName()
        {
            string dbName;
            CommonBL commonBL = new CommonBL();
            dbName = commonBL.GetDatabaseName();
            commonBL = null;
            return dbName;
        }
        public static string GetBOAccountUserId()
        {
            string BOAccountUserId = string.Empty;
            if (HttpContext.Current.Session["WARSBOConnectionString"] != null)
            {
                BOAccountUserId = Convert.ToString(HttpContext.Current.Session["WARSBOConnectionString"]).Split(';')[0].ToString();
            }

            return BOAccountUserId;
        }

        public static string GetBOAccountUserPassoword()
        {
            string BOAccountUserPassoword = string.Empty;
            if (HttpContext.Current.Session["WARSBOConnectionString"] != null)
            {
                BOAccountUserPassoword = Convert.ToString(HttpContext.Current.Session["WARSBOConnectionString"]).Split(';')[1].ToString();
            }

            return BOAccountUserPassoword;
        }

        //JIRA-746 Changes by Ravi on 07/03/2019 -- Start
        public string SortingDirection(string sortdirection)
        {
            // string direction = string.Empty;
            if (sortdirection == "")
            {
                sortdirection = "Asc";
            }

            return sortdirection;
        }
        //JIRA-746 Changes by Ravi on 07/03/2019 -- End


    }
}
