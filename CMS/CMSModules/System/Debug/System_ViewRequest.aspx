<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_System_Debug_System_ViewRequest" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" Title="Request details"  Codebehind="System_ViewRequest.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlBody">
        <div style="width: 97%">
            <cms:CMSPlaceHolder runat="server" ID="plcLogs" EnableViewState="false" />
        </div>
    </asp:Panel>
</asp:Content>
