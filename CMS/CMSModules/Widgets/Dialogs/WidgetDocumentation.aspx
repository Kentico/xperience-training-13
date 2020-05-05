<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="WidgetDocumentation.aspx.cs"
    Inherits="CMSModules_Widgets_Dialogs_WidgetDocumentation" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/WebPartDocumentation.ascx"
    TagName="WebPartDocumentation" TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:WebPartDocumentation runat="server" ID="ucWebPartDocumentation" />
</asp:Content>
