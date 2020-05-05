<%@ Page Title="Module edit - User interface - New" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true" Theme="Default" Inherits="CMSModules_Modules_Pages_Module_UserInterface_New"  Codebehind="New.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UIProfiles/UIElementEdit.ascx" TagName="UIElementEdit"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcBeforeBody" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plcControls" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:LocalizedLabel ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" Visible="false" />
    <cms:UIElementEdit ID="editElem" runat="server" />
</asp:Content>
