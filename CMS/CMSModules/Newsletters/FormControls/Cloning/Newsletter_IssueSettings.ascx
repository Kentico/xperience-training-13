<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Newsletter_IssueSettings.ascx.cs" Inherits="CMSModules_Newsletters_FormControls_Cloning_Newsletter_IssueSettings" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/NewsletterSelector.ascx" TagName="NewsletterSelector" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCloneToNewsletter" ResourceString="clonning.settings.newsletterissue.newsletter" AssociatedControlID="drpNewsletters:usNewsletters:drpSingleSelect"
                EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:NewsletterSelector runat="server" ID="drpNewsletters" AllowEmpty="true" UseSimpleMode="true" />
        </div>
    </div>
</div>