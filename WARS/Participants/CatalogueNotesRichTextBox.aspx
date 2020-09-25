<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogueNotesRichTextBox.aspx.cs" Inherits="WARS.Participants.CatalogueNotesRichTextBox" %>

<!DOCTYPE html>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript">

        function iFramePopulateCatNo() {
            parent.PopulateCatNo();
        }

        function SetTxtNotesHeight() {
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5;
            document.getElementById("txtNotes").style.height = gridPanelHeight + "px";
        }

        function OnNotesChange() {
            parent.SetDataChanged();
        }

    </script>


</head>
<body>
    <form id="frmNotes" runat="server">
        <ajaxToolkit:ToolkitScriptManager runat="Server" ID="tsm" AsyncPostBackTimeout="3600" EnablePageMethods="true" />
        <div>
            <asp:TextBox ID="txtNotes" runat="server" Width="99%" CssClass="identifierLable" TextMode="MultiLine"></asp:TextBox>
            <ajaxToolkit:HtmlEditorExtender ID="txtNotesExtender" runat="server" TargetControlID="txtNotes" EnableSanitization="false" OnClientChange="OnNotesChange">
            </ajaxToolkit:HtmlEditorExtender>

        </div>
        <style>
            .ajax__html_editor_extender_texteditor {
                font-family: Calibri, Verdana, Arial, Serif !important;
                font-size: 12px;
            }
        </style>
        <asp:HiddenField ID="hdnCatNo" runat="server" Value="" />
    </form>
</body>
</html>
