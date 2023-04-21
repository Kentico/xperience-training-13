<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_Export___objects__"
     Codebehind="__objects__.ascx.cs" %>

<div class="wizard-section">
    <asp:Panel runat="server" ID="pnlInfo" CssClass="content-block">
        <asp:Label ID="lblInfo" runat="server" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlSelection" CssClass="content-block">
        <cms:LocalizedHeading runat="server" ID="headGlobalSelection" ResourceString="importobjects.selection" Level="4" EnableViewState="true" />
        <div class="control-group-inline">
            <cms:CMSButton ID="lnkSelectDefault" runat="server" OnClick="lnkSelectDefault_Click" ButtonStyle="Default" />
            <cms:CMSButton ID="lnkSelectAll" runat="server" OnClick="lnkSelectAll_Click" ButtonStyle="Default" />
            <cms:CMSButton ID="lnkSelectNone" runat="server" OnClick="lnkSelectNone_Click" ButtonStyle="Default" />
        </div>
    </asp:Panel>
</div>
<asp:Literal ID="ltlScript" EnableViewState="false" runat="Server" />
