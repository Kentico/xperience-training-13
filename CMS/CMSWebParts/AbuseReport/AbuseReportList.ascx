<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/AbuseReport/AbuseReportList.ascx.cs"
    Inherits="CMSWebParts_AbuseReport_AbuseReportList" %>
<%@ Register Src="~/CMSModules/AbuseReport/Controls/AbuseReportListAndEdit.ascx"
    TagName="AbuseReportList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:AbuseReportList ID="ucAbuseReportList" runat="server" />
