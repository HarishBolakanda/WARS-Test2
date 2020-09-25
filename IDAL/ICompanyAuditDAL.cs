using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ICompanyAuditDAL
    {        
        DataSet GetCompanyAuditData(Int32 companyCode, out Int32 iErrorId);
    }
}
