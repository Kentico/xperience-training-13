<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Widgets_FormControls_SelectZoneType"  Codebehind="SelectZoneType.ascx.cs" %>
<script type="text/javascript">
    $cmsj(document).ready(function () {
        var radioBtns = $cmsj("#<%=rblOptions.ClientID %> [type=radio]");
        var warningPanel = $cmsj(".zone-type-change-warning");

        // Display the warning at the top of the dialog
        warningPanel.prependTo("#divContent");

        // Show when zone type is changed
        radioBtns
            .on("change", function () {
                warningPanel.show();
            });


        $cmsj(".alert-close")
            .click(function () {
                warningPanel.hide();
            });
    });
</script>

<cms:CMSRadioButtonList ID="rblOptions" runat="server" RepeatDirection="Vertical" />

<!--Warning panel-->
<div class="zone-type-change-warning alert-dismissable alert-warning alert">
    <span class="alert-icon">
        <i class="icon-exclamation-triangle"></i>
        <span class="sr-only"><%=ResHelper.GetString("general.warning")%></span>
    </span>
    <div class="alert-label">
        <%=ResHelper.GetString("widgets.zonetypechangewarning")%>
    </div>
    <span class="alert-close" >
        <i class="close icon-modal-close"></i>
        <span class="sr-only"><%=ResHelper.GetString("general.close")%></span>
    </span>
</div>
