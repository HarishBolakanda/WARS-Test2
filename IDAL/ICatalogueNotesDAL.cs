using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WARS.IDAL
{
    public interface ICatalogueNotesDAL
    {
        DataSet GetCatalogueNotes(string catNo, out Int32 iErrorId);
        void SaveCatalogueNotes(string catNo, string notes, string loggedUser, out Int32 iErrorId);        
    }
}
