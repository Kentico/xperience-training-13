<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FontIconSelector.ascx.cs"
    Inherits="CMSAdminControls_UI_FontIconSelector" %>

<asp:Panel ID="pnlIconSelector" runat="server">
    <div class="selected-icon">
        <asp:TextBox ID="txtIconClass" runat="server" CssClass="form-control"></asp:TextBox>
        <button class="btn btn-default dropdown-toggle icon-only" type="button">
            <i aria-hidden="true"></i>
        </button>
    </div>
    <div class="popup-icon-selector">
        <cms:LocalizedLabel ID="lblNoIcons" runat="server" ResourceString="metafileorfonticonselector.noicons" CssClass="no-icons" EnableViewState="false"></cms:LocalizedLabel>
        <asp:Repeater ID="rptFontIcons" runat="server" EnableViewState="false">
            <ItemTemplate><i class="<%# Container.DataItem.ToString() %>" title="<%# Container.DataItem.ToString() %>" aria-hidden="true"></i></ItemTemplate>
        </asp:Repeater>
    </div>
    <asp:HiddenField ID="hdnIconClass" runat="server" />
</asp:Panel>
