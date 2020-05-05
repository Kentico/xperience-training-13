<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    AutoEventWireup="true"  Codebehind="Role_Edit_UI_Editor.aspx.cs" Inherits="CMSModules_Membership_Pages_Roles_Role_Edit_UI_Editor"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/UIPersonalization/Controls/UIPersonalization.ascx"
    TagName="UIPersonalization" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIPersonalization runat="server" ID="editElem" SingleModule="true" ShowAllElementsFromModuleSection="true"/>
</asp:Content>
