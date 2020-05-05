<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/GeneralSelector.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_UIControls_GeneralSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel runat="server" ID="lblText" DisplayColon="true"  CssClass="control-label" EnableViewState="false" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:UniSelector runat="server" ID="selectorElem" />
        </div>
    </div>
</div>
