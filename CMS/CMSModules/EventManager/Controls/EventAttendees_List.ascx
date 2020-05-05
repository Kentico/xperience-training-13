<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/EventManager/Controls/EventAttendees_List.ascx.cs"
    Inherits="CMSModules_EventManager_Controls_EventAttendees_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:UniGrid runat="server" ID="UniGrid" OrderBy="AttendeeEmail" IsLiveSite="false" />
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
