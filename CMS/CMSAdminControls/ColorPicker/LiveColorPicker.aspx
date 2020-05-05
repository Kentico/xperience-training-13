<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_ColorPicker_LiveColorPicker"
    Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    Title="Color picker"  Codebehind="LiveColorPicker.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/ColorPicker/ColorPicker.ascx" TagName="ColorPicker"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ColorPicker ID="colorPickerElem" runat="server" />    

</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClientClick="CP_SetColor(); return false;"
            ResourceString="general.select" /><cms:LocalizedButton ID="btnCancel" runat="server"
               ButtonStyle="Primary" OnClientClick="CloseWindow(); return false;" ResourceString="general.cancel" />
    </div>
</asp:Content>
