<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="JoinPrerequisites.ascx.cs"
    Inherits="CMSInstall_Controls_WizardSteps_JoinPrerequisites" %>
<div class="install-content">
    <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
        <asp:PlaceHolder ID="plcTasks" runat="server">
            <tr>
                <td nowrap="nowrap" align="left" style="width: 150px;">
                    <cms:CMSUpdatePanel runat="server" ID="pnlUpdateText" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:LocalizedLabel ID="lblTaskStatus" runat="server" ResourceString="separationDB.taskstatus"
                                EnableViewState="False" DisplayColon="True" CssClass="control-label" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </td>
                <td style="height: 35px">
                    <div class="FloatLeft separation-value-cell">
                        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Timer ID="timRefresh" runat="server" Interval="3000" EnableViewState="false" />
                                <cms:LocalizedLabel runat="server" ID="lblStatusValue" EnableViewState="false" CssClass="FloatLeft" />
                                <asp:Literal runat="server" ID="ltlStatus" EnableViewState="false" />
                                <cms:LocalizedLinkButton ID="btnStopTasks" runat="server" ResourceString="separationDB.stoptasks"  CssClass="FloatLeft" />
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
        <asp:PlaceHolder ID="plcInfo" runat="server" EnableViewState="false" Visible="False">
            <tr>
                <td colspan="3" align="left">
                    <cms:LocalizedLabel ID="lblInfo" runat="server" ResourceString="separationDB.joinready" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcSeparationError" runat="server" EnableViewState="False" Visible="False">
            <tr>
                <td colspan="3" align="left">
                    <asp:Label ID="lblErrorTasks" runat="server" CssClass="error-label" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</div>
