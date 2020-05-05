<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Class_New"
    Title="Untitled Page" Theme="Default"  Codebehind="New.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/NewClassWizard.ascx" TagName="NewDocWizard" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:NewDocWizard ID="newDocWizard" runat="server" SystemDevelopmentMode="true" Mode="Class" />
</asp:Content>
