<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_MyDesk_MyProfile_MyProfile_Subscriptions" Theme="Default"
     Codebehind="MyProfile_Subscriptions.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>
<%@ Register Src="~/CMSModules/Membership/Controls/Subscriptions.ascx" TagName="Subscriptions"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:Subscriptions ID="elemSubscriptions" runat="server" IsLiveSite="false" />
    <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="5" />
</asp:Content>
