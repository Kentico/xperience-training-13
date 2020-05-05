<%@ Page Title="Module edit - User interface - General" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    EnableEventValidation="false" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Module_UserInterface_General"
    Theme="Default"  Codebehind="General.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIElementEdit.ascx" TagName="UIElementEdit"
    TagPrefix="cms" %>
<asp:Content ID="Content3" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:UIElementEdit ID="editElem" runat="server" />
</asp:Content>
