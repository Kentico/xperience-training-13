<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Notifications_Controls_TemplateText"
     Codebehind="TemplateText.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:PlaceHolder runat="server" ID="plcTexts" EnableViewState="false" />
<asp:Panel runat="server" ID="pnlGrid">
    <cms:UniGrid runat="server" ID="gridGateways" GridName="~/CMSModules/Notifications/Controls/TemplateText.xml"
        OrderBy="GatewayDisplayName" IsLiveSite="false" Columns="GatewayID, GatewayDisplayName, GatewayEnabled" />
</asp:Panel>
