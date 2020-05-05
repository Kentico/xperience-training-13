<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Staging_FormControls_ServerSelector"  Codebehind="ServerSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" RenderMode="Inline">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%ServerDisplayName%}" UseUniSelectorAutocomplete="false"
            SelectionMode="SingleDropDownList" ObjectType="staging.server" ResourcePrefix="serverselector"
            AllowEmpty="false" AllowAll="true" ReturnColumnName="ServerID" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
