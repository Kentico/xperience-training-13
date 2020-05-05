<%@ Page Language="C#" AutoEventWireup="True"  Codebehind="SubscriptionApproval.aspx.cs"
    Inherits="CMSModules_Newsletters_CMSPages_SubscriptionApproval" Theme="Default" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/SubscriptionApproval.ascx" TagName="SubscriptionApproval"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head" runat="server" enableviewstate="false">
    <title>Subscription approval</title>
</head>
<body>
    <form id="form1" runat="server">
    <cms:SubscriptionApproval ID="subscriptionApproval" runat="server" Visible="true" />
    </form>
</body>
</html>
