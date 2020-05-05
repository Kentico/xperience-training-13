<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_General_List"  Codebehind="General_List.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownListControl"
    TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblVisible" runat="server" ResourceString="srch.index.objectname"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="drpObjType" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DropDownListControl ID="drpObjType" CssClass="DropDownField" runat="server" MacroSource="Flatten(&quot;&quot;, ObjectTypes.AllExceptBindingObjectTypes)" TextFormat="{% if (Item != &quot;&quot;) { GetObjectTypeName(Item) } else { GetResourceString(&quot;general.pleaseselect&quot;) } %}" SortItems="True" />
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