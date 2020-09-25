using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;
using System.Data;

namespace WARS.BusinessLayer
{
    public class SalesTypeSearchBL : ISalesTypeSearchBL
    {
        ISalesTypeSearchDAL salesTypeSearchDAL;
        #region Constructor
        public SalesTypeSearchBL()
        {
            salesTypeSearchDAL = new SalesTypeSearchDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return salesTypeSearchDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetSalesTypeData(string salesTypeCode, out Int32 iErrorId)
        {
            return salesTypeSearchDAL.GetSalesTypeData(salesTypeCode, out iErrorId);
        }

        public DataSet SaveSalesTypeGroup(string flag, string salesTypeCode, string salesTypeName, string salesTypeType, string escalationProrata, string userCode, out Int32 iErrorId)
        {
            return salesTypeSearchDAL.SaveSalesTypeGroup(flag, salesTypeCode, salesTypeName, salesTypeType, escalationProrata, userCode, out iErrorId);
        }
    }
}
