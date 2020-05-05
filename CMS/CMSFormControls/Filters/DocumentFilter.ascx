<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Filters_DocumentFilter"
     Codebehind="DocumentFilter.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlFilter" DefaultButton="btnShow">
    <div class="form-horizontal form-filter">
        <asp:PlaceHolder ID="plcSites" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSites" runat="server" DisplayColon="true" ResourceString="general.site" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" OnlyRunningSites="false"
                        UseCodeNameForSelection="true" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcPath" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" DisplayColon="true" ResourceString="general.documentname" />
                </div>
                <cms:TextSimpleFilter ID="nameFilter" runat="server" Column="DocumentName" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcClass" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblClass" runat="server" DisplayColon="true" ResourceString="general.documenttype" />
                </div>
                <cms:TextSimpleFilter ID="classFilter" runat="server" Column="ClassDisplayName" />
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnShow" runat="server" ResourceString="general.filter" ButtonStyle="Primary"
                    OnClick="btnShow_Click" />
            </div>
        </div>
    </div>
</asp:Panel>
