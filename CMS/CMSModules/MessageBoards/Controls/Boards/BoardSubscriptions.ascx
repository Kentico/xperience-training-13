<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_Boards_BoardSubscriptions"
     Codebehind="BoardSubscriptions.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:UniGrid ID="boardSubscriptions" runat="server" GridName="~/CMSModules/MessageBoards/Tools/Boards/BoardSubscriptions.xml"
    Columns="SubscriptionID, SubscriptionUserID, SubscriptionEmail, SubscriptionApproved" OrderBy="SubscriptionEmail" ObjectType="board.subscription" />
<asp:Panel runat="server" ID="pnlSendConfirmationEmail" Visible="true">
    <div class="form-group">
        <cms:CMSCheckBox runat="server" ID="chkSendConfirmationEmail" Checked="true" ResourceString="forums.forumsubscription.sendemail" />
    </div>
</asp:Panel>
