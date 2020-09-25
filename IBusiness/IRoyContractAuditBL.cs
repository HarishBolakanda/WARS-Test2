using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractAuditBL
    {
        DataSet GetRoyContractAuditData(Int32 royaltorId, out Int32 iErrorId);
        DataSet GetRoyReserveAuditData(Int32 royaltorId, out Int32 iErrorId);
    }
}
