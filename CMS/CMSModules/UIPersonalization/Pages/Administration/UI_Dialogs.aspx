<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false"
    Inherits="CMSModules_UIPersonalization_Pages_Administration_UI_Dialogs" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="UI Personalization - Editor"  Codebehind="UI_Dialogs.aspx.cs" %>

<%@ Register Src="~/CMSModules/UIPersonalization/Controls/UIPersonalization.ascx" TagName="UIPersonalization" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIPersonalization runat="server" ID="editElem" IsLiveSite="false" />
</asp:Content>
