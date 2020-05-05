<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="StrandsCatalogUploadFrequency.ascx.cs" Inherits="CMSModules_StrandsRecommender_FormControls_StrandsCatalogUploadFrequency" EnableViewState="False" %>

<cms:CMSUpdatePanel runat="server" UpdateMode="Always">
    <ContentTemplate>
        <cms:CMSDropDownList ID="ddlMainFrequency" runat="server" IsLiveSite="false" AutoPostBack="True" />
        
        <span class="form-control-text">
            <cms:LocalizedLiteral ID="litExtendedFrequencySpecifier" runat="server" />
        </span>

        <cms:CMSDropDownList ID="ddlHourlyExtendedFrequency" runat="server" IsLiveSite="false" />
        <cms:CMSDropDownList ID="ddlDailyExtendedFrequency" runat="server" IsLiveSite="false" />
        <cms:CMSDropDownList ID="ddlWeeklyExtendedFrequency" runat="server" IsLiveSite="false" />
        
        <asp:Label ID="lblPSTTimeZone" CssClass="form-control-text" runat="server" Text="PST" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

<div class="explanation-text">
    <cms:LocalizedLabel ID="lblCatalogUploadededInfoMessage" runat="server" ResourceString="strands.uploadfrequency.aftersaveupload" />
</div>