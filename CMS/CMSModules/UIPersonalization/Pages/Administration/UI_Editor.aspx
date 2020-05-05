<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_UIPersonalization_Pages_Administration_UI_Editor"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="UI Personalization - Dialogs"  Codebehind="UI_Editor.aspx.cs" %>

<%@ Register Src="~/CMSModules/UIPersonalization/Controls/UIPersonalization.ascx" TagName="UIPersonalization" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIPersonalization runat="server" ID="editElem" IsLiveSite="false" SingleModule="true" ShowAllElementsFromModuleSection="true"/>
</asp:Content>
