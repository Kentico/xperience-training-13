<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Cultures_Pages_ResourceString_Edit"
    ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default"  Codebehind="Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Cultures/Controls/UI/ResourceStringEdit.ascx"
    TagName="ResourceStringEdit" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ResourceStringEdit runat="server" ID="resEditor" DefaultTranslationRequired="true" />
</asp:Content>