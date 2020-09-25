using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IArtistAuditDAL
    {        
        DataSet GetArtistAuditData(Int32 artistId, out string artist, out Int32 iErrorId);
    }
}
