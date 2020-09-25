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
    public class InterestedPartyMaintenanceBL : IInterestedPartyMaintenanceBL
    {
        IInterestedPartyMaintenanceDAL InterestedPartyMaintenanceDAL;
        #region Constructor
        public InterestedPartyMaintenanceBL()
        {
            InterestedPartyMaintenanceDAL = new InterestedPartyMaintenanceDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return InterestedPartyMaintenanceDAL.GetInitialData(out iErrorId);
        }

        public DataSet UpdateInterestedPartyData(Int32 intPartyId, string intPartyName, string intPartyAdd1,
            string intPartyAdd2, string intPartyAdd3, string intPartyAdd4, string intPartyPostcode, string email, string vatNumber, string taxType, string isGenerateInvoice, string isSendStatement, string userCode, out Int32 iErrorId)
        {
            return InterestedPartyMaintenanceDAL.UpdateInterestedPartyData(intPartyId, intPartyName, intPartyAdd1, intPartyAdd2, intPartyAdd3, intPartyAdd4, intPartyPostcode, email, vatNumber, taxType, isGenerateInvoice,isSendStatement, userCode, out iErrorId);
        }

        public DataSet InsertInterestedPartyData(string intPartyType, string intPartyName, string intPartyAdd1,
            string intPartyAdd2, string intPartyAdd3, string intPartyAdd4, string intPartyPostcode, string email, string vatNumber, string taxType, string isGenerateInvoice, string isSendStatement, string userCode, out Int32 iErrorId)
        {
            return InterestedPartyMaintenanceDAL.InsertInterestedPartyData(intPartyType, intPartyName, intPartyAdd1, intPartyAdd2, intPartyAdd3, intPartyAdd4, intPartyPostcode, email, vatNumber, taxType, isGenerateInvoice, isSendStatement, userCode, out iErrorId);
        }

        public DataSet DeleteInterestedPartyData(Int32 intPartyId, out Int32 iErrorId)
        {
            return InterestedPartyMaintenanceDAL.DeleteInterestedPartyData(intPartyId, out iErrorId);
        }

        public DataSet GetLinkedRoyaltorDetails(Int32 intPartyId, out Int32 iErrorId)
        {
            return InterestedPartyMaintenanceDAL.GetLinkedRoyaltorDetails(intPartyId, out iErrorId);
        }

        //public DataSet GetPayeeBankDetails(Int32 intPartyId, out Int32 iErrorId)
        //{
        //    return InterestedPartyMaintenanceDAL.GetPayeeBankDetails(intPartyId, out iErrorId);
        //}

        public DataSet GetSupplierDetails(Int32 intPartyId, out Int32 iErrorId)
        {
            return InterestedPartyMaintenanceDAL.GetSupplierDetails(intPartyId, out iErrorId);
        }
    }
}
