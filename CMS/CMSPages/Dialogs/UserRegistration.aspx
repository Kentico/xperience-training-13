<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSPages_Dialogs_UserRegistration"  Codebehind="UserRegistration.aspx.cs" Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Membership/Controls/RegistrationApproval.ascx"
    TagName="RegistrationApproval" TagPrefix="cms" %>
<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server">
    <cms:RegistrationApproval ID="registrationApproval" runat="server" Visible="true" />
</asp:Content>
