<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_FieldEditor"
    CodeBehind="FieldEditor.ascx.cs" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/ControlSettings.ascx"
    TagName="ControlSettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/CSSsettings.ascx"
    TagName="CSSsettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldTypeSelector.ascx"
    TagName="FieldTypeSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/DatabaseConfiguration.ascx"
    TagName="DatabaseConfiguration" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/DocumentSource.ascx"
    TagName="DocumentSource" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldAppearance.ascx"
    TagName="FieldAppearance" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/ValidationSettings.ascx"
    TagName="ValidationSettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldAdvancedSettings.ascx"
    TagName="FieldAdvancedSettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/CategoryEdit.ascx"
    TagName="CategoryEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/HTMLEnvelope.ascx"
    TagName="HTMLEnvelope" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Panel ID="pnlFieldEditor" runat="server" CssClass="FieldEditor">
    <asp:Panel ID="pnlFieldEditorWrapper" runat="server">
        <asp:Panel ID="pnlHeaderActions" runat="server" CssClass="FieldTopMenuPadding" Visible="false">
            <div class="cms-edit-menu">
                <cms:HeaderActions runat="server" ID="hdrActions" />
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlHeaderPlaceHolder" runat="server" EnableViewState="false" CssClass="FieldTopMenuPlaceHolder" Visible="false" />
        <asp:Panel ID="pnlLeft" runat="server" CssClass="FieldPanelLeft">
            <asp:Panel ID="pnlButtons" runat="server" CssClass="form-group" EnableViewState="false">
                <asp:PlaceHolder ID="plcMainButtons" runat="server">
                    <cms:CMSMoreOptionsButton ID="btnAdd" runat="server" />
                    <asp:Button ID="btnNewCategory" runat="server" OnClick="btnNewCategory_Click" CssClass="hidden" />
                    <asp:Button ID="btnNewAttribute" runat="server" OnClick="btnNewAttribute_Click" CssClass="hidden" />
                    <cms:CMSAccessibleButton ID="btnDeleteItem" runat="server" OnClick="btnDeleteItem_Click" IconOnly="True" IconCssClass="icon-bin" />
                </asp:PlaceHolder>
                <cms:CMSAccessibleButton ID="btnUpAttribute" runat="server" OnClick="btnUpAttribute_Click" IconOnly="True" IconCssClass="icon-chevron-up" />
                <cms:CMSAccessibleButton ID="btnDownAttribute" runat="server" OnClick="btnDownAttribute_Click" IconOnly="True" IconCssClass="icon-chevron-down" />
            </asp:Panel>
            <div class="form-group">
                <cms:CMSListBox ID="lstAttributes" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstAttributes_SelectedIndexChanged"
                    EnableViewState="true" CssClass="AttributesList" Rows="15" />
            </div>
            <asp:Panel CssClass="form-group" ID="pnlDevelopmentMode" runat="server">
                <div>
                    <cms:LocalizedLinkButton ResourceString="FieldEditor.GetFormDefinition" runat="server"
                        ID="lnkFormDef" OnClick="lnkFormDef_Click" OnClientClick="noProgress=true;" />
                </div>
                <div>
                    <cms:LocalizedLinkButton ResourceString="FieldEditor.HideAllFields" runat="server"
                        ID="lnkHideAllFields" OnClick="lnkHideAllFields_OnClick" OnClientClick="noProgress=true;" />
                </div>
                <div>
                    <cms:LocalizedLinkButton ResourceString="FieldEditor.ShowAllFields" runat="server"
                        ID="lnkShowAllFields" OnClick="lnkShowAllFields_OnClick" OnClientClick="noProgress=true;" />
                </div>
            </asp:Panel>
            <div class="form-group">
                <cms:DocumentSource ID="documentSource" runat="server" ShortID="ds" />
            </div>
            <cms:CMSUpdatePanel ID="pnlUpdateQuickSelect" runat="server">
                <ContentTemplate>
                    <asp:Panel ID="pnlQuickSelect" runat="server" EnableViewState="false" CssClass="QuickSelect">
                        <cms:LocalizedLabel ID="lblQuickLinks" runat="server" EnableViewState="false" CssClass="Title"
                            ResourceString="fieldeditor.quicklinks" DisplayColon="true" />
                        <ul>
                            <li><a href="#Database">
                                <cms:LocalizedLabel ID="lblGeneral" runat="server" EnableViewState="false" ResourceString="general.general" /></a></li>
                            <asp:PlaceHolder ID="plcQuickAppearance" runat="server">
                                <li><a href="#FieldAppearance">
                                    <cms:LocalizedLabel ID="lblField" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.fieldappearance" /></a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcQuickSettings" runat="server">
                                <li><a href="#ControlSettings">
                                    <cms:LocalizedLabel ID="lblSettings" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.settings" /></a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcQuickValidation" runat="server">
                                <li><a href="#ValidationSettings">
                                    <cms:LocalizedLabel ID="lblValidation" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.validation" /></a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcQuickStyles" runat="server">
                                <li><a href="#CSSStyles">
                                    <cms:LocalizedLabel ID="lblStyles" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.styles" /></a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcQuickHtmlEnvelope" runat="server">
                                <li><a href="#HtmlEnvelope">
                                    <cms:LocalizedLabel ID="lblHtmlEnvelope" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.htmlenvelope" /></a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcQuickAdvancedSettings" runat="server">
                                <li><a href="#FieldAdvancedSettings">
                                    <cms:LocalizedLabel ID="lblAdvanced" runat="server" EnableViewState="false" ResourceString="templatedesigner.section.fieldadvancedsettings" /></a></li>
                            </asp:PlaceHolder>
                        </ul>
                    </asp:Panel>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </asp:Panel>
        <asp:Panel ID="pnlContent" runat="server" CssClass="FieldPanelRightContent">
            <asp:Panel ID="pnlContentPadding" runat="server" CssClass="FieldPanelRightContentPadding">
                <div class="FieldRightScrollPanel">
                    <div class="FieldPanelRightWizard scroll-area">
                        <asp:Panel runat="server" ID="pnlRightWrapper" CssClass="FieldPanelRightWrapper">
                            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                            <asp:Panel runat="server" ID="pnlField" CssClass="FieldContentTable">
                                <asp:PlaceHolder ID="plcCategory" runat="server" Visible="false">
                                    <cms:CategoryEdit ID="categoryEdit" runat="server" ShortID="ce" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcField" runat="server" Visible="false">
                                    <asp:PlaceHolder ID="plcFieldType" runat="server">
                                        <div class="field-anchor" id="FieldType">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateFieldType" runat="server">
                                            <ContentTemplate>
                                                <cms:FieldTypeSelector ID="fieldTypeSelector" runat="server" ShortID="fts" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcDatabase" runat="server">
                                        <div class="field-anchor" id="Database">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateDatabase" runat="server">
                                            <ContentTemplate>
                                                <cms:DatabaseConfiguration ID="databaseConfiguration" runat="server" ShortID="dc" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <cms:CMSUpdatePanel ID="pnlUpdateDisplay" runat="server" Class="form-group">
                                        <ContentTemplate>
                                            <cms:CMSCheckBox ID="chkDisplayInForm" runat="server" AutoPostBack="True" OnCheckedChanged="chkDisplayInForm_CheckedChanged"
                                                CssClass="CheckBoxMovedLeft" ResourceString="templatedesigner.displayinform" />
                                        </ContentTemplate>
                                    </cms:CMSUpdatePanel>
                                    <asp:PlaceHolder ID="plcFieldAppearance" runat="server">
                                        <div class="field-anchor" id="FieldAppearance">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateAppearance" runat="server">
                                            <ContentTemplate>
                                                <cms:FieldAppearance ID="fieldAppearance" runat="server" ShortID="fa" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcSettings" runat="server">
                                        <div class="field-anchor" id="ControlSettings">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateSettings" runat="server">
                                            <ContentTemplate>
                                                <cms:ControlSettings ID="controlSettings" MinItemsToAllowSwitch="3" runat="server"
                                                    ShortID="cs" AllowModeSwitch="true" SimpleMode="true" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcValidation" runat="server">
                                        <div class="field-anchor" id="ValidationSettings">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateValidation" runat="server">
                                            <ContentTemplate>
                                                <cms:ValidationSettings ID="validationSettings" runat="server" ShortID="vs" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcCSS" runat="server">
                                        <div class="field-anchor" id="CSSStyles">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlUpdateCSS" runat="server">
                                            <ContentTemplate>
                                                <cms:CSSsettings ID="cssSettings" runat="server" ShortID="ss" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcHtmlEnvelope" runat="server">
                                        <div class="field-anchor" id="HtmlEnvelope">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlHtmlEnvelope" runat="server">
                                            <ContentTemplate>
                                                <cms:HTMLEnvelope ID="htmlEnvelope" runat="server" ShortID="he" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcFieldAdvancedSettings" runat="server">
                                        <div class="field-anchor" id="FieldAdvancedSettings">
                                        </div>
                                        <cms:CMSUpdatePanel ID="pnlFieldAdvancedSettings" runat="server">
                                            <ContentTemplate>
                                                <cms:FieldAdvancedSettings ID="fieldAdvancedSettings" runat="server" ShortID="fas" />
                                            </ContentTemplate>
                                        </cms:CMSUpdatePanel>
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>
                            </asp:Panel>
                        </asp:Panel>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        <div class="ClearBoth">
        </div>
    </asp:Panel>
</asp:Panel>
<asp:Literal ID="ltlConfirmText" runat="server" EnableViewState="false" />
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<script type="text/javascript">
    //<![CDATA[
    function confirmDelete() {
        return confirm(document.getElementById('confirmdelete').value);
    }
    //]]>
</script>
