<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_UserContributions_ContributionList"  Codebehind="ContributionList.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/UserContributions/EditForm.ascx" TagName="EditForm" TagPrefix="cms" %>


<asp:Panel ID="pnlList" CssClass="ContributionsList" runat="server">
    <asp:Panel ID="pnlNewDoc" runat="server" CssClass="NewItemLink" EnableViewState="false">
        <cms:LocalizedLinkButton ID="btnNewDoc" runat="server" EnableViewState="false" OnClick="btnNewDoc_Click" CssClass="NewItemLink" />
    </asp:Panel>
    <cms:UniGrid ID="gridDocs" runat="server" GridName="~/CMSModules/Content/Controls/UserContributions/ContributionList.xml" />
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Panel>

<asp:Panel ID="pnlEdit" CssClass="ContributionsEdit" runat="server">
    <div class="ItemsLink">
        <cms:LocalizedLinkButton ID="btnList" runat="server" EnableViewState="false" OnClick="btnList_Click"  CssClass="NewItemLink" />
    </div>
    <cms:EditForm ID="editDoc" runat="server" />
</asp:Panel>
