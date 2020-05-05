<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_LinkedDocs"
    Theme="Default"  Codebehind="LinkedDocs.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading ID="headElem" runat="server" CssClass="listing-title" Level="4" ResourceString="LinkedDocs.LinkedDocs" EnableViewState="False" />
    <asp:Panel ID="pnlContainer" runat="server">
        <cms:UniGrid ID="gridDocs" runat="server" GridName="LinkedDocs.xml" IsLiveSite="false"
            ExportFileName="cms_document" />
    </asp:Panel>
    <cms:LocalizedLabel ID="lblNoData" runat="server" ResourceString="LinkedDocs.NoLinkedDocs" Visible="false" />
    <script type="text/javascript">
        //<![CDATA[
        // Select item action
        function SelectItem(nodeId, parentNodeId) {
            if (nodeId != 0) {
                parent.SelectNode(nodeId);
                parent.RefreshTree(parentNodeId, nodeId);
            }
        }
        //]]>
    </script>
</asp:Content>
