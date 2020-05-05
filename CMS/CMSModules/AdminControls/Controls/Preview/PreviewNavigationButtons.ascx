<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="PreviewNavigationButtons.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Preview_PreviewNavigationButtons" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Preview/Preview.ascx" TagName="Preview"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlHeaderSeparator" CssClass="HeaderSeparator">
    &nbsp;
</asp:Panel>
<div class="Preview">
    <asp:Panel ID="pnlMain" CssClass="SplitToolbar Vertical" runat="server">
        <div class="LeftAlign">
            <cms:CMSButtonGroup ID="buttons" runat="server" />

        </div>
        <div class="RightAlign" runat="server" id="pnlButtons">
            <cms:Preview runat="server" ID="ucButtons" />
        </div>
        <div class="Clear"></div>
    </asp:Panel>
</div>
