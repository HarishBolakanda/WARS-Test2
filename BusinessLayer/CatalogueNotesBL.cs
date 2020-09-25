using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARS.DataAccessLayer;
using WARS.IBusiness;
using WARS.IDAL;

namespace WARS.BusinessLayer
{
    public class CatalogueNotesBL : ICatalogueNotesBL
    {
        ICatalogueNotesDAL catalogueNotesDAL;
        #region Constructor
        public CatalogueNotesBL()
        {
            catalogueNotesDAL = new CatalogueNotesDAL();
        }
        #endregion Constructor

        public DataSet GetCatalogueNotes(string catNo,  out Int32 iErrorId)
        {
            return catalogueNotesDAL.GetCatalogueNotes(catNo, out iErrorId);
        }

        public void SaveCatalogueNotes(string catNo, string notes, string loggedUser, out Int32 iErrorId)
        {
            catalogueNotesDAL.SaveCatalogueNotes(catNo, notes, loggedUser, out iErrorId);
        }
    }
}
