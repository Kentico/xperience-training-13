<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ImportGridTasks"
     Codebehind="ImportGridTasks.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcGrid" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <asp:Panel ID="pnlGrid" runat="server" CssClass="ObjectContent">
                <asp:PlaceHolder runat="server" ID="plcTasks">
                    <cms:LocalizedHeading ID="lblTasks" runat="Server" Level="4" ResourceString="Export.Tasks" EnableViewState="false" />
                    <asp:Panel ID="pnlTaskLinks" runat="server" CssClass="content-block-50">
                        <cms:LocalizedButton ID="btnAllTasks" runat="Server" EnableViewState="false" OnClick="btnAll_Click" ButtonStyle="Default" ResourceString="export.selectall" />
                        <cms:LocalizedButton ID="btnNoneTasks" runat="Server" EnableViewState="false" OnClick="btnNone_Click" ButtonStyle="Default" ResourceString="export.deselectall" />
                    </asp:Panel>
                        <cms:UIGridView ID="gvTasks" ShortID="gt" runat="server" AutoGenerateColumns="False">
                            <HeaderStyle CssClass="unigrid-head" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderStyle Width="50" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="TaskTitle" />
                                <asp:BoundField DataField="TaskType" />
                                <asp:BoundField DataField="TaskTime" />
                            </Columns>
                        </cms:UIGridView>
                        <cms:UIPager ID="pagerElem" ShortID="pg" runat="server" DefaultPageSize="10"
                            DisplayPager="true" VisiblePages="5" PagerMode="Postback"/>
                        <asp:HiddenField runat="server" ID="hdnAvailableTasks" Value="" EnableViewState="false" />
                </asp:PlaceHolder>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:PlaceHolder>
