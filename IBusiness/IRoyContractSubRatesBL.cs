using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractSubRatesBL
    {
        DataSet GetSubsidRatesData(string royaltorId, string userRoleId, out string royaltor, out Int32 iErrorId);
        DataSet SaveSubsidRates(string royaltorId, string loggedUser, string userRoleId, Array addUpdateList, Array deleteList, out string royaltor, out Int32 iErrorId);        

    }
}
