<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_PageElements_HeaderActions"
     Codebehind="HeaderActions.ascx.cs" %>
<asp:PlaceHolder ID="plcMenu" runat="server">
    <cms:CMSUpdatePanel ID="pnlUp" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlActions" runat="server" Visible="true" EnableViewState="false"
                CssClass="btn-actions">
            </asp:Panel>
            <asp:Panel ID="pnlAdditionalControls" runat="server" Visible="true" EnableViewState="false"
                CssClass="dont-check-changes">
                <asp:PlaceHolder ID="plcAdditionalControls" runat="server" />
            </asp:Panel>
            <asp:Panel runat="server" Visible="false" ID="pnlClear" CssClass="clearfix">
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:PlaceHolder>
