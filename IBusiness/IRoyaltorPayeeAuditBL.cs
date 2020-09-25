using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyaltorPayeeAuditBL
    {        
        DataSet GetRoyPayeeAuditData(Int32 royaltorId, out Int32 iErrorId);
    }
}
