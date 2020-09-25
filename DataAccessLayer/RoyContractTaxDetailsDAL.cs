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
    public class RoyContractTaxDetailsDAL : IRoyContractTaxDetailsDAL
    {
        #region Global Declarations
        ConnectionDAL connDAL;
        OracleConnection orlConn;
        OracleDataAdapter orlDA;
        OracleCommand orlCmd;
        DataSet ds;
        OracleParameter ErrorId;
        string sErrorMsg;
        #endregion Global Declarations

        public DataSet GetDropdownData(string royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pPayeeList = new OracleParameter();
                OracleParameter pTaxTypeList = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_tax_details.p_get_dropdown_list_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pPayeeList.OracleDbType = OracleDbType.RefCursor;
                pPayeeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pPayeeList);

                pTaxTypeList.OracleDbType = OracleDbType.RefCursor;
                pTaxTypeList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pTaxTypeList);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "PayeeList";
                    ds.Tables[1].TableName = "TaxTypeList";
                }

            }
            catch (Exception ex)
            {
                iErrorId = 2;
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }

        public DataSet GetInitialData(string royaltorId, out string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pRoyTaxDetailsData = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_tax_details.p_get_roy_tax_details_data", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                pRoyTaxDetailsData.OracleDbType = OracleDbType.RefCursor;
                pRoyTaxDetailsData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoyTaxDetailsData);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();

            }
            catch (Exception ex)
            {
                iErrorId = 2;
            }
            finally
            {
                CloseConnection();
            }
            return ds;

        }

        public DataSet SaveRoyaltorTaxDetailsData(string royaltorId, Array royContTaxDetailsList, Array deleteList, string UserCode, out string royaltor, out Int32 iErrorId)
        {
            royaltor = string.Empty;
            ds = new DataSet();
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pRoyContTaxDetailsList = new OracleParameter();
                OracleParameter pDeleteList = new OracleParameter();
                OracleParameter pUserCode = new OracleParameter();
                OracleParameter pRoyaltor = new OracleParameter();
                OracleParameter pRoyContTaxDetails = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_tax_details.p_save_tax_details", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pRoyContTaxDetailsList.OracleDbType = OracleDbType.Varchar2;
                pRoyContTaxDetailsList.Direction = ParameterDirection.Input;
                pRoyContTaxDetailsList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (royContTaxDetailsList.Length == 0)
                {
                    pRoyContTaxDetailsList.Size = 1;
                    pRoyContTaxDetailsList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pRoyContTaxDetailsList.Size = royContTaxDetailsList.Length;
                    pRoyContTaxDetailsList.Value = royContTaxDetailsList;
                }
                orlCmd.Parameters.Add(pRoyContTaxDetailsList);

                pDeleteList.OracleDbType = OracleDbType.Varchar2;
                pDeleteList.Direction = ParameterDirection.Input;
                pDeleteList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (deleteList.Length == 0)
                {
                    pDeleteList.Size = 1;
                    pDeleteList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pDeleteList.Size = deleteList.Length;
                    pDeleteList.Value = deleteList;
                }
                orlCmd.Parameters.Add(pDeleteList);

                pUserCode.OracleDbType = OracleDbType.Varchar2;
                pUserCode.Direction = ParameterDirection.Input;
                pUserCode.Value = UserCode;
                orlCmd.Parameters.Add(pUserCode);

                pRoyContTaxDetails.OracleDbType = OracleDbType.RefCursor;
                pRoyContTaxDetails.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pRoyContTaxDetails);

                pRoyaltor.OracleDbType = OracleDbType.Varchar2;
                pRoyaltor.Size = 250;
                pRoyaltor.Direction = ParameterDirection.Output;
                pRoyaltor.ParameterName = "out_v_royaltor";
                orlCmd.Parameters.Add(pRoyaltor);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);

                royaltor = orlCmd.Parameters["out_v_royaltor"].Value.ToString();
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());
            }
            catch (Exception ex)
            {
                iErrorId = 2;
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
