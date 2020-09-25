<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuScreen.aspx.cs" Inherits="WARS.MenuScreen" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Menu Options" MaintainScrollPositionOnPostback="true" ClientIDMode="AutoID" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">
        //debugger;
        //probress bar and scroll position functionality - starts
        //to remain scroll position of grid panel and window
        var xPos, yPos;
        var scrollTop;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        //Raised before processing of an asynchronous postback starts and the postback request is sent to the server.
        prm.add_beginRequest(BeginRequestHandler);
        // Raised after an asynchronous postback is finished and control has been returned to the browser.
        prm.add_endRequest(EndRequestHandler);

        function BeginRequestHandler(sender, args) {
            //Shows the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.show();
            }


        }

        function EndRequestHandler(sender, args) {
            //Hide the modal popup - the update progress
            var popup = $find('<%= mPopupPageLevel.ClientID %>');
            if (popup != null) {
                popup.hide();
            }

        }
        //probress bar and scroll position functionality - ends

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        //=============== End
        //JIRA-908 CHanges --Start
        //Trigger button click server side events
        function AdHocFileLoad() {
            var popup = $find('<%= mpeConfirmation.ClientID %>');
            if (popup != null) {
                popup.show();
            }
            document.getElementById("<%= hdnConfirmation.ClientID %>").value = "AdhocFile";
                document.getElementById("<%= lblText.ClientID %>").innerText = "Do you want to load an AdHoc/Cost file?";

        }

        function RunStatements() {
            hdnIsReadyonlyUser = document.getElementById("<%= hdnIsReadyonlyUser.ClientID %>").value;
            if (hdnIsReadyonlyUser == "N") {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                document.getElementById("<%= hdnConfirmation.ClientID %>").value = "RunStmts";
                document.getElementById("<%= lblText.ClientID %>").innerText = "This option will process all Scheduled and Requested statements.  It may take several hours.  Do you want to continue?";
            }
            else {
                DisplayMessagePopup("Sorry! You do not have access to this screen.");
                return false;
            }
        }

        function RunAccruals() {
            hdnIsReadyonlyUser = document.getElementById("<%= hdnIsReadyonlyUser.ClientID %>").value;
            if (hdnIsReadyonlyUser == "N") {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                document.getElementById("<%= hdnConfirmation.ClientID %>").value = "RunAccruals";
                document.getElementById("<%= lblText.ClientID %>").innerText = "Do you want to run Accrual process?";
            }
            else {
                DisplayMessagePopup("Sorry! You do not have access to this screen.");
                return false;
            }
        }
        //JIRA- 1172- Changes by Harshika--Start
        function ReRunAccruals() {
            hdnIsReadyonlyUser = document.getElementById("<%= hdnIsReadyonlyUser.ClientID %>").value;
            if (hdnIsReadyonlyUser == "N") {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                document.getElementById("<%= hdnConfirmation.ClientID %>").value = "ReRunAccruals";
                document.getElementById("<%= lblText.ClientID %>").innerText = "Do you want to rerun Accrual process?";
            }
            else {
                DisplayMessagePopup("Sorry! You do not have access to this screen.");
                return false;
            }
        }
        //JIRA- 1172- Changes by Harshika--End
        function GeneratePayment() {
            hdnIsReadyonlyUser = document.getElementById("<%= hdnIsReadyonlyUser.ClientID %>").value;
            if (hdnIsReadyonlyUser == "N") {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                document.getElementById("<%= hdnConfirmation.ClientID %>").value = "GenPayments";
                document.getElementById("<%= lblText.ClientID %>").innerText = "Do you want to generate payment details?";
            }
            else {
                DisplayMessagePopup("Sorry! You do not have access to this screen.");
                return false;
            }
        }

        //WUIN-715 - runs participant auto consolidation process
        function RunAutoConsolidateClick() {
            hdnIsReadyonlyUser = document.getElementById("<%= hdnIsReadyonlyUser.ClientID %>").value;
            if (hdnIsReadyonlyUser == "N") {
                var popup = $find('<%= mpeConfirmation.ClientID %>');
                if (popup != null) {
                    popup.show();
                }
                document.getElementById("<%= hdnConfirmation.ClientID %>").value = "RunAutoConsolid";
                document.getElementById("<%= lblText.ClientID %>").innerText = "Do you want to run the Auto Consolidate process?";
            }
            else {
                DisplayMessagePopup("Sorry! You do not have access to this screen.");
                return false;
            }
        }
        //JIRA-908 CHanges --End
        //================ End

        //hide and unhide sub menu 
        function tdParticipantsOut() {
            var hovermenu = $find('<%= hmeParticipants.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdParticipants.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdParticipantsOn() {
            var hovermenu = $find('<%= hmeParticipants.ClientID %>');
                if (hovermenu != null) {
                    hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
                }
            }

            function tdPaymentsOut() {
                var hovermenu = $find('<%= hmePayments.ClientID %>');
                if (hovermenu != null) {
                    hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                    document.getElementById('<%= tdPayments.ClientID %>').className = 'MenuScrn_MenuItem';
                    }
                }
                function tdPaymentsOn() {
                    var hovermenu = $find('<%= hmePayments.ClientID %>');
                    if (hovermenu != null) {
                        hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
                    }
                }

                function tdMaintOut() {
                    var hovermenu = $find('<%= hmeMaint.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdMaint.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdMaintOn() {
            var hovermenu = $find('<%= hmeMaint.ClientID %>');
                        if (hovermenu != null) {
                            hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
                        }
                    }

                    function tdDataLoadsOut() {
                        var hovermenu = $find('<%= hmeDataLoads.ClientID %>');
                        if (hovermenu != null) {
                            hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                            document.getElementById('<%= tdDataLoads.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdDataLoadsOn() {
            var hovermenu = $find('<%= hmeDataLoads.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
            }
        }

        function tdFinancialOut() {
            var hovermenu = $find('<%= hmeFinancial.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdFinancial.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdFinancialOn() {
            var hovermenu = $find('<%= hmeFinancial.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
            }
        }

        function tdStmtPrcOut() {
            var hovermenu = $find('<%= hmeStmtPrc.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdStmtPrc.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdStmtPrcOn() {
            var hovermenu = $find('<%= hmeStmtPrc.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
            }
        }

        function tdContractOut() {
            var hovermenu = $find('<%= hmeContract.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdContract.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdContractOn() {
            var hovermenu = $find('<%= hmeContract.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
            }
        }

        function tdAccrualsOut() {
            var hovermenu = $find('<%= hmeAccruals.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdAccruals.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdAccrualsOn() {
            var hovermenu = $find('<%= hmeAccruals.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
            }
        }

        function tdReportsOut() {
            var hovermenu = $find('<%= hmeBOReports.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdReports.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdReportsOn() {
            var hovermenu = $find('<%= hmeBOReports.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
            }
        }

        //JIRA-927 - Changes by Ravi on 22-JAN-2019 -- Start
        function tdNotesOut() {
            var hovermenu = $find('<%= hmeNotes.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "hidden";
                document.getElementById('<%= tdNotes.ClientID %>').className = 'MenuScrn_MenuItem';
            }
        }
        function tdNotesOn() {
            var hovermenu = $find('<%= hmeNotes.ClientID %>');
            if (hovermenu != null) {
                hovermenu._hoverBehavior._hoverElement.style.visibility = "visible";
            }
        }
        //JIRA-927 - Changes by Ravi on 22-JAN-2019 -- End
        //================end

        //To redirect to other pages    

        //to open costs screen 
        function OpenCostScreen() {
            var win = window.open('../DataMaintenance/RoyaltorCosts.aspx', '_self');
            win.focus();
        }

        //to open workflow screen
        function OpenWorkflowScreen() {
            var win = window.open('../StatementProcessing/WorkFlow.aspx', '_self');
            win.focus();
        }

        function WorkflowClick() {
            document.getElementById('<%= btnOpenWorkflow.ClientID %>').click();
        }

        //to open royaltor activity screen
        function OpenActivityScreen() {
            var win = window.open('../StatementProcessing/RoyaltorActivity.aspx', '_self');
            win.focus();
        }

        function ActivityClick() {
            document.getElementById('<%= btnOpenStatementActivity.ClientID %>').click();
        }

        //to open Configuration group screen 
        function OpenConfigGroupScreen() {
            var win = window.open('../DataMaintenance/ConfigurationGrouping.aspx', '_self');
            win.focus();
        }

        function ConfigGroupClick() {
            document.getElementById('<%= btnConfigurationGroupMaint.ClientID %>').click();
        }

        //to open Configuration search screen 
        function OpenConfigSearchScreen() {
            var win = window.open('../DataMaintenance/ConfigurationSearch.aspx', '_self');
            win.focus();
        }

        function ConfigSearchClick() {
            document.getElementById('<%= btnConfigurationSearch.ClientID %>').click();
        }

        //to open territory group screen 
        function OpenTerritoryGroupScreen() {
            var win = window.open('../DataMaintenance/TerritoryGroup.aspx', '_self');
            win.focus();
        }

        function TerritoryGroupClick() {
            document.getElementById('<%= btnTerritoryGroup.ClientID %>').click();
        }

        //to open territory search screen 
        function OpenTerritorySearchScreen() {
            var win = window.open('../DataMaintenance/TerritorySearch.aspx', '_self');
            win.focus();
        }

        function TerritorySearchClick() {
            document.getElementById('<%= btnTerritorySearch.ClientID %>').click();
        }

        //to open transaction maintenance screen 
        function OpenTransactionMaintScreen() {
            var win = window.open('../DataMaintenance/TransactionMaintenance.aspx', '_self');
            win.focus();
        }

        function TransactionMaintClick() {
            document.getElementById('<%= btnTransactionMaint.ClientID %>').click();
        }

        //to open adhoc group screen 
        function OpenAdhocScreen() {
            var win = window.open('../StatementProcessing/AdHocStatement.aspx', '_self');
            win.focus();
        }

        function AdhocClick() {
            document.getElementById('<%= btnAdhoc.ClientID %>').click();
        }

        //to open link artist/royaltor group screen 
        function OpenOptionPeriodLinksScreen() {
            var win = window.open('../DataMaintenance/OptionPeriodLinks.aspx', '_self');
            win.focus();
        }

        function OptionPeriodLinksClick() {
            document.getElementById('<%= btnOptionPeriodLinks.ClientID %>').click();
        }

        //to open company screen 
        function OpenCompanyScreen() {
            var win = window.open('../DataMaintenance/CompanyMaintenance.aspx', '_self');
            win.focus();
        }

        function CompanyClick() {
            document.getElementById('<%= btnCompany.ClientID %>').click();
        }

        //to open owner screen 
        function OpenOwnerScreen() {
            var win = window.open('../DataMaintenance/OwnerMaintenance.aspx', '_self');
            win.focus();
        }

        function OwnerClick() {
            document.getElementById('<%= btnOwner.ClientID %>').click();
        }

        //to open label screen 
        function OpenLabelScreen() {
            var win = window.open('../DataMaintenance/LabelMaintenance.aspx', '_self');
            win.focus();
        }

        function LabelClick() {
            document.getElementById('<%= btnLabel.ClientID %>').click();
        }

        //to open interested Party screen 
        function OpenInterestedPartyScreen() {
            var win = window.open('../DataMaintenance/InterestedPartyMaintenance.aspx', '_self');
            win.focus();
        }

        function InterestedPartyClick() {
            document.getElementById('<%= btnInterestedParty.ClientID %>').click();
        }

        //to open sales type grouping  screen 
        function OpenSalesTypeGroupingScreen() {
            var win = window.open('../DataMaintenance/SalesTypeGroupMaintenance.aspx', '_self');
            win.focus();
        }
        //JIRA-979 Changes by Ravi on 19/02/2019 --Start
        //to open sales type Search  screen 
        function OpenSalesTypeSearchScreen() {
            var win = window.open('../DataMaintenance/SalesTypeSearch.aspx', '_self');
            win.focus();
        }
        //JIRA-979 Changes by Ravi on 19/02/2019 --End

        function SalesTypeGroupingClick() {
            document.getElementById('<%= btnSalesTypeGrouping.ClientID %>').click();
        }

        //JIRA-979 Changes by Ravi on 19/02/2019 --Start
        function SalesTypeSearchClick() {
            document.getElementById('<%= btnSalesTypeSearch.ClientID %>').click();
        }
        //JIRA-979 Changes by Ravi on 19/02/2019 --End

        //to open statement text screen 
        function OpenStatementTextScreen() {
            var win = window.open('../DataMaintenance/StatementTextMaintenance.aspx', '_self');
            win.focus();
        }

        function StatementTextClick() {
            document.getElementById('<%= btnStatementText.ClientID %>').click();
        }

        function TaxRateClick() {
            document.getElementById('<%= btnTaxRate.ClientID %>').click();
        }

        function OpenTaxRateMaintScreen() {
            var win = window.open('../DataMaintenance/TaxRateMaintanance.aspx', '_self');
            win.focus();
        }

        //to open responsibility maint screen 
        function OpenResponsibilityScreen() {
            var win = window.open('../DataMaintenance/ResponsibilityMaintenance.aspx', '_self');
            win.focus();
        }

        function ResponsibilityClick() {
            document.getElementById('<%= btnResponsibility.ClientID %>').click();
        }

        //to open cusomer maint screen 
        function OpenCustomerScreen() {
            var win = window.open('../DataMaintenance/CustomerMaintenance.aspx', '_self');
            win.focus();
        }

        function CustomerClick() {
            document.getElementById('<%= btnCustomer.ClientID %>').click();
        }

        //to open account type maint screen 
        function OpenAccountTypeScreen() {
            var win = window.open('../DataMaintenance/AccountTypeMaintenance.aspx', '_self');
            win.focus();
        }

        function AccountTypeClick() {
            document.getElementById('<%= btnAccountType.ClientID %>').click();
        }

        //to open royaltor cost group screen 
        function OpenRoyaltorCostScreen() {
            var win = window.open('../DataMaintenance/RoyaltorCosts.aspx', '_self');
            win.focus();
        }

        function RoyaltorCostClick() {
            document.getElementById('<%= btnRoyaltorCost.ClientID %>').click();
        }

        //to open royaltor grouping group screen 
        function OpenRoyaltorGroupingScreen() {
            var win = window.open('../DataMaintenance/RoyaltorGroupings.aspx', '_self');
            win.focus();
        }

        function RoyaltorGroupingClick() {
            document.getElementById('<%= btnRoyaltorGrouping.ClientID %>').click();
        }

        //to open royaltor reserves group screen 
        function OpenRoyaltorReservesScreen() {
            var win = window.open('../DataMaintenance/RoyaltorReserves.aspx', '_self');
            win.focus();
        }

        //to open contract escalation history screen
        function ContractEscHistoryClick() {
            document.getElementById('<%= btnContractEscHistory.ClientID %>').click();
        }

        function OpenContractEscHistoryScreen() {
            var win = window.open('../Contract/RoyContractEscHistory.aspx', '_self');
            win.focus();
        }

        //to open user account screen 
        function OpenUserAccountScreen() {
            var win = window.open('../DataMaintenance/UserAccountMaint.aspx', '_self');
            win.focus();
        }

        function UserAccountMaintClick() {
            document.getElementById('<%= btnUserAccount.ClientID %>').click();
        }

        //to open exchange rate screen 
        function OpenExchangeRateScreen() {
            var win = window.open('../StatementProcessing/ExchangeRateFactors.aspx', '_self');
            win.focus();
        }

        function ExchangeRateClick() {
            document.getElementById('<%= btnExchangeRate.ClientID %>').click();
        }

        function PDFStatementsClick() {
            if (document.getElementById("<%=hdnPDFStatementAccess.ClientID %>").value == "Y") {
                document.getElementById('<%= hlPDFStatements.ClientID %>').click();
            }
            else {
                DisplayMessagePopup("There is a problem in accessing this folder. Please contact WMI.RoyaltiesSupport@warnermusic.com");
                return false;
            }

        }

        function FileUploadClick() {
            if (document.getElementById("<%=hdnFileUploadAccess.ClientID %>").value == "Y") {
                     document.getElementById('<%= hlFileUpload.ClientID %>').click();
                 }
                 else {
                     DisplayMessagePopup("There is a problem in accessing this folder. Please contact WMI.RoyaltiesSupport@warnermusic.com");
                     return false;
                 }
             }

             //to open statement process screen 
             function OpenStatementProcessScreen() {
                 var win = window.open('../StatementProcessing/StatementProcess.aspx', '_self');
                 win.focus();
             }

             function StatementProcessClick() {
                 hdnUserRole = document.getElementById("<%= hdnUserRole.ClientID %>").value;
            if (hdnUserRole == "SuperUser") {
                document.getElementById('<%= btnStatementProcess.ClientID %>').click();
            }
            else {
                DisplayMessagePopup("Sorry! Only Super user can access the screen.");
                return false;
            }
        }

        //to open royaltor statement changes screen 
        function OpenRoyaltorStatementChangesScreen() {
            var win = window.open('../StatementProcessing/RoyaltorStatementChanges.aspx', '_self');
            win.focus();
        }

        function OpenTransactionRetrievalScreen() {
            var win = window.open('../StatementProcessing/TransactionRetrieval.aspx', '_self');
            win.focus();
        }

        function RoyaltorStatementChangesClick() {
            hdnUserRole = document.getElementById("<%= hdnUserRole.ClientID %>").value;
                if (hdnUserRole == "SuperUser") {
                    document.getElementById('<%= btnStatementChanges.ClientID %>').click();
                }
                else {
                    DisplayMessagePopup("Sorry! Only Super user can access the screen.");
                    return false;
                }

            }


            function TransactionRetrievalClick() {
                document.getElementById('<%= btnTransactionRetrieval.ClientID %>').click();
            }

            //to open statement progress dashboard screen 
            function OpenStmtProgressDashboardScreen() {
                var win = window.open('../StatementProcessing/StmtProgressDashboard.aspx', '_self');
                win.focus();
            }

            function StmtProgressDashboardClick() {
                document.getElementById('<%= btnStmtProgressDashboard.ClientID %>').click();
            }

            //to open BO reports screen 
            function BOReportsClick(rptFolder) {
                document.getElementById('<%= hdnBOReportsFolder.ClientID %>').value = rptFolder;
            document.getElementById('<%= btnBOReports.ClientID %>').click();
        }

        function OpenBOReportsScreen() {
            var rptFolderName = document.getElementById('<%= hdnBOReportsFolder.ClientID %>').value;
            var win = window.open('../Reporting/BOReports.aspx?folder=' + rptFolderName, '_self');
            win.focus();
        }

        //to open Royaltor search screen 
        function OpenRoySearchScreen() {
            var win = window.open('../Contract/RoyaltorSearch.aspx', '_self');
            win.focus();
        }

        function RoySearchClick() {
            document.getElementById('<%= btnRoySearch.ClientID %>').click();
        }

        //to open Missing Participants search screen 
        function OpenMissingParticipantsScreen() {
            var win = window.open('../Participants/MissingParticipants.aspx', '_self');
            win.focus();
        }

        function MissingParticipantsClick() {
            document.getElementById('<%= btnMissingParticipants.ClientID %>').click();
        }

        //to open Breakdown group screen 
        function OpenBreakdownGroupsScreen() {
            var win = window.open('../DataMaintenance/BreakdownGroupMaintenance.aspx', '_self');
            win.focus();
        }

        function BreakdownGroupsClick() {
            document.getElementById('<%= btnBreakdownGroupSearch.ClientID %>').click();
        }

        //to open Catalogue Search screen 
        function OpenCatalogueSearchScreen() {
            var win = window.open('../Participants/CatalogueSearch.aspx', '_self');
            win.focus();
        }

        function CatalogueSearchClick() {
            document.getElementById('<%= btnCatalogueSearch.ClientID %>').click();
        }


        //WUIN-1044 - Auto Participant Search
        function OpenAutoParticipantSearchScreen() {
            var win = window.open('../Participants/AutoParticipantSearch.aspx', '_self');
            win.focus();
        }

        function AutoParticipantSearchClick() {
            document.getElementById('<%= btnAutoParticipantSearch.ClientID %>').click();
        }


        //to open supplier address overwrite screen 
        function SupplierAddressOverwriteClick() {
            document.getElementById('<%= btnSupplierAddressOverwrite.ClientID %>').click();
        }

        function OpenSupplierAddOverwriteClick() {
            var win = window.open('../Payment/SupplierAddressOverwrite.aspx', '_self');
            win.focus();
        }

        //to open Artist Maintenance screen 
        function OpenArtistMaintenance() {
            var win = window.open('../DataMaintenance/ArtistResponsibilityMaintenance.aspx', '_self');
            win.focus();
        }

        function ArtistMaintenanceClick() {
            document.getElementById('<%= btnArtistMaint.ClientID %>').click();
        }

        //to open Payment Approval screen 
        function OpenPaymentApproval() {
            var win = window.open('../Payment/PaymentApproval.aspx', '_self');
            win.focus();
        }

        function PaymentApprovalClick() {
            document.getElementById('<%=btnPaymentApproval.ClientID %>').click();
        }

        //to open Payment Details screen 
        function OpenPaymentDetails() {
            var win = window.open('../Payment/PaymentDetails.aspx', '_self');
            win.focus();
        }

        function PaymentDetailsClick() {
            document.getElementById('<%=btnPaymentDetails.ClientID %>').click();
        }

        //to open Payment Exchange Rates screen 
        function OpenPaymentExchange() {
            var win = window.open('../Payment/PaymentExchangeRates.aspx', '_self');
            win.focus();
        }

        function PaymentExchangeClick() {
            document.getElementById('<%=btnPaymentExchange.ClientID %>').click();
        }
        //to open Royaltor Balance Screen
        function OpenRoyaltorBalance() {
            var win = window.open('../DataMaintenance/RoyaltorBalance.aspx', '_self');
            win.focus();
        }

        //message for screens to be developed 
        function ToBeDeveloped() {
            alert('To be developed');
        }

        //JIRA-927 - Changes by Ravi on 22-JAN-2019 -- Start
        //to open Notes Overview Screen
        function OpenNotesOverview() {
            var win = window.open('../NotesOverview/NotesOverview.aspx', '_self');
            win.focus();
        }
        //JIRA-927 - Changes by Ravi on 22-JAN-2019 -- End

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    WARS MENU OPTIONS
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table width="50%">
                            <tr>
                                <td width="28%" class="MenuScrn_Header">Menu</td>
                                <td></td>
                                <td>&nbsp
                                                <div>
                                                    <br />
                                                </div>
                                </td>
                                <td width="28%" class="MenuScrn_Header">Shortcuts</td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdContract" runat="server" onmouseout="tdContractOut();" onmouseover="tdContractOn();">Contract 
                                    <ajaxToolkit:HoverMenuExtender ID="hmeContract" runat="server" TargetControlID="tdContract" PopupControlID="pnlContract"
                                        PopupPosition="Right" HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlContract" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="RoySearchClick();">Contract Maintenance                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="ContractEscHistoryClick();">Escalation History                                                  
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="InterestedPartyClick();">Interested Party Maintenance                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="RoyaltorGroupingClick();">Royaltor Grouping                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="OptionPeriodLinksClick();">Artist / Royaltor Links                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="SupplierAddressOverwriteClick();">Payee Address Overwrite                                                    
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                                <td></td>
                                <%--<td valign="top" class="MenuScrn_MenuItem" onclick="location.href='RoyaltorActivity.aspx'">Royaltor Statement Activity                                   
                                </td>--%>
                                <td valign="top" class="MenuScrn_MenuItem" onclick="RoyaltorCostClick();">Costs Maintenance                                  
                                </td>

                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdParticipants" runat="server" onmouseout="tdParticipantsOut();" onmouseover="tdParticipantsOn();">Participants                      
                                    <ajaxToolkit:HoverMenuExtender ID="hmeParticipants" runat="server" TargetControlID="tdParticipants" PopupControlID="pnlParticipants" PopupPosition="Right"
                                        HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlParticipants" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="MissingParticipantsClick();">Missing Participants                                              
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="CatalogueSearchClick();">Catalogue 
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="RunAutoConsolidateClick();">Run Auto Consolidate 
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="AutoParticipantSearchClick();">Auto Participants 
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                                <td></td>
                                <td valign="top" class="MenuScrn_MenuItem" onclick="AdHocFileLoad();">Ad Hoc/Cost File Load                                       
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdMaint" runat="server" onmouseout="tdMaintOut();" onmouseover="tdMaintOn();">Data Maintenance                      
                                    <ajaxToolkit:HoverMenuExtender ID="hmeMaint" runat="server" TargetControlID="tdMaint" PopupControlID="pnlMaint" PopupPosition="Right"
                                        HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlMaint" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="ArtistMaintenanceClick();">Artist
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="BreakdownGroupsClick();">Breakdown Groups
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="CompanyClick();">Company
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="ConfigGroupClick();">Configuration Group
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="ConfigSearchClick();">Configuration Search
                                                </td>
                                            </tr>

                                            <tr>
                                                <td onclick="AccountTypeClick();">Cost Account Type                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="CustomerClick();">DSP                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="LabelClick();">Label
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="OwnerClick();">Owner
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="ToBeDeveloped();">Priority                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="ResponsibilityClick();">Responsibility                                                    
                                                </td>
                                            </tr>

                                            <tr>
                                                <td onclick="SalesTypeGroupingClick();">Sales Type Group                                                     
                                                </td>
                                            </tr>
                                            <%--JIRA-979 Changes by Ravi on 19/02/2019 -- Start--%>
                                            <tr>
                                                <td onclick="SalesTypeSearchClick();">Sales Type Search                                                   
                                                </td>
                                            </tr>
                                            <%--JIRA-979 Changes by Ravi on 19/02/2019 -- End--%>
                                            <tr>
                                                <td onclick="StatementTextClick();">Statement Text
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="TaxRateClick();">Tax Rates 
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="TerritoryGroupClick();">Territory Group                                              
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="TerritorySearchClick();">Territory Search                                              
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="UserAccountMaintClick();">User Accounts                                                    
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                                <td></td>
                                <%--<td valign="top" class="MenuScrn_MenuItem">Participants                                    
                                </td>--%>
                                <td id="Td2" valign="top" class="MenuScrn_MenuItem" runat="server" onclick="WorkflowClick();">Workflow                                   
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdDataLoads" runat="server" onmouseout="tdDataLoadsOut();" onmouseover="tdDataLoadsOn();">Transactions
                                    <ajaxToolkit:HoverMenuExtender ID="hmeDataLoads" runat="server" TargetControlID="tdDataLoads" PopupControlID="pnlDataLoads"
                                        PopupPosition="Right" HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlDataLoads" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="AdHocFileLoad();">Ad Hoc/Cost File Load                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="ExchangeRateClick();">Exchange Rates                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="TransactionRetrievalClick();">Transaction Retrieval
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="TransactionMaintClick();">Transaction Maintenance                                              
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                                <td></td>
                                <%--<td valign="top" class="MenuScrn_MenuItem" onclick="location.href='Workflow.aspx'">Workflow                                   
                                </td>--%>
                                <td valign="top" class="MenuScrn_MenuItem" onclick="ActivityClick();">Statement Activity                                   
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdFinancial" runat="server" onmouseout="tdFinancialOut();" onmouseover="tdFinancialOn();">Financial
                                    <ajaxToolkit:HoverMenuExtender ID="hmeFinancial" runat="server" TargetControlID="tdFinancial" PopupControlID="pnlFinancial"
                                        PopupPosition="Right" HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlFinancial" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="OpenCostScreen();">Cost Maintenance                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="OpenRoyaltorReservesScreen();">Balances and Reserves                                                  
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="OpenRoyaltorBalance();">Royaltor Earnings                                                 
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                                <td></td>
                                <td valign="top" class="MenuScrn_MenuItem" onclick="ExchangeRateClick();">Exchange Rates</td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <%--<td class="MenuScrn_MenuItem" id="tdAccruals" runat="server">Accruals                                    
                                </td>--%>
                                <td class="MenuScrn_MenuItem" id="tdStmtPrc" runat="server" onmouseout="tdStmtPrcOut();" onmouseover="tdStmtPrcOn();">Statement Processing 
                                    <ajaxToolkit:HoverMenuExtender ID="hmeStmtPrc" runat="server" TargetControlID="tdStmtPrc" PopupControlID="pnlStmtPrc"
                                        PopupPosition="Right" HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlStmtPrc" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">

                                            <tr>
                                                <td onclick="StatementProcessClick();">Process Statement                                                    
                                                </td>
                                            </tr>
                                            <tr id="trAdhocStmt" runat="server">
                                                <td onclick="AdhocClick();">Ad Hoc Statements                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="RunStatements();">Run Statements                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="RoyaltorStatementChangesClick();">Statement Changes                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="ActivityClick();">Statement Activity                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="WorkflowClick();">Workflow                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="StmtProgressDashboardClick();">Statement Dashboard                                                    
                                                </td>
                                            </tr>

                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                                <td></td>
                                <td valign="top" class="MenuScrn_MenuItem" onclick="PDFStatementsClick();">PDF Statements</td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdAccruals" runat="server" onmouseout="tdAccrualsOut();" onmouseover="tdAccrualsOn();">Accruals 
                                    <ajaxToolkit:HoverMenuExtender ID="hmeAccruals" runat="server" TargetControlID="tdAccruals" PopupControlID="pnlAccruals"
                                        PopupPosition="Right" HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlAccruals" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="RunAccruals();">Run Accruals</td>
                                            </tr>
                                            <tr>
                                                <td onclick="ReRunAccruals();">Rerun Accruals</td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                                <td></td>
                                <td valign="top" class="MenuScrn_MenuItem" onclick="FileUploadClick();">File Upload Location</td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdReports" runat="server" onmouseout="tdReportsOut();" onmouseover="tdReportsOn();">Reports 
                                    <ajaxToolkit:HoverMenuExtender ID="hmeBOReports" runat="server" TargetControlID="tdReports" PopupControlID="pnlReports"
                                        PopupPosition="Right" HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlReports" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="BOReportsClick('Adhoc');">Ad-Hoc Reports                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="BOReportsClick('Archive');">Archive Reports                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="BOReportsClick('Audit');">Audit Reports                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="BOReportsClick('Checking');">Statement Checking Reports                                                   
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdPayments" runat="server" onmouseout="tdPaymentsOut();" onmouseover="tdPaymentsOn();">Payments                      
                                    <ajaxToolkit:HoverMenuExtender ID="hmePayments" runat="server" TargetControlID="tdPayments" PopupControlID="pnlPayments" PopupPosition="Right"
                                        HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlPayments" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="GeneratePayment();">Generate Payment Details                                              
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="PaymentApprovalClick();">Payment Approval
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="PaymentDetailsClick();">Payment Details 
                                                </td>
                                            </tr>
                                            <tr>
                                                <td onclick="PaymentExchangeClick();">Payment Exchange Rates 
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td></td>
                            </tr>
                            <%--JIRA-927 - Changes by Ravi on 22-JAN-2019 -- Start--%>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="MenuScrn_MenuItem" id="tdNotes" runat="server" onmouseout="tdNotesOut();" onmouseover="tdNotesOn();">Notes                     
                                    <ajaxToolkit:HoverMenuExtender ID="hmeNotes" runat="server" TargetControlID="tdNotes" PopupControlID="pnlNotes" PopupPosition="Right"
                                        HoverCssClass="MenuScrn_MenuItem_Hover">
                                    </ajaxToolkit:HoverMenuExtender>
                                    <asp:Panel ID="pnlNotes" runat="server" CssClass="MenuScrn_SubMenu_Panel">
                                        <table class="MenuScrn_SubMenu_table">
                                            <tr>
                                                <td onclick="OpenNotesOverview();">Notes Overview                                           
                                                </td>
                                            </tr>

                                        </table>
                                    </asp:Panel>

                                </td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <%--JIRA-927 - Changes by Ravi on 22-JAN-2019 -- End--%>
                                <tr>
                                    <td colspan="4"></td>
                                </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="btnOpenWorkflow" runat="server" Style="display: none;"
                            Text="Workflow" OnClientClick="OpenWorkflowScreen();"
                            UseSubmitBehavior="false" CausesValidation="false" />
                        <asp:Button ID="btnOpenStatementActivity" runat="server" Style="display: none;"
                            OnClientClick="OpenActivityScreen();" Text="Statement Activity" UseSubmitBehavior="false"
                            CausesValidation="false" />
                        <asp:Button ID="btnConfigurationGroupMaint" runat="server" Text="Config Group Maintenance"
                            Style="display: none;" OnClientClick="OpenConfigGroupScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnConfigurationSearch" runat="server" Text="Config Search"
                            Style="display: none;" OnClientClick="OpenConfigSearchScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnTerritoryGroup" runat="server" Style="display: none;" Text="Territory Group Maintenance"
                            OnClientClick="OpenTerritoryGroupScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnTerritorySearch" runat="server" Style="display: none;" Text="Territory Search Maintenance"
                            OnClientClick="OpenTerritorySearchScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnTransactionMaint" runat="server" Style="display: none;" Text="Domestic Transaction Maintenance"
                            OnClientClick="OpenTransactionMaintScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnAdhoc" runat="server" Style="display: none;" Text="Adhoc Screen"
                            OnClientClick="OpenAdhocScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnOptionPeriodLinks" runat="server" Style="display: none;" Text="Link Artist/Royaltor"
                            OnClientClick="OpenOptionPeriodLinksScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnCompany" runat="server" Style="display: none;" Text="Company"
                            OnClientClick="OpenCompanyScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnOwner" runat="server" Style="display: none;" Text="Owner"
                            OnClientClick="OpenOwnerScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnStatementText" runat="server" Style="display: none;" Text="Statement Text Maintenance"
                            OnClientClick="OpenStatementTextScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnTaxRate" runat="server" Style="display: none;" Text="Tax Rate Maintenance"
                            OnClientClick="OpenTaxRateMaintScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnLabel" runat="server" Style="display: none;" Text="Label"
                            OnClientClick="OpenLabelScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnInterestedParty" runat="server" Style="display: none;" Text="Interested Party"
                            OnClientClick="OpenInterestedPartyScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnSalesTypeGrouping" runat="server" Style="display: none;" Text="Sales type Group"
                            OnClientClick="OpenSalesTypeGroupingScreen();" UseSubmitBehavior="false" />
                        <%--JIRA-979 Changes by Ravi on 19/02/2019 -- STart--%>
                        <asp:Button ID="btnSalesTypeSearch" runat="server" Style="display: none;" Text="Sales type Search"
                            OnClientClick="OpenSalesTypeSearchScreen();" UseSubmitBehavior="false" />
                        <%--JIRA-979 Changes by Ravi on 19/02/2019 -- End--%>
                        <asp:Button ID="btnResponsibility" runat="server" Style="display: none;" Text="Responsibility"
                            OnClientClick="OpenResponsibilityScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnCustomer" runat="server" Style="display: none;" Text="Customer"
                            OnClientClick="OpenCustomerScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnAccountType" runat="server" Style="display: none;" Text="Account Type"
                            OnClientClick="OpenAccountTypeScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnRoyaltorCost" runat="server" Style="display: none;" Text="Royaltor Cost"
                            OnClientClick="OpenRoyaltorCostScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnRoyaltorGrouping" runat="server" Style="display: none;" Text="Royaltor Grouping"
                            OnClientClick="OpenRoyaltorGroupingScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnUserAccount" runat="server" Style="display: none;" Text="User Account"
                            OnClientClick="OpenUserAccountScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnExchangeRate" runat="server" Style="display: none;" Text="Exchange Rate"
                            OnClientClick="OpenExchangeRateScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnStatementProcess" runat="server" Style="display: none;" Text="Statement Process"
                            OnClientClick="OpenStatementProcessScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnStatementChanges" runat="server" Style="display: none;" Text="Statement Changes"
                            OnClientClick="OpenRoyaltorStatementChangesScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnTransactionRetrieval" runat="server" Style="display: none;" Text="Transaction Retrieval"
                            OnClientClick="OpenTransactionRetrievalScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnStmtProgressDashboard" runat="server" Style="display: none;" Text="Stmt Progress Dashboard"
                            OnClientClick="OpenStmtProgressDashboardScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnBOReports" runat="server" Style="display: none;"
                            OnClientClick="OpenBOReportsScreen();" Text="BO Reports" UseSubmitBehavior="false"
                            CausesValidation="false" />
                        <asp:Button ID="btnRoySearch" runat="server" Style="display: none;" Text="Royaltor Search"
                            OnClientClick="OpenRoySearchScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnMissingParticipants" runat="server" Style="display: none;" Text="Royaltor Search"
                            OnClientClick="OpenMissingParticipantsScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnCatalogueSearch" runat="server" Style="display: none;" Text="Royaltor Search"
                            OnClientClick="OpenCatalogueSearchScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnBreakdownGroupSearch" runat="server" Style="display: none;" Text="Royaltor Search"
                            OnClientClick="OpenBreakdownGroupsScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnSupplierAddressOverwrite" runat="server" Style="display: none;" Text="SupplierAddressOverwrite"
                            OnClientClick="OpenSupplierAddOverwriteClick();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnContractEscHistory" runat="server" Style="display: none;" Text="ContractEscHistory"
                            OnClientClick="OpenContractEscHistoryScreen();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnArtistMaint" runat="server" Style="display: none;" Text="Artist Maintenence"
                            OnClientClick="OpenArtistMaintenance();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnPaymentApproval" runat="server" Style="display: none;" Text="Payment Approval"
                            OnClientClick="OpenPaymentApproval();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnPaymentDetails" runat="server" Style="display: none;" Text="Payment Details"
                            OnClientClick="OpenPaymentDetails();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnPaymentExchange" runat="server" Style="display: none;" Text="Payment Exchange Rates"
                            OnClientClick="OpenPaymentExchange();" UseSubmitBehavior="false" />
                        <asp:Button ID="btnAutoParticipantSearch" runat="server" Style="display: none;" Text="Auto Participant Search"
                            OnClientClick="OpenAutoParticipantSearchScreen();" UseSubmitBehavior="false" />
                        <asp:HyperLink ID="hlPDFStatements" runat="server" Style="display: none;" Text="PDF Statements" NavigateUrl="#"></asp:HyperLink>
                        <asp:HyperLink ID="hlFileUpload" runat="server" Style="display: none;" Text="File Upload" NavigateUrl="#"></asp:HyperLink>
                        <%--JIRA-927 - Changes by Ravi on 22-JAN-2019 -- Start--%>
                        <asp:Button ID="btnNotesOverview" runat="server" Style="display: none;" Text="Notes Overview Screen"
                            OnClientClick="OpenNotesOverview();" UseSubmitBehavior="false" />
                        <%--JIRA-927 - Changes by Ravi on 22-JAN-2019 -- End--%>
                    </td>
                </tr>
            </table>

            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- Start--%>
            <asp:Button ID="dummyConfirmation" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" PopupControlID="pnlConfirmation" TargetControlID="dummyConfirmation"
                CancelControlID="btnNo" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmation" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmation" runat="server" Text="Confirmation Box" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblText" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="ButtonStyle" OnClick="btnYes_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNo" runat="server" Text="No" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--JIRA-908 Changes by Ravi on 12/02/2019 -- End--%>

            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <asp:Button ID="btnAdHocFileLoad" runat="server" OnClick="btnAdHocFileLoad_Click" Style="display: none" />
            <%--JIRA-938 Changes done by Ravi on 28/01/2019 -- Start--%>
            <asp:Button ID="btnRunStatements" runat="server" OnClick="btnRunStatements_Click" Style="display: none" />
            <%--JIRA-938 Changes done by Ravi on 28/01/2019 -- End--%>
            <asp:Button ID="btnAccrual" runat="server" OnClick="btnAccrual_Click" Style="display: none" />
            <asp:Button ID="btnGeneratePayment" runat="server" OnClick="btnGeneratePayment_Click" Style="display: none" />
            <asp:Button ID="btnRunAutoConsolidate" runat="server" OnClick="btnRunAutoConsolidate_Click" Style="display: none" />
            <msg:MsgControl ID="msgView" runat="server" />
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField"></asp:TextBox>
            <asp:TextBox ID="TextBox1" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="OnTabPress();"></asp:TextBox>
            <asp:HiddenField ID="hdnBOReportsFolder" runat="server" Value="" />
            <asp:HiddenField ID="hdnPDFStatementAccess" runat="server" Value="N" />
            <asp:HiddenField ID="hdnFileUploadAccess" runat="server" Value="N" />
            <asp:HiddenField ID="hdnConfirmation" runat="server" />
            <asp:HiddenField ID="hdnIsReadyonlyUser" runat="server" Value="N" />
            <asp:HiddenField ID="hdnUserRole" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
