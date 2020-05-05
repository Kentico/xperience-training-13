<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Accounts.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Accounts"
MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Contact properties - Accounts" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/Accounts.ascx" TagName="Accounts" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Accounts ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>