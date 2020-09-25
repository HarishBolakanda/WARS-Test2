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
    public class RoyContractPayeeSuppBL : IRoyContractPayeeSuppBL
    {
        IRoyContractPayeeSuppDAL royContractPayeeSuppDAL;
        #region Constructor
        public RoyContractPayeeSuppBL()
        {
            royContractPayeeSuppDAL = new RoyContractPayeeSuppDAL();
        }
        #endregion Constructor

        public DataSet GetDropdownData(string royaltorId, out Int32 iErrorId)
        {
            return royContractPayeeSuppDAL.GetDropdownData(royaltorId, out iErrorId);
        }

        public DataSet GetInitialData(string royaltorId, out string royaltor, out string supplierSiteNameRegValue, out Int32 iErrorId)
        {
            return royContractPayeeSuppDAL.GetInitialData(royaltorId, out royaltor, out supplierSiteNameRegValue, out iErrorId);
        }

        public DataSet GetPayeeSupplier(string royaltorId, string intPartyId, out Int32 iErrorId)
        {
            return royContractPayeeSuppDAL.GetPayeeSupplier(royaltorId,intPartyId, out iErrorId);
        }

        public DataSet SavePayeeSupplier(string royaltorId, string intPartyId, string payeeCurrency, string supplierNumberOld, string supplierNumber, string supplierSiteNameOld, string supplierSiteName, string accountCompany, string mismatchFlag, string saveType, string userCode, out Int32 iErrorId)
        {
            return royContractPayeeSuppDAL.SavePayeeSupplier(royaltorId, intPartyId, payeeCurrency, supplierNumberOld, supplierNumber, supplierSiteNameOld, supplierSiteName, accountCompany, mismatchFlag, saveType, userCode, out iErrorId);
        }



    }
}
