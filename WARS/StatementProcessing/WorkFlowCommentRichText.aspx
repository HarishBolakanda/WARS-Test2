<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowCommentRichText.aspx.cs" Inherits="WARS.StatementProcessing.WorkFlowCommentRichText" %>

<!DOCTYPE html>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function SetTxtCommentHeight() {            
            var windowHeight = window.screen.availHeight;
            var gridPanelHeight = windowHeight * 0.5 ;
            document.getElementById("txtWorkFlowComment").style.height = (gridPanelHeight * 0.5 - 40) + "px";
        }

        function OnCommentChange() {            
            parent.SetCommentChangedFlag();
        }
       

        </script>
</head>
<body>
    <form id="frmWorkFlowComment" runat="server">
        <ajaxToolkit:ToolkitScriptManager runat="Server" ID="tsm" AsyncPostBackTimeout="3600" EnablePageMethods="true" />
        <div>
            <asp:TextBox ID="txtWorkFlowComment" runat="server" Width="99%" CssClass="identifierLable" TextMode="MultiLine" Text="" Height="100px"></asp:TextBox>
            <ajaxToolkit:HtmlEditorExtender ID="txtWorkFlowCommentExtender" runat="server" TargetControlID="txtWorkFlowComment" EnableSanitization="false" OnClientChange="OnCommentChange">
            </ajaxToolkit:HtmlEditorExtender>            
        </div>
        <style>
            .ajax__html_editor_extender_texteditor {
                font-family: Calibri, Verdana, Arial, Serif !important;
                font-size: 12px;                  
            }
        </style>        
    </form>    
</body>
</html>
