<%@ Page Title="Localize string" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSFormControls_Selectors_LocalizableTextBox_LocalizeString"
     Codebehind="LocalizeString.aspx.cs" %>

<%@ Register Src="~/CMSModules/Cultures/Controls/UI/ResourceStringEdit.ascx"
    TagName="ResourceStringEdit" TagPrefix="cms" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:ResourceStringEdit runat="server" ID="resEditor" ShowSaveButton="false" DefaultTranslationRequired="true" />
    </asp:Panel>
</asp:Content>
