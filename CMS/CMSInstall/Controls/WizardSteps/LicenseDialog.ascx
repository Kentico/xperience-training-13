<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSInstall_Controls_WizardSteps_LicenseDialog"  Codebehind="LicenseDialog.ascx.cs" %>
<table class="install-wizard-new-site" border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td style="padding-bottom: 10px">
            <asp:Label ID="lblLicenseCaption" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblLicenseTip" runat="server" />
        </td>
    </tr>
    <tr>
        <td style="padding-top: 6px; padding-bottom: 6px">
            <asp:Label ID="lblFreeLicenseInfo" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblEnterLicenseKey" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:CMSTextArea ID="txtLicense" runat="server" Rows="10" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:LinkButton ID="lnkSkipLicense" runat="server" />
        </td>
    </tr>
</table>
