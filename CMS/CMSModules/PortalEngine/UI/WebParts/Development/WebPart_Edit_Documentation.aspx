<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Documentation"
    Theme="Default" EnableEventValidation="false"  Codebehind="WebPart_Edit_Documentation.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSHtmlEditor ID="htmlText" runat="server" Height="500px" />
    <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click" />
</asp:Content>
