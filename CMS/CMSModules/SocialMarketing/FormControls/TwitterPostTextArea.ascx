<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TwitterPostTextArea.ascx.cs" Inherits="CMSModules_SocialMarketing_FormControls_TwitterPostTextArea" %>
<div>
    <cms:CMSTextArea runat="server" ID="textArea" Rows="4" />
    <strong class="explanation-text">
        <cms:LocalizedLabel runat="server" ID="lblCharCount" ToolTipResourceString="sm.twitter.posts.charactersremaining" /></strong>
    <asp:HiddenField runat="server" ID="hdnCharCount" />
</div>

