using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ICompanyAuditBL
    {        
        DataSet GetCompanyAuditData(Int32 companyCode, out Int32 iErrorId);
    }
}
