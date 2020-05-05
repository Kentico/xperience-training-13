<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Languages"
    Title="Languages" ValidateRequest="false" Theme="Default"  Codebehind="Languages.aspx.cs"
    MaintainScrollPositionOnPostback="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Import Namespace="CMS.Modules" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        // Redirect item
        function RedirectItem(nodeId, culture) {
            if (nodeId > 0) {
                if (parent != null) {
                    if (parent.parent != null) {
                        if (parent.parent.parent != null) {
                            if (parent.parent.parent.parent != null) {
                                parent.parent.parent.parent.location.href = "../../../../Admin/CMSAdministration.aspx?action=edit&mode=editform&nodeid=" + nodeId + "&culture=" + culture + "<%= ApplicationUrlHelper.GetApplicationHash("cms.content", "content") %>";
                            }
                        }
                    }
                }
            }
        }
        //]]>
    </script>
    <asp:Panel ID="pnlContainer" runat="server">
        <cms:UniGrid ID="gridLanguages" runat="server" GridName="Languages.xml" GridLines="Horizontal"
            IsLiveSite="false" ExportFileName="cms_document" />
    </asp:Panel>
</asp:Content>
