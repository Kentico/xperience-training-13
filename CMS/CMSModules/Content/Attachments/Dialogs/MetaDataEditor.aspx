<%@ Page Title="Edit metadata" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true"  Codebehind="MetaDataEditor.aspx.cs" Inherits="CMSModules_Content_Attachments_Dialogs_MetaDataEditor"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/MetaDataEdit.ascx" TagName="MetaDataEditor"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:MetaDataEditor ID="metaDataEditor" runat="server" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
