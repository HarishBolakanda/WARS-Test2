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
    public class CatalogueDetailsAuditBL : ICatalogueDetailsAuditBL
    {
        ICatalogueDetailsAuditDAL CatalogueDetailsAuditDAL;
        #region Constructor
        public CatalogueDetailsAuditBL()
        {
            CatalogueDetailsAuditDAL = new CatalogueDetailsAuditDAL();
        }
        #endregion Constructor

        public DataSet GetSearchedData(string catNo, out Int32 iErrorId)
        {
            return CatalogueDetailsAuditDAL.GetSearchedData(catNo,out iErrorId);
        }
    }
}
