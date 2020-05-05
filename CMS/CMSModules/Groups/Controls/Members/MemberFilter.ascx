<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Controls_Members_MemberFilter"  Codebehind="MemberFilter.ascx.cs" %>

<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblMemberName" AssociatedControlID="txtMemberName" ResourceString="editroleusers.username"
                DisplayColon="true" runat="server" EnableViewState="false" />
        </div>
        <div class="filter-form-condition-cell">
            <cms:LocalizedLabel ID="lblMemberDDL" AssociatedControlID="drpMemberName" ResourceString="filter.mode"
                runat="server" EnableViewState="false" CssClass="sr-only" />
            <cms:CMSDropDownList ID="drpMemberName" runat="server" />
        </div>
        <div class="filter-form-value-cell">
            <cms:CMSTextBox ID="txtMemberName" runat="server" MaxLength="100" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblMemberStatus" AssociatedControlID="drpMemberStatus" ResourceString="groups.status"
                runat="server" DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:CMSDropDownList ID="drpMemberStatus" runat="server" />
        </div>
    </div>
    <div class="form-group form-group-buttons">
        <div class="filter-form-buttons-cell-wide">
            <cms:LocalizedButton ResourceString="general.filter" ID="btnSearch" runat="server"
                ButtonStyle="Primary" EnableViewState="false" />
        </div>
    </div>
</div>