<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageBoxControl.ascx.cs" Inherits="WARS.UserControls.MessageBoxControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script type="text/javascript">

    //WUIN-609 - To close the message pop up on Enter key
    //Add keypress event handler when pop up is shown
    //Remove keypress event handler when pop up is closed
    function pageLoad() {        
        if ($find('behaviourMessageBoxPopup') != null) {
            $find('behaviourMessageBoxPopup').add_shown(function () {
                $addHandler(document, "keydown", ClosePopup);
            });

            $find('behaviourMessageBoxPopup').add_hiding(function () {
                $removeHandler(document, "keydown", ClosePopup);
            });
        }
    }    

    function ClosePopup() {        
        //close pop up on Enter key
        if (event.keyCode == 13) {
            $find("behaviourMessageBoxPopup").hide();
        }
    }
    //======= WUIN-609 -- Ends

    function DisplayMessagePopup(message) {        
        var popup = $find('behaviourMessageBoxPopup');
        if (popup != null) {
            popup.show();
            document.getElementById("<%=lblMessage.ClientID %>").innerHTML = message;
        }
    }
</script>

<asp:Button runat="server" ID="btnMpeMessageBoxPopup" Style="display: none" />
<ajaxToolkit:ModalPopupExtender ID="mpeMessageBoxPopup" runat="server" BehaviorID="behaviourMessageBoxPopup" PopupControlID="pnlMessageBoxPopup" TargetControlID="btnMpeMessageBoxPopup"
    CancelControlID="btnClosePopup" RepositionMode="RepositionOnWindowResize" PopupDragHandleControlID="programmaticMessageDragHandle" BackgroundCssClass="messageBackground">
</ajaxToolkit:ModalPopupExtender>
<asp:Panel ID="pnlMessageBoxPopup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" style="display:none">
    <asp:Panel ID="programmaticMessageDragHandle" runat="server" Style="cursor: move;">
        <table id="tblMain" style="width: 100%;">
            <tr class="ScreenName">
                <td align="right" style="vertical-align: top;">
                    <asp:ImageButton ID="btnClosePopup" ImageUrl="../Images/CloseIcon.png" runat="server" Style="cursor: pointer" />
                </td>
            </tr>
            <tr>
                <td></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblMessage" runat="server" CssClass="identifierLable"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
