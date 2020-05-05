<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Translation submission list" Inherits="CMSModules_Translations_Pages_Tools_TranslationSubmission_List"
    Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Translations/Controls/UI/TranslationSubmission/List.ascx"
    TagName="TranslationSubmissionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModuleInfo" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisabledModuleInfo runat="server" ID="DisabledModuleInfo" TestSettingKeys="CMSEnableTranslations" />
    <cms:TranslationSubmissionList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
