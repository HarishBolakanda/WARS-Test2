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
    public class TerritoryGroupRoyaltorsBL : ITerritoryGroupRoyaltorsBL
    {
        ITerritoryGroupRoyaltorsDAL TerritoryGroupRoyaltorsDAL;
        #region Constructor
        public TerritoryGroupRoyaltorsBL()
        {
            TerritoryGroupRoyaltorsDAL = new TerritoryGroupRoyaltorsDAL();
        }
        #endregion Constructor
               
        public DataSet GetRoyaltorList(string territoryGroupCode, out Int32 iErrorId)
        {
            return TerritoryGroupRoyaltorsDAL.GetRoyaltorList(territoryGroupCode, out iErrorId);
        }
    }
}
