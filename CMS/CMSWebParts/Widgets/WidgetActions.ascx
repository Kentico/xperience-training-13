<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Widgets_WidgetActions"
     Codebehind="~/CMSWebParts/Widgets/WidgetActions.ascx.cs" %>
<asp:Panel runat="server" ID="pnlWidgetActions">
    <div class="cms-bootstrap">
        <asp:Panel runat="server" CssClass="btn-actions" ID="pnlActions">
            <cms:CMSButton ID="btnAddWidget" ButtonStyle="Primary" runat="server" Visible="false" EnableViewState="false" />
            <cms:CMSButton ID="btnReset" ButtonStyle="Default" runat="server" Visible="false" EnableViewState="false" />
        </asp:Panel>
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlClear" CssClass="ClearBoth">
</asp:Panel>
