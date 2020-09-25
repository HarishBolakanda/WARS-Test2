using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IParticipantAuditBL
    {        
        DataSet GetParticipantData(string catNo,string fromDate,string toDate, out Int32 iErrorId);
    }
}
