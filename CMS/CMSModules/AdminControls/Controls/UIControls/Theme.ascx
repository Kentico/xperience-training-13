<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AdminControls/Controls/UIControls/Theme.ascx.cs" Inherits="CMSModules_AdminControls_Controls_UIControls_Theme" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/FileSystemSelector.ascx"
    TagName="FileSystemSelector" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:FileSystemSelector ID="selFile" runat="server" RemoveHelpIcon="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
