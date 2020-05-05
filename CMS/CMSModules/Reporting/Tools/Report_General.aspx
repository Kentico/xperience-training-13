<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_Report_General"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
     Codebehind="Report_General.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="ItemsList.ascx" TagName="ItemsList" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileList.ascx" TagName="FileList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/SelectConnectionString.ascx" TagName="SelectString"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Namespace="CMS.Reporting.Web.UI" Assembly="CMS.Reporting.Web.UI" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <script type="text/javascript">
        //<![CDATA[
        // Insert desired HTML at the current cursor position of the HTML editor
        function InsertHTML(htmlString) {
            // Get the editor instance that we want to interact with.
            var oEditor = CKEDITOR.instances[reporting_htmlTemplateBody];
            // Check the active editing mode.
            if (oEditor.mode == 'wysiwyg') {
                // Insert the desired HTML.
                oEditor.insertHtml(htmlString);
            }
            else
                alert('You must be in WYSIWYG mode!');
            return false;
        }

        function PasteImage(imageurl) {
            imageurl = '<img src="' + imageurl + '" />';
            return InsertHTML(imageurl);
        }
        //]]>
    </script>    
    <cms:CMSPanel runat="server" ID="pnlMenu" CssClass="cms-edit-menu" FixedPosition="true">
        <cms:HeaderActions ID="actionsElem" ShortID="a" runat="server" IsLiveSite="false" />
    </cms:CMSPanel>
    <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblReportDisplayName" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="txtReportDisplayName" runat="server"
                        MaxLength="440" EnableViewState="false" />
                    <cms:CMSRequiredFieldValidator ID="rfvReportDisplayName" runat="server" ErrorMessage=""
                        ControlToValidate="txtReportDisplayName:cntrlContainer:textbox" Display="Dynamic" EnableViewState="false"></cms:CMSRequiredFieldValidator>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblReportName" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CodeName ID="txtReportName" runat="server" MaxLength="100"
                        EnableViewState="false" />
                    <cms:CMSRequiredFieldValidator ID="rfvReportName" runat="server" ErrorMessage=""
                        ControlToValidate="txtReportName" Display="Dynamic" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblReportCategory" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:SelectReportCategory ID="selectCategory" runat="server" IsLiveSite="false" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="pnlConnectionString">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblConnString" EnableViewState="false" ResourceString="ConnectionString.Title"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectString ID="ucSelectString" runat="server" DisplayInherit="true" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblReportAccess" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox CssClass="CheckBoxMovedLeft" ID="chkReportAccess" runat="server" Checked="True" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEnableSubscription" EnableViewState="false"
                        ResourceString="rep.enablesubscription" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox CssClass="CheckBoxMovedLeft" ID="chkEnableSubscription" runat="server"
                        Checked="True" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblLayout" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSHtmlEditor ID="htmlTemplateBody" runat="server" Width="90%" Height="435px"
            ToolbarSet="Reporting" />
                </div>
            </div>
        </div>
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblGraphs" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ItemsList ID="ilGraphs" runat="server" IsLiveSite="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblHtmlGraphs" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ItemsList ID="ilHtmlGraphs" runat="server" IsLiveSite="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblTables" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ItemsList ID="ilTables" runat="server" IsLiveSite="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" runat="server" ID="lblValues" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ItemsList ID="ilValues" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </div>
        <cms:PageTitle ID="AttachmentTitle" runat="server" TitleText="Attachments" TitleCssClass="SubTitleHeader" HideTitle="true" />
        <br />
        <cms:FileList ID="attachmentList" runat="server" />
    </asp:Panel>
    <asp:Button runat="server" ID="btnHdnReload" EnableViewState="false" CssClass="HiddenButton" />
</asp:Content>