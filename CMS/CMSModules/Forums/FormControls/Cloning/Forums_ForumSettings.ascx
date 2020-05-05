<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Forums_ForumSettings.ascx.cs"
    Inherits="CMSModules_Forums_FormControls_Cloning_Forums_ForumSettings" %>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCloneForumPosts" ResourceString="clonning.settings.forum.cloneposts"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkCloneForumPosts" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkCloneForumPosts" Checked="false" />
        </div>
    </div>
</div>