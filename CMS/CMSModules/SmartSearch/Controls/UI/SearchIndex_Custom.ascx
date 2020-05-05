<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_Custom"  Codebehind="SearchIndex_Custom.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/AssemblyClassSelector.ascx" TagName="AssemblyClassSelector"
    TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblEnabled" runat="server" ResourceString="srch.index.assembly"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="assemblyElem" />
        </div>
        <div class="editing-form-value-cell">
            <cms:AssemblyClassSelector ID="assemblyElem" runat="server" BaseClassNames="ICustomSearchIndex" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblData" runat="server" ResourceString="srch.index.data"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtData" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LargeTextArea ID="txtData" AllowMacros="false" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton runat="server" ID="btnOk" ResourceString="general.ok" OnClick="btnOk_Click" EnableViewState="false" />
        </div>
    </div>
</div>