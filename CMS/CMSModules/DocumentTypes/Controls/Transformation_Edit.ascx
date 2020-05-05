<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Transformation_Edit.ascx.cs"
    Inherits="CMSModules_DocumentTypes_Controls_Transformation_Edit" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal TransformationNewTable">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" ResourceString="transformationedit.transformationname" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtName" runat="server" MaxLength="100" />
            <cms:CMSRequiredFieldValidator ID="rfvCodeName" ControlToValidate="txtName" Display="Dynamic"
                runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnOK" runat="server" OnClick="btnOK_Click"
                ResourceString="general.saveandclose" />
        </div>
    </div>
</div>