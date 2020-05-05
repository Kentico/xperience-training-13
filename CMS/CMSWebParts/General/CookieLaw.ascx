<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_General_CookieLaw"
    CodeBehind="~/CMSWebParts/General/CookieLaw.ascx.cs" %>
<div class="CookieConsent">
    <asp:Label runat="server" ID="lblText" EnableViewState="false" CssClass="ConsentText" />
    <asp:Panel runat="server" ID="pnlTrackingConsent" CssClass="TrackingConsent">
        <div class="ShortText">
            <asp:Literal runat="server" ID="ltlTrackingConsentShortText" EnableViewState="false" />
        </div>
        <asp:Literal runat="server" ID="ltlConsentReferenceMarkup" EnableViewState="false" />
    </asp:Panel>
    <span class="ConsentButtons">
        <cms:CMSButton runat="server" ID="btnDenyAll" EnableViewState="false" ButtonStyle="Default" CssClass="ConsentButton" OnClick="btnDenyAll_Click" />
        <cms:CMSButton runat="server" ID="btnAllowSpecific" EnableViewState="false" ButtonStyle="Default" CssClass="ConsentButton" OnClick="btnAllowSpecific_Click" />
        <cms:CMSButton runat="server" ID="btnAllowAll" EnableViewState="false" ButtonStyle="Default" CssClass="ConsentButton" OnClick="btnAllowAll_Click" />
    </span>
</div>
