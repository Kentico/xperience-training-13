<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="General.ascx.cs" Inherits="CMSModules_PortalEngine_Controls_Layout_General" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditPanel.ascx" TagName="ObjectEditPanel"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/CheckBoxControl.ascx" TagPrefix="cms" TagName="CheckBoxControl" %>
<%@ Register TagPrefix="cms" TagName="CSSStylesEditor" Src="~/CMSFormControls/Layouts/CSSStylesEditor.ascx" %>


<asp:Panel runat="server" ID="pnlBody" CssClass="TabsPageBody SimpleHeader">
    <asp:Panel ID="pnlTab" runat="server" CssClass="TabsPageContent">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
            <ContentTemplate>
                <cms:ObjectEditPanel runat="server" ID="editMenuElem" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <asp:Panel ID="pnlFormArea" runat="server" CssClass="PreviewBody">
            <asp:Panel runat="server" Visible="true" ID="pnlContent" EnableViewState="True" CssClass="PageContent WebPartLayoutContent">
                <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false" />
                <cms:UIForm runat="server" ID="EditForm" ObjectType="cms.webpartlayout" DefaultFieldLayout="TwoColumns"
                    RedirectUrlAfterCreate="" OnOnAfterDataLoad="EditForm_OnAfterDataLoad">
                    <LayoutTemplate>
                        <asp:PlaceHolder runat="server" ID="plcValues" Visible="false">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDisplayName" ResourceString="general.displayname"
                                        DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:LocalizableTextBox ID="txtDisplayName" runat="server" MaxLength="100" Field="WebPartLayoutDisplayName" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCodeName" ResourceString="general.codename"
                                        DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CodeName runat="server" ID="txtCodeName" Field="WebPartLayoutCodeName" />
                                </div>
                            </div>
                            <asp:Panel runat="server" ID="plcDescription" Visible="false" CssClass="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblDescription" runat="server" ResourceString="general.description"
                                        DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:LocalizableTextBox ID="txtDescription" runat="server" MaxLength="450" Field="WebPartLayoutDescription"
                                        TextMode="MultiLine" />
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="plcIsDefault" Visible="false" CssClass="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel CssClass="control-label" ID="lblIsDefault" runat="server" ResourceString="webparteditlayoutedit.isdefault"
                                        ToolTipResourceString="webparteditlayoutedit.isDefaultTooltip" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CheckBoxControl runat="server" ID="chkIsDefault" Field="WebPartLayoutIsDefault" />
                                </div>
                            </asp:Panel>
                        </asp:PlaceHolder>
                        <div class="form-group">
                            <cms:MacroEditor runat="server" ID="etaCode" Field="WebPartLayoutCode" />
                        </div>
                        <div class="form-group">
                            <cms:CSSStylesEditor runat="server" ID="cssLayoutEditor" Field="WebPartLayoutCss" />
                        </div>
                    </LayoutTemplate>
                </cms:UIForm>
                <asp:HiddenField runat="server" ID="hidRefresh" Value="0" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
<asp:HiddenField ID="hdnClose" runat="server" EnableViewState="false" />
