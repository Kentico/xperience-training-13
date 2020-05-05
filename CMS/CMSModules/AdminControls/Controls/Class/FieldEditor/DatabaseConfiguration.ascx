<%@ Control Language="C#" AutoEventWireup="True"  Codebehind="DatabaseConfiguration.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_DatabaseConfiguration" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownListControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/EnumSelector.ascx" TagName="EnumSelector"
    TagPrefix="cms" %>
<cms:LocalizedHeading runat="server" Level="4" ResourceString="general.general"></cms:LocalizedHeading>
<asp:Panel ID="pnlDatabase" runat="server" CssClass="FieldPanel">
    <div class="form-horizontal">
        <asp:PlaceHolder ID="plcGroup" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblGroup" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.Group"
                        AssociatedControlID="drpGroup" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSDropDownList ID="drpGroup" runat="server" CssClass="DropDownField" OnSelectedIndexChanged="drpGroup_SelectedIndexChanged"
                        AutoPostBack="true" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAttributeName" runat="server" EnableViewState="false"
                    ResourceString="TemplateDesigner.FieldName" AssociatedControlID="txtAttributeName" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList ID="drpSystemFields" runat="server" CssClass="DropDownField" OnSelectedIndexChanged="drpSystemFields_SelectedIndexChanged"
                    AutoPostBack="true" DataValueField="ColumnName" DataTextField="ColumnName" />
                <cms:CMSTextBox ID="txtAttributeName" runat="server"  MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAttributeType" runat="server" EnableViewState="false"
                    ResourceString="TemplateDesigner.FieldType" AssociatedControlID="drpAttributeType" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList ID="drpAttributeType" runat="server" DataTextField="Text" DataValueField="Value"
                    CssClass="DropDownField" AutoPostBack="True" OnSelectedIndexChanged="drpAttributeType_SelectedIndexChanged" />
                </>
            </div>
        </div>
        <asp:PlaceHolder ID="plcAttributeSize" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblAttributeSize" runat="server" EnableViewState="false"
                        ResourceString="TemplateDesigner.FieldSize" AssociatedControlID="txtAttributeSize"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtAttributeSize" runat="server"  MaxLength="9" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcAttributePrecision" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblAttributePrecision" runat="server" EnableViewState="false"
                        ResourceString="TemplateDesigner.FieldPrecision" AssociatedControlID="txtAttributePrecision"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtAttributePrecision" runat="server" MaxLength="2" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcRequired" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblRequired" runat="server" EnableViewState="false" ResourceString="templatedesigner.attributerequired"
                        AssociatedControlID="chkRequired" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkRequired" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcDefaultValue" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDefaultValue" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.ColumnDefaultValue" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EditingFormControl ID="txtDefaultValue" runat="server" AllowMacroEditing="true"
                        FormControlName="TextBoxControl" Visible="false" />
                    <cms:EditingFormControl ID="txtLargeDefaultValue" runat="server" AllowMacroEditing="true"
                        FormControlName="LargeTextArea" Visible="false" />
                    <cms:EditingFormControl ID="chkDefaultValue" runat="server" AllowMacroEditing="true"
                        FormControlName="CheckBoxControl" Visible="false" />
                    <cms:EditingFormControl ID="rbDefaultValue" runat="server" AllowMacroEditing="true"
                        FormControlName="ThreeStateCheckBox" Visible="False" />
                    <cms:EditingFormControl ID="datetimeDefaultValue" runat="server" AllowMacroEditing="true"
                        FormControlName="CalendarControl" Visible="false" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcResolveDefaultValue" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblResolveDefaultValue" runat="server" EnableViewState="false" CssClass="control-label"
                            ResourceString="TemplateDesigner.ColumnResolveDefaultValue" DisplayColon="true" AssociatedControlID="chkResolveDefaultValue" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkResolveDefaultValue" runat="server" CssClass="CheckBoxMovedLeft" Checked="true" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcUnique" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUnique" runat="server" EnableViewState="false" ResourceString="general.unique"
                        AssociatedControlID="chkUnique" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkUnique" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcIsSystem" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblIsSystem" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.FieldIsSystem"
                        DisplayColon="true" AssociatedControlID="chkIsSystem" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkIsSystem" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcReference" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblReferenceToObjectType" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.ReferenceToObjectType"
                        DisplayColon="true" AssociatedControlID="drpObjType" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DropDownListControl ID="drpObjType" CssClass="DropDownField" runat="server" MacroSource="Flatten(&quot;&quot;, ObjectTypes.AllObjectTypes)" TextFormat="{% if (Item != &quot;&quot;) { GetObjectTypeName(Item) } else { GetResourceString(&quot;general.selectnone&quot;) } %}" SortItems="True" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcReferenceType" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblReferenceType" runat="server" EnableViewState="false" ResourceString="TemplateDesigner.ReferenceType"
                            DisplayColon="true" AssociatedControlID="drpReferenceType" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:EnumSelector ID="drpReferenceType" runat="server" AssemblyName="CMS.DataEngine" TypeName="CMS.DataEngine.ObjectDependencyEnum" UseStringRepresentation="True" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcTranslation" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblTranslateField" runat="server" EnableViewState="false"
                        ResourceString="TemplateDesigner.AttributeTranslateField" DisplayColon="true"
                        AssociatedControlID="chkTranslateField" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkTranslateField" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblGuid" runat="server" EnableViewState="false" ResourceString="General.Guid" DisplayColon="true" AssociatedControlID="lblGuidValue" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label ID="lblGuidValue" runat="server" EnableViewState="true" CssClass="form-control-text" />
            </div>
        </div>
    </div>
</asp:Panel>