<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="NewPage.ascx.cs" Inherits="CMSModules_OnlineMarketing_Controls_UI_ABVariant_NewPage" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<div class="PageContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="txtDocumentName" CssClass="control-label" runat="server" ID="lblDocumentName" ResourceString="general.documentname"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtDocumentName" MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvDocumentName" Display="Dynamic" runat="server"
                    ControlToValidate="txtDocumentName" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="ucPath" CssClass="control-label" runat="server" ID="lblSaveTo" ResourceString="om.saveto" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:SelectPath runat="server" ID="ucPath" IsLiveSite="false" SinglePathMode="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="ucABTestSelector" CssClass="control-label" runat="server" ID="lblAssignTo" ResourceString="om.assigntoabtest"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:UniSelector runat="server" ID="ucABTestSelector" ObjectType="OM.ABTest" SelectionMode="SingleDropDownList" ResourcePrefix="selectabtest" ReturnColumnName="ABTestID" ObjectSiteName="#currentsite" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="chkExcludeFromSearch" CssClass="control-label" runat="server" ID="lblExcludeFromSearch" ResourceString="om.newvariantexcludefromsearch"
                    DisplayColon="true"  />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chkExcludeFromSearch" Checked= "true" />
            </div>
        </div>
    </div>
</div>