<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Filters_SiteFilter"
     Codebehind="SiteFilter.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>

<asp:Panel CssClass="form-horizontal form-filter" runat="server" ID="pnlSearch">
    <asp:Panel CssClass="form-group" runat="server" ID="pnlGroup">
        <asp:PlaceHolder runat="server" ID="plcLabel" EnableViewState="false">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSite" runat="server" EnableViewState="false" AssociatedControlID="siteSelector"
                    ResourceString="general.site" DisplayColon="true" CssClass="control-label" />
            </div>
        </asp:PlaceHolder>
        <asp:Panel CssClass="filter-form-value-cell" runat="server" ID="pnlSelector">
            <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>