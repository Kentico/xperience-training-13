<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_FormControls_FacebookPageAccessToken"  Codebehind="FacebookPageAccessToken.ascx.cs" %>

<script type="text/javascript">
    function fbShowMessage(labelId, msg, isError) {
        var label = $cmsj('#' + labelId);
        label.html(msg);
        if (isError) {
            label.addClass('ErrorLabel');
        } else {
            label.removeClass('ErrorLabel');
        }
    }

    function fbOpenModalDialog() {
        modalDialog('<%=URLHelper.GetAbsoluteUrl("~/CMSModules/SocialMarketing/Pages/FacebookPageAccessTokenDialog.aspx") %>', 'FacebookAccessToken', 1030, 560, null, null, null, true);
    }

    function fbSetAccessToken(parameters) {
        var tokenControl = $cmsj('#' + parameters.tokenControlId),
            expirationControl = $cmsj('#' + parameters.expirationControlId),
            pageIdControl = $cmsj('#' + parameters.pageIdControlId),
            appIdControl = $cmsj('#' + parameters.appIdControlId);

        tokenControl.val(parameters.accessToken);
        expirationControl.val(parameters.tokenExpiration);
        pageIdControl.val(parameters.tokenPageId);
        appIdControl.val(parameters.tokenAppId);

        var resStringExpire = '<%= GetString("sm.facebook.account.msg.tokenwillexpire") %>',
            text = '<%= GetString("sm.facebook.account.msg.tokenneverexpire") %>';
        if (parameters.tokenExpirationString != '') {
            text = resStringExpire.replace('{0}', parameters.tokenExpirationString);
        }
        fbShowMessage(parameters.infoLabelId, text, false);
    }
</script>
<asp:Panel ID="pnlToken" runat="server">
    <asp:HiddenField runat="server" ID="hdnToken" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnTokenExpiration" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnTokenPageId" EnableViewState="False" />
    <asp:HiddenField runat="server" ID="hdnTokenAppId" EnableViewState="False" />
    <cms:LocalizedButton ID="btnGetToken" runat="server" ButtonStyle="Default" ResourceString="sm.facebook.account.getaccesstoken" OnClick="btnGetToken_OnClick" />
    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="form-control-text" />
</asp:Panel>
