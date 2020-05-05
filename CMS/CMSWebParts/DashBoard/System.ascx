<%@ Control Language="C#" AutoEventWireup="False" CodeBehind="~/CMSWebParts/DashBoard/System.ascx.cs"
    Inherits="CMSWebParts_DashBoard_System" %>
<%@ Register Src="~/CMSModules/System/Controls/System.ascx" TagName="SystemInformation"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:SystemInformation ID="sysInfo" runat="server" IsLiveSite="False" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
