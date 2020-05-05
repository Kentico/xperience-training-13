<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="OrderByControl.ascx.cs"
    Inherits="CMSAdminControls_Basic_OrderByControl" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:HiddenField ID="hdnIndices" runat="server" />
        <asp:PlaceHolder runat="server" ID="plcOrderBy" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
