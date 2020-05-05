<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Groups_GroupsFilter_files_GroupsFilterControl"  Codebehind="~/CMSWebParts/Community/Groups/GroupsFilter_files/GroupsFilterControl.ascx.cs" %>
<asp:Panel CssClass="Filter" DefaultButton="btnSelect" runat="server" ID="pnlGroupsFilter">
    <span class="FilterSort">
        <asp:Label runat="server" ID="lblSortBy" EnableViewState="false" />
        <asp:LinkButton runat="server" ID="lnkSortByGroupName" OnClick="lnkSortByGroupName_Click"
            EnableViewState="false" />
        <asp:LinkButton runat="server" ID="lnkSortByGroupCreated" OnClick="lnkSortByGroupCreated_Click"
            EnableViewState="false" />
    </span><span class="FilterSearch">
        <cms:LocalizedLabel ID="lblValue" runat="server" EnableViewState="false" AssociatedControlID="txtValue"
            Display="false" ResourceString="general.searchexpression" />
        <cms:CMSTextBox runat="server" ID="txtValue" EnableViewState="false" />
        <cms:CMSButton runat="server" ID="btnSelect" OnClick="btnSelect_Click" ButtonStyle="Default" EnableViewState="false" />
    </span>
</asp:Panel>
