<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_UserContributions_EditContribution"  Codebehind="~/CMSWebParts/UserContributions/EditContribution.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/UserContributions/EditForm.ascx" TagName="EditForm"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlEdit" CssClass="ContributionPanel">
    <cms:LocalizedLinkButton ID="btnEdit" runat="server" OnClick="btnEdit_Click" CssClass="EditContributionEdit" /><cms:LocalizedLinkButton ID="btnDelete" runat="server" OnClick="btnDelete_Click" CssClass="EditContributionDelete" />
    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="ContributionsEdit">
        <cms:EditForm ID="editForm" runat="server" IsLiveSite="true" />
    </asp:Panel>
</asp:Panel>
