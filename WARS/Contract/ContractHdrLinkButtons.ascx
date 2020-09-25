<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContractHdrLinkButtons.ascx.cs" Inherits="WARS.Contract.ContractHdrLinkButtons" %>

<script type="text/javascript">

    function OpenTerritoryGrpScreen() {
        document.getElementById("hdnIsNotContractScreen").value = "Y";
        if (IsDataChanged()) {
            // window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../DataMaintenance/TerritoryGroup.aspx");
        }
        else {
            window.location = "../DataMaintenance/TerritoryGroup.aspx";
        }
    }

    function OpenConfigGrpScreen() {
        document.getElementById("hdnIsNotContractScreen").value = "Y";
        if (IsDataChanged()) {
            // window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../DataMaintenance/ConfigurationGrouping.aspx");
        }
        else {
            window.location = "../DataMaintenance/ConfigurationGrouping.aspx";
        }
    }

    function OpenRoyaltorSearchScreen() {
        document.getElementById("hdnIsNotContractScreen").value = "Y";
        if (IsDataChanged()) {
            // window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../Contract/RoyaltorSearch.aspx");
        }
        else {
            window.location = "../Contract/RoyaltorSearch.aspx";
        }
    }

    function OpenReservesScreen() {
        document.getElementById("hdnIsNotContractScreen").value = "Y";
        if (IsDataChanged()) {
            //window.onbeforeunload = null;
            OpenPopupOnUnSavedData("../DataMaintenance/RoyaltorReserves.aspx");
        }
        else {
            window.location = '../DataMaintenance/RoyaltorReserves.aspx';
        }
    }

    //opening menu screen in javascript to handle issue of data not saved warning validation
    function OpenMenuScreen() {
        document.getElementById("hdnIsNotContractScreen").value = "Y";
        if (window.IsDataChanged != null && IsDataChanged()) {
            // window.onbeforeunload = null;
            OpenPopupOnUnSavedData('../Common/MenuScreen.aspx')

        }
        else {
            window.location = "../Common/MenuScreen.aspx";
        }
    }

</script>

<table width="100%" runat="server" id="tblContHdrNavigation">
    <tr id="trHome">
        <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
            <asp:Button ID="btnOpenMenuScreen" runat="server" Text="Home" UseSubmitBehavior="false" CausesValidation="false"
                CssClass="LinkButtonStyle" TabIndex="0" Width="98%" OnClientClick="if (!OpenMenuScreen()) { return false;};" />
        </td>
    </tr>
    <tr id="trTerritoryGrp">
        <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
            <asp:Button ID="btnOpenTerritoryGrp" runat="server" Text="Territory Group" UseSubmitBehavior="false" CausesValidation="false"
                CssClass="LinkButtonStyle" Width="98%" OnClientClick="if (!OpenTerritoryGrpScreen()) { return false;};" />
            <asp:HiddenField ID="hdnBtnTerritoryGrp" runat="server" Value="Y" />
        </td>
    </tr>
    <tr id="trConfigGrp">
        <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
            <asp:Button ID="btnOpenConfigGrp" runat="server" Text="Configuration Group" UseSubmitBehavior="false" CausesValidation="false"
                CssClass="LinkButtonStyle" Width="98%" OnClientClick="if (!OpenConfigGrpScreen()) { return false;};" />
            <asp:HiddenField ID="hdnBtnConfigGrp" runat="server" Value="Y" />
        </td>
    </tr>
    <tr id="trBalResHis">
        <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
            <asp:Button ID="btnOpenBalAndResv" runat="server" Text="Balances and Reserves" CssClass="LinkButtonStyle"
                Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenReservesScreen()) { return false;};" />
            <asp:HiddenField ID="hdnBtnBalResHis" runat="server" Value="N" />
        </td>
    </tr>
    <tr id="trContMaint">
        <td colspan="11" align="right" style="padding-right: 0; padding-left: 2px;">
            <asp:Button ID="btnContractMaint" runat="server" Text="Contract Maintenance" CssClass="LinkButtonStyle"
                Width="98%" UseSubmitBehavior="false" OnClientClick="if (!OpenRoyaltorSearchScreen()) { return false;};" />
            <asp:HiddenField ID="hdnBtnContractMaint" runat="server" Value="N" />
        </td>
    </tr>
</table>

<asp:HiddenField ID="hdnRoyaltorId" runat="server" />




