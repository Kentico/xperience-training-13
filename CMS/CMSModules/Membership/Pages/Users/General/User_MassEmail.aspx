<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Pages_Users_General_User_MassEmail" Title="Mass email"
    Theme="Default"  Codebehind="User_MassEmail.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="UserSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Roles/selectrole.ascx" TagName="RoleSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/FileUploader.ascx" TagName="FileUploader"
    TagPrefix="cms" %>

<asp:Content ID="cntSite" ContentPlaceHolderID="plcSiteSelector" runat="Server">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSite" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="general.site" DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="true" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFrom" runat="server" AssociatedControlID="emailSender" EnableViewState="false" ResourceString="general.fromemail" DisplayColon="true" ShowRequiredMark="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:EmailInput ID="emailSender" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" AssociatedControlID="txtSubject" EnableViewState="false" ResourceString="general.subject" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSubject" runat="server" MaxLength="450" />
            </div>
        </div>
    </div>
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="form-horizontal">
                <cms:LocalizedHeading Level="4" ID="lblRecipients" runat="server" EnableViewState="false"
                    ResourceString="userlist.recipients" />
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblUsers" runat="server" DisplayColon="True" EnableViewState="false" ResourceString="general.users" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:UserSelector ID="users" ShortID="su" runat="server" SelectionMode="Multiple" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblRoles" runat="server" DisplayColon="True" EnableViewState="false" ResourceString="general.roles" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:RoleSelector UserFriendlyMode="true" ID="roles" ShortID="sr" runat="server" IsLiveSite="false" />
                    </div>
                </div>
                <asp:Panel runat="server" ID="pnlGroups">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblGroups" runat="server" DisplayColon="True" EnableViewState="false" ResourceString="general.groups" />
                        </div>
                        <div class="editing-form-value-cell">
                            <asp:PlaceHolder runat="server" ID="plcGroupSelector" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <div class="form-horizontal">
                <cms:LocalizedHeading Level="4" runat="server" EnableViewState="false"
                    ResourceString="general.message" />
                <asp:PlaceHolder runat="server" ID="plcText">
                    <div class="form-group">
                        <div class="editing-form-label-cell label-full-width">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblText" runat="server" EnableViewState="false"
                                ResourceString="general.text" DisplayColon="True" AssociatedControlID="htmlText" />
                        </div>
                        <div class="editing-form-value-cell textarea-full-width">
                            <cms:CMSHtmlEditor ID="htmlText" runat="server" Height="400px" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcPlainText">
                    <div class="form-group">
                        <div class="editing-form-label-cell label-full-width">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblPlainText" runat="server" EnableViewState="false"
                                ResourceString="general.plaintext" DisplayColon="True" AssociatedControlID="txtPlainText" />
                        </div>
                        <div class="editing-form-value-cell textarea-full-width">
                            <cms:CMSTextArea ID="txtPlainText" runat="server" Rows="19" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" CssClass="control-label" DisplayColon="True" AssociatedControlID="uploader" ResourceString="general.attachments" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:FileUploader ID="uploader" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
