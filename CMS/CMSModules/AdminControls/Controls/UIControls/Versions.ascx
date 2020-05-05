<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/Versions.ascx.cs" Inherits="CMSModules_AdminControls_Controls_UIControls_Versions" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ObjectVersionList.ascx"
    TagPrefix="cms" TagName="VersionList" %>
<asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
    <cms:VersionList ID="versionList" runat="server" />
</asp:Panel>
