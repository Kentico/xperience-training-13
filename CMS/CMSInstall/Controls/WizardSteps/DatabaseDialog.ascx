<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DatabaseDialog.ascx.cs"
    Inherits="CMSInstall_Controls_WizardSteps_DatabaseDialog" %>
<div class="install-content">
    <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
        <asp:PlaceHolder ID="plcRadNew" runat="server">
            <tr>
                <td colspan="3">
                    <cms:LocalizedHeading Level="4" ID="lblDatabase" AssociatedControlID="radCreateNew" runat="server" EnableViewState="False" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plcNewDB">
            <tr>
                <td colspan="3">
                    <cms:CMSRadioButton ID="radCreateNew" runat="server" AutoPostBack="True" GroupName="DatabaseType"
                        Checked="True"></cms:CMSRadioButton>
                </td>
            </tr>
            <tr>
                <td style="width: 25px;">&nbsp;
                </td>
                <td nowrap="nowrap" align="left" style="width: 140px; padding-right: 8px;">
                    <asp:Label ID="lblNewDatabaseName" AssociatedControlID="txtNewDatabaseName" runat="server" CssClass="control-label"
                        EnableViewState="False" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtNewDatabaseName" runat="server" Enabled="False" MaxLength="90" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcExistingRadio" runat="server">
            <tr>
                <td colspan="3">
                    <cms:CMSRadioButton ID="radUseExisting" runat="server" AutoPostBack="True" GroupName="DatabaseType" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td style="width: 25px;">&nbsp;
            </td>
            <td nowrap="nowrap" align="left" style="width: 140px; padding-right: 8px;">
                <asp:Label ID="lblExistingDatabaseName" AssociatedControlID="txtExistingDatabaseName" CssClass="control-label"
                    runat="server" EnableViewState="False" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtExistingDatabaseName" runat="server" MaxLength="90" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcCreateDatabaseObjects" runat="server">
            <tr>
                <td></td>
                <td colspan="2">
                    <cms:CMSCheckBox ID="chkCreateDatabaseObjects" runat="server" CssClass="install-create-db-objects" Checked="True" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcRunningTasks" runat="server" Visible="False">
            <asp:PlaceHolder ID="plcEmptyLine" runat="server">
                <tr>
                    <td colspan="3">&nbsp;
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td style="width: 25px;">&nbsp;
                </td>
                <td nowrap="nowrap" align="left" style="width: 140px;">
                    <cms:CMSUpdatePanel runat="server" ID="pnlUpdateText" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:LocalizedLabel ID="lblTaskStatus" runat="server" ResourceString="separationDB.taskstatus"
                                EnableViewState="False" DisplayColon="True" CssClass="control-label" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </td>
                <td>
                    <div class="FloatLeft separation-value-cell">
                        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Timer ID="timRefresh" runat="server" Interval="3000" EnableViewState="false" />
                                <cms:LocalizedLabel runat="server" ID="lblStatusValue" EnableViewState="false" CssClass="FloatLeft" />
                                <asp:Literal runat="server" ID="ltlStatus" EnableViewState="false" />
                                <cms:LocalizedLinkButton ID="btnStopTasks" runat="server" ResourceString="separationDB.stoptasks" CssClass="FloatLeft" />
                                <asp:HiddenField runat="server" ID="hdnTurnedOff" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ID="spanScreenReader" CssClass="sr-only"></cms:LocalizedLabel>
                        <cms:CMSIcon runat="server" ID="iconHelp" EnableViewState="false" CssClass="icon-question-circle" aria-hidden="true"></cms:CMSIcon>
                    </span>
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcSeparationError" runat="server" EnableViewState="False" Visible="False">
            <tr>
                <td colspan="3">&nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3" align="left">
                    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</div>
