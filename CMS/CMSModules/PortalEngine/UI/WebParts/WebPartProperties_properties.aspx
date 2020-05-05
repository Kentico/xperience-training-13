<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartProperties_properties"
    Theme="default" EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="WebPartProperties_properties.aspx.cs" %>

<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/WebpartProperties.ascx"
    TagName="WebpartProperties" TagPrefix="cms" %>
<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlBody">
        <cms:WebpartProperties ID="webPartProperties" runat="server" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>
