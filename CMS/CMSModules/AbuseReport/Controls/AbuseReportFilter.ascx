<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="AbuseReportFilter.ascx.cs"
    Inherits="CMSModules_AbuseReport_Controls_AbuseReportFilter" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlFilter" DefaultButton="btnShow">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTitle" runat="server" ResourceString="general.title" DisplayColon="true"
                    EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtTitle" runat="server" MaxLength="50" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblStatus" runat="server" ResourceString="abuse.status" DisplayColon="true"
                    EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSDropDownList ID="drpStatus" runat="server" AutoPostBack="false" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcSites" runat="Server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSites" runat="server" ResourceString="general.site" DisplayColon="true"
                        EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton ID="btnReset" ButtonStyle="Default" runat="server" EnableViewState="false" />                
                <cms:LocalizedButton ID="btnShow" ResourceString="General.Filter" runat="server"  ButtonStyle="Primary" EnableViewState="false" OnClick="btnShow_Click" />
            </div>
        </div>
    </div>
</asp:Panel>
