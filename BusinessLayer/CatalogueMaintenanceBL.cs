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
    public class CatalogueMaintenanceBL : ICatalogueMaintenanceBL
    {
        ICatalogueMaintenanceDAL CatalogueMaintenanceDAL;
        #region Constructor
        public CatalogueMaintenanceBL()
        {
            CatalogueMaintenanceDAL = new CatalogueMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(string catNo, out Int32 trackListingCount, out Int32 iErrorId)
        {
            return CatalogueMaintenanceDAL.GetInitialData(catNo, out trackListingCount, out iErrorId);
        }

        public DataSet GetSearchFilters(out Int32 iErrorId)
        {
            return CatalogueMaintenanceDAL.GetSearchFilters(out iErrorId);
        }

        public DataSet GetCatnoParticipants(string catNo, out Int32 iErrorId)
        {
            return CatalogueMaintenanceDAL.GetCatnoParticipants(catNo, out iErrorId);
        }

        public DataSet SaveCatalogueDetails(string catNo, Array catalogueDetails, string userCode, out Int32 trackListingCount, out Int32 iErrorId)
        {
            return CatalogueMaintenanceDAL.SaveCatalogueDetails(catNo, catalogueDetails, userCode, out trackListingCount, out iErrorId);
        }

      
    }
}
