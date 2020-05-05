<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_AbuseReport_Controls_InlineAbuseReport"  Codebehind="InlineAbuseReport.ascx.cs" %>
<cms:SecurityPanel ID="ucWrapPanel" runat="server">
    <cms:LocalizedHyperlink ID="lnkText" runat="server" ResourceString="abuse.reportabuse"
        CssClass="InlineAbuseLink" EnableViewState="false" />
</cms:SecurityPanel>
