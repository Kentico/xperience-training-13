<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_Boards_BoardEdit"
     Codebehind="BoardEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/ThreeStateCheckBox.ascx" TagName="ThreeStateCheckBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblBoardOwner" runat="server" EnableViewState="false"/>
        </div>
        <div class="editing-form-value-cell">
            <asp:Label ID="lblBoardOwnerText" runat="server" EnableViewState="true" CssClass="form-control-text" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblBoardDisplayName" runat="server" EnableViewState="false" AssociatedControlIDD="txtBoardDisplayName" ShowRequiredMark="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtBoardDisplayName" MaxLength="250" runat="server" EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvBoardDisplayName" ControlToValidate="txtBoardDisplayName"
                runat="server" Display="Dynamic" EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcCodeName" runat="Server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblBoardCodeName" runat="server" EnableViewState="false" AssociatedControl="txtBoardCodeName" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtBoardCodeName" MaxLength="250" runat="server" EnableViewState="true" Enabled="false" />
                <cms:CMSRequiredFieldValidator ID="rfvBoardCodeName" ControlToValidate="txtBoardCodeName"
                    runat="server" Display="Dynamic" EnableViewState="false" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblBoardDescription" runat="server" EnableViewState="false" AssociatedControlID="txtBoardDescription" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtBoardDescription" runat="server" TextMode="MultiLine" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblBoardEnable" runat="server" EnableViewState="false" AssociatedControlID="chkBoardEnable" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkBoardEnable" runat="server" CssClass="CheckBoxMovedLeft" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblBoardOpen" runat="server" Text="Label" EnableViewState="false" AssociatedControlID="chkBoardOpen" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkBoardOpen" runat="server" CssClass="CheckBoxMovedLeft" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblBoardOpenFrom" runat="server" EnableViewState="false" AssociatedControlID="dtpBoardOpenFrom" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker ID="dtpBoardOpenFrom" runat="server" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblBoardOpenTo" runat="server" EnableViewState="false" AssociatedControlID="dtpBoardOpenTo" />
        </div>
        <div class="editing-form-value-cell">
            <cms:DateTimePicker ID="dtpBoardOpenTo" runat="server" EnableViewState="false" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblSubscriptionsEnable" runat="server" DisplayColon="true" AssociatedControlID="chkSubscriptionsEnable"
                ResourceString="board.edit.enablesubscriptions" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkSubscriptionsEnable" runat="server" CssClass="CheckBoxMovedLeft"
                EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcUnsubscription">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblBaseUrl" runat="server" EnableViewState="false" AssociatedControlID="txtBaseUrl" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkInheritBaseUrl" Checked="true" ResourceString="boards.inheritbaseurl"
                        EnableViewState="false" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtBaseUrl" runat="server" EnableViewState="false" MaxLength="450" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" ID="lblUnsubscriptionUrl" runat="server" EnableViewState="false" AssociatedControlID="txtUnsubscriptionUrl" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox runat="server" ID="chkInheritUnsubUrl" Checked="true" ResourceString="boards.inheritbaseurl"
                        EnableViewState="false" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtUnsubscriptionUrl" runat="server" EnableViewState="false" MaxLength="450" />
                </div>

            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <asp:Label CssClass="control-label" ID="lblBoardRequireEmail" runat="server" EnableViewState="false" AssociatedControlID="chkBoardRequireEmail" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox ID="chkBoardRequireEmail" runat="server" CssClass="CheckBoxMovedLeft"
                EnableViewState="false" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcOnline" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblLogActivity" EnableViewState="false" ResourceString="board.edit.logactivity" AssociatedControlID="chkLogActivity"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkLogActivity" runat="server" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>
<div class="form-horizontal">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="general.OptIn" EnableViewState="false" DisplayColon="False" />
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblEnableOptIn" runat="server" EnableViewState="false" ResourceString="general.enableoptin"
                DisplayColon="true" AssociatedControlIDD="chkEnableOptIn" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ThreeStateCheckBox ID="chkEnableOptIn" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOptInURL" EnableViewState="false" ResourceString="general.optinurl"
                DisplayColon="true" AssociatedControlIDD="chkInheritOptInURL" />
        </div>
        <div class="editing-form-value-cell">
            <div class="control-group-inline">
                <cms:CMSCheckBox runat="server" ID="chkInheritOptInURL" ResourceString="boards.inheritbaseurl" />
            </div>
            <div class="control-group-inline">
                <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
                    <ContentTemplate>
                        <cms:SelectPath ID="txtOptInURL" runat="server" CssClass="wrap-nowrap" MaxLength="450" SinglePathMode="true" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblSendOptInConfirmation" runat="server" EnableViewState="false"
                ResourceString="general.sendoptinconfirmation" DisplayColon="true" AssociatedControlIDD="chkSendOptInConfirmation" />
        </div>
        <div class="editing-form-value-cell">
            <cms:ThreeStateCheckBox ID="chkSendOptInConfirmation" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnOk" runat="server" EnableViewState="false" OnClick="btnOk_Click"
                />
        </div>
    </div>
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />