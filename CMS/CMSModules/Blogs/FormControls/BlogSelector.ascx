<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_FormControls_BlogSelector"  Codebehind="BlogSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="blogSelector" runat="server" SelectionMode="SingleDropDownList"
            ResourcePrefix="blogselector" OrderBy="BlogName" OnOnSpecialFieldsLoaded="blogSelector_OnSpecialFieldsLoaded"
            AllowEmpty="false" AllowAll="false" DisplayNameFormat="{%BlogName%}" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
