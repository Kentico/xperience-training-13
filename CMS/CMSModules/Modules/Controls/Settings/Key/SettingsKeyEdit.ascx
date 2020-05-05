<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="SettingsKeyEdit.ascx.cs"
    Inherits="CMSModules_Modules_Controls_Settings_Key_SettingsKeyEdit" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Settings/FormControls/SettingsKeyControlSelector.ascx" TagName="SettingsKeyControlSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/UIControls/../Class/FieldEditor/ControlSettings.ascx" TagName="ControlSettings" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea" TagPrefix="cms" %>

<asp:Panel ID="plnEdit" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <%-- General --%>
            <asp:Panel runat="server" ID="pnlGeneral">
                <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.general"></cms:LocalizedHeading>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyDisplayName" runat="server" EnableViewState="false"
                                ResourceString="general.displayname" ShowRequiredMark="True" DisplayColon="true" AssociatedControlID="txtKeyDisplayName" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:LocalizableTextBox ID="txtKeyDisplayName" runat="server"
                                MaxLength="200" ValidationGroup="vgKey" EnableViewState="false" />
                            <cms:CMSRequiredFieldValidator ID="rfvKeyDisplayName" runat="server" Display="Dynamic"
                                ControlToValidate="txtKeyDisplayName:cntrlContainer:textbox" ValidationGroup="vgKey" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyName" runat="server" EnableViewState="false" ResourceString="general.codename"
                                DisplayColon="true" ShowRequiredMark="True" AssociatedControlID="txtKeyName" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtKeyName" runat="server" MaxLength="100"
                                ValidationGroup="vgKey" EnableViewState="false" />
                            <cms:CMSRequiredFieldValidator ID="rfvKeyName" runat="server" Display="Dynamic" ControlToValidate="txtKeyName"
                                ValidationGroup="vgKey" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyDescription" runat="server" EnableViewState="false"
                                ResourceString="general.description" DisplayColon="true" AssociatedControlID="txtKeyDescription" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:LocalizableTextBox ID="txtKeyDescription" runat="server" TextMode="MultiLine" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyExplanationText" runat="server" EnableViewState="false"
                                ResourceString="settings.explanationtext" DisplayColon="true" AssociatedControlID="txtKeyExplanationText" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:LocalizableTextBox ID="txtKeyExplanationText" runat="server" TextMode="MultiLine" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group" id="trCategory" runat="server">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyCategory" runat="server" EnableViewState="false" ResourceString="settings.groupname"
                                DisplayColon="true" AssociatedControlID="drpCategory" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:SelectSettingsCategory ID="drpCategory" runat="server" DisplayOnlyCategories="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyIsGlobal" runat="server" EnableViewState="false" ResourceString="settings.keyisglobal"
                                DisplayColon="true" AssociatedControlID="chkKeyIsGlobal" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkKeyIsGlobal" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyIsHidden" runat="server" EnableViewState="false" ResourceString="settings.keyishidden"
                                DisplayColon="true" AssociatedControlID="chkKeyIsHidden" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkKeyIsHidden" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-value-cell editing-form-value-cell-offset">
                            <cms:FormSubmitButton ID="btnOk" runat="server" EnableViewState="false" CssClass="js-button-ok"
                                ValidationGroup="vgKey" ResourceString="General.OK" OnClick="btnOK_Click" />
                        </div>
                    </div>

                </div>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlValue">
                <cms:LocalizedHeading runat="server" Level="4" ResourceString="general.value"></cms:LocalizedHeading>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyType" runat="server" EnableViewState="false" ResourceString="settings.datatype"
                                DisplayColon="true" AssociatedControlID="drpKeyType" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSDropDownList ID="drpKeyType" runat="server" AutoPostBack="true" CssClass="DropDownField"
                                OnSelectedIndexChanged="drpKeyType_SelectedIndexChanged" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyValue" runat="server" EnableViewState="false" ResourceString="settings.defaultvalue"
                                DisplayColon="true" AssociatedControlID="txtKeyValue" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkKeyValue" runat="server" EnableViewState="false" />
                            <cms:CMSTextBox ID="txtKeyValue" runat="server" EnableViewState="false" />
                            <cms:LargeTextArea ID="txtLongTextKeyValue" runat="server" />
                            <asp:Label ID="lblDefValueError" runat="server" CssClass="ErrorLabel" Style="display: inline;" EnableViewState="false" Visible="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblKeyValidation" runat="server" EnableViewState="false"
                                ResourceString="settings.validationexpresion" DisplayColon="true" AssociatedControlID="txtKeyValidation" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtKeyValidation" runat="server" MaxLength="255"
                                EnableViewState="false" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblFormControl" runat="server" EnableViewState="false" ResourceString="settings.customcontrol"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:SettingsKeyControlSelector ID="ucSettingsKeyControlSelector" runat="server" OnChanged="ucSettingsKeyControlSelector_Changed" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <%-- Form control settings --%>
            <asp:Panel runat="server" ID="pnlControlSettings">
                <cms:ControlSettings ID="ucControlSettings" runat="server" MinItemsToAllowSwitch="3" SimpleMode="True" AllowModeSwitch="true" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>