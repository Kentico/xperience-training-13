<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_RegistrationApproval"  Codebehind="RegistrationApproval.ascx.cs" %>
<cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
<cms:LocalizedLabel runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false" />
<cms:CMSButton runat="server" ID="btnConfirm" EnableViewState="false" ButtonStyle="Default" OnClick="btnConfirm_Click" />

