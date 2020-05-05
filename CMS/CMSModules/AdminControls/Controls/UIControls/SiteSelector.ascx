<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/SiteSelector.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_SiteSelector" %>
<%@ Register TagName="siteselector" TagPrefix="cms" Src="~/CMSFormControls/Sites/SiteSelector.ascx" %>
<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel runat="server" ID="lblSite" ResourceString="general.site" DisplayColon="true" CssClass="control-label" EnableViewState="false" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:siteselector runat="server" ID="SiteSelector" Visible="false" />
            <cms:SiteOrGlobalSelector runat="server" ID="SiteOrGlobal" Visible="false" />
        </div>
    </div>
</div>

