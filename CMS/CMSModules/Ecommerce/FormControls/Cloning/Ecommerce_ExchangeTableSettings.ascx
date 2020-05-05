<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Ecommerce_ExchangeTableSettings.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_ExchangeTableSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="dtValidFrom" CssClass="control-label" runat="server" ID="lblValidFrom" ResourceString="clonning.settings.exchangetable.validfrom"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker runat="server" ID="dtValidFrom" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="dtValidTo" CssClass="control-label" runat="server" ID="lblValidTo" ResourceString="clonning.settings.exchangetable.validto"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker runat="server" ID="dtValidTo" />
        </div>
    </div>
</div>