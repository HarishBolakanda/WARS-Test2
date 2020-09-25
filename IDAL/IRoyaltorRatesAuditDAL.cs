using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyaltorRatesAuditDAL
    {        
        DataSet GetRoyRatesAuditData(string royaltorId, string optionPeriodCode, string fromDate, string toDate, string userRoleId, out Int32 iErrorId);
    }
}
