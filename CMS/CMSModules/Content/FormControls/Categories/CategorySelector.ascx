<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_FormControls_Categories_CategorySelector"
     Codebehind="CategorySelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="selectCategory" runat="server" ReturnColumnName="CategoryName"
            ObjectType="cms.categorylist" ResourcePrefix="categoryselector" OrderBy="CategoryNamePath"
            AdditionalColumns="CategoryNamePath,CategoryEnabled" SelectionMode="SingleTextBox"
            AllowEmpty="true" IsLiveSite="false" AllowEditTextBox="true" DisabledItems="personal" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
