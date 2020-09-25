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
    public class RoyContractSubRatesBL : IRoyContractSubRatesBL
    {
        IRoyContractSubRatesDAL royContractSubRatesDAL;
        #region Constructor
        public RoyContractSubRatesBL()
        {
            royContractSubRatesDAL = new RoyContractSubRatesDAL();
        }
        #endregion Constructor

        public DataSet GetSubsidRatesData(string royaltorId, string userRoleId, out string royaltor, out Int32 iErrorId)
        {
            return royContractSubRatesDAL.GetSubsidRatesData(royaltorId, userRoleId, out royaltor, out iErrorId);
        }

        public DataSet SaveSubsidRates(string royaltorId, string loggedUser, string userRoleId, Array addUpdateList, Array deleteList, out string royaltor, out Int32 iErrorId)
        {
            return royContractSubRatesDAL.SaveSubsidRates(royaltorId, loggedUser, userRoleId, addUpdateList, deleteList, out royaltor, out iErrorId);
        }
       
      
    }
}
