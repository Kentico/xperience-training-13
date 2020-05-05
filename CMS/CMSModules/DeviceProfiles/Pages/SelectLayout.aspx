<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true"  Codebehind="SelectLayout.aspx.cs" Inherits="CMSModules_DeviceProfiles_Pages_SelectLayout"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniFlatSelector.ascx" TagName="UniFlatSelector"
    TagPrefix="cms" %>
<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="server">
    <script type="text/javascript" language="javascript">
        //<![CDATA[

        function pageLoad() {
            if ($cmsj.isFunction(window.resizearea)) {
                resizearea();
            }

            setTimeout('Focus()', 100);
            var timer = null;
            SetupSearch();
        }

        function Client_ConfirmSelection() {
            var parameters = {
                sourceLayoutId: $cmsj('#SourceLayoutId').val(),
                targetLayoutId: $cmsj('#TargetLayoutId').val()
            };
            if (parameters.targetLayoutId == 0) {
                alert('<%= HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.selecttargetlayout"))%>');
        }
        else {
            wopener.Client_SetTargetLayout(parameters);
            CloseDialog();
        }

        return false;
    }

    function Client_SetTargetLayout(targetLayoutId) {
        var parameters = {
            sourceLayoutId: $cmsj('#SourceLayoutId').val(),
            targetLayoutId: targetLayoutId
        };
        wopener.Client_SetTargetLayout(parameters);
        return CloseDialog();
    }

    //]]>   
    </script>
    <div class="DeviceProfileLayoutSelector">
        <table class="SelectorTable" cellpadding="0" cellspacing="0">
            <tr>
                <td class="ItemSelectorArea">
                    <div class="ItemSelector DeviceProfileLayoutSelector">
                        <cms:CMSUpdatePanel runat="server">
                            <ContentTemplate>
                                <cms:UniFlatSelector ID="LayoutSelector" ShortID="f" runat="server" RememberSelectedItem="true">
                                    <HeaderTemplate>
                                        <div class="SelectorFlatItems">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="SelectorEnvelope" style="overflow: hidden">
                                            <div class="SelectorFlatImage">
                                                <%#PortalHelper.GetIconHtml(EvalGuid("LayoutThumbnailGUID"), ValidationHelper.GetString(Eval("LayoutIconClass"), PortalHelper.DefaultPageLayoutIconClass))%>
                                            </div>
                                            <span class="SelectorFlatText">
                                                <%#HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(Eval("LayoutDisplayName"))))%></span>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <div style="clear: both">
                                        </div>
                                        </div>
                                    </FooterTemplate>
                                </cms:UniFlatSelector>
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                        <div class="selector-flat-description  selector-flat-description-background">
                            <asp:Literal runat="server" ID="LayoutDescriptionLiteral" EnableViewState="false"></asp:Literal>
                        </div>
                    </div>
                    </td>
            </tr>
        </table>
    </div>
</asp:Content>
