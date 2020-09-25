/*
File Name   :   FuzzySearch.asmx.cs
Purpose     :   Creating web methods for fuzzy search functionality throughout the application

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     27-Feb-2019     Harish                    Initial Creation(WUIN-929)
        
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using WARS.BusinessLayer;

namespace WARS.Services
{
    /// <summary>
    /// Summary description for FuzzySearch
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class FuzzySearch : System.Web.Services.WebService
    {
        Int32 errorId;
        DataSet dsList = new DataSet();
        DataTable dtList = new DataTable();
        List<string> fuzzyList = new List<string>();
        FuzzySearchBL fuzzySearchBL = new FuzzySearchBL();
        int returnListCount = 20;

        #region Common to all screens

        /// <summary>
        /// Screen Name : Used in all screens where complete royaltor list is required
        /// Retuns list of first 20 Royaltor matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllRoyaltorList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchAllRoyaltorList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllRoyaltorList", out errorId);
                    Session["FuzzySearchAllRoyaltorList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllRoyaltorList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllRoyaltorList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["royaltor"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete royaltor list appended with owner code is required
        /// Retuns list of first 20 Royaltor matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllRoyListWithOwnerCode(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchAllRoyListWithOwnerCode"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllRoyaltorListWithOwnerCode", out errorId);
                    Session["FuzzySearchAllRoyListWithOwnerCode"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllRoyListWithOwnerCode"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllRoyListWithOwnerCode"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["royaltor"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete artist list is required
        /// Retuns list of first 20 artists  matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllArtisList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchAllArtistList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllArtistList", out errorId);
                    Session["FuzzySearchAllArtistList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllArtistList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllArtistList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("artist").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("artist").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["artist"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete company list is required
        /// Retuns list of first 20 companies  matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllCompanyList(string prefixText, int count)
        {
            try
            {
                if (Session["FuzzySearchAllCompanyList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllCompanyList", out errorId);
                    Session["FuzzySearchAllCompanyList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllCompanyList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllCompanyList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("company").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("company").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["company"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in screens where all config group list is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllConfigGroupList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchAllConfigGroupList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllConfigGroupList", out errorId);
                    Session["FuzzySearchAllConfigGroupList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllConfigGroupList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllConfigGroupList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("config_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("config_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["config_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in ConfigurationGroupAudit.aspx,ConfigurationGrouping.aspx screens where config group list of type P is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchConfigGroupListTypeP(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchConfigGroupListTypeP"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetConfigGroupListTypeP", out errorId);
                    Session["FuzzySearchConfigGroupListTypeP"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchConfigGroupListTypeP"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchConfigGroupListTypeP"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("config_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("config_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["config_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where config group list of type C is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchConfigGroupListTypeC(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchConfigGroupListTypeC"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetConfigGroupListTypeC", out errorId);
                    Session["FuzzySearchConfigGroupListTypeC"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchConfigGroupListTypeC"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchConfigGroupListTypeC"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("config_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("config_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["config_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in screens where seller group list is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllSellerGroupList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchAllSellerGroupList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllSellerGroupList", out errorId);
                    Session["FuzzySearchAllSellerGroupList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllSellerGroupList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllSellerGroupList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["seller_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in TerritoryGroupAudit.aspx screens where seller group list of type C is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchSellerGroupListTypeC(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchSellerGroupListTypeC"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetSellerGroupListTypeC", out errorId);
                    Session["FuzzySearchSellerGroupListTypeC"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchSellerGroupListTypeC"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchSellerGroupListTypeC"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["seller_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in TerritoryGroupAudit.aspx screens where seller group list of type P is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchSellerGroupListTypeP(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchSellerGroupListTypeP"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetSellerGroupListTypeP", out errorId);
                    Session["FuzzySearchSellerGroupListTypeP"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchSellerGroupListTypeP"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchSellerGroupListTypeP"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["seller_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in screens where price group list is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllPriceGroupList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchAllPriceGroupList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllPriceGroupList", out errorId);
                    Session["FuzzySearchAllPriceGroupList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllPriceGroupList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllPriceGroupList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["price_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in screens where price group list of type C is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchPriceGroupListTypeC(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchPriceGroupListTypeC"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetPriceGroupListTypeC", out errorId);
                    Session["FuzzySearchPriceGroupListTypeC"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchPriceGroupListTypeC"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchPriceGroupListTypeC"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["price_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in screens where price group list of type P is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchPriceGroupListTypeP(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzySearchPriceGroupListTypeP"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetPriceGroupListTypeP", out errorId);
                    Session["FuzzySearchPriceGroupListTypeP"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchPriceGroupListTypeP"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchPriceGroupListTypeP"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["price_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete Interested party list is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchIntPartyList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchIntPartyList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllIntPartyList", out errorId);
                    Session["FuzzySearchIntPartyList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchIntPartyList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchIntPartyList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("int_party").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("int_party").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["int_party"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in Interested party audit (WUIN-951)
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchIPNumberList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchIPNumberList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllIntPartyList", out errorId);
                    Session["FuzzySearchIPNumberList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchIPNumberList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchIPNumberList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("ip_number").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("ip_number").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["ip_number"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }


        /// <summary>
        /// Screen Name : Used in all screens where complete Owner list is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllOwnerList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchAllOwnerList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllOwnerList", out errorId);
                    Session["FuzzySearchAllOwnerList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllOwnerList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllOwnerList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("owner").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("owner").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["owner"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete Catalogue list is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllCatalogueList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchAllCatalogueList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllCatalogueList", out errorId);
                    Session["FuzzySearchAllCatalogueList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllCatalogueList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllCatalogueList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("catno").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("catno").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["catno"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete project list is required
        /// Retuns list of first 20 Royaltor matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllProjectList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchAllProjectList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllProjectList", out errorId);
                    Session["FuzzySearchAllProjectList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllProjectList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllProjectList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("project").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("project").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["project"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete payee list is required
        /// Retuns list of first 20 Royaltor matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllPayeeList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchAllPayeeList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllPayeeList", out errorId);
                    Session["FuzzySearchAllPayeeList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllPayeeList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllPayeeList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("payee").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("payee").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["payee"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete Label list is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllLabelList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchAllLabelList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllLabelList", out errorId);
                    Session["FuzzySearchAllLabelList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllLabelList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllLabelList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("label").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("label").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["label"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in all screens where complete Responsibility list is required
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchAllResponsibilityList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzySearchAllResponsibilityList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetAllResponsibilityList", out errorId);
                    Session["FuzzySearchAllResponsibilityList"] = dsList.Tables[0];
                }


                if (Session["FuzzySearchAllResponsibilityList"] != null)
                {
                    dtList = (DataTable)Session["FuzzySearchAllResponsibilityList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("responsibility").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("responsibility").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["responsibility"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        #endregion Common to all screens

        #region Contract Maintenance screens

        /// <summary>
        /// Screen Name : Used in Contract/RoyContractPayeeSupp.aspx screen to search supplier
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchContPayeeSuppSupplierList(string prefixText, int count)
        {

            try
            {

                if (Session["ContPayeeSupplierList"] != null)
                {
                    dtList = (DataTable)Session["ContPayeeSupplierList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("supplier").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("supplier").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["supplier"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in Contract/RoyContractRoyRates.aspx screen to search in selected price group
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchContRoyRatesSalesTypeList(string prefixText, int count)
        {

            try
            {

                if (Session["RoyContRoyRatesSalesType"] != null)
                {
                    dtList = (DataTable)Session["RoyContRoyRatesSalesType"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["price_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in Contract/RoyContractSubRates.aspx screen to search in selected price group
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySearchContSubRatesSalesTypeList(string prefixText, int count)
        {

            try
            {

                if (Session["RoyContSubRatesSalesType"] != null)
                {
                    dtList = (DataTable)Session["RoyContSubRatesSalesType"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("price_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["price_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        #endregion Contract Maintenance screens

        #region Data Maintenance screens

        /// <summary>
        /// Screen Name : Used in DataMaintenance/UserAccountMaint.aspx screen to search users
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyUserMaintUserList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzyUserMaintUserList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyUserMaintUserList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("user_name").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("user_name").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["user_name"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in DataMaintenance/RoyaltorGroupings.aspx screen to search royaltors for the selected group type
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyRoyGrpMaintRoyaltorList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzyRoyGrpMaintRoyaltorList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyRoyGrpMaintRoyaltorList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("grp_name").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("grp_name").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["grp_name"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("No results found");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in DataMaintenance/RoyaltorGroupings.aspx screen to search royaltors from the group Out box
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyRoyGrpMaintGroupOutBoxList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzyRoyGrpMaintGroupOutBoxList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyRoyGrpMaintGroupOutBoxList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor_value").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor_value").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["royaltor_value"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("No results found");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in DataMaintenance/RoyaltorGroupings.aspx screen to search royaltors from the group In box
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyRoyGrpMaintGroupInBoxList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzyRoyGrpMaintGroupInBoxList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyRoyGrpMaintGroupInBoxList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor_value").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor_value").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["royaltor_value"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("No results found");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in DataMaintenance/TerritorySearch.aspx screen to search seller group 
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyTerritorySearchSellerGrpList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzyTerritorySearchSellerGrpList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyTerritorySearchSellerGrpList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["seller_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("No results found");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }


        #endregion Data Maintenance screens

        #region Participant screens

        /// <summary>
        /// Screen Name : Used in Participants/ParticipantMaintenance.aspx screen to search track title
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyPartMaintTrackTitleList(string prefixText, int count)
        {

            try
            {

                if (Session["ParticipMaintTrackTitleList"] != null)
                {
                    dtList = (DataTable)Session["ParticipMaintTrackTitleList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("tune_title").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("tune_title").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["tune_title"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in Participants/ParticipantMaintenance.aspx screen to search seller group
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyPartMaintSellerGrpList(string prefixText, int count)
        {

            try
            {

                if (Session["ParticipMaintSellerGrpList"] != null)
                {
                    dtList = (DataTable)Session["ParticipMaintSellerGrpList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["seller_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in Participants/TrackListing.aspx screen to search seller group
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyTrackListingSellerGrpList(string prefixText, int count)
        {

            try
            {

                if (Session["TrackListingSellerGrpList"] != null)
                {
                    dtList = (DataTable)Session["TrackListingSellerGrpList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("seller_group").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["seller_group"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        #endregion Participant screens

        #region Payment screens

        /// <summary>
        /// Screen Name : Used in Payments/SupplierAddressOverwrite.aspx screen to search supplier number
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzySuppAddOverwriteSupplierList(string prefixText, int count)
        {

            try
            {

                if (Session["SuppAddOverwriteSupplierList"] != null)
                {
                    dtList = (DataTable)Session["SuppAddOverwriteSupplierList"];
                    if (dtList.Rows.Count > 0)
                    {
                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("supplier").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);
                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("supplier").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["supplier"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        #endregion Payment screens

        #region Statement processing screens

        /// <summary>
        /// Screen Name : Used in StatementProcessing/AdHocStatement.aspx screen to search royaltors
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyAdHocStmtRoyaltorList(string prefixText, int count)
        {

            try
            {

                if (Session["AdHocStmtsRoyaltorList"] != null)
                {
                    dtList = (DataTable)Session["AdHocStmtsRoyaltorList"];
                    if (dtList.Rows.Count > 0)
                    {

                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltors").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);

                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltors").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["royaltors"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in StatementProcessing/RoyaltorStatementChanges.aspx screen to search owners
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyStmtChangesOwnerList(string prefixText, int count)
        {

            try
            {

                if (Session["StmtChangesOwnerList"] != null)
                {
                    dtList = (DataTable)Session["StmtChangesOwnerList"];
                    if (dtList.Rows.Count > 0)
                    {

                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("owners").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);

                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("owners").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["owners"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in StatementProcessing/WorkFlow.aspx screen to search royaltors
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyWorkflowRoyaltorList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzyWorkflowRoyaltorList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyWorkflowRoyaltorList"];
                    if (dtList.Rows.Count > 0)
                    {

                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);

                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("royaltor").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["royaltor"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in StatementProcessing/WorkFlow.aspx screen to search owners
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyWorkflowOwnerList(string prefixText, int count)
        {

            try
            {

                if (Session["FuzzyWorkflowOwnerList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyWorkflowOwnerList"];
                    if (dtList.Rows.Count > 0)
                    {

                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("owner").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);

                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("owner").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["owner"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }
        //JIRA-898 Changes -- Ravi --Start
        /// <summary>
        /// Screen Name : Used in StatementProcessing/TransactionRetrieval.aspx, TransactionRetrievalStatus.aspx screens 
        ///               to search royaltor,option period list
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyTransRetRoyOpPrdList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzyTransRetRoyOpPrdList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchListTransRetr("GetTransRetrvRoyOptPrdList", Convert.ToString(Session["UserRoleId"]), out errorId);//JIRA-898 CHange
                    Session["FuzzyTransRetRoyOpPrdList"] = dsList.Tables[0];
                }

                //JIRA-898 Changes -- Ravi --End
                if (Session["FuzzyTransRetRoyOpPrdList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyTransRetRoyOpPrdList"];
                    if (dtList.Rows.Count > 0)
                    {

                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("roy_option").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);

                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("roy_option").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["roy_option"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }

        /// <summary>
        /// Screen Name : Used in StatementProcessing/TransactionRetrieval.aspx, TransactionRetrievalStatus.aspx screens 
        ///               to search artist's associated with catno's 
        /// Retuns first 20 list items matching search criteria
        /// </summary>            
        [System.Web.Services.WebMethod(EnableSession = true)]
        public List<string> FuzzyTransRetArtistList(string prefixText, int count)
        {

            try
            {
                if (Session["FuzzyTransRetArtistList"] == null)
                {
                    dsList = fuzzySearchBL.GetFuzzySearchList("GetTransRetrvCatArtistList", out errorId);
                    Session["FuzzyTransRetArtistList"] = dsList.Tables[0];
                }

                if (Session["FuzzyTransRetArtistList"] != null)
                {
                    dtList = (DataTable)Session["FuzzyTransRetArtistList"];
                    if (dtList.Rows.Count > 0)
                    {

                        var results = dtList.AsEnumerable().Where(dr => dr.Field<string>("artist").ToUpper().Contains(prefixText.ToUpper())).Take(returnListCount);

                        if (count == int.MaxValue)
                        {
                            results = dtList.AsEnumerable().Where(dr => dr.Field<string>("artist").ToUpper().Contains(prefixText.ToUpper()));
                        }

                        if (results.Count() > 0)
                        {
                            foreach (DataRow dr in results)
                            {
                                fuzzyList.Add(Convert.ToString(dr["artist"]));
                            }
                        }
                        else
                        {
                            fuzzyList.Add("No results found");
                        }

                    }
                    else
                    {
                        fuzzyList.Add("No results found");
                    }

                }
                else
                {
                    fuzzyList.Add("Error in fetching data");

                }

            }
            catch (Exception ex)
            {
                fuzzyList.Add("Error in fetching data");
            }

            return fuzzyList;

        }


        #endregion Statement processing screens



    }
}
