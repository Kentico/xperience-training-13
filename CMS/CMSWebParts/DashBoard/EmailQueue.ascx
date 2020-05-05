<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/DashBoard/EmailQueue.ascx.cs"
    Inherits="CMSWebParts_DashBoard_EmailQueue" %>
<%@ Register Src="~/CMSModules/EmailQueue/Controls/EmailQueue.ascx" TagName="EmailQueue"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/PermissionMessage.ascx" TagName="PermissionMessage"
    TagPrefix="cms" %>
    
<cms:PermissionMessage ID="messageElem" runat="server" Visible="false" EnableViewState="false" />
<cms:MessagesPlaceHolder ID="messagePlc" runat="server" EnableViewState="false" />
<cms:EmailQueue ID="emailQueue" runat="server" />
