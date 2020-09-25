using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyaltorGroupingAuditBL
    {        
        DataSet GetRoyaltorData(string royaltorId, out Int32 iErrorId);
    }
}
