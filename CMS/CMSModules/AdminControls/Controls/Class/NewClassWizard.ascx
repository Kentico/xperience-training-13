<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_NewClassWizard"
    CodeBehind="NewClassWizard.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx"
    TagName="FieldEditor" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Wizard/Header.ascx" TagName="WizardHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectClass.ascx" TagName="SelectClass"
    TagPrefix="cms" %>

<div class="GlobalWizard new-class-wizard">
    <table>
        <tr class="Top">
            <td class="Left">&nbsp;
            </td>
            <td class="Center">
                <cms:WizardHeader ID="ucHeader" runat="server" />
            </td>
            <td class="Right">&nbsp;
            </td>
        </tr>
        <tr class="Middle">
            <td class="Center" colspan="3">
                <div id="wzdBody">
                    <asp:Wizard ID="wzdNewDocType" runat="server" DisplaySideBar="false" OnNextButtonClick="wzdNewDocType_NextButtonClick"
                        OnFinishButtonClick="wzdNewDocType_FinishButtonClick" CssClass="Wizard">
                        <StartNavigationTemplate>
                            <div id="buttonsDiv" class="WizardButtons">
                                <cms:LocalizedButton UseSubmitBehavior="True" ID="StepNextButton" runat="server"
                                    CommandName="MoveNext" Text="{$general.next$}" ButtonStyle="Primary" />
                            </div>
                        </StartNavigationTemplate>
                        <StepNavigationTemplate>
                            <div id="buttonsDiv" class="WizardButtons">
                                  <cms:LocalizedLinkButton ID="btnContinueWithoutCustomFields" runat="server" CommandName="ContinueWithoutCustomFields"
                                    Text="{$DocumentType_New.ContinueWithoutCustomFields$}" OnClick="btnContinueWithoutCustomFields_Click" Visible ="false" CssClass="wizardContinueWithoutCustomFields" />
                                <cms:LocalizedButton UseSubmitBehavior="True" ID="StepNextButton" runat="server"
                                    CommandName="MoveNext" Text="{$general.next$}" ButtonStyle="Primary" />
                            </div>
                        </StepNavigationTemplate>
                        <FinishNavigationTemplate>
                            <div id="buttonsDiv" class="WizardButtons">
                                <cms:LocalizedButton UseSubmitBehavior="True" ID="StepFinishButton" runat="server"
                                    CommandName="MoveComplete" ResourceString="general.finish" ButtonStyle="Primary" />
                            </div>
                        </FinishNavigationTemplate>
                        <WizardSteps>
                            <%-- Step general --%>
                            <asp:WizardStep ID="wzdStepGeneral" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStepGeneral" runat="server" CssClass="GlobalWizardStep">
                                    <cms:MessagesPlaceHolder runat="server" ID="pnlMessages1" />
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblDisplayName" AssociatedControlID="txtDisplayName"
                                                    ShowRequiredMark="true" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <cms:LocalizableTextBox runat="server" ID="txtDisplayName" MaxLength="100" />
                                                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName:cntrlContainer:textbox"
                                                    Display="dynamic" />
                                            </div>
                                        </div>
                                    </div>
                                    <cms:LocalizedHeading runat="server" ID="lblFullCodeName" Level="4" />
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblNamespaceName" ShowRequiredMark="true" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <cms:CMSTextBox runat="server" ID="txtNamespaceName" MaxLength="49" />
                                                <cms:CMSRequiredFieldValidator ID="rfvNamespaceName" runat="server" EnableViewState="false"
                                                    ControlToValidate="txtNamespaceName" Display="dynamic" />
                                                <cms:CMSRegularExpressionValidator ID="revNameSpaceName" runat="server" EnableViewState="false"
                                                    Display="dynamic" ControlToValidate="txtNamespaceName" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblCodeName" ShowRequiredMark="true" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <cms:CMSTextBox runat="server" ID="txtCodeName" MaxLength="50" />
                                                <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" EnableViewState="false"
                                                    ControlToValidate="txtCodeName" Display="dynamic" />
                                                <cms:CMSRegularExpressionValidator ID="revCodeName" runat="server" EnableViewState="false"
                                                    Display="dynamic" ControlToValidate="txtCodeName" />
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Features type --%>
                            <asp:WizardStep ID="wzdStepFeatures" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStepFeatures" runat="server" CssClass="GlobalWizardStep features-selector">
                                    <div class="feature-card" data-feature="pagebuilder">
                                        <div class="content">
                                            <div class="checkbox-feature">
                                                 <cms:CMSCheckBox ID="chkPageBuilderFeature" runat="server" Checked="False" />
                                            </div>
                                            <div class="icon">
                                                <img runat="server" src="~/CMSPages/GetResource.ashx?image=Wizard/use-page-builder.png"/>
                                            </div>
                                            <div class="title">
                                                <asp:Label runat="server" ID="lblPageBuilder" />
                                            </div>
                                            <div class="description">
                                                <span class="info-icon">
                                                    <asp:Label runat="server" ID="spanScreenReaderPageBuilder" CssClass="sr-only"></asp:Label>
                                                    <cms:CMSIcon runat="server" id="iconHelpPageBuilder" enableviewstate="false" class="icon-question-circle" aria-hidden="true" ></cms:CMSIcon>
                                                </span>
                                            </div>
                                        </div>
                                    </div>                                    
                                    <div class="feature-card" data-feature="url">
                                        <div class="content">
                                            <div class="checkbox-feature">
                                                 <cms:CMSCheckBox ID="chkUrl" runat="server" Checked="False" />
                                            </div>
                                            <div class="icon">
                                                <img runat="server" src="~/CMSPages/GetResource.ashx?image=Wizard/routing.png"/>
                                            </div>
                                            <div class="title">
                                                <asp:Label runat="server" ID="lblUrl" />
                                            </div>
                                            <div class="description">
                                                <span class="info-icon">
                                                    <asp:Label runat="server" ID="spanScreenReaderUrl" CssClass="sr-only"></asp:Label>
                                                    <cms:CMSIcon runat="server" id="iconHelpUrl" enableviewstate="false" class="icon-question-circle" aria-hidden="true" ></cms:CMSIcon>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="feature-card" data-feature="metadata">
                                        <div class="content">
                                            <div class="checkbox-feature">
                                                 <cms:CMSCheckBox ID="chkMetadata" runat="server" Checked="False" />
                                            </div>
                                            <div class="icon">
                                                <img runat="server" src="~/CMSPages/GetResource.ashx?image=Wizard/metadata.png"/>
                                            </div>
                                            <div class="title">
                                                <asp:Label runat="server" ID="lblMetadata" />
                                            </div>
                                            <div class="description">
                                                <span class="info-icon">
                                                    <asp:Label runat="server" ID="spanScreenReaderMetadata" CssClass="sr-only"></asp:Label>
                                                    <cms:CMSIcon runat="server" id="iconHelpMetadata" enableviewstate="false" class="icon-question-circle" aria-hidden="true" ></cms:CMSIcon>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="feature-card" data-feature="navigationitem">
                                        <div class="content">
                                            <div class="checkbox-feature">
                                                 <cms:CMSCheckBox ID="chkNavigationItem" runat="server" Checked="False" />
                                            </div>
                                            <div class="icon">
                                                <img runat="server" src="~/CMSPages/GetResource.ashx?image=Wizard/navigation-item.png"/>
                                            </div>
                                            <div class="title">
                                                <asp:Label runat="server" ID="lblNavigationItem" />
                                            </div>
                                            <div class="description">
                                                <span class="info-icon">
                                                    <asp:Label runat="server" ID="spanScreenReaderNavigationItem" CssClass="sr-only"></asp:Label>
                                                    <cms:CMSIcon runat="server" id="iconHelpNavigationItem" enableviewstate="false" class="icon-question-circle" aria-hidden="true" ></cms:CMSIcon>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step database information --%>
                            <asp:WizardStep ID="wzdStepDatabaseInformation" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStepDatabaseInformation" runat="server" CssClass="GlobalWizardStep">
                                    <cms:MessagesPlaceHolder runat="server" ID="pnlMessages2" />
                                    <div class="form-horizontal">
                                        <div class="radio-list-vertical">
                                            <asp:PlaceHolder ID="plcExisting" runat="server" Visible="false">
                                                <cms:CMSRadioButton ID="radNewTable" runat="server" Checked="True" GroupName="CustomTable"
                                                    AutoPostBack="true" OnCheckedChanged="radExistingTable_CheckedChanged" />
                                                <cms:CMSRadioButton ID="radExistingTable" runat="server" GroupName="CustomTable" AutoPostBack="true"
                                                    OnCheckedChanged="radExistingTable_CheckedChanged" />
                                            </asp:PlaceHolder>
                                            <div class="selector-subitem">
                                                <div class="form-group">
                                                    <div class="editing-form-label-cell">
                                                        <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblTableName" ShowRequiredMark="true" />
                                                    </div>
                                                    <div class="editing-form-value-cell">
                                                        <cms:CMSDropDownList runat="server" ID="drpExistingTables" Visible="false" CssClass="DropDownField" />
                                                        <cms:CMSTextBox ID="txtTableName" runat="server" MaxLength="100" />

                                                        <asp:Label ID="lblTableNameError" runat="server" CssClass="form-control-error" Visible="false" />
                                                    </div>
                                                </div>
                                                <asp:PlaceHolder runat="server" ID="plcPKName">
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblPKName" ShowRequiredMark="true" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSTextBox ID="txtPKName" runat="server" MaxLength="100" />
                                                            <asp:Label ID="lblPKNameError" runat="server" CssClass="form-control-error" Visible="false" />
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" ID="plcDocTypeOptions" Visible="false">
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblInherits" EnableViewState="false" ResourceString="DocumentType.InheritsFrom" AssociatedControlID="selInherits" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:SelectClass ID="selInherits" runat="server" DisplayNoneValue="true" />
                                                        </div>
                                                    </div>
                                                   
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plcMNClassOptions" runat="server" Visible="false">
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblIsMNTable" runat="server" AssociatedControlID="chbIsMNTable" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chbIsMNTable" runat="server" Checked="False" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblClassGuid" runat="server" AssociatedControlID="chkClassGuid" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkClassGuid" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblClassLastModified" runat="server" AssociatedControlID="chkClassLastModified" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkClassLastModified" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plcCustomTablesOptions" runat="server" Visible="false">
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemGUID" runat="server" AssociatedControlID="chkItemGUID" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemGUID" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemCreatedBy" runat="server" AssociatedControlID="chkItemCreatedBy" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemCreatedBy" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemCreatedWhen" runat="server" AssociatedControlID="chkItemCreatedWhen" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemCreatedWhen" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemModifiedBy" runat="server" AssociatedControlID="chkItemModifiedBy" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemModifiedBy" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemModifiedWhen" runat="server" AssociatedControlID="chkItemModifiedWhen" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemModifiedWhen" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <asp:PlaceHolder runat="server" ID="plcOrder">
                                                        <div class="form-group">
                                                            <div class="editing-form-label-cell">
                                                                <asp:Label CssClass="control-label" ID="lblItemOrder" runat="server" AssociatedControlID="chkItemOrder" />
                                                            </div>
                                                            <div class="editing-form-value-cell">
                                                                <cms:CMSCheckBox ID="chkItemOrder" runat="server" Checked="true" />
                                                            </div>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                </asp:PlaceHolder>
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step fields --%>
                            <asp:WizardStep ID="wzdStepFields" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStepFields" runat="server" CssClass="GlobalWizardStep FieldEditorPanel"
                                    Height="500px">
                                    <cms:FieldEditor ID="FieldEditor" IsLiveSite="false" runat="server" DisplaySourceFieldSelection="false" UseCustomHeaderActions="true" ShowQuickLinks="false" AllowDummyFields="true" />
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step parent types --%>
                            <asp:WizardStep ID="wzdStepParentTypes" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStepParentTypes" runat="server" CssClass="GlobalWizardStep">
                                    <cms:UniSelector ID="usParentTypes" runat="server" IsLiveSite="false" ObjectType="cms.documenttype"
                                        SelectionMode="Multiple" ResourcePrefix="allowedclasscontrol" DisplayNameFormat="{%ClassDisplayName%} ({%ClassName%})" />
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step sites --%>
                            <asp:WizardStep ID="wzdStepSites" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStepSites" runat="server" CssClass="GlobalWizardStep">
                                    <cms:MessagesPlaceHolder runat="server" ID="pnlMessages6" />
                                    <cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
                                        SelectionMode="Multiple" ResourcePrefix="sitesselect" />
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step final overview --%>
                            <asp:WizardStep ID="wzdStepFinalOverview" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStepFinalOverview" runat="server" CssClass="GlobalWizardStep">
                                    <cms:LocalizedHeading runat="server" ID="headInfoStep7" Level="3" EnableViewState="false" ResourceString="documenttype_new_step8.info" />
                                    <asp:Label runat="server" ID="lblDocumentCreated" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblTableCreated" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblChildTypesAdded" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblSitesSelected" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblPermissionNameCreated" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblDefaultIconCreated" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblSearchSpecificationCreated" CssClass="WizardFinishedStep" />
                                </asp:Panel>
                            </asp:WizardStep>
                        </WizardSteps>
                    </asp:Wizard>
                </div>
            </td>
        </tr>
    </table>
</div>
