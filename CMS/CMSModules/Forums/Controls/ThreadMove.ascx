<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_ThreadMove"  Codebehind="ThreadMove.ascx.cs" %>
<asp:PlaceHolder ID="plcMoveInner" runat="server">
    <cms:LocalizedLabel runat="server" ID="lblMove" ResourceString="forum.thread.movetopic" AssociatedControlID="drpMoveToForum" DisplayColon="true" />&nbsp;
    <cms:CMSDropDownList ID="drpMoveToForum" CssClass="DropDownField" runat="server" />&nbsp;
    <cms:LocalizedLinkButton ID="btnMove" runat="server" ResourceString="general.move" />&nbsp;
</asp:PlaceHolder>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" OffsetY="10" />
<asp:Literal ID="ltlMoveBack" runat="server" />