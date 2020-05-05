<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Posts_PostApproveFooter"  Codebehind="PostApproveFooter.ascx.cs" %>

<asp:literal id="ltrScript" runat="server"></asp:literal>
    <cms:CMSButton runat="server" ID="btnDelete" EnableViewState="false" ButtonStyle="Primary"
        OnClick="btnDelete_Click" />
    <cms:CMSButton runat="server" ID="btnUnsubscribe" EnableViewState="false" ButtonStyle="Primary"
        OnClick="btnUnsubscribe_Click" />
    <cms:CMSButton runat="server" ID="btnApprove" EnableViewState="false" ButtonStyle="Primary"
        OnClick="btnApprove_Click" />