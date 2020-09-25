using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using WARS.IDAL;

namespace WARS.DataAccessLayer
{
    public class FuzzySearchDAL : IFuzzySearchDAL
    {

        #region Global Declarations        
        ConnectionDAL connDAL;
        OracleConnection orlConn;        
        OracleDataAdapter orlDA = new OracleDataAdapter();
        OracleCommand orlCmd;
        OracleParameter pFuzzySearchList = new OracleParameter();
        OracleParameter ErrorId  = new OracleParameter();
        DataSet ds = new DataSet();
        string sErrorMsg;
        #endregion Global Declarations

        #region DB Procedures
        string getAllRoyaltorList = "pkg_screens_fuzzy_search.p_get_all_royaltor_list";
        string getAllRoyaltorListWithOwnerCode = "pkg_screens_fuzzy_search.p_all_roy_list_with_owner_code";
        string getAllArtistList = "pkg_screens_fuzzy_search.p_get_all_artist_list";
        string getAllCompanyList = "pkg_screens_fuzzy_search.p_get_all_company_list";
        string getAllIntPartyList = "pkg_screens_fuzzy_search.p_get_all_int_party_list";        
        string getAllOwnerList = "pkg_screens_fuzzy_search.p_get_all_owner_list";
        string getAllCatalogueList = "pkg_screens_fuzzy_search.p_get_all_catalogue_list";
        string getAllConfigGroupList = "pkg_screens_fuzzy_search.p_get_all_config_group_list";
        string getConfigGroupListTypeP = "pkg_screens_fuzzy_search.p_get_config_group_list_type_p";
        string getConfigGroupListTypeC = "pkg_screens_fuzzy_search.p_get_config_group_list_type_c";
        string getAllSellerGroupList = "pkg_screens_fuzzy_search.p_get_all_seller_group_list";
        string getSellerGroupListTypeC = "pkg_screens_fuzzy_search.p_get_seller_group_list_type_c";
        string getSellerGroupListTypeP = "pkg_screens_fuzzy_search.p_get_seller_group_list_type_p";
        string getAllPriceGroupList = "pkg_screens_fuzzy_search.p_get_all_price_group_list";
        string getPriceGroupListTypeC = "pkg_screens_fuzzy_search.p_get_price_group_list_type_c";
        string getPriceGroupListTypeP = "pkg_screens_fuzzy_search.p_get_price_group_list_type_p";
        string getAllProjectList = "pkg_screens_fuzzy_search.p_get_all_project_list";
        string getAllPayeeList = "pkg_screens_fuzzy_search.p_get_all_payee_list";
        string getTransRetrvRoyOptPrdList = "pkg_screens_fuzzy_search.p_trasn_retrv_roy_opt_prd_list";
        string getTransRetrvCatArtistList = "pkg_screens_fuzzy_search.p_trasn_retrv_cat_artist_list";
        string getAllLabelList = "pkg_screens_fuzzy_search.p_get_all_label_list";
        string getAllResponsibilityList = "pkg_screens_fuzzy_search.p_get_all_responsibility_list";
        

        #endregion DB Procedures

        public DataSet GetFuzzySearchList(string searchList, out Int32 iErrorId)
        {

            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlCmd = new OracleCommand(GetDBProcName(searchList), orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pFuzzySearchList.OracleDbType = OracleDbType.RefCursor;
                pFuzzySearchList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pFuzzySearchList);

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

        //JIRA-898 Changes -- Ravi -- STart
        public DataSet GetFuzzySearchListTransRetr(string searchList, string userRoleId,  out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                OracleParameter inUserRoleId = new OracleParameter();
                orlCmd = new OracleCommand(GetDBProcName(searchList), orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                inUserRoleId.OracleDbType = OracleDbType.Varchar2;
                inUserRoleId.Direction = ParameterDirection.Input;
                inUserRoleId.Value = userRoleId;
                orlCmd.Parameters.Add(inUserRoleId);

                pFuzzySearchList.OracleDbType = OracleDbType.RefCursor;
                pFuzzySearchList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pFuzzySearchList);

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

        //JIRA-898 Changes -- Ravi --ENd

        private string GetDBProcName(string searchList)
        {
            
            string DBProcName = string.Empty;
            switch (searchList)
            { 
                case "GetAllRoyaltorList":
                    DBProcName = getAllRoyaltorList;
                    break;
                case "GetAllRoyaltorListWithOwnerCode":
                    DBProcName = getAllRoyaltorListWithOwnerCode;
                    break;
                case "GetAllArtistList":
                    DBProcName = getAllArtistList;
                    break;
                case "GetAllCompanyList":
                    DBProcName = getAllCompanyList;
                    break;
                case "GetAllIntPartyList":
                    DBProcName = getAllIntPartyList;
                    break;                
                case "GetAllOwnerList":
                    DBProcName = getAllOwnerList;
                    break;
                case "GetAllCatalogueList":
                    DBProcName = getAllCatalogueList;
                    break;
                case "GetAllConfigGroupList":
                    DBProcName = getAllConfigGroupList;
                    break;
                case "GetConfigGroupListTypeP":
                    DBProcName = getConfigGroupListTypeP;
                    break;
                case "GetConfigGroupListTypeC":
                    DBProcName = getConfigGroupListTypeC;
                    break;
                case "GetAllSellerGroupList":
                    DBProcName = getAllSellerGroupList;
                    break;
                case "GetSellerGroupListTypeC":
                    DBProcName = getSellerGroupListTypeC;
                    break;
                case "GetSellerGroupListTypeP":
                    DBProcName = getSellerGroupListTypeP;
                    break;
                case "GetAllPriceGroupList":
                    DBProcName = getAllPriceGroupList;
                    break;
                case "GetPriceGroupListTypeC":
                    DBProcName = getPriceGroupListTypeC;
                    break;
                case "GetPriceGroupListTypeP":
                    DBProcName = getPriceGroupListTypeP;
                    break;
                case "GetAllProjectList":
                    DBProcName = getAllProjectList;
                    break;
                case "GetAllPayeeList":
                    DBProcName = getAllPayeeList;
                    break;
                case "GetTransRetrvRoyOptPrdList":
                    DBProcName = getTransRetrvRoyOptPrdList;
                    break;
                case "GetTransRetrvCatArtistList":
                    DBProcName = getTransRetrvCatArtistList;
                    break;
                case "GetAllLabelList":
                    DBProcName = getAllLabelList;
                    break;
                case "GetAllResponsibilityList":
                    DBProcName = getAllResponsibilityList;
                    break;

            }

            return DBProcName;
        }

        /*
        public DataSet GetAllRoyaltorList(out Int32 iErrorId)
        {
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                  
                orlCmd = new OracleCommand("pkg_screens_fuzzy_search.p_get_all_royaltor_list", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pFuzzySearchList.OracleDbType = OracleDbType.RefCursor;
                pFuzzySearchList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pFuzzySearchList);
                
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

        public DataSet GetAllArtistList(out Int32 iErrorId)
        {
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlCmd = new OracleCommand("pkg_screens_fuzzy_search.p_get_all_artist_list", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pFuzzySearchList.OracleDbType = OracleDbType.RefCursor;
                pFuzzySearchList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pFuzzySearchList);

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

        public DataSet GetAllComanyList(out Int32 iErrorId)
        {
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlCmd = new OracleCommand("pkg_screens_fuzzy_search.p_get_all_company_list", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pFuzzySearchList.OracleDbType = OracleDbType.RefCursor;
                pFuzzySearchList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pFuzzySearchList);

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

        public DataSet GetAllIntPartyList(out Int32 iErrorId)
        {
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);
                orlCmd = new OracleCommand(getAllIntPartyList, orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pFuzzySearchList.OracleDbType = OracleDbType.RefCursor;
                pFuzzySearchList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pFuzzySearchList);

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

        public DataSet GetTerritoryListTypeP(out Int32 iErrorId)
        {
            
            try
            {
                OpenConnection(out iErrorId, out sErrorMsg);                
                orlCmd = new OracleCommand("pkg_screens_fuzzy_search.p_get_territory_list_type_p", orlConn);
                orlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                pFuzzySearchList.OracleDbType = OracleDbType.RefCursor;
                pFuzzySearchList.Direction = System.Data.ParameterDirection.Output;
                orlCmd.Parameters.Add(pFuzzySearchList);

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
       
        */

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
