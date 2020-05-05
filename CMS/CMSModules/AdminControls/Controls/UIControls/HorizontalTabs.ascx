<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/HorizontalTabs.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_HorizontalTabs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<cms:UIElementLayout runat="server" ID="layoutElem" ShortID="l" CssClass="nav-tabs-horizontal-layout">
    <Panes>
        <cms:UILayoutPane ID="paneTabs" ShortID="t" runat="server" Direction="North" RenderAs="Div" PaneClass="ui-layout-pane-visible"
            Size="auto" Resizable="false" SpacingOpen="0" UseUpdatePanel="false">
            <Template>
                <cms:PageTitle runat="server" ID="pageTitle" Visible="false" />
                <asp:PlaceHolder runat="server" ID="pnlSiteContainer">
                    <cms:LocalizedLabel runat="server" ID="lblSite" ResourceString="general.site" DisplayColon="true" />
                    <cms:SiteSelector runat="server" ID="siteSelector" />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="pnlTabsContainer" EnableViewState="false">
                    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cms:UITabs ID="tabsElem" runat="server" UseIFrame="true"
                                UrlTarget="c" OnOnTabsLoaded="tabsElem_OnTabsLoaded" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </asp:PlaceHolder>
            </Template>
        </cms:UILayoutPane>
        <cms:UILayoutPane ID="paneContent" ShortID="c" runat="server" Direction="Center" RenderAs="Iframe" AppendSrc="false"
            UseUpdatePanel="false" Visible="true" />
    </Panes>
</cms:UIElementLayout>
