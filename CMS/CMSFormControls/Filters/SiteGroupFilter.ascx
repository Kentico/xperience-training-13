<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SiteGroupFilter.ascx.cs"
    Inherits="CMSFormControls_Filters_SiteGroupFilter" %>
<%@ Register Src="~/CMSFormControls/Filters/SiteFilter.ascx" TagName="SiteFilter"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlSearch">
    <div class="form-horizontal form-filter">
        <asp:PlaceHolder ID="plcSite" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteFilter ID="siteFilter" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcGroup" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblGroup" runat="server" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSDropDownList ID="drpGroup" runat="server" OnSelectedIndexChanged="Filter_Changed"
                        AutoPostBack="true" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Panel>
