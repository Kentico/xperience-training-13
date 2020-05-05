<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_Controls_MySubscriptions"  Codebehind="MySubscriptions.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:PlaceHolder runat="server" ID="plcMain">
    <cms:MessagesPlaceHolder runat="server" ID="plcMess" LiveSiteOnly="true" />
    <cms:LocalizedHeading runat="server" Level="5" ID="headNewsletters" ResourceString="mysubscriptions.campaignsetupheader" EnableViewState="false" />
    <asp:Label runat="server" ID="lblUnsubscribeFromAll" EnableViewState="false" CssClass="InfoLabel" />
    <cms:CMSButton runat="server" ID="btnUsubscribeFromAll" OnClick="btnUnsubscribeFromAll_Click" EnableViewState="false" />

    <cms:LocalizedLabel runat="server" ID="lblText" EnableViewState="false" CssClass="InfoLabel" ResourceString="mysubscriptions.selectorheading" />
    <cms:UniSelector ID="usNewsletters" runat="server" ObjectType="Newsletter.Newsletter"
        SelectionMode="Multiple" ResourcePrefix="MySubscriptions.Newsletterselect" />
    <asp:HiddenField ID="hdnValue" runat="server" EnableViewState="false" />
</asp:PlaceHolder>
