<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_AlternativeFormEdit"
     Codebehind="AlternativeFormEdit.ascx.cs" %>
<cms:UIForm ID="form" runat="server" ObjectType="cms.alternativeform" RefreshHeader="true">
    <LayoutTemplate>
        <cms:FormField runat="server" ID="fDispName" Field="FormDisplayName" FormControl="LocalizableTextBox"
            ResourceString="general.displayname" />
        <cms:FormField runat="server" ID="fName" Field="FormName" FormControl="CodeName" Layout="Inline" UseFFI="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:FormLabel CssClass="control-label" ID="lblCodeName" runat="server" EnableViewState="false" ResourceString="general.codename"
                        DisplayColon="true" AssociatedControlID="codeName" ShowRequiredMark="true"/>
                </div>
                <div class="editing-form-value-cell">
                    <cms:FormControl ID="codeName" FormControlName="CodeName" runat="server">
                        <Properties>
                            <cms:Property Name="RequireIdentifier" Value="true" />
                        </Properties>
                    </cms:FormControl>
                </div>
            </div>
        </cms:FormField>
        <cms:FormField runat="server" ID="fInherits" Field="FormHideNewParentFields" ToolTipResourceString="alternativeforms.hidenewparentfields.tooltip" FormControl="CheckBoxControl"
            ResourceString="alternativeforms.hidenewparentfields" />
        <asp:PlaceHolder runat="server" ID="pnlCombineUserSettings" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCombineUserSettings" runat="server" EnableViewState="false"
                        DisplayColon="true" AssociatedControlID="chkCombineUserSettings" ResourceString="altform.combineusersettings" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkCombineUserSettings" runat="server" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <cms:FormSubmit runat="server" ID="fSubmit" />
    </LayoutTemplate>
</cms:UIForm>
