<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="New.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Contact properties" Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_New" Theme="Default" %>
    
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/Edit.ascx" TagName="ContactEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ContactEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>