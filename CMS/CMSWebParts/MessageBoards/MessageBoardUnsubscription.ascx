<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_MessageBoards_MessageBoardUnsubscription"  Codebehind="~/CMSWebParts/MessageBoards/MessageBoardUnsubscription.ascx.cs" %>

<%@ Register Src="~/CMSModules/MessageBoards/Controls/Unsubscription.ascx" TagName="Unsubscription"
    TagPrefix="cms" %>
<cms:Unsubscription runat="server" ID="unsubscription" />