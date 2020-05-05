<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="New.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Account properties" Inherits="CMSModules_ContactManagement_Pages_Tools_Account_New" Theme="Default" %>
    
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Account/Edit.ascx" TagName="AccountEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:AccountEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>