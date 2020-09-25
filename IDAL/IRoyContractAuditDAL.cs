using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyContractAuditDAL
    {
        DataSet GetRoyContractAuditData(Int32 royaltorId, out Int32 iErrorId);
        DataSet GetRoyReserveAuditData(Int32 royaltorId, out Int32 iErrorId);
    }
}
