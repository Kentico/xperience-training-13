<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_StartABTest"
    Theme="default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
     Codebehind="StartABTest.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" CssClass="control-label" ResourceString="abtesting.starttest" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton ID="radNow" runat="server" GroupName="Start" AutoPostBack="true"
                        OnCheckedChanged="radGroupStart_CheckedChanged" ResourceString="calendar.now"
                        Checked="true" />

                    <cms:CMSRadioButton ID="radLater" runat="server" GroupName="Start" AutoPostBack="true"
                        OnCheckedChanged="radGroupStart_CheckedChanged" ResourceString="calendar.later" />
                    <div class="selector-subitem">
                        <cms:DateTimePicker ID="calendarControl" runat="server" Enabled="false" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>