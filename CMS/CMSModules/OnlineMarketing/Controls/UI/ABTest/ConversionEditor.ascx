<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ConversionEditor.ascx.cs" Inherits="CMSModules_OnlineMarketing_Controls_UI_ABTest_ConversionEditor" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="form-horizontal">
            <div class="form-group">
                <cms:LocalizedLabel ID="lblConversions" CssClass="control-label-top" runat="server" ResourceString="campaign.conversion.visitoractivity" EnableViewState="false" DisplayColon="true" AssociatedControlID="drpConversions" />
                <cms:CMSDropDownList ID="drpConversions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="selector_OnSelectionChanged" />
            </div>
            <asp:Panel ID="pnlItem" runat="server" Visible="false" CssClass="form-group">
                <cms:LocalizedLabel ID="lblItem" CssClass="control-label-top" runat="server" EnableViewState="false" DisplayColon="true" />
                <asp:PlaceHolder ID="plcItemControl" runat="server" />
                <cms:LocalizedLabel ID="lblItemError" CssClass="EditingFormErrorLabel" runat="server" EnableViewState="false" Visible="false" />
            </asp:Panel>
            <div class="form-group">
                <cms:LocalizedLabel ID="lblValue" CssClass="control-label-top" runat="server" ResourceString="general.value" EnableViewState="false" DisplayColon="true" AssociatedControlID="txtValue" />
                <cms:CMSTextBox ID="txtValue" CssClass="form-control" runat="server" />
                <cms:LocalizedLabel ID="lblValueError" CssClass="EditingFormErrorLabel" runat="server" ResourceString="filter.validdecimalnumber" EnableViewState="false" Visible="false" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>