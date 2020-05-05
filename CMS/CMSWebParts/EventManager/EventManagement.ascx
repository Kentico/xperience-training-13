<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/EventManager/EventManagement.ascx.cs"
    Inherits="CMSWebParts_EventManager_EventManagement" %>
<%@ Register Src="~/CMSModules/EventManager/Controls/EventManagement.ascx" TagName="EventManager"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:EventManager runat="server" ID="EventManager" />
