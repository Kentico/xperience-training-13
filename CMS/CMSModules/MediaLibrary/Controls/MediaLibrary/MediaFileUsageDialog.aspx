<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MediaFileUsageDialog.aspx.cs" Theme="Default"
    Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileUsageDialog" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaFileUsage.ascx" TagName="FileUsage" 
    TagPrefix="cms" %>

<asp:Content ID="folderEditContent" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdateUsage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:FileUsage ID="fileUsage" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
