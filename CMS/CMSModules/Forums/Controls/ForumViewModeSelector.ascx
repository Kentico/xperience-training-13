<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_ForumViewModeSelector"  Codebehind="ForumViewModeSelector.ascx.cs" %>
<asp:Literal runat="server" ID="litText" />
<cms:LocalizedLabel ID="lblViewMode" EnableViewState="false" AssociatedControlID="drpViewModeSelector" runat="server" />
<cms:CMSDropDownList ID="drpViewModeSelector" runat="server" AutoPostBack="true" />