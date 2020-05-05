<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Attendee list" Inherits="CMSModules_EventManager_Tools_Events_Attendee_List"
    Theme="Default"  Codebehind="Events_Attendee_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/EventManager/Controls/EventAttendees_List.ascx" TagName="AttendeesList"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
        <cms:AttendeesList ID="attendeesList" runat="server" IsLiveSite="false" />
</asp:Content>
