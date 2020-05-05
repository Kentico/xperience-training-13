<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Profile_GroupMessageBoards"  Codebehind="~/CMSWebParts/Community/Profile/GroupMessageBoards.ascx.cs" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/LiveControls/MessageBoards.ascx"
    TagName="MessageBoards" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:MessageBoards ID="boardsElem" runat="server" />
