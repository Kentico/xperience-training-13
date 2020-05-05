<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSFormControls_Filters_DocTypeFilter"
     Codebehind="DocTypeFilter.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblClassType" runat="server" ResourceString="queryselection.lblclasstype"
                EnableViewState="false" AssociatedControlID="drpClassType" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:CMSDropDownList ID="drpClassType" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="drpClassType_SelectedIndexChanged" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblDocType" runat="server" EnableViewState="false" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:UniSelector ID="uniSelector" runat="server" SelectionMode="SingleDropDownList" ReturnColumnName="ClassID" ResourcePrefix="allowedclasscontrol"
                             AllowAll="False" AllowEmpty="False" DisplayNameFormat="{%ClassDisplayName%} ({%ClassName%})" DialogWindowName="DocumentTypeSelectionDialog"
                             OnOnSelectionChanged="uniSelector_OnSelectionChanged" />
        </div>
    </div>
</div>
