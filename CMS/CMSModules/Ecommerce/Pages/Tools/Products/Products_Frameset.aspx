<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Products_Frameset.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Products_Frameset" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="E-commerce - Products" Theme="Default" %>
<%@ Import Namespace="CMS.Localization" %>
<asp:Content runat="server" ID="cntContent" ContentPlaceHolderID="plcContent">
    <input type="hidden" id="selectedNodeId" name="selectedNodeId" value="<%=ResultNodeID%>" />
    <input type="hidden" id="selectedCulture" name="selectedCulture" value="<%=LocalizationContext.PreferredCultureCode%>" />
    <input type="hidden" id="selectedSplitModeCulture" name="selectedSplitModeCulture"
        value="<%=PortalUIHelper.SplitModeCultureCode %>" />
    <input type="hidden" id="selectedMode" name="selectedMode" value="editform" />
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="contenttree" runat="server" Direction="West" SpacingOpen="8"
                SpacingClosed="8" TogglerLengthOpen="32" TogglerLengthClosed="32" Size="304" MinSize="250" ControlPath="~/CMSModules/Ecommerce/Controls/UI/ProductNavigationPanel.ascx"
                RenderAs="div" IsLiveSite="False" PaneClass="ProductSectionNavigation" UseUpdatePanel="False" />
            <cms:UILayoutPane ID="contentview" runat="server" Direction="Center" RenderAs="Iframe"
                MaskContents="true" />
        </Panes>
    </cms:UILayout>
</asp:Content>
