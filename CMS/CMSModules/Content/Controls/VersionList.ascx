<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="VersionList.ascx.cs" Inherits="CMSModules_Content_Controls_VersionList" %>
<%@ Register TagPrefix="cms" TagName="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>

<asp:PlaceHolder ID="plcLabels" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
</asp:PlaceHolder>
<cms:LocalizedHeading ID="headHistory" runat="server" Level="4" ResourceString="VersionsProperties.History"
    EnableViewState="false" />
<cms:UniGrid ID="gridHistory" runat="server" OrderBy="VersionHistoryID DESC" RememberState="False" />
<div class="clear-history">
    <cms:LocalizedButton ID="btnDestroy" runat="server" ButtonStyle="Default" OnClick="btnDestroy_Click"
        OnClientClick="return confirm(varConfirmDestroy);" ResourceString="VersionsProperties.Clear" />
</div>