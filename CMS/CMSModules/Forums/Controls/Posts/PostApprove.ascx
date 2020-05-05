<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Posts_PostApprove"
     Codebehind="PostApprove.ascx.cs" %>
    
<cms:CMSPanel CssClass="ForumPostApprove" runat="server" ID="pnlContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" id="lblUserTitle" runat="server" displaycolon="true" resourcestring="general.user" />
            </div>
            <div class="editing-form-value-cell">
                <asp:label id="lblUser" runat="server" CssClass="form-control-text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" id="lblSubjectTitle" runat="server" displaycolon="true" resourcestring="general.subject" />
            </div>
            <div class="editing-form-value-cell">
                <asp:label id="lblSubject" runat="server" CssClass="form-control-text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" id="lblDateTitle" runat="server" displaycolon="true" resourcestring="general.date" />
            </div>
            <div class="editing-form-value-cell">
                <asp:label id="lblDate" runat="server" CssClass="form-control-text" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" id="lblTextTitle" runat="server" displaycolon="true" resourcestring="general.text" />
            </div>
            <div class="editing-form-value-cell">
                <div class="PostText">
                    <cms:resolvedliteral id="ltrText" runat="server" />
                </div>
            </div>
        </div>
    </div>
</cms:CMSPanel>