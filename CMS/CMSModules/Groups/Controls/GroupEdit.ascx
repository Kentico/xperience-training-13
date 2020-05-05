<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Controls_GroupEdit"
     Codebehind="GroupEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/selectdocument.ascx"
    TagName="SelectDocument" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Groups/FormControls/GroupPictureEdit.ascx" TagName="GroupPictureEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/SelectCssStylesheet.ascx" TagName="SelectCssStylesheet"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <asp:PlaceHolder runat="server" ID="plcAdvanceOptions">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayName" AssociatedControlID="txtDisplayName" runat="server"
                    ResourceString="general.displayname" DisplayColon="true"
                    EnableViewState="false" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <!-- Do not disable view state - simple mode -->
                <cms:LocalizableTextBox ID="txtDisplayName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName"
                    Display="Dynamic" ValidationGroup="GroupEdit" EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcCodeName">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCodeName" AssociatedControlID="txtCodeName" runat="server"
                        ResourceString="general.codename" DisplayColon="true" EnableViewState="false" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <!-- Do not disable view state - simple mode -->
                    <cms:CodeName ID="txtCodeName" runat="server" MaxLength="100" />
                    <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" ControlToValidate="txtCodeName"
                        Display="Dynamic" ValidationGroup="GroupEdit" EnableViewState="false" />
                </div>
            </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblDescription" AssociatedControlID="txtDescription" runat="server"
                ResourceString="general.description" DisplayColon="true"
                EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtDescription" runat="server" TextMode="MultiLine" EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcGroupLocation" runat="server" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblGroupPageURL" runat="server" ResourceString="group.group.grouppagelocation"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:SelectDocument ID="groupPageURLElem" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcStyleSheetSelector" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblStyleSheetName" runat="server" ResourceString="community.group.theme"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="ctrlSiteSelectStyleSheet" />
            </div>
            <div class="editing-form-value-cell">
                <cms:SelectCssStylesheet ID="ctrlSiteSelectStyleSheet" runat="server" ReturnColumnName="StyleSheetID" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblGroupAvatar" runat="server" ResourceString="group.group.avatar"
                DisplayColon="true" EnableViewState="false" AssociatedControlID="groupPictureEdit" />
        </div>
        <div class="editing-form-value-cell">
            <cms:GroupPictureEdit ID="groupPictureEdit" runat="server" MaxSideSize="100" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblApproveMembers" runat="server" ResourceString="group.group.approvemembers"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSRadioButton ID="radMembersAny" runat="server" GroupName="approvemembers"
                Checked="true" ResourceString="group.group.approveany" EnableViewState="false" />
            <cms:CMSRadioButton ID="radMembersApproved" runat="server" GroupName="approvemembers"
                ResourceString="group.group.approveapproved" EnableViewState="false" />
            <cms:CMSRadioButton ID="radMembersInvited" runat="server" GroupName="approvemembers"
                ResourceString="group.group.approveinvited" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblContentAccess" runat="server" ResourceString="group.group.contentaccess"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSRadioButton ID="radAnybody" runat="server" GroupName="contentaccess"
                Checked="true" ResourceString="group.group.accessanybody" EnableViewState="false" />
            <cms:CMSRadioButton ID="radSiteMembers" runat="server" GroupName="contentaccess"
                ResourceString="group.group.accesssitemembers" EnableViewState="false" />
            <cms:CMSRadioButton ID="radGroupMembers" runat="server" GroupName="contentaccess"
                ResourceString="group.group.accessgroupmembers" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblJoinLeave" AssociatedControlID="chkJoinLeave" runat="server"
                ResourceString="group.group.sendjoinleave" DisplayColon="true"
                EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkJoinLeave" runat="server" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblWaitingForApproval" AssociatedControlID="chkWaitingForApproval"
                runat="server" ResourceString="group.group.sendwaitingforapproval"
                DisplayColon="true" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkWaitingForApproval" runat="server" EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcCreatedBy" runat="server" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCreatedByTitle" runat="server" ResourceString="group.group.createdby"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizedLabel ID="lblCreatedByValue" runat="server" CssClass="form-control-text" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcApprovedBy" runat="server" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblApprovedByTitle" runat="server"
                    ResourceString="group.group.approvedby" DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizedLabel ID="lblApprovedByValue" runat="server" CssClass="form-control-text" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcOnline" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblLogActivity" runat="server" EnableViewState="false" ResourceString="group.group.logactivity"
                    DisplayColon="true" AssociatedControlID="chkLogActivity" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkLogActivity" runat="server" Checked="true" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnSave" runat="server" EnableViewState="false"
                OnClick="btnSave_Click" ValidationGroup="GroupEdit" />
        </div>
    </div>
</div>