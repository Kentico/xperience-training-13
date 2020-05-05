<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Cultures_SiteCultureSelector"
     Codebehind="SiteCultureSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" ShortID="sc" runat="server" DisplayNameFormat="{%CultureName%}"
            OrderBy="CultureName" ObjectType="cms.culture" AllowEmpty="false" AllowAll="false"
            LocalizeItems="true" SelectionMode="SingleDropDownList" ReturnColumnName="CultureCode" AllowDefault="true" OnOnSpecialFieldsLoaded="uniSelector_OnSpecialFieldsLoaded" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
