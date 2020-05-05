<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="UnlockUserAccount.ascx.cs" Inherits="CMSModules_Membership_Controls_UnlockUserAccount" %>
<cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
<cms:LocalizedLabel runat="server" ID="lblInfo" EnableViewState="false" />
<cms:LocalizedButton runat="server" ID="btnConfirm" EnableViewState="false" ButtonStyle="Default"
    onclick="btnConfirm_Click" />