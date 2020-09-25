using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;


namespace WARS.BusinessLayer
{
    public class PaymentApprovalBL:IPaymentApprovalBL
    {
        IPaymentApprovalDAL paymentApprovalDAL;
        #region Constructor
        public PaymentApprovalBL()
        {
            paymentApprovalDAL = new PaymentApprovalDAL();
        }
        #endregion Constructor

        public DataSet GetInitialLoadData(string userAccountId, out string userRoleApprovalLevel, out Int32 iErrorId)
        {
            return paymentApprovalDAL.GetInitialLoadData(userAccountId, out userRoleApprovalLevel, out iErrorId);
        }

        public DataSet GetSearchData(string companyCode, string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                               string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId)
        {
           
            return (ProcessPaymentData(paymentApprovalDAL.GetSearchData(companyCode, stmtEndPeriod, reportedDays, stmtStatus, payeeStatus, paymentStatus,
                                                    ownerCode, intPartyId, royaltorId, balThreshold, respCode, out iErrorId), 0));
        }

        public DataSet SavePaymentApproval(Array paymentList, string userCode, string companyCode, string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                                    string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId)

        {
            return (ProcessPaymentData(paymentApprovalDAL.SavePaymentApproval(paymentList, userCode, companyCode, stmtEndPeriod, reportedDays, stmtStatus, payeeStatus, paymentStatus,
                                                        ownerCode, intPartyId, royaltorId, balThreshold, respCode, out iErrorId), 0));
        }

        public DataSet UpdateInvoices(string userCode, string companyCode, string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                                    string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId)
        {
            return (ProcessPaymentData(paymentApprovalDAL.UpdateInvoices(userCode, companyCode, stmtEndPeriod, reportedDays, stmtStatus, payeeStatus, paymentStatus,
                                                            ownerCode, intPartyId, royaltorId, balThreshold, respCode, out iErrorId),0));
        }


        /// <summary>
        ///  If more than one Statement Period for Royaltor with same Payment Id  only display Payment Details for latest Period
        ///  Statement details should be displayed for each statement for a royaltor - but the payment details (and payee details)
        ///             should only display for the latest period.
        ///  payee and payment details should be blank for periods other than the latest one
        /// </summary>
        /// <param name="paymentData"></param>
        /// <param name="tableNum"></param>
        private DataSet ProcessPaymentData(DataSet paymentData, int tableNum)
        {
            if (paymentData.Tables[tableNum].Rows.Count != 0)
            {
                //bool moreThanOneStmt = false;
                string royaltorId = string.Empty;
                string paymentId = string.Empty;
                string maxStmtPeriodId = string.Empty;

                //get list of max stmt period ids where there are more than one Statement Period for Royaltor with same Payment Id
                var result = paymentData.Tables[tableNum].AsEnumerable()
                     .GroupBy(dtRow => new
                     {
                         RoyaltorId = dtRow["ROYALTOR_ID"],
                         PaymenId = dtRow["PAYMENT_ID"]
                     })
                     .Select(g => new
                     {
                         Group = g,
                         Count = g.Count(),
                         MaxStmtPeriodId = g.Max(r => r["END_MONTH"])
                     })
                     .Select(g => new
                     {
                         Key = g.Group.Key,
                         Count = g.Count,
                         MaxStmtPeriodId = g.MaxStmtPeriodId
                     }).Where(g => g.Count > 1);


                //make payee and payment details blank for periods other than the latest one
                foreach (var item in result)
                {
                    royaltorId = item.Key.RoyaltorId.ToString();
                    paymentId = item.Key.PaymenId.ToString();
                    maxStmtPeriodId = item.MaxStmtPeriodId.ToString();

                    foreach (DataRow dr in paymentData.Tables[tableNum].Rows)
                    {
                        if (dr["ROYALTOR_ID"].ToString() == royaltorId && dr["PAYMENT_ID"].ToString() == paymentId && Convert.ToInt32(dr["END_MONTH"].ToString()) < Convert.ToInt32(maxStmtPeriodId))
                        {
                            //payee details
                            dr["int_party_name"] = DBNull.Value;
                            dr["payee_status"] = DBNull.Value;
                            dr["payee_share"] = DBNull.Value;

                            //payment details
                            dr["payment_status"] = DBNull.Value;
                            dr["currency_code"] = DBNull.Value;
                            dr["vat"] = DBNull.Value;
                            dr["is_approval_enabled"] = "N";
                            dr["value_in_gbp"] = DBNull.Value;
                            dr["exchange_rate"] = DBNull.Value;
                            dr["payment_value"] = DBNull.Value;
                        }

                    }

                }

                /*
                if (moreThanOneStmt)
                {
                    var resultMaxStmtPeriod = paymentData.Tables[tableNum].AsEnumerable()
                         .GroupBy(dtRow => new
                         {
                             RoyaltorId = dtRow["ROYALTOR_ID"],
                             PaymenId = dtRow["PAYMENT_ID"]
                         })
                         .Select(g => new
                         {
                             Group = g,
                             MaxStmtPeriodId = g.Max(r => r["STATEMENT_PERIOD_ID"])
                         })
                         .Select(g => new
                         {
                             Key = g.Group.Key,
                             MaxStmtPeriodId = g.MaxStmtPeriodId
                         });


                    //Output
                    foreach (var item in resultMaxStmtPeriod)
                    {

                    }

                }
                */
            }


            return paymentData;
        }

        
    }
}
