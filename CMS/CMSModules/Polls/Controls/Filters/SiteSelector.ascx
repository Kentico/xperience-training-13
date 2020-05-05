<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SiteSelector.ascx.cs"
    Inherits="CMSModules_Polls_Controls_Filters_SiteSelector" %>

<asp:Panel CssClass="form-horizontal form-filter" runat="server" ID="pnlSearch">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" AssociatedControlID="siteSelector" runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"
                ResourceString="General.Site" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:SiteOrGlobalSelector ID="siteSelector" runat="server" IsLiveSite="false" />
        </div>
    </div>
</asp:Panel>