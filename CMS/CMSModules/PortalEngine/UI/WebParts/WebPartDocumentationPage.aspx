<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartDocumentationPage"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master" Title="Web part - Documentation"
    Theme="Default"  Codebehind="WebPartDocumentationPage.aspx.cs" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/WebPartDocumentation.ascx" TagName="WebPartDocumentation" TagPrefix="cms" %>    
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
   <cms:WebPartDocumentation runat="server" ID="ucWebPartDocumentation" />
</asp:Content>
