<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Subsidiaries.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Account_Tab_Subsidiaries"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Account properties - Braches"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/Subsidiaries.ascx"
    TagName="Branches" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Branches ID="branches" runat="server" />
</asp:Content>
