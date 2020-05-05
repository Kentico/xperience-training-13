<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_View_PollView"  Codebehind="PollView.ascx.cs" %>

<asp:Panel runat="server" ID="pnlControl" CssClass="PollControl" DefaultButton="btnVote" EnableViewState="false">
    <cms:LocalizedLabel runat="server" CssClass="PollTitle" ID="lblTitle" EnableViewState="false" />
    <cms:LocalizedLabel runat="server" ID="lblQuestion" CssClass="PollQuestion" EnableViewState="false" />
    <cms:LocalizedLabel runat="server" ID="lblInfo" CssClass="PollInfo" EnableViewState="false" />
    <asp:Panel runat="server" ID="pnlAnswer" CssClass="PollAnswers" />
    <cms:LocalizedLabel runat="server" ID="lblResult" CssClass="PollResult" EnableViewState="false" Visible="false" />
    <asp:Panel runat="server" ID="pnlFooter" CssClass="PollFooter" EnableViewState="false">
        <cms:LocalizedButton runat="server" ID="btnVote" ButtonStyle="Default" CssClass="PollVoteButton" OnClick="btnVote_OnClick" />
    </asp:Panel>
</asp:Panel>