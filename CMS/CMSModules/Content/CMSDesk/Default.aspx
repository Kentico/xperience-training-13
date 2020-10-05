<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Default"
     Codebehind="Default.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Theme="Default" Title="Content" %>
<%@ Import Namespace="CMS.Localization" %>

<asp:Content runat="server" ID="cntContent" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        window.ddNotScroll = true;
        //]]>
    </script>
    <input type="hidden" id="selectedNodeId" name="selectedNodeId" value="<%= ResultNodeID %>" />
    <input type="hidden" id="selectedCulture" name="selectedCulture" value="<%= LocalizationContext.PreferredCultureCode %>" />
    <input type="hidden" id="selectedSplitModeCulture" name="selectedSplitModeCulture" value="<%= PortalUIHelper.SplitModeCultureCode %>" />
    <input type="hidden" id="selectedMode" name="selectedMode" value="<%= HTMLHelper.HTMLEncode(ResultMode) %>" />
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="contentcontrolpanel" runat="server" Direction="West" RenderAs="div"
                ControlPath="~/CMSModules/Content/Controls/ContentNavigationPanel.ascx" SpacingOpen="8"
                SpacingClosed="8" PaneClass="ContentMenu" Size="304" TogglerLengthOpen="32" TogglerLengthClosed="32" UseUpdatePanel="false" MinSize="304" />
            <cms:UILayoutPane ID="contentview" runat="server" Direction="Center" RenderAs="Iframe"
                MaskContents="true" />
        </Panes>
    </cms:UILayout>
</asp:Content>
