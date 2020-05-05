<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ViewCode.aspx.cs" Inherits="CMSModules_Content_CMSDesk_Validation_ViewCode"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Theme="Default" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="cntContent">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Always">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlCode">
                <cms:ExtendedTextArea runat="server" ID="txtCodeText" EnableViewState="false" EditorMode="Advanced"
                    Language="HTML" Height="521" ShowLineNumbers="true" Width="100%" ReadOnly="true"
                    Wrap="true" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:HiddenField ID="hdnHTML" runat="server" EnableViewState="false" />
</asp:Content>
<asp:Content ContentPlaceHolderID="plcFooter" runat="server" ID="cntFooter">
    <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Primary" ResourceString="general.close" OnClientClick="return CloseDialog();"
        EnableViewState="false" />
</asp:Content>
