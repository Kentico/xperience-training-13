<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Tools_Polls_Sites"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Polls - sites"
    Theme="Default"  Codebehind="Polls_Sites.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcC" runat="server" Visible="false">
        <cms:LocalizedHeading ID="headTitle" Level="4" runat="server" CssClass="listing-title" ResourceString="Poll_Sites.Available" />
        <cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
            SelectionMode="Multiple" ResourcePrefix="sitesselect" />
    </asp:PlaceHolder>
</asp:Content>
