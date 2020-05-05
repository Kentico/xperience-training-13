<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ImportExport_Controls_Global_ObjectAttachmentSelector"  Codebehind="ObjectAttachmentSelector.ascx.cs" %>
<asp:HiddenField ID="hdnLastSelected" runat="server" />
<asp:Literal ID="ltlScript" EnableViewState="false" runat="server" />

<script type="text/javascript">

    function SelectItem(id) {
        if (id != '') {
            if (hdnLastSelected.value != '') {
                var lastElem = document.getElementById(hdnLastSelected.value);
                if (lastElem != null) {
                    lastElem.className = 'GlobalItem';
                }
            }

            var elem = document.getElementById(id);
            if (elem != null) {
                elem.className = 'GlobalSelectedItem';
            }
            hdnLastSelected.value = id;
        }
    }
</script>

<asp:Panel ID="pnlTemplate" runat="Server">
    <div style="height: <%=mPanelHeight%>px;" class="TemplatePanel">
        <asp:Repeater ID="rptItems" runat="server">
            <ItemTemplate>
                <div class="GlobalItem" id="<%#Eval(IDColumn)%>" onclick="SelectItem('<%#Eval(IDColumn)%>')">
                    <table cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td style="vertical-align: middle;">
                                <%#PortalHelper.GetIconHtml(EvalGuid(ThumbnailGUIDColumn), DataHelper.GetNotEmpty(Eval(IconClassColumn), DefaulIconClass), mPreviewWidth)%>
                            </td>
                            <td style="vertical-align: top;">
                                <div>
                                    <cms:LocalizedHeading runat="server" Level="3" Text="<%#HTMLHelper.HTMLEncode(Eval(DisplayNameColumn).ToString())%>" EnableViewState="false" />
                                    <div>
                                        <%# ResHelper.LocalizeString(HTMLHelper.HTMLEncode(Eval(DescriptionColumn).ToString()))%>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>
<asp:Literal ID="ltlScriptAfter" EnableViewState="false" runat="server" />