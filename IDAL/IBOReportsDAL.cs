using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IBOReportsDAL
    {
        void GetBOReportFolderCuid(string rptFolderName,out string rptFolderCUID, out Int32 iErrorId);
        
        
    }
}
