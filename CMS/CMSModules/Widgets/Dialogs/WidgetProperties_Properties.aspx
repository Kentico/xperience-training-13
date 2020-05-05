<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Widgets_Dialogs_WidgetProperties_Properties"
    Theme="default" EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="WidgetProperties_Properties.aspx.cs" %>

<%@ Register Src="~/CMSModules/Widgets/Controls/WidgetProperties.ascx" TagName="WidgetProperties"
    TagPrefix="cms" %>

<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlBody">
        <cms:WidgetProperties IsLiveSite="false" ID="widgetProperties" runat="server" />
    </asp:Panel>
</asp:Content>