<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Filters/DocumentNameFilter_files/DocumentNameFilterControl.ascx.cs"
    Inherits="CMSWebParts_Filters_DocumentNameFilter_files_DocumentNameFilterControl" %>
<asp:Panel CssClass="Filter" DefaultButton="btnSelect" runat="server" ID="pnlUsersFilter">
    <span class="FilterSearch">
        <cms:LocalizedLabel ID="lblValue" runat="server" EnableViewState="false" AssociatedControlID="txtValue"
            Display="false" ResourceString="general.searchexpression" />
        <cms:CMSTextBox runat="server" ID="txtValue" EnableViewState="false" /><cms:CMSButton runat="server" ID="btnSelect" OnClick="btnSelect_Click" ButtonStyle="Default" EnableViewState="false" />
    </span>
</asp:Panel>