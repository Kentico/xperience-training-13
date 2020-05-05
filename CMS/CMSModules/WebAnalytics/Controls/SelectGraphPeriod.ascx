<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectGraphPeriod.ascx.cs"
    Inherits="CMSModules_WebAnalytics_Controls_SelectGraphPeriod" %>
<asp:Panel ID="pnlRange" runat="server" CssClass="control-group-inline date-range-selector">
    <cms:RangeDateTimePicker runat="server" ID="ucRangeDatePicker" />
    <cms:LocalizedButton runat="server" ID="btnUpdate" ResourceString="general.update" ButtonStyle="Primary" />
</asp:Panel>
