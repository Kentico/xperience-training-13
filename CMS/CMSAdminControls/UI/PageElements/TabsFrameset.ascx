<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_PageElements_TabsFrameset"  Codebehind="TabsFrameset.ascx.cs" %>

<frameset border="0" id="rowsFrameset" runat="server">
    <frame name="menu" id="frmTabs" scrolling="no" frameborder="0" noresize="noresize" runat="server" />
    <frame name="content" id="frmContent" frameborder="0" noresize="noresize" runat="server" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>