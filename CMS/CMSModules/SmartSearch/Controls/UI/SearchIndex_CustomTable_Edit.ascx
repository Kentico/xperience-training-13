<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_CustomTable_Edit"
     Codebehind="SearchIndex_CustomTable_Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/CustomTableSelector.ascx" TagName="CustomTableSelector"
    TagPrefix="uc1" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Panel ID="pnlConetnEdit" runat="server">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSSearchIndexingEnabled" InfoText="{$srch.searchdisabledinfo$}" />
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCustomTable" EnableViewState="false" DisplayColon="true"
                    ResourceString="srch.index.customtable" AssociatedControlID="customTableSelector" />
            </div>
            <div class="editing-form-value-cell">
                <uc1:CustomTableSelector ID="customTableSelector" IsLiveSite="false" AllSites="true"
                    runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblWhere" EnableViewState="false" DisplayColon="true"
                    ResourceString="srch.index.where" AssociatedControlID="txtWhere" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LargeTextArea ID="txtWhere" AllowMacros="false" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>