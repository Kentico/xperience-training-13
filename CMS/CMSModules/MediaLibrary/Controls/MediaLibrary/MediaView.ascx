<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaView"
     Codebehind="MediaView.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/Search.ascx"
    TagName="DialogSearch" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/InnerMediaView.ascx"
    TagName="InnerMediaView" TagPrefix="cms" %>

<asp:PlaceHolder ID="plcListingInfo" runat="server" Visible="false" EnableViewState="false">
    <div class="DialogListingInfo">
        <div class="alert-info alert">
            <span class="alert-icon">
                <i class="icon-i-circle"></i>
                <span class="sr-only"><%= GetString("general.info") %></span>
            </span>
            <div class="alert-label"><asp:Label ID="lblListingInfo" runat="server" EnableViewState="false"></asp:Label></div>
        </div>
    </div>
</asp:PlaceHolder>
<div class="DialogViewContentTop">
    <cms:CMSUpdatePanel ID="pnlUpdateDialog" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:DialogSearch ID="dialogSearch" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</div>
<div class="DialogViewContentBottom">
    <cms:CMSUpdatePanel ID="pnlUpdateView" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:InnerMediaView ID="innermedia" ShortID="v" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</div>
