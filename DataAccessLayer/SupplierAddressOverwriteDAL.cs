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
    public class SupplierAddressOverwriteDAL : ISupplierAddressOverwriteDAL
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
                OracleParameter curRoyaltorList = new OracleParameter();
                OracleParameter curSupplierList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_supplier_address_overwrite.p_get_initial_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

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

        public DataSet GetPayeeDetails(string intPartyId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                OracleParameter curPayeeDetails = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_supplier_address_overwrite.p_get_payee_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyId.OracleDbType = OracleDbType.Varchar2;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                curPayeeDetails.OracleDbType = OracleDbType.RefCursor;
                curPayeeDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeDetails);

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

        public DataSet GetPayeeList(string intParty, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntParty = new OracleParameter();
                OracleParameter curPayeeDetails = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_supplier_address_overwrite.p_get_searched_payee_list", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntParty.OracleDbType = OracleDbType.Varchar2;
                inIntParty.Direction = ParameterDirection.Input;
                inIntParty.Value = intParty;
                orlCmd.Parameters.Add(inIntParty);

                curPayeeDetails.OracleDbType = OracleDbType.RefCursor;
                curPayeeDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeDetails);

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

        public DataSet GetPayeeSuppData(string intPartyId, string royaltorId, string supplierNumber, string supplierSiteName, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter inSupplierNumber = new OracleParameter();
                OracleParameter inSupplierSiteName = new OracleParameter();
                OracleParameter curPayeeDetails = new OracleParameter();
                OracleParameter curSupplierDetails = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_supplier_address_overwrite.p_get_supp_payee_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyId.OracleDbType = OracleDbType.Varchar2;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                inRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                inSupplierNumber.OracleDbType = OracleDbType.Varchar2;
                inSupplierNumber.Direction = ParameterDirection.Input;
                inSupplierNumber.Value = supplierNumber;
                orlCmd.Parameters.Add(inSupplierNumber);

                inSupplierSiteName.OracleDbType = OracleDbType.Varchar2;
                inSupplierSiteName.Direction = ParameterDirection.Input;
                inSupplierSiteName.Value = supplierSiteName;
                orlCmd.Parameters.Add(inSupplierSiteName);

                curPayeeDetails.OracleDbType = OracleDbType.RefCursor;
                curPayeeDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeDetails);

                curSupplierDetails.OracleDbType = OracleDbType.RefCursor;
                curSupplierDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSupplierDetails);

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

        public DataSet OverwritePayeeAddress(string intPartyId, string royaltorId, string supplierName, string supplierNumber, string supplierSiteName, string suppAddress1, string suppAddress2, string suppAddress3, string suppAddress4, string suppPostCode, string userCode, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                OracleParameter inRoyaltorId = new OracleParameter();
                OracleParameter inSupplierName = new OracleParameter();
                OracleParameter inSupplierNumber = new OracleParameter();
                OracleParameter inSupplierSiteName = new OracleParameter();
                OracleParameter inSuppAddress1 = new OracleParameter();
                OracleParameter inSuppAddress2 = new OracleParameter();
                OracleParameter inSuppAddress3 = new OracleParameter();
                OracleParameter inSuppAddress4 = new OracleParameter();
                OracleParameter inSuppPostcode = new OracleParameter();
                OracleParameter inUserCode = new OracleParameter();
                OracleParameter curPayeeDetails = new OracleParameter();
                OracleParameter curSupplierDetails = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_supplier_address_overwrite.p_overwrite_payee_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyId.OracleDbType = OracleDbType.Varchar2;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                inRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                inRoyaltorId.Direction = ParameterDirection.Input;
                inRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(inRoyaltorId);

                inSupplierName.OracleDbType = OracleDbType.Varchar2;
                inSupplierName.Direction = ParameterDirection.Input;
                inSupplierName.Value = supplierName;
                orlCmd.Parameters.Add(inSupplierName);

                inSupplierNumber.OracleDbType = OracleDbType.Varchar2;
                inSupplierNumber.Direction = ParameterDirection.Input;
                inSupplierNumber.Value = supplierNumber;
                orlCmd.Parameters.Add(inSupplierNumber);

                inSupplierSiteName.OracleDbType = OracleDbType.Varchar2;
                inSupplierSiteName.Direction = ParameterDirection.Input;
                inSupplierSiteName.Value = supplierSiteName;
                orlCmd.Parameters.Add(inSupplierSiteName);

                inSuppAddress1.OracleDbType = OracleDbType.Varchar2;
                inSuppAddress1.Direction = ParameterDirection.Input;
                inSuppAddress1.Value = suppAddress1;
                orlCmd.Parameters.Add(inSuppAddress1);

                inSuppAddress2.OracleDbType = OracleDbType.Varchar2;
                inSuppAddress2.Direction = ParameterDirection.Input;
                inSuppAddress2.Value = suppAddress2;
                orlCmd.Parameters.Add(inSuppAddress2);

                inSuppAddress3.OracleDbType = OracleDbType.Varchar2;
                inSuppAddress3.Direction = ParameterDirection.Input;
                inSuppAddress3.Value = suppAddress3;
                orlCmd.Parameters.Add(inSuppAddress3);

                inSuppAddress4.OracleDbType = OracleDbType.Varchar2;
                inSuppAddress4.Direction = ParameterDirection.Input;
                inSuppAddress4.Value = suppAddress4;
                orlCmd.Parameters.Add(inSuppAddress4);

                inSuppPostcode.OracleDbType = OracleDbType.Varchar2;
                inSuppPostcode.Direction = ParameterDirection.Input;
                inSuppPostcode.Value = suppPostCode;
                orlCmd.Parameters.Add(inSuppPostcode);

                inUserCode.OracleDbType = OracleDbType.Varchar2;
                inUserCode.Direction = ParameterDirection.Input;
                inUserCode.Value = userCode;
                orlCmd.Parameters.Add(inUserCode);

                curPayeeDetails.OracleDbType = OracleDbType.RefCursor;
                curPayeeDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeDetails);

                curSupplierDetails.OracleDbType = OracleDbType.RefCursor;
                curSupplierDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curSupplierDetails);

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

        public DataSet GetPayeeRoyaltorList(string intPartyId, string supplierNumber, string supplierSiteName, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);

                ErrorId = new OracleParameter();
                OracleParameter inIntPartyId = new OracleParameter();
                OracleParameter inSupplierNumber = new OracleParameter();
                OracleParameter inSupplierSiteName = new OracleParameter();
                OracleParameter curPayeeRoyaltorList = new OracleParameter();
                orlDA = new OracleDataAdapter();

                orlCmd = new OracleCommand("pkg_supplier_address_overwrite.p_get_payee_royaltor_list", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inIntPartyId.OracleDbType = OracleDbType.Varchar2;
                inIntPartyId.Direction = ParameterDirection.Input;
                inIntPartyId.Value = intPartyId;
                orlCmd.Parameters.Add(inIntPartyId);

                inSupplierNumber.OracleDbType = OracleDbType.Varchar2;
                inSupplierNumber.Direction = ParameterDirection.Input;
                inSupplierNumber.Value = supplierNumber;
                orlCmd.Parameters.Add(inSupplierNumber);

                inSupplierSiteName.OracleDbType = OracleDbType.Varchar2;
                inSupplierSiteName.Direction = ParameterDirection.Input;
                inSupplierSiteName.Value = supplierSiteName;
                orlCmd.Parameters.Add(inSupplierSiteName);

                curPayeeRoyaltorList.OracleDbType = OracleDbType.RefCursor;
                curPayeeRoyaltorList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(curPayeeRoyaltorList);

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
