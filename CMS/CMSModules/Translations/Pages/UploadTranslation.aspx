<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Pages_UploadTranslation"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
     Codebehind="UploadTranslation.aspx.cs" Title="Upload translation" %>

<%@ Register Src="~/CMSModules/Translations/Controls/UI/TranslationSubmission/UploadTranslation.ascx"
    TagName="UploadTranslation" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UploadTranslation runat="server" ID="uploadElem" />
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
<asp:Content ContentPlaceHolderID="plcFooter" runat="server" ID="plcFooter">
    <cms:LocalizedButton runat="server" ID="btnUpload" EnableViewState="false" ResourceString="general.upload"
        ButtonStyle="Primary" />
</asp:Content>
