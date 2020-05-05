<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ExportPanel"
     Codebehind="ExportPanel.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/ObjectTree.ascx" TagName="ObjectTree"
    TagPrefix="cms" %>
<%@ Register Src="ExportGridView.ascx" TagName="ExportGridView" TagPrefix="cms" %>
<%@ Register Src="ExportGridTasks.ascx" TagName="ExportGridTasks" TagPrefix="cms" %>

<script type="text/javascript">
    //<![CDATA[
    function setDivBorder() {
        if (rtl == 'True') {
            document.getElementById("exTreeDiv").style.borderLeft = 'solid 8px #e5e5e5';
        }
        else {
            document.getElementById("exTreeDiv").style.borderRight = 'solid 8px #e5e5e5';
        }
    }

    function exSetDivPosition() {
        var intY = document.getElementById("exTreeDiv").scrollTop;
        document.getElementById("hdnexDivScrollBar").value = intY;
    }

    function exGetDivPosition() {
        var intY = document.getElementById('hdnexDivScrollBar').value;
        document.getElementById("exTreeDiv").scrollTop = intY;
    }

    // Hook onload handler
    if (window.onload != null) {
        var oldOnload = window.onload;
        window.onload = function(e) { oldOnload(e); exGetDivPosition(); setDivBorder(); };
    }
    else {
        window.onload = function(e) { exGetDivPosition(); setDivBorder(); };
    }
    //]]>
</script>

<asp:Panel ID="pnlGrid" runat="Server">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td style="width: 250px; vertical-align: top;">
                <div id="exTreeDiv" style="width: 250px; height: 384px; padding: 0 16px 16px; overflow: auto;"
                    onclick="exSetDivPosition();" onunload="exSetDivPosition();">
                    <cms:ObjectTree ID="objectTree" ShortID="t" runat="server" UsePostback="true" UseImages="true"
                        EnableViewState="true" />
                </div>
                <input type="hidden" id="hdnexDivScrollBar" name="hdnexDivScrollBar" value="<%=HTMLHelper.EncodeForHtmlAttribute(mScrollPosition)%>" />
            </td>
            <td style="vertical-align: top;">
                <div style="height: 400px; overflow: auto;">
                    <div>
                        <cms:MessagesPlaceHolder runat="server" ID="pnlMessages" ContainerCssClass="wizard-section" />
                        <asp:PlaceHolder ID="plcPanel" runat="server">
                            <asp:PlaceHolder ID="plcControl" runat="Server" />
                                <cms:ExportGridView ID="gvObjects" ShortID="g" runat="server" />
                                <cms:ExportGridTasks ID="gvTasks" ShortID="gt" runat="server" />
                        </asp:PlaceHolder>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
