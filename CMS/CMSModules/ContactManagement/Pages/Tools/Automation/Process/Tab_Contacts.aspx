<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Automation process – Contacts" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Contacts"
    Theme="Default"  Codebehind="Tab_Contacts.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/Contacts.ascx" TagName="Contacts" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx"
    TagName="ContactSelector" TagPrefix="cms" %>

<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <div class="control-group-inline header-actions-container">
        <cms:ContactSelector ID="ucSelector" runat="server" Enabled="false" IsLiveSite="false" />
        <cms:HeaderActions ID="headerActions" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:Contacts ID="listContacts" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
