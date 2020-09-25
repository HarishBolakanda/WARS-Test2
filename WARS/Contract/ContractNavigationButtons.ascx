<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContractNavigationButtons.ascx.cs" Inherits="WARS.Contract.ContractNavigationButtons" %>

<script type="text/javascript">

    var hdnIsNewRoyaltor;

    function OpenRoySearchScreen() {
        if (IsDataChanged()) {
            document.getElementById("hdnIsNotContractScreen").value = "Y";
            
           // window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyaltorSearch.aspx?isNewRequest=N");
        }
        else {
            window.location = ("../Contract/RoyaltorSearch.aspx?isNewRequest=N");
        }
    }

    function OpenRoyContractScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyaltorContract.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);

        }
        else {
            window.location = "../Contract/RoyaltorContract.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenPayeeScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractPayee.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);

        }
        else {
            window.location = "../Contract/RoyContractPayee.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenSupplierScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractPayeeSupp.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);

        }
        else {
            window.location = "../Contract/RoyContractPayeeSupp.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenOptionPrdScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractOptionPeriods.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../Contract/RoyContractOptionPeriods.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenRoyRatesScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractRoyRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../Contract/RoyContractRoyRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenSubsidRatesScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractSubRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../Contract/RoyContractSubRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenPackagingRatesScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractPkgRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../Contract/RoyContractPkgRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenEscalationRatesScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractEscRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../Contract/RoyContractEscRates.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }

    }

    function OpenContractGrpsScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {

            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractGrouping.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../Contract/RoyContractGrouping.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }
    //JIRA-1146 CHanges -- Start
    function OpenTaxDetailsScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractTaxDetails.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../Contract/RoyContractTaxDetails.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }
    //JIRA-1146 CHanges -- End
    function OpenNotesScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractNotes.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../Contract/RoyContractNotes.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenReservesScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../DataMaintenance/RoyaltorReserves.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);
        }
        else {
            window.location = "../DataMaintenance/RoyaltorReserves.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }

    function OpenEscHisScreen() {
        var royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        hdnIsNewRoyaltor = document.getElementById("<%=hdnIsNewRoyaltor.ClientID %>").value;
        royaltorId = document.getElementById("<%=hdnRoyaltorId.ClientID %>").value;
        document.getElementById("hdnIsContractScreen").value = "Y";
        if (IsDataChanged()) {
            window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyContractEscHistory.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor);

        }
        else {
            window.location = "../Contract/RoyContractEscHistory.aspx?RoyaltorId=" + royaltorId + "&isNewRoyaltor=" + hdnIsNewRoyaltor;
        }
    }
</script>

<table width="100%" id="TblContractNavigation" runat="server">
    <tr id="trRoyaltorSearch">
        <td>
            <asp:Button ID="btnRoySearch" runat="server" CssClass="LinkButtonStyle"
                Text="Royaltor Search" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenRoySearchScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trContractDetails">
        <td>
            <asp:Button ID="btnRoyContract" runat="server" CssClass="LinkButtonStyle"
                Text="Contract Details" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenRoyContractScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trPayeeDetails">
        <td>
            <asp:Button ID="btnPayee" runat="server" CssClass="LinkButtonStyle"
                Text="Payee/Courtesy Details" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenPayeeScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trSupplierDetails">
        <td>
            <asp:Button ID="btnSupplierDetails" runat="server" CssClass="LinkButtonStyle"
                Text="Supplier Details" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenSupplierScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trOptionPeriods">
        <td>
            <asp:Button ID="btnOptionPeriods" runat="server" CssClass="LinkButtonStyle"
                Text="Option Periods" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenOptionPrdScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trRoyaltyRates">
        <td>
            <asp:Button ID="btnRoyRates" runat="server" CssClass="LinkButtonStyle"
                Text="Royalty Rates" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenRoyRatesScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trSubsidiaryRates">
        <td>
            <asp:Button ID="btnSubsidRates" runat="server" CssClass="LinkButtonStyle"
                Text="Subsidiary Rates" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenSubsidRatesScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trPackagingRates">
        <td>
            <asp:Button ID="btnPackagingRates" runat="server" CssClass="LinkButtonStyle"
                Text="Packaging Rates" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenPackagingRatesScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trEscalationRates">
        <td>
            <asp:Button ID="btnEscalationRates" runat="server" CssClass="LinkButtonStyle"
                Text="Escalation Rates" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenEscalationRatesScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trContractGroupings">
        <td>
            <asp:Button ID="btnContractGrps" runat="server" CssClass="LinkButtonStyle"
                Text="Contract Groupings" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenContractGrpsScreen()) { return false;};" />
        </td>
    </tr>
    <%--JIRA-1146 Changes --Start--%>
    <tr id="trTaxDetails">
        <td>
            <asp:Button ID="btnTaxDetails" runat="server" CssClass="LinkButtonStyle"
                Text="Tax Details" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenTaxDetailsScreen()) { return false;};" />
        </td>
    </tr>
    <%--JIRA-1146 Changes --End --%>
    <tr id="trNotes">
        <td>
            <asp:Button ID="btnNotes" runat="server" CssClass="LinkButtonStyle"
                Text="Notes" ToolTip="Notes" UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenNotesScreen()) { return false;};" />
        </td>
    </tr>
    <tr>
        <td>
            <br />
        </td>
    </tr>    
    <tr id="trBalResHistory">
        <td>
            <asp:Button ID="btnBalResvHistory" runat="server" CssClass="LinkButtonStyle" Text="Balances and Reserves" ToolTip="Balances and Reserves"
                 UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenReservesScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trEscHitory">
        <td>
            <asp:Button ID="btnEscHistory" runat="server" CssClass="LinkButtonStyle" Text="Escalation History" ToolTip="Escalation History"
                 UseSubmitBehavior="false" Width="90%" OnClientClick="if (!OpenEscHisScreen()) { return false;};" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdnRoyaltorId" runat="server" />
<asp:HiddenField ID="hdnIsNewRoyaltor" runat="server" Value="N" />






