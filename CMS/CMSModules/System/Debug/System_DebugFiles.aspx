<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_DebugFiles"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - Files"
     Codebehind="System_DebugFiles.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ContentPlaceHolderID="plcActions" runat="server">
    <div class="header-actions-container">
        <cms:CMSButton runat="server" ID="btnClear" OnClick="btnClear_Click" ButtonStyle="Default"
                EnableViewState="false" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisabledModule runat="server" ID="ucDisabled" InfoText="{$DebugFiles.NotConfigured$}" TestAnyKey="True" KeyScope="Global"
        SetSettingKeys="CMSDebugFiles"
        TestSettingKeys="CMSDebugFiles;CMSDebugEverything;CMSDebugEverythingEverywhere" 
        TestConfigKeys="CMSDebugFiles;CMSDebugEverything;CMSDebugEverythingEverywhere"  />
    <div class="clearfix">
        <div class="form-horizontal form-filter pull-left">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel runat="server" ID="lblOperationType" ResourceString="FilesLog.OperationType"
                        DisplayColon="true" EnableViewState="false" Visible="false" CssClass="control-label" AssociatedControlID="drpOperationType" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList runat="server" ID="drpOperationType" AutoPostBack="true" CssClass="input-width-60" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblWriteOnly" runat="server" CssClass="control-label" 
                        DisplayColon="True" ResourceString="FilesLog.WriteOnly" AssociatedControlID="chkWriteOnly" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkWriteOnly"
                        AutoPostBack="true" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel runat="server" CssClass="control-label" 
                        DisplayColon="True" ResourceString="Debug.ShowCompleteContext" AssociatedControlID="chkCompleteContext" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSCheckBox runat="server" ID="chkCompleteContext"
                        AutoPostBack="true" />
                </div>
            </div>
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcLogs" />
</asp:Content>

