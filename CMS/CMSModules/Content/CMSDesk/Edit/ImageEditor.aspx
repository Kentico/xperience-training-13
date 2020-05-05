<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Edit_ImageEditor"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Edit image"  Codebehind="ImageEditor.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/ImageEditor/ImageEditor.ascx" TagName="ImageEditor"
    TagPrefix="cms" %>
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
                <cms:LocalizedButton ID="btnSave" runat="server" ButtonStyle="Primary" ResourceString="general.saveandclose"
                    EnableViewState="false" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
