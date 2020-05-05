<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Newsletters_MySubscriptionsWebpart"  Codebehind="~/CMSWebParts/Newsletters/MySubscriptionsWebpart.ascx.cs" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/MySubscriptions.ascx" TagName="MySubscriptions" TagPrefix="cms" %>
<cms:MySubscriptions id="ucMySubsriptions" runat="server" IsLiveSite="true" />
