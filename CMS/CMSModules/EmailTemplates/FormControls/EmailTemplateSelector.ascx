<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_EmailTemplates_FormControls_EmailTemplateSelector"
     Codebehind="EmailTemplateSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector runat="server" ID="usTemplate" ObjectType="cms.emailtemplate" SelectionMode="SingleTextBox"
            OrderBy="EmailTemplateDisplayName" ResourcePrefix="emailtemplateselect" AllowEmpty="true" AllowEditTextBox="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
