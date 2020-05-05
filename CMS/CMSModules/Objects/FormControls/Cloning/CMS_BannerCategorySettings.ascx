<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_BannerCategorySettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_BannerCategorySettings" %>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCloneBanners" ResourceString="clonning.settings.bannercategory.banners"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkCloneBanners" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkCloneBanners" Checked="true" />
        </div>
    </div>
</div>