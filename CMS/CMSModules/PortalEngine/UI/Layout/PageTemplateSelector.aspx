<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false"
    Inherits="CMSModules_PortalEngine_UI_Layout_PageTemplateSelector" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" Title="Select page template"  Codebehind="PageTemplateSelector.aspx.cs" %>

<%@ Register Src="~/CMSModules/PortalEngine/Controls/Layout/PageTemplateSelector.ascx"
    TagName="PageTemplateSelector" TagPrefix="cms" %>

<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="content">
    <cms:PageTemplateSelector runat="server" ID="selectElem" ShowEmptyCategories="false" IsLiveSite="false" />
</asp:Content>
