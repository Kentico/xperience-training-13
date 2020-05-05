<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Comment.ascx.cs" Inherits="CMSModules_Objects_Controls_Locking_Comment" %>
<cms:CMSObjectManager ID="objectManager" runat="server" />
<cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblComment" runat="server" ResourceString="worfklowproperties.comment" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea ID="txtComment" CssClass="form-control" runat="server" Rows="15" />
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnArg" runat="server" />
</cms:CMSPanel>