<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_ContentRating_Controls_DropDown"  Codebehind="DropDown.ascx.cs" %>
<cms:LocalizedLabel ID="lblRatings" runat="server" EnableViewState="false" Display="false" AssociatedControlID="drpRatings" ResourceString="general.rating" />
<cms:CMSDropDownList ID="drpRatings" runat="server" CssClass="CntRatingDrpList" />
<cms:CMSButton ID="btnSubmit" runat="server" ButtonStyle="Default" />
