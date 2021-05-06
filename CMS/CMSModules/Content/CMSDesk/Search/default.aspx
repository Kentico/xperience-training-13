<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Search_default"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - Search" CodeBehind="default.aspx.cs" %>

<%@ Import Namespace="CMS.Base" %>
<%@ Import Namespace="CMS.Modules" %>
<%@ Import Namespace="CMS.Search" %>

<%@ Register Src="~/CMSModules/Content/Controls/SearchDialog.ascx" TagName="SearchDialog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/SearchResults.ascx" TagName="SearchResults"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:SearchDialog ID="searchDialog" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <br />
    <asp:Panel runat="server" ID="pnlBody">
        <asp:Panel ID="pnlResultsSQL" runat="server">
            <cms:CMSSearchResults ID="repSearchSQL" runat="server" Path="/%" CheckPermissions="true" CssClass="search-results" />
            <cms:SearchResults ID="repSmartSearch" runat="server" Path="/%" CheckPermissions="true" CssClass="search-results">
                <ItemTemplate>
                    <div class="smart-search">
                        <div class="title">
                            <strong>
                                <a href="javascript:SelectItem(<%#TransformationHelper.HelperObject.GetSearchValue((SearchResultItem)((IDataItemContainer)Container).DataItem, "nodeId")%>, '<%#TransformationHelper.HelperObject.GetSearchValue((SearchResultItem)((IDataItemContainer)Container).DataItem, "DocumentCulture")%>')">
                                    <%#TransformationHelper.HelperObject.SearchHighlight((SearchResultItem)((IDataItemContainer)Container).DataItem, HTMLHelper.HTMLEncode(DataHelper.GetNotEmpty(Eval("Title"), "/")), "<span class=\"highlight\">", "</span>")%> (<%#TransformationHelper.HelperObject.GetSearchValue((SearchResultItem)((IDataItemContainer)Container).DataItem, "DocumentCulture")%>)
                                </a>
                            </strong>
                        </div>
                        <div class="text">
                            <%#TransformationHelper.HelperObject.SearchHighlight((SearchResultItem)((IDataItemContainer)Container).DataItem, HTMLHelper.HTMLEncode(TextHelper.LimitLength(HttpUtility.HtmlDecode(HTMLHelper.StripTags(CMS.Base.Web.UI.ControlsHelper.RemoveDynamicControls(TransformationHelper.HelperObject.GetSearchedContent((SearchResultItem)((IDataItemContainer)Container).DataItem, DataHelper.GetNotEmpty(Eval("Content"), ""))), false, true, " ", "@", "")), 280, "...")), "<span class=\"highlight\">", "</span>")%>
                        </div>
                        <div class="footer">
                            <span class="url">
                                <%# TransformationHelper.HelperObject.SearchHighlight((SearchResultItem)((IDataItemContainer)Container).DataItem, TransformationHelper.HelperObject.SearchResultUrl((SearchResultItem)((IDataItemContainer)Container).DataItem, true),"<span class=\"highlight\">","</span>") %>
                            </span>
                            <span class="date">
                                <%# TransformationHelper.HelperObject.GetDateTimeString(this, ValidationHelper.GetDateTime(Eval("Created"), DateTimeHelper.ZERO_TIME), true) %>
                            </span>
                        </div>
                    </div>
                </ItemTemplate>
            </cms:SearchResults>
            <cms:UIPager runat="server" ID="pagerElem" />
        </asp:Panel>
    </asp:Panel>

    <script type="text/javascript">
        //<![CDATA[
        // Select item action for transformation
        function SelectItem(nodeId, culture) {
            if (nodeId != 0) {
                var origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : ''),
                    newHref = origin + "<%= SystemContext.ApplicationPath %>" + "/Admin/CMSAdministration.aspx/default.aspx?action=edit&nodeid=" + nodeId + "&culture=" + culture + "<%= ApplicationUrlHelper.GetApplicationHash("cms.content", "content") %>",
                    target = window.parent.parent;

                if (target.location.href !== newHref) {
                    parent.parent.location.href = newHref;
                } else {
                    // Reload topWindow when the only thing that has changed is hash
                    target.location.reload(true);
                }
            }
        }
        //]]>
    </script>

</asp:Content>
