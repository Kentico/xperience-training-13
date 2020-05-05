<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Widgets_UI_Widget_Edit_Security" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Widget Edit - Security" Theme="Default"  Codebehind="Widget_Edit_Security.aspx.cs" %>

<%@ Register Src="~/CMSModules/Widgets/Controls/WidgetSecurity.ascx" TagName="WidgetSecurity"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:WidgetSecurity ID="widgetSecurity" runat="server" />
</asp:Content>
