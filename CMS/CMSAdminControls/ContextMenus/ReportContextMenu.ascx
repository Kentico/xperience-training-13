<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ReportContextMenu.ascx.cs"
    Inherits="CMSAdminControls_ContextMenus_ReportContextMenu" %>
<asp:Panel runat="server" ID="pnlReportContextMenu" CssClass="PortalContextMenu WebPartContextMenu"
    EnableViewState="false">
    <asp:PlaceHolder runat="server" ID="pnlExport">
        <asp:Panel runat="server" ID="pnlExcel" CssClass="Item">
            <asp:Panel runat="server" ID="pnlExcelPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblExcel" CssClass="Name" EnableViewState="false"
                    ResourceString="export.exporttoexcel" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlCSV" CssClass="Item">
            <asp:Panel runat="server" ID="pnlCSVPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblCSV" CssClass="Name" EnableViewState="false"
                    ResourceString="export.exporttocsv" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlXML" CssClass="Item">
            <asp:Panel runat="server" ID="pnlXMLPadding" CssClass="ItemPadding">
                <cms:LocalizedLabel runat="server" ID="lblXML" CssClass="Name" EnableViewState="false"
                    ResourceString="export.exporttoxml" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSeparator" CssClass="Separator" />
    </asp:PlaceHolder>
    <asp:Panel runat="server" ID="pnlSubscription" CssClass="item-last">
        <asp:Panel runat="server" ID="Panel2" CssClass="ItemPadding">
            <cms:LocalizedLabel runat="server" ID="lblSubscription" CssClass="Name" EnableViewState="false"
                ResourceString="newslettertemplate_list.subscription" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
