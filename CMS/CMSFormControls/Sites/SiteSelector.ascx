<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Sites_SiteSelector"
     Codebehind="SiteSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlSelector" runat="server" CssClass="site-selector">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniSelector ID="uniSelector" ShortID="ss" runat="server" ObjectType="cms.site" ResourcePrefix="siteselect" SelectionMode="SingleDropDownList" AllowEmpty="False" DisplayNameFormat="{%SiteDisplayName%}" OrderBy="SiteDisplayName" AllowAll="True" OnOnSpecialFieldsLoaded="uniSelector_OnSpecialFieldsLoaded" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
