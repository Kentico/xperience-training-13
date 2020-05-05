<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Sites_Pages_Site_Edit_OfflineMode"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Site Edit - Offline mode"
     Codebehind="Site_Edit_OfflineMode.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblOfflineTitle" runat="server" CssClass="control-label" EnableViewState="false" ResourceString="sm.offline.offlinetitle" />
            </div>
            <div class="editing-form-value-cell">
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton ID="radMessage" runat="server" GroupName="Offline" EnableViewState="false"
                        ResourceString="sm.offline.displaymessage" />
                    <div class="selector-subitem">
                        <cms:CMSHtmlEditor runat="server" ID="txtMessage" ToolbarSet="SimpleEdit" Width="700px" Height="340px" />
                    </div>
                    <cms:CMSRadioButton ID="radURL" runat="server" GroupName="Offline" EnableViewState="false"
                        ResourceString="sm.offline.redirecttourl" />
                    <div class="selector-subitem">
                        <cms:CMSTextBox ID="txtURL" runat="server" MaxLength="400" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="plcControls">
    <cms:CMSButton runat="server" ID="btnOK" OnClick="btnOK_Click" ButtonStyle="Primary" />
    <cms:CMSButton ID="btnSubmit" runat="server" ButtonStyle="Primary" OnClick="btnSubmit_Click" />
</asp:Content>
