<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ShippingServiceSelector.ascx.cs" Inherits="CMSModules_Ecommerce_FormControls_ShippingServiceSelector" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" ViewStateMode="Enabled">
    <ContentTemplate>
        <cms:CMSDropDownList ID="drpService" DataValueField="Key" DataTextField="Value" runat="server" />
    </ContentTemplate>
</cms:CMSUpdatePanel>