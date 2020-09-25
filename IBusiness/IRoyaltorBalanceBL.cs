using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IRoyaltorBalanceBL
    {        
        DataSet GetRoyaltorDate(string royaltorId,out Int32 iErrorId);
        DataSet GetSearchedData(string royaltorId, string dateType, string balanceDate, string voucherDate, out Int32 iErrorId);
    }
}
