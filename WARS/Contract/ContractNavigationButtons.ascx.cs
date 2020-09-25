using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace WARS.Contract
{
    public partial class ContractNavigationButtons : System.Web.UI.UserControl
    {
        DataTable dtScreenStatus;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Disable()
        {
            btnSupplierDetails.Enabled = false;
            btnContractGrps.Enabled = false;
            btnEscalationRates.Enabled = false;
            btnNotes.Enabled = false;
            btnOptionPeriods.Enabled = false;
            btnPackagingRates.Enabled = false;
            btnPayee.Enabled = false;
            btnRoyContract.Enabled = false;
            btnRoyRates.Enabled = false;
            btnSubsidRates.Enabled = false;
            btnTaxDetails.Enabled = false; //JIRA-1146 CHanges
            btnNotes.Enabled = false;
            btnBalResvHistory.Enabled = false;
            btnEscHistory.Enabled = false;
        }

        /* WUIN-450 change
        /// <summary>
        /// WUIN-359 change: as entering payee supplier is not mandatory for new royaltor, do not disable next tab(option periods) in payee supplier screen
        /// </summary>
        public void DisableForPayeeSupp()
        {
            btnPayeeSuppDetails.Enabled = false;
            btnContractGrps.Enabled = false;
            btnEscalationRates.Enabled = false;
            btnNotes.Enabled = false;
            //btnOptionPeriods.Enabled = false;            
            btnPackagingRates.Enabled = false;
            btnPayee.Enabled = false;
            btnRoyContract.Enabled = false;
            btnRoyRates.Enabled = false;
            btnSubsidRates.Enabled = false;

            hdnIsNewRoyaltor.Value = "Y";
        }
        */

        /// <summary>
        /// WUIN-450 - On new Contract, enable side bar buttons that have already been completed
        /// </summary>
        public void EnableNewRoyNavButtons(string Screen)
        {
            if (Session[hdnRoyaltorId.Value + "-" + "NewRoySaved"] == null)
            {
                //Create datatable to hold saved status of contract screens
                dtScreenStatus = new DataTable();
                dtScreenStatus.Columns.Add("Screen", typeof(string));
                dtScreenStatus.Columns.Add("Enable", typeof(string));

                dtScreenStatus.Rows.Add(ContractScreens.ContractDetails.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.PayeeDetails.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.PayeeSupplier.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.OptionPeriod.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.RoyaltyRates.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.SubsidiaryRates.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.PackagingRates.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.EscalationRates.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.ContractGroupings.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.TaxDetails.ToString(), "N");//JIRA-1146 CHanges
                dtScreenStatus.Rows.Add(ContractScreens.Notes.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.BalResvHistory.ToString(), "N");
                dtScreenStatus.Rows.Add(ContractScreens.EscHistory.ToString(), "N");

                Session[hdnRoyaltorId.Value + "-" + "NewRoySaved"] = dtScreenStatus;

            }
            else
            {
                dtScreenStatus = (DataTable)Session[hdnRoyaltorId.Value + "-" + "NewRoySaved"];
            }


            btnRoySearch.Enabled = true;
            btnRoyContract.Enabled = IsNewRoySaved(ContractScreens.ContractDetails.ToString());
            btnPayee.Enabled = IsNewRoySaved(ContractScreens.PayeeDetails.ToString());
            btnSupplierDetails.Enabled = IsNewRoySaved(ContractScreens.PayeeSupplier.ToString());
            btnOptionPeriods.Enabled = IsNewRoySaved(ContractScreens.OptionPeriod.ToString());
            btnRoyRates.Enabled = IsNewRoySaved(ContractScreens.RoyaltyRates.ToString());
            btnSubsidRates.Enabled = IsNewRoySaved(ContractScreens.SubsidiaryRates.ToString());
            btnPackagingRates.Enabled = IsNewRoySaved(ContractScreens.PackagingRates.ToString());
            btnEscalationRates.Enabled = IsNewRoySaved(ContractScreens.EscalationRates.ToString());
            btnContractGrps.Enabled = IsNewRoySaved(ContractScreens.ContractGroupings.ToString());
            btnTaxDetails.Enabled = IsNewRoySaved(ContractScreens.TaxDetails.ToString());//JIRA-1146 CHanges
            btnNotes.Enabled = IsNewRoySaved(ContractScreens.Notes.ToString());
            btnBalResvHistory.Enabled = IsNewRoySaved(ContractScreens.BalResvHistory.ToString());
            btnEscHistory.Enabled = IsNewRoySaved(ContractScreens.EscHistory.ToString());


            hdnIsNewRoyaltor.Value = "Y";

            if (Screen == ContractScreens.ContractDetails.ToString())
            {
                //as per initial requirement,
                //disable royaltor search navigation button for contract screen
                btnRoySearch.Enabled = false;
                btnRoyContract.Enabled = true;

                if (IsNewRoySaved(ContractScreens.PayeeDetails.ToString()))
                {
                    btnRoySearch.Enabled = true;
                }


            }
            else if (Screen == ContractScreens.PayeeDetails.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;

                SetNewRoyButtonStatus(ContractScreens.PayeeDetails.ToString());

            }
            else if (Screen == ContractScreens.PayeeSupplier.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;

                //WUIN-359 change: as entering payee supplier is not mandatory for new royaltor, do not disable next tab(option periods) in payee supplier screen
                btnOptionPeriods.Enabled = true;
                //hdnIsNewRoyaltor.Value = "Y";

                //as it can be opted to next screen without any changes, set enable status of payee supplier = Y
                SetNewRoyButtonStatus(ContractScreens.PayeeSupplier.ToString());
                SetNewRoyButtonStatus(ContractScreens.OptionPeriod.ToString());
            }
            else if (Screen == ContractScreens.OptionPeriod.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;


                SetNewRoyButtonStatus(ContractScreens.OptionPeriod.ToString());
            }
            else if (Screen == ContractScreens.RoyaltyRates.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;

                SetNewRoyButtonStatus(ContractScreens.RoyaltyRates.ToString());
            }
            else if (Screen == ContractScreens.SubsidiaryRates.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;
                btnSubsidRates.Enabled = true;

                SetNewRoyButtonStatus(ContractScreens.SubsidiaryRates.ToString());
            }
            else if (Screen == ContractScreens.PackagingRates.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;
                btnSubsidRates.Enabled = true;
                btnPackagingRates.Enabled = true;

                SetNewRoyButtonStatus(ContractScreens.PackagingRates.ToString());
            }
            else if (Screen == ContractScreens.EscalationRates.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;
                btnSubsidRates.Enabled = true;
                btnPackagingRates.Enabled = true;
                btnEscalationRates.Enabled = true;

                SetNewRoyButtonStatus(ContractScreens.EscalationRates.ToString());
            }
            else if (Screen == ContractScreens.ContractGroupings.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;
                btnSubsidRates.Enabled = true;
                btnPackagingRates.Enabled = true;
                btnEscalationRates.Enabled = true;
                btnContractGrps.Enabled = true;

                SetNewRoyButtonStatus(ContractScreens.ContractGroupings.ToString());
            }
            else if (Screen == ContractScreens.TaxDetails.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;
                btnSubsidRates.Enabled = true;
                btnPackagingRates.Enabled = true;
                btnEscalationRates.Enabled = true;
                btnContractGrps.Enabled = true;
                btnTaxDetails.Enabled = true; //JIRA-1146 CHanges

                SetNewRoyButtonStatus(ContractScreens.ContractGroupings.ToString());
            }
            else if (Screen == ContractScreens.Notes.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;
                btnSubsidRates.Enabled = true;
                btnPackagingRates.Enabled = true;
                btnEscalationRates.Enabled = true;
                btnContractGrps.Enabled = true;
                btnTaxDetails.Enabled = true; //JIRA-1146 CHanges
                btnNotes.Enabled = true;

                SetNewRoyButtonStatus(ContractScreens.Notes.ToString());
            }
            else if (Screen == ContractScreens.BalResvHistory.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;
                btnSubsidRates.Enabled = true;
                btnPackagingRates.Enabled = true;
                btnEscalationRates.Enabled = true;
                btnContractGrps.Enabled = true;
                btnTaxDetails.Enabled = true; //JIRA-1146 CHanges
                btnNotes.Enabled = true;
                btnBalResvHistory.Enabled = true;                

                SetNewRoyButtonStatus(ContractScreens.BalResvHistory.ToString());
            }
            else if (Screen == ContractScreens.EscHistory.ToString())
            {
                btnRoySearch.Enabled = true;
                btnRoyContract.Enabled = true;
                btnPayee.Enabled = true;
                btnSupplierDetails.Enabled = true;
                btnOptionPeriods.Enabled = true;
                btnRoyRates.Enabled = true;
                btnSubsidRates.Enabled = true;
                btnPackagingRates.Enabled = true;
                btnEscalationRates.Enabled = true;
                btnContractGrps.Enabled = true;
                btnTaxDetails.Enabled = true; //JIRA-1146 CHanges
                btnNotes.Enabled = true;
                btnBalResvHistory.Enabled = true;
                btnEscHistory.Enabled = true;

                SetNewRoyButtonStatus(ContractScreens.EscHistory.ToString());
            }
        }

        public void SetNewRoyButtonStatus(string Screen)
        {
            dtScreenStatus = (DataTable)Session[hdnRoyaltorId.Value + "-" + "NewRoySaved"];
            foreach (DataRow dr in dtScreenStatus.Rows)
            {
                if (dr["Screen"].ToString() == Screen)
                {
                    dr["Enable"] = "Y";
                }
            }
        }

        private bool IsNewRoySaved(string Screen)
        {
            bool saved = false;

            DataRow[] dtScreen = dtScreenStatus.Select("Screen = '" + Screen + "'");

            if (dtScreen[0][1].ToString() == "Y")
            {
                saved = true;

            }

            return saved;
        }

        /// <summary>
        /// WUIN-517 - Disable Contract edit if Locked
        /// Identifies if royaltor is locked or not from session value set in contract maintenance screen for the royaltor
        /// </summary>
        public bool IsRoyaltorLocked(string royaltorId)
        {
            bool isLocked = false;

            if (Session[royaltorId + "-" + "RoyaltorLocked"] != null &&
                Convert.ToString(Session[royaltorId + "-" + "RoyaltorLocked"]) == "Y")
            {
                isLocked = true;
            }

            return isLocked;
        }

        public bool IsScreenLocked(string royaltorId)
        {
            bool isScreenLocked = false;

            string screenLockFlag = Convert.ToString(Session["ContractScreenLocked"]);
            string screenLockedUser = Convert.ToString(Session["ContractScreenLockedUser"]);

            if (screenLockFlag == "Y" && Session["UserCode"].ToString() != screenLockedUser)
            {
                isScreenLocked = true;
            }
            return isScreenLocked;

        }


    }
}