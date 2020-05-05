<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_ColorPicker_ColorPicker"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Color picker"  Codebehind="ColorPicker.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/ColorPicker/ColorPicker.ascx" TagName="ColorPicker"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ColorPicker ID="colorPickerElem" runat="server" />    
</asp:Content>