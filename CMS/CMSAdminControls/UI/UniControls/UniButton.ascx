<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="UniButton.ascx.cs" Inherits="CMSAdminControls_UI_UniControls_UniButton" %>
<asp:HyperLink ID="hyperLink" runat="server" EnableViewState="false">
    <asp:Image ID="image" runat="server" EnableViewState="false" /><asp:Label ID="lblText"
        runat="server" EnableViewState="false" /></asp:HyperLink><cms:CMSButton runat="server"
            ID="btn" EnableViewState="false" OnClick="btn_Click" ButtonStyle="Default" />