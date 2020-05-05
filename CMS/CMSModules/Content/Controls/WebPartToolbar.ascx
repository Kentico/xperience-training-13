<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_WebPartToolbar"
     Codebehind="WebPartToolbar.ascx.cs" EnableViewState="false" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Base.Web.UI.DragAndDrop" Assembly="CMS.Base.Web.UI" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/SelectWebpart.ascx"
    TagName="SelectWebpart" TagPrefix="cms" %>
<div id="wptPanel" class="WPTPanel">
    <div id="wptLayout" class="WPTLayout">
        <asp:Panel ID="pnlMinimized" CssClass="WPTMinimized" runat="server">
            <div onclick="wptMaximize()">
                <i aria-hidden="true" class="<%= (uiCultureRTL ? "icon-caret-right" : "icon-caret-left") %>"></i>
            </div>
        </asp:Panel>
        <cms:CMSPanel ID="pnlMaximized" ShortID="m" runat="server" CssClass="WPTMaximized">
            <asp:Literal ID="ltrScript" runat="server" EnableViewState="false"></asp:Literal>
            <div class="WPTMenu">
                <div class="WPTMenuContent">
                    <div>
                        <cms:SelectWebpart ID="categorySelector" ShortID="c" runat="server" ShowWebparts="false"
                            EnableCategorySelection="true" EnableViewState="false" ShowRoot="true" />
                    </div>
                    <div class="nav-search-container">
                        <h3 class="sr-only"><%= GetString("webparttoolbar.searchtitle", prefferedUICultureCode)  %></h3>
                        <cms:CMSTextBox ID="txtSearch" runat="server" MaxLength="200" EnableViewState="true" />
                        <label for="<%= txtSearch.ClientID %>" class="sr-only"><%= GetString("webparttoolbar.search", prefferedUICultureCode) %></label>
                        <i aria-hidden="true" class="icon-magnifier"></i>
                    </div>
                </div>
            </div>
            <div class="WPTMinimize">
                <div onclick="wptMinimize()">
                    <i aria-hidden="true" class="<%= (uiCultureRTL ? "icon-caret-left" : "icon-caret-right") %>"></i>
                </div>
            </div>
            <asp:Panel ID="pnlScrollBack" runat="server" CssClass="WPTBackSlider">
                <i class="icon-chevron-up-circle" aria-hidden="true"></i>
            </asp:Panel>
            <asp:Panel ID="pnlScrollForward" runat="server" CssClass="WPTForwardSlider">
                <i class="icon-chevron-down-circle" aria-hidden="true"></i>
            </asp:Panel>
            <cms:CMSUpdatePanel ID="pnlU" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="hdnUpdater" runat="server" />
                    <cms:ScrollPanel ID="scrollPanel" ShortID="sp" runat="server" CssClass="WPTItemsContainer"
                        ScrollAreaCssClass="WPTItemsRow" BackwardScrollerControlID="pnlScrollBack"
                        ForwardScrollerControlID="pnlScrollForward" InnerItemClass="WPTSelectorEnvelope"
                        EnableViewState="false" Layout="Vertical">
                        <asp:Button runat="server" ID="btnLoadMore" CssClass="HiddenButton" />
                        <asp:Literal ID="ltlRecentlyUsedWebParts" runat="server"></asp:Literal>
                        <cms:QueryRepeater ID="repItems" runat="server" QueryName="cms.webpartcategory.selectallview" ShortID="r" StopProcessing="true" DataBindByDefault="false" ForceCacheMinutes="true">
                            <ItemTemplate>
                                <asp:Literal ID="ltlRecUsed" runat="server"></asp:Literal>
                                <asp:Panel ID="i" CssClass="WPTSelectorEnvelope" runat="server">
                                    <asp:Panel ID="wptHandle" CssClass="WPTHandle" runat="server">
                                        <asp:Literal ID="ltrImage" runat="server"></asp:Literal>
                                        <div><%# HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(Eval("DisplayName")), prefferedUICultureCode))%><asp:Literal ID="ltlCategorytComment" runat="server"></asp:Literal></div>
                                    </asp:Panel>
                                </asp:Panel>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div class="WPTHandle AppearElement"></div>
                            </FooterTemplate>
                        </cms:QueryRepeater>
                    </cms:ScrollPanel>
                    <asp:PlaceHolder runat="server" ID="plcDrag" EnableViewState="false">
                        <div style="display: none;">
                            <asp:Panel runat="server" ID="pnlCue" EnableViewState="false" Width="0" Height="0">
                            </asp:Panel>
                        </div>
                        <cms:DragAndDropExtender ID="ddExtender" runat="server" Enabled="true" HighlightDropableAreas="false">
                        </cms:DragAndDropExtender>
                        <cms:DragAndDropExtender ID="ddExtenderHovered" runat="server" Enabled="true" HighlightDropableAreas="false">
                        </cms:DragAndDropExtender>
                    </asp:PlaceHolder>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </cms:CMSPanel>
    </div>
</div>
<div id="wptLoader" class="WPTLoader">
    <div class="WPTLoaderBackground">
    </div>
    <asp:Panel ID="pnlLoader" runat="server" CssClass="WPTLoaderBox">
    </asp:Panel>
</div>
