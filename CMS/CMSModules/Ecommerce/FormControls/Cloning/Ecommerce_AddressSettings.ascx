<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Ecommerce_AddressSettings.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_AddressSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="txtPersonalName" CssClass="control-label" runat="server" ID="lblPersonalName" ResourceString="customer_edit_address_edit.lblpersonalname" EnableViewState="false"
                DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtPersonalName"  MaxLength="100" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="txtAddress1" CssClass="control-label" runat="server" ID="lblAddress1" ResourceString="customer_edit_address_edit.addressline1label" EnableViewState="false"
                DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtAddress1"  MaxLength="100" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:CMSTextBox runat="server" ID="txtAddress2" MaxLength="100" />
        </div>
    </div>
</div>