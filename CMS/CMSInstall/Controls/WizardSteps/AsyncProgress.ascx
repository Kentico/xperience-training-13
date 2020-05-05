<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="AsyncProgress.ascx.cs"
    Inherits="CMSInstall_Controls_WizardSteps_AsyncProgress" %>
<script type="text/javascript">
        //<![CDATA[
    var installTimerId = 0;
    var iMessageText = '';
    var iErrorText = '';
    var iWarningText = '';
    var getBusy = false;

    // Start timer function
    function StartTimer() {
        var act = document.getElementById('activity');
        if (act != null) {
            act.style.display = 'inline';
        }
        installTimerId = setInterval("CallServer('false')", 500);
    }

    // End timer function
    function StopTimer() {
        if (installTimerId) {
            clearInterval(installTimerId);
            installTimerId = 0;

            var act = document.getElementById('activity');
            if (act != null) {
                act.style.display = 'none';
            }
        }
    }
        //]]>
</script>
<asp:Panel ID="pnlProgress" runat="server">
    <div class="install-progress">
        <table class="install-wizard" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="left" style="vertical-align: top">
                    <div class="install-progress-database">
                        <div style="margin: 5px 0px 5px 5px;" class="FloatLeft">
                            <asp:Label ID="lblProgress" runat="server" />
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:HiddenField ID="hdnState" runat="server" />
<asp:Literal ID="ltlScript" runat="server" />