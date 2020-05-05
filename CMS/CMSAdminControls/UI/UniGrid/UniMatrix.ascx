<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UniGrid_UniMatrix"
     Codebehind="UniMatrix.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
<asp:Table runat="server" ID="tblMatrix" CssClass="table table-hover">
    <asp:TableHeaderRow runat="server" CssClass="unigrid-head" TableSection="TableHeader" ID="thrFirstRow" EnableViewState="false">
        <asp:TableHeaderCell runat="server" ID="thcFirstColumn" Scope="Column">
            <asp:PlaceHolder runat="server" ID="plcFilter">
                <div class="unimatrix-filter">
                    <asp:Panel runat="server" ID="pnlFilter" DefaultButton="btnFilter" Visible="false" CssClass="control-group-inline">
                        <cms:CMSTextBox runat="server" ID="txtFilter" CssClass="input-width-60" /><cms:LocalizedButton
                            runat="server" ID="btnFilter" OnClick="btnFilter_Click" ButtonStyle="Default" />
                    </asp:Panel>
                </div>
            </asp:PlaceHolder>
        </asp:TableHeaderCell>
    </asp:TableHeaderRow>
    <asp:TableRow runat="server" ID="trContentBefore" ClientIDMode="Static" />
</asp:Table>
<asp:PlaceHolder runat="server" ID="plcPager">
    <cms:UIPager ID="pagerElem" ShortID="pg" runat="server" DefaultPageSize="20" PagerMode="Postback" />
</asp:PlaceHolder>
<asp:Label ID="lblInfoAfter" runat="server" CssClass="PageFooter" Visible="false"
    EnableViewState="false" />