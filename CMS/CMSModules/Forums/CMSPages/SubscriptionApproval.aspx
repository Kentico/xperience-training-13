<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="SubscriptionApproval.aspx.cs"
    Inherits="CMSModules_Forums_CMSPages_SubscriptionApproval" MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Forums/Controls/SubscriptionApproval.ascx" TagName="SubscriptionApproval"
    TagPrefix="cms" %>
<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server">
    <cms:SubscriptionApproval ID="subscriptionApproval" runat="server" Visible="true" />
</asp:Content>
