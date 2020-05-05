<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/VerticalTabs.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_VerticalTabs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<cms:UIElementLayout runat="server" ID="layoutElem" ShortID="l" CssClass="nav-tabs-vertical-layout">
    <Panes>
        <cms:UILayoutPane ID="paneTitle" runat="server" Direction="North" RenderAs="Div"
            Size="auto" SpacingOpen="0" UseUpdatePanel="false">
            <Template>
                <cms:PageTitle runat="server" ID="pageTitle" Visible="false" />
            </Template>
        </cms:UILayoutPane>
        <cms:UILayoutPane ID="paneTabs" ShortID="t" runat="server" Direction="West" RenderAs="Div"
            Size="0" Resizable="false" Closable="false" UseUpdatePanel="false" SpacingOpen="0">
            <Template>
                <div id="tabOverlayer" class="overlayer overlayer-tabs">
                </div>
                <div class="nav-tabs-container-vertical-background">
                    <div class="nav-tabs-container-vertical-hidden">
                        <i aria-hidden="true" class="icon-ellipsis nav-tabs-container-vertical-hidden-icon"></i>
                        <span class="sr-only"><%= GetString("cms.tabs.show") %></span>
                    </div>
                    <asp:PlaceHolder runat="server" ID="pnlSiteContainer">
                        <div class="nav-tabs-site-selector">
                            <cms:LocalizedLabel runat="server" ID="lblSite" ResourceString="general.site" DisplayColon="true" CssClass="control-label" />
                            <cms:SiteSelector runat="server" ID="siteSelector" />
                        </div>
                    </asp:PlaceHolder>
                    <div class="nav-tabs-back">
                        <button id="lnkBack" title="<%= GetString("cms.tabs.back") %>" class="btn btn-icon">
                            <i aria-hidden="true" class="icon-arrow-crooked-left"></i>
                            <span class="sr-only"><%= GetString("cms.tabs.back") %></span>
                        </button>
                    </div>
                    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cms:UITabs ID="tabsElem" runat="server" UseIFrame="true" UrlTarget="c"
                                OnOnTabsLoaded="tabsElem_OnTabsLoaded" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </Template>
        </cms:UILayoutPane>
        <cms:UILayoutPane ID="paneContent" ShortID="c" runat="server" Direction="Center" RenderAs="Iframe" AppendSrc="false"
            UseUpdatePanel="false" Visible="true" />
    </Panes>
</cms:UIElementLayout>
