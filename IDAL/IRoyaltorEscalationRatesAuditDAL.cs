using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IDAL
{
    public interface IRoyaltorEscalationRatesAuditDAL
    {        
        DataSet GetEscalationRatesAuditData(string royaltorId, string fromDate, string toDate, out Int32 iErrorId);
    }
}
