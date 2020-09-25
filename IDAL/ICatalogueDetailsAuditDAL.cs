using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ICatalogueDetailsAuditDAL
    {
        DataSet GetSearchedData(string catNo, out Int32 iErrorId); 
    }
}
