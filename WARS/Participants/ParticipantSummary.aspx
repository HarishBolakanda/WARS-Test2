<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParticipantSummary.aspx.cs" Inherits="WARS.ParticipantSummary" MasterPageFile="~/MasterPage.Master"
    Title="WARS - Participant Summary" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

<asp:Content ID="LinkButtonContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderLinkButtons">
    <script type="text/javascript">

        function MissingParticipScreen() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Participants/MissingParticipants.aspx?isNewRequest=N');
            }
            else {
                window.location = '../Participants/MissingParticipants.aspx?isNewRequest=N';
            }

        }

        function ParticipantAuditScreen() {
            //debugger;
            var catNum = document.getElementById("<%=lblCatNo.ClientID %>").innerHTML;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Audit/ParticipantAudit.aspx?CatNo=" + catNum + "&pageName=" + "ParticipantSum" + "");
            }
            else {
                window.location = "../Audit/ParticipantAudit.aspx?CatNo=" + catNum + "&pageName=" + "ParticipantSum" + "";
            }

        }

        function OpenContractMaintenance() {
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Contract/RoyaltorSearch.aspx');
            }
            else {
                window.location = '../Contract/RoyaltorSearch.aspx';
            }
        }


        //================================End

    </script>
    <asp:UpdatePanel ID="updPnlLinkButtons" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 0; padding-left: 2px;">
                        <asp:Button ID="btnContractMaintenance" runat="server" Text="Contract Maintenance"
                            CssClass="LinkButtonStyle" Width="98%" OnClientClick="if (!OpenContractMaintenance()) { return false;};" UseSubmitBehavior="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnCatalogueSearch" runat="server" CssClass="LinkButtonStyle" Text="Catalogue Search" UseSubmitBehavior="false" Width="98%" OnClientClick="if (!CatalogueSearchScreen()) { return false;};" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <script type="text/javascript">


        function TrackListingScreen() {
            var catNum = document.getElementById("<%=lblCatNo.ClientID %>").innerHTML;
            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData("../Participants/TrackListing.aspx?CatNo=" + catNum);
            }
            else {
                window.location = "../Participants/TrackListing.aspx?CatNo=" + catNum;
            }

        }

        function CatalogueSearchScreen() {

            if (IsDataChanged()) {
                window.onbeforeunload = null;
                OpenPopupOnUnSavedData('../Participants/CatalogueSearch.aspx?isNewRequest=N');
            }
            else {
                window.location = '../Participants/CatalogueSearch.aspx?isNewRequest=N';
            }

        }
        //probress bar and scroll position functionality - starts
        //to remain scroll position of grid panel and window
        var xPos, yPos;
        var scrollTop;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var gridClientId = "ContentPlaceHolderBody_gvPartSummary_";
        var selectedRowIndex;

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


        //on page load        
        function SetGrdPnlHeightOnLoad() {
            //grid panel height adjustment functioanlity - starts
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("<%=PnlGrid.ClientID %>").style.height = gridPanelHeight + "px";
            document.getElementById("<%=hdnGridPnlHeight.ClientID %>").innerText = gridPanelHeight;
        }

        //Display control text on hover
        function DisplayDdlTextOnHover(control) {
            var ctrlId = control.id;
            var popUpText = control.options[control.selectedIndex].text;
            control.title = popUpText;
        }

        //Validations - Begin

        //Enable Esc Include Units of Escalation code is selected
        function OnChangeEscCodeInsert() {
            //debugger;
            var ddlEscCodeInsert = document.getElementById("<%=ddlEscCodeInsert.ClientID %>");
            var cbEscIncludeUnitsInsert = document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>");

            if (ddlEscCodeInsert.value == '-') {
                cbEscIncludeUnitsInsert.disabled = true;
            }
            else {
                cbEscIncludeUnitsInsert.disabled = false;
            }
        }

        function ValidateCatStatus() {
            //Validate Catalogue status update - cat no status cannot be greater than the lowest status of the participants
            //Valid options :			
            //PARTICIPATION_TYPE		PARTICIPATION.STATUS_CODE		CATNO.STATUS_CODE	
            //At least one Active		All active = 3		            1,2,3	
            //At least one Active		Active = 2, 3		            1 or 2	
            //At least one Active		Active = 1, 2, 3		        1	
            //None Active				                                0	            
            var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
            var selectedCode = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
            var gvPartSummary = document.getElementById("<%= gvPartSummary.ClientID %>");
            var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
            var atLeast1Active = false;
            var minOfPartStatus = 0;
            var hdnActive;
            var hdnPartStatusCode;

            if (gvPartSummary != null) {
                var gvRows = gvPartSummary.rows; // WUIN-746 grid view rows including header row
                var rowIndex;
                for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                    rowIndex = i - 1; //WUIN-746 row index start from 0

                    //handling empty data row
                    if (gvRows.length == 2 && document.getElementById(gridClientId + 'hdnParticipationId' + '_' + rowIndex) == null) {
                        break;
                    }

                    hdnActive = document.getElementById(gridClientId + 'hdnActive' + '_' + rowIndex).value;
                    hdnPartStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + rowIndex).value;

                    if (atLeast1Active != true && hdnActive == "Y") {
                        atLeast1Active = true;
                    }

                    if (i == 0) {
                        minOfPartStatus = hdnPartStatusCode;
                    }

                    if (parseInt(hdnPartStatusCode) < parseInt(minOfPartStatus)) {
                        minOfPartStatus = hdnPartStatusCode;
                    }
                }

            }

            if (atLeast1Active == false && selectedCode != "0") {
                DisplayMessagePopup("Status cannot be changed from No Participants when there are no active participants!");
                document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnCatStatusCode;
                return false;
            }
                //else if (atLeast1Active == true && parseInt(selectedCode) > parseInt(minOfPartStatus)) {
                //    DisplayMessagePopup("Catalogue status cannot be greater than minimum status of participants!");
                //    document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnCatStatusCode;
                //    return false;
                //}
            else {
                //If Status = 1  Allow update to 2        
                //If Status = 2  Allow update to 1,3        
                //If Status = 3  Allow update to 1,2  Display warning 'This update will prevent the generation of Statement details for all Participants'
                //JIRA - 983 Changes by Ravi on 20/02/2019 -- Start
                //WUIN-1167 - only super user/supervisor can change the catalogue status to Manager sign off
                if (((hdnUserRole != "SuperUser") && (hdnUserRole != "Supervisor")) && selectedCode == "3") {
                    DisplayMessagePopup("Only super user and Supervisor can change the status to 'Manager Sign Off'");
                    document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnCatStatusCode;
                    return false;
                }
                //JIRA - 983 Changes by Ravi on 20/02/2019 -- End

                if (hdnCatStatusCode == "1" && selectedCode != "2" && selectedCode != "1") {
                    DisplayMessagePopup("Status can only be changed from Under Review to Team Sign Off!");
                    document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnCatStatusCode;
                }
                else if (hdnCatStatusCode == "2" && (selectedCode != "1" && selectedCode != "3") && selectedCode != "2") {
                    DisplayMessagePopup("Status can only be changed from Team Sign Off to Under Review or Manager Sign Off!");
                    document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnCatStatusCode;
                }
                else if (hdnCatStatusCode == "3" && (selectedCode != "1" && selectedCode != "2") && selectedCode != "3") {
                    DisplayMessagePopup("Status can only be changed from Manager Sign Off to either Team Sign Off or Under Review!");
                    document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnCatStatusCode;
                }
                else if (hdnCatStatusCode == "3" && (selectedCode == "1" || selectedCode == "2") && selectedCode != "3") {
                    DisplayMessagePopup("This update will prevent the generation of Statement details for all Participants!");
                }
                else {
                    var participantValue;
                    var valueFound = false;
                    var str = "ContentPlaceHolderBody_gvPartSummary_";
                    if (gvPartSummary != null) {
                        var gvRows = gvPartSummary.rows; // WUIN-746 grid view rows including header row
                        var rowIndex;
                        for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                            rowIndex = i - 1; //WUIN-746 row index start from 0
                            if (document.getElementById(str + 'ddlStatus' + '_' + rowIndex) != null) {
                                participantValue = document.getElementById(str + 'ddlStatus' + '_' + rowIndex).value;
                                if (participantValue != selectedCode) {
                                    valueFound = true;
                                    break;
                                }
                            }
                        }
                        if (valueFound == true) {
                            //JIRA-908 CHanges --Start
                            var popup = $find('<%= mpeCatStatusPopup.ClientID %>');
                            if (popup != null) {
                                popup.show();
                            }
                        }
                        //JIRA-908 CHanges --End
                    }
                }
}

    return false;
}

function ValidatePartStatus(gridRow) {
    //WUIN-111
    //Update PARTICIPATION.STATUS_CODE - valid options						
    //validate only for active participants
    //PARTICIPATION_TYPE		PARTICIPATION.STATUS_CODE		CATNO.STATUS_CODE	
    //Active		            1	Allow update to 2	        1 or 2	
    //Active		            2	Allow update to 3	        2 or 3	
    //Active		            3	Allow update to 1 or 2	    not 3	
    var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
    var cbActive = document.getElementById(gridClientId + 'cbActive' + '_' + selectedRowIndex);
    var hdnUserRole = document.getElementById("<%=hdnUserRole.ClientID %>").value;
    var hdnPartStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
    var selectedCode = document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value;
    var ddlStatus = document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex);
    var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;

    //WUIN-1167 - only super user/supervisor can change the status to Manager sign off for any Participant(Active)
    if (((hdnUserRole != "SuperUser") && (hdnUserRole != "Supervisor")) && selectedCode == "3") {
        DisplayMessagePopup("Only super user and Supervisor can change the status to 'Manager Sign Off'!");
        ddlStatus.value = hdnPartStatusCode;
        return false;
    }
    if (cbActive.checked) {
        if (parseInt(selectedCode) < parseInt(hdnCatStatusCode)) {
            DisplayMessagePopup("Participants status cannot be less than status of catalogue!");
            ddlStatus.value = hdnPartStatusCode;
            return false;
        }
        if (hdnCatStatusCode == "1" || hdnCatStatusCode == "2") {
            if (hdnPartStatusCode == "1" && selectedCode != "2" && selectedCode != "1") {
                DisplayMessagePopup("Status can only be changed from Under Review to Team Sign Off!");
                ddlStatus.value = hdnPartStatusCode;
            }
        }
        else if (hdnCatStatusCode == "2" || hdnCatStatusCode == "3") {
            if (hdnPartStatusCode == "2" && selectedCode != "3" && selectedCode != "2") {
                DisplayMessagePopup("Status can only be changed from Team Sign Off to Manager Sign Off!");
                ddlStatus.value = hdnPartStatusCode;
            }
        }
        else if (hdnCatStatusCode != "3") {
            if (hdnPartStatusCode == "3" && (selectedCode != "1" && selectedCode != "2" && selectedCode != "3")) {
                DisplayMessagePopup("Status can only be changed from Manager Sign Off to Team Sign Off or Under Review!");
                ddlStatus.value = hdnPartStatusCode;
            }
        }

        return false;

    }
}

function ValidateSaveChanges() {
    var dupRowsCount = 0;
    var newPartAdded = "N";
    var partModified = "N";
    gvPartSummary = document.getElementById("<%= gvPartSummary.ClientID %>");
    if (gvPartSummary != null) {
        var gvRows = gvPartSummary.rows; // WUIN-746 grid view rows including header row
        var selectedRowIndex;
        for (var gvIndex = 1; gvIndex < gvRows.length; gvIndex++) { // WUIN-746 Looping only data rows
            selectedRowIndex = gvIndex - 1; //WUIN-746 row index start from 0
            var gridClientId = "ContentPlaceHolderBody_gvPartSummary_";
            hdnActive = document.getElementById(gridClientId + 'hdnActive' + '_' + selectedRowIndex).value;
            hdnPartStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
            cbActiveSelec = document.getElementById(gridClientId + 'cbActive' + '_' + selectedRowIndex);
            selectedCode = document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex).value;
            hdnParticipationIdSelec = document.getElementById(gridClientId + 'hdnParticipationId' + '_' + selectedRowIndex).value;
            hdnRoyaltorIdSelec = document.getElementById(gridClientId + 'hdnRoyaltorId' + '_' + selectedRowIndex).value;
            hdnOptPrtCodeSelec = document.getElementById(gridClientId + 'hdnOptPrtCode' + '_' + selectedRowIndex).value;
            hdnSellerGrpCodeSelec = document.getElementById(gridClientId + 'hdnSellerGrpCode' + '_' + selectedRowIndex).value;
            var isModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + selectedRowIndex);
            if (isModified.value != "-") {
                if ((selectedCode != hdnPartStatusCode) || ((hdnActive == "N" && cbActiveSelec.checked) || (hdnActive == "Y" && !cbActiveSelec.checked))) {
                    isModified.value = "Y";
                }
                else {
                    isModified.value = "N";
                }
            }

            if (isModified.value == "-") {
                newPartAdded = "Y";
            }
            else if (isModified.value == "Y") {
                partModified = "Y";
                //validation: a participant cannot be updated with same active status
                //CATNO, ROYALTOR_ID, OPTION_PERIOD_CODE, SELLER_GROUP_CODE, is_active
                gvPartSummary = document.getElementById("<%= gvPartSummary.ClientID %>");
                if (gvPartSummary != null) {
                    var gvRows = gvPartSummary.rows; // WUIN-746 grid view rows including header row
                    var rowIndex;
                    for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                        rowIndex = i - 1; //WUIN-746 row index start from 0

                        //handling empty data row
                        if (gvRows.length == 2 && document.getElementById(gridClientId + 'hdnActive' + '_' + rowIndex) == null) {
                            break;
                        }

                        hdnParticipationId = document.getElementById(gridClientId + 'hdnParticipationId' + '_' + rowIndex).value;
                        hdnRoyaltorId = document.getElementById(gridClientId + 'hdnRoyaltorId' + '_' + rowIndex).value;
                        hdnOptPrtCode = document.getElementById(gridClientId + 'hdnOptPrtCode' + '_' + rowIndex).value;
                        hdnSellerGrpCode = document.getElementById(gridClientId + 'hdnSellerGrpCode' + '_' + rowIndex).value;
                        cbActive = document.getElementById(gridClientId + 'cbActive' + '_' + rowIndex);

                        if (hdnParticipationIdSelec != hdnParticipationId && hdnRoyaltorIdSelec == hdnRoyaltorId && hdnOptPrtCodeSelec == hdnOptPrtCode &&
                            hdnSellerGrpCodeSelec == hdnSellerGrpCode && cbActiveSelec.checked == cbActive.checked) {
                            dupRowsCount += 1;
                            //replace the status checkbox with original status
                            if (hdnActive == "Y") {
                                cbActiveSelec.checked = true;
                            }
                            else {
                                cbActiveSelec.checked = false;
                            }

                        }

                    }

                }

            }
    }
    if (dupRowsCount > 0) {
        DisplayMessagePopup("Participant with status being updated is already present");
        return false;
    }
    else if (!(IsCatDataChanged()) && partModified == "N" && newPartAdded == "N") {
        DisplayMessagePopup("No changes made to save");
        return false;
    }
    else {
        return true;
    }
}
}

//===========Warn on changes made and not saved === Begin
function WarnOnUnSavedData() {
    var isExceptionRaised = document.getElementById("<%=hdnExceptionRaised.ClientID %>").value;
    if (isExceptionRaised != "Y") {
        if (IsCatDataChanged() || IsGridDataChanged() || IsAddRowDataChanged()) {
            return warningMsgOnUnSavedData;
        }
    }

}
window.onbeforeunload = WarnOnUnSavedData;

function IsCatDataChanged() {
    var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
    if (hdnCatStatusCode != "" && document.getElementById("<%=ddlCatStatus.ClientID %>").value != hdnCatStatusCode) {
        return true;
    }
    else {
        return false;
    }

}

function IsGridDataChanged() {
    var gvPartSummary = document.getElementById("<%= gvPartSummary.ClientID %>");
    if (gvPartSummary != null) {
        var gvRows = gvPartSummary.rows; // WUIN-746 grid view rows including header row
        var isModified;
        var isGridDataChanged = "N";
        var rowIndex;
        for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
            rowIndex = i - 1; //WUIN-746 row index start from 0
            if (document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex) != null) {
                isModified = document.getElementById(gridClientId + 'hdnIsModified' + '_' + rowIndex).value;
                if (isModified == "Y" || isModified == "-") {
                    isGridDataChanged = "Y";
                    break;

                }
            }
        }

        if (isGridDataChanged == "Y") {
            return true;
        }
        else {
            return false;
        }
    }

}

function IsDataChanged() {
    if (IsCatDataChanged() || IsGridDataChanged() || IsAddRowDataChanged()) { //JIRA-1070 Changes
        return true;
    }
    else {
        return false;
    }
}

function RedirectToErrorPage() {
    document.getElementById("<%=hdnExceptionRaised.ClientID %>").innerText = "Y";
    window.location = "../Common/ExceptionPage.aspx";
}

function DisplayActive(button) {
    //debugger;
    if (document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value == "Y") {
        return true;
    }
    document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "N";
    if (IsDataChanged()) {
        OpenOnUnSavedData();
        document.getElementById("<%=hdnButtonSelection.ClientID %>").value = button;
    }
    else {
        return true;
    }
}

// WUIN-662 - confirmation on un saved data
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
            document.getElementById('<%=hdnIsConfirmPopup.ClientID%>').value = "Y";
            window.onbeforeunload = WarnOnUnSavedData;
        }
        //============== End
        //===========Warn on changes made and not saved === End

        //==========Validations - End

        //Tab key to remain only on screen fields
        function OnTabPress() {
            if (event.keyCode == 9) {
                document.getElementById("<%= lblTab.ClientID %>").focus();
        }
    }

    function FocusLblKeyPress() {
        document.getElementById("<%= ddlCatStatus.ClientID %>").focus();
    }

    //=============== End

    //Undo button functionality - Begins        
    function UndoCatNoChanges() {
        var hdnStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
        document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnStatusCode;
        return false;
    }

    function UndoParticipChanges(gridRow) {
        var selectedRowIndex = gridRow.id.substring(gridRow.id.lastIndexOf("_") + 1);
        hdnActive = document.getElementById(gridClientId + 'hdnActive' + '_' + selectedRowIndex).value;
        hdnPartStatusCode = document.getElementById(gridClientId + 'hdnStatusCode' + '_' + selectedRowIndex).value;
        cbActive = document.getElementById(gridClientId + 'cbActive' + '_' + selectedRowIndex);
        ddlStatus = document.getElementById(gridClientId + 'ddlStatus' + '_' + selectedRowIndex);
        ddlStatus.value = hdnPartStatusCode;
        if (hdnActive == "Y") {
            cbActive.checked = true;
        }
        else {
            cbActive.checked = false;
        }
        return false;
    }

    //===========Undo button functionality - End

    function UpdateStatusConfirmYes() {
        var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
        var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
        var str = "ContentPlaceHolderBody_gvPartSummary_";
        var gvPartSummary = document.getElementById("<%= gvPartSummary.ClientID %>");
        if (gvPartSummary != null) {
            var gvRows = gvPartSummary.rows;  // WUIN-746 grid view rows including header row
            var rowIndex;
            for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                rowIndex = i - 1; //WUIN-746 row index start from 0
                document.getElementById(str + 'ddlStatus' + '_' + rowIndex).value = ddlCatStatus;
                if (document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value != "-") {
                    document.getElementById(str + 'hdnIsModified' + '_' + rowIndex).value = "Y";

                }

            }
        }
    }

    function UpdateStatusConfirmNo() {
        var minPartStatusCode = 0;
        var ddlCatStatus = document.getElementById("<%=ddlCatStatus.ClientID %>").value;
        var hdnCatStatusCode = document.getElementById("<%=hdnCatStatusCode.ClientID %>").value;
        var str = "ContentPlaceHolderBody_gvPartSummary_";
        var gvPartSummary = document.getElementById("<%= gvPartSummary.ClientID %>");
        if (gvPartSummary != null) {
            var gvRows = gvPartSummary.rows; // WUIN-746 grid view rows including header row
            var rowIndex;
            for (var i = 1; i < gvRows.length; i++) { // WUIN-746 Looping only data rows
                rowIndex = i - 1; //WUIN-746 row index start from 0
                var hdnParticipStatus = document.getElementById(str + 'hdnParticipStatus' + '_' + rowIndex).value;
                if (i == 0) {
                    minPartStatusCode = hdnParticipStatus;
                }
                else if (hdnParticipStatus < minPartStatusCode) {
                    minPartStatusCode = hdnParticipStatus;
                }
            }
        }
        if (parseInt(ddlCatStatus) > minPartStatusCode) {
            document.getElementById("<%=ddlCatStatus.ClientID %>").value = hdnCatStatusCode;
            DisplayMessagePopup("Catalogue status cannot be greater than minimum status of participants!");

        }

        var mpeCatStatusPopup = $find('<%= mpeCatStatusPopup.ClientID %>');
        if (mpeCatStatusPopup != null) {
            mpeCatStatusPopup.hide();
        }

        return false;
    }




    //JIRA-1070 Changes by Ravi -- Start
    //Royaltor Add row fuzzy search -- Start

    function royaltorListPopulatingInsert() {
        txtRoyaltorInsert = document.getElementById("<%= txtRoyaltorInsert.ClientID %>");
        txtRoyaltorInsert.style.backgroundImage = 'url(Images/textbox_loader.gif)';
        txtRoyaltorInsert.style.backgroundRepeat = 'no-repeat';
        txtRoyaltorInsert.style.backgroundPosition = 'right';
    }

    function royaltorListPopulatedInsert() {
        txtRoyaltorInsert = document.getElementById("<%= txtRoyaltorInsert.ClientID %>");
        txtRoyaltorInsert.style.backgroundImage = 'none';
        var ddlOptionPeriod = document.getElementById("<%= ddlOptionPeriodInsert.ClientID %>");
        ddlOptionPeriod.innerHTML = "";

        var listItem1 = document.createElement('option');
        listItem1.text = listItem1.value = "-";
        ddlOptionPeriod.add(listItem1);
    }

    function royaltorListItemSelectedInsert(sender, args) {
        var srchVal = args.get_value();
        if (srchVal == 'No results found') {
            document.getElementById("<%= txtRoyaltorInsert.ClientID %>").value = "";
        }
        else {
            document.getElementById('<%=btnHdnRoyaltorInsertSearch.ClientID%>').click();
        }
    }

    //Pop up fuzzy search list       
    function OntxtRoyaltorInsertKeyDown() {
        if ((event.keyCode == 13)) {
            //Enter key can be used to select the dropdown list item or to pop up the complete list
            //to know this, check if list item is selected or not
            var aceRoyaltorAddRow = $find('ContentPlaceHolderBody_' + 'aceRoyaltorAddRow');
            if (aceRoyaltorAddRow._selectIndex == -1) {
                txtRoyaltorInsert = document.getElementById("<%= txtRoyaltorInsert.ClientID %>").value;
                document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtRoyaltorInsert;
                document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "RoyaltorAddRow";
                document.getElementById('<%=btnHdnRoyaltorInsertSearch.ClientID%>').click();
            }
        }
    }

    function OnRoyFuzzySearchChangeInsert() {
        //debugger;
        var royaltorSearchText = document.getElementById("<%= txtRoyaltorInsert.ClientID %>").value;
        var ddlOptionPeriod = document.getElementById("<%= ddlOptionPeriodInsert.ClientID %>");
        ddlOptionPeriod.innerHTML = "";

        var listItem1 = document.createElement('option');
        listItem1.text = listItem1.value = "-";
        ddlOptionPeriod.add(listItem1);

        var ddlEscCodeInsert = document.getElementById("<%= ddlEscCodeInsert.ClientID %>");
        ddlEscCodeInsert.innerHTML = "";

        var listItem2 = document.createElement('option');
        listItem2.text = listItem2.value = "-";
        ddlEscCodeInsert.add(listItem2);

        OntxtRoyaltorAddRowChange();

        return false;
    }

    //reset field width when empty
    function OntxtRoyaltorAddRowChange() {
        txtRoyaltorInsert = document.getElementById("<%=txtRoyaltorInsert.ClientID %>");
            if (txtRoyaltorInsert.value == "") {
                txtRoyaltorInsert.style["width"] = '90%';
            }
        }

        //Validate if the field value is a valid one from fuzzy search list
        function ValRoyaltorAddRow(sender, args) {
            txtRoyaltorInsert = document.getElementById("<%=txtRoyaltorInsert.ClientID %>");
        if (txtRoyaltorInsert.value == "") {
            args.IsValid = true;
            txtRoyaltorInsert.style["width"] = '90%';
        }
        else if (txtRoyaltorInsert.value == "No results found") {
            args.IsValid = true;
            txtRoyaltorInsert.value = "";
            txtRoyaltorInsert.style["width"] = '90%';
        }
        else if (txtRoyaltorInsert.value != "" && txtRoyaltorInsert.value.indexOf('-') == -1) {
            args.IsValid = false;
            //adjust width of the textbox to display error
            fieldWidth = txtRoyaltorInsert.offsetWidth;
            txtRoyaltorInsert.style["width"] = (fieldWidth - 15);
        }
        else if (args.IsValid == true) {
            txtRoyaltorInsert.style["width"] = '90%';
        }

    }
    //Royaltor Add row fuzzy search ====== End ==

    //Territory Add row fuzzy search -- Start

    function territoryAddRowListPopulating() {
        txtTerritoryInsert = document.getElementById("<%= txtTerritoryInsert.ClientID %>");
        txtTerritoryInsert.style.backgroundImage = 'url(Images/textbox_loader.gif)';
        txtTerritoryInsert.style.backgroundRepeat = 'no-repeat';
        txtTerritoryInsert.style.backgroundPosition = 'right';

    }

    function territoryAddRowListPopulated() {
        txtTerritoryInsert = document.getElementById("<%= txtTerritoryInsert.ClientID %>");
            txtTerritoryInsert.style.backgroundImage = 'none';
        }

        function territoryAddRowListHidden() {
            txtTerritoryInsert = document.getElementById("<%= txtTerritoryInsert.ClientID %>");
        txtTerritoryInsert.style.backgroundImage = 'none';

    }

    function territoryAddRowListItemSelected(sender, args) {
        var roySrchVal = args.get_value();
        if (roySrchVal == 'No results found') {
            txtTerritoryInsert = document.getElementById("<%= txtTerritoryInsert.ClientID %>");
            txtTerritoryInsert.value = "";
        }
    }

    //Pop up fuzzy search list       
    function OntxtTerritoryAddRowKeyDown() {
        if ((event.keyCode == 13)) {
            //Enter key can be used to select the dropdown list item or to pop up the complete list
            //to know this, check if list item is selected or not
            var aceTerritoryAddRow = $find('ContentPlaceHolderBody_' + 'aceTerritoryAddRow');
            if (aceTerritoryAddRow._selectIndex == -1) {
                txtTerritoryInsert = document.getElementById("<%= txtTerritoryInsert.ClientID %>").value;
                document.getElementById("<%=hdnFuzzySearchText.ClientID %>").innerText = txtTerritoryInsert;
                document.getElementById("<%=hdnFuzzySearchField.ClientID %>").innerText = "TerritoryAddRow";
                document.getElementById('<%=btnFuzzyTerritoryListPopup.ClientID%>').click();
            }
        }
    }

    //reset field width when empty
    function OntxtTerritoryAddRowChange() {
        txtTerritoryInsert = document.getElementById("<%=txtTerritoryInsert.ClientID %>");
        if (txtTerritoryInsert.value == "") {
            txtTerritoryInsert.style["width"] = '98%';
        }
    }

    //Validate if the field value is a valid one from fuzzy search list
    function ValTerritoryAddRow(sender, args) {
        txtTerritoryInsert = document.getElementById("<%=txtTerritoryInsert.ClientID %>");
        if (txtTerritoryInsert.value == "") {
            args.IsValid = true;
            txtTerritoryInsert.style["width"] = '98%';
        }
        else if (txtTerritoryInsert.value == "No results found") {
            args.IsValid = true;
            txtTerritoryInsert.value = "";
            txtTerritoryInsert.style["width"] = '98%';
        }
        else if (txtTerritoryInsert.value != "" && txtTerritoryInsert.value.indexOf('-') == -1) {
            args.IsValid = false;
            //adjust width of the textbox to display error
            fieldWidth = txtTerritoryInsert.offsetWidth;
            txtTerritoryInsert.style["width"] = (fieldWidth - 10);
        }
        else if (args.IsValid == true) {
            txtTerritoryInsert.style["width"] = '98%';
        }

    }

    //Territory Add row fuzzy search ======= End ====
    //Pop up interested party list       
    function OnTrackNoInsert() {
        //debugger;
        var txtTrackNoInsert = document.getElementById("<%= txtTrackNoInsert.ClientID %>").value;
        var txtShareInsert = document.getElementById("<%=txtShareInsert.ClientID %>").value;
        var txtTotalShareInsert = document.getElementById("<%=txtTotalShareInsert.ClientID %>").value;
        var hdnTrackNoInsertTemp = document.getElementById("<%=hdnTrackNoInsertTemp.ClientID %>").value;

        if (txtShareInsert == 1 && txtTotalShareInsert > 1) {
            if ((txtTrackNoInsert != hdnTrackNoInsertTemp) && txtTrackNoInsert != "") {
                if (event.keyCode == 13 || event.keyCode == 9) {
                    document.getElementById("<%=hdnTrackNoInsertTemp.ClientID %>").value = txtTrackNoInsert;
                    window.onbeforeunload = null;
                    __doPostBack("txtTrackNoInsert", "txtTrackNoInsert_OnTextChanged");
                }
            }
        }

    }

    function ClearTrackTitleAddRow() {
        var txtTrackNoInsert = document.getElementById("<%= txtTrackNoInsert.ClientID %>").value;
        var txtTrackTitleInsert = document.getElementById("<%= txtTrackTitleInsert.ClientID %>").value;
        var hdnTrackNoInsertTemp = document.getElementById("<%= hdnTrackNoInsertTemp.ClientID %>").value;

        if (txtTrackNoInsert != hdnTrackNoInsertTemp) {
            document.getElementById("<%= txtTrackTitleInsert.ClientID %>").value = "";
        }
    }

    //prevent page navigation to previous page on back space key press when track title is readonly.
    function OnTrackTitleInsertBackSapce() {
        var txtTrackTitleInsert = document.getElementById("<%= txtTrackTitleInsert.ClientID %>");
        if (txtTrackTitleInsert.readOnly && event.keyCode == 8) {
            event.returnValue = false;
        }
    }



    function CloseShowNoTrackMsgPopup() {
        var popup = $find('<%= mpeShowNoTrackMsg.ClientID %>');
        if (popup != null) {
            popup.hide();
        }
        document.getElementById("<%=txtTrackNoInsert.ClientID %>").focus();
        return false;
    }




    function OnFocusOutShareInsert() {
        //debugger;
        var txtShareInsert = document.getElementById("<%=txtShareInsert.ClientID %>").value;
        var txtTotalShareInsert = document.getElementById("<%=txtTotalShareInsert.ClientID %>").value;
        var txtTrackTitleInsert = document.getElementById("<%=txtTrackTitleInsert.ClientID %>");
        var txtTrackNoInsert = document.getElementById("<%=txtTrackNoInsert.ClientID %>");

        if (txtShareInsert == 1 && txtTotalShareInsert > 1) {
            txtTrackNoInsert.disabled = false;
            txtTrackTitleInsert.disabled = false;
            txtTrackTitleInsert.readOnly = true;
            txtTrackNoInsert.value = "";
            txtTrackTitleInsert.value = "";
            txtTrackNoInsert.focus();
            document.getElementById("<%=hdnTrackNoInsertTemp.ClientID %>").value = "";
            ValidatorValidate(document.getElementById("<%=revTrackNoInsert.ClientID %>"));

        }
        else if (txtShareInsert >= 1 && txtTotalShareInsert >= 1) {
            txtTrackNoInsert.value = "";
            txtTrackTitleInsert.value = "";
            txtTrackTitleInsert.readOnly = false;
            txtTrackNoInsert.focus();
            ValidatorValidate(document.getElementById("<%=revTrackNoInsert.ClientID %>"));
            document.getElementById("<%=hdnTrackNoInsertTemp.ClientID %>").value = "";
            document.getElementById("<%=hdnTrackTitlefromISRC.ClientID %>").value = "N";
            document.getElementById("<%=hdnISRC.ClientID %>").value = "";
            document.getElementById("<%=hdnTrackListingID.ClientID %>").value = "";
        }
}



//=========End

//validations ========== Begin

function ValidatePopUpAddRow() {
    //warning on add row validation fail
    if (!Page_ClientValidate("valAddRow")) {
        Page_BlockSubmit = false;
        DisplayMessagePopup("Invalid or missing data!");
        return false;
    }
    else {
        return true;
    }
}

//clear add row data
function ClearAddRow() {
    document.getElementById('<%=txtRoyaltorInsert.ClientID%>').value = "";

    document.getElementById('<%=ddlOptionPeriodInsert.ClientID%>').innerHTML = "";
    var listItem1 = document.createElement('option');
    listItem1.text = listItem1.value = "";
    document.getElementById('<%=ddlOptionPeriodInsert.ClientID%>').add(listItem1);

    var ddlEscCodeInsert = document.getElementById("<%= ddlEscCodeInsert.ClientID %>");
    ddlEscCodeInsert.innerHTML = "";

    var listItem2 = document.createElement('option');
    listItem2.text = listItem2.value = "";
    ddlEscCodeInsert.add(listItem2);

    document.getElementById('<%=txtTerritoryInsert.ClientID%>').value = "";
    document.getElementById('<%=txtShareInsert.ClientID%>').value = document.getElementById('<%=hdnShareInsert.ClientID%>').value;
    document.getElementById('<%=txtTotalShareInsert.ClientID%>').value = document.getElementById('<%=hdnTotalShareInsert.ClientID%>').value;;
    document.getElementById('<%=txtTimeInsert.ClientID%>').value = '___:__:__';
    document.getElementById('<%=txtTotalTimeInsert.ClientID%>').value = '___:__:__';
    document.getElementById('<%=txtTrackTitleInsert.ClientID%>').value = "";
    document.getElementById('<%=txtTrackNoInsert.ClientID%>').value = "";
    document.getElementById('<%=ddlEscCodeInsert.ClientID%>').selectedIndex = 0;
    document.getElementById('<%=cbEscIncludeUnitsInsert.ClientID%>').checked = false;
    document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>").disabled = false;
    Page_ClientValidate('');//clear all validators of the page
    document.getElementById("<%= txtRoyaltorInsert.ClientID %>").focus();
    return false;
}

function IsAddRowDataChanged() {
    //debugger;           
    var txtRoyaltorInsert = document.getElementById("<%=txtRoyaltorInsert.ClientID %>").value;
    var ddlOptionPeriodInsert = document.getElementById("<%=ddlOptionPeriodInsert.ClientID %>").value;
    var txtTerritoryAddRow = document.getElementById("<%=txtTerritoryInsert.ClientID %>").value;
    var txtShareInsert = document.getElementById("<%=txtShareInsert.ClientID %>").value;
    var txtTotalShareInsert = document.getElementById("<%=txtTotalShareInsert.ClientID %>").value;
    var txtTimeInsert = document.getElementById("<%=txtTimeInsert.ClientID %>").value;
    var txtTotalTimeInsert = document.getElementById("<%=txtTotalTimeInsert.ClientID %>").value;
    var txtTrackTitleInsert = document.getElementById("<%=txtTrackTitleInsert.ClientID %>").value;
    var ddlEscCodeInsert = document.getElementById("<%=ddlEscCodeInsert.ClientID %>").value;

    var escIncludeUnitsInsert;
    var cbEscIncludeUnitsInsert = document.getElementById("<%=cbEscIncludeUnitsInsert.ClientID %>");
    if (cbEscIncludeUnitsInsert.checked == true) {
        escIncludeUnitsInsert = 'Y';
    }
    else {
        escIncludeUnitsInsert = 'N';
    }

    var hdnShareInsert = document.getElementById("<%=hdnShareInsert.ClientID %>").value;
    var hdnTotalShareInsert = document.getElementById("<%=hdnTotalShareInsert.ClientID %>").value;

    if (txtRoyaltorInsert != '' || ddlOptionPeriodInsert != '' || txtTerritoryAddRow != '' ||
        txtShareInsert != hdnShareInsert || txtTotalShareInsert != hdnTotalShareInsert || txtTimeInsert != '___:__:__' || txtTotalTimeInsert != '___:__:__'
        || txtTrackTitleInsert != '' || ddlEscCodeInsert != '' || escIncludeUnitsInsert != 'N') {
        return true;
    }
    else {
        return false;
    }
}
//Time on each Participant must not be > Total Time of that Participant
function ValTimeInsertRow(args) {
    txtTime = document.getElementById("<%= txtTimeInsert.ClientID %>").value;
    txtTotalTime = document.getElementById("<%= txtTotalTimeInsert.ClientID %>").value;

    args.IsValid = true;
    if (CompareTimes(txtTime, txtTotalTime)) {
        args.IsValid = false;
    }
}
// compares two time values.
// if time1 > time2 then returned true
function CompareTimes(time1, time2) {
    var time1Array = time1.split(":");
    var time2Array = time2.split(":");

    time1Value = Number(time1Array[0]) * 3600 + Number(time1Array[1]) * 60 + Number(time1Array[2]);
    time2Value = Number(time2Array[0]) * 3600 + Number(time2Array[1]) * 60 + Number(time2Array[2]);

    if (time1Value > time2Value) {
        return true;
    }
    else if (time1Value == '0' && time2Value == '0') {
        return true;
    }
    else if ((time1 == '___:__:__' || time2 == '___:__:__')) {
        return true;
    }
    else {
        return false;
    }
}
//============== End
//JIRA-1070 Changes by Ravi -- End

//WUIN-1181 changes
function imgBtnSaveKeydown() {
    if ((event.keyCode == 13)) {
        document.getElementById('<%=imgBtnSave.ClientID%>').click();
    }
}
    </script>
    <asp:UpdatePanel ID="updPnlPageLevel" runat="server">
        <ContentTemplate>
            <table id="tblMain" width="100%">
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ScreenName">
                            <div style="padding: 5px; cursor: none; vertical-align: middle;">
                                <div style="float: left;">
                                    PARTICIPANT SUMMARY
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td valign="top">
                        <table width="100%" class="table_with_border">
                            <tr>
                                <td style="padding: 10px">
                                    <table width="99.9%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="10%" class="gridHeaderStyle_1row">Catalogue No</td>
                                            <td width="20%" class="gridHeaderStyle_1row">Title</td>
                                            <td width="15%" class="gridHeaderStyle_1row">Artist</td>
                                            <td width="12%" class="gridHeaderStyle_1row">Deal Type</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Compilation?</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Total Tracks</td>
                                            <td width="6%" class="gridHeaderStyle_1row">Total Time</td>
                                            <td width="10%" class="gridHeaderStyle_1row">Track / Time Share</td>
                                            <td width="12%" class="gridHeaderStyle_1row">Status</td>
                                            <td width="3%" class="gridHeaderStyle_1row">&nbsp</td>
                                        </tr>
                                        <tr>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblCatNo" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblCatTitle" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblCatArtist" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle">
                                                <asp:Label ID="lblCatDealType" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:CheckBox ID="cbCatCompilation" runat="server" TabIndex="103" Enabled="false" />
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblCatTotalTracks" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblCatTotalTime" runat="server" CssClass="identifierLable" Width="97%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" align="center">
                                                <asp:Label ID="lblCatTrackTimeShare" runat="server" CssClass="identifierLable" Width="95%"></asp:Label>
                                            </td>
                                            <td class="insertBoxStyle" style="padding-top: 3px; padding-bottom: 3px;">
                                                <asp:DropDownList ID="ddlCatStatus" runat="server" Width="98%" CssClass="ddlStyle" TabIndex="101" onchange="ValidateCatStatus();">
                                                </asp:DropDownList>
                                            </td>

                                            <td class="insertBoxStyle" align="center">
                                                <asp:ImageButton ID="btnUndoSaveCat" runat="server" ImageUrl="../Images/cancel_row3.png"
                                                    ToolTip="Undo Changes" CausesValidation="false" TabIndex="102" OnClientClick="return UndoCatNoChanges();" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <asp:HiddenField ID="hdnCatStatusCode" runat="server" />
                        </table>
                    </td>
                    <td width="11%" rowspan="1" valign="top" align="right">
                        <table width="100%">
                            <td width="5%"></td>
                            <td align="right" width="70%">
                                <asp:Button ID="btnSaveAllChanges" runat="server" CssClass="ButtonStyle" OnClick="btnSaveAllChanges_Click" OnClientClick="if (!ValidateSaveChanges()) { return false;};"
                                    Text="Save Changes" UseSubmitBehavior="false" Width="90%" TabIndex="103" />
                            </td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button ID="btnTrackListing" runat="server" CssClass="ButtonStyle"
                            Text="Track Listing" UseSubmitBehavior="false" Width="90%" TabIndex="104"
                            OnClientClick="if (!TrackListingScreen()) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button ID="btnMissingParticip" TabIndex="105" runat="server" CssClass="ButtonStyle" Text="Missing Participants" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!MissingParticipScreen()) { return false;};" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button ID="btnParticipantAudit" TabIndex="106" runat="server" CssClass="ButtonStyle" Text="Audit" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!ParticipantAuditScreen()) { return false;};" />
                    </td>
                </tr>
                <%--JIRA-1049 Changes by Ravi on 24/06/2019 -- Start--%>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button ID="btnCorrectMismatches" TabIndex="107" runat="server" CssClass="ButtonStyle" Text="Correct Mismatches" UseSubmitBehavior="false" Width="90%" OnClick="btnCorrectMismatches_Click" />
                    </td>
                </tr>
                <%--JIRA-1049 Changes by Ravi on 24/06/2019 -- End--%>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnDisplayActive" runat="server" CssClass="ButtonStyle" OnClientClick="if (!DisplayActive('DisplayActive')) { return false;};" OnClick="btnDisplayActive_Click"
                            Text="Display Active" UseSubmitBehavior="false" Width="90%" TabIndex="108" OnKeyDown="OnTabPress()" />
                    </td>
                </tr>
            </table>
            </td>
                </tr>
                <tr>
                    <td class="table_header_with_border" valign="top" colspan="2" style="padding-left: 15px">Participant Summary</td>
                </tr>
            <tr>
                <td colspan="2">
                    <table width="100%" class="table_with_border" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <%--JIRA-953 Changes by Ravi -- Start--%>
                            <td style="padding-left: 10px; padding-bottom: 10px">
                                <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto" Width="100%">
                                    <asp:GridView ID="gvPartSummary" runat="server" AutoGenerateColumns="False" Width="98.75%" AlternatingRowStyle-BackColor="#E3EFFF"
                                        CssClass="gridStyle" BackColor="White" HorizontalAlign="Left" ShowHeaderWhenEmpty="true" EmptyDataText="No Data Found"
                                        EmptyDataRowStyle-CssClass="gridEmptyDataRowStyle" OnRowDataBound="gvPartSummary_RowDataBound" AllowSorting="true" OnSorting="gvPartSummary_Sorting" HeaderStyle-CssClass="FixedHeader">
                                        <Columns>
                                            <asp:TemplateField ItemStyle-Width="17%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Royaltor" SortExpression="royaltor">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnParticipId" runat="server" Value='<%# Bind("participation_id") %>' />
                                                    <asp:HiddenField ID="hdnParticipRoyId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                    <asp:Label ID="lblRoyaltor" runat="server" Text='<%#Bind("royaltor")%>' CssClass="identifierLable" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Option Period" SortExpression="option_period">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnParticipOptionPeriod" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                    <asp:Label ID="lblOptionPeriod" runat="server" Text='<%#Bind("option_period")%>' CssClass="identifierLable" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="14%" ItemStyle-CssClass="gridItemStyle_Left_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Territory" SortExpression="territory">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnSellerGrp" runat="server" Value='<%# Bind("seller_group_code") %>' />
                                                    <asp:Label ID="lblTerritory" runat="server" Text='<%#Bind("territory")%>' CssClass="identifierLable" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Share" SortExpression="share_tracks">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnShare" runat="server" Value='<%# Bind("share_tracks") %>' />
                                                    <asp:Label ID="lblShare" runat="server" Text='<%#Bind("share_tracks")%>' CssClass="identifierLable" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="3%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Total Share" SortExpression="share_total_tracks">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnTotalShare" runat="server" Value='<%# Bind("share_total_tracks") %>' />
                                                    <asp:Label ID="lblTotalShare" runat="server" Text='<%#Bind("share_total_tracks")%>' CssClass="identifierLable" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Time" SortExpression="share_time">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnTime" runat="server" Value='<%# Bind("share_time") %>' />
                                                    <asp:Label ID="lblTime" runat="server" Text='<%#Bind("share_time")%>' CssClass="identifierLable" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Total Time" SortExpression="share_total_time">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnTotalTime" runat="server" Value='<%# Bind("share_total_time") %>' />
                                                    <asp:Label ID="lblTotalTime" runat="server" Text='<%#Bind("share_total_time")%>' CssClass="identifierLable" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="14%" ItemStyle-CssClass="gridItemStyle_Left_Align_No_Padding" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Track Title" SortExpression="track_title">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnTrackTitle" runat="server" Value='<%# Bind("track_title") %>' />
                                                    <asp:Label ID="lblTrackTitle" runat="server" Text='<%#Bind("track_title")%>' CssClass="identifierLable" />
                                                    <asp:HiddenField ID="hdnISRC" runat="server" Value='<%# Bind("isrc") %>' />
                                                    <asp:HiddenField ID="hdnTrackNo" runat="server" Value='<%# Bind("tune_code") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Escalation Code" SortExpression="esc_code">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnEscalationCode" runat="server" Value='<%# Bind("esc_code") %>' />
                                                    <asp:Label ID="lblEscCode" runat="server" Text='<%#Bind("esc_code")%>' CssClass="identifierLable" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Esc Include Units" SortExpression="inc_in_escalation">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnEscIncludeUnits" runat="server" Value='<%# Bind("inc_in_escalation") %>' />
                                                    <asp:CheckBox ID="cbEscIncludeUnits" runat="server" Enabled="false" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Active?" SortExpression="participation_type">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnParticipActive" runat="server" Value='<%# Bind("participation_type") %>' />
                                                    <asp:CheckBox ID="cbActive" runat="server"/>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="4%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Override" SortExpression="override">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnParticipantOverride" runat="server" Value='<%# Bind("override") %>' />
                                                    <asp:CheckBox ID="cbOverride" runat="server" Enabled="false" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="10%" ItemStyle-CssClass="gridItemStyle_Center_Align" HeaderStyle-CssClass="gridHeaderStyle_1row" HeaderText="Status" SortExpression="status_code">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnParticipStatus" runat="server" Value='<%# Bind("status_code") %>' />
                                                    <asp:DropDownList ID="ddlStatus" runat="server" Width="95%" CssClass="ddlStyle"
                                                        onmouseover="DisplayDdlTextOnHover(this);" onchange="ValidatePartStatus(this);">
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-CssClass="gridItemStyle_Center_Align" ItemStyle-Width="3%" HeaderStyle-CssClass="gridHeaderStyle_1row">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imgBtnUndo" runat="server" CommandName="cancelRow" ImageUrl="../Images/cancel_row3.png"
                                                        ToolTip="Cancel" OnClientClick="return UndoParticipChanges(this);" />
                                                    <asp:HiddenField ID="hdnRoyaltorId" runat="server" Value='<%# Bind("royaltor_id") %>' />
                                                    <asp:HiddenField ID="hdnOptPrtCode" runat="server" Value='<%# Bind("option_period_code") %>' />
                                                    <asp:HiddenField ID="hdnSellerGrpCode" runat="server" Value='<%# Bind("seller_group_code") %>' />
                                                    <asp:HiddenField ID="hdnParticipationId" runat="server" Value='<%# Bind("participation_id") %>' />
                                                    <asp:HiddenField ID="hdnActive" runat="server" Value='<%# Bind("is_active") %>' />
                                                    <asp:HiddenField ID="hdnStatusCode" runat="server" Value='<%# Bind("status_code") %>' />
                                                    <asp:HiddenField ID="hdnIsModified" runat="server" Value='<%# Bind("is_modified") %>' />
                                                    <asp:HiddenField ID="hdnISRCVal" runat="server" Value='<%# Bind("isrc_val") %>' />
                                                    <asp:HiddenField ID="hdnTrackListingId" runat="server" Value='<%# Bind("tracklisting_id") %>' />
                                                    <asp:HiddenField ID="hdnIsISRCTrackTitle" runat="server" Value='<%# Bind("is_isrc_track_title") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 10px; padding-top: 10px">
                                <table width="100%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <table width="98%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td width="18%" class="gridHeaderStyle_1row">Royaltor</td>
                                                    <td width="11%" class="gridHeaderStyle_1row">Option Period</td>
                                                    <td width="18%" class="gridHeaderStyle_1row">Territory</td>
                                                    <td width="4%" class="gridHeaderStyle_1row">Share</td>
                                                    <td width="4%" class="gridHeaderStyle_1row">Total Share</td>
                                                    <td width="6%" class="gridHeaderStyle_1row">Time</td>
                                                    <td width="6%" class="gridHeaderStyle_1row">Total Time</td>
                                                    <td width="4%" class="gridHeaderStyle_1row">Track No.</td>
                                                    <td width="12%" class="gridHeaderStyle_1row">Track Title</td>
                                                    <td width="6%" class="gridHeaderStyle_1row">Escalation Code</td>
                                                    <td width="6%" class="gridHeaderStyle_1row">Esc Include Units</td>
                                                    <td width="4%" class="gridHeaderStyle_1row">&nbsp</td>
                                                </tr>
                                                <tr>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:TextBox ID="txtRoyaltorInsert" runat="server" Width="90%" CssClass="textbox_FuzzySearch" TabIndex="109"
                                                            onkeydown="OntxtRoyaltorInsertKeyDown(this);" OnChange="OnRoyFuzzySearchChangeInsert();"></asp:TextBox>
                                                        <ajaxToolkit:AutoCompleteExtender ID="aceRoyaltorAddRow" runat="server"
                                                            ServiceMethod="FuzzySearchAllRoyaltorList"
                                                            ServicePath="~/Services/FuzzySearch.asmx"
                                                            MinimumPrefixLength="1"
                                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                            TargetControlID="txtRoyaltorInsert"
                                                            FirstRowSelected="true"
                                                            OnClientPopulating="royaltorListPopulatingInsert"
                                                            OnClientPopulated="royaltorListPopulatedInsert"
                                                            OnClientHidden="royaltorListPopulatedInsert"
                                                            OnClientItemSelected="royaltorListItemSelectedInsert"
                                                            CompletionListElementID="pnlRoyFuzzySearchInsert" />
                                                        <asp:Panel ID="pnlRoyFuzzySearchInsert" runat="server" CssClass="identifierLable" Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                        <asp:CustomValidator ID="valtxtRoyaltorInsert" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                            ToolTip="Please select valid royaltor from the search list"
                                                            ControlToValidate="txtRoyaltorInsert" ErrorMessage="*" Display="Dynamic" ClientValidationFunction="ValRoyaltorAddRow"></asp:CustomValidator>
                                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtRoyaltorInsert" ValidationGroup="valAddRow"
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select valid royaltor from the search list" InitialValue="" Display="Dynamic">
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                    <td class="insertBoxStyle" align="left">
                                                        <asp:DropDownList ID="ddlOptionPeriodInsert" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="110"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator runat="server" ID="rfvOptionPeriodInsert" ControlToValidate="ddlOptionPeriodInsert" ValidationGroup="valAddRow"
                                                            Text="*" CssClass="requiredFieldValidator" ToolTip="Please select option period" InitialValue="-" Display="Dynamic">
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                    <td class="insertBoxStyle" align="left">
                                                        <asp:TextBox ID="txtTerritoryInsert" runat="server" Width="90%" CssClass="textbox_FuzzySearch" TabIndex="111"
                                                            onkeydown="OntxtTerritoryAddRowKeyDown(this);" onchange="OntxtTerritoryAddRowChange();"></asp:TextBox>
                                                        <ajaxToolkit:AutoCompleteExtender ID="aceTerritoryAddRow" runat="server"
                                                            ServiceMethod="FuzzyPartMaintSellerGrpList"
                                                            ServicePath="~/Services/FuzzySearch.asmx"
                                                            MinimumPrefixLength="1"
                                                            CompletionInterval="100" EnableCaching="false" CompletionSetCount="20"
                                                            TargetControlID="txtTerritoryInsert"
                                                            FirstRowSelected="true"
                                                            OnClientPopulating="territoryAddRowListPopulating"
                                                            OnClientPopulated="territoryAddRowListPopulated"
                                                            OnClientHidden="territoryAddRowListHidden"
                                                            OnClientItemSelected="territoryAddRowListItemSelected"
                                                            CompletionListElementID="pnlTerritoryAddRowFuzzySearch" />
                                                        <asp:Panel ID="pnlTerritoryAddRowFuzzySearch" runat="server" CssClass="identifierLable"
                                                            Style="bottom: 0px; top: 1000px; position: absolute; height: 200px;" />
                                                        <asp:CustomValidator ID="valtxtTerritoryAddRow" runat="server" ValidationGroup="valGrpAppendAddRow" CssClass="requiredFieldValidator"
                                                            ClientValidationFunction="ValTerritoryAddRow" ToolTip="Please select valid territory from the search list"
                                                            ControlToValidate="txtTerritoryInsert" ErrorMessage="*" Display="Dynamic"></asp:CustomValidator>
                                                    </td>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:HiddenField ID="hdnShareInsert" runat="server" Value="" />
                                                        <asp:TextBox ID="txtShareInsert" runat="server" CssClass="textboxStyle" onchange="OnFocusOutShareInsert();" TabIndex="112" Width="50%"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revShareInsert" runat="server" Text="*" ControlToValidate="txtShareInsert"
                                                            ValidationExpression="^[1-9][0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valAddRow"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>

                                                    </td>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:HiddenField ID="hdnTotalShareInsert" runat="server" Value="" />
                                                        <asp:TextBox ID="txtTotalShareInsert" runat="server" CssClass="textboxStyle" onchange="OnFocusOutShareInsert();" TabIndex="113" Width="50%"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTotalShareInsert" runat="server" Text="*" ControlToValidate="txtTotalShareInsert"
                                                            ValidationExpression="^[1-9][0-9]*$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valAddRow"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>

                                                        <asp:CompareValidator runat="server" ID="cvTotalShareInsert" ControlToValidate="txtTotalShareInsert" ControlToCompare="txtShareInsert" Operator="GreaterThanEqual"
                                                            Type="Integer" Display="Dynamic" CssClass="requiredFieldValidator" ForeColor="Red" ErrorMessage="*" ToolTip="Share must not be greater than Total Share or participation multiplier should not be greater than 1"></asp:CompareValidator>

                                                    </td>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:TextBox ID="txtTimeInsert" runat="server" CssClass="textboxStyle" TabIndex="114" Width="70%" MaxLength="7"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTimeInsert" runat="server" Text="*" ControlToValidate="txtTimeInsert" ValidationGroup="valAddRow"
                                                            ValidationExpression="^(___:__:__)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        <asp:CustomValidator ID="valTimeInsert" runat="server" ValidationGroup="valAddRow" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            ClientValidationFunction="ValTimeInsertRow" ControlToValidate="txtTimeInsert" ErrorMessage="*" ToolTip="Time Share must not be greater than Total Time">
                                                        </asp:CustomValidator>
                                                        <ajaxToolkit:MaskedEditExtender ID="mteTimeInsert" runat="server"
                                                            TargetControlID="txtTimeInsert" Mask="999:99:99" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="false" />
                                                    </td>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:TextBox ID="txtTotalTimeInsert" runat="server" CssClass="textboxStyle" TabIndex="115" Width="70%" MaxLength="10"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTotalTimeInsert" runat="server" Text="*" ControlToValidate="txtTotalTimeInsert" ValidationGroup="valAddRow"
                                                            ValidationExpression="^(___:__:__)|([0-9]{0,3}:[0-5][0-9]:[0-5][0-9])$" CssClass="requiredFieldValidator" ForeColor="Red"
                                                            ToolTip="Please enter a valid time in hhh:mm:ss format" Display="Dynamic"> </asp:RegularExpressionValidator>
                                                        <asp:CustomValidator ID="CustomValidator2" runat="server" ValidationGroup="valAddRow" CssClass="requiredFieldValidator" Display="Dynamic"
                                                            ClientValidationFunction="ValTimeInsertRow" ControlToValidate="txtTotalTimeInsert" ErrorMessage="*" ToolTip="Time Share must not be greater than Total Time">
                                                        </asp:CustomValidator>
                                                        <ajaxToolkit:MaskedEditExtender ID="mteTotalTimeInsert" runat="server"
                                                            TargetControlID="txtTotalTimeInsert" Mask="999:99:99" AcceptNegative="None"
                                                            ClearMaskOnLostFocus="false" />
                                                    </td>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:TextBox ID="txtTrackNoInsert" runat="server" CssClass="textboxStyle" onkeydown="javascript: OnTrackNoInsert();" onblur="ClearTrackTitleAddRow();" OnTextChanged="txtTrackNoInsert_OnTextChanged" TabIndex="116" Width="50%" MaxLength="10"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revTrackNoInsert" runat="server" Text="*" ControlToValidate="txtTrackNoInsert"
                                                            ValidationExpression="^[+]?\d+$" CssClass="requiredFieldValidator" ForeColor="Red" ValidationGroup="valAddRow"
                                                            ToolTip="Please enter only integers" Display="Dynamic"> 
                                                        </asp:RegularExpressionValidator>
                                                    </td>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:TextBox ID="txtTrackTitleInsert" runat="server" Width="90%" MaxLength="30" CssClass="textboxStyle" TabIndex="117" onkeydown="OnTrackTitleInsertBackSapce();"></asp:TextBox>
                                                    </td>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:DropDownList ID="ddlEscCodeInsert" runat="server" Width="90%" CssClass="ddlStyle" TabIndex="118" onchange="OnChangeEscCodeInsert();"></asp:DropDownList>
                                                    </td>
                                                    <td align="center" class="insertBoxStyle_No_Padding">
                                                        <asp:CheckBox ID="cbEscIncludeUnitsInsert" runat="server" TabIndex="119" />
                                                    </td>
                                                    <td class="insertBoxStyle_No_Padding">
                                                        <table width="100%" style="float: right; table-layout: fixed">
                                                            <tr style="float: right">
                                                                <td align="right" style="float: right" width="50%">
                                                                    <asp:ImageButton ID="imgBtnSave" runat="server" TabIndex="120" ImageUrl="../Images/add_row.png" ToolTip="Save participant"
                                                                        OnClientClick="if (!ValidatePopUpAddRow()) { return false;};" OnClick="imgBtnSave_Click" onkeydown="imgBtnSaveKeydown();" />
                                                                </td>
                                                                <td align="right" style="float: right" width="50%">
                                                                    <asp:ImageButton ID="imgBtnCancel" runat="server" CommandName="cancelrow" TabIndex="121" ImageUrl="../Images/cancel_row3.png"
                                                                        ToolTip="Cancel" OnClientClick="return ClearAddRow();" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
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
                    </table>
                </td>
            </tr>
            </table>
            <asp:UpdateProgress ID="progressBarPageLevel" runat="server" AssociatedUpdatePanelID="updPnlPageLevel" DisplayAfter="100">
                <ProgressTemplate>
                    <div id="Search" style="font-weight: bold; color: Black; z-index: 2">
                        <img src="../Images/InProgress2.gif" alt="" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <ajaxToolkit:ModalPopupExtender ID="mPopupPageLevel" runat="server" PopupControlID="progressBarPageLevel" TargetControlID="progressBarPageLevel"
                BackgroundCssClass="progressBar" RepositionMode="RepositionOnWindowResize">
            </ajaxToolkit:ModalPopupExtender>

            <%--Cat status change pop up--%>
            <asp:Button ID="dummyCatStatusPopup" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeCatStatusPopup" runat="server" PopupControlID="pnlCatStatusPopup" TargetControlID="dummyCatStatusPopup"
                BackgroundCssClass="popupBox">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlCatStatusPopup" runat="server" align="center" Width="25%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <asp:Label ID="lblHdrCatStatusPopup" runat="server" Text="Confirmation" CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMsgCatStatusPopup" runat="server" Text="Do you want to update Status of all participants to this Catalogue Status?"
                                CssClass="identifierLable"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnYesCatStatusPopup" runat="server" Text="Yes" CssClass="ButtonStyle" OnClientClick="UpdateStatusConfirmYes();" CausesValidation="false" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="btnNoCatStatusPopup" runat="server" Text="No" CssClass="ButtonStyle" OnClientClick="return UpdateStatusConfirmNo();" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Cat status change pop up - Ends--%>

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
                                        <asp:Button ID="btnUnSavedDataExit" runat="server" Text="Exit" CssClass="ButtonStyle" Width="30%" OnClientClick="OnUnSavedDataExit();"
                                            OnClick="btnUnSavedDataExit_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--Warning on unsaved data popup--%>

            <%--Fuzzy search pop up - starts --%>
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
                                    <td>
                                        <asp:Label ID="lblFuzzySearchPopUp" runat="server" Text="Complete Search List" CssClass="identifierLable"></asp:Label>
                                    </td>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseFuzzySearchPopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClick="btnCloseFuzzySearchPopup_Click" />
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
            <%--Fuzzy search pop up - Ends --%>

            <asp:Button ID="btnShowNoTrackDummy" runat="server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="mpeShowNoTrackMsg" runat="server" PopupControlID="pnlShowNoTrackMsg" TargetControlID="btnShowNoTrackDummy"
                BackgroundCssClass="popupBox" CancelControlID="btnCloseShowNoTrackMsg">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlShowNoTrackMsg" runat="server" align="center" Width="35%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid"
                Style="z-index: 1; display: none">
                <table width="100%">
                    <tr>
                        <td class="ScreenName" style="align-content: center">
                            <table width="100%">
                                <tr>
                                    <td align="right" style="vertical-align: top;">
                                        <asp:ImageButton ID="btnCloseShowNoTrackMsg" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" OnClientClick="return CloseShowNoTrackMsgPopup();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblNoTrackMessage" runat="server" CssClass="identifierLable" Text="Track No doesn't exist. Please select another Track No!"></asp:Label>
                        </td>
                    </tr>

                </table>
            </asp:Panel>

            <msg:MsgControl ID="msgView" runat="server" />
            <asp:HiddenField ID="hdnExceptionRaised" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridPnlHeight" runat="server" />
            <asp:HiddenField ID="hdnDataChanged" runat="server" Value="N" />
            <asp:HiddenField ID="hdnUserRole" runat="server" />
            <asp:HiddenField ID="hdnPageName" runat="server" />
            <asp:HiddenField ID="hdnIsConfirmPopup" runat="server" Value="N" />
            <asp:HiddenField ID="hdnButtonSelection" runat="server" />
            <asp:Label ID="lblTab" runat="server" TabIndex="99" onkeydown="FocusLblKeyPress();"></asp:Label>
            <asp:HiddenField ID="hdnFuzzySearchField" runat="server" Value="" />
            <asp:HiddenField ID="hdnFuzzySearchText" runat="server" />
            <asp:Button ID="btnHdnRoyaltorInsertSearch" runat="server" Style="display: none;" OnClick="btnHdnRoyaltorInsertSearch_Click" CausesValidation="false" />
            <asp:Button ID="btnFuzzyTerritoryListPopup" runat="server" Style="display: none;" OnClick="btnFuzzyTerritoryListPopup_Click" CausesValidation="false" />
            <asp:HiddenField ID="hdnTimeTrack" runat="server" />
            <asp:HiddenField ID="hdnTrackTitlefromISRC" runat="server" Value="N" />
            <asp:HiddenField ID="hdnISRC" runat="server" />
            <asp:HiddenField ID="hdnTrackListingID" runat="server" />
            <asp:HiddenField ID="hdnGridFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnIsSuperUser" runat="server" Value="N" />
            <asp:HiddenField ID="hdnGridRoyFuzzySearchRowId" runat="server" />
            <asp:HiddenField ID="hdnChangesMadeNotSavedPartId" runat="server" />
            <asp:HiddenField ID="hdnTrackNoInsertTemp" runat="server" />
            <asp:HiddenField ID="hdnNewParticipId" runat="server" Value="-1" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- Start--%>
            <asp:HiddenField ID="hdnSortExpression" runat="server" />
            <asp:HiddenField ID="hdnSortDirection" runat="server" />
            <%--JIRA-746 Changes by Ravi on 06/03/2019 -- End--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
