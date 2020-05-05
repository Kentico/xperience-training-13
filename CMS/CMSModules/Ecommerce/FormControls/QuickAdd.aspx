<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="QuickAdd.aspx.cs" Inherits="CMSModules_Ecommerce_FormControls_QuickAdd"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" FormButtonCssClass="Hidden" ID="EditForm" OnOnCreate="OnFormCreate" OnOnAfterDataLoad="OnAfterDataLoad"
        RedirectUrlAfterCreate="QuickAdd.aspx?objectType={?objectType?}&siteId={?siteId?}" CssClass="QuickAddForm" />
</asp:Content>
