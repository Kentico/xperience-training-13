<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserServer.ascx.cs" Inherits="CMSInstall_Controls_WizardSteps_UserServer" %>
<div class="install-content">
    <div class="install-wizard">
        <table class="install-wizard">
            <tr>
                <td colspan="2">
                    <cms:LocalizedHeading Level="4" ID="lblSQLServer" runat="server" />
                </td>
            </tr>
            <tr>
                <td nowrap="nowrap" align="right" style="padding-right: 8px">
                    <cms:LocalizedLabel ID="lblServerName" AssociatedControlID="txtServerName" runat="server" CssClass="control-label" ResourceString="Install.lblServername" />
                </td>
                <td width="100%">
                    <cms:CMSTextBox ID="txtServerName" runat="server" />
                </td>
            </tr>
            <asp:PlaceHolder ID="plcRadSQL" runat="server">
                <tr>
                    <td colspan="2" align="left">
                        <cms:LocalizedRadioButton ID="radSQLAuthentication" runat="server" AutoPostBack="True" GroupName="AuthenticationType" ResourceString="Install.radSQlAuthentication"
                            Checked="True" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr class="install-sql-name">
                <td nowrap="nowrap" align="right" style="padding-right: 8px">
                    <cms:LocalizedLabel ID="lblDBUsername" AssociatedControlID="txtDBUsername" runat="server" CssClass="control-label" ResourceString="Install.lblUsername" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtDBUsername" CssClass="InstallFormTextBox" runat="server" />
                </td>
            </tr>
            <tr class="install-sql-password">
                <td nowrap="nowrap" align="right" style="padding-right: 8px">
                    <cms:LocalizedLabel ID="lblDBPassword" AssociatedControlID="txtDBPassword" runat="server" CssClass="control-label" ResourceString="Install.lblPassword" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtDBPassword" CssClass="InstallFormTextBox" runat="server" TextMode="Password" />
                </td>
            </tr>
            <asp:PlaceHolder ID="plcWinAuth" runat="server">
                <tr>
                    <td colspan="2" align="left">
                        <cms:CMSRadioButton ID="radWindowsAuthentication" runat="server" AutoPostBack="True"
                            GroupName="AuthenticationType" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td colspan="2">&nbsp;
                </td>
            </tr>
            <asp:PlaceHolder ID="plcSeparationError" runat="server" EnableViewState="False" Visible="False">
                <tr>
                    <td colspan="2" align="left">
                        <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </div>
</div>
