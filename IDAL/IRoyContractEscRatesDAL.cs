using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyContractEscRatesDAL
    {
        DataSet GetEscRatesData(string royaltorId, out string royaltor, out Int32 iErrorId);
        //DataSet SaveEscRates(string royaltorId, string loggedUser, Array addUpdateProfileList, Array addUpdateRateList, Array deleteProfileList, Array deleteRateList, out string royaltor, out Int32 iErrorId);        
        DataSet SaveEscRates(string royaltorId, string loggedUser, Array addUpdateProfileList, Array addUpdateRateList, Array deleteProfileList, out string royaltor, out Int32 iErrorId);
        DataSet GetSalesCategoryProRata(string royaltorId, out Int32 iErrorId);
        DataSet SaveSalesCategoryProRata(string royaltorId, string loggedUser, Array addUpdateProRataList, out Int32 iErrorId);
        void AddDefaultProrata(string royaltorId, string loggedUser, out Int32 iErrorId);
        
    }
}
