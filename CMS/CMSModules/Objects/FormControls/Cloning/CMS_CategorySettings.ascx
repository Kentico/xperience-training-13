<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_CategorySettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_CategorySettings" %>

<%@ Register Src="~/CMSModules/Categories/Controls/SelectCategory.ascx" TagName="SelectCategory" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCategory" ResourceString="clonning.settings.category.parentcategory"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="drpCategories" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectCategory runat="server" ID="drpCategories" AddNoneRecord="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSubcategories" ResourceString="clonning.settings.category.subcategories"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkSubcategories" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkSubcategories" Checked="true" />
        </div>
    </div>
</div>