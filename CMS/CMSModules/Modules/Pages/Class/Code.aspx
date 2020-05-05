<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Code.aspx.cs" Inherits="CMSModules_Modules_Pages_Class_Code" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Class - Code" Theme="Default" %>

<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagName="FileSystemSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/DropDownListControl.ascx" TagName="DropDownListControl"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="general.objecttype" DisplayColon="True" AssociatedControlID="txtObjectType" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSTextBox runat="server" ID="txtObjectType" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkUseIdHashtable" ResourceString="Classes.Code.UseIdHashtable" />
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcNormalInfo">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="classes.code.displaynamecolumn"
                        DisplayColon="True" AssociatedControlID="drpDisplayNameColumn" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DropDownListControl ID="drpDisplayNameColumn" CssClass="DropDownField" runat="server" SortItems="True" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="classes.code.codenamecolumn" DisplayColon="True" AssociatedControlID="drpCodeNameColumn" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                        <cms:DropDownListControl ID="drpCodeNameColumn" CssClass="DropDownField" runat="server" SortItems="True" />
                    </div>
                    <div class="control-group-inline">
                        <cms:CMSCheckBox runat="server" ID="chkUseNameHashtable" ResourceString="Classes.Code.UseNameHashtable" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="classes.code.guidcolumn" DisplayColon="True" AssociatedControlID="drpGuidColumn" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                        <cms:DropDownListControl ID="drpGuidColumn" CssClass="DropDownField" runat="server" SortItems="True" />
                    </div>
                    <div class="control-group-inline">
                        <cms:CMSCheckBox runat="server" ID="chkUseGuidHashtable" ResourceString="Classes.Code.UseGuidHashtable" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="classes.code.lastmodifiedcolumn" DisplayColon="True" AssociatedControlID="drpLastModifiedColumn" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DropDownListControl ID="drpLastModifiedColumn" CssClass="DropDownField" runat="server" SortItems="True" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="classes.code.binarycolumn" DisplayColon="True" AssociatedControlID="drpBinaryColumn" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DropDownListControl ID="drpBinaryColumn" CssClass="DropDownField" runat="server" SortItems="True" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="classes.code.siteidcolumn" DisplayColon="True" AssociatedControlID="drpSiteIdColumn" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DropDownListControl ID="drpSiteIdColumn" CssClass="DropDownField" runat="server" SortItems="True" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblNamespace" runat="server" ResourceString="classes.code.namespace" DisplayColon="True" AssociatedControlID="txtNamespace" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtNamespace" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblLegacyV12Code" runat="server" ResourceString="classes.code.legacyv12code" DisplayColon="True" AssociatedControlID="chkLegacyV12Code" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkLegacyV12Code" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:LocalizedButton runat="server" ID="btnGenerateCode" ButtonStyle="Primary" ResourceString="Classes.Code.GenerateCode" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="Classes.Code.SavePath" DisplayColon="True" AssociatedControlID="ucSaveFsSelector" />
            </div>
            <div class="editing-form-value-cell">
                <cms:FileSystemSelector runat="server" ID="ucSaveFsSelector" ShowFolders="True" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:LocalizedButton runat="server" ID="btnSaveCode" ButtonStyle="Primary" ResourceString="Classes.Code.SaveCode" />
            </div>
        </div>
    </div>
    <asp:PlaceHolder ID="plcLegacyV12Code" runat="server">
        <div class="layout-2-columns">
	        <div class="col-50">
		        <cms:LocalizedHeading runat="server" ID="headInfoLegacyV12Code" Level="4" EnableViewState="false" ResourceString="Classes.Code.InfoCode" DisplayColon="True" />
		        <cms:ExtendedTextArea ID="txtInfoLegacyV12Code" runat="server" ReadOnly="true" EditorMode="Advanced" Language="CSharp" Height="450px" />
		        <asp:HiddenField runat="server" ID="hdnInfoLegacyV12ClassName" />
	        </div>
	        <div class="col-50">
		        <cms:LocalizedHeading runat="server" ID="headProviderLegacyV12Code" Level="4" EnableViewState="false" ResourceString="Classes.Code.ProviderCode" DisplayColon="True" />
		        <cms:ExtendedTextArea ID="txtInfoProviderLegacyV12Code" runat="server" ReadOnly="true" EditorMode="Advanced" Language="CSharp" Height="450px" />
		        <asp:HiddenField runat="server" ID="hdnInfoProviderLegacyV12ClassName" />
	        </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcCurrentCode" runat="server">
        <div class="layout-3-columns">
            <div class="col-33">
                <cms:LocalizedHeading runat="server" ID="headInfoCode" Level="4" EnableViewState="false" ResourceString="Classes.Code.InfoCode" DisplayColon="True" />
                <cms:ExtendedTextArea ID="txtInfoCode" runat="server" ReadOnly="true" EditorMode="Advanced" Language="CSharp" Height="450px" />
                <asp:HiddenField runat="server" ID="hdnInfoClassName" />
            </div>
            <div class="col-33">
                <cms:LocalizedHeading runat="server" ID="headProviderInterfaceCode" Level="4" EnableViewState="false" ResourceString="Classes.Code.ProviderInterfaceCode" DisplayColon="True" />
                <cms:ExtendedTextArea ID="txtInfoProviderInterfaceCode" runat="server" ReadOnly="true" EditorMode="Advanced" Language="CSharp" Height="450px" />
                <asp:HiddenField runat="server" ID="hdnInfoProviderInterfaceName" />
            </div>
            <div class="col-33">
                <cms:LocalizedHeading runat="server" ID="headProviderCode" Level="4" EnableViewState="false" ResourceString="Classes.Code.ProviderCode" DisplayColon="True" />
                <cms:ExtendedTextArea ID="txtInfoProviderCode" runat="server" ReadOnly="true" EditorMode="Advanced" Language="CSharp" Height="450px" />
                <asp:HiddenField runat="server" ID="hdnInfoProviderClassName" />
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
