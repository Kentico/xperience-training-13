<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/TaggingCategories/CategoryMenu.ascx.cs"
    Inherits="CMSWebParts_TaggingCategories_CategoryMenu" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdateTrees" runat="server">
    <ContentTemplate>
        <cms:UniTree runat="server" ID="treeElemG" ShortID="tg" Localize="true" EnableRootAction="false" />
        <cms:UniTree runat="server" ID="treeElemP" ShortID="tp" Localize="true" EnableRootAction="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
