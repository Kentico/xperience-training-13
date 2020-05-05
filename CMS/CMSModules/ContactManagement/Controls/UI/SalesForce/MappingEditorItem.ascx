<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="MappingEditorItem.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_SalesForce_MappingEditorItem" %>

<div class="form-group">
    <div class="editing-form-label-cell">
        <cms:LocalizedLabel DisplayColon="True" AssociatedControlID="SourceDropDownList" CssClass="control-label" ID="AttributeLabel" runat="server" EnableViewState="false" />
    </div>
    <div class="editing-form-value-cell">
        <div class="control-group-inline">
            <cms:LocalizedLabel CssClass="form-control-text" id="EmptyMessageControl" runat="server" EnableViewState="false" Visible="false" />
            <cms:CMSDropDownList ID="SourceDropDownList" runat="server" EnableViewState="false" />
            <asp:PlaceHolder ID="WarningPlaceHolder" runat="server" EnableViewState="false"></asp:PlaceHolder>
        </div>
    </div>
</div>
