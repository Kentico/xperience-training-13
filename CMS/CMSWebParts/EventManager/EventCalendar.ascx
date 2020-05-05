<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_EventManager_EventCalendar"  Codebehind="~/CMSWebParts/EventManager/EventCalendar.ascx.cs" %>
<div class="Calendar">
    <cms:CMSCalendar ID="calItems" runat="server" EnableViewState="false" />
</div>
<div class="EventDetail">
    <cms:CMSRepeater ID="repEvent" runat="server" Visible="false" StopProcessing="true" EnableViewState="false" />
</div>
