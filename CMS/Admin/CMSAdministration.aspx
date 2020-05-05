<%@ Page Language="C#" AutoEventWireup="true" Inherits="Admin_CMSAdministration"  Codebehind="CMSAdministration.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Theme="Default" Title="Administration" %>
<%@ Import Namespace="CMS.Modules" %>

<%@ Register Src="~/CMSAdminControls/UI/ScreenLock/ScreenLockDialog.ascx" TagName="ScreenLockDialog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/AdvancedPopupHandler.ascx" TagName="AdvancedPopupHandler"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="CMSDeskContent">
        <cms:UILayout runat="server" ID="layoutElem">
            <Panes>
                <cms:UILayoutPane ID="paneHeader" ShortID="h" runat="server" Direction="North" RenderAs="Div"
                    ControlPath="~/CMSAdminControls/UI/Header.ascx" Resizable="False" SpacingOpen="0"
                    SpacingClosed="0" PaneClass="main-header" />
                <cms:UILayoutPane ID="cmsdesktop" runat="server" Direction="Center" RenderAs="Iframe"
                    ModuleAvailabilityForSiteRequired="true" Src="about:blank" />
            </Panes>
        </cms:UILayout>
    </div>
    <%-- APPLIST START --%>
    <div class="applist-panel" id="cms-applist-panel">
        <h2 class="sr-only"><%= GetString("applicationlist.title")  %></h2>
        <%-- APPLIST HEADER START --%>
        <div id="cms-applist-header">
            <a href="javascript:void(0)" class="applist-toggle navbar-left" id="cms-applist-toggle-close" title="<%= GetString("applicationlist.close") %>">
                <i aria-hidden="true" class="icon-chevron-left-circle cms-nav-icon-medium"></i>
                <span class="sr-only"><%= GetString("applicationlist.close") %></span>
            </a>
            <h3 class="applist-header"><%= GetString("applicationlist.header")  %></h3>
        </div>
        <%-- APPLIST HEADER END --%>

        <%-- APPLIST FILTER START --%>
        <div class="nav-search-container">
            <h3 class="sr-only"><%= GetString("applicationlist.searchtitle")  %></h3>
            <input id="app_search" autocomplete="off" type="search" class="js-filter-search" placeholder="<%= GetString("applicationlist.search") %>" />
            <label for="app_search" class="sr-only"><%= GetString("applicationlist.search") %></label>
            <i aria-hidden="true" class="icon-magnifier"></i>
        </div>
        <%-- APPLIST FILTER END --%>

        <%-- APPLIST CONTENT START --%>
        <div class="js-scrollable">
            <div class="panel-group" id="cms-applist">
                <cms:BasicUniView runat="server" ID="appListUniview" HierarchicalDisplayMode="Inner" RelationColumnID="ElementID" DataBindByDefault="False" EnableViewState="False">
                    <ItemTemplate>
                        <asp:PlaceHolder runat="server" ID="plcCategoryTemplate" Visible="false">
                            <div class="panel panel-default js-filter-parent">
                                <div class="panel-heading">
                                    <h3><span class="accordion-toggle" data-toggle="collapse" data-parent="#cms-applist" data-target="#collapse<%# Eval("ElementID") %>"><%# HTMLHelper.HTMLEncode((string)Eval("ElementDisplayName")) %></span></h3>
                                </div>
                                <div id="collapse<%# Eval("ElementID") %>" class="panel-collapse collapse">
                                    <div class="panel-body">
                                        <ul>
                                            <cms:SubLevelPlaceHolder runat="server" ID="plcInnerContent" />
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="plcItemTemplate" Visible="False">
                            <li class="js-filter-item"><a title="<%# ResHelper.LocalizeString(Eval("ElementDescription").ToString()) %>" href="#<%# Eval("ElementGUID")%>" data-appguid="<%# Eval("ElementGUID")%>"><%# UIHelper.GetAccessibleImageMarkup(Page, EvalText("ElementIconClass", "icon-app-default"), EvalText("ElementIconPath"), size: FontIconSizeEnum.Standard, iconColorClass: ApplicationCSSHelper.GetApplicationIconCssClass(EvalGuid("ElementGUID"))) %>
                                <%# HTMLHelper.HTMLEncode((string)Eval("ElementDisplayName")) %></a></li>
                        </asp:PlaceHolder>
                    </ItemTemplate>
                </cms:BasicUniView>
                <p class="js-filter-empty padding-100"><%= GetString("applicationlist.noresults") %></p>
            </div>
        </div>
        <%-- APPLIST CONTENT END --%>

        <%-- LIVESITE BUTTON --%>
        <asp:PlaceHolder runat="server" ID="plcLiveSite" Visible="False">
            <div class="btn-livesite-wrapper">
                <asp:HyperLink runat="server" ID="lnkLiveSite" CssClass="btn btn-default" Target="_blank" />
            </div>
        </asp:PlaceHolder>
    </div>
    <div id="cms-overlayer" class="applist-overlayer"></div>
    <script type="text/javascript">
        //<![CDATA[
        function SetLiveSiteURL(liveSiteURL) {
            if (!liveSiteURL) {
                // Set default URL to root
                liveSiteURL = '<%= DefaultLiveSiteUrl %>';
            }

            var element = document.getElementById('<%= lnkLiveSite.ClientID %>');
            if (element != null) {
                element.href = liveSiteURL;
            }
        }
        //]]>
    </script>
    <%-- APPLIST END --%>
    <cms:AdvancedPopupHandler runat="server" ID="popupHandler" />
    <cms:ScreenLockDialog ID="screenLockDialogElem" runat="server" />
</asp:Content>
