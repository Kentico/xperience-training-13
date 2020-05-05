<%@ Page Title="Submit translation" Language="C#" MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master"
    AutoEventWireup="true"  Codebehind="SubmitTranslation.aspx.cs" Inherits="CMSModules_Translations_CMSPages_SubmitTranslation"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Translations/Controls/UI/TranslationSubmission/UploadTranslation.ascx"
    TagName="UploadTranslation" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UploadTranslation runat="server" ID="uploadElem" CheckContentPermissions="false" IsLiveSite="true" AutoImport="true" />
    <div style="padding-top: 15px;">
        <cms:LocalizedButton runat="server" ID="btnUpload" EnableViewState="false" ResourceString="general.upload"
            ButtonStyle="Primary" IsLiveSite="true" />
    </div>
</asp:Content>
