using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;

namespace WARS.BusinessLayer
{
    public class CompanyAuditBL : ICompanyAuditBL
    {
        ICompanyAuditDAL CompanyAuditDAL;
        #region Constructor
        public CompanyAuditBL()
        {
            CompanyAuditDAL = new CompanyAuditDAL();
        }
        #endregion Constructor
       
        public DataSet GetCompanyAuditData(Int32 companyCode, out Int32 iErrorId)
        {
            return CompanyAuditDAL.GetCompanyAuditData(companyCode, out iErrorId);
        }
    }
}
