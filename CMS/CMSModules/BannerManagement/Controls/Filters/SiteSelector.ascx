<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SiteSelector.ascx.cs" Inherits="CMSModules_BannerManagement_Controls_Filters_SiteSelector" %>
<asp:Panel CssClass="Filter" runat="server" ID="pnlSearch">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"  CssClass="control-label"
                ResourceString="General.Site" />
            </div>
            <div class="filter-form-value-cell">
                <cms:SiteOrGlobalSelector ID="siteSelector" runat="server" IsLiveSite="false" />
            </div>
        </div>
    </div>
</asp:Panel>
