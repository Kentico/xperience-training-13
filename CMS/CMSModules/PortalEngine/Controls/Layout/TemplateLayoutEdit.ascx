<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_Controls_Layout_TemplateLayoutEdit"
     Codebehind="TemplateLayoutEdit.ascx.cs" %>

<%@ Register Src="~/CMSModules/PortalEngine/FormControls/PageLayouts/PageLayoutSelector.ascx"
    TagName="LayoutSelector" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="PageLayoutCode" Src="~/CMSFormControls/Layouts/PageLayoutCode.ascx" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditPanel.ascx" TagName="ObjectEditPanel"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="CSSStylesEditor" Src="~/CMSFormControls/Layouts/CSSStylesEditor.ascx" %>

<asp:Panel runat="server" ID="pnlBody">
    <script type="text/javascript">
        function RefreshWOpener(w) {
            if (w.refreshPageOnClose) {
                window.location = window.location.href + '&refreshParent=1';
            }
        }

        function RefreshAfterDelete() {
            if (wopener.parent.RefreshAfterDelete) {
                wopener.parent.RefreshAfterDelete();
            }
        }
    </script>
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Always">
        <ContentTemplate>
            <cms:ObjectEditPanel runat="server" ID="editMenuElem" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <div class="PreviewBody">
        <div class="PageContent">
            <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false" />
            <asp:Panel runat="server" ID="pnlType" CssClass="form-horizontal">
                <div class="editing-form-label-cell">
                    <cms:CMSRadioButton runat="server" ID="radShared" GroupName="LayoutGroup" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LayoutSelector runat="server" ID="drpLayout" IsLiveSite="false" OnChanged="selectShared_Changed" />
                </div>
                <div>
                    <cms:CMSRadioButton runat="server" ID="radCustom" GroupName="LayoutGroup" />
                </div>
            </asp:Panel>
            <cms:UIForm runat="server" ID="EditFormTemplate" ObjectType="cms.pagetemplate" DefaultFieldLayout="Default"
                OnOnCreate="EditForm_Create" EnabledByLockState="False">
                <SecurityCheck Resource="CMS.Design" Permission="Design" />
                <LayoutTemplate>
                    <cms:FormField runat="server" ID="fCode" Field="PageTemplateLayout" UseFFI="false" Layout="Inline">
                        <cms:PageLayoutCode runat="server" ID="codeElem" CodeColumn="PageTemplateLayout"
                            TypeColumn="PageTemplateLayoutType" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fCSS" Field="PageTemplateCSS" UseFFI="false" Layout="Inline">
                        <cms:CSSStylesEditor runat="server" ID="cssEditor" />
                    </cms:FormField>
                </LayoutTemplate>
            </cms:UIForm>
            <cms:UIForm runat="server" ID="EditFormLayout" ObjectType="cms.layout" DefaultFieldLayout="TwoColumns"
                EnabledByLockState="False">
                <SecurityCheck Resource="CMS.Design" Permission="Design" />
                <LayoutTemplate>
                    <cms:CMSUpdatePanel runat="server" ID="pnlServer" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:FormCategory runat="server" ID="pnlGeneral" CategoryTitleResourceString="general.general">
                                <cms:FormField runat="server" ID="fDisplayName" Field="LayoutDisplayName" FormControl="LocalizableTextBox"
                                    ResourceString="general.displayname" DisplayColon="true" />
                                <cms:FormField runat="server" ID="fName" Field="LayoutCodeName" FormControl="CodeName"
                                    ResourceString="general.codename" DisplayColon="true" />
                                <cms:FormField runat="server" ID="fDescription" Field="LayoutDescription" FormControl="TextAreaControl"
                                    ResourceString="general.description" DisplayColon="true" />
                            </cms:FormCategory>
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                    <cms:FormCategory runat="server" ID="pnlLayout" CategoryTitleResourceString="general.layout">
                        <cms:FormField runat="server" ID="fLayoutCode" Field="LayoutCode" Layout="Inline" UseFFI="false">
                            <cms:PageLayoutCode runat="server" ID="codeLayoutElem" CodeColumn="LayoutCode" TypeColumn="LayoutType" />
                        </cms:FormField>
                        <cms:FormField runat="server" ID="fLayoutCSS" Field="LayoutCSS" UseFFI="false" Layout="Inline">
                            <cms:CSSStylesEditor runat="server" ID="cssLayoutEditor" />
                        </cms:FormField>
                    </cms:FormCategory>
                </LayoutTemplate>
            </cms:UIForm>
        </div>
    </div>
</asp:Panel>
<asp:HiddenField ID="hdnWOpenerRefreshed" runat="server" />
<asp:HiddenField ID="hdnClose" runat="server" EnableViewState="false" />
