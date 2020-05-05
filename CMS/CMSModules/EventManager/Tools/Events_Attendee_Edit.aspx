<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Attendee details" Inherits="CMSModules_EventManager_Tools_Events_Attendee_Edit"
    Theme="Default"  Codebehind="Events_Attendee_Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/EventManager/Controls/EventAttendees_Edit.ascx" TagName="AttendeesEdit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:AttendeesEdit runat="server" ID="attendeeEdit" />
</asp:Content>