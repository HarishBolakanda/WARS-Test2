using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IDAL
{
    public interface IRoyaltorPayeeSuppAuditDAL
    {        
        DataSet GetRoyPayeeSuppAuditData(string royaltorId, string intPartyId, out Int32 iErrorId);
    }
}
