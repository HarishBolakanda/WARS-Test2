using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WARS.IDAL;
using Oracle.DataAccess.Types;

namespace WARS.DataAccessLayer
{
    public class PaymentDetailsDAL : IPaymentDetailsDAL
    {
        #region Global Declarations
        ConnectionDAL connDAL;
        OracleConnection orlConn;
        OracleTransaction txn;
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        OracleParameter ErrorId;
        string sErrorMsg;
        #endregion Global Declarations

        public DataSet GetInitialData(out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();                
                OracleParameter curStatusList = new OracleParameter();                
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_payment_details.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                curStatusList.OracleDbType = OracleDbType.RefCursor;
                curStatusList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curStatusList);
                                
                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet GetSupplierList(string searchText, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inSearchText = new OracleParameter();
                OracleParameter curSupplierList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_payment_details.p_get_searched_supplier_list", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inSearchText.OracleDbType = OracleDbType.Varchar2;
                inSearchText.Direction = ParameterDirection.Input;
                inSearchText.Value = searchText;
                orlCmd.Parameters.Add(inSearchText);

                curSupplierList.OracleDbType = OracleDbType.RefCursor;
                curSupplierList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSupplierList);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet GetSearchedData(string paymentNo, string paymentDate, string statusCode, string supplierNumber, string supplierSiteName, string paymentAmount, string filename, string royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inPaymentNo = new OracleParameter();
                OracleParameter inPaymentDate = new OracleParameter();
                OracleParameter inStatusCode = new OracleParameter();
                OracleParameter inSupplierNumber = new OracleParameter();
                OracleParameter inSupplierSiteName = new OracleParameter();
                OracleParameter inPaymentAmount = new OracleParameter();
                OracleParameter inFilename = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter curPaymentDetails = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_payment_details.p_get_searched_payment_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inPaymentNo.OracleDbType = OracleDbType.Varchar2;
                inPaymentNo.Direction = ParameterDirection.Input;
                inPaymentNo.Value = paymentNo;
                orlCmd.Parameters.Add(inPaymentNo);

                inPaymentDate.OracleDbType = OracleDbType.Varchar2;
                inPaymentDate.Direction = ParameterDirection.Input;
                inPaymentDate.Value = paymentDate;
                orlCmd.Parameters.Add(inPaymentDate);

                inStatusCode.OracleDbType = OracleDbType.Varchar2;
                inStatusCode.Direction = ParameterDirection.Input;
                inStatusCode.Value = statusCode;
                orlCmd.Parameters.Add(inStatusCode);

                inSupplierNumber.OracleDbType = OracleDbType.Varchar2;
                inSupplierNumber.Direction = ParameterDirection.Input;
                inSupplierNumber.Value = supplierNumber;
                orlCmd.Parameters.Add(inSupplierNumber);

                inSupplierSiteName.OracleDbType = OracleDbType.Varchar2;
                inSupplierSiteName.Direction = ParameterDirection.Input;
                inSupplierSiteName.Value = supplierSiteName;
                orlCmd.Parameters.Add(inSupplierSiteName);

                inPaymentAmount.OracleDbType = OracleDbType.Varchar2;
                inPaymentAmount.Direction = ParameterDirection.Input;
                inPaymentAmount.Value = paymentAmount;
                orlCmd.Parameters.Add(inPaymentAmount);

                inFilename.OracleDbType = OracleDbType.Varchar2;
                inFilename.Direction = ParameterDirection.Input;
                inFilename.Value = filename;
                orlCmd.Parameters.Add(inFilename);

                inRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                curPaymentDetails.OracleDbType = OracleDbType.RefCursor;
                curPaymentDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPaymentDetails);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

            }
            catch (Exception ex)
            {
                iErrorId = 2;
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public void CreateAPInterface()
        {
            ds = new DataSet();
            try
            {
                Int32 iErrorId;
                OpenConnection(out iErrorId, out sErrorMsg);

                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_payment_file_interface.payment_file_interface_job", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;
               
                orlCmd.ExecuteNonQuery();            

            }
            catch (Exception ex)
            {
                 throw ex;
            }
            finally
            {
                CloseConnection();
            }

        }

        #region Private Methods
        private void OpenConnection(out Int32 iErrorId, out string sErrorMsg)
        {
            connDAL = new ConnectionDAL();
            orlConn = connDAL.Open(out iErrorId, out sErrorMsg);
        }

        public void CloseConnection()
        {
            if (connDAL != null)
            {
                connDAL.Close();
            }
        }
        #endregion
    }
}
