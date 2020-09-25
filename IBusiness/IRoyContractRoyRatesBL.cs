using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractRoyRatesBL
    {
        DataSet GetRoyaltyRatesData(string royaltorId, string optionPeriod, string userRoleId, out string royaltor, out Int32 iErrorId);
        DataSet SaveRoyaltyRates(string royaltorId, string optionPeriod, string loggedUser, string userRoleId, Array addUpdateList, Array deleteList, out string royaltor, out string invalidCatno, out Int32 iErrorId);        
    }
}
