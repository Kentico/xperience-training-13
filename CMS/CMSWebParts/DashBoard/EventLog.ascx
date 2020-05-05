<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/DashBoard/EventLog.ascx.cs"
    Inherits="CMSWebParts_DashBoard_EventLog" %>
<%@ Register Src="~/CMSModules/EventLog/Controls/EventLog.ascx"
    TagName="EventLog" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>

<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />        
<cms:EventLog ID="eventLog" runat="server" />
