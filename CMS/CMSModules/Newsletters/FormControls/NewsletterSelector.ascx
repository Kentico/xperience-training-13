<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_FormControls_NewsletterSelector"  Codebehind="NewsletterSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="usNewsletters" runat="server" IsLiveSite="false" ObjectType="Newsletter.Newsletter"
            SelectionMode="SingleDropDownList" AllowEmpty="false" ResourcePrefix="newsletterselect" ReturnColumnName="NewsletterName" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
