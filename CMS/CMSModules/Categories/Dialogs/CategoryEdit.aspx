<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="CategoryEdit.aspx.cs" Inherits="CMSModules_Categories_Dialogs_CategoryEdit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Message - Edit" %>

<%@ Register Src="~/CMSModules/Categories/Controls/CategoryEdit.ascx" TagName="CategoryEdit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CategoryEdit ID="catEdit" runat="server" Visible="true" IsLiveSite="false" DisplayOkButton="false" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
