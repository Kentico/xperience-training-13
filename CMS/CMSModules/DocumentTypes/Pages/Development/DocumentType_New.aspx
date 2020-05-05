<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_New" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Types - New Page Type"  Codebehind="DocumentType_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/NewClassWizard.ascx" TagName="NewDocWizard" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:NewDocWizard ID="newDocWizard" runat="server" />
</asp:Content>
