<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Events - Send email"
    Inherits="CMSModules_EventManager_Tools_Events_SendEmail" Theme="Default"  Codebehind="Events_SendEmail.aspx.cs" %>

<%@ Register Src="~/CMSModules/EventManager/Controls/EventAttendeesSendEmail.ascx" TagName="EMailSender" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:EMailSender runat="server" ID="emailSender" IsLiveSite="false" />  
</asp:Content>
