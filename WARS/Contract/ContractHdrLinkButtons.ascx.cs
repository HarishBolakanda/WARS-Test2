using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WARS.Contract
{
    public partial class ContractHdrLinkButtons : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //make contract navigation and Balances and Reserves button available only for escalation history screen
            trContMaint.Visible = false;
            trBalResHis.Visible = false;
            string escHistoryScreenTitle = "Royaltor Contract - Escalation History";
            string royaltorResScreenTittle = "Royaltor Reserves";
            if (this.Page.Title.ToUpper().Contains(escHistoryScreenTitle.ToUpper()) && hdnBtnContractMaint.Value == "Y")
            {
                trContMaint.Visible = true;
                trBalResHis.Visible = true;
                trTerritoryGrp.Visible = false;
                trConfigGrp.Visible = false;
            }

            if (this.Page.Title.ToUpper().Contains(royaltorResScreenTittle.ToUpper()) && hdnBtnTerritoryGrp.Value == "Y" && hdnBtnConfigGrp.Value == "Y")
            {
                trTerritoryGrp.Visible = true;
                trConfigGrp.Visible = true;
            }

            else if (this.Page.Title.ToLower().Contains("audit") || this.Page.Title.ToLower().Contains("reserves"))
            {             
                trTerritoryGrp.Visible = false;
                trConfigGrp.Visible = false;
            }           
         
        }       
    }
}