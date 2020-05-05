<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalSimplePage.master"
    Inherits="CMSModules_Chat_CMSPages_OfflineSupportForm"  Codebehind="OfflineSupportForm.aspx.cs"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Chat/Controls/UI/SupportOfflineMessage/Edit.ascx" TagName="SupportOfflineMessageEdit" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="cntContent">
    <div class="PageContent">
        <cms:SupportOfflineMessageEdit ID="messageEditElem" runat="server" IsLiveState="true" />
        <cms:LocalizedLabel ID="lblStatusMessage" runat="server" CssClass="ChatMailFormStatus" />

        <div id="divFooter" class="PageFooterLine" style="min-height: 0px;">
            <asp:Panel runat="server" ID="pnlFooter">
                <div class="FloatRight">
                    <cms:LocalizedButton ID="btnSubmit" runat="server" ResourceString="chat.sendmessage" ButtonStyle="Primary" OnClick="btnSubmit_Click" />
                    <cms:LocalizedButton ID="btnClose" runat="server" ResourceString="chat.close" ButtonStyle="Primary" OnClientClick="window.close();" />
                </div>
            </asp:Panel>
            <div class="ClearBoth">&nbsp;</div>
        </div>
    </div>
</asp:Content>
