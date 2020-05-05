<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_Content_Edit"  Codebehind="SearchIndex_Content_Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Classes/selectclassnames.ascx" TagName="SelectClassnames"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/selectpath.ascx" TagName="selectpath" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Panel ID="pnlConetnEdit" runat="server">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSSearchIndexingEnabled" InfoText="{$srch.searchdisabledinfo$}" />
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblLocation" EnableViewState="false" ResourceString="srch.index.location"
                    DisplayColon="true" AssociatedControlID="selectpath" />
            </div>
            <div class="editing-form-value-cell">
                <cms:selectpath ID="selectpath" runat="server" IsLiveSite="false" SinglePathMode="False" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDocTypes" EnableViewState="false" ResourceString="development.doctypes"
                    DisplayColon="true" AssociatedControlID="selectClassnames" />
            </div>
            <div class="editing-form-value-cell">
                <cms:SelectClassnames ID="selectClassnames" runat="server" IsLiveSite="false" SiteID="0" />
                <cms:LocalizedLabel runat="server" ID="lblClassNamesInfo" ResourceString="srch.index.classnamesinfo" CssClass="explanation-text" />
            </div>
        </div>
        <asp:PlaceHolder ID="pnlAllowed" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblIncludeAttachments" EnableViewState="false" ResourceString="srch.index.inclatt"
                        DisplayColon="true" AssociatedControlID="chkInclAtt" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkInclAtt" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblInclCats" EnableViewState="false" ResourceString="srch.index.inclcats"
                        DisplayColon="true" AssociatedControlID="chkInclCats" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkInclCats" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>