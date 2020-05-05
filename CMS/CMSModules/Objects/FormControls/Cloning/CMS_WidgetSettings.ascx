<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_WidgetSettings.ascx.cs"
    Inherits="CMSModules_Objects_FormControls_Cloning_CMS_WidgetSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWidgetCategory" ResourceString="clonning.settings.widget.category"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="categorySelector" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectWidgetCategory ID="categorySelector" runat="server" />
        </div>
    </div>
</div>