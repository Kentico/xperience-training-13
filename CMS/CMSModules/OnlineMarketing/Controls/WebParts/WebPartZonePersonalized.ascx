<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="WebPartZonePersonalized.ascx.cs" Inherits="CMSModules_OnlineMarketing_Controls_WebParts_WebPartZonePersonalized" %>
<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ContentPersonalizationVariant/Edit.ascx"
    TagName="ContentPersonalizationVariantEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/MVTVariant/Edit.ascx"
    TagName="MvtVariantEdit" TagPrefix="cms" %>

<cms:UIContextPanel ID="uiContextPanelCP" runat="server">
    <cms:ContentPersonalizationVariantEdit ID="cpEditElem" runat="server" IsLiveSite="false" Visible="false" />
</cms:UIContextPanel>

<cms:UIContextPanel ID="uiContextPanelMVT" runat="server">
    <cms:MvtVariantEdit ID="mvtEditElem" runat="server" IsLiveSite="false" Visible="false" />
</cms:UIContextPanel>
