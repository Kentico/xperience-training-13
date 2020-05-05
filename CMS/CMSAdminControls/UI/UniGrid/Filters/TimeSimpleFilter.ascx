<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSAdminControls_UI_UniGrid_Filters_TimeSimpleFilter"  Codebehind="TimeSimpleFilter.ascx.cs" %>
<div class="control-group-inline">
    <cms:DateTimePicker ID="dtmTimeFrom" runat="server" />
    <cms:LocalizedLabel ID="lblTimeBetweenAnd" runat="server" ResourceString="eventlog.timebetweenand" CssClass="form-control-text" />
</div>
<div class="control-group-inline">
    <cms:DateTimePicker ID="dtmTimeTo" runat="server" />
</div>