<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Events list" Inherits="CMSModules_EventManager_Tools_Events_List" Theme="Default"
     Codebehind="Events_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/EventManager/Controls/EventList.ascx" TagName="EventList"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:EventList ID="eventList" runat="server" />
</asp:Content>