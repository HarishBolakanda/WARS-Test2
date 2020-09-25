/*
File Name   :   WARSAffiliates.cs
Purpose     :   WARS Landing page functionality

Version  Date            Modified By              Modification Log
______________________________________________________________________
1.0     03-Dec-2018      Harish                   WUIN-930
      
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using WARS.BusinessLayer;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace WARS
{
   
    public partial class WARSAffiliates : System.Web.UI.Page
    {
        #region Global Declarations
        Utilities util;
        Dictionary<string, string> dicAffiliates = new Dictionary<string, string>();
        string databaseName;
        #endregion Global Declarations

        #region EVENTS
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Master.FindControl("lnkBtnHome").Visible = false;
                this.Master.FindControl("btnWARSGlobal").Visible = false;
                this.Master.FindControl("lblDBname").Visible = false;
                Session["WARSDBConnectionString"] = null;
                Session["DatabaseName"] = null;
                Session["UserRole"] = null;
                DefineAffiliateName();
                trNonPRODAffiliates.Visible = false;

                //hide non PROD related buttons up on configuration setting value defined for "WARSAffiliate"
                if (ConfigurationManager.AppSettings["WARSAffiliate"].ToString().ToLower() != "PROD".ToLower())
                {
                    trNonPRODAffiliates.Visible = true;
                }
                
            }            
            catch (Exception ex)
            {
                ExceptionHandler("Error in loading WARS Affiliate home page", ex.Message);
            }

        }
        
        protected void btnBenelux_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.BNLX.ToString();
                GetConnectionString(AffiliateName.Benelux.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Benelux.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnCentralEurope_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.GSA.ToString();
                GetConnectionString(AffiliateName.CentralEurope.ToString());
                dicAffiliates.TryGetValue(AffiliateName.CentralEurope.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnCzechRepublic_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.CZE.ToString();
                GetConnectionString(AffiliateName.CzechRepublic.ToString());
                dicAffiliates.TryGetValue(AffiliateName.CzechRepublic.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnDenmark_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.DNK.ToString();
                GetConnectionString(AffiliateName.Denmark.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Denmark.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnFinland_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.FIN.ToString();
                GetConnectionString(AffiliateName.Finland.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Finland.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnItaly_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.ITA.ToString();
                GetConnectionString(AffiliateName.Italy.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Italy.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnMiddleEast_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.MiddleEast.ToString();
                GetConnectionString(AffiliateName.MiddleEast.ToString());
                dicAffiliates.TryGetValue(AffiliateName.MiddleEast.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnNorway_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.NOR.ToString();
                GetConnectionString(AffiliateName.Norway.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Norway.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnPoland_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.POL.ToString();
                GetConnectionString(AffiliateName.Poland.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Poland.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnPortugal_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.PRT.ToString();
                GetConnectionString(AffiliateName.Portugal.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Portugal.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnRussia_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.RUS.ToString();
                GetConnectionString(AffiliateName.Russia.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Russia.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnSouthAfrica_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.ZAF.ToString();
                GetConnectionString(AffiliateName.SouthAfrica.ToString());
                dicAffiliates.TryGetValue(AffiliateName.SouthAfrica.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnSpain_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.ESP.ToString();
                GetConnectionString(AffiliateName.Spain.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Spain.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnSweden_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.SWE.ToString();
                GetConnectionString(AffiliateName.Sweden.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Sweden.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnUKADA_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.ADA.ToString();
                GetConnectionString(AffiliateName.UKADA.ToString());
                dicAffiliates.TryGetValue(AffiliateName.UKADA.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in fetching connection details", ex.Message);
            }
        }

        protected void btnUKERATO_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.ERATO.ToString();
                GetConnectionString(AffiliateName.UKERATO.ToString());
                dicAffiliates.TryGetValue(AffiliateName.UKERATO.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnUKGB_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.GB.ToString();
                GetConnectionString(AffiliateName.UKGB.ToString());
                dicAffiliates.TryGetValue(AffiliateName.UKGB.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnUKNVC_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.NVC.ToString();
                GetConnectionString(AffiliateName.UKNVC.ToString());
                dicAffiliates.TryGetValue(AffiliateName.UKNVC.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnUKPLG_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.PLG.ToString();
                GetConnectionString(AffiliateName.UKPLG.ToString());
                dicAffiliates.TryGetValue(AffiliateName.UKPLG.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnUKTELDEC_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.TELDEC.ToString();
                GetConnectionString(AffiliateName.UKTELDEC.ToString());
                dicAffiliates.TryGetValue(AffiliateName.UKTELDEC.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnUKWEA_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.WEA.ToString();
                GetConnectionString(AffiliateName.UKWEA.ToString());
                dicAffiliates.TryGetValue(AffiliateName.UKWEA.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnAustraliaNZ_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.AUS.ToString();
                GetConnectionString(AffiliateName.AustraliaNZ.ToString());
                dicAffiliates.TryGetValue(AffiliateName.AustraliaNZ.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnChina_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.CHN.ToString();
                GetConnectionString(AffiliateName.China.ToString());
                dicAffiliates.TryGetValue(AffiliateName.China.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnHongKong_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.HKG.ToString();
                GetConnectionString(AffiliateName.HongKong.ToString());
                dicAffiliates.TryGetValue(AffiliateName.HongKong.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnIndonesia_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.IND.ToString();
                GetConnectionString(AffiliateName.Indonesia.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Indonesia.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnJapan_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.JAP.ToString();
                GetConnectionString(AffiliateName.Japan.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Japan.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnKorea_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.KOR.ToString();
                GetConnectionString(AffiliateName.Korea.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Korea.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnMalaysia_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.MYS.ToString();
                GetConnectionString(AffiliateName.Malaysia.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Malaysia.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnPhilippines_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.PHL.ToString();
                GetConnectionString(AffiliateName.Philippines.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Philippines.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnSingapore_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.SGP.ToString();
                GetConnectionString(AffiliateName.Singapore.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Singapore.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnTaiwan_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.TWN.ToString();
                GetConnectionString(AffiliateName.Taiwan.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Taiwan.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnThailand_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.THA.ToString();
                GetConnectionString(AffiliateName.Thailand.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Thailand.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnArgentina_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.ARG.ToString();
                GetConnectionString(AffiliateName.Argentina.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Argentina.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnBrazil_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.BRA.ToString();
                GetConnectionString(AffiliateName.Brazil.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Brazil.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnCanada_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.CAN.ToString();
                GetConnectionString(AffiliateName.Canada.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Canada.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnLatina_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.LAT.ToString();
                GetConnectionString(AffiliateName.Latina.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Latina.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnMexico_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.MEX.ToString();
                GetConnectionString(AffiliateName.Mexico.ToString());
                dicAffiliates.TryGetValue(AffiliateName.Mexico.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnDEV_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.DEV.ToString();
                GetConnectionString(AffiliateName.DEV.ToString());
                dicAffiliates.TryGetValue(AffiliateName.DEV.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnUAT_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.UAT.ToString();
                GetConnectionString(AffiliateName.UAT.ToString());
                dicAffiliates.TryGetValue(AffiliateName.UAT.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        protected void btnQA_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SharePointFolderAffiliateCode"] = AffiliateCode.QA.ToString();
                GetConnectionString(AffiliateName.QA.ToString());
                dicAffiliates.TryGetValue(AffiliateName.QA.ToString(), out databaseName);
                OpenMenuScreen();
            }
            catch (Exception ex)
            {
                ExceptionHandler("Error in connecting to WARS affiliate", ex.Message);
            }
        }

        #endregion EVENTS

        #region METHODS
        private void DefineAffiliateName()
        {
            dicAffiliates.Add("Benelux", "Benelux");
            dicAffiliates.Add("CentralEurope", "Central Europe");            
            dicAffiliates.Add("CzechRepublic", "Czech Republic");
            dicAffiliates.Add("Denmark", "Denmark");
            dicAffiliates.Add("Finland", "Finland");
            dicAffiliates.Add("Italy", "Italy");
            dicAffiliates.Add("MiddleEast", "Middle East");
            dicAffiliates.Add("Norway", "Norway");
            dicAffiliates.Add("Poland", "Poland");
            dicAffiliates.Add("Portugal", "Portugal");
            dicAffiliates.Add("Russia", "Russia");
            dicAffiliates.Add("SouthAfrica", "South Africa");
            dicAffiliates.Add("Spain", "Spain");
            dicAffiliates.Add("Sweden", "Sweden");
            dicAffiliates.Add("UKADA", "UK - ADA");
            dicAffiliates.Add("UKERATO", "UK - ERATO");
            dicAffiliates.Add("UKGB", "UK - GB");
            dicAffiliates.Add("UKNVC", "UK - NVC");
            dicAffiliates.Add("UKPLG", "UK - PLG");
            dicAffiliates.Add("UKTELDEC", "UK - TELDEC");
            dicAffiliates.Add("UKWEA", "UK - WEA");

            dicAffiliates.Add("AustraliaNZ", "Australia/NZ");
            dicAffiliates.Add("China", "China");
            dicAffiliates.Add("HongKong", "Hong Kong");
            dicAffiliates.Add("Indonesia", "Indonesia");
            dicAffiliates.Add("Japan", "Japan");
            dicAffiliates.Add("Korea", "Korea");
            dicAffiliates.Add("Malaysia", "Malaysia");
            dicAffiliates.Add("Philippines", "Philippines");
            dicAffiliates.Add("Singapore", "Singapore");
            dicAffiliates.Add("Taiwan", "Taiwan");
            dicAffiliates.Add("Thailand", "Thailand");

            dicAffiliates.Add("Argentina", "Argentina");
            dicAffiliates.Add("Brazil", "Brazil");
            dicAffiliates.Add("Canada", "Canada");
            dicAffiliates.Add("Latina", "Latina");
            dicAffiliates.Add("Mexico", "Mexico");

            dicAffiliates.Add("DEV", "DEV");
            dicAffiliates.Add("UAT", "UAT");
            dicAffiliates.Add("QA", "QA");

        }
        
        private void GetConnectionString(string WARSAffiliate)
        {
            Session["WARSAffiliate"] = WARSAffiliate;//This will be used throughout the application where ever affiliate specification is used
            string connectionStringFile = @AppDomain.CurrentDomain.BaseDirectory + @"\ConnectionString\ConnectionString.txt";
            string sWARSServiceDomain = ConfigurationManager.AppSettings["WARSServiceDomain"];
            string sWARSServiceUser = ConfigurationManager.AppSettings["WARSServiceUser"];
            string sWARSServicePwd = ConfigurationManager.AppSettings["WARSServicePwd"];
            string connectionDetails = string.Empty;

            using (new Impersonator(sWARSServiceUser, sWARSServiceDomain, sWARSServicePwd))
            {
                using (StreamReader sr = new StreamReader(connectionStringFile))
                {                    
                    while ((connectionDetails = sr.ReadLine()) != null)
                    {
                        if (connectionDetails.ToLower().Contains(WARSAffiliate.ToLower()))
                        {
                            if (connectionDetails.Split(':')[1].Trim() != string.Empty)
                            {
                                Session["WARSDBConnectionString"] = connectionDetails.Split(':')[1].Trim();

                                break;
                            }
                        }
                    }
                }

            }

            if (connectionDetails == string.Empty || connectionDetails == null)
            {
                msgView.SetMessage("Selected WARS affiliate has not been set up on this version. Please contact <b>WMI.RoyaltiesSupport@warnermusic.com</b> if required.",
                                            MessageType.Warning, PositionType.Auto);
                return;
            }

            //WUIN-1007 - fetching and holding BO Connection details of the selected affiliate in a session
            string BOConnectionStringFile = @AppDomain.CurrentDomain.BaseDirectory + @"\ConnectionString\BOConnectionString.txt";
            string BOconnectionDetails = string.Empty;

            using (new Impersonator(sWARSServiceUser, sWARSServiceDomain, sWARSServicePwd))
            {
                using (StreamReader sr = new StreamReader(BOConnectionStringFile))
                {
                    while ((BOconnectionDetails = sr.ReadLine()) != null)
                    {
                        if (BOconnectionDetails.ToLower().Contains(WARSAffiliate.ToLower()))
                        {
                            if (BOconnectionDetails.Split(':')[1].Trim() != string.Empty)
                            {
                                Session["WARSBOConnectionString"] = BOconnectionDetails.Split(':')[1].Trim();

                                break;
                            }
                        }
                    }
                }

            }

            if (BOconnectionDetails == string.Empty || BOconnectionDetails == null)
            {
                //empty the DB connection string as well so that application does not get connected
                Session["WARSDBConnectionString"] = null;

                msgView.SetMessage("BO reporting connection has not been defined for the selected WARS affiliate. Please contact <b>WMI.RoyaltiesSupport@warnermusic.com</b> if required.",
                                            MessageType.Warning, PositionType.Auto);
                return;
            }

            

        }

        private void OpenMenuScreen()
        {
            if (Session["WARSDBConnectionString"] != null && Convert.ToString(Session["WARSDBConnectionString"]) != string.Empty)
            {
                Session["DatabaseName"] = databaseName;
                Response.Redirect("../Common/MenuScreen.aspx", false);
            }
        }

        private void ExceptionHandler(string errorMsg, string expMsg)
        {
            util = new Utilities();
            util.GenericExceptionHandler(errorMsg + "<br />" + expMsg);
            util = null;
        }

        #endregion METHODS

        
        

    }
}