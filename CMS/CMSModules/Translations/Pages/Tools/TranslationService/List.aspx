<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Translation service list"
    Inherits="CMSModules_Translations_Pages_Tools_TranslationService_List" Theme="Default"  Codebehind="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/Translations/Controls/UI/TranslationService/List.ascx" TagName="TranslationServiceList" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:TranslationServiceList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
