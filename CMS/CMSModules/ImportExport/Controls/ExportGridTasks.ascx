<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ExportGridTasks"
     Codebehind="ExportGridTasks.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcGrid" runat="server">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <cms:CMSPanel ID="pnlGrid" ShortID="p" runat="server">
                <asp:PlaceHolder runat="server" ID="plcTasks">
                    <div class="wizard-section">
                        <cms:LocalizedHeading ID="lblTasks" runat="Server" Level="4" ResourceString="Export.Tasks" EnableViewState="false" />
                        <cms:LocalizedButton ID="btnAllTasks" runat="Server" EnableViewState="false" OnClick="btnAll_Click" ButtonStyle="Default" ResourceString="export.selectall" />
                        <cms:LocalizedButton ID="btnNoneTasks" runat="Server" EnableViewState="false" OnClick="btnNone_Click" ButtonStyle="Default" ResourceString="export.deselectall" />
                    </div>
                    <div class="wizard-section content-block-50">
                        <cms:UIGridView ID="gvTasks" ShortID="gt" runat="server" AutoGenerateColumns="False">
                            <HeaderStyle CssClass="unigrid-head" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderStyle Width="50" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderStyle CssClass="main-column-100" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblName" runat="server" EnableViewState="false" ToolTip='<%#HttpUtility.HtmlEncode(ValidationHelper.GetString(Eval("TaskTitle"), ""))%>'
                                            Text='<%#HttpUtility.HtmlEncode(TextHelper.LimitLength(ResHelper.LocalizeString(ValidationHelper.GetString(Eval("TaskTitle"), "")), 60))%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="TaskType" />
                                <asp:BoundField DataField="TaskTime" />
                            </Columns>
                        </cms:UIGridView>
                        <cms:UIPager ID="pagerElem" ShortID="pg" runat="server" DefaultPageSize="10"
                            DisplayPager="true" VisiblePages="5" PagerMode="Postback"/>
                        <asp:HiddenField runat="server" ID="hdnAvailableTasks" Value="" EnableViewState="false" />
                    </div>
                </asp:PlaceHolder>
            </cms:CMSPanel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:PlaceHolder>
