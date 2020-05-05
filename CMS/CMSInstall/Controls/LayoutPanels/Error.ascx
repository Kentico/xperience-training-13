<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Error.ascx.cs" Inherits="CMSInstall_Controls_LayoutPanels_Error" %>
<%@ Register TagPrefix="cms" TagName="Help" Src="~/CMSAdminControls/UI/PageElements/Help.ascx" %>
<asp:PlaceHolder ID="plcError" runat="server" EnableViewState="False">
    <div class="install-error">
        <cms:Help ID="hlpTroubleshoot" runat="Server" Visible="false" IconCssClass="cms-icon-80" />
        <asp:Label ID="lblError" runat="server" CssClass="error-label hidden" />
    </div>
</asp:PlaceHolder>
