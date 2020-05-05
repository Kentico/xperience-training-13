<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ConversionReportHeader.ascx.cs"
    Inherits="CMSModules_WebAnalytics_Controls_ConversionReportHeader" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="SelectConversion"
    TagPrefix="cms" %>

<div class="form-horizontal form-filter">
    <asp:PlaceHolder runat="server" ID="pnlConversion">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblConversion" CssClass="control-label" runat="server" ResourceString="analytics.conversion" AssociatedControlID="usSelectConversion"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell">
                <cms:SelectConversion runat="server" ID="usSelectConversion" SelectionMode="SingleDropDownList" PostbackOnDropDownChange="true" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>