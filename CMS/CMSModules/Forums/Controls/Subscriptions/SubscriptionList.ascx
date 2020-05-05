<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_Subscriptions_SubscriptionList"  Codebehind="SubscriptionList.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<cms:UniGrid runat="server" ID="gridElem" GridName="~/CMSModules/Forums/Controls/Subscriptions/ForumSubscription_List.xml" Columns="SubscriptionID, SubscriptionEmail, PostSubject, SubscriptionApproved" DelayedReload="true" />
<asp:Panel runat="server" ID="pnlSendConfirmationEmail" Visible="true">
    <cms:CMSCheckBox runat="server" ID="chkSendConfirmationEmail" Checked="true" ResourceString="forums.forumsubscription.sendemail" />
</asp:Panel>
