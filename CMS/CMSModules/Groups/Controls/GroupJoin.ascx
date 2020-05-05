<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Controls_GroupJoin"
     Codebehind="GroupJoin.ascx.cs" %>
<table class="JoinTable">
    <tr>
        <td colspan="2" class="InfoArea">
            <asp:Label runat="server" ID="lblInfo" EnableViewState="false" />
        </td>
    </tr>
    <asp:PlaceHolder ID="plcButtons" runat="server">
        <tr>
            <td colspan="2" class="ButtonsArea">
                <cms:CMSButton runat="server" ButtonStyle="Default" ID="btnJoin" EnableViewState="false" /><cms:CMSButton
                    runat="server" ButtonStyle="Default" ID="btnCancel" EnableViewState="false" />
            </td>
        </tr>
    </asp:PlaceHolder>
</table>
