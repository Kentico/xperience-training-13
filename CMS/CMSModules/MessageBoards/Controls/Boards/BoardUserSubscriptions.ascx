<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_Controls_Boards_BoardUserSubscriptions"  Codebehind="BoardUserSubscriptions.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" LiveSiteOnly="true" />
<cms:LocalizedLabel ID="lblMessage" runat="server" CssClass="InfoLabel" EnableViewState="false"
    ResourceString="boardsubscripitons.userissubscribed" />
<cms:UniGrid ID="boardSubscriptions" runat="server" FilterLimit="10" ExportFileName="board_subscription"
    GridName="~/CMSModules/MessageBoards/Tools/Boards/BoardUserSubscriptions.xml" />
