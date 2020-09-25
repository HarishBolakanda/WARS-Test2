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
    public class RoyContractPkgRatesBL : IRoyContractPkgRatesBL
    {
        IRoyContractPkgRatesDAL royContractPkgRatesDAL;
        #region Constructor
        public RoyContractPkgRatesBL()
        {
            royContractPkgRatesDAL = new RoyContractPkgRatesDAL();
        }
        #endregion Constructor

        public DataSet GetPkgRatesData(string royaltorId, string optionPeriod, string userRoleId, out string royaltor, out Int32 iErrorId)
        {
            return royContractPkgRatesDAL.GetPkgRatesData(royaltorId, optionPeriod, userRoleId, out royaltor, out iErrorId);
        }

        public DataSet SavePkgRates(string royaltorId, string optionPeriod, string loggedUser, string userRoleId, Array addUpdateList, Array deleteList, out string royaltor, out string invalidCatno, out Int32 iErrorId)
        {
            return royContractPkgRatesDAL.SavePkgRates(royaltorId, optionPeriod, loggedUser, userRoleId, addUpdateList, deleteList, out royaltor, out invalidCatno, out iErrorId);
        }
       
      
    }
}
