<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TimeIntervalSelector.ascx.cs" Inherits="CMSModules_SharePoint_FormControls_TimeIntervalSelector" %>

<div class="control-group-inline time-interval-selector">
    <cms:CMSTextBox ID="txtQuantity" runat="server" MaxLength="4" CssClass="input-width-20" />
    <cms:CMSDropDownList ID="drpScale" runat="server" AutoPostBack="true" CssClass="input-width-40" />
</div>
