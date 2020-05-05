<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Controls_GroupSizeSlider"
     Codebehind="GroupSizeSlider.ascx.cs" %>

<asp:Panel runat="server" ID="pW" CssClass="newsletter-group-size">
    <asp:Panel runat="server" ID="pnlSlider" >
        <asp:Panel runat="server" ID="pnlRes" />
    </asp:Panel>
</asp:Panel>
<table width="100%" style="text-align: center;">
    <tr>
        <td runat="server" id="cellSub" nowrap="nowrap">
            <asp:Label runat="server" ID="lblTestGroup" EnableViewState="false" />
        </td>
        <td nowrap="nowrap">
            <asp:Label runat="server" ID="lblRemainder" EnableViewState="false" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdnSize" runat="server" EnableViewState="true" Value="50" />
