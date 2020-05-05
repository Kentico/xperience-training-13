<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="MetaFileDialog.aspx.cs" Inherits="CMSModules_AdminControls_Controls_MetaFiles_MetaFileDialog"
     MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="MetaFile - Attachments" Theme="Default" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileList.ascx" TagPrefix="cms" TagName="FileList" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="ClearBorder Newsletters">
        <cms:FileList ID="AttachmentList" runat="server" ShortID="fl" />
    </div>
    <asp:HiddenField ID="hdnCount" runat="server" EnableViewState="false" Value="-1" />
</asp:Content>