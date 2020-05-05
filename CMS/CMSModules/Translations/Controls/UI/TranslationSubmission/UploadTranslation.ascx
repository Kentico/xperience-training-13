<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_Controls_UI_TranslationSubmission_UploadTranslation"
     Codebehind="UploadTranslation.ascx.cs" %>
<asp:Panel runat="server" ID="pnlContent">
    <cms:LocalizedLabel runat="server" ID="lblInfo" ResourceString="translationservice.uploadtranslationinfo" /><br />
    <br />
    <cms:CMSFileUpload runat="server" ID="uploadElem" /><br />
</asp:Panel>