<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SQLList.ascx.cs" Inherits="CMSModules_SystemTables_Controls_Views_SQLList" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="SimpleFilter"
    TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayName" runat="server" Visible="true"
                ResourceString="general.name" DisplayColon="true" />
        </div>
        <cms:SimpleFilter ID="fltViews" runat="server" />
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblIsCustom" runat="server" Visible="true"
                ResourceString="systbl.view.iscustom" DisplayColon="true" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:CMSDropDownList ID="drpCustom" runat="server" CssClass="DropDownFieldFilter" />
        </div>
    </div>
    <div class="form-group form-group-buttons">
        <div class="filter-form-buttons-cell-wide">
            <cms:LocalizedButton ID="btnShow" runat="server" ButtonStyle="Primary" OnClick="btnShow_Click"
                ResourceString="general.search" />
        </div>
    </div>
</div>
<cms:UniGrid ID="gridViews" runat="server" IsLiveSite="false" />