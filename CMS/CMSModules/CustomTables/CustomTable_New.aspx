<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_CustomTables_CustomTable_New"
    Title="Untitled Page" Theme="Default"  Codebehind="CustomTable_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/NewClassWizard.ascx" TagName="NewDocWizard" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:NewDocWizard ID="newTableWizard" runat="server" />
</asp:Content>
