using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyContractTaxDetailsBL
    {
        DataSet GetDropdownData(string royaltorId, out Int32 iErrorId);
        DataSet GetInitialData(string royaltorId, out string royaltor, out Int32 iErrorId);
        DataSet SaveRoyaltorTaxDetailsData(string royaltorId, Array royContTaxDetailsList, Array deleteList, string UserCode, out string royaltor, out Int32 iErrorId);
    }
}
