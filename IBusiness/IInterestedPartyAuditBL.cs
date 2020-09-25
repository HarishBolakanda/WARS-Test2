using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IInterestedPartyAuditBL
    {        
        DataSet GetInterestedPartyAuditData(Int32 intPartyId,out string intParty, out Int32 iErrorId);
    }
}
