using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface ITerritoryGroupRoyaltorsBL
    {        
        DataSet GetRoyaltorList(string territoryGroupCode, out Int32 iErrorId);
    }
}
