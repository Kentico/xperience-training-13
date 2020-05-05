<%@ Page Language="C#" AutoEventWireup="false"  Codebehind="InsightsMenu.aspx.cs" Inherits="CMSModules_SocialMarketing_Pages_InsightsMenu" MasterPageFile="~/CMSMasterPages/UI/Tree.master" Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIMenu.ascx" TagName="UIMenu" TagPrefix="cms" %>
<asp:Content ID="MainContent" runat="server" ContentPlaceHolderID="plcTree">
    <cms:UIMenu runat="server" ID="treeElem" ModuleName="CMS.SocialMarketing" TargetFrame="analyticsDefault" EnableRootSelect="false" QueryParameterName="node" ModuleAvailabilityForSiteRequired="true" />
</asp:Content>
