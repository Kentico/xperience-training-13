<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Tools_Forums_Subscriptions_ForumSubscription_List" Theme="Default" 
     MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="ForumSubscription_List.aspx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Subscriptions/SubscriptionList.ascx" TagName="SubscriptionList" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:SubscriptionList ID="subscriptionList" runat="server" />
</asp:Content>