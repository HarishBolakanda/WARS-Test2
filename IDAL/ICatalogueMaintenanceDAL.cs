﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ICatalogueMaintenanceDAL
    {
        DataSet GetInitialData(string catNo, out Int32 trackListingCount, out Int32 iErrorId);
        DataSet GetSearchFilters(out Int32 iErrorId);
        DataSet GetCatnoParticipants(string catNo, out Int32 iErrorId);
        DataSet SaveCatalogueDetails(string catNo, Array catalogueDetails, string userCode, out Int32 trackListingCount, out Int32 iErrorId);        
    }
}