<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_ImageEditor"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="Edit image"
    Theme="Default"  Codebehind="ImageEditor.aspx.cs" %>

<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/ImageEditor.ascx"
    TagName="ImageEditor" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ImageEditor ID="imageEditor" runat="server" IsLiveSite="false" EnableViewState="true" />
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <cms:CMSUpdatePanel ID="updPanelFooter" runat="server">
        <ContentTemplate>
            <div class="FloatLeft">
                <cms:LocalizedButton ID="btnUndo" runat="server" ButtonStyle="Primary" ResourceString="general.undo"
                    EnableViewState="false" />
                <cms:LocalizedButton ID="btnRedo" runat="server"
                    ButtonStyle="Primary" ResourceString="general.redo" EnableViewState="false" />
            </div>
            <div class="FloatRight">
                <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Primary" ResourceString="general.close"
                    EnableViewState="false" />
                <cms:LocalizedButton ID="btnSave" runat="server" ButtonStyle="Primary" ResourceString="general.saveandclose"
                    EnableViewState="false" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
