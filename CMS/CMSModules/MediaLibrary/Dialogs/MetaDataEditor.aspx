<%@ Page Title="Edit metadata" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true"  Codebehind="MetaDataEditor.aspx.cs" Inherits="CMSModules_MediaLibrary_Dialogs_MetaDataEditor"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaFileMetaDataEditor.ascx"
    TagName="MediaFileMetaDataEditor" TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:MediaFileMetaDataEditor ID="metaDataEditor" runat="server" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
