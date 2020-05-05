<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Translation service properties" Inherits="CMSModules_Translations_Pages_Tools_TranslationService_Edit" Theme="Default"  Codebehind="Edit.aspx.cs" %>
<%@ Register Src="~/CMSModules/Translations/Controls/UI/TranslationService/Edit.ascx"
    TagName="TranslationServiceEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:TranslationServiceEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>