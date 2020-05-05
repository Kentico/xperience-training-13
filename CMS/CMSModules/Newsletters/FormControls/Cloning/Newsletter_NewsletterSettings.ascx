<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Newsletter_NewsletterSettings.ascx.cs"
    Inherits="CMSModules_Newsletters_FormControls_Cloning_Newsletter_NewsletterSettings" %>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel AssociatedControlID="chkSubscribers" CssClass="control-label" runat="server" ID="lblSubscribers" ResourceString="clonning.settings.newsletter.subscribers"
                EnableViewState="false" DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkSubscribers" Checked="true" />
        </div>
    </div>
</div>