<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSModules_Chat_Pages_ChatSupportSettings" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default"  Codebehind="ChatSupportSettings.aspx.cs" %>

<%@ Register Src="~/CMSModules/Chat/Controls/UI/ChatSupportCannedResponse/List.ascx" TagName="ChatSupportCannedResponseList" TagPrefix="cms" %>

<asp:Content runat="server" ContentPlaceHolderID="plcContent" ID="content">
    <asp:Panel runat="server" ID="pnlCannedResponseList">
        <cms:ChatSupportCannedResponseList ID="listElem" runat="server" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>