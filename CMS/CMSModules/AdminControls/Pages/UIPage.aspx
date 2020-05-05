<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="UIPage.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/UIPage.master"
    EnableEventValidation="false" Inherits="CMSModules_AdminControls_Pages_UIPage" Theme="Default" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/UIControls/DialogFooter.ascx" TagName="DialogFooter" TagPrefix="cms" %>

<asp:Content runat="server" ID="cplcContent" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder runat="server" ID="plcManagers">
        <cms:CMSPortalManager ID="manPortal" ShortID="m" runat="server" EnableViewState="false" />
    </asp:PlaceHolder>
    <cms:CMSPlaceHolder ID="plcHeader" runat="server" />
    <cms:CMSPagePlaceholder ID="plc" runat="server" Root="true" />
    
    <asp:Literal runat="server" ID="ltlFooterBefore" EnableViewState="False"></asp:Literal>
    <cms:CMSPlaceHolder ID="plcFooter" runat="server" Visible="False">
            <cms:DialogFooter runat="server" ID="dialogFooter" />
    </cms:CMSPlaceHolder>
    <asp:Literal runat="server" ID="ltlFooterAfter" EnableViewState="False"></asp:Literal>
</asp:Content>
