using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IPaymentApprovalBL
    {
        DataSet GetInitialLoadData(string userAccountId, out string userRoleApprovalLevel, out Int32 iErrorId);

        DataSet GetSearchData(string companyCode, string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                               string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId);

        DataSet SavePaymentApproval(Array paymentList, string userCode, string companyCode, string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                                    string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId);

        DataSet UpdateInvoices(string userCode, string companyCode, string stmtEndPeriod, string reportedDays, string stmtStatus, string payeeStatus, string paymentStatus,
                                  string ownerCode, string intPartyId, string royaltorId, string balThreshold, string respCode, out Int32 iErrorId); 
    }
}
