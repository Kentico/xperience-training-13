<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="SubscriptionApproval.aspx.cs"
    Inherits="CMSModules_MessageBoards_CMSPages_SubscriptionApproval" Theme="Default"
    MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/SubscriptionApproval.ascx"
    TagName="SubscriptionApproval" TagPrefix="cms" %>
<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server">
    <cms:SubscriptionApproval ID="subscriptionApproval" runat="server" Visible="true" />
</asp:Content>
