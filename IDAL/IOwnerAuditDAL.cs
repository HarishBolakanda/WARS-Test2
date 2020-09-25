using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IOwnerAuditDAL
    {        
        DataSet GetOwnerAuditData(string ownerCode, out Int32 iErrorId);
    }
}
