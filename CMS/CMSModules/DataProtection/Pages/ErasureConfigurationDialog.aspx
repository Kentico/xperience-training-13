<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErasureConfigurationDialog.aspx.cs" 
    Inherits="CMSModules_DataProtection_Pages_ErasureConfigurationDialog" 
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" 
    Theme="Default" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MessagesPlaceHolder ID="plcMessages" runat="server" IsLiveSite="false"></cms:MessagesPlaceHolder>
    
    <asp:PlaceHolder ID="plcErasureConfiguration" runat="server" />
</asp:Content>
<asp:Content ID="footer" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Default" OnClientClick="CloseDialog()" EnableViewState="false" ResourceString="general.close" />
        <cms:LocalizedButton ID="btnDelete" runat="server" ButtonStyle="Primary" OnClick="btnDelete_Click" EnableViewState="false" ResourceString="general.delete" />
    </div>
</asp:Content>
