<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Debug_CacheItemsGrid"
     Codebehind="CacheItemsGrid.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>
<asp:HiddenField runat="server" ID="hdnKey" EnableViewState="false" />
<asp:Panel runat="server" ID="pnlSearch" DefaultButton="btnSearch">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-value-cell-wide form-search-container">
                <cms:LocalizedLabel runat="server" CssClass="sr-only" ID="lblFilter" ResourceString="General.Search" EnableViewState="false" AssociatedControlID="txtFilter" />
                <cms:CMSTextBox runat="server" ID="txtFilter" CssClass="form-control" />
                <cms:CMSIcon runat="server" ID="icSearch" CssClass="icon-magnifier" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton runat="server" ID="btnSearch" ResourceString="General.Search"
                    ButtonStyle="Primary" EnableViewState="false" OnClick="btnSearch_Click" />
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="plcItems">
    <table class="table table-hover">
        <thead>
            <tr>
                <th>
                    <asp:Label runat="server" ID="lblAction" EnableViewState="false" />
                </th>
                <th style="width: 100%;">
                    <asp:Label runat="server" ID="lblKey" EnableViewState="false" />
                </th>
                <asp:PlaceHolder runat="server" ID="plcData">
                    <th>
                        <asp:Label runat="server" ID="lblData" EnableViewState="false" />
                    </th>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcContainer" Visible="false">
                    <th>
                        <asp:Label runat="server" ID="lblExpiration" EnableViewState="false" />
                    </th>
                    <th>
                        <asp:Label runat="server" ID="lblPriority" EnableViewState="false" />
                    </th>
                </asp:PlaceHolder>
            </tr>
        </thead>
        <tbody>
            <asp:Literal ID="ltlCacheInfo" runat="server" EnableViewState="false" />
        </tbody>
    </table>
    <cms:UIPager ID="pagerItems" ShortID="p" ShowDirectPageControl="true" runat="server" PagerMode="Postback" />
</asp:Panel>
<cms:LocalizedLabel CssClass="InfoLabel" runat="server" ID="lblInfo" ResourceString="General.NoDataFound"
    Visible="false" EnableViewState="false" />
