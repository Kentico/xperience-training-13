<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Filters_ForumGroupFilter"  Codebehind="ForumGroupFilter.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/FormControls/ForumGroupSelector.ascx" TagName="ForumGroupSelector"
    TagPrefix="cms" %>
<asp:Panel CssClass="Filter" runat="server" ID="pnlSearch">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSite" CssClass="control-label" runat="server" DisplayColon="true" ResourceString="forums.forumgroup" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:ForumGroupSelector ID="forumGroupSelector" runat="server" />
            </div>
        </div>
    </div>
</asp:Panel>
