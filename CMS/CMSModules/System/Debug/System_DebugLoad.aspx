<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_System_DebugLoad"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group list"
    MaintainScrollPositionOnPostback="true"  Codebehind="System_DebugLoad.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel runat="server" ID="pnlInfo">
        <ContentTemplate>
            <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
            <asp:Timer runat="server" ID="timRefresh" Interval="1000" Enabled="true" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:CMSUpdatePanel runat="server" ID="pnlBody" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblThreads" EnableViewState="false" ResourceString="DebugLoad.Threads" AssociatedControlID="txtThreads" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtThreads" Text="10" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblIterations" EnableViewState="false" ResourceString="DebugLoad.Iterations"
                            DisplayColon="true" AssociatedControlID="txtIterations" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtIterations" Text="1000" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDuration" EnableViewState="false" ResourceString="DebugLoad.Duration" AssociatedControlID="txtDuration" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtDuration" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblInterval" EnableViewState="false" ResourceString="DebugLoad.Interval" AssociatedControlID="txtInterval" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtInterval" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUserName" EnableViewState="false" ResourceString="DebugLoad.UserName"
                            DisplayColon="true" AssociatedControlID="userElem" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectUserName runat="server" ID="userElem" IsLiveSite="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSplitUrls" EnableViewState="false" ResourceString="DebugLoad.SplitUrls"
                            DisplayColon="true" AssociatedControlID="chkSplitUrls" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox runat="server" ID="chkSplitUrls" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUserAgent" EnableViewState="false" ResourceString="DebugLoad.UserAgent"
                            DisplayColon="true" AssociatedControlID="txtUserAgent" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextArea runat="server"  Rows="4" ID="txtUserAgent"
                            Text="Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; .NET4.0C; .NET4.0E; MS-RTC LM 8)" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblURLs" EnableViewState="false" ResourceString="DebugLoad.URLs" AssociatedControlID="txtURLs" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextArea runat="server" Rows="4" ID="txtURLs" Text="~/Home.aspx" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-value-cell editing-form-value-cell-offset">
                        <cms:CMSButton runat="server" ID="btnStart" ButtonStyle="Primary" OnClick="btnStart_Click" /><cms:CMSButton
                            runat="server" ID="btnStop" ButtonStyle="Primary" OnClick="btnStop_Click" /><cms:CMSButton
                                runat="server" ID="btnReset" ButtonStyle="Primary" OnClick="btnReset_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>