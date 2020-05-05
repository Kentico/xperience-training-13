<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_OnLineForm_Edit"
     Codebehind="SearchIndex_OnLineForm_Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SelectSite" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlConetnEdit" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSSearchIndexingEnabled" InfoText="{$srch.searchdisabledinfo$}" />
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <asp:Panel ID="pnlForm" runat="server">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSite" EnableViewState="false" ResourceString="srch.index.site"
                            DisplayColon="true" AssociatedControlID="selSite" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectSite IsLiveSite="false" ID="selSite" runat="server" AllowAll="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCustomTable" EnableViewState="false" DisplayColon="true"
                            ResourceString="srch.index.onlineform" AssociatedControlID="selectForm" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:UniSelector runat="server" ID="selectForm" DisplayNameFormat="{%FormDisplayName%}"
                            ReturnColumnName="FormName" ObjectType="cms.form" ResourcePrefix="onlineform"
                            SelectionMode="SingleDropDownList" AllowEmpty="false" AllowAll="false" />
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
    </ContentTemplate>
</cms:CMSUpdatePanel>