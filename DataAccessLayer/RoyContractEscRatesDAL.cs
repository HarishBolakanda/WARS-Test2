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
    public class RoyContractEscRatesDAL : IRoyContractEscRatesDAL
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

        public DataSet GetEscRatesData(string royaltorId, out string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();                
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();                
                OracleParameter pEscRatesData = new OracleParameter();                                
                OracleParameter pRoyaltor = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_esc_rates.p_get_contract_esc_rates", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pEscRatesData.OracleDbType = OracleDbType.RefCursor;
                pEscRatesData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscRatesData);

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

                
        public DataSet SaveEscRates(string royaltorId, string loggedUser, Array addUpdateProfileList, Array addUpdateRateList, Array deleteProfileList, out string royaltor, out Int32 iErrorId)
        {
            ds = new DataSet();
            royaltor = string.Empty;
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();                
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pAddUpdateProfileList = new OracleParameter();
                OracleParameter pAddUpdateRateList = new OracleParameter();
                OracleParameter pDeleteProfileList = new OracleParameter();
                OracleParameter pDeleteRateList = new OracleParameter();
                OracleParameter pEscRatesData = new OracleParameter();                                
                OracleParameter pRoyaltor = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_esc_rates.p_save_contract_esc_rates", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);
                
                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pAddUpdateProfileList.OracleDbType = OracleDbType.Varchar2;
                pAddUpdateProfileList.Direction = ParameterDirection.Input;
                pAddUpdateProfileList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (addUpdateProfileList.Length == 0)
                {
                    pAddUpdateProfileList.Size = 1;
                    pAddUpdateProfileList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pAddUpdateProfileList.Size = addUpdateProfileList.Length;
                    pAddUpdateProfileList.Value = addUpdateProfileList;
                }
                orlCmd.Parameters.Add(pAddUpdateProfileList);

                pAddUpdateRateList.OracleDbType = OracleDbType.Varchar2;
                pAddUpdateRateList.Direction = ParameterDirection.Input;
                pAddUpdateRateList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (addUpdateRateList.Length == 0)
                {
                    pAddUpdateRateList.Size = 1;
                    pAddUpdateRateList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pAddUpdateRateList.Size = addUpdateRateList.Length;
                    pAddUpdateRateList.Value = addUpdateRateList;
                }
                orlCmd.Parameters.Add(pAddUpdateRateList);
                
                pDeleteProfileList.OracleDbType = OracleDbType.Varchar2;
                pDeleteProfileList.Direction = ParameterDirection.Input;
                pDeleteProfileList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (deleteProfileList.Length == 0)
                {
                    pDeleteProfileList.Size = 1;
                    pDeleteProfileList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pDeleteProfileList.Size = deleteProfileList.Length;
                    pDeleteProfileList.Value = deleteProfileList;
                }
                orlCmd.Parameters.Add(pDeleteProfileList);

                pEscRatesData.OracleDbType = OracleDbType.RefCursor;
                pEscRatesData.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscRatesData);

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


        public DataSet GetSalesCategoryProRata(string royaltorId, out Int32 iErrorId)
        {
            ds = new DataSet();            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pEscProfiles = new OracleParameter();
                OracleParameter pProRataPriceGroup = new OracleParameter();
                OracleParameter pProRataEscalations = new OracleParameter();                

                orlCmd = new OracleCommand("pkg_roy_contract_esc_rates.p_get_sales_category_prorata", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pEscProfiles.OracleDbType = OracleDbType.RefCursor;
                pEscProfiles.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscProfiles);

                pProRataPriceGroup.OracleDbType = OracleDbType.RefCursor;
                pProRataPriceGroup.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pProRataPriceGroup);

                pProRataEscalations.OracleDbType = OracleDbType.RefCursor;
                pProRataEscalations.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pProRataEscalations);
                               
                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "dtEscProfiles";
                    ds.Tables[1].TableName = "dtProRataPriceGroup";
                    ds.Tables[2].TableName = "dtProRataEscalations";
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


        public DataSet SaveSalesCategoryProRata(string royaltorId, string loggedUser, Array addUpdateProRataList, out Int32 iErrorId)
        {
            ds = new DataSet();
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();
                OracleParameter pAddUpdateProRataList = new OracleParameter();
                OracleParameter pEscProfiles = new OracleParameter();
                OracleParameter pProRataPriceGroup = new OracleParameter();
                OracleParameter pProRataEscalations = new OracleParameter();                

                orlCmd = new OracleCommand("pkg_roy_contract_esc_rates.p_save_sales_category_prorata", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                pAddUpdateProRataList.OracleDbType = OracleDbType.Varchar2;
                pAddUpdateProRataList.Direction = ParameterDirection.Input;
                pAddUpdateProRataList.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                if (addUpdateProRataList.Length == 0)
                {
                    pAddUpdateProRataList.Size = 1;
                    pAddUpdateProRataList.Value = new OracleDecimal[1] { OracleDecimal.Null };
                }
                else
                {
                    pAddUpdateProRataList.Size = addUpdateProRataList.Length;
                    pAddUpdateProRataList.Value = addUpdateProRataList;
                }
                orlCmd.Parameters.Add(pAddUpdateProRataList);

                pEscProfiles.OracleDbType = OracleDbType.RefCursor;
                pEscProfiles.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pEscProfiles);

                pProRataPriceGroup.OracleDbType = OracleDbType.RefCursor;
                pProRataPriceGroup.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pProRataPriceGroup);

                pProRataEscalations.OracleDbType = OracleDbType.RefCursor;
                pProRataEscalations.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pProRataEscalations);
                                
                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlDA = new OracleDataAdapter(orlCmd);
                orlDA.Fill(ds);
                iErrorId = Convert.ToInt32(orlCmd.Parameters["ErrorId"].Value.ToString());

                if (iErrorId != 2)
                {
                    ds.Tables[0].TableName = "dtEscProfiles";
                    ds.Tables[1].TableName = "dtProRataPriceGroup";
                    ds.Tables[2].TableName = "dtProRataEscalations";
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

        public void AddDefaultProrata(string royaltorId, string loggedUser, out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlDA = new OracleDataAdapter();
                ErrorId = new OracleParameter();
                OracleParameter pRoyaltorId = new OracleParameter();
                OracleParameter pLoggedUser = new OracleParameter();

                orlCmd = new OracleCommand("pkg_roy_contract_esc_rates.p_add_default_prorata", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pRoyaltorId.OracleDbType = OracleDbType.Varchar2;
                pRoyaltorId.Direction = ParameterDirection.Input;
                pRoyaltorId.Value = royaltorId;
                orlCmd.Parameters.Add(pRoyaltorId);

                pLoggedUser.OracleDbType = OracleDbType.Varchar2;
                pLoggedUser.Direction = ParameterDirection.Input;
                pLoggedUser.Value = loggedUser;
                orlCmd.Parameters.Add(pLoggedUser);

                ErrorId.OracleDbType = OracleDbType.Int32;
                ErrorId.Direction = ParameterDirection.Output;
                ErrorId.ParameterName = "ErrorId";
                orlCmd.Parameters.Add(ErrorId);

                orlCmd.ExecuteNonQuery();

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
