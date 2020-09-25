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
    public class RoyContractEscRatesBL : IRoyContractEscRatesBL
    {
        IRoyContractEscRatesDAL royContractEscRatesDAL;
        #region Constructor
        public RoyContractEscRatesBL()
        {
            royContractEscRatesDAL = new RoyContractEscRatesDAL();
        }
        #endregion Constructor

        public DataSet GetEscRatesData(string royaltorId, out string royaltor, out Int32 iErrorId)
        {
            return royContractEscRatesDAL.GetEscRatesData(royaltorId, out royaltor, out iErrorId);
        }

        public DataSet SaveEscRates(string royaltorId, string loggedUser, Array addUpdateProfileList, Array addUpdateRateList, Array deleteProfileList, out string royaltor, out Int32 iErrorId)
        {
            return royContractEscRatesDAL.SaveEscRates(royaltorId, loggedUser, addUpdateProfileList, addUpdateRateList, deleteProfileList, out royaltor, out iErrorId);
        }

        public DataSet GetSalesCategoryProRata(string royaltorId, out Int32 iErrorId)
        {
            return royContractEscRatesDAL.GetSalesCategoryProRata(royaltorId, out iErrorId);
        }

        public DataSet SaveSalesCategoryProRata(string royaltorId, string loggedUser, Array addUpdateProRataList, out Int32 iErrorId)
        {
            return royContractEscRatesDAL.SaveSalesCategoryProRata(royaltorId, loggedUser, addUpdateProRataList, out iErrorId);
        }

        public void AddDefaultProrata(string royaltorId, string loggedUser, out Int32 iErrorId)
        {
            royContractEscRatesDAL.AddDefaultProrata(royaltorId, loggedUser, out iErrorId);
        }
       
      
    }
}
