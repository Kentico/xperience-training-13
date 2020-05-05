<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="PendingContacts.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_ContactManagement_Pages_Tools_PendingContacts_MyPendingContacts_PendingContacts" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/PendingContacts.ascx" TagName="PendingContacts" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PendingContacts ID="listContacts" runat="server" ShowOnlyMyPendingContacts="True" />
</asp:Content>
