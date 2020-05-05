<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagSelectorDialog.ascx.cs" Inherits="CMSModules_TagGroups_Controls_TagSelectorDialog" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Panel ID="pnlTags" runat="server">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="tags.tagselector.listold" CssClass="listing-title" EnableViewState="false" />
        <cms:UniGrid ID="gridElem" runat="server" GridName="~/CMSModules/TagGroups/Controls/TagSelector.xml" IsLiveSite="false" OrderBy="TagName" ShowActionsMenu="false" />
        <asp:HiddenField ID="hdnValues" runat="server" />
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </asp:Panel>
</asp:Panel>