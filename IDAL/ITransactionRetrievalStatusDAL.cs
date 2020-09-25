using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;
namespace WARS.IDAL
{
    public interface ITransactionRetrievalStatusDAL
    {
        DataSet GetInitialTransactionData(out Int32 iErrorId);        
        DataSet GetSearchData(string royaltorId, string optionPeriodCode, string artistId, string catNumber, string projectCode, out Int32 iErrorId);
    }
}
