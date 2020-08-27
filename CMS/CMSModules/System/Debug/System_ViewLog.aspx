<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_ViewLog"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    MaintainScrollPositionOnPostback="true"  Codebehind="System_ViewLog.aspx.cs" %>

<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcActions">
    <asp:Panel runat="server" ID="pnlCancel" Visible="False" CssClass="control-group-inline header-actions-container" EnableViewState="False">
        <cms:CMSButton runat="server" ID="btnCancel" ButtonStyle="Primary" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel runat="server" ID="pnlBody" UpdateMode="Conditional" EnableViewState="false">
        <ContentTemplate>
            <asp:Literal runat="server" ID="litMessage" EnableViewState="false" Visible="false" />
            <asp:Button runat="server" ID="btnHiddenRefresh" CausesValidation="false" CssClass="HiddenButton" UseSubmitBehavior="false" />
            <asp:Panel runat="server" ID="pnlLog" Visible="false">
                <asp:Panel ID="pnlAsyncContent" runat="server">
                    <cms:AsyncControl ID="ctlAsync" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Primary" ResourceString="general.close" OnClientClick="return CloseDialog();"
        EnableViewState="false" />
</asp:Content>
