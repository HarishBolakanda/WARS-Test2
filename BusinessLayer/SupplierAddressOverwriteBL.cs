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
    public class SupplierAddressOverwriteBL : ISupplierAddressOverwriteBL
    {
        ISupplierAddressOverwriteDAL SupplierAddressOverwriteDAL;
        #region Constructor
        public SupplierAddressOverwriteBL()
        {
            SupplierAddressOverwriteDAL = new SupplierAddressOverwriteDAL();
        }
        #endregion Constructor

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            return SupplierAddressOverwriteDAL.GetInitialData(out iErrorId);
        }

        public DataSet GetPayeeList(string intParty, out Int32 iErrorId)
        {
            return SupplierAddressOverwriteDAL.GetPayeeList(intParty, out iErrorId);
        }
       
        public DataSet GetPayeeSuppData(string intPartyId, string royaltorId, string supplierNumber, string supplierSiteName, out Int32 iErrorId)
        {
            return SupplierAddressOverwriteDAL.GetPayeeSuppData(intPartyId, royaltorId, supplierNumber, supplierSiteName,  out iErrorId);
        }

        public DataSet OverwritePayeeAddress(string intPartyId, string royaltorId, string supplierName, string supplierNumber, string supplierSiteName, string suppAddress1, string suppAddress2, string suppAddress3, string suppAddress4, string suppPostCode, string userCode, out Int32 iErrorId)
        {
            return SupplierAddressOverwriteDAL.OverwritePayeeAddress(intPartyId, royaltorId, supplierName, supplierNumber, supplierSiteName,  suppAddress1, suppAddress2, suppAddress3, suppAddress4, suppPostCode, userCode, out iErrorId);
        }

        public DataSet GetPayeeRoyaltorList(string intPartyId, string supplierNumber, string supplierSiteName, out Int32 iErrorId)
        {
            return SupplierAddressOverwriteDAL.GetPayeeRoyaltorList(intPartyId, supplierNumber, supplierSiteName, out iErrorId);
        }
    }
}
