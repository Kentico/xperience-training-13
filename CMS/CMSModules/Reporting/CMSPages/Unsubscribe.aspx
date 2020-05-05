<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Unsubscribe.aspx.cs" Inherits="CMSModules_Reporting_CMSPages_Unsubscribe"
    MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master" Theme="Default" %>

<asp:Content runat="server" ID="pnlContent" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlInfo" CssClass="ReportUnsubscriptionInfo">
        <asp:Label ID="lblInfo" runat="server" />
    </asp:Panel>
    <cms:CMSButton runat="server" ID="btnUnsubscribe" OnClick="btnUnsubscribe_click" ButtonStyle="Default" />
</asp:Content>
