<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSInstall_Controls_WizardSteps_CollationDialog" CodeBehind="CollationDialog.ascx.cs" %>
<div class="install-content">
    <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td align="left">
                <div>
                    <asp:Label ID="lblCollation" runat="server" /><br />
                    <br />
                    <br />
                    <cms:CMSRadioButton ID="rbChangeCollationCI" Checked="true" runat="server" GroupName="Collation" />
                    <br />
                    <br />
                    <cms:CMSRadioButton ID="rbLeaveCollation" runat="server" GroupName="Collation" />
                </div>
            </td>
        </tr>
    </table>
</div>
