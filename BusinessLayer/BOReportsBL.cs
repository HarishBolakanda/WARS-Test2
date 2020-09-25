using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;


namespace WARS.BusinessLayer
{
    public class BOReportsBL:IBOReportsBL
    {
        IBOReportsDAL BOReportsDAL;
        #region Constructor
        public BOReportsBL()
        {
            BOReportsDAL = new BOReportsDAL();
        }
        #endregion Constructor

        public void GetBOReportFolderCuid(string rptFolderName, out string rptFolderCUID, out Int32 iErrorId)
        {
            BOReportsDAL.GetBOReportFolderCuid(rptFolderName, out rptFolderCUID, out iErrorId);
        }

       

      
    }
}
