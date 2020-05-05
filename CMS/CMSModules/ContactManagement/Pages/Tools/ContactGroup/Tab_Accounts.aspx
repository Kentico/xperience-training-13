<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Accounts.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_ContactGroup_Tab_Accounts"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Contact group properties - Members" Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/ContactGroup/Accounts.ascx" TagName="Accounts" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Accounts ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>