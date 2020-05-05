<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_Controls_Boards_BoardModerators"  Codebehind="BoardModerators.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/securityAddUsers.ascx" TagName="SelectUser" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="board-moderators">
    <div class="form-horizontal">
        <cms:LocalizedHeading ID="lblModerators" runat="server" Level="4" DisplayColon="False" ResourceString="board.moderators.title" EnableViewState="false" CssClass="listing-title" />
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" DisplayColon="True" ResourceString="board.moderators.ismoderated" AssociatedControlID="chkBoardModerated" CssClass="control-label"></cms:LocalizedLabel>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkBoardModerated" runat="server"
                    OnCheckedChanged="chkBoardModerated_CheckedChanged" AutoPostBack="true" />
            </div>
        </div>
    </div>
    <cms:SelectUser ID="userSelector" runat="server" />
</div>
