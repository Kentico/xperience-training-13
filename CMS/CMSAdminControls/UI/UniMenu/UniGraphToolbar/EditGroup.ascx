<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="EditGroup.ascx.cs" Inherits="CMSAdminControls_UI_UniMenu_UniGraphToolbar_EditGroup" %>

<div>
    <div class="UniMenuContent">
        <div class="form-search-container">
            <asp:Label AssociatedControlID="txtSearch" runat="server" CssClass="sr-only">
                <%= GetString("general.search") %>
            </asp:Label>
            <cms:CMSTextBox ID="txtSearch" runat="server" MaxLength="200" EnableViewState="true" WatermarkCssClass="WatermarkText" />
            <i class="icon-magnifier" aria-hidden="true"></i>
        </div>
    </div>
</div>