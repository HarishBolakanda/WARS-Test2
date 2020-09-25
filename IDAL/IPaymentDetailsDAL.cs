using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface IPaymentDetailsDAL
    {
        DataSet GetInitialData(out Int32 iErrorId);        
        DataSet GetSupplierList(string searchText, out Int32 iErrorId);
        DataSet GetSearchedData(string paymentNo, string paymentDate, string statusCode, string supplierNumber, string supplierSiteName, string paymentAmount, string filename, string royaltorId, out Int32 iErrorId);
        void CreateAPInterface();
    }
}
