<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="WARS.Test" 
    Title="TEST" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>

<%--MasterPageFile="~/MasterPage.Master"--%>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="msg" TagName="MsgControl" Src="~/UserControls/MessageBoxControl.ascx" %>

    
<%--<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">--%>
<script type="text/javascript" src="Scripts/jquery-3.1.1.js">
   
    <%--Test GIT checkin1--%>
  



</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
      <asp:Button runat="server" ID="btnMpeMessageBoxPopup" Style="display: none" />
<ajaxToolkit:ModalPopupExtender ID="mpeMessageBoxPopup" runat="server" BehaviorID="behaviourMessageBoxPopup" PopupControlID="pnlMessageBoxPopup" TargetControlID="btnMpeMessageBoxPopup"
    CancelControlID="btnClosePopup" RepositionMode="RepositionOnWindowResize" PopupDragHandleControlID="programmaticMessageDragHandle" BackgroundCssClass="messageBackground">
</ajaxToolkit:ModalPopupExtender>
<asp:Panel ID="pnlMessageBoxPopup" runat="server" align="center" Width="30%" BackColor="Window" BorderColor="#808080" BorderWidth="1px" BorderStyle="Solid" style="z-index:1">
    <asp:Panel ID="programmaticMessageDragHandle" runat="server" Style="cursor: move">
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
                    <%--<asp:Label ID="lblMessage" runat="server"></asp:Label>--%>
                    <asp:TextBox ID="lblMessage" runat="server" TextMode="MultiLine" ReadOnly="true" CssClass="gridTextField" Width="90%" ></asp:TextBox>
                </td>                
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
     
    </form>
</body>
</html>

<%--<asp:TextBox ID="TextBox1" runat="server" Height="240px" Width="99%"></asp:TextBox>--%>
<%--<ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtender1" runat="server" TargetControlID="txtNotes" EnableSanitization="false"></ajaxToolkit:HtmlEditorExtender>--%>







