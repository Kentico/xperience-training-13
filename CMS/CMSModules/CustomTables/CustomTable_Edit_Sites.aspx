<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_CustomTables_CustomTable_Edit_Sites" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Custom table edit - Sites"  Codebehind="CustomTable_Edit_Sites.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/ClassSites.ascx" TagName="ClassSites"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ClassSites ID="ClassSites" runat="server" />
</asp:Content>
