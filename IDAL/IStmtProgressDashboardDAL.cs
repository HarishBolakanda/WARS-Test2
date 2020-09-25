using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IStmtProgressDashboardDAL
    {        
        DataSet GetInitialData(out Int32 iErrorId);
        DataSet GetFilterData(string companyCode, string repSchedule, string teamResponsibility, string mngrResponsibility, string earningsCompare, string earnings, out Int32 iErrorId);        
    }
}
