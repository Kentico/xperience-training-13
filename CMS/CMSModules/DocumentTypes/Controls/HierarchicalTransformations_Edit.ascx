<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Controls_HierarchicalTransformations_Edit"
     Codebehind="HierarchicalTransformations_Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Classes/SelectTransformation.ascx" TagName="SelectTransformation"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectClassNames.ascx" TagName="ClassSelector"
    TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="documenttype_edit_transformation_edit.transformtype" AssociatedControlID="drpTemplateType" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList runat="server" ID="drpTemplateType" CssClass="DropDownField" AutoPostBack="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblDocTypes" runat="server" ResourceString="development.documenttypes" AssociatedControlID="ucClassSelector" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ClassSelector runat="server" ID="ucClassSelector" IsLiveSite="false" SiteID="0" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblLevel" runat="server" ResourceString="development.level" AssociatedControlID="txtLevel" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtLevel" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblApplyToSublevels" runat="server" ResourceString="hiertransf.allowforlevel" AssociatedControlID="chkApplyToSublevels" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkApplyToSublevels" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" ResourceString="documenttype_edit_transformation_edit.transformname" AssociatedControlID="ucTransformations" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectTransformation ID="ucTransformations" runat="server" IsLiveSite="false"
                EditWindowName="EditLevel2" />
        </div>
    </div>
</div>