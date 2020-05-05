<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_Information"
    Theme="Default" EnableEventValidation="false"  Codebehind="Information.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/Debug/SecurityLog.ascx" TagName="SecurityLog"
    TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:LocalizedHeading Level="4" ClientIDMode="Static" ID="hdnPermission" runat="server" />
    <asp:Panel ID="pnlBody" runat="server">
        <asp:Panel ID="pnlContent" runat="server">
            <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="InfoLabel" />
            <cms:SecurityLog ID="logSec" runat="server" InitFromRequest="true" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>
