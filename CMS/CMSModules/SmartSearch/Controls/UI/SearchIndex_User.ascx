<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_User"  Codebehind="SearchIndex_User.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea" TagPrefix="cms" %>
<%@ Register src="~/CMSModules/Membership/FormControls/Roles/selectrole.ascx" tagname="SelectRole" tagprefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblVisible" runat="server" ResourceString="srch.index.userhidden"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkHidden" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkHidden" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblEnabled" runat="server" ResourceString="srch.index.userenabled"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkOnlyEnabled" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkOnlyEnabled" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" ResourceString="srch.index.usersite"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkSite" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkSite" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblInRole" runat="server" ResourceString="srch.index.userinrole"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="selectInRole" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectRole ID="selectInRole" runat="server" IsLiveSite="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblNotInRole" runat="server" ResourceString="srch.index.usernotinrole"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="selectNotInRole" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SelectRole ID="selectNotInRole" runat="server" IsLiveSite="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblWhere" runat="server" ResourceString="srch.index.where"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtWhere" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LargeTextArea ID="txtWhere" AllowMacros="false" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOk_Click" EnableViewState="false" />
        </div>
    </div>
</div>