<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TransformationCode.ascx.cs" Inherits="CMSFormControls_Layouts_TransformationCode" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" ID="lblType" ResourceString="DocumentType_Edit_Transformation_Edit.TransformType" AssociatedControlID="drpType" CssClass="control-label" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList runat="server" ID="drpType" AutoPostBack="true" OnSelectedIndexChanged="drpTransformationType_SelectedIndexChanged"
                EnableViewState="true" />
        </div>
    </div>
    <div class="form-group">
        <cms:CMSHtmlEditor runat="server" ID="tbWysiwyg" Width="99%" Height="300" Visible="false" />
        <cms:MacroEditor runat="server" ID="txtCode" ShortID="e" Visible="false" Width="100%" />
    </div>
</div>
