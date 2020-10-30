<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConversionSelector.ascx.cs" Inherits="CMSFormControls_Selectors_ConversionSelector" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:CMSDropDownList ID="drpConversions" runat="server" AutoPostBack="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
