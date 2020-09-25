using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IRoyaltorBalanceDAL
    {        
        DataSet GetRoyaltorDate(string royaltorId,out Int32 iErrorId);
        DataSet GetSearchedData(string royaltorId, string dateType, string balanceDate, string voucherDate, out Int32 iErrorId);
       
    }
}
