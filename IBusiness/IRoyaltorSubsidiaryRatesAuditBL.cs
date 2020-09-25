using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyaltorSubsidiaryRatesAuditBL
    {
        DataSet GetSubsidiaryRatesAuditData(string royaltorId, string fromDate, string toDate, string userRoleId, out Int32 iErrorId);
    }
}
