<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Selectors_TagSelector"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Metadata - Select tags" CodeBehind="TagSelector.aspx.cs" %>
<%@ Register Src="~/CMSModules/TagGroups/Controls/TagSelectorDialog.ascx" TagPrefix="cms" TagName="TagSelectorDialog" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="Server">
    <cms:TagSelectorDialog runat="server" id="tagSelectorDialog" />
</asp:Content>
