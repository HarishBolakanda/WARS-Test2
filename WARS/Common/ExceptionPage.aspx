<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExceptionPage.aspx.cs" Inherits="WARS.ExceptionPage" MasterPageFile="~/MasterPage.Master" %>

<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="ContentPlaceHolderBody">
    <table width="100%">
        <tr>
            <td colspan="2">
                <br />
                <br />
                <div>
                    <asp:Label ID="Label1" runat="server" CssClass="identifierLable" Text="Error Occurred." />
                </div>
                <p>
                    <font color="red">
                        <asp:Label ID="Exp_reason" runat="server" CssClass="identifierLable" /></font>
                </p>
                <div class="identifierLable">
                    Please contact <b>WMI.RoyaltiesSupport@warnermusic.com</b> if required.
                </div>
                <br />
                <br />
            </td>
        </tr>
    </table>

</asp:Content>
