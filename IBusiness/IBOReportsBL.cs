using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IBOReportsBL
    {
        void GetBOReportFolderCuid(string rptFolderName, out string rptFolderCUID, out Int32 iErrorId);

    }
}
