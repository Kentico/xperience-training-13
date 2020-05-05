<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_Controls_CommentFilter"  CodeBehind="CommentFilter.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblBlog" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="blog.comments.blog" />
        </div>
        <div class="filter-form-value-cell">
            <cms:UniSelector ID="uniSelector" runat="server" OnOnSpecialFieldsLoaded="uniSelector_OnSpecialFieldsLoaded" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.username" />
        </div>
        <div class="filter-form-value-cell">
            <cms:CMSTextBox ID="txtUserName" runat="server" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblComment" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="blog.comments.comment" />
        </div>
        <div class="filter-form-value-cell">
            <cms:CMSTextBox ID="txtComment" runat="server" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblApproved" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="blog.comments.approved" />
        </div>
        <div class="filter-form-value-cell">
            <cms:CMSDropDownList ID="drpApproved" runat="server" CssClass="DropDownField" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblSpam" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="blog.comments.spam" />
        </div>
        <div class="filter-form-value-cell">
            <cms:CMSDropDownList ID="drpSpam" runat="server" CssClass="DropDownField" />
        </div>
    </div>
    <div class="form-group form-group-buttons">
        <div class="filter-form-buttons-cell">
            <cms:CMSButton ID="btnFilter" runat="server" OnClick="btnFilter_Click" ButtonStyle="Primary"
                EnableViewState="false" />
        </div>
    </div>
</div>