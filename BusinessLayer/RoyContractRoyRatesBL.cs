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
    public class RoyContractRoyRatesBL:IRoyContractRoyRatesBL
    {
        IRoyContractRoyRatesDAL royContractRoyRatesDAL;
        #region Constructor
        public RoyContractRoyRatesBL()
        {
            royContractRoyRatesDAL = new RoyContractRoyRatesDAL();
        }
        #endregion Constructor

        public DataSet GetRoyaltyRatesData(string royaltorId, string optionPeriod, string userRoleId, out string royaltor, out Int32 iErrorId)
        {
            return royContractRoyRatesDAL.GetRoyaltyRatesData(royaltorId, optionPeriod, userRoleId, out royaltor, out iErrorId);
        }

        public DataSet SaveRoyaltyRates(string royaltorId, string optionPeriod, string loggedUser, string userRoleId, Array addUpdateList, Array deleteList, out string royaltor, out string invalidCatno, out Int32 iErrorId)
        {
            return royContractRoyRatesDAL.SaveRoyaltyRates(royaltorId, optionPeriod, loggedUser, userRoleId, addUpdateList, deleteList, out royaltor, out invalidCatno, out iErrorId);
        }
       
      
    }
}
