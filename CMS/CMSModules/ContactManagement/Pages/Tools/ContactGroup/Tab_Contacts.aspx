<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Contacts.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_ContactGroup_Tab_Contacts"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Contact group properties - Members" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/ContactGroup/Contacts.ascx" TagName="Contacts" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Contacts ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>