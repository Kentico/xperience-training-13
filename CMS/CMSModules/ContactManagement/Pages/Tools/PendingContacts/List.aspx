<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="List.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_ContactManagement_Pages_Tools_PendingContacts_List" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/PendingContacts.ascx" TagName="PendingContacts" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading ID="headTitle" Level="4" runat="server" CssClass="listing-title" EnableViewState="false" DisplayColon="true" ResourceString="ma.pendingcontact.list" />
    <cms:PendingContacts ID="listContacts" runat="server" />
</asp:Content>
