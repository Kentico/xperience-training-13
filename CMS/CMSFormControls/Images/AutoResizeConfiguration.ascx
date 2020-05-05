<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Images_AutoResizeConfiguration"
     Codebehind="AutoResizeConfiguration.ascx.cs" %>

<div class="form-group">
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel CssClass="control-label" ID="lblResizeMode" runat="server" EnableViewState="false" ResourceString="dialogs.resize.resizemode" AssociatedControlID="drpSettings" />
    </div>
    <div class="editing-form-value-cell">
        <cms:CMSDropDownList ID="drpSettings" runat="server" UseResourceStrings="true" CssClass="DropDownField" />
    </div>
</div>
<div class="form-group">
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel CssClass="control-label" ID="lblWidth" runat="server" EnableViewState="false" ResourceString="dialogs.resize.width" AssociatedControlID="txtWidth" />
    </div>
    <div class="editing-form-value-cell">
        <cms:CMSTextBox ID="txtWidth" runat="server" CssClass="input-width-20 input-number" />
    </div>
</div>
<div class="form-group">
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel CssClass="control-label" ID="lblHeight" runat="server" EnableViewState="false" ResourceString="dialogs.resize.height" AssociatedControlID="txtHeight" />
    </div>
    <div class="editing-form-value-cell">
        <cms:CMSTextBox ID="txtHeight" runat="server" CssClass="input-width-20 input-number" />
    </div>
</div>
<div class="form-group">
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel CssClass="control-label" ID="lblMax" runat="server" EnableViewState="false" ResourceString="dialogs.resize.maxsidesize" AssociatedControlID="txtMax" />
    </div>
    <div class="editing-form-value-cell">
        <cms:CMSTextBox ID="txtMax" runat="server" CssClass="input-width-20 input-number" />
    </div>
</div>