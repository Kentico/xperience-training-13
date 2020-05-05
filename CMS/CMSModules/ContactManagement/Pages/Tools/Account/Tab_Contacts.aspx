<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Contacts.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Account_Tab_Contacts"
MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Account properties - Contacts" Theme="Default"  %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/Contacts.ascx" TagName="Contacts" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Contacts ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>