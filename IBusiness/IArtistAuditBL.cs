using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WARS.IBusiness
{
    public interface IArtistAuditBL
    {        
        DataSet GetArtistAuditData(Int32 artistId, out string artist, out Int32 iErrorId);
    }
}
