<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SeparationFinished.ascx.cs"
    Inherits="CMSInstall_Controls_WizardSteps_SeparationFinished" %>
<div class="install-content">
    <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
        <asp:PlaceHolder ID="plcContent" runat="server">
            <tr>
                <td class="separation-finished" colspan="3">
                    <cms:LocalizedLabel ID="lblCompleted" runat="server" ResourceString="separationDB.OK" />
                </td>
            </tr>
            <asp:PlaceHolder ID="plcDeleteOldDB" runat="server">
                <tr>
                    <td style="width: 180px">
                        <cms:LocalizedLabel ID="lblDeleteOldDB" runat="server" EnableViewState="False" AssociatedControlID="chkDeleteOldDB"
                            CssClass="control-label" DisplayColon="True" ResourceString="separationDB.deleteOldDB" />
                    </td>
                    <td>
                        <cms:CMSCheckBox ID="chkDeleteOldDB" runat="server" Checked="True" CssClass="separation-checkbox" />
                        <span class="info-icon">
                            <cms:LocalizedLabel runat="server" ID="spanScreenReader" CssClass="sr-only"></cms:LocalizedLabel>
                            <cms:CMSIcon runat="server" ID="iconHelp" EnableViewState="false" CssClass="icon-question-circle" aria-hidden="true"></cms:CMSIcon>
                        </span>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcChangeCollation" runat="server" Visible="false">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblChangeCollation" runat="server" EnableViewState="False"
                            CssClass="control-label" />
                    </td>
                    <td>
                        <cms:LocalizedLinkButton ID="btnChangeCollation" runat="server" OnClick="btnChangeCollation_Click" />
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td colspan="3">&nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <cms:LocalizedLabel ID="lblError" runat="server" CssClass="SelectorError" Visible="true" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcAzureError" runat="server" Visible="False">
            <tr>
                <td colspan="3">
                    <div class="separation-azure-error">
                        <cms:LocalizedLabel ID="lblAzureError" runat="server" CssClass="SelectorError" Visible="False" />
                    </div>
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</div>
