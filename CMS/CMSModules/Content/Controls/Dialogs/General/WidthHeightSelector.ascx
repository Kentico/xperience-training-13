<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_General_WidthHeightSelector"  Codebehind="WidthHeightSelector.ascx.cs" %>

<div class="form-group">
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel ID="lblWidth" runat="server" DisplayColon="true" CssClass="control-label" ResourceString="general.width" AssociatedControlID="txtWidth" />
    </div>
    <div class="editing-form-value-cell">
        <cms:CMSTextBox ID="txtWidth" runat="server" CssClass="form-control input-width-20" />
    </div>
</div>
<div class="form-group">
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel ID="lblHeight" runat="server" DisplayColon="true" CssClass="control-label" ResourceString="general.height" AssociatedControlID="txtHeight" />
    </div>
    <div class="editing-form-value-cell">
        <cms:CMSTextBox ID="txtHeight" runat="server" CssClass="form-control input-width-20" />
    </div>
</div>
<div class="form-group">
    <div class="editing-form-value-cell editing-form-value-cell-offset">
        <cms:CMSButton ID="imgLock" runat="server" EnableViewState="false" ButtonStyle="Default" />
        <cms:CMSButton ID="imgRefresh" runat="server" EnableViewState="false" ButtonStyle="Default" />
    </div>
</div>

<asp:HiddenField ID="hdnWidth" runat="server" />
<asp:HiddenField ID="hdnHeight" runat="server" />
<asp:HiddenField ID="hdnLocked" runat="server" />