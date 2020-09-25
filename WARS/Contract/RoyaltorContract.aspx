<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoyaltorContract.aspx.cs" Inherits="WARS.RoyaltorContract" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Royaltor Contract" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>
<%@ Register TagPrefix="ContNav" TagName="ContractNavigation" Src="~/Contract/ContractNavigationButtons.ascx" %>
<%@ Register TagPrefix="ContHdrNav" TagName="ContractHdrNavigation" Src="~/Contract/ContractHdrLinkButtons.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <ContHdrNav:ContractHdrNavigation ID="contractHdrNavigation" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">
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
        //======================= End


        //getting window height
        function SetGrdPnlHeightOnLoad() {
            var windowHeight = window.screen.availHeight;
            document.getElementById("<%=hdnWindowHeight.ClientID %>").innerText = windowHeight;

        }

        //================================End

        //Owner auto populate search functionalities        
        var txtOwnSrch;

        function ownerListPopulating() {
            txtOwnSrch = document.getElementById("<%= txtOwnerSearch.ClientID %>");
            txtOwnSrch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtOwnSrch.style.backgroundRepeat = 'no-repeat';
            txtOwnSrch.style.backgroundPosition = 'right';
            document.getElementById("<%= hdnIsValidOwner.ClientID %>").value = "N";
        }

        function ownerListPopulated() {
            txtOwnSrch = document.getElementById("<%= txtOwnerSearch.ClientID %>");
            txtOwnSrch.style.backgroundImage = 'none';
        }

        function ownerScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= acePnlOwnerFuzzySearch.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function ownerListItemSelected(sender, args) {
            var ownSrchVal = args.get_value();
            if (ownSrchVal == 'No results found') {
                document.getElementById("<%= txtOwnerSearch.ClientID %>").value = "";
            }
            else {
                document.getElementById("<%= hdnIsValidOwner.ClientID %>").value = "Y";
            }

        }
        //================================End

        //Royaltor auto populate search functionalities        
        var txtRoyaltorSearch;

        function RoyaltorListPopulating() {
            txtRoyaltorSearch = document.getElementById("<%= txtRoyaltorSearch.ClientID %>");
            txtRoyaltorSearch.style.backgroundImage = 'url(Images/textbox_loader.gif)';
            txtRoyaltorSearch.style.backgroundRepeat = 'no-repeat';
            txtRoyaltorSearch.style.backgroundPosition = 'right';
        }

        function RoyaltorListPopulated() {
            txtRoyaltorSearch = document.getElementById("<%= txtRoyaltorSearch.ClientID %>");
            txtRoyaltorSearch.style.backgroundImage = 'none';
        }

        function RoyaltorScrollPosition(sender, args) {
            var autoCompPnl = document.getElementById("<%= acePnlRoyaltorFuzzySearch.ClientID %>");
            autoCompPnl.scrollTop = 1;

        }

        function RoyaltorListItemSelected(sender, args) {
            var ownSrchVal = args.get_value();
            if (ownSrchVal == 'No results found') {
                document.getElementById("<%= txtRoyaltorSearch.ClientID %>").value = "";
                document.getElementById("<%= hdnRoyaltorType.ClientID %>").value = "A";
            }
            else {
                document.getElementById("<%= hdnRoyaltorType.ClientID %>").value = "P";
            }


        }
        //================================End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
            }
        }

        function FocusLblKeyPress() {
            document.getElementById("<%= txtRoyaltorId.ClientID %>").focus();
        }

        //=============== End

        //Validation: warning message if changes made and not saved or on page change                                
        //set flag value when data is changed
        function CompareDataChange() {
            var ddl;
            var controlValue;
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "N";
            if (document.getElementById("<%=txtRoyaltorName.ClientID %>").value != document.getElementById("<%=hdntxtRoyaltorName.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            if (document.getElementById("<%=txtRoyPLGId.ClientID %>").value != document.getElementById("<%=hdntxtRoyPLGId.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            if (document.getElementById("<%=txtOwnerSearch.ClientID %>").value != document.getElementById("<%=hdntxtOwnerSearch.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            controlValue = document.getElementById("<%=txtExpiryDate.ClientID %>").value;
            if (
                (document.getElementById("<%=hdntxtExpiryDate.ClientID %>").value != "" && (controlValue == "DD/MM/YYYY" || controlValue == "")) ||
                (document.getElementById("<%=hdntxtExpiryDate.ClientID %>").value == "" && (controlValue != "DD/MM/YYYY" && controlValue != "")) ||
                (document.getElementById("<%=hdntxtExpiryDate.ClientID %>").value != "" && (controlValue != document.getElementById("<%=hdntxtExpiryDate.ClientID %>").value))
                ) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            controlValue = document.getElementById("<%=txtStartDate.ClientID %>").value;
            if (
                (document.getElementById("<%=hdntxtStartDate.ClientID %>").value != "" && (controlValue == "DD/MM/YYYY" || controlValue == "")) ||
                (document.getElementById("<%=hdntxtStartDate.ClientID %>").value == "" && (controlValue != "DD/MM/YYYY" && controlValue != "")) ||
                (document.getElementById("<%=hdntxtStartDate.ClientID %>").value != "" && (controlValue != document.getElementById("<%=hdntxtStartDate.ClientID %>").value))
                ) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            if (document.getElementById("<%=txtRoyaltorSearch.ClientID %>").value != document.getElementById("<%=hdntxtRoyaltorSearch.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            if (document.getElementById("<%=txtChargeablePct.ClientID %>").value != document.getElementById("<%=hdntxtChargeablePct.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            controlValue = document.getElementById("<%=txtResvEndDate.ClientID %>").value;
            hdntxtResvEndDateVal = document.getElementById("<%=hdntxtResvEndDate.ClientID %>").value;
            if (((controlValue == "" || controlValue == "__/____") && hdntxtResvEndDateVal != "") || ((controlValue != "" && controlValue != "__/____") && hdntxtResvEndDateVal == "") ||
                 ((controlValue != "" && controlValue != "__/____") && (controlValue != hdntxtResvEndDateVal))) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            if (document.getElementById("<%=txtDefaultResvPct.ClientID %>").value != document.getElementById("<%=hdntxtDefaultResvPct.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlNettOrSalesUnits.ClientID %>");
            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdntxtNettOrSalesUnits.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdntxtNettOrSalesUnits.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlStatus.ClientID %>");
            if (document.getElementById("<%=hdnStatusCode.ClientID %>").value != "" &&
                (ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnStatusCode.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlCompany.ClientID %>");
            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdnddlCompany.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnddlCompany.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlLabel.ClientID %>");
            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdnddlLabel.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnddlLabel.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlResponsibility.ClientID %>");
            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdnddlResponsibility.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnddlResponsibility.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlContractType.ClientID %>");
            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdnddlContractType.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnddlContractType.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlStatementFormat.ClientID %>");
            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdnddlStatementFormat.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnddlStatementFormat.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlReportingSch.ClientID %>");
            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdnddlReportingSch.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnddlReportingSch.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlStmtPriority.ClientID %>");

            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdnddlStmtPriority.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnddlStmtPriority.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            ddl = document.getElementById("<%=ddlResvTakenOn.ClientID %>");
            if ((ddl.selectedIndex == 0 && document.getElementById("<%=hdnddlResvTakenOn.ClientID %>").value != "") ||
                (ddl.selectedIndex != 0 && ddl.options[ddl.selectedIndex].value != document.getElementById("<%=hdnddlResvTakenOn.ClientID %>").value)) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

            if (document.getElementById("<%=cbHeld.ClientID %>").checked && document.getElementById("<%=hdncbHeld.ClientID %>").value == "N") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }
            else if (!document.getElementById("<%=cbHeld.ClientID %>").checked && document.getElementById("<%=hdncbHeld.ClientID %>").value == "Y") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
                return;
            }

        if (document.getElementById("<%=cbSendToPortal.ClientID %>").checked && document.getElementById("<%=hdncbSendToPortal.ClientID %>").value == "N") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            return;
        }
        else if (!document.getElementById("<%=cbSendToPortal.ClientID %>").checked && document.getElementById("<%=hdncbSendToPortal.ClientID %>").value == "Y") {
            document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
            return;
        }
            //JIRA-1006 Changes by Ravi --Start
    if (document.getElementById("<%=cbExcludeFromAccrual.ClientID %>").checked && document.getElementById("<%=hdncbExcludeFromAccrual.ClientID %>").value == "N") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }
    else if (!document.getElementById("<%=cbExcludeFromAccrual.ClientID %>").checked && document.getElementById("<%=hdncbExcludeFromAccrual.ClientID %>").value == "Y") {
        document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }
            //JIRA-1006 Changes by Ravi --End
            //JIRA-970 CHanges by Ravi on 14/02/2019 -- Start
    if (document.getElementById("<%=txtSocSecNo.ClientID %>").value != document.getElementById("<%=hdnSocSecNo.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }
            //JIRA-970 CHanges by Ravi on 14/02/2019 -- End
    if (document.getElementById("<%=cbLock.ClientID %>").checked && document.getElementById("<%=hdncbLock.ClientID %>").value == "N") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }
    else if (!document.getElementById("<%=cbLock.ClientID %>").checked && document.getElementById("<%=hdncbLock.ClientID %>").value == "Y") {
        document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=cbSigned.ClientID %>").checked && document.getElementById("<%=hdncbSigned.ClientID %>").value == "N") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }
    else if (!document.getElementById("<%=cbSigned.ClientID %>").checked && document.getElementById("<%=hdncbSigned.ClientID %>").value == "Y") {
        document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=cbDisplayZero.ClientID %>").checked && document.getElementById("<%=hdncbDisplayZero.ClientID %>").value == "N") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }
    else if (!document.getElementById("<%=cbDisplayZero.ClientID %>").checked && document.getElementById("<%=hdncbDisplayZero.ClientID %>").value == "Y") {
        document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=cbProducerSumm.ClientID %>").checked && document.getElementById("<%=hdncbProducerSumm.ClientID %>").value == "N") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }
    else if (!document.getElementById("<%=cbProducerSumm.ClientID %>").checked && document.getElementById("<%=hdncbProducerSumm.ClientID %>").value == "Y") {
        document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=cbCostSummary.ClientID %>").checked && document.getElementById("<%=hdncbCostSummary.ClientID %>").value == "N") {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }
    else if (!document.getElementById("<%=cbCostSummary.ClientID %>").checked && document.getElementById("<%=hdncbCostSummary.ClientID %>").value == "Y") {
        document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=txtLiqPct1.ClientID %>").value != document.getElementById("<%=hdntxtLiqPct1.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=txtLiqPct2.ClientID %>").value != document.getElementById("<%=hdntxtLiqPct2.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=txtLiqPct3.ClientID %>").value != document.getElementById("<%=hdntxtLiqPct3.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=txtLiqPct4.ClientID %>").value != document.getElementById("<%=hdntxtLiqPct4.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=txtLiqPct5.ClientID %>").value != document.getElementById("<%=hdntxtLiqPct5.ClientID %>").value) {
                //document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=txtLiqPct6.ClientID %>").value != document.getElementById("<%=hdntxtLiqPct6.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=txtLiqPct7.ClientID %>").value != document.getElementById("<%=hdntxtLiqPct7.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }

    if (document.getElementById("<%=txtLiqPct8.ClientID %>").value != document.getElementById("<%=hdntxtLiqPct8.ClientID %>").value) {
                document.getElementById("<%=hdnChangeNotSaved.ClientID %>").innerText = "Y";
        return;
    }


}

//Validate any unsaved data on browser window close/refresh
function RedirectToErrorPage() {
    document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
    window.location = "../Common/ExceptionPage.aspx";
}

//redirect to payee screen on saving data of new royaltor so that issue of data not saved validation would be handled
function RedirectOnNewRoyaltorSave(royaltorId) {
    document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value = "Y";
    window.location = "../Contract/RoyContractPayee.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=Y";
}

var unSaveBrowserClose = false;
function WarnOnUnSavedData() {
    CompareDataChange();
    var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
    var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
    var isNewRoyaltorSaved = document.getElementById("<%=hdnNewRoyaltorSaved.ClientID %>").value;
    var isContractScreen = document.getElementById("hdnIsContractScreen").value;
    var isNotContractScreen = document.getElementById("hdnIsNotContractScreen").value;
    if (isExceptionRaised != "Y" && isNewRoyaltorSaved != "Y" && isContractScreen != "Y" && isNotContractScreen != "Y") {
        if (isDataChanged == "Y") {
            unSaveBrowserClose = true;
            return warningMsgOnUnSavedData;
        }
    }
    UpdateScreenLockFlag();// WUIN-599 - Unset the screen lock flag If an user close the browser with out unsaved data or navigate to other than contract screens
}
window.onbeforeunload = WarnOnUnSavedData;

function UpdateScreenLockFlag() {
    var isOtherUserScreenLocked = document.getElementById("<%=hdnOtherUserScreenLocked.ClientID %>").value;
    var isAuditScreen = document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value;
    var isContractScreen = document.getElementById("hdnIsContractScreen").value;
    if (isOtherUserScreenLocked == "N" && isAuditScreen == "N" && isContractScreen == "N") {
        document.getElementById("<%=hdnIsAuditScreen.ClientID %>").value = "Y";
        PageMethods.UpdateScreenLockFlag();
    }
}

//WUIN-599 Unset the screen lock flag If an user close the browser with unsaved data
window.onunload = function () {
    if (unSaveBrowserClose) {
        UpdateScreenLockFlag();
    }

}

//used to check if any changes to allow navigation to other screen 
function IsDataChanged() {
    CompareDataChange();
    var isDataChanged = document.getElementById("<%=hdnChangeNotSaved.ClientID %>").value;
    if (isDataChanged == "Y") {
        return true;
    }
    else {
        return false;
    }
}


//=================End

//Validations ============== Begin
function ValidatePopUpSave() {
    //warning on save validation fail        
    if (!Page_ClientValidate("valGrpSave")) {
        Page_BlockSubmit = false;
        DisplayMessagePopup("Royaltor Contract details not saved – invalid or missing data");
        return false;
    }
    else {
        CompareDataChange();
        return true;
    }

}

//triggers on status change
function ValidateRoyStatus() {
    //Validate status update    
    //Only Super Users can update to STATUS_CODE = 3							    
    var hdnStatusCode = document.getElementById("<%=hdnStatusCode.ClientID %>").value;
    var selectedCode = document.getElementById("<%=ddlStatus.ClientID %>").value;
    var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;

    if (hdnUserRole != "SuperUser" && hdnUserRole != "Supervisor" && selectedCode == "3") {

        DisplayMessagePopup("Only SuperUser and Supervisor can update to Manager Sign Off!");

        if (hdnStatusCode == "") {
            document.getElementById("<%=ddlStatus.ClientID %>").value = "1";
        }
        else {
            document.getElementById("<%=ddlStatus.ClientID %>").value = hdnStatusCode;
        }
    }


    if (hdnStatusCode != selectedCode) {
        document.getElementById("<%=hdnIsStatusChange.ClientID %>").value = "Y";
    }
    return false;

}

//Validations =================End

//Disable back button on royaltor id textbox for existing royaltor
function MoveRoyaltorIdFocus() {
    document.getElementById("<%=txtRoyPLGId.ClientID %>").focus();
}

//=================End

function CancelOwnerClick() {
    var popup = $find('<%= mpeAddOwner.ClientID %>');
    if (popup != null) {
        document.getElementById('<%=txtOwnerDesc.ClientID%>').value = "";
        document.getElementById('<%=revOwnerCode.ClientID%>').style.display = "none";
        document.getElementById('<%=rvOwneDesc.ClientID%>').style.display = "none";
        popup.hide();
    }
    return false;
}
//==============End

//WUIN-800 - Auto populating Royaltor type based on chargeableTo value
function ChangeRoyltorType() {
    var chargeableTo = document.getElementById('<%=txtRoyaltorSearch.ClientID%>').value;
    if (chargeableTo == "") {
        document.getElementById('<%=hdnRoyaltorType.ClientID%>').value = "A";
    }

}


//Audit button navigation
function RedirectToAuditScreen(royaltorId) {
    if (IsDataChanged()) {
        window.onbeforeunload = null;
        OpenPopupOnUnSavedData("../Audit/RoyContractAudit.aspx?RoyaltorId=" + royaltorId);
    }
    else {
        window.location = "../Audit/RoyContractAudit.aspx?RoyaltorId=" + royaltorId;
    }
}
function RedirectToPreviousScreen(royaltorId) {
    if (IsDataChanged()) {
        window.onbeforeunload = null;
        OpenPopupOnUnSavedData("../Contract/RoyaltorSearch.aspx?isNewRequest=N");
    }
    else {
        window.location = "../Contract/RoyaltorSearch.aspx?isNewRequest=N";
    }
}

//=================End

// Copy contract events -- Start
var gvOptionClientId = "ContentPlaceHolderBody_gvOptionCopy_";
var gvEscClientId = "ContentPlaceHolderBody_gvEscCodes_";

function CheckAllOptionPeriods() {
    var chkAllOptions = document.getElementById("<%= chkAllOptions.ClientID %>");
            var gvOptionCopy = document.getElementById("<%= gvOptionCopy.ClientID %>");
            if (gvOptionCopy != null) {
                var gvRows = gvOptionCopy.rows;
                for (var i = 0; i < gvRows.length; i++) {
                    if (chkAllOptions.checked == true) {
                        document.getElementById(gvOptionClientId + 'chkOptionCode' + '_' + i).checked = true;
                    }
                    else {
                        document.getElementById("<%= chkAllRoyRates.ClientID %>").checked = false;
                        document.getElementById("<%= chkAllSubRates.ClientID %>").checked = false;
                        document.getElementById("<%= chkAllPackRates.ClientID %>").checked = false;

                        document.getElementById(gvOptionClientId + 'chkOptionCode' + '_' + i).checked = false;
                        document.getElementById(gvOptionClientId + 'chkRoyRates' + '_' + i).checked = false;
                        document.getElementById(gvOptionClientId + 'chkSubRates' + '_' + i).checked = false;
                        document.getElementById(gvOptionClientId + 'chkPackRates' + '_' + i).checked = false;
                    }
                }
            }
        }

        function CheckAllRates(ratetype) {
            var chkAllOptions = document.getElementById("<%= chkAllOptions.ClientID %>");
    var chkAllRates = document.getElementById('ContentPlaceHolderBody_chkAll' + ratetype + 'Rates');
    var gvOptionCopy = document.getElementById("<%= gvOptionCopy.ClientID %>");
    if (gvOptionCopy != null) {
        var gvRows = gvOptionCopy.rows;
        if (chkAllRates.checked == true) {
            if (chkAllOptions.checked == true) {
                for (var i = 0; i < gvRows.length; i++) {
                    if (document.getElementById(gvOptionClientId + 'chkOptionCode_' + i).checked) {
                        document.getElementById(gvOptionClientId + 'chk' + ratetype + 'Rates_' + i).checked = true;
                    }
                }
            }
            else {
                DisplayMessagePopup("Please select option periods first.");
                chkAllRates.checked = false;
            }
        }
        else {
            for (var i = 0; i < gvRows.length; i++) {
                document.getElementById(gvOptionClientId + 'chk' + ratetype + 'Rates' + '_' + i).checked = false;
            }
        }

    }
}
function CheckAllEscCodes() {
    var chkEscRates = document.getElementById("<%= chkEscRates.ClientID %>");
            var gvEscCodes = document.getElementById("<%= gvEscCodes.ClientID %>");
            if (gvEscCodes != null) {
                var gvRows = gvEscCodes.rows;
                for (var i = 0; i < gvRows.length; i++) {
                    if (chkEscRates.checked == true) {
                        document.getElementById(gvEscClientId + 'chkEscCode' + '_' + i).checked = true;
                    }
                    else {
                        document.getElementById(gvEscClientId + 'chkEscCode' + '_' + i).checked = false;
                    }
                }
            }
        }

        function CheckRate(control, ratetype) {
            var rowIndex = control.id.substring(control.id.lastIndexOf("_") + 1);
            if (ratetype == "Option") {
                if (!(document.getElementById(control.id).checked)) {
                    document.getElementById(gvOptionClientId + 'chkRoyRates' + '_' + rowIndex).checked = false;
                    document.getElementById(gvOptionClientId + 'chkSubRates' + '_' + rowIndex).checked = false;
                    document.getElementById(gvOptionClientId + 'chkPackRates' + '_' + rowIndex).checked = false;
                }

            }
            else {
                chkgridOption = document.getElementById(gvOptionClientId + 'chkOptionCode' + '_' + rowIndex);
                if (document.getElementById(control.id).checked) {
                    if (chkgridOption.checked) {
                        document.getElementById(gvOptionClientId + 'chk' + ratetype + 'Rates_' + rowIndex).checked = true;
                    }
                    else {
                        DisplayMessagePopup("Please select option period first.");
                        document.getElementById(gvOptionClientId + 'chk' + ratetype + 'Rates_' + rowIndex).checked = false;

                    }
                }
            }

        }

        //WUIN-627 Validation
        //Need to check at least one option to have copy selected functionality
        //do not check any copy options to have copy all functionality

        function ValidatSelectedCopy(id) {

            id = id.substring(id.lastIndexOf("_") + 1, id.length);
            if (!Page_ClientValidate("valGrpCopyContract")) {
                Page_BlockSubmit = false;
                return false;
            }
            // Looping option period grid to check atleast one option selected or not
            var gvOptionCopy = document.getElementById("<%= gvOptionCopy.ClientID %>");
    var gvEscCodes = document.getElementById("<%= gvEscCodes.ClientID %>");
    if (gvOptionCopy != null) {
        var gvRows = gvOptionCopy.rows;
        for (var i = 0; i < gvRows.length; i++) {
            if (
            document.getElementById(gvOptionClientId + 'chkOptionCode' + '_' + i).checked ||
            document.getElementById(gvOptionClientId + 'chkRoyRates' + '_' + i).checked ||
            document.getElementById(gvOptionClientId + 'chkSubRates' + '_' + i).checked ||
            document.getElementById(gvOptionClientId + 'chkPackRates' + '_' + i).checked) {
                document.getElementById("<%= hdnCopySelected.ClientID %>").value = "Y";
                break;

            }
        }
    }
    // Looping in esc rates grid to check atleast one option selected or not
    if (gvEscCodes != null) {
        var gvRows = gvEscCodes.rows;
        for (var i = 0; i < gvRows.length; i++) {
            if (document.getElementById(gvEscClientId + 'chkEscCode' + '_' + i).checked) {
                document.getElementById("<%= hdnCopySelected.ClientID %>").value = "Y";
                break;
            }
        }
    }

    if (id == "btnCopySelectedContract") {
        if (document.getElementById("<%= hdnCopySelected.ClientID %>").value != "Y") {
            DisplayMessagePopup("Please select copy options.");
            return false;
        }
        else {
            return true;
        }
    }
    else {
        if (document.getElementById("<%= hdnCopySelected.ClientID %>").value == "Y") {
            DisplayMessagePopup("Please do not select copy options.");
            document.getElementById("<%= hdnCopySelected.ClientID %>").value = "N";
            return false;
        }
        else {
            return true;
        }
    }

}


function CancelCopyContract() {
    var popup = $find('<%= mpeCopyContract.ClientID %>');
    if (popup != null) {
        document.getElementById('<%=txtRoyaltorIdCopyCont.ClientID%>').value = "";
        document.getElementById('<%=txtRoyaltorNameCopyCont.ClientID%>').value = "";
        document.getElementById('<%=rfvtxtRoyaltorIdCopyCont.ClientID%>').style.display = "none";
        document.getElementById('<%=revtxtRoyaltorIdCopyCont.ClientID%>').style.display = "none";
        document.getElementById('<%=rfvtxtRoyaltorNameCopyCont.ClientID%>').style.display = "none";
        document.getElementById("<%= hdnCopySelected.ClientID %>").value = "N";
        document.getElementById("<%= chkAllOptions.ClientID %>").checked = false;
        document.getElementById("<%= chkAllRoyRates.ClientID %>").checked = false;
        document.getElementById("<%= chkAllSubRates.ClientID %>").checked = false;
        document.getElementById("<%= chkAllPackRates.ClientID %>").checked = false;
        document.getElementById("<%= chkEscRates.ClientID %>").checked = false;

        var gvOptionCopy = document.getElementById("<%= gvOptionCopy.ClientID %>");
        var gvEscCodes = document.getElementById("<%= gvEscCodes.ClientID %>");

        if (gvOptionCopy != null) {
            var gvRows = gvOptionCopy.rows;
            for (var i = 0; i < gvRows.length; i++) {
                document.getElementById(gvOptionClientId + 'chkOptionCode' + '_' + i).checked = false;
                document.getElementById(gvOptionClientId + 'chkRoyRates' + '_' + i).checked = false;
                document.getElementById(gvOptionClientId + 'chkSubRates' + '_' + i).checked = false;
                document.getElementById(gvOptionClientId + 'chkPackRates' + '_' + i).checked = false;
            }
        }
        if (gvEscCodes != null) {
            var gvRows = gvEscCodes.rows;
            for (var i = 0; i < gvRows.length; i++) {
                document.getElementById(gvEscClientId + 'chkEscCode' + '_' + i).checked = false;
            }
        }
        popup.hide();
    }

    return false;
}
//==============End

function CloseScreenLockMsgPopup() {
    //close pop up on Enter key
    if (event.keyCode == 13) {
        document.getElementById("<%= btnCloseScreenLockPopup.ClientID %>").click();
            }
        }

        function ValidateCopy(button) {            
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
                return false;
            }
            return true;
        }

        function ValidateLock(button) {           
            if (IsDataChanged()) {
                OpenOnUnSavedData();
                document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
        return false;
    }
    return true;
}


//WUIN-932 Unsaved Pop-up implementation
function OpenOnUnSavedData() {   
    var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                document.getElementById("<%=lblUnSavedWarnMsg.ClientID %>").innerText = warningMsgOnUnSavedData;
        warnPopup.show();

    }
}

function OnUnSavedDataReturn() {   
    var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
    if (warnPopup != null) {
        warnPopup.hide();
    }

    window.onbeforeunload = WarnOnUnSavedData;
    return false;
}

function OnUnSavedDataExit() {    
    window.onbeforeunload = WarnOnUnSavedData;

    var warnPopup = $find('<%= mpeUnSavedWarning.ClientID %>');
            if (warnPopup != null) {
                warnPopup.hide();
            }
            return true;
        }

    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    ROYALTOR CONTRACT
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td width="2%"></td>
                    <td valign="top">
                        <table width="99%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="table_header_with_border">Royaltor Details</td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" class="table_with_border">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td width="10%" class="identifierLable_large_bold">Royaltor Number</td>
                                                        <td width="35%">
                                                            <table width="100%" cellpadding="0" cellspacing="0" style="padding-bottom: 0">
                                                                <tr>
                                                                    <td width="30%">
                                                                        <asp:TextBox ID="txtRoyaltorId" runat="server" Width="80%" CssClass="textboxStyle" MaxLength="5" TextMode="Number"
                                                                            OnTextChanged="txtRoyaltorId_TextChanged" AutoPostBack="true" TabIndex="100"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtRoyaltorId" ControlToValidate="txtRoyaltorId" ValidationGroup="valGrpSave"
                                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter royaltor number" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator ID="revtxtRoyaltorId" runat="server" Text="*" ControlToValidate="txtRoyaltorId" ValidationGroup="valGrpSave"
                                                                            ValidationExpression="^[1-9][0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                            ToolTip="Please enter only positive number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                                    </td>
                                                                    <td width="15%"></td>
                                                                    <td class="identifierLable_large_bold">PLG Royaltor Number</td>
                                                                    <td width="25%" align="right">
                                                                        <asp:TextBox ID="txtRoyPLGId" runat="server" Width="96%" CssClass="textboxStyle" MaxLength="20" TabIndex="101"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td width="10%"></td>
                                                        <td width="10%" class="identifierLable_large_bold">Status</td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlStatus" runat="server" Width="65%" CssClass="ddlStyle" TabIndex="108" onchange="return ValidateRoyStatus();">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td width="5%"></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="identifierLable_large_bold">Royaltor Name</td>
                                                        <td>
                                                            <asp:TextBox ID="txtRoyaltorName" runat="server" Width="99%" CssClass="textboxStyle" MaxLength="200" TabIndex="102"></asp:TextBox>

                                                        </td>
                                                        <td>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvtxtRoyaltorName" ControlToValidate="txtRoyaltorName" ValidationGroup="valGrpSave"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter royaltor name" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                        <td class="identifierLable_large_bold">Company</td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlCompany" runat="server" Width="65%" CssClass="ddlStyle" TabIndex="109">
                                                            </asp:DropDownList>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvddlCompany" ControlToValidate="ddlCompany" ValidationGroup="valGrpSave"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please select company" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Responsibility</td>
                                            <td>
                                                <asp:DropDownList ID="ddlResponsibility" runat="server" Width="50%" CssClass="ddlStyle" AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlResponsibility_SelectedIndexChanged" TabIndex="103">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvddlResponsibility" ControlToValidate="ddlResponsibility" ValidationGroup="valGrpSave"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select responsibility" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Owner</td>
                                            <td>

                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td width="86%">
                                                            <asp:TextBox ID="txtOwnerSearch" runat="server" CssClass="textboxStyle" TabIndex="110" ToolTip="Search by owner code/name" Width="98%"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="aceOwnerFuzzySearch" runat="server"
                                                                ServiceMethod="FuzzySearchAllOwnerList"
                                                                ServicePath="~/Services/FuzzySearch.asmx"
                                                                CompletionInterval="100"
                                                                CompletionListElementID="acePnlOwnerFuzzySearch"
                                                                CompletionSetCount="20"
                                                                EnableCaching="false"
                                                                FirstRowSelected="true"
                                                                MinimumPrefixLength="1"
                                                                OnClientHidden="ownerListPopulated"
                                                                OnClientItemSelected="ownerListItemSelected"
                                                                OnClientPopulated="ownerListPopulated"
                                                                OnClientPopulating="ownerListPopulating"
                                                                OnClientShown="ownerScrollPosition"
                                                                TargetControlID="txtOwnerSearch" />
                                                            <asp:Panel ID="acePnlOwnerFuzzySearch" runat="server" CssClass="identifierLable" />
                                                        </td>
                                                        <td align="left" width="10%">
                                                            <asp:CustomValidator ID="valOwner" runat="server" CssClass="requiredFieldValidator" Display="Dynamic" ErrorMessage="*" OnServerValidate="valOwner_ServerValidate" ToolTip="Please select a owner from list" ValidationGroup="valGrpSave"></asp:CustomValidator>
                                                            <asp:ImageButton ID="btnOwnerFuzzySearch" runat="server" CssClass="FuzzySearch_Button" ImageUrl="../Images/search.png" OnClick="btnOwnerFuzzySearch_Click" ToolTip="Search by owner code/name" />
                                                        </td>
                                                        <td align="left">
                                                            <asp:Button ID="btnAddOwner" runat="server" CssClass="ButtonStyle" TabIndex="111" Text="Add Owner" UseSubmitBehavior="false" OnClick="btnAddOwner_Click" />
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Label</td>
                                            <td>
                                                <asp:DropDownList ID="ddlLabel" runat="server" Width="50%" CssClass="ddlStyle" TabIndex="104">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Lock</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbLock" runat="server" TabIndex="112" />
                                                </div>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Contract Type</td>
                                            <td>
                                                <asp:DropDownList ID="ddlContractType" runat="server" Width="50%" CssClass="ddlStyle" TabIndex="104">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfddlContractType" ControlToValidate="ddlContractType" ValidationGroup="valGrpSave"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Contract Type" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>

                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Statement Format</td>
                                            <td>
                                                <asp:DropDownList ID="ddlStatementFormat" runat="server" Width="50%" CssClass="ddlStyle" TabIndex="113">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfddlStatementFormat" ControlToValidate="ddlStatementFormat" ValidationGroup="valGrpSave"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select Statement Format" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>

                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Start Date</td>
                                            <td>
                                                <asp:TextBox ID="txtStartDate" runat="server" Width="72" CssClass="textboxStyle" TabIndex="105"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="wmetxtStartDate" runat="server" TargetControlID="txtStartDate"
                                                    WatermarkText="DD/MM/YYYY" WatermarkCssClass="waterMarkText">
                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                                <ajaxToolkit:MaskedEditExtender ID="masktxtStartDate" runat="server"
                                                    TargetControlID="txtStartDate" Mask="99/99/9999" AcceptNegative="None"
                                                    ClearMaskOnLostFocus="true" MaskType="Date" />
                                                <asp:RegularExpressionValidator ID="regtxtStartDate" runat="server" ControlToValidate="txtStartDate"
                                                    ValidationExpression="((0[1-9]|1[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$"
                                                    ErrorMessage="*" ToolTip="Please enter valid start date in DD/MM/YYYY format" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    Display="Dynamic" />
                                                <asp:CustomValidator ID="valStartDate" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    OnServerValidate="valStartDate_ServerValidate" ToolTip="Please enter a start date"
                                                    ErrorMessage="*"></asp:CustomValidator>


                                            </td>

                                            <td></td>
                                            <td class="identifierLable_large_bold">Signed</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbSigned" runat="server" TabIndex="114" />
                                                </div>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Expiry Date</td>
                                            <td>
                                                <asp:TextBox ID="txtExpiryDate" runat="server" Width="72" CssClass="textboxStyle" TabIndex="106"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="wmetxtExpiryDate" runat="server" TargetControlID="txtExpiryDate"
                                                    WatermarkText="DD/MM/YYYY" WatermarkCssClass="waterMarkText">
                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                                <ajaxToolkit:MaskedEditExtender ID="masktxtExpiryDate" runat="server"
                                                    TargetControlID="txtExpiryDate" Mask="99/99/9999" AcceptNegative="None"
                                                    ClearMaskOnLostFocus="true" MaskType="Date" />
                                                <asp:RegularExpressionValidator ID="regtxtExpiryDate" runat="server" ControlToValidate="txtExpiryDate"
                                                    ValidationExpression="((0[1-9]|1[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$"
                                                    ErrorMessage="*" ToolTip="Please enter valid future date in DD/MM/YYYY format" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    Display="Dynamic" />
                                                <asp:CustomValidator ID="valExpiryDate" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    OnServerValidate="valExpiryDate_ServerValidate" ToolTip="Please enter a future date"
                                                    ErrorMessage="*"></asp:CustomValidator>

                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Held</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbHeld" runat="server" TabIndex="115" />
                                                </div>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <%--JIRA-970 CHanges by Ravi on 14/02/2019 -- Start--%>
                                            <td class="identifierLable_large_bold">Soc Sec No</td>
                                            <td width="25%">
                                                <asp:TextBox ID="txtSocSecNo" runat="server" CssClass="textboxStyle" TabIndex="107" MaxLength="20" Style="text-transform: uppercase">
                                                </asp:TextBox>
                                            </td>
                                            <%--JIRA-970 CHanges by Ravi on 14/02/2019 -- End--%>

                                            <td></td>
                                            <td class="identifierLable_large_bold">Send to Portal</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbSendToPortal" runat="server" Checked="true" TabIndex="116" />
                                                </div>
                                            </td>
                                            <td></td>

                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <%--//JIRA-1006 Changes by Ravi --Start--%>
                                            <td class="identifierLable_large_bold">Exclude from Accrual</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbExcludeFromAccrual" runat="server" Checked="false" TabIndex="117" />
                                                </div>
                                            </td>
                                            <%--//JIRA-1006 Changes by Ravi --End--%>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="table_header_with_border">Producer Deductions</td>
                </tr>
                <tr>
                    <td>
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td width="10%" class="identifierLable_large_bold">Chargeable To:</td>
                                            <td width="20%">
                                                <asp:TextBox ID="txtRoyaltorSearch" runat="server" Width="99%" CssClass="textboxStyle" TabIndex="118" onchange="ChangeRoyltorType();"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="aceRoyaltorFuzzySearch" runat="server"
                                                    ServiceMethod="FuzzySearchAllRoyListWithOwnerCode"
                                                    ServicePath="~/Services/FuzzySearch.asmx"
                                                    MinimumPrefixLength="1"
                                                    CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                    TargetControlID="txtRoyaltorSearch"
                                                    FirstRowSelected="true"
                                                    OnClientPopulating="RoyaltorListPopulating"
                                                    OnClientPopulated="RoyaltorListPopulated"
                                                    OnClientHidden="RoyaltorListPopulated"
                                                    OnClientShown="RoyaltorScrollPosition"
                                                    OnClientItemSelected="RoyaltorListItemSelected"
                                                    CompletionListElementID="acePnlRoyaltorFuzzySearch" />
                                                <asp:Panel ID="acePnlRoyaltorFuzzySearch" runat="server" CssClass="identifierLable" />

                                            </td>
                                            <td width="5%">
                                                <asp:CustomValidator ID="valtxtRoyaltorSearch" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    OnServerValidate="valtxtRoyaltorSearch_ServerValidate" ToolTip="Please select royaltor from search list"
                                                    ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                <asp:ImageButton ID="btnRoyaltorSearch" ImageUrl="../Images/search.png" runat="server" CssClass="FuzzySearch_Button"
                                                    OnClick="btnRoyaltorSearch_Click" ToolTip="Search by royaltor id/name" />
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td width="10%" class="identifierLable_large_bold">Chargeable %</td>
                                            <td width="15%">
                                                <asp:TextBox ID="txtChargeablePct" runat="server" Width="65%" CssClass="textboxStyle" TabIndex="119"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revtxtChargeablePct" runat="server" Text="*" ControlToValidate="txtChargeablePct" ValidationGroup="valGrpSave"
                                                    ValidationExpression="^100$|^\d{0,2}(\.\d{1,3})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                    ToolTip="Please enter only positive number <= 100 upto 3 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                <asp:CustomValidator ID="valtxtChargeablePct" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    OnServerValidate="valtxtChargeablePct_ServerValidate" ToolTip="Please enter chargeable %"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                            </td>

                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="table_header_with_border">Statement Details</td>
                </tr>
                <tr>
                    <td>
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td width="10%" class="identifierLable_large_bold">Reporting Schedule</td>
                                            <td width="35%" style="padding-bottom: 3px">
                                                <asp:DropDownList ID="ddlReportingSch" runat="server" Width="50%" CssClass="ddlStyle" TabIndex="121">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvddlReportingSch" ControlToValidate="ddlReportingSch" ValidationGroup="valGrpSave"
                                                    Text="*" CssClass="requiredFieldValidator" ToolTip="Please select reporting schedule" InitialValue="-" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </td>
                                            <td width="10%"></td>
                                            <td width="14%" class="identifierLable_large_bold">Display 0 Values on Stmts</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbDisplayZero" runat="server" TabIndex="122" />
                                                </div>
                                            </td>
                                            <td width="5%"></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Statement Priority</td>
                                            <td>
                                                <asp:DropDownList ID="ddlStmtPriority" runat="server" Width="50%" CssClass="ddlStyle" TabIndex="123">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Producer Summary</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbProducerSumm" runat="server" TabIndex="124" />
                                                </div>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <%--<td class="identifierLable_large_bold">Print Group</td>
                                                        <td>
                                                            <asp:TextBox ID="txtPrintGrp" runat="server" Width="25%" CssClass="textboxStyle" TabIndex="115"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvtxtPrintGrp" ControlToValidate="txtPrintGrp" ValidationGroup="valGrpSave"
                                                                Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter print group" Display="Dynamic"></asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="revtxtPrintGrp" runat="server" Text="*" ControlToValidate="txtPrintGrp" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^([1-3]|3)$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only numbers 1,2 or 3 " Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>--%>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Cost Report</td>
                                            <td>
                                                <div style="position: relative; left: -3px;">
                                                    <asp:CheckBox ID="cbCostSummary" runat="server" TabIndex="125" />
                                                </div>
                                            </td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="table_header_with_border">Reserve Details</td>
                </tr>
                <tr>
                    <td>
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td width="15%" class="identifierLable_large_bold">Products Reserves Taken On</td>
                                            <td width="35%">
                                                <asp:DropDownList ID="ddlResvTakenOn" runat="server" Width="50%" CssClass="ddlStyle" TabIndex="126">
                                                </asp:DropDownList>
                                                <asp:CustomValidator ID="valddlResvTakenOn" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    OnServerValidate="valddlResvTakenOn_ServerValidate" ToolTip="Please select product reserves taken on"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                            </td>
                                            <td width="5%"></td>
                                            <td width="14%" class="identifierLable_large_bold">Default Royaltor Reserve %</td>
                                            <td>
                                                <asp:TextBox ID="txtDefaultResvPct" runat="server" Width="22.5%" CssClass="textboxStyle" TabIndex="127"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revtxtDefaultResvPct" runat="server" Text="*" ControlToValidate="txtDefaultResvPct" ValidationGroup="valGrpSave"
                                                    ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                    ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>

                                            </td>
                                            <td width="5%"></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Reserves End Date</td>
                                            <td>
                                                <asp:TextBox ID="txtResvEndDate" runat="server" Width="65" CssClass="textboxStyle" ToolTip="MM/YYYY" TabIndex="128"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="wmetxtResvEndDate" runat="server" TargetControlID="txtResvEndDate"
                                                    WatermarkText="MM/YYYY" WatermarkCssClass="waterMarkText">
                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                                <ajaxToolkit:MaskedEditExtender ID="mtetxtResvEndDate" runat="server"
                                                    TargetControlID="txtResvEndDate" Mask="99/9999" AcceptNegative="None" ClearMaskOnLostFocus="false" />
                                                <asp:CustomValidator ID="valResvEndAte" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    OnServerValidate="valResvEndAte_ServerValidate" ToolTip="Please enter a valid date in MM/YYYY format"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                            </td>
                                            <td></td>
                                            <td class="identifierLable_large_bold">Nett units / Sales units (N/S)</td>
                                            <td>
                                                <asp:DropDownList ID="ddlNettOrSalesUnits" runat="server" Width="23.5%" CssClass="ddlStyle" TabIndex="129">
                                                </asp:DropDownList>
                                                <%--<asp:RegularExpressionValidator ID="revtxtNettOrSalesUnits" runat="server" Text="*" ControlToValidate="txtNettOrSalesUnits" ValidationGroup="valGrpSave"
                                                                ValidationExpression="[NS]" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only N/S" Display="Dynamic"> </asp:RegularExpressionValidator>--%>
                                                <asp:CustomValidator ID="valNettOrSalesUnits" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                    OnServerValidate="valcbSalesUnits_ServerValidate" ToolTip="Please enter nett / sales units"
                                                    ErrorMessage="*"></asp:CustomValidator>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td class="identifierLable_large_bold">Liquidation Period %</td>
                                            <td colspan="4">
                                                <table width="77.5%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtLiqPct1" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="130"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="rfvtxtLiqPct1" runat="server" Text="*" ControlToValidate="txtLiqPct1" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtLiqPct2" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="131"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="rfvtxtLiqPct2" runat="server" Text="*" ControlToValidate="txtLiqPct2" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtLiqPct3" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="132"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="rfvtxtLiqPct3" runat="server" Text="*" ControlToValidate="txtLiqPct3" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtLiqPct4" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="133"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="rfvtxtLiqPct4" runat="server" Text="*" ControlToValidate="txtLiqPct4" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtLiqPct5" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="134"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="rfvtxtLiqPct5" runat="server" Text="*" ControlToValidate="txtLiqPct5" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtLiqPct6" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="135"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="rfvtxtLiqPct6" runat="server" Text="*" ControlToValidate="txtLiqPct6" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtLiqPct7" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="136"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="rfvtxtLiqPct7" runat="server" Text="*" ControlToValidate="txtLiqPct7" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtLiqPct8" runat="server" Width="75%" CssClass="textboxStyle" TabIndex="137"></asp:TextBox>
                                                            <asp:CustomValidator ID="valLiqPeriodPct" runat="server" ValidationGroup="valGrpSave" CssClass="requiredFieldValidator"
                                                                OnServerValidate="valLiqPeriodPct_ServerValidate" ToolTip="Please enter liquidation period % values summing to 100%"
                                                                ErrorMessage="*"></asp:CustomValidator>
                                                            <asp:RegularExpressionValidator ID="rfvtxtLiqPct8" runat="server" Text="*" ControlToValidate="txtLiqPct8" ValidationGroup="valGrpSave"
                                                                ValidationExpression="^100$|^\d{0,2}(\.\d{1,2})? *%?$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                                ToolTip="Please enter only positive number <= 100 upto 2 decimal places" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        </td>

                                                    </tr>
                                                </table>
                                            </td>
                                            <td></td>
                                        </tr>

                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <%--hidden fields for to compare data changes for each control--%>
            <asp:HiddenField ID="hdntxtRoyaltorId" runat="server" />
            <asp:HiddenField ID="hdntxtRoyaltorName" runat="server" />
            <asp:HiddenField ID="hdntxtRoyPLGId" runat="server" />
            <asp:HiddenField ID="hdntxtOwnerSearch" runat="server" />
            <asp:HiddenField ID="hdntxtExpiryDate" runat="server" />
            <asp:HiddenField ID="hdntxtStartDate" runat="server" />
            <asp:HiddenField ID="hdntxtRoyaltorSearch" runat="server" />
            <asp:HiddenField ID="hdntxtChargeablePct" runat="server" />
            <asp:HiddenField ID="hdntxtPrintGrp" runat="server" />
            <asp:HiddenField ID="hdntxtResvEndDate" runat="server" />
            <asp:HiddenField ID="hdntxtDefaultResvPct" runat="server" />
            <asp:HiddenField ID="hdntxtNettOrSalesUnits" runat="server" />
            <asp:HiddenField ID="hdnStatusCode" runat="server" />
            <asp:HiddenField ID="hdnddlCompany" runat="server" />
            <asp:HiddenField ID="hdnddlLabel" runat="server" />
            <asp:HiddenField ID="hdnddlResponsibility" runat="server" />
            <asp:HiddenField ID="hdnddlContractType" runat="server" />
            <asp:HiddenField ID="hdnddlStatementFormat" runat="server" />
            <asp:HiddenField ID="hdnddlReportingSch" runat="server" />
            <asp:HiddenField ID="hdnddlStmtPriority" runat="server" />
            <asp:HiddenField ID="hdnddlResvTakenOn" runat="server" />
            <asp:HiddenField ID="hdncbHeld" runat="server" />
            <asp:HiddenField ID="hdncbLock" runat="server" />
            <asp:HiddenField ID="hdncbSigned" runat="server" />
            <asp:HiddenField ID="hdncbSendToPortal" runat="server" />
            <asp:HiddenField ID="hdncbExcludeFromAccrual" runat="server" />
            <asp:HiddenField ID="hdnSocSecNo" runat="server" />
            <asp:HiddenField ID="hdncbDisplayZero" runat="server" />
            <asp:HiddenField ID="hdncbProducerSumm" runat="server" />
            <asp:HiddenField ID="hdncbCostSummary" runat="server" />
            <asp:HiddenField ID="hdntxtLiqPct1" runat="server" />
            <asp:HiddenField ID="hdntxtLiqPct2" runat="server" />
            <asp:HiddenField ID="hdntxtLiqPct3" runat="server" />
            <asp:HiddenField ID="hdntxtLiqPct4" runat="server" />
            <asp:HiddenField ID="hdntxtLiqPct5" runat="server" />
            <asp:HiddenField ID="hdntxtLiqPct6" runat="server" />
            <asp:HiddenField ID="hdntxtLiqPct7" runat="server" />
            <asp:HiddenField ID="hdntxtLiqPct8" runat="server" />
            <asp:HiddenField ID="hdnRoyaltorType" runat="server" Value="A" />
            </td>
                    <td width="5%"></td>
            <td width="15%" rowspan="11" valign="top" align="right">
                <table width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="30%"></td>
                                    <td width="70%">
                                        <asp:Button ID="btnSave" runat="server" CssClass="ButtonStyle" OnClientClick="if (!ValidatePopUpSave()) { return false;};" OnClick="btnSave_Click"
                                            Text="Save Changes" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpSave" TabIndex="138" />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnAudit" runat="server" CssClass="ButtonStyle" OnClick="btnAudit_Click"
                                            Text="Audit" UseSubmitBehavior="false" Width="90%" TabIndex="139" />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnCopyContractPopup" runat="server" CssClass="ButtonStyle"  OnClientClick="if (!ValidateCopy('CopyContract')) { return false;};"
                                            Text="Copy Contract" UseSubmitBehavior="false" Width="90%" TabIndex="140"
                                            OnClick="btnCopyContractPopup_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnLock" runat="server" CssClass="ButtonStyle"  OnClientClick="if (!ValidateLock('Lock')) { return false;};"
                                            Text="Lock" UseSubmitBehavior="false" Width="90%" TabIndex="141" onkeydown="OnTabPress();" OnClick="btnLock_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td width="30%"></td>
                                    <td width="70%">
                                        <ContNav:ContractNavigation ID="contractNavigationButtons" runat="server" />
                                    </td>
                                </tr>
                            </table>

                        </td>
                    </tr>

                </table>

            </td>
            </tr>
            </table>

            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>
            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black; z-index: 2">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <asp:Button ID="dummyFuzzySearch" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeFuzzySearch" runat="server" PopupControlID="pnlFuzzySearch" TargetControlID="dummyFuzzySearch"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlFuzzySearch" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable">Complete Search List
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseFuzzySearchPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>

                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ListBox ID="lbFuzzySearch" runat="server" Width="95%" CssClass="ListBox"
                                OnSelectedIndexChanged="lbFuzzySearch_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <%--Warning popup--%>
            <asp:Button ID="dummyConfirm" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmRespChange" runat="server" PopupControlID="pnlPopup" TargetControlID="dummyConfirm"
                CancelControlID="btnConfirmOk" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmMsgHdr" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirmMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnConfirmRespChangeCancel" runat="server" Text="Cancel" CssClass="ButtonStyle"
                                            OnClick="btnConfirmRespChangeCancel_Click" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnConfirmOk" runat="server" Text="OK" CssClass="ButtonStyle" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Button ID="dummyConfirmOwn" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeConfirmOwnerChange" runat="server" PopupControlID="pnlPopupOwn" TargetControlID="dummyConfirmOwn"
                CancelControlID="btnCancelOwnChange" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopupOwn" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="Label1" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblOwnChangeMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnCancelOwnChange" runat="server" Text="Cancel" CssClass="ButtonStyle" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnOkOwnChange" runat="server" Text="OK" CssClass="ButtonStyle" OnClick="btnOkOwnChange_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Button ID="dummyCopyContract" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCopyContract" runat="server" PopupControlID="pnlmpeCopyContract" TargetControlID="dummyCopyContract"
                CancelControlID="btnCloseCopyContPopup" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlmpeCopyContract" runat="server" align="center" Width="55%" ScrollBars="Auto" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table id="tblCopyContract" width="100%" runat="server">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Copy to new Contract
                                    </td>
                                    <td align="right" style="vertical-align: top;" width="10%">
                                        <asp:ImageButton ID="btnCloseCopyContPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClientClick="return CancelCopyContract();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="3%"></td>
                                    <td width="30%" class="identifierLable_large_bold" align="left">Royaltor Number</td>
                                    <td align="left">
                                        <asp:TextBox ID="txtRoyaltorIdCopyCont" runat="server" Width="35%" CssClass="textboxStyle" MaxLength="5"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtRoyaltorIdCopyCont" ControlToValidate="txtRoyaltorIdCopyCont" ValidationGroup="valGrpCopyContract"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter royaltor number" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revtxtRoyaltorIdCopyCont" runat="server" Text="*" ControlToValidate="txtRoyaltorIdCopyCont" ValidationGroup="valGrpCopyContract"
                                            ValidationExpression="^[1-9][0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red"
                                            ToolTip="Please enter only positive number" Display="Dynamic"> </asp:RegularExpressionValidator>
                                    </td>
                                </tr>

                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Royaltor Name</td>
                                    <td align="left">
                                        <asp:TextBox ID="txtRoyaltorNameCopyCont" runat="server" Width="95%" CssClass="textboxStyle" MaxLength="200"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rfvtxtRoyaltorNameCopyCont" ControlToValidate="txtRoyaltorNameCopyCont" ValidationGroup="valGrpCopyContract"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter royaltor name" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr style="height: 40px">
                                    <td></td>
                                    <td colspan="2" class="identifierLable_large_bold" align="left">Copy Options:</td>

                                </tr>
                                <tr>
                                    <td></td>
                                    <td colspan="2">
                                        <table width="95%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <table width="97.45%" cellpadding="0" cellspacing="0" align="left">
                                                        <tr>
                                                            <td width="30%" class="gridHeaderStyle_1row">Option Period</td>
                                                            <td width="5%" class="gridHeaderStyle_1row">
                                                                <asp:CheckBox ID="chkAllOptions" runat="server" onclick="CheckAllOptionPeriods();" /></td>
                                                            <td width="15%" class="gridHeaderStyle_1row">&nbsp;</td>
                                                            <td width="5%" class="gridHeaderStyle_1row">
                                                                <asp:CheckBox ID="chkAllRoyRates" runat="server" onclick="CheckAllRates('Roy');" /></td>
                                                            <td width="15%" class="gridHeaderStyle_1row">&nbsp;</td>
                                                            <td width="5%" class="gridHeaderStyle_1row">
                                                                <asp:CheckBox ID="chkAllSubRates" runat="server" onclick="CheckAllRates('Sub');" /></td>
                                                            <td width="15%" class="gridHeaderStyle_1row">&nbsp;</td>
                                                            <td width="5%" class="gridHeaderStyle_1row">
                                                                <asp:CheckBox ID="chkAllPackRates" runat="server" onclick="CheckAllRates('Pack');" />
                                                            </td>
                                                        </tr>
                                                        <tr id="trNoOptions" runat="server" visible="false">
                                                            <td colspan="8" class="gridItemStyle_Center_Align">
                                                                <asp:Label ID="lblEmpty" runat="server" Text="No Option Periods found" CssClass="gridEmptyDataRowStyle"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Panel ID="pnlOptionCopy" runat="server" ScrollBars="Auto" Width="100%">
                                                        <asp:GridView ID="gvOptionCopy" runat="server" AutoGenerateColumns="False" Width="97.45%"
                                                            CssClass="gridStyle" BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeader="false">
                                                            <%--<SelectedRowStyle BackColor="#99b8fa" Font-Bold="true" />--%>
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="" ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1rows">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblRowNo" runat="server" Text='<%# Container.DataItemIndex+1 %>' CssClass="identifierLable"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblOptionCode" runat="server" Text='<%# Bind("option_period_code") %>' CssClass="identifierLable" Visible="false"></asp:Label>
                                                                        <asp:Label ID="lblOptonDesc" runat="server" Text='<%# Bind("option_period_desc") %>' CssClass="identifierLable"></asp:Label>

                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="20%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkOptionCode" runat="server" CssClass="identifierLable" onclick="CheckRate(this,'Option');" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblRoyRates" runat="server" Text='<%# Bind("roy_rates") %>' CssClass="identifierLable"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkRoyRates" runat="server" CssClass="identifierLable" onclick="CheckRate(this,'Roy');" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblSubRates" runat="server" Text='<%# Bind("sub_rates") %>' CssClass="identifierLable"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSubRates" runat="server" CssClass="identifierLable" onclick="CheckRate(this,'Sub');" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblPackRates" runat="server" Text='<%# Bind("pack_rates") %>' CssClass="identifierLable"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkPackRates" runat="server" CssClass="identifierLable" onclick="CheckRate(this,'Pack');" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataRowStyle CssClass="gridEmptyDataRowStyle" />
                                                        </asp:GridView>
                                                    </asp:Panel>

                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td colspan="2">
                                        <table width="45%" cellspacing="0" cellpadding="0" style="margin-top: 5px" align="center" runat="server" id="tblEscRates">
                                            <tr>
                                                <td width="53%" class="gridItemStyle_Center_Align">
                                                    <asp:Label ID="lblEscRates" runat="server" Text="Escalation Rates" CssClass="identifierLable"></asp:Label>
                                                </td>

                                                <td width="8%" class="gridItemStyle_Center_Align">
                                                    <asp:CheckBox ID="chkEscRates" runat="server" onclick="CheckAllEscCodes();" /></td>
                                                <td width="39%" class="gridItemStyle_Center_Align">
                                                    <asp:Panel ID="pnlEscCodeCopy" runat="server" ScrollBars="Auto" Width="100%">
                                                        <asp:GridView ID="gvEscCodes" runat="server" AutoGenerateColumns="False" Width="85%" GridLines="None"
                                                            BackColor="White" EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" HorizontalAlign="Left" ShowHeaderWhenEmpty="True"
                                                            EmptyDataText="No Escalation rates for selected contract" ShowHeader="False">
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblEscProfileId" runat="server" Text='<%# Bind("esc_profile_id") %>' Visible="false" CssClass="identifierLable"></asp:Label>
                                                                        <asp:Label ID="lblEscCode" runat="server" Text='<%# Bind("esc_code") %>' CssClass="identifierLable"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="60%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkEscCode" runat="server" CssClass="identifierLable" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="40%" HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>

                                                    </asp:Panel>
                                                </td>

                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" align="center">
                                        <table width="50%" align="center" style="margin-top: 5px; margin-bottom: 5px;">
                                            <tr>
                                                <td width="35%">
                                                    <asp:Button ID="btnCopyAllContract" runat="server" CssClass="ButtonStyle" OnClick="btnCopyContract_Click" OnClientClick="if(!ValidatSelectedCopy(this.id)){return false;}"
                                                        Text="Copy All" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpCopyContract" />
                                                </td>
                                                <td width="35%">
                                                    <asp:Button ID="btnCopySelectedContract" runat="server" CssClass="ButtonStyle" ValidationGroup="valGrpCopyContract" OnClientClick="if(!ValidatSelectedCopy(this.id)){return false;}" OnClick="btnCopyContractSelected_Click"
                                                        Text="Copy Selected" UseSubmitBehavior="false" Width="90%" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnCancelCopyContract" runat="server" CssClass="ButtonStyle" OnClientClick="return CancelCopyContract();"
                                                        Text="Cancel" UseSubmitBehavior="false" Width="90%" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Button ID="dummyOwner" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeAddOwner" runat="server" PopupControlID="pnlPopupAdd" TargetControlID="dummyOwner"
                CancelControlID="btnAddOwnerCancel" BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPopupAdd" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr class="ScreenName">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="identifierLable" align="center">Add new Owner
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="3%"></td>
                                    <td width="30%" class="identifierLable_large_bold" align="left">Owner Code</td>
                                    <td align="left">
                                        <asp:TextBox ID="txtOwnerCode" runat="server" Width="60%" CssClass="textboxStyle" MaxLength="5"></asp:TextBox>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftetxtOwnerCode" runat="server"
                                            Enabled="True" TargetControlID="txtOwnerCode" FilterType="Numbers">
                                        </ajaxToolkit:FilteredTextBoxExtender>

                                        <asp:RequiredFieldValidator runat="server" ID="rfvOwnerCode" ControlToValidate="txtOwnerCode" ValidationGroup="valGrpOwnerDetails"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter owner code" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revOwnerCode" runat="server" Text="*" ControlToValidate="txtOwnerCode"
                                            ValidationExpression="^[0-9]\d*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valInsertOwner"
                                            ToolTip="Please enter only positive integer number" Display="Dynamic"> </asp:RegularExpressionValidator>

                                    </td>
                                </tr>

                                <tr>
                                    <td></td>
                                    <td class="identifierLable_large_bold" align="left">Description</td>
                                    <td align="left">
                                        <asp:TextBox ID="txtOwnerDesc" runat="server" Width="60%" Style="text-transform: uppercase" CssClass="textboxStyle" MaxLength="30"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ID="rvOwneDesc" ControlToValidate="txtOwnerDesc" ValidationGroup="valGrpOwnerDetails"
                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please enter owner description" Display="Dynamic"></asp:RequiredFieldValidator>

                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" align="center">
                                        <table width="30%">
                                            <tr>
                                                <td width="50%">
                                                    <asp:Button ID="btnAddOwnerSave" runat="server" CssClass="ButtonStyle" OnClick="btnSaveOwner_Click"
                                                        Text="Save" UseSubmitBehavior="false" Width="90%" ValidationGroup="valGrpOwnerDetails" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnAddOwnerCancel" runat="server" Text="Cancel" CssClass="ButtonStyle" UseSubmitBehavior="false" Width="90%" OnClientClick="return CancelOwnerClick();" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>


            <asp:Button ID="btnShowScreenLockDummy" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeShowScreenLockMsg" runat="server" PopupControlID="pnlShowScreenLockMsg" TargetControlID="btnShowScreenLockDummy"
                BackgroundCssClass="popupBox" CancelControlID="btnCloseShowScreenLockMsg">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlShowScreenLockMsg" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none"  onkeydown="CloseScreenLockMsgPopup();">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <table width="100%">
                                <tr>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseShowScreenLockMsg" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblScreenLockMessage" runat="server" CssClass="identifierLable" Text=""></asp:Label>
                        </td>
                    </tr>

                </table>
            </asp:Panel>

             <%--Warning on unsaved data popup--%>
            <asp:Button ID="dummyUnsavedWarnMsg" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeUnSavedWarning" runat="server" PopupControlID="pnlUnsavedWarnMsgPopup" TargetControlID="dummyUnsavedWarnMsg"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlUnsavedWarnMsgPopup" runat="server" align="center" Width="25%" CssClass="popupPanel" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblConfirmOnUnsavedData" runat="server" Text="Unsaved Data Warning" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblUnSavedWarnMsg" runat="server" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="48%" align="right">
                                        <asp:Button ID="btnUnSavedDataReturn" runat="server" Text="Return" CssClass="ButtonStyle" Width="30%" OnClientClick="return OnUnSavedDataReturn();" />
                                    </td>
                                    <td width="4%"></td>
                                    <td width="48%" align="left">
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="if (!OnUnSavedDataExit()) { return false;};"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnWindowHeight" runat="server" />
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnResponsibility" runat="server" Value="" />
            <asp:HiddenField ID="hdnOwner" runat="server" Value="" />
            <asp:HiddenField ID="hdnChangeNotSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnNewRoyaltorSaved" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsValidOwner" runat="server" Value="Y" />
            <asp:HiddenField ID="hdnInitialData" runat="server" />
            <asp:HiddenField ID="hdnUserRole" runat="server" />
            <asp:HiddenField ID="hdnIsStatusChange" runat="server" Value="N" />
            <asp:HiddenField ID="hdnCopySelected" runat="server" Value="N" />
            <asp:HiddenField ID="hdnIsAuditScreen" runat="server" Value="N" />
            <asp:HiddenField ID="hdnOtherUserScreenLocked" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:Button ID="btnCloseScreenLockPopup" runat="server" style="display:none;" OnClick="btnCloseScreenLockPopup_Click" CausesValidation="false" />
            <asp:TextBox ID="txtScreenLockMsgPopup" runat="server" Width="2" onkeydown="CloseScreenLockMsgPopup();" Visible="false"></asp:TextBox>
            <asp:TextBox ID="lblTab" runat="server" Text="" TabIndex="99" CssClass="gridTextField" onkeydown="FocusLblKeyPress();"></asp:TextBox>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
