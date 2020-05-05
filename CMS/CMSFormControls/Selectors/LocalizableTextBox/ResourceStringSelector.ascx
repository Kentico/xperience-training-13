<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ResourceStringSelector.ascx.cs"
    Inherits="CMSFormControls_Selectors_LocalizableTextBox_ResourceStringSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" AllowEditTextBox="true" ObjectType="cms.resourcetranslated"
            ResourcePrefix="resourcestring" SelectionMode="SingleTextBox" AllowEmpty="false"
            DialogWindowWidth="850" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
