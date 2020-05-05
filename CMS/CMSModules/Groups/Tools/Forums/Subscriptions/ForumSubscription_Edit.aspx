<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Tools_Forums_Subscriptions_ForumSubscription_Edit" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="ForumSubscription_Edit.aspx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Subscriptions/SubscriptionEdit.ascx" TagName="SubscriptionEdit" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:SubscriptionEdit ID="subscriptionEdit" runat="server" />
</asp:Content>