<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_FormControls_Documents_SelectPath"
     Codebehind="SelectPath.ascx.cs" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
        <div class="control-group-inline">
            <cms:CMSTextBox ID="txtPath" runat="server" CssClass="form-control" />
            <cms:CMSButton ID="btnSelectPath" runat="server" EnableViewState="false" RenderScript="True" />
            <cms:CMSButton ID="btnSetPermissions" runat="server" EnableViewState="false" RenderScript="True" ButtonStyle="Default" />
            <cms:CMSTextBox ID="txtNodeId" runat="server" AutoPostBack="true" CssClass="Hidden" />
            <cms:LocalizedLabel ID="lblNodeId" runat="server" EnableViewState="false" Display="false" ResourceString="generalproperties.nodeid"
                AssociatedControlID="txtNodeId" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
